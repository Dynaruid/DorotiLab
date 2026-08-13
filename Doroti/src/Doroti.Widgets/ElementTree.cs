using Doroti.Rendering;

namespace Doroti.Widgets;

public enum BuildPhase
{
    Idle,
    Building,
    Finalizing,
}

public enum ElementLifecycle
{
    Initial,
    Active,
    Inactive,
    Defunct,
}

public sealed record WidgetTraceEvent(long Sequence, string Event, string Element, int Depth, string Detail);

public sealed class WidgetBuildException : InvalidOperationException
{
    internal WidgetBuildException(Element element, Exception innerException)
        : base($"Build failed for {element}: {innerException.Message}", innerException)
    {
        Element = element;
    }

    public Element Element { get; }
}

public sealed class BuildOwner
{
    private readonly int _threadId = Environment.CurrentManagedThreadId;
    private readonly HashSet<Element> _dirty = [];
    private readonly List<Element> _inactiveRoots = [];
    private readonly Dictionary<GlobalKey, Element> _globalKeys = new(ReferenceEqualityComparer.Instance);
    private readonly HashSet<GlobalKey> _globalKeyReservations = new(ReferenceEqualityComparer.Instance);
    private readonly List<WidgetTraceEvent> _trace = [];
    private long _sequence;

    public BuildOwner(Action? onBuildScheduled = null)
    {
        OnBuildScheduled = onBuildScheduled;
    }

    public Action? OnBuildScheduled { get; }

    public BuildPhase Phase { get; private set; }

    public bool IsBuilding => Phase == BuildPhase.Building;

    public Element? RootElement { get; private set; }

    public RenderObject? RootRenderObject => RootElement?.FindFirstRenderObject();

    public IReadOnlyList<WidgetTraceEvent> Trace => _trace;

    public int DirtyElementCount => _dirty.Count;

    public int InactiveElementCount => _inactiveRoots.Count;

    public int GlobalKeyReservationCount => _globalKeyReservations.Count;

    public Element Mount(Widget root)
    {
        ArgumentNullException.ThrowIfNull(root);
        VerifyThread();
        if (RootElement is not null)
        {
            throw new InvalidOperationException("BuildOwner already has a root Element.");
        }
        var element = root.CreateElement();
        RootElement = element;
        try
        {
            element.Mount(this, null, null);
            BuildScope();
            return element;
        }
        catch
        {
            if (element.Lifecycle == ElementLifecycle.Active)
            {
                Deactivate(element);
                FinalizeTree();
            }
            RootElement = null;
            throw;
        }
    }

    public Element UpdateRoot(Widget root)
    {
        ArgumentNullException.ThrowIfNull(root);
        VerifyThread();
        var oldRoot = RootElement ?? throw new InvalidOperationException("BuildOwner has no mounted root Element.");
        if (Widget.CanUpdate(oldRoot.Widget, root))
        {
            oldRoot.Update(root);
            BuildScope();
            return oldRoot;
        }

        Deactivate(oldRoot);
        var replacement = root.CreateElement();
        try
        {
            replacement.Mount(this, null, null);
            RootElement = replacement;
            BuildScope();
            return replacement;
        }
        catch
        {
            RemoveInactive(oldRoot);
            oldRoot.Activate(null, null);
            if (replacement.Lifecycle == ElementLifecycle.Active)
            {
                Deactivate(replacement);
            }
            FinalizeTree();
            RootElement = oldRoot;
            throw;
        }
    }

    public void UnmountRoot()
    {
        VerifyThread();
        if (RootElement is not { } root)
        {
            return;
        }
        if (root.Lifecycle == ElementLifecycle.Active)
        {
            Deactivate(root);
        }
        FinalizeTree();
        RootElement = null;
        _dirty.Clear();
    }

    public void BuildScope()
    {
        VerifyThread();
        if (Phase != BuildPhase.Idle)
        {
            throw new InvalidOperationException($"BuildScope cannot start during {Phase}.");
        }
        Phase = BuildPhase.Building;
        try
        {
            while (_dirty.Count > 0)
            {
                var element = FlutterF3DirtyElementOrder.Next(_dirty);
                _dirty.Remove(element);
                if (!element.Mounted || !element.Dirty)
                {
                    continue;
                }
                try
                {
                    element.Rebuild();
                }
                catch (Exception exception) when (exception is not WidgetBuildException)
                {
                    _dirty.Add(element);
                    throw new WidgetBuildException(element, exception);
                }
            }
        }
        finally
        {
            Phase = BuildPhase.Idle;
        }
    }

    public void FinalizeTree()
    {
        VerifyThread();
        if (Phase != BuildPhase.Idle)
        {
            throw new InvalidOperationException($"FinalizeTree cannot start during {Phase}.");
        }
        Phase = BuildPhase.Finalizing;
        try
        {
            foreach (var element in _inactiveRoots.OrderByDescending(item => item.Depth).ToArray())
            {
                element.Unmount();
            }
            _inactiveRoots.Clear();
            _globalKeyReservations.Clear();
        }
        finally
        {
            Phase = BuildPhase.Idle;
        }
        if (_inactiveRoots.Count != 0 || _globalKeyReservations.Count != 0)
        {
            throw new InvalidOperationException("Inactive Elements or GlobalKey reservations survived FinalizeTree.");
        }
    }

    internal void VerifyThread()
    {
        if (Environment.CurrentManagedThreadId != _threadId)
        {
            throw new InvalidOperationException("Widget tree mutation must occur on the BuildOwner UI thread.");
        }
    }

    internal void ScheduleBuildFor(Element element)
    {
        VerifyThread();
        if (element.Lifecycle != ElementLifecycle.Active)
        {
            throw new InvalidOperationException("Only active Elements can be scheduled for build.");
        }
        if (_dirty.Add(element))
        {
            element.DirtySequence = ++_sequence;
            OnBuildScheduled?.Invoke();
            Record("dirty", element, "scheduled");
        }
    }

    internal void Deactivate(Element element)
    {
        VerifyThread();
        element.Deactivate();
        if (!_inactiveRoots.Contains(element))
        {
            _inactiveRoots.Add(element);
        }
    }

    internal void RemoveInactive(Element element) => _inactiveRoots.Remove(element);

    internal void Unschedule(Element element) => _dirty.Remove(element);

    internal Element? RetakeInactive(GlobalKey key, Element? parent, object? slot)
    {
        VerifyThread();
        if (!_globalKeys.TryGetValue(key, out var element) || element.Lifecycle != ElementLifecycle.Inactive)
        {
            return null;
        }
        if (!_inactiveRoots.Remove(element))
        {
            throw new InvalidOperationException("A nested inactive GlobalKey subtree cannot be moved independently.");
        }
        if (!_globalKeyReservations.Add(key))
        {
            throw new InvalidOperationException($"GlobalKey {key} was reserved more than once in one build scope.");
        }
        element.Activate(parent, slot);
        Record("global-key", element, "retake");
        return element;
    }

    internal void RegisterGlobalKey(GlobalKey key, Element element)
    {
        VerifyThread();
        if (_globalKeys.TryGetValue(key, out var current) && !ReferenceEquals(current, element))
        {
            throw new InvalidOperationException($"Duplicate GlobalKey {key} for {current} and {element}.");
        }
        _globalKeys[key] = element;
        key.CurrentElement = element;
    }

    internal void UnregisterGlobalKey(GlobalKey key, Element element)
    {
        if (_globalKeys.TryGetValue(key, out var current) && ReferenceEquals(current, element))
        {
            _globalKeys.Remove(key);
            key.CurrentElement = null;
        }
    }

    internal void Record(string eventName, Element element, string detail) =>
        _trace.Add(new(++_sequence, eventName, element.ToString(), element.Depth, detail));
}

public abstract class Element : BuildContext
{
    private readonly HashSet<InheritedElement> _dependencies = [];

    protected Element(Widget widget)
    {
        Widget = widget ?? throw new ArgumentNullException(nameof(widget));
    }

    public Widget Widget { get; private set; }

    public BuildOwner Owner { get; private set; } = null!;

    public Element? Parent { get; private set; }

    public object? Slot { get; private set; }

    public int Depth { get; private set; }

    public bool Dirty { get; private set; } = true;

    public bool Mounted => Lifecycle == ElementLifecycle.Active;

    public ElementLifecycle Lifecycle { get; private set; }

    public abstract IReadOnlyList<Element> Children { get; }

    internal long DirtySequence { get; set; }

    internal void Mount(BuildOwner owner, Element? parent, object? slot)
    {
        if (Lifecycle != ElementLifecycle.Initial)
        {
            throw new InvalidOperationException("An Element can be mounted only from its initial lifecycle state.");
        }
        Owner = owner;
        Parent = parent;
        Slot = slot;
        Depth = parent is null ? 1 : parent.Depth + 1;
        Lifecycle = ElementLifecycle.Active;
        try
        {
            RegisterKey();
            Owner.Record("mount", this, SlotDetail(slot));
            FirstMount();
            MarkNeedsBuild();
        }
        catch
        {
            UnregisterKey();
            Lifecycle = ElementLifecycle.Defunct;
            Parent = null;
            Slot = null;
            throw;
        }
    }

    public virtual void Update(Widget newWidget)
    {
        Owner.VerifyThread();
        if (!Widget.CanUpdate(Widget, newWidget))
        {
            throw new InvalidOperationException("Element.Update requires the same runtime type and Key.");
        }
        var oldWidget = Widget;
        Widget = newWidget;
        Owner.Record("update", this, SlotDetail(Slot));
        OnWidgetUpdated(oldWidget);
        MarkNeedsBuild();
    }

    public void MarkNeedsBuild()
    {
        Owner.VerifyThread();
        if (Lifecycle != ElementLifecycle.Active)
        {
            throw new InvalidOperationException("Cannot dirty an inactive or defunct Element.");
        }
        if (!Dirty)
        {
            Dirty = true;
        }
        Owner.ScheduleBuildFor(this);
    }

    public TWidget? DependOnInheritedWidgetOfExactType<TWidget>()
        where TWidget : InheritedWidget
    {
        Owner.VerifyThread();
        for (var ancestor = Parent; ancestor is not null; ancestor = ancestor.Parent)
        {
            if (ancestor is InheritedElement inherited && inherited.Widget is TWidget widget)
            {
                _dependencies.Add(inherited);
                inherited.AddDependent(this);
                Owner.Record("dependency", this, typeof(TWidget).Name);
                return widget;
            }
        }
        return null;
    }

    internal void Rebuild()
    {
        if (!Dirty || Lifecycle != ElementLifecycle.Active)
        {
            return;
        }
        Owner.Record("build", this, "begin");
        PerformRebuild();
        Dirty = false;
        Owner.Record("build", this, "end");
    }

    internal void Deactivate()
    {
        if (Lifecycle != ElementLifecycle.Active)
        {
            throw new InvalidOperationException("Only an active Element can be deactivated.");
        }
        ClearDependencies();
        OnDeactivate();
        foreach (var child in Children)
        {
            child.Deactivate();
        }
        Lifecycle = ElementLifecycle.Inactive;
        Dirty = false;
        Owner.Unschedule(this);
        Owner.Record("deactivate", this, SlotDetail(Slot));
    }

    internal void Activate(Element? parent, object? slot)
    {
        if (Lifecycle != ElementLifecycle.Inactive)
        {
            throw new InvalidOperationException("Only an inactive Element can be activated.");
        }
        Parent = parent;
        Slot = slot;
        SetDepth(parent is null ? 1 : parent.Depth + 1);
        Lifecycle = ElementLifecycle.Active;
        OnActivate();
        foreach (var child in Children)
        {
            child.Activate(this, child.Slot);
        }
        Owner.Record("activate", this, SlotDetail(slot));
        MarkNeedsBuild();
    }

    internal void Unmount()
    {
        if (Lifecycle != ElementLifecycle.Inactive)
        {
            throw new InvalidOperationException("Only an inactive Element can be unmounted.");
        }
        foreach (var child in Children.Reverse())
        {
            child.Unmount();
        }
        OnUnmount();
        UnregisterKey();
        Lifecycle = ElementLifecycle.Defunct;
        Owner.Record("unmount", this, SlotDetail(Slot));
        Parent = null;
        Slot = null;
    }

    internal RenderObject? FindFirstRenderObject()
    {
        if (this is RenderObjectElement renderElement)
        {
            return renderElement.RenderObject;
        }
        foreach (var child in Children)
        {
            if (child.FindFirstRenderObject() is { } renderObject)
            {
                return renderObject;
            }
        }
        return null;
    }

    internal IReadOnlyList<RenderObject> FindDirectRenderObjects()
    {
        if (this is RenderObjectElement renderElement)
        {
            return [renderElement.RenderObject];
        }
        return Children.SelectMany(child => child.FindDirectRenderObjects()).ToArray();
    }

    protected Element? UpdateChild(Element? child, Widget? newWidget, object? newSlot)
    {
        if (newWidget is null)
        {
            if (child is not null)
            {
                Owner.Deactivate(child);
            }
            return null;
        }
        if (child is not null && Widget.CanUpdate(child.Widget, newWidget))
        {
            child.UpdateSlot(newSlot);
            child.Update(newWidget);
            return child;
        }
        if (newWidget.Key is GlobalKey globalKey && Owner.RetakeInactive(globalKey, this, newSlot) is { } retaken)
        {
            if (!Widget.CanUpdate(retaken.Widget, newWidget))
            {
                throw new InvalidOperationException($"GlobalKey {globalKey} was reused with a different Widget type.");
            }
            retaken.Update(newWidget);
            return retaken;
        }

        if (child is not null)
        {
            Owner.Deactivate(child);
        }
        var replacement = newWidget.CreateElement();
        try
        {
            replacement.Mount(Owner, this, newSlot);
            return replacement;
        }
        catch
        {
            if (child is not null)
            {
                Owner.RemoveInactive(child);
                child.Activate(this, child.Slot);
            }
            throw;
        }
    }

    protected IReadOnlyList<Element> UpdateChildren(IReadOnlyList<Element> oldChildren, IReadOnlyList<Widget> newWidgets)
    {
        var result = new Element[newWidgets.Count];
        var used = new HashSet<Element>(ReferenceEqualityComparer.Instance);
        var newlyActivated = new List<Element>();
        try
        {
            for (var index = 0; index < newWidgets.Count; index++)
            {
                var widget = newWidgets[index];
                var match = FlutterF3ElementDiff.Match(oldChildren, used, widget, index);
                result[index] = UpdateChild(match, widget, index)!;
                used.Add(result[index]);
                if (!oldChildren.Contains(result[index], ReferenceEqualityComparer.Instance))
                {
                    newlyActivated.Add(result[index]);
                }
            }
            foreach (var obsolete in oldChildren.Where(item => !used.Contains(item)))
            {
                Owner.Deactivate(obsolete);
            }
            return result;
        }
        catch
        {
            foreach (var element in newlyActivated.Where(item => item.Lifecycle == ElementLifecycle.Active).Reverse<Element>())
            {
                Owner.Deactivate(element);
            }
            throw;
        }
    }

    protected void NotifyRenderStructureChanged()
    {
        for (var ancestor = Parent; ancestor is not null; ancestor = ancestor.Parent)
        {
            if (ancestor is RenderObjectElement renderElement)
            {
                renderElement.SyncRenderChildren();
                return;
            }
        }
    }

    protected virtual void FirstMount()
    {
    }

    protected virtual void OnWidgetUpdated(Widget oldWidget)
    {
    }

    protected virtual void OnDeactivate()
    {
    }

    protected virtual void OnActivate()
    {
    }

    protected virtual void OnUnmount()
    {
    }

    protected virtual void DidChangeDependencies() => MarkNeedsBuild();

    protected abstract void PerformRebuild();

    public override string ToString() => $"{GetType().Name}({Widget.GetType().Name}{(Widget.Key is null ? string.Empty : $", {Widget.Key}")})";

    internal void NotifyDependencyChanged()
    {
        if (Mounted)
        {
            DidChangeDependencies();
        }
    }

    private void UpdateSlot(object? slot)
    {
        if (!Equals(Slot, slot))
        {
            Slot = slot;
            Owner.Record("slot", this, SlotDetail(slot));
        }
    }

    private void SetDepth(int depth)
    {
        Depth = depth;
        foreach (var child in Children)
        {
            child.SetDepth(depth + 1);
        }
    }

    private void ClearDependencies()
    {
        foreach (var dependency in _dependencies)
        {
            dependency.RemoveDependent(this);
        }
        _dependencies.Clear();
    }

    private void RegisterKey()
    {
        if (Widget.Key is GlobalKey key)
        {
            Owner.RegisterGlobalKey(key, this);
        }
    }

    private void UnregisterKey()
    {
        if (Widget.Key is GlobalKey key)
        {
            Owner.UnregisterGlobalKey(key, this);
        }
    }

    private static string SlotDetail(object? slot) => slot is null ? "root" : $"slot={slot}";
}

public abstract class ComponentElement(Widget widget) : Element(widget)
{
    private Element? _child;

    public override IReadOnlyList<Element> Children => _child is null ? [] : [_child];

    protected abstract Widget? Build();

    protected override void PerformRebuild()
    {
        _child = UpdateChild(_child, Build(), 0);
        NotifyRenderStructureChanged();
    }
}

public sealed class StatelessElement(StatelessWidget widget) : ComponentElement(widget)
{
    protected override Widget? Build() => ((StatelessWidget)Widget).Build(this);
}

public sealed class StatefulElement(StatefulWidget widget) : ComponentElement(widget)
{
    public State State { get; private set; } = null!;

    protected override void FirstMount()
    {
        State = ((StatefulWidget)Widget).CreateState() ?? throw new InvalidOperationException("StatefulWidget.CreateState returned null.");
        State.Attach(this);
        try
        {
            Owner.Record("state", this, "initState");
            State.InitState();
            Owner.Record("state", this, "didChangeDependencies");
            State.DidChangeDependencies();
        }
        catch
        {
            Owner.Record("state", this, "dispose-after-mount-failure");
            State.DetachAndDispose();
            throw;
        }
    }

    protected override Widget? Build() => State.Build(this);

    protected override void OnWidgetUpdated(Widget oldWidget)
    {
        Owner.Record("state", this, "didUpdateWidget");
        State.DidUpdateWidget((StatefulWidget)oldWidget);
    }

    protected override void DidChangeDependencies()
    {
        Owner.Record("state", this, "didChangeDependencies");
        State.DidChangeDependencies();
        base.DidChangeDependencies();
    }

    protected override void OnDeactivate()
    {
        Owner.Record("state", this, "deactivate");
        State.Deactivate();
    }

    protected override void OnActivate()
    {
        Owner.Record("state", this, "activate");
        State.Activate();
    }

    protected override void OnUnmount()
    {
        Owner.Record("state", this, "dispose");
        State.DetachAndDispose();
    }
}

public sealed class InheritedElement(InheritedWidget widget) : ComponentElement(widget)
{
    private readonly HashSet<Element> _dependents = new(ReferenceEqualityComparer.Instance);

    internal void AddDependent(Element dependent) => _dependents.Add(dependent);

    internal void RemoveDependent(Element dependent) => _dependents.Remove(dependent);

    protected override Widget Build() => ((InheritedWidget)Widget).Child;

    protected override void OnWidgetUpdated(Widget oldWidget)
    {
        if (((InheritedWidget)Widget).UpdateShouldNotify((InheritedWidget)oldWidget))
        {
            foreach (var dependent in _dependents.OrderBy(item => item.Depth).ToArray())
            {
                Owner.Record("dependency", dependent, "changed");
                dependent.NotifyDependencyChanged();
            }
        }
    }
}

public sealed class RenderObjectElement(RenderObjectWidget widget) : Element(widget)
{
    private IReadOnlyList<Element> _children = [];

    public RenderObject RenderObject { get; private set; } = null!;

    public override IReadOnlyList<Element> Children => _children;

    protected override void FirstMount()
    {
        RenderObject = ((RenderObjectWidget)Widget).CreateRenderObject(this)
            ?? throw new InvalidOperationException("RenderObjectWidget.CreateRenderObject returned null.");
        Owner.Record("render-object", this, "create");
    }

    protected override void PerformRebuild()
    {
        var widget = (RenderObjectWidget)Widget;
        widget.UpdateRenderObject(this, RenderObject);
        _children = UpdateChildren(_children, widget.ChildWidgets);
        SyncRenderChildren();
    }

    protected override void OnUnmount()
    {
        SyncRenderChildren([]);
        if (RenderObject is IDisposable disposable)
        {
            disposable.Dispose();
        }
        Owner.Record("render-object", this, "dispose-adapter");
    }

    internal void SyncRenderChildren() => SyncRenderChildren(_children.SelectMany(child => child.FindDirectRenderObjects()).ToArray());

    private void SyncRenderChildren(IReadOnlyList<RenderObject> desired)
    {
        switch (RenderObject)
        {
            case RenderProxyBox proxy:
                if (desired.Count > 1 || desired.Any(item => item is not RenderBox))
                {
                    throw new InvalidOperationException($"{Widget.GetType().Name} requires zero or one RenderBox child.");
                }
                proxy.Child = desired.Count == 0 ? null : (RenderBox)desired[0];
                break;
            case IRenderObjectChildContainer container:
                container.SetChildren(desired);
                break;
            default:
                if (desired.Count != 0)
                {
                    throw new InvalidOperationException($"{RenderObject.DebugName} does not accept render children.");
                }
                break;
        }
        NotifyRenderStructureChanged();
    }
}
