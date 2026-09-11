using D11 = Silk.NET.Direct3D11;
using D12 = Silk.NET.Direct3D12;
using DX = Silk.NET.DXGI;

namespace Doroti.Graphics.DirectX;

public readonly struct Texture2DDescription(D11.Texture2DDesc native)
{
    public uint Width => native.Width;
    public uint Height => native.Height;
    public DX.Format Format => native.Format;
    public DX.SampleDesc SampleDescription => native.SampleDesc;
    public D11.Usage Usage => native.Usage;
    public D11.BindFlag BindFlags => (D11.BindFlag)native.BindFlags;
}

public unsafe partial class ID3D11Texture2D
{
    public Texture2DDescription Description
    {
        get
        {
            using var lifetime = new ComScope(this);
            D11.Texture2DDesc desc;
            Native->GetDesc(&desc);
            return new(desc);
        }
    }
}

public unsafe partial class ID3D11DeviceContext
{
    public void ClearState()
    {
        using var lifetime = new ComScope(this);
        Native->ClearState();
    }

    public void Flush()
    {
        using var lifetime = new ComScope(this);
        Native->Flush();
    }

    public void UpdateSubresource(uint[] data, ID3D11Texture2D texture, uint subresource, uint rowPitch, uint depthPitch, D11.Box? box)
    {
        using var lifetime = new ComScope(this, texture);
        var region = box.GetValueOrDefault();
        fixed (uint* pointer = data)
            Native->UpdateSubresource((D11.ID3D11Resource*)texture.NativePointer, subresource, box.HasValue ? &region : null, pointer, rowPitch, depthPitch);
        GC.KeepAlive(texture);
    }
}

public unsafe partial class ID3D11On12Device2
{
    public T UnwrapUnderlyingResource<T>(ID3D11Texture2D texture, ID3D12CommandQueue queue)
        where T : ComObject, IComOwner<T>
    {
        using var lifetime = new ComScope(this, texture, queue);
        var iid = T.InterfaceId;
        void* pointer = null;
        var hr = Native->UnwrapUnderlyingResource((D11.ID3D11Resource*)texture.NativePointer, queue.Native, &iid, &pointer);
        return Adopt<T>(hr, pointer);
    }

    public HResult ReturnUnderlyingResource(ID3D11Texture2D texture, ulong[] values, ID3D12Fence[] fences)
    {
        using var lifetime = new ComScope(this, texture, fences);
        if (values.Length != fences.Length)
            throw new ArgumentException("Each fence requires a value.");
        var pointers = fences.Select(fence => fence.NativePointer).ToArray();
        int hr;
        fixed (ulong* valuePointer = values)
        fixed (nint* fencePointer = pointers)
            hr = Native->ReturnUnderlyingResource((D11.ID3D11Resource*)texture.NativePointer, (uint)fences.Length, valuePointer, (D12.ID3D12Fence**)fencePointer);
        GC.KeepAlive(fences);
        GC.KeepAlive(texture);
        return hr;
    }
}
