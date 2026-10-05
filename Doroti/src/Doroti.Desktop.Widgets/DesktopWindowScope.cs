using Doroti.Framework;
using Doroti.Framework.Widgets;

namespace Doroti.Desktop.Widgets;

/// <summary>The explicit owning window of this tree; never guesses from foreground focus.</summary>
public sealed class DesktopWindowScope(DesktopWindowContext value, Widget child)
    : InheritedWidget(child)
{
    public DesktopWindowContext Value { get; } = value;

    public static DesktopWindowContext Of(BuildContext context) =>
        context.dependOnInheritedWidgetOfExactType<DesktopWindowScope>()?.Value
        ?? throw new InvalidOperationException(
            "This widget tree has no owning DesktopWindowScope."
        );

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        !ReferenceEquals(Value, ((DesktopWindowScope)oldWidget).Value);
}
