using System.Runtime.CompilerServices;

namespace Doroti.Ui;

/// <summary>Mutable framework state owned by the active dispatcher, including async continuations.</summary>
public sealed class DispatcherLocal<T>(Func<T> factory)
{
    private sealed class Slot(T value) { public T Value = value; }
    private readonly ConditionalWeakTable<PlatformDispatcher, Slot> _values = new();
    private readonly Lazy<Slot> _legacy = new(() => new(factory()));
    private Slot Current => PlatformDispatcher.current is { } dispatcher
        ? _values.GetValue(dispatcher, _ => new(factory())) : _legacy.Value;
    public T Value { get => Current.Value; set => Current.Value = value; }
}
