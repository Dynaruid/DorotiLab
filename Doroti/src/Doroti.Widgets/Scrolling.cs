using Doroti.Core;
using Doroti.Graphics;
using Doroti.Platform;
using Doroti.Rendering;

namespace Doroti.Widgets;

public readonly record struct ScrollMetrics(
    double Pixels,
    double MinScrollExtent,
    double MaxScrollExtent,
    double ViewportDimension,
    double ContentExtent)
{
    public bool AtStart => Pixels <= MinScrollExtent;

    public bool AtEnd => Pixels >= MaxScrollExtent;
}

public abstract class ScrollPhysics
{
    public virtual bool ShouldAcceptUserOffset(ScrollMetrics metrics) =>
        metrics.MaxScrollExtent > metrics.MinScrollExtent;

    public virtual double ApplyPhysicsToUserOffset(ScrollMetrics metrics, double offset) => offset;

    /// <summary>Returns the part of <paramref name="proposedPixels"/> that must be rejected.</summary>
    public abstract double ApplyBoundaryConditions(ScrollMetrics metrics, double proposedPixels);

    public virtual ScrollSimulation? CreateBallisticSimulation(ScrollMetrics metrics, double velocity) =>
        Math.Abs(velocity) < Tolerance.Velocity
            ? null
            : new ClampingScrollSimulation(metrics.Pixels, velocity, metrics.MinScrollExtent, metrics.MaxScrollExtent);

    internal static class Tolerance
    {
        internal const double Distance = 0.000000001;
        internal const double Velocity = 5;
    }
}

public sealed class ClampingScrollPhysics : ScrollPhysics
{
    public static ClampingScrollPhysics Instance { get; } = new();

    private ClampingScrollPhysics()
    {
    }

    public override double ApplyBoundaryConditions(ScrollMetrics metrics, double proposedPixels)
    {
        if (proposedPixels < metrics.MinScrollExtent)
        {
            return proposedPixels - metrics.MinScrollExtent;
        }
        if (proposedPixels > metrics.MaxScrollExtent)
        {
            return proposedPixels - metrics.MaxScrollExtent;
        }
        return 0;
    }
}

public abstract class ScrollSimulation
{
    public abstract double PositionAt(TimeSpan elapsed);

    public abstract double VelocityAt(TimeSpan elapsed);

    public abstract bool IsDone(TimeSpan elapsed);
}

/// <summary>Deterministic friction simulation adapted from Flutter's clamping scroll simulation.</summary>
public sealed class ClampingScrollSimulation : ScrollSimulation
{
    private readonly FlutterClampingScrollSimulation _simulation;
    private readonly double _min;
    private readonly double _max;

    public ClampingScrollSimulation(double position, double velocity, double min, double max)
    {
        if (!double.IsFinite(position) || !double.IsFinite(velocity) || !double.IsFinite(min) || !double.IsFinite(max) || min > max)
        {
            throw new ArgumentOutOfRangeException(nameof(position));
        }
        _simulation = new(position, velocity);
        _min = min;
        _max = max;
    }

    public override double PositionAt(TimeSpan elapsed)
    {
        return Math.Clamp(_simulation.X(Math.Max(0, elapsed.TotalSeconds)), _min, _max);
    }

    public override double VelocityAt(TimeSpan elapsed) =>
        _simulation.Dx(Math.Max(0, elapsed.TotalSeconds));

    public override bool IsDone(TimeSpan elapsed)
    {
        var position = PositionAt(elapsed);
        return _simulation.IsDone(Math.Max(0, elapsed.TotalSeconds)) ||
            position <= _min + ScrollPhysics.Tolerance.Distance ||
            position >= _max - ScrollPhysics.Tolerance.Distance;
    }
}

public abstract class ScrollActivity
{
    public abstract string Kind { get; }

    public virtual bool IsScrolling => true;

    public virtual double Velocity => 0;
}

public sealed class IdleScrollActivity : ScrollActivity
{
    public static IdleScrollActivity Instance { get; } = new();

    private IdleScrollActivity()
    {
    }

    public override string Kind => "idle";

    public override bool IsScrolling => false;
}

public sealed class DragScrollActivity(double velocity) : ScrollActivity
{
    public override string Kind => "drag";

    public override double Velocity { get; } = velocity;
}

public sealed class BallisticScrollActivity(ScrollSimulation simulation, TimeSpan startedAt) : ScrollActivity
{
    public override string Kind => "ballistic";

    public ScrollSimulation Simulation { get; } = simulation ?? throw new ArgumentNullException(nameof(simulation));

    public TimeSpan StartedAt { get; } = startedAt;

    public override double Velocity => Simulation.VelocityAt(TimeSpan.Zero);
}

public sealed class ScrollPosition : ChangeNotifier
{
    private IFrameDispatcher? _frameDispatcher;
    private double _pixels;
    private double _minScrollExtent;
    private double _maxScrollExtent;
    private double _viewportDimension;
    private double _contentExtent;
    private bool _ballisticFrameScheduled;
    private TimeSpan? _ballisticClockStart;

    public ScrollPosition(double initialPixels, ScrollPhysics physics, IFrameDispatcher? frameDispatcher = null)
    {
        if (!double.IsFinite(initialPixels) || initialPixels < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(initialPixels));
        }
        _pixels = initialPixels;
        Physics = physics ?? throw new ArgumentNullException(nameof(physics));
        _frameDispatcher = frameDispatcher;
        Activity = IdleScrollActivity.Instance;
    }

    public ScrollPhysics Physics { get; }

    public ScrollActivity Activity { get; private set; }

    public ScrollMetrics Metrics => new(_pixels, _minScrollExtent, _maxScrollExtent, _viewportDimension, _contentExtent);

    public double Pixels => _pixels;

    public bool CanScroll(double delta)
    {
        if (!double.IsFinite(delta) || delta == 0 || !Physics.ShouldAcceptUserOffset(Metrics))
        {
            return false;
        }
        var target = Math.Clamp(_pixels + delta, _minScrollExtent, _maxScrollExtent);
        return Math.Abs(target - _pixels) >= ScrollPhysics.Tolerance.Distance;
    }

    public void JumpTo(double pixels)
    {
        GoIdle();
        SetPixels(pixels);
    }

    public void PointerScroll(double delta, TimeSpan timestamp)
    {
        if (!double.IsFinite(delta))
        {
            throw new ArgumentOutOfRangeException(nameof(delta));
        }
        if (delta == 0)
        {
            GoBallistic(0, timestamp);
            return;
        }
        GoIdle();
        SetPixels(_pixels + delta);
    }

    public void ApplyUserOffset(double delta, TimeSpan timestamp)
    {
        if (!double.IsFinite(delta))
        {
            throw new ArgumentOutOfRangeException(nameof(delta));
        }
        Activity = new DragScrollActivity(delta);
        SetPixels(_pixels + Physics.ApplyPhysicsToUserOffset(Metrics, delta));
    }

    public void GoBallistic(double velocity, TimeSpan timestamp)
    {
        if (!double.IsFinite(velocity))
        {
            throw new ArgumentOutOfRangeException(nameof(velocity));
        }
        var simulation = Physics.CreateBallisticSimulation(Metrics, velocity);
        if (simulation is null)
        {
            GoIdle();
            return;
        }
        Activity = new BallisticScrollActivity(simulation, timestamp);
        _ballisticClockStart = _frameDispatcher is null ? timestamp : null;
        ScheduleBallisticFrame();
    }

    public void EnsureVisible(double leading, double trailing)
    {
        if (!double.IsFinite(leading) || !double.IsFinite(trailing) || leading > trailing)
        {
            throw new ArgumentOutOfRangeException(nameof(leading));
        }
        if (leading < _pixels)
        {
            JumpTo(leading);
        }
        else if (trailing > _pixels + _viewportDimension)
        {
            JumpTo(trailing - _viewportDimension);
        }
    }

    public void AttachFrameDispatcher(IFrameDispatcher dispatcher)
    {
        ArgumentNullException.ThrowIfNull(dispatcher);
        if (_frameDispatcher is not null && !ReferenceEquals(_frameDispatcher, dispatcher))
        {
            throw new InvalidOperationException("A ScrollPosition cannot be attached to two frame dispatchers.");
        }
        _frameDispatcher = dispatcher;
        if (Activity is BallisticScrollActivity)
        {
            _ballisticClockStart = null;
        }
        ScheduleBallisticFrame();
    }

    public bool AdvanceBallistic(TimeSpan timestamp)
    {
        if (Activity is not BallisticScrollActivity ballistic)
        {
            return false;
        }
        _ballisticClockStart ??= timestamp;
        var elapsed = timestamp - _ballisticClockStart.Value;
        SetPixels(ballistic.Simulation.PositionAt(elapsed));
        if (ballistic.Simulation.IsDone(elapsed))
        {
            GoIdle();
            return false;
        }
        return true;
    }

    public void ApplyContentDimensions(double viewportDimension, double contentExtent)
    {
        if (!double.IsFinite(viewportDimension) || viewportDimension < 0 || !double.IsFinite(contentExtent) || contentExtent < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(viewportDimension));
        }
        var dimensionsChanged = Math.Abs(_viewportDimension - viewportDimension) >= ScrollPhysics.Tolerance.Distance ||
            Math.Abs(_contentExtent - contentExtent) >= ScrollPhysics.Tolerance.Distance;
        _viewportDimension = viewportDimension;
        _contentExtent = contentExtent;
        _minScrollExtent = 0;
        _maxScrollExtent = Math.Max(0, contentExtent - viewportDimension);
        var changed = SetPixels(Math.Clamp(_pixels, _minScrollExtent, _maxScrollExtent));
        if (dimensionsChanged && !changed)
        {
            NotifyListeners();
        }
    }

    private bool SetPixels(double pixels)
    {
        if (!double.IsFinite(pixels))
        {
            throw new ArgumentOutOfRangeException(nameof(pixels));
        }
        var overscroll = Physics.ApplyBoundaryConditions(Metrics, pixels);
        var accepted = pixels - overscroll;
        if (Math.Abs(accepted - _pixels) < ScrollPhysics.Tolerance.Distance)
        {
            return false;
        }
        _pixels = accepted;
        NotifyListeners();
        return true;
    }

    private void GoIdle()
    {
        Activity = IdleScrollActivity.Instance;
        _ballisticFrameScheduled = false;
        _ballisticClockStart = null;
    }

    private void ScheduleBallisticFrame()
    {
        if (_frameDispatcher is null || _ballisticFrameScheduled || Activity is not BallisticScrollActivity)
        {
            return;
        }
        _ballisticFrameScheduled = true;
        var scheduledActivity = Activity;
        _frameDispatcher.ScheduleFrame(timestamp =>
        {
            if (!ReferenceEquals(Activity, scheduledActivity))
            {
                return;
            }
            _ballisticFrameScheduled = false;
            if (AdvanceBallistic(timestamp))
            {
                ScheduleBallisticFrame();
            }
        });
    }
}

public sealed class SliverScrollController : ChangeNotifier
{
    public SliverScrollController(
        double initialScrollOffset = 0,
        ScrollPhysics? physics = null,
        IFrameDispatcher? frameDispatcher = null)
    {
        Position = new(initialScrollOffset, physics ?? ClampingScrollPhysics.Instance, frameDispatcher);
        Position.AddListener(NotifyListeners);
    }

    public ScrollPosition Position { get; }

    public ScrollPhysics Physics => Position.Physics;

    public ScrollMetrics Metrics => Position.Metrics;

    public double Offset => Position.Pixels;

    public void JumpTo(double pixels)
    {
        if (!double.IsFinite(pixels))
        {
            throw new ArgumentOutOfRangeException(nameof(pixels));
        }
        Position.JumpTo(pixels);
    }

    public void ScrollBy(double delta) => Position.PointerScroll(delta, TimeSpan.Zero);

    public void PointerScroll(double delta, TimeSpan timestamp) => Position.PointerScroll(delta, timestamp);

    public void GoBallistic(double velocity, TimeSpan timestamp) => Position.GoBallistic(velocity, timestamp);

    public void EnsureVisible(double leading, double trailing) => Position.EnsureVisible(leading, trailing);

    internal void ApplyContentDimensions(double viewportDimension, double contentExtent)
    {
        Position.ApplyContentDimensions(viewportDimension, contentExtent);
    }

    public override void Dispose()
    {
        Position.RemoveListener(NotifyListeners);
        Position.Dispose();
        base.Dispose();
    }
}

public delegate Widget IndexedWidgetBuilder(BuildContext context, int index);

public sealed class VirtualListView : StatefulWidget
{
    public VirtualListView(
        int itemCount,
        double itemExtent,
        IndexedWidgetBuilder itemBuilder,
        SliverScrollController? controller = null,
        double cacheExtent = 0,
        double? viewportExtentHint = null,
        Key? key = null)
        : base(key)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(itemCount);
        ValidatePositiveFinite(itemExtent, nameof(itemExtent));
        if (viewportExtentHint is { } hint)
        {
            ValidatePositiveFinite(hint, nameof(viewportExtentHint));
        }
        if (!double.IsFinite(cacheExtent) || cacheExtent < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cacheExtent));
        }
        ItemCount = itemCount;
        ItemExtent = itemExtent;
        ViewportExtentHint = viewportExtentHint;
        ItemBuilder = itemBuilder ?? throw new ArgumentNullException(nameof(itemBuilder));
        Controller = controller;
        CacheExtent = cacheExtent;
    }

    public VirtualListView(
        int itemCount,
        double itemExtent,
        double viewportExtent,
        IndexedWidgetBuilder itemBuilder,
        SliverScrollController? controller = null,
        double cacheExtent = 0,
        Key? key = null)
        : this(itemCount, itemExtent, itemBuilder, controller, cacheExtent, viewportExtent, key)
    {
    }

    public int ItemCount { get; }

    public double ItemExtent { get; }

    public double? ViewportExtentHint { get; }

    public IndexedWidgetBuilder ItemBuilder { get; }

    public SliverScrollController? Controller { get; }

    public double CacheExtent { get; }

    public override State CreateState() => new VirtualListViewState();

    private static void ValidatePositiveFinite(double value, string name)
    {
        if (!double.IsFinite(value) || value <= 0)
        {
            throw new ArgumentOutOfRangeException(name);
        }
    }
}

public sealed class VirtualListViewState : State<VirtualListView>
{
    private SliverScrollController? _ownedController;
    private int _firstVisible;
    private int _lastVisible = -1;

    public SliverScrollController Controller => Widget.Controller ?? _ownedController!;

    protected internal override void InitState()
    {
        _ownedController = Widget.Controller is null ? new() : null;
        Controller.AddListener(HandleScroll);
    }

    protected internal override void DidUpdateWidget(VirtualListView oldWidget)
    {
        if (!ReferenceEquals(oldWidget.Controller, Widget.Controller))
        {
            (oldWidget.Controller ?? _ownedController!).RemoveListener(HandleScroll);
            _ownedController?.Dispose();
            _ownedController = Widget.Controller is null ? new() : null;
            Controller.AddListener(HandleScroll);
        }
    }

    protected internal override void Dispose()
    {
        Controller.RemoveListener(HandleScroll);
        _ownedController?.Dispose();
    }

    public override Widget Build(BuildContext context)
    {
        var viewportExtent = Controller.Metrics.ViewportDimension > 0
            ? Controller.Metrics.ViewportDimension
            : Widget.ViewportExtentHint ?? Widget.ItemExtent;
        var startPixels = Math.Max(0, Controller.Offset - Widget.CacheExtent);
        var endPixels = Controller.Offset + viewportExtent + Widget.CacheExtent;
        var first = Widget.ItemCount == 0 ? 0 : Math.Clamp((int)Math.Floor(startPixels / Widget.ItemExtent), 0, Widget.ItemCount - 1);
        var last = Widget.ItemCount == 0 ? -1 : Math.Clamp((int)Math.Ceiling(endPixels / Widget.ItemExtent) - 1, first, Widget.ItemCount - 1);
        _firstVisible = first;
        _lastVisible = last;
        var children = new List<Widget>(Math.Max(0, last - first + 1));
        for (var index = first; index <= last; index++)
        {
            children.Add(new IndexedListItem(index, Widget.ItemExtent, Widget.ItemBuilder(context, index), new ValueKey<int>(index)));
        }
        return new SliverFixedExtentViewport(
            children,
            Widget.ItemCount,
            Widget.ItemExtent,
            Controller);
    }

    private void HandleScroll()
    {
        if (!Mounted)
        {
            return;
        }
        var viewportExtent = Controller.Metrics.ViewportDimension > 0
            ? Controller.Metrics.ViewportDimension
            : Widget.ViewportExtentHint ?? Widget.ItemExtent;
        var startPixels = Math.Max(0, Controller.Offset - Widget.CacheExtent);
        var endPixels = Controller.Offset + viewportExtent + Widget.CacheExtent;
        var first = Widget.ItemCount == 0 ? 0 : Math.Clamp((int)Math.Floor(startPixels / Widget.ItemExtent), 0, Widget.ItemCount - 1);
        var last = Widget.ItemCount == 0 ? -1 : Math.Clamp((int)Math.Ceiling(endPixels / Widget.ItemExtent) - 1, first, Widget.ItemCount - 1);
        if (first != _firstVisible || last != _lastVisible)
        {
            SetState(static () => { });
        }
    }
}

public sealed class IndexedListItem(int index, double itemExtent, Widget child, Key? key = null) : SingleChildRenderObjectWidget(child, key)
{
    public int Index { get; } = index;

    public double ItemExtent { get; } = itemExtent;

    public override RenderObject CreateRenderObject(BuildContext context) => new RenderIndexedListItem(Index, ItemExtent);

    public override void UpdateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var item = (RenderIndexedListItem)renderObject;
        item.Index = Index;
        item.ItemExtent = ItemExtent;
    }
}

public sealed class RenderIndexedListItem : RenderProxyBox
{
    private int _index;
    private double _itemExtent;

    public RenderIndexedListItem(int index, double itemExtent)
    {
        _index = index;
        _itemExtent = itemExtent;
    }

    public int Index
    {
        get => _index;
        set
        {
            if (_index != value)
            {
                _index = value;
                MarkNeedsLayout();
            }
        }
    }

    public int SemanticIndex => _index;

    public override void DescribeSemanticsConfiguration(SemanticsConfiguration configuration)
    {
        configuration.Role = SemanticsRole.ListItem;
        configuration.IndexInParent = _index;
    }

    public double ItemExtent
    {
        get => _itemExtent;
        set
        {
            if (_itemExtent != value)
            {
                _itemExtent = value;
                MarkNeedsLayout();
            }
        }
    }

    protected override void PerformLayout()
    {
        var width = Constraints.HasBoundedWidth ? Constraints.MaxWidth : 0;
        Child?.Layout(BoxConstraints.TightFor(width: width, height: _itemExtent), parentUsesSize: true);
        SetSize(Constraints.Constrain(new(width, _itemExtent)));
        if (Child is not null)
        {
            ((BoxParentData)Child.ParentData!).Offset = Offset.Zero;
        }
    }
}

public sealed class SliverFixedExtentViewport : MultiChildRenderObjectWidget
{
    public SliverFixedExtentViewport(
        IEnumerable<Widget> children,
        int itemCount,
        double itemExtent,
        SliverScrollController controller,
        Key? key = null)
        : base(children, key)
    {
        ItemCount = itemCount;
        ItemExtent = itemExtent;
        Controller = controller;
    }

    public int ItemCount { get; }

    public double ItemExtent { get; }

    public SliverScrollController Controller { get; }

    public override RenderObject CreateRenderObject(BuildContext context) =>
        new RenderSliverFixedExtentViewport(ItemCount, ItemExtent, Controller);

    public override void UpdateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var viewport = (RenderSliverFixedExtentViewport)renderObject;
        viewport.ItemCount = ItemCount;
        viewport.ItemExtent = ItemExtent;
        viewport.Controller = Controller;
    }
}

public sealed class RenderSliverFixedExtentViewport : ContainerRenderBox<BoxParentData>, IFlutterArenaTarget, IPointerSignalTarget, IFrameDrivenScrollTarget, IFocusableKeyboardTarget, IDisposable
{
    private int _itemCount;
    private double _itemExtent;
    private SliverScrollController _controller;
    private FlutterArenaAdapter? _arena;
    private Doroti.Generated.Framework.Gestures.VerticalDragGestureRecognizer? _drag;
    private readonly Dictionary<ulong, Offset> _dragLastPositions = [];
    private TimeSpan _dragTimestamp;

    public RenderSliverFixedExtentViewport(int itemCount, double itemExtent, SliverScrollController controller)
    {
        _itemCount = itemCount;
        _itemExtent = itemExtent;
        _controller = controller;
        _controller.AddListener(HandleControllerChanged);
    }

    public int ItemCount
    {
        get => _itemCount;
        set
        {
            if (_itemCount != value)
            {
                _itemCount = value;
                MarkNeedsLayout();
            }
        }
    }

    public double ItemExtent
    {
        get => _itemExtent;
        set
        {
            if (_itemExtent != value)
            {
                _itemExtent = value;
                MarkNeedsLayout();
            }
        }
    }

    public SliverScrollController Controller
    {
        get => _controller;
        set
        {
            if (ReferenceEquals(_controller, value))
            {
                return;
            }
            _controller.RemoveListener(HandleControllerChanged);
            _controller = value;
            _controller.AddListener(HandleControllerChanged);
            MarkNeedsLayout();
        }
    }

    public void BindFlutterArena(FlutterArenaAdapter arena, Action<string>? trace = null)
    {
        _arena = arena;
        _drag ??= FlutterInputAdapter.CreateVerticalDrag(
            details =>
            {
                var delta = details.primaryDelta ?? details.delta.dy;
                var timestamp = details.sourceTimeStamp is { } source ? (TimeSpan)source : _dragTimestamp;
                _controller.Position.ApplyUserOffset(-delta, timestamp);
            },
            details =>
            {
                var velocity = details.primaryVelocity ?? details.velocity.pixelsPerSecond.dy;
                _controller.GoBallistic(-velocity, _dragTimestamp);
            },
            trace: trace,
            touchSlop: Doroti.Generated.Framework.Gestures.ConstantsLibrary.kTouchSlop);
    }

    public void BindScrollFrameDispatcher(IFrameDispatcher dispatcher) =>
        _controller.Position.AttachFrameDispatcher(dispatcher);

    public void HandlePointerEvent(PointerEvent input)
    {
        if (_arena is null)
        {
            return;
        }
        _drag ??= FlutterInputAdapter.CreateVerticalDrag(
            details =>
            {
                var delta = details.primaryDelta ?? details.delta.dy;
                var timestamp = details.sourceTimeStamp is { } source ? (TimeSpan)source : _dragTimestamp;
                _controller.Position.ApplyUserOffset(-delta, timestamp);
            },
            details =>
            {
                var velocity = details.primaryVelocity ?? details.velocity.pixelsPerSecond.dy;
                _controller.GoBallistic(-velocity, _dragTimestamp);
            },
            trace: null,
            touchSlop: Doroti.Generated.Framework.Gestures.ConstantsLibrary.kTouchSlop);
        _dragTimestamp = input.Timestamp;
        var previous = _dragLastPositions.GetValueOrDefault(input.DeviceId, input.LocalPosition);
        var delta = new Doroti.Flutter.Ui.Offset(input.LocalPosition.X - previous.X, input.LocalPosition.Y - previous.Y);
        _dragLastPositions[input.DeviceId] = input.LocalPosition;
        if (input.Phase is PointerEventPhase.Down)
        {
            FlutterInputAdapter.AddPointer(_drag, input, _arena);
        }
        _drag.handleEvent(FlutterPointerEventAdapter.Convert(input, delta));
        if (input.Phase is PointerEventPhase.Up or PointerEventPhase.Cancelled or PointerEventPhase.Removed)
        {
            _dragLastPositions.Remove(input.DeviceId);
        }
    }

    public void RegisterPointerSignal(PointerScrollEvent input, PointerSignalResolver resolver)
    {
        if (_controller.Position.CanScroll(input.ScrollDelta.Y))
        {
            resolver.Register(input, resolved => _controller.PointerScroll(resolved.ScrollDelta.Y, resolved.Timestamp));
        }
    }

    public bool RequestFocus() => true;

    public bool HandleKeyboardEvent(KeyboardEvent input)
    {
        if (input.Phase is not (KeyboardEventPhase.Down or KeyboardEventPhase.Repeat))
        {
            return false;
        }
        switch (input.LogicalKey)
        {
            case 0x21: // Page Up
                _controller.PointerScroll(-Size.Height, input.Timestamp);
                return true;
            case 0x22: // Page Down
                _controller.PointerScroll(Size.Height, input.Timestamp);
                return true;
            case 0x23: // End
                _controller.JumpTo(_controller.Metrics.MaxScrollExtent);
                return true;
            case 0x24: // Home
                _controller.JumpTo(_controller.Metrics.MinScrollExtent);
                return true;
            case 0x26: // Arrow Up
                _controller.PointerScroll(-_itemExtent, input.Timestamp);
                return true;
            case 0x28: // Arrow Down
                _controller.PointerScroll(_itemExtent, input.Timestamp);
                return true;
            default:
                return false;
        }
    }

    public void RevealItem(int index)
    {
        if (index < 0 || index >= _itemCount)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        _controller.EnsureVisible(index * _itemExtent, (index + 1) * _itemExtent);
    }

    public void Dispose()
    {
        _controller.RemoveListener(HandleControllerChanged);
        _drag?.dispose();
        _dragLastPositions.Clear();
    }

    public override void DescribeSemanticsConfiguration(SemanticsConfiguration configuration)
    {
        configuration.Role = SemanticsRole.List;
        configuration.State = SemanticsState.Enabled;
        if (!_controller.Metrics.AtStart)
        {
            configuration.On(SemanticsAction.ScrollUp, () => _controller.ScrollBy(-Size.Height));
        }
        if (!_controller.Metrics.AtEnd)
        {
            configuration.On(SemanticsAction.ScrollDown, () => _controller.ScrollBy(Size.Height));
        }
    }

    protected override void PerformLayout()
    {
        if (!Constraints.HasBoundedHeight)
        {
            throw new InvalidOperationException("SliverFixedExtentViewport requires a bounded height.");
        }
        var width = Constraints.HasBoundedWidth ? Constraints.MaxWidth : 0;
        SetSize(Constraints.Constrain(new(width, Constraints.MaxHeight)));
        _controller.ApplyContentDimensions(Size.Height, _itemCount * _itemExtent);
        foreach (var child in Children)
        {
            child.Layout(BoxConstraints.TightFor(width: Size.Width, height: _itemExtent), parentUsesSize: true);
        }
        ApplyChildOffsets();
    }

    protected override bool HitTestSelf(Offset position) => true;

    protected override bool HitTestChildren(HitTestResult result, Offset position)
    {
        for (var index = Children.Count - 1; index >= 0; index--)
        {
            if (HitTestChild(result, Children[index], position))
            {
                return true;
            }
        }
        return false;
    }

    protected override void Paint(PaintingContext context, Offset offset)
    {
        var viewport = Rect.FromLeftTopWidthHeight(0, 0, Size.Width, Size.Height);
        context.PushClipRect(viewport, nested =>
        {
            foreach (var child in Children)
            {
                var childOffset = ((BoxParentData)child.ParentData!).Offset;
                if (childOffset.Y < Size.Height && childOffset.Y + child.Size.Height > 0)
                {
                    nested.PaintChild(child, childOffset);
                }
            }
        });
    }

    private void HandleControllerChanged()
    {
        ApplyChildOffsets();
        MarkNeedsPaint();
    }

    private void ApplyChildOffsets()
    {
        foreach (var child in Children)
        {
            var item = child as RenderIndexedListItem
                ?? throw new InvalidOperationException("SliverFixedExtentViewport children must be IndexedListItem render objects.");
            ((BoxParentData)child.ParentData!).Offset = new(0, (item.Index * _itemExtent) - _controller.Offset);
        }
    }
}
