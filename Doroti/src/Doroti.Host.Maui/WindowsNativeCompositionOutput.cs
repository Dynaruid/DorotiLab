#if WINDOWS
using System.Runtime.InteropServices;
using Doroti.Graphics.DirectX;
using static Doroti.Graphics.DirectX.DirectX;
using DX = Silk.NET.DXGI;
using D3D = Silk.NET.Direct3D12;

namespace Doroti.Host.Maui;

/// <summary>
/// HWND-attached, premultiplied DXGI output for the full-window Doroti page.
/// One retained-capacity swap chain stays attached during live resize. Only its
/// back buffer changes; SetContent is not a per-frame presentation operation.
/// DWM owns displayed buffers, so reuse has no XAML completion dependency.
/// </summary>
internal sealed unsafe class WindowsNativeCompositionOutput : IDisposable
{
    private sealed class Slot : IDisposable
    {
        internal IDXGISwapChain3? Chain;
        internal int CapacityWidth;
        internal int CapacityHeight;
        internal int Width;
        internal int Height;

        public void Dispose() => Chain?.Dispose();
    }

    private readonly Slot _slot = new();
    private readonly nint _window;
    private readonly IDXGIFactory6 _factory;
    private readonly ID3D12CommandQueue _queue;
    private readonly ID3D12CommandAllocator _allocator;
    private readonly ID3D12GraphicsCommandList _commands;
    private readonly ID3D12Fence _fence;
    private readonly object _gate = new();
    private nint _compositionDevice;
    private nint _target;
    private nint _visual;
    private bool _attached;
    private int _top;
    private ulong _fenceValue;
    private bool _detached;
    private bool _disposed;
    private bool _copyPending;

    internal WindowsNativeCompositionOutput(
        nint window,
        IDXGIFactory6 factory,
        ID3D12Device2 device,
        ID3D12CommandQueue queue
    )
    {
        _window = window;
        _factory = factory;
        _queue = queue;
        _allocator = device.CreateCommandAllocator(D3D.CommandListType.Direct);
        _commands = device.CreateCommandList<ID3D12GraphicsCommandList>(
            D3D.CommandListType.Direct,
            _allocator,
            null
        );
        _commands.Close();
        _fence = device.CreateFence(0, D3D.FenceFlags.None);
        try
        {
            var iid = new Guid("C37EA93A-E7AA-450D-B16F-9746CB0407F3");
            Check(DCompositionCreateDevice(0, in iid, out _compositionDevice));
            nint target = 0,
                visual = 0;
            Check(
                (
                    (delegate* unmanaged[Stdcall]<nint, nint, int, nint*, int>)
                        Vtable(_compositionDevice)[6]
                )(_compositionDevice, window, 1, &target)
            );
            _target = target;
            Check(
                ((delegate* unmanaged[Stdcall]<nint, nint*, int>)Vtable(_compositionDevice)[7])(
                    _compositionDevice,
                    &visual
                )
            );
            _visual = visual;
            Check(SetPointer(_target, 3, _visual));
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    internal long ContentAttachments { get; private set; }
    internal long Copies { get; private set; }
    internal long Presents { get; private set; }
    internal long ResizeBuffers { get; private set; }

    internal void Prepare(
        ID3D12Resource source,
        int width,
        int height,
        D3D.ResourceStates sourceState
    )
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            WaitForCopy();
            var slot = _slot;
            // Reserve the current monitor's physical extent once. Ordinary
            // grow/shrink and maximize/restore must not clear visible buffers.
            var monitor = new MonitorInfo { Size = (uint)Marshal.SizeOf<MonitorInfo>() };
            var capacityWidth = Math.Max(width, slot.CapacityWidth);
            var capacityHeight = Math.Max(height, slot.CapacityHeight);
            if (GetMonitorInfoW(MonitorFromWindow(_window, 2), ref monitor))
            {
                capacityWidth = Math.Max(
                    capacityWidth,
                    monitor.Monitor.Right - monitor.Monitor.Left
                );
                capacityHeight = Math.Max(
                    capacityHeight,
                    monitor.Monitor.Bottom - monitor.Monitor.Top
                );
            }
            capacityWidth = checked((capacityWidth + 255) / 256 * 256);
            capacityHeight = checked((capacityHeight + 255) / 256 * 256);
            if (slot.Chain is null)
            {
                using var chain = _factory.CreateSwapChainForComposition(
                    _queue,
                    new(
                        (uint)capacityWidth,
                        (uint)capacityHeight,
                        DX.Format.FormatR8G8B8A8Unorm,
                        false,
                        Usage.RenderTargetOutput,
                        2,
                        DX.Scaling.Stretch,
                        DX.SwapEffect.FlipSequential,
                        DX.AlphaMode.Premultiplied,
                        DX.SwapChainFlag.None
                    ),
                    null
                );
                slot.Chain = chain.QueryInterface<IDXGISwapChain3>();
                slot.CapacityWidth = capacityWidth;
                slot.CapacityHeight = capacityHeight;
            }
            else if (slot.CapacityWidth < width || slot.CapacityHeight < height)
            {
                // Only growth beyond retained monitor capacity takes this path.
                // All prior copies have drained; DXGI owns display retirement.
                slot.Chain.ResizeBuffers(
                        2,
                        (uint)capacityWidth,
                        (uint)capacityHeight,
                        DX.Format.FormatR8G8B8A8Unorm,
                        DX.SwapChainFlag.None
                    )
                    .CheckError();
                slot.CapacityWidth = capacityWidth;
                slot.CapacityHeight = capacityHeight;
                ResizeBuffers++;
            }
            slot.Width = width;
            slot.Height = height;
            // Keep the full buffer's natural pixel size. SetSourceSize would
            // stretch this crop back to retained capacity on a composition chain.
            // The HWND clips retained capacity at the actual client rectangle.
            _allocator.Reset();
            _commands.Reset(_allocator);
            using (
                var buffer = slot.Chain.GetBuffer<ID3D12Resource>(slot.Chain.CurrentBackBufferIndex)
            )
            {
                _commands.ResourceBarrier([
                    ResourceBarrier.BarrierTransition(
                        source,
                        sourceState,
                        D3D.ResourceStates.CopySource
                    ),
                    ResourceBarrier.BarrierTransition(
                        buffer,
                        D3D.ResourceStates.Present,
                        D3D.ResourceStates.CopyDest
                    ),
                ]);
                _commands.CopyTextureRegion(
                    new TextureCopyLocation(buffer, 0),
                    0,
                    0,
                    0,
                    new TextureCopyLocation(source, 0),
                    null
                );
                _commands.ResourceBarrier([
                    ResourceBarrier.BarrierTransition(
                        source,
                        D3D.ResourceStates.CopySource,
                        sourceState
                    ),
                    ResourceBarrier.BarrierTransition(
                        buffer,
                        D3D.ResourceStates.CopyDest,
                        D3D.ResourceStates.Present
                    ),
                ]);
                _commands.Close();
                _copyPending = true;
                _fenceValue++;
                _queue.ExecuteCommandList(_commands);
            }
            _queue.Signal(_fence, _fenceValue).CheckError();
            WaitForCopy();
            Copies++;
        }
    }

    internal bool Present(int top, long generation)
    {
        lock (_gate)
        {
            if (_detached)
            {
                return false;
            }
            var slot = _slot;
            if (!_attached || _top != top)
            {
                // MSVC overload vtable order: SetOffsetY(float)=6.
                // Verified against the installed Windows SDK dcomp.h ABI.
                Check(
                    ((delegate* unmanaged[Stdcall]<nint, float, int>)Vtable(_visual)[6])(
                        _visual,
                        top
                    )
                );
                if (!_attached)
                {
                    Check(SetPointer(_visual, 15, slot.Chain!.NativePointer));
                    ContentAttachments++;
                    _attached = true;
                }
                Check(Call(_compositionDevice, 3));
                _top = top;
            }
            WindowsResizeTimeline.Record(
                _window,
                "present-enter",
                generation: generation,
                bufferWidth: slot.Width,
                bufferHeight: slot.Height
            );
            slot.Chain!.Present(0, PresentFlags.None).CheckError();
            WindowsResizeTimeline.Record(
                _window,
                "present-exit",
                generation: generation,
                bufferWidth: slot.Width,
                bufferHeight: slot.Height
            );
            // Include queue work inserted by DXGI before any later buffer resize
            // or teardown, not just the copy recorded before Present.
            _copyPending = true;
            _queue.Signal(_fence, ++_fenceValue).CheckError();
            WaitForCopy();
            // Do not call DwmFlush inside the nested WM_SIZE transaction.
            // USER32/WinUI must finish their geometry handling before the next
            // desktop frame. Flushing here displayed new pixels at old bounds.
            // The copy fence above protects GPU resources, not display timing.
            Presents++;
            return true;
        }
    }

    internal void Detach()
    {
        lock (_gate)
        {
            if (_detached || _target == 0)
            {
                return;
            }
            _detached = true;
            Check(SetPointer(_target, 3, 0));
            Check(Call(_compositionDevice, 3));
            Check(Call(_compositionDevice, 4));
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        // A timed-out copy is not permission to dispose its GPU resources.
        WaitForCopy();
        Detach();
        _disposed = true;
        _slot.Dispose();
        if (_visual != 0)
            Marshal.Release(_visual);
        if (_target != 0)
            Marshal.Release(_target);
        if (_compositionDevice != 0)
            Marshal.Release(_compositionDevice);
        _visual = _target = _compositionDevice = 0;
        _fence.Dispose();
        _commands.Dispose();
        _allocator.Dispose();
    }

    private void WaitForCopy()
    {
        if (!_copyPending)
        {
            return;
        }
        if (_fence.CompletedValue < _fenceValue)
        {
            using var completed = new EventWaitHandle(false, EventResetMode.AutoReset);
            _fence.SetEventOnCompletion(_fenceValue, completed).CheckError();
            // Present runs on the STA UI thread. CLR waits can dispatch COM or
            // window messages and re-enter a partially applied resize.
            var result = WaitForSingleObject(completed.SafeWaitHandle, 5000);
            if (result == 258)
            {
                throw new TimeoutException(
                    "Native Composition copy did not complete; retaining GPU resources."
                );
            }
            if (result != 0)
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
        }
        _copyPending = false;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        internal int Left,
            Top,
            Right,
            Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MonitorInfo
    {
        internal uint Size;
        internal NativeRect Monitor,
            Work;
        internal uint Flags;
    }

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern nint MonitorFromWindow(nint window, uint flags);

    [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
    private static extern uint WaitForSingleObject(
        Microsoft.Win32.SafeHandles.SafeWaitHandle handle,
        uint milliseconds
    );

    [DllImport("user32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetMonitorInfoW(nint monitor, ref MonitorInfo info);

    internal static uint AlignResizeWithCompositor()
    {
        try
        {
            return DCompositionWaitForCompositorClock(0, 0, 32);
        }
        catch (EntryPointNotFoundException)
        {
            return 0xc0000002; // STATUS_NOT_IMPLEMENTED; report without blocking USER32.
        }
    }

    [DllImport("dcomp.dll", ExactSpelling = true)]
    private static extern uint DCompositionWaitForCompositorClock(
        uint count,
        nint handles,
        uint timeout
    );

    private static nint* Vtable(nint value) => *(nint**)value;

    private static int Call(nint value, int index) =>
        ((delegate* unmanaged[Stdcall]<nint, int>)Vtable(value)[index])(value);

    private static int SetPointer(nint value, int index, nint argument) =>
        ((delegate* unmanaged[Stdcall]<nint, nint, int>)Vtable(value)[index])(value, argument);

    private static void Check(int result) => Marshal.ThrowExceptionForHR(result);

    [DllImport("dcomp.dll", ExactSpelling = true)]
    private static extern int DCompositionCreateDevice(nint device, in Guid iid, out nint result);
}
#endif
