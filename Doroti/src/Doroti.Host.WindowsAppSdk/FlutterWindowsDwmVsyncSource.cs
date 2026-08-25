using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>
/// Per-view source of composition timing.  Sampling is deliberately passive:
/// the F4 raster owner retains <c>eglSwapInterval(1)</c> as the presentation
/// throttle, while this source supplies the refresh cadence and a causal
/// timestamp to the frame scheduler.
/// </summary>
internal interface IFlutterWindowsVsyncSource : IDisposable
{
    FlutterWindowsVsyncSample SampleNext(ulong viewId);
}

internal readonly record struct FlutterWindowsVsyncSample(
    ulong ViewId,
    long Sequence,
    TimeSpan Timestamp,
    TimeSpan Interval,
    double RefreshRateHz,
    bool IsDwmCompositionTiming);

internal sealed record FlutterWindowsDwmVsyncSourceSnapshot(
    nint ChildHwnd,
    nint DwmTimingCallHwnd,
    bool UsesDesktopCompositionTiming,
    long NativeTimingSampleCount,
    long FallbackSampleCount,
    long LastSequence,
    TimeSpan LastInterval,
    double LastRefreshRateHz,
    int LastHResult,
    bool Disposed);

/// <summary>
/// Samples the desktop composition clock for one bound child view HWND.  The
/// HWND is kept as per-view provenance only: on Windows 8.1 and later
/// <c>DwmGetCompositionTimingInfo</c> requires a null HWND, so passing either
/// the child or its top-level owner would turn every timing sample into
/// <c>E_INVALIDARG</c>. It never runs a native message pump or creates a
/// timing thread; callers decide when a frame is eligible to run and take one
/// sample immediately before it.
/// </summary>
internal sealed class FlutterWindowsDwmVsyncSource : IFlutterWindowsVsyncSource
{
    private static readonly TimeSpan DefaultFallbackInterval = TimeSpan.FromTicks(
        TimeSpan.TicksPerSecond / 60);

    // This source belongs to a single raw-child-HWND view, even though modern
    // DWM exposes only desktop-wide composition timing through this API.
    private readonly nint _childHwnd;
    private readonly TimeSpan _fallbackInterval;
    private long _nativeTimingSampleCount;
    private long _fallbackSampleCount;
    private long _lastSequence;
    private TimeSpan _lastInterval;
    private double _lastRefreshRateHz;
    private int _lastHResult;
    private bool _disposed;

    internal FlutterWindowsDwmVsyncSource(
        nint childHwnd,
        TimeSpan? fallbackInterval = null)
    {
        if (childHwnd == 0) throw new ArgumentOutOfRangeException(nameof(childHwnd));
        _childHwnd = childHwnd;
        _fallbackInterval = fallbackInterval ?? DefaultFallbackInterval;
        if (_fallbackInterval <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(fallbackInterval));
        _lastInterval = _fallbackInterval;
        _lastRefreshRateHz = ToRefreshRateHz(_fallbackInterval);
    }

    public FlutterWindowsVsyncSample SampleNext(ulong viewId)
    {
        if (viewId == 0) throw new ArgumentOutOfRangeException(nameof(viewId));
        ObjectDisposedException.ThrowIf(_disposed, this);

        var timing = new DwmTimingInfo
        {
            cbSize = checked((uint)Marshal.SizeOf<DwmTimingInfo>()),
        };
        // Windows 8.1+ rejects any non-null window handle.  Do not substitute
        // the top-level HWND here: it has the same E_INVALIDARG behavior.
        var hresult = NativeMethods.DwmGetCompositionTimingInfo(nint.Zero, ref timing);
        var interval = _fallbackInterval;
        var timestamp = StopwatchTime();
        var native = hresult >= 0 && timing.qpcRefreshPeriod > 0;
        if (native)
        {
            interval = FromQpc(timing.qpcRefreshPeriod);
            if (interval <= TimeSpan.Zero) interval = _fallbackInterval;
            if (timing.qpcVBlank > 0) timestamp = FromQpc(timing.qpcVBlank);
            Interlocked.Increment(ref _nativeTimingSampleCount);
        }
        else
        {
            Interlocked.Increment(ref _fallbackSampleCount);
        }

        var observedSequence = timing.cRefresh > long.MaxValue
            ? long.MaxValue
            : checked((long)timing.cRefresh);
        var sequence = AdvanceSequence(observedSequence);
        _lastInterval = interval;
        _lastRefreshRateHz = ToRefreshRateHz(interval);
        _lastHResult = hresult;
        return new FlutterWindowsVsyncSample(
            viewId,
            sequence,
            timestamp,
            interval,
            _lastRefreshRateHz,
            native);
    }

    internal FlutterWindowsDwmVsyncSourceSnapshot Snapshot => new(
        _childHwnd,
        nint.Zero,
        true,
        Interlocked.Read(ref _nativeTimingSampleCount),
        Interlocked.Read(ref _fallbackSampleCount),
        Interlocked.Read(ref _lastSequence),
        _lastInterval,
        _lastRefreshRateHz,
        _lastHResult,
        _disposed);

    public void Dispose() => _disposed = true;

    private long AdvanceSequence(long observed)
    {
        while (true)
        {
            var current = Volatile.Read(ref _lastSequence);
            var next = Math.Max(current == long.MaxValue ? current : current + 1, observed);
            if (Interlocked.CompareExchange(ref _lastSequence, next, current) == current)
                return next;
        }
    }

    private static TimeSpan StopwatchTime() => FromQpc((ulong)Stopwatch.GetTimestamp());

    private static TimeSpan FromQpc(ulong qpc) =>
        TimeSpan.FromSeconds(qpc / (double)Stopwatch.Frequency);

    private static double ToRefreshRateHz(TimeSpan interval) =>
        interval <= TimeSpan.Zero ? 0 : 1.0 / interval.TotalSeconds;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct UnsignedRatio
    {
        internal uint uiNumerator;
        internal uint uiDenominator;
    }

    // dwmapi.h wraps DWM_TIMING_INFO in pshpack1.h.  The exact packed layout
    // is part of the ABI: a default 8-byte CLR packing reports 320 bytes while
    // DWM expects 292, yielding MILERR_MISMATCHED_SIZE before it can sample.
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct DwmTimingInfo
    {
        internal uint cbSize;
        internal UnsignedRatio rateRefresh;
        internal ulong qpcRefreshPeriod;
        internal UnsignedRatio rateCompose;
        internal ulong qpcVBlank;
        internal ulong cRefresh;
        internal uint cDXRefresh;
        internal ulong qpcCompose;
        internal ulong cFrame;
        internal uint cDXPresent;
        internal ulong cRefreshFrame;
        internal ulong cFrameSubmitted;
        internal uint cDXPresentSubmitted;
        internal ulong cFrameConfirmed;
        internal uint cDXPresentConfirmed;
        internal ulong cRefreshConfirmed;
        internal uint cDXRefreshConfirmed;
        internal ulong cFramesLate;
        internal uint cFramesOutstanding;
        internal ulong cFrameDisplayed;
        internal ulong qpcFrameDisplayed;
        internal ulong cRefreshFrameDisplayed;
        internal ulong cFrameComplete;
        internal ulong qpcFrameComplete;
        internal ulong cFramePending;
        internal ulong qpcFramePending;
        internal ulong cFramesDisplayed;
        internal ulong cFramesComplete;
        internal ulong cFramesPending;
        internal ulong cFramesAvailable;
        internal ulong cFramesDropped;
        internal ulong cFramesMissed;
        internal ulong cRefreshNextDisplayed;
        internal ulong cRefreshNextPresented;
        internal ulong cRefreshesDisplayed;
        internal ulong cRefreshesPresented;
        internal ulong cRefreshStarted;
        internal ulong cPixelsReceived;
        internal ulong cPixelsDrawn;
        internal ulong cBuffersEmpty;
    }

    private static class NativeMethods
    {
        [DllImport("dwmapi.dll")]
        internal static extern int DwmGetCompositionTimingInfo(
            nint hwnd,
            ref DwmTimingInfo timingInfo);
    }
}

/// <summary>
/// Deterministic source used by the F6 fixture to exercise 60/120/144/165Hz
/// scheduling rules without claiming a synthetic cadence is a visible-vsync
/// observation.
/// </summary>
internal sealed class FlutterWindowsDeterministicVsyncSource : IFlutterWindowsVsyncSource
{
    private TimeSpan _interval;
    private TimeSpan _timestamp;
    private long _sequence;
    private bool _disposed;

    internal FlutterWindowsDeterministicVsyncSource(
        double refreshRateHz,
        TimeSpan? initialTimestamp = null)
    {
        _timestamp = initialTimestamp ?? TimeSpan.Zero;
        SetRefreshRate(refreshRateHz);
    }

    internal double RefreshRateHz => 1.0 / _interval.TotalSeconds;

    internal void SetRefreshRate(double refreshRateHz)
    {
        if (!double.IsFinite(refreshRateHz) || refreshRateHz <= 0 || refreshRateHz > 1_000)
            throw new ArgumentOutOfRangeException(nameof(refreshRateHz));
        _interval = TimeSpan.FromSeconds(1.0 / refreshRateHz);
    }

    public FlutterWindowsVsyncSample SampleNext(ulong viewId)
    {
        if (viewId == 0) throw new ArgumentOutOfRangeException(nameof(viewId));
        ObjectDisposedException.ThrowIf(_disposed, this);
        _timestamp += _interval;
        return new FlutterWindowsVsyncSample(
            viewId,
            checked(++_sequence),
            _timestamp,
            _interval,
            RefreshRateHz,
            IsDwmCompositionTiming: false);
    }

    public void Dispose() => _disposed = true;
}
