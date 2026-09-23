// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/semantics/semantics.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Semantics;

public delegate bool SemanticsNodeVisitor(SemanticsNode node);

public delegate void MoveCursorHandler(bool extendSelection);

public delegate void SetSelectionHandler(TextSelection selection);

public delegate void SetTextHandler(string text);

public delegate void ScrollToOffsetHandler(Offset targetOffset);

public delegate void SemanticsActionHandler(object? args);

public delegate void SemanticsUpdateCallback(SemanticsUpdate update);

public delegate ChildSemanticsConfigurationsResult ChildSemanticsConfigurationsDelegate(
    List<SemanticsConfiguration> __unnamed_
);

public enum AccessibilityFocusBlockType
{
    none,
    blockSubtree,
    blockNode,
}

public static class AccessibilityFocusBlockTypeMembers
{
    internal static AccessibilityFocusBlockType _merge(
        this AccessibilityFocusBlockType value,
        AccessibilityFocusBlockType other
    )
    {
        if (
            Equals(value, AccessibilityFocusBlockType.blockSubtree)
            || Equals(other, AccessibilityFocusBlockType.blockSubtree)
        )
        {
            return AccessibilityFocusBlockType.blockSubtree;
        }
        if (
            Equals(value, AccessibilityFocusBlockType.blockNode)
            || Equals(other, AccessibilityFocusBlockType.blockNode)
        )
        {
            return AccessibilityFocusBlockType.blockNode;
        }
        return AccessibilityFocusBlockType.none;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class SemanticsLibrary
{
    internal static long _kUnblockedUserActions =
        (long)SemanticsAction.didGainAccessibilityFocus
        | (long)SemanticsAction.didLoseAccessibilityFocus;
}

internal abstract class _DebugSemanticsRoleChecks__semantics
{
    internal static FlutterError? _checkSemanticsData(SemanticsNode node)
    {
        FlutterError? error = (
            (Func<SemanticsNode, FlutterError?>)(
                node.role switch
                {
                    SemanticsRole.alertDialog => _noCheckRequired,
                    SemanticsRole.dialog => _noCheckRequired,
                    SemanticsRole.none => _noCheckRequired,
                    SemanticsRole.tab => _semanticsTab,
                    SemanticsRole.tabBar => _semanticsTabBar,
                    SemanticsRole.tabPanel => _noCheckRequired,
                    SemanticsRole.table => _semanticsTable,
                    SemanticsRole.cell => _semanticsCell,
                    SemanticsRole.row => _semanticsRow,
                    SemanticsRole.columnHeader => _semanticsColumnHeader,
                    SemanticsRole.radioGroup => _semanticsRadioGroup,
                    SemanticsRole.menu => _semanticsMenu,
                    SemanticsRole.menuBar => _semanticsMenuBar,
                    SemanticsRole.menuItem => _semanticsMenuItem,
                    SemanticsRole.menuItemCheckbox => _semanticsMenuItemCheckbox,
                    SemanticsRole.menuItemRadio => _semanticsMenuItemRadio,
                    SemanticsRole.alert => _noLiveRegion,
                    SemanticsRole.status => _noLiveRegion,
                    SemanticsRole.list => _noCheckRequired,
                    SemanticsRole.listItem => _semanticsListItem,
                    SemanticsRole.complementary => _semanticsComplementary,
                    SemanticsRole.contentInfo => _semanticsContentInfo,
                    SemanticsRole.main => _semanticsMain,
                    SemanticsRole.navigation => _semanticsNavigation,
                    SemanticsRole.region => _semanticsRegion,
                    SemanticsRole.form => _noCheckRequired,
                    SemanticsRole.loadingSpinner => _noCheckRequired,
                    SemanticsRole.progressBar => _semanticsProgressBar,
                    SemanticsRole.dragHandle => _unimplemented,
                    SemanticsRole.spinButton => _unimplemented,
                    SemanticsRole.comboBox => _unimplemented,
                    SemanticsRole.tooltip => _unimplemented,
                    SemanticsRole.hotKey => _unimplemented,
                    _ => throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
                }
            )
        )(node);
        if (error is not null)
        {
            return error;
        }
        return _semanticsGeneral(node);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static FlutterError? _unimplemented(SemanticsNode node) =>
        new FlutterError($"Missing checks for role {node.getSemanticsData().role}");

    internal static FlutterError? _noCheckRequired(SemanticsNode node) => null;

    internal static FlutterError? _semanticsProgressBar(SemanticsNode node)
    {
        SemanticsData data = node.getSemanticsData();
        if (
            (data.value.Length == 0)
            || ((data.minValue is null ? (bool?)null : data.minValue.Length == 0) ?? true)
            || ((data.maxValue is null ? (bool?)null : data.maxValue.Length == 0) ?? true)
        )
        {
            return new FlutterError("A progress bar must have a value, a minValue, a maxValue.");
        }
        double? minVal = Dart_coreLibrary.tryParse(data.minValue!);
        double? maxVal = Dart_coreLibrary.tryParse(data.maxValue!);
        double? currentValue = Dart_coreLibrary.tryParse(data.value);
        double? percentValue = data.value.endsWith("%")
            ? Dart_coreLibrary.tryParse(data.value.substring(0L, data.value.Length - 1L))
            : null;
        if (
            (minVal is null)
            || (maxVal is null)
            || ((currentValue is null) && (percentValue is null))
        )
        {
            return new FlutterError(
                "Progress bar value, minValue, and maxValue must be valid numbers. "
                    + $"value: \"{data.value}\", minValue: \"{data.minValue}\", maxValue: \"{data.maxValue}\""
            );
        }
        if (
            minVal
            >= (
                maxVal
                ?? throw new global::System.NullReferenceException("A required value was null.")
            )
        )
        {
            return new FlutterError(
                $"Progress bar minValue ({data.minValue}) must be less than maxValue ({data.maxValue})"
            );
        }
        if (currentValue is not null)
        {
            double currentValue__8479__value9301 = (
                currentValue
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            if (
                (
                    (currentValue__8479__value9301)
                    < (
                        minVal
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                )
                || (
                    (currentValue__8479__value9301)
                    > (
                        maxVal
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                )
            )
            {
                return new FlutterError(
                    $"Progress bar value ({data.value}) must be between minValue ({data.minValue}) and maxValue ({data.maxValue})"
                );
            }
            return null;
        }
        if (
            (percentValue is not null)
            && (
                (
                    (
                        percentValue
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ) < 0L
                )
                || (
                    (
                        percentValue
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ) > 100L
                )
            )
        )
        {
            double percentValue__8541__value9681 = (
                percentValue
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            return new FlutterError(
                $"Progress bar percentage value ({data.value}) must be between 0% and 100%"
            );
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static FlutterError? _semanticsTab(SemanticsNode node)
    {
        SemanticsData data = node.getSemanticsData();
        if (Equals(data.flagsCollection.isSelected, Tristate.none))
        {
            return new FlutterError("A tab needs selected states");
        }
        if (node.areUserActionsBlocked)
        {
            return null;
        }
        if (
            (!Equals(data.flagsCollection.isEnabled, Tristate.isFalse))
            && !data.hasAction(SemanticsAction.tap)
        )
        {
            return new FlutterError("A tab must have a tap action");
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static FlutterError? _semanticsTabBar(SemanticsNode node)
    {
        if (node.childrenCount < 1L)
        {
            return new FlutterError("a TabBar cannot be empty");
        }
        FlutterError? error = default!;
        node.visitChildren(
            (child) =>
            {
                if (!Equals(child.getSemanticsData().role, SemanticsRole.tab))
                {
                    error = new FlutterError("Children of TabBar must have the tab role");
                }
                return error is null;
            }
        );
        return error;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static FlutterError? _semanticsTable(SemanticsNode node)
    {
        FlutterError? error = default!;
        node.visitChildren(
            (child) =>
            {
                if (!Equals(child.getSemanticsData().role, SemanticsRole.row))
                {
                    error = new FlutterError("Children of Table must have the row role");
                }
                return error is null;
            }
        );
        return error;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static FlutterError? _semanticsRow(SemanticsNode node)
    {
        if (!Equals(node.parent?.role, SemanticsRole.table))
        {
            return new FlutterError("A row must be a child of a table");
        }
        FlutterError? error = default!;
        node.visitChildren(
            (child) =>
            {
                if (
                    (!Equals(child.getSemanticsData().role, SemanticsRole.cell))
                    && (!Equals(child.getSemanticsData().role, SemanticsRole.columnHeader))
                )
                {
                    error = new FlutterError(
                        "Children of Row must have the cell or columnHeader role"
                    );
                }
                return error is null;
            }
        );
        return error;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static FlutterError? _semanticsCell(SemanticsNode node)
    {
        if (
            (!Equals(node.parent?.role, SemanticsRole.row))
            && (!Equals(node.parent?.role, SemanticsRole.cell))
        )
        {
            return new FlutterError("A cell must be a child of a row or another cell");
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static FlutterError? _semanticsColumnHeader(SemanticsNode node)
    {
        if (
            (!Equals(node.parent?.role, SemanticsRole.row))
            && (!Equals(node.parent?.role, SemanticsRole.cell))
        )
        {
            return new FlutterError("A columnHeader must be a child or another cell");
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static FlutterError? _semanticsRadioGroup(SemanticsNode node)
    {
        FlutterError? error = default!;
        var hasCheckedChild = false;
        bool validateRadioGroupChildren(SemanticsNode node)
        {
            SemanticsData data = node.getSemanticsData();
            if (Equals(data.role, SemanticsRole.radioGroup))
            {
                return error is null;
            }
            if (!data.flagsCollection.isInMutuallyExclusiveGroup)
            {
                node.visitChildren(validateRadioGroupChildren);
                return error is null;
            }
            if (Equals(data.flagsCollection.isChecked, CheckedState.isTrue))
            {
                if (hasCheckedChild)
                {
                    error = new FlutterError(
                        "Radio groups must not have multiple checked children"
                    );
                    return false;
                }
                hasCheckedChild = true;
            }
            DartRuntimePrimitives.Assert(() => error is null);
            return true;
            throw new InvalidOperationException(
                "Control flow completed without returning a value."
            );
        }
        node.visitChildren(validateRadioGroupChildren);
        return error;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static FlutterError? _semanticsMenu(SemanticsNode node)
    {
        if (node.childrenCount < 1L)
        {
            return new FlutterError("a menu cannot be empty");
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static FlutterError? _semanticsMenuBar(SemanticsNode node)
    {
        if (node.childrenCount < 1L)
        {
            return new FlutterError("a menu bar cannot be empty");
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static FlutterError? _semanticsMenuItem(SemanticsNode node)
    {
        SemanticsNode? currentNode = node;
        while (currentNode?.parent is not null)
        {
            if (
                Equals(currentNode?.parent?.role, SemanticsRole.menu)
                || Equals(currentNode?.parent?.role, SemanticsRole.menuBar)
            )
            {
                return null;
            }
            currentNode = currentNode?.parent;
        }
        return new FlutterError("A menu item must be a child of a menu or a menu bar");
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static FlutterError? _semanticsMenuItemCheckbox(SemanticsNode node)
    {
        SemanticsData data = node.getSemanticsData();
        if (Equals(data.flagsCollection.isChecked, CheckedState.none))
        {
            return new FlutterError("a menu item checkbox must be checkable");
        }
        SemanticsNode? currentNode = node;
        while (currentNode?.parent is not null)
        {
            if (
                Equals(currentNode?.parent?.role, SemanticsRole.menu)
                || Equals(currentNode?.parent?.role, SemanticsRole.menuBar)
            )
            {
                return null;
            }
            currentNode = currentNode?.parent;
        }
        return new FlutterError("A menu item checkbox must be a child of a menu or a menu bar");
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static FlutterError? _semanticsMenuItemRadio(SemanticsNode node)
    {
        SemanticsData data = node.getSemanticsData();
        if (Equals(data.flagsCollection.isChecked, CheckedState.none))
        {
            return new FlutterError("a menu item radio must be checkable");
        }
        SemanticsNode? currentNode = node;
        while (currentNode?.parent is not null)
        {
            if (
                Equals(currentNode?.parent?.role, SemanticsRole.menu)
                || Equals(currentNode?.parent?.role, SemanticsRole.menuBar)
            )
            {
                return null;
            }
            currentNode = currentNode?.parent;
        }
        return new FlutterError("A menu item radio must be a child of a menu or a menu bar");
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static FlutterError? _noLiveRegion(SemanticsNode node)
    {
        SemanticsData data = node.getSemanticsData();
        if (data.flagsCollection.isLiveRegion)
        {
            return new FlutterError(
                $"Node {node.id} has role {data.role} but is also a live region. "
                    + $"A node can not have {data.role} and be live region at the same time. "
                    + "Either remove the role or the live region"
            );
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static FlutterError? _semanticsListItem(SemanticsNode node)
    {
        SemanticsData data = node.getSemanticsData();
        SemanticsNode? parentLocal = node.parent;
        if (parentLocal is null)
        {
            return new FlutterError(
                $"Semantics node {node.id} has role {data.role} but doesn't have a parent"
            );
        }
        SemanticsData parentSemanticsData = parentLocal.getSemanticsData();
        if (!Equals(parentSemanticsData.role, SemanticsRole.list))
        {
            return new FlutterError(
                $"Semantics node {node.id} has role {data.role}, but its "
                    + $"parent node {parentLocal.id} doesn't have the role {SemanticsRole.list}. "
                    + $"Please assign the {SemanticsRole.list} to node {parentLocal.id}"
            );
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static bool _isLandmarkRole(SemanticsData nodeData) =>
        Equals(nodeData.role, SemanticsRole.complementary)
        || Equals(nodeData.role, SemanticsRole.contentInfo)
        || Equals(nodeData.role, SemanticsRole.main)
        || Equals(nodeData.role, SemanticsRole.navigation)
        || Equals(nodeData.role, SemanticsRole.region);

    internal static bool _isSameRoleExisted(SemanticsNode semanticsNode)
    {
        DartMap<long, SemanticsNode> treeNodes = semanticsNode.owner!._nodes;
        var sameRoleCount = 0L;
        foreach (long id in treeNodes.Keys)
        {
            if (
                Equals(treeNodes.GetValueOrDefault(id)?.getSemanticsData().role, semanticsNode.role)
            )
            {
                sameRoleCount++;
                if (sameRoleCount > 1L)
                {
                    return true;
                }
            }
        }
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static FlutterError? _semanticsComplementary(SemanticsNode node)
    {
        SemanticsNode? currentNode = node.parent;
        while (currentNode is not null)
        {
            if (_isLandmarkRole(currentNode.getSemanticsData()))
            {
                return new FlutterError(
                    "The complementary landmark role should not contained within any other landmark roles."
                );
            }
            currentNode = currentNode.parent;
        }
        SemanticsData data = node.getSemanticsData();
        if (_isSameRoleExisted(node) && (data.label.Length == 0))
        {
            return new FlutterError(
                "The complementary landmark role should have a unique label as it is used more than once."
            );
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static FlutterError? _semanticsContentInfo(SemanticsNode node)
    {
        SemanticsNode? currentNode = node.parent;
        while (currentNode is not null)
        {
            if (_isLandmarkRole(currentNode.getSemanticsData()))
            {
                return new FlutterError(
                    "The contentInfo landmark role should not contained within any other landmark roles."
                );
            }
            currentNode = currentNode.parent;
        }
        SemanticsData data = node.getSemanticsData();
        if (_isSameRoleExisted(node) && (data.label.Length == 0))
        {
            return new FlutterError(
                "The contentInfo landmark role should have a unique label as it is used more than once."
            );
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static FlutterError? _semanticsMain(SemanticsNode node)
    {
        SemanticsNode? currentNode = node.parent;
        while (currentNode is not null)
        {
            if (_isLandmarkRole(currentNode.getSemanticsData()))
            {
                return new FlutterError(
                    "The main landmark role should not contained within any other landmark roles."
                );
            }
            currentNode = currentNode.parent;
        }
        SemanticsData data = node.getSemanticsData();
        if (_isSameRoleExisted(node) && (data.label.Length == 0))
        {
            return new FlutterError(
                "The main landmark role should have a unique label as it is used more than once."
            );
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static FlutterError? _semanticsNavigation(SemanticsNode node)
    {
        SemanticsData data = node.getSemanticsData();
        if (_isSameRoleExisted(node) && (data.label.Length == 0))
        {
            return new FlutterError(
                "The navigation landmark role should have a unique label as it is used more than once."
            );
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static FlutterError? _semanticsRegion(SemanticsNode node)
    {
        SemanticsData data = node.getSemanticsData();
        if (data.label.Length == 0)
        {
            return new FlutterError(
                "A region role should include a label that describes the purpose of the content."
            );
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static FlutterError? _semanticsGeneral(SemanticsNode node)
    {
        SemanticsData data = node.getSemanticsData();
        bool? isExpandedLocal = data.flagsCollection.isExpanded.toBoolOrNull();
        if (isExpandedLocal is not null)
        {
            bool isExpanded__19946__value20016 = (
                isExpandedLocal
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            bool hasExpandAction = data.hasAction(SemanticsAction.expand);
            bool hasCollapseAction = data.hasAction(SemanticsAction.collapse);
            if (hasExpandAction && hasCollapseAction)
            {
                return new FlutterError(
                    "An expandable node cannot have both expand and collapse actions set at the same time."
                );
            }
            if ((isExpanded__19946__value20016) && hasExpandAction)
            {
                return new FlutterError("An expanded node cannot have an expand action.");
            }
            if (!(isExpanded__19946__value20016) && hasCollapseAction)
            {
                return new FlutterError("A collapsed node cannot have a collapse action.");
            }
        }
        if (
            data.flagsCollection.isAccessibilityFocusBlocked
            && (!Equals(data.flagsCollection.isFocused, Tristate.none))
        )
        {
            return new FlutterError(
                "A node that is keyboard focusable cannot be set to accessibility unfocusable"
            );
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class SemanticsTag
{
    public virtual string name { get; private set; } = default!;

    public SemanticsTag(string name)
    {
        this.name = name;
    }

    public override string ToString() =>
        $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "SemanticsTag")}({name})";
}

public class ChildSemanticsConfigurationsResult
{
    public virtual List<SemanticsConfiguration> mergeUp { get; private set; } = default!;
    public virtual List<List<SemanticsConfiguration>> siblingMergeGroups { get; private set; } =
        default!;

    public ChildSemanticsConfigurationsResult(
        List<SemanticsConfiguration> mergeUp,
        List<List<SemanticsConfiguration>> siblingMergeGroups
    )
    {
        this.mergeUp = mergeUp;
        this.siblingMergeGroups = siblingMergeGroups;
    }
}

public class ChildSemanticsConfigurationsResultBuilder
{
    internal virtual List<SemanticsConfiguration> _mergeUp { get; private set; } =
        new List<SemanticsConfiguration>();
    internal virtual List<List<SemanticsConfiguration>> _siblingMergeGroups { get; private set; } =
        new List<List<SemanticsConfiguration>>();

    public ChildSemanticsConfigurationsResultBuilder() { }

    public virtual void markAsMergeUp(SemanticsConfiguration config) => _mergeUp.Add(config);

    public virtual void markAsSiblingMergeGroup(List<SemanticsConfiguration> configs) =>
        _siblingMergeGroups.Add(configs);

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
        return new ChildSemanticsConfigurationsResult(_mergeUp, _siblingMergeGroups);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class CustomSemanticsAction
{
    public virtual string? label { get; private set; }
    public virtual string? hint { get; private set; }
    public virtual SemanticsAction? action { get; private set; }
    internal static long _nextId = 0L;
    internal static DartMap<long, CustomSemanticsAction> _actions =
        new DartMap<long, CustomSemanticsAction>();
    internal static DartMap<CustomSemanticsAction, long> _ids =
        new DartMap<CustomSemanticsAction, long>();

    public CustomSemanticsAction(string label)
    {
        this.label = label;
        hint = null;
        action = null;
        System.Diagnostics.Debug.Assert(label != "");
    }

    public static CustomSemanticsAction CreateOverridingAction(string hint, SemanticsAction action)
    {
        var __instance = new CustomSemanticsAction(default!);
        __instance.hint = hint;
        __instance.action = action;
        __instance.label = null;
        return __instance;
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(label, hint, action);

    public override bool Equals(object? other)
    {
        var __other = other as CustomSemanticsAction;
        if (__other is null)
        {
            return false;
        }

        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is CustomSemanticsAction)
            && (__other.label == label)
            && (__other.hint == hint)
            && Equals(__other.action, action);
    }

    public override string ToString()
    {
        return $"CustomSemanticsAction({_ids.GetValueOrDefault(this)}, label:{label}, hint:{hint}, action:{action})";
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static CustomSemanticsAction? getAction(long id)
    {
        return _actions.GetValueOrDefault(id);
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
        System.Diagnostics.Debug.Assert(
            (@string.Length != 0) || (checked((long)__attributes.Count) == 0)
        );
        System.Diagnostics.Debug.Assert(
            (
                (Func<bool>)(
                    () =>
                    {
                        foreach (var attribute in __attributes)
                        {
                            DartRuntimePrimitives.Assert(() =>
                                (@string.Length >= attribute.range.start)
                                && (@string.Length >= attribute.range.end)
                            );
                        }
                        return true;
                    }
                )
            )()
        );
    }

    public virtual AttributedString op_Add(AttributedString other)
    {
        if (@string.Length == 0)
        {
            return other;
        }
        if (other.@string.Length == 0)
        {
            return this;
        }
        string newString = @string + other.@string;
        var newAttributes = new List<StringAttribute>(attributes);
        if (checked((long)other.attributes.Count) != 0)
        {
            long offset = @string.Length;
            foreach (StringAttribute attribute in other.attributes)
            {
                var newRange = new TextRange(
                    start: attribute.range.start + offset,
                    end: attribute.range.end + offset
                );
                StringAttribute adjustedAttribute = attribute.copy(range: newRange);
                newAttributes.Add(adjustedAttribute);
            }
        }
        return new AttributedString(newString, attributes: newAttributes);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as AttributedString;
        if (__other is null)
        {
            return false;
        }

        return Equals(DartRuntimePrimitives.RuntimeType(__other), GetType())
            && (__other is AttributedString)
            && (__other.@string == @string)
            && CollectionsLibrary.listEquals(__other.attributes, attributes);
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(@string, attributes);

    public override string ToString()
    {
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "AttributedString")}('{@string}', attributes: {attributes})";
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class AttributedStringProperty : DiagnosticsProperty<AttributedString>
{
    public virtual bool showWhenEmpty { get; private set; } = default!;

    public AttributedStringProperty(
        string name,
        AttributedString? value,
        bool showName = true,
        bool showWhenEmpty = false,
        object? defaultValue = default!,
        DiagnosticLevel level = DiagnosticLevel.info,
        string? description = null
    )
        : base(
            name,
            value,
            showName: showName,
            defaultValue: defaultValue ?? DiagnosticsLibrary.kNoDefaultValue,
            level: level,
            description: description
        )
    {
        this.showWhenEmpty = showWhenEmpty;
    }

    public new virtual bool isInteresting =>
        base.isInteresting
        && (showWhenEmpty || ((value is not null) && (value!.@string.Length != 0)));

    public virtual string valueToString(TextTreeConfiguration? parentConfiguration = null)
    {
        if (value is null)
        {
            return "null";
        }
        string text = value!.@string;
        if ((parentConfiguration is not null) && !parentConfiguration.lineBreakProperties)
        {
            text = text.replaceAll("\n", "\\n");
        }
        if (checked((long)value!.attributes.Count) == 0)
        {
            return $"\"{text}\"";
        }
        return $"\"{text}\" {value!.attributes}";
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal delegate void _LabelPart__semantics();

public class SemanticsLabelBuilder
{
    public virtual string separator { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }
    internal virtual List<(string, TextDirection?)> _parts { get; private set; } =
        new List<(string, TextDirection?)>();

    public SemanticsLabelBuilder(string separator = " ", TextDirection? textDirection = null)
    {
        this.separator = separator;
        this.textDirection = textDirection;
    }

    public virtual void addPart(string label, TextDirection? textDirection = null)
    {
        if (label.Length != 0)
        {
            _parts.Add((label, textDirection));
        }
    }

    public virtual bool isEmpty => checked((long)_parts.Count) == 0;
    public virtual long length => checked(_parts.Count);

    public virtual string build()
    {
        if (checked((long)_parts.Count) == 0)
        {
            return "";
        }
        if (checked(_parts.Count) == 1L)
        {
            var (text, _) = _parts.First();
            return text;
        }
        var buffer = new System.Text.StringBuilder();
        var (firstText, _) = _parts.First();
        buffer.Append(firstText);
        foreach (var (partText, partTextDirection) in _parts.skip(1L))
        {
            TextDirection? partDirection = partTextDirection ?? textDirection;
            if (separator.Length != 0)
            {
                buffer.Append(separator);
            }
            var processedText = partText;
            if (
                (textDirection is not null)
                && (partDirection is not null)
                && (
                    !Equals(
                        textDirection,
                        (
                            partDirection
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        )
                    )
                )
            )
            {
                TextDirection textDirection__value36162 = (
                    textDirection
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
                TextDirection partDirection__35987__value36187 = (
                    partDirection
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
                string directionalEmbedding = (partDirection__35987__value36187) switch
                {
                    TextDirection.rtl => Unicode.RLE,
                    TextDirection.ltr => Unicode.LRE,
                    _ => throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
                };
                processedText = directionalEmbedding + partText + Unicode.PDF;
            }
            buffer.Append(processedText);
        }
        return buffer.ToString();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void clear()
    {
        _parts.Clear();
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

    public SemanticsData(
        SemanticsFlags flagsCollection,
        long actions,
        string identifier,
        object? traversalParentIdentifier,
        object? traversalChildIdentifier,
        AttributedString attributedLabel,
        AttributedString attributedValue,
        AttributedString attributedIncreasedValue,
        AttributedString attributedDecreasedValue,
        AttributedString attributedHint,
        string tooltip,
        TextDirection? textDirection,
        Rect rect,
        TextSelection? textSelection,
        long? scrollIndex,
        long? scrollChildCount,
        double? scrollPosition,
        double? scrollExtentMax,
        double? scrollExtentMin,
        long? platformViewId,
        long? maxValueLength,
        long? currentValueLength,
        long headingLevel,
        DartUri? linkUrl,
        SemanticsRole role,
        HashSet<string>? controlsNodes,
        SemanticsValidationResult validationResult,
        SemanticsHitTestBehavior hitTestBehavior,
        SemanticsInputType inputType,
        Locale? locale,
        string? minValue,
        string? maxValue,
        HashSet<SemanticsTag>? tags = null,
        Matrix4? transform = null,
        List<long>? customSemanticsActionIds = null
    )
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
        System.Diagnostics.Debug.Assert((tooltip == "") || (textDirection is not null));
        System.Diagnostics.Debug.Assert(
            (attributedLabel.@string == "") || (textDirection is not null)
        );
        System.Diagnostics.Debug.Assert(
            (attributedValue.@string == "") || (textDirection is not null)
        );
        System.Diagnostics.Debug.Assert(
            (attributedDecreasedValue.@string == "") || (textDirection is not null)
        );
        System.Diagnostics.Debug.Assert(
            (attributedIncreasedValue.@string == "") || (textDirection is not null)
        );
        System.Diagnostics.Debug.Assert(
            (attributedHint.@string == "") || (textDirection is not null)
        );
        System.Diagnostics.Debug.Assert((headingLevel >= 0L) && (headingLevel <= 6L));
        System.Diagnostics.Debug.Assert((linkUrl is null) || flagsCollection.isLink);
    }

    public virtual long flags => SemanticsLibrary._toBitMask(flagsCollection);
    public virtual string label => attributedLabel.@string;
    public virtual string value => attributedValue.@string;
    public virtual string increasedValue => attributedIncreasedValue.@string;
    public virtual string decreasedValue => attributedDecreasedValue.@string;
    public virtual string hint => attributedHint.@string;

    public virtual bool hasFlag(SemanticsFlag flag) =>
        (flags & FoundationRuntimePorts.EnumIndex(flag)) != 0L;

    public virtual bool hasAction(SemanticsAction action) => (actions & (long)action) != 0L;

    public virtual string toStringShort() =>
        objectRuntimeTypeFunctions.objectRuntimeType(this, "SemanticsData");

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Rect>("rect", rect, showName: false));
        properties.add(
            new TransformProperty("transform", transform, showName: false, defaultValue: null)
        );
        var actionSummary = new List<string>();
        List<string?> customSemanticsActionSummary = customSemanticsActionIds!
            .map((actionId) => CustomSemanticsAction.getAction(actionId)!.label)
            .ToList();
        properties.add(new IterableProperty<string>("actions", actionSummary, ifEmpty: null));
        properties.add(
            new IterableProperty<string?>(
                "customActions",
                customSemanticsActionSummary,
                ifEmpty: null
            )
        );
        List<string> flagSummary = flagsCollection.toStrings();
        properties.add(new IterableProperty<string>("flags", flagSummary, ifEmpty: null));
        properties.add(new StringProperty("identifier", identifier, defaultValue: ""));
        properties.add(
            new DiagnosticsProperty<object>(
                "traversalParentIdentifier",
                traversalParentIdentifier,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<object>(
                "traversalChildIdentifier",
                traversalChildIdentifier,
                defaultValue: null
            )
        );
        properties.add(new AttributedStringProperty("label", attributedLabel));
        properties.add(new AttributedStringProperty("value", attributedValue));
        properties.add(new AttributedStringProperty("increasedValue", attributedIncreasedValue));
        properties.add(new AttributedStringProperty("decreasedValue", attributedDecreasedValue));
        properties.add(new AttributedStringProperty("hint", attributedHint));
        properties.add(new StringProperty("tooltip", tooltip, defaultValue: ""));
        properties.add(
            new EnumProperty<TextDirection>("textDirection", textDirection, defaultValue: null)
        );
        if (textSelection?.isValid ?? false)
        {
            properties.add(
                new MessageProperty(
                    "textSelection",
                    $"[{textSelection!.start}, {textSelection!.end}]"
                )
            );
        }
        properties.add(new IntProperty("platformViewId", platformViewId, defaultValue: null));
        properties.add(new IntProperty("maxValueLength", maxValueLength, defaultValue: null));
        properties.add(
            new IntProperty("currentValueLength", currentValueLength, defaultValue: null)
        );
        properties.add(new IntProperty("scrollChildren", scrollChildCount, defaultValue: null));
        properties.add(new IntProperty("scrollIndex", scrollIndex, defaultValue: null));
        properties.add(new DoubleProperty("scrollExtentMin", scrollExtentMin, defaultValue: null));
        properties.add(new DoubleProperty("scrollPosition", scrollPosition, defaultValue: null));
        properties.add(new DoubleProperty("scrollExtentMax", scrollExtentMax, defaultValue: null));
        properties.add(new IntProperty("headingLevel", headingLevel, defaultValue: 0L));
        properties.add(new DiagnosticsProperty<DartUri>("linkUrl", linkUrl, defaultValue: null));
        if (controlsNodes is not null)
        {
            properties.add(new IterableProperty<string>("controls", controlsNodes, ifEmpty: null));
        }
        if (!Equals(role, SemanticsRole.none))
        {
            properties.add(
                new EnumProperty<SemanticsRole>("role", role, defaultValue: SemanticsRole.none)
            );
        }
        if (!Equals(validationResult, SemanticsValidationResult.none))
        {
            properties.add(
                new EnumProperty<SemanticsValidationResult>(
                    "validationResult",
                    validationResult,
                    defaultValue: SemanticsValidationResult.none
                )
            );
        }
        properties.add(new StringProperty("minValue", minValue, defaultValue: null));
        properties.add(new StringProperty("maxValue", maxValue, defaultValue: null));
    }

    public override bool Equals(object? other)
    {
        var __other = other as SemanticsData;
        if (__other is null)
        {
            return false;
        }

        return (__other is SemanticsData)
            && (__other.flags == flags)
            && (__other.actions == actions)
            && (__other.identifier == identifier)
            && Equals(__other.traversalParentIdentifier, traversalParentIdentifier)
            && Equals(__other.traversalChildIdentifier, traversalChildIdentifier)
            && Equals(__other.attributedLabel, attributedLabel)
            && Equals(__other.attributedValue, attributedValue)
            && Equals(__other.attributedIncreasedValue, attributedIncreasedValue)
            && Equals(__other.attributedDecreasedValue, attributedDecreasedValue)
            && Equals(__other.attributedHint, attributedHint)
            && (__other.tooltip == tooltip)
            && Equals(__other.textDirection, textDirection)
            && Equals(__other.rect, rect)
            && CollectionsLibrary.setEquals(__other.tags, tags)
            && (__other.scrollChildCount == scrollChildCount)
            && (__other.scrollIndex == scrollIndex)
            && Equals(__other.textSelection, textSelection)
            && (__other.scrollPosition == scrollPosition)
            && (__other.scrollExtentMax == scrollExtentMax)
            && (__other.scrollExtentMin == scrollExtentMin)
            && (__other.platformViewId == platformViewId)
            && (__other.maxValueLength == maxValueLength)
            && (__other.currentValueLength == currentValueLength)
            && Equals(__other.transform, transform)
            && (__other.headingLevel == headingLevel)
            && Equals(__other.linkUrl, linkUrl)
            && Equals(__other.role, role)
            && Equals(__other.validationResult, validationResult)
            && Equals(__other.inputType, inputType)
            && Equals(__other.hitTestBehavior, hitTestBehavior)
            && _sortedListsEqual(__other.customSemanticsActionIds, customSemanticsActionIds)
            && CollectionsLibrary.setEquals(controlsNodes, __other.controlsNodes)
            && Equals(__other.traversalParentIdentifier, traversalParentIdentifier)
            && Equals(__other.traversalChildIdentifier, traversalChildIdentifier)
            && (__other.minValue == minValue)
            && (__other.maxValue == maxValue);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(
            flags,
            actions,
            identifier,
            attributedLabel,
            attributedValue,
            attributedIncreasedValue,
            attributedDecreasedValue,
            attributedHint,
            tooltip,
            textDirection,
            rect,
            tags,
            textSelection,
            scrollChildCount,
            scrollIndex,
            scrollPosition,
            scrollExtentMax,
            scrollExtentMin,
            platformViewId,
            FoundationRuntimePorts.ObjectHash(
                maxValueLength,
                currentValueLength,
                transform,
                headingLevel,
                linkUrl,
                (customSemanticsActionIds is null)
                    ? null
                    : FoundationRuntimePorts.ObjectHashAll(customSemanticsActionIds!),
                role,
                validationResult,
                (controlsNodes is null)
                    ? null
                    : FoundationRuntimePorts.ObjectHashAll(controlsNodes!),
                inputType,
                hitTestBehavior,
                traversalParentIdentifier,
                traversalChildIdentifier,
                minValue,
                maxValue
            )
        );

    internal static bool _sortedListsEqual(List<long>? left, List<long>? right)
    {
        if ((left is null) && (right is null))
        {
            return true;
        }
        if ((left is not null) && (right is not null))
        {
            if (checked(left.Count) != checked((long)right.Count))
            {
                return false;
            }
            for (var i = 0L; i < checked(left.Count); i++)
            {
                if (left[(int)i] != right[(int)i])
                {
                    return false;
                }
            }
            return true;
        }
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _SemanticsDiagnosticableNode__semantics : DiagnosticableNode<SemanticsNode>
{
    public virtual DebugSemanticsDumpOrder childOrder { get; private set; } = default!;

    internal _SemanticsDiagnosticableNode__semantics(
        string? name = null,
        SemanticsNode value = default!,
        DiagnosticsTreeStyle? style = default!,
        DebugSemanticsDumpOrder childOrder = default!
    )
        : base(
            name: name,
            value: value,
            style: (
                style
                ?? throw new global::System.NullReferenceException("A required value was null.")
            )
        )
    {
        this.childOrder = childOrder;
    }

    public override List<DiagnosticsNode> getChildren() =>
        value.debugDescribeChildren(childOrder: childOrder);
}

public class SemanticsHintOverrides : DiagnosticableTree
{
    public virtual string? onTapHint { get; private set; }
    public virtual string? onLongPressHint { get; private set; }

    public SemanticsHintOverrides(string? onTapHint = null, string? onLongPressHint = null)
    {
        this.onTapHint = onTapHint;
        this.onLongPressHint = onLongPressHint;
        System.Diagnostics.Debug.Assert(onTapHint != "");
        System.Diagnostics.Debug.Assert(onLongPressHint != "");
    }

    public virtual bool isNotEmpty => (onTapHint is not null) || (onLongPressHint is not null);

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(onTapHint, onLongPressHint);

    public override bool Equals(object? other)
    {
        var __other = other as SemanticsHintOverrides;
        if (__other is null)
        {
            return false;
        }

        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is SemanticsHintOverrides)
            && (__other.onTapHint == onTapHint)
            && (__other.onLongPressHint == onLongPressHint);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new StringProperty("onTapHint", onTapHint, defaultValue: null));
        properties.add(new StringProperty("onLongPressHint", onLongPressHint, defaultValue: null));
    }

    public virtual string toStringDeep(
        string prefixLineOne = "",
        string? prefixOtherLines = null,
        DiagnosticLevel minLevel = DiagnosticLevel.debug,
        long? wrapWidth = null
    ) =>
        ((DiagnosticableTree)this).toStringDeep(
            prefixLineOne,
            prefixOtherLines,
            minLevel,
            wrapWidth
        );
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
    public virtual DartMap<CustomSemanticsAction, Action>? customSemanticsActions
    {
        get;
        private set;
    }
    public virtual SemanticsRole? role { get; private set; }
    public virtual HashSet<string>? controlsNodes { get; private set; }
    public virtual SemanticsValidationResult validationResult { get; private set; } = default!;
    public virtual SemanticsHitTestBehavior? hitTestBehavior { get; private set; }
    public virtual SemanticsInputType? inputType { get; private set; }
    public virtual string? maxValue { get; private set; }
    public virtual string? minValue { get; private set; }

    public SemanticsProperties(
        bool? enabled = null,
        bool? @checked = null,
        bool? mixed = null,
        bool? expanded = null,
        bool? selected = null,
        bool? toggled = null,
        bool? button = null,
        bool? link = null,
        DartUri? linkUrl = null,
        bool? header = null,
        long? headingLevel = null,
        bool? textField = null,
        bool? slider = null,
        bool? keyboardKey = null,
        bool? readOnly = null,
        bool? focusable = null,
        bool? focused = null,
        AccessibilityFocusBlockType? accessibilityFocusBlockType = null,
        bool? inMutuallyExclusiveGroup = null,
        bool? hidden = null,
        bool? obscured = null,
        bool? multiline = null,
        bool? scopesRoute = null,
        bool? namesRoute = null,
        bool? image = null,
        bool? liveRegion = null,
        bool? isRequired = null,
        long? maxValueLength = null,
        long? currentValueLength = null,
        string? identifier = null,
        object? traversalParentIdentifier = null,
        object? traversalChildIdentifier = null,
        string? label = null,
        AttributedString? attributedLabel = null,
        string? value = null,
        AttributedString? attributedValue = null,
        string? increasedValue = null,
        AttributedString? attributedIncreasedValue = null,
        string? decreasedValue = null,
        AttributedString? attributedDecreasedValue = null,
        string? hint = null,
        string? tooltip = null,
        AttributedString? attributedHint = null,
        SemanticsHintOverrides? hintOverrides = null,
        TextDirection? textDirection = null,
        SemanticsSortKey? sortKey = null,
        SemanticsTag? tagForChildren = null,
        SemanticsRole? role = null,
        HashSet<string>? controlsNodes = null,
        SemanticsInputType? inputType = null,
        SemanticsValidationResult validationResult = SemanticsValidationResult.none,
        SemanticsHitTestBehavior? hitTestBehavior = null,
        Action? onTap = null,
        Action? onLongPress = null,
        Action? onScrollLeft = null,
        Action? onScrollRight = null,
        Action? onScrollUp = null,
        Action? onScrollDown = null,
        Action? onIncrease = null,
        Action? onDecrease = null,
        Action? onCopy = null,
        Action? onCut = null,
        Action? onPaste = null,
        Action<bool>? onMoveCursorForwardByCharacter = null,
        Action<bool>? onMoveCursorBackwardByCharacter = null,
        Action<bool>? onMoveCursorForwardByWord = null,
        Action<bool>? onMoveCursorBackwardByWord = null,
        Action<TextSelection>? onSetSelection = null,
        Action<string>? onSetText = null,
        Action? onDidGainAccessibilityFocus = null,
        Action? onDidLoseAccessibilityFocus = null,
        Action? onFocus = null,
        Action? onDismiss = null,
        Action? onExpand = null,
        Action? onCollapse = null,
        DartMap<CustomSemanticsAction, Action>? customSemanticsActions = null,
        string? minValue = null,
        string? maxValue = null
    )
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
        System.Diagnostics.Debug.Assert((label is null) || (attributedLabel is null));
        System.Diagnostics.Debug.Assert((value is null) || (attributedValue is null));
        System.Diagnostics.Debug.Assert(
            (increasedValue is null) || (attributedIncreasedValue is null)
        );
        System.Diagnostics.Debug.Assert(
            (decreasedValue is null) || (attributedDecreasedValue is null)
        );
        System.Diagnostics.Debug.Assert((hint is null) || (attributedHint is null));
        System.Diagnostics.Debug.Assert(
            (headingLevel is null)
                || (
                    (
                        (
                            headingLevel
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        ) > 0L
                    ) && (headingLevel <= 6L)
                )
        );
        System.Diagnostics.Debug.Assert((linkUrl is null) || (link ?? false));
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<bool>("checked", @checked, defaultValue: null));
        properties.add(new DiagnosticsProperty<bool>("mixed", mixed, defaultValue: null));
        properties.add(new DiagnosticsProperty<bool>("expanded", expanded, defaultValue: null));
        properties.add(new DiagnosticsProperty<bool>("selected", selected, defaultValue: null));
        properties.add(new DiagnosticsProperty<bool>("isRequired", isRequired, defaultValue: null));
        properties.add(new StringProperty("identifier", identifier, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<object>(
                "traversalParentIdentifier",
                traversalParentIdentifier,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<object>(
                "traversalChildIdentifier",
                traversalChildIdentifier,
                defaultValue: null
            )
        );
        properties.add(new StringProperty("label", label, defaultValue: null));
        properties.add(
            new AttributedStringProperty("attributedLabel", attributedLabel, defaultValue: null)
        );
        properties.add(new StringProperty("value", value, defaultValue: null));
        properties.add(
            new AttributedStringProperty("attributedValue", attributedValue, defaultValue: null)
        );
        properties.add(new StringProperty("increasedValue", value, defaultValue: null));
        properties.add(
            new AttributedStringProperty(
                "attributedIncreasedValue",
                attributedIncreasedValue,
                defaultValue: null
            )
        );
        properties.add(new StringProperty("decreasedValue", value, defaultValue: null));
        properties.add(
            new AttributedStringProperty(
                "attributedDecreasedValue",
                attributedDecreasedValue,
                defaultValue: null
            )
        );
        properties.add(new StringProperty("hint", hint, defaultValue: null));
        properties.add(
            new AttributedStringProperty("attributedHint", attributedHint, defaultValue: null)
        );
        properties.add(new StringProperty("tooltip", tooltip, defaultValue: null));
        properties.add(
            new EnumProperty<TextDirection>("textDirection", textDirection, defaultValue: null)
        );
        properties.add(new EnumProperty<SemanticsRole>("role", role, defaultValue: null));
        properties.add(
            new EnumProperty<SemanticsValidationResult>(
                "validationResult",
                validationResult,
                defaultValue: SemanticsValidationResult.none
            )
        );
        properties.add(
            new DiagnosticsProperty<SemanticsSortKey>("sortKey", sortKey, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<SemanticsHintOverrides>(
                "hintOverrides",
                hintOverrides,
                defaultValue: null
            )
        );
    }

    public virtual string toStringShort() =>
        objectRuntimeTypeFunctions.objectRuntimeType(this, "SemanticsProperties");

    public virtual string toStringDeep(
        string prefixLineOne = "",
        string? prefixOtherLines = null,
        DiagnosticLevel minLevel = DiagnosticLevel.debug,
        long? wrapWidth = null
    ) =>
        ((DiagnosticableTree)this).toStringDeep(
            prefixLineOne,
            prefixOtherLines,
            minLevel,
            wrapWidth
        );
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
    internal static long _maxFrameworkAccessibilityIdentifier = (1L << (int)16L) - 1L;
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
    internal virtual bool _mergeAllDescendantsIntoThisNode { get; set; } =
        _kEmptyConfig.isMergingSemanticsOfDescendants;
    internal virtual List<SemanticsNode>? _children { get; set; } = default;
    internal virtual List<SemanticsNode> _debugPreviousSnapshot { get; set; } = default!;
    internal virtual bool _dead { get; set; } = false;
    internal virtual SemanticsOwner? _owner { get; set; } = default;
    internal virtual SemanticsNode? _parent { get; set; } = default;
    internal virtual SemanticsNode? _traversalParent { get; set; } = default;
    internal virtual long _depth { get; set; } = 0L;
    internal virtual Locale? _locale { get; set; } = default;
    internal virtual bool _dirty { get; set; } = false;
    internal virtual DartMap<SemanticsAction, Action<object?>> _actions { get; set; } =
        _kEmptyConfig._actions;
    internal virtual DartMap<CustomSemanticsAction, Action> _customSemanticsActions { get; set; } =
        _kEmptyConfig._customSemanticsActions;
    internal virtual long _actionsAsBits { get; set; } = _kEmptyConfig._actionsAsBits;
    public virtual HashSet<SemanticsTag>? tags { get; set; } = default;
    internal virtual SemanticsFlags _flags { get; set; } = SemanticsFlags.none;
    internal virtual string _identifier { get; set; } = _kEmptyConfig.identifier;
    internal virtual object? _traversalParentIdentifier { get; set; } = default;
    internal virtual object? _traversalChildIdentifier { get; set; } = default;
    internal virtual AttributedString _attributedLabel { get; set; } =
        _kEmptyConfig.attributedLabel;
    internal virtual AttributedString _attributedValue { get; set; } =
        _kEmptyConfig.attributedValue;
    internal virtual AttributedString _attributedIncreasedValue { get; set; } =
        _kEmptyConfig.attributedIncreasedValue;
    internal virtual AttributedString _attributedDecreasedValue { get; set; } =
        _kEmptyConfig.attributedDecreasedValue;
    internal virtual AttributedString _attributedHint { get; set; } = _kEmptyConfig.attributedHint;
    internal virtual string _tooltip { get; set; } = _kEmptyConfig.tooltip;
    internal virtual SemanticsHintOverrides? _hintOverrides { get; set; } = default;
    internal virtual TextDirection? _textDirection { get; set; } = _kEmptyConfig.textDirection;
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
    internal virtual long _headingLevel { get; set; } = _kEmptyConfig._headingLevel;
    internal virtual DartUri? _linkUrl { get; set; } = _kEmptyConfig._linkUrl;
    internal virtual SemanticsRole _role { get; set; } = _kEmptyConfig.role;
    internal virtual HashSet<string>? _controlsNodes { get; set; } = _kEmptyConfig.controlsNodes;
    internal virtual string? _minValue { get; set; } = default;
    internal virtual string? _maxValue { get; set; } = default;
    internal virtual SemanticsValidationResult _validationResult { get; set; } =
        _kEmptyConfig.validationResult;
    internal virtual SemanticsHitTestBehavior _hitTestBehavior { get; set; } =
        DorotiUiLibrary.SemanticsHitTestBehavior.defer;
    internal virtual SemanticsInputType _inputType { get; set; } = _kEmptyConfig.inputType;
    internal static SemanticsConfiguration _kEmptyConfig = new SemanticsConfiguration();
    internal static int[] _kEmptyChildList = [];
    internal static int[] _kEmptyCustomSemanticsActionsList = [];
    internal static Matrix4 _kIdentityTransform = Matrix4.identity();

    public SemanticsNode(Key? key = null, Action? showOnScreen = null)
    {
        this.key = key;
        _id = _generateNewId();
        _showOnScreen = showOnScreen;
    }

    public static SemanticsNode CreateRoot(
        Key? key = null,
        Action? showOnScreen = null,
        SemanticsOwner owner = default!
    )
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
        _lastIdentifier = (_lastIdentifier + 1L) % _maxFrameworkAccessibilityIdentifier;
        return _lastIdentifier;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual long id => _id;
    public virtual Matrix4? transform
    {
        get => _transform;
        set
        {
            var __value = value;
            if (!MatrixUtils.matrixEquals(_transform, __value))
            {
                _transform =
                    ((__value is null) || MatrixUtils.isIdentity(__value)) ? null : __value;
                _markDirty();
            }
        }
    }
    internal virtual Matrix4? _traversalTransform
    {
        get
        {
            return ConstantsLibrary.kIsWeb ? transform : (_traversalChildTransform ?? transform);
        }
    }
    public virtual Rect rect
    {
        get => _rect;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value).isFinite);
            if (!Equals(_rect, (__value)))
            {
                _rect = (__value);
                _markDirty();
            }
        }
    }
    public virtual bool isInvisible =>
        !isMergedIntoParent && (rect.isEmpty || (transform?.isZero() ?? false));
    public virtual bool isMergedIntoParent
    {
        get => _isMergedIntoParent;
        set
        {
            var __value = value;
            if (_isMergedIntoParent == (__value))
            {
                return;
            }
            _isMergedIntoParent = (__value);
            parent?._markDirty();
        }
    }
    public virtual bool areUserActionsBlocked
    {
        get => _areUserActionsBlocked;
        set
        {
            var __value = value;
            if (_areUserActionsBlocked == (__value))
            {
                return;
            }
            _areUserActionsBlocked = (__value);
            _markDirty();
        }
    }
    public virtual bool isPartOfNodeMerging =>
        mergeAllDescendantsIntoThisNode || isMergedIntoParent;
    public virtual bool mergeAllDescendantsIntoThisNode => _mergeAllDescendantsIntoThisNode;

    internal virtual void _replaceChildren(List<SemanticsNode> newChildren)
    {
        DartRuntimePrimitives.Assert(() => !newChildren.any((child) => Equals(child, this)));
        DartRuntimePrimitives.Assert(() =>
        {
            var seenChildren = new HashSet<SemanticsNode>();
            foreach (var childLocal in newChildren)
            {
                DartRuntimePrimitives.Assert(() => seenChildren.Add(childLocal));
            }
            return true;
        });
        if (_children is not null)
        {
            foreach (SemanticsNode childAlternate in _children!)
            {
                childAlternate._dead = true;
            }
        }
        foreach (var childNested in newChildren)
        {
            childNested._dead = false;
        }
        var sawChange = false;
        if (_children is not null)
        {
            foreach (SemanticsNode childCurrent in _children!)
            {
                if (childCurrent._dead)
                {
                    if (Equals(childCurrent.parent, this))
                    {
                        _dropChild(childCurrent);
                    }
                    sawChange = true;
                }
            }
        }
        foreach (var childNext in newChildren)
        {
            if (!Equals(childNext.parent, this))
            {
                if (childNext.parent is not null)
                {
                    childNext.parent?._dropChild(childNext);
                }
                DartRuntimePrimitives.Assert(() => !childNext.attached);
                _adoptChild(childNext);
                sawChange = true;
            }
        }
        DartRuntimePrimitives.Assert(() =>
        {
            if (DartRuntimePrimitives.Identical(newChildren, _children))
            {
                var mutationErrors = new List<DiagnosticsNode>();
                if (checked(newChildren.Count) != checked((long)_debugPreviousSnapshot.Count))
                {
                    mutationErrors.Add(
                        new ErrorDescription(
                            $"The list's length has changed from {checked((long)_debugPreviousSnapshot.Count)} "
                                + $"to {checked((long)newChildren.Count)}."
                        )
                    );
                }
                else
                {
                    for (var i = 0L; i < checked(newChildren.Count); i++)
                    {
                        if (
                            !DartRuntimePrimitives.Identical(
                                newChildren[(int)i],
                                _debugPreviousSnapshot[(int)i]
                            )
                        )
                        {
                            if (checked((long)mutationErrors.Count) != 0)
                            {
                                mutationErrors.Add(new ErrorSpacer());
                            }
                            mutationErrors.Add(
                                new ErrorDescription($"Child node at position {i} was replaced:")
                            );
                            mutationErrors.Add(
                                ((Diagnosticable)_debugPreviousSnapshot[(int)i]).toDiagnosticsNode(
                                    name: "Previous child",
                                    style: DiagnosticsTreeStyle.singleLine
                                )
                            );
                            mutationErrors.Add(
                                ((Diagnosticable)newChildren[(int)i]).toDiagnosticsNode(
                                    name: "New child",
                                    style: DiagnosticsTreeStyle.singleLine
                                )
                            );
                        }
                    }
                }
                if (checked((long)mutationErrors.Count) != 0)
                {
                    throw new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            new ErrorSummary(
                                "Failed to replace child semantics nodes because the list of `SemanticsNode`s was mutated."
                            ),
                            new ErrorHint(
                                "Instead of mutating the existing list, create a new list containing the desired `SemanticsNode`s."
                            ),
                            new ErrorDescription("Error details:"),
                        }
                    );
                }
            }
            _debugPreviousSnapshot = new List<SemanticsNode>(newChildren);
            var ancestor = this;
            while (ancestor.parent is SemanticsNode)
            {
                ancestor = ancestor.parent!;
            }
            DartRuntimePrimitives.Assert(() =>
                !newChildren.any((child) => Equals(child, ancestor))
            );
            return true;
        });
        if (!sawChange && (_children is not null))
        {
            DartRuntimePrimitives.Assert(() =>
                checked(newChildren.Count) == checked((long)_children!.Count)
            );
            for (var iLocal = 0L; iLocal < checked(_children!.Count); iLocal++)
            {
                if (_children![(int)iLocal].id != newChildren[(int)iLocal].id)
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

    public virtual bool hasChildren =>
        (((long?)(_children?.Count)) is { } __count116564 ? __count116564 != 0 : (bool?)null)
        ?? false;
    public virtual long childrenCount => hasChildren ? checked(_children!.Count) : 0L;
    public virtual long childrenCountInTraversalOrder => checked(_childrenInTraversalOrder().Count);

    public virtual void visitChildren(Func<SemanticsNode, bool> visitor)
    {
        if (_children is not null)
        {
            foreach (SemanticsNode child in _children!)
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
        if (_children is not null)
        {
            foreach (SemanticsNode child in _children!)
            {
                if (!visitor(child) || !child._visitDescendants(visitor))
                {
                    return false;
                }
            }
        }
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual SemanticsOwner? owner => _owner;
    public virtual bool attached => _owner is not null;
    public virtual SemanticsNode? parent => _parent;
    public virtual SemanticsNode? traversalParent
    {
        get => _traversalParent ?? parent;
        set
        {
            var __value = value;
            if (Equals(_traversalParent, __value))
            {
                return;
            }
            _traversalParent = __value;
            _markDirty();
        }
    }
    public virtual long depth => _depth;

    internal virtual void _redepthChild(SemanticsNode child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.owner, owner));
        if (child._depth <= _depth)
        {
            child._depth = _depth + 1L;
            child._redepthChildren();
        }
    }

    internal virtual void _redepthChildren()
    {
        _children?.forEach(_redepthChild);
    }

    internal virtual void _updateChildMergeFlagRecursively(SemanticsNode child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.owner, owner));
        bool childShouldMergeToParent = isPartOfNodeMerging;
        if (childShouldMergeToParent == child.isMergedIntoParent)
        {
            return;
        }
        child.isMergedIntoParent = childShouldMergeToParent;
        if (child.mergeAllDescendantsIntoThisNode) { }
        else
        {
            child._updateChildrenMergeFlags();
        }
    }

    internal virtual void _updateChildrenMergeFlags()
    {
        _children?.forEach(_updateChildMergeFlagRecursively);
    }

    internal virtual void _adoptChild(SemanticsNode child)
    {
        DartRuntimePrimitives.Assert(() => child._parent is null);
        DartRuntimePrimitives.Assert(() =>
        {
            var node = this;
            while (node.parent is not null)
            {
                node = node.parent!;
            }
            DartRuntimePrimitives.Assert(() => !Equals(node, child));
            return true;
        });
        child._parent = this;
        if (attached)
        {
            child.attach(_owner!);
        }
        _redepthChild(child);
        _updateChildMergeFlagRecursively(child);
    }

    internal virtual void _dropChild(SemanticsNode child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child._parent, this));
        DartRuntimePrimitives.Assert(() => child.attached == attached);
        child._parent = null;
        if (attached)
        {
            child.detach();
        }
    }

    public virtual void attach(SemanticsOwner owner)
    {
        DartRuntimePrimitives.Assert(() => _owner is null);
        _owner = owner;
        while (owner._nodes.ContainsKey(id))
        {
            _id = _generateNewId();
        }
        owner._nodes[id] = this;
        owner._detachedNodes.Remove(this);
        if (_dirty)
        {
            _dirty = false;
            _markDirty();
        }
        if (_children is not null)
        {
            foreach (SemanticsNode child in _children!)
            {
                child.attach(owner);
            }
        }
    }

    public virtual void detach()
    {
        DartRuntimePrimitives.Assert(() => _owner is not null);
        DartRuntimePrimitives.Assert(() => owner!._nodes.ContainsKey(id));
        DartRuntimePrimitives.Assert(() => !owner!._detachedNodes.Contains(this));
        owner!._nodes.remove(id);
        owner!._detachedNodes.Add(this);
        if (_traversalChildIdentifier is object identifier)
        {
            owner!._traversalParentNodes.GetValueOrDefault(identifier)?._markDirty();
        }
        owner!._traversalParentNodes.removeWhere((key, node) => Equals(node, this));
        foreach (HashSet<SemanticsNode> childSet in owner!._traversalChildNodes.Values)
        {
            childSet.removeWhere((node) => Equals(node, this));
        }
        owner!._traversalChildNodes.removeWhere((key, value) => checked((long)value.Count) == 0);
        _owner = null;
        DartRuntimePrimitives.Assert(() => (parent is null) || (attached == parent!.attached));
        if (_children is not null)
        {
            foreach (SemanticsNode child in _children!)
            {
                if (Equals(child.parent, this))
                {
                    child.detach();
                }
            }
        }
        _markDirty();
    }

    internal virtual void _markDirty()
    {
        if (_dirty)
        {
            return;
        }
        _dirty = true;
        if (attached)
        {
            DartRuntimePrimitives.Assert(() => !owner!._detachedNodes.Contains(this));
            owner!._dirtyNodes.Add(this);
        }
    }

    public virtual bool? debugIsDirty
    {
        get
        {
            bool? isDirty = default!;
            DartRuntimePrimitives.Assert(() =>
            {
                isDirty = _dirty;
                return true;
            });
            return isDirty;
        }
    }

    internal virtual bool _isDifferentFromCurrentSemanticAnnotation(SemanticsConfiguration config)
    {
        return (!Equals(_attributedLabel, config.attributedLabel))
            || (!Equals(_attributedHint, config.attributedHint))
            || (!Equals(_attributedValue, config.attributedValue))
            || (!Equals(_attributedIncreasedValue, config.attributedIncreasedValue))
            || (!Equals(_attributedDecreasedValue, config.attributedDecreasedValue))
            || (_tooltip != config.tooltip)
            || (!Equals(_flags, config._flags))
            || (!Equals(_textDirection, config.textDirection))
            || (!Equals(_sortKey, config._sortKey))
            || (!Equals(_textSelection, config._textSelection))
            || (_scrollPosition != config._scrollPosition)
            || (_scrollExtentMax != config._scrollExtentMax)
            || (_scrollExtentMin != config._scrollExtentMin)
            || (_actionsAsBits != config._actionsAsBits)
            || (indexInParent != config.indexInParent)
            || (platformViewId != config.platformViewId)
            || (_maxValueLength != config._maxValueLength)
            || (_currentValueLength != config._currentValueLength)
            || (_mergeAllDescendantsIntoThisNode != config.isMergingSemanticsOfDescendants)
            || (_areUserActionsBlocked != config.isBlockingUserActions)
            || (_headingLevel != config._headingLevel)
            || (!Equals(_linkUrl, config._linkUrl))
            || (!Equals(_role, config.role))
            || (!Equals(_validationResult, config.validationResult))
            || (!Equals(_hitTestBehavior, config.hitTestBehavior))
            || (!Equals(_traversalChildIdentifier, config._traversalChildIdentifier))
            || (!Equals(_traversalParentIdentifier, config._traversalParentIdentifier))
            || (_minValue != config._minValue)
            || (_maxValue != config._maxValue)
            || !CollectionsLibrary.mapEquals(
                _customSemanticsActions,
                config._customSemanticsActions
            );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual long _effectiveActionsAsBits =>
        _areUserActionsBlocked
            ? (_actionsAsBits & SemanticsLibrary._kUnblockedUserActions)
            : _actionsAsBits;

    public virtual bool isTagged(SemanticsTag tag) => (tags is not null) && tags!.Contains(tag);

    public virtual SemanticsFlags flagsCollection => _flags;
    internal virtual long _flagsBitMask => SemanticsLibrary._toBitMask(flagsCollection);

    public virtual bool hasFlag(SemanticsFlag flag) =>
        (_flagsBitMask & FoundationRuntimePorts.EnumIndex(flag)) != 0L;

    public virtual string identifier => _identifier;
    public virtual object? traversalParentIdentifier => _traversalParentIdentifier;
    public virtual object? traversalChildIdentifier => _traversalChildIdentifier;
    internal virtual bool _isTraversalParent => _traversalParentIdentifier is not null;
    internal virtual bool _isTraversalChild => _traversalChildIdentifier is not null;
    public virtual string label => _attributedLabel.@string;
    public virtual AttributedString attributedLabel => _attributedLabel;
    public virtual string value => _attributedValue.@string;
    public virtual AttributedString attributedValue => _attributedValue;
    public virtual string increasedValue => _attributedIncreasedValue.@string;
    public virtual AttributedString attributedIncreasedValue => _attributedIncreasedValue;
    public virtual string decreasedValue => _attributedDecreasedValue.@string;
    public virtual AttributedString attributedDecreasedValue => _attributedDecreasedValue;
    public virtual string hint => _attributedHint.@string;
    public virtual AttributedString attributedHint => _attributedHint;
    public virtual string tooltip => _tooltip;
    public virtual SemanticsHintOverrides? hintOverrides => _hintOverrides;
    public virtual TextDirection? textDirection => _textDirection;
    public virtual SemanticsSortKey? sortKey => _sortKey;
    public virtual TextSelection? textSelection => _textSelection;
    public virtual bool? isMultiline => _isMultiline;
    public virtual long? scrollChildCount => _scrollChildCount;
    public virtual long? scrollIndex => _scrollIndex;
    public virtual double? scrollPosition => _scrollPosition;
    public virtual double? scrollExtentMax => _scrollExtentMax;
    public virtual double? scrollExtentMin => _scrollExtentMin;
    public virtual long? platformViewId => _platformViewId;
    public virtual long? maxValueLength => _maxValueLength;
    public virtual long? currentValueLength => _currentValueLength;
    public virtual long headingLevel => _headingLevel;
    public virtual DartUri? linkUrl => _linkUrl;
    public virtual SemanticsRole role => _role;
    public virtual HashSet<string>? controlsNodes => _controlsNodes;
    public virtual string? minValue => _minValue;
    public virtual string? maxValue => _maxValue;
    public virtual SemanticsValidationResult validationResult => _validationResult;
    public virtual SemanticsHitTestBehavior hitTestBehavior => _hitTestBehavior;
    public virtual SemanticsInputType inputType => _inputType;

    internal virtual bool _canPerformAction(SemanticsAction action) => _actions.ContainsKey(action);

    internal virtual bool _canPerformCustomAction(long actionId)
    {
        CustomSemanticsAction? customAction = CustomSemanticsAction.getAction(actionId);
        return (customAction is not null) && _customSemanticsActions.ContainsKey(customAction);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual bool _canHandleAction(SemanticsAction action, object? args)
    {
        if (Equals(action, SemanticsAction.customAction))
        {
            return (args is long) && _canPerformCustomAction(((long)args));
        }
        return _canPerformAction(action);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void updateWith(
        SemanticsConfiguration? config,
        List<SemanticsNode>? childrenInInversePaintOrder = null
    )
    {
        config ??= _kEmptyConfig;
        if (_isDifferentFromCurrentSemanticAnnotation(config))
        {
            _markDirty();
        }
        DartRuntimePrimitives.Assert(() =>
            (config.platformViewId is null)
            || (childrenInInversePaintOrder is null)
            || (checked((long)childrenInInversePaintOrder.Count) == 0)
        );
        var mergeAllDescendantsIntoThisNodeValueChanged =
            _mergeAllDescendantsIntoThisNode != config.isMergingSemanticsOfDescendants;
        _identifier = config.identifier;
        _traversalParentIdentifier = config.traversalParentIdentifier;
        _traversalChildIdentifier = config.traversalChildIdentifier;
        _attributedLabel = config.attributedLabel;
        _attributedValue = config.attributedValue;
        _attributedIncreasedValue = config.attributedIncreasedValue;
        _attributedDecreasedValue = config.attributedDecreasedValue;
        _attributedHint = config.attributedHint;
        _tooltip = config.tooltip;
        _hintOverrides = config.hintOverrides;
        _flags = config._flags;
        _textDirection = config.textDirection;
        _sortKey = config.sortKey;
        _actions = new DartMap<SemanticsAction, Action<object?>>(config._actions);
        _customSemanticsActions = new DartMap<CustomSemanticsAction, Action>(
            config._customSemanticsActions
        );
        _actionsAsBits = config._actionsAsBits;
        _textSelection = config._textSelection;
        _isMultiline = config.isMultiline;
        _scrollPosition = config._scrollPosition;
        _scrollExtentMax = config._scrollExtentMax;
        _scrollExtentMin = config._scrollExtentMin;
        _mergeAllDescendantsIntoThisNode = config.isMergingSemanticsOfDescendants;
        _scrollChildCount = config.scrollChildCount;
        _scrollIndex = config.scrollIndex;
        indexInParent = config.indexInParent;
        _platformViewId = config._platformViewId;
        _maxValueLength = config._maxValueLength;
        _currentValueLength = config._currentValueLength;
        _areUserActionsBlocked = config.isBlockingUserActions;
        _headingLevel = config._headingLevel;
        _linkUrl = config._linkUrl;
        _role = config._role;
        _controlsNodes = config._controlsNodes;
        _validationResult = config._validationResult;
        _hitTestBehavior = config._hitTestBehavior;
        _inputType = config._inputType;
        _locale = config.locale;
        _minValue = config.minValue;
        _maxValue = config.maxValue;
        _replaceChildren(childrenInInversePaintOrder ?? new List<SemanticsNode>());
        if (mergeAllDescendantsIntoThisNodeValueChanged)
        {
            _updateChildrenMergeFlags();
        }
        DartRuntimePrimitives.Assert(() =>
            !_canPerformAction(SemanticsAction.increase) || (value == "" == (increasedValue == ""))
        );
        DartRuntimePrimitives.Assert(() =>
            !_canPerformAction(SemanticsAction.decrease) || (value == "" == (decreasedValue == ""))
        );
    }

    public virtual SemanticsData getSemanticsData()
    {
        SemanticsFlags flags = _flags;
        long actionsLocal = _actionsAsBits;
        string identifierLocal = _identifier;
        object? traversalParentIdentifierLocal = _traversalParentIdentifier;
        object? traversalChildIdentifierLocal = _traversalChildIdentifier;
        AttributedString attributedLabelLocal = _attributedLabel;
        AttributedString attributedValueLocal = _attributedValue;
        AttributedString attributedIncreasedValueLocal = _attributedIncreasedValue;
        AttributedString attributedDecreasedValueLocal = _attributedDecreasedValue;
        AttributedString attributedHintLocal = _attributedHint;
        string tooltipLocal = _tooltip;
        TextDirection? textDirectionLocal = _textDirection;
        HashSet<SemanticsTag>? mergedTags =
            (tags is null) ? null : new HashSet<SemanticsTag>(tags!);
        TextSelection? textSelectionLocal = _textSelection;
        long? scrollChildCountLocal = _scrollChildCount;
        long? scrollIndexLocal = _scrollIndex;
        double? scrollPositionLocal = _scrollPosition;
        double? scrollExtentMaxLocal = _scrollExtentMax;
        double? scrollExtentMinLocal = _scrollExtentMin;
        long? platformViewIdLocal = _platformViewId;
        long? maxValueLengthLocal = _maxValueLength;
        long? currentValueLengthLocal = _currentValueLength;
        long headingLevelLocal = _headingLevel;
        DartUri? linkUrlLocal = _linkUrl;
        SemanticsRole roleLocal = _role;
        HashSet<string>? controlsNodesLocal = _controlsNodes;
        SemanticsValidationResult validationResultLocal = _validationResult;
        SemanticsHitTestBehavior hitTestBehaviorLocal = _hitTestBehavior;
        SemanticsInputType inputTypeLocal = _inputType;
        Locale? localeLocal = _locale;
        var customSemanticsActionIdsLocal = new HashSet<long>();
        string? minValueLocal = _minValue;
        string? maxValueLocal = _maxValue;
        foreach (CustomSemanticsAction actionLocal in _customSemanticsActions.Keys)
        {
            customSemanticsActionIdsLocal.Add(CustomSemanticsAction.getIdentifier(actionLocal));
        }
        if (hintOverrides is not null)
        {
            if (hintOverrides!.onTapHint is not null)
            {
                var actionAlternate = CustomSemanticsAction.CreateOverridingAction(
                    hint: hintOverrides!.onTapHint!,
                    action: SemanticsAction.tap
                );
                customSemanticsActionIdsLocal.Add(
                    CustomSemanticsAction.getIdentifier(actionAlternate)
                );
            }
            if (hintOverrides!.onLongPressHint is not null)
            {
                var actionNested = CustomSemanticsAction.CreateOverridingAction(
                    hint: hintOverrides!.onLongPressHint!,
                    action: SemanticsAction.longPress
                );
                customSemanticsActionIdsLocal.Add(
                    CustomSemanticsAction.getIdentifier(actionNested)
                );
            }
        }
        if (mergeAllDescendantsIntoThisNode)
        {
            _visitDescendants(
                (node) =>
                {
                    DartRuntimePrimitives.Assert(() => node.isMergedIntoParent);
                    flags = flags.merge(node._flags);
                    actionsLocal |= node._effectiveActionsAsBits;
                    textDirectionLocal ??= node._textDirection;
                    textSelectionLocal ??= node._textSelection;
                    scrollChildCountLocal ??= node._scrollChildCount;
                    scrollIndexLocal ??= node._scrollIndex;
                    scrollPositionLocal ??= node._scrollPosition;
                    scrollExtentMaxLocal ??= node._scrollExtentMax;
                    scrollExtentMinLocal ??= node._scrollExtentMin;
                    platformViewIdLocal ??= node._platformViewId;
                    maxValueLengthLocal ??= node._maxValueLength;
                    currentValueLengthLocal ??= node._currentValueLength;
                    linkUrlLocal ??= node._linkUrl;
                    headingLevelLocal = SemanticsLibrary._mergeHeadingLevels(
                        sourceLevel: node._headingLevel,
                        targetLevel: headingLevelLocal
                    );
                    if (identifierLocal == "")
                    {
                        identifierLocal = node._identifier;
                    }
                    traversalParentIdentifierLocal ??= node.traversalParentIdentifier;
                    traversalChildIdentifierLocal ??= node.traversalChildIdentifier;
                    if (attributedValueLocal.@string == "")
                    {
                        attributedValueLocal = node._attributedValue;
                    }
                    if (attributedIncreasedValueLocal.@string == "")
                    {
                        attributedIncreasedValueLocal = node._attributedIncreasedValue;
                    }
                    if (attributedDecreasedValueLocal.@string == "")
                    {
                        attributedDecreasedValueLocal = node._attributedDecreasedValue;
                    }
                    if (Equals(roleLocal, SemanticsRole.none))
                    {
                        roleLocal = node._role;
                    }
                    if (Equals(inputTypeLocal, SemanticsInputType.none))
                    {
                        inputTypeLocal = node._inputType;
                    }
                    if (Equals(hitTestBehaviorLocal, DorotiUiLibrary.SemanticsHitTestBehavior.defer))
                    {
                        hitTestBehaviorLocal = node._hitTestBehavior;
                    }
                    if (tooltipLocal == "")
                    {
                        tooltipLocal = node._tooltip;
                    }
                    if (node.tags is not null)
                    {
                        mergedTags ??= new HashSet<SemanticsTag>();
                        mergedTags!.UnionWith(node.tags!);
                    }
                    foreach (
                        CustomSemanticsAction actionCurrent in node._customSemanticsActions.Keys
                    )
                    {
                        customSemanticsActionIdsLocal.Add(
                            CustomSemanticsAction.getIdentifier(actionCurrent)
                        );
                    }
                    if (node.hintOverrides is not null)
                    {
                        if (node.hintOverrides!.onTapHint is not null)
                        {
                            var actionNext = CustomSemanticsAction.CreateOverridingAction(
                                hint: node.hintOverrides!.onTapHint!,
                                action: SemanticsAction.tap
                            );
                            customSemanticsActionIdsLocal.Add(
                                CustomSemanticsAction.getIdentifier(actionNext)
                            );
                        }
                        if (node.hintOverrides!.onLongPressHint is not null)
                        {
                            var actionCandidate = CustomSemanticsAction.CreateOverridingAction(
                                hint: node.hintOverrides!.onLongPressHint!,
                                action: SemanticsAction.longPress
                            );
                            customSemanticsActionIdsLocal.Add(
                                CustomSemanticsAction.getIdentifier(actionCandidate)
                            );
                        }
                    }
                    attributedLabelLocal = SemanticsLibrary._concatAttributedString(
                        thisAttributedString: attributedLabelLocal,
                        thisTextDirection: textDirectionLocal,
                        otherAttributedString: node._attributedLabel,
                        otherTextDirection: node._textDirection
                    );
                    attributedHintLocal = SemanticsLibrary._concatAttributedString(
                        thisAttributedString: attributedHintLocal,
                        thisTextDirection: textDirectionLocal,
                        otherAttributedString: node._attributedHint,
                        otherTextDirection: node._textDirection
                    );
                    if (controlsNodesLocal is null)
                    {
                        controlsNodesLocal = node._controlsNodes;
                    }
                    else
                    {
                        if (node._controlsNodes is not null)
                        {
                            controlsNodesLocal = new HashSet<string>();
                        }
                    }
                    minValueLocal ??= node._minValue;
                    maxValueLocal ??= node._maxValue;
                    if (Equals(validationResultLocal, SemanticsValidationResult.none))
                    {
                        validationResultLocal = node._validationResult;
                    }
                    else
                    {
                        if (Equals(validationResultLocal, SemanticsValidationResult.valid))
                        {
                            if (
                                (!Equals(node._validationResult, SemanticsValidationResult.none))
                                && (
                                    !Equals(node._validationResult, SemanticsValidationResult.valid)
                                )
                            )
                            {
                                validationResultLocal = node._validationResult;
                            }
                        }
                    }
                    return true;
                }
            );
        }
        return new SemanticsData(
            flagsCollection: flags,
            actions: _areUserActionsBlocked
                ? (actionsLocal & SemanticsLibrary._kUnblockedUserActions)
                : actionsLocal,
            identifier: identifierLocal,
            traversalParentIdentifier: traversalParentIdentifierLocal,
            traversalChildIdentifier: traversalChildIdentifierLocal,
            attributedLabel: attributedLabelLocal,
            attributedValue: attributedValueLocal,
            attributedIncreasedValue: attributedIncreasedValueLocal,
            attributedDecreasedValue: attributedDecreasedValueLocal,
            attributedHint: attributedHintLocal,
            tooltip: tooltipLocal,
            textDirection: textDirectionLocal,
            rect: rect,
            transform: transform,
            tags: mergedTags,
            textSelection: textSelectionLocal,
            scrollChildCount: scrollChildCountLocal,
            scrollIndex: scrollIndexLocal,
            scrollPosition: scrollPositionLocal,
            scrollExtentMax: scrollExtentMaxLocal,
            scrollExtentMin: scrollExtentMinLocal,
            platformViewId: platformViewIdLocal,
            maxValueLength: maxValueLengthLocal,
            currentValueLength: currentValueLengthLocal,
            customSemanticsActionIds: (
                (Func<List<long>>)(
                    () =>
                    {
                        var __cascade = customSemanticsActionIdsLocal.ToList();
                        __cascade.sort();
                        return __cascade;
                    }
                )
            )(),
            headingLevel: headingLevelLocal,
            linkUrl: linkUrlLocal,
            role: roleLocal,
            controlsNodes: controlsNodesLocal,
            validationResult: validationResultLocal,
            hitTestBehavior: hitTestBehaviorLocal,
            inputType: inputTypeLocal,
            locale: localeLocal,
            minValue: minValueLocal,
            maxValue: maxValueLocal
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static Matrix4 _computeTraversalTransform(SemanticsNode parent, SemanticsNode child)
    {
        var traversalTransform = Matrix4.identity();
        Matrix4? parentToCommonAncestorTransform = default!;
        var fromNode = child;
        var toNode = parent;
        while (!DartRuntimePrimitives.Identical(fromNode, toNode))
        {
            long fromDepth = fromNode.depth;
            long toDepth = toNode.depth;
            if (fromDepth >= toDepth)
            {
                if (fromNode.transform is Matrix4 transformLocal)
                {
                    traversalTransform.multiply(transformLocal);
                }
                fromNode = fromNode.parent!;
            }
            if (fromDepth <= toDepth)
            {
                parentToCommonAncestorTransform ??= Matrix4.identity();
                if (toNode.transform is Matrix4 transformAlternate)
                {
                    parentToCommonAncestorTransform.multiply(transformAlternate);
                }
                toNode = toNode.parent!;
            }
        }
        if (parentToCommonAncestorTransform is not null)
        {
            if (parentToCommonAncestorTransform.invert() != 0L)
            {
                traversalTransform.multiply(parentToCommonAncestorTransform);
            }
            else
            {
                traversalTransform.setZero();
            }
        }
        return traversalTransform;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual int[] _childrenIdInTraversalOrder()
    {
        List<SemanticsNode> sortedChildren = _childrenInTraversalOrder();
        var childrenInTraversalOrder = new int[sortedChildren.Count];
        for (var i = 0L; i < checked(sortedChildren.Count); i += 1L)
        {
            childrenInTraversalOrder[i] = checked((int)sortedChildren[(int)i].id);
        }
        return childrenInTraversalOrder;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual List<SemanticsNode> _childrenInHitTestOrder()
    {
        if (_children is null)
        {
            return new List<SemanticsNode>();
        }
        if (ConstantsLibrary.kIsWeb || _isTraversalParent)
        {
            return _children!;
        }
        bool shouldNotSkipInHitTest(SemanticsNode child)
        {
            if (child._isTraversalChild)
            {
                SemanticsNode? traversalParent = owner!._traversalParentNodes.GetValueOrDefault(
                    DartRuntimePrimitives.RequireReference(
                        child.getSemanticsData().traversalChildIdentifier
                    )
                );
                return traversalParent is not null;
            }
            return true;
            throw new InvalidOperationException(
                "Control flow completed without returning a value."
            );
        }
        return _children!.where(shouldNotSkipInHitTest).ToList();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual int[] _childrenIdInHitTestOrder()
    {
        List<SemanticsNode> children = _childrenInHitTestOrder();
        return Enumerable.Reverse(children).Select(node => checked((int)node.id)).ToArray();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _addToUpdate(
        SemanticsUpdateBuilder builder,
        HashSet<long> customSemanticsActionIdsUpdate
    )
    {
        DartRuntimePrimitives.Assert(() => _dirty);
        SemanticsData data = getSemanticsData();
        DartRuntimePrimitives.Assert(() =>
        {
            FlutterError? error = _DebugSemanticsRoleChecks__semantics._checkSemanticsData(this);
            if (error is not null)
            {
                throw error;
            }
            return true;
        });
        int[] childrenInTraversalOrderLocal = default!;
        int[] childrenInHitTestOrderLocal = default!;
        if (!hasChildren || mergeAllDescendantsIntoThisNode)
        {
            if (_isTraversalParent && !ConstantsLibrary.kIsWeb)
            {
                if (
                    this.owner is { } owner
                    && traversalParentIdentifier is { } parentIdentifier
                    && owner._traversalChildNodes.ContainsKey(parentIdentifier)
                )
                {
                    HashSet<SemanticsNode> traversalChildren =
                        owner._traversalChildNodes.GetValueOrDefault(parentIdentifier)!;
                    var index = 0L;
                    childrenInTraversalOrderLocal = new int[traversalChildren.Count];
                    foreach (var node in traversalChildren)
                    {
                        if (node.attached)
                        {
                            childrenInTraversalOrderLocal[index] = checked((int)node.id);
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
        int[]? customSemanticsActionIdsLocal = default!;
        if (
            (
                ((long?)(data.customSemanticsActionIds?.Count)) is { } __count156027
                    ? __count156027 != 0
                    : (bool?)null
            ) ?? false
        )
        {
            customSemanticsActionIdsLocal = new int[data.customSemanticsActionIds!.Count];
            for (var i = 0L; i < checked(data.customSemanticsActionIds!.Count); i++)
            {
                customSemanticsActionIdsLocal[i] = checked(
                    (int)data.customSemanticsActionIds![(int)i]
                );
                customSemanticsActionIdsUpdate.Add(data.customSemanticsActionIds![(int)i]);
            }
        }
        var traversalParentId = -1L;
        if (data.traversalChildIdentifier is object identifierLocal)
        {
            if (
                owner!._traversalParentNodes.GetValueOrDefault(identifierLocal)
                is SemanticsNode parentNode
            )
            {
                traversalParentId = parentNode.id;
            }
        }
        object? childIdentifier = traversalChildIdentifier;
        if (childIdentifier is not null)
        {
            traversalParent = owner!._traversalParentNodes.GetValueOrDefault(childIdentifier);
            if (!ConstantsLibrary.kIsWeb)
            {
                _traversalChildTransform = _computeTraversalTransform(
                    parent: traversalParent!,
                    child: this
                );
            }
        }
        builder.updateNode(
            id: id,
            flags: data.flagsCollection,
            actions: data.actions,
            rect: data.rect,
            identifier: data.identifier,
            label: data.attributedLabel.@string,
            labelAttributes: data.attributedLabel.attributes,
            value: data.attributedValue.@string,
            valueAttributes: data.attributedValue.attributes,
            increasedValue: data.attributedIncreasedValue.@string,
            increasedValueAttributes: data.attributedIncreasedValue.attributes,
            decreasedValue: data.attributedDecreasedValue.@string,
            decreasedValueAttributes: data.attributedDecreasedValue.attributes,
            hint: data.attributedHint.@string,
            hintAttributes: data.attributedHint.attributes,
            tooltip: data.tooltip,
            textDirection: data.textDirection,
            textSelectionBase: (data.textSelection is not null)
                ? data.textSelection!.baseOffset
                : -1L,
            textSelectionExtent: (data.textSelection is not null)
                ? data.textSelection!.extentOffset
                : -1L,
            platformViewId: data.platformViewId ?? -1L,
            maxValueLength: data.maxValueLength ?? -1L,
            currentValueLength: data.currentValueLength ?? -1L,
            scrollChildren: data.scrollChildCount ?? 0L,
            scrollIndex: data.scrollIndex ?? 0L,
            scrollPosition: data.scrollPosition ?? double.NaN,
            scrollExtentMax: data.scrollExtentMax ?? double.NaN,
            scrollExtentMin: data.scrollExtentMin ?? double.NaN,
            transform: (_traversalTransform ?? _kIdentityTransform).storage,
            traversalParent: traversalParentId,
            hitTestTransform: (data.transform ?? _kIdentityTransform).storage,
            childrenInTraversalOrder: childrenInTraversalOrderLocal,
            childrenInHitTestOrder: childrenInHitTestOrderLocal,
            additionalActions: customSemanticsActionIdsLocal ?? _kEmptyCustomSemanticsActionsList,
            headingLevel: data.headingLevel,
            linkUrl: data.linkUrl?.ToString() ?? "",
            role: data.role,
            controlsNodes: data.controlsNodes?.ToList(),
            validationResult: data.validationResult,
            hitTestBehavior: data.hitTestBehavior,
            inputType: data.inputType,
            locale: data.locale,
            minValue: data.minValue ?? "",
            maxValue: data.maxValue ?? ""
        );
        _dirty = false;
    }

    internal virtual List<SemanticsNode>? _updateChildrenInTraversalOrder()
    {
        if (ConstantsLibrary.kIsWeb)
        {
            return _children;
        }
        var updatedChildren = new List<SemanticsNode>();
        foreach (SemanticsNode child in _children!)
        {
            if (child._isTraversalChild && !_isTraversalParent)
            {
                SemanticsNode? traversalParent = owner!._traversalParentNodes.GetValueOrDefault(
                    DartRuntimePrimitives.RequireReference(
                        child.getSemanticsData().traversalChildIdentifier
                    )
                );
                long? traversalParentId = traversalParent?.id;
                while (traversalParent is not null)
                {
                    if (Equals(traversalParent, child))
                    {
                        throw new FlutterError(
                            $"The traversalParent__160618 {traversalParentId} cannot be the child of the traversalChild {child.id} in hit-test order"
                        );
                    }
                    traversalParent = traversalParent.parent;
                }
                continue;
            }
            updatedChildren.Add(child);
        }
        if (_isTraversalParent)
        {
            HashSet<SemanticsNode>? traversalChildren =
                owner?._traversalChildNodes.GetValueOrDefault(traversalParentIdentifier!);
            if (traversalChildren is not null)
            {
                var currentNode = this;
                while (currentNode.parent is not null)
                {
                    currentNode = currentNode.parent!;
                    if (traversalChildren.Contains(currentNode))
                    {
                        throw new FlutterError(
                            $"The traversalParent {id} cannot be the child of the traversalChild {currentNode.id} in hit-test order"
                        );
                    }
                }
                foreach (SemanticsNode node in traversalChildren)
                {
                    if (node.attached)
                    {
                        updatedChildren.Add(node);
                    }
                }
            }
        }
        return updatedChildren;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual List<SemanticsNode> _childrenInTraversalOrder()
    {
        List<SemanticsNode>? updatedChildren = _updateChildrenInTraversalOrder();
        TextDirection? inheritedTextDirection = textDirection;
        SemanticsNode? ancestor = parent;
        while ((inheritedTextDirection is null) && (ancestor is not null))
        {
            inheritedTextDirection = ancestor.textDirection;
            ancestor = ancestor.parent;
        }
        List<SemanticsNode>? childrenInDefaultOrder = default!;
        if (inheritedTextDirection is not null)
        {
            TextDirection inheritedTextDirection__162729__value163025 = (
                inheritedTextDirection
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            childrenInDefaultOrder = SemanticsLibrary._childrenInDefaultOrder(
                updatedChildren!,
                ((inheritedTextDirection__162729__value163025))
            );
        }
        else
        {
            childrenInDefaultOrder = updatedChildren;
        }
        var everythingSorted = new List<_TraversalSortNode__semantics>();
        var sortNodes = new List<_TraversalSortNode__semantics>();
        SemanticsSortKey? lastSortKey = default!;
        for (
            var positionLocal = 0L;
            positionLocal < checked(childrenInDefaultOrder!.Count);
            positionLocal += 1L
        )
        {
            SemanticsNode child = childrenInDefaultOrder[(int)positionLocal];
            SemanticsSortKey? sortKeyLocal = child.sortKey;
            lastSortKey =
                (positionLocal > 0L)
                    ? childrenInDefaultOrder[(int)(positionLocal - 1L)].sortKey
                    : null;
            bool isCompatibleWithPreviousSortKey =
                (positionLocal == 0L)
                || (
                    Equals(
                        DartRuntimePrimitives.RuntimeType(sortKeyLocal),
                        DartRuntimePrimitives.RuntimeType(lastSortKey)
                    ) && ((sortKeyLocal is null) || (sortKeyLocal.name == lastSortKey!.ToString()))
                );
            if (!isCompatibleWithPreviousSortKey && (checked((long)sortNodes.Count) != 0))
            {
                if (lastSortKey is not null)
                {
                    sortNodes.sort();
                }
                everythingSorted.AddRange(sortNodes);
                sortNodes.Clear();
            }
            sortNodes.Add(
                new _TraversalSortNode__semantics(
                    node: child,
                    sortKey: sortKeyLocal,
                    position: positionLocal
                )
            );
        }
        if (lastSortKey is not null)
        {
            sortNodes.sort();
        }
        everythingSorted.AddRange(sortNodes);
        return everythingSorted.map((sortNode) => sortNode.node).ToList();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void sendEvent(SemanticsEvent @event)
    {
        if (!attached)
        {
            return;
        }
        _ = SystemChannels
            .accessibility.send(@event.toMap(nodeId: id))
            .then(
                (_) => { },
                onError: (error, stack) =>
                {
                    FlutterError.reportError(
                        new FlutterErrorDetails(
                            exception: error,
                            stack: stack,
                            library: "semantics library",
                            context: new ErrorDescription("while sending accessibility event"),
                            informationCollector: () =>
                                new List<DiagnosticsNode>
                                {
                                    new DiagnosticsProperty<SemanticsEvent>("event", @event),
                                    new DiagnosticsProperty<SemanticsNode>("node", this),
                                }
                        )
                    );
                }
            );
    }

    internal virtual bool _debugIsActionBlocked(SemanticsAction action)
    {
        var result = false;
        DartRuntimePrimitives.Assert(() =>
        {
            result = (_effectiveActionsAsBits & (long)action) == 0L;
            return true;
        });
        return result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override string toStringShort() =>
        $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "SemanticsNode")}#{id}";

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        var hideOwner = true;
        if (_dirty)
        {
            bool inDirtyNodes = (owner is not null) && owner!._dirtyNodes.Contains(this);
            properties.add(
                new FlagProperty(
                    "inDirtyNodes",
                    value: inDirtyNodes,
                    ifTrue: "dirty",
                    ifFalse: "STALE"
                )
            );
            hideOwner = inDirtyNodes;
        }
        properties.add(
            new DiagnosticsProperty<SemanticsOwner>(
                "owner",
                owner,
                level: hideOwner ? DiagnosticLevel.hidden : DiagnosticLevel.info
            )
        );
        properties.add(
            new FlagProperty(
                "isMergedIntoParent",
                value: isMergedIntoParent,
                ifTrue: "merged up ⬆️"
            )
        );
        properties.add(
            new FlagProperty(
                "mergeAllDescendantsIntoThisNode",
                value: mergeAllDescendantsIntoThisNode,
                ifTrue: "merge boundary ⛔️"
            )
        );
        if (_locale is not null)
        {
            properties.add(new StringProperty("locale", _locale.ToString()));
        }
        Offset? offset = (transform is not null) ? MatrixUtils.getAsTranslation(transform!) : null;
        if (offset is not null)
        {
            Offset offset__167351__value167437 = (
                offset
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            properties.add(
                new DiagnosticsProperty<Rect>(
                    "rect",
                    rect.shift(((offset__167351__value167437))),
                    showName: false
                )
            );
        }
        else
        {
            double? scale = (transform is not null) ? MatrixUtils.getAsScale(transform!) : null;
            string? descriptionLocal = default!;
            if (scale is not null)
            {
                double scale__167582__value167690 = (
                    scale
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
                descriptionLocal =
                    $"{rect} scaled by {(scale__167582__value167690).toStringAsFixed(1L)}x";
            }
            else
            {
                if (
                    this.transform is { } transform
                    && !MatrixUtils.isIdentity(transform)
                    && transform.ToString() is { } matrixDescription
                )
                {
                    string matrix = string.Join(
                        "; ",
                        matrixDescription.split("\n").take(4L).map((line) => line.substring(4L))
                    );
                    descriptionLocal = $"{rect} with transform [{matrix}]";
                }
            }
            properties.add(
                new DiagnosticsProperty<Rect>(
                    "rect",
                    rect,
                    description: descriptionLocal,
                    showName: false
                )
            );
        }
        properties.add(
            new IterableProperty<string>("tags", tags?.map((tag) => tag.name), defaultValue: null)
        );
        List<string> actions = (
            (Func<List<string>>)(
                () =>
                {
                    var __cascade = _actions
                        .Keys.map(
                            (action) =>
                                $"{action.ToString()}{(_debugIsActionBlocked(action) ? "🚫️" : "")}"
                        )
                        .ToList();
                    __cascade.sort();
                    return __cascade;
                }
            )
        )();
        List<string?> customSemanticsActions = _customSemanticsActions
            .Keys.map((action) => action.label)
            .ToList();
        properties.add(new IterableProperty<string>("actions", actions, ifEmpty: null));
        properties.add(
            new IterableProperty<string?>("customActions", customSemanticsActions, ifEmpty: null)
        );
        properties.add(
            new IterableProperty<string>("flags", flagsCollection.toStrings(), ifEmpty: null)
        );
        properties.add(new FlagProperty("isInvisible", value: isInvisible, ifTrue: "invisible"));
        properties.add(
            new FlagProperty("isHidden", value: flagsCollection.isHidden, ifTrue: "HIDDEN")
        );
        properties.add(new StringProperty("identifier", _identifier, defaultValue: ""));
        properties.add(
            new DiagnosticsProperty<object>(
                "traversalParentIdentifier",
                traversalParentIdentifier,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<object>(
                "traversalChildIdentifier",
                traversalChildIdentifier,
                defaultValue: null
            )
        );
        properties.add(new AttributedStringProperty("label", _attributedLabel));
        properties.add(new AttributedStringProperty("value", _attributedValue));
        properties.add(new AttributedStringProperty("increasedValue", _attributedIncreasedValue));
        properties.add(new AttributedStringProperty("decreasedValue", _attributedDecreasedValue));
        properties.add(new AttributedStringProperty("hint", _attributedHint));
        properties.add(new StringProperty("tooltip", _tooltip, defaultValue: ""));
        properties.add(
            new EnumProperty<TextDirection>("textDirection", _textDirection, defaultValue: null)
        );
        if (!Equals(_role, SemanticsRole.none))
        {
            properties.add(new EnumProperty<SemanticsRole>("role", _role));
        }
        properties.add(
            new DiagnosticsProperty<SemanticsSortKey>("sortKey", sortKey, defaultValue: null)
        );
        if (_textSelection?.isValid ?? false)
        {
            properties.add(
                new MessageProperty(
                    "text selection",
                    $"[{_textSelection!.start}, {_textSelection!.end}]"
                )
            );
        }
        properties.add(new IntProperty("platformViewId", platformViewId, defaultValue: null));
        properties.add(new IntProperty("maxValueLength", maxValueLength, defaultValue: null));
        properties.add(
            new IntProperty("currentValueLength", currentValueLength, defaultValue: null)
        );
        properties.add(new IntProperty("scrollChildren", scrollChildCount, defaultValue: null));
        properties.add(new IntProperty("scrollIndex", scrollIndex, defaultValue: null));
        properties.add(new DoubleProperty("scrollExtentMin", scrollExtentMin, defaultValue: null));
        properties.add(new DoubleProperty("scrollPosition", scrollPosition, defaultValue: null));
        properties.add(new DoubleProperty("scrollExtentMax", scrollExtentMax, defaultValue: null));
        properties.add(new IntProperty("indexInParent", indexInParent, defaultValue: null));
        properties.add(new IntProperty("headingLevel", _headingLevel, defaultValue: 0L));
        if (!Equals(_inputType, SemanticsInputType.none))
        {
            properties.add(new EnumProperty<SemanticsInputType>("inputType", _inputType));
        }
        if (!Equals(validationResult, SemanticsValidationResult.none))
        {
            properties.add(
                new EnumProperty<SemanticsValidationResult>(
                    "validationResult",
                    validationResult,
                    defaultValue: SemanticsValidationResult.none
                )
            );
        }
        properties.add(new StringProperty("minValue", _minValue, defaultValue: null));
        properties.add(new StringProperty("maxValue", _maxValue, defaultValue: null));
    }

    public override string toStringDeep(
        string prefixLineOne = "",
        string? prefixOtherLines = null,
        DiagnosticLevel minLevel = DiagnosticLevel.debug,
        long? wrapWidth = 65
    ) =>
        toStringDeep(
            childOrder: DebugSemanticsDumpOrder.traversalOrder,
            prefixLineOne: prefixLineOne,
            prefixOtherLines: prefixOtherLines,
            minLevel: minLevel,
            wrapWidth: wrapWidth ?? 65
        );

    public virtual string toStringDeep(
        DebugSemanticsDumpOrder childOrder,
        string prefixLineOne = "",
        string? prefixOtherLines = null,
        DiagnosticLevel minLevel = DiagnosticLevel.debug,
        long wrapWidth = 65
    )
    {
        return toDiagnosticsNode(childOrder: childOrder)
            .toStringDeep(
                prefixLineOne: prefixLineOne,
                prefixOtherLines: prefixOtherLines,
                minLevel: minLevel,
                wrapWidth: wrapWidth
            );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(
        string? name = null,
        DiagnosticsTreeStyle? style = DiagnosticsTreeStyle.sparse,
        DebugSemanticsDumpOrder childOrder = DebugSemanticsDumpOrder.traversalOrder
    )
    {
        return new _SemanticsDiagnosticableNode__semantics(
            name: name,
            value: this,
            style: style,
            childOrder: childOrder
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override List<DiagnosticsNode> debugDescribeChildren() =>
        debugDescribeChildren(DebugSemanticsDumpOrder.traversalOrder);

    public virtual List<DiagnosticsNode> debugDescribeChildren(DebugSemanticsDumpOrder childOrder)
    {
        return debugListChildrenInOrder(childOrder)
            .map((node) => node.toDiagnosticsNode(childOrder: childOrder))
            .ToList();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual List<SemanticsNode> debugListChildrenInOrder(DebugSemanticsDumpOrder childOrder)
    {
        if (_children is null)
        {
            return new List<SemanticsNode>();
        }
        return childOrder switch
        {
            DebugSemanticsDumpOrder.inverseHitTest => _childrenInHitTestOrder(),
            DebugSemanticsDumpOrder.traversalOrder => _childrenInTraversalOrder(),
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
        return offset.CompareTo(other.offset);
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
        return startOffset.CompareTo(other.startOffset);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual List<SemanticsNode> sortedWithinVerticalGroup()
    {
        var edges = new List<_BoxEdge__semantics>();
        foreach (SemanticsNode child in nodes)
        {
            Rect childRect = child.rect.deflate(0.1);
            edges.Add(
                new _BoxEdge__semantics(
                    isLeadingEdge: true,
                    offset: SemanticsLibrary._pointInParentCoordinates(child, childRect.topLeft).dx,
                    node: child
                )
            );
            edges.Add(
                new _BoxEdge__semantics(
                    isLeadingEdge: false,
                    offset: SemanticsLibrary
                        ._pointInParentCoordinates(child, childRect.bottomRight)
                        .dx,
                    node: child
                )
            );
        }
        edges.sort();
        var horizontalGroups = new List<_SemanticsSortGroup__semantics>();
        _SemanticsSortGroup__semantics? groupLocal = default!;
        var depth = 0L;
        foreach (var edge in edges)
        {
            if (edge.isLeadingEdge)
            {
                depth += 1L;
                groupLocal ??= new _SemanticsSortGroup__semantics(
                    startOffset: edge.offset,
                    textDirection: textDirection
                );
                groupLocal.nodes.Add(edge.node);
            }
            else
            {
                depth -= 1L;
            }
            if (depth == 0L)
            {
                horizontalGroups.Add(groupLocal!);
                groupLocal = null;
            }
        }
        horizontalGroups.sort();
        if (Equals(textDirection, TextDirection.rtl))
        {
            horizontalGroups = Enumerable.Reverse(horizontalGroups).ToList();
        }
        return horizontalGroups.expand((group) => group.sortedWithinKnot()).ToList();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual List<SemanticsNode> sortedWithinKnot()
    {
        if (checked(nodes.Count) <= 1L)
        {
            return nodes;
        }
        var nodeMap = new DartMap<long, SemanticsNode>();
        var edges = new DartMap<long, long>();
        foreach (SemanticsNode nodeLocal in nodes)
        {
            nodeMap[nodeLocal.id] = nodeLocal;
            Offset centerLocal = SemanticsLibrary._pointInParentCoordinates(
                nodeLocal,
                nodeLocal.rect.center
            );
            foreach (SemanticsNode nextNode in nodes)
            {
                if (
                    DartRuntimePrimitives.Identical(nodeLocal, nextNode)
                    || (edges.GetValueOrDefault(nextNode.id) == nodeLocal.id)
                )
                {
                    continue;
                }
                Offset nextCenter = SemanticsLibrary._pointInParentCoordinates(
                    nextNode,
                    nextNode.rect.center
                );
                Offset centerDelta = nextCenter - centerLocal;
                double directionLocal = centerDelta.direction;
                bool isLtrAndForward =
                    Equals(textDirection, TextDirection.ltr)
                    && ((-Math.PI / 4L) < directionLocal)
                    && (directionLocal < (3L * Math.PI / 4L));
                bool isRtlAndForward =
                    Equals(textDirection, TextDirection.rtl)
                    && (
                        (directionLocal < (-3L * Math.PI / 4L))
                        || (directionLocal > (3L * Math.PI / 4L))
                    );
                if (isLtrAndForward || isRtlAndForward)
                {
                    edges[nodeLocal.id] = nextNode.id;
                }
            }
        }
        var sortedIds = new List<long>();
        var visitedIds = new HashSet<long>();
        List<SemanticsNode> startNodes = (
            (Func<List<SemanticsNode>>)(
                () =>
                {
                    var __cascade = nodes.ToList();
                    __cascade.sort(
                        (a, b) =>
                        {
                            Offset aTopLeft = SemanticsLibrary._pointInParentCoordinates(
                                a,
                                a.rect.topLeft
                            );
                            Offset bTopLeft = SemanticsLibrary._pointInParentCoordinates(
                                b,
                                b.rect.topLeft
                            );
                            long verticalDiff = aTopLeft.dy.CompareTo(bTopLeft.dy);
                            if (verticalDiff != 0L)
                            {
                                return -verticalDiff;
                            }
                            return -aTopLeft.dx.CompareTo(bTopLeft.dx);
                        }
                    );
                    return __cascade;
                }
            )
        )();
        void search(long id)
        {
            if (visitedIds.Contains(id))
            {
                return;
            }
            visitedIds.Add(id);
            if (edges.ContainsKey(id))
            {
                search((edges.GetValueOrDefault(id)));
            }
            sortedIds.Add(id);
        }
        startNodes.map((node) => node.id).forEach(search);
        return Enumerable
            .Reverse(sortedIds.map((id) => nodeMap.GetValueOrDefault(id)!).ToList())
            .ToList();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public int CompareTo(_SemanticsSortGroup__semantics? other) => checked((int)compareTo(other!));
}

public static partial class SemanticsLibrary
{
    internal static Offset _pointInParentCoordinates(SemanticsNode node, Offset point)
    {
        Matrix4? traversalTransform = node._traversalTransform;
        if (traversalTransform is null)
        {
            return point;
        }
        var vector = new Vector3(point.dx, point.dy, 0.0);
        traversalTransform.transform3(vector);
        return new Offset(vector.x, vector.y);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class SemanticsLibrary
{
    internal static List<SemanticsNode> _childrenInDefaultOrder(
        List<SemanticsNode> children,
        TextDirection textDirection
    )
    {
        var edges = new List<_BoxEdge__semantics>();
        foreach (var child in children)
        {
            DartRuntimePrimitives.Assert(() => child.rect.isFinite);
            Rect childRect = child.rect.deflate(0.1);
            edges.Add(
                new _BoxEdge__semantics(
                    isLeadingEdge: true,
                    offset: _pointInParentCoordinates(child, childRect.topLeft).dy,
                    node: child
                )
            );
            edges.Add(
                new _BoxEdge__semantics(
                    isLeadingEdge: false,
                    offset: _pointInParentCoordinates(child, childRect.bottomRight).dy,
                    node: child
                )
            );
        }
        edges.sort();
        var verticalGroups = new List<_SemanticsSortGroup__semantics>();
        _SemanticsSortGroup__semantics? groupLocal = default!;
        var depth = 0L;
        foreach (var edge in edges)
        {
            if (edge.isLeadingEdge)
            {
                depth += 1L;
                groupLocal ??= new _SemanticsSortGroup__semantics(
                    startOffset: edge.offset,
                    textDirection: textDirection
                );
                groupLocal.nodes.Add(edge.node);
            }
            else
            {
                depth -= 1L;
            }
            if (depth == 0L)
            {
                verticalGroups.Add(groupLocal!);
                groupLocal = null;
            }
        }
        verticalGroups.sort();
        return verticalGroups.expand((group) => group.sortedWithinVerticalGroup()).ToList();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _TraversalSortNode__semantics : IComparable<_TraversalSortNode__semantics>
{
    public virtual SemanticsNode node { get; private set; } = default!;
    public virtual SemanticsSortKey? sortKey { get; private set; }
    public virtual long position { get; private set; } = default!;

    internal _TraversalSortNode__semantics(
        SemanticsNode node,
        SemanticsSortKey? sortKey = null,
        long position = default!
    )
    {
        this.node = node;
        this.sortKey = sortKey;
        this.position = position;
    }

    public virtual long compareTo(_TraversalSortNode__semantics other)
    {
        if ((sortKey is null) || (other.sortKey is null))
        {
            return position - other.position;
        }
        return sortKey!.compareTo(other.sortKey!);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public int CompareTo(_TraversalSortNode__semantics? other) => checked((int)compareTo(other!));
}

public class SemanticsOwner : ChangeNotifier
{
    public virtual Action<SemanticsUpdate> onSemanticsUpdate { get; private set; } = default!;
    internal virtual HashSet<SemanticsNode> _dirtyNodes { get; private set; } =
        new HashSet<SemanticsNode>();
    internal virtual DartMap<long, SemanticsNode> _nodes { get; private set; } =
        new DartMap<long, SemanticsNode>();
    internal virtual HashSet<SemanticsNode> _detachedNodes { get; private set; } =
        new HashSet<SemanticsNode>();
    internal virtual DartMap<object, SemanticsNode> _traversalParentNodes { get; private set; } =
        new DartMap<object, SemanticsNode>();
    internal virtual DartMap<object, HashSet<SemanticsNode>> _traversalChildNodes
    {
        get;
        private set;
    } = new DartMap<object, HashSet<SemanticsNode>>();

    public SemanticsOwner(Action<SemanticsUpdate> onSemanticsUpdate)
    {
        this.onSemanticsUpdate = onSemanticsUpdate;
    }

    public virtual SemanticsNode? rootSemanticsNode => _nodes.GetValueOrDefault(0L);

    public virtual SemanticsNode? getSemanticsNode(long id) => _nodes.GetValueOrDefault(id);

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
            Foundation.DebugLibrary.debugMaybeDispatchDisposed(this)
        );
        _dirtyNodes.Clear();
        _nodes.Clear();
        _detachedNodes.Clear();
        _traversalChildNodes.Clear();
        _traversalParentNodes.Clear();
        base.dispose();
    }

    public virtual void sendSemanticsUpdate()
    {
        DartRuntimePrimitives.Assert(() =>
        {
            var invisibleNodes = new List<SemanticsNode>();
            bool findInvisibleNodes(SemanticsNode node)
            {
                if (node.rect.isEmpty)
                {
                    invisibleNodes.Add(node);
                }
                else
                {
                    if (!node.mergeAllDescendantsIntoThisNode)
                    {
                        node.visitChildren(findInvisibleNodes);
                    }
                }
                return true;
                throw new InvalidOperationException(
                    "Control flow completed without returning a value."
                );
            }
            SemanticsNode? rootSemanticsNodeLocal = rootSemanticsNode;
            if (rootSemanticsNodeLocal is not null)
            {
                if (
                    (rootSemanticsNodeLocal.childrenCount > 0L)
                    && rootSemanticsNodeLocal.rect.isEmpty
                )
                {
                    invisibleNodes.Add(rootSemanticsNodeLocal);
                }
                else
                {
                    if (!rootSemanticsNodeLocal.mergeAllDescendantsIntoThisNode)
                    {
                        rootSemanticsNodeLocal.visitChildren(findInvisibleNodes);
                    }
                }
            }
            if (checked((long)invisibleNodes.Count) == 0)
            {
                return true;
            }
            List<DiagnosticsNode> nodeToMessage(SemanticsNode invisibleNode)
            {
                SemanticsNode? parentLocal = invisibleNode.parent;
                return new List<DiagnosticsNode>
                {
                    invisibleNode.toDiagnosticsNode(style: DiagnosticsTreeStyle.errorProperty),
                    parentLocal?.toDiagnosticsNode(
                        name: "which was added as a child of",
                        style: DiagnosticsTreeStyle.errorProperty
                    ) ?? new ErrorDescription("which was added as the root SemanticsNode"),
                };
                throw new InvalidOperationException(
                    "Control flow completed without returning a value."
                );
            }
            throw new FlutterError([
                new ErrorSummary("Invisible SemanticsNodes should not be added to the tree."),
                new ErrorDescription(
                    "The following invisible SemanticsNodes were added to the tree:"
                ),
                .. invisibleNodes.SelectMany(nodeToMessage),
                new ErrorHint(
                    "An invisible SemanticsNode is one whose rect is not on screen hence not reachable for users, "
                        + "and its semantic information is not merged into a visible parent."
                ),
                new ErrorHint(
                    "An invisible SemanticsNode makes the accessibility experience confusing, "
                        + "as it does not provide any visual indication when the user selects it "
                        + "via accessibility technologies."
                ),
                new ErrorHint(
                    "Consider removing the above invisible SemanticsNodes if they were added by your "
                        + "RenderObject.assembleSemanticsNode implementation, or filing a bug on GitHub:\n"
                        + "  https://github.com/flutter/flutter/issues/new?template=02_bug.yml"
                ),
            ]);
        });
        if (checked((long)_dirtyNodes.Count) == 0)
        {
            return;
        }
        var customSemanticsActionIds = new HashSet<long>();
        var visitedNodes = new List<SemanticsNode>();
        while (checked((long)_dirtyNodes.Count) != 0)
        {
            List<SemanticsNode> localDirtyNodes = _dirtyNodes
                .where((node) => !_detachedNodes.Contains(node))
                .ToList();
            _dirtyNodes.Clear();
            _detachedNodes.Clear();
            localDirtyNodes.sort((a, b) => a.depth - b.depth);
            visitedNodes.AddRange(localDirtyNodes);
            foreach (var nodeLocal in localDirtyNodes)
            {
                DartRuntimePrimitives.Assert(() => nodeLocal._dirty);
                DartRuntimePrimitives.Assert(() =>
                    (nodeLocal.parent is null)
                    || !nodeLocal.parent!.isPartOfNodeMerging
                    || nodeLocal.isMergedIntoParent
                );
                if (nodeLocal.isPartOfNodeMerging)
                {
                    DartRuntimePrimitives.Assert(() =>
                        nodeLocal.mergeAllDescendantsIntoThisNode || (nodeLocal.parent is not null)
                    );
                    if ((nodeLocal.parent is not null) && nodeLocal.parent!.isPartOfNodeMerging)
                    {
                        nodeLocal.parent!._markDirty();
                        nodeLocal._dirty = false;
                    }
                }
                _traversalParentNodes.removeWhere((key, oldNode) => Equals(nodeLocal, oldNode));
                foreach (HashSet<SemanticsNode> childSet in _traversalChildNodes.Values)
                {
                    childSet.removeWhere((oldNode) => Equals(nodeLocal, oldNode));
                }
                _traversalChildNodes.removeWhere((key, value) => checked((long)value.Count) == 0);
                bool isTraversalParent = nodeLocal._isTraversalParent;
                bool isTraversalChild = nodeLocal._isTraversalChild;
                if (isTraversalParent)
                {
                    var parentIdentifier = DartRuntimePrimitives.RequireReference(
                        nodeLocal.traversalParentIdentifier
                    );
                    DartRuntimePrimitives.Assert(() =>
                        !_traversalParentNodes.ContainsKey(parentIdentifier)
                        || Equals(
                            _traversalParentNodes.GetValueOrDefault(parentIdentifier),
                            nodeLocal
                        )
                    );
                    _traversalParentNodes[parentIdentifier] = nodeLocal;
                }
                else
                {
                    if (isTraversalChild)
                    {
                        _traversalChildNodes
                            .putIfAbsent(
                                nodeLocal.traversalChildIdentifier!,
                                () => new HashSet<SemanticsNode>()
                            )
                            .Add(nodeLocal);
                    }
                }
                if (!ConstantsLibrary.kIsWeb)
                {
                    if (nodeLocal._isTraversalChild)
                    {
                        SemanticsNode? parentNode = _traversalParentNodes.GetValueOrDefault(
                            DartRuntimePrimitives.RequireReference(
                                nodeLocal.traversalChildIdentifier
                            )
                        );
                        if ((parentNode is not null) && !visitedNodes.Contains(parentNode))
                        {
                            parentNode._markDirty();
                        }
                    }
                }
            }
        }
        visitedNodes.sort((a, b) => a.depth - b.depth);
        SemanticsUpdateBuilder builder = SemanticsBinding.instance.createSemanticsUpdateBuilder();
        foreach (var nodeAlternate in visitedNodes)
        {
            DartRuntimePrimitives.Assert(() => nodeAlternate.parent?._dirty != true);
            if (nodeAlternate._dirty && nodeAlternate.attached)
            {
                nodeAlternate._addToUpdate(builder, customSemanticsActionIds);
            }
        }
        _dirtyNodes.Clear();
        foreach (var actionId in customSemanticsActionIds)
        {
            CustomSemanticsAction actionLocal = CustomSemanticsAction.getAction(actionId)!;
            builder.updateCustomAction(
                id: actionId,
                label: actionLocal.label,
                hint: actionLocal.hint,
                overrideId: FoundationRuntimePorts.EnumIndexNullable(actionLocal.action) ?? -1L
            );
        }
        onSemanticsUpdate(builder.build());
        notifyListeners();
    }

    internal virtual Action<object?>? _getSemanticsActionHandlerForId(
        long id,
        SemanticsAction action,
        object? args = null
    )
    {
        SemanticsNode? result = _nodes.GetValueOrDefault(id);
        if (result is null)
        {
            return null;
        }
        if (result.isPartOfNodeMerging && !result._canHandleAction(action, args))
        {
            SemanticsNode? found = default!;
            result._visitDescendants(
                (node) =>
                {
                    if (node._canHandleAction(action, args))
                    {
                        found = node;
                        return false;
                    }
                    return true;
                }
            );
            result = found;
        }
        if ((result is null) || !result._canHandleAction(action, args))
        {
            return null;
        }
        return result._actions.GetValueOrDefault(action);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void performAction(long id, SemanticsAction action, object? args = null)
    {
        Action<object?>? handler = _getSemanticsActionHandlerForId(id, action, args);
        if (handler is not null)
        {
            handler(args);
            return;
        }
        if (
            Equals(action, SemanticsAction.showOnScreen)
            && (_nodes.GetValueOrDefault(id)?._showOnScreen is not null)
        )
        {
            _nodes.GetValueOrDefault(id)!._showOnScreen!();
        }
    }

    internal virtual Action<object?>? _getSemanticsActionHandlerForPosition(
        SemanticsNode node,
        Offset position,
        SemanticsAction action,
        object? args = null
    )
    {
        if (node.transform is not null)
        {
            var inverse = Matrix4.identity();
            if (inverse.copyInverse(node.transform!) == 0.0)
            {
                return null;
            }
            position = MatrixUtils.transformPoint(inverse, position);
        }
        if (!node.rect.contains(position))
        {
            return null;
        }
        if (node.mergeAllDescendantsIntoThisNode)
        {
            if (node._canHandleAction(action, args))
            {
                return node._actions.GetValueOrDefault(action);
            }
            SemanticsNode? result = default!;
            node._visitDescendants(
                (child) =>
                {
                    if (child._canHandleAction(action, args))
                    {
                        result = child;
                        return false;
                    }
                    return true;
                }
            );
            return result?._actions.GetValueOrDefault(action);
        }
        if (node.hasChildren)
        {
            foreach (SemanticsNode childLocal in Enumerable.Reverse(node._children!))
            {
                Action<object?>? handler = _getSemanticsActionHandlerForPosition(
                    childLocal,
                    position,
                    action,
                    args
                );
                if (handler is not null)
                {
                    return handler;
                }
            }
        }
        return node._actions.GetValueOrDefault(action);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void performActionAt(
        Offset position,
        SemanticsAction action,
        object? args = null
    )
    {
        SemanticsNode? node = rootSemanticsNode;
        if (node is null)
        {
            return;
        }
        Action<object?>? handler = _getSemanticsActionHandlerForPosition(
            node,
            position,
            action,
            args
        );
        if (handler is not null)
        {
            handler(args);
        }
    }

    public override string ToString() => DiagnosticsLibrary.describeIdentity(this);
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
    internal virtual DartMap<SemanticsAction, Action<object?>> _actions { get; private set; } =
        new DartMap<SemanticsAction, Action<object?>>();
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
    internal virtual Func<
        List<SemanticsConfiguration>,
        ChildSemanticsConfigurationsResult
    >? _childConfigurationsDelegate { get; set; } = default;
    internal virtual SemanticsSortKey? _sortKey { get; set; } = default;
    internal virtual long? _indexInParent { get; set; } = default;
    internal virtual long? _scrollChildCount { get; set; } = default;
    internal virtual long? _scrollIndex { get; set; } = default;
    internal virtual long? _platformViewId { get; set; } = default;
    internal virtual long? _maxValueLength { get; set; } = default;
    internal virtual long? _currentValueLength { get; set; } = default;
    internal virtual bool _isMergingSemanticsOfDescendants { get; set; } = false;
    internal virtual DartMap<CustomSemanticsAction, Action> _customSemanticsActions { get; set; } =
        new DartMap<CustomSemanticsAction, Action>();
    internal virtual string _identifier { get; set; } = "";
    internal virtual object? _traversalParentIdentifier { get; set; } = default;
    internal virtual object? _traversalChildIdentifier { get; set; } = default;
    internal virtual SemanticsRole _role { get; set; } = SemanticsRole.none;
    internal virtual AttributedString _attributedLabel { get; set; } = new AttributedString("");
    internal virtual AttributedString _attributedValue { get; set; } = new AttributedString("");
    internal virtual AttributedString _attributedIncreasedValue { get; set; } =
        new AttributedString("");
    internal virtual AttributedString _attributedDecreasedValue { get; set; } =
        new AttributedString("");
    internal virtual AttributedString _attributedHint { get; set; } = new AttributedString("");
    internal virtual string _tooltip { get; set; } = "";
    internal virtual SemanticsHintOverrides? _hintOverrides { get; set; } = default;
    internal virtual TextDirection? _textDirection { get; set; } = default;
    internal virtual AccessibilityFocusBlockType _accessibilityFocusBlockType { get; set; } =
        AccessibilityFocusBlockType.none;
    internal virtual DartUri? _linkUrl { get; set; } = default;
    internal virtual long _headingLevel { get; set; } = 0L;
    internal virtual TextSelection? _textSelection { get; set; } = default;
    internal virtual double? _scrollPosition { get; set; } = default;
    internal virtual double? _scrollExtentMax { get; set; } = default;
    internal virtual double? _scrollExtentMin { get; set; } = default;
    internal virtual HashSet<string>? _controlsNodes { get; set; } = default;
    internal virtual SemanticsValidationResult _validationResult { get; set; } =
        SemanticsValidationResult.none;
    internal virtual SemanticsHitTestBehavior _hitTestBehavior { get; set; } =
        DorotiUiLibrary.SemanticsHitTestBehavior.defer;
    internal virtual SemanticsInputType _inputType { get; set; } = SemanticsInputType.none;
    internal virtual string? _maxValue { get; set; } = default;
    internal virtual string? _minValue { get; set; } = default;
    internal virtual HashSet<SemanticsTag>? _tagsForChildren { get; set; } = default;
    internal virtual SemanticsFlags _flags { get; set; } = SemanticsFlags.none;

    public virtual bool isSemanticBoundary
    {
        get => _isSemanticBoundary;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => !isMergingSemanticsOfDescendants || (__value));
            _isSemanticBoundary = (__value);
        }
    }
    public virtual Locale? localeForSubtree
    {
        get => _localeForSubtree;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value is not null);
            _localeForSubtree = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool hasBeenAnnotated => _hasBeenAnnotated;
    internal virtual long _effectiveActionsAsBits =>
        isBlockingUserActions
            ? (_actionsAsBits & SemanticsLibrary._kUnblockedUserActions)
            : _actionsAsBits;

    internal virtual void _addAction(SemanticsAction action, Action<object?> handler)
    {
        _actions[(action)] = handler;
        _actionsAsBits |= (long)action;
        _hasBeenAnnotated = true;
    }

    internal virtual void _addArgumentlessAction(SemanticsAction action, Action handler)
    {
        _addAction(
            action,
            (args) =>
            {
                DartRuntimePrimitives.Assert(() => args is null);
                handler();
            }
        );
    }

    public virtual Action? onTap
    {
        get => _onTap;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.tap, __value!);
            _onTap = __value;
        }
    }
    public virtual Action? onLongPress
    {
        get => _onLongPress;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.longPress, __value!);
            _onLongPress = __value;
        }
    }
    public virtual Action? onScrollLeft
    {
        get => _onScrollLeft;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.scrollLeft, __value!);
            _onScrollLeft = __value;
        }
    }
    public virtual Action? onDismiss
    {
        get => _onDismiss;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.dismiss, __value!);
            _onDismiss = __value;
        }
    }
    public virtual Action? onScrollRight
    {
        get => _onScrollRight;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.scrollRight, __value!);
            _onScrollRight = __value;
        }
    }
    public virtual Action? onScrollUp
    {
        get => _onScrollUp;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.scrollUp, __value!);
            _onScrollUp = __value;
        }
    }
    public virtual Action? onScrollDown
    {
        get => _onScrollDown;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.scrollDown, __value!);
            _onScrollDown = __value;
        }
    }
    public virtual Action<Offset>? onScrollToOffset
    {
        get => _onScrollToOffset;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value is not null);
            _addAction(
                SemanticsAction.scrollToOffset,
                (args) =>
                {
                    var list = ((IReadOnlyList<double>?)args!)!;
                    __value!(new Offset(list[0], list[1]));
                }
            );
            _onScrollToOffset = __value;
        }
    }
    public virtual Action? onIncrease
    {
        get => _onIncrease;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.increase, __value!);
            _onIncrease = __value;
        }
    }
    public virtual Action? onDecrease
    {
        get => _onDecrease;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.decrease, __value!);
            _onDecrease = __value;
        }
    }
    public virtual Action? onCopy
    {
        get => _onCopy;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.copy, __value!);
            _onCopy = __value;
        }
    }
    public virtual Action? onCut
    {
        get => _onCut;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.cut, __value!);
            _onCut = __value;
        }
    }
    public virtual Action? onPaste
    {
        get => _onPaste;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.paste, __value!);
            _onPaste = __value;
        }
    }
    public virtual Action? onShowOnScreen
    {
        get => _onShowOnScreen;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.showOnScreen, __value!);
            _onShowOnScreen = __value;
        }
    }
    public virtual Action<bool>? onMoveCursorForwardByCharacter
    {
        get => _onMoveCursorForwardByCharacter;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value is not null);
            _addAction(
                SemanticsAction.moveCursorForwardByCharacter,
                (args) =>
                {
                    var extendSelection = (bool)args!;
                    __value!(extendSelection);
                }
            );
            _onMoveCursorForwardByCharacter = __value;
        }
    }
    public virtual Action<bool>? onMoveCursorBackwardByCharacter
    {
        get => _onMoveCursorBackwardByCharacter;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value is not null);
            _addAction(
                SemanticsAction.moveCursorBackwardByCharacter,
                (args) =>
                {
                    var extendSelection = (bool)args!;
                    __value!(extendSelection);
                }
            );
            _onMoveCursorBackwardByCharacter = __value;
        }
    }
    public virtual Action<bool>? onMoveCursorForwardByWord
    {
        get => _onMoveCursorForwardByWord;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value is not null);
            _addAction(
                SemanticsAction.moveCursorForwardByWord,
                (args) =>
                {
                    var extendSelection = (bool)args!;
                    __value!(extendSelection);
                }
            );
            _onMoveCursorForwardByCharacter = __value;
        }
    }
    public virtual Action<bool>? onMoveCursorBackwardByWord
    {
        get => _onMoveCursorBackwardByWord;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value is not null);
            _addAction(
                SemanticsAction.moveCursorBackwardByWord,
                (args) =>
                {
                    var extendSelection = (bool)args!;
                    __value!(extendSelection);
                }
            );
            _onMoveCursorBackwardByCharacter = __value;
        }
    }
    public virtual Action<TextSelection>? onSetSelection
    {
        get => _onSetSelection;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value is not null);
            _addAction(
                SemanticsAction.setSelection,
                (args) =>
                {
                    DartRuntimePrimitives.Assert(() =>
                        (args is not null) && (args is System.Collections.IDictionary)
                    );
                    DartMap<string, long> selection = DartRuntimePrimitives
                        .ConvertMap<object, object>((System.Collections.IDictionary)args!)
                        .cast<string, long>();
                    DartRuntimePrimitives.Assert(() =>
                        selection.ContainsKey("base") && selection.ContainsKey("extent")
                    );
                    __value!(
                        new TextSelection(
                            baseOffset: (selection.GetValueOrDefault("base")),
                            extentOffset: (selection.GetValueOrDefault("extent"))
                        )
                    );
                }
            );
            _onSetSelection = __value;
        }
    }
    public virtual Action<string>? onSetText
    {
        get => _onSetText;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value is not null);
            _addAction(
                SemanticsAction.setText,
                (args) =>
                {
                    DartRuntimePrimitives.Assert(() => (args is not null) && (args is string));
                    var text = ((string?)args!)!;
                    __value!(text);
                }
            );
            _onSetText = __value;
        }
    }
    public virtual Action? onDidGainAccessibilityFocus
    {
        get => _onDidGainAccessibilityFocus;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.didGainAccessibilityFocus, __value!);
            _onDidGainAccessibilityFocus = __value;
        }
    }
    public virtual Action? onDidLoseAccessibilityFocus
    {
        get => _onDidLoseAccessibilityFocus;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.didLoseAccessibilityFocus, __value!);
            _onDidLoseAccessibilityFocus = __value;
        }
    }
    public virtual Action? onFocus
    {
        get => _onFocus;
        set
        {
            var __value = value;
            _addArgumentlessAction(SemanticsAction.focus, __value!);
            _onFocus = __value;
        }
    }
    public virtual Action? onExpand
    {
        get => _onExpand;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value is not null);
            _addArgumentlessAction(SemanticsAction.expand, __value!);
            _onExpand = __value;
        }
    }
    public virtual Action? onCollapse
    {
        get => _onCollapse;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value is not null);
            _addArgumentlessAction(SemanticsAction.collapse, __value!);
            _onCollapse = __value;
        }
    }
    public virtual Func<
        List<SemanticsConfiguration>,
        ChildSemanticsConfigurationsResult
    >? childConfigurationsDelegate
    {
        get => _childConfigurationsDelegate;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value is not null);
            _childConfigurationsDelegate = __value;
        }
    }

    public virtual Action<object?>? getActionHandler(SemanticsAction action) =>
        _actions.GetValueOrDefault(action);

    public virtual SemanticsSortKey? sortKey
    {
        get => _sortKey;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value is not null);
            _sortKey = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual long? indexInParent
    {
        get => _indexInParent;
        set
        {
            var __value = value;
            _indexInParent = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual long? scrollChildCount
    {
        get => _scrollChildCount;
        set
        {
            var __value = value;
            if (__value == scrollChildCount)
            {
                return;
            }
            _scrollChildCount = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual long? scrollIndex
    {
        get => _scrollIndex;
        set
        {
            var __value = value;
            if (__value == scrollIndex)
            {
                return;
            }
            _scrollIndex = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual long? platformViewId
    {
        get => _platformViewId;
        set
        {
            var __value = value;
            if (__value == platformViewId)
            {
                return;
            }
            _platformViewId = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual long? maxValueLength
    {
        get => _maxValueLength;
        set
        {
            var __value = value;
            if (__value == maxValueLength)
            {
                return;
            }
            _maxValueLength = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual long? currentValueLength
    {
        get => _currentValueLength;
        set
        {
            var __value = value;
            if (__value == currentValueLength)
            {
                return;
            }
            _currentValueLength = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isMergingSemanticsOfDescendants
    {
        get => _isMergingSemanticsOfDescendants;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => isSemanticBoundary);
            _isMergingSemanticsOfDescendants = (__value);
            _hasBeenAnnotated = true;
        }
    }
    public virtual DartMap<CustomSemanticsAction, Action> customSemanticsActions
    {
        get => _customSemanticsActions;
        set
        {
            var __value = value;
            _hasBeenAnnotated = true;
            _actionsAsBits |= (long)SemanticsAction.customAction;
            _customSemanticsActions = __value;
            _actions[SemanticsAction.customAction] = _onCustomSemanticsAction;
        }
    }

    internal virtual void _onCustomSemanticsAction(object? args)
    {
        CustomSemanticsAction? action = CustomSemanticsAction.getAction((long)args!);
        if (action is null)
        {
            return;
        }
        Action? callback = _customSemanticsActions.GetValueOrDefault(action);
        if (callback is not null)
        {
            callback();
        }
    }

    public virtual string identifier
    {
        get => _identifier;
        set
        {
            var identifier = value;
            _identifier = identifier;
            _hasBeenAnnotated = true;
        }
    }
    public virtual object? traversalParentIdentifier
    {
        get => _traversalParentIdentifier;
        set
        {
            var __value = value;
            if (Equals(__value, traversalParentIdentifier))
            {
                return;
            }
            _traversalParentIdentifier = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual object? traversalChildIdentifier
    {
        get => _traversalChildIdentifier;
        set
        {
            var __value = value;
            if (Equals(__value, traversalChildIdentifier))
            {
                return;
            }
            _traversalChildIdentifier = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual SemanticsRole role
    {
        get => _role;
        set
        {
            var __value = value;
            _role = (__value);
            _hasBeenAnnotated = true;
        }
    }
    public virtual string label
    {
        get => _attributedLabel.@string;
        set
        {
            var label = value;
            _attributedLabel = new AttributedString(label);
            _hasBeenAnnotated = true;
        }
    }
    public virtual AttributedString attributedLabel
    {
        get => _attributedLabel;
        set
        {
            var attributedLabel = value;
            _attributedLabel = attributedLabel;
            _hasBeenAnnotated = true;
        }
    }
    public virtual string value
    {
        get => _attributedValue.@string;
        set
        {
            var __value = value;
            _attributedValue = new AttributedString(__value);
            _hasBeenAnnotated = true;
        }
    }
    public virtual AttributedString attributedValue
    {
        get => _attributedValue;
        set
        {
            var attributedValue = value;
            _attributedValue = attributedValue;
            _hasBeenAnnotated = true;
        }
    }
    public virtual string increasedValue
    {
        get => _attributedIncreasedValue.@string;
        set
        {
            var increasedValue = value;
            _attributedIncreasedValue = new AttributedString(increasedValue);
            _hasBeenAnnotated = true;
        }
    }
    public virtual AttributedString attributedIncreasedValue
    {
        get => _attributedIncreasedValue;
        set
        {
            var attributedIncreasedValue = value;
            _attributedIncreasedValue = attributedIncreasedValue;
            _hasBeenAnnotated = true;
        }
    }
    public virtual string decreasedValue
    {
        get => _attributedDecreasedValue.@string;
        set
        {
            var decreasedValue = value;
            _attributedDecreasedValue = new AttributedString(decreasedValue);
            _hasBeenAnnotated = true;
        }
    }
    public virtual AttributedString attributedDecreasedValue
    {
        get => _attributedDecreasedValue;
        set
        {
            var attributedDecreasedValue = value;
            _attributedDecreasedValue = attributedDecreasedValue;
            _hasBeenAnnotated = true;
        }
    }
    public virtual string hint
    {
        get => _attributedHint.@string;
        set
        {
            var hint = value;
            _attributedHint = new AttributedString(hint);
            _hasBeenAnnotated = true;
        }
    }
    public virtual AttributedString attributedHint
    {
        get => _attributedHint;
        set
        {
            var attributedHint = value;
            _attributedHint = attributedHint;
            _hasBeenAnnotated = true;
        }
    }
    public virtual string tooltip
    {
        get => _tooltip;
        set
        {
            var tooltip = value;
            _tooltip = tooltip;
            _hasBeenAnnotated = true;
        }
    }
    public virtual SemanticsHintOverrides? hintOverrides
    {
        get => _hintOverrides;
        set
        {
            var __value = value;
            if (__value is null)
            {
                return;
            }
            _hintOverrides = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool scopesRoute
    {
        get => _flags.scopesRoute;
        set
        {
            var __value = value;
            _flags = _flags.copyWith(scopesRoute: (__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool namesRoute
    {
        get => _flags.namesRoute;
        set
        {
            var __value = value;
            _flags = _flags.copyWith(namesRoute: (__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isImage
    {
        get => _flags.isImage;
        set
        {
            var __value = value;
            _flags = _flags.copyWith(isImage: (__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool liveRegion
    {
        get => _flags.isLiveRegion;
        set
        {
            var __value = value;
            _flags = _flags.copyWith(isLiveRegion: (__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual TextDirection? textDirection
    {
        get => _textDirection;
        set
        {
            var textDirection = value;
            _textDirection = textDirection;
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isSelected
    {
        get => Equals(_flags.isSelected, Tristate.isTrue);
        set
        {
            var __value = value;
            _flags = _flags.copyWith(
                isSelected: SemanticsLibrary._tristateFromBoolOrNull((__value))
            );
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool? isExpanded
    {
        get => _flags.isExpanded.toBoolOrNull();
        set
        {
            var __value = value;
            _flags = _flags.copyWith(isExpanded: SemanticsLibrary._tristateFromBoolOrNull(__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool? isEnabled
    {
        get => _flags.isEnabled.toBoolOrNull();
        set
        {
            var __value = value;
            _flags = _flags.copyWith(isEnabled: SemanticsLibrary._tristateFromBoolOrNull(__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool? isChecked
    {
        get =>
            Equals(_flags.isChecked, CheckedState.none)
                ? null
                : object.Equals(_flags.isChecked, CheckedState.isTrue);
        set
        {
            var __value = value;
            if (__value is not null)
            {
                bool value__value243016 = (
                    __value
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
                _flags = _flags.copyWith(
                    isChecked: (value__value243016) ? CheckedState.isTrue : CheckedState.isFalse
                );
            }
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool? isCheckStateMixed
    {
        get =>
            Equals(_flags.isChecked, CheckedState.none)
                ? null
                : object.Equals(_flags.isChecked, CheckedState.mixed);
        set
        {
            var __value = value;
            if (__value ?? false)
            {
                _flags = _flags.copyWith(isChecked: CheckedState.mixed);
            }
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool? isToggled
    {
        get => _flags.isToggled.toBoolOrNull();
        set
        {
            var __value = value;
            _flags = _flags.copyWith(isToggled: SemanticsLibrary._tristateFromBoolOrNull(__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isInMutuallyExclusiveGroup
    {
        get => _flags.isInMutuallyExclusiveGroup;
        set
        {
            var __value = value;
            _flags = _flags.copyWith(isInMutuallyExclusiveGroup: (__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isFocusable
    {
        get => !Equals(_flags.isFocused, Tristate.none);
        set
        {
            var __value = value;
            if (!(__value))
            {
                _flags = _flags.copyWith(isFocused: Tristate.none);
            }
            else
            {
                if (Equals(_flags.isFocused, Tristate.none))
                {
                    _flags = _flags.copyWith(isFocused: Tristate.isFalse);
                }
            }
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool? isFocused
    {
        get => _flags.isFocused.toBoolOrNull();
        set
        {
            var __value = value;
            _flags = _flags.copyWith(isFocused: SemanticsLibrary._tristateFromBoolOrNull(__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual AccessibilityFocusBlockType accessibilityFocusBlockType
    {
        get => _accessibilityFocusBlockType;
        set
        {
            var __value = value;
            _accessibilityFocusBlockType = (__value);
            _flags = _flags.copyWith(
                isAccessibilityFocusBlocked: !Equals((__value), AccessibilityFocusBlockType.none)
            );
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isButton
    {
        get => _flags.isButton;
        set
        {
            var __value = value;
            _flags = _flags.copyWith(isButton: (__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isLink
    {
        get => _flags.isLink;
        set
        {
            var __value = value;
            _flags = _flags.copyWith(isLink: (__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual DartUri? linkUrl
    {
        get => _linkUrl;
        set
        {
            var __value = value;
            if (Equals(__value, _linkUrl))
            {
                return;
            }
            _linkUrl = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isHeader
    {
        get => _flags.isHeader;
        set
        {
            var __value = value;
            _flags = _flags.copyWith(isHeader: (__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual long headingLevel
    {
        get => _headingLevel;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value >= 0L) && (__value <= 6L));
            if ((__value) == headingLevel)
            {
                return;
            }
            _headingLevel = (__value);
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isSlider
    {
        get => _flags.isSlider;
        set
        {
            var __value = value;
            _flags = _flags.copyWith(isSlider: (__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isKeyboardKey
    {
        get => _flags.isKeyboardKey;
        set
        {
            var __value = value;
            _flags = _flags.copyWith(isKeyboardKey: (__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isHidden
    {
        get => _flags.isHidden;
        set
        {
            var __value = value;
            _flags = _flags.copyWith(isHidden: (__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isTextField
    {
        get => _flags.isTextField;
        set
        {
            var __value = value;
            _flags = _flags.copyWith(isTextField: (__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isReadOnly
    {
        get => _flags.isReadOnly;
        set
        {
            var __value = value;
            _flags = _flags.copyWith(isReadOnly: (__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isObscured
    {
        get => _flags.isObscured;
        set
        {
            var __value = value;
            _flags = _flags.copyWith(isObscured: (__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool isMultiline
    {
        get => _flags.isMultiline;
        set
        {
            var __value = value;
            _flags = _flags.copyWith(isMultiline: (__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool? isRequired
    {
        get => _flags.isRequired.toBoolOrNull();
        set
        {
            var __value = value;
            _flags = _flags.copyWith(isRequired: SemanticsLibrary._tristateFromBoolOrNull(__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual bool hasImplicitScrolling
    {
        get => _flags.hasImplicitScrolling;
        set
        {
            var __value = value;
            _flags = _flags.copyWith(hasImplicitScrolling: (__value));
            _hasBeenAnnotated = true;
        }
    }
    public virtual TextSelection? textSelection
    {
        get => _textSelection;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value is not null);
            _textSelection = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual double? scrollPosition
    {
        get => _scrollPosition;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value is not null);
            _scrollPosition = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual double? scrollExtentMax
    {
        get => _scrollExtentMax;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value is not null);
            _scrollExtentMax = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual double? scrollExtentMin
    {
        get => _scrollExtentMin;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value is not null);
            _scrollExtentMin = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual HashSet<string>? controlsNodes
    {
        get => _controlsNodes;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value is not null);
            _controlsNodes = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual SemanticsValidationResult validationResult
    {
        get => _validationResult;
        set
        {
            var __value = value;
            _validationResult = (__value);
            _hasBeenAnnotated = true;
        }
    }
    public virtual SemanticsHitTestBehavior hitTestBehavior
    {
        get => _hitTestBehavior;
        set
        {
            var __value = value;
            _hitTestBehavior = (__value);
            _hasBeenAnnotated = true;
        }
    }
    public virtual SemanticsInputType inputType
    {
        get => _inputType;
        set
        {
            var __value = value;
            _inputType = (__value);
            _hasBeenAnnotated = true;
        }
    }
    public virtual string? maxValue
    {
        get => _maxValue;
        set
        {
            var __value = value;
            _maxValue = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual string? minValue
    {
        get => _minValue;
        set
        {
            var __value = value;
            _minValue = __value;
            _hasBeenAnnotated = true;
        }
    }
    public virtual IEnumerable<SemanticsTag>? tagsForChildren => _tagsForChildren;

    public virtual bool tagsChildrenWith(SemanticsTag tag) =>
        _tagsForChildren?.Contains(tag) ?? false;

    public virtual void addTagForChildren(SemanticsTag tag)
    {
        _tagsForChildren ??= new HashSet<SemanticsTag>();
        _tagsForChildren!.Add(tag);
    }

    internal virtual bool _hasExplicitRole
    {
        get
        {
            if (!Equals(_role, SemanticsRole.none))
            {
                return true;
            }
            if (
                _flags.isTextField
                || (_flags.isHeader && ConstantsLibrary.kIsWeb)
                || _flags.isSlider
                || _flags.isLink
                || _flags.scopesRoute
                || _flags.isImage
                || _flags.isKeyboardKey
            )
            {
                return true;
            }
            return false;
        }
    }

    public virtual bool isCompatibleWith(SemanticsConfiguration? other)
    {
        if ((other is null) || !other.hasBeenAnnotated)
        {
            return true;
        }
        if (!Equals(_traversalChildIdentifier, other._traversalChildIdentifier))
        {
            return false;
        }
        if (!hasBeenAnnotated)
        {
            return true;
        }
        if ((_actionsAsBits & other._actionsAsBits) != 0L)
        {
            return false;
        }
        if (_flags.hasConflictingFlags(other._flags))
        {
            return false;
        }
        if ((_platformViewId is not null) && (other._platformViewId is not null))
        {
            return false;
        }
        if ((_maxValueLength is not null) && (other._maxValueLength is not null))
        {
            return false;
        }
        if ((_currentValueLength is not null) && (other._currentValueLength is not null))
        {
            return false;
        }
        if ((_attributedValue.@string.Length != 0) && (other._attributedValue.@string.Length != 0))
        {
            return false;
        }
        if (!Equals(_localeForSubtree, other._localeForSubtree))
        {
            return false;
        }
        if (_hasExplicitRole && other._hasExplicitRole)
        {
            return false;
        }
        if (
            (!Equals(_hitTestBehavior, DorotiUiLibrary.SemanticsHitTestBehavior.defer))
            || (!Equals(other._hitTestBehavior, DorotiUiLibrary.SemanticsHitTestBehavior.defer))
        )
        {
            return false;
        }
        if ((_minValue is not null) && (other._minValue is not null))
        {
            return false;
        }
        if ((_maxValue is not null) && (other._maxValue is not null))
        {
            return false;
        }
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void absorb(SemanticsConfiguration child)
    {
        DartRuntimePrimitives.Assert(() => !explicitChildNodes);
        if (!child.hasBeenAnnotated)
        {
            return;
        }
        if (child.isBlockingUserActions)
        {
            child._actions.forEach(
                (key, value) =>
                {
                    if (
                        (
                            SemanticsLibrary._kUnblockedUserActions
                            & FoundationRuntimePorts.EnumIndex(key)
                        ) > 0L
                    )
                    {
                        _actions[key] = value;
                    }
                }
            );
        }
        else
        {
            _actions.AddRange(child._actions);
        }
        _actionsAsBits |= child._effectiveActionsAsBits;
        _customSemanticsActions.AddRange(child._customSemanticsActions);
        _flags = _flags.merge(child._flags);
        _linkUrl ??= child._linkUrl;
        _textSelection ??= child._textSelection;
        _scrollPosition ??= child._scrollPosition;
        _scrollExtentMax ??= child._scrollExtentMax;
        _scrollExtentMin ??= child._scrollExtentMin;
        _hintOverrides ??= child._hintOverrides;
        _indexInParent ??= child.indexInParent;
        _scrollIndex ??= child._scrollIndex;
        _scrollChildCount ??= child._scrollChildCount;
        _platformViewId ??= child._platformViewId;
        _maxValueLength ??= child._maxValueLength;
        _currentValueLength ??= child._currentValueLength;
        if (_traversalChildIdentifier is null)
        {
            _traversalParentIdentifier ??= child._traversalParentIdentifier;
        }
        _traversalChildIdentifier ??= child._traversalChildIdentifier;
        _headingLevel = SemanticsLibrary._mergeHeadingLevels(
            sourceLevel: child._headingLevel,
            targetLevel: _headingLevel
        );
        textDirection ??= child.textDirection;
        _sortKey ??= child._sortKey;
        if (_identifier == "")
        {
            _identifier = child._identifier;
        }
        _attributedLabel = SemanticsLibrary._concatAttributedString(
            thisAttributedString: _attributedLabel,
            thisTextDirection: textDirection,
            otherAttributedString: child._attributedLabel,
            otherTextDirection: child.textDirection
        );
        if (_attributedValue.@string == "")
        {
            _attributedValue = child._attributedValue;
        }
        if (_attributedIncreasedValue.@string == "")
        {
            _attributedIncreasedValue = child._attributedIncreasedValue;
        }
        if (_attributedDecreasedValue.@string == "")
        {
            _attributedDecreasedValue = child._attributedDecreasedValue;
        }
        if (Equals(_role, SemanticsRole.none))
        {
            _role = child._role;
        }
        if (Equals(_inputType, SemanticsInputType.none))
        {
            _inputType = child._inputType;
        }
        _attributedHint = SemanticsLibrary._concatAttributedString(
            thisAttributedString: _attributedHint,
            thisTextDirection: textDirection,
            otherAttributedString: child._attributedHint,
            otherTextDirection: child.textDirection
        );
        if (_tooltip == "")
        {
            _tooltip = child._tooltip;
        }
        if (_controlsNodes is null)
        {
            _controlsNodes = child._controlsNodes;
        }
        else
        {
            if (child._controlsNodes is not null)
            {
                _controlsNodes = new HashSet<string>();
            }
        }
        if (!Equals(child._validationResult, _validationResult))
        {
            if (Equals(child._validationResult, SemanticsValidationResult.invalid))
            {
                _validationResult = SemanticsValidationResult.invalid;
            }
            else
            {
                if (Equals(_validationResult, SemanticsValidationResult.none))
                {
                    _validationResult = child._validationResult;
                }
            }
        }
        _accessibilityFocusBlockType = _accessibilityFocusBlockType._merge(
            child._accessibilityFocusBlockType
        );
        _minValue ??= child._minValue;
        _maxValue ??= child._maxValue;
        if (
            Equals(_hitTestBehavior, DorotiUiLibrary.SemanticsHitTestBehavior.defer)
            && (!Equals(child._hitTestBehavior, DorotiUiLibrary.SemanticsHitTestBehavior.defer))
        )
        {
            _hitTestBehavior = child._hitTestBehavior;
        }
        _hasBeenAnnotated = hasBeenAnnotated || child.hasBeenAnnotated;
    }

    public virtual SemanticsConfiguration copy()
    {
        return (
            (Func<SemanticsConfiguration>)(
                () =>
                {
                    var __cascade = new SemanticsConfiguration();
                    __cascade._isSemanticBoundary = _isSemanticBoundary;
                    __cascade.explicitChildNodes = explicitChildNodes;
                    __cascade.isBlockingSemanticsOfPreviouslyPaintedNodes =
                        isBlockingSemanticsOfPreviouslyPaintedNodes;
                    __cascade._hasBeenAnnotated = hasBeenAnnotated;
                    __cascade._isMergingSemanticsOfDescendants = _isMergingSemanticsOfDescendants;
                    __cascade._textDirection = _textDirection;
                    __cascade._sortKey = _sortKey;
                    __cascade._identifier = _identifier;
                    __cascade._traversalParentIdentifier = _traversalParentIdentifier;
                    __cascade._traversalChildIdentifier = _traversalChildIdentifier;
                    __cascade._attributedLabel = _attributedLabel;
                    __cascade._attributedIncreasedValue = _attributedIncreasedValue;
                    __cascade._attributedValue = _attributedValue;
                    __cascade._attributedDecreasedValue = _attributedDecreasedValue;
                    __cascade._attributedHint = _attributedHint;
                    __cascade._accessibilityFocusBlockType = _accessibilityFocusBlockType;
                    __cascade._hintOverrides = _hintOverrides;
                    __cascade._tooltip = _tooltip;
                    __cascade._flags = _flags;
                    __cascade._tagsForChildren = _tagsForChildren;
                    __cascade._textSelection = _textSelection;
                    __cascade._scrollPosition = _scrollPosition;
                    __cascade._scrollExtentMax = _scrollExtentMax;
                    __cascade._scrollExtentMin = _scrollExtentMin;
                    __cascade._actionsAsBits = _actionsAsBits;
                    __cascade._indexInParent = indexInParent;
                    __cascade._scrollIndex = _scrollIndex;
                    __cascade._scrollChildCount = _scrollChildCount;
                    __cascade._platformViewId = _platformViewId;
                    __cascade._maxValueLength = _maxValueLength;
                    __cascade._currentValueLength = _currentValueLength;
                    __cascade._actions.AddRange(_actions);
                    __cascade._customSemanticsActions.AddRange(_customSemanticsActions);
                    __cascade.isBlockingUserActions = isBlockingUserActions;
                    __cascade._headingLevel = _headingLevel;
                    __cascade._linkUrl = _linkUrl;
                    __cascade._role = _role;
                    __cascade._controlsNodes = _controlsNodes;
                    __cascade._validationResult = _validationResult;
                    __cascade._inputType = _inputType;
                    __cascade._hitTestBehavior = _hitTestBehavior;
                    __cascade._traversalChildIdentifier = _traversalChildIdentifier;
                    __cascade._traversalParentIdentifier = _traversalParentIdentifier;
                    __cascade._minValue = _minValue;
                    __cascade._maxValue = _maxValue;
                    return __cascade;
                }
            )
        )();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public enum DebugSemanticsDumpOrder
{
    inverseHitTest,
    traversalOrder,
}

public static partial class SemanticsLibrary
{
    internal static AttributedString _concatAttributedString(
        AttributedString thisAttributedString,
        AttributedString otherAttributedString,
        TextDirection? thisTextDirection,
        TextDirection? otherTextDirection
    )
    {
        if (otherAttributedString.@string.Length == 0)
        {
            return thisAttributedString;
        }
        if ((!Equals(thisTextDirection, otherTextDirection)) && (otherTextDirection is not null))
        {
            TextDirection otherTextDirection__value266687 = (
                otherTextDirection
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            AttributedString directionEmbedding = (otherTextDirection__value266687) switch
            {
                TextDirection.rtl => new AttributedString(Unicode.RLE),
                TextDirection.ltr => new AttributedString(Unicode.LRE),
                _ => throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
            };
            otherAttributedString = directionEmbedding
                .op_Add(otherAttributedString)
                .op_Add(new AttributedString(Unicode.PDF));
        }
        if (thisAttributedString.@string.Length == 0)
        {
            return otherAttributedString;
        }
        return thisAttributedString
            .op_Add(new AttributedString("\n"))
            .op_Add(otherAttributedString);
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
        DartRuntimePrimitives.Assert(() =>
            Equals(GetType(), DartRuntimePrimitives.RuntimeType(other))
        );
        if (name == other.name)
        {
            return doCompare(other);
        }
        if ((name is null) && (other.name is not null))
        {
            return -1L;
        }
        else
        {
            if ((name is not null) && (other.name is null))
            {
                return 1L;
            }
        }
        return name!.CompareTo(other.name!);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public abstract long doCompare(SemanticsSortKey other);

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new StringProperty("name", name, defaultValue: null));
    }

    public int CompareTo(SemanticsSortKey? other) => checked((int)compareTo(other!));
}

public class OrdinalSortKey : SemanticsSortKey
{
    public virtual double order { get; private set; } = default!;

    public OrdinalSortKey(double order, string? name = null)
        : base(name: name)
    {
        this.order = order;
        System.Diagnostics.Debug.Assert(order > double.NegativeInfinity);
        System.Diagnostics.Debug.Assert(order < double.PositiveInfinity);
    }

    public override long doCompare(SemanticsSortKey other)
    {
        var __other = (OrdinalSortKey)other;
        if (__other.order == order)
        {
            return 0L;
        }
        return order.CompareTo(__other.order);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("order", order, defaultValue: null));
    }
}

public static partial class SemanticsLibrary
{
    internal static long _mergeHeadingLevels(long sourceLevel, long targetLevel)
    {
        return (targetLevel == 0L) ? sourceLevel : targetLevel;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class SemanticsLibrary
{
    internal static Tristate _tristateFromBoolOrNull(bool? value)
    {
        if (value is null)
        {
            return Tristate.none;
        }
        if (
            (value ?? throw new global::System.NullReferenceException("A required value was null."))
        )
        {
            return Tristate.isTrue;
        }
        return Tristate.isFalse;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class SemanticsLibrary
{
    internal static long _toBitMask(SemanticsFlags flags)
    {
        var bitmask = 0L;
        if (!Equals(flags.isChecked, CheckedState.none))
        {
            bitmask |= 1L << (int)0L;
        }
        if (Equals(flags.isChecked, CheckedState.isTrue))
        {
            bitmask |= 1L << (int)1L;
        }
        if (Equals(flags.isSelected, Tristate.isTrue))
        {
            bitmask |= 1L << (int)2L;
        }
        if (flags.isButton)
        {
            bitmask |= 1L << (int)3L;
        }
        if (flags.isTextField)
        {
            bitmask |= 1L << (int)4L;
        }
        if (Equals(flags.isFocused, Tristate.isTrue))
        {
            bitmask |= 1L << (int)5L;
        }
        if (!Equals(flags.isEnabled, Tristate.none))
        {
            bitmask |= 1L << (int)6L;
        }
        if (Equals(flags.isEnabled, Tristate.isTrue))
        {
            bitmask |= 1L << (int)7L;
        }
        if (flags.isInMutuallyExclusiveGroup)
        {
            bitmask |= 1L << (int)8L;
        }
        if (flags.isHeader)
        {
            bitmask |= 1L << (int)9L;
        }
        if (flags.isObscured)
        {
            bitmask |= 1L << (int)10L;
        }
        if (flags.scopesRoute)
        {
            bitmask |= 1L << (int)11L;
        }
        if (flags.namesRoute)
        {
            bitmask |= 1L << (int)12L;
        }
        if (flags.isHidden)
        {
            bitmask |= 1L << (int)13L;
        }
        if (flags.isImage)
        {
            bitmask |= 1L << (int)14L;
        }
        if (flags.isLiveRegion)
        {
            bitmask |= 1L << (int)15L;
        }
        if (!Equals(flags.isToggled, Tristate.none))
        {
            bitmask |= 1L << (int)16L;
        }
        if (Equals(flags.isToggled, Tristate.isTrue))
        {
            bitmask |= 1L << (int)17L;
        }
        if (flags.hasImplicitScrolling)
        {
            bitmask |= 1L << (int)18L;
        }
        if (flags.isMultiline)
        {
            bitmask |= 1L << (int)19L;
        }
        if (flags.isReadOnly)
        {
            bitmask |= 1L << (int)20L;
        }
        if (!Equals(flags.isFocused, Tristate.none))
        {
            bitmask |= 1L << (int)21L;
        }
        if (flags.isLink)
        {
            bitmask |= 1L << (int)22L;
        }
        if (flags.isSlider)
        {
            bitmask |= 1L << (int)23L;
        }
        if (flags.isKeyboardKey)
        {
            bitmask |= 1L << (int)24L;
        }
        if (Equals(flags.isChecked, CheckedState.mixed))
        {
            bitmask |= 1L << (int)25L;
        }
        if (!Equals(flags.isExpanded, Tristate.none))
        {
            bitmask |= 1L << (int)26L;
        }
        if (Equals(flags.isExpanded, Tristate.isTrue))
        {
            bitmask |= 1L << (int)27L;
        }
        if (!Equals(flags.isSelected, Tristate.none))
        {
            bitmask |= 1L << (int)28L;
        }
        if (!Equals(flags.isRequired, Tristate.none))
        {
            bitmask |= 1L << (int)29L;
        }
        if (Equals(flags.isRequired, Tristate.isTrue))
        {
            bitmask |= 1L << (int)30L;
        }
        return bitmask;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
