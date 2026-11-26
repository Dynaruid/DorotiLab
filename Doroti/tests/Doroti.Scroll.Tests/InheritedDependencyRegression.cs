using System.Runtime.CompilerServices;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;

internal static class InheritedDependencyRegression
{
    internal static void Run()
    {
        var lookup = new RecordingElement(new TestInherited(SizedBox.CreateShrink()));
        var children = Enumerable.Range(0, 512).Select(_ => new CountedElement(new TestWidget())).ToArray();
        for (var i = 0; i < children.Length; i++) lookup.setDependencies(children[i], i);
        CountedElement.Comparisons = 0;
        for (var i = 0; i < 20; i++)
            Check(Equals(lookup.getDependencies(children[^1]), 511), "dependency aspects are retained");
        Check(CountedElement.Comparisons <= 40, "dependency lookup does not scan the entire dependent subtree");
        Console.WriteLine($"INFO: 20 lookups among 512 inherited dependents perform {CountedElement.Comparisons} element comparisons");

        var widgets = Enumerable.Range(0, 5).Select(_ => new TestWidget()).ToArray();
        var widget = new TestInherited(new Column(children: [.. widgets]));
        var focus = new FocusManager();
        var owner = new BuildOwner(focusManager: focus);
        var container = new RenderProxyBox();
        var root = new RenderObjectToWidgetAdapter<RenderBox>(container: container, child: widget).attachToRenderTree(owner);
        void Replace(Widget? child)
        {
            new RenderObjectToWidgetAdapter<RenderBox>(container: container, child: child).attachToRenderTree(owner, root);
            owner.buildScope(root);
            owner.finalizeTree();
        }
        try
        {
            VerifyNotifications(widget, widgets, Replace,
                () => owner.buildScope(widget.Element, () => widget.Element.notifyClients(widget)));
        }
        finally
        {
            Replace(null);
            root.deactivate();
            root.unmount();
            focus.rootScope.dispose();
        }
        Console.WriteLine("PASS: inherited dependency lookup, aspect updates, stable notification snapshots, deactivation and removed-owner collection");
    }

    private static void VerifyNotifications(TestInherited widget, TestWidget[] widgets,
        Action<Widget> replace, Action notify)
    {
        var inherited = widget.Element;
        var children = widgets.Select(child => child.Element).ToArray();
        // Updates keep notification order. Removal and re-registration append
        // once, and notification uses a snapshot when a callback mutates it.
        foreach (var child in children) inherited.removeDependent(child);
        var a = children[0];
        var b = children[1];
        var c = children[2];
        var d = children[3];
        inherited.setDependencies(a, "before");
        inherited.setDependencies(b, null);
        inherited.setDependencies(c, new object());
        inherited.setDependencies(a, "after");
        inherited.removeDependent(b);
        inherited.removeDependent(b);
        inherited.setDependencies(b, "re-added");
        inherited.OnNotify = element =>
        {
            if (ReferenceEquals(element, a))
            {
                inherited.removeDependent(c);
                inherited.setDependencies(d, null);
            }
        };
        notify();
        Check(inherited.Notified.SequenceEqual([a, c, b]), "notification retains insertion order and its original snapshot");
        Check(Equals(inherited.getDependencies(a), "after") && inherited.getDependencies(c) is null,
            "aspect updates and removals remain visible");
        inherited.OnNotify = null;
        inherited.Notified.Clear();
        notify();
        Check(inherited.Notified.SequenceEqual([a, b, d]), "callback registration takes effect on the next notification");

        var mountedDependency = children[4];
        mountedDependency.dependOnInheritedElement(inherited);
        inherited.setDependencies(mountedDependency, "mounted");
        replace(new TestInherited(new Column(children: [.. widgets.Take(4)])));
        Check(inherited.getDependencies(mountedDependency) is null, "unmount removes the inherited dependency");
        inherited.Notified.Clear();
        notify();
        Check(!inherited.Notified.Contains(mountedDependency), "element deactivation unregisters its inherited dependency");

        var removed = AddAndRemove(inherited);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        Check(!removed.IsAlive, "removed dependents are not retained by lookup or notification order");
        GC.KeepAlive(inherited);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference AddAndRemove(InheritedElement inherited)
    {
        var element = new CountedElement(new TestWidget());
        inherited.setDependencies(element, null);
        inherited.removeDependent(element);
        return new WeakReference(element);
    }

    private sealed class TestInherited(Widget child) : InheritedWidget(child: child)
    {
        internal RecordingElement Element { get; private set; } = null!;
        public override InheritedElement createElement() => Element = new(this);
        public override bool updateShouldNotify(InheritedWidget oldWidget) => true;
    }

    private sealed class RecordingElement(TestInherited widget) : InheritedElement(widget)
    {
        internal readonly List<Element> Notified = [];
        internal Action<Element>? OnNotify;
        public override void notifyDependent(InheritedWidget oldWidget, Element dependent)
        {
            Notified.Add(dependent);
            OnNotify?.Invoke(dependent);
        }
    }

    private sealed class TestWidget : StatelessWidget
    {
        internal CountedElement Element { get; private set; } = null!;
        public override StatelessElement createElement() => Element = new(this);
        public override Widget build(BuildContext context)
        {
            context.dependOnInheritedWidgetOfExactType<TestInherited>();
            return SizedBox.CreateShrink();
        }
    }

    private sealed class CountedElement(TestWidget widget) : StatelessElement(widget)
    {
        internal static int Comparisons;
        public override bool Equals(object? obj) { Comparisons++; return ReferenceEquals(this, obj); }
        public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
