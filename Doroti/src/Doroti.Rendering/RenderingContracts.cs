using Doroti.Composition;
using Doroti.Graphics;

namespace Doroti.Rendering;

public interface IRenderNode
{
    Rect Bounds { get; }
}

public interface ISceneBuilder
{
    ICommittedScene Commit(FrameId frameId, SurfaceGeneration surfaceGeneration, IRenderNode root);
}

/// <summary>Engine-owned ordering port used by compatibility layers without reproducing a Dart event loop.</summary>
public interface IFrameLifecyclePort
{
    SchedulerPhase Phase { get; }

    int ScheduleFrameCallback(Action<TimeSpan> callback);

    void CancelFrameCallback(int id);

    void AddPersistentFrameCallback(Action<TimeSpan> callback);

    void AddPostFrameCallback(Action<TimeSpan> callback);

    void ScheduleMicrotask(Action callback);

    ValueTask<T> ScheduleTask<T>(Func<T> callback, int priority);

    void MarkBuildDirty(Action callback);

    void MarkLayoutDirty(Action callback);

    void MarkPaintDirty(Action callback);
}

public enum SchedulerPhase
{
    Idle,
    TransientCallbacks,
    MidFrameMicrotasks,
    PersistentCallbacks,
    PostFrameCallbacks,
}
