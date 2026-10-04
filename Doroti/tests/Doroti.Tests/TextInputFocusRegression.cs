using Doroti.Framework.Cupertino;
using Doroti.Framework.Foundation;
using Doroti.Framework.Widgets;
using Doroti.Testing;
using Doroti.Ui;

internal static class TextInputFocusRegression
{
    internal static void Run()
    {
        using var tester = new WidgetTester(operatingSystem: HostOperatingSystem.android);
        using var first = new TextEditingController();
        using var second = new TextEditingController();
        using var firstFocus = new FocusNode();
        using var secondFocus = new FocusNode();
        var background = new ValueKey<string>("text-input-background");
        tester.pumpWidget(new CupertinoApp(home: new CupertinoPageScaffold(child: new Column(children:
        [
            new CupertinoTextField(controller: first, focusNode: firstFocus),
            new CupertinoTextField(controller: second, focusNode: secondFocus),
            new SizedBox(key: background, height: 200, width: 400),
        ]))));
        tester.tap(tester.byType<CupertinoTextField>()[0]);
        tester.enterText(new("한", new(1, 1), new(0, 1)));
        tester.tap(tester.byKey(background).Single());
        Require(firstFocus.hasFocus && tester.HasTextClient, "A background touch closed the framework text client.");
        Require(first.text == "한" && first.value.selection.extentOffset == 1 && first.value.composing.end == 1,
            "A background touch changed text, selection or composition.");
        tester.enterText(new("한글 붙여넣기", new(7, 7), null));
        Require(first.text == "한글 붙여넣기", "An IME edit after background touch missed the focused field.");
        tester.tap(tester.byType<CupertinoTextField>()[1]);
        tester.enterText(new("second", new(6, 6), null));
        Require(!firstFocus.hasFocus && secondFocus.hasFocus && second.text == "second" && first.text == "한글 붙여넣기",
            "Changing text fields retained the previous editing target.");
        secondFocus.unfocus();
        tester.pump();
        Require(!tester.HasTextClient, "Explicit unfocus retained the text input connection.");
        tester.pumpWidget(new SizedBox());
        tester.pump();
        Console.WriteLine("PASS: background touch retains framework focus/composition and subsequent edits; field switch and explicit unfocus.");
    }

    private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
}
