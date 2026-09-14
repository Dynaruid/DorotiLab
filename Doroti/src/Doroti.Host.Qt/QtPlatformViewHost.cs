using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Doroti.Hosting;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Host.Qt;

/// <summary>Owner-local native controls. The optional Quick ABI composes GPU raster
/// images with live Quick Controls; the legacy ABI places disjoint child Widgets.</summary>
internal sealed partial class QtPlatformViewHost : IPlatformViewDispatcher, IDisposable
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly record struct NativeRect(double X, double Y, double Width, double Height);
    [StructLayout(LayoutKind.Sequential)]
    internal struct Placement
    {
        public uint Size, Visible;
        public ulong Id;
        public NativeRect Bounds, Clip;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct Api
    {
        public uint Version, Size;
        public ulong Features;
        public delegate* unmanaged[Cdecl]<ulong, delegate* unmanaged[Cdecl]<nint, int, void>, nint, int> Post;
        public delegate* unmanaged[Cdecl]<ulong, uint, QtNativeV2.Utf8, delegate* unmanaged[Cdecl]<nint, ulong, void>, nint, ulong*, int> Create;
        public nint AdoptWidget;
        public delegate* unmanaged[Cdecl]<ulong, Placement*, ulong, uint, int> Commit;
        public delegate* unmanaged[Cdecl]<ulong, ulong, uint, int> Focus;
        public delegate* unmanaged[Cdecl]<ulong, ulong, int> Remove;
    }
    private Api _api;
    private ulong _owner;
    private int _thread;
    private volatile bool _closed;
    private readonly Dictionary<PlatformViewHandle, Instance> _instances = [];
    private readonly Dictionary<ulong, Instance> _nativeInstances = [];
    private GCHandle _focusContext;
    private Action<Action>? _dispatchFocus;
    private PlatformViewCoordinator? _coordinator;
    private PlatformCompositionPlan? _pending;
    private IPlatformViewPlacementBatch? _reservation;
    private Placement[] _next = [];
    private PlatformViewPlacement[] _nextPlacements = [];
    private Action? _yieldText;
    private Action<Exception>? _fatal;
    internal QtSkiaSurface? QuickSurface { get; set; }
    private bool IsQuick => (_api.Features & 2) != 0;
    internal IEnumerable<IPlatformViewFactory> Factories =>
        [new Factory(this, false), new Factory(this, true)];

    internal unsafe void Bind(nint window, Action yieldText, Action<Exception> fatal, Action<Action> dispatchFocus)
    {
        if (_owner != 0) throw new InvalidOperationException("Qt PlatformView owner is already bound.");
        Check(GetApi(window, 1, (uint)sizeof(Api), out _owner, out _api));
        if (_api.Version != 1 || _api.Size != sizeof(Api) || _api.Features is not (1 or 3) ||
            _api.Post == null || _api.Create == null || _api.Commit == null || _api.Focus == null || _api.Remove == null)
            throw new InvalidDataException("Invalid Qt PlatformView ABI table.");
        _thread = Environment.CurrentManagedThreadId;
        _yieldText = yieldText;
        _fatal = fatal;
        _dispatchFocus = dispatchFocus;
        _focusContext = GCHandle.Alloc(this);
    }
    internal void Configure(PlatformViewCoordinator coordinator) => _coordinator = coordinator;
    internal static void Check(int result)
    {
        if (result != 0) throw new InvalidOperationException($"Qt PlatformView operation rejected (status {result}).");
    }
    private void Verify()
    {
        ObjectDisposedException.ThrowIf(_closed, this);
        if (_owner == 0 || Environment.CurrentManagedThreadId != _thread)
            throw new InvalidOperationException("Qt PlatformView requires its bound GUI thread.");
    }

    private sealed class Factory(QtPlatformViewHost host, bool editor) : IPlatformViewFactory
    {
        public string ViewType => editor ? "doroti/native-editor" : "doroti/native-button";
        public PlatformViewSupport QuerySupport(PlatformViewRequest request) => new(
            host.IsQuick ? "linux/qt-quick-gpu" : "linux/qt-native-child-widgets", "Qt6", ViewType,
            !host._closed && host._owner != 0 && request.ViewType == ViewType &&
            (request.Composition == PlatformViewComposition.NativeOverlay ||
                host.IsQuick && request.Composition == PlatformViewComposition.InterleavedComposition) &&
            (request.Effects & ~PlatformViewEffects.RectClip) == 0,
            host.IsQuick ? request.Composition : PlatformViewComposition.NativeOverlay, PlatformViewEffects.RectClip,
            Reason: host.IsQuick ? "Qt Quick Controls and Graphite Vulkan GPU images; translation and rect clip. Physical presentation atomicity is not qualified." :
                "Limited B: disjoint Widgets, rounded logical translation/inward rect clip. Interleaving, shields, affine transforms and synchronized placement are unsupported.");
        public unsafe ValueTask<IPlatformViewInstance> CreateAsync(PlatformViewHandle handle,
            ReadOnlyMemory<byte> parameters, Action<PlatformViewHandle> focused, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            host.Verify();
            if (parameters.Length != 0) throw new NotSupportedException("Qt built-in controls do not accept creation parameters.");
            var instance = new Instance(host, handle, focused);
            var context = host._focusContext;
            var bytes = Encoding.UTF8.GetBytes(editor ? "Edit native Qt text" : "Native Qt button");
            fixed (byte* text = bytes)
            {
                ulong id;
                Check(host._api.Create(host._owner, editor ? 1u : 0u,
                    new(text, (ulong)bytes.Length), &Focused, GCHandle.ToIntPtr(context), &id));
                instance.Id = id;
            }
            host._instances.Add(handle, instance);
            host._nativeInstances.Add(instance.Id, instance);
            return ValueTask.FromResult<IPlatformViewInstance>(instance);
        }
    }
    private sealed class Instance(QtPlatformViewHost host, PlatformViewHandle handle,
        Action<PlatformViewHandle> focused) : IPlatformViewInstance
    {
        internal ulong Id;
        internal void NotifyFocus() { host._yieldText?.Invoke(); focused(handle); }
        public ValueTask ApplyAsync(PlatformViewPlacement placement)
        {
            host.Verify();
            if (!host._committing)
            {
                var next = host._visible.Where(p => p.Id != Id).Append(Translate(placement, Id)).ToArray();
                host.Commit(next, apply: true);
                host._visible = next;
                host._visibleHandles = host._visibleHandles.Where(h => h != handle).Append(handle).ToArray();
            }
            return ValueTask.CompletedTask;
        }
        public ValueTask DetachAsync() => DisableInputAsync();
        public unsafe ValueTask SetFocusAsync(bool focused)
        { if (!host._closed) { host.Verify(); Check(host._api.Focus(host._owner, Id, focused ? 1u : 0u)); } return ValueTask.CompletedTask; }
        public unsafe ValueTask DisableInputAsync()
        {
            if (!host._closed)
            {
                host.Verify();
                // Preserve all other visible placements when retiring one instance.
                var placements = host._visible.Where(p => p.Id != Id).ToArray();
                host.Commit(placements, apply: true);
                host._visible = placements;
            }
            return ValueTask.CompletedTask;
        }
        public unsafe ValueTask DisposeAsync()
        {
            if (!host._closed) { host.Verify(); Check(host._api.Remove(host._owner, Id)); }
            host._instances.Remove(handle); host._nativeInstances.Remove(Id);
            return ValueTask.CompletedTask;
        }
    }
    private bool _committing;
    private Placement[] _visible = [];
    private PlatformViewHandle[] _visibleHandles = [];

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void Focused(nint context, ulong id)
    {
        QtPlatformViewHost? host = null;
        try
        {
            host = (QtPlatformViewHost)GCHandle.FromIntPtr(context).Target!;
            if (!host._closed && host._nativeInstances.TryGetValue(id, out var instance)) host._dispatchFocus?.Invoke(instance.NotifyFocus);
        }
        catch (Exception error) { host?._fatal?.Invoke(error); }
    }

    private sealed record Work(QtPlatformViewHost Host, Func<ValueTask> Action, TaskCompletionSource Completion);
    public unsafe ValueTask InvokeAsync(Func<ValueTask> action)
    {
        // After native teardown only managed instance cleanup remains. Serialize it
        // with the same lock used by the closed callback path.
        if (_closed) { lock (_instances) return action(); }
        if (Environment.CurrentManagedThreadId == _thread) return action();
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var context = GCHandle.Alloc(new Work(this, action, completion));
        var result = _api.Post(_owner, &RunWork, GCHandle.ToIntPtr(context));
        if (result != 0)
        {
            context.Free();
            if (result == 71) { _closed = true; lock (_instances) return action(); }
            Check(result);
        }
        return new(completion.Task);
    }
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void RunWork(nint context, int result)
    {
        var handle = GCHandle.FromIntPtr(context);
        var work = (Work)handle.Target!;
        handle.Free();
        try
        {
            if (result == 71) work.Host._closed = true;
            else Check(result);
            ValueTask task;
            lock (work.Host._instances) task = work.Action();
            if (!task.IsCompleted) throw new InvalidOperationException("Qt built-in native operations must complete synchronously on the GUI thread.");
            task.GetAwaiter().GetResult();
            work.Completion.TrySetResult();
        }
        catch (Exception error) { work.Completion.TrySetException(error); }
    }

    internal static Placement Translate(PlatformViewPlacement p, ulong id)
    {
        p.Validate();
        var t = p.Transform;
        if (t.M11 != 1 || t.M22 != 1 || t.M12 != 0 || t.M21 != 0)
            throw new NotSupportedException("Qt native child Widgets only support translation.");
        var bounds = p.Bounds.shift(new Offset(t.Dx, t.Dy));
        var clip = p.Clip is { } c ? bounds.intersect(c) : bounds;
        static NativeRect Convert(Rect r, bool inward)
        {
            foreach (var value in new[] { r.left, r.top, r.right, r.bottom })
                if (!double.IsFinite(value) || Math.Abs(value) > 1e6)
                    throw new NotSupportedException("Qt native child Widgets require finite geometry within 1,000,000 logical pixels.");
            var x = inward ? Math.Ceiling(r.left) : Math.Round(r.left);
            var y = inward ? Math.Ceiling(r.top) : Math.Round(r.top);
            var right = inward ? Math.Floor(r.right) : Math.Round(r.right);
            var bottom = inward ? Math.Floor(r.bottom) : Math.Round(r.bottom);
            return new(x, y, Math.Max(0, right - x), Math.Max(0, bottom - y));
        }
        return new() { Size = 80, Visible = p.Visible && !clip.isEmpty ? 1u : 0u, Id = id,
            Bounds = Convert(bounds, false), Clip = clip.isEmpty ? default : Convert(clip, true) };
    }

    private unsafe void Commit(Placement[] placements, bool apply)
    {
        fixed (Placement* items = placements) Check(_api.Commit(_owner, items, (ulong)placements.Length, apply ? 1u : 0u));
    }
    internal bool TryBeginFrame()
    {
        CancelPending();
        if (_coordinator is null) return true;
        Verify();
        var live = _instances.Keys.Where(handle => _coordinator.GetState(handle) is
            PlatformViewState.Ready or PlatformViewState.Attached or PlatformViewState.Hidden or PlatformViewState.Detached).ToArray();
        // Before the renderer consumes a scene or acquires a swapchain image.
        // Focus/disposal contention leaves the previous frame and native geometry intact.
        return _coordinator.TryBeginPlacementBatch(live, out _reservation);
    }
    internal void Draw(SkiaSceneRenderer renderer, SKCanvas canvas, IReadOnlyList<SceneCommand> commands,
        DorotiFrameDescriptor descriptor, int width, int height)
    {
        Verify();
        if (_pending is not null) throw new InvalidOperationException("Qt PlatformView has an unretired frame.");
        var token = new PlatformCompositionToken(descriptor.ViewId, descriptor.MetricsGeneration,
            descriptor.SceneSequence, descriptor.ResizeTargetGeneration, descriptor.DeviceScaleX, descriptor.DeviceScaleY);
        var plan = PlatformCompositionPlanner.Build(commands, token, _coordinator!,
            IsQuick ? PlatformViewComposition.InterleavedComposition : PlatformViewComposition.NativeOverlay);
        try
        {
            if (!IsQuick && plan.HasNativeContent && plan.Parts.OfType<PlatformShieldSegment>().Any())
                throw new NotSupportedException("Qt NativeOverlay does not support input shields; interleaving is required.");
            var placements = plan.Parts.OfType<PlatformNativeSegment>().Select(p => p.Placement).ToArray();
            var native = placements.Select(p => Translate(p, _instances[p.Handle].Id)).ToArray();
            if (_reservation is null || placements.Any(p => !_reservation.Contains(p.Handle)))
                throw new InvalidOperationException("Qt PlatformView frame references an unreserved instance.");
            Commit(native, apply: false);
            if (IsQuick) DrawQuick(renderer, plan, width, height, descriptor);
            else foreach (var raster in plan.Parts.OfType<PlatformRasterSegment>())
                renderer.DrawPlatformRasterSegment(canvas, raster.Commands, width, height);
            _pending = plan; _next = native; _nextPlacements = placements;
        }
        catch { _reservation?.Dispose(); _reservation = null; plan.Dispose(); throw; }
    }
    private void DrawQuick(SkiaSceneRenderer renderer, PlatformCompositionPlan plan, int width, int height, DorotiFrameDescriptor descriptor)
    {
        var surface=QuickSurface ?? throw new InvalidOperationException("Qt Quick surface is not configured.");
        var gpu=surface.QuickGpu ?? throw new InvalidOperationException("Qt Quick GPU is not initialized.");
        var parts=new List<QtQuickNative.Part>();
        var rasterIndex=0;
        var viewport=new NativeRect(0,0,width/descriptor.DeviceScaleX,height/descriptor.DeviceScaleY);
        foreach(var part in plan.Parts)
        {
            switch(part)
            {
                case PlatformRasterSegment raster:
                    var target=gpu.Canvas(rasterIndex);
                    if(rasterIndex==0)target.DrawColor(renderer.PlatformBackgroundColor);
                    renderer.DrawPlatformRasterSegment(target,raster.Commands,width,height);
                    parts.Add(new() { Size=96,Kind=0,Id=gpu.Identity(rasterIndex),Image=gpu.Image(rasterIndex++),PixelWidth=(uint)width,PixelHeight=(uint)height,Bounds=viewport,Clip=viewport });
                    break;
                case PlatformNativeSegment native:
                    var placement=Translate(native.Placement,_instances[native.Placement.Handle].Id);
                    parts.Add(new() { Size=96,Kind=1,Id=placement.Id,Bounds=placement.Bounds,Clip=placement.Visible!=0?placement.Clip:default });
                    break;
                case PlatformShieldSegment shield:
                    var t=shield.Shield.Transform;
                    if(!t.IsAxisAligned)throw new NotSupportedException("Qt Quick input shields require axis-aligned bounds.");
                    var a=t.Map(shield.Shield.Bounds.topLeft);var b=t.Map(shield.Shield.Bounds.bottomRight);
                    var bounds=new Rect(a.dx,a.dy,b.dx,b.dy);
                    var clip=shield.Shield.Clip is {} c?bounds.intersect(c):bounds;
                    parts.Add(new() { Size=96,Kind=2,Bounds=new(bounds.left,bounds.top,bounds.width,bounds.height),
                        Clip=new(clip.left,clip.top,Math.Max(0,clip.width),Math.Max(0,clip.height)) });
                    break;
            }
        }
        surface.QuickParts=parts.ToArray();
    }
    internal void FinishFrame(bool presented)
    {
        if (_pending is null) { CancelPending(); return; }
        try
        {
            if (!presented || _closed) return;
            if (!IsQuick) Commit(_next, apply: true);
            _visible = _next;
            _committing = true;
            foreach (var placement in _nextPlacements)
            {
                var task = _reservation!.AttachAsync(placement);
                if (!task.IsCompleted) throw new InvalidOperationException("Qt placement unexpectedly suspended on GUI thread.");
                task.GetAwaiter().GetResult();
            }
            var nextHandles = _nextPlacements.Select(p => p.Handle).ToHashSet();
            foreach (var old in _visibleHandles.Where(h => !nextHandles.Contains(h) && _reservation!.Contains(h)))
                _reservation!.DetachAsync(old).GetAwaiter().GetResult();
            _visibleHandles = nextHandles.ToArray();
        }
        finally { _committing = false; CancelPending(); }
    }
    internal void CancelPending()
    {
        _reservation?.Dispose(); _reservation = null;
        _pending?.Dispose(); _pending = null;
        _next = []; _nextPlacements = [];
    }
    // Called from native closed only after pending post callbacks and QWidgets die.
    internal void NativeClosed() { _closed = true; CancelPending(); }
    public void Dispose()
    {
        NativeClosed();
        if (_focusContext.IsAllocated) _focusContext.Free();
    }
    [LibraryImport("doroti_qt_host", EntryPoint = "doroti_qt_get_platform_views")]
    private static partial int GetApi(nint view, uint version, uint size, out ulong owner, out Api api);
}
