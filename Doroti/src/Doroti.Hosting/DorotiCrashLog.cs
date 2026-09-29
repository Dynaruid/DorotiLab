namespace Doroti.Hosting;

/// <summary>Best-effort local crash record for GUI executables without a console.</summary>
public static class DorotiCrashLog
{
    public static void Write(string applicationId, Exception error)
    {
        try
        {
            var name = string.Concat(applicationId.Select(character => char.IsLetterOrDigit(character) || character is '.' or '-' ? character : '_'));
            var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Doroti", name, "logs");
            Directory.CreateDirectory(directory);
            File.WriteAllText(Path.Combine(directory, "last-crash.txt"),
                $"UTC: {DateTimeOffset.UtcNow:O}\nRuntime: {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}\n{error}");
        }
        catch (Exception failure) when (failure is IOException or UnauthorizedAccessException or ArgumentException)
        { Console.Error.WriteLine("Could not write the local Doroti crash log: " + failure.Message); }
    }
}
