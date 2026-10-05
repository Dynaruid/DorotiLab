using Doroti.Ui;
using Doroti.Hosting;

static class TypedTransportRegression
{
    sealed class CodecTrap : IPlatformMessageHostCapability
    {
        public int Calls { get; private set; }
        public ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel, ReadOnlyMemory<byte>? data, CancellationToken cancellationToken = default)
        { Calls++; throw new InvalidOperationException("Typed services must not enter the codec adapter."); }
        public void SetMessageHandler(string channel, PlatformMessageHandler? handler) { }
    }
    sealed class Feedback(ulong owner) : IPlatformFeedbackHostCapability, IDisposable
    {
        public int Calls { get; private set; }
        public int Disposals { get; private set; }
        public ulong Owner => owner;
        public TaskCompletionSource<PlatformFeedbackResult>? Completion { get; set; }
        public CancellationToken LastToken { get; private set; }
        public PlatformFeedbackStatus Status { get; set; } = PlatformFeedbackStatus.Scheduled;
        public ValueTask<PlatformFeedbackResult> PlaySoundAsync(PlatformSystemSound sound, CancellationToken cancellationToken = default)
        {
            if (!Enum.IsDefined(sound)) throw new ArgumentOutOfRangeException(nameof(sound));
            Calls++; LastToken = cancellationToken;
            return Completion is { } wait ? new(wait.Task) : ValueTask.FromResult(new PlatformFeedbackResult(Status));
        }
        public ValueTask<PlatformFeedbackResult> HapticAsync(PlatformHaptic haptic, CancellationToken cancellationToken = default)
        {
            if (!Enum.IsDefined(haptic)) throw new ArgumentOutOfRangeException(nameof(haptic));
            Calls++; return ValueTask.FromResult(new PlatformFeedbackResult(Status));
        }
        public void Dispose() => Disposals++;
    }
    public static async Task Run()
    {
        var traps = new List<CodecTrap>();
        var feedback = new Dictionary<ulong, Feedback>();
        var provider = new FakeProvider { AdditionalCapabilities = (capabilities, id) => {
            feedback.Add(id, new Feedback(id));
            var trap = new CodecTrap(); traps.Add(trap);
            capabilities.Register<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging, trap, DorotiCapabilityOwnership.Owned);
            capabilities.Register<IPlatformFeedbackHostCapability>(DorotiCapabilityIds.PlatformFeedback, feedback[id], DorotiCapabilityOwnership.Owned);
        }};
        await using var app = new DorotiPlatformApplication(provider.Plan());
        await app.StartAsync(); await app.CreateViewAsync();
        var dispatcher = provider.Framework.Dispatcher!;
        var first = dispatcher.GetView(1, new("typed-owner"));
        var second = dispatcher.GetView(2, new("typed-owner"));
        await first.DispatchPlatformEventAsync(() => Check.True(ReferenceEquals(dispatcher.RequireInvocationView(new("explicit-view")), first), "Selected view was lost."));
        await second.DispatchPlatformEventAsync(() => Check.True(ReferenceEquals(dispatcher.RequireInvocationView(new("explicit-view")), second), "Selected survivor view was lost."));
        var invocation = new DorotiUiInvocation("typed-feedback");
        var result = await first.InvokeCapabilityAsync<IPlatformFeedbackHostCapability, PlatformFeedbackResult>(DorotiCapabilityIds.PlatformFeedback, invocation,
            (host, token) => host.HapticAsync(PlatformHaptic.SelectionClick, token));
        result.RequireScheduled();
        Check.True(feedback[1].Calls == 1 && feedback[2].Calls == 0 && traps.All(trap => trap.Calls == 0), "Feedback selected another view or used a codec.");
        feedback[2].Status = PlatformFeedbackStatus.Unsupported;
        var unsupported = await second.InvokeCapabilityAsync<IPlatformFeedbackHostCapability, PlatformFeedbackResult>(DorotiCapabilityIds.PlatformFeedback, invocation,
            (host, token) => host.PlaySoundAsync(PlatformSystemSound.Tick, token));
        Check.Throws<NotSupportedException>(unsupported.RequireScheduled);
        feedback[2].Status = PlatformFeedbackStatus.Failed;
        var failed = await second.InvokeCapabilityAsync<IPlatformFeedbackHostCapability, PlatformFeedbackResult>(DorotiCapabilityIds.PlatformFeedback, invocation,
            (host, token) => host.PlaySoundAsync(PlatformSystemSound.Alert, token));
        Check.Throws<InvalidOperationException>(failed.RequireScheduled);
        feedback[1].Completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        var pending = first.InvokeCapabilityAsync<IPlatformFeedbackHostCapability, PlatformFeedbackResult>(DorotiCapabilityIds.PlatformFeedback, invocation,
            (host, token) => host.PlaySoundAsync(PlatformSystemSound.Click, token)).AsTask();
        var closing = app.CloseViewAsync(1);
        await provider.Owner.InvokeAsync(() => { });
        Check.True(!closing.IsCompleted && feedback[1].Disposals == 0 && feedback[1].LastToken.IsCancellationRequested,
            "Native close released a service before its actual typed completion.");
        await Check.ThrowsAsync<ObjectDisposedException>(() => first.InvokeCapabilityAsync<IPlatformFeedbackHostCapability, PlatformFeedbackResult>(DorotiCapabilityIds.PlatformFeedback, invocation,
            (host, token) => host.HapticAsync(PlatformHaptic.Vibrate, token)).AsTask());
        feedback[1].Completion!.SetResult(new(PlatformFeedbackStatus.Scheduled));
        await Check.ThrowsAsync<OperationCanceledException>(() => pending);
        await closing;
        Check.True(feedback[1].Disposals == 1 && feedback[2].Disposals == 0 && traps.All(trap => trap.Calls == 0),
            "Typed late completion or native close affected the survivor.");
        await app.StopAsync();
        Check.True(feedback[2].Disposals == 1, "Surviving feedback was not released once.");
        Console.WriteLine("PASS: direct typed feedback, codec count zero, explicit view, unsupported/error propagation, actual-completion lease and late-result rejection (headless contract).");
    }
}
