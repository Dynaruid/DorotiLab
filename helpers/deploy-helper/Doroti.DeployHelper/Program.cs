using System.ComponentModel;
using System.Text.Json;
using System.Xml;

namespace Doroti.DeployHelper;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        using var cancellation = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; cancellation.Cancel(); };
        try
        {
            var options = Options.Parse(args);
            if (options.Help)
            {
                Console.WriteLine(Options.HelpText);
                return 0;
            }
            var ui = new ConsoleUi();
            var platform = await new Selector(ui).SelectAsync("Platform", new[] { "ios", "android" },
                item => item, "--platform / -Platform", cancellation.Token, options.Platform, (item, value) => item == value);
            options.ValidateForPlatform(platform);
            var runner = new ProcessRunner(ui);
            var repository = FindRepository();
            if (platform == "ios")
            {
                if (!OperatingSystem.IsMacOS()) throw new DeployException("iOS deployment requires macOS and Xcode. Use --platform android on Windows/Linux.");
                foreach (var tool in new[] { "xcrun", "security", "plutil" }) _ = Executables.Resolve(tool);
                await new IosDeploymentCli(repository, runner, ui).RunAsync(options, cancellation.Token);
            }
            else
                await new AndroidDeploymentCli(repository, runner, ui).RunAsync(options, cancellation.Token);
            return 0;
        }
        catch (OperationCanceledException)
        {
            Console.Error.WriteLine("\nCancelled.");
            return 130;
        }
        catch (Exception error) when (error is DeployException or IOException or Win32Exception
                                      or JsonException or XmlException or FormatException)
        {
            Console.Error.WriteLine($"Error: {error.Message}");
            return 1;
        }
    }

    private static string FindRepository()
    {
        foreach (var start in new[] { AppContext.BaseDirectory, Environment.CurrentDirectory })
            for (var directory = new DirectoryInfo(start); directory is not null; directory = directory.Parent)
                if (File.Exists(Path.Combine(directory.FullName, "global.json")) &&
                    File.Exists(Path.Combine(directory.FullName, "samples/DorotiSampleApp2/ios/DorotiSampleApp2.iOS.csproj")))
                    return directory.FullName;
        throw new DeployException("Cannot locate the DorotiLab repository from the CLI location or current directory.");
    }
}
