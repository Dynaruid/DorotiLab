// Adapted from Avalonia.Base/Rendering/SleepLoopRenderTimer.cs at the pinned A0 revision.
// Doroti owns the lifecycle and exposes no Avalonia type from this internal source-port boundary.
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Doroti.Vendor.Avalonia.Base;

internal interface ISourcePortRenderTimer : IDisposable
{
    Action<TimeSpan>? Tick { get; set; }

    bool RunsInBackground { get; }
}

internal sealed class SleepLoopRenderTimer : ISourcePortRenderTimer
{
    private readonly AutoResetEvent _wakeEvent = new(false);
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private readonly Thread _thread;
    private volatile Action<TimeSpan>? _tick;
    private volatile bool _stopped = true;
    private volatile bool _disposed;
    private int _desiredFps;

    internal SleepLoopRenderTimer(int framesPerSecond)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(framesPerSecond, 1);
        _desiredFps = framesPerSecond;
        _thread = new(Loop) { IsBackground = true, Name = "Doroti source-port render timer" };
        _thread.Start();
    }

    public Action<TimeSpan>? Tick
    {
        get => _tick;
        set
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _tick = value;
            _stopped = value is null;
            _wakeEvent.Set();
        }
    }

    public bool RunsInBackground => true;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        _tick = null;
        _stopped = false;
        _wakeEvent.Set();
        _thread.Join();
        _wakeEvent.Dispose();
    }

    private void Loop()
    {
        var lastTick = _stopwatch.Elapsed;
        while (!_disposed)
        {
            if (_stopped)
            {
                _wakeEvent.WaitOne();
                lastTick = _stopwatch.Elapsed;
                continue;
            }

            if (OperatingSystem.IsWindows() && DwmFlush() >= 0)
            {
                if (!_disposed && !_stopped)
                {
                    lastTick = _stopwatch.Elapsed;
                    _tick?.Invoke(lastTick);
                }
                continue;
            }

            var interval = TimeSpan.FromSeconds(1d / Volatile.Read(ref _desiredFps));
            var remaining = lastTick + interval - _stopwatch.Elapsed;
            if (remaining.TotalMilliseconds > 2)
            {
                Thread.Sleep(Math.Max(1, (int)remaining.TotalMilliseconds - 1));
            }
            while (!_disposed && !_stopped && _stopwatch.Elapsed < lastTick + interval)
            {
                Thread.SpinWait(64);
            }
            if (_disposed || _stopped)
            {
                continue;
            }
            lastTick = _stopwatch.Elapsed;
            _tick?.Invoke(lastTick);
        }
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmFlush();
}
