// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/foundation/memory_allocations.dart
namespace Doroti.Framework.Foundation;

public delegate void ObjectEventListener(ObjectEvent value);

public abstract record ObjectEvent(string library, string className, object instance, DateTimeOffset timestamp);
public sealed record ObjectCreated(string Library, string ClassName, object Instance, DateTimeOffset Timestamp)
    : ObjectEvent(Library, ClassName, Instance, Timestamp);
public sealed record ObjectDisposed(string Library, string ClassName, object Instance, DateTimeOffset Timestamp)
    : ObjectEvent(Library, ClassName, Instance, Timestamp);

public class MemoryAllocations
{
    private event ObjectEventListener? _listeners;

    public static MemoryAllocations instance { get; } = new FlutterMemoryAllocations();

    public bool hasListeners => _listeners is not null;

    public void addListener(ObjectEventListener listener) => _listeners += listener ?? throw new ArgumentNullException(nameof(listener));

    public void removeListener(ObjectEventListener listener) => _listeners -= listener ?? throw new ArgumentNullException(nameof(listener));

    public void dispatchObjectCreated(string library, string className, object instance)
    {
        ArgumentNullException.ThrowIfNull(instance);
        _listeners?.Invoke(new ObjectCreated(library, className, instance, DateTimeOffset.UtcNow));
    }

    public void dispatchObjectDisposed(object instance)
    {
        ArgumentNullException.ThrowIfNull(instance);
        _listeners?.Invoke(new ObjectDisposed(instance.GetType().Assembly.GetName().Name ?? "unknown", instance.GetType().Name, instance, DateTimeOffset.UtcNow));
    }
}

public sealed class FlutterMemoryAllocations : MemoryAllocations;

internal static class _FieldNames
{
    internal const string eventType = "eventType";
    internal const string library = "library";
    internal const string className = "className";
}

public static class MemoryAllocationsLibrary
{
    internal const string _dartUiLibrary = "dart:ui";
    internal static MemoryAllocations _kMemoryAllocations => MemoryAllocations.instance;
    public static bool kFlutterMemoryAllocationsEnabled => MemoryAllocations.instance.hasListeners;
}
