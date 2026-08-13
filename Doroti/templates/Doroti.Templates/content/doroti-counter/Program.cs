using System.Diagnostics;
using Doroti;
using Doroti.Composition;
using Doroti.Core;
using Doroti.Engine;
using Doroti.Graphics;
using Doroti.Host.Desktop;
using Doroti.Platform;
using Doroti.Widgets;

internal static class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        var smoke = args.Contains("--smoke", StringComparer.Ordinal);
        using var backend = new DesktopWindowBackend();
        var events = new CounterWindowEvents();
        using var window = backend.CreateWindow(new("Doroti Counter", new(640, 360)), events);
        using var frameSink = new DesktopGpuFrameSink(window);
        var counterKey = new GlobalKey<CounterState>("counter");
        using var application = DorotiApp.CreateInteractive(window, new Counter(counterKey), frameSink);
        if (!window.TryGetFeature<IFrameDispatcher>(out var frameDispatcher) || frameDispatcher is null)
        {
            throw new InvalidOperationException("The source-ported desktop shell did not expose its render timer.");
        }

        var scheduler = new FrameSchedulerPort(new MonotonicClock(), frameDispatcher);
        var pending = new List<Task<FrameAckResult>>();
        scheduler.BeginFrame += _ =>
        {
            if (!window.IsClosed && !window.Metrics.IsMinimized && application.NeedsFrame)
            {
                pending.Add(application.PumpFrameNonBlocking());
            }
        };
        application.FrameRequested += scheduler.ScheduleFrame;
        events.Bind(application, scheduler.ScheduleFrame);
        window.Show();
        scheduler.ScheduleFrame();

        var changed = false;
        var deadline = smoke ? Stopwatch.GetTimestamp() + (long)(Stopwatch.Frequency * 5d) : long.MaxValue;
        while (!window.IsClosed)
        {
            backend.PumpPendingMessages();
            pending.RemoveAll(task => task.IsCompleted);
            if (smoke && application.PresentedFrameCount > 0 && !changed)
            {
                counterKey.CurrentState?.Increment();
                changed = true;
            }
            if (smoke && changed && application.PresentedFrameCount > 1)
            {
                window.Close();
            }
            if (Stopwatch.GetTimestamp() > deadline)
            {
                throw new TimeoutException("The strict asynchronous GPU counter did not present two frames in five seconds.");
            }
            Thread.Sleep(1);
        }

        Console.WriteLine($"Doroti Counter: PASS ({frameSink.Diagnostic}; queueHighWatermark={frameSink.QueueHighWatermark})");
        return events.Closed == 1 && !frameSink.SoftwareFallbackUsed && frameSink.QueueHighWatermark <= 2 ? 0 : 1;
    }
}

internal sealed class Counter(Key? key = null) : StatefulWidget(key)
{
    public override State CreateState() => new CounterState();
}

internal sealed class CounterState : State<Counter>
{
    private int _count;

    public override Widget Build(BuildContext context) => new ColoredBox(
        Color.FromArgb(255, 21, 35, 61),
        new Center(new Column([
            new Text("DOROTI COUNTER"),
            new Text(_count.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            new GestureDetector(Increment, new Padding(new(18), new Text("+1"))),
        ])));

    internal void Increment() => SetState(() => _count++);
}

internal sealed class CounterWindowEvents : IWindowEventSink
{
    private InteractiveApplication? _application;
    private Action? _requestFrame;

    internal int Closed { get; private set; }

    internal void Bind(InteractiveApplication application, Action requestFrame)
    {
        _application = application;
        _requestFrame = requestFrame;
    }

    public void OnMetricsChanged(WindowId window, WindowMetrics metrics)
    {
        _application?.OnMetricsChanged();
        _requestFrame?.Invoke();
    }

    public void OnCloseRequested(WindowId window) { }

    public void OnClosed(WindowId window) => Closed++;
}

internal sealed class MonotonicClock : IClock
{
    private readonly long _origin = Stopwatch.GetTimestamp();

    public TimeSpan Now => Stopwatch.GetElapsedTime(_origin);
}
