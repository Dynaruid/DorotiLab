// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/focus_manager.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Widgets;

public static partial class Focus_managerLibrary
{
    public static bool debugFocusChanges = false;
}

public static partial class Focus_managerLibrary
{
    internal static bool _focusDebug(global::System.Func<string> messageFunc, global::System.Func<IEnumerable<object>>? detailsFunc = null)
    {
        if (global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            throw new InvalidOperationException("_focusDebug was called in Release mode. It should always be wrapped in " + "an assert. Always call _focusDebug like so:\n" + "  assert(_focusDebug(() => 'Blah $foo'));");
        }
        if (!Focus_managerLibrary.debugFocusChanges)
        {
            return true;
        }
        global::Doroti.Framework.Foundation.PrintLibrary.debugPrint($"FOCUS: {messageFunc()}");
        IEnumerable<object> details = ((detailsFunc is null ? new List<object>() : detailsFunc.Invoke()));
        if (System.Linq.Enumerable.Any(details))
        {
            foreach (var detail in details)
            {
                global::Doroti.Framework.Foundation.PrintLibrary.debugPrint($"    {detail}");
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
    skipRemainingHandlers
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
        return (hasSkipRemainingHandlers ? KeyEventResult.skipRemainingHandlers : KeyEventResult.ignored);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public delegate KeyEventResult FocusOnKeyCallback(FocusNode node, global::Doroti.Framework.Services.RawKeyEvent @event);

public delegate KeyEventResult FocusOnKeyEventCallback(FocusNode node, global::Doroti.Framework.Services.KeyEvent @event);

public delegate KeyEventResult OnKeyEventCallback(global::Doroti.Framework.Services.KeyEvent @event);

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
        bool shouldApply = ((((((this.scope.parent is not null) || DartRuntimePrimitives.Identical(this.scope, ((FocusManager)manager).rootScope))) && DartRuntimePrimitives.Identical(this.scope._manager, manager)) && (((FocusScopeNode)this.scope).focusedChild is null)) && ((FocusNode)this.autofocusNode).ancestors.contains(this.scope));
        if (shouldApply)
        {
            DartRuntimePrimitives.Assert(() => Focus_managerLibrary._focusDebug((() => $"Applying autofocus: {this.autofocusNode}")));
            this.autofocusNode._doRequestFocus(findFirstFocus: true);
        }
        else
        {
            DartRuntimePrimitives.Assert(() => Focus_managerLibrary._focusDebug((() => $"Autofocus request discarded for node: {this.autofocusNode}.")));
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

    public virtual bool isAttached => DartRuntimePrimitives.ConvertValue<bool>((object.Equals(((FocusNode)this._node)._attachment, this)));
    public virtual void detach()
    {
        DartRuntimePrimitives.Assert(() => Focus_managerLibrary._focusDebug((() => "Detaching node:"), (() => new List<object> { this._node, $"With enclosing scope {((FocusNode)this._node).enclosingScope}" })));
        if (this.isAttached)
        {
            if ((((FocusNode)this._node).hasPrimaryFocus || (((((FocusNode)this._node)._manager is not null) && (object.Equals(((FocusNode)this._node)._manager!._markedForFocus, this._node))))))
            {
                this._node.unfocus(disposition: UnfocusDisposition.previouslyFocusedChild);
            }
            ((FocusNode)this._node)._manager?._markDetached(this._node);
            ((dynamic)((FocusNode)this._node)._parent)?._removeChild(this._node);
            this._node._attachment = null;
            DartRuntimePrimitives.Assert(() => !((FocusNode)this._node).hasPrimaryFocus, () => (object?)$"Node {(((object?)((FocusNode)this._node).debugLabel ?? (object?)this._node))} still has primary focus while being detached.");
            DartRuntimePrimitives.Assert(() => (!object.Equals(((FocusNode)this._node)._manager?._markedForFocus, this._node)), () => (object?)$"Node {(((object?)((FocusNode)this._node).debugLabel ?? (object?)this._node))} still marked for focus while being detached.");
        }
        DartRuntimePrimitives.Assert(() => !this.isAttached);
    }

    public virtual void reparent(FocusNode? parent = null)
    {
        if (this.isAttached)
        {
            DartRuntimePrimitives.Assert(() => (((FocusNode)this._node).context is not null));
            parent ??= Focus.maybeOf(((FocusNode)this._node).context!, scopeOk: true);
            parent ??= ((FocusNode)this._node).context!.owner!.focusManager.rootScope;
            parent._reparent(this._node);
        }
    }

}

public enum UnfocusDisposition
{
    scope,
    previouslyFocusedChild
}

public class FocusNode : ChangeNotifier
{
    internal virtual bool _skipTraversal { get; set; } = default!;
    internal virtual bool _canRequestFocus { get; set; } = default!;
    internal virtual bool _descendantsAreFocusable { get; set; } = default!;
    internal virtual bool _descendantsAreTraversable { get; set; } = default!;
    internal virtual BuildContext? _context { get; set; } = default;
    public virtual global::System.Func<FocusNode, global::Doroti.Framework.Services.RawKeyEvent, KeyEventResult>? onKey { get; set; } = default;
    public virtual global::System.Func<FocusNode, global::Doroti.Framework.Services.KeyEvent, KeyEventResult>? onKeyEvent { get; set; } = default;
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

    public FocusNode(string? debugLabel = null, global::System.Func<FocusNode, global::Doroti.Framework.Services.RawKeyEvent, KeyEventResult>? onKey = null, global::System.Func<FocusNode, global::Doroti.Framework.Services.KeyEvent, KeyEventResult>? onKeyEvent = null, bool skipTraversal = false, bool canRequestFocus = true, bool descendantsAreFocusable = true, bool descendantsAreTraversable = true)
    {
        this.onKey = onKey;
        this.onKeyEvent = onKeyEvent;
        this._skipTraversal = skipTraversal;
        this._canRequestFocus = canRequestFocus;
        this._descendantsAreFocusable = descendantsAreFocusable;
        this._descendantsAreTraversable = descendantsAreTraversable;
    }

    public virtual bool skipTraversal
    {
        get
        {
            if (this._skipTraversal)
            {
                return true;
            }
            foreach (FocusNode ancestor in this.ancestors)
            {
                if (!((FocusNode)ancestor).descendantsAreTraversable)
                {
                    return true;
                }
            }
            return false;
            return default!;
        }
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) != this._skipTraversal))
            {
                _skipTraversal = DartRuntimePrimitives.RequireValue(__value);
                this._manager?._markPropertiesChanged(this);
            }
        }
    }
    public virtual bool canRequestFocus
    {
        get => (this._canRequestFocus && this.ancestors.All(_allowDescendantsToBeFocused));
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) != this._canRequestFocus))
            {
                _canRequestFocus = DartRuntimePrimitives.RequireValue(__value);
                if ((this.hasFocus && !DartRuntimePrimitives.RequireValue(__value)))
                {
                    unfocus(disposition: UnfocusDisposition.previouslyFocusedChild);
                }
                this._manager?._markPropertiesChanged(this);
            }
        }
    }
    internal static bool _allowDescendantsToBeFocused(FocusNode ancestor) => ((FocusNode)ancestor).descendantsAreFocusable;
    public virtual bool descendantsAreFocusable
    {
        get => this._descendantsAreFocusable;
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._descendantsAreFocusable))
            {
                return;
            }
            _descendantsAreFocusable = DartRuntimePrimitives.RequireValue(__value);
            if ((!DartRuntimePrimitives.RequireValue(__value) && this.hasFocus))
            {
                unfocus(disposition: UnfocusDisposition.previouslyFocusedChild);
            }
            this._manager?._markPropertiesChanged(this);
        }
    }
    public virtual bool descendantsAreTraversable
    {
        get => this._descendantsAreTraversable;
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) != this._descendantsAreTraversable))
            {
                _descendantsAreTraversable = DartRuntimePrimitives.RequireValue(__value);
                this._manager?._markPropertiesChanged(this);
            }
        }
    }
    public virtual BuildContext? context => this._context;
    public virtual FocusNode? parent => this._parent;
    public virtual IEnumerable<FocusNode> children => DartRuntimePrimitives.ConvertValue<IEnumerable<FocusNode>>(this._children);
    public virtual IEnumerable<FocusNode> traversalChildren
    {
        get
        {
            if (!this.descendantsAreFocusable)
            {
                return System.Linq.Enumerable.Empty<FocusNode>();
            }
            return this.children.where(((node) => (!((FocusNode)node).skipTraversal && ((FocusNode)node).canRequestFocus)));
            return default!;
        }
    }
    public virtual string? debugLabel
    {
        get => this._debugLabel;
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
            if ((this._descendants is null))
            {
                var result = new List<FocusNode>();
                foreach (FocusNode child in this._children)
                {
                    result.AddRange(((FocusNode)child).descendants.Cast<FocusNode>());
                    result.Add(child);
                }
                _descendants = result;
            }
            return ((IEnumerable<FocusNode>)(object?)this._descendants!);
            return default!;
        }
    }
    public virtual IEnumerable<FocusNode> traversalDescendants
    {
        get
        {
            if (!this.descendantsAreFocusable)
            {
                return System.Linq.Enumerable.Empty<FocusNode>();
            }
            return this.descendants.where(((node) => (!((FocusNode)node).skipTraversal && ((FocusNode)node).canRequestFocus)));
            return default!;
        }
    }
    public virtual IEnumerable<FocusNode> ancestors
    {
        get
        {
            if ((this._ancestors is null))
            {
                var result = new List<FocusNode>();
                FocusNode? parent = this._parent;
                while ((parent is not null))
                {
                    result.Add(parent);
                    parent = ((FocusNode)parent)._parent;
                }
                _ancestors = result;
            }
            return ((IEnumerable<FocusNode>)(object?)this._ancestors!);
            return default!;
        }
    }
    public virtual bool hasFocus => DartRuntimePrimitives.ConvertValue<bool>((this.hasPrimaryFocus || ((this._manager?.primaryFocus?.ancestors.contains(this) ?? false))));
    public virtual bool hasPrimaryFocus => DartRuntimePrimitives.ConvertValue<bool>((object.Equals(this._manager?.primaryFocus, this)));
    public virtual FocusHighlightMode highlightMode => FocusManager.instance.highlightMode;
    public virtual FocusScopeNode? nearestScope => this.enclosingScope;
    internal virtual void _clearEnclosingScopeCache()
    {
        FocusScopeNode? cachedScope = this._enclosingScope;
        if ((cachedScope is null))
        {
            return;
        }
        _enclosingScope = null;
        if (System.Linq.Enumerable.Any(this.children))
        {
            foreach (FocusNode child in this.children)
            {
                if (DartRuntimePrimitives.Identical(cachedScope, ((FocusNode)child)._enclosingScope))
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
            FocusScopeNode? enclosingScope = _enclosingScope ??= this.parent?.nearestScope;
            DartRuntimePrimitives.Assert(() => (object.Equals(enclosingScope, this.parent?.nearestScope)), () => (object?)$"{this} has invalid scope cache: {this._enclosingScope} != {this.parent?.nearestScope}");
            return enclosingScope;
            return default!;
        }
    }
    public virtual global::Doroti.Ui.Size size => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Size>(this.rect.size);
    public virtual global::Doroti.Ui.Offset offset
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (this.context is not null), () => (object?)"Tried to get the offset of a focus node that didn't have its context set yet.\n" + "The context needs to be set before trying to evaluate traversal policies. " + "Setting the context is typically done with the attach method.");
            global::Doroti.Framework.Rendering.RenderObject @object = this.context!.findRenderObject()!;
            return MatrixUtils.transformPoint(((Matrix4)((dynamic)@object).getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null))), ((global::Doroti.Framework.Rendering.RenderObject)@object).semanticBounds.topLeft);
            return default!;
        }
    }
    public virtual global::Doroti.Ui.Rect rect
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (this.context is not null), () => (object?)"Tried to get the bounds of a focus node that didn't have its context set yet.\n" + "The context needs to be set before trying to evaluate traversal policies. " + "Setting the context is typically done with the attach method.");
            global::Doroti.Framework.Rendering.RenderObject @object = this.context!.findRenderObject()!;
            global::Doroti.Ui.Offset topLeftLocal = ((global::Doroti.Ui.Offset)(object?)MatrixUtils.transformPoint(((Matrix4)((dynamic)@object).getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null))), ((global::Doroti.Framework.Rendering.RenderObject)@object).semanticBounds.topLeft));
            global::Doroti.Ui.Offset bottomRightLocal = ((global::Doroti.Ui.Offset)(object?)MatrixUtils.transformPoint(((Matrix4)((dynamic)@object).getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null))), ((global::Doroti.Framework.Rendering.RenderObject)@object).semanticBounds.bottomRight));
            return global::Doroti.Ui.Rect.fromLTRB(topLeftLocal.dx, topLeftLocal.dy, bottomRightLocal.dx, bottomRightLocal.dy);
            return default!;
        }
    }
    public virtual void unfocus(UnfocusDisposition disposition = UnfocusDisposition.scope)
    {
        if ((!this.hasFocus && (((this._manager is null) || (!object.Equals(this._manager!._markedForFocus, this))))))
        {
            return;
        }
        FocusScopeNode? scopeLocal = this.enclosingScope;
        if ((scopeLocal is null))
        {
            return;
        }
        switch (disposition)
        {
            case UnfocusDisposition.scope:
                {
                    if (scopeLocal.canRequestFocus)
                    {
                        ((FocusScopeNode)scopeLocal)._focusedChildren.Clear();
                    }
                    while (!scopeLocal!.canRequestFocus)
                    {
                        scopeLocal = (scopeLocal.enclosingScope ?? this._manager?.rootScope);
                    }
                    scopeLocal._doRequestFocus(findFirstFocus: false);
                    break;
                }
            case UnfocusDisposition.previouslyFocusedChild:
                {
                    if (scopeLocal.canRequestFocus)
                    {
                        ((FocusScopeNode)scopeLocal)._focusedChildren.Remove(this);
                    }
                    while (!scopeLocal!.canRequestFocus)
                    {
                        scopeLocal.enclosingScope?._focusedChildren.Remove(scopeLocal);
                        scopeLocal = (scopeLocal.enclosingScope ?? this._manager?.rootScope);
                    }
                    scopeLocal._doRequestFocus(findFirstFocus: true);
                    break;
                }
        }
        DartRuntimePrimitives.Assert(() => Focus_managerLibrary._focusDebug((() => "Unfocused node:"), (() => new List<object> { $"primary focus was {this}", $"next focus will be {this._manager?._markedForFocus}" })));
    }

    public virtual bool consumeKeyboardToken()
    {
        if (!this._hasKeyboardToken)
        {
            return false;
        }
        _hasKeyboardToken = false;
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _markNextFocus(FocusNode newFocus)
    {
        if ((this._manager is not null))
        {
            this._manager!._markNextFocus(this);
            return;
        }
        newFocus._setAsFocusedChildForScope();
        newFocus._notify();
        if ((!object.Equals(newFocus, this)))
        {
            _notify();
        }
    }

    internal virtual void _removeChild(FocusNode node, bool removeScopeFocus = true)
    {
        DartRuntimePrimitives.Assert(() => this._children.Contains(node), () => (object?)"Tried to remove a node that wasn't a child.");
        DartRuntimePrimitives.Assert(() => (object.Equals(((FocusNode)node)._parent, this)));
        DartRuntimePrimitives.Assert(() => (object.Equals(((FocusNode)node)._manager, this._manager)));
        if (removeScopeFocus)
        {
            FocusScopeNode? nodeScope = ((FocusNode)node).enclosingScope;
            if ((nodeScope is not null))
            {
                ((FocusScopeNode)nodeScope)._focusedChildren.Remove(node);
                ((FocusNode)node).descendants.where(((descendant) =>
                {
                    return (object.Equals(((FocusNode)descendant).enclosingScope, nodeScope));
                    throw new InvalidOperationException("Dart closure completed without a value.");
                })).forEach((__arg0) => { _ = ((FocusScopeNode)nodeScope)._focusedChildren.Remove(DartRuntimePrimitives.ConvertValue<FocusScopeNode>(__arg0)); });
            }
        }
        node._parent = null;
        node._clearEnclosingScopeCache();
        this._children.Remove(node);
        foreach (FocusNode ancestor in this.ancestors)
        {
            ancestor._descendants = null;
        }
        _descendants = null;
        DartRuntimePrimitives.Assert(() => ((this._manager is null) || !this._manager!.rootScope.descendants.contains(node)));
    }

    internal virtual void _updateManager(FocusManager? manager)
    {
        _manager = manager;
        foreach (FocusNode descendant in this.descendants)
        {
            descendant._manager = manager;
            descendant._ancestors = null;
        }
    }

    internal virtual void _reparent(FocusNode child)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this)), () => (object?)"Tried to make a child into a parent of itself.");
        if ((object.Equals(((FocusNode)child)._parent, this)))
        {
            DartRuntimePrimitives.Assert(() => this._children.Contains(child), () => (object?)"Found a node that says it's a child, but doesn't appear in the child list.");
            return;
        }
        DartRuntimePrimitives.Assert(() => ((this._manager is null) || (!object.Equals(child, this._manager!.rootScope))), () => (object?)"Reparenting the root node isn't allowed.");
        DartRuntimePrimitives.Assert(() => !this.ancestors.contains(child), () => (object?)"The supplied child is already an ancestor of this node. Loops are not allowed.");
        FocusScopeNode? oldScopeLocal = ((FocusNode)child).enclosingScope;
        bool hadFocus = ((FocusNode)child).hasFocus;
        ((dynamic)((FocusNode)child)._parent)?._removeChild(child, removeScopeFocus: (!object.Equals(oldScopeLocal, this.nearestScope)));
        this._children.Add(child);
        child._parent = this;
        child._ancestors = null;
        child._updateManager(this._manager);
        foreach (FocusNode ancestor in ((FocusNode)child).ancestors)
        {
            ancestor._descendants = null;
        }
        if (hadFocus)
        {
            this._manager?.primaryFocus?._setAsFocusedChildForScope();
        }
        if ((((oldScopeLocal is not null) && (((FocusNode)child).context is not null)) && (!object.Equals(((FocusNode)child).enclosingScope, oldScopeLocal))))
        {
            FocusTraversalGroup.maybeOf(((FocusNode)child).context!)?.changedScope(node: child, oldScope: oldScopeLocal);
        }
        if (((FocusNode)child)._requestFocusWhenReparented)
        {
            child._doRequestFocus(findFirstFocus: true);
            child._requestFocusWhenReparented = false;
        }
    }

    public virtual FocusAttachment attach(BuildContext? context, global::System.Func<FocusNode, global::Doroti.Framework.Services.KeyEvent, KeyEventResult>? onKeyEvent = null, global::System.Func<FocusNode, global::Doroti.Framework.Services.RawKeyEvent, KeyEventResult>? onKey = null)
    {
        _context = context;
        this.onKey = (global::System.Func<FocusNode, global::Doroti.Framework.Services.RawKeyEvent, KeyEventResult>)((onKey ?? (global::System.Func<FocusNode, global::Doroti.Framework.Services.RawKeyEvent, KeyEventResult>)this.onKey));
        this.onKeyEvent = (global::System.Func<FocusNode, global::Doroti.Framework.Services.KeyEvent, KeyEventResult>)((onKeyEvent ?? (global::System.Func<FocusNode, global::Doroti.Framework.Services.KeyEvent, KeyEventResult>)this.onKeyEvent));
        _attachment = new FocusAttachment(this);
        return this._attachment!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void dispose()
    {
        this._attachment?.detach();
        base.dispose();
    }

    internal virtual void _notify()
    {
        if ((this._parent is null))
        {
            return;
        }
        if (this.hasPrimaryFocus)
        {
            _setAsFocusedChildForScope();
        }
        notifyListeners();
    }

    public virtual void requestFocus(FocusNode? node = null)
    {
        if ((node is not null))
        {
            if ((((FocusNode)node)._parent is null))
            {
                _reparent(node);
            }
            DartRuntimePrimitives.Assert(() => ((FocusNode)node).ancestors.contains(this), () => (object?)"Focus was requested for a node that is not a descendant of the scope from which it was requested.");
            node._doRequestFocus(findFirstFocus: true);
            return;
        }
        _doRequestFocus(findFirstFocus: true);
    }

    internal virtual void _doRequestFocus(bool findFirstFocus)
    {
        if (!this.canRequestFocus)
        {
            DartRuntimePrimitives.Assert(() => Focus_managerLibrary._focusDebug((() => $"Node NOT requesting focus because canRequestFocus is false: {this}")));
            return;
        }
        if ((this._parent is null))
        {
            _requestFocusWhenReparented = true;
            return;
        }
        _setAsFocusedChildForScope();
        if ((this.hasPrimaryFocus && (((this._manager!._markedForFocus is null) || (object.Equals(this._manager!._markedForFocus, this))))))
        {
            return;
        }
        _hasKeyboardToken = true;
        DartRuntimePrimitives.Assert(() => Focus_managerLibrary._focusDebug((() => $"Node requesting focus: {this}")));
        _markNextFocus(this);
    }

    internal virtual void _setAsFocusedChildForScope()
    {
        var scopeFocus = this;
        foreach (FocusScopeNode ancestor in this.ancestors.OfType<FocusScopeNode>())
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(scopeFocus, ancestor)), () => (object?)"Somehow made a loop by setting focusedChild to its scope.");
            DartRuntimePrimitives.Assert(() => Focus_managerLibrary._focusDebug((() => $"Setting {scopeFocus} as focused child for scope:"), (() => new List<object> { ancestor })));
            var focusedChildren = ((FocusScopeNode)ancestor)._focusedChildren;
            lock (focusedChildren)
            {
                focusedChildren.RemoveAll(child => ReferenceEquals(child, scopeFocus));
                focusedChildren.Add(scopeFocus);
            }
            scopeFocus = DartRuntimePrimitives.ConvertValue<FocusNode>(ancestor);
        }
    }

    public virtual bool nextFocus() => FocusTraversalGroup.of(this.context!).next(this);
    public virtual bool previousFocus() => FocusTraversalGroup.of(this.context!).previous(this);
    public virtual bool focusInDirection(TraversalDirection direction) => FocusTraversalGroup.of(this.context!).inDirection(this, direction);
    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<BuildContext>("context", this.context, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("descendantsAreFocusable", value: this.descendantsAreFocusable, ifFalse: "DESCENDANTS UNFOCUSABLE", defaultValue: true));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("descendantsAreTraversable", value: this.descendantsAreTraversable, ifFalse: "DESCENDANTS UNTRAVERSABLE", defaultValue: true));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("canRequestFocus", value: this.canRequestFocus, ifFalse: "NOT FOCUSABLE", defaultValue: true));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("hasFocus", value: (this.hasFocus && !this.hasPrimaryFocus), ifTrue: "IN FOCUS PATH", defaultValue: false));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("hasPrimaryFocus", value: this.hasPrimaryFocus, ifTrue: "PRIMARY FOCUS", defaultValue: false));
    }

    public virtual List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var count = 1L;
        return this._children.map<FocusNode, global::Doroti.Framework.Foundation.DiagnosticsNode>(((child) =>
        {
            return ((global::Doroti.Framework.Foundation.DiagnosticsNode)(object?)((Diagnosticable)child).toDiagnosticsNode(name: $"Child {count++}"));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string toStringShort()
    {
        bool hasDebugLabel = ((this.debugLabel is not null) && (this.debugLabel!.Length != 0));
        var extraData = $"{(hasDebugLabel ? this.debugLabel : "")}" + $"{((this.hasFocus && hasDebugLabel) ? " " : "")}" + $"{((this.hasFocus && !this.hasPrimaryFocus) ? "[IN FOCUS PATH]" : "")}" + $"{(this.hasPrimaryFocus ? "[PRIMARY FOCUS]" : "")}";
        return $"{(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}{((extraData.Length != 0) ? $"({extraData})" : "")}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class FocusScopeNode : FocusNode
{
    public virtual TraversalEdgeBehavior traversalEdgeBehavior { get; set; } = default!;
    public virtual TraversalEdgeBehavior directionalTraversalEdgeBehavior { get; set; } = default!;
    internal virtual List<FocusNode> _focusedChildren { get; private set; } = new List<FocusNode>();

    public FocusScopeNode(string? debugLabel = null, global::System.Func<FocusNode, global::Doroti.Framework.Services.KeyEvent, KeyEventResult>? onKeyEvent = null, global::System.Func<FocusNode, global::Doroti.Framework.Services.RawKeyEvent, KeyEventResult>? onKey = null, bool skipTraversal = false, bool canRequestFocus = true, TraversalEdgeBehavior traversalEdgeBehavior = TraversalEdgeBehavior.closedLoop, TraversalEdgeBehavior directionalTraversalEdgeBehavior = TraversalEdgeBehavior.stop) : base(debugLabel: debugLabel, onKeyEvent: onKeyEvent, onKey: onKey, skipTraversal: skipTraversal, canRequestFocus: canRequestFocus, descendantsAreFocusable: true)
    {
        this.traversalEdgeBehavior = traversalEdgeBehavior;
        this.directionalTraversalEdgeBehavior = directionalTraversalEdgeBehavior;
    }

    public override FocusScopeNode? nearestScope => this;
    public override bool descendantsAreFocusable => DartRuntimePrimitives.ConvertValue<bool>((this._canRequestFocus && base.descendantsAreFocusable));
    public virtual bool isFirstFocus => DartRuntimePrimitives.ConvertValue<bool>((object.Equals(this.enclosingScope!.focusedChild, this)));
    public virtual FocusNode? focusedChild
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (!System.Linq.Enumerable.Any(this._focusedChildren) || (object.Equals(this._focusedChildren.Last().enclosingScope, this))), () => (object?)$"{this.debugLabel}: Focused child does not have the same idea of its enclosing scope " + $"({this._focusedChildren.LastOrDefault()?.enclosingScope}) as the scope does.");
            return this._focusedChildren.LastOrDefault();
            return default!;
        }
    }
    public override IEnumerable<FocusNode> traversalChildren
    {
        get
        {
            if (!this.canRequestFocus)
            {
                return System.Linq.Enumerable.Empty<FocusNode>();
            }
            return base.traversalChildren;
            return default!;
        }
    }
    public override IEnumerable<FocusNode> traversalDescendants
    {
        get
        {
            if (!this.canRequestFocus)
            {
                return System.Linq.Enumerable.Empty<FocusNode>();
            }
            return base.traversalDescendants;
            return default!;
        }
    }
    public virtual void setFirstFocus(FocusScopeNode scope)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(scope, this)), () => (object?)"Unexpected self-reference in setFirstFocus.");
        DartRuntimePrimitives.Assert(() => Focus_managerLibrary._focusDebug((() => $"Setting scope as first focus in {this} to node:"), (() => new List<object> { scope })));
        if ((scope._parent is null))
        {
            _reparent(scope);
        }
        DartRuntimePrimitives.Assert(() => scope.ancestors.contains(this), () => (object?)$"{typeof(FocusScopeNode)} {scope} must be a child of {this} to set it as first focus.");
        if (this.hasFocus)
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
        if ((((FocusNode)node)._parent is null))
        {
            _reparent(node);
        }
        DartRuntimePrimitives.Assert(() => (this._manager is not null));
        DartRuntimePrimitives.Assert(() => Focus_managerLibrary._focusDebug((() => $"Autofocus scheduled for {node}: scope {this}")));
        this._manager?._pendingAutofocuses.Add(new _Autofocus__focus_manager(scope: this, autofocusNode: node));
        this._manager?._markNeedsUpdate();
    }

    public virtual void requestScopeFocus()
    {
        _doRequestFocus(findFirstFocus: false);
    }

    internal override void _doRequestFocus(bool findFirstFocus)
    {
        while ((System.Linq.Enumerable.Any(this._focusedChildren) && ((!this._focusedChildren.Last().canRequestFocus || (this._focusedChildren.Last().enclosingScope is null)))))
        {
            this._focusedChildren.removeLast<FocusNode>();
        }
        FocusNode? focusedChildLocal = this.focusedChild;
        if ((!findFirstFocus || (focusedChildLocal is null)))
        {
            if (this.canRequestFocus)
            {
                _setAsFocusedChildForScope();
                _markNextFocus(this);
            }
            return;
        }
        focusedChildLocal._doRequestFocus(findFirstFocus: true);
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        if (!System.Linq.Enumerable.Any(this._focusedChildren))
        {
            return;
        }
        List<string> childList = System.Linq.Enumerable.Reverse(this._focusedChildren).map<FocusNode, string>(((child) =>
        {
            return ((string)(object?)((Diagnosticable)child).toStringShort());
            throw new InvalidOperationException("Dart closure completed without a value.");
        })).ToList().ToList();
        properties.add(new global::Doroti.Framework.Foundation.IterableProperty<string>("focusedChildren", childList.Cast<string>(), defaultValue: System.Linq.Enumerable.Empty<string>()));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<TraversalEdgeBehavior>("traversalEdgeBehavior", this.traversalEdgeBehavior, defaultValue: TraversalEdgeBehavior.closedLoop));
    }

}

public enum FocusHighlightMode
{
    touch,
    traditional
}

public enum FocusHighlightStrategy
{
    automatic,
    alwaysTouch,
    alwaysTraditional
}

internal class _AppLifecycleListener__focus_manager : WidgetsBindingObserver
{
    public virtual global::System.Action<AppLifecycleState> onLifecycleStateChanged { get; private set; } = default!;

    internal _AppLifecycleListener__focus_manager(global::System.Action<AppLifecycleState> onLifecycleStateChanged)
    {
        this.onLifecycleStateChanged = onLifecycleStateChanged;
    }

    public virtual void didChangeAppLifecycleState(AppLifecycleState state) => this.onLifecycleStateChanged(state);
}

public class FocusManager : ChangeNotifier
{
    private readonly object _focusUpdateGate = new();
    internal virtual _HighlightModeManager__focus_manager _highlightManager { get; private set; } = new _HighlightModeManager__focus_manager();
    public virtual FocusScopeNode rootScope { get; private set; } = new FocusScopeNode(debugLabel: "Root Focus Scope");
    internal virtual FocusNode? _primaryFocus { get; set; } = default;
    internal virtual HashSet<FocusNode> _dirtyNodes { get; private set; } = new HashSet<FocusNode>();
    internal virtual _AppLifecycleListener__focus_manager? _appLifecycleListener { get; set; } = default;
    internal virtual FocusNode? _suspendedNode { get; set; } = default;
    internal virtual FocusNode? _markedForFocus { get; set; } = default;
    internal virtual List<_Autofocus__focus_manager> _pendingAutofocuses { get; private set; } = new List<_Autofocus__focus_manager>();
    internal virtual bool _haveScheduledUpdate { get; set; } = false;

    public FocusManager()
    {
        this.rootScope._manager = this;
    }

    internal virtual bool _respondToLifecycleChange => DartRuntimePrimitives.ConvertValue<bool>((global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb || (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform switch { global::Doroti.Framework.Foundation.TargetPlatform.android => false, global::Doroti.Framework.Foundation.TargetPlatform.iOS => false, global::Doroti.Framework.Foundation.TargetPlatform.fuchsia => true, global::Doroti.Framework.Foundation.TargetPlatform.linux => true, global::Doroti.Framework.Foundation.TargetPlatform.windows => true, global::Doroti.Framework.Foundation.TargetPlatform.macOS => true, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })));
    public virtual void registerGlobalHandlers() => this._highlightManager.registerGlobalHandlers();
    public virtual void dispose()
    {
        if ((this._appLifecycleListener is not null))
        {
            WidgetsBinding.instance.removeObserver(this._appLifecycleListener!);
        }
        this._highlightManager.dispose();
        this.rootScope.dispose();
        base.dispose();
    }

    public static FocusManager instance => WidgetsBinding.instance.focusManager;
    public virtual FocusHighlightStrategy highlightStrategy
    {
        get => ((_HighlightModeManager__focus_manager)this._highlightManager).strategy;
        set
        {
            var __value = value;
            if ((object.Equals(((_HighlightModeManager__focus_manager)this._highlightManager).strategy, __value)))
            {
                return;
            }
            this._highlightManager.strategy = __value;
        }
    }
    public virtual FocusHighlightMode highlightMode => ((_HighlightModeManager__focus_manager)this._highlightManager).highlightMode;
    public virtual void addHighlightModeListener(global::System.Action<FocusHighlightMode> listener) => this._highlightManager.addListener((global::System.Action<FocusHighlightMode>)listener);
    public virtual void removeHighlightModeListener(global::System.Action<FocusHighlightMode> listener) => this._highlightManager.removeListener((global::System.Action<FocusHighlightMode>)listener);
    public virtual void addEarlyKeyEventHandler(global::System.Func<global::Doroti.Framework.Services.KeyEvent, KeyEventResult> handler)
    {
        this._highlightManager.addEarlyKeyEventHandler((global::System.Func<global::Doroti.Framework.Services.KeyEvent, KeyEventResult>)handler);
    }

    public virtual void removeEarlyKeyEventHandler(global::System.Func<global::Doroti.Framework.Services.KeyEvent, KeyEventResult> handler)
    {
        this._highlightManager.removeEarlyKeyEventHandler((global::System.Func<global::Doroti.Framework.Services.KeyEvent, KeyEventResult>)handler);
    }

    public virtual void addLateKeyEventHandler(global::System.Func<global::Doroti.Framework.Services.KeyEvent, KeyEventResult> handler)
    {
        this._highlightManager.addLateKeyEventHandler((global::System.Func<global::Doroti.Framework.Services.KeyEvent, KeyEventResult>)handler);
    }

    public virtual void removeLateKeyEventHandler(global::System.Func<global::Doroti.Framework.Services.KeyEvent, KeyEventResult> handler)
    {
        this._highlightManager.removeLateKeyEventHandler((global::System.Func<global::Doroti.Framework.Services.KeyEvent, KeyEventResult>)handler);
    }

    public virtual FocusNode? primaryFocus => this._primaryFocus;
    internal virtual void _appLifecycleChange(AppLifecycleState state)
    {
        if ((object.Equals(state, AppLifecycleState.resumed)))
        {
            if ((!object.Equals(this._primaryFocus, this.rootScope)))
            {
                DartRuntimePrimitives.Assert(() => Focus_managerLibrary._focusDebug((() => $"focus changed while app was paused, ignoring {this._suspendedNode}")));
                _suspendedNode = null;
            }
            else
            {
                if ((this._suspendedNode is not null))
                {
                    if ((this._markedForFocus is null))
                    {
                        DartRuntimePrimitives.Assert(() => Focus_managerLibrary._focusDebug((() => $"requesting focus for {this._suspendedNode}")));
                        this._suspendedNode!.requestFocus();
                        _suspendedNode = null;
                    }
                    else
                    {
                        DartRuntimePrimitives.Assert(() => this._haveScheduledUpdate);
                        _suspendedNode = null;
                    }
                }
            }
        }
        else
        {
            if ((!object.Equals(this._primaryFocus, this.rootScope)))
            {
                DartRuntimePrimitives.Assert(() => Focus_managerLibrary._focusDebug((() => $"suspending {this._primaryFocus}")));
                _markedForFocus = DartRuntimePrimitives.ConvertValue<FocusNode>(this.rootScope);
                _suspendedNode = this._primaryFocus;
                applyFocusChangesIfNeeded();
            }
        }
    }

    internal virtual void _markDetached(FocusNode node)
    {
        DartRuntimePrimitives.Assert(() => Focus_managerLibrary._focusDebug((() => $"Node was detached: {node}")));
        if ((object.Equals(this._primaryFocus, node)))
        {
            _primaryFocus = null;
        }
        if ((object.Equals(this._suspendedNode, node)))
        {
            _suspendedNode = null;
        }
        this._dirtyNodes.Remove(node);
    }

    internal virtual void _markPropertiesChanged(FocusNode node)
    {
        _markNeedsUpdate();
        DartRuntimePrimitives.Assert(() => Focus_managerLibrary._focusDebug((() => $"Properties changed for node {node}.")));
        this._dirtyNodes.Add(node);
    }

    internal virtual void _markNextFocus(FocusNode node)
    {
        if ((object.Equals(this._primaryFocus, node)))
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
        DartRuntimePrimitives.Assert(() => Focus_managerLibrary._focusDebug((() => $"Scheduling update, current focus is {this._primaryFocus}, next focus will be {this._markedForFocus}")));
        if (this._haveScheduledUpdate)
        {
            return;
        }
        _haveScheduledUpdate = true;
        DartAsyncRuntime.scheduleMicrotask(this.applyFocusChangesIfNeeded);
    }

    public virtual void applyFocusChangesIfNeeded()
    {
        lock (_focusUpdateGate)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(global::Doroti.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase, global::Doroti.Framework.Scheduler.SchedulerPhase.persistentCallbacks)), () => (object?)"applyFocusChangesIfNeeded() should not be called during the build phase.");
            _haveScheduledUpdate = false;
            FocusNode? previousFocus = this._primaryFocus;
            foreach (_Autofocus__focus_manager autofocus in this._pendingAutofocuses)
            {
                autofocus.applyIfValid(this);
            }
            this._pendingAutofocuses.Clear();
            if (((this._primaryFocus is null) && (this._markedForFocus is null)))
            {
                _markedForFocus = DartRuntimePrimitives.ConvertValue<FocusNode>(this.rootScope);
            }
            DartRuntimePrimitives.Assert(() => Focus_managerLibrary._focusDebug((() => $"Refreshing focus state. Next focus will be {this._markedForFocus}")));
            if (((this._markedForFocus is not null) && (!object.Equals(this._markedForFocus, this._primaryFocus))))
            {
                HashSet<FocusNode> previousPath = (previousFocus?.ancestors.toSet() ?? new HashSet<FocusNode>());
                HashSet<FocusNode> nextPath = this._markedForFocus!.ancestors.toSet();
                this._dirtyNodes.UnionWith(nextPath.difference<FocusNode>(previousPath));
                this._dirtyNodes.UnionWith(previousPath.difference<FocusNode>(nextPath));
                _primaryFocus = this._markedForFocus;
                _markedForFocus = null;
            }
            DartRuntimePrimitives.Assert(() => (this._markedForFocus is null));
            if ((!object.Equals(previousFocus, this._primaryFocus)))
            {
                DartRuntimePrimitives.Assert(() => Focus_managerLibrary._focusDebug((() => $"Updating focus from {previousFocus} to {this._primaryFocus}")));
                if ((previousFocus is not null))
                {
                    this._dirtyNodes.Add(previousFocus);
                }
                if ((this._primaryFocus is not null))
                {
                    this._dirtyNodes.Add(this._primaryFocus!);
                }
            }
            foreach (FocusNode node in this._dirtyNodes)
            {
                node?._notify();
            }
            DartRuntimePrimitives.Assert(() => Focus_managerLibrary._focusDebug((() => $"Notified {checked((long)(this._dirtyNodes.Count))} dirty nodes:"), (() => this._dirtyNodes)));
            this._dirtyNodes.Clear();
            if ((!object.Equals(previousFocus, this._primaryFocus)))
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
        if (((this._appLifecycleListener is null) && this._respondToLifecycleChange))
        {
            _appLifecycleListener = new _AppLifecycleListener__focus_manager((global::System.Action<AppLifecycleState>)this._appLifecycleChange);
            WidgetsBinding.instance.addObserver(this._appLifecycleListener!);
        }
    }

    public virtual List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        return new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { ((Diagnosticable)this.rootScope).toDiagnosticsNode(name: "rootScope") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("haveScheduledUpdate", value: this._haveScheduledUpdate, ifTrue: "UPDATE SCHEDULED"));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<FocusNode>("primaryFocus", this.primaryFocus, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<FocusNode>("nextFocus", this._markedForFocus, defaultValue: null));
        var element = ((Element?)(object?)this.primaryFocus?.context)!;
        if ((element is not null))
        {
            properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<string>("primaryFocusCreator", element.debugGetCreatorChain(20L)));
        }
    }

}

internal class _HighlightModeManager__focus_manager
{
    internal virtual bool? _lastInteractionRequiresTraditionalHighlights { get; set; } = default;
    internal virtual FocusHighlightMode? _highlightMode { get; set; } = default;
    internal virtual FocusHighlightStrategy _strategy { get; set; } = FocusHighlightStrategy.automatic;
    internal virtual global::Doroti.Framework.Foundation.HashedObserverList<global::System.Func<global::Doroti.Framework.Services.KeyEvent, KeyEventResult>> _earlyKeyEventHandlers { get; private set; } = new global::Doroti.Framework.Foundation.HashedObserverList<global::System.Func<global::Doroti.Framework.Services.KeyEvent, KeyEventResult>>();
    internal virtual global::Doroti.Framework.Foundation.HashedObserverList<global::System.Func<global::Doroti.Framework.Services.KeyEvent, KeyEventResult>> _lateKeyEventHandlers { get; private set; } = new global::Doroti.Framework.Foundation.HashedObserverList<global::System.Func<global::Doroti.Framework.Services.KeyEvent, KeyEventResult>>();
    internal virtual global::Doroti.Framework.Foundation.HashedObserverList<global::System.Action<FocusHighlightMode>> _listeners { get; set; } = new global::Doroti.Framework.Foundation.HashedObserverList<global::System.Action<FocusHighlightMode>>();
    internal const long _kAndroidSoftKeyboardFlag = 2L;
    internal static long _kAndroidVirtualKeyboardDeviceId = -1L;

    internal _HighlightModeManager__focus_manager()
    {
    }

    public virtual FocusHighlightMode highlightMode => DartRuntimePrimitives.ConvertValue<FocusHighlightMode>(((this._highlightMode ?? (FocusHighlightMode)_defaultModeForPlatform)));
    public virtual FocusHighlightStrategy strategy
    {
        get => this._strategy;
        set
        {
            var __value = value;
            if ((object.Equals(this._strategy, __value)))
            {
                return;
            }
            _strategy = __value;
            updateMode();
        }
    }
    public virtual void addEarlyKeyEventHandler(global::System.Func<global::Doroti.Framework.Services.KeyEvent, KeyEventResult> callback) => this._earlyKeyEventHandlers.add((global::System.Func<global::Doroti.Framework.Services.KeyEvent, KeyEventResult>)callback);
    public virtual void removeEarlyKeyEventHandler(global::System.Func<global::Doroti.Framework.Services.KeyEvent, KeyEventResult> callback) => this._earlyKeyEventHandlers.remove((global::System.Func<global::Doroti.Framework.Services.KeyEvent, KeyEventResult>)callback);
    public virtual void addLateKeyEventHandler(global::System.Func<global::Doroti.Framework.Services.KeyEvent, KeyEventResult> callback) => this._lateKeyEventHandlers.add((global::System.Func<global::Doroti.Framework.Services.KeyEvent, KeyEventResult>)callback);
    public virtual void removeLateKeyEventHandler(global::System.Func<global::Doroti.Framework.Services.KeyEvent, KeyEventResult> callback) => this._lateKeyEventHandlers.remove((global::System.Func<global::Doroti.Framework.Services.KeyEvent, KeyEventResult>)callback);
    public virtual void addListener(global::System.Action<FocusHighlightMode> listener) => this._listeners.add((global::System.Action<FocusHighlightMode>)listener);
    public virtual void removeListener(global::System.Action<FocusHighlightMode> listener) => this._listeners.remove((global::System.Action<FocusHighlightMode>)listener);
    public virtual void registerGlobalHandlers()
    {
        DartRuntimePrimitives.Assert(() => (global::Doroti.Framework.Services.ServicesBinding.instance.keyEventManager.keyMessageHandler is null));
        global::Doroti.Framework.Services.ServicesBinding.instance.keyEventManager.keyMessageHandler = (global::System.Func<global::Doroti.Framework.Services.KeyMessage, bool>)this.handleKeyMessage;
        global::Doroti.Framework.Gestures.GestureBinding.instance.pointerRouter.addGlobalRoute((global::System.Action<global::Doroti.Framework.Gestures.PointerEvent>)this.handlePointerEvent);
        global::Doroti.Framework.Semantics.SemanticsBinding.instance.addSemanticsActionListener((global::System.Action<SemanticsActionEvent>)this.handleSemanticsAction);
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        if ((object.Equals((global::System.Func<global::Doroti.Framework.Services.KeyMessage, bool>?)global::Doroti.Framework.Services.ServicesBinding.instance.keyEventManager.keyMessageHandler, (global::System.Func<global::Doroti.Framework.Services.KeyMessage, bool>)this.handleKeyMessage)))
        {
            global::Doroti.Framework.Gestures.GestureBinding.instance.pointerRouter.removeGlobalRoute((global::System.Action<global::Doroti.Framework.Gestures.PointerEvent>)this.handlePointerEvent);
            global::Doroti.Framework.Services.ServicesBinding.instance.keyEventManager.keyMessageHandler = null;
            global::Doroti.Framework.Semantics.SemanticsBinding.instance.removeSemanticsActionListener((global::System.Action<SemanticsActionEvent>)this.handleSemanticsAction);
        }
        _listeners = new global::Doroti.Framework.Foundation.HashedObserverList<global::System.Action<FocusHighlightMode>>();
    }

    public virtual void notifyListeners()
    {
        if (!System.Linq.Enumerable.Any(this._listeners))
        {
            return;
        }
        var localListeners = new List<global::System.Action<FocusHighlightMode>>(this._listeners);
        foreach (var listener in localListeners)
        {
            try
            {
                if (this._listeners.contains((global::System.Action<FocusHighlightMode>)listener))
                {
                    listener(this.highlightMode);
                }
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                InformationCollector? collector = default!;
                DartRuntimePrimitives.Assert(() =>
                    {
                        collector = (() => new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.DiagnosticsProperty<_HighlightModeManager__focus_manager>($"The {this.GetType()} sending notification was", this, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) });
                        return true;
                        throw new InvalidOperationException("Dart closure completed without a value.");
                    });
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exceptionLocal, stack: stackLocal, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription($"while dispatching notifications for {this.GetType()}"), informationCollector: (InformationCollector?)collector));
            }
        }
    }

    public virtual void handlePointerEvent(global::Doroti.Framework.Gestures.PointerEvent @event)
    {
        switch (((global::Doroti.Framework.Gestures.PointerEvent)@event).kind)
        {
            case PointerDeviceKind.touch:
            case PointerDeviceKind.stylus:
            case PointerDeviceKind.invertedStylus:
                {
                    if ((this._lastInteractionRequiresTraditionalHighlights != true))
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

    internal virtual bool _isKeyMessageFromAndroidIME(global::Doroti.Framework.Services.KeyMessage message)
    {
        global::Doroti.Framework.Services.RawKeyEvent? rawEventLocal = ((global::Doroti.Framework.Services.KeyMessage)message).rawEvent;
        if ((rawEventLocal is null))
        {
            return false;
        }
        global::Doroti.Framework.Services.RawKeyEventData dataLocal = ((global::Doroti.Framework.Services.RawKeyEvent)rawEventLocal).data;
        if ((dataLocal is not global::Doroti.Framework.Services.RawKeyEventDataAndroid))
        {
            return false;
        }
        return ((((((global::Doroti.Framework.Services.RawKeyEventDataAndroid)((global::Doroti.Framework.Services.RawKeyEventDataAndroid)dataLocal)).flags & _kAndroidSoftKeyboardFlag)) != 0L) || (((global::Doroti.Framework.Services.RawKeyEventDataAndroid)((global::Doroti.Framework.Services.RawKeyEventDataAndroid)dataLocal)).deviceId == _kAndroidVirtualKeyboardDeviceId));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool handleKeyMessage(global::Doroti.Framework.Services.KeyMessage message)
    {
        if ((this._lastInteractionRequiresTraditionalHighlights != false))
        {
            bool isFromVirtualKeyboard = _isKeyMessageFromAndroidIME(message);
            if (!isFromVirtualKeyboard)
            {
                _lastInteractionRequiresTraditionalHighlights = false;
                updateMode();
            }
        }
        DartRuntimePrimitives.Assert(() => Focus_managerLibrary._focusDebug((() => $"Received key event {message}")));
        if ((FocusManager.instance.primaryFocus is null))
        {
            DartRuntimePrimitives.Assert(() => Focus_managerLibrary._focusDebug((() => $"No primary focus for key event, ignored: {message}")));
            return false;
        }
        var handledLocal = false;
        if (System.Linq.Enumerable.Any(this._earlyKeyEventHandlers))
        {
            var results = new List<KeyEventResult>();
            KeyEventResult result = Focus_managerLibrary.combineKeyEventResults(results.Cast<KeyEventResult>());
            switch (result)
            {
                case KeyEventResult.ignored:
                    {
                        break;
                    }
                case KeyEventResult.handled:
                    {
                        DartRuntimePrimitives.Assert(() => Focus_managerLibrary._focusDebug((() => $"Key event {message} handled by early key event callback.")));
                        handledLocal = true;
                        break;
                    }
                case KeyEventResult.skipRemainingHandlers:
                    {
                        DartRuntimePrimitives.Assert(() => Focus_managerLibrary._focusDebug((() => $"Key event {message} propagation stopped by early key event callback.")));
                        handledLocal = false;
                        break;
                    }
            }
        }
        if (handledLocal)
        {
            return true;
        }
        foreach (var node in new List<FocusNode> { FocusManager.instance.primaryFocus! })
        {
            var resultsLocal = new List<KeyEventResult>();
            KeyEventResult resultLocal = Focus_managerLibrary.combineKeyEventResults(resultsLocal.Cast<KeyEventResult>());
            switch (resultLocal)
            {
                case KeyEventResult.ignored:
                    {
                        continue;
                    }
                case KeyEventResult.handled:
                    {
                        DartRuntimePrimitives.Assert(() => Focus_managerLibrary._focusDebug((() => $"Node {node} handled key event {message}.")));
                        handledLocal = true;
                        break;
                    }
                case KeyEventResult.skipRemainingHandlers:
                    {
                        DartRuntimePrimitives.Assert(() => Focus_managerLibrary._focusDebug((() => $"Node {node} stopped key event propagation: {message}.")));
                        handledLocal = false;
                        break;
                    }
            }
            DartRuntimePrimitives.Assert(() => (!object.Equals(resultLocal, KeyEventResult.ignored)));
            break;
        }
        if ((!handledLocal && System.Linq.Enumerable.Any(this._lateKeyEventHandlers)))
        {
            var resultsAlternate = new List<KeyEventResult>();
            KeyEventResult resultAlternate = Focus_managerLibrary.combineKeyEventResults(resultsAlternate.Cast<KeyEventResult>());
            switch (resultAlternate)
            {
                case KeyEventResult.ignored:
                    {
                        break;
                    }
                case KeyEventResult.handled:
                    {
                        DartRuntimePrimitives.Assert(() => Focus_managerLibrary._focusDebug((() => $"Key event {message} handled by late key event callback.")));
                        handledLocal = true;
                        break;
                    }
                case KeyEventResult.skipRemainingHandlers:
                    {
                        DartRuntimePrimitives.Assert(() => Focus_managerLibrary._focusDebug((() => $"Key event {message} propagation stopped by late key event callback.")));
                        handledLocal = false;
                        break;
                    }
            }
        }
        if (!handledLocal)
        {
            DartRuntimePrimitives.Assert(() => Focus_managerLibrary._focusDebug((() => $"Key event not handled by focus system: {message}.")));
        }
        return handledLocal;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleSemanticsAction(SemanticsActionEvent semanticsActionEvent)
    {
        if (((global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb && (object.Equals(semanticsActionEvent.type, SemanticsAction.focus))) && (this._lastInteractionRequiresTraditionalHighlights != true)))
        {
            _lastInteractionRequiresTraditionalHighlights = true;
            updateMode();
        }
    }

    public virtual void updateMode()
    {
        FocusHighlightMode newMode = default!;
        switch (this.strategy)
        {
            case FocusHighlightStrategy.automatic:
                {
                    if ((this._lastInteractionRequiresTraditionalHighlights is null))
                    {
                        return;
                    }
                    if (DartRuntimePrimitives.RequireValue(this._lastInteractionRequiresTraditionalHighlights))
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
        FocusHighlightMode oldMode = this.highlightMode;
        _highlightMode = newMode;
        if ((!object.Equals(this.highlightMode, oldMode)))
        {
            notifyListeners();
        }
    }

    internal static FocusHighlightMode _defaultModeForPlatform
    {
        get
        {
            switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
            {
                case global::Doroti.Framework.Foundation.TargetPlatform.android:
                case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                    {
                        if (((global::Doroti.Framework.Rendering.MouseTracker)((dynamic)WidgetsBinding.instance).mouseTracker).mouseIsConnected)
                        {
                            return FocusHighlightMode.traditional;
                        }
                        return FocusHighlightMode.touch;
                    }
                case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                    {
                        return FocusHighlightMode.traditional;
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
            return default!;
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
                result = ((string)((dynamic)FocusManager.instance).toStringDeep());
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return (result ?? "");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Focus_managerLibrary
{
    public static void debugDumpFocusTree()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                global::Doroti.Framework.Foundation.PrintLibrary.debugPrint(Focus_managerLibrary.debugDescribeFocusTree());
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }
}
