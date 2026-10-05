using System.Runtime.Loader;
using Doroti.Tooling.Contracts;

namespace Doroti.Tooling.Extension.Sdk;

/// <summary>One activation per lifetime. Timeout retains live invocations and the load context for a retry.</summary>
public sealed class ToolExtensionLifetime(ProviderDescriptor descriptor, Func<CancellationToken, ValueTask<IDorotiToolExtension>> factory, TimeSpan? timeout = null) : IAsyncDisposable
{
    private readonly object _gate = new();
    private readonly CancellationTokenSource _lifetime = new();
    private readonly HashSet<Task> _pending = [];
    private Task<IDorotiToolExtension>? _activation;
    private bool _stopping;
    private bool _disposed;
    private Task? _disposeWork;
    private readonly SemaphoreSlim _shutdown = new(1);
    private readonly TimeSpan _timeout = timeout ?? TimeSpan.FromSeconds(30);

    private Task<IDorotiToolExtension> Activate()
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_stopping, this);
            return _activation ??= ActivateCore();
        }
    }
    private async Task<IDorotiToolExtension> ActivateCore()
    {
        var extension = await factory(_lifetime.Token);
        try { ToolContract.Validate(await extension.GetCapabilitiesAsync(_lifetime.Token), descriptor); return extension; }
        catch { await extension.DisposeAsync(); throw; }
    }
    public Task<T> InvokeAsync<T>(ToolService service, long generation, Func<IDorotiToolExtension, CancellationToken, ValueTask<T>> invocation, CancellationToken cancellationToken = default)
    {
        if (generation <= 0) throw new ArgumentOutOfRangeException(nameof(generation));
        if (!descriptor.Services.Contains(service)) throw new ToolContractException("unsupported-service", service.ToString());
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_stopping, this);
            if (_pending.Count >= 128) throw new ToolContractException("invocation-capacity", "The extension has 128 active invocations.");
            // Start outside the caller's stack so a synchronous extension cannot block registration or shutdown.
            var work = Task.Run(async () =>
            {
                using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _lifetime.Token);
                var extension = await Activate().WaitAsync(linked.Token);
                var result = await invocation(extension, linked.Token);
                linked.Token.ThrowIfCancellationRequested();
                return result;
            }, CancellationToken.None);
            _pending.Add(work);
            _ = work.ContinueWith(completed => { lock (_gate) _pending.Remove(completed); }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            return work;
        }
    }
    public async ValueTask DisposeAsync()
    {
        await _shutdown.WaitAsync();
        try
        {
        Task[] pending;
        lock (_gate) { if (_disposed) return; _stopping = true; pending = _pending.ToArray(); }
        _lifetime.Cancel();
        var drain = Task.WhenAll(pending.Select(async task => { try { await task; } catch { } }));
        await drain.WaitAsync(_timeout);
        if (_activation is not null)
        {
            IDorotiToolExtension? extension = null;
            try { extension = await _activation.WaitAsync(_timeout); } catch when (_activation.IsCompleted && !_activation.IsCompletedSuccessfully) { }
            if (extension is not null)
            {
                lock (_gate)
                {
                    if (_disposeWork is null || _disposeWork.IsFaulted) _disposeWork = extension.DisposeAsync().AsTask();
                }
                await _disposeWork.WaitAsync(_timeout);
            }
        }
        lock (_gate) { _disposed = true; _activation = null; _disposeWork = null; }
        _lifetime.Dispose();
        }
        finally { _shutdown.Release(); }
    }
}

/// <summary>Only the declared entry type loads; the Contracts assembly is shared with the host.</summary>
public sealed class ToolAssemblyLoader : IAsyncDisposable
{
    private sealed class ToolContext(string assembly) : AssemblyLoadContext(isCollectible: true)
    {
        private readonly AssemblyDependencyResolver _resolver = new(assembly);
        protected override System.Reflection.Assembly? Load(System.Reflection.AssemblyName name)
        {
            var contracts = typeof(IDorotiToolExtension).Assembly;
            if (name.Name == contracts.GetName().Name)
            {
                if (name.Version != contracts.GetName().Version) throw new ToolContractException("contract-assembly-mismatch", "The extension requires another Contracts assembly version.");
                return contracts;
            }
            var file = _resolver.ResolveAssemblyToPath(name);
            return file is null ? null : LoadFromAssemblyPath(file);
        }
        protected override nint LoadUnmanagedDll(string name) => throw new ToolContractException("native-tool-dependency", "Runtime native hosts cannot be loaded into the managed CLI.");
    }
    private ToolContext? _context;
    public ToolExtensionLifetime Lifetime { get; }
    public WeakReference LoadContextReference { get; }
    public ToolAssemblyLoader(ProviderDescriptor descriptor, string assemblyPath, TimeSpan? timeout = null)
    {
        if (!System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported) throw new ToolContractException("dynamic-loading-unavailable", "Declare process-stdio for this tool host.");
        if (descriptor.Tool.Mode != ToolMode.DotnetInproc) throw new ToolContractException("wrong-mode", "The manifest did not declare dotnet-inproc.");
        assemblyPath = Path.GetFullPath(assemblyPath);
        _context = new(assemblyPath);
        LoadContextReference = new(_context);
        Lifetime = new(descriptor, token =>
        {
            token.ThrowIfCancellationRequested();
            var assembly = _context!.LoadFromAssemblyPath(assemblyPath);
            var type = assembly.GetType(descriptor.Tool.EntryType, throwOnError: true)!;
            if (!typeof(IDorotiToolExtension).IsAssignableFrom(type) || type.IsAbstract) throw new ToolContractException("invalid-entry", "The declared type does not implement IDorotiToolExtension.");
            return ValueTask.FromResult((IDorotiToolExtension)(Activator.CreateInstance(type) ?? throw new ToolContractException("invalid-entry", "The declared type could not be created.")));
        }, timeout);
    }
    public async ValueTask DisposeAsync()
    {
        await Lifetime.DisposeAsync();
        // A timeout leaves this reference live; only completed cooperative cleanup permits unloading.
        Interlocked.Exchange(ref _context, null)?.Unload();
    }
}
