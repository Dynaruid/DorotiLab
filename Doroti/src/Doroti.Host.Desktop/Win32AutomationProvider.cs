using System.Runtime.InteropServices;
using Doroti.Graphics;
using Doroti.Platform;
using Doroti.Vendor.Avalonia.Win32;

namespace Doroti.Host.Desktop;

// Adapted from the A0-pinned Avalonia Win32 automation provider seam. Doroti owns
// the semantics-to-UIA mapping while the source-ported window owns WM_GETOBJECT.
[ComVisible(true)]
internal sealed class Win32AutomationRootProvider :
    IRawElementProviderSimple,
    IRawElementProviderFragment,
    IRawElementProviderFragmentRoot
{
    private readonly NativeWindowHost _host;
    private readonly string _title;
    private AutomationNodeProvider? _semanticsRoot;
    private Func<SemanticsActionRequest, bool>? _performAction;

    internal Win32AutomationRootProvider(NativeWindowHost host, string title)
    {
        _host = host;
        _title = title;
    }

    internal void Update(SemanticsTreeSnapshot snapshot, Func<SemanticsActionRequest, bool> performAction)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(performAction);
        var root = AutomationNodeProvider.Create(this, snapshot.Root, null);
        Volatile.Write(ref _performAction, performAction);
        Volatile.Write(ref _semanticsRoot, root);
    }

    internal void Clear()
    {
        Volatile.Write(ref _semanticsRoot, null);
        Volatile.Write(ref _performAction, null);
    }

    internal nint HandleGetObject(nint window, nuint wParam, nint lParam) =>
        UiaCoreProviderApi.ReturnRawElementProvider(window, unchecked((nint)wParam), lParam, this);

    internal bool Invoke(SemanticsActionRequest request) =>
        Volatile.Read(ref _performAction)?.Invoke(request) is true;

    internal UiaRect GetNodeBounds(Rect logicalBounds)
    {
        var bounds = _host.LogicalClientRectToScreen(
            logicalBounds.Left,
            logicalBounds.Top,
            logicalBounds.Right,
            logicalBounds.Bottom);
        return new(bounds.Left, bounds.Top, Math.Max(0, bounds.Width), Math.Max(0, bounds.Height));
    }

    ProviderOptions IRawElementProviderSimple.GetProviderOptions() =>
        ProviderOptions.ServerSideProvider | ProviderOptions.UseComThreading;

    object? IRawElementProviderSimple.GetPatternProvider(int patternId) => null;

    object? IRawElementProviderSimple.GetPropertyValue(int propertyId) => propertyId switch
    {
        UiaPropertyId.ProcessId => Environment.ProcessId,
        UiaPropertyId.ControlType => UiaControlTypeId.Window,
        UiaPropertyId.Name => _title,
        UiaPropertyId.IsKeyboardFocusable => true,
        UiaPropertyId.IsEnabled => true,
        UiaPropertyId.AutomationId => $"doroti-window-{_host.WindowId}",
        UiaPropertyId.ClassName => "Doroti.SourcePortWindow",
        UiaPropertyId.IsControlElement => true,
        UiaPropertyId.IsContentElement => true,
        UiaPropertyId.NativeWindowHandle => unchecked((int)_host.Handle),
        UiaPropertyId.FrameworkId => "Doroti",
        _ => null,
    };

    IRawElementProviderSimple? IRawElementProviderSimple.GetHostRawElementProvider() =>
        UiaCoreProviderApi.GetHostProvider(_host.Handle);

    IRawElementProviderFragment? IRawElementProviderFragment.Navigate(NavigateDirection direction) => direction switch
    {
        NavigateDirection.FirstChild or NavigateDirection.LastChild => Volatile.Read(ref _semanticsRoot),
        _ => null,
    };

    int[]? IRawElementProviderFragment.GetRuntimeId() => null;

    UiaRect IRawElementProviderFragment.GetBoundingRectangle()
    {
        var bounds = _host.WindowBounds;
        return new(bounds.Left, bounds.Top, Math.Max(0, bounds.Width), Math.Max(0, bounds.Height));
    }

    IRawElementProviderSimple[]? IRawElementProviderFragment.GetEmbeddedFragmentRoots() => null;

    void IRawElementProviderFragment.SetFocus() => _ = NativeInterop.SetFocus(_host.Handle);

    IRawElementProviderFragmentRoot IRawElementProviderFragment.GetFragmentRoot() => this;

    IRawElementProviderFragment? IRawElementProviderFragmentRoot.ElementProviderFromPoint(double x, double y) =>
        Volatile.Read(ref _semanticsRoot)?.HitTest(x, y) ?? (IRawElementProviderFragment)this;

    IRawElementProviderFragment? IRawElementProviderFragmentRoot.GetFocus() =>
        Volatile.Read(ref _semanticsRoot)?.FindFocused();

    internal void FocusNativeWindow() => _ = NativeInterop.SetFocus(_host.Handle);
}

[ComVisible(true)]
internal sealed class AutomationNodeProvider :
    IRawElementProviderSimple,
    IRawElementProviderFragment,
    IInvokeProvider,
    IValueProvider,
    IToggleProvider,
    IScrollProvider
{
    private readonly Win32AutomationRootProvider _root;
    private readonly AutomationNodeProvider? _parent;
    private readonly SemanticsNodeSnapshot _node;

    private AutomationNodeProvider(
        Win32AutomationRootProvider root,
        SemanticsNodeSnapshot node,
        AutomationNodeProvider? parent)
    {
        _root = root;
        _node = node;
        _parent = parent;
    }

    internal AutomationNodeProvider[] Children { get; private set; } = [];

    internal static AutomationNodeProvider Create(
        Win32AutomationRootProvider root,
        SemanticsNodeSnapshot node,
        AutomationNodeProvider? parent)
    {
        var provider = new AutomationNodeProvider(root, node, parent);
        provider.Children = node.Children.Select(child => Create(root, child, provider)).ToArray();
        return provider;
    }

    internal AutomationNodeProvider? FindFocused()
    {
        if ((_node.State & SemanticsState.Focused) != 0)
        {
            return this;
        }
        return Children.Select(child => child.FindFocused()).FirstOrDefault(child => child is not null);
    }

    internal AutomationNodeProvider? HitTest(double x, double y)
    {
        var bounds = _root.GetNodeBounds(_node.Bounds);
        if (!bounds.Contains(x, y))
        {
            return null;
        }
        for (var index = Children.Length - 1; index >= 0; index--)
        {
            if (Children[index].HitTest(x, y) is { } child)
            {
                return child;
            }
        }
        return this;
    }

    ProviderOptions IRawElementProviderSimple.GetProviderOptions() =>
        ProviderOptions.ServerSideProvider | ProviderOptions.UseComThreading;

    object? IRawElementProviderSimple.GetPatternProvider(int patternId) => patternId switch
    {
        UiaPatternId.Invoke when (_node.Actions & SemanticsAction.Tap) != 0 => this,
        UiaPatternId.Value when (_node.Actions & SemanticsAction.SetText) != 0 => this,
        UiaPatternId.Toggle when (_node.Actions & SemanticsAction.Toggle) != 0 => this,
        UiaPatternId.Scroll when (_node.Actions & (SemanticsAction.ScrollUp | SemanticsAction.ScrollDown)) != 0 => this,
        _ => null,
    };

    object? IRawElementProviderSimple.GetPropertyValue(int propertyId) => propertyId switch
    {
        UiaPropertyId.ProcessId => Environment.ProcessId,
        UiaPropertyId.ControlType => MapControlType(_node.Role),
        UiaPropertyId.Name => _node.Label ?? string.Empty,
        UiaPropertyId.HasKeyboardFocus => (_node.State & SemanticsState.Focused) != 0,
        UiaPropertyId.IsKeyboardFocusable => (_node.Actions & SemanticsAction.Focus) != 0,
        UiaPropertyId.IsEnabled => (_node.State & SemanticsState.Enabled) != 0,
        UiaPropertyId.AutomationId => $"semantics-{_node.Id}",
        UiaPropertyId.ClassName => $"Doroti.{_node.Role}",
        UiaPropertyId.IsControlElement => (_node.State & SemanticsState.Hidden) == 0,
        UiaPropertyId.IsContentElement => (_node.State & SemanticsState.Hidden) == 0,
        UiaPropertyId.IsOffscreen => (_node.State & SemanticsState.Hidden) != 0,
        UiaPropertyId.FrameworkId => "Doroti",
        UiaPropertyId.IsInvokePatternAvailable => (_node.Actions & SemanticsAction.Tap) != 0,
        UiaPropertyId.IsValuePatternAvailable => (_node.Actions & SemanticsAction.SetText) != 0,
        UiaPropertyId.IsTogglePatternAvailable => (_node.Actions & SemanticsAction.Toggle) != 0,
        UiaPropertyId.IsScrollPatternAvailable => (_node.Actions & (SemanticsAction.ScrollUp | SemanticsAction.ScrollDown)) != 0,
        UiaPropertyId.ValueValue => _node.Value ?? string.Empty,
        _ => null,
    };

    IRawElementProviderSimple? IRawElementProviderSimple.GetHostRawElementProvider() => null;

    IRawElementProviderFragment? IRawElementProviderFragment.Navigate(NavigateDirection direction) => direction switch
    {
        NavigateDirection.Parent => _parent ?? (IRawElementProviderFragment)_root,
        NavigateDirection.FirstChild => Children.FirstOrDefault(),
        NavigateDirection.LastChild => Children.LastOrDefault(),
        NavigateDirection.NextSibling => GetSibling(1),
        NavigateDirection.PreviousSibling => GetSibling(-1),
        _ => null,
    };

    int[] IRawElementProviderFragment.GetRuntimeId() => [UiaCoreProviderApi.AppendRuntimeId, _node.Id];

    UiaRect IRawElementProviderFragment.GetBoundingRectangle() => _root.GetNodeBounds(_node.Bounds);

    IRawElementProviderSimple[]? IRawElementProviderFragment.GetEmbeddedFragmentRoots() => null;

    void IRawElementProviderFragment.SetFocus()
    {
        _root.FocusNativeWindow();
        if ((_node.Actions & SemanticsAction.Focus) != 0)
        {
            _ = _root.Invoke(new(_node.Id, SemanticsAction.Focus));
        }
    }

    IRawElementProviderFragmentRoot IRawElementProviderFragment.GetFragmentRoot() => _root;

    void IInvokeProvider.Invoke()
    {
        if ((_node.State & SemanticsState.Enabled) == 0)
        {
            throw new COMException("The Doroti semantics node is disabled.", UiaCoreProviderApi.ElementNotEnabled);
        }
        if ((_node.Actions & SemanticsAction.Tap) == 0 || !_root.Invoke(new(_node.Id, SemanticsAction.Tap)))
        {
            throw new InvalidOperationException($"Doroti semantics node {_node.Id} rejected Invoke.");
        }
    }

    bool IValueProvider.GetIsReadOnly() => (_node.State & SemanticsState.ReadOnly) != 0;

    string IValueProvider.GetValue() => _node.Value ?? string.Empty;

    void IValueProvider.SetValue(string value)
    {
        if ((_node.Actions & SemanticsAction.SetText) == 0 || !_root.Invoke(new(_node.Id, SemanticsAction.SetText, value)))
        {
            throw new InvalidOperationException($"Doroti semantics node {_node.Id} rejected SetValue.");
        }
    }

    ToggleState IToggleProvider.GetToggleState() => (_node.State & SemanticsState.Mixed) != 0
        ? ToggleState.Indeterminate
        : (_node.State & (SemanticsState.Checked | SemanticsState.Toggled)) != 0
            ? ToggleState.On
            : ToggleState.Off;

    void IToggleProvider.Toggle()
    {
        if ((_node.Actions & SemanticsAction.Toggle) == 0 || !_root.Invoke(new(_node.Id, SemanticsAction.Toggle)))
        {
            throw new InvalidOperationException($"Doroti semantics node {_node.Id} rejected Toggle.");
        }
    }

    void IScrollProvider.Scroll(ScrollAmount horizontalAmount, ScrollAmount verticalAmount)
    {
        var action = verticalAmount < ScrollAmount.NoAmount ? SemanticsAction.ScrollUp :
            verticalAmount > ScrollAmount.NoAmount ? SemanticsAction.ScrollDown : SemanticsAction.None;
        if (action == SemanticsAction.None || (_node.Actions & action) == 0 || !_root.Invoke(new(_node.Id, action)))
        {
            throw new InvalidOperationException($"Doroti semantics node {_node.Id} rejected Scroll.");
        }
    }

    void IScrollProvider.SetScrollPercent(double horizontalPercent, double verticalPercent)
    {
        var action = verticalPercent <= 0 ? SemanticsAction.ScrollUp : SemanticsAction.ScrollDown;
        if ((_node.Actions & action) == 0 || !_root.Invoke(new(_node.Id, action, verticalPercent)))
        {
            throw new InvalidOperationException($"Doroti semantics node {_node.Id} rejected SetScrollPercent.");
        }
    }

    double IScrollProvider.GetHorizontalScrollPercent() => UiaCoreProviderApi.NoScroll;
    double IScrollProvider.GetHorizontalViewSize() => 100;
    bool IScrollProvider.GetHorizontallyScrollable() => false;
    double IScrollProvider.GetVerticalScrollPercent() => 0;
    double IScrollProvider.GetVerticalViewSize() => 50;
    bool IScrollProvider.GetVerticallyScrollable() => (_node.Actions & (SemanticsAction.ScrollUp | SemanticsAction.ScrollDown)) != 0;

    private AutomationNodeProvider? GetSibling(int offset)
    {
        if (_parent is null)
        {
            return null;
        }
        var index = Array.IndexOf(_parent.Children, this);
        var sibling = index + offset;
        return sibling >= 0 && sibling < _parent.Children.Length ? _parent.Children[sibling] : null;
    }

    private static int MapControlType(SemanticsRole role) => role switch
    {
        SemanticsRole.Button => UiaControlTypeId.Button,
        SemanticsRole.Text => UiaControlTypeId.Text,
        SemanticsRole.TextField => UiaControlTypeId.Edit,
        SemanticsRole.Image => UiaControlTypeId.Image,
        SemanticsRole.List => UiaControlTypeId.List,
        SemanticsRole.ListItem => UiaControlTypeId.ListItem,
        SemanticsRole.Dialog => UiaControlTypeId.Window,
        SemanticsRole.CheckBox => UiaControlTypeId.CheckBox,
        SemanticsRole.Slider => UiaControlTypeId.Slider,
        _ => UiaControlTypeId.Custom,
    };
}

[Flags]
internal enum ProviderOptions
{
    ServerSideProvider = 0x0002,
    UseComThreading = 0x0020,
}

internal enum NavigateDirection
{
    Parent,
    NextSibling,
    PreviousSibling,
    FirstChild,
    LastChild,
}

[StructLayout(LayoutKind.Sequential)]
internal readonly record struct UiaRect(double Left, double Top, double Width, double Height)
{
    internal bool Contains(double x, double y) =>
        x >= Left && x <= Left + Width && y >= Top && y <= Top + Height;
}

internal static class UiaPropertyId
{
    internal const int ProcessId = 30002;
    internal const int ControlType = 30003;
    internal const int Name = 30005;
    internal const int HasKeyboardFocus = 30008;
    internal const int IsKeyboardFocusable = 30009;
    internal const int IsEnabled = 30010;
    internal const int AutomationId = 30011;
    internal const int ClassName = 30012;
    internal const int IsControlElement = 30016;
    internal const int IsContentElement = 30017;
    internal const int NativeWindowHandle = 30020;
    internal const int IsOffscreen = 30022;
    internal const int FrameworkId = 30024;
    internal const int IsInvokePatternAvailable = 30031;
    internal const int IsScrollPatternAvailable = 30034;
    internal const int IsTogglePatternAvailable = 30041;
    internal const int IsValuePatternAvailable = 30043;
    internal const int ValueValue = 30045;
}

internal static class UiaPatternId
{
    internal const int Invoke = 10000;
    internal const int Value = 10002;
    internal const int Scroll = 10004;
    internal const int Toggle = 10015;
}

internal static class UiaControlTypeId
{
    internal const int Button = 50000;
    internal const int Edit = 50004;
    internal const int Image = 50006;
    internal const int CheckBox = 50002;
    internal const int ListItem = 50007;
    internal const int List = 50008;
    internal const int Slider = 50015;
    internal const int Text = 50020;
    internal const int Custom = 50025;
    internal const int Window = 50032;
}

internal static class UiaCoreProviderApi
{
    internal const int AppendRuntimeId = 3;
    internal const int ElementNotEnabled = unchecked((int)0x80040200);
    internal const double NoScroll = -1;

    [DllImport("UIAutomationCore.dll")]
    private static extern nint UiaReturnRawElementProvider(
        nint window,
        nint wParam,
        nint lParam,
        [MarshalAs(UnmanagedType.Interface)] IRawElementProviderSimple provider);

    [DllImport("UIAutomationCore.dll")]
    private static extern int UiaHostProviderFromHwnd(
        nint window,
        [MarshalAs(UnmanagedType.Interface)] out IRawElementProviderSimple provider);

    internal static IRawElementProviderSimple? GetHostProvider(nint window) =>
        window != 0 && UiaHostProviderFromHwnd(window, out var provider) >= 0 ? provider : null;

    internal static nint ReturnRawElementProvider(
        nint window,
        nint wParam,
        nint lParam,
        IRawElementProviderSimple provider) => UiaReturnRawElementProvider(window, wParam, lParam, provider);
}

[ComVisible(true)]
[ComImport]
[Guid("d6dd68d1-86fd-4332-8666-9abedea2d24c")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IRawElementProviderSimple
{
    ProviderOptions GetProviderOptions();

    [return: MarshalAs(UnmanagedType.IUnknown)]
    object? GetPatternProvider(int patternId);

    [return: MarshalAs(UnmanagedType.Struct)]
    object? GetPropertyValue(int propertyId);

    [return: MarshalAs(UnmanagedType.Interface)]
    IRawElementProviderSimple? GetHostRawElementProvider();
}

[ComVisible(true)]
[ComImport]
[Guid("f7063da8-8359-439c-9297-bbc5299a7d87")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IRawElementProviderFragment
{
    [return: MarshalAs(UnmanagedType.Interface)]
    IRawElementProviderFragment? Navigate(NavigateDirection direction);

    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4)]
    int[]? GetRuntimeId();

    UiaRect GetBoundingRectangle();

    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)]
    IRawElementProviderSimple[]? GetEmbeddedFragmentRoots();

    void SetFocus();

    [return: MarshalAs(UnmanagedType.Interface)]
    IRawElementProviderFragmentRoot GetFragmentRoot();
}

[ComVisible(true)]
[ComImport]
[Guid("620ce2a5-ab8f-40a9-86cb-de3c75599b58")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IRawElementProviderFragmentRoot
{
    [return: MarshalAs(UnmanagedType.Interface)]
    IRawElementProviderFragment? ElementProviderFromPoint(double x, double y);

    [return: MarshalAs(UnmanagedType.Interface)]
    IRawElementProviderFragment? GetFocus();
}

[ComVisible(true)]
[ComImport]
[Guid("54fcb24b-e18e-47a2-b4d3-eccbe77599a2")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IInvokeProvider
{
    void Invoke();
}

internal enum ToggleState
{
    Off,
    On,
    Indeterminate,
}

internal enum ScrollAmount
{
    LargeDecrement,
    SmallDecrement,
    NoAmount,
    LargeIncrement,
    SmallIncrement,
}

[ComVisible(true)]
[ComImport]
[Guid("c7935180-6fb3-4201-b174-7df73adbf64a")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IValueProvider
{
    void SetValue([MarshalAs(UnmanagedType.LPWStr)] string value);

    [return: MarshalAs(UnmanagedType.BStr)]
    string GetValue();

    [return: MarshalAs(UnmanagedType.Bool)]
    bool GetIsReadOnly();
}

[ComVisible(true)]
[ComImport]
[Guid("56d00bd0-c4f4-433c-a836-1a52a57e0892")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IToggleProvider
{
    void Toggle();

    ToggleState GetToggleState();
}

[ComVisible(true)]
[ComImport]
[Guid("b38b8077-1fc3-42a5-8cae-d40c2215055a")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IScrollProvider
{
    void Scroll(ScrollAmount horizontalAmount, ScrollAmount verticalAmount);

    void SetScrollPercent(double horizontalPercent, double verticalPercent);

    double GetHorizontalScrollPercent();

    double GetVerticalScrollPercent();

    double GetHorizontalViewSize();

    double GetVerticalViewSize();

    [return: MarshalAs(UnmanagedType.Bool)]
    bool GetHorizontallyScrollable();

    [return: MarshalAs(UnmanagedType.Bool)]
    bool GetVerticallyScrollable();
}
