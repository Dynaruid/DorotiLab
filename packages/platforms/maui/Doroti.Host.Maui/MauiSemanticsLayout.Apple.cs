#if MACOS || IOS || MACCATALYST
using Doroti.Ui;
using Foundation;
#if MACOS
using AppKit;
using Microsoft.Maui.Platforms.MacOS.Platform;
#else
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;
#endif

namespace Doroti.Host.Maui;

internal sealed partial class MauiSemanticsLayout
{
    internal static readonly SemanticsAction[] AppleActions =
    [
        SemanticsAction.tap,
        SemanticsAction.longPress,
        SemanticsAction.increase,
        SemanticsAction.decrease,
        SemanticsAction.expand,
        SemanticsAction.collapse,
        SemanticsAction.showOnScreen,
        SemanticsAction.dismiss,
        SemanticsAction.scrollUp,
        SemanticsAction.scrollDown,
        SemanticsAction.scrollLeft,
        SemanticsAction.scrollRight,
        SemanticsAction.copy,
        SemanticsAction.cut,
        SemanticsAction.paste,
#if MACOS
        SemanticsAction.setText,
#endif
    ];

    private static string ActionName(SemanticsAction action) =>
        action switch
        {
            SemanticsAction.tap => "Activate",
            SemanticsAction.longPress => "Long press",
            SemanticsAction.increase => "Increase",
            SemanticsAction.decrease => "Decrease",
            SemanticsAction.expand => "Expand",
            SemanticsAction.collapse => "Collapse",
            SemanticsAction.showOnScreen => "Show on screen",
            SemanticsAction.dismiss => "Dismiss",
            SemanticsAction.scrollUp => "Scroll up",
            SemanticsAction.scrollDown => "Scroll down",
            SemanticsAction.scrollLeft => "Scroll left",
            SemanticsAction.scrollRight => "Scroll right",
            SemanticsAction.copy => "Copy",
            SemanticsAction.cut => "Cut",
            SemanticsAction.paste => "Paste",
            _ => action.ToString(),
        };

    private AppleSemanticsView? _appleView;
    private NSObject[] _appleChildren = [];
    private SemanticsAction _appleActionMask;
    private int? _appleActionOwner;

    private void AttachAppleSemantics()
    {
        var view = Handler?.PlatformView as AppleSemanticsView;
        if (_appleView?.Handle == view?.Handle) return;
        if (_appleView is { } retired && retired.Handle != IntPtr.Zero)
        {
#if MACOS
            retired.AccessibilityElement = false;
            retired.AccessibilityChildren = [];
#else
            retired.IsAccessibilityElement = false;
            retired.Elements = [];
#endif
            retired.AccessibilityCustomActions = [];
        }
        _appleView = view;
        _appleChildren = [];
        _appleActionMask = SemanticsAction.none;
        _appleActionOwner = null;
        UpdateAppleNode(null, force: true);
        RefreshAppleChildren();
    }

    private void UpdateAppleNode(SemanticsNodeUpdate? previous, bool force = false)
    {
        if (_appleView is not { } native) return;
        var node = Node;
        if (!force && previous is not null && node is not null
            && previous.label == node.label && previous.value == node.value
            && previous.identifier == node.identifier && previous.hint == node.hint
            && previous.tooltip == node.tooltip && previous.flags == node.flags
            && previous.actions == node.actions && previous.id == node.id && previous.role == node.role
            && (previous.children.Count == 0) == (node.children.Count == 0))
            return;

        if (force || previous?.label != node?.label) native.AccessibilityLabel = node?.label;
        var value = node?.flags?.isObscured == true ? null : node?.value;
        var previousValue = previous?.flags?.isObscured == true ? null : previous?.value;
        var numeric = node?.flags?.isObscured == true ? null : AppleSemanticsState.NumericValue(node);
        var oldNumeric = previous?.flags?.isObscured == true ? null : AppleSemanticsState.NumericValue(previous);
#if MACOS
        if (force || (previous is null) != (node is null)) native.AccessibilityElement = node is not null;
        if (force || numeric != oldNumeric || (numeric is null && value != previousValue))
        {
            // AXCheckBox exposes a numeric state independently of AXSelected.
            using NSObject? projected = numeric is { } number ? new NSNumber(number)
                : value is { } text ? new NSString(text) : null;
            native.SetProjectedValue(projected);
        }
        if (force || Help(previous, previousValue, oldNumeric) != Help(node, value, numeric))
            native.AccessibilityHelp = Help(node, value, numeric);
        if (force || previous?.identifier != node?.identifier) native.AccessibilityIdentifier = node?.identifier;
        if (force || previous?.flags?.isEnabled != node?.flags?.isEnabled)
            native.AccessibilityEnabled = node?.flags?.isEnabled != Tristate.isFalse;
        if (force || previous?.flags?.isSelected != node?.flags?.isSelected)
            native.AccessibilitySelected = node?.flags?.isSelected == Tristate.isTrue;
        if (force || previous?.flags?.isExpanded != node?.flags?.isExpanded)
            native.SetProjectedExpanded(node?.flags?.isExpanded == Tristate.isTrue);
        if (force || previous?.flags?.isObscured != node?.flags?.isObscured)
            native.AccessibilityProtectedContent = node?.flags?.isObscured == true;
        if (force || IsDialog(previous) != IsDialog(node))
            native.AccessibilitySubrole = IsDialog(node) ? NSAccessibilitySubroles.DialogSubrole : null;
        var role = node is null ? NSAccessibilityRoles.UnknownRole : Role(node);
        if (force || role != (previous is null ? NSAccessibilityRoles.UnknownRole : Role(previous)))
            native.AccessibilityRole = role;
#else
        if (force || numeric != oldNumeric || value != previousValue)
            native.AccessibilityValue = AppleSemanticsState.TextValue(node,
                key => NSBundle.MainBundle.GetLocalizedString(key, key));
        var hint = node?.hint ?? node?.tooltip;
        if (force || hint != (previous?.hint ?? previous?.tooltip)) native.AccessibilityHint = hint;
        var traits = Traits(node);
        if (force || traits != Traits(previous)) native.AccessibilityTraits = traits;
        RefreshAppleContainer(native);
#endif
        if ((previous is null) != (node is null)) RefreshAppleChildren();
        RefreshAppleActions(native, node);
    }

    private void RefreshAppleChildren()
    {
        if (_appleView is not { } native) return;
        var children = Node is null ? Enumerable.Empty<NSObject>() : Children.OfType<MauiSemanticsLayout>()
            .Select(child => child.Handler?.PlatformView).OfType<NSObject>();
        if (_appleChildren.Select(child => child.Handle).SequenceEqual(children.Select(child => child.Handle))) return;
        _appleChildren = children.ToArray();
#if MACOS
        native.AccessibilityChildren = _appleChildren;
#else
        native.Elements = _appleChildren;
        RefreshAppleContainer(native);
#endif
    }

    private void RefreshAppleActions(AppleSemanticsView native, SemanticsNodeUpdate? node)
    {
        var mask = SemanticsAction.none;
        if (node is not null)
            foreach (var action in AppleActions)
                if (action != SemanticsAction.setText && SemanticsActionPolicy.Allows(node, action))
                    mask |= action;
        var id = node?.id;
        if (mask == _appleActionMask && id == _appleActionOwner) return;
        _appleActionMask = mask;
        _appleActionOwner = id;
        bool Invoke(SemanticsAction action) =>
            Node?.id == id && OwnsAppleSemantics(native) && Dispatch(action);
#if MACOS
        native.AccessibilityCustomActions = AppleActions.Where(action => (mask & action) != 0)
            .Select(action => new NSAccessibilityCustomAction(ActionName(action), () => Invoke(action)))
            .ToArray();
#else
        native.AccessibilityCustomActions = AppleActions.Where(action => (mask & action) != 0)
            .Select(action => new UIAccessibilityCustomAction(ActionName(action),
                new UIAccessibilityCustomActionHandler(_ => Invoke(action)))).ToArray();
#endif
    }

    internal bool OwnsAppleSemantics(AppleSemanticsView native) =>
        native.Handle != IntPtr.Zero && native.Handle == (Handler?.PlatformView as AppleSemanticsView)?.Handle;

#if MACOS
    private static bool IsDialog(SemanticsNodeUpdate? node) =>
        node?.role is SemanticsRole.dialog or SemanticsRole.alertDialog;

    private static string Help(SemanticsNodeUpdate? node, string? value, int? numeric)
    {
        var hint = node?.hint ?? node?.tooltip;
        var stateValue = numeric is null ? null : value;
        if (string.IsNullOrWhiteSpace(hint)) return string.IsNullOrWhiteSpace(stateValue) ? "" : stateValue;
        return string.IsNullOrWhiteSpace(stateValue) ? hint : hint + " " + stateValue;
    }
#else
    private void RefreshAppleContainer(AppleSemanticsView native)
    {
        var exposed = Node is not null && _appleChildren.Length == 0;
        if (native.IsAccessibilityElement != exposed) native.IsAccessibilityElement = exposed;
        var type = Node?.role switch
        {
            SemanticsRole.list => UIAccessibilityContainerType.List,
            SemanticsRole.table => UIAccessibilityContainerType.DataTable,
            _ => _appleChildren.Length > 0 ? UIAccessibilityContainerType.SemanticGroup : UIAccessibilityContainerType.None,
        };
        if (native.AccessibilityContainerType != type) native.AccessibilityContainerType = type;
    }

    private static UIAccessibilityTrait Traits(SemanticsNodeUpdate? node)
    {
        var traits = UIAccessibilityTrait.None;
        if (node?.flags?.isLink == true) traits |= UIAccessibilityTrait.Link;
        if (node?.flags?.isHeader == true) traits |= UIAccessibilityTrait.Header;
        if (node?.flags?.isImage == true) traits |= UIAccessibilityTrait.Image;
        if (node?.flags?.isButton == true || node?.actions.HasFlag(SemanticsAction.tap) == true)
            traits |= UIAccessibilityTrait.Button;
        if (node?.flags?.isSlider == true || node?.actions.HasFlag(SemanticsAction.increase) == true
            || node?.actions.HasFlag(SemanticsAction.decrease) == true) traits |= UIAccessibilityTrait.Adjustable;
        if (node?.flags?.isEnabled == Tristate.isFalse) traits |= UIAccessibilityTrait.NotEnabled;
        if (node?.flags?.isSelected == Tristate.isTrue) traits |= UIAccessibilityTrait.Selected;
        return traits;
    }
#endif

#if MACOS
    private static string Role(SemanticsNodeUpdate node) =>
        node.role switch
        {
            SemanticsRole.dialog or SemanticsRole.alertDialog => NSAccessibilityRoles.GroupRole,
            SemanticsRole.list => NSAccessibilityRoles.ListRole,
            SemanticsRole.listItem => NSAccessibilityRoles.RowRole,
            SemanticsRole.table => NSAccessibilityRoles.TableRole,
            SemanticsRole.cell => NSAccessibilityRoles.CellRole,
            SemanticsRole.row => NSAccessibilityRoles.RowRole,
            SemanticsRole.columnHeader => NSAccessibilityRoles.ColumnRole,
            SemanticsRole.menu => NSAccessibilityRoles.MenuRole,
            SemanticsRole.menuBar => "AXMenuBar",
            SemanticsRole.menuItem
            or SemanticsRole.menuItemCheckbox
            or SemanticsRole.menuItemRadio => NSAccessibilityRoles.MenuItemRole,
            SemanticsRole.tabBar => NSAccessibilityRoles.TabGroupRole,
            SemanticsRole.tab => NSAccessibilityRoles.RadioButtonRole,
            SemanticsRole.comboBox => NSAccessibilityRoles.ComboBoxRole,
            SemanticsRole.progressBar => NSAccessibilityRoles.ProgressIndicatorRole,
            SemanticsRole.loadingSpinner => NSAccessibilityRoles.BusyIndicatorRole,
            _ when node.flags?.isTextField == true => NSAccessibilityRoles.TextFieldRole,
            _ when node.flags?.isSlider == true => NSAccessibilityRoles.SliderRole,
            _ when node.flags?.isLink == true => NSAccessibilityRoles.LinkRole,
            _ when node.flags?.isHeader == true => OperatingSystem.IsMacOSVersionAtLeast(26)
                ? NSAccessibilityRoles.HeadingRole
                : NSAccessibilityRoles.StaticTextRole,
            _ when node.flags?.isImage == true => NSAccessibilityRoles.ImageRole,
            _ when node.flags?.isInMutuallyExclusiveGroup == true =>
                NSAccessibilityRoles.RadioButtonRole,
            _ when node.flags?.isChecked
                    is CheckedState.isTrue
                        or CheckedState.isFalse
                        or CheckedState.mixed
                    || node.flags?.isToggled is Tristate.isTrue or Tristate.isFalse =>
                NSAccessibilityRoles.CheckBoxRole,
            _ when node.flags?.isButton == true || node.actions.HasFlag(SemanticsAction.tap) =>
                NSAccessibilityRoles.ButtonRole,
            _ => node.children.Count == 0
                ? NSAccessibilityRoles.StaticTextRole
                : NSAccessibilityRoles.GroupRole,
        };
#endif
}

#if MACOS
internal sealed class MauiSemanticsLayoutHandler : DorotiMacOSLayoutHandler
{
    protected override MacOSContainerView CreatePlatformView() =>
        new AppleSemanticsView((MauiSemanticsLayout)VirtualView);
}

internal sealed class AppleSemanticsView(MauiSemanticsLayout owner) : MacOSContainerView
{
    private bool Dispatch(SemanticsAction action) =>
        ReferenceEquals(owner.Handler?.PlatformView, this) && owner.Dispatch(action);

    internal void SetProjectedValue(NSObject? value) => base.AccessibilityValue = value;
    internal void SetProjectedExpanded(bool value) => base.AccessibilityExpanded = value;
    public override bool AccessibilityExpanded
    {
        get => base.AccessibilityExpanded;
        set => Dispatch(value ? SemanticsAction.expand : SemanticsAction.collapse);
    }

    public override NSObject? AccessibilityValue
    {
        get => base.AccessibilityValue;
        set
        {
            if (
                ReferenceEquals(owner.Handler?.PlatformView, this)
                && value is NSString text
                && owner.Node?.flags?.isTextField == true
            )
                owner.Dispatch(SemanticsAction.setText, text.ToString());
        }
    }

    public override bool IsAccessibilitySelectorAllowed(ObjCRuntime.Selector selector) =>
        selector.Name switch
        {
            "setAccessibilityValue:" => owner.Node is { flags.isTextField: true } node
                && SemanticsActionPolicy.Allows(node, SemanticsAction.setText),
            "setAccessibilityExpanded:" => owner.Node is { } expanded
                && (SemanticsActionPolicy.Allows(expanded, SemanticsAction.expand)
                    || SemanticsActionPolicy.Allows(expanded, SemanticsAction.collapse)),
            "setAccessibilityFocused:" or "setAccessibilitySelected:" or "setAccessibilitySelectedTextRange:" => false,
            "accessibilityPerformPress" => owner.Node is { } press
                && SemanticsActionPolicy.Allows(press, SemanticsAction.tap),
            "accessibilityPerformIncrement" => owner.Node is { } increase
                && SemanticsActionPolicy.Allows(increase, SemanticsAction.increase),
            "accessibilityPerformDecrement" => owner.Node is { } decrease
                && SemanticsActionPolicy.Allows(decrease, SemanticsAction.decrease),
            "accessibilityPerformCancel" => owner.Node is { } dismiss
                && SemanticsActionPolicy.Allows(dismiss, SemanticsAction.dismiss),
            _ => base.IsAccessibilitySelectorAllowed(selector),
        };

    public override bool AccessibilityPerformPress() => Dispatch(SemanticsAction.tap);

    public override bool AccessibilityPerformIncrement() => Dispatch(SemanticsAction.increase);

    public override bool AccessibilityPerformDecrement() => Dispatch(SemanticsAction.decrease);

    public override bool AccessibilityPerformCancel() => Dispatch(SemanticsAction.dismiss);
}
#else
internal sealed class MauiSemanticsLayoutHandler : LayoutHandler
{
    protected override LayoutView CreatePlatformView() => new AppleSemanticsView((MauiSemanticsLayout)VirtualView);
}

internal sealed class AppleSemanticsView(MauiSemanticsLayout owner) : LayoutView, IUIAccessibilityContainer
{
    private bool Dispatch(SemanticsAction action) => ReferenceEquals(owner.Handler?.PlatformView, this) && owner.Dispatch(action);
    internal NSObject[] Elements { get; set; } = [];
    [Export("accessibilityContainerType")]
    public UIAccessibilityContainerType AccessibilityContainerType { get; set; }
    [Export("accessibilityElementCount")]
    public nint AccessibilityElementCount() => Elements.Length;
    [Export("accessibilityElementAtIndex:")]
    public NSObject GetAccessibilityElementAt(nint index) => index >= 0 && index < Elements.Length ? Elements[(int)index] : null!;
    [Export("indexOfAccessibilityElement:")]
    public nint GetIndexOfAccessibilityElement(NSObject element)
    {
        var index = Array.IndexOf(Elements, element);
        return index < 0 ? nint.MaxValue : index;
    }
    public override bool AccessibilityActivate() => Dispatch(SemanticsAction.tap);
    public override void AccessibilityIncrement() => Dispatch(SemanticsAction.increase);
    public override void AccessibilityDecrement() => Dispatch(SemanticsAction.decrease);
    public override bool AccessibilityScroll(UIAccessibilityScrollDirection direction) => direction switch
    {
        UIAccessibilityScrollDirection.Up => Dispatch(SemanticsAction.scrollUp),
        UIAccessibilityScrollDirection.Down => Dispatch(SemanticsAction.scrollDown),
        UIAccessibilityScrollDirection.Left => Dispatch(SemanticsAction.scrollLeft),
        UIAccessibilityScrollDirection.Right => Dispatch(SemanticsAction.scrollRight),
        _ => false,
    };
}
#endif
#endif
