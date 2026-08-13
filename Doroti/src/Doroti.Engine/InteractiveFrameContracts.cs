using Doroti.Composition;
using Doroti.Graphics;
using Doroti.Rendering;

namespace Doroti.Engine;

public interface IInteractiveFrameSink
{
    FrameAckStatus Present(FrameId frameId, RenderPipelineFrame frame);
}

/// <summary>
/// Product frame sinks implement non-blocking commit submission and expose the terminal ACK as a task.
/// The synchronous interface remains a compatibility boundary for deterministic unit fixtures only.
/// </summary>
public interface IAsyncInteractiveFrameSink : IInteractiveFrameSink
{
    ValueTask<FrameAckResult> PresentAsync(
        FrameId frameId,
        RenderPipelineFrame frame,
        CancellationToken cancellationToken = default);
}

public interface IInteractiveImageHost
{
    ImageCache Images { get; }
}
