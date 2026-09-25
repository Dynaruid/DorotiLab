using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Doroti.Desktop;
using Doroti.Hosting;
using Doroti.Ui;
using Appearance = Doroti.Desktop.WindowAppearanceOptions;

namespace Doroti.Host.Qt;

/// <summary>Single Quick window. Native QObject access runs through its generation-scoped GUI queue.</summary>
internal sealed class QtDesktopWindowHost : IWindowHost, IWindowHostFactory
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeState
    {
        internal uint Size, Flags;
        internal double Width, Height, Scale;
        internal uint Presentation, Reserved;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeCommand
    {
        internal uint Size, Kind;
        internal double X, Y;
        internal QtNativeV2.Utf8 Text;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct Api
    {
        internal uint Version, Size;
        internal ulong Features;
        internal delegate* unmanaged[Cdecl]<ulong, delegate* unmanaged[Cdecl]<nint, int, void>, nint, int> Post;
        internal delegate* unmanaged[Cdecl]<ulong, delegate* unmanaged[Cdecl]<nint, uint, void>, nint, int> Observe;
        internal delegate* unmanaged[Cdecl]<ulong, NativeCommand*, int> Command;
        internal delegate* unmanaged[Cdecl]<ulong, NativeState*, int> Snapshot;
    }
    [DllImport("doroti_qt_host", EntryPoint = "doroti_qt_get_desktop", CallingConvention = CallingConvention.Cdecl)]
    private static extern int GetApi(nint window, uint version, uint size, out ulong owner, out Api api);
    [DllImport("doroti_qt_host", EntryPoint = "doroti_qt_desktop_quit", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void Quit();

    private readonly TaskCompletionSource _attached = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource _ready = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource _closed = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private Api _api;
    private ulong _owner;
    private GCHandle _context;
    private WindowOptions _options = new();
    private WindowState _state = new(null, new(800, 600), 1, false, false,
        WindowPresentationState.Normal, new(), new(new()), new(0, ViewPadding.zero, 0, 0, 0));
    private long _revision;
    private int _allocated;
    private readonly object _closeGate = new();
    private Task? _closeWork;
    internal IDorotiViewEntrypoint Content { get; private set; } = null!;
    internal Action<Exception>? Fatal { get; set; }
    public WindowCapabilities Capabilities { get; } = new(QtDesktopWindowPolicy.Evaluate);
    WindowManagerCapabilities IWindowHostFactory.Capabilities => new(false, 1);
    public WindowState State => Volatile.Read(ref _state);
    public Task ReadyToShow => _ready.Task;
    public event Action<WindowState>? StateChanged;
    public event Action? CloseRequested;
    public event Action? Closed;

    public WindowEvaluation Evaluate(WindowCreateOptions options) => Capabilities.Evaluate(options.Options);
    public ValueTask<IWindowHost> CreateAsync(WindowId id, WindowCreateOptions options, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        Evaluate(options).ThrowIfUnsupported();
        if (Interlocked.Exchange(ref _allocated, 1) != 0)
            throw new NotSupportedException("Qt Desktop cannot reopen or create additional native windows.");
        return ValueTask.FromResult<IWindowHost>(this);
    }
    public Task InitializeAsync(WindowOptions options, DesktopWindowContext context,
        IDorotiViewEntrypoint content, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        Capabilities.Evaluate(options).ThrowIfUnsupported();
        _options = options;
        Content = content;
        _state = State with { ClientSize = options.Size, RequestedAppearance = options.Appearance,
            EffectiveAppearance = new(options.Appearance) };
        return Task.CompletedTask;
    }

    internal unsafe void Attach(nint window)
    {
        try { Check(GetApi(window, 1, (uint)sizeof(Api), out _owner, out _api), "get desktop ABI"); }
        catch (EntryPointNotFoundException error)
        {
            throw new NotSupportedException("The Qt shim predates Desktop ABI 1. Rebuild the app-owned shim and template.", error);
        }
        if (_api.Version != 1 || _api.Size != sizeof(Api) || (_api.Features & 1) == 0
            || _api.Post == null || _api.Observe == null || _api.Command == null || _api.Snapshot == null)
            throw new InvalidDataException("Invalid Qt Desktop ABI 1 table.");
        _context = GCHandle.Alloc(this);
        var status = _api.Observe(_owner, &OnEvent, GCHandle.ToIntPtr(_context));
        if (status != 0) { _context.Free(); Check(status, "observe"); }
        Send(new(WindowCommandKind.MinimumSize, _options.MinimumSize));
        Send(new(WindowCommandKind.MaximumSize, _options.MaximumSize));
        Send(new(WindowCommandKind.Size, _options.Size));
        Send(new(WindowCommandKind.Resizable, _options.Resizable));
        var initial = new NativeCommand { Size = (uint)sizeof(NativeCommand), Kind = 102,
            X = (uint)_options.PresentationState };
        Check(_api.Command(_owner, &initial), "initial presentation");
        ReadState();
        _attached.TrySetResult();
    }

    internal void FramePresented()
    {
        if (_ready.TrySetResult()) Console.Error.WriteLine("doroti.qt.desktop.ready=frameSwapped");
    }
    internal void Fail(Exception error)
    {
        _attached.TrySetException(error);
        _ready.TrySetException(error);
    }
    internal void RequestClose() => CloseRequested?.Invoke();
    internal async void Abort()
    {
        try { await CloseAsync(CancellationToken.None); }
        catch (Exception error) { Console.Error.WriteLine($"doroti.qt.desktop.abort={error}"); }
        finally { if (_attached.Task.IsCompletedSuccessfully) Quit(); }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void OnEvent(nint context, uint kind)
    {
        var host = (QtDesktopWindowHost)GCHandle.FromIntPtr(context).Target!;
        try
        {
            if (kind == 0) host.ReadState();
            else if (kind == 1) host.RequestClose();
            else if (kind == 2)
            {
                host.Fail(new ObjectDisposedException(nameof(QtDesktopWindowHost)));
                Volatile.Write(ref host._state, host.State with { Closed = true, Visible = false,
                    Focused = false, Revision = ++host._revision });
                host._context.Free();
                host._closed.TrySetResult();
                host.Closed?.Invoke();
            }
        }
        catch (Exception error) { host.Fatal?.Invoke(error); }
    }

    private unsafe WindowState ReadState()
    {
        var state = new NativeState { Size = (uint)sizeof(NativeState) };
        Check(_api.Snapshot(_owner, &state), "snapshot");
        var next = State with { Bounds = null, ClientSize = new(state.Width, state.Height), Scale = state.Scale,
            Visible = (state.Flags & 1) != 0, Focused = (state.Flags & 2) != 0,
            PresentationState = (WindowPresentationState)state.Presentation, Revision = ++_revision };
        Volatile.Write(ref _state, next);
        StateChanged?.Invoke(next);
        return next;
    }
    private unsafe void Send(WindowCommand command)
    {
        var native = new NativeCommand { Size = (uint)sizeof(NativeCommand), Kind = (uint)command.Kind };
        if (command.Value is Size size) { native.X = size.width; native.Y = size.height; }
        if (command.Value is bool enabled) native.X = enabled ? 1 : 0;
        if (command.Kind == WindowCommandKind.MaximumSize && command.Value is null)
            native.X = native.Y = 16777215;
        var text = command.Value is string title ? Encoding.UTF8.GetBytes(title) : [];
        fixed (byte* pointer = text)
        {
            native.Text = new(pointer, (ulong)text.Length);
            Check(_api.Command(_owner, &native), command.Kind.ToString());
        }
    }
    private unsafe void Destroy()
    {
        var command = new NativeCommand { Size = (uint)sizeof(NativeCommand), Kind = 100 };
        Check(_api.Command(_owner, &command), "destroy");
    }

    private sealed record Work(Action Action, CancellationToken Token, TaskCompletionSource Completion);
    private unsafe Task Post(Action action, CancellationToken ct)
    {
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var handle = GCHandle.Alloc(new Work(action, ct, completion));
        var status = _api.Post(_owner, &RunWork, GCHandle.ToIntPtr(handle));
        if (status != 0) { handle.Free(); Check(status, "post"); }
        return completion.Task;
    }
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void RunWork(nint context, int status)
    {
        var handle = GCHandle.FromIntPtr(context);
        var work = (Work)handle.Target!;
        handle.Free();
        try
        {
            Check(status, "dispatch");
            work.Token.ThrowIfCancellationRequested();
            work.Action();
            work.Completion.TrySetResult();
        }
        catch (OperationCanceledException) { work.Completion.TrySetCanceled(work.Token); }
        catch (Exception error) { work.Completion.TrySetException(error); }
    }

    public async Task<WindowState> ExecuteAsync(WindowCommand command, CancellationToken ct)
    {
        if (command.Kind is WindowCommandKind.Bounds or WindowCommandKind.Center
            or WindowCommandKind.AlwaysOnTop or WindowCommandKind.SkipTaskbar
            or WindowCommandKind.Drag or WindowCommandKind.Resize)
            throw new NotSupportedException("Qt Desktop placement, topmost/taskbar commands and deferred system drag/resize are unsupported.");
        await _attached.Task.WaitAsync(ct);
        WindowPresentationState? expected = command.Kind switch
        {
            WindowCommandKind.Minimize => WindowPresentationState.Minimized,
            WindowCommandKind.Maximize => WindowPresentationState.Maximized,
            WindowCommandKind.Restore => WindowPresentationState.Normal,
            WindowCommandKind.FullScreen => (bool)command.Value!
                ? WindowPresentationState.FullScreen : WindowPresentationState.Normal,
            _ => null,
        };
        var observed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        void Changed(WindowState state)
        {
            if (state.PresentationState == expected) observed.TrySetResult();
        }
        if (expected is not null) StateChanged += Changed;
        try
        {
            await Post(() => { Send(command); ReadState(); }, ct);
            // Completion follows a platform state event, not QWindow's requested
            // state signal. A compositor that declines a request cannot hang the queue.
            if (expected is not null)
                await observed.Task.WaitAsync(TimeSpan.FromSeconds(5), ct);
            return State;
        }
        finally { if (expected is not null) StateChanged -= Changed; }
    }
    public Task<WindowState> ApplyAppearanceAsync(Appearance appearance, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        Capabilities.Evaluate(_options with { Appearance = appearance }, _options).ThrowIfUnsupported();
        return Task.FromResult(State);
    }
    public Task CloseAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        lock (_closeGate)
        {
            if (State.Closed) return Task.CompletedTask;
            _closeWork ??= DestroyAsync();
            return _closeWork.WaitAsync(ct);
        }
    }
    private async Task DestroyAsync()
    {
        await _attached.Task;
        await Post(Destroy, CancellationToken.None);
        await _closed.Task;
    }
    public async ValueTask DisposeAsync()
    {
        if (State.Closed || !_attached.Task.IsCompletedSuccessfully) return;
        await CloseAsync(CancellationToken.None);
    }
    private static void Check(int status, string operation)
    {
        if (status == 0) return;
        if (status == 74) throw new NotSupportedException($"Qt Desktop {operation} unsupported (status {status}).");
        if (status is 71 or 73) throw new ObjectDisposedException($"Qt Desktop {operation} (status {status})");
        throw new InvalidOperationException($"Qt Desktop {operation} failed (status {status}).");
    }
}
