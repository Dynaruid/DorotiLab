// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/focus_manager.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public static partial class Focus_managerLibrary
{
    public static bool debugFocusChanges = false;
}

public static partial class Focus_managerLibrary
{
    internal static bool _focusDebug(
        Func<string> messageFunc,
        Func<IEnumerable<object>>? detailsFunc = null
    )
    {
        if (Foundation.ConstantsLibrary.kReleaseMode)
        {
            throw new InvalidOperationException(
                "_focusDebug was called in Release mode. It should always be wrapped in "
                    + "an assert. Always call _focusDebug like so:\n"
                    + "  assert(_focusDebug(() => 'Blah $foo'));"
            );
        }
        if (!debugFocusChanges)
        {
            return true;
        }
        PrintLibrary.debugPrint($"FOCUS: {messageFunc()}");
        IEnumerable<object> details = detailsFunc is null
            ? new List<object>()
            : detailsFunc.Invoke();
        if (Enumerable.Any(details))
        {
            foreach (var detail in details)
            {
                PrintLibrary.debugPrint($"    {detail}");
            }
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public enum KeyEventResult
{
    handled,
    ignored,
    skipRemainingHandlers,
}

public static partial class Focus_managerLibrary
{
    public static KeyEventResult combineKeyEventResults(IEnumerable<KeyEventResult> results)
    {
        var hasSkipRemainingHandlers = false;
        foreach (var result in results)
        {
            switch (result)
            {
                case KeyEventResult.handled:
                {
                    return KeyEventResult.handled;
                }
                case KeyEventResult.skipRemainingHandlers:
                {
                    hasSkipRemainingHandlers = true;
                    break;
                }
                case KeyEventResult.ignored:
                {
                    break;
                }
            }
        }
        return hasSkipRemainingHandlers
            ? KeyEventResult.skipRemainingHandlers
            : KeyEventResult.ignored;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public delegate KeyEventResult FocusOnKeyCallback(FocusNode node, RawKeyEvent @event);

public delegate KeyEventResult FocusOnKeyEventCallback(FocusNode node, KeyEvent @event);

public delegate KeyEventResult OnKeyEventCallback(KeyEvent @event);

internal class _Autofocus__focus_manager
{
    public virtual FocusScopeNode scope { get; private set; } = default!;
    public virtual FocusNode autofocusNode { get; private set; } = default!;

    internal _Autofocus__focus_manager(FocusScopeNode scope, FocusNode autofocusNode)
    {
        this.scope = scope;
        this.autofocusNode = autofocusNode;
    }

    public virtual void applyIfValid(FocusManager manager)
    {
        bool shouldApply =
            (
                (scope.parent is not null)
                || DartRuntimePrimitives.Identical(scope, manager.rootScope)
            )
            && DartRuntimePrimitives.Identical(scope._manager, manager)
            && (scope.focusedChild is null)
            && autofocusNode.ancestors.contains(scope);
        if (shouldApply)
        {
            DartRuntimePrimitives.Assert(() =>
                Focus_managerLibrary._focusDebug(() => $"Applying autofocus: {autofocusNode}")
            );
            autofocusNode._doRequestFocus(findFirstFocus: true);
        }
        else
        {
            DartRuntimePrimitives.Assert(() =>
                Focus_managerLibrary._focusDebug(() =>
                    $"Autofocus request discarded for node: {autofocusNode}."
                )
            );
        }
    }
}

public class FocusAttachment
{
    internal virtual FocusNode _node { get; private set; } = default!;

    public FocusAttachment(FocusNode _node)
    {
        this._node = _node;
    }

    public virtual bool isAttached =>
        DartRuntimePrimitives.ConvertValue<bool>(Equals(_node._attachment, this));

    public virtual void detach()
    {
        DartRuntimePrimitives.Assert(() =>
            Focus_managerLibrary._focusDebug(
                () => "Detaching node:",
                () => new List<object> { _node, $"With enclosing scope {_node.enclosingScope}" }
            )
        );
        if (isAttached)
        {
            if (
                _node.hasPrimaryFocus
                || ((_node._manager is not null) && Equals(_node._manager!._markedForFocus, _node))
            )
            {
                _node.unfocus(disposition: UnfocusDisposition.previouslyFocusedChild);
            }
            _node._manager?._markDetached(_node);
            _node._parent?._removeChild(_node);
            _node._attachment = null;
            DartRuntimePrimitives.Assert(
                () => !_node.hasPrimaryFocus,
                () =>
                    (object?)
                        $"Node {(object?)_node.debugLabel ?? (object?)_node} still has primary focus while being detached."
            );
            DartRuntimePrimitives.Assert(
                () => !Equals(_node._manager?._markedForFocus, _node),
                () =>
                    (object?)
                        $"Node {(object?)_node.debugLabel ?? (object?)_node} still marked for focus while being detached."
            );
        }
        DartRuntimePrimitives.Assert(() => !isAttached);
    }

    public virtual void reparent(FocusNode? parent = null)
    {
        if (isAttached)
        {
            DartRuntimePrimitives.Assert(() => _node.context is not null);
            parent ??= Focus.maybeOf(_node.context!, scopeOk: true);
            parent ??= _node.context!.owner!.focusManager.rootScope;
            parent._reparent(_node);
        }
    }
}

public enum UnfocusDisposition
{
    scope,
    previouslyFocusedChild,
}

public class FocusNode : ChangeNotifier, DiagnosticableTree
{
    internal virtual bool _skipTraversal { get; set; } = default!;
    internal virtual bool _canRequestFocus { get; set; } = default!;
    internal virtual bool _descendantsAreFocusable { get; set; } = default!;
    internal virtual bool _descendantsAreTraversable { get; set; } = default!;
    internal virtual BuildContext? _context { get; set; } = default;
    public virtual Func<FocusNode, RawKeyEvent, KeyEventResult>? onKey { get; set; } = default;
    public virtual Func<FocusNode, KeyEvent, KeyEventResult>? onKeyEvent { get; set; } = default;
    internal virtual FocusManager? _manager { get; set; } = default;
    internal virtual List<FocusNode>? _ancestors { get; set; } = default;
    internal virtual List<FocusNode>? _descendants { get; set; } = default;
    internal virtual bool _hasKeyboardToken { get; set; } = false;
    internal virtual FocusNode? _parent { get; set; } = default;
    internal virtual List<FocusNode> _children { get; private set; } = new List<FocusNode>();
    internal virtual string? _debugLabel { get; set; } = default;
    internal virtual FocusAttachment? _attachment { get; set; } = default;
    internal virtual FocusScopeNode? _enclosingScope { get; set; } = default;
    internal virtual bool _requestFocusWhenReparented { get; set; } = false;

    public FocusNode(
        string? debugLabel = null,
        Func<FocusNode, RawKeyEvent, KeyEventResult>? onKey = null,
        Func<FocusNode, KeyEvent, KeyEventResult>? onKeyEvent = null,
        bool skipTraversal = false,
        bool canRequestFocus = true,
        bool descendantsAreFocusable = true,
        bool descendantsAreTraversable = true
    )
    {
        this.onKey = onKey;
        this.onKeyEvent = onKeyEvent;
        _skipTraversal = skipTraversal;
        _canRequestFocus = canRequestFocus;
        _descendantsAreFocusable = descendantsAreFocusable;
        _descendantsAreTraversable = descendantsAreTraversable;
    }

    public virtual bool skipTraversal
    {
        get
        {
            if (_skipTraversal || _isInKeptAliveSliver)
            {
                return true;
            }
            foreach (FocusNode ancestor in ancestors)
            {
                if (!ancestor.descendantsAreTraversable)
                {
                    return true;
                }
            }
            return false;
        }
        set
        {
            var __value = value;
            if ((__value) != _skipTraversal)
            {
                _skipTraversal = (__value);
                _manager?._markPropertiesChanged(this);
            }
        }
    }
    public virtual bool canRequestFocus
    {
        get => _canRequestFocus && ancestors.All(_allowDescendantsToBeFocused);
        set
        {
            var __value = value;
            if ((__value) != _canRequestFocus)
            {
                _canRequestFocus = (__value);
                if (hasFocus && !(__value))
                {
                    unfocus(disposition: UnfocusDisposition.previouslyFocusedChild);
                }
                _manager?._markPropertiesChanged(this);
            }
        }
    }
    internal bool _isInKeptAliveSliver =>
        KeptAliveSliverVisibility.IsHidden(context?.findRenderObject());

    internal static bool _allowDescendantsToBeFocused(FocusNode ancestor) =>
        ancestor.descendantsAreFocusable;

    public virtual bool descendantsAreFocusable
    {
        get => _descendantsAreFocusable;
        set
        {
            var __value = value;
            if ((__value) == _descendantsAreFocusable)
            {
                return;
            }
            _descendantsAreFocusable = (__value);
            if (!(__value) && hasFocus)
            {
                unfocus(disposition: UnfocusDisposition.previouslyFocusedChild);
            }
            _manager?._markPropertiesChanged(this);
        }
    }
    public virtual bool descendantsAreTraversable
    {
        get => _descendantsAreTraversable;
        set
        {
            var __value = value;
            if ((__value) != _descendantsAreTraversable)
            {
                _descendantsAreTraversable = (__value);
                _manager?._markPropertiesChanged(this);
            }
        }
    }
    public virtual BuildContext? context => _context;
    public virtual FocusNode? parent => _parent;
    public virtual IEnumerable<FocusNode> children =>
        DartRuntimePrimitives.ConvertValue<IEnumerable<FocusNode>>(_children);
    public virtual IEnumerable<FocusNode> traversalChildren
    {
        get
        {
            if (!descendantsAreFocusable)
            {
                return Enumerable.Empty<FocusNode>();
            }
            return children.where((node) => !node.skipTraversal && node.canRequestFocus);
        }
    }
    public virtual string? debugLabel
    {
        get => _debugLabel;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() =>
            {
                _debugLabel = __value;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    public virtual IEnumerable<FocusNode> descendants
    {
        get
        {
            if (_descendants is null)
            {
                var result = new List<FocusNode>();
                foreach (FocusNode child in _children)
                {
                    result.AddRange(child.descendants.Cast<FocusNode>());
                    result.Add(child);
                }
                _descendants = result;
            }
            return _descendants!;
        }
    }
    public virtual IEnumerable<FocusNode> traversalDescendants
    {
        get
        {
            if (!descendantsAreFocusable)
            {
                return Enumerable.Empty<FocusNode>();
            }
            return descendants.where((node) => !node.skipTraversal && node.canRequestFocus);
        }
    }
    public virtual IEnumerable<FocusNode> ancestors
    {
        get
        {
            if (_ancestors is null)
            {
                var result = new List<FocusNode>();
                FocusNode? parent = _parent;
                while (parent is not null)
                {
                    result.Add(parent);
                    parent = parent._parent;
                }
                _ancestors = result;
            }
            return _ancestors!;
        }
    }
    public virtual bool hasFocus =>
        DartRuntimePrimitives.ConvertValue<bool>(
            hasPrimaryFocus || (_manager?.primaryFocus?.ancestors.contains(this) ?? false)
        );
    public virtual bool hasPrimaryFocus =>
        DartRuntimePrimitives.ConvertValue<bool>(Equals(_manager?.primaryFocus, this));
    public virtual FocusHighlightMode highlightMode => FocusManager.instance.highlightMode;
    public virtual FocusScopeNode? nearestScope => enclosingScope;

    internal virtual void _clearEnclosingScopeCache()
    {
        FocusScopeNode? cachedScope = _enclosingScope;
        if (cachedScope is null)
        {
            return;
        }
        _enclosingScope = null;
        if (Enumerable.Any(children))
        {
            foreach (FocusNode child in children)
            {
                if (DartRuntimePrimitives.Identical(cachedScope, child._enclosingScope))
                {
                    child._clearEnclosingScopeCache();
                }
            }
        }
    }

    public virtual FocusScopeNode? enclosingScope
    {
        get
        {
            FocusScopeNode? enclosingScope = _enclosingScope ??= parent?.nearestScope;
            DartRuntimePrimitives.Assert(
                () => Equals(enclosingScope, parent?.nearestScope),
                () =>
                    (object?)
                        $"{this} has invalid scope cache: {_enclosingScope} != {parent?.nearestScope}"
            );
            return enclosingScope;
        }
    }
    public virtual Size size => DartRuntimePrimitives.ConvertValue<Size>(rect.size);
    public virtual Offset offset
    {
        get
        {
            DartRuntimePrimitives.Assert(
                () => context is not null,
                () =>
                    (object?)
                        "Tried to get the offset of a focus node that didn't have its context set yet.\n"
                    + "The context needs to be set before trying to evaluate traversal policies. "
                    + "Setting the context is typically done with the attach method."
            );
            RenderObject @object = context!.findRenderObject()!;
            return MatrixUtils.transformPoint(
                @object.getTransformTo(null),
                @object.semanticBounds.topLeft
            );
        }
    }
    public virtual Rect rect
    {
        get
        {
            DartRuntimePrimitives.Assert(
                () => context is not null,
                () =>
                    (object?)
                        "Tried to get the bounds of a focus node that didn't have its context set yet.\n"
                    + "The context needs to be set before trying to evaluate traversal policies. "
                    + "Setting the context is typically done with the attach method."
            );
            RenderObject @object = context!.findRenderObject()!;
            Offset topLeftLocal = MatrixUtils.transformPoint(
                @object.getTransformTo(null),
                @object.semanticBounds.topLeft
            );
            Offset bottomRightLocal = MatrixUtils.transformPoint(
                @object.getTransformTo(null),
                @object.semanticBounds.bottomRight
            );
            return Rect.fromLTRB(
                topLeftLocal.dx,
                topLeftLocal.dy,
                bottomRightLocal.dx,
                bottomRightLocal.dy
            );
        }
    }

    public virtual void unfocus(UnfocusDisposition disposition = UnfocusDisposition.scope)
    {
        if (!hasFocus && ((_manager is null) || (!Equals(_manager!._markedForFocus, this))))
        {
            return;
        }
        FocusScopeNode? scopeLocal = enclosingScope;
        if (scopeLocal is null)
        {
            return;
        }
        switch (disposition)
        {
            case UnfocusDisposition.scope:
            {
                if (scopeLocal.canRequestFocus)
                {
                    scopeLocal._focusedChildren.Clear();
                }
                while (!scopeLocal!.canRequestFocus)
                {
                    scopeLocal = scopeLocal.enclosingScope ?? _manager?.rootScope;
                }
                scopeLocal._doRequestFocus(findFirstFocus: false);
                break;
            }
            case UnfocusDisposition.previouslyFocusedChild:
            {
                if (scopeLocal.canRequestFocus)
                {
                    scopeLocal._focusedChildren.Remove(this);
                }
                while (!scopeLocal!.canRequestFocus)
                {
                    scopeLocal.enclosingScope?._focusedChildren.Remove(scopeLocal);
                    scopeLocal = scopeLocal.enclosingScope ?? _manager?.rootScope;
                }
                scopeLocal._doRequestFocus(findFirstFocus: true);
                break;
            }
        }
        DartRuntimePrimitives.Assert(() =>
            Focus_managerLibrary._focusDebug(
                () => "Unfocused node:",
                () =>
                    new List<object>
                    {
                        $"primary focus was {this}",
                        $"next focus will be {_manager?._markedForFocus}",
                    }
            )
        );
    }

    public virtual bool consumeKeyboardToken()
    {
        if (!_hasKeyboardToken)
        {
            return false;
        }
        _hasKeyboardToken = false;
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _markNextFocus(FocusNode newFocus)
    {
        if (_manager is not null)
        {
            _manager!._markNextFocus(this);
            return;
        }
        newFocus._setAsFocusedChildForScope();
        newFocus._notify();
        if (!Equals(newFocus, this))
        {
            _notify();
        }
    }

    internal virtual void _removeChild(FocusNode node, bool removeScopeFocus = true)
    {
        DartRuntimePrimitives.Assert(
            () => _children.Contains(node),
            () => (object?)"Tried to remove a node that wasn't a child."
        );
        DartRuntimePrimitives.Assert(() => Equals(node._parent, this));
        DartRuntimePrimitives.Assert(() => Equals(node._manager, _manager));
        if (removeScopeFocus)
        {
            FocusScopeNode? nodeScope = node.enclosingScope;
            if (nodeScope is not null)
            {
                nodeScope._focusedChildren.Remove(node);
                node.descendants.where(
                        (descendant) =>
                        {
                            return Equals(descendant.enclosingScope, nodeScope);
                            throw new InvalidOperationException(
                                "Dart closure completed without a value."
                            );
                        }
                    )
                    .forEach(
                        (__arg0) =>
                        {
                            _ = nodeScope._focusedChildren.Remove(
                                DartRuntimePrimitives.ConvertValue<FocusScopeNode>(__arg0)
                            );
                        }
                    );
            }
        }
        node._parent = null;
        node._clearEnclosingScopeCache();
        _children.Remove(node);
        foreach (FocusNode ancestor in ancestors)
        {
            ancestor._descendants = null;
        }
        _descendants = null;
        DartRuntimePrimitives.Assert(() =>
            (_manager is null) || !_manager!.rootScope.descendants.contains(node)
        );
    }

    internal virtual void _updateManager(FocusManager? manager)
    {
        _manager = manager;
        foreach (FocusNode descendant in descendants)
        {
            descendant._manager = manager;
            descendant._ancestors = null;
        }
    }

    internal virtual void _reparent(FocusNode child)
    {
        DartRuntimePrimitives.Assert(
            () => !Equals(child, this),
            () => (object?)"Tried to make a child into a parent of itself."
        );
        if (Equals(child._parent, this))
        {
            DartRuntimePrimitives.Assert(
                () => _children.Contains(child),
                () =>
                    (object?)
                        "Found a node that says it's a child, but doesn't appear in the child list."
            );
            return;
        }
        DartRuntimePrimitives.Assert(
            () => (_manager is null) || (!Equals(child, _manager!.rootScope)),
            () => (object?)"Reparenting the root node isn't allowed."
        );
        DartRuntimePrimitives.Assert(
            () => !ancestors.contains(child),
            () =>
                (object?)
                    "The supplied child is already an ancestor of this node. Loops are not allowed."
        );
        FocusScopeNode? oldScopeLocal = child.enclosingScope;
        bool hadFocus = child.hasFocus;
        child._parent?._removeChild(child, removeScopeFocus: !Equals(oldScopeLocal, nearestScope));
        _children.Add(child);
        child._parent = this;
        child._ancestors = null;
        child._updateManager(_manager);
        foreach (FocusNode ancestor in child.ancestors)
        {
            ancestor._descendants = null;
        }
        if (hadFocus)
        {
            _manager?.primaryFocus?._setAsFocusedChildForScope();
        }
        if (
            (oldScopeLocal is not null)
            && (child.context is not null)
            && (!Equals(child.enclosingScope, oldScopeLocal))
        )
        {
            FocusTraversalGroup
                .maybeOf(child.context!)
                ?.changedScope(node: child, oldScope: oldScopeLocal);
        }
        if (child._requestFocusWhenReparented)
        {
            child._doRequestFocus(findFirstFocus: true);
            child._requestFocusWhenReparented = false;
        }
    }

    public virtual FocusAttachment attach(
        BuildContext? context,
        Func<FocusNode, KeyEvent, KeyEventResult>? onKeyEvent = null,
        Func<FocusNode, RawKeyEvent, KeyEventResult>? onKey = null
    )
    {
        _context = context;
        this.onKey = onKey ?? this.onKey;
        this.onKeyEvent = onKeyEvent ?? this.onKeyEvent;
        _attachment = new FocusAttachment(this);
        return _attachment!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        _attachment?.detach();
        base.dispose();
    }

    internal virtual void _notify()
    {
        if (_parent is null)
        {
            return;
        }
        if (hasPrimaryFocus)
        {
            _setAsFocusedChildForScope();
        }
        notifyListeners();
    }

    public virtual void requestFocus(FocusNode? node = null)
    {
        if (node is not null)
        {
            if (node._parent is null)
            {
                _reparent(node);
            }
            DartRuntimePrimitives.Assert(
                () => node.ancestors.contains(this),
                () =>
                    (object?)
                        "Focus was requested for a node that is not a descendant of the scope from which it was requested."
            );
            node._doRequestFocus(findFirstFocus: true);
            return;
        }
        _doRequestFocus(findFirstFocus: true);
    }

    internal virtual void _doRequestFocus(bool findFirstFocus)
    {
        if (!canRequestFocus)
        {
            DartRuntimePrimitives.Assert(() =>
                Focus_managerLibrary._focusDebug(() =>
                    $"Node NOT requesting focus because canRequestFocus is false: {this}"
                )
            );
            return;
        }
        if (_parent is null)
        {
            _requestFocusWhenReparented = true;
            return;
        }
        _setAsFocusedChildForScope();
        if (
            hasPrimaryFocus
            && ((_manager!._markedForFocus is null) || Equals(_manager!._markedForFocus, this))
        )
        {
            return;
        }
        _hasKeyboardToken = true;
        DartRuntimePrimitives.Assert(() =>
            Focus_managerLibrary._focusDebug(() => $"Node requesting focus: {this}")
        );
        _markNextFocus(this);
    }

    internal virtual void _setAsFocusedChildForScope()
    {
        var scopeFocus = this;
        foreach (FocusScopeNode ancestor in ancestors.OfType<FocusScopeNode>())
        {
            DartRuntimePrimitives.Assert(
                () => !Equals(scopeFocus, ancestor),
                () => (object?)"Somehow made a loop by setting focusedChild to its scope."
            );
            DartRuntimePrimitives.Assert(() =>
                Focus_managerLibrary._focusDebug(
                    () => $"Setting {scopeFocus} as focused child for scope:",
                    () => new List<object> { ancestor }
                )
            );
            var focusedChildren = ancestor._focusedChildren;
            lock (focusedChildren)
            {
                focusedChildren.RemoveAll(child => ReferenceEquals(child, scopeFocus));
                focusedChildren.Add(scopeFocus);
            }
            scopeFocus = DartRuntimePrimitives.ConvertValue<FocusNode>(ancestor);
        }
    }

    public virtual bool nextFocus() => FocusTraversalGroup.of(context!).next(this);

    public virtual bool previousFocus() => FocusTraversalGroup.of(context!).previous(this);

    public virtual bool focusInDirection(TraversalDirection direction) =>
        FocusTraversalGroup.of(context!).inDirection(this, direction);

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(
            new DiagnosticsProperty<BuildContext>("context", context, defaultValue: null)
        );
        properties.add(
            new FlagProperty(
                "descendantsAreFocusable",
                value: descendantsAreFocusable,
                ifFalse: "DESCENDANTS UNFOCUSABLE",
                defaultValue: true
            )
        );
        properties.add(
            new FlagProperty(
                "descendantsAreTraversable",
                value: descendantsAreTraversable,
                ifFalse: "DESCENDANTS UNTRAVERSABLE",
                defaultValue: true
            )
        );
        properties.add(
            new FlagProperty(
                "canRequestFocus",
                value: canRequestFocus,
                ifFalse: "NOT FOCUSABLE",
                defaultValue: true
            )
        );
        properties.add(
            new FlagProperty(
                "hasFocus",
                value: hasFocus && !hasPrimaryFocus,
                ifTrue: "IN FOCUS PATH",
                defaultValue: false
            )
        );
        properties.add(
            new FlagProperty(
                "hasPrimaryFocus",
                value: hasPrimaryFocus,
                ifTrue: "PRIMARY FOCUS",
                defaultValue: false
            )
        );
    }

    IEnumerable<DiagnosticsNode> DiagnosticableTree.debugDescribeChildren() =>
        debugDescribeChildren();

    public virtual List<DiagnosticsNode> debugDescribeChildren()
    {
        var count = 1L;
        return _children
            .map(
                (child) =>
                {
                    return ((Diagnosticable)child).toDiagnosticsNode(name: $"Child {count++}");
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            )
            .ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string toStringShort()
    {
        bool hasDebugLabel = (debugLabel is not null) && (debugLabel!.Length != 0);
        var extraData =
            $"{(hasDebugLabel ? debugLabel : "")}"
            + $"{((hasFocus && hasDebugLabel) ? " " : "")}"
            + $"{((hasFocus && !hasPrimaryFocus) ? "[IN FOCUS PATH]" : "")}"
            + $"{(hasPrimaryFocus ? "[PRIMARY FOCUS]" : "")}";
        return $"{DiagnosticsLibrary.describeIdentity(this)}{((extraData.Length != 0) ? $"({extraData})" : "")}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class FocusScopeNode : FocusNode
{
    public virtual TraversalEdgeBehavior traversalEdgeBehavior { get; set; } = default!;
    public virtual TraversalEdgeBehavior directionalTraversalEdgeBehavior { get; set; } = default!;
    internal virtual List<FocusNode> _focusedChildren { get; private set; } = new List<FocusNode>();

    public FocusScopeNode(
        string? debugLabel = null,
        Func<FocusNode, KeyEvent, KeyEventResult>? onKeyEvent = null,
        Func<FocusNode, RawKeyEvent, KeyEventResult>? onKey = null,
        bool skipTraversal = false,
        bool canRequestFocus = true,
        TraversalEdgeBehavior traversalEdgeBehavior = TraversalEdgeBehavior.closedLoop,
        TraversalEdgeBehavior directionalTraversalEdgeBehavior = TraversalEdgeBehavior.stop
    )
        : base(
            debugLabel: debugLabel,
            onKeyEvent: onKeyEvent,
            onKey: onKey,
            skipTraversal: skipTraversal,
            canRequestFocus: canRequestFocus,
            descendantsAreFocusable: true
        )
    {
        this.traversalEdgeBehavior = traversalEdgeBehavior;
        this.directionalTraversalEdgeBehavior = directionalTraversalEdgeBehavior;
    }

    public override FocusScopeNode? nearestScope => this;
    public override bool descendantsAreFocusable =>
        DartRuntimePrimitives.ConvertValue<bool>(_canRequestFocus && base.descendantsAreFocusable);
    public virtual bool isFirstFocus =>
        DartRuntimePrimitives.ConvertValue<bool>(Equals(enclosingScope!.focusedChild, this));
    public virtual FocusNode? focusedChild
    {
        get
        {
            DartRuntimePrimitives.Assert(
                () =>
                    !Enumerable.Any(_focusedChildren)
                    || Equals(_focusedChildren.Last().enclosingScope, this),
                () =>
                    (object?)
                        $"{debugLabel}: Focused child does not have the same idea of its enclosing scope "
                    + $"({_focusedChildren.LastOrDefault()?.enclosingScope}) as the scope does."
            );
            return _focusedChildren.LastOrDefault();
        }
    }
    public override IEnumerable<FocusNode> traversalChildren
    {
        get
        {
            if (!canRequestFocus)
            {
                return Enumerable.Empty<FocusNode>();
            }
            return base.traversalChildren;
        }
    }
    public override IEnumerable<FocusNode> traversalDescendants
    {
        get
        {
            if (!canRequestFocus)
            {
                return Enumerable.Empty<FocusNode>();
            }
            return base.traversalDescendants;
        }
    }

    public virtual void setFirstFocus(FocusScopeNode scope)
    {
        DartRuntimePrimitives.Assert(
            () => !Equals(scope, this),
            () => (object?)"Unexpected self-reference in setFirstFocus."
        );
        DartRuntimePrimitives.Assert(() =>
            Focus_managerLibrary._focusDebug(
                () => $"Setting scope as first focus in {this} to node:",
                () => new List<object> { scope }
            )
        );
        if (scope._parent is null)
        {
            _reparent(scope);
        }
        DartRuntimePrimitives.Assert(
            () => scope.ancestors.contains(this),
            () =>
                (object?)
                    $"{typeof(FocusScopeNode)} {scope} must be a child of {this} to set it as first focus."
        );
        if (hasFocus)
        {
            scope._doRequestFocus(findFirstFocus: true);
        }
        else
        {
            scope._setAsFocusedChildForScope();
        }
    }

    public virtual void autofocus(FocusNode node)
    {
        if (node._parent is null)
        {
            _reparent(node);
        }
        DartRuntimePrimitives.Assert(() => _manager is not null);
        DartRuntimePrimitives.Assert(() =>
            Focus_managerLibrary._focusDebug(() => $"Autofocus scheduled for {node}: scope {this}")
        );
        _manager?._pendingAutofocuses.Add(
            new _Autofocus__focus_manager(scope: this, autofocusNode: node)
        );
        _manager?._markNeedsUpdate();
    }

    public virtual void requestScopeFocus()
    {
        _doRequestFocus(findFirstFocus: false);
    }

    internal override void _doRequestFocus(bool findFirstFocus)
    {
        while (
            Enumerable.Any(_focusedChildren)
            && (
                !_focusedChildren.Last().canRequestFocus
                || (_focusedChildren.Last().enclosingScope is null)
            )
        )
        {
            _focusedChildren.removeLast();
        }
        FocusNode? focusedChildLocal = focusedChild;
        if (!findFirstFocus || (focusedChildLocal is null))
        {
            if (canRequestFocus)
            {
                _setAsFocusedChildForScope();
                _markNextFocus(this);
            }
            return;
        }
        focusedChildLocal._doRequestFocus(findFirstFocus: true);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        if (!Enumerable.Any(_focusedChildren))
        {
            return;
        }
        List<string> childList = Enumerable
            .Reverse(_focusedChildren)
            .map(
                (child) =>
                {
                    return ((Diagnosticable)child).toStringShort();
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            )
            .ToList()
            .ToList();
        properties.add(
            new IterableProperty<string>(
                "focusedChildren",
                childList.Cast<string>(),
                defaultValue: Enumerable.Empty<string>()
            )
        );
        properties.add(
            new DiagnosticsProperty<TraversalEdgeBehavior>(
                "traversalEdgeBehavior",
                traversalEdgeBehavior,
                defaultValue: TraversalEdgeBehavior.closedLoop
            )
        );
    }
}

public enum FocusHighlightMode
{
    touch,
    traditional,
}

public enum FocusHighlightStrategy
{
    automatic,
    alwaysTouch,
    alwaysTraditional,
}

internal class _AppLifecycleListener__focus_manager : WidgetsBindingObserver
{
    public virtual Action<AppLifecycleState> onLifecycleStateChanged { get; private set; } =
        default!;

    internal _AppLifecycleListener__focus_manager(Action<AppLifecycleState> onLifecycleStateChanged)
    {
        this.onLifecycleStateChanged = onLifecycleStateChanged;
    }

    public virtual void didChangeAppLifecycleState(AppLifecycleState state) =>
        onLifecycleStateChanged(state);
}

public class FocusManager : ChangeNotifier, DiagnosticableTree
{
    private readonly object _focusUpdateGate = new();
    internal virtual _HighlightModeManager__focus_manager _highlightManager { get; private set; } =
        new _HighlightModeManager__focus_manager();
    public virtual FocusScopeNode rootScope { get; private set; } =
        new FocusScopeNode(debugLabel: "Root Focus Scope");
    internal virtual FocusNode? _primaryFocus { get; set; } = default;
    internal virtual HashSet<FocusNode> _dirtyNodes { get; private set; } =
        new HashSet<FocusNode>();
    internal virtual _AppLifecycleListener__focus_manager? _appLifecycleListener { get; set; } =
        default;
    internal virtual FocusNode? _suspendedNode { get; set; } = default;
    internal virtual FocusNode? _markedForFocus { get; set; } = default;
    internal virtual List<_Autofocus__focus_manager> _pendingAutofocuses { get; private set; } =
        new List<_Autofocus__focus_manager>();
    internal virtual bool _haveScheduledUpdate { get; set; } = false;

    public FocusManager()
    {
        rootScope._manager = this;
    }

    internal virtual bool _respondToLifecycleChange =>
        DartRuntimePrimitives.ConvertValue<bool>(
            Foundation.ConstantsLibrary.kIsWeb
                || (
                    PlatformLibrary.defaultTargetPlatform switch
                    {
                        TargetPlatform.android => false,
                        TargetPlatform.iOS => false,
                        TargetPlatform.fuchsia => true,
                        TargetPlatform.linux => true,
                        TargetPlatform.windows => true,
                        TargetPlatform.macOS => true,
                        _ => throw new InvalidOperationException(
                            "Non-exhaustive Dart switch value."
                        ),
                    }
                )
        );

    public virtual void registerGlobalHandlers() => _highlightManager.registerGlobalHandlers();

    public override void dispose()
    {
        if (_appLifecycleListener is not null)
        {
            WidgetsBinding.instance.removeObserver(_appLifecycleListener!);
        }
        _highlightManager.dispose();
        rootScope.dispose();
        base.dispose();
    }

    public static FocusManager instance => WidgetsBinding.instance.focusManager;
    public virtual FocusHighlightStrategy highlightStrategy
    {
        get => _highlightManager.strategy;
        set
        {
            var __value = value;
            if (Equals(_highlightManager.strategy, __value))
            {
                return;
            }
            _highlightManager.strategy = __value;
        }
    }
    public virtual FocusHighlightMode highlightMode => _highlightManager.highlightMode;

    public virtual void addHighlightModeListener(Action<FocusHighlightMode> listener) =>
        _highlightManager.addListener(listener);

    public virtual void removeHighlightModeListener(Action<FocusHighlightMode> listener) =>
        _highlightManager.removeListener(listener);

    public virtual void addEarlyKeyEventHandler(Func<KeyEvent, KeyEventResult> handler)
    {
        _highlightManager.addEarlyKeyEventHandler(handler);
    }

    public virtual void removeEarlyKeyEventHandler(Func<KeyEvent, KeyEventResult> handler)
    {
        _highlightManager.removeEarlyKeyEventHandler(handler);
    }

    public virtual void addLateKeyEventHandler(Func<KeyEvent, KeyEventResult> handler)
    {
        _highlightManager.addLateKeyEventHandler(handler);
    }

    public virtual void removeLateKeyEventHandler(Func<KeyEvent, KeyEventResult> handler)
    {
        _highlightManager.removeLateKeyEventHandler(handler);
    }

    public virtual FocusNode? primaryFocus => _primaryFocus;

    internal virtual void _appLifecycleChange(AppLifecycleState state)
    {
        if (Equals(state, AppLifecycleState.resumed))
        {
            if (!Equals(_primaryFocus, rootScope))
            {
                DartRuntimePrimitives.Assert(() =>
                    Focus_managerLibrary._focusDebug(() =>
                        $"focus changed while app was paused, ignoring {_suspendedNode}"
                    )
                );
                _suspendedNode = null;
            }
            else
            {
                if (_suspendedNode is not null)
                {
                    if (_markedForFocus is null)
                    {
                        DartRuntimePrimitives.Assert(() =>
                            Focus_managerLibrary._focusDebug(() =>
                                $"requesting focus for {_suspendedNode}"
                            )
                        );
                        _suspendedNode!.requestFocus();
                        _suspendedNode = null;
                    }
                    else
                    {
                        DartRuntimePrimitives.Assert(() => _haveScheduledUpdate);
                        _suspendedNode = null;
                    }
                }
            }
        }
        else
        {
            if (!Equals(_primaryFocus, rootScope))
            {
                DartRuntimePrimitives.Assert(() =>
                    Focus_managerLibrary._focusDebug(() => $"suspending {_primaryFocus}")
                );
                _markedForFocus = DartRuntimePrimitives.ConvertValue<FocusNode>(rootScope);
                _suspendedNode = _primaryFocus;
                applyFocusChangesIfNeeded();
            }
        }
    }

    internal virtual void _markDetached(FocusNode node)
    {
        DartRuntimePrimitives.Assert(() =>
            Focus_managerLibrary._focusDebug(() => $"Node was detached: {node}")
        );
        if (Equals(_primaryFocus, node))
        {
            _primaryFocus = null;
        }
        if (Equals(_suspendedNode, node))
        {
            _suspendedNode = null;
        }
        _dirtyNodes.Remove(node);
    }

    internal virtual void _markPropertiesChanged(FocusNode node)
    {
        _markNeedsUpdate();
        DartRuntimePrimitives.Assert(() =>
            Focus_managerLibrary._focusDebug(() => $"Properties changed for node {node}.")
        );
        _dirtyNodes.Add(node);
    }

    internal virtual void _markNextFocus(FocusNode node)
    {
        if (Equals(_primaryFocus, node))
        {
            _markedForFocus = null;
        }
        else
        {
            _markedForFocus = node;
            _markNeedsUpdate();
        }
    }

    internal virtual void _markNeedsUpdate()
    {
        DartRuntimePrimitives.Assert(() =>
            Focus_managerLibrary._focusDebug(() =>
                $"Scheduling update, current focus is {_primaryFocus}, next focus will be {_markedForFocus}"
            )
        );
        if (_haveScheduledUpdate)
        {
            return;
        }
        _haveScheduledUpdate = true;
        DartAsyncRuntime.scheduleMicrotask(applyFocusChangesIfNeeded);
    }

    public virtual void applyFocusChangesIfNeeded()
    {
        lock (_focusUpdateGate)
        {
            DartRuntimePrimitives.Assert(
                () =>
                    !Equals(
                        Scheduler.SchedulerBinding.instance.schedulerPhase,
                        Scheduler.SchedulerPhase.persistentCallbacks
                    ),
                () =>
                    (object?)
                        "applyFocusChangesIfNeeded() should not be called during the build phase."
            );
            _haveScheduledUpdate = false;
            FocusNode? previousFocus = _primaryFocus;
            foreach (_Autofocus__focus_manager autofocus in _pendingAutofocuses)
            {
                autofocus.applyIfValid(this);
            }
            _pendingAutofocuses.Clear();
            if ((_primaryFocus is null) && (_markedForFocus is null))
            {
                _markedForFocus = DartRuntimePrimitives.ConvertValue<FocusNode>(rootScope);
            }
            DartRuntimePrimitives.Assert(() =>
                Focus_managerLibrary._focusDebug(() =>
                    $"Refreshing focus state. Next focus will be {_markedForFocus}"
                )
            );
            if ((_markedForFocus is not null) && (!Equals(_markedForFocus, _primaryFocus)))
            {
                HashSet<FocusNode> previousPath =
                    previousFocus?.ancestors.toSet() ?? new HashSet<FocusNode>();
                HashSet<FocusNode> nextPath = _markedForFocus!.ancestors.toSet();
                _dirtyNodes.UnionWith(nextPath.difference(previousPath));
                _dirtyNodes.UnionWith(previousPath.difference(nextPath));
                _primaryFocus = _markedForFocus;
                _markedForFocus = null;
            }
            DartRuntimePrimitives.Assert(() => _markedForFocus is null);
            if (!Equals(previousFocus, _primaryFocus))
            {
                DartRuntimePrimitives.Assert(() =>
                    Focus_managerLibrary._focusDebug(() =>
                        $"Updating focus from {previousFocus} to {_primaryFocus}"
                    )
                );
                if (previousFocus is not null)
                {
                    _dirtyNodes.Add(previousFocus);
                }
                if (_primaryFocus is not null)
                {
                    _dirtyNodes.Add(_primaryFocus!);
                }
            }
            foreach (FocusNode node in _dirtyNodes)
            {
                node?._notify();
            }
            DartRuntimePrimitives.Assert(() =>
                Focus_managerLibrary._focusDebug(
                    () => $"Notified {checked((long)_dirtyNodes.Count)} dirty nodes:",
                    () => _dirtyNodes
                )
            );
            _dirtyNodes.Clear();
            if (!Equals(previousFocus, _primaryFocus))
            {
                notifyListeners();
            }
            DartRuntimePrimitives.Assert(() =>
            {
                if (Focus_managerLibrary.debugFocusChanges)
                {
                    Focus_managerLibrary.debugDumpFocusTree();
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }

    public virtual void listenToApplicationLifecycleChangesIfSupported()
    {
        if ((_appLifecycleListener is null) && _respondToLifecycleChange)
        {
            _appLifecycleListener = new _AppLifecycleListener__focus_manager(_appLifecycleChange);
            WidgetsBinding.instance.addObserver(_appLifecycleListener!);
        }
    }

    IEnumerable<DiagnosticsNode> DiagnosticableTree.debugDescribeChildren() =>
        debugDescribeChildren();

    public virtual List<DiagnosticsNode> debugDescribeChildren()
    {
        return new List<DiagnosticsNode>
        {
            ((Diagnosticable)rootScope).toDiagnosticsNode(name: "rootScope"),
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(
            new FlagProperty(
                "haveScheduledUpdate",
                value: _haveScheduledUpdate,
                ifTrue: "UPDATE SCHEDULED"
            )
        );
        properties.add(
            new DiagnosticsProperty<FocusNode>("primaryFocus", primaryFocus, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<FocusNode>("nextFocus", _markedForFocus, defaultValue: null)
        );
        var element = ((Element?)primaryFocus?.context)!;
        if (element is not null)
        {
            properties.add(
                new DiagnosticsProperty<string>(
                    "primaryFocusCreator",
                    element.debugGetCreatorChain(20L)
                )
            );
        }
    }
}

internal class _HighlightModeManager__focus_manager
{
    internal virtual bool? _lastInteractionRequiresTraditionalHighlights { get; set; } = default;
    internal virtual FocusHighlightMode? _highlightMode { get; set; } = default;
    internal virtual FocusHighlightStrategy _strategy { get; set; } =
        FocusHighlightStrategy.automatic;
    internal virtual HashedObserverList<Func<KeyEvent, KeyEventResult>> _earlyKeyEventHandlers
    {
        get;
        private set;
    } = new HashedObserverList<Func<KeyEvent, KeyEventResult>>();
    internal virtual HashedObserverList<Func<KeyEvent, KeyEventResult>> _lateKeyEventHandlers
    {
        get;
        private set;
    } = new HashedObserverList<Func<KeyEvent, KeyEventResult>>();
    internal virtual HashedObserverList<Action<FocusHighlightMode>> _listeners { get; set; } =
        new HashedObserverList<Action<FocusHighlightMode>>();
    internal const long _kAndroidSoftKeyboardFlag = 2L;
    internal static long _kAndroidVirtualKeyboardDeviceId = -1L;

    internal _HighlightModeManager__focus_manager() { }

    public virtual FocusHighlightMode highlightMode =>
        DartRuntimePrimitives.ConvertValue<FocusHighlightMode>(
            _highlightMode ?? _defaultModeForPlatform
        );
    public virtual FocusHighlightStrategy strategy
    {
        get => _strategy;
        set
        {
            var __value = value;
            if (Equals(_strategy, __value))
            {
                return;
            }
            _strategy = __value;
            updateMode();
        }
    }

    public virtual void addEarlyKeyEventHandler(Func<KeyEvent, KeyEventResult> callback) =>
        _earlyKeyEventHandlers.add(callback);

    public virtual void removeEarlyKeyEventHandler(Func<KeyEvent, KeyEventResult> callback) =>
        _earlyKeyEventHandlers.remove(callback);

    public virtual void addLateKeyEventHandler(Func<KeyEvent, KeyEventResult> callback) =>
        _lateKeyEventHandlers.add(callback);

    public virtual void removeLateKeyEventHandler(Func<KeyEvent, KeyEventResult> callback) =>
        _lateKeyEventHandlers.remove(callback);

    public virtual void addListener(Action<FocusHighlightMode> listener) =>
        _listeners.add(listener);

    public virtual void removeListener(Action<FocusHighlightMode> listener) =>
        _listeners.remove(listener);

    public virtual void registerGlobalHandlers()
    {
        DartRuntimePrimitives.Assert(() =>
            ServicesBinding.instance.keyEventManager.keyMessageHandler is null
        );
        ServicesBinding.instance.keyEventManager.keyMessageHandler = handleKeyMessage;
        GestureBinding.instance.pointerRouter.addGlobalRoute(handlePointerEvent);
        Framework.Semantics.SemanticsBinding.instance.addSemanticsActionListener(
            handleSemanticsAction
        );
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
            Foundation.DebugLibrary.debugMaybeDispatchDisposed(this)
        );
        if (
            Equals(
                ServicesBinding.instance.keyEventManager.keyMessageHandler,
                (Func<KeyMessage, bool>)handleKeyMessage
            )
        )
        {
            GestureBinding.instance.pointerRouter.removeGlobalRoute(handlePointerEvent);
            ServicesBinding.instance.keyEventManager.keyMessageHandler = null;
            Framework.Semantics.SemanticsBinding.instance.removeSemanticsActionListener(
                handleSemanticsAction
            );
        }
        _listeners = new HashedObserverList<Action<FocusHighlightMode>>();
    }

    public virtual void notifyListeners()
    {
        if (!Enumerable.Any(_listeners))
        {
            return;
        }
        var localListeners = new List<Action<FocusHighlightMode>>(_listeners);
        foreach (var listener in localListeners)
        {
            try
            {
                if (_listeners.contains(listener))
                {
                    listener(highlightMode);
                }
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                InformationCollector? collector = default!;
                DartRuntimePrimitives.Assert(() =>
                {
                    collector = () =>
                        new List<DiagnosticsNode>
                        {
                            new DiagnosticsProperty<_HighlightModeManager__focus_manager>(
                                $"The {GetType()} sending notification was",
                                this,
                                style: DiagnosticsTreeStyle.errorProperty
                            ),
                        };
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
                FlutterError.reportError(
                    new FlutterErrorDetails(
                        exception: exceptionLocal,
                        stack: stackLocal,
                        library: "widgets library",
                        context: new ErrorDescription(
                            $"while dispatching notifications for {GetType()}"
                        ),
                        informationCollector: (InformationCollector?)collector
                    )
                );
            }
        }
    }

    public virtual void handlePointerEvent(PointerEvent @event)
    {
        switch (@event.kind)
        {
            case PointerDeviceKind.touch:
            case PointerDeviceKind.stylus:
            case PointerDeviceKind.invertedStylus:
            {
                if (_lastInteractionRequiresTraditionalHighlights != true)
                {
                    _lastInteractionRequiresTraditionalHighlights = true;
                    updateMode();
                }
                break;
            }
            case PointerDeviceKind.mouse:
            case PointerDeviceKind.trackpad:
            case PointerDeviceKind.unknown:
                break;
        }
    }

    internal virtual bool _isKeyMessageFromAndroidIME(KeyMessage message)
    {
        RawKeyEvent? rawEventLocal = message.rawEvent;
        if (rawEventLocal is null)
        {
            return false;
        }
        RawKeyEventData dataLocal = rawEventLocal.data;
        if (dataLocal is not RawKeyEventDataAndroid)
        {
            return false;
        }
        return ((((RawKeyEventDataAndroid)dataLocal).flags & _kAndroidSoftKeyboardFlag) != 0L)
            || (((RawKeyEventDataAndroid)dataLocal).deviceId == _kAndroidVirtualKeyboardDeviceId);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool handleKeyMessage(KeyMessage message)
    {
        if (_lastInteractionRequiresTraditionalHighlights != false)
        {
            bool isFromVirtualKeyboard = _isKeyMessageFromAndroidIME(message);
            if (!isFromVirtualKeyboard)
            {
                _lastInteractionRequiresTraditionalHighlights = false;
                updateMode();
            }
        }
        DartRuntimePrimitives.Assert(() =>
            Focus_managerLibrary._focusDebug(() => $"Received key event {message}")
        );
        if (FocusManager.instance.primaryFocus is null)
        {
            DartRuntimePrimitives.Assert(() =>
                Focus_managerLibrary._focusDebug(() =>
                    $"No primary focus for key event, ignored: {message}"
                )
            );
            return false;
        }
        var handledLocal = false;
        if (Enumerable.Any(_earlyKeyEventHandlers))
        {
            var results = new List<KeyEventResult>();
            foreach (var callback in _earlyKeyEventHandlers.ToList())
            {
                foreach (var @event in message.events)
                {
                    results.Add(callback(@event));
                }
            }
            KeyEventResult result = Focus_managerLibrary.combineKeyEventResults(
                results.Cast<KeyEventResult>()
            );
            switch (result)
            {
                case KeyEventResult.ignored:
                {
                    break;
                }
                case KeyEventResult.handled:
                {
                    DartRuntimePrimitives.Assert(() =>
                        Focus_managerLibrary._focusDebug(() =>
                            $"Key event {message} handled by early key event callback."
                        )
                    );
                    handledLocal = true;
                    break;
                }
                case KeyEventResult.skipRemainingHandlers:
                {
                    DartRuntimePrimitives.Assert(() =>
                        Focus_managerLibrary._focusDebug(() =>
                            $"Key event {message} propagation stopped by early key event callback."
                        )
                    );
                    handledLocal = false;
                    break;
                }
            }
        }
        if (handledLocal)
        {
            return true;
        }
        var focusPath = new List<FocusNode> { FocusManager.instance.primaryFocus! };
        focusPath.AddRange(FocusManager.instance.primaryFocus!.ancestors);
        foreach (var node in focusPath)
        {
            var resultsLocal = new List<KeyEventResult>();
            if (node.onKeyEvent is not null)
            {
                foreach (var @event in message.events)
                {
                    resultsLocal.Add(node.onKeyEvent(node, @event));
                }
            }
            if (node.onKey is not null && message.rawEvent is not null)
            {
                resultsLocal.Add(node.onKey(node, message.rawEvent));
            }
            KeyEventResult resultLocal = Focus_managerLibrary.combineKeyEventResults(
                resultsLocal.Cast<KeyEventResult>()
            );
            switch (resultLocal)
            {
                case KeyEventResult.ignored:
                {
                    continue;
                }
                case KeyEventResult.handled:
                {
                    DartRuntimePrimitives.Assert(() =>
                        Focus_managerLibrary._focusDebug(() =>
                            $"Node {node} handled key event {message}."
                        )
                    );
                    handledLocal = true;
                    break;
                }
                case KeyEventResult.skipRemainingHandlers:
                {
                    DartRuntimePrimitives.Assert(() =>
                        Focus_managerLibrary._focusDebug(() =>
                            $"Node {node} stopped key event propagation: {message}."
                        )
                    );
                    handledLocal = false;
                    break;
                }
            }
            DartRuntimePrimitives.Assert(() => !Equals(resultLocal, KeyEventResult.ignored));
            break;
        }
        if (!handledLocal && Enumerable.Any(_lateKeyEventHandlers))
        {
            var resultsAlternate = new List<KeyEventResult>();
            foreach (var callback in _lateKeyEventHandlers.ToList())
            {
                foreach (var @event in message.events)
                {
                    resultsAlternate.Add(callback(@event));
                }
            }
            KeyEventResult resultAlternate = Focus_managerLibrary.combineKeyEventResults(
                resultsAlternate.Cast<KeyEventResult>()
            );
            switch (resultAlternate)
            {
                case KeyEventResult.ignored:
                {
                    break;
                }
                case KeyEventResult.handled:
                {
                    DartRuntimePrimitives.Assert(() =>
                        Focus_managerLibrary._focusDebug(() =>
                            $"Key event {message} handled by late key event callback."
                        )
                    );
                    handledLocal = true;
                    break;
                }
                case KeyEventResult.skipRemainingHandlers:
                {
                    DartRuntimePrimitives.Assert(() =>
                        Focus_managerLibrary._focusDebug(() =>
                            $"Key event {message} propagation stopped by late key event callback."
                        )
                    );
                    handledLocal = false;
                    break;
                }
            }
        }
        if (!handledLocal)
        {
            DartRuntimePrimitives.Assert(() =>
                Focus_managerLibrary._focusDebug(() =>
                    $"Key event not handled by focus system: {message}."
                )
            );
        }
        return handledLocal;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleSemanticsAction(SemanticsActionEvent semanticsActionEvent)
    {
        if (
            Foundation.ConstantsLibrary.kIsWeb
            && Equals(semanticsActionEvent.type, SemanticsAction.focus)
            && (_lastInteractionRequiresTraditionalHighlights != true)
        )
        {
            _lastInteractionRequiresTraditionalHighlights = true;
            updateMode();
        }
    }

    public virtual void updateMode()
    {
        FocusHighlightMode newMode = default!;
        switch (strategy)
        {
            case FocusHighlightStrategy.automatic:
            {
                if (_lastInteractionRequiresTraditionalHighlights is null)
                {
                    return;
                }
                if (
                    (
                        _lastInteractionRequiresTraditionalHighlights
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    )
                )
                {
                    newMode = FocusHighlightMode.touch;
                }
                else
                {
                    newMode = FocusHighlightMode.traditional;
                }
                break;
            }
            case FocusHighlightStrategy.alwaysTouch:
            {
                newMode = FocusHighlightMode.touch;
                break;
            }
            case FocusHighlightStrategy.alwaysTraditional:
            {
                newMode = FocusHighlightMode.traditional;
                break;
            }
        }
        FocusHighlightMode oldMode = highlightMode;
        _highlightMode = newMode;
        if (!Equals(highlightMode, oldMode))
        {
            notifyListeners();
        }
    }

    internal static FocusHighlightMode _defaultModeForPlatform
    {
        get
        {
            switch (PlatformLibrary.defaultTargetPlatform)
            {
                case TargetPlatform.android:
                case TargetPlatform.fuchsia:
                case TargetPlatform.iOS:
                {
                    if (((RendererBinding)WidgetsBinding.instance).mouseTracker.mouseIsConnected)
                    {
                        return FocusHighlightMode.traditional;
                    }
                    return FocusHighlightMode.touch;
                }
                case TargetPlatform.linux:
                case TargetPlatform.macOS:
                case TargetPlatform.windows:
                {
                    return FocusHighlightMode.traditional;
                }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
        }
    }
}

public static partial class Focus_managerLibrary
{
    public static FocusNode? primaryFocus => WidgetsBinding.instance.focusManager.primaryFocus;
}

public static partial class Focus_managerLibrary
{
    public static string debugDescribeFocusTree()
    {
        string? result = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            result = ((DiagnosticableTree)FocusManager.instance).toStringDeep();
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        return result ?? "";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Focus_managerLibrary
{
    public static void debugDumpFocusTree()
    {
        DartRuntimePrimitives.Assert(() =>
        {
            PrintLibrary.debugPrint(debugDescribeFocusTree());
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
    }
}
