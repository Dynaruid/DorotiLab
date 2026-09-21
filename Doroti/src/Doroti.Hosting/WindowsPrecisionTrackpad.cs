using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Doroti.Ui;

namespace Doroti.Hosting;

/// <summary>DirectManipulation ingress shared by the HWND and MAUI Windows hosts.</summary>
/// <remarks>Matches Flutter direct_manipulation.cc: pan/scale during contact,
/// framework-owned inertia, and an inertia-cancel signal on interrupted momentum.
/// COM callbacks use an owned unmanaged vtable so no runtime COM wrappers are required.</remarks>
[SupportedOSPlatform("windows")]
public sealed unsafe class WindowsPrecisionTrackpad : IDisposable
{
    private static readonly Guid ManagerClass = new("54E211B6-3650-4F75-8334-FA359598E1C5");
    private static readonly Guid ManagerId = new("FBF5D3B4-70C7-4163-9322-5A6F660D6FBC");
    private static readonly Guid UpdateId = new("B0AE62FD-BE34-46E7-9CAA-D361FACBB9CC");
    private static readonly Guid ViewportId = new("28b85a3d-60a0-48bd-9ba1-5ce8d9ea3a6d");
    private static readonly Guid ContentId = new("B89962CB-3D89-442B-BB58-5098FA0F9F16");
    private static readonly Guid HandlerId = new("952121DA-D69F-45F9-B0F9-F23944321A6D");
    private static readonly Guid UnknownId = new("00000000-0000-0000-C000-000000000046");
    private static readonly nint* HandlerVtable = CreateVtable();
    private static long _nextPointer = 1L << 56;
    private readonly nint _window,
        _root;
    private readonly ulong _viewId;
    private readonly Action<PointerDataPacket> _dispatch;
    private readonly SubclassProc _subclass;
    private nint _manager,
        _updates,
        _viewport;
    private Callback* _callback;
    private uint _cookie;
    private nuint _subclassId;
    private bool _attached,
        _rootAttached,
        _disposed,
        _running,
        _inertia,
        _resetting,
        _added;
    private double _initialScale = 1,
        _initialX,
        _initialY,
        _lastX,
        _lastY,
        _lastDeltaX,
        _lastDeltaY;
    private PointerData _last;

    public WindowsPrecisionTrackpad(nint window, ulong viewId, Action<PointerDataPacket> dispatch)
    {
        _window = window;
        _root = GetAncestor(window, 2);
        _viewId = viewId;
        _dispatch = dispatch;
        _subclass = HandleMessage;
        try
        {
            Initialize();
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    private void Initialize()
    {
        var clsid = ManagerClass;
        var iid = ManagerId;
        nint result;
        Check(CoCreateInstance(&clsid, 0, 1, &iid, &result));
        _manager = result;
        iid = UpdateId;
        Check(
            ((delegate* unmanaged[Stdcall]<nint, Guid*, nint*, int>)V(_manager, 7))(
                _manager,
                &iid,
                &result
            )
        );
        _updates = result;
        iid = ViewportId;
        Check(
            ((delegate* unmanaged[Stdcall]<nint, nint, nint, Guid*, nint*, int>)V(_manager, 8))(
                _manager,
                0,
                _window,
                &iid,
                &result
            )
        );
        _viewport = result;
        Check(U32(_viewport, 22, 0x1 | 0x2 | 0x4 | 0x10 | 0x20));
        Check(U32(_viewport, 19, 2)); // MANUALUPDATE
        _callback = (Callback*)NativeMemory.AllocZeroed((nuint)sizeof(Callback));
        _callback->Vtable = HandlerVtable;
        _callback->Owner = GCHandle.ToIntPtr(GCHandle.Alloc(this));
        _callback->References = 1;
        uint cookie;
        Check(
            ((delegate* unmanaged[Stdcall]<nint, nint, Callback*, uint*, int>)V(_viewport, 25))(
                _viewport,
                _window,
                _callback,
                &cookie
            )
        );
        _cookie = cookie;
        Resize();
        Check(((delegate* unmanaged[Stdcall]<nint, nint, int>)V(_manager, 3))(_manager, _window));
        Check(Call(_viewport, 3));
        _subclassId = (nuint)_callback;
        _attached = SetWindowSubclass(_window, _subclass, _subclassId, 0);
        if (!_attached)
        {
            throw new InvalidOperationException(
                "Cannot attach precision trackpad input to the window thread."
            );
        }

        if (_root != 0 && _root != _window)
        {
            _rootAttached = SetWindowSubclass(_root, _subclass, _subclassId, 0);
        }

        Update();
    }

    private static void Check(int result) => Marshal.ThrowExceptionForHR(result);

    private static nint V(nint value, int slot) => (*(nint**)value)[slot];

    private static int Call(nint value, int slot) =>
        ((delegate* unmanaged[Stdcall]<nint, int>)V(value, slot))(value);

    private static int U32(nint value, int slot, uint argument) =>
        ((delegate* unmanaged[Stdcall]<nint, uint, int>)V(value, slot))(value, argument);

    private void Update() =>
        Check(((delegate* unmanaged[Stdcall]<nint, nint, int>)V(_updates, 5))(_updates, 0));

    private void Resize()
    {
        if (!GetClientRect(_window, out var rect))
        {
            return;
        }

        rect.Right = Math.Max(1, rect.Right);
        rect.Bottom = Math.Max(1, rect.Bottom);
        Check(((delegate* unmanaged[Stdcall]<nint, Rect*, int>)V(_viewport, 12))(_viewport, &rect));
    }

    private nint HandleMessage(
        nint hwnd,
        uint message,
        nuint wparam,
        nint lparam,
        nuint id,
        nuint data
    )
    {
        try
        {
            if (!_disposed)
            {
                if (
                    message == 0x0250
                    && GetPointerType((uint)(wparam & 0xffff), out var type)
                    && type == 5
                )
                {
                    Check(U32(_viewport, 5, (uint)(wparam & 0xffff)));
                    SetTimer(_window, _subclassId, 8, 0);
                    Update();
                    return 0;
                }
                if (message == 0x0113 && wparam == _subclassId)
                {
                    Update();
                    return 0;
                }
                if (message == 0x0005)
                {
                    Resize();
                }

                if (message == 0x001f || (message == 0x001c && wparam == 0))
                {
                    Cancel();
                }

                if (message == 0x0082 && hwnd == _window)
                {
                    Dispose();
                }
            }
        }
        catch (Exception error)
        {
            Console.Error.WriteLine($"[Doroti trackpad] {error.Message}");
            Cancel();
        }
        return DefSubclassProc(hwnd, message, wparam, lparam);
    }

    private void Emit(PointerData data)
    {
        _last = data;
        _dispatch(new([data]));
    }

    private void Start(nint viewport)
    {
        var iid = ContentId;
        nint content = 0;
        Check(
            ((delegate* unmanaged[Stdcall]<nint, Guid*, nint*, int>)V(viewport, 16))(
                viewport,
                &iid,
                &content
            )
        );
        try
        {
            ReadTransform(content, out _initialScale, out _initialX, out _initialY);
        }
        finally
        {
            Release(content);
        }
        _lastX = _lastY = _lastDeltaX = _lastDeltaY = 0;
        GetCursorPos(out var point);
        ScreenToClient(_window, ref point);
        var pointer = checked((ulong)Interlocked.Increment(ref _nextPointer));
        var packet = new PointerData(
            _viewId,
            DorotiFrameClock.Now,
            PointerChange.panZoomStart,
            PointerDeviceKind.trackpad,
            (1UL << 62) | unchecked((ulong)_window),
            point.X,
            point.Y,
            0,
            0,
            0,
            pointerIdentifier: pointer
        );
        if (!_added)
        {
            Emit(packet with { change = PointerChange.add });
            _added = true;
        }
        _running = true;
        Emit(packet);
    }

    private void End()
    {
        if (!_running)
        {
            return;
        }

        _running = false;
        Emit(
            _last with
            {
                timeStamp = DorotiFrameClock.Now,
                change = PointerChange.panZoomEnd,
                panDeltaX = 0,
                panDeltaY = 0,
            }
        );
    }

    private void Status(nint viewport, uint current, uint previous)
    {
        if (_disposed)
        {
            return;
        }

        if (_resetting)
        {
            _resetting = current != 5;
            return;
        }
        _inertia = current == 4;
        if (previous == 4 && current != 4)
        {
            if (Math.Max(Math.Abs(_lastDeltaX), Math.Abs(_lastDeltaY)) > .01)
            {
                Emit(
                    _last with
                    {
                        timeStamp = DorotiFrameClock.Now,
                        change = PointerChange.hover,
                        signalKind = PointerSignalKind.scrollInertiaCancel,
                    }
                );
            }
        }
        if (current == 3)
        {
            if (!_running)
            {
                Start(viewport);
            }
        }
        else if (previous == 3)
        {
            End();
            _lastDeltaX = _lastDeltaY = 0;
        }
        if (current == 5)
        {
            KillTimer(_window, _subclassId);
            _resetting = true;
            GetClientRect(_window, out var rect);
            Check(
                (
                    (delegate* unmanaged[Stdcall]<nint, float, float, float, float, int, int>)V(
                        viewport,
                        13
                    )
                )(viewport, 0, 0, Math.Max(1, rect.Right), Math.Max(1, rect.Bottom), 0)
            );
            _resetting = false;
        }
    }

    private static void ReadTransform(nint content, out double scale, out double x, out double y)
    {
        float* matrix = stackalloc float[6];
        Check(
            ((delegate* unmanaged[Stdcall]<nint, float*, uint, int>)V(content, 9))(
                content,
                matrix,
                6
            )
        );
        var chopped = 5 * matrix[0];
        scale = chopped - (chopped - matrix[0]);
        x = matrix[4];
        y = matrix[5];
    }

    private void Content(nint content)
    {
        if (_disposed || _resetting)
        {
            return;
        }

        ReadTransform(content, out var scale, out var x, out var y);
        x -= _initialX;
        y -= _initialY;
        _lastDeltaX = x - _lastX;
        _lastDeltaY = y - _lastY;
        _lastX = x;
        _lastY = y;
        if (_running && !_inertia && _initialScale > 0)
        {
            Emit(
                _last with
                {
                    timeStamp = DorotiFrameClock.Now,
                    change = PointerChange.panZoomUpdate,
                    panX = x,
                    panY = y,
                    panDeltaX = _lastDeltaX,
                    panDeltaY = _lastDeltaY,
                    scale = scale / _initialScale,
                }
            );
        }
    }

    private void Cancel()
    {
        End();
        if (_added)
        {
            Emit(
                _last with
                {
                    timeStamp = DorotiFrameClock.Now,
                    change = PointerChange.remove,
                    signalKind = PointerSignalKind.none,
                }
            );
        }

        _added = false;
        _resetting = true;
        if (_viewport != 0)
        {
            Call(_viewport, 7);
            Call(_viewport, 29);
        }
        _resetting = false;
        _inertia = false;
        _lastDeltaX = _lastDeltaY = 0;
        KillTimer(_window, _subclassId);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        Cancel();
        if (_attached)
        {
            RemoveWindowSubclass(_window, _subclass, _subclassId);
        }

        if (_rootAttached)
        {
            RemoveWindowSubclass(_root, _subclass, _subclassId);
        }

        if (_viewport != 0)
        {
            Call(_viewport, 4);
            if (_callback != null)
            {
                U32(_viewport, 26, _cookie);
            }

            Call(_viewport, 30);
            Release(_viewport);
            _viewport = 0;
        }
        if (_manager != 0)
        {
            ((delegate* unmanaged[Stdcall]<nint, nint, int>)V(_manager, 4))(_manager, _window);
        }

        Release(_updates);
        Release(_manager);
        _updates = _manager = 0;
        if (_callback != null)
        {
            ReleaseCallback(_callback);
            _callback = null;
        }
    }

    private static void Release(nint value)
    {
        if (value != 0)
        {
            ((delegate* unmanaged[Stdcall]<nint, uint>)V(value, 2))(value);
        }
    }

    private struct Callback
    {
        internal nint* Vtable;
        internal nint Owner;
        internal int References;
    }

    private static WindowsPrecisionTrackpad Owner(Callback* self) =>
        (WindowsPrecisionTrackpad)GCHandle.FromIntPtr(self->Owner).Target!;

    private static nint* CreateVtable()
    {
        var table = (nint*)
            RuntimeHelpers.AllocateTypeAssociatedMemory(
                typeof(WindowsPrecisionTrackpad),
                6 * sizeof(nint)
            );
        table[0] = (nint)(delegate* unmanaged[Stdcall]<Callback*, Guid*, nint*, int>)&Query;
        table[1] = (nint)(delegate* unmanaged[Stdcall]<Callback*, uint>)&AddRef;
        table[2] = (nint)(delegate* unmanaged[Stdcall]<Callback*, uint>)&ReleaseRef;
        table[3] = (nint)(delegate* unmanaged[Stdcall]<Callback*, nint, uint, uint, int>)&OnStatus;
        table[4] = (nint)(delegate* unmanaged[Stdcall]<Callback*, nint, int>)&OnViewport;
        table[5] = (nint)(delegate* unmanaged[Stdcall]<Callback*, nint, nint, int>)&OnContent;
        return table;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static int Query(Callback* self, Guid* iid, nint* result)
    {
        if (result == null)
        {
            return unchecked((int)0x80004003);
        }

        *result = 0;
        if (*iid != UnknownId && *iid != HandlerId)
        {
            return unchecked((int)0x80004002);
        }

        *result = (nint)self;
        Interlocked.Increment(ref self->References);
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static uint AddRef(Callback* self) => (uint)Interlocked.Increment(ref self->References);

    private static uint ReleaseCallback(Callback* self)
    {
        var count = Interlocked.Decrement(ref self->References);
        if (count == 0)
        {
            GCHandle.FromIntPtr(self->Owner).Free();
            NativeMemory.Free(self);
        }
        return (uint)count;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static uint ReleaseRef(Callback* self) => ReleaseCallback(self);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static int OnStatus(Callback* self, nint viewport, uint current, uint previous)
    {
        try
        {
            Owner(self).Status(viewport, current, previous);
            return 0;
        }
        catch (Exception e)
        {
            return Marshal.GetHRForException(e);
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static int OnViewport(Callback* self, nint viewport) => 0;

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static int OnContent(Callback* self, nint viewport, nint content)
    {
        try
        {
            Owner(self).Content(content);
            return 0;
        }
        catch (Exception e)
        {
            return Marshal.GetHRForException(e);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int Left,
            Top,
            Right,
            Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Point
    {
        public int X,
            Y;
    }

    private delegate nint SubclassProc(
        nint hwnd,
        uint message,
        nuint wparam,
        nint lparam,
        nuint id,
        nuint data
    );

    [DllImport("ole32.dll")]
    private static extern int CoCreateInstance(
        Guid* clsid,
        nint outer,
        uint context,
        Guid* iid,
        nint* result
    );

    [DllImport("comctl32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowSubclass(
        nint hwnd,
        SubclassProc proc,
        nuint id,
        nuint data
    );

    [DllImport("comctl32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool RemoveWindowSubclass(nint hwnd, SubclassProc proc, nuint id);

    [DllImport("comctl32.dll")]
    private static extern nint DefSubclassProc(nint hwnd, uint message, nuint wparam, nint lparam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetClientRect(nint hwnd, out Rect rect);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetCursorPos(out Point point);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ScreenToClient(nint hwnd, ref Point point);

    [DllImport("user32.dll")]
    private static extern nint GetAncestor(nint hwnd, uint flags);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetPointerType(uint id, out uint type);

    [DllImport("user32.dll")]
    private static extern nuint SetTimer(nint hwnd, nuint id, uint interval, nint callback);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool KillTimer(nint hwnd, nuint id);
}
