using Doroti.Graphics;
using Doroti.Rendering;
using FlutterGestures = Doroti.Generated.Framework.Gestures;
using FlutterRuntime = Doroti.Flutter.Runtime;
using FlutterUi = Doroti.Flutter.Ui;

namespace Doroti.Widgets;

public enum PointerEventPhase
{
    Added,
    Hover,
    Down,
    Move,
    Up,
    Wheel,
    Removed,
    Cancelled,
}

public readonly record struct PointerEvent(
    ulong DeviceId,
    PointerEventPhase Phase,
    Offset Position,
    Offset LocalPosition,
    uint Buttons,
    Offset ScrollDelta,
    TimeSpan Timestamp,
    Doroti.Platform.PointerDeviceKind DeviceKind = Doroti.Platform.PointerDeviceKind.Mouse);

public abstract record PointerSignalEvent(
    ulong DeviceId,
    Offset Position,
    TimeSpan Timestamp);

/// <summary>A host-normalized, logical-pixel scroll signal.</summary>
public sealed record PointerScrollEvent(
    ulong DeviceId,
    Offset Position,
    Offset ScrollDelta,
    TimeSpan Timestamp) : PointerSignalEvent(DeviceId, Position, Timestamp);

public sealed class PointerSignalResolver
{
    private readonly FlutterGestures.PointerSignalResolver _inner = new();
    private PointerSignalEvent? _currentEvent;
    private FlutterGestures.PointerScrollEvent? _innerEvent;

    public bool HasRegisteredHandler { get; private set; }

    public void Register<TEvent>(TEvent input, Action<TEvent> callback)
        where TEvent : PointerSignalEvent
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(callback);
        if (_currentEvent is not null && !ReferenceEquals(_currentEvent, input))
        {
            throw new InvalidOperationException("A pointer signal resolver can resolve only one event at a time.");
        }
        _currentEvent = input;
        _innerEvent ??= ToFlutterSignal(input);
        if (!HasRegisteredHandler)
        {
            _inner.register(_innerEvent, _ => callback(input));
            HasRegisteredHandler = true;
        }
    }

    public bool Resolve(PointerSignalEvent input)
    {
        ArgumentNullException.ThrowIfNull(input);
        if (_currentEvent is not null && !ReferenceEquals(_currentEvent, input))
        {
            throw new InvalidOperationException("The resolved pointer signal does not match the registered event.");
        }
        var handled = HasRegisteredHandler;
        var innerEvent = _innerEvent ?? ToFlutterSignal(input);
        _inner.resolve(innerEvent);
        _currentEvent = null;
        _innerEvent = null;
        HasRegisteredHandler = false;
        return handled;
    }

    private static FlutterGestures.PointerScrollEvent ToFlutterSignal(PointerSignalEvent input)
    {
        var scrollDelta = input is PointerScrollEvent scroll ? scroll.ScrollDelta : default;
        return new FlutterGestures.PointerScrollEvent(
            timeStamp: (FlutterRuntime.Duration)input.Timestamp,
            device: checked((long)input.DeviceId),
            position: new FlutterUi.Offset(input.Position.X, input.Position.Y),
            scrollDelta: new FlutterUi.Offset(scrollDelta.X, scrollDelta.Y));
    }
}

public enum KeyboardEventPhase
{
    Down,
    Repeat,
    Up,
}

public readonly record struct KeyboardEvent(
    uint PhysicalKey,
    uint LogicalKey,
    KeyboardEventPhase Phase,
    TimeSpan Timestamp,
    KeyboardModifiers Modifiers = KeyboardModifiers.None);

public interface IPointerEventTarget
{
    void HandlePointerEvent(PointerEvent input);
}

public interface IPointerSignalTarget
{
    void RegisterPointerSignal(PointerScrollEvent input, PointerSignalResolver resolver);
}

public interface IFrameDrivenScrollTarget
{
    void BindScrollFrameDispatcher(Doroti.Core.IFrameDispatcher dispatcher);
}

public interface IFlutterArenaTarget : IPointerEventTarget
{
    void BindFlutterArena(FlutterArenaAdapter arena, Action<string>? trace = null);
}

public interface IKeyboardEventTarget
{
    bool HandleKeyboardEvent(KeyboardEvent input);
}

public sealed class PointerRouter
{
    private readonly FlutterGestures.PointerRouter _inner = new();
    private readonly Dictionary<(ulong DeviceId, Action<PointerEvent> Route), Action<FlutterGestures.PointerEvent>> _adapters = [];
    private PointerEvent? _currentInput;

    public void AddRoute(ulong deviceId, Action<PointerEvent> route)
    {
        ArgumentNullException.ThrowIfNull(route);
        if (_adapters.ContainsKey((deviceId, route))) return;
        Action<FlutterGestures.PointerEvent> adapter = _ => route(_currentInput ?? throw new InvalidOperationException("Pointer route escaped synchronous dispatch."));
        _adapters.Add((deviceId, route), adapter);
        _inner.addRoute(checked((long)deviceId), adapter);
    }

    public void RemoveRoute(ulong deviceId, Action<PointerEvent> route)
    {
        if (_adapters.Remove((deviceId, route), out var adapter))
        {
            _inner.removeRoute(checked((long)deviceId), adapter);
        }
    }

    public void Route(PointerEvent input)
    {
        var previous = _currentInput;
        _currentInput = input;
        try
        {
            _inner.route(new FlutterGestures.PointerHoverEvent(
                timeStamp: (FlutterRuntime.Duration)input.Timestamp,
                pointer: checked((long)input.DeviceId),
                device: checked((long)input.DeviceId),
                position: new FlutterUi.Offset(input.Position.X, input.Position.Y),
                buttons: input.Buttons));
        }
        finally
        {
            _currentInput = previous;
        }
    }
}

/// <summary>
/// Host-neutral lifetime handoff for Flutter's reviewed gesture arena. Winner selection,
/// hold/release/sweep behavior and recognizer policy remain in the reviewed Gestures package.
/// </summary>
public sealed class FlutterArenaAdapter
{
    private readonly FlutterGestures.GestureArenaManager _inner = new();
    private readonly FlutterRuntime.DartMicrotaskQueue _microtasks = new();

    public void Close(ulong deviceId)
    {
        using var scope = FlutterRuntime.DartAsyncRuntime.enterMicrotaskScheduler(_microtasks.enqueue);
        _inner.close(checked((long)deviceId));
    }

    public void Sweep(ulong deviceId)
    {
        _inner.sweep(checked((long)deviceId));
    }

    public void Hold(ulong deviceId)
    {
        _inner.hold(checked((long)deviceId));
    }

    public void Release(ulong deviceId)
    {
        _inner.release(checked((long)deviceId));
    }

    public void FlushMicrotasks()
    {
        _microtasks.drain();
    }

    internal FlutterGestures.GestureArenaEntry Add(long pointer, FlutterGestures.GestureArenaMember member) =>
        _inner.add(pointer, member);
}

internal static class FlutterInputAdapter
{
    internal static FlutterGestures.TapGestureRecognizer CreateTap(Action onTap, Action<string>? trace)
    {
        ArgumentNullException.ThrowIfNull(onTap);
        return new HostTapBinding(trace)
        {
            onTap = () =>
            {
                trace?.Invoke("tap:invoke");
                onTap();
            },
            onTapCancel = () => trace?.Invoke("tap:cancel"),
        };
    }

    internal static FlutterGestures.VerticalDragGestureRecognizer CreateVerticalDrag(
        Action<FlutterGestures.DragUpdateDetails> onUpdate,
        Action<FlutterGestures.DragEndDetails>? onEnd,
        Action<string>? trace,
        double touchSlop)
    {
        ArgumentNullException.ThrowIfNull(onUpdate);
        return new HostVerticalDragBinding(trace)
        {
            gestureSettings = new FlutterGestures.DeviceGestureSettings(touchSlop),
            dragStartBehavior = FlutterGestures.DragStartBehavior.down,
            onUpdate = onUpdate,
            onEnd = onEnd,
            onCancel = () => trace?.Invoke("vertical-drag:cancel"),
        };
    }

    internal static void AddPointer(
        FlutterGestures.OneSequenceGestureRecognizer recognizer,
        PointerEvent input,
        FlutterArenaAdapter arena)
    {
        if (input.Phase is not PointerEventPhase.Down) return;
        ((IHostArenaBinding)recognizer).Bind(arena);
        recognizer.addPointer((FlutterGestures.PointerDownEvent)FlutterPointerEventAdapter.Convert(input));
    }

    private interface IHostArenaBinding
    {
        void Bind(FlutterArenaAdapter arena);
    }

    private sealed class HostTapBinding(Action<string>? trace)
        : FlutterGestures.TapGestureRecognizer(
            preAcceptSlopTolerance: FlutterGestures.ConstantsLibrary.kTouchSlop,
            postAcceptSlopTolerance: FlutterGestures.ConstantsLibrary.kTouchSlop), IHostArenaBinding
    {
        private FlutterArenaAdapter? _arena;

        public void Bind(FlutterArenaAdapter arena) => _arena = arena;

        public override FlutterGestures.GestureArenaEntry _addPointerToArena(long pointer) =>
            (_arena ?? throw new InvalidOperationException("The Flutter tap recognizer has no host arena binding."))
                .Add(pointer, this);

        public override void startTrackingPointer(long pointer, FlutterUi.Matrix4? transform = null)
        {
            trace?.Invoke($"tap:add:{pointer}");
            _trackedPointers.Add(pointer);
            _entries[pointer] = _addPointerToArena(pointer);
        }

        public override void stopTrackingPointer(long pointer)
        {
            if (!_trackedPointers.Remove(pointer)) return;
            if (_trackedPointers.Count == 0) didStopTrackingLastPointer(pointer);
        }

        public override void dispose()
        {
            resolve(FlutterGestures.GestureDisposition.rejected);
            _trackedPointers.Clear();
            base.dispose();
        }
    }

    private sealed class HostVerticalDragBinding(Action<string>? trace) : FlutterGestures.VerticalDragGestureRecognizer, IHostArenaBinding
    {
        private FlutterArenaAdapter? _arena;

        public void Bind(FlutterArenaAdapter arena) => _arena = arena;

        public override FlutterGestures.GestureArenaEntry _addPointerToArena(long pointer) =>
            (_arena ?? throw new InvalidOperationException("The Flutter vertical drag recognizer has no host arena binding."))
                .Add(pointer, this);

        public override void startTrackingPointer(long pointer, FlutterUi.Matrix4? transform = null)
        {
            trace?.Invoke($"vertical-drag:add:{pointer}");
            _trackedPointers.Add(pointer);
            _entries[pointer] = _addPointerToArena(pointer);
        }

        public override void stopTrackingPointer(long pointer)
        {
            if (!_trackedPointers.Remove(pointer)) return;
            if (_trackedPointers.Count == 0) didStopTrackingLastPointer(pointer);
        }

        public override void dispose()
        {
            resolve(FlutterGestures.GestureDisposition.rejected);
            _trackedPointers.Clear();
            base.dispose();
        }
    }
}

internal static class FlutterPointerEventAdapter
{
    internal static FlutterGestures.PointerEvent Convert(PointerEvent input, FlutterUi.Offset? eventDelta = null)
    {
        var pointer = checked((long)input.DeviceId);
        var position = new FlutterUi.Offset(input.Position.X, input.Position.Y);
        var delta = eventDelta ?? FlutterUi.Offset.zero;
        var buttons = checked((long)input.Buttons);
        var timeStamp = (FlutterRuntime.Duration)input.Timestamp;
        var kind = input.DeviceKind switch
        {
            Doroti.Platform.PointerDeviceKind.Mouse => FlutterUi.PointerDeviceKind.mouse,
            Doroti.Platform.PointerDeviceKind.Touch => FlutterUi.PointerDeviceKind.touch,
            Doroti.Platform.PointerDeviceKind.Pen => FlutterUi.PointerDeviceKind.stylus,
            _ => FlutterUi.PointerDeviceKind.unknown,
        };
        FlutterGestures.PointerEvent converted = input.Phase switch
        {
            PointerEventPhase.Down => new FlutterGestures.PointerDownEvent(timeStamp: timeStamp, pointer: pointer, device: pointer, kind: kind, position: position, buttons: buttons),
            PointerEventPhase.Move => new FlutterGestures.PointerMoveEvent(timeStamp: timeStamp, pointer: pointer, device: pointer, kind: kind, position: position, delta: delta, buttons: buttons),
            PointerEventPhase.Up => new FlutterGestures.PointerUpEvent(timeStamp: timeStamp, pointer: pointer, device: pointer, kind: kind, position: position, buttons: buttons),
            PointerEventPhase.Cancelled or PointerEventPhase.Removed => new FlutterGestures.PointerCancelEvent(timeStamp: timeStamp, pointer: pointer, device: pointer, kind: kind, position: position, buttons: buttons),
            _ => new FlutterGestures.PointerMoveEvent(timeStamp: timeStamp, pointer: pointer, device: pointer, kind: kind, position: position, delta: delta, buttons: buttons),
        };
        var localOffset = input.LocalPosition - input.Position;
        return converted.transformed(FlutterUi.Matrix4.translationValues(localOffset.X, localOffset.Y, 0));
    }
}

public sealed class Text : LeafRenderObjectWidget
{
    public Text(string data, double fontSize = 24, Color? color = null, Key? key = null)
        : base(key)
    {
        ArgumentException.ThrowIfNullOrEmpty(data);
        Data = data;
        FontSize = fontSize;
        Color = color ?? Color.FromArgb(255, 240, 244, 255);
    }

    public string Data { get; }

    public double FontSize { get; }

    public Color Color { get; }

    public override RenderObject CreateRenderObject(BuildContext context) => new RenderParagraph(Data, new UnicodeParagraphLayout(), FontSize, Color);

    public override void UpdateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var paragraph = (RenderParagraph)renderObject;
        paragraph.Text = Data;
        paragraph.FontSize = FontSize;
        paragraph.Color = Color;
    }
}

public sealed class ColoredBox(Color color, Widget? child = null, Key? key = null) : SingleChildRenderObjectWidget(child, key)
{
    public Color Color { get; } = color;

    public override RenderObject CreateRenderObject(BuildContext context) => new RenderColoredBox(Color);

    public override void UpdateRenderObject(BuildContext context, RenderObject renderObject) => ((RenderColoredBox)renderObject).Color = Color;
}

public sealed class Center(Widget? child = null, Key? key = null) : SingleChildRenderObjectWidget(child, key)
{
    public override RenderObject CreateRenderObject(BuildContext context) => new RenderPositionedBox(Alignment.Center);
}

public sealed class Padding(EdgeInsets padding, Widget? child = null, Key? key = null) : SingleChildRenderObjectWidget(child, key)
{
    public EdgeInsets PaddingValue { get; } = padding;

    public override RenderObject CreateRenderObject(BuildContext context) => new RenderPadding(PaddingValue);

    public override void UpdateRenderObject(BuildContext context, RenderObject renderObject) => ((RenderPadding)renderObject).Padding = PaddingValue;
}

public sealed class Row(IEnumerable<Widget> children, Key? key = null) : MultiChildRenderObjectWidget(children, key)
{
    public override RenderObject CreateRenderObject(BuildContext context) => new RenderFlex(Axis.Horizontal, MainAxisAlignment.Center);
}

public sealed class Column(IEnumerable<Widget> children, Key? key = null) : MultiChildRenderObjectWidget(children, key)
{
    public override RenderObject CreateRenderObject(BuildContext context) => new RenderFlex(Axis.Vertical, MainAxisAlignment.Center);
}

public sealed class Stack(IEnumerable<Widget> children, Key? key = null) : MultiChildRenderObjectWidget(children, key)
{
    public override RenderObject CreateRenderObject(BuildContext context) => new RenderStack(Alignment.Center);
}

public sealed class GestureDetector : SingleChildRenderObjectWidget
{
    public GestureDetector(Action onTap, Widget? child = null, Key? key = null)
        : base(child, key)
    {
        OnTap = onTap ?? throw new ArgumentNullException(nameof(onTap));
    }

    public Action OnTap { get; }

    public override RenderObject CreateRenderObject(BuildContext context) => new RenderGestureDetector(OnTap);

    public override void UpdateRenderObject(BuildContext context, RenderObject renderObject) => ((RenderGestureDetector)renderObject).OnTap = OnTap;
}

public sealed class RenderGestureDetector : RenderProxyBox, IFlutterArenaTarget, IKeyboardEventTarget
{
    private FlutterGestures.TapGestureRecognizer? _recognizer;
    private FlutterArenaAdapter? _arena;
    private Action _onTap;

    public RenderGestureDetector(Action onTap)
    {
        _onTap = onTap ?? throw new ArgumentNullException(nameof(onTap));
    }

    public Action OnTap
    {
        get => _onTap;
        set
        {
            _onTap = value ?? throw new ArgumentNullException(nameof(value));
            _recognizer?.dispose();
            _recognizer = null;
        }
    }

    public void BindFlutterArena(FlutterArenaAdapter arena, Action<string>? trace = null)
    {
        _arena = arena;
        _recognizer ??= FlutterInputAdapter.CreateTap(() => _onTap(), trace);
    }

    public void HandlePointerEvent(PointerEvent input)
    {
        if (_arena is null)
        {
            throw new InvalidOperationException("GestureDetector was dispatched without a gesture arena.");
        }
        _recognizer ??= FlutterInputAdapter.CreateTap(() => _onTap(), trace: null);
        if (input.Phase is PointerEventPhase.Down)
        {
            FlutterInputAdapter.AddPointer(_recognizer, input, _arena);
        }
        _recognizer.handleEvent(FlutterPointerEventAdapter.Convert(input));
    }

    public bool HandleKeyboardEvent(KeyboardEvent input)
    {
        if (input.Phase is KeyboardEventPhase.Down && input.LogicalKey is 0x0D or 0x20)
        {
            _onTap();
            return true;
        }
        return false;
    }

    public override void DescribeSemanticsConfiguration(SemanticsConfiguration configuration)
    {
        configuration.Role = Doroti.Platform.SemanticsRole.Button;
        configuration.State = Doroti.Platform.SemanticsState.Enabled;
        configuration.On(Doroti.Platform.SemanticsAction.Tap, _onTap);
    }

    protected override void PerformLayout()
    {
        if (Child is null)
        {
            SetSize(Constraints.Constrain(Size.Zero));
            return;
        }
        Child.Layout(Constraints, parentUsesSize: true);
        SetSize(Constraints.Constrain(Child.Size));
        ((BoxParentData)Child.ParentData!).Offset = Offset.Zero;
    }

    protected override bool HitTestSelf(Offset position) => true;
}
