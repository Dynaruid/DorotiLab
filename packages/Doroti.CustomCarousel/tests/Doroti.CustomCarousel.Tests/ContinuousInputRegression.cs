using System.Text.Json;
using Doroti.CustomCarousel;
using Doroti.Framework.Gestures;
using Doroti.Framework.Painting;
using Doroti.Framework.Widgets;
using Doroti.Runtime;
using Doroti.Testing;
using Doroti.Ui;
using Carousel = Doroti.CustomCarousel.CustomCarousel;
using Path = System.IO.Path;
using PointerEvent = Doroti.Framework.Gestures.PointerEvent;
using PointerDownEvent = Doroti.Framework.Gestures.PointerDownEvent;
using PointerMoveEvent = Doroti.Framework.Gestures.PointerMoveEvent;
using PointerHoverEvent = Doroti.Framework.Gestures.PointerHoverEvent;
using PointerUpEvent = Doroti.Framework.Gestures.PointerUpEvent;

internal static class ContinuousInputRegression
{
    internal static void Run(string output)
    {
        Directory.CreateDirectory(output);
        var results = new List<object>();
        var failures = new List<string>();
        void Case(string name, Action<List<double>> body)
        {
            var samples = new List<double>();
            string? error = null;
            try { body(samples); }
            catch (Exception failure) { error = failure.Message; failures.Add($"{name}: {error}"); }
            results.Add(new { name, status = error is null ? "PASS" : "FAIL", samples, error });
            Console.WriteLine($"{(error is null ? "PASS" : "FAIL")}: {name}{(error is null ? "" : ": " + error)}");
        }
        foreach (var loop in new[] { false, true })
            Case($"wheel-burst-loop-{loop}", samples =>
            {
                using var session = new InputSession(loop);
                var tester = session.Tester;
                for (var packet = 0; packet < 12; packet++)
                {
                    session.Send(new PointerScrollEvent(viewId: session.ViewId, timeStamp: session.Time,
                        position: session.Origin, scrollDelta: new Offset(30, 0)));
                    // Render between wheel packets, as a real 60 Hz host does.
                    for (var frame = 0; frame < 3; frame++)
                    {
                        tester.pump(TimeSpan.FromMilliseconds(16));
                        samples.Add(session.Controller.position.pixels);
                    }
                }
                Check(samples.Zip(samples.Skip(1), (a, b) => b >= a - 0.01).All(v => v),
                    $"Positive wheel input reversed during the burst; final pixels={samples.Last():F2}.");
                Check(Math.Abs(samples.Last() - 360) < 0.1, "Wheel displacement was lost to snapping before input ended.");
                Check(session.Settled.Count(i => i is null) == 1 && session.Settled.Count(i => i is not null) == 0,
                    "Wheel packets repeatedly reported start/end instead of one input session.");
                session.Settle();
                Check(session.Controller.selectedItem == 1 && session.Settled.Last() == 1,
                    $"Wheel burst did not snap after silence: pixels={session.Controller.position.pixels:F2}, selected={session.Controller.selectedItem}, settled={string.Join(',', session.Settled)}.");
            });
        foreach (var trackpad in new[] { false, true })
            Case(trackpad ? "trackpad-pan-seam" : "mouse-drag-seam", samples =>
            {
                using var session = new InputSession(loop: true);
                var tester = session.Tester;
                const int pointer = 41;
                session.Send(trackpad
                    ? new PointerPanZoomStartEvent(viewId: session.ViewId, pointer: pointer, timeStamp: session.Time, position: session.Origin)
                    : new PointerDownEvent(viewId: session.ViewId, pointer: pointer, timeStamp: session.Time,
                        position: session.Origin, kind: PointerDeviceKind.mouse));
                for (var step = 1; step <= 8; step++)
                {
                    tester.pump(TimeSpan.FromMilliseconds(16));
                    session.Send(trackpad
                        ? new PointerPanZoomUpdateEvent(viewId: session.ViewId, pointer: pointer, timeStamp: session.Time,
                            position: session.Origin, pan: new Offset(30 * step, 0), panDelta: new Offset(30, 0))
                        : new PointerMoveEvent(viewId: session.ViewId, pointer: pointer, timeStamp: session.Time,
                            position: session.Origin + new Offset(30 * step, 0), delta: new Offset(30, 0), kind: PointerDeviceKind.mouse));
                    tester.pump();
                    samples.Add(session.Controller.position.pixels);
                }
                Check(samples.Distinct().Count() >= 6, "The view did not move while the gesture was active.");
                Check(session.ScrollDeltas.All(d => Math.Abs(d) <= 30.01),
                    $"Crossing the loop seam emitted a spurious delta: {string.Join(',', session.ScrollDeltas.Select(v => v.ToString("F1")))}.");
                session.Send(trackpad
                    ? new PointerPanZoomEndEvent(viewId: session.ViewId, pointer: pointer, timeStamp: session.Time, position: session.Origin)
                    : new PointerUpEvent(viewId: session.ViewId, pointer: pointer, timeStamp: session.Time,
                        position: session.Origin + new Offset(240, 0), kind: PointerDeviceKind.mouse));
                session.Settle();
                Check(session.Controller.selectedItem != 0, "Gesture release returned to the original item.");
            });
        foreach (var reverse in new[] { false, true })
            Case($"vertical-wheel-on-horizontal-reverse-{reverse}", samples =>
            {
                using var session = new InputSession(loop: true, reverse: reverse);
                session.Send(new PointerScrollEvent(viewId: session.ViewId, timeStamp: session.Time,
                    position: session.Origin, scrollDelta: new Offset(0, 120)));
                session.Tester.pump();
                Check(session.Controller.position.pixels == 0, "A coarse notch jumped before its animation frame.");
                session.Tester.pump(TimeSpan.FromMilliseconds(45));
                samples.Add(session.Controller.position.pixels);
                Check(Math.Abs(samples[0]) > 0 && Math.Abs(samples[0]) < 120,
                    "Vertical mouse wheel did not interpolate along the horizontal carousel.");
                for (var notch = 0; notch < 3; notch++)
                {
                    session.Send(new PointerScrollEvent(viewId: session.ViewId, timeStamp: session.Time,
                        position: session.Origin, scrollDelta: new Offset(0, 120)));
                    session.Tester.pump(TimeSpan.FromMilliseconds(16));
                }
                session.Settle();
                Check(session.Controller.selectedItem == (reverse ? 7 : 1), "Coarse notches lost their accumulated target.");
                Check(session.Settled.Count(i => i is null) == 1 && session.Settled.Count(i => i is not null) == 1,
                    "Coarse wheel burst emitted repeated settle callbacks.");
            });
        Case("trackpad-wheel-packet-to-carousel", samples =>
        {
            using var session = new InputSession(loop: true);
            var packets = new[]
            {
                (PointerDeviceKind.trackpad, 20.0),
                (PointerDeviceKind.mouse, 8.0),
                (PointerDeviceKind.trackpad, -12.0),
            };
            var expected = 0.0;
            foreach (var (kind, delta) in packets)
            {
                var datum = new PointerData((ulong)session.ViewId, session.Tester.Clock.Elapsed,
                    PointerChange.hover, kind, 1, session.Origin.dx, session.Origin.dy,
                    0, 0, 0, scrollDeltaY: delta, signalKind: PointerSignalKind.scroll);
                var input = PointerEventConverter.expand([datum], _ => 1).Single();
                Check(input is PointerScrollEvent scroll && scroll.kind == kind
                    && scroll.scrollDelta == new Offset(0, delta), "Conversion lost scroll source or delta.");
                session.Send(input);
                session.Tester.pump();
                expected += delta;
                samples.Add(session.Controller.position.pixels);
                Check(Math.Abs(samples[^1] - expected) < 0.001,
                    "A trackpad wheel packet did not reach the carousel immediately.");
            }
            // Cursor events on the same device still use their own mouse kind.
            var hover = new PointerData((ulong)session.ViewId, session.Tester.Clock.Elapsed,
                PointerChange.hover, PointerDeviceKind.mouse, 1,
                session.Origin.dx, session.Origin.dy, 0, 0, 0);
            Check(PointerEventConverter.expand([hover], _ => 1).Single() is PointerHoverEvent
                { kind: PointerDeviceKind.mouse }, "Trackpad scroll contaminated the next cursor event.");
            session.Settle();
        });
        Case("wheel-interrupted-by-button", samples =>
        {
            using var session = new InputSession(loop: true);
            session.Send(new PointerScrollEvent(viewId: session.ViewId, timeStamp: session.Time,
                position: session.Origin, scrollDelta: new Offset(120, 0)));
            session.Tester.pump(TimeSpan.FromMilliseconds(32));
            Task? operation = null;
            session.Tester.View.DispatchPlatformEvent(() => operation = session.Controller.animateToItem(3, Duration.Create(milliseconds: 240)));
            session.Tester.pumpUntilComplete(operation!);
            session.Settle();
            samples.Add(session.Controller.position.pixels);
            Check(session.Controller.selectedItem == 3, "A pending wheel timer interrupted button navigation.");
        });
        Case("wheel-interrupted-by-trackpad", samples =>
        {
            using var session = new InputSession(loop: true);
            session.Send(new PointerScrollEvent(viewId: session.ViewId, timeStamp: session.Time,
                position: session.Origin, scrollDelta: new Offset(120, 0)));
            session.Tester.pump(TimeSpan.FromMilliseconds(16));
            const int pointer = 42;
            session.Send(new PointerPanZoomStartEvent(viewId: session.ViewId, pointer: pointer, timeStamp: session.Time, position: session.Origin));
            for (var step = 1; step <= 12; step++)
            {
                session.Tester.pump(TimeSpan.FromMilliseconds(16));
                session.Send(new PointerPanZoomUpdateEvent(viewId: session.ViewId, pointer: pointer, timeStamp: session.Time,
                    position: session.Origin, pan: new Offset(-30 * step, 0), panDelta: new Offset(-30, 0)));
                session.Tester.pump();
                samples.Add(session.Controller.position.pixels);
            }
            Check(samples.Last() > samples.First() + 250, "Wheel timeout stole the active trackpad gesture.");
            session.Send(new PointerPanZoomEndEvent(viewId: session.ViewId, pointer: pointer, timeStamp: session.Time, position: session.Origin));
            session.Settle();
        });
        Case("wheel-unmount-cancels-timer", samples =>
        {
            using var session = new InputSession(loop: true);
            session.Send(new PointerScrollEvent(viewId: session.ViewId, timeStamp: session.Time,
                position: session.Origin, scrollDelta: new Offset(120, 0)));
            session.Tester.pumpWidget(new SizedBox());
            session.Tester.pump(TimeSpan.FromMilliseconds(240));
            Check(!session.Controller.hasClients && session.Tester.Clock.PendingTimers == 0, "Wheel timer survived unmount.");
        });
        File.WriteAllText(Path.Combine(output, "continuous-input.json"), JsonSerializer.Serialize(results,
            new JsonSerializerOptions { WriteIndented = true }));
        if (failures.Count > 0) throw new InvalidOperationException(string.Join(Environment.NewLine, failures));
    }

    private static void Check(bool value, string message)
    {
        if (!value) throw new InvalidOperationException(message);
    }

    private sealed class InputSession : IDisposable
    {
        internal WidgetTester Tester { get; } = new(new Size(400, 300));
        internal CustomCarouselScrollController Controller { get; } = new();
        internal List<int?> Settled { get; } = [];
        internal List<double> ScrollDeltas { get; } = [];
        internal long ViewId => checked((long)Tester.View.viewId);
        internal Duration Time => new(Tester.Clock.Elapsed.Ticks / 10);
        internal Offset Origin => new(160, 150);
        internal InputSession(bool loop, bool reverse = false)
        {
            Tester.pumpWidget(new Directionality(textDirection: TextDirection.ltr,
                child: new ScrollConfiguration(behavior: new ScrollBehavior().copyWith(scrollbars: false,
                    dragDevices: [PointerDeviceKind.touch, PointerDeviceKind.mouse, PointerDeviceKind.trackpad]),
                    child: new NotificationListener<ScrollNotification>(onNotification: notification =>
                    {
                        if (notification is ScrollUpdateNotification update) ScrollDeltas.Add(update.scrollDelta ?? 0);
                        return false;
                    }, child: new Carousel(controller: Controller, loop: loop, reverse: reverse, scrollDirection: Axis.horizontal,
                        itemCountBefore: 1, itemCountAfter: 1, alignment: Alignment.center,
                        onSettledItemChanged: Settled.Add, physics: new CustomCarouselScrollPhysics(sticky: true),
                        effectsBuilder: (_, ratio, child) => Transform.CreateTranslate(offset: new Offset(ratio * 150, 0), child: child),
                        children: Enumerable.Range(0, 8).Select(i => (Widget)new SizedBox(width: 70, height: 100,
                            child: new ColoredBox(color: new Color(0xff336699 + i * 0x110000)))).ToArray())))));
        }
        internal void Send(PointerEvent input) => Tester.View.DispatchPlatformEvent(() =>
            WidgetsFlutterBinding.instance.handlePointerEvent(input));
        internal void Settle()
        {
            Tester.pumpAndSettle();
            // The spring's completion posts back to the view's owner queue.
            for (var frame = 0; frame < 30 && Controller.position.isScrollingNotifier.value; frame++)
            {
                Thread.Sleep(1);
                Tester.pump(TimeSpan.FromMilliseconds(16));
            }
            Check(!Controller.position.isScrollingNotifier.value, "Scroll completion did not reach the owner queue.");
        }
        public void Dispose()
        {
            Tester.pumpWidget(new SizedBox());
            Controller.dispose();
            Tester.Dispose();
        }
    }
}
