// Managed ownership only. All DirectX calls are dispatched through Silk.NET bindings.
namespace Doroti.Graphics.DirectX;

public unsafe partial class IDXGIFactory2(nint pointer) : ComObject(pointer), IComOwner<IDXGIFactory2>
{
    public static Guid InterfaceId => typeof(Silk.NET.DXGI.IDXGIFactory2).GUID;
    public static IDXGIFactory2 Attach(nint pointer) => new(pointer);
    internal Silk.NET.DXGI.IDXGIFactory2* Native => (Silk.NET.DXGI.IDXGIFactory2*)NativePointer;
}

public unsafe partial class IDXGIFactory6(nint pointer) : IDXGIFactory2(pointer), IComOwner<IDXGIFactory6>
{
    public new static Guid InterfaceId => typeof(Silk.NET.DXGI.IDXGIFactory6).GUID;
    public new static IDXGIFactory6 Attach(nint pointer) => new(pointer);
    internal new Silk.NET.DXGI.IDXGIFactory6* Native => (Silk.NET.DXGI.IDXGIFactory6*)NativePointer;
}

public unsafe partial class IDXGIAdapter1(nint pointer) : ComObject(pointer), IComOwner<IDXGIAdapter1>
{
    public static Guid InterfaceId => typeof(Silk.NET.DXGI.IDXGIAdapter1).GUID;
    public static IDXGIAdapter1 Attach(nint pointer) => new(pointer);
    internal Silk.NET.DXGI.IDXGIAdapter1* Native => (Silk.NET.DXGI.IDXGIAdapter1*)NativePointer;
}

public unsafe partial class IDXGIOutput(nint pointer) : ComObject(pointer), IComOwner<IDXGIOutput>
{
    public static Guid InterfaceId => typeof(Silk.NET.DXGI.IDXGIOutput).GUID;
    public static IDXGIOutput Attach(nint pointer) => new(pointer);
    internal Silk.NET.DXGI.IDXGIOutput* Native => (Silk.NET.DXGI.IDXGIOutput*)NativePointer;
}

public unsafe partial class IDXGISwapChain1(nint pointer) : ComObject(pointer), IComOwner<IDXGISwapChain1>
{
    public static Guid InterfaceId => typeof(Silk.NET.DXGI.IDXGISwapChain1).GUID;
    public static IDXGISwapChain1 Attach(nint pointer) => new(pointer);
    internal Silk.NET.DXGI.IDXGISwapChain1* Native => (Silk.NET.DXGI.IDXGISwapChain1*)NativePointer;
}

public unsafe partial class IDXGISwapChain2(nint pointer) : IDXGISwapChain1(pointer), IComOwner<IDXGISwapChain2>
{
    public new static Guid InterfaceId => typeof(Silk.NET.DXGI.IDXGISwapChain2).GUID;
    public new static IDXGISwapChain2 Attach(nint pointer) => new(pointer);
    internal new Silk.NET.DXGI.IDXGISwapChain2* Native => (Silk.NET.DXGI.IDXGISwapChain2*)NativePointer;
}

public unsafe partial class IDXGISwapChain3(nint pointer) : IDXGISwapChain2(pointer), IComOwner<IDXGISwapChain3>
{
    public new static Guid InterfaceId => typeof(Silk.NET.DXGI.IDXGISwapChain3).GUID;
    public new static IDXGISwapChain3 Attach(nint pointer) => new(pointer);
    internal new Silk.NET.DXGI.IDXGISwapChain3* Native => (Silk.NET.DXGI.IDXGISwapChain3*)NativePointer;
}

public unsafe partial class ID3D12Device(nint pointer) : ComObject(pointer), IComOwner<ID3D12Device>
{
    public static Guid InterfaceId => typeof(Silk.NET.Direct3D12.ID3D12Device).GUID;
    public static ID3D12Device Attach(nint pointer) => new(pointer);
    internal Silk.NET.Direct3D12.ID3D12Device* Native => (Silk.NET.Direct3D12.ID3D12Device*)NativePointer;
}

public unsafe partial class ID3D12Device2(nint pointer) : ID3D12Device(pointer), IComOwner<ID3D12Device2>
{
    public new static Guid InterfaceId => typeof(Silk.NET.Direct3D12.ID3D12Device2).GUID;
    public new static ID3D12Device2 Attach(nint pointer) => new(pointer);
    internal new Silk.NET.Direct3D12.ID3D12Device2* Native => (Silk.NET.Direct3D12.ID3D12Device2*)NativePointer;
}

public unsafe partial class ID3D12CommandQueue(nint pointer) : ComObject(pointer), IComOwner<ID3D12CommandQueue>
{
    public static Guid InterfaceId => typeof(Silk.NET.Direct3D12.ID3D12CommandQueue).GUID;
    public static ID3D12CommandQueue Attach(nint pointer) => new(pointer);
    internal Silk.NET.Direct3D12.ID3D12CommandQueue* Native => (Silk.NET.Direct3D12.ID3D12CommandQueue*)NativePointer;
}

public unsafe partial class ID3D12CommandAllocator(nint pointer) : ComObject(pointer), IComOwner<ID3D12CommandAllocator>
{
    public static Guid InterfaceId => typeof(Silk.NET.Direct3D12.ID3D12CommandAllocator).GUID;
    public static ID3D12CommandAllocator Attach(nint pointer) => new(pointer);
    internal Silk.NET.Direct3D12.ID3D12CommandAllocator* Native => (Silk.NET.Direct3D12.ID3D12CommandAllocator*)NativePointer;
}

public unsafe partial class ID3D12GraphicsCommandList(nint pointer) : ComObject(pointer), IComOwner<ID3D12GraphicsCommandList>
{
    public static Guid InterfaceId => typeof(Silk.NET.Direct3D12.ID3D12GraphicsCommandList).GUID;
    public static ID3D12GraphicsCommandList Attach(nint pointer) => new(pointer);
    internal Silk.NET.Direct3D12.ID3D12GraphicsCommandList* Native => (Silk.NET.Direct3D12.ID3D12GraphicsCommandList*)NativePointer;
}

public unsafe partial class ID3D12Fence(nint pointer) : ComObject(pointer), IComOwner<ID3D12Fence>
{
    public static Guid InterfaceId => typeof(Silk.NET.Direct3D12.ID3D12Fence).GUID;
    public static ID3D12Fence Attach(nint pointer) => new(pointer);
    internal Silk.NET.Direct3D12.ID3D12Fence* Native => (Silk.NET.Direct3D12.ID3D12Fence*)NativePointer;
}

public unsafe partial class ID3D12Resource(nint pointer) : ComObject(pointer), IComOwner<ID3D12Resource>
{
    public static Guid InterfaceId => typeof(Silk.NET.Direct3D12.ID3D12Resource).GUID;
    public static ID3D12Resource Attach(nint pointer) => new(pointer);
    internal Silk.NET.Direct3D12.ID3D12Resource* Native => (Silk.NET.Direct3D12.ID3D12Resource*)NativePointer;
}

public unsafe partial class ID3D12Debug(nint pointer) : ComObject(pointer), IComOwner<ID3D12Debug>
{
    public static Guid InterfaceId => typeof(Silk.NET.Direct3D12.ID3D12Debug).GUID;
    public static ID3D12Debug Attach(nint pointer) => new(pointer);
    internal Silk.NET.Direct3D12.ID3D12Debug* Native => (Silk.NET.Direct3D12.ID3D12Debug*)NativePointer;
}

public unsafe partial class ID3D12InfoQueue(nint pointer) : ComObject(pointer), IComOwner<ID3D12InfoQueue>
{
    public static Guid InterfaceId => typeof(Silk.NET.Direct3D12.ID3D12InfoQueue).GUID;
    public static ID3D12InfoQueue Attach(nint pointer) => new(pointer);
    internal Silk.NET.Direct3D12.ID3D12InfoQueue* Native => (Silk.NET.Direct3D12.ID3D12InfoQueue*)NativePointer;
}

public unsafe partial class ID3D11Device(nint pointer) : ComObject(pointer), IComOwner<ID3D11Device>
{
    public static Guid InterfaceId => typeof(Silk.NET.Direct3D11.ID3D11Device).GUID;
    public static ID3D11Device Attach(nint pointer) => new(pointer);
    internal Silk.NET.Direct3D11.ID3D11Device* Native => (Silk.NET.Direct3D11.ID3D11Device*)NativePointer;
}

public unsafe partial class ID3D11DeviceContext(nint pointer) : ComObject(pointer), IComOwner<ID3D11DeviceContext>
{
    public static Guid InterfaceId => typeof(Silk.NET.Direct3D11.ID3D11DeviceContext).GUID;
    public static ID3D11DeviceContext Attach(nint pointer) => new(pointer);
    internal Silk.NET.Direct3D11.ID3D11DeviceContext* Native => (Silk.NET.Direct3D11.ID3D11DeviceContext*)NativePointer;
}

public unsafe partial class ID3D11Texture2D(nint pointer) : ComObject(pointer), IComOwner<ID3D11Texture2D>
{
    public static Guid InterfaceId => typeof(Silk.NET.Direct3D11.ID3D11Texture2D).GUID;
    public static ID3D11Texture2D Attach(nint pointer) => new(pointer);
    internal Silk.NET.Direct3D11.ID3D11Texture2D* Native => (Silk.NET.Direct3D11.ID3D11Texture2D*)NativePointer;
}

public unsafe partial class ID3D11On12Device2(nint pointer) : ComObject(pointer), IComOwner<ID3D11On12Device2>
{
    public static Guid InterfaceId => typeof(Silk.NET.Direct3D11.Extensions.D3D11On12.ID3D11On12Device2).GUID;
    public static ID3D11On12Device2 Attach(nint pointer) => new(pointer);
    internal Silk.NET.Direct3D11.Extensions.D3D11On12.ID3D11On12Device2* Native => (Silk.NET.Direct3D11.Extensions.D3D11On12.ID3D11On12Device2*)NativePointer;
}

