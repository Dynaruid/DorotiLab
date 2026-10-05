using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Doroti.Hosting;
using Doroti.Ui;

namespace Doroti.Host.WindowsAppSdk;

[StructLayout(LayoutKind.Sequential)]
public struct OleDropPoint { public int X, Y; }

[ComVisible(true), Guid("00000122-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IOleDropTarget
{
    [PreserveSig] int DragEnter([MarshalAs(UnmanagedType.Interface)] IDataObject data, uint keys, OleDropPoint point, ref uint effect);
    [PreserveSig] int DragOver(uint keys, OleDropPoint point, ref uint effect);
    [PreserveSig] int DragLeave();
    [PreserveSig] int Drop([MarshalAs(UnmanagedType.Interface)] IDataObject data, uint keys, OleDropPoint point, ref uint effect);
}

/// <summary>OLE reception on the owner HWND thread; app callbacks dispatch to the view's framework queue.</summary>
[ComVisible(true), ClassInterface(ClassInterfaceType.None)]
public sealed class WindowsOsDropTarget : IOsDragDropHostCapability, IOleDropTarget, IDisposable
{
    private readonly nint _client;
    private readonly uint _thread;
    private readonly List<nint> _windows = [];
    private readonly OsDropReceiver _receiver;
    private IReadOnlyList<string> _formats = [];
    private OsDropOffer? _last;
    private bool _closed;
    public OsDropSupport Support => _receiver.Support;

    public WindowsOsDropTarget(nint clientWindow, Action<Action> dispatch, IEnumerable<nint>? surfaces = null)
    {
        _thread = GetCurrentThreadId();
        if (clientWindow == 0 || GetWindowThreadProcessId(clientWindow, out var process) != _thread || process != Environment.ProcessId)
            throw new ArgumentException("The client HWND must belong to the calling UI thread.", nameof(clientWindow));
        _client = clientWindow;
        _receiver = new(new(true, false, OsDropAction.Copy, [OsDropFormats.Files, OsDropFormats.Text, OsDropFormats.UriList]), dispatch);
        Marshal.ThrowExceptionForHR(OleInitialize(0));
        try
        {
            foreach (var window in (surfaces ?? [clientWindow]).Distinct())
            {
                if (window == 0 || GetWindowThreadProcessId(window, out process) != _thread || process != Environment.ProcessId)
                    throw new ArgumentException("All drop surfaces must belong to the same owner UI thread.", nameof(surfaces));
                Marshal.ThrowExceptionForHR(RegisterDragDrop(window, this));
                _windows.Add(window);
            }
        }
        catch
        {
            foreach (var window in _windows) RevokeDragDrop(window);
            OleUninitialize();
            _receiver.Dispose();
            throw;
        }
    }

    public IOsDropRegistration Register(OsDropOptions options, Action<OsDropEvent> onEvent) => _receiver.Register(options, onEvent);

    private OsDropOffer Offer(OleDropPoint screen, uint keys, uint source)
    {
        if (!ScreenToClient(_client, ref screen) || !GetClientRect(_client, out var bounds)) throw new InvalidOperationException("Drop owner coordinates unavailable.");
        var dpi = GetDpiForWindow(_client);
        if (dpi == 0) throw new InvalidOperationException("Drop owner DPI unavailable.");
        var requested = (keys & 0x20) != 0 || (keys & 0x0c) == 0x0c ? OsDropAction.Link
            : (keys & 4) != 0 ? OsDropAction.Move : (keys & 8) != 0 ? OsDropAction.Copy : OsDropAction.None;
        if (screen.X < 0 || screen.Y < 0 || screen.X >= bounds.Right || screen.Y >= bounds.Bottom) source = 0;
        return new(new Offset(screen.X * 96d / dpi, screen.Y * 96d / dpi), _formats,
            (OsDropAction)(source & 7), requested);
    }

    int IOleDropTarget.DragEnter(IDataObject data, uint keys, OleDropPoint point, ref uint effect)
    {
        try
        {
            if (_closed) { effect = 0; return 0; }
            _formats = WindowsDropData.Formats(data);
            _last = Offer(point, keys, effect);
            effect = (uint)_receiver.Hover(OsDropPhase.Enter, _last);
        }
        catch (Exception error) { effect = 0; System.Diagnostics.Trace.TraceError(error.ToString()); }
        return 0;
    }
    int IOleDropTarget.DragOver(uint keys, OleDropPoint point, ref uint effect)
    {
        try
        {
            if (_closed || _last is null) { effect = 0; return 0; }
            _last = Offer(point, keys, effect);
            effect = (uint)_receiver.Hover(OsDropPhase.Over, _last);
        }
        catch (Exception error) { effect = 0; System.Diagnostics.Trace.TraceError(error.ToString()); }
        return 0;
    }
    int IOleDropTarget.DragLeave()
    {
        try { if (!_closed && _last is { } offer) _receiver.Hover(OsDropPhase.Leave, offer); }
        catch (Exception error) { System.Diagnostics.Trace.TraceError(error.ToString()); }
        finally { _last = null; _formats = []; }
        return 0;
    }
    int IOleDropTarget.Drop(IDataObject data, uint keys, OleDropPoint point, ref uint effect)
    {
        try
        {
            if (_closed) { effect = 0; return 0; }
            _formats = WindowsDropData.Formats(data);
            var offer = Offer(point, keys, effect);
            effect = (uint)_receiver.Drop(offer, formats => WindowsDropData.Acquire(data, formats));
        }
        catch (Exception error) { effect = 0; System.Diagnostics.Trace.TraceError(error.ToString()); }
        finally { _last = null; _formats = []; }
        return 0;
    }

    public void Dispose()
    {
        if (_closed) return;
        if (GetCurrentThreadId() != _thread) throw new InvalidOperationException("Revoke OLE drop targets on the registering thread before HWND destruction.");
        _closed = true;
        List<Exception> errors = [];
        try
        {
            foreach (var window in _windows)
                try { Marshal.ThrowExceptionForHR(RevokeDragDrop(window)); } catch (Exception error) { errors.Add(error); }
        }
        finally
        {
            _windows.Clear();
            try { _receiver.Dispose(); } finally { OleUninitialize(); }
        }
        GC.KeepAlive(this);
        if (errors.Count != 0) throw new AggregateException(errors);
    }

    [StructLayout(LayoutKind.Sequential)] private struct NativeRect { public int Left, Top, Right, Bottom; }
    [DllImport("ole32.dll")] private static extern int OleInitialize(nint reserved);
    [DllImport("ole32.dll")] private static extern void OleUninitialize();
    [DllImport("ole32.dll")] private static extern int RegisterDragDrop(nint window, [MarshalAs(UnmanagedType.Interface)] IOleDropTarget target);
    [DllImport("ole32.dll")] private static extern int RevokeDragDrop(nint window);
    [DllImport("user32.dll")] private static extern bool ScreenToClient(nint window, ref OleDropPoint point);
    [DllImport("user32.dll")] private static extern bool GetClientRect(nint window, out NativeRect bounds);
    [DllImport("user32.dll")] private static extern uint GetDpiForWindow(nint window);
    [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(nint window, out int process);
    [DllImport("kernel32.dll")] private static extern uint GetCurrentThreadId();
}
