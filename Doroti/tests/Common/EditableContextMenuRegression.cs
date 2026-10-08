using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Runtime;
using Doroti.Testing;
using Doroti.Ui;

internal static class EditableContextMenuRegression
{
    internal static void Run(Func<TextEditingController, FocusNode, bool, bool, Widget> build)
    {
        var originalChannel = SystemChannels.platform;
        var originalProcessChannel = SystemChannels.processText;
        var channel = new TextChannel();
        var processChannel = new ProcessChannel();
        SystemChannels.platform = channel;
        SystemChannels.processText = processChannel;
        try
        {
            foreach (var operatingSystem in new[] { HostOperatingSystem.android, HostOperatingSystem.iOS,
                HostOperatingSystem.windows, HostOperatingSystem.linux, HostOperatingSystem.macOS })
            {
                using var tester = new WidgetTester(size: new(400, 600), operatingSystem: operatingSystem);
                using var controller = new TextEditingController(text: "앞 선택한 한글 😀 뒤");
                using var focus = new FocusNode();
                tester.pumpWidget(build(controller, focus, false, false));
                var state = (EditableTextState)((StatefulElement)tester.byType<EditableText>().Single()).state;
                focus.requestFocus();
                tester.pump();
                using var scope = tester.View.EnterInvocationScope();
                Select();
                var buttons = state.contextMenuButtonItems.Select(item => item.type).ToList();
                Require(buttons.Contains(ContextMenuButtonType.share) ==
                    (operatingSystem is HostOperatingSystem.android or HostOperatingSystem.iOS), "Share platform gating changed.");
                Require(buttons.Contains(ContextMenuButtonType.lookUp) == (operatingSystem == HostOperatingSystem.iOS)
                    && buttons.Contains(ContextMenuButtonType.searchWeb) == (operatingSystem == HostOperatingSystem.iOS),
                    "Lookup/search platform gating changed.");
                Complete(state.shareSelection(SelectionChangedCause.toolbar));
                Require(channel.LastMethod == "Share.invoke" && channel.LastText == "선택한 한글 😀", "Share sent incorrect selected text.");
                state.copySelection(SelectionChangedCause.keyboard);
                var copied = Clipboard.getData(Clipboard.kTextPlain).asTask();
                CompleteTask(copied);
                Require(copied.Result?.text == "선택한 한글 😀", "Copy lost Unicode selection.");
                state.cutSelection(SelectionChangedCause.keyboard);
                tester.pump();
                Require(controller.text == "앞  뒤", "Cut removed text outside the selection.");
                Complete(state.pasteText(SelectionChangedCause.keyboard));
                Require(controller.text == "앞 선택한 한글 😀 뒤", "Paste did not restore cut text.");
                state.selectAll(SelectionChangedCause.keyboard);
                Require(controller.selection.start == 0 && controller.selection.end == controller.text.Length, "Select all missed text.");
                Select();
                var transform = state.contextMenuButtonItems.Single(item => item.label == "Test transform");
                transform.onPressed!();
                Require(processChannel.LastText == "선택한 한글 😀", "Process text received incorrect selection.");
                processChannel.Pending.SetResult("replacement");
                tester.pump();
                Require(controller.text == "앞 replacement 뒤", "Process text did not replace the selected range.");
                controller.text = "앞 선택한 한글 😀 뒤";
                Select();
                state.contextMenuButtonItems.Single(item => item.label == "Test transform").onPressed!();
                var changed = controller.text + " changed";
                controller.text = changed;
                processChannel.Pending.SetResult("replacement");
                tester.pump();
                Require(controller.text == changed, "A delayed process result overwrote newer input.");
                tester.pumpWidget(build(controller, focus, true, false));
                Select();
                Require(!state.cutEnabled && !state.pasteEnabled && state.copyEnabled, "Read-only editing actions are incorrect.");
                var before = controller.text;
                state.cutSelection(SelectionChangedCause.keyboard);
                Complete(state.pasteText(SelectionChangedCause.keyboard));
                Require(controller.text == before, "Read-only text was modified.");
                tester.pumpWidget(build(controller, focus, false, true));
                Select();
                Require(!state.copyEnabled && !state.cutEnabled && !state.shareEnabled
                    && !state.lookUpEnabled && !state.searchWebEnabled, "Password field exposed a text action.");
                tester.pumpWidget(new SizedBox());
                tester.pump();

                void Select()
                {
                    controller.selection = new TextSelection(2, 2 + "선택한 한글 😀".Length);
                    tester.pump();
                }
                void Complete(Future future) => CompleteTask(future.asTask());
                void CompleteTask(Task task)
                {
                    for (var count = 0; !task.IsCompleted && count < 30; count++) tester.pump(TimeSpan.FromMilliseconds(16));
                    Require(task.IsCompleted, "Context menu action did not complete.");
                    task.GetAwaiter().GetResult();
                }
            }
        }
        finally { SystemChannels.platform = originalChannel; SystemChannels.processText = originalProcessChannel; }
        Console.WriteLine("PASS: editable context menus on five platforms; Unicode copy/cut/paste/select-all; mobile Share; stale process results; read-only/password restrictions.");
    }

    private static void Require(bool value, string message)
    { if (!value) throw new InvalidOperationException(message); }

    private sealed class TextChannel() : MethodChannel("test/text-actions")
    {
        internal string? LastMethod;
        internal string? LastText;
        public override Future<T?> invokeMethod<T>(string method, object? arguments = null) where T : default
        {
            LastMethod = method;
            LastText = arguments as string;
            return Future<T?>.fromTask(Task.FromResult(default(T)));
        }
    }

    private sealed class ProcessChannel() : MethodChannel("test/process-text")
    {
        internal TaskCompletionSource<string?> Pending = new();
        internal string? LastText;
        public override Future<T?> invokeMethod<T>(string method, object? arguments = null) where T : default
        {
            if (method == "ProcessText.queryTextActions")
                return Future<T?>.fromTask(Task.FromResult((T?)(object)new Dictionary<string, string>
                    { ["test/transform"] = "Test transform" }));
            LastText = ((List<object>)arguments!)[1] as string;
            Pending = new();
            return Future<T?>.fromTask(Result<T>(Pending.Task));
        }
        private static async Task<T?> Result<T>(Task<string?> task) => (T?)(object?)await task;
    }
}
