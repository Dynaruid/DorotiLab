using System.Diagnostics;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace Doroti.Host.WindowsAppSdk;

internal sealed unsafe partial class WindowsManagedVulkanPresenter
{
    [LibraryImport(WindowsNativeV1.LibraryName,
        EntryPoint = "doroti_windows_vulkan_composition_trace_prepared_v1")]
    private static partial void TracePreparedCopyComplete();
    private readonly PreparedMovingFrameLedger _preparedMoving = new();
    private MovingFrameKey? _movingPrepareRequest;
    private PreparedMovingFrame? _phaseAlignedFrame;
    private ulong _postGeometryFallback;
    internal ulong MovingOriginWindowPosCommitAttempt { get; private set; }
    internal ulong MovingOriginWindowPosCommitMismatch { get; private set; }
    internal ulong MovingOriginWindowPosCommitFailed { get; private set; }
    internal void RecordPreparedMismatch()
    {
        lock (_viewportGate) MovingOriginWindowPosCommitMismatch++;
    }
    internal bool LastPrepareSucceeded { get; private set; }

    internal (ulong Prepared, ulong Cancelled, ulong Committed, int Reserved) PreparedDiagnostics
    {
        get
        {
            lock (_viewportGate)
                return (_preparedMoving.Prepared, _preparedMoving.Cancelled,
                    _preparedMoving.Committed, _preparedMoving.Current is null ? 0 : 1);
        }
    }

    // Raster-worker-only entry point. Ordinary frames continue to call
    // RenderAndPresent directly. The native host owns the later commit/cancel.
    internal T RenderAndPrepare<T>(MovingFrameKey key,
        Func<SKSurface, T> paint, Predicate<T> shouldPrepare)
    {
        LastPrepareSucceeded = false;
        lock (_viewportGate)
        {
            _phaseAlignedFrame = null;
            if (!SizingEdgeMovesWindowOrigin(key.SizingEdge) ||
                key.Width != _viewportWidth || key.Height != _viewportHeight ||
                key.Scale != _viewportScale)
                throw new InvalidOperationException("Prepared key differs from the exact viewport.");
            _movingPrepareRequest = key;
        }
        try { return RenderAndPresent(paint, shouldPrepare); }
        catch
        {
            CancelPreparedMovingFrame();
            throw;
        }
        finally
        {
            lock (_viewportGate)
            {
                _movingPrepareRequest = null;
                if (!LastPrepareSucceeded) _preparedMoving.Cancel();
            }
        }
    }

    internal void CancelPreparedMovingFrame()
    {
        lock (_viewportGate)
        {
            _movingPrepareRequest = null;
            _phaseAlignedFrame = null;
            _preparedMoving.Cancel();
            LastPrepareSucceeded = false;
        }
    }

    internal int AlignPreparedMovingFrame(MovingFrameKey key)
    {
        lock (_viewportGate)
        {
            if (!_preparedMoving.Matches(key, _viewportRevision) ||
                _presentationRetiring || _presentationPoisoned)
                return 1;
            var frame = _preparedMoving.Current!.Value;
            if (_phaseAlignedFrame == frame) return 0;
            WaitForPreparedResizeClock();
            _phaseAlignedFrame = frame;
            return 0;
        }
    }

    // Platform thread, after actual HWND geometry. Copy and clock alignment are
    // already complete. Submit immediately, then await this present's bounded
    // CompositionFrame receipt before allowing the next geometry transaction.
    internal int CommitPreparedMovingFrame(MovingFrameKey key)
    {
        lock (_viewportGate)
        {
            MovingOriginWindowPosCommitAttempt++;
            if (!_preparedMoving.Matches(key, _viewportRevision) ||
                _phaseAlignedFrame != _preparedMoving.Current ||
                _presentationRetiring || _presentationPoisoned)
            {
                MovingOriginWindowPosCommitMismatch++;
                _phaseAlignedFrame = null;
                _preparedMoving.Cancel();
                return 1;
            }
            var frame = _preparedMoving.Current!.Value;
            _phaseAlignedFrame = null;
            try
            {
                // Never inherit the ordinary post-geometry display wait.
                if (_displayWaitViewportRevision == frame.ViewportRevision)
                    throw new InvalidOperationException("Prepared commit inherited a display wait.");
                PresentSlotLocked(frame.SlotIndex, frame.ViewportRevision, "moving-origin-windowpos-commit", waitForResizeReceipt: true);
                if (!LastPresentSucceeded)
                    throw new InvalidOperationException("Prepared Present did not complete.");
                _preparedMoving.Complete(key, frame.ViewportRevision);
                LastPrepareSucceeded = false;
                return 0;
            }
            catch
            {
                MovingOriginWindowPosCommitFailed++;
                _preparedMoving.Cancel();
                LastPrepareSucceeded = false;
                return -1;
            }
        }
    }

    private void WaitForPreparedResizeClock()
    {
        // Platform thread, after exact key validation and completed worker copy.
        // Keep signal -> geometry -> Present on one thread; a
        // worker-side signal would leave a scheduler handoff before geometry.
        // This bounded phase alignment is not an atomic display receipt.
        _resizeClockWaitCount++;
        var started = Stopwatch.GetTimestamp();
        try
        {
            _lastResizeClockStatus = TakeInjectedResult("COMPOSITOR_CLOCK_TIMEOUT")
                ? 0x00000102u // STATUS_TIMEOUT; existing one-shot fault injection
                : DCompositionWaitForCompositorClock(0, 0, 32);
        }
        catch (EntryPointNotFoundException)
        {
            _lastResizeClockStatus = 0xc0000002; // STATUS_NOT_IMPLEMENTED
        }
        _maximumResizeClockWaitMicroseconds = Math.Max(
            _maximumResizeClockWaitMicroseconds,
            checked((long)Stopwatch.GetElapsedTime(started).TotalMicroseconds));
        if (_lastResizeClockStatus == 0)
            _resizeClockSignalCount++;
        else
        {
            // Timeout/occlusion/failure still submits and releases the HWND;
            // qualification must reject the recorded failure.
            _resizeClockFailureCount++;
            RecordEvent($"resize clock wait failed status=0x{_lastResizeClockStatus:x8}");
        }
    }

    internal bool PreparedSlotIsAvailableForValidation()
    {
        lock (_viewportGate)
        {
            if (_preparedMoving.Current is not { } frame) return false;
            Marshal.ThrowExceptionForHR(IsCompositionBufferAvailable(
                _presentationContext, checked((uint)frame.SlotIndex), out var available));
            var handle = unchecked((nint)_presentationSlots[frame.SlotIndex].AvailableEvent);
            return available != 0 && WaitForMultipleObjects(1, &handle, false, 0) == WaitObject0;
        }
    }
}
