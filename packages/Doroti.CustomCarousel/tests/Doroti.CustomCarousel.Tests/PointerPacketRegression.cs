using Doroti.CustomCarousel;
using Doroti.Framework.Foundation;
using Doroti.Framework.Widgets;
using Doroti.Testing;
using Doroti.Ui;
using Carousel = Doroti.CustomCarousel.CustomCarousel;

internal static class PointerPacketRegression
{
    internal static void Run()
    {
        using var tester = new WidgetTester(new Size(560, 850));
        tester.pumpWidget(new DorotiCarouselApp.CarouselGallery());
        tester.pumpAndSettle();
        var previousError = FlutterError.onError;
        FlutterError.onError = details =>
        {
            Console.Error.WriteLine(details.exceptionThrown);
            previousError?.Invoke(details);
        };
        try
        {
            void Send(PointerChange change, Offset point, PointerDeviceKind kind = PointerDeviceKind.mouse,
                double wheel = 0)
            {
                var datum = new PointerData(tester.View.viewId, tester.Clock.Elapsed, change, kind, 1,
                    point.dx * tester.View.devicePixelRatio, point.dy * tester.View.devicePixelRatio,
                    0, 0, 0, scrollDeltaY: wheel * tester.View.devicePixelRatio,
                    signalKind: wheel == 0 ? PointerSignalKind.none : PointerSignalKind.scroll,
                    pointerIdentifier: 1);
                tester.View.DispatchPlatformEvent(() => PlatformDispatcher.instance.onPointerDataPacket!
                    .Invoke(tester.View, new PointerDataPacket([datum])));
            }

            for (var demo = 0; demo < 5; demo++)
            {
                var home = ((Carousel)tester.byType<Carousel>().Single().widget).controller!;
                tester.View.DispatchPlatformEvent(() => home.jumpToItem(demo));
                tester.pumpAndSettle();
                tester.tap(tester.byKey(new ValueKey<string>("open-demo")).Single());
                tester.pumpAndSettle();
                var target = tester.byType<Carousel>().First();
                var point = tester.center(target);
                Send(PointerChange.add, point);
                Send(PointerChange.hover, point);
                for (var packet = 0; packet < 6; packet++)
                {
                    Send(PointerChange.hover, point,
                        packet % 2 == 0 ? PointerDeviceKind.mouse : PointerDeviceKind.trackpad,
                        packet < 3 ? 30 : -30);
                    tester.pump(TimeSpan.FromMilliseconds(16));
                    Send(PointerChange.hover, point);
                }
                Send(PointerChange.remove, point);
                tester.pumpAndSettle();
                tester.tap(tester.byKey(new ValueKey<string>("back")).Single());
                tester.pumpAndSettle();
                Console.WriteLine($"PASS: demo {demo} mouse/trackpad packets and animated hover hit testing.");
            }
        }
        finally { FlutterError.onError = previousError; }
    }
}
