using Doroti.Cupertino;
using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Testing;
using Doroti.Ui;
using SkiaSharp;
using TextStyle = Doroti.Framework.Painting.TextStyle;

internal static class TextInputTapCursorRegression
{
    internal static void Run()
    {
        CheckParagraphGeometry();
        var failures = new List<Exception>();
        foreach (var platform in new[] { HostOperatingSystem.iOS, HostOperatingSystem.android,
            HostOperatingSystem.windows, HostOperatingSystem.macOS, HostOperatingSystem.linux })
        {
            try { CheckPlatform(platform); }
            catch (Exception error) { failures.Add(error); Console.WriteLine(error.Message); }
        }
        if (failures.Count != 0) throw new AggregateException(failures);
        Console.WriteLine("PASS: multiline first focus/refocus, hard breaks, blank lines, soft wraps, Unicode, painted caret and subsequent IME insertion across iOS/Android/Windows/macOS/Linux.");
    }

    private static void CheckPlatform(HostOperatingSystem platform)
    {
        using var tester = new WidgetTester(size: new(360, 600), operatingSystem: platform);
        foreach (var text in new[] { "first line\nsecond line\nlast line", "한글🙂\n둘째 줄\n마지막",
            "first\n\nlast", "\n\nlast", "first\n\n", "single line" })
        {
            using var controller = new TextEditingController(text: text);
            using var focus = new FocusNode();
            tester.pumpWidget(new CupertinoApp(home: new CupertinoPageScaffold(child:
                new CupertinoTextField(controller: controller, focusNode: focus, maxLines: 3, cursorColor: new Color(0xffff00ff),
                    style: new TextStyle(fontSize: 24)))));
            var editable = ((EditableTextState)((StatefulElement)tester.byType<EditableText>().Single()).state).renderEditable;
            double firstLineTop;
            double firstLineCenter;
            double lineHeight;
            using (tester.View.EnterPlatformEnvironmentScope())
            {
                var firstCaret = editable.getLocalRectForCaret(new(0));
                firstLineTop = firstCaret.top;
                firstLineCenter = firstCaret.center.dy;
                lineHeight = editable.getBoxesForSelection(new TextSelection(0, 1)).First().toRect().height;
            }
            var lines = text.Split('\n');
            var offset = 0;
            for (var row = 0; row < lines.Length; row++)
            {
                if (focus.hasFocus) { focus.unfocus(); tester.pump(); }
                // Keep a previous selection at EOT when refocusing an earlier line.
                if (row > 0) tester.View.DispatchPlatformEvent(() => controller.selection = TextSelection.CreateCollapsed(text.Length));
                tester.pump(TimeSpan.FromMilliseconds(400)); // Each activation is a single tap, outside the double-tap window.
                Offset tap;
                using (tester.View.EnterPlatformEnvironmentScope())
                    tap = editable.localToGlobal(new Offset(editable.size.width - 8, firstLineCenter + row * lineHeight));
                tester.tapAt(tap);
                var expectedOffset = offset + lines[row].Length;
                Require(focus.hasFocus && tester.HasTextClient, $"{platform}: tap did not focus line {row}.");
                Require(controller.selection.extentOffset == expectedOffset && controller.selection.isCollapsed,
                    $"{platform}: tap past line {row} selected {controller.selection}, expected {expectedOffset}.");
                Require(tester.EditingState?.selection.extentOffset == expectedOffset,
                    $"{platform}: native selection differs from the controller on line {row}.");
                using (tester.View.EnterPlatformEnvironmentScope())
                {
                    var actual = editable.getLocalRectForCaret(controller.selection.extent);
                    var expectedTop = firstLineTop + row * lineHeight;
                    Require(Math.Abs(actual.top - expectedTop) < 1,
                        $"{platform}: line {row} caret expected top={expectedTop}, actual top={actual.top}.");
                    var painted = editable.localToGlobal(actual.center);
                    Require(tester.pixel((int)painted.dx, (int)painted.dy) == SKColors.Magenta,
                        $"{platform}: the visible caret was not painted at the selected position.");
                }
                // Emulate an IME inserting at its accepted selection, then restore
                // the text so each row starts from the same multiline document.
                var native = tester.EditingState!.Value;
                var inserted = native.text.Insert(native.selection.extentOffset, "한");
                tester.enterText(new(inserted, new(expectedOffset + 1, expectedOffset + 1), new(expectedOffset, expectedOffset + 1)));
                Require(controller.text == text.Insert(expectedOffset, "한") && controller.value.composing.start == expectedOffset,
                    $"{platform}: IME insertion missed the tapped line or lost composition.");
                tester.enterText(new(text, new(expectedOffset, expectedOffset), null));
                offset = expectedOffset + 1;
            }
            if (text.StartsWith("first line"))
            {
                focus.unfocus();
                tester.pump(TimeSpan.FromMilliseconds(400));
                Offset wordTap;
                using (tester.View.EnterPlatformEnvironmentScope())
                    wordTap = editable.localToGlobal(editable.getLocalRectForCaret(new(3)).center);
                tester.tapAt(wordTap);
                var expected = platform == HostOperatingSystem.iOS ? 5 : 3;
                Require(controller.selection.extentOffset == expected && tester.EditingState?.selection.extentOffset == expected,
                    $"{platform}: initial word tap diverged from Flutter's platform selection policy.");
            }
            tester.pumpWidget(new SizedBox());
            tester.pump();
        }
        CheckSoftWrap(tester, platform);
    }

    private static void CheckSoftWrap(WidgetTester tester, HostOperatingSystem platform)
    {
        var text = "first " + string.Concat(Enumerable.Repeat("iiiiWWWW한글 ", 6));
        using var controller = new TextEditingController(text: text);
        using var focus = new FocusNode();
        tester.pumpWidget(new CupertinoApp(home: new CupertinoPageScaffold(child:
            new CupertinoTextField(controller: controller, focusNode: focus, maxLines: 3,
                style: new TextStyle(fontSize: 24)))));
        var editable = ((EditableTextState)((StatefulElement)tester.byType<EditableText>().Single()).state).renderEditable;
        Offset tap;
        double firstTop;
        long expectedOffset;
        using (tester.View.EnterPlatformEnvironmentScope())
        {
            var firstCaret = editable.getLocalRectForCaret(new(0));
            firstTop = firstCaret.top;
            // iOS touch retains Flutter's word-edge policy. Precise selection
            // on other platforms also exercises the soft-wrap trailing edge.
            tap = editable.localToGlobal(platform == HostOperatingSystem.iOS
                ? editable.getLocalRectForCaret(new(3)).center
                : new Offset(editable.size.width - 1, firstCaret.center.dy));
            expectedOffset = platform == HostOperatingSystem.iOS ? 5 : editable.getPositionForPoint(tap).offset;
        }
        tester.tapAt(tap);
        Require(expectedOffset > 0 && expectedOffset < text.Length
            && controller.selection.extentOffset == expectedOffset && tester.EditingState?.selection.extentOffset == expectedOffset,
            $"{platform}: soft-wrap tap changed the selected insertion point.");
        if (platform != HostOperatingSystem.iOS)
            Require(controller.selection.affinity == TextAffinity.upstream, $"{platform}: soft-wrap tap lost its visual line affinity.");
        tester.enterText(tester.EditingState!.Value); // A native echo must preserve visual affinity.
        using (tester.View.EnterPlatformEnvironmentScope())
            Require(Math.Abs(editable.getLocalRectForCaret(controller.selection.extent).top - firstTop) < 1,
                $"{platform}: soft-wrap caret moved to the next line after activation/native echo.");
        tester.pumpWidget(new SizedBox());
        tester.pump();
    }

    private static void CheckParagraphGeometry()
    {
        const string text = "a\n\nb\n";
        using var paragraph = new Paragraph(text, 100, 20, codeUnitAdvances: [10, 0, 0, 10, 0]);
        paragraph.layout(new ParagraphConstraints(100));
        foreach (var (offset, row, x) in new[] { (1, 0, 10d), (2, 1, 0d), (4, 2, 10d) })
        {
            var glyph = paragraph.getGlyphInfoAt(offset);
            Require(glyph is not null && glyph.graphemeClusterCodeUnitRange.start == offset
                && glyph.graphemeClusterLayoutBounds == Rect.fromLTWH(x, row * 20, 0, 20),
                $"Hard break {offset} has no zero-width glyph on its own line.");
            var box = paragraph.getBoxesForRange(offset, offset + 1, BoxHeightStyle.strut).Single();
            Require(box.toRect() == Rect.fromLTWH(x, row * 20, 0, 20), "Hard-break glyph and selection box disagree.");
        }
        Require(paragraph.getGlyphInfoAt(-1) is null && paragraph.getGlyphInfoAt(text.Length) is null,
            "Out-of-range glyph positions became visible.");
        using var truncated = new Paragraph(text, 100, 20, maxLines: 1, codeUnitAdvances: [10, 0, 0, 10, 0]);
        truncated.layout(new ParagraphConstraints(100));
        Require(truncated.getGlyphInfoAt(1) is not null && truncated.getGlyphInfoAt(2) is null
            && truncated.getBoxesForRange(2, 3).Count == 0, "Hard-break geometry escaped maxLines.");
        using var wrapped = new Paragraph("abcd", 20, 20, codeUnitAdvances: [10, 10, 10, 10]);
        wrapped.layout(new ParagraphConstraints(20));
        var wrapEnd = wrapped.getPositionForOffset(new Offset(30, 10));
        Require(wrapEnd.offset == 2 && wrapEnd.affinity == TextAffinity.upstream,
            "A tap past a soft-wrapped line must retain the preceding visual line's affinity.");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
