namespace Doroti.Ui;

public sealed record DorotiManagedPluginContext(DorotiSceneOwner Owner, DorotiUiInvocation Invocation);

/// <summary>A strongly typed in-runtime plugin. External byte/ABI adapters are separate.</summary>
public interface IDorotiManagedPluginHandler<TRequest, TResult>
{
    ValueTask<TResult> HandleAsync(DorotiManagedPluginContext context, TRequest request,
        CancellationToken cancellationToken = default);
}

public interface IDorotiManagedPluginInvoker
{
    ValueTask<TResult> InvokeAsync<TRequest, TResult>(DorotiView view, string pluginId, TRequest request,
        DorotiUiInvocation invocation, CancellationToken cancellationToken = default);
}
