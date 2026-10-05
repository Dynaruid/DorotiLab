using Doroti.Cupertino;
using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Testing;
using Doroti.Ui;
using TextStyle = Doroti.Framework.Painting.TextStyle;

internal static class TextInputVerticalCursorRegression
{
    internal static void Run()
    {
        var collapsed = TextSelection.CreateCollapsed(6);
        var fromPosition = TextSelection.CreateFromPosition(new(6, TextAffinity.upstream));
        Require(collapsed.start == 6 && collapsed.end == 6 && collapsed.isCollapsed
            && fromPosition.start == 6 && fromPosition.end == 6 && fromPosition.isCollapsed
            && fromPosition.affinity == TextAffinity.upstream, "Collapsed selection factories left stale range bounds.");
        using var tester = new WidgetTester(size: new(360, 600), operatingSystem: HostOperatingSystem.android);
        const string line = "iiiiWWWW한글";
        var text = line + "\n짧음\n" + line;
        using var controller = new TextEditingController(text: text);
        using var focus = new FocusNode();
        tester.pumpWidget(new CupertinoApp(home: new CupertinoPageScaffold(child:
            new CupertinoTextField(controller: controller, focusNode: focus, maxLines: 3,
                style: new TextStyle(fontSize: 24)))));
        tester.tap(tester.byType<CupertinoTextField>().Single());
        SetSelection(6);
        Press(false);
        Require(controller.selection.extentOffset == line.Length + 3, "Down did not clamp to the shorter second line.");
        Press(false);
        Require(controller.selection.extentOffset == line.Length + 4 + 6, "Down lost the preferred column after a short line.");
        Press(true);
        Press(true);
        Require(controller.selection.extentOffset == 6, "Up did not restore the original column.");
        Press(true);
        Require(controller.selection.extentOffset == 0, "Up at the first line did not reach the document start.");
        SetSelection(text.Length);
        Press(false);
        Require(controller.selection.extentOffset == text.Length, "Down moved past the document end.");

        // This contains no newline: navigation must use the visible soft wraps.
        var wrapped = string.Concat(Enumerable.Repeat("iiiiWWWW한글 ", 6));
        tester.enterText(new(wrapped, new(wrapped.Length, wrapped.Length), null));
        var editable = ((EditableTextState)((StatefulElement)tester.byType<EditableText>().Single()).state).renderEditable;
        var original = Caret(new(wrapped.Length));
        Press(true);
        var upper = Caret(controller.selection.extent);
        Require(upper.top < original.top && controller.selection.extentOffset < wrapped.Length,
            "Up ignored a soft-wrapped visual line.");
        Press(false);
        Require(controller.selection.extentOffset == wrapped.Length, "Down did not return to the soft-wrapped caret.");
        Require(controller.text == wrapped && focus.hasFocus && tester.HasTextClient,
            "Vertical movement changed the text or detached the IME client.");
        tester.enterText(new(wrapped + "한", new(wrapped.Length + 1, wrapped.Length + 1), new(wrapped.Length, wrapped.Length + 1)));
        Require(controller.value.composing.start == wrapped.Length, "Composition after vertical movement was lost.");
        tester.pumpWidget(new SizedBox());
        tester.pump();
        Console.WriteLine("PASS: Android vertical caret keys; short-line column retention; document boundaries; soft wraps; subsequent composition.");

        void SetSelection(int offset) { tester.enterText(new(text, new(offset, offset), null)); }
        Rect Caret(TextPosition position)
        {
            using var scope = tester.View.EnterPlatformEnvironmentScope();
            return editable.getLocalRectForCaret(position);
        }
        void Press(bool up)
        {
            var physical = up ? PhysicalKeyboardKey.arrowUp.usbHidUsage : PhysicalKeyboardKey.arrowDown.usbHidUsage;
            var logical = up ? LogicalKeyboardKey.arrowUp.keyId : LogicalKeyboardKey.arrowDown.keyId;
            tester.sendKey(new(1, tester.Clock.Elapsed, KeyEventType.down, physical, logical, false));
            tester.sendKey(new(1, tester.Clock.Elapsed, KeyEventType.up, physical, logical, false));
            tester.pump();
        }
    }

    private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
}
