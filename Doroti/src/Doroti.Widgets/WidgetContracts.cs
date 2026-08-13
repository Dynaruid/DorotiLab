using Doroti.Rendering;

namespace Doroti.Widgets;

/// <summary>The native Doroti widget marker. Flutter compatibility APIs remain in a separate assembly.</summary>
public interface IWidget;

public interface IWidgetHost
{
    IWidget Root { get; set; }
}

public interface BuildContext
{
    Widget Widget { get; }

    bool Mounted { get; }

    TWidget? DependOnInheritedWidgetOfExactType<TWidget>()
        where TWidget : InheritedWidget;
}

/// <summary>
/// Widget-host key surface (includes <see cref="GlobalKey"/>).
/// Flutter public Key/ValueKey/UniqueKey behavior owner for G3-B0 is
/// <c>Doroti.Generated.Framework.Foundation</c>; this hierarchy remains the
/// Doroti widget-tree adapter until GlobalKey cutover in a later milestone.
/// </summary>
public abstract class Key;

public abstract class LocalKey : Key;

public sealed class ValueKey<T>(T value) : LocalKey, IEquatable<ValueKey<T>>
{
    public T Value { get; } = value;

    public bool Equals(ValueKey<T>? other) => other is not null && EqualityComparer<T>.Default.Equals(Value, other.Value);

    public override bool Equals(object? obj) => obj is ValueKey<T> other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(typeof(T), Value);

    public override string ToString() => $"[{typeof(T).Name} {Value}]";
}

public sealed class UniqueKey : LocalKey
{
    private readonly Guid _identity = Guid.NewGuid();

    public override string ToString() => $"[UniqueKey {_identity:N}]";
}

public class GlobalKey : Key
{
    public GlobalKey(string? debugLabel = null)
    {
        DebugLabel = debugLabel;
    }

    public string? DebugLabel { get; }

    internal Element? CurrentElement { get; set; }

    public override string ToString() => DebugLabel is null ? "[GlobalKey]" : $"[GlobalKey {DebugLabel}]";
}

public sealed class GlobalKey<TState>(string? debugLabel = null) : GlobalKey(debugLabel)
    where TState : State
{
    public TState? CurrentState => CurrentElement is StatefulElement element ? element.State as TState : null;
}

public abstract class Widget : IWidget
{
    protected Widget(Key? key = null)
    {
        Key = key;
    }

    public Key? Key { get; }

    public virtual Type IdentityType => GetType();

    public abstract Element CreateElement();

    public static bool CanUpdate(Widget oldWidget, Widget newWidget) =>
        oldWidget.IdentityType == newWidget.IdentityType && Equals(oldWidget.Key, newWidget.Key);
}

public abstract class StatelessWidget(Key? key = null) : Widget(key)
{
    public abstract Widget? Build(BuildContext context);

    public sealed override Element CreateElement() => new StatelessElement(this);
}

public abstract class StatefulWidget(Key? key = null) : Widget(key)
{
    public abstract State CreateState();

    public sealed override Element CreateElement() => new StatefulElement(this);
}

public abstract class ProxyWidget(Widget child, Key? key = null) : Widget(key)
{
    public Widget Child { get; } = child ?? throw new ArgumentNullException(nameof(child));
}

public abstract class InheritedWidget(Widget child, Key? key = null) : ProxyWidget(child, key)
{
    public abstract bool UpdateShouldNotify(InheritedWidget oldWidget);

    public sealed override Element CreateElement() => new InheritedElement(this);
}

public abstract class RenderObjectWidget(Key? key = null) : Widget(key)
{
    public abstract RenderObject CreateRenderObject(BuildContext context);

    public virtual void UpdateRenderObject(BuildContext context, RenderObject renderObject)
    {
    }

    internal IReadOnlyList<Widget> ChildWidgets => GetChildWidgets();

    protected internal abstract IReadOnlyList<Widget> GetChildWidgets();

    public sealed override Element CreateElement() => new RenderObjectElement(this);
}

public abstract class LeafRenderObjectWidget(Key? key = null) : RenderObjectWidget(key)
{
    protected internal sealed override IReadOnlyList<Widget> GetChildWidgets() => [];
}

public abstract class SingleChildRenderObjectWidget(Widget? child = null, Key? key = null) : RenderObjectWidget(key)
{
    public Widget? Child { get; } = child;

    protected internal sealed override IReadOnlyList<Widget> GetChildWidgets() => Child is null ? [] : [Child];
}

public abstract class MultiChildRenderObjectWidget(IEnumerable<Widget> children, Key? key = null) : RenderObjectWidget(key)
{
    private readonly Widget[] _children = children?.ToArray() ?? throw new ArgumentNullException(nameof(children));

    public IReadOnlyList<Widget> Children => _children;

    protected internal sealed override IReadOnlyList<Widget> GetChildWidgets() => _children;
}

public abstract class State
{
    private StatefulElement? _element;
    private bool _disposed;

    public bool Mounted => _element is not null && _element.Mounted;

    public StatefulWidget Widget => _element?.Widget as StatefulWidget
        ?? throw new InvalidOperationException("State has no widget before mount or after dispose.");

    protected internal virtual void InitState()
    {
    }

    protected internal virtual void DidUpdateWidget(StatefulWidget oldWidget)
    {
    }

    protected internal virtual void DidChangeDependencies()
    {
    }

    protected internal virtual void Activate()
    {
    }

    protected internal virtual void Deactivate()
    {
    }

    protected internal virtual void Dispose()
    {
    }

    public abstract Widget? Build(BuildContext context);

    protected void SetState(Action callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        if (_disposed)
        {
            throw new InvalidOperationException("setState() called after dispose().");
        }
        var element = _element ?? throw new InvalidOperationException("setState() called before the State was mounted.");
        element.Owner.VerifyThread();
        if (element.Owner.IsBuilding)
        {
            throw new InvalidOperationException("setState() cannot be called while BuildOwner is building.");
        }
        callback();
        element.MarkNeedsBuild();
    }

    protected void SetState(Func<Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        throw new InvalidOperationException("setState() callback must be synchronous and must not return a Task.");
    }

    internal void Attach(StatefulElement element)
    {
        if (_element is not null || _disposed)
        {
            throw new InvalidOperationException("A State instance can be mounted only once.");
        }
        _element = element;
    }

    internal void DetachAndDispose()
    {
        if (_disposed)
        {
            throw new InvalidOperationException("State.dispose() was invoked more than once.");
        }
        Dispose();
        _disposed = true;
        _element = null;
    }
}

public abstract class State<TWidget> : State
    where TWidget : StatefulWidget
{
    public new TWidget Widget => (TWidget)base.Widget;

    protected internal virtual void DidUpdateWidget(TWidget oldWidget)
    {
    }

    protected internal sealed override void DidUpdateWidget(StatefulWidget oldWidget) => DidUpdateWidget((TWidget)oldWidget);
}
