using Doroti.Hosting;
using Doroti.Ui;

internal static class PlatformRetirementRegression
{
    public static async Task Run()
    {
        using var coordinator = new PlatformViewCoordinator(1, "test", new([new Factory()]), new Dispatcher());
        var old = await coordinator.CreateAsync(new PlatformViewRequest(1, "test", PlatformViewComposition.InterleavedComposition));
        SceneCommand[] Commands(PlatformViewHandle handle) =>
            [new("platformView", null) { HostPayload = new ScenePlatformViewPayload(handle, Rect.fromLTWH(0, 0, 30, 30)) }];
        if (coordinator.ReferencesRetiredHandle(Commands(old))) throw new Exception("Live view was classified as retired.");
        await coordinator.DisposeAsync(old);
        SceneCommand[] retained = [new("retained", null) { HostPayload = new SceneRetainedPayload(Commands(old), 1, 1) }];
        if (!coordinator.ReferencesRetiredHandle(retained)) throw new Exception("Nested retained layer bypassed native retirement detection.");
        var replacement = await coordinator.CreateAsync(new PlatformViewRequest(1, "test", PlatformViewComposition.InterleavedComposition));
        if (!coordinator.ReferencesRetiredHandle(Commands(old)) || coordinator.ReferencesRetiredHandle(Commands(replacement)) ||
            coordinator.ReferencesRetiredHandle(Commands(new(2, old.InstanceId, old.InstanceGeneration))))
            throw new Exception("Retirement classification lost generation or owner isolation.");
        using var plan = PlatformCompositionPlanner.Build(Commands(replacement), new(1, 1, 1, 1, 1, 1),
            coordinator, PlatformViewComposition.InterleavedComposition);
        plan.Dispose();
        await coordinator.DisposeAsync();
        Console.WriteLine("PASS: queued native-view retirement is distinguished from a live replacement and a foreign handle.");
    }
    private sealed class Dispatcher : IPlatformViewDispatcher
    {
        public ValueTask InvokeAsync(Func<ValueTask> action) => action();
    }
    private sealed class Factory : IPlatformViewFactory
    {
        public string ViewType => "test";
        public PlatformViewSupport QuerySupport(PlatformViewRequest request) => new("test", "test", ViewType, true,
            request.Composition, request.Effects, SynchronizedPlacement: true);
        public ValueTask<IPlatformViewInstance> CreateAsync(PlatformViewHandle handle, ReadOnlyMemory<byte> parameters,
            Action<PlatformViewHandle> onFocused, CancellationToken cancellationToken) => ValueTask.FromResult<IPlatformViewInstance>(new Instance());
    }
    private sealed class Instance : IPlatformViewInstance
    {
        public ValueTask ApplyAsync(PlatformViewPlacement placement) => ValueTask.CompletedTask;
        public ValueTask DetachAsync() => ValueTask.CompletedTask;
        public ValueTask SetFocusAsync(bool focused) => ValueTask.CompletedTask;
        public ValueTask DisableInputAsync() => ValueTask.CompletedTask;
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
