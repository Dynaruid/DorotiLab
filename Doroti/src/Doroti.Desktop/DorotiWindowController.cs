using Doroti.Hosting;
using Doroti.Ui;

namespace Doroti.Desktop;

public sealed class DorotiWindowController
{
    private readonly IWindowHost _host;
    private readonly DorotiWindowManager _manager;
    private readonly SemaphoreSlim _commands = new(1);
    private readonly CancellationTokenSource _lifetime = new();
    private readonly TaskCompletionSource _initialized = new(
        TaskCreationOptions.RunContinuationsAsynchronously
    );
    private readonly TaskCompletionSource _ready = new(
        TaskCreationOptions.RunContinuationsAsynchronously
    );
    private readonly object _gate = new();
    private readonly List<
        Func<WindowClosingContext, CancellationToken, Task<WindowCloseDecision>>
    > _closing = [];
    private Action<WindowState>? _changed;
    private Task<bool>? _close;
    private WindowOptions _options;
    private WindowState _state;
    private long _appearanceSequence;
    private long _latestReplacement;
    private int _closed;
    private bool _closingAccepted;

    internal DorotiWindowController(
        WindowId id,
        IWindowHost host,
        DorotiWindowManager manager,
        WindowOptions options
    )
    {
        Id = id;
        _host = host;
        _manager = manager;
        _options = options;
        _state = host.State;
        _host.StateChanged += HandleState;
        _host.CloseRequested += HandleCloseRequested;
        _host.Closed += HandleClosed;
        // Observe faults even if the app never waits for readiness.
        Observe(_initialized.Task);
        Observe(_ready.Task);
        Observe(_host.ReadyToShow);
    }

    public WindowId Id { get; }
    public WindowCapabilities Capabilities => _host.Capabilities;
    public WindowState State => Volatile.Read(ref _state);
    public Task InitializationWork { get; private set; } = Task.CompletedTask;
    public Exception? InitializationError { get; private set; }

    public Task EnsureInitializedAsync(CancellationToken cancellationToken = default) =>
        _initialized.Task.WaitAsync(cancellationToken);

    public Task WaitUntilReadyToShowAsync(CancellationToken cancellationToken = default) =>
        _ready.Task.WaitAsync(cancellationToken);

    internal async Task InitializeAsync(
        WindowCreateOptions request,
        IDorotiViewEntrypoint content,
        CancellationToken cancellationToken
    )
    {
        await _host.InitializeAsync(
            request.Options,
            new(this, _manager),
            content,
            cancellationToken
        );
        cancellationToken.ThrowIfCancellationRequested();
        _initialized.TrySetResult();
        HandleState(_host.State);
    }

    internal void Start(WindowCreateOptions request)
    {
        Observe(ObserveReadinessAsync());
        InitializationWork = RunStartupAsync(request);
        Observe(InitializationWork);
    }

    private async Task ObserveReadinessAsync()
    {
        try
        {
            await _host.ReadyToShow.WaitAsync(_lifetime.Token);
            _ready.TrySetResult();
        }
        catch (Exception error)
        {
            _ready.TrySetException(error);
            if (!_lifetime.IsCancellationRequested && !_closingAccepted)
            {
                RecordInitializationFailure(error);
                await AbortAsync(error);
            }
        }
    }

    private async Task RunStartupAsync(WindowCreateOptions request)
    {
        // Yield so application callbacks cannot block manager creation/registry insertion.
        await Task.Yield();
        try
        {
            var hook =
                request.OnCreated?.Invoke(new(this, _manager), _lifetime.Token)
                ?? Task.CompletedTask;
            var autoShow =
                request.Options.StartupVisibility == WindowStartupVisibility.WhenReady
                    ? ShowAsync(_lifetime.Token)
                    : Task.CompletedTask;
            Observe(autoShow);
            Observe(hook);
            // A failure in either task must not wait for the other callback to finish.
            var first = await Task.WhenAny(hook, autoShow);
            await first;
            await (ReferenceEquals(first, hook) ? autoShow : hook);
        }
        catch (Exception error)
        {
            var beforeReady = !_ready.Task.IsCompletedSuccessfully;
            _ready.TrySetException(error);
            if (!_lifetime.IsCancellationRequested)
                RecordInitializationFailure(error);
            if (beforeReady && Volatile.Read(ref _closed) == 0 && !_closingAccepted)
                await AbortAsync(error);
            throw;
        }
    }

    private void RecordInitializationFailure(Exception error)
    {
        lock (_gate)
        {
            if (InitializationError is not null)
                return;
            InitializationError = error;
        }
        _manager.ReportFailure(this, error);
    }

    public IDisposable Subscribe(Action<WindowState> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);
        lock (_gate)
        {
            ThrowIfClosed();
            _changed += observer;
        }
        return new WindowSubscription(() =>
        {
            lock (_gate)
                _changed -= observer;
        });
    }

    /// <summary>
    /// Registers an API close decision. Native close requests participate only when
    /// Capabilities.CanCancelNativeClose is true; platform termination may bypass it.
    /// </summary>
    public IDisposable RegisterClosing(
        Func<WindowClosingContext, CancellationToken, Task<WindowCloseDecision>> callback
    )
    {
        ArgumentNullException.ThrowIfNull(callback);
        lock (_gate)
        {
            ThrowIfClosed();
            _closing.Add(callback);
        }
        return new WindowSubscription(() =>
        {
            lock (_gate)
                _closing.Remove(callback);
        });
    }

    private void HandleState(WindowState state)
    {
        if (Volatile.Read(ref _closed) != 0)
            return;
        Volatile.Write(ref _state, state);
        Action<WindowState>? callbacks;
        lock (_gate)
            callbacks = _changed;
        foreach (var callback in callbacks?.GetInvocationList() ?? [])
        {
            try
            {
                ((Action<WindowState>)callback)(state);
            }
            catch (Exception error)
            {
                _manager.ReportFailure(this, error);
            }
        }
    }

    private async void HandleCloseRequested()
    {
        try
        {
            await CloseAsync();
        }
        catch (Exception error)
        {
            _manager.ReportFailure(this, error);
        }
    }

    public Task<bool> CloseAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (_gate)
        {
            if (_closed != 0)
                return Task.FromResult(true);
            if (_close is null || _close.IsCompleted)
                _close = CloseCoreAsync();
            // Canceling a waiter does not cancel another caller's shared close decision.
            return _close.WaitAsync(cancellationToken);
        }
    }

    private async Task<bool> CloseCoreAsync()
    {
        await Task.Yield();
        Func<WindowClosingContext, CancellationToken, Task<WindowCloseDecision>>[] callbacks;
        lock (_gate)
            callbacks = _closing.ToArray();
        foreach (var callback in callbacks)
            if (await callback(new(this), _lifetime.Token) == WindowCloseDecision.Cancel)
                return false;
        _closingAccepted = true;
        _ready.TrySetException(new ObjectDisposedException(nameof(DorotiWindowController)));
        _lifetime.Cancel(); // Interrupt deferred operations before draining their queue.
        await _commands.WaitAsync();
        try
        {
            await _host.CloseAsync(CancellationToken.None);
            await _host.DisposeAsync();
            CompleteClosed();
            return true;
        }
        catch
        {
            _closingAccepted = false;
            throw;
        }
        finally
        {
            _commands.Release();
        }
    }

    private void HandleClosed()
    {
        // Expected close is published only after CloseAsync and disposal finish.
        if (_closingAccepted)
            return;
        _ = FinishUnexpectedCloseAsync();
    }

    private async Task FinishUnexpectedCloseAsync()
    {
        _closingAccepted = true;
        _ready.TrySetException(new ObjectDisposedException(nameof(DorotiWindowController)));
        try
        {
            await _host.DisposeAsync();
        }
        catch (Exception error)
        {
            _manager.ReportFailure(this, error);
        }
        finally
        {
            CompleteClosed();
        }
    }

    private void CompleteClosed()
    {
        if (Interlocked.Exchange(ref _closed, 1) != 0)
            return;
        _lifetime.Cancel();
        var error = new ObjectDisposedException(nameof(DorotiWindowController));
        _initialized.TrySetException(error);
        _ready.TrySetException(error);
        _host.StateChanged -= HandleState;
        _host.CloseRequested -= HandleCloseRequested;
        _host.Closed -= HandleClosed;
        Volatile.Write(ref _state, State with { Closed = true, Visible = false, Focused = false });
        lock (_gate)
        {
            _changed = null;
            _closing.Clear();
        }
        _manager.Remove(this);
    }

    internal async Task AbortAsync(Exception error)
    {
        lock (_gate)
        {
            if (_closingAccepted || _closed != 0)
                return;
            _closingAccepted = true;
        }
        _initialized.TrySetException(error);
        _ready.TrySetException(error);
        _lifetime.Cancel();
        await _commands.WaitAsync();
        try
        {
            await _host.DisposeAsync();
        }
        finally
        {
            CompleteClosed();
            _commands.Release();
        }
    }

    private void ThrowIfClosed()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _closed) != 0 || _closingAccepted, this);
    }

    private async Task<WindowState> CommandAsync(
        WindowCommand command,
        CancellationToken cancellationToken
    )
    {
        ThrowIfClosed();
        await EnsureInitializedAsync(cancellationToken);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            _lifetime.Token
        );
        await _commands.WaitAsync(linked.Token);
        try
        {
            ThrowIfClosed();
            var candidate = command.Kind switch
            {
                WindowCommandKind.Size => _options with { Size = (Size)command.Value! },
                WindowCommandKind.MinimumSize => _options with
                {
                    MinimumSize = (Size?)command.Value,
                },
                WindowCommandKind.MaximumSize => _options with
                {
                    MaximumSize = (Size?)command.Value,
                },
                WindowCommandKind.AlwaysOnTop => _options with
                {
                    AlwaysOnTop = (bool)command.Value!,
                },
                WindowCommandKind.SkipTaskbar => _options with
                {
                    SkipTaskbar = (bool)command.Value!,
                },
                WindowCommandKind.Resizable => _options with { Resizable = (bool)command.Value! },
                WindowCommandKind.Title => _options with { Title = (string)command.Value! },
                _ => _options,
            };
            Capabilities.Evaluate(candidate, _options).ThrowIfUnsupported();
            var state = await _host.ExecuteAsync(command, linked.Token);
            _options = candidate;
            HandleState(state);
            return state;
        }
        finally
        {
            _commands.Release();
        }
    }

    public async Task<WindowState> ShowAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfClosed();
        await WaitUntilReadyToShowAsync(cancellationToken);
        return await CommandAsync(new(WindowCommandKind.Show), cancellationToken);
    }

    public Task<WindowState> HideAsync(CancellationToken cancellationToken = default) =>
        CommandAsync(new(WindowCommandKind.Hide), cancellationToken);

    public Task<WindowState> FocusAsync(CancellationToken cancellationToken = default) =>
        CommandAsync(new(WindowCommandKind.Focus), cancellationToken);

    public Task<WindowState> SetTitleAsync(
        string title,
        CancellationToken cancellationToken = default
    ) => CommandAsync(new(WindowCommandKind.Title, title), cancellationToken);

    public Task<WindowState> SetSizeAsync(
        Size size,
        CancellationToken cancellationToken = default
    ) => CommandAsync(new(WindowCommandKind.Size, size), cancellationToken);

    /// <summary>Outer-frame bounds in physical desktop pixels, unlike logical client Size.</summary>
    public Task<WindowState> SetBoundsAsync(
        Rect bounds,
        CancellationToken cancellationToken = default
    )
    {
        if (!bounds.IsFinite || bounds.isEmpty)
            throw new ArgumentOutOfRangeException(nameof(bounds));
        return CommandAsync(new(WindowCommandKind.Bounds, bounds), cancellationToken);
    }

    public Task<WindowState> CenterAsync(CancellationToken cancellationToken = default) =>
        CommandAsync(new(WindowCommandKind.Center), cancellationToken);

    public Task<WindowState> SetMinimumSizeAsync(
        Size? size,
        CancellationToken cancellationToken = default
    ) => CommandAsync(new(WindowCommandKind.MinimumSize, size), cancellationToken);

    public Task<WindowState> SetMaximumSizeAsync(
        Size? size,
        CancellationToken cancellationToken = default
    ) => CommandAsync(new(WindowCommandKind.MaximumSize, size), cancellationToken);

    public Task<WindowState> SetAlwaysOnTopAsync(
        bool value,
        CancellationToken cancellationToken = default
    ) => CommandAsync(new(WindowCommandKind.AlwaysOnTop, value), cancellationToken);

    public Task<WindowState> SetSkipTaskbarAsync(
        bool value,
        CancellationToken cancellationToken = default
    ) => CommandAsync(new(WindowCommandKind.SkipTaskbar, value), cancellationToken);

    public Task<WindowState> SetResizableAsync(
        bool value,
        CancellationToken cancellationToken = default
    ) => CommandAsync(new(WindowCommandKind.Resizable, value), cancellationToken);

    public Task<WindowState> MinimizeAsync(CancellationToken cancellationToken = default) =>
        CommandAsync(new(WindowCommandKind.Minimize), cancellationToken);

    public Task<WindowState> MaximizeAsync(CancellationToken cancellationToken = default) =>
        CommandAsync(new(WindowCommandKind.Maximize), cancellationToken);

    public Task<WindowState> RestoreAsync(CancellationToken cancellationToken = default) =>
        CommandAsync(new(WindowCommandKind.Restore), cancellationToken);

    public Task<WindowState> SetFullScreenAsync(
        bool value,
        CancellationToken cancellationToken = default
    ) => CommandAsync(new(WindowCommandKind.FullScreen, value), cancellationToken);

    public Task<WindowState> StartDraggingAsync(CancellationToken cancellationToken = default) =>
        CommandAsync(new(WindowCommandKind.Drag), cancellationToken);

    public Task<WindowState> StartResizingAsync(
        WindowResizeEdge edge,
        CancellationToken cancellationToken = default
    ) => CommandAsync(new(WindowCommandKind.Resize, edge), cancellationToken);

    public Task<WindowApplyResult> ApplyAppearanceAsync(
        WindowAppearanceOptions appearance,
        CancellationToken cancellationToken = default
    ) => ApplyAsync(_ => appearance, true, cancellationToken);

    public Task<WindowApplyResult> UpdateAppearanceAsync(
        Func<WindowAppearanceOptions, WindowAppearanceOptions> update,
        CancellationToken cancellationToken = default
    ) => ApplyAsync(update, false, cancellationToken);

    private async Task<WindowApplyResult> ApplyAsync(
        Func<WindowAppearanceOptions, WindowAppearanceOptions> update,
        bool replace,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(update);
        ThrowIfClosed();
        var sequence = Interlocked.Increment(ref _appearanceSequence);
        if (replace)
            Interlocked.Exchange(ref _latestReplacement, sequence);
        var entered = false;
        try
        {
            await EnsureInitializedAsync(cancellationToken);
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                _lifetime.Token
            );
            await _commands.WaitAsync(linked.Token);
            entered = true;
            ThrowIfClosed();
            if (replace && sequence != Volatile.Read(ref _latestReplacement))
                return new(WindowApplyStatus.Superseded, sequence, State);
            var appearance = update(_options.Appearance);
            var candidate = _options with { Appearance = appearance };
            var evaluation = Capabilities.Evaluate(candidate, _options);
            if (evaluation.Support != WindowSupport.Supported)
                return new(
                    WindowApplyStatus.Rejected,
                    sequence,
                    State,
                    new NotSupportedException($"{evaluation.Support}: {evaluation.Reason}")
                );
            var previous = _options.Appearance;
            try
            {
                var state = await _host.ApplyAppearanceAsync(appearance, linked.Token);
                _options = candidate;
                HandleState(state);
                return new(WindowApplyStatus.Applied, sequence, State);
            }
            catch (Exception error)
            {
                try
                {
                    // A canceled pre-dispatch/deferred operation has no changes to undo.
                    if (
                        error is not OperationCanceledException
                        || _host.State.RequestedAppearance != previous
                    )
                        HandleState(
                            await _host.ApplyAppearanceAsync(previous, CancellationToken.None)
                        );
                }
                catch (Exception rollback)
                {
                    HandleState(_host.State);
                    error = new AggregateException(error, rollback);
                }
                return new(
                    error is OperationCanceledException
                        ? WindowApplyStatus.Canceled
                        : WindowApplyStatus.Failed,
                    sequence,
                    State,
                    error
                );
            }
        }
        catch (OperationCanceledException error)
        {
            return new(WindowApplyStatus.Canceled, sequence, State, error);
        }
        finally
        {
            if (entered)
                _commands.Release();
        }
    }

    private static void Observe(Task task) =>
        _ = task.ContinueWith(
            t => _ = t.Exception,
            CancellationToken.None,
            TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default
        );
}
