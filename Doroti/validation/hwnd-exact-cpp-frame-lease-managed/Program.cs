using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using Doroti.Host.WindowsAppSdk;
using SkiaSharp;
using Vortice.Direct3D12;
using Vortice.DXGI;
using static Vortice.DXGI.DXGI;

[assembly: DisableRuntimeMarshalling]

namespace Doroti.Validation.HwndExactCppFrameLease;

internal static unsafe partial class Program
{
    private const string ProbeLibrary = "Doroti.HwndExactCpp.FrameLeaseProbe";
    private const ulong SupersededCausalId = ulong.MaxValue - 1;
    private const ulong ExceptionCausalId = ulong.MaxValue;
    private const ulong TimeoutCausalId = ulong.MaxValue - 2;

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    private struct ProbeResult
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal uint Status;
        internal uint Reserved;
        internal ulong ContextAcquireCount;
        internal ulong ContextReleaseCount;
        internal ulong RenderCallbackCount;
        internal ulong PresentedCount;
        internal ulong SupersededCount;
        internal ulong FailedCount;
        internal ulong FenceAfterSubmitCount;
        internal ulong ResizeBuffersCount;
        internal ulong ResizeInvalidCallCount;
        internal ulong PerFrameReferenceLeakCount;
        internal ulong DebugErrorCount;
        internal ulong DebugCorruptionCount;
        internal int LiveObjectReportHResult;
        internal uint Reserved2;
    }

    [LibraryImport(ProbeLibrary, EntryPoint = "doroti_windows_run_lease_probe_v1")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial uint RunProbe(in WindowsNativeV1.Callbacks callbacks, ref ProbeResult result);

    [STAThread]
    private static int Main()
    {
        WindowsNativeV1.ValidateLayout();
        using var state = new ManagedLeaseState();
        var handle = GCHandle.Alloc(state, GCHandleType.Normal);
        try
        {
            var callbacks = new WindowsNativeV1.Callbacks
            {
                AbiVersion = WindowsNativeV1.AbiVersion,
                StructSize = checked((uint)Marshal.SizeOf<WindowsNativeV1.Callbacks>()),
                CallbackContext = GCHandle.ToIntPtr(handle),
                HostReady = (nint)(delegate* unmanaged[Cdecl]<nint, ulong, void>)&HostReady,
                AcquireContext = (nint)(delegate* unmanaged[Cdecl]<nint, WindowsNativeV1.D3D12HostLease*, uint>)&AcquireContext,
                ReleaseContext = (nint)(delegate* unmanaged[Cdecl]<nint, ulong, ulong, uint>)&ReleaseContext,
                Metrics = (nint)(delegate* unmanaged[Cdecl]<nint, WindowsNativeV1.Metrics*, void>)&Metrics,
                Render = (nint)(delegate* unmanaged[Cdecl]<nint, WindowsNativeV1.FrameRequest*, WindowsNativeV1.D3D12Lease*, uint>)&Render,
                FrameTerminal = (nint)(delegate* unmanaged[Cdecl]<nint, WindowsNativeV1.FrameTerminal*, void>)&FrameTerminal,
                Log = (nint)(delegate* unmanaged[Cdecl]<nint, uint, WindowsNativeV1.Utf8, void>)&Log,
            };
            var result = new ProbeResult
            {
                AbiVersion = WindowsNativeV1.AbiVersion,
                StructSize = checked((uint)Marshal.SizeOf<ProbeResult>()),
            };
            var status = RunProbe(in callbacks, ref result);
            if (status != 0 || result.Status != 0)
                Console.Error.WriteLine(
                    $"nativeResult status={result.Status} acquire={result.ContextAcquireCount} release={result.ContextReleaseCount} " +
                    $"render={result.RenderCallbackCount} presented={result.PresentedCount} superseded={result.SupersededCount} " +
                    $"failed={result.FailedCount} fence={result.FenceAfterSubmitCount} resize={result.ResizeBuffersCount} " +
                    $"invalid={result.ResizeInvalidCallCount} refLeak={result.PerFrameReferenceLeakCount} debugError={result.DebugErrorCount} " +
                    $"debugCorruption={result.DebugCorruptionCount} live=0x{result.LiveObjectReportHResult:x8}");
            state.Validate(status, result);
            Console.WriteLine(JsonSerializer.Serialize(new
            {
                status = "PASS",
                result.ContextAcquireCount,
                result.ContextReleaseCount,
                result.RenderCallbackCount,
                result.PresentedCount,
                result.SupersededCount,
                result.FailedCount,
                result.FenceAfterSubmitCount,
                result.ResizeBuffersCount,
                result.ResizeInvalidCallCount,
                result.PerFrameReferenceLeakCount,
                result.DebugErrorCount,
                result.DebugCorruptionCount,
                liveObjectReportHResult = $"0x{result.LiveObjectReportHResult:x8}",
                state.ManagedRenderCount,
                state.ManagedTerminalCount,
                state.GuardedExceptionCount,
                state.TimeoutReturnCount,
                state.SupersededReturnCount,
            }));
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception);
            return 1;
        }
        finally
        {
            if (handle.IsAllocated)
                handle.Free();
        }
    }

    private static ManagedLeaseState State(nint context) =>
        (ManagedLeaseState)(GCHandle.FromIntPtr(context).Target ??
            throw new InvalidOperationException("The managed lease callback state was collected."));

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void HostReady(nint context, ulong viewId) { }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static uint AcquireContext(nint context, WindowsNativeV1.D3D12HostLease* lease) =>
        State(context).Acquire(lease);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static uint ReleaseContext(nint context, ulong hostGeneration, ulong contextGeneration) =>
        State(context).Release(hostGeneration, contextGeneration);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void Metrics(nint context, WindowsNativeV1.Metrics* metrics) { }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static uint Render(
        nint context,
        WindowsNativeV1.FrameRequest* request,
        WindowsNativeV1.D3D12Lease* lease) => State(context).Render(request, lease);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void FrameTerminal(nint context, WindowsNativeV1.FrameTerminal* terminal) =>
        State(context).Terminal(terminal);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void Log(nint context, uint level, WindowsNativeV1.Utf8 message) { }

    private sealed class ManagedLeaseState : IDisposable
    {
        private readonly HashSet<(ulong Generation, ulong LeaseId)> _terminals = [];
        private IDXGIFactory6? _factory;
        private IDXGIAdapter1? _adapter;
        private ID3D12Device2? _device;
        private ID3D12CommandQueue? _queue;
        private GRVorticeD3DBackendContext? _backend;
        private GRContext? _context;
        private ulong _hostGeneration;
        private ulong _contextGeneration;
        private bool _active;

        internal ulong ManagedRenderCount { get; private set; }
        internal ulong ManagedTerminalCount { get; private set; }
        internal ulong GuardedExceptionCount { get; private set; }
        internal ulong TimeoutReturnCount { get; private set; }
        internal ulong SupersededReturnCount { get; private set; }
        private ulong ManagedAcquireCount { get; set; }
        private ulong ManagedReleaseCount { get; set; }

        internal uint Acquire(WindowsNativeV1.D3D12HostLease* lease)
        {
            try
            {
                Require(!_active, "A second D3D12 host lease arrived before release.");
                Require(lease is not null, "The D3D12 host lease is null.");
                Require(lease->AbiVersion == WindowsNativeV1.AbiVersion, "Host lease ABI mismatch.");
                Require(lease->StructSize >= Marshal.SizeOf<WindowsNativeV1.D3D12HostLease>(), "Host lease is truncated.");
                Require(lease->Device != 0 && lease->CommandQueue != 0, "Host lease COM pointers are null.");

                _hostGeneration = lease->HostGeneration;
                _contextGeneration = lease->ContextGeneration;
                _device = new ID3D12Device2(lease->Device);
                _queue = new ID3D12CommandQueue(lease->CommandQueue);
                _factory = CreateDXGIFactory2<IDXGIFactory6>(false);
                _adapter = _factory.EnumAdapterByLuid<IDXGIAdapter1>((Vortice.Luid)_device.AdapterLuid);
                _backend = new GRVorticeD3DBackendContext
                {
                    Adapter = _adapter,
                    Device = _device,
                    Queue = _queue,
                };
                _context = GRContext.CreateDirect3D(_backend) ??
                    throw new InvalidOperationException("Skia could not create the callback-scoped D3D12 context.");
                _active = true;
                ManagedAcquireCount++;
                return 0;
            }
            catch
            {
                DisposeContext();
                return 1;
            }
        }

        internal uint Release(ulong hostGeneration, ulong contextGeneration)
        {
            try
            {
                Require(_active, "The native host released an inactive D3D12 context.");
                Require(hostGeneration == _hostGeneration, "Host generation changed before context release.");
                Require(contextGeneration == _contextGeneration, "Context generation changed before context release.");
                DisposeContext();
                ManagedReleaseCount++;
                return 0;
            }
            catch
            {
                DisposeContext();
                return 1;
            }
        }

        internal uint Render(WindowsNativeV1.FrameRequest* request, WindowsNativeV1.D3D12Lease* lease)
        {
            ID3D12Resource? resource = null;
            try
            {
                Require(lease is not null && lease->Resource != 0, "The frame resource lease is null.");
                resource = new ID3D12Resource(lease->Resource);
                Require(request is not null, "The frame request is null.");
                Require(_active && _context is not null, "A frame arrived outside an active host lease.");
                Require(request->AbiVersion == WindowsNativeV1.AbiVersion, "Frame request ABI mismatch.");
                Require(lease->AbiVersion == WindowsNativeV1.AbiVersion, "Frame lease ABI mismatch.");
                Require(request->LeaseId == lease->LeaseId, "Frame request and resource lease IDs differ.");
                Require(lease->HostGeneration == _hostGeneration, "Stale host generation reached managed rendering.");
                Require(lease->ContextGeneration == _contextGeneration, "Stale context generation reached managed rendering.");
                Require(request->WidthPx == lease->WidthPx && request->HeightPx == lease->HeightPx,
                    "Frame request and resource extents differ.");
                Require(lease->DxgiFormat == (uint)Format.B8G8R8A8_UNorm, "Unexpected frame format.");
                Require(lease->ResourceState == (uint)WindowsNativeV1.LeaseResourceState.RenderTarget,
                    "Unexpected frame resource state.");
                Require(lease->SampleCount == 1 && lease->SampleQuality == 0, "The frame resource is not single-sampled.");
                ManagedRenderCount++;

                if (request->CausalFrameId == SupersededCausalId)
                {
                    SupersededReturnCount++;
                    return 2;
                }
                if (request->CausalFrameId == TimeoutCausalId)
                {
                    TimeoutReturnCount++;
                    return 3;
                }
                if (request->CausalFrameId == ExceptionCausalId)
                    throw new InvalidOperationException("Intentional managed render exception probe.");

                using var resourceInfo = new GRVorticeD3DTextureResourceInfo
                {
                    Resource = resource,
                    ResourceState = ResourceStates.RenderTarget,
                    Format = Format.B8G8R8A8_UNorm,
                    SampleCount = 1,
                    LevelCount = 1,
                };
                using var target = new GRBackendRenderTarget(
                    checked((int)lease->WidthPx), checked((int)lease->HeightPx), resourceInfo);
                var context = _context ??
                    throw new InvalidOperationException("The callback-scoped Skia context is unavailable.");
                using var surface = SKSurface.Create(
                    context, target, GRSurfaceOrigin.TopLeft, SKColorType.Bgra8888) ??
                    throw new InvalidOperationException("Skia could not wrap the leased D3D12 resource.");
                Draw(surface.Canvas, checked((int)request->WidthPx), checked((int)request->HeightPx), request->CausalFrameId);
                surface.Canvas.Flush();
                context.Flush(surface);
                context.Submit(true);
                return 0;
            }
            catch (InvalidOperationException) when (request is not null && request->CausalFrameId == ExceptionCausalId)
            {
                GuardedExceptionCount++;
                return 1;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine($"renderFailure causal={(request is null ? 0 : request->CausalFrameId)}: {exception}");
                return 1;
            }
            finally
            {
                resource?.Dispose();
            }
        }

        internal void Terminal(WindowsNativeV1.FrameTerminal* terminal)
        {
            Require(terminal is not null, "The terminal receipt is null.");
            Require(terminal->AbiVersion == WindowsNativeV1.AbiVersion, "Terminal receipt ABI mismatch.");
            Require(terminal->TerminalKind is >= 1 and <= 3, "Terminal receipt kind is invalid.");
            Require(_terminals.Add((terminal->Generation, terminal->LeaseId)), "A frame received more than one terminal receipt.");
            if (terminal->CausalFrameId == SupersededCausalId)
                Require(terminal->TerminalKind == (uint)WindowsNativeV1.FrameTerminalKind.Superseded, "Stale frame was not superseded.");
            else if (terminal->CausalFrameId is ExceptionCausalId or TimeoutCausalId)
                Require(terminal->TerminalKind == (uint)WindowsNativeV1.FrameTerminalKind.Failed, "Failure probe did not terminate as failed.");
            else
                Require(terminal->TerminalKind == (uint)WindowsNativeV1.FrameTerminalKind.Presented, "Normal frame was not presented.");
            ManagedTerminalCount++;
        }

        internal void Validate(uint callStatus, ProbeResult result)
        {
            Require(callStatus == 0 && result.Status == 0, $"Native probe failed: call={callStatus}, result={result.Status}.");
            Require(result.ContextAcquireCount == 2 && result.ContextReleaseCount == 2, "Native context lease counts differ.");
            Require(result.RenderCallbackCount == 13 && ManagedRenderCount == 13, "Render callback count differs.");
            Require(result.PresentedCount == 10 && result.SupersededCount == 1 && result.FailedCount == 2,
                "Native terminal classification differs.");
            Require(result.FenceAfterSubmitCount == 10, "Native fence did not follow every managed submit.");
            Require(result.ResizeBuffersCount == 10 && result.ResizeInvalidCallCount == 0, "ResizeBuffers evidence failed.");
            Require(result.PerFrameReferenceLeakCount == 0, "A per-frame COM reference survived its callback.");
            Require(result.DebugErrorCount == 0 && result.DebugCorruptionCount == 0, "D3D12 debug validation reported errors.");
            Require(result.LiveObjectReportHResult >= 0, "DXGI live-object reporting failed.");
            Require(ManagedAcquireCount == 2 && ManagedReleaseCount == 2 && !_active, "Managed host lease lifetime differs.");
            Require(ManagedTerminalCount == 13 && _terminals.Count == 13, "Terminal receipts are missing or duplicated.");
            Require(GuardedExceptionCount == 1 && TimeoutReturnCount == 1 && SupersededReturnCount == 1,
                "Managed failure paths were not all exercised once.");
        }

        public void Dispose() => DisposeContext();

        private void DisposeContext()
        {
            _context?.Dispose();
            _context = null;
            _backend?.Dispose();
            _backend = null;
            _adapter?.Dispose();
            _adapter = null;
            _factory?.Dispose();
            _factory = null;
            _queue?.Dispose();
            _queue = null;
            _device?.Dispose();
            _device = null;
            _active = false;
        }

        private static void Draw(SKCanvas canvas, int width, int height, ulong frameId)
        {
            canvas.Clear(new SKColor(8, 14, 24, 255));
            var tile = Math.Max(16, Math.Min(width, height) / 12);
            using var paint = new SKPaint { IsAntialias = false };
            for (var y = 0; y < height; y += tile)
            {
                for (var x = 0; x < width; x += tile)
                {
                    paint.Color = (((x / tile) + (y / tile) + (long)frameId) & 1) == 0
                        ? new SKColor(20, 184, 166, 255)
                        : new SKColor(30, 41, 59, 255);
                    canvas.DrawRect(x, y, Math.Min(tile, width - x), Math.Min(tile, height - y), paint);
                }
            }
            paint.Color = new SKColor(244, 63, 94, 255);
            canvas.DrawRect(0, 0, width, Math.Max(8, height / 20), paint);
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException(message);
        }
    }
}
