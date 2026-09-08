namespace Doroti.Host.Maui;

// UIKit's pan recognizer ends when the fingers lift. Continue its point-space
// velocity on the display link; native AppKit wheel events already carry inertia.
internal sealed class MauiScrollMomentum
{
    private const double Decay = 6;
    private const double MinimumSpeed = 20;
    private double _velocityX;
    private double _velocityY;
    private TimeSpan _lastTime;
    internal bool IsActive { get; private set; }

    internal bool Start(double velocityX, double velocityY, TimeSpan timestamp)
    {
        Stop();
        if (!double.IsFinite(velocityX) || !double.IsFinite(velocityY)) return false;
        var speed = Math.Sqrt(velocityX * velocityX + velocityY * velocityY);
        if (speed < MinimumSpeed) return false;
        var limit = Math.Min(1, 8000 / speed);
        _velocityX = velocityX * limit;
        _velocityY = velocityY * limit;
        _lastTime = timestamp;
        return IsActive = true;
    }

    internal (double X, double Y) Advance(TimeSpan timestamp)
    {
        if (!IsActive) return default;
        var seconds = (timestamp - _lastTime).TotalSeconds;
        if (seconds <= 0) return default;
        _lastTime = timestamp;
        // A suspended window must not jump when its display link resumes.
        if (seconds > .1) { Stop(); return default; }
        var decay = Math.Exp(-Decay * seconds);
        var distance = (1 - decay) / Decay;
        var delta = (_velocityX * distance, _velocityY * distance);
        _velocityX *= decay;
        _velocityY *= decay;
        if (_velocityX * _velocityX + _velocityY * _velocityY < MinimumSpeed * MinimumSpeed) Stop();
        return delta;
    }

    internal void Stop() => IsActive = false;
}
