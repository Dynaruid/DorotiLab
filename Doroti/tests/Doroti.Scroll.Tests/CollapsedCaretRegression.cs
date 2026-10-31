using System.Reflection;
using Doroti.Framework.Painting;
using Doroti.Framework.Foundation;
using Doroti.Framework.Rendering;
using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Ui;

internal static class CollapsedCaretRegression
{
    private sealed class CustomController(string text) : TextEditingController(text);

    internal static void Run()
    {
        using var dispatcher = new PlatformDispatcher();
        using var dispatcherScope = dispatcher.EnterScope();
        using var environment = PlatformEnvironmentContext.Enter(new PlatformConfiguration([], Brightness.light, false, false,
            operatingSystem: HostOperatingSystem.android));
        var previousPainting = PaintingBinding._instance;
        var painting = DispatchProxy.Create<PaintingBinding, PaintingProxy>();
        PaintingBinding._instance = painting;
        using var restorePainting = new DisposeScope();
        restorePainting.Add(() => PaintingBinding._instance = previousPainting);
        var errors = new List<FlutterErrorDetails>();
        var oldErrorHandler = FlutterError.onError;
        FlutterError.onError = errors.Add;
        restorePainting.Add(() => FlutterError.onError = oldErrorHandler);
        var apply = typeof(EditableTextState).GetMethod("TryApplyCollapsedSelection", BindingFlags.NonPublic | BindingFlags.Static)!
            .CreateDelegate<Func<TextEditingController, TextEditingValue?, TextEditingValue, RenderEditable, bool, bool, bool>>();
        var needsLayout = typeof(RenderObject).GetProperty("_needsLayout", BindingFlags.Instance | BindingFlags.NonPublic)!;
        using var unused = new DisposeScope();
        var controller = new TextEditingController("ab😀한글");
        var previous = controller.value.copyWith(selection: TextSelection.CreateCollapsed(offset: 1));
        var next = previous.copyWith(selection: TextSelection.CreateCollapsed(offset: 4));
        var peers = Enumerable.Range(0, 3).Select(_ =>
        {
            var render = new RenderEditable(text: new TextSpan(text: previous.text), textDirection: TextDirection.ltr,
                textScaler: TextScaler.noScaling, selection: previous.selection, offset: ViewportOffset.CreateFixed(0));
            var owner = new PipelineOwner { rootNode = render };
            render.layout(BoxConstraints.CreateTight(new Size(200, 40)));
            unused.Add(() => { owner.rootNode = null; render.dispose(); });
            return render;
        }).ToArray();
        controller.value = next;
        foreach (var render in peers)
        {
            var span = render.text;
            Check(apply(controller, previous, next, render, false, false), "shared-controller peer accepts a plain collapsed move");
            Check(render.selection == next.selection && ReferenceEquals(render.text, span), "latest selection reaches the real renderer without replacing text");
            Check(!(bool)needsLayout.GetValue(render)!, "collapsed selection requests no text/ancestor layout");
        }
        var target = peers[0];
        Check(!apply(controller, next, next.copyWith(text: "changed"), target, false, false), "text edits rebuild");
        Check(!apply(controller, next, next.copyWith(composing: new TextRange(start: 0, end: 1)), target, false, false), "composing changes rebuild");
        Check(!apply(controller, next, next.copyWith(selection: new TextSelection(baseOffset: 0, extentOffset: 4)), target, false, false), "expanded selection refreshes copy/cut semantics");
        Check(!apply(controller, next, next.copyWith(selection: TextSelection.CreateCollapsed(offset: -1)), target, false, false), "invalid selections rebuild");
        Check(!apply(controller, next, next.copyWith(selection: TextSelection.CreateCollapsed(offset: 100)), target, false, false), "out-of-range selections rebuild");
        Check(!apply(controller, next, next, target, false, true), "obscured text retains the full build path");
        Check(!apply(controller, next, next, target, true, false), "focus transition refreshes the renderer properties");
        var custom = new CustomController(next.text);
        Check(!apply(custom, next, next, target, false, false), "custom spans may depend on selection and retain their build");
        custom.dispose();
        target.text = new TextSpan(text: "pending old layout");
        Check(!apply(controller, next, next, target, false, false), "a pending text build cannot take the selection path");

        // An inherited-property update can revisit a retained leaf widget later.
        // It must read the current controller, not replay an old selection snapshot.
        var type = typeof(EditableTextState).Assembly.GetType("Doroti.Framework.Widgets._Editable__editable_text")!;
        var constructor = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Single();
        var args = constructor.GetParameters().Select(p => p.Name switch
        {
            "inlineSpan" => (object)new TextSpan(text: next.text),
            "controller" => controller,
            "textScaler" => TextScaler.noScaling,
            _ => p.DefaultValue,
        }).ToArray();
        var leaf = constructor.Invoke(args);
        var value = type.GetProperty("value")!;
        controller.selection = TextSelection.CreateCollapsed(offset: 0);
        Check(ReferenceEquals(value.GetValue(leaf), controller.value), "retained leaf reads the latest editing revision");
        controller.dispose();
        Check(errors.Count == 0, "real renderer layout completes without reported framework errors");
        CheckRetainedDecorationSlot();
        Console.WriteLine("PASS: shared-controller collapsed caret paint without layout; text/composition/range/focus/custom fallbacks; retained leaf freshness");
    }

    private static void CheckRetainedDecorationSlot()
    {
        var constructor = typeof(Doroti.Material._RenderDecoration__input_decorator)
            .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Single();
        // This ownership check never lays out/paints the decoration, so no
        // decoration metrics are needed. Use the real slot/child implementation.
        var decoration = (Doroti.Material._RenderDecoration__input_decorator)constructor.Invoke(
            [null, TextDirection.ltr, TextBaseline.alphabetic, false, false, null]);
        var first = new CountingBox();
        var second = new CountingBox();
        var owner = new PipelineOwner { rootNode = decoration };
        decoration._setChild(first, Doroti.Material._DecorationSlot__input_decorator.input);
        decoration._setChild(first, Doroti.Material._DecorationSlot__input_decorator.input);
        Check(first.Attaches == 1 && first.Detaches == 0, "unchanged decoration slot retains attachment");
        decoration._setChild(second, Doroti.Material._DecorationSlot__input_decorator.input);
        Check(first.Detaches == 1 && second.Attaches == 1, "replacement still transfers ownership");
        owner.rootNode = null;
        Check(second.Detaches == 1, "final parent detach retires its current child once");
        decoration._setChild(null, Doroti.Material._DecorationSlot__input_decorator.input);
        decoration.dispose(); first.dispose(); second.dispose();
    }

    private sealed class CountingBox : RenderBox
    {
        internal int Attaches, Detaches;
        public override void attach(PipelineOwner owner) { base.attach(owner); Attaches++; }
        public override void detach() { Detaches++; base.detach(); }
    }

    private sealed class DisposeScope : IDisposable
    {
        private readonly List<Action> _actions = [];
        internal void Add(Action action) => _actions.Add(action);
        public void Dispose() { foreach (var action in _actions) action(); }
    }
    public class PaintingProxy : DispatchProxy
    {
        private readonly _SystemFontsNotifier__binding _fonts = new();
        protected override object? Invoke(MethodInfo? method, object?[]? args) => method?.Name switch
        {
            "get_systemFonts" or "get__systemFonts" => _fonts,
            _ => throw new NotSupportedException(method?.Name),
        };
    }
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
