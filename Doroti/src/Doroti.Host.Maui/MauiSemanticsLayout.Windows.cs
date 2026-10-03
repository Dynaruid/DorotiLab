#if WINDOWS
using Doroti.Ui;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
using IValueProvider = Microsoft.UI.Xaml.Automation.Provider.IValueProvider;

namespace Doroti.Host.Maui;

internal sealed class MauiSemanticsLayoutHandler : LayoutHandler
{
    protected override LayoutPanel CreatePlatformView() => new SemanticsPanel((MauiSemanticsLayout)VirtualView)
        { CrossPlatformLayout = VirtualView };
}

internal sealed class SemanticsPanel(MauiSemanticsLayout view) : LayoutPanel
{
    internal MauiSemanticsLayout View => view;
    protected override AutomationPeer OnCreateAutomationPeer() => new SemanticsPeer(this);
}

internal sealed class SemanticsPeer : FrameworkElementAutomationPeer, IInvokeProvider, IValueProvider,
    IExpandCollapseProvider, IScrollItemProvider, IToggleProvider
{
    private readonly MauiSemanticsLayout _view;
    private readonly int _id;
    internal SemanticsPeer(SemanticsPanel panel) : base(panel)
    {
        _view = panel.View;
        _id = _view.Node?.id ?? -1;
    }
    private SemanticsNodeUpdate Node => _view.Node is { } node && node.id == _id
        ? node : throw new ElementNotAvailableException();
    protected override string GetClassNameCore() => "DorotiSemantics";
    protected override string GetNameCore() => Node.label ?? (Node.flags?.isObscured == true ? "" : Node.value) ?? "";
    protected override string GetAutomationIdCore() => Node.identifier ?? $"doroti-semantics-{_id}";
    protected override string GetHelpTextCore() => Node.hint ?? Node.tooltip ?? "";
    protected override string GetLocalizedControlTypeCore() => Node.role.ToString();
    protected override bool IsEnabledCore() => Node.flags?.isEnabled != Tristate.isFalse;
    protected override bool IsPasswordCore() => Node.flags?.isObscured == true;
    protected override bool IsControlElementCore() => !Node.rect.isEmpty;
    protected override bool IsContentElementCore() => !Node.rect.isEmpty;
    protected override bool IsKeyboardFocusableCore() => Node.actions.HasFlag(SemanticsAction.focus)
        || Node.actions.HasFlag(SemanticsAction.didGainAccessibilityFocus);
    protected override bool HasKeyboardFocusCore() => Node.flags?.isFocused == Tristate.isTrue;
    protected override void SetFocusCore() => Dispatch(Node.actions.HasFlag(SemanticsAction.focus)
        ? SemanticsAction.focus : SemanticsAction.didGainAccessibilityFocus);
    protected override IList<AutomationPeer> GetChildrenCore() => _view.Children.OfType<MauiSemanticsLayout>()
        .Select(child => child.Handler?.PlatformView as FrameworkElement)
        .Where(child => child is not null).Select(child => CreatePeerForElement(child!))
        .Where(peer => peer is not null).ToList()!;
    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        var node = Node;
        if (node.role == SemanticsRole.none && node.flags?.isTextField == true) return AutomationControlType.Edit;
        if (node.role == SemanticsRole.none && node.flags?.isSlider == true) return AutomationControlType.Slider;
        if (node.role == SemanticsRole.none && node.flags?.isInMutuallyExclusiveGroup == true) return AutomationControlType.RadioButton;
        if (node.role == SemanticsRole.none && node.flags?.isChecked is CheckedState.isTrue or CheckedState.isFalse or CheckedState.mixed) return AutomationControlType.CheckBox;
        if (node.role == SemanticsRole.none && (node.flags?.isButton == true || node.actions.HasFlag(SemanticsAction.tap))) return AutomationControlType.Button;
        return node.role switch
        {
            SemanticsRole.dialog or SemanticsRole.alertDialog => AutomationControlType.Window,
            SemanticsRole.list => AutomationControlType.List,
            SemanticsRole.listItem => AutomationControlType.ListItem,
            SemanticsRole.table => AutomationControlType.DataGrid,
            SemanticsRole.row or SemanticsRole.cell => AutomationControlType.DataItem,
            SemanticsRole.columnHeader => AutomationControlType.HeaderItem,
            SemanticsRole.menu => AutomationControlType.Menu,
            SemanticsRole.menuBar => AutomationControlType.MenuBar,
            SemanticsRole.menuItem or SemanticsRole.menuItemCheckbox or SemanticsRole.menuItemRadio => AutomationControlType.MenuItem,
            SemanticsRole.tab => AutomationControlType.TabItem,
            SemanticsRole.tabBar => AutomationControlType.Tab,
            SemanticsRole.comboBox => AutomationControlType.ComboBox,
            SemanticsRole.spinButton => AutomationControlType.Spinner,
            SemanticsRole.progressBar or SemanticsRole.loadingSpinner => AutomationControlType.ProgressBar,
            SemanticsRole.none when node.children.Count == 0 => AutomationControlType.Text,
            _ => AutomationControlType.Group,
        };
    }
    protected override object? GetPatternCore(PatternInterface pattern) => pattern switch
    {
        PatternInterface.Invoke when Node.actions.HasFlag(SemanticsAction.tap) => this,
        PatternInterface.Value when Node.flags?.isTextField == true => this,
        PatternInterface.ExpandCollapse when (Node.actions & (SemanticsAction.expand | SemanticsAction.collapse)) != 0 => this,
        PatternInterface.ScrollItem when Node.actions.HasFlag(SemanticsAction.showOnScreen) => this,
        PatternInterface.Toggle when Node.flags?.isChecked is CheckedState.isTrue or CheckedState.isFalse or CheckedState.mixed
            || Node.flags?.isToggled is Tristate.isTrue or Tristate.isFalse => this,
        _ => base.GetPatternCore(pattern),
    };
    private void Dispatch(SemanticsAction action, object? arguments = null)
    {
        _ = Node;
        if (!_view.Dispatch(action, arguments)) throw new InvalidOperationException("The current semantic node does not allow this action.");
    }
    public void Invoke() => Dispatch(SemanticsAction.tap);
    public bool IsReadOnly => Node.flags?.isReadOnly == true || !Node.actions.HasFlag(SemanticsAction.setText);
    public string Value => Node.flags?.isObscured == true ? "" : Node.value ?? "";
    public void SetValue(string value) => Dispatch(SemanticsAction.setText, value);
    public ExpandCollapseState ExpandCollapseState => Node.flags?.isExpanded == Tristate.isTrue
        ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
    public void Expand() => Dispatch(SemanticsAction.expand);
    public void Collapse() => Dispatch(SemanticsAction.collapse);
    public void ScrollIntoView() => Dispatch(SemanticsAction.showOnScreen);
    public ToggleState ToggleState => Node.flags?.isChecked == CheckedState.mixed ? ToggleState.Indeterminate
        : Node.flags?.isChecked == CheckedState.isTrue || Node.flags?.isToggled == Tristate.isTrue ? ToggleState.On : ToggleState.Off;
    public void Toggle() => Dispatch(SemanticsAction.tap);
}
#endif
