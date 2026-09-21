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
    ValueTask<IPlatformViewInstance> CreateAsync(
        PlatformViewHandle handle,
        ReadOnlyMemory<byte> parameters,
        Action<PlatformViewHandle> onFocused,
        CancellationToken cancellationToken
    );
}

/// <summary>Reserves native operations for an entire placement transaction. Await
/// each operation before disposal. Contended batches are retried without blocking UI.</summary>
public interface IPlatformViewPlacementBatch : IDisposable
{
    bool Contains(PlatformViewHandle handle);
    ValueTask AttachAsync(PlatformViewPlacement placement);
    ValueTask DetachAsync(PlatformViewHandle handle);
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
public sealed class PlatformViewCoordinator
    : IPlatformViewHostCapability,
        IWebViewHostCapability,
        IAsyncDisposable,
        IDisposable
{
    private sealed class Entry(
        PlatformViewHandle handle,
        IPlatformViewFactory factory,
        PlatformViewRequest request
    )
    {
        public readonly PlatformViewHandle Handle = handle;
        public readonly IPlatformViewFactory Factory = factory;
        public readonly PlatformViewRequest Request = request;
        public readonly CancellationTokenSource Cancellation = new();
        public readonly TaskCompletionSource<PlatformViewHandle> Ready = new(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        public readonly TaskCompletionSource Created = new(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        public readonly TaskCompletionSource Retired = new(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
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
    private long _allocatedId;
    private bool _closed;
    private Task? _closeTask;
    private PlatformCompositionToken? _placementReceipt;

    public PlatformViewCoordinator(
        ulong ownerViewId,
        string backend,
        PlatformViewFactoryRegistry factories,
        IPlatformViewDispatcher dispatcher
    )
    {
        OwnerViewId = ownerViewId;
        _backend = backend;
        _factories = factories;
        _dispatcher = dispatcher;
    }

    public ulong OwnerViewId { get; }
    public int LiveInstanceCount
    {
        get
        {
            lock (_gate)
            {
                return _entries.Count;
            }
        }
    }
    public event Action<PlatformViewHandle>? ViewFocused;
    public event Action<WebViewEvent>? WebViewChanged;

    public async Task<WebViewResult> ExecuteWebViewAsync(
        PlatformViewHandle handle,
        WebViewCommand command,
        CancellationToken cancellationToken = default
    )
    {
        Task<WebViewResult>? pending = null;
        await OperateAsync(
                handle,
                entry =>
                {
                    if (entry.Instance is not IPlatformWebViewInstance web)
                    {
                        throw new WebViewException(
                            WebViewError.Unsupported,
                            "This attachment has no WebView commands."
                        );
                    }

                    pending = web.ExecuteAsync(command, cancellationToken);
                    return ValueTask.CompletedTask;
                },
                cancellationToken
            )
            .ConfigureAwait(false);
        return await pending!.ConfigureAwait(false);
    }

    private void OnWebViewChanged(WebViewEvent value)
    {
        Action<WebViewEvent>? callback;
        lock (_gate)
        {
            if (
                _closed
                || !_entries.TryGetValue(value.Handle.InstanceId, out var entry)
                || entry.Handle != value.Handle
                || !IsLive(entry.State)
            )
            {
                return;
            }

            callback = WebViewChanged;
        }
        callback?.Invoke(value);
    }

    public Task DisposalCompletion
    {
        get
        {
            lock (_gate)
            {
                return _closeTask ?? Task.CompletedTask;
            }
        }
    }

    public long AllocateInstanceId()
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_closed, this);
            do
            {
                _allocatedId = checked(_allocatedId + 1);
            } while (_entries.ContainsKey(_allocatedId));
            return _allocatedId;
        }
    }

    public Task GetDisposalCompletion(long instanceId)
    {
        lock (_gate)
        {
            return _entries.TryGetValue(instanceId, out var entry)
                ? entry.Disposal ?? Task.CompletedTask
                : Task.CompletedTask;
        }
    }

    public ValueTask<PlatformViewHandle> CreateAsync(
        PlatformViewDescriptor descriptor,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        if (descriptor.Input != PlatformViewInputPolicy.DirectNative)
        {
            throw new NotSupportedException(
                "Gesture arena admission has not been qualified for this host."
            );
        }

        var request = new PlatformViewRequest(
            AllocateInstanceId(),
            descriptor.ViewType,
            descriptor.Composition,
            CreationParameters: descriptor.CreationParameters
        );
        if (
            descriptor.Strategy == PlatformViewStrategyPolicy.PlatformPreferred
            && !QuerySupport(request).Supported
        )
        {
            request = request with { Composition = PlatformViewComposition.NativeOverlay };
        }

        return CreateAsync(request, cancellationToken);
    }

    public PlatformViewSnapshot CaptureSnapshot(PlatformViewComposition composition)
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_closed, this);
            var instances =
                new Dictionary<
                    PlatformViewHandle,
                    IReadOnlyDictionary<PlatformViewEffects, PlatformViewSupport>
                >();
            foreach (var entry in _entries.Values.Where(entry => IsLive(entry.State)))
            {
                var variants = new Dictionary<PlatformViewEffects, PlatformViewSupport>();
                foreach (
                    var effects in new[]
                    {
                        PlatformViewEffects.None,
                        PlatformViewEffects.RectClip,
                        PlatformViewEffects.AffineTransform,
                        PlatformViewEffects.RectClip | PlatformViewEffects.AffineTransform,
                    }
                )
                {
                    variants.Add(
                        effects,
                        entry.Factory.QuerySupport(
                            entry.Request with
                            {
                                Composition = composition,
                                Effects = effects,
                            }
                        )
                    );
                }

                instances.Add(entry.Handle, variants.AsReadOnly());
            }
            return new(OwnerViewId, composition, instances);
        }
    }

    /// <summary>Rechecks identities atomically after pure planning, then retains the whole batch.</summary>
    public void Admit(PlatformCompositionPlan plan)
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_closed, this);
            if (plan.Token.OwnerViewId != OwnerViewId)
            {
                throw Error(default, "foreign frame owner");
            }

            var handles = plan
                .Parts.OfType<PlatformNativeSegment>()
                .Select(p => p.Placement.Handle)
                .Distinct()
                .ToArray();
            foreach (var handle in handles)
            {
                Require(handle);
            }

            var leases = new List<IDisposable>();
            try
            {
                foreach (var handle in handles)
                {
                    leases.Add(Retain(handle));
                }

                plan.Admit(leases.ToArray());
            }
            catch
            {
                foreach (var lease in leases)
                {
                    lease.Dispose();
                }
                throw;
            }
        }
    }

    public PlatformViewSupport QuerySupport(PlatformViewRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return _factories.Find(request.ViewType)?.QuerySupport(request)
            ?? new(
                _backend,
                System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier,
                request.ViewType,
                false,
                request.Composition,
                PlatformViewEffects.None,
                Reason: "No factory is registered for this view type."
            );
    }

    public async ValueTask<PlatformViewHandle> CreateAsync(
        PlatformViewRequest request,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ViewType);
        cancellationToken.ThrowIfCancellationRequested();
        if (request.InstanceId < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request));
        }

        Entry entry;
        lock (_gate)
        {
            if (_closed)
            {
                throw Error(default, "owner is closed");
            }

            var support = QuerySupport(request);
            if (
                !support.Supported
                || support.Composition != request.Composition
                || (request.Effects & ~support.Effects) != 0
            )
            {
                throw Error(
                    new(OwnerViewId, request.InstanceId, 0),
                    support.Reason ?? "composition/effect request is unsupported"
                );
            }

            if (_entries.ContainsKey(request.InstanceId))
            {
                throw Error(new(OwnerViewId, request.InstanceId, 0), "instance id is already live");
            }

            entry = new(
                new(OwnerViewId, request.InstanceId, checked(++_generation)),
                _factories.Find(request.ViewType)!,
                request with
                {
                    CreationParameters = default,
                }
            );
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
            await _dispatcher
                .InvokeAsync(async () =>
                {
                    var instance =
                        await entry.Factory.CreateAsync(
                            entry.Handle,
                            parameters,
                            OnFocused,
                            entry.Cancellation.Token
                        )
                        ?? throw new InvalidOperationException(
                            "PlatformView factory returned null."
                        );
                    lock (_gate)
                    {
                        entry.Instance = instance;
                        if (instance is IPlatformWebViewInstance web)
                        {
                            web.WebViewChanged += OnWebViewChanged;
                        }

                        if (entry.State == PlatformViewState.Creating)
                        {
                            entry.State = PlatformViewState.Ready;
                            entry.Ready.TrySetResult(entry.Handle);
                        }
                    }
                })
                .ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            lock (_gate)
            {
                if (entry.State == PlatformViewState.Creating)
                {
                    entry.State = PlatformViewState.Failed;
                    entry.Ready.TrySetException(
                        exception is WebViewException
                            ? exception
                            : Error(entry.Handle, $"factory failed: {exception.Message}")
                    );
                }
            }
            _ = BeginDispose(entry);
        }
        finally
        {
            entry.Created.TrySetResult();
        }
    }

    public PlatformViewHandle Resolve(long instanceId)
    {
        lock (_gate)
        {
            if (!_entries.TryGetValue(instanceId, out var entry) || !IsLive(entry.State))
            {
                throw Error(
                    new(OwnerViewId, instanceId, 0),
                    "instance is not ready or has been removed"
                );
            }

            return entry.Handle;
        }
    }

    public PlatformViewState GetState(PlatformViewHandle handle)
    {
        lock (_gate)
        {
            return Require(handle, false).State;
        }
    }

    /// <summary>Records attachment state after a host-owned whole-frame commit, without
    /// applying SDK placement a second time. Keep the admitted plan alive through this call.
    /// A receipt is not physical presentation and cannot revive a retiring instance.</summary>
    public bool RecordPlacementReceipt(PlatformCompositionPlan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);
        if (plan.IsDisposed || !plan.IsAdmitted || plan.Token.OwnerViewId != OwnerViewId)
        {
            throw new InvalidOperationException(
                "Placement receipt requires a live admitted owner plan."
            );
        }

        var placements = plan
            .Parts.OfType<PlatformNativeSegment>()
            .ToDictionary(p => p.Placement.Handle, p => p.Placement);
        lock (_gate)
        {
            if (
                _closed
                || (
                    _placementReceipt is { } previous
                    && (
                        plan.Token.ViewEpoch < previous.ViewEpoch
                        || plan.Token.SurfaceGeneration < previous.SurfaceGeneration
                        || plan.Token.FrameNumber <= previous.FrameNumber
                    )
                )
            )
            {
                return false;
            }

            foreach (var entry in _entries.Values)
            {
                if (!IsLive(entry.State))
                {
                    continue;
                }

                if (placements.TryGetValue(entry.Handle, out var placement))
                {
                    entry.State =
                        placement.Visible && !placement.Bounds.isEmpty
                            ? PlatformViewState.Attached
                            : PlatformViewState.Hidden;
                }
                else if (entry.State == PlatformViewState.Attached)
                {
                    entry.State = PlatformViewState.Hidden;
                }
            }
            _placementReceipt = plan.Token;
            return true;
        }
    }

    private static bool IsLive(PlatformViewState state) =>
        state
            is PlatformViewState.Ready
                or PlatformViewState.Attached
                or PlatformViewState.Hidden
                or PlatformViewState.Detached;

    private Entry Require(PlatformViewHandle handle, bool live = true)
    {
        if (
            handle.OwnerViewId != OwnerViewId
            || !_entries.TryGetValue(handle.InstanceId, out var entry)
            || entry.Handle != handle
            || (live && !IsLive(entry.State))
        )
        {
            throw Error(handle, "stale, foreign, or unavailable instance");
        }

        return entry;
    }

    public ValueTask AttachAsync(
        PlatformViewPlacement placement,
        CancellationToken cancellationToken = default
    )
    {
        placement.Validate();
        ValidatePlacementSupport(placement, false);
        return OperateAsync(
            placement.Handle,
            async entry =>
            {
                await entry.Instance!.ApplyAsync(placement);
                lock (_gate)
                {
                    if (IsLive(entry.State))
                    {
                        entry.State =
                            placement.Visible && !placement.Bounds.isEmpty
                                ? PlatformViewState.Attached
                                : PlatformViewState.Hidden;
                    }
                }
            },
            cancellationToken
        );
    }

    internal void ValidatePlacementSupport(
        PlatformViewPlacement placement,
        bool interleaved,
        bool reserved = false
    )
    {
        lock (_gate)
        {
            var entry = Require(placement.Handle, live: !reserved);
            var effects = placement.Clip is null
                ? PlatformViewEffects.None
                : PlatformViewEffects.RectClip;
            if (
                placement.Transform.M11 != 1
                || placement.Transform.M12 != 0
                || placement.Transform.M21 != 0
                || placement.Transform.M22 != 1
            )
            {
                effects |= PlatformViewEffects.AffineTransform;
            }

            var request = entry.Request with
            {
                Effects = effects,
                Composition = interleaved
                    ? PlatformViewComposition.InterleavedComposition
                    : entry.Request.Composition,
            };
            var support = entry.Factory.QuerySupport(request);
            if (
                !support.Supported
                || support.Composition != request.Composition
                || (effects & ~support.Effects) != 0
            )
            {
                throw Error(
                    placement.Handle,
                    support.Reason ?? "native placement/composition effects are unsupported"
                );
            }
        }
    }

    public ValueTask DetachAsync(
        PlatformViewHandle handle,
        CancellationToken cancellationToken = default
    ) =>
        OperateAsync(
            handle,
            async entry =>
            {
                await entry.Instance!.DetachAsync();
                lock (_gate)
                {
                    if (IsLive(entry.State))
                    {
                        entry.State = PlatformViewState.Detached;
                    }
                }
            },
            cancellationToken
        );

    public ValueTask SetFocusAsync(
        PlatformViewHandle handle,
        bool focused,
        CancellationToken cancellationToken = default
    ) => OperateAsync(handle, entry => entry.Instance!.SetFocusAsync(focused), cancellationToken);

    public bool TryBeginPlacementBatch(
        IEnumerable<PlatformViewHandle> handles,
        out IPlatformViewPlacementBatch? batch
    )
    {
        var entries = new Dictionary<PlatformViewHandle, Entry>();
        lock (_gate)
        {
            batch = null;
            if (_closed)
            {
                return false;
            }

            try
            {
                foreach (var handle in handles.Distinct().OrderBy(h => h.InstanceId))
                {
                    var entry = Require(handle);
                    if (!entry.Operations.Wait(0))
                    {
                        foreach (var acquired in entries.Values)
                        {
                            acquired.Operations.Release();
                        }

                        return false;
                    }
                    entries.Add(handle, entry);
                }
                batch = new PlacementBatch(this, entries);
                return true;
            }
            catch
            {
                foreach (var acquired in entries.Values)
                {
                    acquired.Operations.Release();
                }

                throw;
            }
        }
    }

    private sealed class PlacementBatch(
        PlatformViewCoordinator owner,
        Dictionary<PlatformViewHandle, Entry> entries
    ) : IPlatformViewPlacementBatch
    {
        private bool _disposed;
        private int _pending;

        public bool Contains(PlatformViewHandle handle) =>
            !_disposed && entries.ContainsKey(handle);

        public ValueTask AttachAsync(PlatformViewPlacement placement)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (!entries.ContainsKey(placement.Handle))
            {
                throw owner.Error(
                    placement.Handle,
                    "native operation is outside the reserved batch"
                );
            }

            placement.Validate();
            // Disposal waits on the reservation. Finish this already-admitted
            // operation without reviving its state, then allow removal to run.
            owner.ValidatePlacementSupport(placement, false, reserved: true);
            return Apply(
                placement.Handle,
                async entry =>
                {
                    await entry.Instance!.ApplyAsync(placement);
                    lock (owner._gate)
                    {
                        if (IsLive(entry.State))
                        {
                            entry.State =
                                placement.Visible && !placement.Bounds.isEmpty
                                    ? PlatformViewState.Attached
                                    : PlatformViewState.Hidden;
                        }
                    }
                }
            );
        }

        public ValueTask DetachAsync(PlatformViewHandle handle) =>
            Apply(
                handle,
                async entry =>
                {
                    await entry.Instance!.DetachAsync();
                    lock (owner._gate)
                    {
                        if (IsLive(entry.State))
                        {
                            entry.State = PlatformViewState.Detached;
                        }
                    }
                }
            );

        private async ValueTask Apply(PlatformViewHandle handle, Func<Entry, ValueTask> action)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (!entries.TryGetValue(handle, out var entry))
            {
                throw owner.Error(handle, "native operation is outside the reserved batch");
            }

            Interlocked.Increment(ref _pending);
            try
            {
                await owner._dispatcher.InvokeAsync(() => action(entry)).ConfigureAwait(false);
            }
            finally
            {
                Interlocked.Decrement(ref _pending);
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            if (Volatile.Read(ref _pending) != 0)
            {
                throw new InvalidOperationException(
                    "Placement batch still has pending native operations."
                );
            }

            _disposed = true;
            foreach (var entry in entries.Values)
            {
                entry.Operations.Release();
            }
        }
    }

    private async ValueTask OperateAsync(
        PlatformViewHandle handle,
        Func<Entry, ValueTask> action,
        CancellationToken cancellationToken
    )
    {
        Entry entry;
        lock (_gate)
        {
            entry = Require(handle);
        }

        await entry.Operations.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await _dispatcher
                .InvokeAsync(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    lock (_gate)
                    {
                        Require(handle);
                    }

                    await action(entry);
                })
                .ConfigureAwait(false);
        }
        finally
        {
            entry.Operations.Release();
        }
    }

    /// <summary>Acquire before submission. Release only after GPU AND compositor retirement.</summary>
    public IDisposable Retain(PlatformViewHandle handle)
    {
        lock (_gate)
        {
            var entry = Require(handle);
            checked
            {
                entry.References++;
            }
            return new Lease(this, entry);
        }
    }

    private sealed class Lease(PlatformViewCoordinator owner, Entry entry) : IDisposable
    {
        private int _disposed;

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
            {
                return;
            }

            lock (owner._gate)
            {
                if (--entry.References == 0 && entry.Disposal is not null)
                {
                    entry.Retired.TrySetResult();
                }
            }
        }
    }

    public ValueTask DisposeAsync(PlatformViewHandle handle)
    {
        lock (_gate)
        {
            if (handle.OwnerViewId != OwnerViewId)
            {
                throw Error(handle, "foreign owner");
            }
            // Old disposal must never destroy a replacement with the same legacy id.
            if (!_entries.TryGetValue(handle.InstanceId, out var entry) || entry.Handle != handle)
            {
                return ValueTask.CompletedTask;
            }

            return new(BeginDispose(entry));
        }
    }

    private Task BeginDispose(Entry entry)
    {
        lock (_gate)
        {
            if (entry.Disposal is not null)
            {
                return entry.Disposal;
            }

            if (entry.State != PlatformViewState.Failed)
            {
                entry.State = PlatformViewState.Disposing;
            }

            entry.Ready.TrySetCanceled();
            if (entry.References == 0)
            {
                entry.Retired.TrySetResult();
            }
            // Run outside this lock: cancellation callbacks/native code must not execute under it.
            entry.Disposal = Task.Run(() => DisposeCoreAsync(entry));
            return entry.Disposal;
        }
    }

    private async Task DisposeCoreAsync(Entry entry)
    {
        var failures = new List<Exception>();
        try
        {
            entry.Cancellation.Cancel();
        }
        catch (Exception error)
        {
            failures.Add(error);
        }
        await entry.Created.Task.ConfigureAwait(false);
        await entry.Operations.WaitAsync().ConfigureAwait(false);
        try
        {
            if (entry.Instance is { } instance)
            {
                if (instance is IPlatformWebViewInstance web)
                {
                    web.WebViewChanged -= OnWebViewChanged;
                }

                try
                {
                    await _dispatcher.InvokeAsync(instance.DisableInputAsync).ConfigureAwait(false);
                }
                catch (Exception error)
                {
                    failures.Add(error);
                }
                await entry.Retired.Task.ConfigureAwait(false);
                try
                {
                    await _dispatcher.InvokeAsync(instance.DetachAsync).ConfigureAwait(false);
                }
                catch (Exception error)
                {
                    failures.Add(error);
                }
                try
                {
                    await _dispatcher.InvokeAsync(instance.DisposeAsync).ConfigureAwait(false);
                }
                catch (Exception error)
                {
                    failures.Add(error);
                }
            }
        }
        finally
        {
            entry.Operations.Release();
            entry.Cancellation.Dispose();
            lock (_gate)
            {
                entry.State =
                    failures.Count == 0 && entry.State != PlatformViewState.Failed
                        ? PlatformViewState.Disposed
                        : PlatformViewState.Failed;
                _entries.Remove(entry.Handle.InstanceId);
            }
        }
        if (failures.Count != 0)
        {
            throw new AggregateException(
                $"PlatformView {entry.Handle} cleanup failed on {_backend}.",
                failures
            );
        }
    }

    private void OnFocused(PlatformViewHandle handle)
    {
        Action<PlatformViewHandle>? callback;
        lock (_gate)
        {
            if (
                _closed
                || !_entries.TryGetValue(handle.InstanceId, out var entry)
                || entry.Handle != handle
                || entry.State != PlatformViewState.Attached
            )
            {
                return;
            }

            callback = ViewFocused;
        }
        callback?.Invoke(handle);
    }

    public ValueTask DisposeAsync()
    {
        lock (_gate)
        {
            if (_closeTask is not null)
            {
                return new(_closeTask);
            }

            _closed = true;
            ViewFocused = null;
            WebViewChanged = null;
            _closeTask = Task.WhenAll(_entries.Values.ToArray().Select(BeginDispose));
            return new(_closeTask);
        }
    }

    // View capability teardown is synchronous. It starts cleanup; runner shutdown awaits DisposalCompletion.
    public void Dispose() => _ = DisposeAsync();

    private DorotiCapabilityException Error(PlatformViewHandle handle, string reason) =>
        new(
            DorotiCapabilityIds.PlatformViews,
            OwnerViewId,
            DorotiUiInvocation.Managed("PlatformView"),
            $"instance={handle.InstanceId}; generation={handle.InstanceGeneration}; {reason}",
            _backend
        );
}
