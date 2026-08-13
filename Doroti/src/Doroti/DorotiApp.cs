using Doroti.Engine;
using Doroti.Platform;
using Doroti.Widgets;

namespace Doroti;

public static class DorotiApp
{
    public static InteractiveApplication CreateInteractive(
        IWindow window,
        Widget root,
        IInteractiveFrameSink frameSink,
        InteractiveTraceRecorder? trace = null)
    {
        ArgumentNullException.ThrowIfNull(root);
        return new(window, root, frameSink, trace);
    }

    public static IEngineHost Create(
        IEngineFactory engineFactory,
        WindowConfiguration window,
        IWidget root)
    {
        ArgumentNullException.ThrowIfNull(engineFactory);
        ArgumentNullException.ThrowIfNull(root);
        return engineFactory.Create(window, root);
    }
}
