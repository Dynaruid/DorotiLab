using Doroti.Framework.Cupertino;
using Doroti.Framework.Widgets;
using Doroti.Testing;
using Doroti.Ui;

static void Require(bool value, string message) { if (!value) throw new Exception(message); }
#if DOROTI_REPO_TESTS
if (args.FirstOrDefault() == "--full-review-process-child") { ProcessRunnerRegression.Child(args[1]); return; }
FullReviewRegression.Run();
ImageReadbackLifetimeRegression.Run();
ProcessRunnerRegression.Run();
if (Array.IndexOf(args, "--native-frame-metal-gpu") is var metalGpuIndex && metalGpuIndex >= 0)
{
    if (metalGpuIndex + 1 >= args.Length || args[metalGpuIndex + 1].StartsWith("--"))
        throw new ArgumentException("--native-frame-metal-gpu requires an output JSON path.");
    NativeFrameMetalGpuRegression.Run(args[metalGpuIndex + 1]);
    return;
}
if (Array.IndexOf(args, "--native-frame-gpu") is var nativeGpuIndex && nativeGpuIndex >= 0)
{
    NativeFrameGpuRegression.Run(args[nativeGpuIndex + 1]);
    return;
}
if (args.Contains("--shader-frame-pipeline"))
{
    ShaderFramePipelineRegression.Run();
    TextureBudgetRegression.Run();
    return;
}
if (args.Contains("--variable-blur-gpu"))
{
    VariableBlurGpuRegression.Run();
    return;
}
if (Array.IndexOf(args, "--variable-blur-quality") is var qualityIndex && qualityIndex >= 0)
{
    if (qualityIndex + 1 >= args.Length || args[qualityIndex + 1].StartsWith("--"))
        throw new ArgumentException("--variable-blur-quality requires an output directory.");
    VariableBlurGpuRegression.Run(args[qualityIndex + 1]);
    return;
}
if (args.Contains("--variable-blur-kawase"))
{
    VariableBlurKawaseRegression.Run();
    return;
}
if (args.Contains("--variable-blur-capture"))
{
    VariableBlurCaptureRegression.Run();
    return;
}
if (args.Contains("--variable-blur-kernel"))
{
    VariableBlurKernelRegression.Run();
    return;
}
#endif
for (var run = 0; run < 2; run++)
{
    using var tester = new WidgetTester();
    long selected = -1;
    var tabs = new CupertinoTabController();
    tester.pumpWidget(new CupertinoApp(home: new IgnorePointer(ignoring: args.Contains("--break-tab"), child:
        new CupertinoTabScaffold(controller: tabs,
        tabBuilder: (_, index) => new Center(child: new Text($"Page {index}")),
        tabBar: new CupertinoTabBar(items:
        [new BottomNavigationBarItem(icon: new SizedBox(width: 24, height: 24), label: "First"),
         new BottomNavigationBarItem(icon: new SizedBox(width: 24, height: 24), label: "Second")],
            onTap: index => selected = index)))));
    tester.pumpAndSettle();
    tester.tap(tester.text("Second").Single());
    Require(selected == 1 && tabs.index == 1 && tester.text("Page 1").Count == 1, "Cupertino second tab did not receive its pointer tap or change page.");
    tester.tap(tester.text("First").Single());
    Require(selected == 0 && tabs.index == 0, "Cupertino first tab did not receive its pointer tap or change page.");
    tester.pumpAndSettle();
    tester.pumpWidget(new SizedBox());
    tabs.dispose();
    using var timer = tester.Clock.CreateTimer(_ => { }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(10));
    try { tester.pumpAndSettle(timeout: TimeSpan.FromMilliseconds(40)); throw new Exception("Infinite animation/timer was accepted."); }
    catch (TimeoutException) { }
    Require(tester.Clock.Elapsed > TimeSpan.Zero, "Clock was not advanced.");
}
using (var tester = new WidgetTester())
{
    var controller = new TextEditingController();
    tester.pumpWidget(new CupertinoApp(home: new CupertinoPageScaffold(child:
        new CupertinoTextField(controller: controller))));
    tester.tap(tester.byType<CupertinoTextField>().Single());
    Require(tester.HasTextClient, "Pointer focus did not attach native text input.");
    tester.enterText(new("ㅎ", new(1, 1), new(0, 1)));
    Require(controller.text == "ㅎ" && controller.value.composing.start == 0, "Hangul composition start failed.");
    tester.enterText(new("한", new(1, 1), new(0, 1)));
    Require(controller.text == "한" && controller.value.composing.end == 1, "Hangul composition update failed.");
    tester.enterText(new("한글", new(2, 2), null));
    Require(controller.text == "한글" && controller.value.composing.isCollapsed, "Hangul commit failed.");
    tester.enterText(new("한글ㄷ", new(3, 3), new(2, 3)));
    tester.enterText(new("한글", new(0, 2), null));
    Require(controller.text == "한글" && controller.value.selection.start == 0 && controller.value.selection.end == 2,
        "Composition cancellation/selection did not preserve committed text.");
    var reload = tester.reassemble();
    tester.pump(TimeSpan.FromMilliseconds(16));
    reload.WaitAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
    Require(tester.HasTextClient && controller.text == "한글" && controller.value.selection.start == 0 && controller.value.selection.end == 2,
        "Focused input/selection was lost during reassemble.");
    tester.pumpWidget(new SizedBox());
    tester.pump();
    Require(!tester.HasTextClient, "Unmount retained native text client.");
    controller.dispose();
}
Console.WriteLine("PASS: real Cupertino pointer routing; bounded settle; serial teardown/recreation.");
Console.WriteLine("PASS: pointer focus; synthetic Hangul start/update/commit/cancel/selection; IME teardown.");
await DesktopCloseRegression.Run();
PlatformPolicyRegression.Run();
#if DOROTI_REPO_TESTS
RenderingRegressions.Run();
VariableBlurCaptureRegression.Run();
VariableBlurKernelRegression.Run();
VariableBlurKawaseRegression.Run();
ShaderFramePipelineRegression.Run();
TextureBudgetRegression.Run();
NavigationRegression.Run();
WindowContextRegression.Run();
await PlatformRetirementRegression.Run();
#endif
