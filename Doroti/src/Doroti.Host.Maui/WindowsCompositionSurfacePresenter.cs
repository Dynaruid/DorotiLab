#if WINDOWS
using System.Numerics;
using System.Runtime.InteropServices;
using Doroti.Ui;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Hosting;
using SharpGen.Runtime;
using SkiaSharp;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.Direct3D11on12;
using Vortice.Direct3D12;
using Vortice.DXGI;
using Windows.Foundation;
using Windows.Graphics;
using static Vortice.Direct3D11on12.Apis;
using static Vortice.Direct3D12.D3D12;
using static Vortice.DXGI.DXGI;

namespace Doroti.Host.Maui;

internal static class WindowsCompositionSurfaceFeature
{
    internal const string EnvironmentVariable = "DOROTI_WINDOWS_COMPOSITION_SURFACE";

    internal static bool Enabled =>
        string.Equals(Environment.GetEnvironmentVariable(EnvironmentVariable), "1",
            StringComparison.Ordinal);
}

/// <summary>
/// Candidate presentation boundary used only behind
/// DOROTI_WINDOWS_COMPOSITION_SURFACE=1. Skia renders into an exact D3D12
/// backing store; this owner copies that resource into a bounded pool of
/// CompositionDrawingSurface fronts without CPU readback.
/// </summary>
internal sealed class WindowsCompositionSurfacePresenter : IDisposable
{
    private const int MaximumSurfaceSlots = 3;
    private readonly Compositor _compositor;
    private readonly Action<Action> _invokeOnUiThread;
    private readonly Action _frontSlotAvailable;
    private readonly object _poolGate = new();
    private readonly List<WindowsCompositionSurfaceSlot> _slots = [];
    private readonly ManualResetEventSlim _noPendingCommits = new(true);
    private IDXGIFactory6? _factory;
    private IDXGIAdapter1? _adapter;
    private ID3D12Device2? _device12;
    private ID3D12CommandQueue? _queue;
    private ID3D11Device? _device11;
    private ID3D11DeviceContext? _context11;
    private ID3D11On12Device2? _on12;
    private CompositionGraphicsDevice? _graphicsDevice;
    private GRVorticeD3DBackendContext? _skiaBackend;
    private GRContext? _skiaContext;
    private WindowsD3D12BackingStore? _backingStore;
    private ID3D12CommandAllocator? _copyAllocator;
    private ID3D12GraphicsCommandList? _copyCommandList;
    private ID3D12Fence? _copyFence;
    private CompositionSurfaceBrush? _brush;
    private SpriteVisual? _visual;
    private DorotiWindowsDxgiHost? _attachedHost;
    private WindowsCompositionSurfaceSlot? _front;
    private ulong _nextFenceValue;
    private long _commitSerial;
    private long _completedCommitSerial;
    private int _pendingCommitCount;
    private int _slotWaiter;
    private Exception? _asyncCommitFailure;
    private string _adapterDescription = "uninitialized";
    private bool _uiTeardown;
    private bool _disposed;

    internal WindowsCompositionSurfacePresenter(
        Compositor compositor,
        Action<Action> invokeOnUiThread,
        Action frontSlotAvailable)
    {
        _compositor = compositor;
        _invokeOnUiThread = invokeOnUiThread;
        _frontSlotAvailable = frontSlotAvailable;
    }

    internal int Width { get; private set; }
    internal int Height { get; private set; }
    internal bool SurfaceChanged { get; private set; }
    internal int SurfacePoolHighWater { get; private set; }
    internal long SupersededCount { get; private set; }
    internal long FrontAdoptedCount { get; private set; }
    internal long RetirementCount { get; private set; }
    internal long BeginDrawCount { get; private set; }
    internal long EndDrawCount { get; private set; }
    internal long GpuFenceCount { get; private set; }
    internal long CommitRequestCount { get; private set; }
    internal long CommitCompletionCount { get; private set; }
    internal long CommitBatchCompletionCount { get; private set; }
    internal int CheckedOutResourceCount { get; private set; }
    internal int OpenDrawCount { get; private set; }
    internal string AdapterDescription => _adapterDescription;
    internal GRContext Context => _skiaContext ??
        throw new InvalidOperationException("Composition Skia context is unavailable.");
    internal SKSurface Surface => _backingStore?.Surface ??
        throw new InvalidOperationException("Composition D3D12 backing store is unavailable.");

    internal void EnsureTarget(
        DorotiWindowsDxgiHost host,
        int width,
        int height)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ThrowAsyncCommitFailure();
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
        EnsureDeviceAndVisual(host);
        _backingStore ??= new WindowsD3D12BackingStore(_device12!, Context);
        SurfaceChanged = _backingStore.EnsureSize(width, height);
        Width = width;
        Height = height;
    }

    internal void Flush()
    {
        Surface.Canvas.Flush();
        Context.Flush(Surface);
        Context.Submit(false);
    }

    internal bool TryPresent(
        DorotiWindowsDxgiHost host,
        DorotiResizeEpoch target,
        Func<long> latestTargetGeneration,
        Action onCommitStarting,
        out long observedTargetGeneration)
    {
        ArgumentNullException.ThrowIfNull(latestTargetGeneration);
        ArgumentNullException.ThrowIfNull(onCommitStarting);
        if (!ReferenceEquals(host, _attachedHost))
            throw new InvalidOperationException("Composition presenter host changed before present.");
        if (target.PhysicalWidth != Width || target.PhysicalHeight != Height)
            throw new InvalidOperationException("Composition source and target dimensions differ.");
        if (Math.Abs(target.DeviceScaleX - target.DeviceScaleY) > 0.0001)
            throw new InvalidOperationException("Non-uniform Composition device scale is not supported.");

        ThrowAsyncCommitFailure();
        var slot = TryAcquireSlot();
        if (slot is null)
        {
            Interlocked.Exchange(ref _slotWaiter, 1);
            observedTargetGeneration = latestTargetGeneration();
            SupersededCount++;
            return false;
        }
        slot.Prepare(_graphicsDevice!, Width, Height);
        slot.Transition(WindowsCompositionSurfaceSlotState.Drawing);
        CopyBackingToSurface(slot);
        slot.Transition(WindowsCompositionSurfaceSlotState.GpuWorkQueued);
        slot.Transition(WindowsCompositionSurfaceSlotState.DrawEnded);

        var adopted = false;
        var observed = 0L;
        _invokeOnUiThread(() =>
        {
            observed = latestTargetGeneration();
            if (observed != target.Generation) return;
            long requestSerial;
            lock (_poolGate)
            {
                requestSerial = ++_commitSerial;
                slot.Transition(WindowsCompositionSurfaceSlotState.PendingVisualCommit);
                var oldFront = _front;
                if (oldFront is not null)
                {
                    oldFront.Transition(WindowsCompositionSurfaceSlotState.Retired);
                    oldFront.RetireAfterCommit = requestSerial + 1;
                }
                slot.Transition(WindowsCompositionSurfaceSlotState.Front);
                _front = slot;
                FrontAdoptedCount++;
            }
            _brush!.Surface = slot.Surface;
            var reciprocalScale = (float)(1.0 / target.DeviceScaleX);
            _brush.Scale = new Vector2(reciprocalScale, reciprocalScale);
            _visual!.Offset = System.Numerics.Vector3.Zero;
            _visual.Size = new Vector2(
                (float)target.LogicalWidth,
                (float)target.LogicalHeight);
            _visual.Clip = _compositor.CreateInsetClip();
            onCommitStarting();
            var animation = _compositor.CreateScalarKeyFrameAnimation();
            animation.Duration = TimeSpan.FromMilliseconds(1);
            animation.InsertKeyFrame(1, 1);
            var batch = _compositor.GetCommitBatch(CompositionBatchTypes.Animation);
            var batchCompletion = new TaskCompletionSource(
                TaskCreationOptions.RunContinuationsAsynchronously);
            TypedEventHandler<object, CompositionBatchCompletedEventArgs>? handler = null;
            handler = (_, _) => batchCompletion.TrySetResult();
            batch.Completed += handler;
            _visual.StartAnimation("Opacity", animation);
            var commitTask = _compositor.RequestCommitAsync().AsTask();
            CommitRequestCount++;
            if (Interlocked.Increment(ref _pendingCommitCount) == 1)
                _noPendingCommits.Reset();
            _ = CompleteCommitAsync(
                requestSerial,
                commitTask,
                batchCompletion.Task,
                batch,
                animation,
                handler);
            adopted = true;
        });

        observedTargetGeneration = observed;
        if (!adopted)
        {
            slot.AbandonDrawEnded();
            SupersededCount++;
            return false;
        }
        return true;
    }

    private async Task CompleteCommitAsync(
        long requestSerial,
        Task commitTask,
        Task batchTask,
        CompositionCommitBatch batch,
        ScalarKeyFrameAnimation animation,
        TypedEventHandler<object, CompositionBatchCompletedEventArgs> handler)
    {
        try
        {
            // RequestCommitAsync is the dispatch barrier used for bounded
            // retirement. A retired front additionally waits for a later
            // request serial before reuse. The animation commit batch remains
            // tracked to terminal completion, but it must not throttle raster
            // admission: on some WinUI/DWM paths its Completed event arrives
            // several composition frames after the request action.
            await commitTask.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            var releasedSlot = false;
            lock (_poolGate)
            {
                CommitCompletionCount++;
                _completedCommitSerial = Math.Max(_completedCommitSerial, requestSerial);
                releasedSlot = ReleaseEligibleRetiredSlotsLocked();
            }
            if (releasedSlot && Interlocked.Exchange(ref _slotWaiter, 0) == 1)
                _frontSlotAvailable();
            await batchTask.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            lock (_poolGate) CommitBatchCompletionCount++;
        }
        catch (Exception exception)
        {
            lock (_poolGate) _asyncCommitFailure ??= exception;
        }
        finally
        {
            try
            {
                void CleanupCompositionObjects()
                {
                    if (!_uiTeardown) _visual?.StopAnimation("Opacity");
                    batch.Completed -= handler;
                    animation.Dispose();
                    batch.Dispose();
                }
                if (_uiTeardown) CleanupCompositionObjects();
                else _invokeOnUiThread(CleanupCompositionObjects);
            }
            catch (Exception exception)
            {
                lock (_poolGate) _asyncCommitFailure ??= exception;
            }
            if (Interlocked.Decrement(ref _pendingCommitCount) == 0)
                _noPendingCommits.Set();
        }
    }

    private void CopyBackingToSurface(WindowsCompositionSurfaceSlot slot)
    {
        BeginDrawCount++;
        var draw = WindowsCompositionInterop.BeginDraw(slot.Surface);
        OpenDrawCount++;
        ID3D11Texture2D? texture11 = null;
        ID3D12Resource? destination12 = null;
        var returned = false;
        var submitted = false;
        ulong fenceValue = 0;
        try
        {
            texture11 = new ID3D11Texture2D(draw.TexturePointer);
            draw.TexturePointer = 0;
            destination12 = _on12!.UnwrapUnderlyingResource<ID3D12Resource>(texture11, _queue!);
            CheckedOutResourceCount++;
            ValidateDestination(slot, draw.Offset, texture11.Description, destination12.Description);

            _copyAllocator!.Reset();
            _copyCommandList!.Reset(_copyAllocator);
            _copyCommandList.ResourceBarrier(
            [
                ResourceBarrier.BarrierTransition(
                    _backingStore!.Resource,
                    ResourceStates.RenderTarget,
                    ResourceStates.CopySource),
                ResourceBarrier.BarrierTransition(
                    destination12,
                    ResourceStates.Common,
                    ResourceStates.CopyDest),
            ]);
            _copyCommandList.CopyTextureRegion(
                new TextureCopyLocation(destination12, 0),
                checked((uint)draw.Offset.X),
                checked((uint)draw.Offset.Y),
                0,
                new TextureCopyLocation(_backingStore.Resource, 0),
                null);
            _copyCommandList.ResourceBarrier(
            [
                ResourceBarrier.BarrierTransition(
                    _backingStore.Resource,
                    ResourceStates.CopySource,
                    ResourceStates.RenderTarget),
                ResourceBarrier.BarrierTransition(
                    destination12,
                    ResourceStates.CopyDest,
                    ResourceStates.Common),
            ]);
            _copyCommandList.Close();
            _queue!.ExecuteCommandList(_copyCommandList);
            submitted = true;
            fenceValue = checked(++_nextFenceValue);
            _queue.Signal(_copyFence!, fenceValue).CheckError();
            _on12.ReturnUnderlyingResource(texture11, [fenceValue], [_copyFence!]).CheckError();
            returned = true;
            CheckedOutResourceCount--;
            WindowsCompositionInterop.EndDraw(draw);
            OpenDrawCount--;
            EndDrawCount++;
            WaitForFence(fenceValue);
            GpuFenceCount++;
        }
        finally
        {
            if (destination12 is not null && !returned)
            {
                if (submitted)
                    _on12!.ReturnUnderlyingResource(texture11!, [fenceValue], [_copyFence!]).CheckError();
                else
                    _on12!.ReturnUnderlyingResource(texture11!, [], []).CheckError();
                CheckedOutResourceCount--;
            }
            destination12?.Dispose();
            texture11?.Dispose();
            if (draw.TexturePointer != 0) Marshal.Release(draw.TexturePointer);
            if (!draw.Ended)
            {
                WindowsCompositionInterop.EndDraw(draw);
                OpenDrawCount--;
                EndDrawCount++;
            }
        }
    }

    private static void ValidateDestination(
        WindowsCompositionSurfaceSlot slot,
        WindowsCompositionNativePoint offset,
        Texture2DDescription description11,
        ResourceDescription description12)
    {
        if (description11.Format != Format.R8G8B8A8_UNorm ||
            description12.Format != Format.R8G8B8A8_UNorm)
            throw new InvalidOperationException("Composition surface format mismatch.");
        if (description11.SampleDescription.Count != 1 || description12.SampleDescription.Count != 1)
            throw new InvalidOperationException("Composition surface sample-count mismatch.");
        if (offset.X < 0 || offset.Y < 0 ||
            description11.Width < offset.X + slot.Width ||
            description11.Height < offset.Y + slot.Height ||
            description12.Width != description11.Width ||
            description12.Height != description11.Height)
            throw new InvalidOperationException("Composition BeginDraw geometry mismatch.");
    }

    private WindowsCompositionSurfaceSlot? TryAcquireSlot()
    {
        lock (_poolGate)
        {
            var slot = _slots.FirstOrDefault(candidate =>
                candidate.State == WindowsCompositionSurfaceSlotState.Free);
            if (slot is not null) return slot;
            if (_slots.Count >= MaximumSurfaceSlots) return null;
            slot = new WindowsCompositionSurfaceSlot(_slots.Count);
            _slots.Add(slot);
            SurfacePoolHighWater = Math.Max(SurfacePoolHighWater, _slots.Count);
            return slot;
        }
    }

    private bool ReleaseEligibleRetiredSlotsLocked()
    {
        var released = false;
        foreach (var slot in _slots.Where(candidate =>
                     candidate.State == WindowsCompositionSurfaceSlotState.Retired &&
                     candidate.RetireAfterCommit <= _completedCommitSerial))
        {
            slot.Transition(WindowsCompositionSurfaceSlotState.Free);
            slot.RetireAfterCommit = 0;
            RetirementCount++;
            released = true;
        }
        return released;
    }

    private void ThrowAsyncCommitFailure()
    {
        Exception? failure;
        lock (_poolGate) failure = _asyncCommitFailure;
        if (failure is not null)
            throw new InvalidOperationException("An asynchronous Composition commit failed.", failure);
    }

    private void EnsureDeviceAndVisual(DorotiWindowsDxgiHost host)
    {
        if (_skiaContext is null)
        {
            _factory = CreateDXGIFactory2<IDXGIFactory6>(false);
            _adapter = _factory.EnumAdapterByGpuPreference<IDXGIAdapter1>(
                0, GpuPreference.HighPerformance);
            _adapterDescription = _adapter.Description1.Description;
            _device12 = D3D12CreateDevice<ID3D12Device2>(_adapter, FeatureLevel.Level_11_0);
            _queue = _device12.CreateCommandQueue(
                CommandListType.Direct, 0, CommandQueueFlags.None, 0);
            D3D11On12CreateDevice(
                _device12,
                DeviceCreationFlags.BgraSupport,
                [FeatureLevel.Level_11_0],
                [_queue],
                0,
                out _device11,
                out _context11,
                out var chosenFeatureLevel).CheckError();
            if (chosenFeatureLevel < FeatureLevel.Level_11_0)
                throw new InvalidOperationException($"D3D11On12 selected {chosenFeatureLevel}.");
            _on12 = _device11.QueryInterface<ID3D11On12Device2>();
            _graphicsDevice = WindowsCompositionInterop.CreateGraphicsDevice(_compositor, _device11);
            _skiaBackend = new GRVorticeD3DBackendContext
            {
                Adapter = _adapter,
                Device = _device12,
                Queue = _queue,
            };
            _skiaContext = GRContext.CreateDirect3D(_skiaBackend) ??
                throw new InvalidOperationException("Skia could not create the Composition D3D12 context.");
            _copyAllocator = _device12.CreateCommandAllocator(CommandListType.Direct);
            _copyCommandList = _device12.CreateCommandList<ID3D12GraphicsCommandList>(
                CommandListType.Direct, _copyAllocator, null);
            _copyCommandList.Close();
            _copyFence = _device12.CreateFence(0, Vortice.Direct3D12.FenceFlags.None);
        }

        if (_visual is null)
        {
            _brush = _compositor.CreateSurfaceBrush();
            _brush.Stretch = CompositionStretch.None;
            _brush.HorizontalAlignmentRatio = 0;
            _brush.VerticalAlignmentRatio = 0;
            _visual = _compositor.CreateSpriteVisual();
            _visual.Brush = _brush;
            _visual.Opacity = 1;
        }

        if (!ReferenceEquals(host, _attachedHost))
        {
            _invokeOnUiThread(() =>
            {
                if (_attachedHost is not null)
                    ElementCompositionPreview.SetElementChildVisual(_attachedHost, null);
                ElementCompositionPreview.SetElementChildVisual(host, _visual);
                _uiTeardown = false;
            });
            _attachedHost = host;
        }
    }

    internal void PrepareForUiTeardown(DorotiWindowsDxgiHost host)
    {
        if (!ReferenceEquals(host, _attachedHost)) return;
        ElementCompositionPreview.SetElementChildVisual(host, null);
        _attachedHost = null;
        _uiTeardown = true;
    }

    private void WaitForFence(ulong fenceValue)
    {
        if (_copyFence!.CompletedValue >= fenceValue) return;
        using var completion = new EventWaitHandle(false, EventResetMode.AutoReset);
        _copyFence.SetEventOnCompletion(fenceValue, completion).CheckError();
        if (!completion.WaitOne(TimeSpan.FromSeconds(5)))
            throw new TimeoutException($"Composition D3D12 fence {fenceValue} did not complete.");
    }

    internal void Reset()
    {
        if (!_noPendingCommits.Wait(TimeSpan.FromSeconds(5)))
            throw new TimeoutException("Composition commits did not drain within five seconds during reset.");
        _invokeOnUiThread(() =>
        {
            if (_attachedHost is not null)
            {
                ElementCompositionPreview.SetElementChildVisual(_attachedHost, null);
                _attachedHost = null;
            }
            foreach (var slot in _slots) slot.Dispose();
            _slots.Clear();
            _visual?.Dispose();
            _visual = null;
            _brush?.Dispose();
            _brush = null;
            _graphicsDevice?.Dispose();
            _graphicsDevice = null;
        });
        _front = null;
        _backingStore?.Dispose();
        _backingStore = null;
        _on12?.Dispose();
        _on12 = null;
        _context11?.Dispose();
        _context11 = null;
        _device11?.Dispose();
        _device11 = null;
        _copyFence?.Dispose();
        _copyFence = null;
        _copyCommandList?.Dispose();
        _copyCommandList = null;
        _copyAllocator?.Dispose();
        _copyAllocator = null;
        _skiaContext?.AbandonContext(false);
        _skiaContext?.Dispose();
        _skiaContext = null;
        _skiaBackend?.Dispose();
        _skiaBackend = null;
        _queue?.Dispose();
        _queue = null;
        _device12?.Dispose();
        _device12 = null;
        _adapter?.Dispose();
        _adapter = null;
        _factory?.Dispose();
        _factory = null;
        Width = Height = 0;
        _nextFenceValue = 0;
        _commitSerial = 0;
        _completedCommitSerial = 0;
        _pendingCommitCount = 0;
        _slotWaiter = 0;
        _asyncCommitFailure = null;
        _adapterDescription = "uninitialized";
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Reset();
        _noPendingCommits.Dispose();
    }
}

internal sealed class WindowsCompositionSurfaceSlot(int id) : IDisposable
{
    internal int Id { get; } = id;
    internal int Width { get; private set; }
    internal int Height { get; private set; }
    internal long RetireAfterCommit { get; set; }
    internal WindowsCompositionSurfaceSlotState State { get; private set; } =
        WindowsCompositionSurfaceSlotState.Free;
    internal CompositionDrawingSurface Surface { get; private set; } = null!;

    internal void Prepare(CompositionGraphicsDevice graphicsDevice, int width, int height)
    {
        if (State != WindowsCompositionSurfaceSlotState.Free)
            throw new InvalidOperationException($"Composition slot {Id} is not free.");
        if (Surface is null)
        {
            Surface = graphicsDevice.CreateDrawingSurface2(
                new SizeInt32(width, height),
                Microsoft.Graphics.DirectX.DirectXPixelFormat.R8G8B8A8UIntNormalized,
                Microsoft.Graphics.DirectX.DirectXAlphaMode.Premultiplied);
        }
        else if (Width != width || Height != height)
        {
            WindowsCompositionInterop.Resize(Surface, width, height);
        }
        Width = width;
        Height = height;
    }

    internal void Transition(WindowsCompositionSurfaceSlotState target)
    {
        var valid = (State, target) switch
        {
            (WindowsCompositionSurfaceSlotState.Free, WindowsCompositionSurfaceSlotState.Drawing) => true,
            (WindowsCompositionSurfaceSlotState.Drawing, WindowsCompositionSurfaceSlotState.GpuWorkQueued) => true,
            (WindowsCompositionSurfaceSlotState.GpuWorkQueued, WindowsCompositionSurfaceSlotState.DrawEnded) => true,
            (WindowsCompositionSurfaceSlotState.DrawEnded, WindowsCompositionSurfaceSlotState.PendingVisualCommit) => true,
            (WindowsCompositionSurfaceSlotState.PendingVisualCommit, WindowsCompositionSurfaceSlotState.Front) => true,
            (WindowsCompositionSurfaceSlotState.Front, WindowsCompositionSurfaceSlotState.Retired) => true,
            (WindowsCompositionSurfaceSlotState.Retired, WindowsCompositionSurfaceSlotState.Free) => true,
            _ => false,
        };
        if (!valid)
            throw new InvalidOperationException($"Illegal Composition slot transition {State} -> {target} for {Id}.");
        State = target;
    }

    internal void AbandonDrawEnded()
    {
        if (State != WindowsCompositionSurfaceSlotState.DrawEnded)
            throw new InvalidOperationException($"Composition slot {Id} cannot abandon {State}.");
        State = WindowsCompositionSurfaceSlotState.Free;
    }

    public void Dispose()
    {
        Surface?.Dispose();
        Surface = null!;
    }
}

internal enum WindowsCompositionSurfaceSlotState
{
    Free,
    Drawing,
    GpuWorkQueued,
    DrawEnded,
    PendingVisualCommit,
    Front,
    Retired,
}

internal static class WindowsCompositionInterop
{
    private static readonly Guid CompositorInteropIid = new("FAB19398-6D19-4D8A-B752-8F096C396069");
    private static readonly Guid DrawingSurfaceInteropIid = new("2D6355C2-AD57-4EAE-92E4-4C3EFF65D578");
    private static readonly Guid Texture2DIid = new("6F15AAF2-D208-4E89-9AB4-489535D34F9C");

    internal static unsafe CompositionGraphicsDevice CreateGraphicsDevice(
        Compositor compositor,
        ID3D11Device renderingDevice)
    {
        using var interop = ((WinRT.IWinRTObject)compositor).NativeObject.As(CompositorInteropIid);
        var thisPointer = interop.ThisPtr;
        var vtable = *(nint**)thisPointer;
        var create = (delegate* unmanaged[Stdcall]<nint, nint, nint*, int>)vtable[3];
        nint result = 0;
        Check(create(thisPointer, renderingDevice.NativePointer, &result));
        try
        {
            return WinRT.MarshalInterface<CompositionGraphicsDevice>.FromAbi(result);
        }
        finally
        {
            Marshal.Release(result);
        }
    }

    internal static unsafe WindowsCompositionActiveDraw BeginDraw(CompositionDrawingSurface surface)
    {
        var interop = ((WinRT.IWinRTObject)surface).NativeObject.As(DrawingSurfaceInteropIid);
        var thisPointer = interop.ThisPtr;
        var vtable = *(nint**)thisPointer;
        var begin = (delegate* unmanaged[Stdcall]<nint, void*, Guid*, nint*, WindowsCompositionNativePoint*, int>)vtable[3];
        nint texture = 0;
        WindowsCompositionNativePoint offset = default;
        var textureIid = Texture2DIid;
        Check(begin(thisPointer, null, &textureIid, &texture, &offset));
        return new WindowsCompositionActiveDraw(interop, texture, offset);
    }

    internal static unsafe void EndDraw(WindowsCompositionActiveDraw draw)
    {
        if (draw.Ended) return;
        var thisPointer = draw.Interop.ThisPtr;
        var vtable = *(nint**)thisPointer;
        var end = (delegate* unmanaged[Stdcall]<nint, int>)vtable[4];
        Check(end(thisPointer));
        draw.Ended = true;
        draw.Interop.Dispose();
    }

    internal static unsafe void Resize(CompositionDrawingSurface surface, int width, int height)
    {
        using var interop = ((WinRT.IWinRTObject)surface).NativeObject.As(DrawingSurfaceInteropIid);
        var thisPointer = interop.ThisPtr;
        var vtable = *(nint**)thisPointer;
        var resize = (delegate* unmanaged[Stdcall]<nint, WindowsCompositionNativeSize, int>)vtable[5];
        Check(resize(thisPointer, new WindowsCompositionNativeSize(width, height)));
    }

    private static void Check(int hresult)
    {
        if (hresult < 0) Marshal.ThrowExceptionForHR(hresult);
    }
}

internal sealed class WindowsCompositionActiveDraw(
    WinRT.IObjectReference interop,
    nint texturePointer,
    WindowsCompositionNativePoint offset)
{
    internal WinRT.IObjectReference Interop { get; } = interop;
    internal nint TexturePointer { get; set; } = texturePointer;
    internal WindowsCompositionNativePoint Offset { get; } = offset;
    internal bool Ended { get; set; }
}

[StructLayout(LayoutKind.Sequential)]
internal readonly record struct WindowsCompositionNativePoint(int X, int Y);

[StructLayout(LayoutKind.Sequential)]
internal readonly record struct WindowsCompositionNativeSize(int Width, int Height);
#endif
