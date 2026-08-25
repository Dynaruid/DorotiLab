using System.Runtime.InteropServices;
using Doroti.Hosting;
using Doroti.Skia.Rendering;
using Doroti.Skia.RuntimeEffects;
using Doroti.Ui;

namespace Doroti.Host.WindowsAppSdk;

public static partial class DorotiWindowsAppSdkRunner
{
    public static int Run(DorotiApplicationDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        if (!OperatingSystem.IsWindows())
            throw new PlatformNotSupportedException(
                "Doroti.Host.WindowsAppSdk can only launch on Windows.");

        var initializeResult = RoInitialize(0);
        if (initializeResult < 0) Marshal.ThrowExceptionForHR(initializeResult);
        try
        {
            return RunCore(descriptor);
        }
        finally
        {
            RoUninitialize();
        }
    }

    private static int RunCore(DorotiApplicationDescriptor descriptor)
    {
        using var application = DorotiApplicationBoundary.Load(
            descriptor.ManifestAssembly,
            descriptor.ApplicationAssembly,
            descriptor.LaunchContext.RuntimeIdentifier,
            descriptor.NativePluginHandlers);
        using var session = new DorotiHostSession(descriptor.EntrypointFactory());
        session.Start(deferFrameworkBootstrap: true);

        var adapter = WindowsAppSdkAdapterSelection.Resolve();
        IWindowsAppSdkProductHost host = adapter switch
        {
            WindowsAppSdkAdapterKind.FlutterEmbedder =>
                new FlutterWindowsHostAdapter(1, descriptor.ViewConfiguration),
            WindowsAppSdkAdapterKind.ArmNLegacy =>
                new WindowsAppSdkHostAdapter(1, descriptor.ViewConfiguration),
            _ => throw new ArgumentOutOfRangeException(nameof(adapter)),
        };
        var targetIdentity = adapter switch
        {
            WindowsAppSdkAdapterKind.FlutterEmbedder =>
                "win-x64/windowsappsdk-2.4/raw-child-hwnd/flutter-embedder/angle-egl-skia",
            WindowsAppSdkAdapterKind.ArmNLegacy =>
                "win-x64/windowsappsdk-2.4/raw-hwnd/arm-n-dual-front/d3d12-skia",
            _ => throw new ArgumentOutOfRangeException(nameof(adapter)),
        };
        var renderer = new SkiaSceneRenderer(
            1,
            host,
            descriptor.ViewConfiguration.backgroundColor,
            descriptor.ViewConfiguration.darkBackgroundColor,
            targetIdentity,
            adapter == WindowsAppSdkAdapterKind.FlutterEmbedder
                ? DorotiSkiaRuntimeEffects.WindowsAngleEglBackend
                : DorotiSkiaRuntimeEffects.MauiGpuBackend,
            adapter == WindowsAppSdkAdapterKind.FlutterEmbedder
                ? "windowsappsdk-2.4-flutter-angle-egl-skia"
                : "windowsappsdk-2.4-arm-n-d3d12-skia");
        var messages = new WindowsAppSdkPlatformMessageCapability();
        var capabilities = new DorotiViewCapabilities(targetIdentity)
            .Register<IViewHostCapability>(DorotiCapabilityIds.WindowLifecycle, host)
            .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, host)
            .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, host)
            .Register<IInputHostCapability>(DorotiCapabilityIds.InputEvents, host)
            .Register<ITextInputHostCapability>(DorotiCapabilityIds.TextInput, host)
            .Register<IPlatformServicesHostCapability>(DorotiCapabilityIds.PlatformServices, host)
            .Register<IPlatformEnvironmentHostCapability>(DorotiCapabilityIds.PlatformEnvironment, host)
            .Register<ISceneHostCapability>(DorotiCapabilityIds.GraphicsScene, renderer)
            .Register<IParagraphHostCapability>(DorotiCapabilityIds.GraphicsText, renderer)
            .Register<IImageHostCapability>(DorotiCapabilityIds.GraphicsImage, renderer)
            .Register<ISemanticsHostCapability>(DorotiCapabilityIds.AccessibilitySemantics, renderer);
        application.Configure(capabilities, messages);

        DorotiView? view = null;
        Timer? smokeTimer = null;
        Timer? resizeSmokeTimer = null;
        try
        {
            using (session.dispatcher.EnterScope())
            {
                view = session.dispatcher.RegisterView(1, capabilities);
                renderer.AttachFrameworkTrace(session.dispatcher.frameTrace);
                session.AttachView(view);
                host.AttachRenderer(renderer);
                session.dispatcher.setSemanticsTreeEnabled(true);
                host.Show();
            }

            if (TryGetSmokeDuration(out var smokeDuration))
            {
                if (string.Equals(
                    Environment.GetEnvironmentVariable("DOROTI_WINDOWS_APPSDK_RESIZE_SMOKE"),
                    "left",
                    StringComparison.OrdinalIgnoreCase))
                {
                    var smokeClock = System.Diagnostics.Stopwatch.StartNew();
                    var resizeStep = 0;
                    resizeSmokeTimer = new(_ =>
                    {
                        if (smokeClock.Elapsed < smokeDuration - TimeSpan.FromSeconds(1))
                            host.ApplyLeftResizeSmokeStep(Interlocked.Increment(ref resizeStep));
                    }, null, TimeSpan.FromMilliseconds(250), TimeSpan.FromMilliseconds(8));
                }
                smokeTimer = new(_ => host.Close(), null, smokeDuration, Timeout.InfiniteTimeSpan);
            }
            var exitCode = host.RunMessageLoop();
            if (ShouldWriteDiagnostics()) host.WriteDiagnostics(renderer.Diagnostics);
            return exitCode;
        }
        finally
        {
            smokeTimer?.Dispose();
            resizeSmokeTimer?.Dispose();
            if (view is null)
            {
                capabilities.Dispose();
            }
            else
            {
                session.DetachView(view);
                view.Dispose();
            }
        }
    }

    private static bool TryGetSmokeDuration(out TimeSpan duration)
    {
        var value = Environment.GetEnvironmentVariable("DOROTI_WINDOWS_APPSDK_SMOKE_MS");
        if (int.TryParse(value, out var milliseconds) && milliseconds is >= 250 and <= 60_000)
        {
            duration = TimeSpan.FromMilliseconds(milliseconds);
            return true;
        }
        duration = default;
        return false;
    }

    private static bool ShouldWriteDiagnostics() =>
        TryGetSmokeDuration(out _) ||
        string.Equals(
            Environment.GetEnvironmentVariable("DOROTI_WINDOWS_APPSDK_DIAGNOSTICS"),
            "1",
            StringComparison.Ordinal);

    [LibraryImport("combase.dll")]
    private static partial int RoInitialize(uint initializationType);

    [LibraryImport("combase.dll")]
    private static partial void RoUninitialize();
}
