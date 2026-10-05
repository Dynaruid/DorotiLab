using System.Text.Json;
using Doroti.Framework.Widgets;
using Doroti.Framework.Foundation;
using Doroti.Runtime;
using Doroti.Ui;
using M = Doroti.Material;
using C = Doroti.Cupertino;
internal sealed class NativeDesignPresentationProbe : StatefulWidget
{
    public override IState createState() => new ProbeState();
    private sealed class ProbeState : State<NativeDesignPresentationProbe>
    {
        private bool started;
        public override Widget build(BuildContext context)
        {
            if (!started && WindowScope.maybeOf(context) is { Kind: WindowKind.Regular })
            { started = true; WidgetsBinding.instance.addPostFrameCallback(timestamp => { _ = RunAsync(); }); }
            return new ColoredBox(color: new Color(0xff235577), child: new Text("Design presentation probe"));
        }
        private async Task RunAsync()
        {
            try
            {
                var owner = View.of(context); var windows = owner.GetCapabilityOrDefault<IWindowService>(DorotiCapabilityIds.WindowService)!;
                var cupertino = Environment.GetEnvironmentVariable("DOROTI_DESIGN_PROBE") == "cupertino";
                var nativeViews = new HashSet<ulong>(); var overlayViews = new HashSet<ulong>();
                Future<int?> Show(WindowPresentation policy, bool closeWindow = false)
                {
                    Console.Error.WriteLine($"doroti.design.show {policy} close={closeWindow}");
                    Widget Builder(BuildContext childContext) => new DialogResultContent(() =>
                    {
                        Console.Error.WriteLine($"doroti.design.result {policy} close={closeWindow}");
                        var child = View.of(childContext);
                        if (policy == WindowPresentation.Native)
                        {
                            var window = WindowScope.of(childContext);
                            if (window.Kind != WindowKind.Dialog || child.viewId == owner.viewId) throw new InvalidOperationException("Native design branch ownership failed.");
                            nativeViews.Add(child.viewId);
                        }
                        else { if (child.viewId != owner.viewId) throw new InvalidOperationException("Overlay design branch changed view."); overlayViews.Add(child.viewId); }
                        if (cupertino)
                        { if (C.CupertinoTheme.of(childContext).primaryColor.value != 0xffee7722) throw new InvalidOperationException("Cupertino captured theme changed."); }
                        else if (M.Theme.of(childContext).primaryColor.value != 0xffee7722) throw new InvalidOperationException("Material captured theme changed.");
                        if (Localizations.localeOf(childContext) != Localizations.localeOf(context)) throw new InvalidOperationException("Captured localization changed.");
                        if (closeWindow) _ = windows.CloseAsync(WindowScope.of(childContext).Id).AsTask();
                        else Navigator.of(childContext).pop(23);
                    });
                    return cupertino ? C.RouteLibrary.showCupertinoDialog<int?>(context, Builder, presentation: policy, barrierDismissible: false)
                        : M.DialogLibrary.showDialog<int?>(context, Builder, presentation: policy, barrierDismissible: false);
                }
                var nativeResult = await Show(WindowPresentation.Native);
                Console.Error.WriteLine("doroti.design.native-completed");
                var overlayResult = await Show(WindowPresentation.Overlay);
                Console.Error.WriteLine("doroti.design.overlay-completed");
                if (nativeResult != 23 || overlayResult != 23)
                    throw new InvalidOperationException("Native/Overlay design result failed.");
                if (await Show(WindowPresentation.Native, true) is not null) throw new InvalidOperationException("Native window close result failed.");
                if (windows.GetWindows().Count != 1 || nativeViews.Count != 2 || overlayViews.Count != 1)
                    throw new InvalidOperationException("Design branches did not drain independently.");
                var tooltip = new WindowRequest(WindowKind.Tooltip, "", new Size(180, 48), WindowScope.of(context).Id,
                    new WindowAnchor(WindowScope.of(context).Id, Rect.fromLTWH(20, 20, 30, 20)), Activate: false);
                var content = await Future<NativeWindowPresentation.ContentHandle>.fromTask(NativeWindowPresentation.ShowContentAsync(context,
                    tooltip, new Text("Native tooltip content")).AsTask());
                if (!windows.GetWindows().Single(value => value.Id == content.Window.Id).Visible)
                    throw new InvalidOperationException("Native tooltip returned before Show completed.");
                await Future.fromTask(Task.WhenAll(content.DisposeAsync().AsTask(), content.DisposeAsync().AsTask()));
                using var tooltipLifetime = new CancellationTokenSource();
                var canceledContent = await Future<NativeWindowPresentation.ContentHandle>.fromTask(NativeWindowPresentation.ShowContentAsync(context,
                    tooltip, new Text("Canceled tooltip"), tooltipLifetime.Token).AsTask());
                tooltipLifetime.Cancel();
                await Future.fromTask(canceledContent.DisposeAsync().AsTask());
                using var earlyClose = new CancellationTokenSource();
                void CloseDuringCreate(WindowEvent change)
                {
                    if (change.Window.Kind == WindowKind.Tooltip && !change.Window.Closing && !change.Window.Closed)
                        earlyClose.Cancel();
                }
                windows.Changed += CloseDuringCreate;
                try
                {
                    await Future<NativeWindowPresentation.ContentHandle>.fromTask(NativeWindowPresentation.ShowContentAsync(context,
                        tooltip, new Text("Never installed"), earlyClose.Token).AsTask());
                    throw new InvalidOperationException("Early tooltip cancellation was hidden as successful installation.");
                }
                catch (OperationCanceledException) when (earlyClose.IsCancellationRequested) { }
                finally { windows.Changed -= CloseDuringCreate; }
                if (windows.GetWindows().Count != 1) throw new InvalidOperationException("Tooltip content leaked an auxiliary window.");
                var output = Environment.GetEnvironmentVariable("DOROTI_DESIGN_PROBE_OUTPUT")!;
                File.WriteAllText(output, JsonSerializer.Serialize(new { schema="doroti.native-design-probe/v1", result="PASS", design=cupertino?"cupertino":"material", nativeViews=nativeViews.Select(value=>value.ToString()).ToArray(), ownerView=owner.viewId.ToString(), capturedTheme=true, capturedLocalization=true, nativeVisibleBeforeResult=true, nativeResult=23, overlayResult=23, windowCloseResult=(int?)null, tooltipVisible=true, tooltipCancellation=true, tooltipEarlyCancellation=true, tooltipConcurrentDispose=true, remainingWindows=windows.GetWindows().Count }));
                await windows.CloseAsync(WindowScope.of(context).Id);
            }
            catch (Exception error) { Console.Error.WriteLine("doroti.design.probe FAIL " + error); }
        }
    }
    private sealed class DialogResultContent(Action result) : StatefulWidget
    {
        public override IState createState() => new ContentState();
        private sealed class ContentState : State<DialogResultContent>
        {
            public override void initState() { base.initState(); WidgetsBinding.instance.addPostFrameCallback(_ => DartRuntimePrimitives.ObserveTask(CompleteWhenVisibleAsync(), "design dialog result")); }
            private async Task CompleteWhenVisibleAsync()
            {
                var window = WindowScope.of(context);
                if (window.Kind == WindowKind.Dialog)
                {
                    var windows = View.of(context).GetCapabilityOrDefault<IWindowService>(DorotiCapabilityIds.WindowService)!;
                    var visible = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                    void Changed(WindowEvent value)
                    {
                        if (value.Window.Id == window.Id && value.Window.Visible) visible.TrySetResult();
                    }
                    windows.Changed += Changed;
                    try
                    {
                        if (!windows.GetWindows().Any(value => value.Id == window.Id && value.Visible))
                            await Future.fromTask(visible.Task.WaitAsync(TimeSpan.FromSeconds(10)));
                    }
                    finally { windows.Changed -= Changed; }
                }
                if (mounted) widget.Complete();
            }
            public override Widget build(BuildContext context) => new ColoredBox(color:new Color(0xffee7722), child:new Text("Native design dialog result"));
        }
        internal Action Complete => result;
    }
}
