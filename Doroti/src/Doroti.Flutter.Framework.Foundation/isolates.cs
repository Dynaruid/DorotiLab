// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/foundation/isolates.dart
namespace Doroti.Generated.Framework.Foundation;

public delegate TResult ComputeCallback<in TMessage, out TResult>(TMessage message);

public static class ComputeImpl
{
    public static Task<TResult> compute<TMessage, TResult>(ComputeCallback<TMessage, TResult> callback, TMessage message, string? debugLabel = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _ = debugLabel;
        return Task.Run(() => callback(message), cancellationToken);
    }
}

public static class IsolatesLibrary
{
    public static Task<TResult> compute<TMessage, TResult>(ComputeCallback<TMessage, TResult> callback, TMessage message, string? debugLabel = null, CancellationToken cancellationToken = default) =>
        ComputeImpl.compute(callback, message, debugLabel, cancellationToken);
}
