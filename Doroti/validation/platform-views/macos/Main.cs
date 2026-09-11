using AppKit;
using CoreGraphics;
using Doroti.Host.Maui;
using Doroti.Hosting;
using Doroti.Ui;
using Foundation;
using Rect = Doroti.Ui.Rect;

NSApplication.Init();
using var first = new NSWindow(new CGRect(100, 100, 600, 400), NSWindowStyle.Titled | NSWindowStyle.Resizable,
    NSBackingStore.Buffered, false) { Title = "Doroti AppKit PlatformView validation" };
using var second = new NSWindow(new CGRect(750, 100, 500, 300), NSWindowStyle.Titled,
    NSBackingStore.Buffered, false);
using var firstRoot = new FlippedView(new CGRect(0, 0, 600, 400));
using var secondRoot = new NSView(new CGRect(0, 0, 500, 300));
first.ContentView = firstRoot;
second.ContentView = secondRoot;
first.MakeKeyAndOrderFront(null);
second.OrderFront(null);
NSApplication.SharedApplication.Activate();
var dispatcher = new AppKitPlatformViewDispatcher();
var firstFactories = new PlatformViewFactoryRegistry([
    new AppKitPlatformViewFactory(() => firstRoot, false), new AppKitPlatformViewFactory(() => firstRoot, true)]);
var secondFactories = new PlatformViewFactoryRegistry([new AppKitPlatformViewFactory(() => secondRoot, false)]);
var owner = new PlatformViewCoordinator(1, "AppKit", firstFactories, dispatcher);
var other = new PlatformViewCoordinator(2, "AppKit", secondFactories, dispatcher);
var focus = new List<PlatformViewHandle>();
owner.ViewFocused += focus.Add;
try
{
    Require(!owner.QuerySupport(new(1, "doroti/native-button", PlatformViewComposition.InterleavedComposition)).Supported, "C support must be rejected");
    var interleavedFactory = new AppKitPlatformViewFactory(() => firstRoot, false, interleaved: true);
    Require(interleavedFactory.QuerySupport(new(1, "doroti/native-button", PlatformViewComposition.InterleavedComposition)).Supported,
        "C requires explicit compositor opt-in");
    Require(!interleavedFactory.QuerySupport(new(1, "doroti/native-button", PlatformViewComposition.Snapshot)).Supported,
        "C does not enable snapshot fallback");
    Require(!owner.QuerySupport(new(1, "doroti/native-button", Effects: PlatformViewEffects.Opacity)).Supported, "opacity must be rejected");
    var button = AwaitValue(owner.CreateAsync(new(1, "doroti/native-button")));
    var editor = AwaitValue(owner.CreateAsync(new(2, "doroti/native-editor")));
    var otherButton = AwaitValue(other.CreateAsync(new(1, "doroti/native-button")));
    Await(owner.AttachAsync(Place(button, 20, 30, 220, 60)));
    Await(owner.AttachAsync(Place(editor, 20, 130, 240, 60)));
    Await(other.AttachAsync(Place(otherButton, 10, 20, 180, 50)));
    Require(firstRoot.Subviews.Length == 2 && secondRoot.Subviews.Length == 1, "owner containers are isolated");
    Require(secondRoot.Subviews[0].Frame.Y == 230, "unflipped coordinate conversion");
    var nativeButton = (NSButton)firstRoot.Subviews[0].Subviews[0];
    var nativeEditor = (NSTextField)firstRoot.Subviews[1].Subviews[0];
    Await(owner.AttachAsync(Place(button, 20, 30, 220, 60) with { PaintOrder = 9 }));
    Require(nativeButton.Superview!.Layer!.ZPosition == 9, "native layer follows scene paint order");
    Require(nativeButton.Superview!.Frame.Y == 30, "flipped coordinate conversion");
    nativeButton.PerformClick(nativeButton);
    Require(nativeButton.Title == "Native clicks: 1", "native button activation delivers once");
    Await(owner.SetFocusAsync(editor, true));
    Pump();
    Require(first.FirstResponder == nativeEditor.CurrentEditor && focus.Contains(editor), "field editor focus callback");
    var fieldEditor = (NSTextView)nativeEditor.CurrentEditor!;
    fieldEditor.InsertText(new NSString("abc"), new NSRange(NSRange.NotFound, 0));
    Require(nativeEditor.StringValue.Contains("abc"), "native text insertion");
    Await(owner.SetFocusAsync(editor, false));
    Require(first.FirstResponder != fieldEditor, "clearFocus releases field editor");
    nativeButton.NextKeyView = nativeEditor;
    nativeEditor.NextKeyView = nativeButton;
    Await(owner.SetFocusAsync(button, true));
    first.SelectNextKeyView(nativeButton);
    Require(first.FirstResponder == nativeEditor.CurrentEditor, "Tab traverses into native editor");
    nativeEditor.CurrentEditor!.PerformSelector(new ObjCRuntime.Selector("insertBacktab:"), nativeEditor, 0);
    Pump();
    Require(first.FirstResponder == nativeButton, $"Shift+Tab traverses back to native button: responder={first.FirstResponder?.GetType().Name}, previous={nativeEditor.PreviousKeyView?.GetType().Name}, valid={nativeEditor.PreviousValidKeyView?.GetType().Name}, buttonEligible={nativeButton.CanBecomeKeyView}");
    var clipped = Place(button, 40, 50, 220, 60) with { Clip = Rect.fromLTWH(80, 60, 100, 30) };
    Await(owner.AttachAsync(clipped));
    Require(nativeButton.Superview!.Frame == new CGRect(80, 60, 100, 30), "rectangular clipping bounds");
    Require(nativeButton.Frame.X == -40 && nativeButton.Frame.Y == -10, "clipping preserves content origin");
    Require(firstRoot.HitTest(new CGPoint(50, 70)) != nativeButton, "clipped area cannot hit native control");
    Await(owner.AttachAsync(clipped with { Visible = false }));
    Require(nativeButton.Superview.Hidden, "hidden native view");
    Await(owner.AttachAsync(Place(button, 20, 30, 220, 60)));
    Await(owner.DetachAsync(button));
    Require(nativeButton.Superview.Superview is null, "detach removes attachment");
    Await(owner.AttachAsync(Place(button, 20, 30, 220, 60)));

    // Exercise the product planner's explicit B mode, logical scale, and rejection gates.
    using var scene = Scene(button, 2);
    using (var plan = PlatformCompositionPlanner.Build(scene, new(1, 0, 1, 1, 2, 2), owner, PlatformViewComposition.NativeOverlay))
        Require(plan.Parts.OfType<PlatformNativeSegment>().Single().Placement.Transform == PlatformViewTransform.Identity, "DPR applied once");
    Reject(() => PlatformCompositionPlanner.Build(scene, new(1, 0, 2, 1, 2, 2), owner), "C planner rejection");
    var overlap = new SceneBuilder(1);
    overlap.addPlatformView(button, new Offset(0, 0), 100, 100);
    overlap.addPlatformView(editor, new Offset(50, 50), 100, 100);
    using var overlapScene = overlap.build();
    Reject(() => PlatformCompositionPlanner.Build(overlapScene, new(1, 0, 3, 1), owner, PlatformViewComposition.NativeOverlay), "overlap rejection");
    var shield = new SceneBuilder(1);
    shield.addPlatformView(button, Offset.zero, 100, 100);
    shield.addInputShield(Rect.fromLTWH(0, 0, 100, 100), false);
    using var shieldScene = shield.build();
    Reject(() => PlatformCompositionPlanner.Build(shieldScene, new(1, 0, 4, 1), owner, PlatformViewComposition.NativeOverlay), "foreground shield rejection");

    // Removal disables input immediately but must retain the attachment until the frame releases it.
    var lease = owner.Retain(button);
    var disposal = owner.DisposeAsync(button).AsTask();
    WaitUntil(() => !nativeButton.Enabled);
    Require(!disposal.IsCompleted, "native resource waits for retirement");
    lease.Dispose(); AwaitTask(disposal);
    Await(owner.DisposeAsync(button));
    for (var i = 0; i < 100; i++)
    {
        var instance = AwaitValue(owner.CreateAsync(new(1, "doroti/native-button")));
        Require(instance.InstanceGeneration > button.InstanceGeneration, "generation increases");
        Await(owner.AttachAsync(Place(instance, 10 + i % 10, 10, 160, 50)));
        Await(owner.DisposeAsync(instance));
    }
    Require(owner.LiveInstanceCount == 1 && firstRoot.Subviews.Length == 1, "100 cycles restore native baseline");
    var background = Task.Run(async () =>
    {
        var instance = await owner.CreateAsync(new(9, "doroti/native-button"));
        await owner.AttachAsync(Place(instance, 300, 10, 150, 50));
        await owner.DisposeAsync(instance);
    });
    AwaitTask(background);
    Await(owner.DisposeAsync()); Await(other.DisposeAsync());
    Require(owner.LiveInstanceCount == 0 && other.LiveInstanceCount == 0 && firstRoot.Subviews.Length == 0 && secondRoot.Subviews.Length == 0, "owner close releases all attachments");
    Console.WriteLine("PASS AppKit NativeOverlay: two owners, flipped/unflipped points, clip/hide/detach, native button/text/focus, B planner/DPR/rejections, retirement, main-thread dispatch, 100 cycles, owner close. Physical input, Korean IME, VoiceOver and Metal C are not verified.");
}
finally
{
    Await(owner.DisposeAsync()); Await(other.DisposeAsync());
    first.Close(); second.Close();
}

static PlatformViewPlacement Place(PlatformViewHandle handle, double x, double y, double width, double height) =>
    new(handle, Rect.fromLTWH(x, y, width, height), PlatformViewTransform.Identity, null, 0);
static Scene Scene(PlatformViewHandle handle, double scale)
{
    var builder = new SceneBuilder(handle.OwnerViewId);
    builder.pushTransform(new double[] { scale, 0, 0, 0, 0, scale, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 });
    builder.addPlatformView(handle, new Offset(20, 30), 220, 60);
    builder.pop();
    return builder.build();
}
static void Reject(Func<PlatformCompositionPlan> action, string message)
{
    try { using var plan = action(); }
    catch (DorotiCapabilityException) { return; }
    throw new Exception(message);
}
static void Pump() => NSRunLoop.Main.RunUntil(NSDate.FromTimeIntervalSinceNow(.01));
static void WaitUntil(Func<bool> ready)
{
    var watch = System.Diagnostics.Stopwatch.StartNew();
    while (!ready()) { if (watch.Elapsed.TotalSeconds > 15) throw new TimeoutException("AppKit operation did not complete."); Pump(); }
}
static void AwaitTask(Task task) { WaitUntil(() => task.IsCompleted); task.GetAwaiter().GetResult(); }
static void Await(ValueTask task) => AwaitTask(task.AsTask());
static T AwaitValue<T>(ValueTask<T> task) { var result = task.AsTask(); AwaitTask(result); return result.GetAwaiter().GetResult(); }
static void Require(bool condition, string message) { if (!condition) throw new Exception(message); }
sealed class FlippedView(CGRect frame) : NSView(frame) { public override bool IsFlipped => true; }
