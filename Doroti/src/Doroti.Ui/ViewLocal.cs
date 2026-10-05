using System.Runtime.CompilerServices;

namespace Doroti.Ui;

/// <summary>Mutable service state belongs to an explicit live view, including its async scope.</summary>
public sealed class ViewLocal<T>(Func<T> factory)
{
    private sealed class Slot(T value) { public T Value = value; }
    private readonly ConditionalWeakTable<DorotiView, Slot> _values = new();
    private Slot Current
    {
        get
        {
            var view = PlatformDispatcher.instance.RequireInvocationView(DorotiUiInvocation.Managed("ViewLocal"));
            return _values.GetValue(view, _ => new(factory()));
        }
    }
    public T Value { get => Current.Value; set => Current.Value = value; }
}
