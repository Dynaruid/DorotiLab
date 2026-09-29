using Doroti.Framework.Cupertino;
using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Framework.Semantics;
using Doroti.Testing;

internal static class WindowContextRegression
{
    public static void Run()
    {
        VerifyOwnerGuards();
        using var both = new Barrier(2);
        var errors = new System.Collections.Concurrent.ConcurrentQueue<Exception>();
        var threads = Enumerable.Range(0, 2).Select(index => new Thread(() =>
        {
            try
            {
                using var tester = new WidgetTester();
                var binding = WidgetsBinding.instance;
                var keyboard = RawKeyboard.instance;
                var semantics = new CustomSemanticsAction("Window " + index);
                var semanticsId = CustomSemanticsAction.getIdentifier(semantics);
                var text = new TextEditingController();
                tester.pumpWidget(new CupertinoApp(home: new CupertinoPageScaffold(child: new CupertinoTextField(controller: text))));
                if (!both.SignalAndWait(TimeSpan.FromSeconds(20))) throw new TimeoutException();
                for (var step = 0; step < 4; step++)
                {
                    tester.tap(tester.byType<CupertinoTextField>().Single());
                    var value = $"창 {index} - {step}";
                    tester.enterText(new(value, new(value.Length, value.Length), null));
                    if (step == 2)
                    {
                        var reload = tester.reassemble();
                        tester.pump(TimeSpan.FromMilliseconds(16));
                        reload.WaitAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
                    }
                    if (!ReferenceEquals(binding, WidgetsBinding.instance) || !ReferenceEquals(keyboard, RawKeyboard.instance) || text.text != value)
                        throw new Exception("Another window replaced binding/keyboard/text state.");
                    if (!ReferenceEquals(CustomSemanticsAction.getAction(semanticsId), semantics))
                        throw new Exception("Another window replaced a custom semantics action.");
                    if (!both.SignalAndWait(TimeSpan.FromSeconds(20))) throw new TimeoutException();
                }
                tester.pumpWidget(new SizedBox());
                text.dispose();
            }
            catch (Exception error) { errors.Enqueue(error); }
        }) { IsBackground = true }).ToArray();
        foreach (var thread in threads) thread.Start();
        foreach (var thread in threads) if (!thread.Join(TimeSpan.FromSeconds(90))) throw new TimeoutException("Context teardown stalled.");
        if (!errors.IsEmpty) throw new AggregateException(errors);
        Console.WriteLine("PASS: two concurrent dispatcher contexts preserve binding, pointer focus, Korean text, reassemble, semantics and teardown (CPU).");
    }
    private static void VerifyOwnerGuards()
    {
        using var tester = new WidgetTester();
        tester.pumpWidget(new SizedBox());
        var element = tester.byType<SizedBox>().Single();
        Action[] operations = [() => tester.sendKey(default),
            () => tester.performSemanticsAction(0, Doroti.Ui.SemanticsAction.tap),
            () => tester.enterText(new("wrong owner", new(0, 0), null)),
            () => tester.tapAt(Doroti.Ui.Offset.zero), () => tester.drag(element, Doroti.Ui.Offset.zero),
            () => tester.center(element), () => tester.pixel(0, 0), () => tester.reassemble(),
            () => tester.WritePng("must-not-write.png")];
        Exception? failure = null;
        var other = new Thread(() =>
        {
            try
            {
                foreach (var operation in operations)
                {
                    try { operation(); }
                    catch (InvalidOperationException error) when (error.Message.Contains("owning thread")) { continue; }
                    throw new Exception("A WidgetTester operation bypassed owner-thread validation.");
                }
            }
            catch (Exception error) { failure = error; }
        });
        other.Start();
        if (!other.Join(TimeSpan.FromSeconds(5))) throw new TimeoutException("Owner guard check stalled.");
        if (failure is not null) throw failure;
        tester.Dispose();
        foreach (var operation in operations)
        {
            try { operation(); }
            catch (ObjectDisposedException) { continue; }
            throw new Exception("A WidgetTester operation accepted a disposed owner.");
        }
        Console.WriteLine("PASS: input, semantics, reassemble and image APIs reject wrong-thread/disposed owners before work.");
    }
}
