using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;
using Doroti.Hosting;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Host.Web;

/// <summary>One coordinator per browser owner; DOM operations use the existing Worker control mailbox.</summary>
[SupportedOSPlatform("browser")]
internal sealed partial class BrowserPlatformViewHost : IPlatformViewDispatcher, IDisposable
{
    private static readonly Dictionary<int, BrowserPlatformViewHost> Hosts = [];
    private static readonly List<Task> Retirements = [];
    private readonly int _hostId;
    private readonly PlatformEffectSupport _effects;
    private readonly SynchronizationContext _context = SynchronizationContext.Current
        ?? throw new InvalidOperationException("Browser platform views require the JS owner context.");
    private readonly Dictionary<PlatformViewHandle, Instance> _instances = [];
    private PlatformCompositionPlan? _pending;
    private bool _closed;
    private long _frame;
    private bool _composed;
    private bool _nextComposed;
    private sealed record CachedRaster(SkiaPlatformRasterContent.CacheScope Scope, SkiaPlatformRasterContent.Slice Slice);
    private Dictionary<int, CachedRaster> _rasters = [], _nextRasters = [];
    internal static IEnumerable<IPlatformViewFactory> Factories => [new Factory(null)];
    internal PlatformViewCoordinator Coordinator { get; }
    internal static readonly PlatformEffectSupport Effects = new(true, 4, 16, Saturation: true,
        Reason: "Browser CSS isotropic backdrop sigma 0–16; actual sampling depends on browser and embedding policy.");
    internal BrowserPlatformViewHost(ulong owner, int hostId, bool enabled, bool backdrop)
    {
        _hostId = hostId;
        _effects = backdrop ? Effects : PlatformEffectSupport.Unsupported;
        Coordinator = new(owner, "browser/dom", new(enabled ? [new Factory(this)] : []), this);
        Hosts.Add(hostId, this);
    }
    internal static void Dispatch(int hostId, string json)
    {
        if (!Hosts.TryGetValue(hostId, out var host) || host._closed) return;
        using var document = JsonDocument.Parse(json);
        var e = document.RootElement;
        var identity = e.GetProperty("identity");
        var handle = new PlatformViewHandle(ulong.Parse(identity.GetProperty("owner").GetString()!),
            long.Parse(identity.GetProperty("id").GetString()!), long.Parse(identity.GetProperty("generation").GetString()!));
        if (!host._instances.TryGetValue(handle, out var instance)) return;
        if (e.TryGetProperty("focused", out _)) { instance.Focused(handle); return; }
        instance.Notify(new(handle, e.GetProperty("NavigationId").GetInt64(), e.GetProperty("DocumentGeneration").GetInt64(),
            (WebViewEventKind)e.GetProperty("Kind").GetInt32(), Text("Url"), Text("Error"), Text("MessageName"), Text("MessageJson"),
            e.TryGetProperty("MessageRequestId", out var id) ? id.GetInt64() : 0));
        string? Text(string key) => e.TryGetProperty(key, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;
    }
    public ValueTask InvokeAsync(Func<ValueTask> action)
    {
        if (SynchronizationContext.Current == _context) return action();
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _context.Post(async _ => { try { await action(); completion.SetResult(); } catch (Exception error) { completion.SetException(error); } }, null);
        return new(completion.Task);
    }
    private static object Identity(PlatformViewHandle h) => new { owner = h.OwnerViewId.ToString(), id = h.InstanceId.ToString(), generation = h.InstanceGeneration.ToString() };
    private async Task<string> Request(object payload)
    {
        string json;
        try { json = await RequestAsync(_hostId, JsonSerializer.Serialize(payload)); }
        catch (JSException transportError) { throw new WebViewException(WebViewError.ProcessFailed, "DOM control transport failed: " + transportError.Message); }
        using var result = JsonDocument.Parse(json);
        if (result.RootElement.TryGetProperty("error", out var error))
            throw new WebViewException((WebViewError)error.GetInt32(), result.RootElement.GetProperty("message").GetString()!);
        return json;
    }
    private async ValueTask Cleanup(object payload)
    {
        try { await Request(payload); }
        catch (WebViewException error) when (error.Code == WebViewError.Closed) { /* Main owner already reclaimed its DOM. */ }
    }
    internal static async Task DrainAsync()
    {
        Task[] pending;
        lock (Retirements) { pending = Retirements.ToArray(); Retirements.Clear(); }
        await Task.WhenAll(pending);
    }
    private sealed class Factory(BrowserPlatformViewHost? host) : IPlatformViewFactory
    {
        public string ViewType => "doroti/webview";
        public PlatformViewSupport QuerySupport(PlatformViewRequest request) => new("browser/dom", "iframe", ViewType,
            host?._closed != true && request.ViewType == ViewType && request.Composition == PlatformViewComposition.InterleavedComposition &&
            (request.Effects & ~PlatformViewEffects.RectClip) == 0, PlatformViewComposition.InterleavedComposition,
            PlatformViewEffects.RectClip, WebViewCommands: true, NativeBackdropBlur: host?._effects.LiveSourceSampling == true,
            Capabilities: new(PlatformViewRepresentation.Dom, PlatformViewTransport.CpuUpload, PlatformViewInputPolicy.DirectNative,
                host?._effects ?? PlatformEffectSupport.Unsupported),
            Reason: "Stable iframe + bounded worker raster upload. BrowserDefault profile only; no cross-origin JS/history, private profile, physical atomicity or native GestureArena.");
        public async ValueTask<IPlatformViewInstance> CreateAsync(PlatformViewHandle handle, ReadOnlyMemory<byte> parameters,
            Action<PlatformViewHandle> focused, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var owner = host ?? Hosts.Values.Single(h => h.Coordinator.OwnerViewId == handle.OwnerViewId);
            var text = Encoding.UTF8.GetString(parameters.Span);
            var options = text.StartsWith(WebViewOptions.Prefix, StringComparison.Ordinal)
                ? JsonSerializer.Deserialize(text[WebViewOptions.Prefix.Length..], WebViewJsonContext.Default.WebViewOptions)!
                : new WebViewOptions(Html: text, Profile: WebViewProfile.BrowserDefault);
            options.Validate();
            await owner.Request(new { action = "create", identity = Identity(handle), viewType = ViewType, options });
            var instance = new Instance(owner, handle, focused);
            owner._instances.Add(handle, instance);
            return instance; // coordinator disposes a late successful creation after cancellation/close
        }
    }
    private sealed class Instance(BrowserPlatformViewHost host, PlatformViewHandle handle, Action<PlatformViewHandle> focused)
        : IPlatformViewInstance, IPlatformWebViewInstance
    {
        private int _requests;
        private long _sequence;
        private readonly CancellationTokenSource _lifetime = new();
        internal Action<PlatformViewHandle> Focused => focused;
        public event Action<WebViewEvent>? WebViewChanged;
        internal void Notify(WebViewEvent value) => WebViewChanged?.Invoke(value);
        public async Task<WebViewResult> ExecuteAsync(WebViewCommand command, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!Enum.IsDefined(command.Operation) || Encoding.UTF8.GetByteCount(command.Text ?? "") > 2 * 1024 * 1024)
                throw new WebViewException(WebViewError.InvalidRequest, "Invalid operation or command larger than 2 MiB.");
            if (_lifetime.IsCancellationRequested) throw new WebViewException(WebViewError.Closed, "WebView is closed.");
            if (++_requests > 32) { _requests--; throw new WebViewException(WebViewError.Busy, "WebView has 32 pending requests."); }
            var requestId = ++_sequence;
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _lifetime.Token);
            try
            {
                var result = await host.Request(new { action = "command", identity = Identity(handle), command })
                    .WaitAsync(TimeSpan.FromSeconds(30), linked.Token);
                return (JsonSerializer.Deserialize<WebViewResult>(result) ?? throw new InvalidDataException("Missing WebView result.")) with { RequestId = requestId };
            }
            catch (TimeoutException) { throw new WebViewException(WebViewError.JavaScript, "WebView command exceeded 30 seconds."); }
            catch (OperationCanceledException) when (_lifetime.IsCancellationRequested)
            { throw new WebViewException(WebViewError.Closed, "WebView closed during a command."); }
            finally { _requests--; }
        }
        public ValueTask ApplyAsync(PlatformViewPlacement placement) =>
            throw new NotSupportedException("Browser attachment is committed by the owning scene frame, not an independent placement call.");
        public ValueTask DetachAsync() => host.Cleanup(new { action = "hide", identity = Identity(handle) });
        public ValueTask DisableInputAsync() => host.Cleanup(new { action = "disable", identity = Identity(handle) });
        public async ValueTask SetFocusAsync(bool focused) => await host.Request(new { action = "focus", identity = Identity(handle), focused });
        public async ValueTask DisposeAsync()
        {
            _lifetime.Cancel();
            try { await host.Cleanup(new { action = "remove", identity = Identity(handle) }); }
            finally { host._instances.Remove(handle); WebViewChanged = null; }
        }
    }
    internal unsafe void Draw(SkiaSceneRenderer renderer, SKCanvas canvas, IReadOnlyList<SceneCommand> commands,
        DorotiFrameDescriptor descriptor, int width, int height)
    {
        if (_pending is not null) throw new InvalidOperationException("An unacknowledged DOM frame is still live.");
        var token = new PlatformCompositionToken(descriptor.ViewId, descriptor.MetricsGeneration, ++_frame,
            descriptor.ResizeTargetGeneration, descriptor.DeviceScaleX, descriptor.DeviceScaleY);
        var plan = PlatformCompositionPlanner.Build(commands, token, Coordinator, effects: _effects);
        _nextRasters = [];
        var scope = SkiaPlatformRasterContent.CacheScope.From(token, width, height, renderer.PlatformBackgroundColor);
        try
        {
            var views = new List<object>(); var shields = new List<object>(); var effects = new List<object>();
            var mixed = plan.Parts.Any(p => p is not PlatformRasterSegment);
            var needsCommit = mixed || _composed;
            _nextComposed = mixed;
            long bytes = 0;
            foreach (var part in plan.Parts)
            {
                switch (part)
                {
                    case PlatformRasterSegment raster when !mixed:
                        renderer.DrawPlatformRasterSegment(canvas, raster.Commands, width, height); break;
                    case PlatformRasterSegment raster:
                    {
                        var bounds = raster.PaintOrder == 0 ? new SKRectI(0, 0, width, height) : SkiaPlatformRasterContent.Coverage(raster.Commands, width, height);
                        if (bounds.Width <= 0 || bounds.Height <= 0) break;
                        bytes += (long)bounds.Width * bounds.Height * 4;
                        if (bytes > 64 * 1024 * 1024) throw new NotSupportedException("Browser composition exceeds the 64 MiB raster frame budget.");
                        var slice = new SkiaPlatformRasterContent.Slice(raster.Commands, bounds);
                        _nextRasters.Add(raster.PaintOrder, new(scope, slice));
                        if (_rasters.TryGetValue(raster.PaintOrder, out var prior) && SkiaPlatformRasterContent.CanReuse(prior.Scope, prior.Slice, scope, slice))
                        {
                            StageRaster(raster.PaintOrder, bounds.Left / descriptor.DeviceScaleX, bounds.Top / descriptor.DeviceScaleY,
                                bounds.Width / descriptor.DeviceScaleX, bounds.Height / descriptor.DeviceScaleY, bounds.Width, bounds.Height, []);
                            break;
                        }
                        using var bitmap = new SKBitmap(new SKImageInfo(bounds.Width, bounds.Height, SKColorType.Rgba8888, SKAlphaType.Premul));
                        using var target = new SKCanvas(bitmap);
                        target.Clear(raster.PaintOrder == 0 ? renderer.PlatformBackgroundColor : SKColors.Transparent);
                        target.Translate(-bounds.Left, -bounds.Top);
                        renderer.DrawPlatformRasterSegment(target, raster.Commands, width, height);
                        target.Flush();
                        var pixels = new byte[checked(bounds.Width * bounds.Height * 4)];
                        using var image = SKImage.FromBitmap(bitmap);
                        fixed (byte* destination = pixels)
                            if (!image.ReadPixels(new SKImageInfo(bounds.Width, bounds.Height, SKColorType.Rgba8888, SKAlphaType.Unpremul),
                                (nint)destination, bounds.Width * 4, 0, 0))
                                throw new InvalidOperationException("Browser raster RGBA conversion failed.");
                        StageRaster(raster.PaintOrder, bounds.Left / descriptor.DeviceScaleX, bounds.Top / descriptor.DeviceScaleY,
                            bounds.Width / descriptor.DeviceScaleX, bounds.Height / descriptor.DeviceScaleY, bounds.Width, bounds.Height, pixels);
                        break;
                    }
                    case PlatformNativeSegment native:
                        var p = native.Placement;
                        views.Add(new { identity = Identity(p.Handle), bounds = Bounds(Map(p.Bounds, p.Transform)), clip = p.Clip is { } c ? Bounds(c) : null,
                            visible = p.Visible, order = p.PaintOrder }); break;
                    case PlatformShieldSegment shield:
                        var s = shield.Shield;
                        shields.Add(new { id = s.PaintOrder.ToString(), bounds = Bounds(Map(s.Bounds, s.Transform)), clip = s.Clip is { } sc ? Bounds(sc) : null,
                            order = s.PaintOrder, debug = s.Debug }); break;
                    case PlatformBackdropSegment effect:
                        if (effect.SigmaX != effect.SigmaY) throw new NotSupportedException("CSS backdrop requires isotropic blur.");
                        effects.Add(new { id = effect.PaintOrder.ToString(), bounds = Bounds(effect.Bounds), order = effect.PaintOrder,
                            strength = effect.SigmaX / 16, tint = "#00000000", saturation = effect.Style?.Saturation ?? 1 }); break;
                }
            }
            if (!needsCommit) { plan.Dispose(); return; }
            StageFrame(JsonSerializer.Serialize(new { version = 2, owner = token.OwnerViewId.ToString(), epoch = token.ViewEpoch,
                surfaceGeneration = token.SurfaceGeneration, frame = token.FrameNumber, views, shields, effects }));
            _pending = plan;
        }
        catch { plan.Dispose(); throw; }
    }
    private static object Bounds(Rect r) => new { left = r.left, top = r.top, width = Math.Max(0, r.width), height = Math.Max(0, r.height) };
    private static Rect Map(Rect rect, PlatformViewTransform transform)
    {
        if (!transform.IsAxisAligned) throw new NotSupportedException("Browser platform views require axis-aligned placements.");
        var a = transform.Map(rect.topLeft); var b = transform.Map(rect.bottomRight); return new(a.dx, a.dy, b.dx, b.dy);
    }
    internal void Complete(bool accepted = false)
    {
        if (_pending is not null && accepted && Coordinator.RecordPlacementReceipt(_pending))
        { _composed = _nextComposed; _rasters = _nextRasters; }
        _nextRasters = [];
        _pending?.Dispose(); _pending = null;
    }
    public void Dispose()
    {
        if (_closed) return;
        _closed = true; Complete(); _rasters.Clear(); Hosts.Remove(_hostId); Coordinator.Dispose();
        lock (Retirements) Retirements.Add(Coordinator.DisposalCompletion);
    }
    [JSImport("platformViewRequest", "doroti.web")]
    private static partial Task<string> RequestAsync(int hostId, string json);
    [JSImport("stagePlatformFrame", "doroti.web")]
    private static partial void StageFrame(string json);
    [JSImport("stagePlatformRaster", "doroti.web")]
    private static partial void StageRaster(int order, double left, double top, double width, double height, int pixelWidth, int pixelHeight, byte[] pixels);
}
