using Doroti.Ui;

namespace Doroti.Hosting;

/// <summary>Must execute callbacks on the platform UI thread, preserving its async context.</summary>
public interface IPlatformViewDispatcher
{
    ValueTask InvokeAsync(Func<ValueTask> action);
}

/// <summary>SDK/native handles stay inside implementations of this interface.</summary>
public interface IPlatformViewInstance : IAsyncDisposable
{
    ValueTask ApplyAsync(PlatformViewPlacement placement);
    ValueTask DetachAsync();
    ValueTask SetFocusAsync(bool focused);
    // Called immediately on removal, before references held by submitted frames retire.
    ValueTask DisableInputAsync();
}

public interface IPlatformViewFactory
{
    string ViewType { get; }
    PlatformViewSupport QuerySupport(PlatformViewRequest request);
    ValueTask<IPlatformViewInstance> CreateAsync(PlatformViewHandle handle, ReadOnlyMemory<byte> parameters,
        Action<PlatformViewHandle> onFocused, CancellationToken cancellationToken);
}

/// <summary>Shares factory definitions only. Each owner receives a separate coordinator.</summary>
public sealed class PlatformViewFactoryRegistry
{
    private readonly Dictionary<string, IPlatformViewFactory> _factories;
    public PlatformViewFactoryRegistry(IEnumerable<IPlatformViewFactory> factories)
    {
        ArgumentNullException.ThrowIfNull(factories);
        _factories = factories.ToDictionary(factory => factory.ViewType, StringComparer.Ordinal);
    }
    public IReadOnlyCollection<string> ViewTypes => _factories.Keys.ToArray();
    internal IPlatformViewFactory? Find(string viewType) => _factories.GetValueOrDefault(viewType);
}

/// <summary>Thread-safe identity and retirement owner; native operations run through the UI dispatcher.</summary>
public sealed class PlatformViewCoordinator : IPlatformViewHostCapability, IAsyncDisposable, IDisposable
{
    private sealed class Entry(PlatformViewHandle handle, IPlatformViewFactory factory, PlatformViewRequest request)
    {
        public readonly PlatformViewHandle Handle = handle;
        public readonly IPlatformViewFactory Factory = factory;
        public readonly PlatformViewRequest Request = request;
        public readonly CancellationTokenSource Cancellation = new();
        public readonly TaskCompletionSource<PlatformViewHandle> Ready = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public readonly TaskCompletionSource Created = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public readonly TaskCompletionSource Retired = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public readonly SemaphoreSlim Operations = new(1, 1);
        public PlatformViewState State = PlatformViewState.Requested;
        public IPlatformViewInstance? Instance;
        public int References;
        public Task? Disposal;
    }

    private readonly object _gate = new();
    private readonly Dictionary<long, Entry> _entries = [];
    private readonly PlatformViewFactoryRegistry _factories;
    private readonly IPlatformViewDispatcher _dispatcher;
    private readonly string _backend;
    private long _generation;
    private bool _closed;
    private Task? _closeTask;
    public PlatformViewCoordinator(ulong ownerViewId, string backend, PlatformViewFactoryRegistry factories, IPlatformViewDispatcher dispatcher)
    {
        OwnerViewId = ownerViewId;
        _backend = backend;
        _factories = factories;
        _dispatcher = dispatcher;
    }
    public ulong OwnerViewId { get; }
    public int LiveInstanceCount { get { lock (_gate) return _entries.Count; } }
    public event Action<PlatformViewHandle>? ViewFocused;
    public Task DisposalCompletion { get { lock (_gate) return _closeTask ?? Task.CompletedTask; } }

    public PlatformViewSupport QuerySupport(PlatformViewRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return _factories.Find(request.ViewType)?.QuerySupport(request) ?? new(
            _backend, System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier, request.ViewType,
            false, request.Composition, PlatformViewEffects.None, Reason: "No factory is registered for this view type.");
    }

    public async ValueTask<PlatformViewHandle> CreateAsync(PlatformViewRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ViewType);
        cancellationToken.ThrowIfCancellationRequested();
        if (request.InstanceId < 0) throw new ArgumentOutOfRangeException(nameof(request));
        Entry entry;
        lock (_gate)
        {
            if (_closed) throw Error(default, "owner is closed");
            var support = QuerySupport(request);
            if (!support.Supported || support.Composition != request.Composition || (request.Effects & ~support.Effects) != 0)
                throw Error(new(OwnerViewId, request.InstanceId, 0), support.Reason ?? "composition/effect request is unsupported");
            if (_entries.ContainsKey(request.InstanceId)) throw Error(new(OwnerViewId, request.InstanceId, 0), "instance id is already live");
            entry = new(new(OwnerViewId, request.InstanceId, checked(++_generation)), _factories.Find(request.ViewType)!, request with { CreationParameters = default });
            _entries.Add(request.InstanceId, entry);
            entry.State = PlatformViewState.Creating;
        }
        // Own the parameter bytes across async creation; callers may reuse their transport buffer.
        var parameters = request.CreationParameters.ToArray();
        using var registration = cancellationToken.Register(() => BeginDispose(entry));
        _ = CreateCoreAsync(entry, parameters);
        return await entry.Ready.Task.ConfigureAwait(false);
    }

    private async Task CreateCoreAsync(Entry entry, ReadOnlyMemory<byte> parameters)
    {
        try
        {
            await _dispatcher.InvokeAsync(async () =>
            {
                var instance = await entry.Factory.CreateAsync(entry.Handle, parameters, OnFocused, entry.Cancellation.Token)
                    ?? throw new InvalidOperationException("PlatformView factory returned null.");
                lock (_gate)
                {
                    entry.Instance = instance;
                    if (entry.State == PlatformViewState.Creating)
                    {
                        entry.State = PlatformViewState.Ready;
                        entry.Ready.TrySetResult(entry.Handle);
                    }
                }
            }).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            lock (_gate)
            {
                if (entry.State == PlatformViewState.Creating)
                {
                    entry.State = PlatformViewState.Failed;
                    entry.Ready.TrySetException(Error(entry.Handle, $"factory failed: {exception.Message}"));
                }
            }
            _ = BeginDispose(entry);
        }
        finally { entry.Created.TrySetResult(); }
    }

    public PlatformViewHandle Resolve(long instanceId)
    {
        lock (_gate)
        {
            if (!_entries.TryGetValue(instanceId, out var entry) || !IsLive(entry.State))
                throw Error(new(OwnerViewId, instanceId, 0), "instance is not ready or has been removed");
            return entry.Handle;
        }
    }
    public PlatformViewState GetState(PlatformViewHandle handle)
    {
        lock (_gate) return Require(handle, false).State;
    }
    private static bool IsLive(PlatformViewState state) => state is PlatformViewState.Ready or PlatformViewState.Attached or PlatformViewState.Hidden or PlatformViewState.Detached;
    private Entry Require(PlatformViewHandle handle, bool live = true)
    {
        if (handle.OwnerViewId != OwnerViewId || !_entries.TryGetValue(handle.InstanceId, out var entry) || entry.Handle != handle || live && !IsLive(entry.State))
            throw Error(handle, "stale, foreign, or unavailable instance");
        return entry;
    }

    public ValueTask AttachAsync(PlatformViewPlacement placement, CancellationToken cancellationToken = default)
    {
        placement.Validate();
        ValidatePlacementSupport(placement, false);
        return OperateAsync(placement.Handle, async entry =>
        {
            await entry.Instance!.ApplyAsync(placement);
            lock (_gate) if (IsLive(entry.State)) entry.State = placement.Visible && !placement.Bounds.isEmpty ? PlatformViewState.Attached : PlatformViewState.Hidden;
        }, cancellationToken);
    }

    internal void ValidatePlacementSupport(PlatformViewPlacement placement, bool interleaved)
    {
        lock (_gate)
        {
            var entry = Require(placement.Handle);
            var effects = placement.Clip is null ? PlatformViewEffects.None : PlatformViewEffects.RectClip;
            if (placement.Transform.M11 != 1 || placement.Transform.M12 != 0 || placement.Transform.M21 != 0 || placement.Transform.M22 != 1)
                effects |= PlatformViewEffects.AffineTransform;
            var request = entry.Request with
            {
                Effects = effects,
                Composition = interleaved ? PlatformViewComposition.InterleavedComposition : entry.Request.Composition,
            };
            var support = entry.Factory.QuerySupport(request);
            if (!support.Supported || support.Composition != request.Composition || (effects & ~support.Effects) != 0)
                throw Error(placement.Handle, support.Reason ?? "native placement/composition effects are unsupported");
        }
    }
    public ValueTask DetachAsync(PlatformViewHandle handle, CancellationToken cancellationToken = default) =>
        OperateAsync(handle, async entry =>
        {
            await entry.Instance!.DetachAsync();
            lock (_gate) if (IsLive(entry.State)) entry.State = PlatformViewState.Detached;
        }, cancellationToken);
    public ValueTask SetFocusAsync(PlatformViewHandle handle, bool focused, CancellationToken cancellationToken = default) =>
        OperateAsync(handle, entry => entry.Instance!.SetFocusAsync(focused), cancellationToken);
    private async ValueTask OperateAsync(PlatformViewHandle handle, Func<Entry, ValueTask> action, CancellationToken cancellationToken)
    {
        Entry entry;
        lock (_gate) entry = Require(handle);
        await entry.Operations.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await _dispatcher.InvokeAsync(async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                lock (_gate) Require(handle);
                await action(entry);
            }).ConfigureAwait(false);
        }
        finally { entry.Operations.Release(); }
    }

    /// <summary>Acquire before submission. Release only after GPU AND compositor retirement.</summary>
    public IDisposable Retain(PlatformViewHandle handle)
    {
        lock (_gate)
        {
            var entry = Require(handle);
            checked { entry.References++; }
            return new Lease(this, entry);
        }
    }
    private sealed class Lease(PlatformViewCoordinator owner, Entry entry) : IDisposable
    {
        private int _disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
            lock (owner._gate) if (--entry.References == 0 && entry.Disposal is not null) entry.Retired.TrySetResult();
        }
    }

    public ValueTask DisposeAsync(PlatformViewHandle handle)
    {
        lock (_gate)
        {
            if (handle.OwnerViewId != OwnerViewId) throw Error(handle, "foreign owner");
            // Old disposal must never destroy a replacement with the same legacy id.
            if (!_entries.TryGetValue(handle.InstanceId, out var entry) || entry.Handle != handle) return ValueTask.CompletedTask;
            return new(BeginDispose(entry));
        }
    }
    private Task BeginDispose(Entry entry)
    {
        lock (_gate)
        {
            if (entry.Disposal is not null) return entry.Disposal;
            if (entry.State != PlatformViewState.Failed) entry.State = PlatformViewState.Disposing;
            entry.Ready.TrySetCanceled();
            if (entry.References == 0) entry.Retired.TrySetResult();
            // Run outside this lock: cancellation callbacks/native code must not execute under it.
            entry.Disposal = Task.Run(() => DisposeCoreAsync(entry));
            return entry.Disposal;
        }
    }
    private async Task DisposeCoreAsync(Entry entry)
    {
        var failures = new List<Exception>();
        try { entry.Cancellation.Cancel(); } catch (Exception error) { failures.Add(error); }
        await entry.Created.Task.ConfigureAwait(false);
        await entry.Operations.WaitAsync().ConfigureAwait(false);
        try
        {
            if (entry.Instance is { } instance)
            {
                try { await _dispatcher.InvokeAsync(instance.DisableInputAsync).ConfigureAwait(false); }
                catch (Exception error) { failures.Add(error); }
                await entry.Retired.Task.ConfigureAwait(false);
                try { await _dispatcher.InvokeAsync(instance.DetachAsync).ConfigureAwait(false); }
                catch (Exception error) { failures.Add(error); }
                try { await _dispatcher.InvokeAsync(instance.DisposeAsync).ConfigureAwait(false); }
                catch (Exception error) { failures.Add(error); }
            }
        }
        finally
        {
            entry.Operations.Release();
            entry.Cancellation.Dispose();
            lock (_gate)
            {
                entry.State = failures.Count == 0 && entry.State != PlatformViewState.Failed ? PlatformViewState.Disposed : PlatformViewState.Failed;
                _entries.Remove(entry.Handle.InstanceId);
            }
        }
        if (failures.Count != 0) throw new AggregateException($"PlatformView {entry.Handle} cleanup failed on {_backend}.", failures);
    }
    private void OnFocused(PlatformViewHandle handle)
    {
        Action<PlatformViewHandle>? callback;
        lock (_gate)
        {
            if (_closed || !_entries.TryGetValue(handle.InstanceId, out var entry) || entry.Handle != handle || entry.State != PlatformViewState.Attached) return;
            callback = ViewFocused;
        }
        callback?.Invoke(handle);
    }
    public ValueTask DisposeAsync()
    {
        lock (_gate)
        {
            if (_closeTask is not null) return new(_closeTask);
            _closed = true;
            ViewFocused = null;
            _closeTask = Task.WhenAll(_entries.Values.ToArray().Select(BeginDispose));
            return new(_closeTask);
        }
    }
    // View capability teardown is synchronous. It starts cleanup; runner shutdown awaits DisposalCompletion.
    public void Dispose() => _ = DisposeAsync();
    private DorotiCapabilityException Error(PlatformViewHandle handle, string reason) => new(
        DorotiCapabilityIds.PlatformViews, OwnerViewId, DartUiInvocation.Managed("PlatformView"),
        $"instance={handle.InstanceId}; generation={handle.InstanceGeneration}; {reason}", _backend);
}
