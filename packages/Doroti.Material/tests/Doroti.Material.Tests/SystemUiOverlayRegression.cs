using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Hosting;
using Doroti.Material;
using Doroti.Testing;
using Doroti.Ui;

internal static class SystemUiOverlayRegression
{
    private static void Require(bool value, string message)
    {
        if (!value) throw new InvalidOperationException(message);
    }

    internal static void Run()
    {
        var received = new List<SystemUiOverlaySettings>();
        var channel = new SystemUiOverlayPlatformMessageCapability(new Fallback(), (style, _) =>
        { received.Add(style); return ValueTask.CompletedTask; });
        using var tester = new WidgetTester(size: new Size(320, 600), devicePixelRatio: 2,
            operatingSystem: HostOperatingSystem.android, platformMessages: channel);
        using var environment = tester.View.EnterPlatformEnvironmentScope();
        var explicitStyle = new SystemUiOverlayStyle(statusBarColor: new Color(0xff123456),
            statusBarIconBrightness: Brightness.light, statusBarBrightness: Brightness.dark);
        var themeStyle = new SystemUiOverlayStyle(statusBarColor: new Color(0xffabcdef),
            statusBarIconBrightness: Brightness.dark, statusBarBrightness: Brightness.light);
        var surroundingStyle = new SystemUiOverlayStyle(statusBarColor: new Color(0xff000001),
            systemNavigationBarColor: new Color(0xff654321), systemNavigationBarIconBrightness: Brightness.dark);

        void Pump(SystemUiOverlayStyle? style, SystemUiOverlayStyle? themed, Color background) =>
            tester.pumpWidget(new MaterialApp(themeAnimationDuration: Doroti.Runtime.Duration.zero,
                theme: ThemeData.Create(appBarTheme: new AppBarThemeData(systemOverlayStyle: themed)),
                home: new AnnotatedRegion<SystemUiOverlayStyle>(value: surroundingStyle,
                    child: new Scaffold(appBar: new AppBar(title: new Text("System bars"), systemOverlayStyle: style,
                        backgroundColor: background), body: new SizedBox()))));

        Pump(explicitStyle, themeStyle, new Color(0xff000000));
        Require(received[^1].StatusBarColor == 0xff123456 && received[^1].StatusBarIconBrightness == Brightness.light,
            "AppBar.systemOverlayStyle did not override its theme and surrounding annotation.");
        Require(received[^1].SystemNavigationBarColor == 0xff654321
            && received[^1].SystemNavigationBarIconBrightness == Brightness.dark,
            "The enclosing region must retain ownership of the lower Android system bar.");
        Pump(null, themeStyle, new Color(0xff000000));
        Require(received[^1].StatusBarColor == 0xffabcdef && received[^1].StatusBarIconBrightness == Brightness.dark,
            "AppBarThemeData.systemOverlayStyle was not used when the AppBar omitted its style.");
        Pump(null, null, new Color(0xff000000));
        Require(received[^1].StatusBarIconBrightness == Brightness.light && received[^1].StatusBarBrightness == Brightness.dark,
            "A dark AppBar background did not infer light status bar icons.");
        Pump(null, null, new Color(0xffffffff));
        Require(received[^1].StatusBarIconBrightness == Brightness.dark && received[^1].StatusBarBrightness == Brightness.light,
            "A light AppBar background did not infer dark status bar icons.");
        tester.pumpWidget(new MaterialApp(home: new Scaffold(body:
            new AnnotatedRegion<SystemUiOverlayStyle>(value: explicitStyle, child: new SizedBox(
                width: double.PositiveInfinity, height: double.PositiveInfinity)))));
        Require(received[^1].StatusBarColor == 0xff123456, "A screen without an AppBar could not use AnnotatedRegion.");
        Console.WriteLine("PASS: AppBar systemOverlayStyle/theme/background precedence and enclosing navigation region; standalone AnnotatedRegion (CPU/synthetic).");
    }

    private sealed class Fallback : IPlatformMessageHostCapability
    {
        public ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel, ReadOnlyMemory<byte>? data,
            CancellationToken cancellationToken = default) => ValueTask.FromResult<ReadOnlyMemory<byte>?>("[null]"u8.ToArray());
        public void SetMessageHandler(string channel, PlatformMessageHandler? handler) { }
    }
}
