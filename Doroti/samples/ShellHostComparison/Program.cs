using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Doroti.Graphics;
using Doroti.Host.Avalonia;
using Doroti.Host.Desktop;
using Doroti.Platform;

internal static class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        var options = ComparisonOptions.Parse(args);
        if (options.Host == "source-port")
        {
            return RunSourcePort(options);
        }

        ComparisonApplication.Options = options;
        return AppBuilder.Configure<ComparisonApplication>()
            .UsePlatformDetect()
            .LogToTrace()
            .StartWithClassicDesktopLifetime(args, ShutdownMode.OnExplicitShutdown);
    }

    private static int RunSourcePort(ComparisonOptions options)
    {
        using var backend = new DesktopWindowBackend();
        var events = new ComparisonEvents();
        using var window = backend.CreateWindow(new("Doroti A1 source-port comparison", new(520, 320)), events);
        window.Show();
        Pump(backend, 120);
        window.Resize(new(640, 400));
        Pump(backend, 120);
        window.SetMinimized(true);
        Pump(backend, 120);
        window.SetMinimized(false);
        Pump(backend, 120);
        MoveAcrossDisplays(window, () => Pump(backend, 180));
        window.Close();
        Pump(backend, 50);
        return ComparisonReport.Write(options, events);
    }

    private static void Pump(DesktopWindowBackend backend, int milliseconds)
    {
        var deadline = Environment.TickCount64 + milliseconds;
        while (Environment.TickCount64 < deadline)
        {
            backend.PumpPendingMessages();
            Thread.Sleep(5);
        }
    }

    internal static void MoveAcrossDisplays(IWindow window, Action pump)
    {
        if (!window.TryGetFeature<IWindowPlacementController>(out var placement) || placement is null)
        {
            return;
        }
        foreach (var display in placement.Displays)
        {
            placement.MoveToDisplay(display.Id);
            pump();
        }
    }
}

internal sealed class ComparisonApplication : Application
{
    internal static ComparisonOptions Options { get; set; } = null!;

    public override void OnFrameworkInitializationCompleted()
    {
        base.OnFrameworkInitializationCompleted();
        var lifetime = ApplicationLifetime as IClassicDesktopStyleApplicationLifetime
            ?? throw new InvalidOperationException("The comparison launcher requires a classic desktop lifetime.");
        Dispatcher.UIThread.Post(async () =>
        {
            var exitCode = 1;
            try
            {
                var backend = new AvaloniaWindowBackend(AvaloniaHostRenderingMode.Software);
                var events = new ComparisonEvents();
                using var window = backend.CreateWindow(new("Doroti A1 package comparison", new(520, 320)), events);
                window.Show();
                await Task.Delay(120);
                window.Resize(new(640, 400));
                await Task.Delay(120);
                window.SetMinimized(true);
                await Task.Delay(120);
                window.SetMinimized(false);
                await Task.Delay(120);
                Program.MoveAcrossDisplays(window, () => Thread.Sleep(180));
                window.Close();
                await Task.Delay(50);
                exitCode = ComparisonReport.Write(Options, events);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception);
            }
            finally
            {
                lifetime.Shutdown(exitCode);
            }
        }, DispatcherPriority.Loaded);
    }
}

internal sealed class ComparisonEvents : IWindowEventSink
{
    internal List<ComparisonEvent> Events { get; } = [];

    public void OnMetricsChanged(WindowId window, WindowMetrics metrics) => Events.Add(new(
        "metrics",
        metrics.LogicalSize.Width,
        metrics.LogicalSize.Height,
        metrics.PixelSize.Width,
        metrics.PixelSize.Height,
        metrics.ScaleFactor,
        metrics.Generation,
        metrics.IsMinimized));

    public void OnCloseRequested(WindowId window) => Events.Add(new("close-requested", 0, 0, 0, 0, 0, 0, false));

    public void OnClosed(WindowId window) => Events.Add(new("closed", 0, 0, 0, 0, 0, 0, false));
}

internal sealed record ComparisonEvent(
    string Kind,
    double LogicalWidth,
    double LogicalHeight,
    double PixelWidth,
    double PixelHeight,
    double Scale,
    long Generation,
    bool IsMinimized);

internal sealed record ComparisonOptions(string Host, string Output)
{
    internal static ComparisonOptions Parse(string[] args)
    {
        var host = "source-port";
        var output = Path.Combine("artifacts", "a1-shell");
        for (var index = 0; index < args.Length; index++)
        {
            switch (args[index])
            {
                case "--host" when index + 1 < args.Length:
                    host = args[++index];
                    break;
                case "--output" when index + 1 < args.Length:
                    output = args[++index];
                    break;
            }
        }
        if (host is not ("source-port" or "package"))
        {
            throw new ArgumentException("--host must be source-port or package.");
        }
        return new(host, Path.GetFullPath(output));
    }
}

internal static class ComparisonReport
{
    internal static int Write(ComparisonOptions options, ComparisonEvents events)
    {
        Directory.CreateDirectory(options.Output);
        var required = new[] { "metrics", "close-requested", "closed" };
        var success = required.All(kind => events.Events.Any(item => item.Kind == kind)) &&
            events.Events.Where(item => item.Kind == "metrics").Select(item => item.IsMinimized).Distinct().Count() == 2;
        var report = new
        {
            schemaVersion = "doroti.a1-shell-comparison/v1",
            host = options.Host,
            success,
            processId = Environment.ProcessId,
            operatingSystem = Environment.OSVersion.VersionString,
            selectedHostHasAvaloniaBinaryDependency = options.Host == "package",
            comparisonHarnessLoadsBothHostGraphs = true,
            observedScales = events.Events.Where(item => item.Scale > 0).Select(item => item.Scale).Distinct().Order().ToArray(),
            events = events.Events,
        };
        var path = Path.Combine(options.Output, $"{options.Host}-report.json");
        File.WriteAllText(path, JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }) + "\n");
        Console.WriteLine($"A1 {options.Host}: {(success ? "PASS" : "FAIL")}");
        Console.WriteLine($"Report: {path}");
        return success ? 0 : 1;
    }
}
