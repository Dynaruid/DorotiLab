using Doroti.Framework.Foundation;
using Doroti.Framework.Widgets;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using M = Doroti.Framework.Material;

internal static partial class MountedPickerContracts
{
    internal static void VerifyWindowTitlebarTheme()
    {
        using var platform = PlatformEnvironmentContext.Enter(new PlatformConfiguration(
            [new Locale("en", "US")], Brightness.light, false, false, HostOperatingSystem.linux));
        using var dispatcher = new PlatformDispatcher();
        using var scope = dispatcher.EnterScope();
        using var host = new Host();
        using var renderer = new SkiaSceneRenderer(1, host, new Color(0), null,
            "caption-theme", "caption-theme", "caption-theme");
        var caption = new CaptionThemeRecorder();
        using var view = dispatcher.RegisterView(1, new DorotiViewCapabilities("caption-theme")
            .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, host)
            .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, host)
            .Register<IInputHostCapability>(DorotiCapabilityIds.InputEvents, host)
            .Register<IParagraphHostCapability>(DorotiCapabilityIds.GraphicsText, renderer)
            .Register<ISceneHostCapability>(DorotiCapabilityIds.GraphicsScene, renderer)
            .Register<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging, host)
            .Register<IWindowTitlebarHostCapability>(DorotiCapabilityIds.WindowTitlebar, caption));
        var binding = new WidgetsFlutterBinding(dispatcher);
        var errors = new List<FlutterErrorDetails>();
        var previousError = FlutterError.onError;
        FlutterError.onError = errors.Add;
        try
        {
            var light = M.ThemeData.Create(scaffoldBackgroundColor: new Color(0x99fffbfe));
            var dark = M.ThemeData.Create(brightness: Brightness.dark,
                scaffoldBackgroundColor: new Color(0x99141218));
            SetApp(M.ThemeMode.light, light);
            CheckTheme(light);
            caption.Themes.Clear();
            SetApp(M.ThemeMode.dark, light);
            CheckTheme(dark);
            if (!caption.Themes.Any(t => t.BackgroundColor != light.scaffoldBackgroundColor &&
                t.BackgroundColor != dark.scaffoldBackgroundColor))
                throw new Exception("Caption skipped the MaterialApp theme animation.");
            var custom = light.copyWith(scaffoldBackgroundColor: new Color(0xffd0e8d5));
            SetApp(M.ThemeMode.light, custom);
            CheckTheme(custom);
            Console.WriteLine("MaterialApp caption theme: PASS (light, dark, animated transition, custom opaque surface)");

            void SetApp(M.ThemeMode mode, M.ThemeData theme)
            {
                view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(
                    new M.MaterialApp(locale: new Locale("en", "US"), theme: theme, darkTheme: dark,
                        themeMode: mode, home: new M.Scaffold(body: SizedBox.CreateShrink())))));
                for (var i = 0; i < 40; i++) { host.Fire(); Thread.Sleep(10); }
                if (errors.Count > 0) throw new Exception(string.Join("\n", errors.Select(e => e.exceptionThrown)));
            }
            void CheckTheme(M.ThemeData theme)
            {
                if (caption.Themes.LastOrDefault() != new WindowTitlebarTheme(theme.scaffoldBackgroundColor, theme.brightness))
                    throw new Exception("Caption does not match the mounted MaterialApp theme.");
            }
        }
        finally { FlutterError.onError = previousError; }
    }

    private sealed class CaptionThemeRecorder : IWindowTitlebarHostCapability
    {
        internal List<WindowTitlebarTheme> Themes { get; } = [];
        public void SetTheme(WindowTitlebarTheme theme) => Themes.Add(theme);
    }
}
