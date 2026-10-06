using Doroti.Framework.Scheduler;
using Doroti.Framework.Widgets;
using Doroti.Runtime;
using Timer = Doroti.Runtime.Timer;

namespace Doroti.CustomCarousel;

// A burst is one scroll activity. Small precision deltas follow input directly;
// coarse mouse-wheel notches interpolate while preserving the accumulated target.
internal sealed class CarouselWheelScrollActivity : ScrollActivity
{
    private readonly Ticker _ticker;
    private Timer? _quietTimer;
    private long _lastTickTimestamp;
    private double _velocity;
    public double target { get; private set; }

    internal CarouselWheelScrollActivity(CustomCarouselScrollPosition position, TickerProvider vsync) : base(position)
    {
        _ticker = vsync.createTicker(tick);
        target = position.pixels;
    }

    internal void updateTarget(double to, bool smooth)
    {
        target = to;
        if (smooth)
        {
            if (!_ticker.isActive)
            {
                _lastTickTimestamp = DartAsyncRuntime.timeProvider.GetTimestamp();
                _ticker.start();
            }
        }
        else
        {
            _ticker.stop();
            _velocity = 0;
            @delegate.setPixels(to);
        }
        _quietTimer?.cancel();
        _quietTimer = new Timer(Duration.Create(milliseconds: 120), () =>
        {
            @delegate.setPixels(target);
            @delegate.goBallistic(0);
        });
    }

    private void tick(Duration elapsed)
    {
        // Retarget without restarting the ticker. Restarting a tween for each
        // queued packet leaves its elapsed time at zero under continuous input.
        var now = DartAsyncRuntime.timeProvider.GetTimestamp();
        var seconds = DartAsyncRuntime.timeProvider.GetElapsedTime(_lastTickTimestamp, now).TotalSeconds;
        _lastTickTimestamp = now;
        if (seconds <= 0) return;
        var from = ((ScrollPosition)@delegate).pixels;
        var next = from + (target - from) * (1 - Math.Exp(-seconds / 0.012));
        if (Math.Abs(target - next) <= 0.1) next = target;
        _velocity = (next - from) / seconds;
        @delegate.setPixels(next);
        if (next == target)
        {
            _ticker.stop();
            _velocity = 0;
        }
    }

    public override bool shouldIgnorePointer => false;
    public override bool isScrolling => true;
    public override double velocity => _velocity;

    public override void dispose()
    {
        _quietTimer?.cancel();
        _quietTimer = null;
        _ticker.dispose();
        base.dispose();
    }
}
