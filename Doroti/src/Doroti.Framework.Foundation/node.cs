// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/foundation/node.dart
namespace Doroti.Generated.Framework.Foundation;

public class AbstractNode
{
    private int _depth;
    private object? _owner;
    private AbstractNode? _parent;

    public int depth => _depth;

    public object? owner => _owner;

    public bool attached => _owner is not null;

    public AbstractNode? parent => _parent;

    public virtual void redepthChildren()
    {
    }

    public void redepthChild(AbstractNode child)
    {
        ArgumentNullException.ThrowIfNull(child);
        if (!ReferenceEquals(child.owner, owner))
        {
            throw new InvalidOperationException("A child and parent must have the same owner.");
        }
        if (child._depth <= _depth)
        {
            child._depth = _depth + 1;
            child.redepthChildren();
        }
    }

    public virtual void attach(object owner)
    {
        ArgumentNullException.ThrowIfNull(owner);
        if (_owner is not null)
        {
            throw new InvalidOperationException("The node is already attached.");
        }
        _owner = owner;
    }

    public virtual void detach()
    {
        if (_owner is null)
        {
            throw new InvalidOperationException("The node is not attached.");
        }
        _owner = null;
    }

    protected void adoptChild(AbstractNode child)
    {
        ArgumentNullException.ThrowIfNull(child);
        if (child._parent is not null || ReferenceEquals(child, this) || IsAncestorOf(child, this))
        {
            throw new InvalidOperationException("The child already has a parent or would create a cycle.");
        }
        child._parent = this;
        if (attached)
        {
            child.attach(_owner!);
        }
        redepthChild(child);
    }

    protected void dropChild(AbstractNode child)
    {
        ArgumentNullException.ThrowIfNull(child);
        if (!ReferenceEquals(child._parent, this))
        {
            throw new InvalidOperationException("The node is not a child of this parent.");
        }
        child._parent = null;
        if (attached)
        {
            child.detach();
        }
    }

    private static bool IsAncestorOf(AbstractNode ancestor, AbstractNode node)
    {
        for (var current = node; current is not null; current = current._parent)
        {
            if (ReferenceEquals(current, ancestor))
            {
                return true;
            }
        }
        return false;
    }
}
