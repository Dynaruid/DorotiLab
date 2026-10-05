using Doroti.Ui;
namespace Doroti.Framework.Widgets;
/// <summary>Explicit owning window identity inside one view branch.</summary>
public sealed class WindowScope(WindowSnapshot window, Widget child) : InheritedWidget(child: child)
{
    public WindowSnapshot Window { get; } = window;
    public static WindowSnapshot? maybeOf(BuildContext context) => context.dependOnInheritedWidgetOfExactType<WindowScope>()?.Window;
    public static WindowSnapshot of(BuildContext context) => maybeOf(context) ?? throw new InvalidOperationException("This branch has no WindowScope.");
    public override bool updateShouldNotify(InheritedWidget previous) => Window != ((WindowScope)previous).Window;
}
/// <summary>Attaches UI to an already registered native view; it never creates a framework root.</summary>
public class Window(DorotiView view, WindowSnapshot window, Widget child) : StatelessWidget
{
    protected virtual WindowKind Kind => WindowKind.Regular;
    public override Widget build(BuildContext context)
    {
        if (window.Kind != Kind || window.ViewId != view.viewId || window.Closed) throw new InvalidOperationException("The window branch does not match a live registered view.");
        return new View(view: view, child: new WindowScope(window, child));
    }
}
public sealed class DialogWindow(DorotiView view, WindowSnapshot window, Widget child) : Window(view, window, child) { protected override WindowKind Kind => WindowKind.Dialog; }
public sealed class PopupWindow(DorotiView view, WindowSnapshot window, Widget child) : Window(view, window, child) { protected override WindowKind Kind => WindowKind.Popup; }
public sealed class TooltipWindow(DorotiView view, WindowSnapshot window, Widget child) : Window(view, window, child) { protected override WindowKind Kind => WindowKind.Tooltip; }
public sealed class SatelliteWindow(DorotiView view, WindowSnapshot window, Widget child) : Window(view, window, child) { protected override WindowKind Kind => WindowKind.Satellite; }
