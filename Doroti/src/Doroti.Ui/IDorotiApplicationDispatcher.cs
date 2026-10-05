namespace Doroti.Ui;

/// <summary>One explicit application/framework owner. Native/render owners
/// enqueue typed work here and never synchronously wait on one another.</summary>
public interface IDorotiApplicationDispatcher
{
    bool HasThreadAccess { get; }
    ValueTask InvokeAsync(Action callback, CancellationToken cancellationToken = default);
    ValueTask<T> InvokeAsync<T>(Func<T> callback, CancellationToken cancellationToken = default);
}
