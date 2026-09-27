namespace Doroti.Host.WindowsAppSdk;

// Native input arrives on the platform thread while the render worker owns
// framework dispatch/build/raster. Queued input belongs to the NEXT frame;
// receiving it must not invalidate the frame currently being drawn.
internal sealed class WindowsFrameInputState
{
    private long _received;
    private long _dispatched;

    internal long Received => Volatile.Read(ref _received);
    internal long Dispatched => Volatile.Read(ref _dispatched);

    internal long Receive() => Interlocked.Increment(ref _received);

    // Called by the render worker before delivering the queued input callback.
    internal void Dispatch(long sequence) => Volatile.Write(ref _dispatched, sequence);

    internal bool CanPresent(long sequence) => sequence >= Dispatched;

    // Prepared resize buffers can outlive a frame and still need the stricter
    // check when the platform thread commits a previously prepared buffer.
    internal bool IsLatestReceived(long sequence) => sequence >= Received;
}
