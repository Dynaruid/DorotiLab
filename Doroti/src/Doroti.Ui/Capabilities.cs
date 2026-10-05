namespace Doroti.Ui;

/// <summary>The stable capability identifiers shared with host capability evidence.</summary>
public static class DorotiCapabilityIds
{
    public const string Windowing = "windowing.host";
    public const string WindowService = "windowing.service";
    public const string FramePresentation = "view.frame-presentation";
    public const string PlatformMenuBar = "windowing.menu-bar";
    public const string PlatformMenu = "windowing.menu";
    public const string WindowLifecycle = "window.lifecycle";
    public const string WindowTitlebar = "window.titlebar";
    public const string ViewLifecycleMetrics = "view.lifecycle-metrics";
    public const string ViewFrameDispatch = "view.frame-dispatch";
    public const string InputEvents = "input.events";
    public const string InputCursor = "input.cursor";
    public const string TextInput = "text.input";
    public const string UrlLauncher = "platform.url-launcher";
    public const string FilePicker = "platform.file-picker";
    public const string OsDragDrop = "platform.os-drag-drop";
    public const string OsDragSource = "platform.os-drag-source";
    public const string ApplicationNavigation = "application.navigation";
    public const string PlatformFeedback = "platform.feedback";
    public const string PlatformServices = "platform.services";
    public const string PlatformEnvironment = "platform.environment";
    public const string PlatformMessaging = "platform.messaging";
    public const string ApplicationResources = "application.resources";
    public const string PlatformPlugins = "platform.plugins";
    public const string ManagedPlugins = "plugins.managed";
    public const string PlatformPluginEvents = "platform.plugin-events";
    public const string PlatformViews = "platform.views";
    public const string DartPerformanceMode = "runtime.dart-performance-mode";
    public const string GraphicsScene = "graphics.scene";
    public const string GraphicsSceneSnapshot = "graphics.scene-snapshot";
    public const string GraphicsFont = "graphics.font";
    public const string GraphicsText = "graphics.text";
    public const string GraphicsTexture = "graphics.texture";
    public const string GraphicsImage = "graphics.image";
    public const string AccessibilitySemantics = "accessibility.semantics";
    public const string FrameworkViewAttachment = "framework.view-attachment";

    public static IReadOnlyList<string> RequiredDesktop { get; } =
    [
        WindowLifecycle,
        ViewLifecycleMetrics,
        ViewFrameDispatch,
        InputEvents,
        TextInput,
        PlatformServices,
        PlatformEnvironment,
        PlatformMessaging,
        GraphicsScene,
        GraphicsText,
        GraphicsImage,
        AccessibilitySemantics,
    ];
}

/// <summary>Typed execution boundary for Doroti UI PlatformDispatcher performance requests.</summary>
public interface IDartPerformanceModeCapability
{
    void Request(DartPerformanceMode mode);
}

public readonly record struct DorotiUiInvocation(string ElementId)
{
    public static DorotiUiInvocation Managed(string elementId) => new(elementId);
}

/// <summary>A fail-closed error for a missing or lifetime-invalid host capability.</summary>
public sealed class DorotiCapabilityException : InvalidOperationException
{
    public DorotiCapabilityException(
        string capabilityId,
        ulong? viewId,
        DorotiUiInvocation invocation,
        string reason,
        string targetIdentity = "<unspecified>"
    )
        : base(
            $"Flutter capability '{capabilityId}' is unavailable for view "
                + $"{(viewId is null ? "<unregistered>" : viewId.Value)} "
                + $"({invocation.ElementId}) on target '{targetIdentity}': {reason}"
        )
    {
        CapabilityId = capabilityId;
        ViewId = viewId;
        ElementId = invocation.ElementId;
        TargetIdentity = targetIdentity;
    }

    public string CapabilityId { get; }

    public ulong? ViewId { get; }

    public string ElementId { get; }

    public string TargetIdentity { get; }
}

/// <summary>A per-view registry. Capability instances are never process-global.</summary>
public sealed class DorotiViewCapabilities : IDisposable
{
    private sealed class CapabilityRegistrar(DorotiViewCapabilities capabilities) : IDorotiCapabilityRegistrar
    {
        public void Register<TCapability>(string id, TCapability capability, DorotiCapabilityOwnership ownership)
            where TCapability : class => capabilities.Register(id, capability, ownership);
    }

    private readonly object _gate = new();
    private readonly Dictionary<string, object> _values = new(StringComparer.Ordinal);
    private readonly Dictionary<object, DorotiCapabilityOwnership> _ownership = new(ReferenceEqualityComparer.Instance);
    private readonly DorotiCapabilityLifetime? _lifetime;
    private int _activeCalls;
    private TaskCompletionSource? _invocationsDrained;
    private bool _quiescing;
    private bool _sealed;
    private bool _disposed;

    public DorotiViewCapabilities(string targetIdentity = "<unspecified>", DorotiCapabilityLifetime? lifetime = null)
    {
        TargetIdentity = string.IsNullOrWhiteSpace(targetIdentity) ? "<unspecified>" : targetIdentity;
        _lifetime = lifetime;
    }

    internal bool HasApplicationLifetime => _lifetime is not null;
    public string TargetIdentity { get; }
    public IDorotiCapabilityRegistrar Registrar => new CapabilityRegistrar(this);

    public IReadOnlyCollection<string> RegisteredIds
    {
        get { lock (_gate) return _values.Keys.Order(StringComparer.Ordinal).ToArray(); }
    }

    public DorotiViewCapabilities Register<TCapability>(string id, TCapability capability)
        where TCapability : class => Register(id, capability, DorotiCapabilityOwnership.Owned);

    public DorotiViewCapabilities Register<TCapability>(string id, TCapability capability, DorotiCapabilityOwnership ownership)
        where TCapability : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(capability);
        if (!Enum.IsDefined(ownership)) throw new ArgumentOutOfRangeException(nameof(ownership));
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (_sealed) throw new InvalidOperationException("A registered view capability set is immutable.");
            if (_values.ContainsKey(id)) throw new InvalidOperationException($"Capability '{id}' was registered more than once.");
            if (_ownership.TryGetValue(capability, out var previous) && previous != ownership)
                throw new InvalidOperationException("Conflicting capability ownership in the same view.");
            if (ownership == DorotiCapabilityOwnership.Borrowed && _lifetime is null)
                throw new InvalidOperationException("Borrowed capabilities require an application lifetime.");
            _lifetime?.Register(this, capability, ownership);
            _ownership[capability] = ownership;
            _values.Add(id, capability);
        }
        return this;
    }

    public TCapability Require<TCapability>(ulong viewId, string id, DorotiUiInvocation invocation)
        where TCapability : class
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            ObjectDisposedException.ThrowIf(_quiescing, this);
            if (!_values.TryGetValue(id, out var value))
                throw new DorotiCapabilityException(id, viewId, invocation, "the active host did not register it", TargetIdentity);
            if (value is not TCapability typed)
                throw new DorotiCapabilityException(id, viewId, invocation,
                    $"the registered implementation has type {value.GetType().FullName}, not {typeof(TCapability).FullName}", TargetIdentity);
            return typed;
        }
    }

    public DorotiCapabilityLease<TCapability> Acquire<TCapability>(ulong viewId, string id, DorotiUiInvocation invocation)
        where TCapability : class
    {
        lock (_gate)
        {
            var value = Require<TCapability>(viewId, id, invocation);
            var releaseLifetime = _lifetime?.Acquire(value);
            if (_activeCalls++ == 0)
                _invocationsDrained = new(TaskCreationOptions.RunContinuationsAsynchronously);
            return new(value, () =>
            {
                lock (_gate)
                {
                    releaseLifetime?.Invoke();
                    if (--_activeCalls == 0) _invocationsDrained?.TrySetResult();
                }
            });
        }
    }

    internal void BeginClose() { lock (_gate) _quiescing = true; }
    internal Task WaitForInvocationDrainAsync()
    {
        lock (_gate) return _activeCalls == 0 ? Task.CompletedTask : _invocationsDrained!.Task;
    }

    internal void Seal()
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _sealed = true;
        }
    }

    public void Dispose()
    {
        KeyValuePair<object, DorotiCapabilityOwnership>[] registrations;
        lock (_gate)
        {
            if (_disposed) return;
            if (_activeCalls != 0) throw new InvalidOperationException("Drain active capability calls before disposing the view.");
            _lifetime?.ValidateRelease(this, _ownership.Keys);
            _disposed = true;
            registrations = _ownership.ToArray();
            _values.Clear();
            _ownership.Clear();
            foreach (var registration in registrations)
                _lifetime?.Release(this, registration.Key, registration.Value);
        }
        Doroti.Runtime.DorotiCleanup.Run(registrations
            .Where(entry => entry.Value == DorotiCapabilityOwnership.Owned)
            .Select(entry => entry.Key).OfType<IDisposable>()
            .Select<IDisposable, Action>(item => item.Dispose).ToArray());
    }
}
