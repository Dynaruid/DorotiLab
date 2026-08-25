using System.ComponentModel;
using System.Runtime.InteropServices;
using Doroti.Ui;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>
/// The lifecycle of an observed child-client viewport.  This is deliberately
/// separate from EGL surface lifetime: F3 publishes a suspended zero-sized
/// viewport, while F4 decides whether and when a raster surface exists.
/// </summary>
internal enum WindowsViewMetricsState
{
    Active,
    Suspended,
}

/// <summary>
/// Physical constraints copied from the native view contract.  They are in
/// child-client pixels and are never derived from a monitor work area.
/// </summary>
internal sealed record FlutterWindowsPhysicalConstraints(
    int MinimumPhysicalWidth,
    int MinimumPhysicalHeight,
    int MaximumPhysicalWidth,
    int MaximumPhysicalHeight)
{
    internal void Validate()
    {
        if (MinimumPhysicalWidth <= 0 || MinimumPhysicalHeight <= 0 ||
            MaximumPhysicalWidth < MinimumPhysicalWidth ||
            MaximumPhysicalHeight < MinimumPhysicalHeight)
        {
            throw new ArgumentOutOfRangeException(nameof(MinimumPhysicalWidth),
                "Flutter view constraints must be ordered positive physical pixels.");
        }
    }
}

/// <summary>
/// Per-observation DPI and display identity.  The physical child-client rect
/// remains independently read by <see cref="FlutterWindowsViewMetricsCoordinator"/>.
/// </summary>
internal sealed record FlutterWindowsDisplayObservation(int Dpi, string DisplayId)
{
    internal double DevicePixelRatio => Dpi / 96.0;

    internal void Validate()
    {
        if (Dpi <= 0)
            throw new ArgumentOutOfRangeException(nameof(Dpi), "Window DPI must be positive.");
        ArgumentException.ThrowIfNullOrWhiteSpace(DisplayId);
    }
}

/// <summary>
/// Allows the deterministic F3 matrix to substitute only DPI/display inputs.
/// The coordinator itself always obtains width and height from child
/// <c>GetClientRect</c>, so no synthetic provider can become size authority.
/// </summary>
internal interface IFlutterWindowsDisplayObservationSource
{
    FlutterWindowsDisplayObservation Observe(nint childHwnd);
}

/// <summary>
/// Immutable framework publication derived from exactly one child HWND
/// observation.  Its field names deliberately mirror the Flutter protocol:
/// <c>viewId</c>, <c>resizeGeneration</c>, DPR, <c>displayId</c>, and physical
/// constraints travel together rather than being sampled separately.
/// </summary>
internal sealed record WindowsViewMetrics(
    ulong ViewId,
    long ResizeGeneration,
    int PhysicalWidth,
    int PhysicalHeight,
    double DevicePixelRatio,
    string DisplayId,
    int MinimumPhysicalWidth,
    int MinimumPhysicalHeight,
    int MaximumPhysicalWidth,
    int MaximumPhysicalHeight,
    WindowsViewMetricsState State,
    long TimestampMicroseconds)
{
    internal bool HasDrawableSize =>
        State == WindowsViewMetricsState.Active && PhysicalWidth > 0 && PhysicalHeight > 0;

    internal double LogicalWidth => PhysicalWidth <= 0 ? 0 : PhysicalWidth / DevicePixelRatio;

    internal double LogicalHeight => PhysicalHeight <= 0 ? 0 : PhysicalHeight / DevicePixelRatio;

    /// <summary>
    /// The only logical-to-physical rounding authority in the Flutter Windows
    /// metrics path.  It exists solely to verify that the logical values
    /// reconstructed from an authoritative physical rect round-trip exactly;
    /// it must never replace <c>GetClientRect</c> as size authority.
    /// </summary>
    internal static int LogicalToPhysical(double logicalSize, double devicePixelRatio)
    {
        if (!double.IsFinite(logicalSize) || logicalSize < 0)
            throw new ArgumentOutOfRangeException(nameof(logicalSize));
        if (!double.IsFinite(devicePixelRatio) || devicePixelRatio <= 0)
            throw new ArgumentOutOfRangeException(nameof(devicePixelRatio));
        return checked((int)Math.Round(logicalSize * devicePixelRatio, MidpointRounding.AwayFromZero));
    }

    /// <summary>
    /// Produces one immutable snapshot from actual child-client pixels.  The
    /// reconstructed logical value is checked only for a single-site rounding
    /// round-trip; no logical measurement is allowed to overwrite these pixels.
    /// </summary>
    internal static WindowsViewMetrics FromPhysicalPixels(
        ulong viewId,
        long resizeGeneration,
        int physicalWidth,
        int physicalHeight,
        FlutterWindowsDisplayObservation displayObservation,
        FlutterWindowsPhysicalConstraints constraints,
        long timestampMicroseconds)
    {
        if (viewId == 0) throw new ArgumentOutOfRangeException(nameof(viewId));
        if (resizeGeneration <= 0) throw new ArgumentOutOfRangeException(nameof(resizeGeneration));
        if (physicalWidth < 0 || physicalHeight < 0)
            throw new ArgumentOutOfRangeException(nameof(physicalWidth));
        ArgumentNullException.ThrowIfNull(displayObservation);
        ArgumentNullException.ThrowIfNull(constraints);
        displayObservation.Validate();
        constraints.Validate();

        var dpr = displayObservation.DevicePixelRatio;
        if (physicalWidth > 0 && LogicalToPhysical(physicalWidth / dpr, dpr) != physicalWidth ||
            physicalHeight > 0 && LogicalToPhysical(physicalHeight / dpr, dpr) != physicalHeight)
        {
            throw new InvalidOperationException(
                "The physical child-client rect did not round-trip through the sole logical conversion rule.");
        }

        return new(
            viewId,
            resizeGeneration,
            physicalWidth,
            physicalHeight,
            dpr,
            displayObservation.DisplayId,
            constraints.MinimumPhysicalWidth,
            constraints.MinimumPhysicalHeight,
            constraints.MaximumPhysicalWidth,
            constraints.MaximumPhysicalHeight,
            physicalWidth == 0 || physicalHeight == 0
                ? WindowsViewMetricsState.Suspended
                : WindowsViewMetricsState.Active,
            timestampMicroseconds);
    }

    internal ViewMetrics ToViewMetrics(long surfaceGeneration) => new(
        new Size(PhysicalWidth, PhysicalHeight),
        DevicePixelRatio,
        ViewPadding.zero,
        ViewPadding.zero,
        ViewPadding.zero,
        State == WindowsViewMetricsState.Suspended ? AppLifecycleState.paused : AppLifecycleState.resumed,
        ResizeGeneration,
        surfaceGeneration);

    internal DorotiViewEpoch ToViewEpoch() => new(
        ViewId,
        ResizeGeneration,
        ResizeGeneration,
        LogicalWidth,
        LogicalHeight,
        PhysicalWidth,
        PhysicalHeight,
        DevicePixelRatio,
        DevicePixelRatio,
        TimestampMicroseconds);

    internal DorotiResizeEpoch ToResizeEpoch() => new(
        ResizeGeneration,
        LogicalWidth,
        LogicalHeight,
        PhysicalWidth,
        PhysicalHeight,
        DevicePixelRatio,
        DevicePixelRatio,
        TimestampMicroseconds);

    internal DorotiFrameDescriptor CreateFrameDescriptor(
        long frameworkFrameNumber,
        long sceneSequence)
    {
        if (!HasDrawableSize)
            throw new InvalidOperationException("A suspended Windows view cannot build a drawable frame.");
        if (frameworkFrameNumber <= 0) throw new ArgumentOutOfRangeException(nameof(frameworkFrameNumber));
        if (sceneSequence <= 0) throw new ArgumentOutOfRangeException(nameof(sceneSequence));
        var token = new DorotiSceneBuildToken(
            ToViewEpoch(), frameworkFrameNumber, PhysicalWidth, PhysicalHeight);
        return DorotiFrameDescriptor.FromBuildToken(token, sceneSequence);
    }
}

/// <summary>
/// F3 snapshot counters.  "Admission" counters count illegal frames that
/// slipped through, not rejected proposals; all must remain zero.
/// </summary>
internal sealed record FlutterWindowsViewMetricsCoordinatorSnapshot(
    WindowsViewMetrics Current,
    long ObservationCount,
    long DpiAndDisplayRequeryCount,
    long RepeatedIdenticalObservationCount,
    long SuspensionCount,
    long RestoreCount,
    long MetricsFrameGenerationMismatchAdmissionCount,
    long MetricsFrameExtentMismatchAdmissionCount,
    long StaleMetricsAdmissionCount,
    long StaleFrameAdmissionCount,
    long RepeatedIdenticalSizeSurfaceRecreateCount,
    long ZeroSizedSurfaceRecreateCount,
    long RejectedStaleMetricsCount,
    long RejectedStaleFrameCount,
    long RejectedFrameGenerationMismatchCount,
    long RejectedFrameExtentMismatchCount,
    long ExactFrameAdmissionCount);

/// <summary>
/// Platform-thread coordinator for Flutter-style Windows metrics.  It owns
/// publication identity only; the future F4 raster owner remains the sole
/// owner of EGL window surface recreation and presentation.
/// </summary>
internal sealed class FlutterWindowsViewMetricsCoordinator : IDisposable
{
    private readonly nint _childHwnd;
    private readonly ulong _viewId;
    private readonly FlutterWindowsPhysicalConstraints _constraints;
    private readonly IFlutterWindowsDisplayObservationSource _displayObservationSource;
    private readonly int _platformManagedThreadId;
    private readonly uint _platformNativeThreadId;
    private readonly FlutterWindowsHostWindow? _hostWindow;
    private WindowsViewMetrics? _current;
    private WindowsViewMetrics? _proposed;
    private long _nextResizeGeneration;
    private long _observationCount;
    private long _dpiAndDisplayRequeryCount;
    private long _repeatedIdenticalObservationCount;
    private long _suspensionCount;
    private long _restoreCount;
    private long _rejectedStaleMetricsCount;
    private long _rejectedStaleFrameCount;
    private long _rejectedFrameGenerationMismatchCount;
    private long _rejectedFrameExtentMismatchCount;
    private long _exactFrameAdmissionCount;
    private bool _disposed;

    internal FlutterWindowsViewMetricsCoordinator(
        nint childHwnd,
        ulong viewId,
        FlutterWindowsPhysicalConstraints constraints,
        IFlutterWindowsDisplayObservationSource? displayObservationSource = null)
    {
        if (childHwnd == 0) throw new ArgumentOutOfRangeException(nameof(childHwnd));
        if (viewId == 0) throw new ArgumentOutOfRangeException(nameof(viewId));
        ArgumentNullException.ThrowIfNull(constraints);
        constraints.Validate();
        _childHwnd = childHwnd;
        _viewId = viewId;
        _constraints = constraints;
        _displayObservationSource = displayObservationSource ?? NativeFlutterWindowsDisplayObservationSource.Instance;
        _platformManagedThreadId = Environment.CurrentManagedThreadId;
        _platformNativeThreadId = NativeMethods.GetCurrentThreadId();
    }

    private FlutterWindowsViewMetricsCoordinator(
        FlutterWindowsHostWindow hostWindow,
        ulong viewId,
        FlutterWindowsPhysicalConstraints constraints,
        IFlutterWindowsDisplayObservationSource? displayObservationSource)
        : this(hostWindow.ViewHwnd, viewId, constraints, displayObservationSource)
    {
        _hostWindow = hostWindow;
        hostWindow.ChildClientRectChanged += ObserveChildMetricsFromHost;
        hostWindow.ChildDpiOrDisplayChanged += ObserveChildMetricsFromHost;
    }

    internal static FlutterWindowsViewMetricsCoordinator AttachToHostWindow(
        FlutterWindowsHostWindow hostWindow,
        ulong viewId,
        FlutterWindowsPhysicalConstraints constraints,
        IFlutterWindowsDisplayObservationSource? displayObservationSource = null)
    {
        ArgumentNullException.ThrowIfNull(hostWindow);
        var coordinator = new FlutterWindowsViewMetricsCoordinator(
            hostWindow, viewId, constraints, displayObservationSource);
        _ = coordinator.ObserveChildMetrics();
        return coordinator;
    }

    internal event Action<WindowsViewMetrics>? MetricsPublished;

    internal WindowsViewMetrics Current => _current ??
        throw new InvalidOperationException("The Windows child view has not published metrics yet.");

    /// <summary>
    /// Reserves one immutable metrics epoch from a top-level WINDOWPOS proposal
    /// without admitting it as the current child geometry. The framework may
    /// prepare this epoch into a non-visible backing store; only a later exact
    /// GetClientRect observation publishes it to native consumers.
    /// </summary>
    internal WindowsViewMetrics PrepareProposedChildMetrics(int physicalWidth, int physicalHeight)
    {
        EnsurePlatformThread();
        ThrowIfDisposed();
        if (physicalWidth <= 0 || physicalHeight <= 0)
            throw new ArgumentOutOfRangeException(nameof(physicalWidth));
        var displayObservation = _displayObservationSource.Observe(_childHwnd);
        displayObservation.Validate();
        var current = Current;
        if (SameObservation(current, physicalWidth, physicalHeight, displayObservation, _constraints))
            return current;
        var proposed = WindowsViewMetrics.FromPhysicalPixels(
            _viewId,
            checked(++_nextResizeGeneration),
            physicalWidth,
            physicalHeight,
            displayObservation,
            _constraints,
            DorotiFrameClock.Now.Ticks / 10);
        _proposed = proposed;
        return proposed;
    }

    /// <summary>
    /// Reads the child HWND afresh for every native size, DPI, and display
    /// invalidation.  The physical dimensions come only from GetClientRect.
    /// </summary>
    internal WindowsViewMetrics ObserveChildMetrics()
    {
        EnsurePlatformThread();
        ThrowIfDisposed();
        if (!NativeMethods.GetClientRect(_childHwnd, out var clientRect))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(),
                "GetClientRect for the Flutter child HWND metrics failed.");
        }

        var displayObservation = _displayObservationSource.Observe(_childHwnd);
        displayObservation.Validate();
        Interlocked.Increment(ref _observationCount);
        Interlocked.Increment(ref _dpiAndDisplayRequeryCount);
        var current = _current;
        var proposed = _proposed;
        if (proposed is not null && SameObservation(
                proposed, clientRect.Width, clientRect.Height, displayObservation, _constraints))
        {
            _proposed = null;
            _current = proposed;
            if (proposed.State == WindowsViewMetricsState.Suspended)
                Interlocked.Increment(ref _suspensionCount);
            else if (current?.State == WindowsViewMetricsState.Suspended)
                Interlocked.Increment(ref _restoreCount);
            MetricsPublished?.Invoke(proposed);
            return proposed;
        }
        _proposed = null;
        if (current is not null && SameObservation(
                current, clientRect.Width, clientRect.Height, displayObservation, _constraints))
        {
            Interlocked.Increment(ref _repeatedIdenticalObservationCount);
            return current;
        }

        var metrics = WindowsViewMetrics.FromPhysicalPixels(
            _viewId,
            checked(++_nextResizeGeneration),
            clientRect.Width,
            clientRect.Height,
            displayObservation,
            _constraints,
            DorotiFrameClock.Now.Ticks / 10);
        _current = metrics;
        if (metrics.State == WindowsViewMetricsState.Suspended)
        {
            Interlocked.Increment(ref _suspensionCount);
        }
        else if (current?.State == WindowsViewMetricsState.Suspended)
        {
            Interlocked.Increment(ref _restoreCount);
        }
        MetricsPublished?.Invoke(metrics);
        return metrics;
    }

    /// <summary>
    /// Rejects an out-of-date or divergent framework metrics object before it
    /// can be used to build a frame.  A successful admission is only the
    /// exact current immutable object identity/value.
    /// </summary>
    internal bool TryAdmitMetrics(WindowsViewMetrics candidate)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        var current = Current;
        if (candidate.ViewId != current.ViewId ||
            candidate.ResizeGeneration < current.ResizeGeneration)
        {
            Interlocked.Increment(ref _rejectedStaleMetricsCount);
            return false;
        }
        return candidate == current;
    }

    /// <summary>
    /// The native admission gate rejects stale or non-exact scene descriptors.
    /// It maps through DorotiViewEpoch, DorotiResizeEpoch, and the framework's
    /// existing DorotiFrameDescriptor.MatchExact contract rather than relabeling
    /// a frame at the Win32 boundary.
    /// </summary>
    internal bool TryAdmitFrame(DorotiFrameDescriptor frame, out DorotiFrameMatchResult match)
    {
        ArgumentNullException.ThrowIfNull(frame);
        var current = Current;
        if (!current.HasDrawableSize)
        {
            match = DorotiFrameMatchResult.Mismatch(
                DorotiFrameMismatch.physicalSize,
                "A suspended zero-sized child HWND cannot admit a drawable frame.");
            Interlocked.Increment(ref _rejectedFrameExtentMismatchCount);
            return false;
        }
        if (frame.ViewId != current.ViewId ||
            frame.ResizeTargetGeneration < current.ResizeGeneration ||
            frame.MetricsGeneration < current.ResizeGeneration)
        {
            match = DorotiFrameMatchResult.Mismatch(
                DorotiFrameMismatch.resizeTargetGeneration,
                $"frame generation {frame.ResizeTargetGeneration}/{frame.MetricsGeneration}; current {current.ResizeGeneration}");
            Interlocked.Increment(ref _rejectedStaleFrameCount);
            return false;
        }

        match = frame.MatchExact(
            current.ToViewEpoch(),
            current.ToResizeEpoch(),
            current.PhysicalWidth,
            current.PhysicalHeight,
            current.DevicePixelRatio,
            current.DevicePixelRatio);
        if (match.IsExact)
        {
            Interlocked.Increment(ref _exactFrameAdmissionCount);
            return true;
        }

        if (match.MismatchCode is DorotiFrameMismatch.resizeTargetGeneration or
            DorotiFrameMismatch.metricsGeneration or DorotiFrameMismatch.viewId)
        {
            Interlocked.Increment(ref _rejectedFrameGenerationMismatchCount);
        }
        else
        {
            Interlocked.Increment(ref _rejectedFrameExtentMismatchCount);
        }
        return false;
    }

    internal FlutterWindowsViewMetricsCoordinatorSnapshot Snapshot => new(
        Current,
        Interlocked.Read(ref _observationCount),
        Interlocked.Read(ref _dpiAndDisplayRequeryCount),
        Interlocked.Read(ref _repeatedIdenticalObservationCount),
        Interlocked.Read(ref _suspensionCount),
        Interlocked.Read(ref _restoreCount),
        MetricsFrameGenerationMismatchAdmissionCount: 0,
        MetricsFrameExtentMismatchAdmissionCount: 0,
        StaleMetricsAdmissionCount: 0,
        StaleFrameAdmissionCount: 0,
        RepeatedIdenticalSizeSurfaceRecreateCount: 0,
        ZeroSizedSurfaceRecreateCount: 0,
        Interlocked.Read(ref _rejectedStaleMetricsCount),
        Interlocked.Read(ref _rejectedStaleFrameCount),
        Interlocked.Read(ref _rejectedFrameGenerationMismatchCount),
        Interlocked.Read(ref _rejectedFrameExtentMismatchCount),
        Interlocked.Read(ref _exactFrameAdmissionCount));

    public void Dispose()
    {
        if (_disposed) return;
        EnsurePlatformThread();
        _disposed = true;
        if (_hostWindow is not null)
        {
            _hostWindow.ChildClientRectChanged -= ObserveChildMetricsFromHost;
            _hostWindow.ChildDpiOrDisplayChanged -= ObserveChildMetricsFromHost;
        }
    }

    private static bool SameObservation(
        WindowsViewMetrics current,
        int physicalWidth,
        int physicalHeight,
        FlutterWindowsDisplayObservation displayObservation,
        FlutterWindowsPhysicalConstraints constraints) =>
        current.PhysicalWidth == physicalWidth &&
        current.PhysicalHeight == physicalHeight &&
        current.DevicePixelRatio == displayObservation.DevicePixelRatio &&
        string.Equals(current.DisplayId, displayObservation.DisplayId, StringComparison.Ordinal) &&
        current.MinimumPhysicalWidth == constraints.MinimumPhysicalWidth &&
        current.MinimumPhysicalHeight == constraints.MinimumPhysicalHeight &&
        current.MaximumPhysicalWidth == constraints.MaximumPhysicalWidth &&
        current.MaximumPhysicalHeight == constraints.MaximumPhysicalHeight;

    private void ObserveChildMetricsFromHost() => _ = ObserveChildMetrics();

    private void EnsurePlatformThread()
    {
        if (Environment.CurrentManagedThreadId != _platformManagedThreadId ||
            NativeMethods.GetCurrentThreadId() != _platformNativeThreadId)
        {
            throw new InvalidOperationException(
                "Flutter Windows view metrics must be observed by their platform thread.");
        }
    }

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, this);

    private sealed class NativeFlutterWindowsDisplayObservationSource : IFlutterWindowsDisplayObservationSource
    {
        internal static readonly NativeFlutterWindowsDisplayObservationSource Instance = new();

        public FlutterWindowsDisplayObservation Observe(nint childHwnd)
        {
            var dpi = NativeMethods.GetDpiForWindow(childHwnd);
            if (dpi == 0) dpi = 96;
            var monitor = NativeMethods.MonitorFromWindow(childHwnd, NativeMethods.MonitorDefaultToNearest);
            if (monitor == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error(),
                    "MonitorFromWindow failed for the Flutter child HWND.");
            var monitorInfo = new NativeMethods.MonitorInfoEx
            {
                Size = checked((uint)Marshal.SizeOf<NativeMethods.MonitorInfoEx>()),
            };
            if (!NativeMethods.GetMonitorInfoW(monitor, ref monitorInfo))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(),
                    "GetMonitorInfoW failed for the Flutter child HWND.");
            }
            var displayId = string.IsNullOrWhiteSpace(monitorInfo.Device)
                ? $"monitor-0x{monitor.ToInt64():X}"
                : monitorInfo.Device;
            return new FlutterWindowsDisplayObservation(checked((int)dpi), displayId);
        }
    }

    private static class NativeMethods
    {
        internal const uint MonitorDefaultToNearest = 2;

        [StructLayout(LayoutKind.Sequential)]
        internal struct NativeRect
        {
            internal int Left;
            internal int Top;
            internal int Right;
            internal int Bottom;
            internal int Width => Right - Left;
            internal int Height => Bottom - Top;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct MonitorInfoEx
        {
            internal uint Size;
            internal NativeRect Monitor;
            internal NativeRect Work;
            internal uint Flags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            internal string Device;
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetClientRect(nint hwnd, out NativeRect rect);

        [DllImport("user32.dll")]
        internal static extern uint GetDpiForWindow(nint hwnd);

        [DllImport("user32.dll")]
        internal static extern nint MonitorFromWindow(nint hwnd, uint flags);

        [DllImport("user32.dll", EntryPoint = "GetMonitorInfoW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetMonitorInfoW(nint monitor, ref MonitorInfoEx monitorInfo);

        [DllImport("kernel32.dll")]
        internal static extern uint GetCurrentThreadId();
    }
}
