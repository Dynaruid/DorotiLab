using System.Text.Json;
using Doroti.Host.WindowsAppSdk;

internal static class GpuSelectionFixture
{
    internal static void Run()
    {
        Require(WindowsGpuSelection.ParsePreference(null) == WindowsGpuPreference.NoPreference,
            "Missing preference must use the system default.");
        Require(WindowsGpuSelection.ParsePreference("  ") == WindowsGpuPreference.NoPreference,
            "Blank preference must use the system default.");
        Require(WindowsGpuSelection.ParsePreference(" highperformancepreference ") ==
            WindowsGpuPreference.HighPerformancePreference, "Preference must ignore case and surrounding spaces.");
        ExpectFailure<InvalidOperationException>(() => WindowsGpuSelection.ParsePreference("2"));
        ExpectFailure<InvalidOperationException>(() => WindowsGpuSelection.ParsePreference("typo"));
        Require(WindowsGpuSelection.ParseVulkanLuid("8877665544332211") == 0x1122334455667788UL,
            "Vulkan LUID bytes must retain Windows low/high word order.");

        var previous = Environment.GetEnvironmentVariable("DOROTI_WINDOWS_GPU_PREFERENCE");
        try
        {
            var results = new List<object>();
            foreach (var preference in Enum.GetValues<WindowsGpuPreference>())
            {
                Environment.SetEnvironmentVariable("DOROTI_WINDOWS_GPU_PREFERENCE", preference.ToString());
                var luid = WindowsGpuSelection.SelectAdapter(preference);
                Require(WindowsGpuSelection.SelectAdapter(preference, [luid]) == luid,
                    "An eligible preferred adapter must retain its identity.");
                var attributes = WindowsGpuSelection.AnglePlatformAttributes();
                if (preference == WindowsGpuPreference.NoPreference)
                {
                    Require(attributes.SequenceEqual(new[] { 0x3203, 0x3208, 0x3209, 0x320A, 0x3038 }),
                        "Default ANGLE selection must remain unpinned.");
                }
                else
                {
                    Require(attributes.Length == 9 && attributes[4] == 0x34A0 && attributes[6] == 0x34A1 &&
                        unchecked((uint)attributes[5]) == (uint)(luid >> 32) &&
                        unchecked((uint)attributes[7]) == unchecked((uint)luid) && attributes[8] == 0x3038,
                        "ANGLE preference must use the selected DXGI adapter's LUID.");
                }
                // A single eligible device must win even if the requested preference ranks it last.
                foreach (var otherPreference in Enum.GetValues<WindowsGpuPreference>())
                    Require(WindowsGpuSelection.SelectAdapter(otherPreference, [luid]) == luid,
                        "Preference must not select outside the eligible hardware set.");
                results.Add(new { preference = preference.ToString(), luid = $"{luid:X16}" });
            }
            ExpectFailure<ArgumentException>(() => WindowsGpuSelection.SelectAdapter(WindowsGpuPreference.NoPreference, []));
            ExpectFailure<InvalidOperationException>(() => WindowsGpuSelection.SelectAdapter(WindowsGpuPreference.NoPreference, [ulong.MaxValue]));
            ExpectFailure<InvalidOperationException>(() => WindowsGpuSelection.SelectAdapter((WindowsGpuPreference)99));
            Console.WriteLine(JsonSerializer.Serialize(new { status = "PASS", adapters = results }));
        }
        finally
        {
            Environment.SetEnvironmentVariable("DOROTI_WINDOWS_GPU_PREFERENCE", previous);
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static void ExpectFailure<T>(Action action) where T : Exception
    {
        try { action(); }
        catch (T) { return; }
        throw new InvalidOperationException($"Expected {typeof(T).Name}.");
    }
}
