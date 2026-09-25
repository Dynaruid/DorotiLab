using System.Runtime.CompilerServices;
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

public static class WidgetWindowContent
{
    private static readonly ConditionalWeakTable<Widget, object> MountedRoots = new();

    public static WindowContent Create(Func<Widget> factory) => Create(_ => factory());

    public static WindowContent Create(Func<DesktopWindowContext, Widget> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        return WindowContent.FromEntrypoint(context => new DorotiWidgetEntrypoint(() =>
        {
            var root =
                factory(context)
                ?? throw new InvalidOperationException("Widget factory returned null.");
            lock (MountedRoots)
            {
                if (MountedRoots.TryGetValue(root, out _))
                    throw new InvalidOperationException(
                        "Each window must create a new widget root."
                    );
                MountedRoots.Add(root, new());
            }
            return new DesktopWindowScope(context, root);
        }));
    }
}
