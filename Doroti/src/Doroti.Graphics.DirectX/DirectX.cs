using Silk.NET.Core.Native;
using D11 = Silk.NET.Direct3D11;
using D12 = Silk.NET.Direct3D12;
using DX = Silk.NET.DXGI;
using On12 = Silk.NET.Direct3D11.Extensions.D3D11On12;

namespace Doroti.Graphics.DirectX;

public static unsafe class DirectX
{
    private static string WindowsLibrary(string name) => Path.Combine(Environment.SystemDirectory, name);

    // COM references do not keep a NativeLibrary load alive. Keep API modules loaded
    // for the process lifetime so objects' vtables remain valid after factory calls.
    private static class DxgiModule
    {
        internal static readonly DX.DXGI Api = new(DX.DXGI.CreateDefaultContext([WindowsLibrary("dxgi.dll")]));
    }

    private static class D3D12Module
    {
        internal static readonly D12.D3D12 Api = new(D12.D3D12.CreateDefaultContext([WindowsLibrary("d3d12.dll")]));
    }

    private static class D3D11Module
    {
        internal static readonly D11.D3D11 Api = new(D11.D3D11.CreateDefaultContext([WindowsLibrary("d3d11.dll")]));
    }

    private static class On12Module
    {
        internal static readonly On12.D3D11On12 Api = new(On12.D3D11On12.CreateDefaultContext([WindowsLibrary("d3d11.dll")]));
    }

    public static T CreateDXGIFactory2<T>(bool debug)
        where T : ComObject, IComOwner<T>
    {
        var api = DxgiModule.Api;
        var iid = T.InterfaceId;
        void* pointer = null;
        var hr = api.CreateDXGIFactory2(debug ? 1u : 0, &iid, &pointer);
        return ComObject.Adopt<T>(hr, pointer);
    }

    public static T D3D12CreateDevice<T>(IDXGIAdapter1 adapter, D3DFeatureLevel level)
        where T : ComObject, IComOwner<T>
    {
        using var lifetime = new ComScope(adapter);
        var api = D3D12Module.Api;
        var iid = T.InterfaceId;
        void* pointer = null;
        var hr = api.CreateDevice((IUnknown*)adapter.NativePointer, level, &iid, &pointer);
        return ComObject.Adopt<T>(hr, pointer);
    }

    public static T D3D12GetDebugInterface<T>()
        where T : ComObject, IComOwner<T>
    {
        var api = D3D12Module.Api;
        var iid = T.InterfaceId;
        void* pointer = null;
        var hr = api.GetDebugInterface(&iid, &pointer);
        return ComObject.Adopt<T>(hr, pointer);
    }

    public static HResult D3D11CreateDevice(IDXGIAdapter1? adapter, D3DDriverType driver, D11.CreateDeviceFlag flags, D3DFeatureLevel[] levels, out ID3D11Device device, out D3DFeatureLevel chosen, out ID3D11DeviceContext context)
    {
        using var lifetime = new ComScope(adapter);
        var api = D3D11Module.Api;
        D11.ID3D11Device* nativeDevice = null;
        D11.ID3D11DeviceContext* nativeContext = null;
        D3DFeatureLevel nativeLevel = default;
        int hr;
        fixed (D3DFeatureLevel* levelPointer = levels)
            hr = api.CreateDevice((DX.IDXGIAdapter*)(adapter?.NativePointer ?? 0), driver, 0, (uint)flags, levelPointer, (uint)levels.Length, D11.D3D11.SdkVersion, &nativeDevice, &nativeLevel, &nativeContext);
        chosen = nativeLevel;
        return WrapDevice(hr, nativeDevice, nativeContext, out device, out context);
    }

    public static HResult D3D11On12CreateDevice(ID3D12Device device12, D11.CreateDeviceFlag flags, D3DFeatureLevel[] levels, ID3D12CommandQueue[] queues, uint nodeMask, out ID3D11Device device, out ID3D11DeviceContext context, out D3DFeatureLevel chosen)
    {
        using var lifetime = new ComScope(device12, queues);
        var api = On12Module.Api;
        var queuePointers = queues.Select(queue => queue.NativePointer).ToArray();
        D11.ID3D11Device* nativeDevice = null;
        D11.ID3D11DeviceContext* nativeContext = null;
        D3DFeatureLevel nativeLevel = default;
        int hr;
        fixed (D3DFeatureLevel* levelPointer = levels)
        fixed (nint* queuePointer = queuePointers)
            hr = api.On12CreateDevice((IUnknown*)device12.NativePointer, (uint)flags, levelPointer, (uint)levels.Length, (IUnknown**)queuePointer, (uint)queues.Length, nodeMask, &nativeDevice, &nativeContext, &nativeLevel);
        chosen = nativeLevel;
        return WrapDevice(hr, nativeDevice, nativeContext, out device, out context);
    }

    private static HResult WrapDevice(int hr, D11.ID3D11Device* nativeDevice, D11.ID3D11DeviceContext* nativeContext, out ID3D11Device device, out ID3D11DeviceContext context)
    {
        if (hr < 0)
        {
            if (nativeContext != null)
                nativeContext->Release();
            if (nativeDevice != null)
                nativeDevice->Release();
            device = null!;
            context = null!;
            return hr;
        }

        device = new((nint)nativeDevice);
        context = new((nint)nativeContext);
        return hr;
    }
}
