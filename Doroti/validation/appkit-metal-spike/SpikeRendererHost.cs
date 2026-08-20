using Doroti.Skia.Rendering;
using Doroti.Ui;

namespace Doroti.Validation.AppKitMetalSpike;

internal sealed class SpikeRendererHost : ISkiaSceneRendererHost
{
    private Action<int, SemanticsAction, object?>? _semanticsAction;
    private Action<long, TimeSpan>? _inputReceived;
    private Action<PlatformConfiguration>? _configurationChanged;
    private Action? _invalidate;
    private long _surfaceGeneration = 1;

    public long InputSequence => 0;
    public long SurfaceGeneration => Interlocked.Read(ref _surfaceGeneration);
    public PlatformConfiguration Configuration { get; } = new(
        [new Doroti.Ui.Locale("en", "US")],
        Brightness.light,
        false,
        false,
        HostOperatingSystem.macOS);

    public event Action<int, SemanticsAction, object?>? SemanticsAction
    {
        add => _semanticsAction += value;
        remove => _semanticsAction -= value;
    }

    public event Action<long, TimeSpan>? InputReceived
    {
        add => _inputReceived += value;
        remove => _inputReceived -= value;
    }

    public event Action<PlatformConfiguration>? ConfigurationChanged
    {
        add => _configurationChanged += value;
        remove => _configurationChanged -= value;
    }

    public void AttachInvalidate(Action invalidate) => _invalidate = invalidate;

    public void SetSurfaceGeneration(long generation) =>
        Interlocked.Exchange(ref _surfaceGeneration, generation);

    public void UpdateSemantics(SemanticsUpdate update) => _ = update;
    public void ClearSemantics() { }
    public void RequestInvalidate() => _invalidate?.Invoke();
}
