namespace Doroti.Host.Web;

// Records the native interop boundary; the production duration mapping is linked into this fixture.
internal static class BrowserInterop
{
    internal static readonly List<int> Durations = [];
    internal static Task VibrateAsync(int duration)
    {
        Durations.Add(duration);
        return Task.CompletedTask;
    }
}
