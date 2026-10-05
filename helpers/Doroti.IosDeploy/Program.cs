using System.ComponentModel;
using System.Text.Json;
using System.Xml;

namespace Doroti.IosDeploy;

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
            if (!OperatingSystem.IsMacOS())
                throw new DeployException("This helper requires macOS and Xcode.");
            foreach (var tool in new[] { "xcrun", "security", "plutil" })
                _ = Executables.Resolve(tool);
            var ui = new ConsoleUi();
            var cli = new DeploymentCli(FindRepository(), new ProcessRunner(ui), ui);
            await cli.RunAsync(options, cancellation.Token);
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
