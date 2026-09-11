using Silk.NET.Core.Native;
using D12 = Silk.NET.Direct3D12;
using DX = Silk.NET.DXGI;

namespace Doroti.Graphics.DirectX;

public unsafe partial class ID3D12Device
{
    public Luid AdapterLuid
    {
        get
        {
            using var lifetime = new ComScope(this);
            return Native->GetAdapterLuid();
        }
    }

    public HResult DeviceRemovedReason
    {
        get
        {
            using var lifetime = new ComScope(this);
            return Native->GetDeviceRemovedReason();
        }
    }

    public ID3D12CommandQueue CreateCommandQueue(D12.CommandListType type, int priority = 0, D12.CommandQueueFlags flags = D12.CommandQueueFlags.None, uint nodeMask = 0)
    {
        using var lifetime = new ComScope(this);
        var desc = new D12.CommandQueueDesc(type, priority, flags, nodeMask);
        var iid = ID3D12CommandQueue.InterfaceId;
        void* pointer = null;
        var hr = Native->CreateCommandQueue(&desc, &iid, &pointer);
        return Adopt<ID3D12CommandQueue>(hr, pointer);
    }

    public ID3D12CommandAllocator CreateCommandAllocator(D12.CommandListType type)
    {
        using var lifetime = new ComScope(this);
        var iid = ID3D12CommandAllocator.InterfaceId;
        void* pointer = null;
        var hr = Native->CreateCommandAllocator(type, &iid, &pointer);
        return Adopt<ID3D12CommandAllocator>(hr, pointer);
    }

    public T CreateCommandList<T>(D12.CommandListType type, ID3D12CommandAllocator allocator, ComObject? initialState)
        where T : ComObject, IComOwner<T>
    {
        using var lifetime = new ComScope(this, allocator, initialState);
        var iid = T.InterfaceId;
        void* pointer = null;
        var hr = Native->CreateCommandList(0, type, allocator.Native, (D12.ID3D12PipelineState*)(initialState?.NativePointer ?? 0), &iid, &pointer);
        return Adopt<T>(hr, pointer);
    }

    public ID3D12Fence CreateFence(ulong initialValue, D12.FenceFlags flags = D12.FenceFlags.None)
    {
        using var lifetime = new ComScope(this);
        var iid = ID3D12Fence.InterfaceId;
        void* pointer = null;
        var hr = Native->CreateFence(initialValue, flags, &iid, &pointer);
        return Adopt<ID3D12Fence>(hr, pointer);
    }

    public ID3D12Resource CreateCommittedResource(D12.HeapType heap, D12.HeapFlags flags, ResourceDescription description, D12.ResourceStates state, D12.ClearValue? clearValue)
    {
        using var lifetime = new ComScope(this);
        var properties = new D12.HeapProperties
        {
            Type = heap,
            CreationNodeMask = 1,
            VisibleNodeMask = 1
        };
        var desc = description.Native;
        var clear = clearValue.GetValueOrDefault();
        var iid = ID3D12Resource.InterfaceId;
        void* pointer = null;
        var hr = Native->CreateCommittedResource(&properties, flags, &desc, state, clearValue.HasValue ? &clear : null, &iid, &pointer);
        return Adopt<ID3D12Resource>(hr, pointer);
    }

    public nint CreateSharedHandle(ID3D12Resource resource, SecurityAttributes? attributes, string? name)
    {
        using var lifetime = new ComScope(this, resource);
        var security = attributes.GetValueOrDefault();
        void* handle = null;
        fixed (char* text = name)
            new HResult(Native->CreateSharedHandle((D12.ID3D12DeviceChild*)resource.NativePointer, attributes.HasValue ? &security : null, 0x10000000 /* GENERIC_ALL */, text, &handle)).CheckError();
        return (nint)handle;
    }
}

public unsafe partial class ID3D12CommandAllocator
{
    public void Reset()
    {
        using var lifetime = new ComScope(this);
        new HResult(Native->Reset()).CheckError();
    }
}

public unsafe partial class ID3D12GraphicsCommandList
{
    public void Close()
    {
        using var lifetime = new ComScope(this);
        new HResult(Native->Close()).CheckError();
    }

    public void Reset(ID3D12CommandAllocator allocator)
    {
        using var lifetime = new ComScope(this, allocator);
        new HResult(Native->Reset(allocator.Native, (D12.ID3D12PipelineState*)null)).CheckError();
    }

    public void CopyResource(ID3D12Resource destination, ID3D12Resource source)
    {
        using var lifetime = new ComScope(this, destination, source);
        Native->CopyResource(destination.Native, source.Native);
    }

    public void ResourceBarrier(ResourceBarrier[] barriers)
    {
        using var lifetime = new ComScope(this);
        var native = new D12.ResourceBarrier[barriers.Length];
        for (var i = 0; i < barriers.Length; i++)
            native[i] = barriers[i].Native;
        fixed (D12.ResourceBarrier* pointer = native)
            Native->ResourceBarrier((uint)native.Length, pointer);
        GC.KeepAlive(barriers);
    }

    public void ResourceBarrierTransition(ID3D12Resource resource, D12.ResourceStates before, D12.ResourceStates after)
    {
        using var lifetime = new ComScope(this, resource);
        ResourceBarrier([global::Doroti.Graphics.DirectX.ResourceBarrier.BarrierTransition(resource, before, after)]);
    }

    public void CopyTextureRegion(TextureCopyLocation destination, uint x, uint y, uint z, TextureCopyLocation source, D12.Box? sourceBox)
    {
        using var lifetime = new ComScope(this);
        var dest = destination.Native;
        var src = source.Native;
        var box = sourceBox.GetValueOrDefault();
        Native->CopyTextureRegion(&dest, x, y, z, &src, sourceBox.HasValue ? &box : null);
        GC.KeepAlive(destination);
        GC.KeepAlive(source);
    }
}

public unsafe partial class ID3D12CommandQueue
{
    public void ExecuteCommandList(ID3D12GraphicsCommandList commands)
    {
        using var lifetime = new ComScope(this, commands);
        var pointer = (D12.ID3D12CommandList*)commands.NativePointer;
        Native->ExecuteCommandLists(1, &pointer);
        GC.KeepAlive(commands);
    }

    public HResult Signal(ID3D12Fence fence, ulong value)
    {
        using var lifetime = new ComScope(this, fence);
        return Native->Signal(fence.Native, value);
    }
}

public unsafe partial class ID3D12Fence
{
    public ulong CompletedValue
    {
        get
        {
            using var lifetime = new ComScope(this);
            return Native->GetCompletedValue();
        }
    }

    public HResult SetEventOnCompletion(ulong value, EventWaitHandle completion)
    {
        using var lifetime = new ComScope(this);
        var safeHandle = completion.SafeWaitHandle;
        var added = false;
        try
        {
            safeHandle.DangerousAddRef(ref added);
            return Native->SetEventOnCompletion(value, (void*)safeHandle.DangerousGetHandle());
        }
        finally
        {
            if (added)
                safeHandle.DangerousRelease();
        }
    }
}

public unsafe partial class ID3D12Resource
{
    public ResourceDescription Description
    {
        get
        {
            using var lifetime = new ComScope(this);
            return new(Native->GetDesc());
        }
    }

    public HResult Map(uint subresource, D12.Range* range, void** data)
    {
        using var lifetime = new ComScope(this);
        return Native->Map(subresource, range, data);
    }

    public void Unmap(uint subresource)
    {
        using var lifetime = new ComScope(this);
        Native->Unmap(subresource, (D12.Range*)null);
    }
}

public unsafe partial class ID3D12Debug
{
    public void EnableDebugLayer()
    {
        using var lifetime = new ComScope(this);
        Native->EnableDebugLayer();
    }
}

public readonly record struct DebugMessage(D12.MessageSeverity Severity, D12.MessageID Id, string Description);
public unsafe partial class ID3D12InfoQueue
{
    public ulong NumStoredMessages
    {
        get
        {
            using var lifetime = new ComScope(this);
            return Native->GetNumStoredMessages();
        }
    }

    public void ClearStoredMessages()
    {
        using var lifetime = new ComScope(this);
        Native->ClearStoredMessages();
    }

    public DebugMessage GetMessage(ulong index)
    {
        using var lifetime = new ComScope(this);
        nuint size = 0;
        new HResult(Native->GetMessageA(index, null, &size)).CheckError();
        var storage = new byte[checked((int)size)];
        fixed (byte* pointer = storage)
        {
            var message = (D12.Message*)pointer;
            new HResult(Native->GetMessageA(index, message, &size)).CheckError();
            return new(message->Severity, message->ID, System.Runtime.InteropServices.Marshal.PtrToStringAnsi((nint)message->PDescription) ?? "");
        }
    }
}

public readonly struct ResourceDescription(D12.ResourceDesc native)
{
    internal D12.ResourceDesc Native => native;
    public DX.Format Format => native.Format;
    public ulong Width => native.Width;
    public uint Height => native.Height;
    public DX.SampleDesc SampleDescription => native.SampleDesc;

    public static ResourceDescription Texture2D(DX.Format format, uint width, uint height, ushort arraySize, ushort mipLevels, uint sampleCount, uint sampleQuality, D12.ResourceFlags flags) => new(new D12.ResourceDesc { Dimension = D12.ResourceDimension.Texture2D, Width = width, Height = height, DepthOrArraySize = arraySize, MipLevels = mipLevels, Format = format, SampleDesc = new(sampleCount, sampleQuality), Layout = D12.TextureLayout.LayoutUnknown, Flags = flags, });
    public static ResourceDescription Buffer(ulong size) => new(new D12.ResourceDesc { Dimension = D12.ResourceDimension.Buffer, Width = size, Height = 1, DepthOrArraySize = 1, MipLevels = 1, SampleDesc = new(1, 0), Layout = D12.TextureLayout.LayoutRowMajor, });
}

public readonly unsafe struct ResourceBarrier
{
    private readonly ID3D12Resource _resource;
    private readonly D12.ResourceStates _before;
    private readonly D12.ResourceStates _after;
    private ResourceBarrier(ID3D12Resource resource, D12.ResourceStates before, D12.ResourceStates after) => (_resource, _before, _after) = (resource, before, after);
    internal D12.ResourceBarrier Native => new()
    {
        Type = D12.ResourceBarrierType.Transition,
        Anonymous = new D12.ResourceBarrierUnion
        {
            Transition = new D12.ResourceTransitionBarrier
            {
                PResource = _resource.Native,
                Subresource = uint.MaxValue,
                StateBefore = _before,
                StateAfter = _after,
            },
        },
    };

    public static ResourceBarrier BarrierTransition(ID3D12Resource resource, D12.ResourceStates before, D12.ResourceStates after) => new(resource, before, after);
}

public readonly unsafe struct TextureCopyLocation
{
    private readonly ID3D12Resource _resource;
    private readonly D12.TextureCopyLocation _location;
    public TextureCopyLocation(ID3D12Resource resource, uint subresource)
    {
        _resource = resource;
        _location = new()
        {
            Type = D12.TextureCopyType.SubresourceIndex,
            Anonymous = new D12.TextureCopyLocationUnion
            {
                SubresourceIndex = subresource
            }
        };
    }

    public TextureCopyLocation(ID3D12Resource resource, D12.PlacedSubresourceFootprint footprint)
    {
        _resource = resource;
        _location = new()
        {
            Type = D12.TextureCopyType.PlacedFootprint,
            Anonymous = new D12.TextureCopyLocationUnion
            {
                PlacedFootprint = footprint
            }
        };
    }

    internal D12.TextureCopyLocation Native
    {
        get
        {
            var location = _location;
            location.PResource = _resource.Native;
            return location;
        }
    }
}
