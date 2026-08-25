namespace Doroti.Host.WindowsAppSdk;

/// <summary>
/// One per-child ordered route over <see cref="FlutterWindowsHostWindow"/>'s
/// typed WndProc hook.  The router has no window procedure of its own: the F2
/// host remains the sole owner of the raw child HWND and forwards each message
/// here only after its child-rect bookkeeping has completed.
/// </summary>
internal sealed class FlutterWindowsChildMessageRouter : IDisposable
{
    private readonly FlutterWindowsHostWindow _host;
    private readonly nint _childHwnd;
    private readonly Func<FlutterWindowsChildMessage, FlutterWindowsChildMessageResult>[] _handlers;
    private readonly object _gate = new();
    private bool _disposed;
    private long _receivedMessageCount;
    private long _handledMessageCount;
    private long _unhandledMessageCount;
    private long _mismatchedHwndMessageCount;

    internal FlutterWindowsChildMessageRouter(
        FlutterWindowsHostWindow host,
        params Func<FlutterWindowsChildMessage, FlutterWindowsChildMessageResult>[] handlers)
    {
        _host = host ?? throw new ArgumentNullException(nameof(host));
        ArgumentNullException.ThrowIfNull(handlers);
        if (handlers.Length == 0)
            throw new ArgumentException("A child message router needs at least one handler.", nameof(handlers));
        if (handlers.Any(static handler => handler is null))
            throw new ArgumentException("A child message router cannot contain a null handler.", nameof(handlers));

        _childHwnd = host.ViewHwnd;
        if (_childHwnd == 0)
            throw new InvalidOperationException("The Flutter input router requires a live child HWND.");
        _handlers = [.. handlers];
        _host.ChildMessageReceived += RouteFromHost;
    }

    /// <summary>
    /// Routes a message through the same ordered path used by the child
    /// WndProc.  It is intentionally internal so the isolated fixture can
    /// exercise ordering without manufacturing a second native WndProc.
    /// </summary>
    internal FlutterWindowsChildMessageResult Route(FlutterWindowsChildMessage message)
    {
        Func<FlutterWindowsChildMessage, FlutterWindowsChildMessageResult>[] handlers;
        lock (_gate)
        {
            if (_disposed) return FlutterWindowsChildMessageResult.Unhandled;
            Interlocked.Increment(ref _receivedMessageCount);
            if (message.Hwnd != _childHwnd)
            {
                Interlocked.Increment(ref _mismatchedHwndMessageCount);
                Interlocked.Increment(ref _unhandledMessageCount);
                return FlutterWindowsChildMessageResult.Unhandled;
            }
            // Do not invoke an input handler while holding the router lock.
            // InputHost.Dispose acquires its own state lock before detaching
            // this router; a snapshot here prevents an ABBA wait during that
            // teardown race while retaining the original route order.
            handlers = _handlers;
        }

        foreach (var handler in handlers)
        {
            var result = handler(message);
            if (!result.Handled) continue;
            Interlocked.Increment(ref _handledMessageCount);
            return result;
        }

        Interlocked.Increment(ref _unhandledMessageCount);
        return FlutterWindowsChildMessageResult.Unhandled;
    }

    internal FlutterWindowsChildMessageRouterSnapshot Snapshot => new(
        _childHwnd,
        Interlocked.Read(ref _receivedMessageCount),
        Interlocked.Read(ref _handledMessageCount),
        Interlocked.Read(ref _unhandledMessageCount),
        Interlocked.Read(ref _mismatchedHwndMessageCount),
        !_disposed,
        _disposed);

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed) return;
            _disposed = true;
            _host.ChildMessageReceived -= RouteFromHost;
        }
    }

    private FlutterWindowsChildMessageResult RouteFromHost(FlutterWindowsChildMessage message) => Route(message);
}

/// <summary>
/// Observable ownership counters for one child-only message router.  A
/// mismatched HWND is deliberately left unhandled; no router may become a
/// process-wide input interception path.
/// </summary>
internal sealed record FlutterWindowsChildMessageRouterSnapshot(
    nint ChildHwnd,
    long ReceivedMessageCount,
    long HandledMessageCount,
    long UnhandledMessageCount,
    long MismatchedHwndMessageCount,
    bool IsAttached,
    bool IsDisposed);
