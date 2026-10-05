using Doroti.Framework;
using Doroti.Framework.Foundation;
using Doroti.Framework.Painting;
using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Hosting;
using Doroti.Runtime;
using Doroti.Ui;
using Doroti.Testing;
using TextStyle = Doroti.Framework.Painting.TextStyle;

static class ViewFocusSelectionRegression
{
    sealed class Editor : IDisposable
    {
        public TextEditingController Controller { get; } = new("한글 입력");
        public FocusNode Focus { get; } = new();
        public Widget Widget() => new WidgetsApp(color: new Color(0xffffffff),
            textStyle: new TextStyle(color: new Color(0xff000000), fontSize: 12),
            builder: (_, _) => new EditableText(controller: Controller, focusNode: Focus,
                keyboardType: TextInputType.text,
                style: new TextStyle(color: new Color(0xff000000), fontSize: 12),
                cursorColor: new Color(0xff000000), backgroundCursorColor: new Color(0xff808080),
                selectionColor: new Color(0xffb0b0ff), selectAllOnFocus: false));
        public void Dispose() { Focus.dispose(); Controller.dispose(); }
    }

    public static void Run()
    {
        using var clock = new TestClock();
        using var time = DartAsyncRuntime.enterTimeProvider(clock);
        using var firstEditor = new Editor();
        using var secondEditor = new Editor();
        var entrypoint = new DorotiWidgetEntrypoint(views => new DorotiApplicationViewCollection(views,
            (_, view) => view.viewId == 1 ? firstEditor.Widget() : secondEditor.Widget()));
        using var session = new DorotiHostSession(entrypoint);
        using var scope = session.dispatcher.EnterScope();
        using var first = new SharedTreeRegression.Surface(session.dispatcher, 1);
        using var second = new SharedTreeRegression.Surface(session.dispatcher, 2);
        var errors = new List<Exception>();
        var previousError = FlutterError.onError;
        FlutterError.onError = details => errors.Add(details.exception as Exception ?? new Exception(details.exception.ToString()));
        try
        {
            session.Start(deferFrameworkBootstrap: true);
            session.AttachView(first.View); session.AttachView(second.View);
            first.Frame(TimeSpan.Zero); second.Frame(TimeSpan.Zero);
            if (errors.Count != 0) throw new AggregateException(errors);
            ImageCache firstCache;
            ImageCache secondCache;
            using (first.View.EnterInvocationScope()) firstCache = PaintingBinding.instance.imageCache;
            using (second.View.EnterInvocationScope())
            {
                secondCache = PaintingBinding.instance.imageCache;
                Check.Throws<InvalidOperationException>(() => PaintingBinding.instance._imageCache = firstCache);
            }
            Check.True(!ReferenceEquals(firstCache, secondCache), "Decoded images share a cache across rendering views.");
            void Pump(SharedTreeRegression.Surface surface)
            {
                using var owner = surface.View.EnterInvocationScope();
                surface.View.DispatchPlatformEvent(() => clock.Advance(TimeSpan.FromMilliseconds(16)));
                surface.Frame(clock.Elapsed);
            }
            using (first.View.EnterInvocationScope()) firstEditor.Focus.requestFocus();
            Pump(first); Pump(second);
            if (errors.Count != 0) throw new AggregateException(errors);
            Check.True(firstEditor.Focus.hasFocus && !secondEditor.Focus.hasFocus && first.Host.HasTextClient,
                $"First editor did not focus/connect independently: first={firstEditor.Focus.hasFocus}, second={secondEditor.Focus.hasFocus}, firstClient={first.Host.HasTextClient}, secondClient={second.Host.HasTextClient}.");
            first.Host.Edit(new("첫째", new(0, 2), null));
            Pump(first);
            Check.True(firstEditor.Controller.text == "첫째" && secondEditor.Controller.text == "한글 입력",
                "First view editing escaped its widget/controller.");
            var oldEdit = first.Host.CaptureEditingCallback();
            using (second.View.EnterInvocationScope()) secondEditor.Focus.requestFocus();
            Pump(second); Pump(first);
            Check.True(secondEditor.Focus.hasFocus && !firstEditor.Focus.hasFocus && second.Host.HasTextClient,
                "Focus transfer did not preserve per-view IME ownership.");
            second.Host.Edit(new("둘째", new(1, 2), new(0, 2)));
            Pump(second);
            first.Host.RequestFocus(ViewFocusState.unfocused, ViewFocusDirection.undefined);
            Pump(second);
            Check.True(secondEditor.Focus.hasFocus && second.Host.HasTextClient,
                "A delayed unfocus event from another view cleared the current editor.");
            Check.True(firstEditor.Controller.selection.baseOffset == 0 && firstEditor.Controller.selection.extentOffset == 2 &&
                secondEditor.Controller.selection.baseOffset == 1 && secondEditor.Controller.selection.extentOffset == 2,
                "Separate view selections collapsed into a shared selection.");
            session.DetachView(first.View); first.View.Dispose();
            Check.Throws<ObjectDisposedException>(() => firstCache.putIfAbsent("late-image", () => throw new InvalidOperationException("Closed cache invoked decoder.")));
            using (second.View.EnterInvocationScope())
                Check.True(ReferenceEquals(PaintingBinding.instance.imageCache, secondCache), "Primary detach replaced survivor's image cache.");
            oldEdit?.Invoke(new("늦은 입력", new(0, 0), null));
            second.Host.Edit(new("생존 창", new(2, 3), null));
            Pump(second);
            Check.True(secondEditor.Controller.text == "생존 창" && second.Host.HasTextClient && secondEditor.Focus.hasFocus,
                "Primary editor detach damaged survivor focus/editing.");
            session.ShutdownFramework();
            if (errors.Count != 0) throw new AggregateException(errors);
            Console.WriteLine("PASS: real EditableText focus transfer, isolated selection/composing/controller state, late IME callback and survivor editing after primary unmount (synthetic host input; physical/native focus remains unverified).");
        }
        finally
        {
            session.ShutdownFramework();
            FlutterError.onError = previousError;
        }
    }
}
