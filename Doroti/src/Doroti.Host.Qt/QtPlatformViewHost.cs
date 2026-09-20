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
    private PlatformCompositionSession? _session;
    private long _compositionFrame;
    private sealed record QuickRaster(SkiaPlatformRasterContent.CacheScope Scope, SkiaPlatformRasterContent.Slice Slice, ulong Identity);
    private Dictionary<int, QuickRaster> _quickRasters = [];
    private Dictionary<int, QuickRaster> _nextQuickRasters = [];
    private object? _quickCacheOwner;
    internal long CommittedFrames { get; private set; }
    private PlatformCompositionPlan? _pending;
    private IPlatformViewPlacementBatch? _reservation;
    private Placement[] _next = [];
    private PlatformViewPlacement[] _nextPlacements = [];
    private Action? _yieldText;
    private Action<Exception>? _fatal;
    internal QtSkiaSurface? QuickSurface { get; set; }
    internal IApplicationResourceHostCapability? Resources { get; set; }
    internal string ApplicationId { get; set; } = "doroti";
    private bool IsQuick => (_api.Features & 2) != 0;
    private bool HasWebEngine => IsQuick && (_api.Features & 4) != 0;
    private bool HasEffects => IsQuick && (_api.Features & 24) == 24;
    private static readonly PlatformEffectSupport QuickEffects = new(true, 1, 32, Saturation: true,
        Reason: "Qt Quick supports one bounded live Gaussian source group, sigma <=32 and saturation 0–2.");
    internal IEnumerable<IPlatformViewFactory> Factories =>
        [new Factory(this, 0), new Factory(this, 1), new Factory(this, 2)];

    internal unsafe void Bind(nint window, Action yieldText, Action<Exception> fatal, Action<Action> dispatchFocus)
    {
        if (_owner != 0) throw new InvalidOperationException("Qt PlatformView owner is already bound.");
        Check(GetApi(window, 1, (uint)sizeof(Api), out _owner, out _api));
        if (_api.Version != 1 || _api.Size != sizeof(Api) || (_api.Features & 1) == 0 || (_api.Features & ~31UL) != 0 ||
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

    private sealed class Factory(QtPlatformViewHost host, uint kind) : IPlatformViewFactory
    {
        public string ViewType => kind switch { 0 => "doroti/native-button", 1 => "doroti/native-editor", _ => "doroti/webview" };
        public PlatformViewSupport QuerySupport(PlatformViewRequest request) => new(
            host.IsQuick ? "linux/qt-quick-gpu" : "linux/qt-native-child-widgets", "Qt6", ViewType,
            !host._closed && host._owner != 0 && request.ViewType == ViewType && (kind != 2 || host.HasWebEngine) &&
            (request.Composition == PlatformViewComposition.NativeOverlay ||
                host.IsQuick && request.Composition == PlatformViewComposition.InterleavedComposition) &&
            (request.Effects & ~PlatformViewEffects.RectClip) == 0,
            host.IsQuick ? request.Composition : PlatformViewComposition.NativeOverlay, PlatformViewEffects.RectClip,
            NativeBackdropBlur: host.HasEffects, WebViewCommands: kind == 2 && host.HasWebEngine,
            Capabilities: new(PlatformViewRepresentation.NativeHierarchy,
                host.IsQuick ? PlatformViewTransport.GpuShared : PlatformViewTransport.Native,
                PlatformViewInputPolicy.DirectNative, host.HasEffects ? QuickEffects : PlatformEffectSupport.Unsupported),
            Reason: kind == 2 && !host.HasWebEngine ? "WebEngine Quick requires DorotiQtQuick=true and DorotiQtWebEngine=true; rebuild the native shim." :
                host.IsQuick ? "Live Qt Quick items and Graphite Vulkan GPU images; translation and rect clip. Physical presentation atomicity is not qualified." :
                "Limited B: disjoint Widgets, rounded logical translation/inward rect clip. Interleaving, shields, affine transforms and synchronized placement are unsupported.");
        public async ValueTask<IPlatformViewInstance> CreateAsync(PlatformViewHandle handle,
            ReadOnlyMemory<byte> parameters, Action<PlatformViewHandle> focused, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (kind != 2 && parameters.Length != 0) throw new NotSupportedException("Qt built-in controls do not accept creation parameters.");
            if (kind == 2 && !host.HasWebEngine) throw new WebViewException(WebViewError.Unsupported, "Qt WebEngine Quick is disabled.");
            var bytes = kind == 2 ? await QtWebViewSession.PrepareAsync(parameters, host.Resources, host.ApplicationId, cancellationToken).ConfigureAwait(false)
                : Encoding.UTF8.GetBytes(kind == 1 ? "Edit native Qt text" : "Native Qt button");
            IPlatformViewInstance? result = null;
            await host.InvokeAsync(() => { cancellationToken.ThrowIfCancellationRequested(); result = Create(handle, focused, bytes); return ValueTask.CompletedTask; });
            return result!;
        }
        private unsafe IPlatformViewInstance Create(PlatformViewHandle handle, Action<PlatformViewHandle> focused, byte[] bytes)
        {
            host.Verify();
            var instance = new Instance(host, handle, focused);
            var context = host._focusContext;
            fixed (byte* text = bytes)
            {
                ulong id;
                var status = host._api.Create(host._owner, kind,
                    new(text, (ulong)bytes.Length), &Focused, GCHandle.ToIntPtr(context), &id);
                if (kind == 2 && status != 0) throw new WebViewException(WebViewError.Unsupported,
                    $"Qt WebEngine creation failed (status {status}); verify system WebEngine/QML/helper dependencies and native diagnostics.");
                Check(status);
                instance.Id = id;
            }
            try
            {
                if (kind == 2) instance.Web = new QtWebViewSession(host, handle, host._owner, instance.Id, instance.NotifyWeb);
            }
            catch { host._api.Remove(host._owner, instance.Id); throw; }
            host._instances.Add(handle, instance);
            host._nativeInstances.Add(instance.Id, instance);
            return instance;
        }
    }
    private sealed class Instance(QtPlatformViewHost host, PlatformViewHandle handle,
        Action<PlatformViewHandle> focused) : IPlatformViewInstance, IPlatformWebViewInstance
    {
        internal ulong Id;
        internal QtWebViewSession? Web;
        public event Action<WebViewEvent>? WebViewChanged;
        internal void NotifyWeb(WebViewEvent value) => WebViewChanged?.Invoke(value);
        public Task<WebViewResult> ExecuteAsync(WebViewCommand command, CancellationToken cancellationToken) =>
            Web?.ExecuteAsync(command, cancellationToken) ?? throw new WebViewException(WebViewError.Unsupported, "This item is not a WebView.");
        internal void NotifyFocus() { host._yieldText?.Invoke(); focused(handle); }
        public ValueTask ApplyAsync(PlatformViewPlacement placement)
        {
            host.Verify();
            if (!host._committing)
            {
                var next = host._visible.Where(p => p.Id != Id).Append(Translate(placement, Id, host.IsQuick)).ToArray();
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
            Web?.Close(host._closed); Web = null; WebViewChanged = null;
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
            if (task.IsCompleted) { task.GetAwaiter().GetResult(); work.Completion.TrySetResult(); }
            else _ = CompleteWorkAsync(task, work.Completion);
        }
        catch (Exception error) { work.Completion.TrySetException(error); }
    }

    private static async Task CompleteWorkAsync(ValueTask task, TaskCompletionSource completion)
    {
        try { await task.ConfigureAwait(false); completion.TrySetResult(); }
        catch (Exception error) { completion.TrySetException(error); }
    }

    internal static Placement Translate(PlatformViewPlacement p, ulong id, bool quick = false)
    {
        p.Validate();
        var t = p.Transform;
        if (t.M11 != 1 || t.M22 != 1 || t.M12 != 0 || t.M21 != 0)
            throw new NotSupportedException("Qt native child Widgets only support translation.");
        var bounds = p.Bounds.shift(new Offset(t.Dx, t.Dy));
        var clip = p.Clip is { } c ? bounds.intersect(c) : bounds;
        NativeRect Convert(Rect r, bool inward)
        {
            foreach (var value in new[] { r.left, r.top, r.right, r.bottom })
                if (!double.IsFinite(value) || Math.Abs(value) > 1e6)
                    throw new NotSupportedException("Qt native child Widgets require finite geometry within 1,000,000 logical pixels.");
            if (quick) return new(r.left, r.top, Math.Max(0, r.width), Math.Max(0, r.height));
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
            ++_compositionFrame, descriptor.ResizeTargetGeneration, descriptor.DeviceScaleX, descriptor.DeviceScaleY);
        var plan = PlatformCompositionPlanner.Build(commands, token, _coordinator!,
            IsQuick ? PlatformViewComposition.InterleavedComposition : PlatformViewComposition.NativeOverlay,
            HasEffects ? QuickEffects : null);
        try
        {
            if (!IsQuick && plan.HasNativeContent && plan.Parts.OfType<PlatformShieldSegment>().Any())
                throw new NotSupportedException("Qt NativeOverlay does not support input shields; interleaving is required.");
            var placements = plan.Parts.OfType<PlatformNativeSegment>().Select(p => p.Placement).ToArray();
            var native = placements.Select(p => Translate(p, _instances[p.Handle].Id, IsQuick)).ToArray();
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
        if (!ReferenceEquals(_quickCacheOwner, gpu)) { _quickRasters.Clear(); _quickCacheOwner = gpu; }
        _nextQuickRasters = [];
        var scope = SkiaPlatformRasterContent.CacheScope.From(plan.Token, width, height, renderer.PlatformBackgroundColor);
        var parts=new List<QtQuickNative.Part>();
        var rasterIndex=0;
        var viewport=new NativeRect(0,0,width/descriptor.DeviceScaleX,height/descriptor.DeviceScaleY);
        foreach(var part in plan.Parts)
        {
            switch(part)
            {
                case PlatformRasterSegment raster:
                {
                    var bounds = rasterIndex == 0 ? new SKRectI(0, 0, width, height) : SkiaPlatformRasterContent.Coverage(raster.Commands, width, height);
                    if (bounds.Width <= 0 || bounds.Height <= 0) continue;
                    var slice = new SkiaPlatformRasterContent.Slice(raster.Commands, bounds);
                    if (!_quickRasters.TryGetValue(raster.PaintOrder, out var old) ||
                        !SkiaPlatformRasterContent.CanReuse(old.Scope, old.Slice, scope, slice) ||
                        !gpu.TryReuse(rasterIndex, old.Identity, bounds.Width, bounds.Height))
                    {
                        var target=gpu.Canvas(rasterIndex, bounds.Width, bounds.Height);
                        if(rasterIndex==0)target.DrawColor(renderer.PlatformBackgroundColor);
                        target.Save(); target.Translate(-bounds.Left, -bounds.Top);
                        renderer.DrawPlatformRasterSegment(target,raster.Commands,width,height);
                        target.Restore();
                    }
                    var logicalBounds = new NativeRect(bounds.Left / descriptor.DeviceScaleX, bounds.Top / descriptor.DeviceScaleY,
                        bounds.Width / descriptor.DeviceScaleX, bounds.Height / descriptor.DeviceScaleY);
                    _nextQuickRasters.Add(raster.PaintOrder, new(scope, slice, gpu.Identity(rasterIndex)));
                    parts.Add(new() { Size=96,Kind=0,Id=gpu.Identity(rasterIndex),Image=gpu.Image(rasterIndex++),PixelWidth=(uint)bounds.Width,PixelHeight=(uint)bounds.Height,Bounds=logicalBounds,Clip=viewport });
                    break;
                }
                case PlatformNativeSegment native:
                    var placement=Translate(native.Placement,_instances[native.Placement.Handle].Id, quick: true);
                    parts.Add(new() { Size=96,Kind=1,Id=placement.Id,Bounds=placement.Bounds,Clip=placement.Visible!=0?placement.Clip:default });
                    break;
                case PlatformBackdropSegment effect:
                    if (effect.SigmaX != effect.SigmaY) throw new NotSupportedException("Qt Quick backdrop requires isotropic Gaussian blur.");
                    var output = effect.Bounds;
                    if (output.isEmpty) break;
                    var sample = effect.SampleBounds;
                    parts.Add(new() { Size=96, Kind=4, Id=BitConverter.DoubleToUInt64Bits(effect.SigmaX),
                        Image=BitConverter.DoubleToUInt64Bits(effect.Style?.Saturation ?? 1),
                        Bounds=new(output.left,output.top,output.width,output.height),
                        Clip=new(sample.left,sample.top,sample.width,sample.height) });
                    break;
                case PlatformShieldSegment shield:
                    var t=shield.Shield.Transform;
                    if(!t.IsAxisAligned)throw new NotSupportedException("Qt Quick input shields require axis-aligned bounds.");
                    var a=t.Map(shield.Shield.Bounds.topLeft);var b=t.Map(shield.Shield.Bounds.bottomRight);
                    var shieldBounds=new Rect(a.dx,a.dy,b.dx,b.dy);
                    var clip=shield.Shield.Clip is {} c?shieldBounds.intersect(c):shieldBounds;
                    parts.Add(new() { Size=96,Kind=2,Bounds=new(shieldBounds.left,shieldBounds.top,shieldBounds.width,shieldBounds.height),
                        Clip=new(clip.left,clip.top,Math.Max(0,clip.width),Math.Max(0,clip.height)) });
                    break;
                default:
                    throw new NotSupportedException($"Unsupported Qt Quick composition part {part.GetType().Name}.");
            }
        }
        surface.QuickParts=parts.ToArray();
    }
    internal void FinishFrame(bool presented)
    {
        if (!presented || _pending is null || _closed) { FinishFrameCore(presented); return; }
        var plan = _pending;
        _session ??= new(plan.Token.OwnerViewId);
        if (_session.CommitRetiredFrame(plan, () => { FinishFrameCore(presented); return true; })) CommittedFrames++;
    }
    private void FinishFrameCore(bool presented)
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
            if (IsQuick) _quickRasters = _nextQuickRasters;
        }
        finally { _committing = false; CancelPending(); }
    }
    internal void CancelPending()
    {
        _reservation?.Dispose(); _reservation = null;
        _pending?.Dispose(); _pending = null;
        _next = []; _nextPlacements = [];
        _nextQuickRasters = [];
    }
    // Called from native closed only after pending post callbacks and QWidgets die.
    internal void NativeClosed() { _closed = true; foreach (var instance in _instances.Values) instance.Web?.Close(true); CancelPending(); _session?.DisposeAsync().GetAwaiter().GetResult(); }
    public void Dispose()
    {
        NativeClosed();
        if (_focusContext.IsAllocated) _focusContext.Free();
    }
    [LibraryImport("doroti_qt_host", EntryPoint = "doroti_qt_get_platform_views")]
    private static partial int GetApi(nint view, uint version, uint size, out ulong owner, out Api api);
}
