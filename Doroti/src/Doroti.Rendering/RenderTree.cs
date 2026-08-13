using System.Diagnostics;
using Doroti.Graphics;

namespace Doroti.Rendering;

public enum PipelinePhase
{
    Idle,
    Layout,
    CompositingBits,
    Paint,
    Commit,
}

public sealed class RenderPipelineException : InvalidOperationException
{
    internal RenderPipelineException(PipelinePhase phase, RenderObject node, Exception innerException)
        : base($"Render {phase} failed for {node.DebugName}: {innerException.Message}", innerException)
    {
        Phase = phase;
        Node = node;
    }

    public PipelinePhase Phase { get; }

    public RenderObject Node { get; }
}

public sealed record RenderTraceEvent(long Sequence, PipelinePhase Phase, string Node, int Depth, string Detail);

public abstract class ParentData;

public class BoxParentData : ParentData
{
    private Offset _offset;
    private Matrix _transform = Matrix.Identity;

    public Offset Offset
    {
        get => _offset;
        set
        {
            if (!value.IsFinite)
            {
                throw new ArgumentException("Child offset must be finite.", nameof(value));
            }

            _offset = value;
        }
    }

    public Matrix Transform
    {
        get => _transform;
        set
        {
            if (!value.IsFinite)
            {
                throw new ArgumentException("Child transform must be finite.", nameof(value));
            }

            _transform = value;
        }
    }
}

public abstract class RenderObject
{
    private PipelineOwner? _owner;
    private RenderObject? _parent;
    private RenderObject? _relayoutBoundary;
    private Layer? _paintLayer;

    protected RenderObject()
    {
        DebugName = GetType().Name;
    }

    public string DebugName { get; init; }

    public PipelineOwner? Owner => _owner;

    public RenderObject? Parent => _parent;

    public ParentData? ParentData { get; internal set; }

    public int Depth { get; private set; }

    public bool Attached => _owner is not null;

    public bool NeedsLayout { get; private set; } = true;

    public bool NeedsPaint { get; private set; } = true;

    public bool NeedsCompositingBitsUpdate { get; private set; } = true;

    public virtual bool IsRepaintBoundary => false;

    protected virtual bool SizedByParent => false;

    internal Layer? CachedPaintLayer => _paintLayer;

    internal void Attach(PipelineOwner owner)
    {
        ArgumentNullException.ThrowIfNull(owner);
        owner.VerifyThread();
        if (_owner is not null)
        {
            throw new InvalidOperationException($"{DebugName} is already attached to a pipeline owner.");
        }

        _owner = owner;
        VisitChildren(child => child.Attach(owner));
        if (NeedsLayout)
        {
            owner.EnqueueLayout(this);
        }
        if (NeedsCompositingBitsUpdate)
        {
            owner.EnqueueCompositingBits(this);
        }
        if (NeedsPaint)
        {
            owner.EnqueuePaint(this);
        }
    }

    internal void Detach()
    {
        _owner?.VerifyThread();
        VisitChildren(child => child.Detach());
        _owner?.RemoveFromQueues(this);
        _owner = null;
        _relayoutBoundary = null;
    }

    public void MarkNeedsLayout()
    {
        VerifyMutationAllowed("mark layout dirty");
        if (NeedsLayout)
        {
            return;
        }

        NeedsLayout = true;
        MarkNeedsPaint();
        if (_relayoutBoundary is null || ReferenceEquals(_relayoutBoundary, this))
        {
            _owner?.EnqueueLayout(this);
        }
        else
        {
            _parent?.MarkNeedsLayout();
        }
    }

    public void MarkNeedsPaint()
    {
        VerifyMutationAllowed("mark paint dirty");
        if (NeedsPaint)
        {
            return;
        }

        NeedsPaint = true;
        if (IsRepaintBoundary || _parent is null)
        {
            _owner?.EnqueuePaint(this);
        }
        if (_parent is not null)
        {
            _parent.MarkNeedsPaint();
        }
    }

    public void MarkNeedsCompositingBitsUpdate()
    {
        VerifyMutationAllowed("mark compositing bits dirty");
        if (NeedsCompositingBitsUpdate)
        {
            return;
        }

        NeedsCompositingBitsUpdate = true;
        _owner?.EnqueueCompositingBits(this);
        _parent?.MarkNeedsCompositingBitsUpdate();
    }

    internal void UpdateCompositingBits()
    {
        VisitChildren(child =>
        {
            if (child.NeedsCompositingBitsUpdate)
            {
                child.UpdateCompositingBits();
            }
        });
        NeedsCompositingBitsUpdate = false;
    }

    internal Layer BuildPaintLayer(PipelineOwner owner)
    {
        if (!NeedsPaint && _paintLayer is not null)
        {
            owner.RecordTrace(PipelinePhase.Paint, this, "reuse-layer");
            return _paintLayer;
        }

        var context = new PaintingContext(owner, this);
        Paint(context, Offset.Zero);
        _paintLayer = context.BuildLayer();
        NeedsPaint = false;
        owner.IncrementPaintCount(this);
        owner.RecordTrace(PipelinePhase.Paint, this, "paint");
        return _paintLayer;
    }

    protected void AdoptChild(RenderObject child)
    {
        ArgumentNullException.ThrowIfNull(child);
        VerifyTreeMutationAllowed();
        if (ReferenceEquals(child, this) || child._parent is not null)
        {
            throw new InvalidOperationException("A render child must be unattached and cannot parent itself.");
        }

        for (var ancestor = this; ancestor is not null; ancestor = ancestor._parent)
        {
            if (ReferenceEquals(ancestor, child))
            {
                throw new InvalidOperationException("RenderObject mutation would create a cycle.");
            }
        }

        child._parent = this;
        SetupParentData(child);
        child.SetDepth(Depth + 1);
        if (_owner is not null)
        {
            child.Attach(_owner);
        }
        MarkNeedsLayout();
        MarkNeedsCompositingBitsUpdate();
    }

    protected void DropChild(RenderObject child)
    {
        ArgumentNullException.ThrowIfNull(child);
        VerifyTreeMutationAllowed();
        if (!ReferenceEquals(child._parent, this))
        {
            throw new InvalidOperationException("The node is not a child of this RenderObject.");
        }

        child.Detach();
        child._parent = null;
        child.ParentData = null;
        child.SetDepth(0);
        MarkNeedsLayout();
        MarkNeedsCompositingBitsUpdate();
    }

    protected internal virtual void SetupParentData(RenderObject child)
    {
    }

    protected internal virtual void Paint(PaintingContext context, Offset offset)
    {
    }

    protected internal virtual void VisitChildren(Action<RenderObject> visitor)
    {
    }

    public virtual void DescribeSemanticsConfiguration(SemanticsConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
    }

    internal void VisitChildrenForSemantics(Action<RenderObject> visitor) => VisitChildren(visitor);

    internal void SetRelayoutBoundary(RenderObject boundary) => _relayoutBoundary = boundary;

    internal void ClearNeedsLayout() => NeedsLayout = false;

    internal void RestoreNeedsLayout() => NeedsLayout = true;

    internal void RestoreNeedsPaint() => NeedsPaint = true;

    private void SetDepth(int depth)
    {
        Depth = depth;
        VisitChildren(child => child.SetDepth(depth + 1));
    }

    private void VerifyTreeMutationAllowed()
    {
        _owner?.VerifyThread();
        if (_owner?.Phase is not (null or PipelinePhase.Idle))
        {
            throw new InvalidOperationException($"Render tree mutation is not allowed during {_owner.Phase}.");
        }
    }

    private void VerifyMutationAllowed(string operation)
    {
        _owner?.VerifyThread();
        if (_owner?.Phase is PipelinePhase.Paint or PipelinePhase.CompositingBits or PipelinePhase.Commit)
        {
            throw new InvalidOperationException($"Cannot {operation} during {_owner.Phase}.");
        }
    }
}

public abstract class RenderBox : RenderObject
{
    private Size _size;
    private bool _hasSize;
    private BoxConstraints _constraints;
    private bool _hasConstraints;

    public Size Size => _hasSize
        ? _size
        : throw new InvalidOperationException($"{DebugName} has no size before layout.");

    public BoxConstraints Constraints => _hasConstraints
        ? _constraints
        : throw new InvalidOperationException($"{DebugName} has no constraints before layout.");

    public int LayoutCount { get; private set; }

    public int PaintCount { get; internal set; }

    public void Layout(BoxConstraints constraints, bool parentUsesSize = false)
    {
        Owner?.VerifyThread();
        if (Owner is not null && Owner.Phase is not PipelinePhase.Layout)
        {
            throw new InvalidOperationException($"{DebugName}.Layout can run only during the layout phase.");
        }

        var boundary = !parentUsesSize || Parent is null ? this : Parent;
        SetRelayoutBoundary(boundary);
        if (!NeedsLayout && _hasConstraints && _constraints == constraints)
        {
            return;
        }

        _constraints = constraints;
        _hasConstraints = true;
        try
        {
            if (SizedByParent)
            {
                PerformResize();
            }
            PerformLayout();
            if (!_hasSize)
            {
                throw new InvalidOperationException($"{DebugName}.PerformLayout did not set Size.");
            }
            if (!constraints.IsSatisfiedBy(_size))
            {
                throw new InvalidOperationException($"{DebugName} produced invalid size {_size} for {constraints}.");
            }

            LayoutCount++;
            ClearNeedsLayout();
            Owner?.RecordTrace(PipelinePhase.Layout, this, "layout");
        }
        catch
        {
            RestoreNeedsLayout();
            Owner?.EnqueueLayout(this);
            throw;
        }
    }

    public bool HitTest(HitTestResult result, Offset position)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (!position.IsFinite)
        {
            throw new ArgumentException("Hit-test positions must be finite.", nameof(position));
        }
        if (!_hasSize || !Rect.FromLeftTopWidthHeight(0, 0, _size.Width, _size.Height).Contains(position))
        {
            return false;
        }

        var hit = HitTestChildren(result, position) || HitTestSelf(position);
        if (hit)
        {
            result.Add(new(this, position));
        }
        return hit;
    }

    public Offset LocalToGlobal(Offset point)
    {
        if (!point.IsFinite)
        {
            throw new ArgumentException("Transform input must be finite.", nameof(point));
        }

        var current = this;
        var transformed = point;
        while (current.Parent is RenderBox parent)
        {
            var data = current.ParentData as BoxParentData
                ?? throw new InvalidOperationException($"{current.DebugName} is missing BoxParentData.");
            transformed = data.Transform.Transform(transformed) + data.Offset;
            current = parent;
        }
        return transformed;
    }

    public Offset GlobalToLocal(Offset point)
    {
        if (!point.IsFinite)
        {
            throw new ArgumentException("Transform input must be finite.", nameof(point));
        }

        var chain = new Stack<BoxParentData>();
        for (var current = this; current.Parent is RenderBox; current = (RenderBox)current.Parent)
        {
            chain.Push((BoxParentData)(current.ParentData ?? throw new InvalidOperationException($"{current.DebugName} is missing BoxParentData.")));
        }

        var transformed = point;
        foreach (var data in chain)
        {
            transformed -= data.Offset;
            if (!data.Transform.TryInvert(out var inverse))
            {
                throw new InvalidOperationException("globalToLocal failed because the render transform is singular.");
            }
            transformed = inverse.Transform(transformed);
        }
        return transformed;
    }

    protected void SetSize(Size size)
    {
        if (!size.IsFinite || size.Width < 0 || size.Height < 0)
        {
            throw new InvalidOperationException($"{DebugName} attempted to set a non-finite or negative size: {size}.");
        }
        _size = size;
        _hasSize = true;
    }

    protected virtual void PerformResize()
    {
    }

    protected abstract void PerformLayout();

    protected virtual bool HitTestSelf(Offset position) => false;

    protected virtual bool HitTestChildren(HitTestResult result, Offset position) => false;

    protected bool HitTestChild(HitTestResult result, RenderBox child, Offset position)
    {
        var data = child.ParentData as BoxParentData
            ?? throw new InvalidOperationException($"{child.DebugName} is missing BoxParentData.");
        var local = position - data.Offset;
        if (!data.Transform.TryInvert(out var inverse))
        {
            return false;
        }
        return child.HitTest(result, inverse.Transform(local));
    }

    protected internal override void SetupParentData(RenderObject child)
    {
        child.ParentData ??= new BoxParentData();
        if (child.ParentData is not BoxParentData)
        {
            throw new InvalidOperationException($"{child.DebugName} has incompatible ParentData {child.ParentData.GetType().Name}.");
        }
    }
}

public sealed record HitTestEntry(RenderBox Target, Offset LocalPosition);

public sealed class HitTestResult
{
    private readonly List<HitTestEntry> _path = [];

    public IReadOnlyList<HitTestEntry> Path => _path;

    internal void Add(HitTestEntry entry) => _path.Add(entry);
}

public sealed record RenderPipelineTiming(
    TimeSpan Layout,
    TimeSpan CompositingBits,
    TimeSpan Semantics,
    TimeSpan Paint,
    TimeSpan Commit);

public sealed class PipelineOwner
{
    private readonly int _threadId = Environment.CurrentManagedThreadId;
    private readonly HashSet<RenderObject> _layoutQueue = [];
    private readonly HashSet<RenderObject> _compositingBitsQueue = [];
    private readonly HashSet<RenderObject> _paintQueue = [];
    private readonly List<RenderTraceEvent> _trace = [];
    private readonly SemanticsOwner _semanticsOwner = new();
    private RenderBox? _root;
    private long _sequence;
    private bool _frameScheduled;

    public PipelineOwner(Action? requestVisualUpdate = null)
    {
        RequestVisualUpdate = requestVisualUpdate;
    }

    public Action? RequestVisualUpdate { get; }

    public PipelinePhase Phase { get; private set; }

    public RenderBox? Root => _root;

    public IReadOnlyList<RenderTraceEvent> Trace => _trace.ToArray();

    public SemanticsOwner SemanticsOwner => _semanticsOwner;

    public int DirtyLayoutCount => _layoutQueue.Count;

    public int DirtyPaintCount => _paintQueue.Count;

    public RenderPipelineTiming LastFrameTiming { get; private set; } = new(
        TimeSpan.Zero,
        TimeSpan.Zero,
        TimeSpan.Zero,
        TimeSpan.Zero,
        TimeSpan.Zero);

    public void SetRoot(RenderBox? root)
    {
        VerifyThread();
        if (Phase is not PipelinePhase.Idle)
        {
            throw new InvalidOperationException("The render root can change only while the pipeline is idle.");
        }
        if (ReferenceEquals(_root, root))
        {
            return;
        }
        if (root?.Parent is not null)
        {
            throw new InvalidOperationException("A render root cannot already have a parent.");
        }

        _root?.Detach();
        _root = root;
        _root?.Attach(this);
        ScheduleFrame();
    }

    public RenderPipelineFrame FlushFrame()
    {
        VerifyThread();
        if (_root is null)
        {
            throw new InvalidOperationException("Cannot flush a frame without a render root.");
        }
        if (Phase is not PipelinePhase.Idle)
        {
            throw new InvalidOperationException($"Cannot start a frame during {Phase}.");
        }

        _frameScheduled = false;
        var layoutStart = Stopwatch.GetTimestamp();
        FlushLayout();
        var layout = Stopwatch.GetElapsedTime(layoutStart);
        var compositingStart = Stopwatch.GetTimestamp();
        FlushCompositingBits();
        var compositing = Stopwatch.GetElapsedTime(compositingStart);
        var semanticsStart = Stopwatch.GetTimestamp();
        _semanticsOwner.Build(_root);
        var semantics = Stopwatch.GetElapsedTime(semanticsStart);
        Layer rootLayer;
        Phase = PipelinePhase.Paint;
        var paintStart = Stopwatch.GetTimestamp();
        TimeSpan paint;
        try
        {
            rootLayer = _root.BuildPaintLayer(this);
            if (_root is RenderView { Configuration.DevicePixelRatio: not 1 } view)
            {
                rootLayer = new TransformLayer(
                    Matrix.CreateScale(view.Configuration.DevicePixelRatio, view.Configuration.DevicePixelRatio),
                    rootLayer);
            }
            _paintQueue.Clear();
            paint = Stopwatch.GetElapsedTime(paintStart);
        }
        catch (Exception exception) when (exception is not RenderPipelineException)
        {
            _root.RestoreNeedsPaint();
            EnqueuePaint(_root);
            throw new RenderPipelineException(PipelinePhase.Paint, _root, exception);
        }
        finally
        {
            Phase = PipelinePhase.Idle;
        }

        Phase = PipelinePhase.Commit;
        var commitStart = Stopwatch.GetTimestamp();
        try
        {
            var snapshot = LayerTreeSnapshot.Create(rootLayer);
            var commit = Stopwatch.GetElapsedTime(commitStart);
            LastFrameTiming = new(layout, compositing, semantics, paint, commit);
            RecordTrace(PipelinePhase.Commit, _root, "commit");
            var configuration = _root is RenderView view
                ? view.Configuration
                : throw new InvalidOperationException("The render root must remain a RenderView through commit.");
            return new(rootLayer, snapshot, ++_sequence, configuration);
        }
        finally
        {
            Phase = PipelinePhase.Idle;
            if (_layoutQueue.Count > 0 || _paintQueue.Count > 0 || _compositingBitsQueue.Count > 0)
            {
                ScheduleFrame();
            }
        }
    }

    internal void EnqueueLayout(RenderObject node)
    {
        if (node.Attached && _layoutQueue.Add(node))
        {
            ScheduleFrame();
        }
    }

    internal void EnqueuePaint(RenderObject node)
    {
        if (node.Attached && _paintQueue.Add(node))
        {
            ScheduleFrame();
        }
    }

    internal void EnqueueCompositingBits(RenderObject node)
    {
        if (node.Attached && _compositingBitsQueue.Add(node))
        {
            ScheduleFrame();
        }
    }

    internal void RemoveFromQueues(RenderObject node)
    {
        _layoutQueue.Remove(node);
        _paintQueue.Remove(node);
        _compositingBitsQueue.Remove(node);
    }

    internal void VerifyThread()
    {
        if (Environment.CurrentManagedThreadId != _threadId)
        {
            throw new InvalidOperationException("RenderObject trees are owned by their creating UI thread.");
        }
    }

    internal void RecordTrace(PipelinePhase phase, RenderObject node, string detail) =>
        _trace.Add(new(_trace.Count + 1, phase, node.DebugName, node.Depth, detail));

    internal void IncrementPaintCount(RenderObject node)
    {
        if (node is RenderBox box)
        {
            box.PaintCount++;
        }
    }

    private void FlushLayout()
    {
        Phase = PipelinePhase.Layout;
        try
        {
            while (_layoutQueue.Count > 0)
            {
                var node = _layoutQueue.OrderBy(item => item.Depth).First();
                _layoutQueue.Remove(node);
                if (!node.Attached || !node.NeedsLayout)
                {
                    continue;
                }
                try
                {
                    if (node is RenderView view)
                    {
                        view.Layout(BoxConstraints.Tight(view.Configuration.LogicalSize));
                    }
                    else if (node is RenderBox box)
                    {
                        box.Layout(box.Constraints);
                    }
                }
                catch (Exception exception) when (exception is not RenderPipelineException)
                {
                    throw new RenderPipelineException(PipelinePhase.Layout, node, exception);
                }
            }
        }
        finally
        {
            Phase = PipelinePhase.Idle;
        }
    }

    private void FlushCompositingBits()
    {
        Phase = PipelinePhase.CompositingBits;
        try
        {
            while (_compositingBitsQueue.Count > 0)
            {
                var node = _compositingBitsQueue.OrderBy(item => item.Depth).First();
                _compositingBitsQueue.Remove(node);
                if (node.Attached && node.NeedsCompositingBitsUpdate)
                {
                    node.UpdateCompositingBits();
                    RecordTrace(PipelinePhase.CompositingBits, node, "update");
                }
            }
        }
        finally
        {
            Phase = PipelinePhase.Idle;
        }
    }

    private void ScheduleFrame()
    {
        if (_frameScheduled)
        {
            return;
        }
        _frameScheduled = true;
        RequestVisualUpdate?.Invoke();
    }
}
