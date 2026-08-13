using Doroti.Composition;
using Doroti.Platform;
using Doroti.Widgets;

namespace Doroti.Engine;

public interface IEngineHost : IDisposable
{
    IWindow Window { get; }

    IFrameScheduler FrameScheduler { get; }

    IWidgetHost Widgets { get; }

    void Run();
}

public interface IEngineFactory
{
    IEngineHost Create(WindowConfiguration window, IWidget root);
}
