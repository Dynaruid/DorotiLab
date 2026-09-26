using Doroti.Runtime;
using Doroti.Framework.Scheduler;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

/// <summary>Explicit repaint and animation source. Construction and Stop do not
/// request continuous frames. Dispose this controller with its owning widget state.</summary>
public sealed class GpuEffectController : ChangeNotifier
{
    private readonly Ticker? _ticker;
    private readonly Func<Duration, GpuEffectParameters>? _update;
    private bool _disposed;
    private double _previousTime;
    public GpuEffectParameters Parameters { get; private set; }
    public bool IsAnimating => _ticker?.isActive == true;

    public GpuEffectController(GpuEffectParameters? initial = null, TickerProvider? vsync = null,
        Func<Duration, GpuEffectParameters>? onTick = null)
    {
        if (onTick is not null && vsync is null) throw new ArgumentException("Animated effects require a ticker provider.");
        Parameters = initial ?? GpuEffectParameters.Empty;
        _update = onTick;
        _ticker = vsync?.createTicker(Tick);
    }

    public void Update(GpuEffectParameters parameters)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        notifyListeners();
    }

    public void Start()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_ticker is null) throw new InvalidOperationException("This controller has no ticker provider.");
        if (_ticker.isActive) return;
        _previousTime = 0;
        _ticker.start();
    }

    public void Stop()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _ticker?.stop();
    }

    private void Tick(Duration elapsed)
    {
        var time = elapsed.inMicroseconds / 1_000_000d;
        var parameters = _update?.Invoke(elapsed) ?? Parameters;
        Update(parameters.WithFrameTiming((float)time, (float)Math.Max(0, time - _previousTime)));
        _previousTime = time;
    }

    public override void dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _ticker?.dispose();
        base.dispose();
    }
}
