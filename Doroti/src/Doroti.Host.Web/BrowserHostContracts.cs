using System.Text.Json;
using System.Runtime.Versioning;
using Doroti.Flutter.Hosting;
using Doroti.Flutter.Ui;

namespace Doroti.Host.Web;

public sealed record BrowserGpuIdentity(
    string Api,
    string Vendor,
    string Renderer,
    bool Hardware,
    bool SoftwareFallbackUsed);

public sealed record BrowserHostSnapshot(
    string CanvasId,
    double LogicalWidth,
    double LogicalHeight,
    double DevicePixelRatio,
    bool Visible,
    bool Focused,
    string LanguageTag,
    string Brightness,
    long Generation,
    long SurfaceGeneration,
    BrowserGpuIdentity Gpu);

public sealed record BrowserJavaScriptPluginDescriptor(
    string Id,
    string Channel,
    string AbiVersion,
    string ModuleUrl,
    string ExportName);

/// <summary>Browser-only JavaScript boundary. DOM and WebGL types never leave this project.</summary>
[SupportedOSPlatform("browser")]
internal static partial class BrowserInterop
{
    private const string Module = "doroti.web";

    [System.Runtime.InteropServices.JavaScript.JSImport("createHost", Module)]
    internal static partial string CreateHost(
        int hostId,
        string canvasId,
        double logicalWidth,
        double logicalHeight);

    [System.Runtime.InteropServices.JavaScript.JSImport("showHost", Module)]
    internal static partial string ShowHost(int hostId);

    [System.Runtime.InteropServices.JavaScript.JSImport("resizeHost", Module)]
    internal static partial string ResizeHost(int hostId, double logicalWidth, double logicalHeight);

    [System.Runtime.InteropServices.JavaScript.JSImport("requestFrame", Module)]
    internal static partial void RequestFrame(int hostId, int callbackId);

    [System.Runtime.InteropServices.JavaScript.JSImport("closeHost", Module)]
    internal static partial void CloseHost(int hostId);

    [System.Runtime.InteropServices.JavaScript.JSImport("resolveResourceUrl", Module)]
    internal static partial string ResolveResourceUrl(string relativeUrl);

    [System.Runtime.InteropServices.JavaScript.JSImport("invokePlugin", Module)]
    [return: System.Runtime.InteropServices.JavaScript.JSMarshalAs<System.Runtime.InteropServices.JavaScript.JSType.Promise<System.Runtime.InteropServices.JavaScript.JSType.String>>]
    internal static partial Task<string> InvokePluginAsync(
        string moduleUrl,
        string exportName,
        string channel,
        string codec,
        string payloadBase64);

    [System.Runtime.InteropServices.JavaScript.JSExport]
    internal static void DispatchAnimationFrame(int hostId, int callbackId, double timestampMilliseconds) =>
        BrowserHostAdapter.DispatchAnimationFrame(hostId, callbackId, timestampMilliseconds);

    [System.Runtime.InteropServices.JavaScript.JSExport]
    internal static void DispatchSnapshot(int hostId, string json) =>
        BrowserHostAdapter.DispatchSnapshot(hostId, json);

    internal static BrowserHostSnapshot ParseSnapshot(string json) =>
        JsonSerializer.Deserialize<BrowserHostSnapshot>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        }) ?? throw new InvalidDataException("The browser host returned an empty snapshot.");
}

[SupportedOSPlatform("browser")]
public sealed class BrowserHostAdapter :
    IViewHostCapability,
    IFrameHostCapability,
    IPlatformEnvironmentHostCapability
{
    private static readonly object RegistryGate = new();
    private static readonly Dictionary<int, WeakReference<BrowserHostAdapter>> Registry = [];
    private static int _nextHostId;

    private readonly object _gate = new();
    private readonly Dictionary<int, Action<TimeSpan>> _pendingFrames = [];
    private int _nextCallbackId;
    private BrowserHostSnapshot _snapshot;
    private PlatformConfiguration _configuration;
    private bool _disposed;

    public BrowserHostAdapter(string canvasId, Size logicalSize)
    {
        if (!OperatingSystem.IsBrowser())
            throw new PlatformNotSupportedException("Doroti.Host.Web requires a browser-wasm process.");
        ArgumentException.ThrowIfNullOrWhiteSpace(canvasId);
        ArgumentNullException.ThrowIfNull(logicalSize);
        if (!logicalSize.IsFinite || logicalSize.IsEmpty)
            throw new ArgumentOutOfRangeException(nameof(logicalSize));

        HostId = Interlocked.Increment(ref _nextHostId);
        lock (RegistryGate) Registry.Add(HostId, new(this));
        try
        {
            _snapshot = Validate(BrowserInterop.ParseSnapshot(
                BrowserInterop.CreateHost(HostId, canvasId, logicalSize.width, logicalSize.height)));
            _configuration = ToConfiguration(_snapshot);
        }
        catch
        {
            lock (RegistryGate) Registry.Remove(HostId);
            throw;
        }
    }

    public int HostId { get; }
    public BrowserGpuIdentity Gpu => _snapshot.Gpu;
    public BrowserHostSnapshot Snapshot => _snapshot;
    public ViewMetrics Metrics => ToMetrics(_snapshot);
    public PlatformConfiguration Configuration => _configuration;

    public event Action<ViewMetrics>? MetricsChanged;
    public event Action<AppLifecycleState>? LifecycleChanged;
    public event Action? CloseRequested;
    public event Action? Closed;
    public event Action<PlatformConfiguration>? ConfigurationChanged;

    public string ResolveResourceUrl(string relativeUrl)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(relativeUrl);
        return BrowserInterop.ResolveResourceUrl(relativeUrl);
    }

    public void Show()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ApplySnapshot(BrowserInterop.ParseSnapshot(BrowserInterop.ShowHost(HostId)));
    }

    public void Resize(Size logicalSize)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(logicalSize);
        if (!logicalSize.IsFinite || logicalSize.IsEmpty)
            throw new ArgumentOutOfRangeException(nameof(logicalSize));
        ApplySnapshot(BrowserInterop.ParseSnapshot(
            BrowserInterop.ResizeHost(HostId, logicalSize.width, logicalSize.height)));
    }

    public void ScheduleFrame(Action<TimeSpan> callback)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(callback);
        int callbackId;
        lock (_gate)
        {
            callbackId = checked(++_nextCallbackId);
            _pendingFrames.Add(callbackId, callback);
        }
        BrowserInterop.RequestFrame(HostId, callbackId);
    }

    public void Close()
    {
        if (_disposed) return;
        CloseRequested?.Invoke();
        Dispose();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        lock (RegistryGate) Registry.Remove(HostId);
        lock (_gate) _pendingFrames.Clear();
        BrowserInterop.CloseHost(HostId);
        Closed?.Invoke();
    }

    internal static void DispatchAnimationFrame(int hostId, int callbackId, double timestampMilliseconds)
    {
        if (!TryGet(hostId, out var host)) return;
        Action<TimeSpan>? callback;
        lock (host._gate)
        {
            if (!host._pendingFrames.Remove(callbackId, out callback)) return;
        }
        callback(TimeSpan.FromMilliseconds(timestampMilliseconds));
    }

    internal static void DispatchSnapshot(int hostId, string json)
    {
        if (TryGet(hostId, out var host)) host.ApplySnapshot(BrowserInterop.ParseSnapshot(json));
    }

    private static bool TryGet(int hostId, out BrowserHostAdapter host)
    {
        lock (RegistryGate)
        {
            if (Registry.TryGetValue(hostId, out var reference) && reference.TryGetTarget(out host!)) return true;
            Registry.Remove(hostId);
            host = null!;
            return false;
        }
    }

    private void ApplySnapshot(BrowserHostSnapshot next)
    {
        next = Validate(next);
        var previous = _snapshot;
        _snapshot = next;
        if (ToMetrics(previous) != ToMetrics(next)) MetricsChanged?.Invoke(ToMetrics(next));
        var previousState = Lifecycle(previous);
        var nextState = Lifecycle(next);
        if (previousState != nextState) LifecycleChanged?.Invoke(nextState);
        var nextConfiguration = ToConfiguration(next);
        if (previous.LanguageTag != next.LanguageTag || previous.Brightness != next.Brightness)
        {
            _configuration = nextConfiguration;
            ConfigurationChanged?.Invoke(nextConfiguration);
        }
    }

    private static BrowserHostSnapshot Validate(BrowserHostSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        if (snapshot.LogicalWidth <= 0 || snapshot.LogicalHeight <= 0 ||
            !double.IsFinite(snapshot.LogicalWidth) || !double.IsFinite(snapshot.LogicalHeight) ||
            snapshot.DevicePixelRatio <= 0 || !double.IsFinite(snapshot.DevicePixelRatio))
            throw new InvalidDataException("The browser returned invalid canvas metrics.");
        if (!snapshot.Gpu.Hardware || snapshot.Gpu.SoftwareFallbackUsed || snapshot.Gpu.Api != "webgl2")
            throw new PlatformNotSupportedException(
                $"A hardware WebGL2 canvas is required; browser reported '{snapshot.Gpu.Api}/{snapshot.Gpu.Renderer}'.");
        return snapshot;
    }

    private static ViewMetrics ToMetrics(BrowserHostSnapshot snapshot) => new(
        new Size(snapshot.LogicalWidth * snapshot.DevicePixelRatio, snapshot.LogicalHeight * snapshot.DevicePixelRatio),
        snapshot.DevicePixelRatio, ViewPadding.zero, ViewPadding.zero, ViewPadding.zero,
        Lifecycle(snapshot), snapshot.Generation, snapshot.SurfaceGeneration);

    private static AppLifecycleState Lifecycle(BrowserHostSnapshot snapshot) =>
        snapshot.Visible ? (snapshot.Focused ? AppLifecycleState.resumed : AppLifecycleState.inactive) : AppLifecycleState.hidden;

    private static PlatformConfiguration ToConfiguration(BrowserHostSnapshot snapshot)
    {
        var pieces = snapshot.LanguageTag.Split('-', StringSplitOptions.RemoveEmptyEntries);
        var locale = pieces.Length switch
        {
            0 => new Locale("en", "US"),
            1 => new Locale(pieces[0]),
            2 => new Locale(pieces[0], pieces[1]),
            _ => new Locale(pieces[0], pieces[^1], pieces[1]),
        };
        return new([locale], snapshot.Brightness == "dark" ? Brightness.dark : Brightness.light,
            false, false, HostOperatingSystem.web);
    }
}

[SupportedOSPlatform("browser")]
public sealed class BrowserJavaScriptPluginHandler : IFlutterNativePluginHandler
{
    private sealed record PluginResponse(bool HasValue, string Base64);

    private readonly BrowserJavaScriptPluginDescriptor _descriptor;

    public BrowserJavaScriptPluginHandler(BrowserJavaScriptPluginDescriptor descriptor) =>
        _descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));

    public string PluginId => _descriptor.Id;
    public string AbiVersion => _descriptor.AbiVersion;

    public async ValueTask<ReadOnlyMemory<byte>?> HandleAsync(
        string channel,
        string codec,
        ReadOnlyMemory<byte>? message,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (channel != _descriptor.Channel)
            throw new InvalidOperationException($"Plugin '{PluginId}' does not own channel '{channel}'.");
        var json = await BrowserInterop.InvokePluginAsync(
            _descriptor.ModuleUrl,
            _descriptor.ExportName,
            channel,
            codec,
            message is null ? string.Empty : Convert.ToBase64String(message.Value.Span));
        cancellationToken.ThrowIfCancellationRequested();
        var response = JsonSerializer.Deserialize<PluginResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        }) ?? throw new InvalidDataException($"Plugin '{PluginId}' returned an invalid response envelope.");
        return response.HasValue ? Convert.FromBase64String(response.Base64) : null;
    }
}
