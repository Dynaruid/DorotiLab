using Android.App;
using Android.OS;
using AndroidX.Core.View;
using Doroti.Ui;
using Microsoft.Maui.Dispatching;
using NativeAction = Android.Views.Accessibility.Action;
using Rect = Doroti.Ui.Rect;

namespace Doroti.Host.Maui;

[Activity(Label = "Semantics provider contracts", MainLauncher = true, Exported = true)]
public sealed class MainActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        DispatcherProvider.SetCurrent(new InlineProvider());
        var host = new DorotiAndroidVulkanView(this);
        SetContentView(host);
        host.PostDelayed(() =>
        {
            var result = "";
            try { result = MauiAndroidSemanticsBridge.Verify(host); }
            catch (Exception error) { result = "FAIL\n" + error; }
            File.WriteAllText(System.IO.Path.Combine(ExternalCacheDir!.AbsolutePath, "result.txt"), result);
            Android.Util.Log.Info("DorotiSemanticsContract", result);
        }, 300);
    }
}

// The production provider source is tested against a real Android View and
// AndroidX node provider. Vulkan and app startup are deliberately unnecessary.
internal sealed class DorotiAndroidVulkanView(Android.Content.Context context) : Android.Views.View(context)
{
    internal double SemanticsDensity => 2;
    internal AndroidX.CustomView.Widget.ExploreByTouchHelper? SemanticsHelper { get; set; }
}

internal sealed partial class MauiAndroidSemanticsBridge
{
    internal static string Verify(DorotiAndroidVulkanView host)
    {
        using var bridge = new MauiAndroidSemanticsBridge(new Microsoft.Maui.Controls.ContentView());
        bridge._native = host;
        bridge._helper = new VirtualNodes(host, bridge);
        host.SemanticsHelper = bridge._helper;
        ViewCompat.SetAccessibilityDelegate(host, bridge._helper);
        var provider = bridge._helper.GetAccessibilityNodeProvider(host)!;
        var checks = new List<string>();
        var calls = new List<(int, SemanticsAction, object?)>();
        var enabled = new SemanticsFlags(isEnabled: Tristate.isTrue);
        SemanticsNodeUpdate[] nodes = [
            new(0, Rect.fromLTWH(0, 0, 300, 600), null, null, SemanticsAction.none, [1,2,3,4,5,6,7,8,9,10]),
            new(1, Rect.fromLTWH(10, 20, 100, 30), "Plain label", null, SemanticsAction.none, []),
            new(2, Rect.fromLTWH(10, 60, 100, 30), "Button", null, SemanticsAction.tap | SemanticsAction.showOnScreen, [], enabled),
            new(3, Rect.fromLTWH(10, 100, 100, 30), "Check", null, SemanticsAction.tap, [], enabled with { isChecked = CheckedState.isFalse }),
            new(4, Rect.fromLTWH(10, 140, 100, 30), "Switch", null, SemanticsAction.tap, [], enabled with { isToggled = Tristate.isTrue }),
            new(5, Rect.fromLTWH(10, 180, 100, 30), "Edit", "hello", SemanticsAction.setText | SemanticsAction.setSelection | SemanticsAction.focus |
                SemanticsAction.moveCursorForwardByCharacter | SemanticsAction.moveCursorBackwardByWord, [],
                enabled with { isTextField = true }, textSelectionBase: 1, textSelectionExtent: 4),
            new(6, Rect.fromLTWH(10, 220, 100, 30), "Range", "0.25", SemanticsAction.increase | SemanticsAction.decrease, [],
                enabled with { isSlider = true }, minValue: "0", maxValue: "1"),
            new(7, Rect.fromLTWH(10, 260, 100, 30), "Scroll", null, SemanticsAction.scrollUp | SemanticsAction.scrollDown, [], enabled),
            new(8, Rect.fromLTWH(10, 300, 100, 30), "Disabled", null, SemanticsAction.tap, [], enabled with { isEnabled = Tristate.isFalse }),
            new(9, Rect.fromLTWH(10, 340, 100, 30), "Hidden", null, SemanticsAction.none, [10], enabled with { isHidden = true }),
            new(10, Rect.fromLTWH(10, 380, 100, 30), "Hidden child", null, SemanticsAction.tap, [], enabled),
        ];
        void Require(bool value, string message) { if (!value) throw new InvalidOperationException(message); checks.Add(message); }
        void Project(long generation) => bridge.Update(new(generation, nodes), (id, action, value) => calls.Add((id, action, value)));
        Project(1);
        using var root = provider.CreateAccessibilityNodeInfo(-1)!;
        Require(root.ChildCount == 8 && !bridge._nodes.ContainsKey(10), "visible virtual children exclude structural/hidden subtrees");
        Require(bridge.Diagnostics.ElementsCreated == 0 && bridge.Diagnostics.RetainedNodes == 9, "no native controls per semantic node");
        using var label = provider.CreateAccessibilityNodeInfo(1)!;
        Require(label.ClassName == "android.view.View" && !label.Checkable && label.ContentDescription == "Plain label", "null flags stay a plain label");
        using var bounds = new Android.Graphics.Rect(); label.GetBoundsInScreen(bounds);
        var origin = new int[2]; host.GetLocationOnScreen(origin);
        Require(bounds.Left == origin[0] + 20 && bounds.Top == origin[1] + 40 && bounds.Width() == 200, "logical bounds map to native screen pixels");
        using var check = provider.CreateAccessibilityNodeInfo(3)!;
        using var toggle = provider.CreateAccessibilityNodeInfo(4)!;
        using var edit = provider.CreateAccessibilityNodeInfo(5)!;
        using var slider = provider.CreateAccessibilityNodeInfo(6)!;
        Require(check.Checkable && !check.Checked && toggle.Checked, "checkbox and toggle state retained");
        Require(edit.Editable && edit.Text == "hello" && edit.TextSelectionStart == 1 && edit.TextSelectionEnd == 4,
            $"editable value and selection retained: editable={edit.Editable}, text={edit.Text}, start={edit.TextSelectionStart}, end={edit.TextSelectionEnd}");
        Require(slider.RangeInfo?.Current == .25f, "slider exposes native range info");
        Require(provider.PerformAction(2, (int)NativeAction.Click, null) && calls.Count == 1 && calls[0].Item2 == SemanticsAction.tap, "accessibility click reaches framework exactly once");
        using var text = new Bundle(); text.PutString("ACTION_ARGUMENT_SET_TEXT_CHARSEQUENCE", "changed");
        Require(provider.PerformAction(5, (int)NativeAction.SetText, text) && Equals(calls[^1].Item3, "changed"), "setText forwards text payload");
        using var selection = new Bundle(); selection.PutInt("ACTION_ARGUMENT_SELECTION_START_INT", 2); selection.PutInt("ACTION_ARGUMENT_SELECTION_END_INT", 5);
        Require(provider.PerformAction(5, (int)NativeAction.SetSelection, selection) && calls[^1].Item3 is Dictionary<string,long> d && d["base"] == 2 && d["extent"] == 5, "selection payload forwarded");
        using var movement = new Bundle(); movement.PutInt("ACTION_ARGUMENT_MOVEMENT_GRANULARITY_INT", 2); movement.PutBoolean("ACTION_ARGUMENT_EXTEND_SELECTION_BOOLEAN", true);
        Require(provider.PerformAction(5, (int)NativeAction.PreviousAtMovementGranularity, movement) && calls[^1].Item2 == SemanticsAction.moveCursorBackwardByWord && Equals(calls[^1].Item3, true), "word movement and selection extension forwarded");
        Require(provider.PerformAction(6, (int)NativeAction.ScrollForward, null) && calls[^1].Item2 == SemanticsAction.increase, "range increment mapping");
        Require(provider.PerformAction(7, (int)NativeAction.ScrollBackward, null) && calls[^1].Item2 == SemanticsAction.scrollDown, "scroll direction mapping");
        Require(!provider.PerformAction(8, (int)NativeAction.Click, null) && !provider.PerformAction(2, (int)NativeAction.SetText, text), "disabled and unsupported actions rejected");
        var count = calls.Count;
        nodes[3] = nodes[3] with { flags = enabled with { isChecked = CheckedState.isTrue } };
        Project(2);
        using var updated = provider.CreateAccessibilityNodeInfo(3)!;
        Require(updated.Checked && !check.Checked && calls.Count == count, "projection preserves snapshots and emits no synthetic input");
        Project(3);
        Require(bridge.Diagnostics.UpdatesSuppressed == 1, "unchanged projection suppressed");
        bridge._helper.RequestKeyboardFocusForVirtualView(3);
        nodes[3] = nodes[3] with { flags = enabled with { isHidden = true } };
        Project(4);
        Require(bridge._helper.KeyboardFocusedVirtualViewId == AndroidX.CustomView.Widget.ExploreByTouchHelper.InvalidId &&
            !provider.PerformAction(3, (int)NativeAction.Click, null), "removed node loses focus and rejects stale actions");
        bridge.Clear();
        Require(bridge.Diagnostics.RetainedNodes == 0 && !provider.PerformAction(2, (int)NativeAction.Click, null), "clear releases nodes and callbacks");
        bridge.Dispose();
        Require(host.SemanticsHelper is null, "detach releases native delegate");
        return "PASS\n" + string.Join("\n", checks);
    }
}

sealed class InlineProvider : IDispatcherProvider
{
    public IDispatcher GetForCurrentThread() => new InlineDispatcher();
}
sealed class InlineDispatcher : IDispatcher
{
    public bool IsDispatchRequired => false;
    public bool Dispatch(Action action) { action(); return true; }
    public bool DispatchDelayed(TimeSpan delay, Action action) { action(); return true; }
    public IDispatcherTimer CreateTimer() => throw new NotSupportedException();
}
