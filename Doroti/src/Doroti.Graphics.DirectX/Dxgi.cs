using System.Numerics;
using Silk.NET.Core.Native;
using DX = Silk.NET.DXGI;

namespace Doroti.Graphics.DirectX;

public unsafe partial class IDXGIFactory2
{
    public HResult EnumAdapters1(uint index, out IDXGIAdapter1 adapter)
    {
        using var lifetime = new ComScope(this);
        DX.IDXGIAdapter1* pointer = null;
        var hr = Native->EnumAdapters1(index, &pointer);
        adapter = hr >= 0 ? new((nint)pointer) : null!;
        return hr;
    }

    public IDXGISwapChain1 CreateSwapChainForHwnd(ComObject device, nint window, SwapChainDescription1 description, DX.SwapChainFullscreenDesc? fullscreen, IDXGIOutput? output)
    {
        using var lifetime = new ComScope(this, device, output);
        var desc = description.Native;
        var full = fullscreen.GetValueOrDefault();
        DX.IDXGISwapChain1* pointer = null;
        var hr = Native->CreateSwapChainForHwnd((IUnknown*)device.NativePointer, window, &desc, fullscreen.HasValue ? &full : null, (DX.IDXGIOutput*)(output?.NativePointer ?? 0), &pointer);
        return Adopt<IDXGISwapChain1>(hr, pointer);
    }

    public IDXGISwapChain1 CreateSwapChainForComposition(ComObject device, SwapChainDescription1 description, IDXGIOutput? output)
    {
        using var lifetime = new ComScope(this, device, output);
        var desc = description.Native;
        DX.IDXGISwapChain1* pointer = null;
        var hr = Native->CreateSwapChainForComposition((IUnknown*)device.NativePointer, &desc, (DX.IDXGIOutput*)(output?.NativePointer ?? 0), &pointer);
        return Adopt<IDXGISwapChain1>(hr, pointer);
    }
}

public unsafe partial class IDXGIFactory6
{
    public HResult EnumAdapterByGpuPreference(uint index, DX.GpuPreference preference, out IDXGIAdapter1? adapter)
    {
        using var lifetime = new ComScope(this);
        var iid = IDXGIAdapter1.InterfaceId;
        void* pointer = null;
        var hr = Native->EnumAdapterByGpuPreference(index, preference, &iid, &pointer);
        adapter = hr >= 0 ? new((nint)pointer) : null;
        return hr;
    }

    public T EnumAdapterByGpuPreference<T>(uint index, DX.GpuPreference preference)
        where T : ComObject, IComOwner<T>
    {
        using var lifetime = new ComScope(this);
        var iid = T.InterfaceId;
        void* pointer = null;
        var hr = Native->EnumAdapterByGpuPreference(index, preference, &iid, &pointer);
        return Adopt<T>(hr, pointer);
    }

    public T EnumAdapterByLuid<T>(Luid luid)
        where T : ComObject, IComOwner<T>
    {
        using var lifetime = new ComScope(this);
        var iid = T.InterfaceId;
        void* pointer = null;
        var hr = Native->EnumAdapterByLuid(luid, &iid, &pointer);
        return Adopt<T>(hr, pointer);
    }
}

public readonly record struct AdapterDescription(string Description, uint Flags, Luid Luid);
public unsafe partial class IDXGIAdapter1
{
    public AdapterDescription Description1
    {
        get
        {
            using var lifetime = new ComScope(this);
            DX.AdapterDesc1 desc;
            new HResult(Native->GetDesc1(&desc)).CheckError();
            return new(new string (desc.Description), desc.Flags, desc.AdapterLuid);
        }
    }

    public HResult EnumOutputs(uint index, out IDXGIOutput output)
    {
        using var lifetime = new ComScope(this);
        DX.IDXGIOutput* pointer = null;
        var hr = Native->EnumOutputs(index, &pointer);
        output = hr >= 0 ? new((nint)pointer) : null!;
        return hr;
    }
}

public unsafe partial class IDXGIOutput
{
    public DX.OutputDesc Description
    {
        get
        {
            using var lifetime = new ComScope(this);
            DX.OutputDesc desc;
            new HResult(Native->GetDesc(&desc)).CheckError();
            return desc;
        }
    }
}

public unsafe partial class IDXGISwapChain1
{
    public HResult ResizeBuffers(uint count, uint width, uint height, DX.Format format, DX.SwapChainFlag flags)
    {
        using var lifetime = new ComScope(this);
        return Native->ResizeBuffers(count, width, height, format, (uint)flags);
    }

    public HResult Present(uint interval, uint flags)
    {
        using var lifetime = new ComScope(this);
        return Native->Present(interval, flags);
    }

    public T GetBuffer<T>(uint index)
        where T : ComObject, IComOwner<T>
    {
        using var lifetime = new ComScope(this);
        var iid = T.InterfaceId;
        void* pointer = null;
        var hr = Native->GetBuffer(index, &iid, &pointer);
        return Adopt<T>(hr, pointer);
    }
}

public unsafe partial class IDXGISwapChain2
{
    public uint MaximumFrameLatency
    {
        set
        {
            using var lifetime = new ComScope(this);
            new HResult(Native->SetMaximumFrameLatency(value)).CheckError();
        }
    }

    public nint FrameLatencyWaitableObject
    {
        get
        {
            using var lifetime = new ComScope(this);
            return (nint)Native->GetFrameLatencyWaitableObject();
        }
    }

    public Matrix3x2 MatrixTransform
    {
        set
        {
            using var lifetime = new ComScope(this);
            var matrix = new DX.Matrix3X2F(value.M11, value.M12, value.M21, value.M22, value.M31, value.M32);
            new HResult(Native->SetMatrixTransform(&matrix)).CheckError();
        }
    }

    public HResult SetSourceSize(uint width, uint height)
    {
        using var lifetime = new ComScope(this);
        return Native->SetSourceSize(width, height);
    }
}

public unsafe partial class IDXGISwapChain3
{
    public uint CurrentBackBufferIndex
    {
        get
        {
            using var lifetime = new ComScope(this);
            return Native->GetCurrentBackBufferIndex();
        }
    }
}

public readonly struct SwapChainDescription1(uint width, uint height, DX.Format format, bool stereo, uint usage, uint bufferCount, DX.Scaling scaling, DX.SwapEffect swapEffect, DX.AlphaMode alphaMode, DX.SwapChainFlag flags)
{
    internal DX.SwapChainDesc1 Native => new()
    {
        Width = width,
        Height = height,
        Format = format,
        Stereo = stereo,
        SampleDesc = new(1, 0),
        BufferUsage = usage,
        BufferCount = bufferCount,
        Scaling = scaling,
        SwapEffect = swapEffect,
        AlphaMode = alphaMode,
        Flags = (uint)flags,
    };
}

public static class Usage
{
    public const uint RenderTargetOutput = DX.DXGI.UsageRenderTargetOutput;
}

public static class PresentFlags
{
    public const uint None = 0;
}

public static class AdapterFlags
{
    public const uint Software = 2;
}

public static class ResultCode
{
    public static HResult NotFound => new(unchecked((int)0x887A0002));
}

/// <summary>The WinUI ISwapChainPanelNative ABI is not included in Silk's DXGI package.</summary>
public sealed unsafe class ISwapChainPanelNative(nint pointer) : ComObject(pointer)
{
    public HResult SetSwapChain(IDXGISwapChain3 swapChain)
    {
        using var lifetime = new ComScope(this, swapChain);
        var pointer = NativePointer;
        var call = (delegate* unmanaged[Stdcall]<nint, nint, int> )(*(void***)pointer)[3];
        return call(pointer, swapChain.NativePointer);
    }
}
