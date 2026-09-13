// The mounted sample runs without the Testbed's native window entrypoint.
// Supply only the window-effect caption consumed by its settings widgets.
internal static class App
{
    internal static string WindowEffectLabel => OperatingSystem.IsMacOS()
        ? "Window blur" : "Acrylic window";
}
