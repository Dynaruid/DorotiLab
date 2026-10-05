using System.Text.RegularExpressions;

namespace Doroti.IosDeploy;

public sealed class Options
{
    public string? App { get; set; }
    public string Target { get; set; } = "auto";
    public string? Device { get; set; }
    public int DotnetVersion { get; set; } = 10;
    public string? Mode { get; set; }
    public string? Configuration { get; set; }
    public string? CodesignKey { get; set; }
    public string? CodesignProvision { get; set; }
    public string DotnetPath { get; set; } = "dotnet";
    public Dictionary<string, string> Environment { get; } = new(StringComparer.Ordinal);
    public bool SkipXcodeValidation { get; set; }
    public bool NoLaunch { get; set; }
    public bool List { get; set; }
    public bool DryRun { get; set; }
    public bool Help { get; set; }

    public static Options Parse(string[] args)
    {
        var options = new Options();
        for (var i = 0; i < args.Length; i++)
        {
            var option = args[i];
            string Value()
            {
                if (++i >= args.Length || args[i].StartsWith("--", StringComparison.Ordinal))
                    throw new DeployException($"Missing value for {option}.");
                return args[i];
            }
            string Choice(params string[] choices)
            {
                var value = Value();
                return choices.FirstOrDefault(choice => choice.Equals(value, StringComparison.OrdinalIgnoreCase))
                       ?? throw new DeployException($"{option} must be one of: {string.Join(", ", choices)}.");
            }
            switch (option)
            {
                case "--app": options.App = Choice("Sample2", "Testbed"); break;
                case "--target": options.Target = Choice("auto", "device", "simulator"); break;
                case "--device": options.Device = Value(); break;
                case "--dotnet-version": options.DotnetVersion = int.Parse(Choice("10", "11")); break;
                case "--mode": options.Mode = Choice("Mono", "NativeAot"); break;
                case "--configuration": options.Configuration = Choice("Debug", "Release"); break;
                case "--codesign-key": options.CodesignKey = Value(); break;
                case "--codesign-provision": options.CodesignProvision = Value(); break;
                case "--dotnet-path": options.DotnetPath = Value(); break;
                case "--env":
                    var entry = Value();
                    var separator = entry.IndexOf('=');
                    if (separator <= 0 || !Regex.IsMatch(entry[..separator], "^[A-Za-z_][A-Za-z0-9_]*$"))
                        throw new DeployException("Use NAME=VALUE, for example --env DOROTI_IOS_GRAPHITE=1.");
                    options.Environment[entry[..separator]] = entry[(separator + 1)..];
                    break;
                case "--skip-xcode-validation": options.SkipXcodeValidation = true; break;
                case "--no-launch": options.NoLaunch = true; break;
                case "--list": options.List = true; break;
                case "--dry-run": options.DryRun = true; break;
                case "--help": case "-h": options.Help = true; break;
                default: throw new DeployException($"Unknown option: {option}. Use --help.");
            }
        }
        return options;
    }

    public const string HelpText = """
        Select, build, install and launch either Doroti iOS sample on macOS.

        dotnet run --project helpers/Doroti.IosDeploy -- [options]

          --app Sample2|Testbed             Omit to select the sample interactively.
          --target auto|device|simulator    Default: combined device/simulator list.
          --device ID                      Exact device UDID, CoreDevice ID or simulator UDID.
          --dotnet-version 10|11           Default: 10; selects the app build SDK.
          --mode Mono|NativeAot            Default: device NativeAot; simulator Mono.
          --configuration Debug|Release    Default: device Release; simulator Debug.
          --codesign-key NAME_OR_SHA1      Exact development identity; omit to select.
          --codesign-provision UUID_OR_NAME  Compatible development profile; omit to select.
          --dotnet-path PATH               dotnet executable for app builds.
          --skip-xcode-validation          Pass ValidateXcodeVersion=false for this build.
          --env NAME=VALUE                 App launch environment; repeat as needed.
          --no-launch                      Install without launching the app.
          --list                           Inventory only; do not build.
          --dry-run                        Select and inspect; do not build, boot, install or launch.
          --help, -h                       Show this help.
        """;
}
