using System.Security.Cryptography;
using System.Text;

namespace Doroti.Host.Qt;

internal static class QtApplicationStorage
{
    internal static string NavigationDirectory(string applicationId)
    {
        // GetFolderPath may return empty when a fresh XDG directory does not yet
        // exist. Never fall back to the working directory for persistent state.
        var directory = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
        if (string.IsNullOrEmpty(directory) || !Path.IsPathFullyQualified(directory))
            directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share");
        var identity = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(applicationId)));
        return Path.Combine(directory, "Doroti", identity, "navigation");
    }
}
