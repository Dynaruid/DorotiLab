using Doroti.Skia.RuntimeEffects;
using SkiaSharp;

namespace Doroti.Skia.Rendering;

/// <summary>
/// One native Graphite context generation, owned by one render thread. The host
/// owns the device, queue and output textures and must outlive this session.
/// Submission does not acknowledge GPU completion or platform presentation.
/// </summary>
public sealed class SkiaGraphiteSession : IDisposable
{
    public const long ContextBudgetBytes = 256L * 1024 * 1024;
    public const long RecorderBudgetBytes = 64L * 1024 * 1024;
    private readonly int _ownerThread = Environment.CurrentManagedThreadId;
    private readonly SKGraphiteContext _context;
    private readonly SKGraphiteImageCache _images = new();
    private readonly SKGraphiteRecorder _recorder;
    private readonly HashSet<Frame> _frames = [];
    private Frame? _recordingFrame;
    private bool _stopping;
    private bool _disposed;
    private bool _faulted;
    private int _pendingReadbacks;

    private SkiaGraphiteSession(SKGraphiteContext context, long generation, int maxFrames)
    {
        _context = context;
        Generation = generation;
        MaxFrames = maxFrames;
        try
        {
            _recorder = context.CreateRecorder(RecorderBudgetBytes,
                (recorder, image, mipmapped) => _images.FindOrCreate(recorder, image, mipmapped))
                ?? throw new InvalidOperationException("Graphite recorder creation failed.");
        }
        catch
        {
            _images.Dispose();
            context.Dispose();
            throw;
        }
    }

    public long Generation { get; }
    public int MaxFrames { get; }
    public int OutstandingFrames { get { CheckOwner(); return _frames.Count; } }
    public bool CanBeginFrame
    {
        get
        {
            CheckOwner();
            return !_stopping && !_faulted && _recordingFrame is null && _frames.Count < MaxFrames;
        }
    }

    // Vulkan needs the enabled-feature/state/semaphore bridge before it can
    // implement this session's submission boundary. Do not use CreateVulkan's
    // incomplete public descriptor as an implicit alternate factory here.
    public static SkiaGraphiteSession CreateMetal(nint device, nint queue, long generation, int maxFrames = 3)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(generation, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(maxFrames, 1);
        if (!SKGraphiteContext.IsBackendAvailable(SKGraphiteBackend.Metal))
            throw new PlatformNotSupportedException("Loaded Skia native asset has no Graphite/Metal backend.");
        using var backend = new SKGraphiteMtlBackendContext { MtlDevice = device, MtlQueue = queue };
        var context = SKGraphiteContext.CreateMetal(backend,
            new SKGraphiteContextOptions { GpuBudgetInBytes = ContextBudgetBytes })
            ?? throw new InvalidOperationException("Graphite/Metal context creation failed.");
        return new(context, generation, maxFrames);
    }

    /// <summary>Wraps a borrowed Metal texture. The host retains it through CompleteGpuWork.</summary>
    public Frame BeginMetalFrame(int width, int height, nint texture)
    {
        CheckOwner();
        if (!CanBeginFrame) throw new InvalidOperationException("Graphite session is stopping, faulted or at its frame limit.");
        if (_context.IsDeviceLost)
        {
            _faulted = true;
            throw new InvalidOperationException("Graphite/Metal device lost.");
        }
        _context.CheckAsyncWorkCompletion();
        var backend = SKGraphiteBackendTexture.CreateMetal(width, height, texture)
            ?? throw new InvalidOperationException("Graphite Metal texture wrapping failed.");
        try
        {
            var surface = SKSurface.Create(_recorder, backend, SKColorType.Bgra8888)
                ?? throw new InvalidOperationException("Graphite Metal output surface creation failed.");
            var frame = new Frame(this, backend, SkiaGpuSurfaces.Register(surface, _recorder));
            _frames.Add(frame);
            _recordingFrame = frame;
            return frame;
        }
        catch { backend.Dispose(); throw; }
    }

    public void StopAcceptingFrames() { CheckOwner(); _stopping = true; }

    public void Dispose()
    {
        if (_disposed) return;
        CheckOwner();
        _stopping = true;
        if (_frames.Count != 0 || _pendingReadbacks != 0)
            throw new InvalidOperationException("Host must complete GPU work and return all Graphite frames before disposal.");
        _context.CheckAsyncWorkCompletion();
        _images.Dispose();
        _recorder.Dispose();
        _context.Dispose();
        _disposed = true;
    }

    private void CheckOwner()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_ownerThread != Environment.CurrentManagedThreadId)
            throw new InvalidOperationException("Graphite session accessed outside its render owner thread.");
    }

    public sealed class Frame
    {
        private readonly SkiaGraphiteSession _session;
        private readonly SKGraphiteBackendTexture _backend;
        private readonly SKSurface _surface;
        private SKGraphiteRecording? _recording;
        private bool _submissionAttempted;
        private bool _returned;
        private SKImageInfo? _readbackInfo;
        private TaskCompletionSource<SkiaGraphiteReadback>? _readback;
        private bool _readbackPending;

        internal Frame(SkiaGraphiteSession session, SKGraphiteBackendTexture backend, SKSurface surface)
        {
            _session = session;
            _backend = backend;
            _surface = surface;
        }

        public SKSurface Surface
        {
            get
            {
                _session.CheckOwner();
                if (_returned || _submissionAttempted)
                    throw new InvalidOperationException("Graphite output is only writable while its frame is recording.");
                return _surface;
            }
        }
        public long Generation => _session.Generation;

        /// <summary>Schedules a diagnostic/capture readback with the frame, never a presentation copy.</summary>
        public Task<SkiaGraphiteReadback> RequestReadback(SKImageInfo info)
        {
            _session.CheckOwner();
            if (_returned || _submissionAttempted || _readback is not null)
                throw new InvalidOperationException("Readback must be requested once before frame submission.");
            if (info.Width != _backend.Dimensions.Width || info.Height != _backend.Dimensions.Height)
                throw new ArgumentException("Readback dimensions must match the complete output texture.", nameof(info));
            _readbackInfo = info;
            _readback = new(TaskCreationOptions.RunContinuationsAsynchronously);
            return _readback.Task;
        }

        public void Submit()
        {
            _session.CheckOwner();
            if (_returned || _submissionAttempted || !ReferenceEquals(_session._recordingFrame, this))
                throw new InvalidOperationException("Graphite frame is not available for recording submission.");
            _submissionAttempted = true;
            try
            {
                _recording = _session._recorder.Snap()
                    ?? throw new InvalidOperationException("Graphite Snap failed.");
                var status = _session._context.InsertRecording(_recording);
                if (status != SKGraphiteInsertStatus.Success)
                    throw new InvalidOperationException($"Graphite recording rejected: {status}.");
                if (_readbackInfo is { } info)
                {
                    _session._pendingReadbacks++;
                    _readbackPending = true;
                    try
                    {
                        _session._context.RequestReadPixels(_surface, info, new SKRectI(0, 0, info.Width, info.Height),
                            SKImageRescaleGamma.Src, SKImageRescaleMode.Nearest, result =>
                            {
                                // Never propagate application exceptions across the native callback.
                                try
                                {
                                    if (result is null) throw new InvalidOperationException("Graphite asynchronous readback failed.");
                                    _readback!.TrySetResult(new(info, result.GetPlaneRowBytes(0), result.ToArray(0)));
                                }
                                catch (Exception exception) { _readback!.TrySetException(exception); }
                                finally { _session._pendingReadbacks--; _readbackPending = false; }
                            });
                    }
                    catch { _session._pendingReadbacks--; _readbackPending = false; throw; }
                }
                if (!_session._context.Submit(new SKGraphiteSubmitInfo { Sync = false }))
                    throw new InvalidOperationException("Graphite Submit failed.");
            }
            catch (Exception exception)
            {
                _session._faulted = true;
                _readback?.TrySetException(exception);
                throw;
            }
            finally { _session._recordingFrame = null; }
        }

        /// <summary>
        /// Call on the owner thread only after the host's same-queue completion
        /// fence has completed, including failed submission attempts. This
        /// releases GPU ownership; the host separately records present results.
        /// </summary>
        public void CompleteGpuWork()
        {
            _session.CheckOwner();
            if (_returned || !_submissionAttempted)
                throw new InvalidOperationException("Graphite frame has no outstanding submission.");
            _session._context.CheckAsyncWorkCompletion();
            if (_readbackPending)
                throw new InvalidOperationException("Graphite readback callback has not completed; retain this frame and poll again.");
            Release();
        }

        /// <summary>Discards an unsubmitted recording, for example after a stale viewport check.</summary>
        public void CancelRecording()
        {
            _session.CheckOwner();
            if (_returned || _submissionAttempted)
                throw new InvalidOperationException("A submitted Graphite frame requires host GPU completion.");
            using var discarded = _session._recorder.Snap();
            _readback?.TrySetCanceled();
            _session._recordingFrame = null;
            Release();
        }

        private void Release()
        {
            _surface.Dispose();
            _backend.Dispose();
            _recording?.Dispose();
            _session._frames.Remove(this);
            _returned = true;
        }
    }
}

public sealed record SkiaGraphiteReadback(SKImageInfo Info, int RowBytes, byte[] Pixels);
