using Doroti.Skia.Rendering;
using Doroti.Ui;

internal sealed class ImageFixtureEnvironment : IDisposable
{
    private readonly PlatformDispatcher _dispatcher = new();
    private readonly IDisposable _scope;
    public ImageFixtureEnvironment()
    {
        _scope = _dispatcher.EnterScope();
        var host = new ImageHost();
        var renderer = new SkiaSceneRenderer(91, host, null, null, "validation/skia", "validation", "validation");
        _dispatcher.RegisterView(91, new DorotiViewCapabilities("validation/skia")
            .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, new ClipboardFixtureHost())
            .Register<IImageHostCapability>(DorotiCapabilityIds.GraphicsImage, renderer));
    }
    public void Dispose() { _scope.Dispose(); _dispatcher.Dispose(); }

    private sealed class ImageHost : ISkiaSceneRendererHost
    {
        public long InputSequence => 0;
        public long SurfaceGeneration => 1;
        public DorotiViewEpoch ViewEpoch => throw new NotSupportedException("Offscreen fixture has no frame epoch.");
        public DorotiResizeEpoch ResizeTarget => throw new NotSupportedException("Offscreen fixture has no resize target.");
        public PlatformConfiguration Configuration { get; } = new(
            [new Locale("en", "US")], Brightness.light, false, false, HostOperatingSystem.windows);
        public event Action<int, SemanticsAction, object?>? SemanticsAction { add { } remove { } }
        public event Action<long, TimeSpan>? InputReceived { add { } remove { } }
        public event Action<PlatformConfiguration>? ConfigurationChanged { add { } remove { } }
        public void UpdateSemantics(SemanticsUpdate update) { }
        public void ClearSemantics() { }
        public void RequestInvalidate() { }
    }
}
