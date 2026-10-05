using System.Text.RegularExpressions;

namespace Doroti.DeployHelper;

public sealed class Options
{
    public string? Platform { get; set; }
    public string? App { get; set; }
    public string Target { get; set; } = "auto";
    public string? Device { get; set; }
    public int DotnetVersion { get; set; } = 10;
    public string? Mode { get; set; }
    public string? Configuration { get; set; }
    public string? CodesignKey { get; set; }
    public string? CodesignProvision { get; set; }
    public string DotnetPath { get; set; } = "dotnet";
    public string AdbPath { get; set; } = "adb";
    public Dictionary<string, string> Extras { get; } = new(StringComparer.Ordinal);
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
                case "--platform": options.Platform = Choice("ios", "android"); break;
                case "--app": options.App = Choice("Sample2", "Testbed"); break;
                case "--target": options.Target = Choice("auto", "device", "simulator", "emulator"); break;
                case "--device": case "--serial": options.Device = Value(); break;
                case "--dotnet-version": options.DotnetVersion = int.Parse(Choice("10", "11")); break;
                case "--mode": options.Mode = Choice("Mono", "NativeAot", "MonoAot", "CoreClrJit", "CoreClrR2R"); break;
                case "--configuration": options.Configuration = Choice("Debug", "Release"); break;
                case "--codesign-key": options.CodesignKey = Value(); break;
                case "--codesign-provision": options.CodesignProvision = Value(); break;
                case "--dotnet-path": options.DotnetPath = Value(); break;
                case "--adb-path": options.AdbPath = Value(); break;
                case "--extra":
                    var extra = Value();
                    var extraSeparator = extra.IndexOf('=');
                    if (extraSeparator <= 0 || !Regex.IsMatch(extra[..extraSeparator], "^[A-Za-z_][A-Za-z0-9_]*$") || extra.Contains('\0'))
                        throw new DeployException("Use KEY=VALUE for an Android string Intent extra.");
                    options.Extras[extra[..extraSeparator]] = extra[(extraSeparator + 1)..];
                    break;
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

    public void ValidateForPlatform(string platform)
    {
        if (platform == "ios")
        {
            if (Target == "emulator" || Mode is not (null or "Mono" or "NativeAot") || Extras.Count > 0 || AdbPath != "adb")
                throw new DeployException("iOS uses --target auto|device|simulator and --mode Mono|NativeAot. Android extras/adb options are unavailable.");
        }
        else if (platform == "android")
        {
            if (Target == "simulator" || Mode is not (null or "Mono" or "MonoAot" or "CoreClrJit" or "CoreClrR2R"))
                throw new DeployException("Android uses --target auto|device|emulator and --mode Mono|MonoAot|CoreClrJit|CoreClrR2R.");
            if (CodesignKey is not null || CodesignProvision is not null || SkipXcodeValidation)
                throw new DeployException("Codesign and Xcode options are only available for iOS.");
            if (Environment.Count > 0)
                throw new DeployException("Android launches accept --extra KEY=VALUE Intent extras, not --env. The app must handle the requested extra.");
            if (DotnetVersion == 11 && Mode is "Mono" or "MonoAot")
                throw new DeployException("This .NET 11 Android profile supports CoreClrJit or CoreClrR2R; use .NET 10 for Mono/MonoAot.");
            if (Configuration == "Debug" && Mode == "MonoAot")
                throw new DeployException("MonoAot requires Release. Use --mode Mono for an Android Debug build.");
        }
        else throw new DeployException($"Unknown platform: {platform}.");
    }

    public const string HelpText = """
        Select, build, install and launch Doroti samples on iOS or Android.

        dotnet run --project helpers/deploy-helper/Doroti.DeployHelper -- [options]

          --platform ios|android           Omit to select interactively; iOS requires macOS.
          --app Sample2|Testbed             Omit to select the sample interactively.
          --target auto|device|simulator|emulator  Default: combined targets; simulator is iOS, emulator is Android.
          --device ID, --serial ID          iOS UDID/CoreDevice ID or exact adb serial.
          --dotnet-version 10|11           Default: 10; selects the app build SDK.
          --mode MODE                     iOS: Mono|NativeAot. Android: Mono|MonoAot|CoreClrJit|CoreClrR2R.
                                          iOS defaults: device NativeAot, simulator Mono.
                                          Android defaults: .NET 10 Release MonoAot/Debug Mono, .NET 11 CoreClrR2R.
          --configuration Debug|Release    Default: iOS device/Android Release; iOS simulator Debug.
          --codesign-key NAME_OR_SHA1      Exact development identity; omit to select.
          --codesign-provision UUID_OR_NAME  Compatible development profile; omit to select.
          --dotnet-path PATH               dotnet executable for app builds.
          --adb-path PATH                  Android adb executable; SDK locations are also searched.
          --skip-xcode-validation          iOS only: ValidateXcodeVersion=false for this build.
          --env NAME=VALUE                 iOS launch environment; repeat as needed.
          --extra KEY=VALUE                Android string Intent extra; repeat as needed.
          --no-launch                      Install without launching the app.
          --list                           Inventory only; do not build.
          --dry-run                        Select and inspect; do not build, boot, install or launch.
          --help, -h                       Show this help.
        """;
}
