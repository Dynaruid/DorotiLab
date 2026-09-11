using System.Runtime.InteropServices;
using Silk.NET.Core.Native;

namespace Doroti.Graphics.DirectX;

/// <summary>A statically constructed owner of one COM reference; no reflection activation.</summary>
public interface IComOwner<T> where T : ComObject
{
    static abstract Guid InterfaceId { get; }
    static abstract T Attach(nint pointer);
}

/// <summary>
/// Adopts one existing COM reference. QueryInterface and successful native factories
/// supply that reference. Borrowed pointers must be AddRef'd by their caller first.
/// Owners are confined to their host thread; Dispose is idempotent.
/// </summary>
public abstract unsafe class ComObject : IDisposable
{
    private nint _pointer;
    protected ComObject(nint pointer)
    {
        if (pointer == 0) throw new ArgumentNullException(nameof(pointer));
        _pointer = pointer;
    }

    public nint NativePointer
    {
        get
        {
            ObjectDisposedException.ThrowIf(_pointer == 0, this);
            return _pointer;
        }
    }

    public T QueryInterface<T>() where T : ComObject, IComOwner<T>
    {
        using var lifetime = new ComScope(this);
        var iid = T.InterfaceId;
        void* result = null;
        var hr = ((IUnknown*)NativePointer)->QueryInterface(&iid, &result);
        return Adopt<T>(hr, result);
    }

    internal static T Adopt<T>(int hr, void* pointer) where T : ComObject, IComOwner<T>
    {
        if (hr < 0)
        {
            if (pointer != null) ((IUnknown*)pointer)->Release();
            new HResult(hr).CheckError();
        }
        return T.Attach((nint)pointer);
    }

    public void Dispose()
    {
        Release();
        GC.SuppressFinalize(this);
    }

    private void Release()
    {
        var pointer = Interlocked.Exchange(ref _pointer, 0);
        if (pointer != 0) ((IUnknown*)pointer)->Release();
    }

    ~ComObject() => Release();
}

// Retain owners through the complete native call, including JIT last-use analysis.
// An extracted unmanaged pointer by itself does not root its finalizable owner.
internal readonly ref struct ComScope(object? first, object? second = null, object? third = null)
{
    public void Dispose()
    {
        GC.KeepAlive(first);
        GC.KeepAlive(second);
        GC.KeepAlive(third);
    }
}

public readonly record struct HResult(int Code)
{
    public bool Success => Code >= 0;
    public bool Failure => Code < 0;
    public void CheckError()
    {
        if (Failure) throw new COMException($"DirectX call failed: 0x{Code:X8}", Code);
    }
    public static implicit operator HResult(int code) => new(code);
    public override string ToString() => $"0x{Code:X8}";
}
