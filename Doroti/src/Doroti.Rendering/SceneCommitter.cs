using System.Diagnostics;
using Doroti.Composition;

namespace Doroti.Rendering;

public sealed class CommittedScene : ICommittedScene, IDisposable
{
    private readonly IReadOnlyList<IResourceLease> _leases;
    private int _completed;

    internal CommittedScene(
        FrameId frameId,
        SurfaceGeneration surfaceGeneration,
        long metricsGeneration,
        Doroti.Graphics.Size expectedPixelSize,
        LayerTreeSnapshot snapshot,
        IResourceLease[] leases,
        TimeSpan commitDuration)
    {
        FrameId = frameId;
        SurfaceGeneration = surfaceGeneration;
        MetricsGeneration = metricsGeneration;
        ExpectedPixelSize = expectedPixelSize;
        Snapshot = snapshot;
        _leases = Array.AsReadOnly(leases);
        Resources = leases.ToDictionary(lease => lease.Resource, lease => lease.Snapshot);
        CommitDuration = commitDuration;
        Ack = new(frameId);
    }

    public FrameId FrameId { get; }

    public SurfaceGeneration SurfaceGeneration { get; }

    public long MetricsGeneration { get; }

    public Doroti.Graphics.Size ExpectedPixelSize { get; }

    public LayerTreeSnapshot Snapshot { get; }

    public IReadOnlyDictionary<ResourceId, IResourceSnapshot> Resources { get; }

    public TimeSpan CommitDuration { get; }

    public FrameAck Ack { get; }

    public bool Complete(FrameAckStatus status, string? diagnostic = null, FrameFaultKind? faultKind = null)
    {
        if (Interlocked.Exchange(ref _completed, 1) != 0)
        {
            return false;
        }

        foreach (var lease in _leases)
        {
            lease.Dispose();
        }
        return Ack.TryComplete(status, diagnostic, faultKind);
    }

    public void Dispose() => Complete(FrameAckStatus.Cancelled, "Committed scene was disposed before presentation.");
}

public sealed class SceneCommitter(IResourceRegistry resourceRegistry) : ISceneBuilder
{
    private readonly object _gate = new();
    private ulong _lastFrameId;
    private bool _shutdown;

    public ICommittedScene Commit(FrameId frameId, SurfaceGeneration surfaceGeneration, IRenderNode root)
    {
        if (root is not Layer layer)
        {
            throw new ArgumentException("R5 commits require an immutable Layer root.", nameof(root));
        }
        return Commit(frameId, surfaceGeneration, layer);
    }

    public CommittedScene Commit(FrameId frameId, SurfaceGeneration surfaceGeneration, Layer root)
        => Commit(frameId, surfaceGeneration, 0, Doroti.Graphics.Size.Zero, root);

    public CommittedScene Commit(
        FrameId frameId,
        SurfaceGeneration surfaceGeneration,
        long metricsGeneration,
        Doroti.Graphics.Size expectedPixelSize,
        Layer root)
    {
        ArgumentNullException.ThrowIfNull(root);
        lock (_gate)
        {
            if (_shutdown)
            {
                throw new InvalidOperationException("Scene commits are blocked during shutdown.");
            }
            if (frameId.Value == 0 || frameId.Value <= _lastFrameId)
            {
                throw new ArgumentOutOfRangeException(nameof(frameId), "Frame identifiers must be positive and strictly increasing.");
            }
            if (surfaceGeneration.Value == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(surfaceGeneration));
            }

            var start = Stopwatch.GetTimestamp();
            var snapshot = LayerTreeSnapshot.Create(root);
            var leases = new List<IResourceLease>();
            try
            {
                foreach (var resource in snapshot.Resources)
                {
                    leases.Add(resourceRegistry.Retain(resource));
                }
                _lastFrameId = frameId.Value;
                return new(
                    frameId,
                    surfaceGeneration,
                    metricsGeneration,
                    expectedPixelSize,
                    snapshot,
                    leases.ToArray(),
                    Stopwatch.GetElapsedTime(start));
            }
            catch
            {
                foreach (var lease in leases)
                {
                    lease.Dispose();
                }
                throw;
            }
        }
    }

    public void Shutdown()
    {
        lock (_gate)
        {
            _shutdown = true;
        }
    }
}
