using Doroti.Framework.Cupertino;
using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Framework.Semantics;
using Doroti.Testing;

internal static class WindowContextRegression
{
    public static void Run()
    {
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
}
