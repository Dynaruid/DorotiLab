namespace Doroti.Flutter.Ui;

/// <summary>The stable capability identifiers shared with migration/flutter-avalonia/capability-map.json.</summary>
public static class FlutterCapabilityIds
{
    public const string WindowLifecycle = "window.lifecycle";
    public const string ViewLifecycleMetrics = "view.lifecycle-metrics";
    public const string ViewFrameDispatch = "view.frame-dispatch";
    public const string InputEvents = "input.events";
    public const string InputCursor = "input.cursor";
    public const string TextInput = "text.input";
    public const string PlatformServices = "platform.services";
    public const string PlatformEnvironment = "platform.environment";
    public const string PlatformMessaging = "platform.messaging";
    public const string ApplicationResources = "application.resources";
    public const string PlatformPlugins = "platform.plugins";
    public const string DartPerformanceMode = "runtime.dart-performance-mode";
    public const string GraphicsScene = "graphics.scene";
    public const string GraphicsText = "graphics.text";
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

/// <summary>Typed execution boundary for dart:ui PlatformDispatcher performance requests.</summary>
public interface IDartPerformanceModeCapability
{
    void Request(DartPerformanceMode mode);
}

public readonly record struct DartSourceSpan(string Source, int Offset, int Length)
{
    public static DartSourceSpan Unknown { get; } = new("<managed-bootstrap>", 0, 0);

    public override string ToString() => $"{Source}:{Offset}+{Length}";
}

public readonly record struct DartUiInvocation(string ElementId, DartSourceSpan SourceSpan)
{
    public static DartUiInvocation Managed(string elementId) => new(elementId, DartSourceSpan.Unknown);
}

/// <summary>A fail-closed error for a missing or lifetime-invalid host capability.</summary>
public sealed class FlutterCapabilityException : InvalidOperationException
{
    public FlutterCapabilityException(
        string capabilityId,
        ulong? viewId,
        DartUiInvocation invocation,
        string reason,
        string targetIdentity = "<unspecified>")
        : base($"Flutter capability '{capabilityId}' is unavailable for view " +
            $"{(viewId is null ? "<unregistered>" : viewId.Value)} at {invocation.SourceSpan} " +
            $"({invocation.ElementId}) on target '{targetIdentity}': {reason}")
    {
        CapabilityId = capabilityId;
        ViewId = viewId;
        ElementId = invocation.ElementId;
        SourceSpan = invocation.SourceSpan;
        TargetIdentity = targetIdentity;
    }

    public string CapabilityId { get; }

    public ulong? ViewId { get; }

    public string ElementId { get; }

    public DartSourceSpan SourceSpan { get; }

    public string TargetIdentity { get; }
}

/// <summary>A per-view registry. Capability instances are never process-global.</summary>
public sealed class FlutterViewCapabilities : IDisposable
{
    private readonly Dictionary<string, object> _values = new(StringComparer.Ordinal);
    private bool _sealed;
    private bool _disposed;

    public FlutterViewCapabilities(string targetIdentity = "<unspecified>") =>
        TargetIdentity = string.IsNullOrWhiteSpace(targetIdentity) ? "<unspecified>" : targetIdentity;

    public string TargetIdentity { get; }

    public IReadOnlyCollection<string> RegisteredIds => _values.Keys.Order(StringComparer.Ordinal).ToArray();

    public FlutterViewCapabilities Register<TCapability>(string id, TCapability capability)
        where TCapability : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(capability);
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_sealed)
        {
            throw new InvalidOperationException("A registered Flutter view capability set is immutable.");
        }
        if (!_values.TryAdd(id, capability))
        {
            throw new InvalidOperationException($"Flutter capability '{id}' was registered more than once.");
        }
        return this;
    }

    public TCapability Require<TCapability>(ulong viewId, string id, DartUiInvocation invocation)
        where TCapability : class
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!_values.TryGetValue(id, out var value))
        {
            throw new FlutterCapabilityException(id, viewId, invocation, "the active host did not register it", TargetIdentity);
        }
        if (value is not TCapability typed)
        {
            throw new FlutterCapabilityException(id, viewId, invocation,
                $"the registered implementation has type {value.GetType().FullName}, not {typeof(TCapability).FullName}",
                TargetIdentity);
        }
        return typed;
    }

    internal void Seal() => _sealed = true;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        foreach (var item in _values.Values.Distinct(ReferenceEqualityComparer.Instance).OfType<IDisposable>())
        {
            item.Dispose();
        }
        _values.Clear();
    }
}
