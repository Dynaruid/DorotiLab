using Doroti.Graphics.DirectX;
using static Doroti.Graphics.DirectX.DirectX;
using FeatureLevel = Silk.NET.Core.Native.D3DFeatureLevel;
using GpuPreference = Silk.NET.DXGI.GpuPreference;
using CommandListType = Silk.NET.Direct3D12.CommandListType;
using CommandQueueFlags = Silk.NET.Direct3D12.CommandQueueFlags;
using SkiaSharp;

// Offscreen hardware context; no window, compositor or input automation.
internal sealed class GpuRasterFixture : IDisposable
{
    private readonly IDXGIFactory6 _factory = CreateDXGIFactory2<IDXGIFactory6>(false);
    private readonly IDXGIAdapter1 _adapter;
    private readonly ID3D12Device2 _device;
    private readonly ID3D12CommandQueue _queue;
    private readonly GRD3DBackendContext _backend;
    internal GRContext Context { get; }
    internal GpuRasterFixture()
    {
        _adapter = _factory.EnumAdapterByGpuPreference<IDXGIAdapter1>(0, GpuPreference.HighPerformance);
        _device = D3D12CreateDevice<ID3D12Device2>(_adapter, FeatureLevel.Level110);
        _queue = _device.CreateCommandQueue(CommandListType.Direct, 0, CommandQueueFlags.None, 0);
        _backend = new GRD3DBackendContext { Adapter = _adapter.NativePointer, Device = _device.NativePointer, Queue = _queue.NativePointer };
        Context = GRContext.CreateDirect3D(_backend) ?? throw new Exception("GPU raster fixture context creation failed");
    }
    public void Dispose()
    {
        Context.Dispose(); _backend.Dispose(); _queue.Dispose(); _device.Dispose(); _adapter.Dispose(); _factory.Dispose();
    }
}
