using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Doroti.Intl.Tests;

internal static class NativeProfile
{
    internal static string Inspect()
    {
        var profiles = new[] { "en-US", "ko-KR", "fr-FR", "hi-IN", "th-TH" }
            .Select(name => new CultureInfo(name, useUserOverride: false))
            .Select(culture => new RawCulture
            {
                Name = culture.Name,
                January = culture.DateTimeFormat.GetMonthName(1),
                Morning = culture.DateTimeFormat.AMDesignator,
                Evening = culture.DateTimeFormat.PMDesignator,
                ShortDatePattern = culture.DateTimeFormat.ShortDatePattern,
                ShortTimePattern = culture.DateTimeFormat.ShortTimePattern,
                Calendar = culture.Calendar.GetType().Name,
                Currency = 1234.5m.ToString("C", culture),
                Number = 1234567.5m.ToString("N2", culture),
            }).ToArray();
        using var process = Process.GetCurrentProcess();
        var modules = process.Modules.Cast<ProcessModule>()
            .Where(module => module.ModuleName.Contains("icu", StringComparison.OrdinalIgnoreCase))
            .Select(module => module.FileName + " (" + module.FileVersionInfo.FileVersion + ")").ToArray();
        var report = new NativeEnvironment
        {
            Runtime = RuntimeInformation.FrameworkDescription,
            OperatingSystem = RuntimeInformation.OSDescription,
            IcuModules = modules,
            Cultures = profiles,
        };
        return JsonSerializer.Serialize(report, NativeProfileJson.Default.NativeEnvironment);
    }
}

internal sealed class NativeEnvironment
{
    public string Runtime { get; set; } = "";
    public string OperatingSystem { get; set; } = "";
    public string[] IcuModules { get; set; } = [];
    public RawCulture[] Cultures { get; set; } = [];
}

internal sealed class RawCulture
{
    public string Name { get; set; } = "";
    public string January { get; set; } = "";
    public string Morning { get; set; } = "";
    public string Evening { get; set; } = "";
    public string ShortDatePattern { get; set; } = "";
    public string ShortTimePattern { get; set; } = "";
    public string Calendar { get; set; } = "";
    public string Currency { get; set; } = "";
    public string Number { get; set; } = "";
}

[JsonSerializable(typeof(NativeEnvironment))]
internal partial class NativeProfileJson : JsonSerializerContext;
