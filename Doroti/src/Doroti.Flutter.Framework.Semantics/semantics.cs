// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/semantics/semantics.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Semantics;

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
        FlutterError? error__5693 = ((Func<SemanticsNode, FlutterError?>)(((SemanticsNode)node).role switch { SemanticsRole.alertDialog => _noCheckRequired, SemanticsRole.dialog => _noCheckRequired, SemanticsRole.none => _noCheckRequired, SemanticsRole.tab => _semanticsTab, SemanticsRole.tabBar => _semanticsTabBar, SemanticsRole.tabPanel => _noCheckRequired, SemanticsRole.table => _semanticsTable, SemanticsRole.cell => _semanticsCell, SemanticsRole.row => _semanticsRow, SemanticsRole.columnHeader => _semanticsColumnHeader, SemanticsRole.radioGroup => _semanticsRadioGroup, SemanticsRole.menu => _semanticsMenu, SemanticsRole.menuBar => _semanticsMenuBar, SemanticsRole.menuItem => _semanticsMenuItem, SemanticsRole.menuItemCheckbox => _semanticsMenuItemCheckbox, SemanticsRole.menuItemRadio => _semanticsMenuItemRadio, SemanticsRole.alert => _noLiveRegion, SemanticsRole.status => _noLiveRegion, SemanticsRole.list => _noCheckRequired, SemanticsRole.listItem => _semanticsListItem, SemanticsRole.complementary => _semanticsComplementary, SemanticsRole.contentInfo => _semanticsContentInfo, SemanticsRole.main => _semanticsMain, SemanticsRole.navigation => _semanticsNavigation, SemanticsRole.region => _semanticsRegion, SemanticsRole.form => _noCheckRequired, SemanticsRole.loadingSpinner => _noCheckRequired, SemanticsRole.progressBar => _semanticsProgressBar, SemanticsRole.dragHandle => _unimplemented, SemanticsRole.spinButton => _unimplemented, SemanticsRole.comboBox => _unimplemented, SemanticsRole.tooltip => _unimplemented, SemanticsRole.hotKey => _unimplemented, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }))(node);
        if ((error__5693 is not null))
        {
            return error__5693;
        }
        return _semanticsGeneral(node);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _unimplemented(SemanticsNode node) => new FlutterError($"Missing checks for role {node.getSemanticsData().role}");
    internal static FlutterError? _noCheckRequired(SemanticsNode node) => null;
    internal static FlutterError? _semanticsProgressBar(SemanticsNode node)
    {
        SemanticsData data__7931 = node.getSemanticsData();
        if ((((((SemanticsData)data__7931).value.Length == 0) || (((((SemanticsData)data__7931).minValue is null ? (bool?)null : ((SemanticsData)data__7931).minValue.Length == 0) ?? true))) || (((((SemanticsData)data__7931).maxValue is null ? (bool?)null : ((SemanticsData)data__7931).maxValue.Length == 0) ?? true))))
        {
            return new FlutterError("A progress bar must have a value, a minValue, a maxValue.");
        }
        double? minVal__8228 = Dart_coreLibrary.tryParse(((SemanticsData)data__7931).minValue!);
        double? maxVal__8288 = Dart_coreLibrary.tryParse(((SemanticsData)data__7931).maxValue!);
        double? currentValue__8479 = Dart_coreLibrary.tryParse(((SemanticsData)data__7931).value);
        double? percentValue__8541 = (((SemanticsData)data__7931).value.endsWith("%") ? Dart_coreLibrary.tryParse(((SemanticsData)data__7931).value.substring(0L, (((SemanticsData)data__7931).value.Length - 1L))) : null);
        if ((((minVal__8228 is null) || (maxVal__8288 is null)) || (((currentValue__8479 is null) && (percentValue__8541 is null)))))
        {
            return new FlutterError("Progress bar value, minValue, and maxValue must be valid numbers. " + $"value: \"{((SemanticsData)data__7931).value}\", minValue: \"{((SemanticsData)data__7931).minValue}\", maxValue: \"{((SemanticsData)data__7931).maxValue}\"");
        }
        if ((minVal__8228 >= DartRuntimePrimitives.RequireValue(maxVal__8288)))
        {
            return new FlutterError($"Progress bar minValue ({((SemanticsData)data__7931).minValue}) must be less than maxValue ({((SemanticsData)data__7931).maxValue})");
        }
        if ((currentValue__8479 is not null))
        {
            double currentValue__8479__value9301 = DartRuntimePrimitives.RequireValue(currentValue__8479);
            if (((DartRuntimePrimitives.RequireValue(currentValue__8479__value9301) < DartRuntimePrimitives.RequireValue(minVal__8228)) || (DartRuntimePrimitives.RequireValue(currentValue__8479__value9301) > DartRuntimePrimitives.RequireValue(maxVal__8288))))
            {
                return new FlutterError($"Progress bar value ({((SemanticsData)data__7931).value}) must be between minValue ({((SemanticsData)data__7931).minValue}) and maxValue ({((SemanticsData)data__7931).maxValue})");
            }
            return null;
        }
        if (((percentValue__8541 is not null) && (((DartRuntimePrimitives.RequireValue(percentValue__8541) < 0L) || (DartRuntimePrimitives.RequireValue(percentValue__8541) > 100L)))))
        {
            double percentValue__8541__value9681 = DartRuntimePrimitives.RequireValue(percentValue__8541);
            return new FlutterError($"Progress bar percentage value ({((SemanticsData)data__7931).value}) must be between 0% and 100%");
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsTab(SemanticsNode node)
    {
        SemanticsData data__9982 = node.getSemanticsData();
        if ((object.Equals(((SemanticsData)data__9982).flagsCollection.isSelected, Tristate.none)))
        {
            return new FlutterError("A tab needs selected states");
        }
        if (((SemanticsNode)node).areUserActionsBlocked)
        {
            return null;
        }
        if (((!object.Equals(((SemanticsData)data__9982).flagsCollection.isEnabled, Tristate.isFalse)) && !data__9982.hasAction(SemanticsAction.tap)))
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
        FlutterError? error__10576 = default!;
        node.visitChildren(((Func<SemanticsNode, bool>)((child) =>
        {
            if ((!object.Equals(child.getSemanticsData().role, SemanticsRole.tab)))
            {
                error__10576 = new FlutterError("Children of TabBar must have the tab role");
            }
            return (error__10576 is null);
            return default;
        })));
        return error__10576;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsTable(SemanticsNode node)
    {
        FlutterError? error__10915 = default!;
        node.visitChildren(((Func<SemanticsNode, bool>)((child) =>
        {
            if ((!object.Equals(child.getSemanticsData().role, SemanticsRole.row)))
            {
                error__10915 = new FlutterError("Children of Table must have the row role");
            }
            return (error__10915 is null);
            return default;
        })));
        return error__10915;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsRow(SemanticsNode node)
    {
        if ((!object.Equals(((SemanticsNode)node).parent?.role, SemanticsRole.table)))
        {
            return new FlutterError("A row must be a child of a table");
        }
        FlutterError? error__11372 = default!;
        node.visitChildren(((Func<SemanticsNode, bool>)((child) =>
        {
            if (((!object.Equals(child.getSemanticsData().role, SemanticsRole.cell)) && (!object.Equals(child.getSemanticsData().role, SemanticsRole.columnHeader))))
            {
                error__11372 = new FlutterError("Children of Row must have the cell or columnHeader role");
            }
            return (error__11372 is null);
            return default;
        })));
        return error__11372;
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
        FlutterError? error__12329 = default!;
        var hasCheckedChild__12344 = false;
        bool validateRadioGroupChildren(SemanticsNode node)
        {
            SemanticsData data__12453 = node.getSemanticsData();
            if ((object.Equals(((SemanticsData)data__12453).role, SemanticsRole.radioGroup)))
            {
                return (error__12329 is null);
            }
            if (!((SemanticsData)data__12453).flagsCollection.isInMutuallyExclusiveGroup)
            {
                node.visitChildren((Func<SemanticsNode, bool>)validateRadioGroupChildren);
                return (error__12329 is null);
            }
            if ((object.Equals(((SemanticsData)data__12453).flagsCollection.isChecked, CheckedState.isTrue)))
            {
                if (hasCheckedChild__12344)
                {
                    error__12329 = new FlutterError("Radio groups must not have multiple checked children");
                    return false;
                }
                hasCheckedChild__12344 = true;
            }
            DartRuntimePrimitives.Assert(() => (error__12329 is null));
            return true;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        node.visitChildren((Func<SemanticsNode, bool>)validateRadioGroupChildren);
        return error__12329;
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
        SemanticsNode? currentNode__13642 = node;
        while ((currentNode__13642?.parent is not null))
        {
            if (((object.Equals(currentNode__13642?.parent?.role, SemanticsRole.menu)) || (object.Equals(currentNode__13642?.parent?.role, SemanticsRole.menuBar))))
            {
                return null;
            }
            currentNode__13642 = currentNode__13642?.parent;
        }
        return new FlutterError("A menu item must be a child of a menu or a menu bar");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsMenuItemCheckbox(SemanticsNode node)
    {
        SemanticsData data__14086 = node.getSemanticsData();
        if ((object.Equals(((SemanticsData)data__14086).flagsCollection.isChecked, CheckedState.none)))
        {
            return new FlutterError("a menu item checkbox must be checkable");
        }
        SemanticsNode? currentNode__14276 = node;
        while ((currentNode__14276?.parent is not null))
        {
            if (((object.Equals(currentNode__14276?.parent?.role, SemanticsRole.menu)) || (object.Equals(currentNode__14276?.parent?.role, SemanticsRole.menuBar))))
            {
                return null;
            }
            currentNode__14276 = currentNode__14276?.parent;
        }
        return new FlutterError("A menu item checkbox must be a child of a menu or a menu bar");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsMenuItemRadio(SemanticsNode node)
    {
        SemanticsData data__14726 = node.getSemanticsData();
        if ((object.Equals(((SemanticsData)data__14726).flagsCollection.isChecked, CheckedState.none)))
        {
            return new FlutterError("a menu item radio must be checkable");
        }
        SemanticsNode? currentNode__14913 = node;
        while ((currentNode__14913?.parent is not null))
        {
            if (((object.Equals(currentNode__14913?.parent?.role, SemanticsRole.menu)) || (object.Equals(currentNode__14913?.parent?.role, SemanticsRole.menuBar))))
            {
                return null;
            }
            currentNode__14913 = currentNode__14913?.parent;
        }
        return new FlutterError("A menu item radio must be a child of a menu or a menu bar");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _noLiveRegion(SemanticsNode node)
    {
        SemanticsData data__15350 = node.getSemanticsData();
        if (((SemanticsData)data__15350).flagsCollection.isLiveRegion)
        {
            return new FlutterError($"Node {((SemanticsNode)node).id} has role {((SemanticsData)data__15350).role} but is also a live region. " + $"A node can not have {((SemanticsData)data__15350).role} and be live region at the same time. " + "Either remove the role or the live region");
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsListItem(SemanticsNode node)
    {
        SemanticsData data__15789 = node.getSemanticsData();
        SemanticsNode? parent__15846 = ((SemanticsNode)node).parent;
        if ((parent__15846 is null))
        {
            return new FlutterError($"Semantics node {((SemanticsNode)node).id} has role {((SemanticsData)data__15789).role} but doesn't have a parent");
        }
        SemanticsData parentSemanticsData__16045 = parent__15846.getSemanticsData();
        if ((!object.Equals(((SemanticsData)parentSemanticsData__16045).role, SemanticsRole.list)))
        {
            return new FlutterError($"Semantics node {((SemanticsNode)node).id} has role {((SemanticsData)data__15789).role}, but its " + $"parent node {((SemanticsNode)parent__15846).id} doesn't have the role {SemanticsRole.list}. " + $"Please assign the {SemanticsRole.list} to node {((SemanticsNode)parent__15846).id}");
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static bool _isLandmarkRole(SemanticsData nodeData) => (((((object.Equals(((SemanticsData)nodeData).role, SemanticsRole.complementary)) || (object.Equals(((SemanticsData)nodeData).role, SemanticsRole.contentInfo))) || (object.Equals(((SemanticsData)nodeData).role, SemanticsRole.main))) || (object.Equals(((SemanticsData)nodeData).role, SemanticsRole.navigation))) || (object.Equals(((SemanticsData)nodeData).role, SemanticsRole.region)));
    internal static bool _isSameRoleExisted(SemanticsNode semanticsNode)
    {
        DartMap<long, SemanticsNode> treeNodes__16840 = ((SemanticsNode)semanticsNode).owner!._nodes;
        var sameRoleCount__16889 = 0L;
        foreach (long id__16927 in treeNodes__16840.Keys)
        {
            if ((object.Equals(treeNodes__16840.GetValueOrDefault(id__16927)?.getSemanticsData().role, ((SemanticsNode)semanticsNode).role)))
            {
                sameRoleCount__16889++;
                if ((sameRoleCount__16889 > 1L))
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
        SemanticsNode? currentNode__17241 = ((SemanticsNode)node).parent;
        while ((currentNode__17241 is not null))
        {
            if (_isLandmarkRole(currentNode__17241.getSemanticsData()))
            {
                return new FlutterError("The complementary landmark role should not contained within any other landmark roles.");
            }
            currentNode__17241 = ((SemanticsNode)currentNode__17241).parent;
        }
        SemanticsData data__17581 = node.getSemanticsData();
        if ((_isSameRoleExisted(node) && (((SemanticsData)data__17581).label.Length == 0)))
        {
            return new FlutterError("The complementary landmark role should have a unique label as it is used more than once.");
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsContentInfo(SemanticsNode node)
    {
        SemanticsNode? currentNode__17921 = ((SemanticsNode)node).parent;
        while ((currentNode__17921 is not null))
        {
            if (_isLandmarkRole(currentNode__17921.getSemanticsData()))
            {
                return new FlutterError("The contentInfo landmark role should not contained within any other landmark roles.");
            }
            currentNode__17921 = ((SemanticsNode)currentNode__17921).parent;
        }
        SemanticsData data__18259 = node.getSemanticsData();
        if ((_isSameRoleExisted(node) && (((SemanticsData)data__18259).label.Length == 0)))
        {
            return new FlutterError("The contentInfo landmark role should have a unique label as it is used more than once.");
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsMain(SemanticsNode node)
    {
        SemanticsNode? currentNode__18590 = ((SemanticsNode)node).parent;
        while ((currentNode__18590 is not null))
        {
            if (_isLandmarkRole(currentNode__18590.getSemanticsData()))
            {
                return new FlutterError("The main landmark role should not contained within any other landmark roles.");
            }
            currentNode__18590 = ((SemanticsNode)currentNode__18590).parent;
        }
        SemanticsData data__18921 = node.getSemanticsData();
        if ((_isSameRoleExisted(node) && (((SemanticsData)data__18921).label.Length == 0)))
        {
            return new FlutterError("The main landmark role should have a unique label as it is used more than once.");
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsNavigation(SemanticsNode node)
    {
        SemanticsData data__19256 = node.getSemanticsData();
        if ((_isSameRoleExisted(node) && (((SemanticsData)data__19256).label.Length == 0)))
        {
            return new FlutterError("The navigation landmark role should have a unique label as it is used more than once.");
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsRegion(SemanticsNode node)
    {
        SemanticsData data__19593 = node.getSemanticsData();
        if ((((SemanticsData)data__19593).label.Length == 0))
        {
            return new FlutterError("A region role should include a label that describes the purpose of the content.");
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlutterError? _semanticsGeneral(SemanticsNode node)
    {
        SemanticsData data__19898 = node.getSemanticsData();
        bool? isExpanded__19946 = ((SemanticsData)data__19898).flagsCollection.isExpanded.toBoolOrNull();
        if ((isExpanded__19946 is not null))
        {
            bool isExpanded__19946__value20016 = DartRuntimePrimitives.RequireValue(isExpanded__19946);
            bool hasExpandAction__20055 = data__19898.hasAction(SemanticsAction.expand);
            bool hasCollapseAction__20130 = data__19898.hasAction(SemanticsAction.collapse);
            if ((hasExpandAction__20055 && hasCollapseAction__20130))
            {
                return new FlutterError("An expandable node cannot have both expand and collapse actions set at the same time.");
            }
            if ((DartRuntimePrimitives.RequireValue(isExpanded__19946__value20016) && hasExpandAction__20055))
            {
                return new FlutterError("An expanded node cannot have an expand action.");
            }
            if ((!DartRuntimePrimitives.RequireValue(isExpanded__19946__value20016) && hasCollapseAction__20130))
            {
                return new FlutterError("A collapsed node cannot have a collapse action.");
            }
        }
        if ((((SemanticsData)data__19898).flagsCollection.isAccessibilityFocusBlocked && (!object.Equals(((SemanticsData)data__19898).flagsCollection.isFocused, Tristate.none))))
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

    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "SemanticsTag"))}({this.name})";
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
                var seenConfigs__25579 = new HashSet<SemanticsConfiguration>();
                foreach (var config__25638 in new List<SemanticsConfiguration>())
                {
                    DartRuntimePrimitives.Assert(() => seenConfigs__25579.Add(config__25638));
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
        if (!_ids.TryGetValue(action, out var result__28793))
        {
            result__28793 = _nextId++;
            _ids[DartRuntimePrimitives.RequireReference(action)] = result__28793;
            _actions[result__28793] = action;
        }
        return result__28793;
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
            foreach (var attribute__29828 in __attributes)
            {
                DartRuntimePrimitives.Assert(() => ((@string.Length >= attribute__29828.range.start) && (@string.Length >= attribute__29828.range.end)));
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
        string newString__30778 = (this.@string + ((AttributedString)other).@string);
        var newAttributes__30823 = new List<global::Doroti.Flutter.Ui.StringAttribute>(this.attributes);
        if ((checked((long)(((AttributedString)other).attributes.Count)) != 0))
        {
            long offset__30932 = this.@string.Length;
            foreach (global::Doroti.Flutter.Ui.StringAttribute attribute__30989 in ((AttributedString)other).attributes)
            {
                var newRange__31036 = new global::Doroti.Flutter.Ui.TextRange(start: (attribute__30989.range.start + offset__30932), end: (attribute__30989.range.end + offset__30932));
                global::Doroti.Flutter.Ui.StringAttribute adjustedAttribute__31193 = attribute__30989.copy(range: newRange__31036);
                newAttributes__30823.Add(adjustedAttribute__31193);
            }
        }
        return new AttributedString(newString__30778, attributes: newAttributes__30823);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as AttributedString;
        if (__other is null) return false;
        return ((((object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())) && (__other is AttributedString)) && (((AttributedString)((AttributedString)__other)).@string == this.@string)) && global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals<global::Doroti.Flutter.Ui.StringAttribute>(((AttributedString)((AttributedString)__other)).attributes, this.attributes));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.@string, this.attributes);
    public override string ToString()
    {
        return $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "AttributedString"))}('{this.@string}', attributes: {this.attributes})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class AttributedStringProperty : DiagnosticsProperty<AttributedString>
{
    public virtual bool showWhenEmpty { get; private set; } = default!;

    public AttributedStringProperty(string name, AttributedString? value, bool showName = true, bool showWhenEmpty = false, object? defaultValue = default!, DiagnosticLevel level = DiagnosticLevel.info, string? description = null) : base(name, value, showName: showName, defaultValue: defaultValue ?? global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.kNoDefaultValue, level: level, description: description)
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
        string text__32930 = value!.@string;
        if (((parentConfiguration is not null) && !parentConfiguration.lineBreakProperties))
        {
            text__32930 = text__32930.replaceAll("\n", "\\n");
        }
        if ((checked((long)(value!.attributes.Count)) == 0))
        {
            return $"\"{text__32930}\"";
        }
        return $"\"{text__32930}\" {value!.attributes}";
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
            var (text__35608, _) = this._parts.First();
            return text__35608;
        }
        var buffer__35754 = new StringBuffer();
        var (firstText__35797, _) = this._parts.First();
        buffer__35754.write(firstText__35797);
        foreach (var (partText__35895, partTextDirection__35920) in this._parts.skip(1L))
        {
            global::Doroti.Flutter.Ui.TextDirection? partDirection__35987 = (partTextDirection__35920 ?? this.textDirection);
            if ((this.separator.Length != 0))
            {
                buffer__35754.write(this.separator);
            }
            var processedText__36126 = partText__35895;
            if ((((this.textDirection is not null) && (partDirection__35987 is not null)) && (!object.Equals(this.textDirection, DartRuntimePrimitives.RequireValue(partDirection__35987)))))
            {
                TextDirection textDirection__value36162 = DartRuntimePrimitives.RequireValue(textDirection);
                TextDirection partDirection__35987__value36187 = DartRuntimePrimitives.RequireValue(partDirection__35987);
                string directionalEmbedding__36267 = (DartRuntimePrimitives.RequireValue(partDirection__35987__value36187) switch { TextDirection.rtl => Unicode.RLE, TextDirection.ltr => Unicode.LRE, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                processedText__36126 = ((directionalEmbedding__36267 + partText__35895) + Unicode.PDF);
            }
            buffer__35754.write(processedText__36126);
        }
        return buffer__35754.ToString();
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
    public virtual string toStringShort() => global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "SemanticsData");
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Flutter.Ui.Rect>("rect", this.rect, showName: false));
        properties.add(new global::Doroti.Generated.Framework.Painting.TransformProperty("transform", this.transform, showName: false, defaultValue: null));
        var actionSummary__49230 = new List<string>();
        List<string?> customSemanticsActionSummary__49410 = this.customSemanticsActionIds!.map<long, string?>(((actionId) => CustomSemanticsAction.getAction(actionId)!.label)).ToList();
        properties.add(new IterableProperty<string>("actions", actionSummary__49230, ifEmpty: null));
        properties.add(new IterableProperty<string?>("customActions", customSemanticsActionSummary__49410, ifEmpty: null));
        List<string> flagSummary__49809 = this.flagsCollection.toStrings();
        properties.add(new IterableProperty<string>("flags", flagSummary__49809, ifEmpty: null));
        properties.add(new StringProperty("identifier", this.identifier, defaultValue: ""));
        properties.add(new DiagnosticsProperty<object>("traversalParentIdentifier", this.traversalParentIdentifier, defaultValue: null));
        properties.add(new DiagnosticsProperty<object>("traversalChildIdentifier", this.traversalChildIdentifier, defaultValue: null));
        properties.add(new AttributedStringProperty("label", this.attributedLabel));
        properties.add(new AttributedStringProperty("value", this.attributedValue));
        properties.add(new AttributedStringProperty("increasedValue", this.attributedIncreasedValue));
        properties.add(new AttributedStringProperty("decreasedValue", this.attributedDecreasedValue));
        properties.add(new AttributedStringProperty("hint", this.attributedHint));
        properties.add(new StringProperty("tooltip", this.tooltip, defaultValue: ""));
        properties.add(new EnumProperty<global::Doroti.Flutter.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
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
            properties.add(new EnumProperty<global::Doroti.Flutter.Ui.SemanticsRole>("role", this.role, defaultValue: SemanticsRole.none));
        }
        if ((!object.Equals(this.validationResult, SemanticsValidationResult.none)))
        {
            properties.add(new EnumProperty<global::Doroti.Flutter.Ui.SemanticsValidationResult>("validationResult", this.validationResult, defaultValue: SemanticsValidationResult.none));
        }
        properties.add(new StringProperty("minValue", this.minValue, defaultValue: null));
        properties.add(new StringProperty("maxValue", this.maxValue, defaultValue: null));
    }

    public override bool Equals(object? other)
    {
        var __other = other as SemanticsData;
        if (__other is null) return false;
        return (((((((((((((((((((((((((((((((((((((__other is SemanticsData) && (((SemanticsData)((SemanticsData)__other)).flags == this.flags)) && (((SemanticsData)((SemanticsData)__other)).actions == this.actions)) && (((SemanticsData)((SemanticsData)__other)).identifier == this.identifier)) && (object.Equals(((SemanticsData)((SemanticsData)__other)).traversalParentIdentifier, this.traversalParentIdentifier))) && (object.Equals(((SemanticsData)((SemanticsData)__other)).traversalChildIdentifier, this.traversalChildIdentifier))) && (object.Equals(((SemanticsData)((SemanticsData)__other)).attributedLabel, this.attributedLabel))) && (object.Equals(((SemanticsData)((SemanticsData)__other)).attributedValue, this.attributedValue))) && (object.Equals(((SemanticsData)((SemanticsData)__other)).attributedIncreasedValue, this.attributedIncreasedValue))) && (object.Equals(((SemanticsData)((SemanticsData)__other)).attributedDecreasedValue, this.attributedDecreasedValue))) && (object.Equals(((SemanticsData)((SemanticsData)__other)).attributedHint, this.attributedHint))) && (((SemanticsData)((SemanticsData)__other)).tooltip == this.tooltip)) && (object.Equals(((SemanticsData)((SemanticsData)__other)).textDirection, this.textDirection))) && (object.Equals(((SemanticsData)((SemanticsData)__other)).rect, this.rect))) && global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.setEquals(((SemanticsData)((SemanticsData)__other)).tags, this.tags)) && (((SemanticsData)((SemanticsData)__other)).scrollChildCount == this.scrollChildCount)) && (((SemanticsData)((SemanticsData)__other)).scrollIndex == this.scrollIndex)) && (object.Equals(((SemanticsData)((SemanticsData)__other)).textSelection, this.textSelection))) && (((SemanticsData)((SemanticsData)__other)).scrollPosition == this.scrollPosition)) && (((SemanticsData)((SemanticsData)__other)).scrollExtentMax == this.scrollExtentMax)) && (((SemanticsData)((SemanticsData)__other)).scrollExtentMin == this.scrollExtentMin)) && (((SemanticsData)((SemanticsData)__other)).platformViewId == this.platformViewId)) && (((SemanticsData)((SemanticsData)__other)).maxValueLength == this.maxValueLength)) && (((SemanticsData)((SemanticsData)__other)).currentValueLength == this.currentValueLength)) && (object.Equals(((SemanticsData)((SemanticsData)__other)).transform, this.transform))) && (((SemanticsData)((SemanticsData)__other)).headingLevel == this.headingLevel)) && (object.Equals(((SemanticsData)((SemanticsData)__other)).linkUrl, this.linkUrl))) && (object.Equals(((SemanticsData)((SemanticsData)__other)).role, this.role))) && (object.Equals(((SemanticsData)((SemanticsData)__other)).validationResult, this.validationResult))) && (object.Equals(((SemanticsData)((SemanticsData)__other)).inputType, this.inputType))) && (object.Equals(((SemanticsData)((SemanticsData)__other)).hitTestBehavior, this.hitTestBehavior))) && _sortedListsEqual(((SemanticsData)((SemanticsData)__other)).customSemanticsActionIds, this.customSemanticsActionIds)) && global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.setEquals<string>(this.controlsNodes, ((SemanticsData)((SemanticsData)__other)).controlsNodes)) && (object.Equals(((SemanticsData)((SemanticsData)__other)).traversalParentIdentifier, this.traversalParentIdentifier))) && (object.Equals(((SemanticsData)((SemanticsData)__other)).traversalChildIdentifier, this.traversalChildIdentifier))) && (((SemanticsData)((SemanticsData)__other)).minValue == this.minValue)) && (((SemanticsData)((SemanticsData)__other)).maxValue == this.maxValue));
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
            for (var i__55721 = 0L; (i__55721 < checked((long)(left.Count))); i__55721++)
            {
                if ((left[(int)(i__55721)] != right[(int)(i__55721)]))
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
        properties.add(new EnumProperty<global::Doroti.Flutter.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
        properties.add(new EnumProperty<global::Doroti.Flutter.Ui.SemanticsRole>("role", this.role, defaultValue: null));
        properties.add(new EnumProperty<global::Doroti.Flutter.Ui.SemanticsValidationResult>("validationResult", this.validationResult, defaultValue: SemanticsValidationResult.none));
        properties.add(new DiagnosticsProperty<SemanticsSortKey>("sortKey", this.sortKey, defaultValue: null));
        properties.add(new DiagnosticsProperty<SemanticsHintOverrides>("hintOverrides", this.hintOverrides, defaultValue: null));
    }

    public virtual string toStringShort() => global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "SemanticsProperties");
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
    internal virtual global::Doroti.Flutter.Ui.TextDirection? _textDirection { get; set; } = ((SemanticsConfiguration)_kEmptyConfig).textDirection;
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
    internal virtual global::Doroti.Flutter.Ui.SemanticsRole _role { get; set; } = ((SemanticsConfiguration)_kEmptyConfig).role;
    internal virtual HashSet<string>? _controlsNodes { get; set; } = ((SemanticsConfiguration)_kEmptyConfig).controlsNodes;
    internal virtual string? _minValue { get; set; } = default;
    internal virtual string? _maxValue { get; set; } = default;
    internal virtual global::Doroti.Flutter.Ui.SemanticsValidationResult _validationResult { get; set; } = ((SemanticsConfiguration)_kEmptyConfig).validationResult;
    internal virtual SemanticsHitTestBehavior _hitTestBehavior { get; set; } = Dart_uiLibrary.SemanticsHitTestBehavior.defer;
    internal virtual global::Doroti.Flutter.Ui.SemanticsInputType _inputType { get; set; } = ((SemanticsConfiguration)_kEmptyConfig).inputType;
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
        var __instance = new SemanticsNode(default!, default!);
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
            return (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb ? this.transform : ((this._traversalChildTransform ?? this.transform)));
            return default!;
        }
    }
    public virtual global::Doroti.Flutter.Ui.Rect rect
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
                var seenChildren__112516 = new HashSet<SemanticsNode>();
                foreach (var child__112567 in newChildren)
                {
                    DartRuntimePrimitives.Assert(() => seenChildren__112516.Add(child__112567));
                }
                return true;
            });
        if ((this._children is not null))
        {
            foreach (SemanticsNode child__112814 in this._children!)
            {
                child__112814._dead = true;
            }
        }
        foreach (var child__112894 in newChildren)
        {
            child__112894._dead = false;
        }
        var sawChange__112959 = false;
        if ((this._children is not null))
        {
            foreach (SemanticsNode child__113038 in this._children!)
            {
                if (((SemanticsNode)child__113038)._dead)
                {
                    if ((object.Equals(((SemanticsNode)child__113038).parent, this)))
                    {
                        _dropChild(child__113038);
                    }
                    sawChange__112959 = true;
                }
            }
        }
        foreach (var child__113361 in newChildren)
        {
            if ((!object.Equals(((SemanticsNode)child__113361).parent, this)))
            {
                if ((((SemanticsNode)child__113361).parent is not null))
                {
                    ((SemanticsNode)child__113361).parent?._dropChild(child__113361);
                }
                DartRuntimePrimitives.Assert(() => !((SemanticsNode)child__113361).attached);
                _adoptChild(child__113361);
                sawChange__112959 = true;
            }
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if (DartRuntimePrimitives.Identical(newChildren, this._children))
                {
                    var mutationErrors__114129 = new List<DiagnosticsNode>();
                    if ((checked((long)(newChildren.Count)) != checked((long)(this._debugPreviousSnapshot.Count))))
                    {
                        mutationErrors__114129.Add(new ErrorDescription($"The list's length has changed from {checked((long)(this._debugPreviousSnapshot.Count))} " + $"to {checked((long)(newChildren.Count))}."));
                    }
                    else
                    {
                        for (var i__114486 = 0L; (i__114486 < checked((long)(newChildren.Count))); i__114486++)
                        {
                            if (!DartRuntimePrimitives.Identical(newChildren[(int)(i__114486)], this._debugPreviousSnapshot[(int)(i__114486)]))
                            {
                                if ((checked((long)(mutationErrors__114129.Count)) != 0))
                                {
                                    mutationErrors__114129.Add(new ErrorSpacer());
                                }
                                mutationErrors__114129.Add(new ErrorDescription($"Child node at position {i__114486} was replaced:"));
                                mutationErrors__114129.Add(((Diagnosticable)this._debugPreviousSnapshot[(int)(i__114486)]).toDiagnosticsNode(name: "Previous child", style: DiagnosticsTreeStyle.singleLine));
                                mutationErrors__114129.Add(((Diagnosticable)newChildren[(int)(i__114486)]).toDiagnosticsNode(name: "New child", style: DiagnosticsTreeStyle.singleLine));
                            }
                        }
                    }
                    if ((checked((long)(mutationErrors__114129.Count)) != 0))
                    {
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Failed to replace child semantics nodes because the list of `SemanticsNode`s was mutated."), new ErrorHint("Instead of mutating the existing list, create a new list containing the desired `SemanticsNode`s."), new ErrorDescription("Error details:") });
                    }
                }
                _debugPreviousSnapshot = new List<SemanticsNode>(newChildren);
                var ancestor__115878 = this;
                while ((((SemanticsNode)ancestor__115878).parent is SemanticsNode))
                {
                    ancestor__115878 = ((SemanticsNode)ancestor__115878).parent!;
                }
                DartRuntimePrimitives.Assert(() => !newChildren.any(((child) => (object.Equals(child, ancestor__115878)))));
                return true;
            });
        if ((!sawChange__112959 && (this._children is not null)))
        {
            DartRuntimePrimitives.Assert(() => (checked((long)(newChildren.Count)) == checked((long)(this._children!.Count))));
            for (var i__116239 = 0L; (i__116239 < checked((long)(this._children!.Count))); i__116239++)
            {
                if ((this._children![(int)(i__116239)].id != newChildren[(int)(i__116239)].id))
                {
                    sawChange__112959 = true;
                    break;
                }
            }
        }
        _children = newChildren;
        if (sawChange__112959)
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
            foreach (SemanticsNode child__117159 in this._children!)
            {
                if (!visitor(child__117159))
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
            foreach (SemanticsNode child__117627 in this._children!)
            {
                if ((!visitor(child__117627) || !child__117627._visitDescendants((Func<SemanticsNode, bool>)visitor)))
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
        bool childShouldMergeToParent__120105 = this.isPartOfNodeMerging;
        if ((childShouldMergeToParent__120105 == ((SemanticsNode)child).isMergedIntoParent))
        {
            return;
        }
        child.isMergedIntoParent = childShouldMergeToParent__120105;
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
                var node__120695 = this;
                while ((((SemanticsNode)node__120695).parent is not null))
                {
                    node__120695 = ((SemanticsNode)node__120695).parent!;
                }
                DartRuntimePrimitives.Assert(() => (!object.Equals(node__120695, child)));
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
            foreach (SemanticsNode child__122164 in this._children!)
            {
                child__122164.attach(owner);
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
        if (this._traversalChildIdentifier is object identifier__122565)
        {
            this.owner!._traversalParentNodes.GetValueOrDefault(identifier__122565)?._markDirty();
        }
        this.owner!._traversalParentNodes.removeWhere(((key, node) => (object.Equals(node, this))));
        foreach (HashSet<SemanticsNode> childSet__122931 in this.owner!._traversalChildNodes.Values)
        {
            childSet__122931.removeWhere(((node) => (object.Equals(node, this))));
        }
        this.owner!._traversalChildNodes.removeWhere(((key, value) => (checked((long)(value.Count)) == 0)));
        _owner = null;
        DartRuntimePrimitives.Assert(() => ((this.parent is null) || (this.attached == this.parent!.attached)));
        if ((this._children is not null))
        {
            foreach (SemanticsNode child__123308 in this._children!)
            {
                if ((object.Equals(((SemanticsNode)child__123308).parent, this)))
                {
                    child__123308.detach();
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
            bool? isDirty__124263 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    isDirty__124263 = this._dirty;
                    return true;
                });
            return isDirty__124263;
            return default!;
        }
    }
    internal virtual bool _isDifferentFromCurrentSemanticAnnotation(SemanticsConfiguration config)
    {
        return ((((((((((((((((((((((((((((((!object.Equals(this._attributedLabel, ((SemanticsConfiguration)config).attributedLabel)) || (!object.Equals(this._attributedHint, ((SemanticsConfiguration)config).attributedHint))) || (!object.Equals(this._attributedValue, ((SemanticsConfiguration)config).attributedValue))) || (!object.Equals(this._attributedIncreasedValue, ((SemanticsConfiguration)config).attributedIncreasedValue))) || (!object.Equals(this._attributedDecreasedValue, ((SemanticsConfiguration)config).attributedDecreasedValue))) || (this._tooltip != ((SemanticsConfiguration)config).tooltip)) || (!object.Equals(this._flags, ((SemanticsConfiguration)config)._flags))) || (!object.Equals(this._textDirection, ((SemanticsConfiguration)config).textDirection))) || (!object.Equals(this._sortKey, ((SemanticsConfiguration)config)._sortKey))) || (!object.Equals(this._textSelection, ((SemanticsConfiguration)config)._textSelection))) || (this._scrollPosition != ((SemanticsConfiguration)config)._scrollPosition)) || (this._scrollExtentMax != ((SemanticsConfiguration)config)._scrollExtentMax)) || (this._scrollExtentMin != ((SemanticsConfiguration)config)._scrollExtentMin)) || (this._actionsAsBits != ((SemanticsConfiguration)config)._actionsAsBits)) || (this.indexInParent != ((SemanticsConfiguration)config).indexInParent)) || (this.platformViewId != ((SemanticsConfiguration)config).platformViewId)) || (this._maxValueLength != ((SemanticsConfiguration)config)._maxValueLength)) || (this._currentValueLength != ((SemanticsConfiguration)config)._currentValueLength)) || (this._mergeAllDescendantsIntoThisNode != ((SemanticsConfiguration)config).isMergingSemanticsOfDescendants)) || (this._areUserActionsBlocked != ((SemanticsConfiguration)config).isBlockingUserActions)) || (this._headingLevel != ((SemanticsConfiguration)config)._headingLevel)) || (!object.Equals(this._linkUrl, ((SemanticsConfiguration)config)._linkUrl))) || (!object.Equals(this._role, ((SemanticsConfiguration)config).role))) || (!object.Equals(this._validationResult, ((SemanticsConfiguration)config).validationResult))) || (!object.Equals(this._hitTestBehavior, ((SemanticsConfiguration)config).hitTestBehavior))) || (!object.Equals(this._traversalChildIdentifier, ((SemanticsConfiguration)config)._traversalChildIdentifier))) || (!object.Equals(this._traversalParentIdentifier, ((SemanticsConfiguration)config)._traversalParentIdentifier))) || (this._minValue != ((SemanticsConfiguration)config)._minValue)) || (this._maxValue != ((SemanticsConfiguration)config)._maxValue)) || !global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.mapEquals<CustomSemanticsAction, Action>(this._customSemanticsActions, ((SemanticsConfiguration)config)._customSemanticsActions));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual long _effectiveActionsAsBits => (this._areUserActionsBlocked ? (this._actionsAsBits & SemanticsLibrary._kUnblockedUserActions) : this._actionsAsBits);
    public virtual bool isTagged(SemanticsTag tag) => ((this.tags is not null) && this.tags!.Contains(tag));
    public virtual global::Doroti.Flutter.Ui.SemanticsFlags flagsCollection => this._flags;
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
    public virtual global::Doroti.Flutter.Ui.TextDirection? textDirection => this._textDirection;
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
    public virtual global::Doroti.Flutter.Ui.SemanticsRole role => this._role;
    public virtual HashSet<string>? controlsNodes => this._controlsNodes;
    public virtual string? minValue => this._minValue;
    public virtual string? maxValue => this._maxValue;
    public virtual global::Doroti.Flutter.Ui.SemanticsValidationResult validationResult => this._validationResult;
    public virtual global::Doroti.Flutter.Ui.SemanticsHitTestBehavior hitTestBehavior => this._hitTestBehavior;
    public virtual global::Doroti.Flutter.Ui.SemanticsInputType inputType => this._inputType;
    internal virtual bool _canPerformAction(SemanticsAction action) => this._actions.ContainsKey(action);
    internal virtual bool _canPerformCustomAction(long actionId)
    {
        CustomSemanticsAction? customAction__138638 = CustomSemanticsAction.getAction(actionId);
        return ((customAction__138638 is not null) && this._customSemanticsActions.ContainsKey(customAction__138638));
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
        var mergeAllDescendantsIntoThisNodeValueChanged__140260 = (this._mergeAllDescendantsIntoThisNode != ((SemanticsConfiguration)config).isMergingSemanticsOfDescendants);
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
        _actions = new DartMap<global::Doroti.Flutter.Ui.SemanticsAction, Action<object?>>(((SemanticsConfiguration)config)._actions);
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
        if (mergeAllDescendantsIntoThisNodeValueChanged__140260)
        {
            _updateChildrenMergeFlags();
        }
        DartRuntimePrimitives.Assert(() => (!_canPerformAction(SemanticsAction.increase) || (((this.value == "")) == ((this.increasedValue == "")))));
        DartRuntimePrimitives.Assert(() => (!_canPerformAction(SemanticsAction.decrease) || (((this.value == "")) == ((this.decreasedValue == "")))));
    }

    public virtual SemanticsData getSemanticsData()
    {
        global::Doroti.Flutter.Ui.SemanticsFlags flags__143252 = this._flags;
        long actions__143411 = this._actionsAsBits;
        string identifier__143448 = this._identifier;
        object? traversalParentIdentifier__143486 = this._traversalParentIdentifier;
        object? traversalChildIdentifier__143554 = this._traversalChildIdentifier;
        AttributedString attributedLabel__143629 = this._attributedLabel;
        AttributedString attributedValue__143686 = this._attributedValue;
        AttributedString attributedIncreasedValue__143743 = this._attributedIncreasedValue;
        AttributedString attributedDecreasedValue__143818 = this._attributedDecreasedValue;
        AttributedString attributedHint__143893 = this._attributedHint;
        string tooltip__143938 = this._tooltip;
        global::Doroti.Flutter.Ui.TextDirection? textDirection__143977 = this._textDirection;
        HashSet<SemanticsTag>? mergedTags__144032 = ((this.tags is null) ? null : new HashSet<SemanticsTag>(this.tags!));
        TextSelection? textSelection__144115 = this._textSelection;
        long? scrollChildCount__144156 = this._scrollChildCount;
        long? scrollIndex__144203 = this._scrollIndex;
        double? scrollPosition__144243 = this._scrollPosition;
        double? scrollExtentMax__144289 = this._scrollExtentMax;
        double? scrollExtentMin__144337 = this._scrollExtentMin;
        long? platformViewId__144382 = this._platformViewId;
        long? maxValueLength__144425 = this._maxValueLength;
        long? currentValueLength__144468 = this._currentValueLength;
        long headingLevel__144518 = this._headingLevel;
        DartUri? linkUrl__144557 = this._linkUrl;
        global::Doroti.Flutter.Ui.SemanticsRole role__144595 = this._role;
        HashSet<string>? controlsNodes__144626 = this._controlsNodes;
        global::Doroti.Flutter.Ui.SemanticsValidationResult validationResult__144688 = this._validationResult;
        global::Doroti.Flutter.Ui.SemanticsHitTestBehavior hitTestBehavior__144758 = this._hitTestBehavior;
        global::Doroti.Flutter.Ui.SemanticsInputType inputType__144817 = this._inputType;
        global::Doroti.Flutter.Ui.Locale? locale__144859 = this._locale;
        var customSemanticsActionIds__144887 = new HashSet<long>();
        string? minValue__144935 = this._minValue;
        string? maxValue__144969 = this._maxValue;
        foreach (CustomSemanticsAction action__145028 in this._customSemanticsActions.Keys)
        {
            customSemanticsActionIds__144887.Add(CustomSemanticsAction.getIdentifier(action__145028));
        }
        if ((this.hintOverrides is not null))
        {
            if ((this.hintOverrides!.onTapHint is not null))
            {
                var action__145250 = CustomSemanticsAction.CreateOverridingAction(hint: this.hintOverrides!.onTapHint!, action: SemanticsAction.tap);
                customSemanticsActionIds__144887.Add(CustomSemanticsAction.getIdentifier(action__145250));
            }
            if ((this.hintOverrides!.onLongPressHint is not null))
            {
                var action__145549 = CustomSemanticsAction.CreateOverridingAction(hint: this.hintOverrides!.onLongPressHint!, action: SemanticsAction.longPress);
                customSemanticsActionIds__144887.Add(CustomSemanticsAction.getIdentifier(action__145549));
            }
        }
        if (this.mergeAllDescendantsIntoThisNode)
        {
            _visitDescendants(((Func<SemanticsNode, bool>)((node) =>
            {
                DartRuntimePrimitives.Assert(() => ((SemanticsNode)node).isMergedIntoParent);
                flags__143252 = flags__143252.merge(((SemanticsNode)node)._flags);
                actions__143411 |= ((SemanticsNode)node)._effectiveActionsAsBits;
                textDirection__143977 ??= ((SemanticsNode)node)._textDirection;
                textSelection__144115 ??= ((SemanticsNode)node)._textSelection;
                scrollChildCount__144156 ??= ((SemanticsNode)node)._scrollChildCount;
                scrollIndex__144203 ??= ((SemanticsNode)node)._scrollIndex;
                scrollPosition__144243 ??= ((SemanticsNode)node)._scrollPosition;
                scrollExtentMax__144289 ??= ((SemanticsNode)node)._scrollExtentMax;
                scrollExtentMin__144337 ??= ((SemanticsNode)node)._scrollExtentMin;
                platformViewId__144382 ??= ((SemanticsNode)node)._platformViewId;
                maxValueLength__144425 ??= ((SemanticsNode)node)._maxValueLength;
                currentValueLength__144468 ??= ((SemanticsNode)node)._currentValueLength;
                linkUrl__144557 ??= ((SemanticsNode)node)._linkUrl;
                headingLevel__144518 = SemanticsLibrary._mergeHeadingLevels(sourceLevel: ((SemanticsNode)node)._headingLevel, targetLevel: headingLevel__144518);
                if ((identifier__143448 == ""))
                {
                    identifier__143448 = ((SemanticsNode)node)._identifier;
                }
                traversalParentIdentifier__143486 ??= ((SemanticsNode)node).traversalParentIdentifier;
                traversalChildIdentifier__143554 ??= ((SemanticsNode)node).traversalChildIdentifier;
                if ((((AttributedString)attributedValue__143686).@string == ""))
                {
                    attributedValue__143686 = ((SemanticsNode)node)._attributedValue;
                }
                if ((((AttributedString)attributedIncreasedValue__143743).@string == ""))
                {
                    attributedIncreasedValue__143743 = ((SemanticsNode)node)._attributedIncreasedValue;
                }
                if ((((AttributedString)attributedDecreasedValue__143818).@string == ""))
                {
                    attributedDecreasedValue__143818 = ((SemanticsNode)node)._attributedDecreasedValue;
                }
                if ((object.Equals(role__144595, SemanticsRole.none)))
                {
                    role__144595 = ((SemanticsNode)node)._role;
                }
                if ((object.Equals(inputType__144817, SemanticsInputType.none)))
                {
                    inputType__144817 = ((SemanticsNode)node)._inputType;
                }
                if ((object.Equals(hitTestBehavior__144758, Dart_uiLibrary.SemanticsHitTestBehavior.defer)))
                {
                    hitTestBehavior__144758 = ((SemanticsNode)node)._hitTestBehavior;
                }
                if ((tooltip__143938 == ""))
                {
                    tooltip__143938 = ((SemanticsNode)node)._tooltip;
                }
                if ((((SemanticsNode)node).tags is not null))
                {
                    mergedTags__144032 ??= new HashSet<SemanticsTag>();
                    mergedTags__144032!.UnionWith(((SemanticsNode)node).tags!);
                }
                foreach (CustomSemanticsAction action__147834 in ((SemanticsNode)node)._customSemanticsActions.Keys)
                {
                    customSemanticsActionIds__144887.Add(CustomSemanticsAction.getIdentifier(action__147834));
                }
                if ((((SemanticsNode)node).hintOverrides is not null))
                {
                    if ((((SemanticsNode)node).hintOverrides!.onTapHint is not null))
                    {
                        var action__148091 = CustomSemanticsAction.CreateOverridingAction(hint: ((SemanticsNode)node).hintOverrides!.onTapHint!, action: SemanticsAction.tap);
                        customSemanticsActionIds__144887.Add(CustomSemanticsAction.getIdentifier(action__148091));
                    }
                    if ((((SemanticsNode)node).hintOverrides!.onLongPressHint is not null))
                    {
                        var action__148428 = CustomSemanticsAction.CreateOverridingAction(hint: ((SemanticsNode)node).hintOverrides!.onLongPressHint!, action: SemanticsAction.longPress);
                        customSemanticsActionIds__144887.Add(CustomSemanticsAction.getIdentifier(action__148428));
                    }
                }
                attributedLabel__143629 = SemanticsLibrary._concatAttributedString(thisAttributedString: attributedLabel__143629, thisTextDirection: textDirection__143977, otherAttributedString: ((SemanticsNode)node)._attributedLabel, otherTextDirection: ((SemanticsNode)node)._textDirection);
                attributedHint__143893 = SemanticsLibrary._concatAttributedString(thisAttributedString: attributedHint__143893, thisTextDirection: textDirection__143977, otherAttributedString: ((SemanticsNode)node)._attributedHint, otherTextDirection: ((SemanticsNode)node)._textDirection);
                if ((controlsNodes__144626 is null))
                {
                    controlsNodes__144626 = ((SemanticsNode)node)._controlsNodes;
                }
                else
                {
                    if ((((SemanticsNode)node)._controlsNodes is not null))
                    {
                        controlsNodes__144626 = new HashSet<string>();
                    }
                }
                minValue__144935 ??= ((SemanticsNode)node)._minValue;
                maxValue__144969 ??= ((SemanticsNode)node)._maxValue;
                if ((object.Equals(validationResult__144688, SemanticsValidationResult.none)))
                {
                    validationResult__144688 = ((SemanticsNode)node)._validationResult;
                }
                else
                {
                    if ((object.Equals(validationResult__144688, SemanticsValidationResult.valid)))
                    {
                        if (((!object.Equals(((SemanticsNode)node)._validationResult, SemanticsValidationResult.none)) && (!object.Equals(((SemanticsNode)node)._validationResult, SemanticsValidationResult.valid))))
                        {
                            validationResult__144688 = ((SemanticsNode)node)._validationResult;
                        }
                    }
                }
                return true;
                return default;
            })));
        }
        return new SemanticsData(flagsCollection: flags__143252, actions: (this._areUserActionsBlocked ? (actions__143411 & SemanticsLibrary._kUnblockedUserActions) : actions__143411), identifier: identifier__143448, traversalParentIdentifier: traversalParentIdentifier__143486, traversalChildIdentifier: traversalChildIdentifier__143554, attributedLabel: attributedLabel__143629, attributedValue: attributedValue__143686, attributedIncreasedValue: attributedIncreasedValue__143743, attributedDecreasedValue: attributedDecreasedValue__143818, attributedHint: attributedHint__143893, tooltip: tooltip__143938, textDirection: textDirection__143977, rect: this.rect, transform: this.transform, tags: mergedTags__144032, textSelection: textSelection__144115, scrollChildCount: scrollChildCount__144156, scrollIndex: scrollIndex__144203, scrollPosition: scrollPosition__144243, scrollExtentMax: scrollExtentMax__144289, scrollExtentMin: scrollExtentMin__144337, platformViewId: platformViewId__144382, maxValueLength: maxValueLength__144425, currentValueLength: currentValueLength__144468, customSemanticsActionIds: ((Func<List<long>>)(() =>
{
    var __cascade = customSemanticsActionIds__144887.ToList();
    __cascade.sort();
    return __cascade;
}))(), headingLevel: headingLevel__144518, linkUrl: linkUrl__144557, role: role__144595, controlsNodes: controlsNodes__144626, validationResult: validationResult__144688, hitTestBehavior: hitTestBehavior__144758, inputType: inputType__144817, locale: locale__144859, minValue: minValue__144935, maxValue: maxValue__144969);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static Matrix4 _computeTraversalTransform(SemanticsNode parent, SemanticsNode child)
    {
        var traversalTransform__151843 = Matrix4.identity();
        Matrix4? parentToCommonAncestorTransform__151897 = default!;
        var fromNode__151938 = child;
        var toNode__151964 = parent;
        while (!DartRuntimePrimitives.Identical(fromNode__151938, toNode__151964))
        {
            long fromDepth__152074 = ((SemanticsNode)fromNode__151938).depth;
            long toDepth__152118 = ((SemanticsNode)toNode__151964).depth;
            if ((fromDepth__152074 >= toDepth__152118))
            {
                if (((SemanticsNode)fromNode__151938).transform is Matrix4 transform__152227)
                {
                    traversalTransform__151843.multiply(transform__152227);
                }
                fromNode__151938 = ((SemanticsNode)fromNode__151938).parent!;
            }
            if ((fromDepth__152074 <= toDepth__152118))
            {
                parentToCommonAncestorTransform__151897 ??= Matrix4.identity();
                if (((SemanticsNode)toNode__151964).transform is Matrix4 transform__152492)
                {
                    parentToCommonAncestorTransform__151897.multiply(transform__152492);
                }
                toNode__151964 = ((SemanticsNode)toNode__151964).parent!;
            }
        }
        if ((parentToCommonAncestorTransform__151897 is not null))
        {
            if ((parentToCommonAncestorTransform__151897.invert() != 0L))
            {
                traversalTransform__151843.multiply(parentToCommonAncestorTransform__151897);
            }
            else
            {
                traversalTransform__151843.setZero();
            }
        }
        return traversalTransform__151843;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Int32List _childrenIdInTraversalOrder()
    {
        List<SemanticsNode> sortedChildren__152984 = _childrenInTraversalOrder();
        var childrenInTraversalOrder__153041 = new Int32List(checked((long)(sortedChildren__152984.Count)));
        for (var i__153115 = 0L; (i__153115 < checked((long)(sortedChildren__152984.Count))); i__153115 += 1L)
        {
            childrenInTraversalOrder__153041[i__153115] = checked((int)(sortedChildren__152984[(int)(i__153115)].id));
        }
        return childrenInTraversalOrder__153041;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<SemanticsNode> _childrenInHitTestOrder()
    {
        if ((this._children is null))
        {
            return new List<SemanticsNode>();
        }
        if ((global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb || this._isTraversalParent))
        {
            return this._children!;
        }
        bool shouldNotSkipInHitTest(SemanticsNode child)
        {
            if (((SemanticsNode)child)._isTraversalChild)
            {
                SemanticsNode? traversalParent__153819 = this.owner!._traversalParentNodes.GetValueOrDefault(DartRuntimePrimitives.RequireReference(child.getSemanticsData().traversalChildIdentifier));
                return (traversalParent__153819 is not null);
            }
            return true;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        return this._children!.where(shouldNotSkipInHitTest).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Int32List _childrenIdInHitTestOrder()
    {
        List<SemanticsNode> children__154143 = _childrenInHitTestOrder();
        return new Int32List(System.Linq.Enumerable.Reverse(children__154143).map<SemanticsNode, long>(((node) => ((SemanticsNode)node).id)).ToList());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _addToUpdate(SemanticsUpdateBuilder builder, HashSet<long> customSemanticsActionIdsUpdate)
    {
        DartRuntimePrimitives.Assert(() => this._dirty);
        SemanticsData data__154426 = getSemanticsData();
        DartRuntimePrimitives.Assert(() =>
            {
                FlutterError? error__154495 = _DebugSemanticsRoleChecks__semantics._checkSemanticsData(this);
                if ((error__154495 is not null))
                {
                    throw error__154495;
                }
                return true;
            });
        Int32List childrenInTraversalOrder__154661 = default!;
        Int32List childrenInHitTestOrder__154707 = default!;
        if ((!this.hasChildren || this.mergeAllDescendantsIntoThisNode))
        {
            if ((this._isTraversalParent && !global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb))
            {
                if (((this.owner is not null) && this.owner!._traversalChildNodes.ContainsKey(this.traversalParentIdentifier)))
                {
                    HashSet<SemanticsNode> traversalChildren__155192 = this.owner!._traversalChildNodes.GetValueOrDefault(DartRuntimePrimitives.RequireReference(this.traversalParentIdentifier))!;
                    var index__155297 = 0L;
                    childrenInTraversalOrder__154661 = new Int32List(checked((long)(traversalChildren__155192.Count)));
                    foreach (var node__155403 in traversalChildren__155192)
                    {
                        if (((SemanticsNode)node__155403).attached)
                        {
                            childrenInTraversalOrder__154661[index__155297] = checked((int)(((SemanticsNode)node__155403).id));
                            index__155297 += 1L;
                        }
                    }
                }
                else
                {
                    childrenInTraversalOrder__154661 = _kEmptyChildList;
                }
                childrenInHitTestOrder__154707 = _kEmptyChildList;
            }
            else
            {
                childrenInTraversalOrder__154661 = _kEmptyChildList;
                childrenInHitTestOrder__154707 = _kEmptyChildList;
            }
        }
        else
        {
            childrenInTraversalOrder__154661 = _childrenIdInTraversalOrder();
            childrenInHitTestOrder__154707 = _childrenIdInHitTestOrder();
        }
        Int32List? customSemanticsActionIds__155993 = default!;
        if (((((long?)(((SemanticsData)data__154426).customSemanticsActionIds?.Count)) is { } __count156027 ? __count156027 != 0 : (bool?)null) ?? false))
        {
            customSemanticsActionIds__155993 = new Int32List(checked((long)(((SemanticsData)data__154426).customSemanticsActionIds!.Count)));
            for (var i__156179 = 0L; (i__156179 < checked((long)(((SemanticsData)data__154426).customSemanticsActionIds!.Count))); i__156179++)
            {
                customSemanticsActionIds__155993[i__156179] = checked((int)(((SemanticsData)data__154426).customSemanticsActionIds![(int)(i__156179)]));
                customSemanticsActionIdsUpdate.Add(((SemanticsData)data__154426).customSemanticsActionIds![(int)(i__156179)]);
            }
        }
        var traversalParentId__156411 = -1L;
        if (((SemanticsData)data__154426).traversalChildIdentifier is object identifier__156491)
        {
            if (this.owner!._traversalParentNodes.GetValueOrDefault(identifier__156491) is SemanticsNode parentNode__156582)
            {
                traversalParentId__156411 = ((SemanticsNode)parentNode__156582).id;
            }
        }
        object? childIdentifier__156672 = this.traversalChildIdentifier;
        if ((childIdentifier__156672 is not null))
        {
            traversalParent = this.owner!._traversalParentNodes.GetValueOrDefault(childIdentifier__156672);
            if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb)
            {
                _traversalChildTransform = _computeTraversalTransform(parent: this.traversalParent!, child: this);
            }
        }
        builder.updateNode(id: this.id, flags: ((SemanticsData)data__154426).flagsCollection, actions: ((SemanticsData)data__154426).actions, rect: ((SemanticsData)data__154426).rect, identifier: ((SemanticsData)data__154426).identifier, label: ((SemanticsData)data__154426).attributedLabel.@string, labelAttributes: ((SemanticsData)data__154426).attributedLabel.attributes, value: ((SemanticsData)data__154426).attributedValue.@string, valueAttributes: ((SemanticsData)data__154426).attributedValue.attributes, increasedValue: ((SemanticsData)data__154426).attributedIncreasedValue.@string, increasedValueAttributes: ((SemanticsData)data__154426).attributedIncreasedValue.attributes, decreasedValue: ((SemanticsData)data__154426).attributedDecreasedValue.@string, decreasedValueAttributes: ((SemanticsData)data__154426).attributedDecreasedValue.attributes, hint: ((SemanticsData)data__154426).attributedHint.@string, hintAttributes: ((SemanticsData)data__154426).attributedHint.attributes, tooltip: ((SemanticsData)data__154426).tooltip, textDirection: ((SemanticsData)data__154426).textDirection, textSelectionBase: ((((SemanticsData)data__154426).textSelection is not null) ? ((SemanticsData)data__154426).textSelection!.baseOffset : -1L), textSelectionExtent: ((((SemanticsData)data__154426).textSelection is not null) ? ((SemanticsData)data__154426).textSelection!.extentOffset : -1L), platformViewId: (((SemanticsData)data__154426).platformViewId ?? -1L), maxValueLength: (((SemanticsData)data__154426).maxValueLength ?? -1L), currentValueLength: (((SemanticsData)data__154426).currentValueLength ?? -1L), scrollChildren: (((SemanticsData)data__154426).scrollChildCount ?? 0L), scrollIndex: (((SemanticsData)data__154426).scrollIndex ?? 0L), scrollPosition: (((SemanticsData)data__154426).scrollPosition ?? double.NaN), scrollExtentMax: (((SemanticsData)data__154426).scrollExtentMax ?? double.NaN), scrollExtentMin: (((SemanticsData)data__154426).scrollExtentMin ?? double.NaN), transform: ((this._traversalTransform ?? _kIdentityTransform)).storage, traversalParent: traversalParentId__156411, hitTestTransform: ((((SemanticsData)data__154426).transform ?? _kIdentityTransform)).storage, childrenInTraversalOrder: childrenInTraversalOrder__154661, childrenInHitTestOrder: childrenInHitTestOrder__154707, additionalActions: (customSemanticsActionIds__155993 ?? _kEmptyCustomSemanticsActionsList), headingLevel: ((SemanticsData)data__154426).headingLevel, linkUrl: (((SemanticsData)data__154426).linkUrl?.ToString() ?? ""), role: ((SemanticsData)data__154426).role, controlsNodes: ((SemanticsData)data__154426).controlsNodes?.ToList(), validationResult: ((SemanticsData)data__154426).validationResult, hitTestBehavior: ((SemanticsData)data__154426).hitTestBehavior, inputType: ((SemanticsData)data__154426).inputType, locale: ((SemanticsData)data__154426).locale, minValue: (((SemanticsData)data__154426).minValue ?? ""), maxValue: (((SemanticsData)data__154426).maxValue ?? ""));
        _dirty = false;
    }

    internal virtual List<SemanticsNode>? _updateChildrenInTraversalOrder()
    {
        if (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb)
        {
            return this._children;
        }
        var updatedChildren__159815 = new List<SemanticsNode>();
        foreach (SemanticsNode child__159881 in this._children!)
        {
            if ((((SemanticsNode)child__159881)._isTraversalChild && !this._isTraversalParent))
            {
                SemanticsNode? traversalParent__160618 = this.owner!._traversalParentNodes.GetValueOrDefault(DartRuntimePrimitives.RequireReference(child__159881.getSemanticsData().traversalChildIdentifier));
                long? traversalParentId__160748 = traversalParent__160618?.id;
                while ((traversalParent__160618 is not null))
                {
                    if ((object.Equals(traversalParent__160618, child__159881)))
                    {
                        throw new FlutterError($"The traversalParent__160618 {traversalParentId__160748} cannot be the child of the traversalChild {((SemanticsNode)child__159881).id} in hit-test order");
                    }
                    traversalParent__160618 = ((SemanticsNode)traversalParent__160618).parent;
                }
                continue;
            }
            updatedChildren__159815.Add(child__159881);
        }
        if (this._isTraversalParent)
        {
            HashSet<SemanticsNode>? traversalChildren__161600 = this.owner?._traversalChildNodes.GetValueOrDefault(this.traversalParentIdentifier!);
            if ((traversalChildren__161600 is not null))
            {
                var currentNode__161936 = this;
                while ((((SemanticsNode)currentNode__161936).parent is not null))
                {
                    currentNode__161936 = ((SemanticsNode)currentNode__161936).parent!;
                    if (traversalChildren__161600.Contains(currentNode__161936))
                    {
                        throw new FlutterError($"The traversalParent {this.id} cannot be the child of the traversalChild {((SemanticsNode)currentNode__161936).id} in hit-test order");
                    }
                }
                foreach (SemanticsNode node__162324 in traversalChildren__161600)
                {
                    if (((SemanticsNode)node__162324).attached)
                    {
                        updatedChildren__159815.Add(node__162324);
                    }
                }
            }
        }
        return updatedChildren__159815;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<SemanticsNode> _childrenInTraversalOrder()
    {
        List<SemanticsNode>? updatedChildren__162656 = _updateChildrenInTraversalOrder();
        global::Doroti.Flutter.Ui.TextDirection? inheritedTextDirection__162729 = this.textDirection;
        SemanticsNode? ancestor__162788 = this.parent;
        while (((inheritedTextDirection__162729 is null) && (ancestor__162788 is not null)))
        {
            inheritedTextDirection__162729 = ((SemanticsNode)ancestor__162788).textDirection;
            ancestor__162788 = ((SemanticsNode)ancestor__162788).parent;
        }
        List<SemanticsNode>? childrenInDefaultOrder__162993 = default!;
        if ((inheritedTextDirection__162729 is not null))
        {
            TextDirection inheritedTextDirection__162729__value163025 = DartRuntimePrimitives.RequireValue(inheritedTextDirection__162729);
            childrenInDefaultOrder__162993 = SemanticsLibrary._childrenInDefaultOrder(updatedChildren__162656!, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(inheritedTextDirection__162729__value163025)));
        }
        else
        {
            childrenInDefaultOrder__162993 = updatedChildren__162656;
        }
        var everythingSorted__163606 = new List<_TraversalSortNode__semantics>();
        var sortNodes__163659 = new List<_TraversalSortNode__semantics>();
        SemanticsSortKey? lastSortKey__163717 = default!;
        for (var position__163743 = 0L; (position__163743 < checked((long)(childrenInDefaultOrder__162993!.Count))); position__163743 += 1L)
        {
            SemanticsNode child__163843 = childrenInDefaultOrder__162993[(int)(position__163743)];
            SemanticsSortKey? sortKey__163915 = ((SemanticsNode)child__163843).sortKey;
            lastSortKey__163717 = ((position__163743 > 0L) ? childrenInDefaultOrder__162993[(int)((position__163743 - 1L))].sortKey : null);
            bool isCompatibleWithPreviousSortKey__164045 = ((position__163743 == 0L) || ((object.Equals(DartRuntimePrimitives.RuntimeType(sortKey__163915), DartRuntimePrimitives.RuntimeType(lastSortKey__163717))) && (((sortKey__163915 is null) || (((SemanticsSortKey)sortKey__163915).name == lastSortKey__163717!.ToString())))));
            if ((!isCompatibleWithPreviousSortKey__164045 && (checked((long)(sortNodes__163659.Count)) != 0)))
            {
                if ((lastSortKey__163717 is not null))
                {
                    sortNodes__163659.sort();
                }
                everythingSorted__163606.AddRange(sortNodes__163659);
                sortNodes__163659.Clear();
            }
            sortNodes__163659.Add(new _TraversalSortNode__semantics(node: child__163843, sortKey: sortKey__163915, position: position__163743));
        }
        if ((lastSortKey__163717 is not null))
        {
            sortNodes__163659.sort();
        }
        everythingSorted__163606.AddRange(sortNodes__163659);
        return everythingSorted__163606.map<_TraversalSortNode__semantics, SemanticsNode>(((sortNode) => ((_TraversalSortNode__semantics)sortNode).node)).ToList();
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
        var result__166089 = false;
        DartRuntimePrimitives.Assert(() =>
            {
                result__166089 = ((this._effectiveActionsAsBits & (long)action) == 0L);
                return true;
            });
        return result__166089;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string toStringShort() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "SemanticsNode"))}#{this.id}";
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        var hideOwner__166460 = true;
        if (this._dirty)
        {
            bool inDirtyNodes__166513 = ((this.owner is not null) && this.owner!._dirtyNodes.Contains(this));
            properties.add(new FlagProperty("inDirtyNodes", value: inDirtyNodes__166513, ifTrue: "dirty", ifFalse: "STALE"));
            hideOwner__166460 = inDirtyNodes__166513;
        }
        properties.add(new DiagnosticsProperty<SemanticsOwner>("owner", this.owner, level: (hideOwner__166460 ? DiagnosticLevel.hidden : DiagnosticLevel.info)));
        properties.add(new FlagProperty("isMergedIntoParent", value: this.isMergedIntoParent, ifTrue: "merged up ⬆️"));
        properties.add(new FlagProperty("mergeAllDescendantsIntoThisNode", value: this.mergeAllDescendantsIntoThisNode, ifTrue: "merge boundary ⛔️"));
        if ((this._locale is not null))
        {
            properties.add(new StringProperty("locale", this._locale.ToString()));
        }
        global::Doroti.Flutter.Ui.Offset? offset__167351 = ((this.transform is not null) ? MatrixUtils.getAsTranslation(this.transform!) : null);
        if ((offset__167351 is not null))
        {
            Offset offset__167351__value167437 = DartRuntimePrimitives.RequireValue(offset__167351);
            properties.add(new DiagnosticsProperty<global::Doroti.Flutter.Ui.Rect>("rect", this.rect.shift(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(offset__167351__value167437))), showName: false));
        }
        else
        {
            double? scale__167582 = ((this.transform is not null) ? MatrixUtils.getAsScale(this.transform!) : null);
            string? description__167667 = default!;
            if ((scale__167582 is not null))
            {
                double scale__167582__value167690 = DartRuntimePrimitives.RequireValue(scale__167582);
                description__167667 = $"{this.rect} scaled by {DartRuntimePrimitives.RequireValue(scale__167582__value167690).toStringAsFixed(1L)}x";
            }
            else
            {
                if (((this.transform is not null) && !MatrixUtils.isIdentity(this.transform!)))
                {
                    string matrix__167875 = string.Join("; ", this.transform.ToString().split("\n").take(4L).map<string, string>(((line) => line.substring(4L))));
                    description__167667 = $"{this.rect} with transform [{matrix__167875}]";
                }
            }
            properties.add(new DiagnosticsProperty<global::Doroti.Flutter.Ui.Rect>("rect", this.rect, description: description__167667, showName: false));
        }
        properties.add(new IterableProperty<string>("tags", this.tags?.map<SemanticsTag, string>(((tag) => ((SemanticsTag)tag).name)), defaultValue: null));
        List<string> actions__168429 = ((Func<List<string>>)(() =>
{
    var __cascade = this._actions.Keys.map<SemanticsAction, string>(((action) => $"{action.ToString()}{(_debugIsActionBlocked(action) ? "🚫️" : "")}")).ToList();
    __cascade.sort();
    return __cascade;
}))();
        List<string?> customSemanticsActions__168690 = this._customSemanticsActions.Keys.map<CustomSemanticsAction, string?>(((action) => ((CustomSemanticsAction)action).label)).ToList();
        properties.add(new IterableProperty<string>("actions", actions__168429, ifEmpty: null));
        properties.add(new IterableProperty<string?>("customActions", customSemanticsActions__168690, ifEmpty: null));
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
        properties.add(new EnumProperty<global::Doroti.Flutter.Ui.TextDirection>("textDirection", this._textDirection, defaultValue: null));
        if ((!object.Equals(this._role, SemanticsRole.none)))
        {
            properties.add(new EnumProperty<global::Doroti.Flutter.Ui.SemanticsRole>("role", this._role));
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
            properties.add(new EnumProperty<global::Doroti.Flutter.Ui.SemanticsInputType>("inputType", this._inputType));
        }
        if ((!object.Equals(this.validationResult, SemanticsValidationResult.none)))
        {
            properties.add(new EnumProperty<global::Doroti.Flutter.Ui.SemanticsValidationResult>("validationResult", this.validationResult, defaultValue: SemanticsValidationResult.none));
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
        var edges__176589 = new List<_BoxEdge__semantics>();
        foreach (SemanticsNode child__176640 in this.nodes)
        {
            global::Doroti.Flutter.Ui.Rect childRect__176753 = ((SemanticsNode)child__176640).rect.deflate(0.1);
            edges__176589.Add(new _BoxEdge__semantics(isLeadingEdge: true, offset: SemanticsLibrary._pointInParentCoordinates(child__176640, childRect__176753.topLeft).dx, node: child__176640));
            edges__176589.Add(new _BoxEdge__semantics(isLeadingEdge: false, offset: SemanticsLibrary._pointInParentCoordinates(child__176640, childRect__176753.bottomRight).dx, node: child__176640));
        }
        edges__176589.sort();
        var horizontalGroups__177194 = new List<_SemanticsSortGroup__semantics>();
        _SemanticsSortGroup__semantics? group__177263 = default!;
        var depth__177278 = 0L;
        foreach (var edge__177304 in edges__176589)
        {
            if (((_BoxEdge__semantics)edge__177304).isLeadingEdge)
            {
                depth__177278 += 1L;
                group__177263 ??= new _SemanticsSortGroup__semantics(startOffset: ((_BoxEdge__semantics)edge__177304).offset, textDirection: this.textDirection);
                ((_SemanticsSortGroup__semantics)group__177263).nodes.Add(((_BoxEdge__semantics)edge__177304).node);
            }
            else
            {
                depth__177278 -= 1L;
            }
            if ((depth__177278 == 0L))
            {
                horizontalGroups__177194.Add(group__177263!);
                group__177263 = null;
            }
        }
        horizontalGroups__177194.sort();
        if ((object.Equals(this.textDirection, TextDirection.rtl)))
        {
            horizontalGroups__177194 = System.Linq.Enumerable.Reverse(horizontalGroups__177194).ToList();
        }
        return horizontalGroups__177194.expand(((group) => group.sortedWithinKnot())).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<SemanticsNode> sortedWithinKnot()
    {
        if ((checked((long)(this.nodes.Count)) <= 1L))
        {
            return this.nodes;
        }
        var nodeMap__178896 = new DartMap<long, SemanticsNode>();
        var edges__178940 = new DartMap<long, long>();
        foreach (SemanticsNode node__178991 in this.nodes)
        {
            nodeMap__178896[((SemanticsNode)node__178991).id] = node__178991;
            global::Doroti.Flutter.Ui.Offset center__179058 = SemanticsLibrary._pointInParentCoordinates(node__178991, ((SemanticsNode)node__178991).rect.center);
            foreach (SemanticsNode nextNode__179149 in this.nodes)
            {
                if ((DartRuntimePrimitives.Identical(node__178991, nextNode__179149) || (edges__178940.GetValueOrDefault(((SemanticsNode)nextNode__179149).id) == ((SemanticsNode)node__178991).id)))
                {
                    continue;
                }
                global::Doroti.Flutter.Ui.Offset nextCenter__179409 = SemanticsLibrary._pointInParentCoordinates(nextNode__179149, ((SemanticsNode)nextNode__179149).rect.center);
                global::Doroti.Flutter.Ui.Offset centerDelta__179502 = (nextCenter__179409 - center__179058);
                double direction__179610 = centerDelta__179502.direction;
                bool isLtrAndForward__179664 = (((object.Equals(this.textDirection, TextDirection.ltr)) && ((-Dart_mathLibrary.pi / 4L) < direction__179610)) && (direction__179610 < ((3L * Dart_mathLibrary.pi) / 4L)));
                bool isRtlAndForward__179832 = ((object.Equals(this.textDirection, TextDirection.rtl)) && (((direction__179610 < ((-3L * Dart_mathLibrary.pi) / 4L)) || (direction__179610 > ((3L * Dart_mathLibrary.pi) / 4L)))));
                if ((isLtrAndForward__179664 || isRtlAndForward__179832))
                {
                    edges__178940[((SemanticsNode)node__178991).id] = ((SemanticsNode)nextNode__179149).id;
                }
            }
        }
        var sortedIds__180100 = new List<long>();
        var visitedIds__180131 = new HashSet<long>();
        List<SemanticsNode> startNodes__180183 = ((Func<List<SemanticsNode>>)(() =>
{
    var __cascade = this.nodes.ToList();
    __cascade.sort(((a, b) =>
    {
        global::Doroti.Flutter.Ui.Offset aTopLeft__180282 = SemanticsLibrary._pointInParentCoordinates(a, ((SemanticsNode)a).rect.topLeft);
        global::Doroti.Flutter.Ui.Offset bTopLeft__180360 = SemanticsLibrary._pointInParentCoordinates(b, ((SemanticsNode)b).rect.topLeft);
        long verticalDiff__180435 = aTopLeft__180282.dy.CompareTo(bTopLeft__180360.dy);
        if ((verticalDiff__180435 != 0L))
        {
            return -verticalDiff__180435;
        }
        return -aTopLeft__180282.dx.CompareTo(bTopLeft__180360.dx);
        return default;
    }));
    return __cascade;
}))();
        void search(long id)
        {
            if (visitedIds__180131.Contains(id))
            {
                return;
            }
            visitedIds__180131.Add(id);
            if (edges__178940.ContainsKey(id))
            {
                search(DartRuntimePrimitives.RequireValue(edges__178940.GetValueOrDefault(id)));
            }
            sortedIds__180100.Add(id);
        }
        startNodes__180183.map<SemanticsNode, long>(((node) => ((SemanticsNode)node).id)).forEach(search);
        return System.Linq.Enumerable.Reverse(sortedIds__180100.map<long, SemanticsNode>(((id) => nodeMap__178896.GetValueOrDefault(id)!)).ToList()).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public int CompareTo(_SemanticsSortGroup__semantics? other) => checked((int)compareTo(other!));
}

public static partial class SemanticsLibrary
{
    internal static Offset _pointInParentCoordinates(SemanticsNode node, Offset point)
    {
        Matrix4? traversalTransform__181166 = ((SemanticsNode)node)._traversalTransform;
        if ((traversalTransform__181166 is null))
        {
            return point;
        }
        var vector__181279 = new Vector3(point.dx, point.dy, 0.0);
        traversalTransform__181166.transform3(vector__181279);
        return new global::Doroti.Flutter.Ui.Offset(vector__181279.x, vector__181279.y);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class SemanticsLibrary
{
    internal static List<SemanticsNode> _childrenInDefaultOrder(List<SemanticsNode> children, TextDirection textDirection)
    {
        var edges__182036 = new List<_BoxEdge__semantics>();
        foreach (var child__182071 in children)
        {
            DartRuntimePrimitives.Assert(() => ((SemanticsNode)child__182071).rect.isFinite);
            global::Doroti.Flutter.Ui.Rect childRect__182216 = ((SemanticsNode)child__182071).rect.deflate(0.1);
            edges__182036.Add(new _BoxEdge__semantics(isLeadingEdge: true, offset: SemanticsLibrary._pointInParentCoordinates(child__182071, childRect__182216.topLeft).dy, node: child__182071));
            edges__182036.Add(new _BoxEdge__semantics(isLeadingEdge: false, offset: SemanticsLibrary._pointInParentCoordinates(child__182071, childRect__182216.bottomRight).dy, node: child__182071));
        }
        edges__182036.sort();
        var verticalGroups__182625 = new List<_SemanticsSortGroup__semantics>();
        _SemanticsSortGroup__semantics? group__182690 = default!;
        var depth__182703 = 0L;
        foreach (var edge__182727 in edges__182036)
        {
            if (((_BoxEdge__semantics)edge__182727).isLeadingEdge)
            {
                depth__182703 += 1L;
                group__182690 ??= new _SemanticsSortGroup__semantics(startOffset: ((_BoxEdge__semantics)edge__182727).offset, textDirection: textDirection);
                ((_SemanticsSortGroup__semantics)group__182690).nodes.Add(((_BoxEdge__semantics)edge__182727).node);
            }
            else
            {
                depth__182703 -= 1L;
            }
            if ((depth__182703 == 0L))
            {
                verticalGroups__182625.Add(group__182690!);
                group__182690 = null;
            }
        }
        verticalGroups__182625.sort();
        return verticalGroups__182625.expand(((group) => group.sortedWithinVerticalGroup())).ToList();
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
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
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
                var invisibleNodes__186389 = new List<SemanticsNode>();
                bool findInvisibleNodes(SemanticsNode node)
                {
                    if (((SemanticsNode)node).rect.isEmpty)
                    {
                        invisibleNodes__186389.Add(node);
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
                SemanticsNode? rootSemanticsNode__186915 = this.rootSemanticsNode;
                if ((rootSemanticsNode__186915 is not null))
                {
                    if (((((SemanticsNode)rootSemanticsNode__186915).childrenCount > 0L) && ((SemanticsNode)rootSemanticsNode__186915).rect.isEmpty))
                    {
                        invisibleNodes__186389.Add(rootSemanticsNode__186915);
                    }
                    else
                    {
                        if (!((SemanticsNode)rootSemanticsNode__186915).mergeAllDescendantsIntoThisNode)
                        {
                            rootSemanticsNode__186915.visitChildren((Func<SemanticsNode, bool>)findInvisibleNodes);
                        }
                    }
                }
                if ((checked((long)(invisibleNodes__186389.Count)) == 0))
                {
                    return true;
                }
                List<DiagnosticsNode> nodeToMessage(SemanticsNode invisibleNode)
                {
                    SemanticsNode? parent__187532 = ((SemanticsNode)invisibleNode).parent;
                    return new List<DiagnosticsNode> { ((Diagnosticable)invisibleNode).toDiagnosticsNode(style: DiagnosticsTreeStyle.errorProperty), (((Diagnosticable)parent__187532).toDiagnosticsNode(name: "which was added as a child of", style: DiagnosticsTreeStyle.errorProperty) ?? new ErrorDescription("which was added as the root SemanticsNode")) };
                    throw new InvalidOperationException("Dart control flow completed without a value.");
                }
                throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Invisible SemanticsNodes should not be added to the tree."), new ErrorDescription("The following invisible SemanticsNodes were added to the tree:"), new ErrorHint("An invisible SemanticsNode is one whose rect is not on screen hence not reachable for users, " + "and its semantic information is not merged into a visible parent."), new ErrorHint("An invisible SemanticsNode makes the accessibility experience confusing, " + "as it does not provide any visual indication when the user selects it " + "via accessibility technologies."), new ErrorHint("Consider removing the above invisible SemanticsNodes if they were added by your " + "RenderObject.assembleSemanticsNode implementation, or filing a bug on GitHub:\n" + "  https://github.com/flutter/flutter/issues/new?template=02_bug.yml") });
            });
        if ((checked((long)(this._dirtyNodes.Count)) == 0))
        {
            return;
        }
        var customSemanticsActionIds__189065 = new HashSet<long>();
        var visitedNodes__189111 = new List<SemanticsNode>();
        while ((checked((long)(this._dirtyNodes.Count)) != 0))
        {
            List<SemanticsNode> localDirtyNodes__189214 = this._dirtyNodes.where(((node) => !this._detachedNodes.Contains(node))).ToList();
            this._dirtyNodes.Clear();
            this._detachedNodes.Clear();
            localDirtyNodes__189214.sort(((a, b) => (((SemanticsNode)a).depth - ((SemanticsNode)b).depth)));
            visitedNodes__189111.AddRange(localDirtyNodes__189214);
            foreach (var node__189541 in localDirtyNodes__189214)
            {
                DartRuntimePrimitives.Assert(() => ((SemanticsNode)node__189541)._dirty);
                DartRuntimePrimitives.Assert(() => (((((SemanticsNode)node__189541).parent is null) || !((SemanticsNode)node__189541).parent!.isPartOfNodeMerging) || ((SemanticsNode)node__189541).isMergedIntoParent));
                if (((SemanticsNode)node__189541).isPartOfNodeMerging)
                {
                    DartRuntimePrimitives.Assert(() => (((SemanticsNode)node__189541).mergeAllDescendantsIntoThisNode || (((SemanticsNode)node__189541).parent is not null)));
                    if (((((SemanticsNode)node__189541).parent is not null) && ((SemanticsNode)node__189541).parent!.isPartOfNodeMerging))
                    {
                        ((SemanticsNode)node__189541).parent!._markDirty();
                        node__189541._dirty = false;
                    }
                }
                this._traversalParentNodes.removeWhere(((key, oldNode) => (object.Equals(node__189541, oldNode))));
                foreach (HashSet<SemanticsNode> childSet__190522 in this._traversalChildNodes.Values)
                {
                    childSet__190522.removeWhere(((oldNode) => (object.Equals(node__189541, oldNode))));
                }
                this._traversalChildNodes.removeWhere(((key, value) => (checked((long)(value.Count)) == 0)));
                bool isTraversalParent__190769 = ((SemanticsNode)node__189541)._isTraversalParent;
                bool isTraversalChild__190833 = ((SemanticsNode)node__189541)._isTraversalChild;
                if (isTraversalParent__190769)
                {
                    DartRuntimePrimitives.Assert(() => (!this._traversalParentNodes.ContainsKey(((SemanticsNode)node__189541)._traversalParentIdentifier) || (object.Equals(this._traversalParentNodes.GetValueOrDefault(((SemanticsNode)node__189541).traversalParentIdentifier!), node__189541))));
                    this._traversalParentNodes[((SemanticsNode)node__189541).traversalParentIdentifier!] = node__189541;
                }
                else
                {
                    if (isTraversalChild__190833)
                    {
                        this._traversalChildNodes.putIfAbsent(((SemanticsNode)node__189541).traversalChildIdentifier!, (() => new HashSet<SemanticsNode>())).Add(node__189541);
                    }
                }
                if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb)
                {
                    if (((SemanticsNode)node__189541)._isTraversalChild)
                    {
                        SemanticsNode? parentNode__192289 = this._traversalParentNodes.GetValueOrDefault(DartRuntimePrimitives.RequireReference(((SemanticsNode)node__189541).traversalChildIdentifier));
                        if (((parentNode__192289 is not null) && !visitedNodes__189111.Contains(parentNode__192289)))
                        {
                            parentNode__192289._markDirty();
                        }
                    }
                }
            }
        }
        visitedNodes__189111.sort(((a, b) => (((SemanticsNode)a).depth - ((SemanticsNode)b).depth)));
        global::Doroti.Flutter.Ui.SemanticsUpdateBuilder builder__192635 = global::Doroti.Generated.Framework.Semantics.SemanticsBinding.instance.createSemanticsUpdateBuilder();
        foreach (var node__192719 in visitedNodes__189111)
        {
            DartRuntimePrimitives.Assert(() => (((SemanticsNode)node__192719).parent?._dirty != true));
            if ((((SemanticsNode)node__192719)._dirty && ((SemanticsNode)node__192719).attached))
            {
                node__192719._addToUpdate(builder__192635, customSemanticsActionIds__189065);
            }
        }
        this._dirtyNodes.Clear();
        foreach (var actionId__193907 in customSemanticsActionIds__189065)
        {
            CustomSemanticsAction action__193981 = CustomSemanticsAction.getAction(actionId__193907)!;
            builder__192635.updateCustomAction(id: actionId__193907, label: ((CustomSemanticsAction)action__193981).label, hint: ((CustomSemanticsAction)action__193981).hint, overrideId: (FoundationRuntimePorts.EnumIndexNullable(((CustomSemanticsAction)action__193981).action) ?? -1L));
        }
        this.onSemanticsUpdate(builder__192635.build());
        notifyListeners();
    }

    internal virtual Action<object?>? _getSemanticsActionHandlerForId(long id, SemanticsAction action, object? args = null)
    {
        SemanticsNode? result__194422 = this._nodes.GetValueOrDefault(id);
        if ((result__194422 is null))
        {
            return null;
        }
        if ((((SemanticsNode)result__194422).isPartOfNodeMerging && !result__194422._canHandleAction(action, args)))
        {
            SemanticsNode? found__194876 = default!;
            result__194422._visitDescendants(((Func<SemanticsNode, bool>)((node) =>
            {
                if (node._canHandleAction(action, args))
                {
                    found__194876 = node;
                    return false;
                }
                return true;
                return default;
            })));
            result__194422 = found__194876;
        }
        if (((result__194422 is null) || !result__194422._canHandleAction(action, args)))
        {
            return null;
        }
        return ((SemanticsNode)result__194422)._actions.GetValueOrDefault(action);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void performAction(long id, SemanticsAction action, object? args = null)
    {
        Action<object?>? handler__195695 = _getSemanticsActionHandlerForId(id, action, args);
        if ((handler__195695 is not null))
        {
            handler__195695(args);
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
            var inverse__196223 = Matrix4.identity();
            if ((inverse__196223.copyInverse(((SemanticsNode)node).transform!) == 0.0))
            {
                return null;
            }
            position = MatrixUtils.transformPoint(inverse__196223, position);
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
            SemanticsNode? result__196639 = default!;
            node._visitDescendants(((Func<SemanticsNode, bool>)((child) =>
            {
                if (child._canHandleAction(action, args))
                {
                    result__196639 = child;
                    return false;
                }
                return true;
                return default;
            })));
            return result__196639?._actions.GetValueOrDefault(action);
        }
        if (((SemanticsNode)node).hasChildren)
        {
            foreach (SemanticsNode child__196947 in System.Linq.Enumerable.Reverse(((SemanticsNode)node)._children!))
            {
                Action<object?>? handler__197022 = _getSemanticsActionHandlerForPosition(child__196947, position, action, args);
                if ((handler__197022 is not null))
                {
                    return handler__197022;
                }
            }
        }
        return ((SemanticsNode)node)._actions.GetValueOrDefault(action);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void performActionAt(Offset position, SemanticsAction action, object? args = null)
    {
        SemanticsNode? node__197692 = this.rootSemanticsNode;
        if ((node__197692 is null))
        {
            return;
        }
        Action<object?>? handler__197796 = _getSemanticsActionHandlerForPosition(node__197692, position, action, args);
        if ((handler__197796 is not null))
        {
            handler__197796(args);
        }
    }

    public override string ToString() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
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
    public virtual global::Doroti.Flutter.Ui.Locale? localeForSubtree
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
                var list__210415 = ((Float64List?)(object?)args!)!;
                __value!(new global::Doroti.Flutter.Ui.Offset(list__210415[0L], list__210415[1L]));
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
                var extendSelection__214657 = ((bool)args!);
                __value!(extendSelection__214657);
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
                var extendSelection__215431 = ((bool)args!);
                __value!(extendSelection__215431);
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
                var extendSelection__216165 = ((bool)args!);
                __value!(extendSelection__216165);
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
                var extendSelection__216904 = ((bool)args!);
                __value!(extendSelection__216904);
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
                DartMap<string, long> selection__217703 = (DartRuntimePrimitives.ConvertMap<object, object>((System.Collections.IDictionary)args!)).cast<string, long>();
                DartRuntimePrimitives.Assert(() => ((selection__217703.ContainsKey("base")) && (selection__217703.ContainsKey("extent"))));
                __value!(new TextSelection(baseOffset: DartRuntimePrimitives.RequireValue(selection__217703.GetValueOrDefault("base")), extentOffset: DartRuntimePrimitives.RequireValue(selection__217703.GetValueOrDefault("extent"))));
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
                var text__218528 = ((string?)(object?)args!)!;
                __value!(text__218528);
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
        CustomSemanticsAction? action__228295 = CustomSemanticsAction.getAction(((long)args!));
        if ((action__228295 is null))
        {
            return;
        }
        Action? callback__228421 = this._customSemanticsActions.GetValueOrDefault(action__228295);
        if ((callback__228421 is not null))
        {
            callback__228421();
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
    public virtual global::Doroti.Flutter.Ui.SemanticsRole role
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
    public virtual global::Doroti.Flutter.Ui.TextDirection? textDirection
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
    public virtual global::Doroti.Flutter.Ui.SemanticsValidationResult validationResult
    {
        get => this._validationResult;
        set
        {
            var __value = value;
            _validationResult = DartRuntimePrimitives.RequireValue(__value);
            _hasBeenAnnotated = true;
        }
    }
    public virtual global::Doroti.Flutter.Ui.SemanticsHitTestBehavior hitTestBehavior
    {
        get => this._hitTestBehavior;
        set
        {
            var __value = value;
            _hitTestBehavior = DartRuntimePrimitives.RequireValue(__value);
            _hasBeenAnnotated = true;
        }
    }
    public virtual global::Doroti.Flutter.Ui.SemanticsInputType inputType
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
            if (((((((this._flags.isTextField || ((this._flags.isHeader && global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb))) || this._flags.isSlider) || this._flags.isLink) || this._flags.scopesRoute) || this._flags.isImage) || this._flags.isKeyboardKey))
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
            AttributedString directionEmbedding__266744 = (DartRuntimePrimitives.RequireValue(otherTextDirection__value266687) switch { TextDirection.rtl => new AttributedString(Unicode.RLE), TextDirection.ltr => new AttributedString(Unicode.LRE), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            otherAttributedString = ((directionEmbedding__266744.op_Add(otherAttributedString)).op_Add(new AttributedString(Unicode.PDF)));
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
        var bitmask__272639 = 0L;
        if ((!object.Equals(flags.isChecked, CheckedState.none)))
        {
            bitmask__272639 |= (1L << (int)(0L));
        }
        if ((object.Equals(flags.isChecked, CheckedState.isTrue)))
        {
            bitmask__272639 |= (1L << (int)(1L));
        }
        if ((object.Equals(flags.isSelected, Tristate.isTrue)))
        {
            bitmask__272639 |= (1L << (int)(2L));
        }
        if (flags.isButton)
        {
            bitmask__272639 |= (1L << (int)(3L));
        }
        if (flags.isTextField)
        {
            bitmask__272639 |= (1L << (int)(4L));
        }
        if ((object.Equals(flags.isFocused, Tristate.isTrue)))
        {
            bitmask__272639 |= (1L << (int)(5L));
        }
        if ((!object.Equals(flags.isEnabled, Tristate.none)))
        {
            bitmask__272639 |= (1L << (int)(6L));
        }
        if ((object.Equals(flags.isEnabled, Tristate.isTrue)))
        {
            bitmask__272639 |= (1L << (int)(7L));
        }
        if (flags.isInMutuallyExclusiveGroup)
        {
            bitmask__272639 |= (1L << (int)(8L));
        }
        if (flags.isHeader)
        {
            bitmask__272639 |= (1L << (int)(9L));
        }
        if (flags.isObscured)
        {
            bitmask__272639 |= (1L << (int)(10L));
        }
        if (flags.scopesRoute)
        {
            bitmask__272639 |= (1L << (int)(11L));
        }
        if (flags.namesRoute)
        {
            bitmask__272639 |= (1L << (int)(12L));
        }
        if (flags.isHidden)
        {
            bitmask__272639 |= (1L << (int)(13L));
        }
        if (flags.isImage)
        {
            bitmask__272639 |= (1L << (int)(14L));
        }
        if (flags.isLiveRegion)
        {
            bitmask__272639 |= (1L << (int)(15L));
        }
        if ((!object.Equals(flags.isToggled, Tristate.none)))
        {
            bitmask__272639 |= (1L << (int)(16L));
        }
        if ((object.Equals(flags.isToggled, Tristate.isTrue)))
        {
            bitmask__272639 |= (1L << (int)(17L));
        }
        if (flags.hasImplicitScrolling)
        {
            bitmask__272639 |= (1L << (int)(18L));
        }
        if (flags.isMultiline)
        {
            bitmask__272639 |= (1L << (int)(19L));
        }
        if (flags.isReadOnly)
        {
            bitmask__272639 |= (1L << (int)(20L));
        }
        if ((!object.Equals(flags.isFocused, Tristate.none)))
        {
            bitmask__272639 |= (1L << (int)(21L));
        }
        if (flags.isLink)
        {
            bitmask__272639 |= (1L << (int)(22L));
        }
        if (flags.isSlider)
        {
            bitmask__272639 |= (1L << (int)(23L));
        }
        if (flags.isKeyboardKey)
        {
            bitmask__272639 |= (1L << (int)(24L));
        }
        if ((object.Equals(flags.isChecked, CheckedState.mixed)))
        {
            bitmask__272639 |= (1L << (int)(25L));
        }
        if ((!object.Equals(flags.isExpanded, Tristate.none)))
        {
            bitmask__272639 |= (1L << (int)(26L));
        }
        if ((object.Equals(flags.isExpanded, Tristate.isTrue)))
        {
            bitmask__272639 |= (1L << (int)(27L));
        }
        if ((!object.Equals(flags.isSelected, Tristate.none)))
        {
            bitmask__272639 |= (1L << (int)(28L));
        }
        if ((!object.Equals(flags.isRequired, Tristate.none)))
        {
            bitmask__272639 |= (1L << (int)(29L));
        }
        if ((object.Equals(flags.isRequired, Tristate.isTrue)))
        {
            bitmask__272639 |= (1L << (int)(30L));
        }
        return bitmask__272639;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
