using Doroti.Framework;
using Doroti.Framework.Foundation;
using Doroti.Framework.Widgets;
using Doroti.Framework.Services;
using Doroti.Hosting;
using Doroti.Runtime;
using Doroti.Skia.Rendering;
using Doroti.Testing;
using Doroti.Ui;
using SkiaSharp;

static class SharedTreeRegression
{
    sealed class InputClient : TextInputClient
    {
        public TextEditingValue? currentTextEditingValue { get; private set; } = TextEditingValue.empty;
        public AutofillScope? currentAutofillScope => null;
        public void updateEditingValue(TextEditingValue value) => currentTextEditingValue = value;
        public void performAction(TextInputAction action) { }
        public void performPrivateCommand(string action, DartMap<string, object?> data) { }
        public void updateFloatingCursor(RawFloatingCursorPoint point) { }
        public void showAutocorrectionPromptRect(long start, long end) { }
        public void connectionClosed() { }
    }
    internal sealed class SceneRecorder(SkiaSceneRenderer renderer) : ISceneHostCapability
    {
        public List<DorotiSceneBuildToken?> Tokens { get; } = [];
        public GraphicsFeatureSupport Features => ((ISceneHostCapability)renderer).Features;
        public void BindOwner(DorotiSceneOwner owner) => renderer.BindOwner(owner);
        public void Submit(ulong viewId, DorotiSceneSubmission submission, DorotiUiInvocation invocation)
        {
            Tokens.Add(submission.BuildToken);
            ((ISceneHostCapability)renderer).Submit(viewId, submission, invocation);
        }
    }
    sealed class CountScope(int count, Widget child) : InheritedWidget(child: child)
    {
        public int Count { get; } = count;
        public override bool updateShouldNotify(InheritedWidget oldWidget) => Count != ((CountScope)oldWidget).Count;
    }
    sealed class SharedRoot(DorotiApplicationViews views) : StatefulWidget
    {
        public DorotiApplicationViews Views => views;
        public RootState? State { get; private set; }
        public override IState createState() => State = new RootState();
        public sealed class RootState : State<SharedRoot>
        {
            public int Count { get; private set; }
            public int Disposals { get; private set; }
            public void Increment() => setState(() => Count++);
            public override Widget build(BuildContext context) => new CountScope(Count,
                new DorotiApplicationViewCollection(widget.Views, (_, view) => new Branch(view.viewId)));
            public override void dispose() { Disposals++; base.dispose(); }
        }
    }
    sealed class Branch(ulong id) : StatelessWidget
    {
        public ulong ViewId => id;
        public override Widget build(BuildContext context)
        {
            var count = context.dependOnInheritedWidgetOfExactType<CountScope>()!.Count;
            return new ColoredBox(color: new Color(count == 0 ? 0xffff0000u : 0xff00ff00u));
        }
    }
    internal sealed class Surface : IDisposable
    {
        public TestHost Host { get; }
        public SkiaSceneRenderer Renderer { get; }
        public SKSurface Pixels { get; }
        public DorotiView View { get; }
        public SceneRecorder SceneFrames { get; }
        public Surface(PlatformDispatcher dispatcher, ulong id)
        {
            Host = new(new Size(48, 48), 1, viewId: id);
            Renderer = new(id, Host, new Color(0xffffffff), null, "SharedTree/CPU", "cpu", "cpu", false);
            SceneFrames = new(Renderer);
            View = dispatcher.RegisterView(id, new DorotiViewCapabilities("SharedTree/CPU")
                .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, Host)
                .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, Host)
                .Register<IInputHostCapability>(DorotiCapabilityIds.InputEvents, Host)
                .Register<ITextInputHostCapability>(DorotiCapabilityIds.TextInput, Host)
                .Register<IPlatformServicesHostCapability>(DorotiCapabilityIds.PlatformServices, Host)
                .Register<IPlatformEnvironmentHostCapability>(DorotiCapabilityIds.PlatformEnvironment, Host)
                .Register<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging, Host)
                .Register<ISceneHostCapability>(DorotiCapabilityIds.GraphicsScene, SceneFrames)
                .Register<IParagraphHostCapability>(DorotiCapabilityIds.GraphicsText, Renderer)
                .Register<IFontHostCapability>(DorotiCapabilityIds.GraphicsFont, Renderer)
                .Register<IImageHostCapability>(DorotiCapabilityIds.GraphicsImage, Renderer)
                .Register<ISceneRasterizationHostCapability>(DorotiCapabilityIds.GraphicsSceneSnapshot, Renderer)
                .Register<ITextureHostCapability>(DorotiCapabilityIds.GraphicsTexture, Renderer)
                .Register<ISemanticsHostCapability>(DorotiCapabilityIds.AccessibilitySemantics, Renderer));
            Pixels = SKSurface.Create(new SKImageInfo(48, 48))!;
        }
        public void Frame(TimeSpan time)
        {
            Host.Frame(time);
            if (Renderer.Paint(Pixels, 48, 48) is { } completion) Renderer.CompletePaint(completion);
        }
        public SKColor Pixel()
        {
            using var snapshot = Pixels.Snapshot();
            using var bitmap = SKBitmap.FromImage(snapshot);
            return bitmap.GetPixel(24, 24);
        }
        public void Dispose() { View.Dispose(); Renderer.Dispose(); Pixels.Dispose(); }
    }

    public static void Run()
    {
        using var clock = new TestClock();
        using var timeScope = DartAsyncRuntime.enterTimeProvider(clock);
        SharedRoot? root = null;
        var entrypoint = new DorotiWidgetEntrypoint((DorotiApplicationViews views) => root = new SharedRoot(views));
        using var session = new DorotiHostSession(entrypoint);
        using var dispatcherScope = session.dispatcher.EnterScope();
        using var first = new Surface(session.dispatcher, 1);
        using var second = new Surface(session.dispatcher, 2);
        var previousErrors = FlutterError.onError;
        var errors = new List<Exception>();
        FlutterError.onError = details => errors.Add(details.exception as Exception ?? new Exception(details.exception.ToString()));
        try
        {
            session.Start(deferFrameworkBootstrap: true);
            session.AttachView(first.View);
            session.AttachView(second.View);
            first.Frame(clock.Elapsed); second.Frame(clock.Elapsed);
            Check.True(root?.State is not null, "Application root was not created.");
            Check.True(first.Pixel() == SKColors.Red && second.Pixel() == SKColors.Red, "Initial independent CPU surfaces differ.");
            first.View.DispatchPlatformEvent(root!.State!.Increment);
            clock.Advance(TimeSpan.FromMilliseconds(16));
            first.Frame(clock.Elapsed); second.Frame(clock.Elapsed);
            Check.True(first.Pixel() == SKColors.Lime && second.Pixel() == SKColors.Lime,
                "Inherited state did not update both branches of the same logical tree.");
            Check.True(first.SceneFrames.Tokens.All(token => token?.ViewEpoch.ViewId == 1) &&
                second.SceneFrames.Tokens.All(token => token?.ViewEpoch.ViewId == 2), "Frame submission lost the originating view epoch.");
            Check.True(first.SceneFrames.Tokens.Last()!.FrameworkFrameNumber == second.SceneFrames.Tokens.Last()!.FrameworkFrameNumber,
                "Two view submissions did not belong to the same application frame.");
            var firstClient = new InputClient();
            var secondClient = new InputClient();
            TextInputConnection? firstConnection = null;
            TextInputConnection? secondConnection = null;
            first.View.DispatchPlatformEvent(() => firstConnection = TextInput.attach(firstClient, new(viewId: 1, inputType: TextInputType.text)));
            second.View.DispatchPlatformEvent(() => secondConnection = TextInput.attach(secondClient, new(viewId: 2, inputType: TextInputType.text)));
            first.Host.Edit(new("한", new(1, 1), new(0, 1)));
            second.Host.Edit(new("둘", new(1, 1), null));
            Check.True(firstClient.currentTextEditingValue?.text == "한" && secondClient.currentTextEditingValue?.text == "둘",
                "A typed IME callback crossed view ownership.");
            var lateEdit = first.Host.CaptureEditingCallback();
            second.View.DispatchPlatformEvent(firstConnection!.close);
            lateEdit?.Invoke(new("late", new(4, 4), null));
            Check.True(second.Host.HasTextClient && secondClient.currentTextEditingValue?.text == "둘",
                "Closing an old view's connection cleared or edited its survivor.");
            var state = root.State;
            var lateFrame = first.Host.PendingFrame;
            session.DetachView(first.View);
            first.View.Dispose();
            lateFrame?.Invoke(clock.Elapsed);
            second.View.DispatchPlatformEvent(state.Increment);
            clock.Advance(TimeSpan.FromMilliseconds(16));
            second.Frame(clock.Elapsed);
            Check.True(state.Count == 2 && state.Disposals == 0 && second.Pixel() == SKColors.Lime,
                "Primary detach destroyed shared state or stopped the survivor.");
            second.Host.Edit(new("둘째", new(2, 2), null));
            Check.True(secondClient.currentTextEditingValue?.text == "둘째", "Survivor text input stopped after primary detach.");
            second.View.DispatchPlatformEvent(secondConnection!.close);
            session.ShutdownFramework();
            Check.True(state.Disposals == 1, "Application root did not unmount exactly once.");
            if (errors.Count != 0) throw new AggregateException(errors);
            Console.WriteLine("PASS: shared State/InheritedWidget, two CPU RenderViews/epochs, isolated typed IME clients, old connection/late callback, primary detach/survivor frame/input and root unmount (G0 subset; native/physical IME/focus/GPU not verified).");
        }
        catch (Exception error) { Console.WriteLine(error); throw; }
        finally
        {
            try { session.ShutdownFramework(); }
            finally { FlutterError.onError = previousErrors; }
        }
    }
}
