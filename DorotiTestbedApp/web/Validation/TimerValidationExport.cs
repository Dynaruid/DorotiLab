using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Text.Json;
using Doroti.Host.Web;
using Doroti.Runtime;
using DartTimer = Doroti.Runtime.Timer;

namespace DorotiTestbedApp.Web.Validation;

[SupportedOSPlatform("browser")]
public static partial class TimerValidationExport
{
    [JSExport]
    public static async Task<string> Run(bool systemTimerControl, int milliseconds)
    {
        if (milliseconds is < 100 or > 120000) throw new ArgumentOutOfRangeException(nameof(milliseconds));
        var owner = Environment.CurrentManagedThreadId;
        var context = SynchronizationContext.Current ?? throw new InvalidOperationException("No JS owner context.");
        using var browserTime = new BrowserTimeProvider();
        var clock = systemTimerControl ? TimeProvider.System : browserTime;
        using var timeScope = DartAsyncRuntime.enterTimeProvider(clock);
        using var microtaskScope = DartAsyncRuntime.enterMicrotaskScheduler(action => context.Post(_ => action(), null));
        var callbacks = 0;
        var wrongThread = 0;
        long peakSystemTimers = 0;
        var timers = new List<DartTimer>();
        void Observe()
        {
            callbacks++;
            if (Environment.CurrentManagedThreadId != owner) wrongThread++;
            peakSystemTimers = Math.Max(peakSystemTimers, System.Threading.Timer.ActiveCount);
        }
        try
        {
            for (var i = 0; i < 24; i++)
                timers.Add(DartTimer.periodic(Duration.Create(milliseconds: 1 + i % 7), _ => Observe()));
            var canceledCalls = 0;
            var canceled = new DartTimer(Duration.zero, () => canceledCalls++);
            canceled.cancel();
            var zeroCalls = 0;
            DartTimer.run(() => { zeroCalls++; Observe(); });
            var delayed = await new Future<int>(Duration.Create(milliseconds: 2)).then<int>(_ => { Observe(); return 42; });
            var completer = new Completer<int>();
            var timeoutResult = await completer.future.timeout(Duration.Create(milliseconds: 5), (Func<object>)(() => { Observe(); return 99; }));
            completer.complete(100);
            var timedOut = false;
            using (var cancellation = new CancellationTokenSource(TimeSpan.FromMilliseconds(5), clock))
            {
                try { await Task.Delay(TimeSpan.FromSeconds(10), clock, cancellation.Token); }
                catch (OperationCanceledException) { timedOut = true; }
            }
            // Change/dispose may arrive from a .NET continuation. The provider
            // must marshal JS calls and discard obsolete arm requests.
            await Task.Run(() => {
                using var timer = clock.CreateTimer(_ => throw new InvalidOperationException("Canceled timer fired"), null,
                    TimeSpan.FromMinutes(1), Timeout.InfiniteTimeSpan);
                timer.Change(TimeSpan.FromMinutes(2), Timeout.InfiniteTimeSpan);
            });
            await Task.Delay(TimeSpan.FromMilliseconds(milliseconds), clock);
            return JsonSerializer.Serialize(new {
                systemTimerControl, callbacks, wrongThread, canceledCalls, zeroCalls, delayed, timeoutResult, timedOut,
                peakSystemTimers, owner,
                diagnostics = JsonSerializer.Deserialize<JsonElement>(BrowserTimeProvider.CaptureDiagnostics()),
            });
        }
        finally { foreach (var timer in timers) timer.cancel(); }
    }
}
