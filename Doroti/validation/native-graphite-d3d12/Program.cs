using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using Doroti.Skia.Vulkan;
using SkiaSharp;
using Vortice.Direct3D;
using Vortice.Direct3D12;
using Vortice.DXGI;
using static Vortice.Direct3D12.D3D12;
using static Vortice.DXGI.DXGI;

internal static unsafe class Program
{
    private static int Main(string[] args)
    {
        var report = args.Length == 0 ? "graphite-d3d12.json" : args[0];
        var rows = new List<object>();
        try
        {
            GraphiteNativeLibrary.Configure();
            using var factory = CreateDXGIFactory2<IDXGIFactory6>(false);
            for (uint adapterIndex = 0; factory.EnumAdapters1(adapterIndex, out var adapter).Success; adapterIndex++)
            {
                using (adapter)
                {
                    if ((adapter.Description1.Flags & AdapterFlags.Software) != 0) continue;
                    using var device = D3D12CreateDevice<ID3D12Device>(adapter, FeatureLevel.Level_11_0);
                    using var queue = device.CreateCommandQueue(CommandListType.Direct);
                    using var allocator = device.CreateCommandAllocator(CommandListType.Direct);
                    using var commands = device.CreateCommandList<ID3D12GraphicsCommandList>(CommandListType.Direct, allocator, null);
                    commands.Close();
                    using var fence = device.CreateFence(0);
                    var luid = adapter.Description1.Luid;
                    using var graphite = GraphiteVulkanWindow.CreateD3D12(Unsafe.As<Vortice.Luid, long>(ref luid));
                    ulong serial = 0;
                    foreach (var width in new[] { 64, 96, 64 })
                    {
                        const int height = 48;
                        using var texture = device.CreateCommittedResource(HeapType.Default, HeapFlags.Shared,
                            ResourceDescription.Texture2D(Format.R8G8B8A8_UNorm, (uint)width, height, 1, 1, 1, 0, ResourceFlags.AllowRenderTarget),
                            ResourceStates.Common, null);
                        var handle = device.CreateSharedHandle(texture, null, null!);
                        try { graphite.ImportD3D12Resource(handle, width, height); }
                        finally { CloseHandle(handle); }
                        var pitch = (uint)((width * 4 + 255) & ~255);
                        using var readback = device.CreateCommittedResource(HeapType.Readback, HeapFlags.None,
                            ResourceDescription.Buffer(pitch * height), ResourceStates.CopyDest, null);
                        for (var frame = 0; frame < 12; frame++)
                        {
                            if (frame == 3) { graphite.BeginD3D12Frame().Canvas.Clear(SKColors.Green); graphite.ReleaseD3D12Frame(); }
                            var surface = graphite.BeginD3D12Frame();
                            var color = frame % 2 == 0 ? SKColors.Red : SKColors.Blue;
                            surface.Canvas.Clear(color);
                            graphite.FlushD3D12Frame();
                            allocator.Reset(); commands.Reset(allocator);
                            commands.ResourceBarrierTransition(texture, ResourceStates.Common, ResourceStates.CopySource);
                            var footprint = new PlacedSubresourceFootPrint { Offset = 0,
                                Footprint = new(Format.R8G8B8A8_UNorm, (uint)width, height, 1, pitch) };
                            commands.CopyTextureRegion(new TextureCopyLocation(readback, footprint), 0, 0, 0, new TextureCopyLocation(texture, 0), null);
                            commands.ResourceBarrierTransition(texture, ResourceStates.CopySource, ResourceStates.Common);
                            commands.Close(); queue.ExecuteCommandList(commands);
                            queue.Signal(fence, ++serial).CheckError();
                            using var done = new EventWaitHandle(false, EventResetMode.AutoReset);
                            fence.SetEventOnCompletion(serial, done).CheckError();
                            if (!done.WaitOne(TimeSpan.FromSeconds(5))) throw new TimeoutException("D3D12 readback fence timed out.");
                            void* data = null;
                            readback.Map(0, null, &data).CheckError();
                            try
                            {
                                for (var y = 0; y < height; y++)
                                    for (var x = 0; x < width; x++)
                                    {
                                        var pixel = (byte*)data + y * pitch + x * 4;
                                        if (pixel[0] != color.Red || pixel[1] != color.Green || pixel[2] != color.Blue || pixel[3] != 255)
                                            throw new InvalidDataException($"{adapter.Description1.Description} pixel mismatch frame={frame} at={x},{y}: {pixel[0]},{pixel[1]},{pixel[2]},{pixel[3]}");
                                    }
                            }
                            finally { readback.Unmap(0); }
                        }
                        rows.Add(new { adapter = adapter.Description1.Description, width, height, frames = 12, cancellation = "PASS", pixels = "PASS" });
                    }
                }
            }
            if (rows.Count == 0) throw new InvalidOperationException("No hardware GPU was tested.");
            File.WriteAllText(report, JsonSerializer.Serialize(new { status = "PASS", rows }, new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine($"PASS {rows.Count} GPU/size rows; 12 pixel-verified frames per row");
            return 0;
        }
        catch (Exception exception)
        {
            File.WriteAllText(report, JsonSerializer.Serialize(new { status = "FAIL", rows, exception = exception.ToString() }, new JsonSerializerOptions { WriteIndented = true }));
            Console.Error.WriteLine(exception); return 1;
        }
    }
    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseHandle(nint handle);
}
