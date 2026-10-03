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

    private void RefreshAppleSemantics()
    {
        var node = Node;
#if MACOS
        if (Handler?.PlatformView is not AppleSemanticsView native)
            return;
        native.AccessibilityElement = node is not null;
        native.AccessibilityLabel = node?.label;
        using var value =
            node?.flags?.isObscured == true || node?.value is null
                ? null
                : new NSString(node.value);
        native.SetProjectedValue(value);
        native.AccessibilityHelp = node?.hint ?? node?.tooltip;
        native.AccessibilityIdentifier = node?.identifier;
        native.AccessibilityEnabled = node?.flags?.isEnabled != Tristate.isFalse;
        native.AccessibilitySelected = node?.flags?.isSelected == Tristate.isTrue;
        native.SetProjectedExpanded(node?.flags?.isExpanded == Tristate.isTrue);
        native.AccessibilityProtectedContent = node?.flags?.isObscured == true;
        native.AccessibilitySubrole = node?.role
            is SemanticsRole.dialog
                or SemanticsRole.alertDialog
            ? NSAccessibilitySubroles.DialogSubrole
            : null;
        native.AccessibilityRole = node is null ? NSAccessibilityRoles.UnknownRole : Role(node);
        // Explicit semantic children omit the transparent MAUI control proxy.
        native.AccessibilityChildren = Children
            .OfType<MauiSemanticsLayout>()
            .Select(child => child.Handler?.PlatformView)
            .OfType<NSObject>()
            .ToArray();
        native.AccessibilityCustomActions = node is null
            ? []
            : AppleActions
                .Where(action =>
                    action != SemanticsAction.setText && SemanticsActionPolicy.Allows(node, action)
                )
                .Select(action => new NSAccessibilityCustomAction(
                    ActionName(action),
                    () =>
                        Node?.id == node.id
                        && ReferenceEquals(Handler?.PlatformView, native)
                        && Dispatch(action)
                ))
                .ToArray();
#else
        if (Handler?.PlatformView is not AppleSemanticsView native) return;
        var children = Children.OfType<MauiSemanticsLayout>()
            .Select(child => child.Handler?.PlatformView).OfType<NSObject>().ToArray();
        native.IsAccessibilityElement = node is not null && children.Length == 0;
        native.AccessibilityLabel = node?.label;
        native.AccessibilityValue = node?.flags?.isObscured == true ? null : node?.value;
        native.AccessibilityHint = node?.hint ?? node?.tooltip;
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
        native.AccessibilityTraits = traits;
        native.AccessibilityContainerType = node?.role switch
        {
            SemanticsRole.list => UIAccessibilityContainerType.List,
            SemanticsRole.table => UIAccessibilityContainerType.DataTable,
            _ => children.Length > 0 ? UIAccessibilityContainerType.SemanticGroup : UIAccessibilityContainerType.None,
        };
        native.Elements = children;
        native.AccessibilityCustomActions = node is null ? [] : AppleActions
            .Where(action => action != SemanticsAction.setText && SemanticsActionPolicy.Allows(node, action))
            .Select(action => new UIAccessibilityCustomAction(ActionName(action),
                new UIAccessibilityCustomActionHandler(_ => Node?.id == node.id
                    && ReferenceEquals(Handler?.PlatformView, native) && Dispatch(action)))).ToArray();
#endif
    }

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
