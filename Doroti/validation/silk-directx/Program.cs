using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Doroti.Graphics.DirectX;
using static Doroti.Graphics.DirectX.DirectX;
using FeatureLevel = Silk.NET.Core.Native.D3DFeatureLevel;
using GpuPreference = Silk.NET.DXGI.GpuPreference;
using CommandListType = Silk.NET.Direct3D12.CommandListType;
using HeapType = Silk.NET.Direct3D12.HeapType;
using HeapFlags = Silk.NET.Direct3D12.HeapFlags;
using ResourceStates = Silk.NET.Direct3D12.ResourceStates;

// Validate ownership/error behavior separately from the Graphite pixel and
// Composition presentation probes. Run with validation/run-with-timeout.py.
using var factory = CreateDXGIFactory2<IDXGIFactory6>(false);
using var adapter = factory.EnumAdapterByGpuPreference<IDXGIAdapter1>(0, GpuPreference.HighPerformance);
using var device = D3D12CreateDevice<ID3D12Device>(adapter, FeatureLevel.Level110);
using var queue = device.CreateCommandQueue(CommandListType.Direct);
using var fence = device.CreateFence(0);

// Disposing a QueryInterface reference twice must not release the original owner.
var other = device.QueryInterface<ID3D12Device>();
other.Dispose();
other.Dispose();
try { _ = other.NativePointer; throw new Exception("Disposed owner remained accessible."); }
catch (ObjectDisposedException) { }
try { using var invalid = device.QueryInterface<IDXGIFactory6>(); throw new Exception("Invalid QI succeeded."); }
catch (COMException exception) when (exception.HResult == unchecked((int)0x80004002)) { }
if (!device.DeviceRemovedReason.Success) throw new Exception("Original device reference was released.");

// A wrapper returned by QueryInterface stays valid after the factory owner dies.
using (var detached = DetachedFactory())
{
    GC.Collect();
    GC.WaitForPendingFinalizers();
    detached.EnumAdapters1(0, out var first).CheckError();
    first.Dispose();
}

// Force collections while temporary resource owners invoke native methods. The
// ComScope must retain the finalizable receiver until its unmanaged call returns.
using var stop = new CancellationTokenSource();
var collections = Task.Run(() =>
{
    while (!stop.IsCancellationRequested)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Thread.Yield();
    }
});
try
{
    for (var index = 0; index < 200; index++)
    {
        using var resource = device.CreateCommittedResource(HeapType.Readback, HeapFlags.None,
            ResourceDescription.Buffer(4096), ResourceStates.CopyDest, null);
        if (resource.Description.Width != 4096) throw new Exception("Resource description changed.");
        queue.Signal(fence, (ulong)index + 1).CheckError();
        using var done = new EventWaitHandle(false, EventResetMode.AutoReset);
        fence.SetEventOnCompletion((ulong)index + 1, done).CheckError();
        if (!done.WaitOne(TimeSpan.FromSeconds(5))) throw new TimeoutException("Fence did not complete.");
        // No using variable: the finalizer is the sole release path for this owner.
        if (TemporaryDescription(device).Width != 4096) throw new Exception("Temporary owner failed.");
    }
}
finally { stop.Cancel(); collections.GetAwaiter().GetResult(); }
GC.Collect();
GC.WaitForPendingFinalizers();
Console.WriteLine("PASS Silk DirectX: QI ownership, double disposal, HRESULT, module lifetime, 200 GC/fence cycles");

[MethodImpl(MethodImplOptions.NoInlining)]
static IDXGIFactory2 DetachedFactory()
{
    using var original = CreateDXGIFactory2<IDXGIFactory6>(false);
    return original.QueryInterface<IDXGIFactory2>();
}

[MethodImpl(MethodImplOptions.NoInlining)]
static ResourceDescription TemporaryDescription(ID3D12Device device)
    => device.CreateCommittedResource(HeapType.Readback, HeapFlags.None,
        ResourceDescription.Buffer(4096), ResourceStates.CopyDest, null).Description;
