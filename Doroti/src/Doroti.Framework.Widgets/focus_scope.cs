// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/focus_scope.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public class Focus : StatefulWidget
{
    public virtual FocusNode? parentNode { get; private set; }
    public virtual Widget child { get; private set; } = default!;
    public virtual FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual Action<bool>? onFocusChange { get; private set; }
    internal virtual Func<FocusNode, KeyEvent, KeyEventResult>? _onKeyEvent { get; private set; }
    internal virtual Func<FocusNode, RawKeyEvent, KeyEventResult>? _onKey { get; private set; }
    internal virtual bool? _canRequestFocus { get; private set; }
    internal virtual bool? _skipTraversal { get; private set; }
    internal virtual bool? _descendantsAreFocusable { get; private set; }
    internal virtual bool? _descendantsAreTraversable { get; private set; }
    public virtual bool includeSemantics { get; private set; } = default!;

    internal static Action? CreateSemanticsFocusAction(
        TargetPlatform targetPlatform,
        bool couldRequestFocus,
        FocusNode focusNode) =>
        targetPlatform != TargetPlatform.iOS && couldRequestFocus
            ? () => focusNode.requestFocus()
            : null;
    internal virtual string? _debugLabel { get; private set; }

    public Focus(Key? key = null, Widget child = default!, FocusNode? focusNode = null, FocusNode? parentNode = null, bool autofocus = false, Action<bool>? onFocusChange = null, Func<FocusNode, KeyEvent, KeyEventResult>? onKeyEvent = null, Func<FocusNode, RawKeyEvent, KeyEventResult>? onKey = null, bool? canRequestFocus = null, bool? skipTraversal = null, bool? descendantsAreFocusable = null, bool? descendantsAreTraversable = null, bool includeSemantics = true, string? debugLabel = null) : base(key: key)
    {
        this.child = child;
        this.focusNode = focusNode;
        this.parentNode = parentNode;
        this.autofocus = autofocus;
        this.onFocusChange = onFocusChange;
        this.includeSemantics = includeSemantics;
        _onKeyEvent = onKeyEvent;
        _onKey = onKey;
        _canRequestFocus = canRequestFocus;
        _skipTraversal = skipTraversal;
        _descendantsAreFocusable = descendantsAreFocusable;
        _descendantsAreTraversable = descendantsAreTraversable;
        _debugLabel = debugLabel;
    }

    public static Focus CreateWithExternalFocusNode(Key? key = null, Widget child = default!, FocusNode focusNode = default!, FocusNode? parentNode = null, bool autofocus = default!, Action<bool>? onFocusChange = null, bool includeSemantics = default!)
        => new _FocusWithExternalFocusNode__focus_scope(key, child, focusNode, parentNode, autofocus, onFocusChange, includeSemantics);

    internal virtual bool _usingExternalFocus => false;
    public virtual Func<FocusNode, KeyEvent, KeyEventResult>? onKeyEvent => DartRuntimePrimitives.ConvertValue<Func<FocusNode, KeyEvent, KeyEventResult>>(_onKeyEvent ?? (focusNode?.onKeyEvent));
    public virtual Func<FocusNode, RawKeyEvent, KeyEventResult>? onKey => DartRuntimePrimitives.ConvertValue<Func<FocusNode, RawKeyEvent, KeyEventResult>>(_onKey ?? (focusNode?.onKey));
    public virtual bool canRequestFocus => DartRuntimePrimitives.ConvertValue<bool>((_canRequestFocus ?? focusNode?.canRequestFocus) ?? true);
    public virtual bool skipTraversal => DartRuntimePrimitives.ConvertValue<bool>((_skipTraversal ?? focusNode?.skipTraversal) ?? false);
    public virtual bool descendantsAreFocusable => DartRuntimePrimitives.ConvertValue<bool>((_descendantsAreFocusable ?? focusNode?.descendantsAreFocusable) ?? true);
    public virtual bool descendantsAreTraversable => DartRuntimePrimitives.ConvertValue<bool>((_descendantsAreTraversable ?? focusNode?.descendantsAreTraversable) ?? true);
    public virtual string? debugLabel => DartRuntimePrimitives.ConvertValue<string>(_debugLabel ?? focusNode?.debugLabel);
    public static FocusNode of(BuildContext context, bool scopeOk = false, bool createDependency = true)
    {
        FocusNode? node = maybeOf(context, scopeOk: scopeOk, createDependency: createDependency);
        DartRuntimePrimitives.Assert(() =>
            {
                if (node is null)
                {
                    throw DartRuntimePrimitives.AsException(FlutterError.Create("Focus.of() was called with a context that does not contain a Focus widget.\n" + "No Focus widget ancestor could be found starting from the context that was passed to " + "Focus.of(). This can happen because you are using a widget that looks for a Focus " + "ancestor, and do not have a Focus widget descendant in the nearest FocusScope.\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() =>
            {
                if (!scopeOk && (node is FocusScopeNode))
                {
                    FocusScopeNode node__17435__as18136 = (FocusScopeNode)node;
                    throw DartRuntimePrimitives.AsException(FlutterError.Create("Focus.of() was called with a context that does not contain a Focus between the given " + "context and the nearest FocusScope widget.\n" + "No Focus ancestor could be found starting from the context that was passed to " + "Focus.of() to the point where it found the nearest FocusScope widget. This can happen " + "because you are using a widget that looks for a Focus ancestor, and do not have a " + "Focus widget ancestor in the current FocusScope.\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return node!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static FocusNode? maybeOf(BuildContext context, bool scopeOk = false, bool createDependency = true)
    {
        _FocusInheritedScope__focus_scope? scope = createDependency ? context.dependOnInheritedWidgetOfExactType<_FocusInheritedScope__focus_scope>() : context.getInheritedWidgetOfExactType<_FocusInheritedScope__focus_scope>();
        return scope?.notifier switch { null => DartRuntimePrimitives.ConvertValue<FocusNode>(null), FocusScopeNode __object19955 when !scopeOk => DartRuntimePrimitives.ConvertValue<FocusNode>(null), FocusNode node => node };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool isAt(BuildContext context) => DartRuntimePrimitives.ConvertValue<bool>(maybeOf(context)?.hasFocus ?? false);
    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new StringProperty("debugLabel", debugLabel, defaultValue: null));
        properties.add(new FlagProperty("autofocus", value: autofocus, ifTrue: "AUTOFOCUS", defaultValue: false));
        properties.add(new FlagProperty("canRequestFocus", value: canRequestFocus, ifFalse: "NOT FOCUSABLE", defaultValue: false));
        properties.add(new FlagProperty("descendantsAreFocusable", value: descendantsAreFocusable, ifFalse: "DESCENDANTS UNFOCUSABLE", defaultValue: true));
        properties.add(new FlagProperty("descendantsAreTraversable", value: descendantsAreTraversable, ifFalse: "DESCENDANTS UNTRAVERSABLE", defaultValue: true));
        properties.add(new DiagnosticsProperty<FocusNode>("focusNode", focusNode, defaultValue: null));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _FocusState__focus_scope());
}

internal class _FocusWithExternalFocusNode__focus_scope : Focus
{
    internal _FocusWithExternalFocusNode__focus_scope(Key? key = null, Widget child = default!, FocusNode focusNode = default!, FocusNode? parentNode = null, bool autofocus = false, Action<bool>? onFocusChange = null, bool includeSemantics = true) : base(key: key, child: child, focusNode: focusNode, parentNode: parentNode, autofocus: autofocus, onFocusChange: onFocusChange, includeSemantics: includeSemantics)
    {
    }

    internal override bool _usingExternalFocus => true;
    public override Func<FocusNode, KeyEvent, KeyEventResult>? onKeyEvent => focusNode!.onKeyEvent;
    public override Func<FocusNode, RawKeyEvent, KeyEventResult>? onKey => focusNode!.onKey;
    public override bool canRequestFocus => focusNode!.canRequestFocus;
    public override bool skipTraversal => focusNode!.skipTraversal;
    public override bool descendantsAreFocusable => focusNode!.descendantsAreFocusable;
    internal override bool? _descendantsAreTraversable => focusNode!.descendantsAreTraversable;
    public override string? debugLabel => focusNode!.debugLabel;
}

internal class _FocusState__focus_scope : State<Focus>
{
    internal virtual FocusNode? _internalNode { get; set; } = default;
    internal virtual bool _hadPrimaryFocus { get; set; } = default!;
    internal virtual bool _couldRequestFocus { get; set; } = default!;
    internal virtual bool _descendantsWereFocusable { get; set; } = default!;
    internal virtual bool _descendantsWereTraversable { get; set; } = default!;
    internal virtual bool _didAutofocus { get; set; } = false;
    internal virtual FocusAttachment? _focusAttachment { get; set; } = default;

    public virtual FocusNode focusNode => DartRuntimePrimitives.ConvertValue<FocusNode>(widget.focusNode ?? (_internalNode ??= _createNode()));
    public override void initState()
    {
        base.initState();
        _initNode();
    }

    internal virtual void _initNode()
    {
        if (!widget._usingExternalFocus)
        {
            focusNode.descendantsAreFocusable = widget.descendantsAreFocusable;
            focusNode.descendantsAreTraversable = widget.descendantsAreTraversable;
            focusNode.skipTraversal = widget.skipTraversal;
            if (widget._canRequestFocus is not null)
            {
                focusNode.canRequestFocus = DartRuntimePrimitives.RequireValue(widget._canRequestFocus);
            }
        }
        _couldRequestFocus = focusNode.canRequestFocus;
        _descendantsWereFocusable = focusNode.descendantsAreFocusable;
        _descendantsWereTraversable = focusNode.descendantsAreTraversable;
        _hadPrimaryFocus = focusNode.hasPrimaryFocus;
        _focusAttachment = focusNode.attach(context, onKeyEvent: widget.onKeyEvent, onKey: widget.onKey);
        focusNode.addListener(_handleFocusChanged);
    }

    internal virtual FocusNode _createNode()
    {
        return new FocusNode(debugLabel: widget.debugLabel, canRequestFocus: widget.canRequestFocus, descendantsAreFocusable: widget.descendantsAreFocusable, descendantsAreTraversable: widget.descendantsAreTraversable, skipTraversal: widget.skipTraversal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        focusNode.removeListener(_handleFocusChanged);
        _focusAttachment!.detach();
        _internalNode?.dispose();
        base.dispose();
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _focusAttachment?.reparent();
        _handleAutofocus();
    }

    internal virtual void _handleAutofocus()
    {
        if (!_didAutofocus && widget.autofocus)
        {
            FocusScope.of(context).autofocus(focusNode);
            _didAutofocus = true;
        }
    }

    public override void deactivate()
    {
        base.deactivate();
        _focusAttachment?.reparent();
        _didAutofocus = false;
    }

    public override void didUpdateWidget(Focus oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        DartRuntimePrimitives.Assert(() =>
            {
                if (Equals(oldWidget.focusNode, widget.focusNode) && !widget._usingExternalFocus && (oldWidget.debugLabel != widget.debugLabel))
                {
                    focusNode.debugLabel = widget.debugLabel;
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        if (Equals(oldWidget.focusNode, widget.focusNode))
        {
            if (!widget._usingExternalFocus)
            {
                if (!Equals(widget.onKey, focusNode.onKey))
                {
                    focusNode.onKey = widget.onKey;
                }
                if (!Equals(widget.onKeyEvent, focusNode.onKeyEvent))
                {
                    focusNode.onKeyEvent = widget.onKeyEvent;
                }
                focusNode.skipTraversal = widget.skipTraversal;
                if (widget._canRequestFocus is not null)
                {
                    focusNode.canRequestFocus = DartRuntimePrimitives.RequireValue(widget._canRequestFocus);
                }
                focusNode.descendantsAreFocusable = widget.descendantsAreFocusable;
                focusNode.descendantsAreTraversable = widget.descendantsAreTraversable;
            }
        }
        else
        {
            _focusAttachment!.detach();
            oldWidget.focusNode?.removeListener(_handleFocusChanged);
            _initNode();
        }
        if (oldWidget.autofocus != widget.autofocus)
        {
            _handleAutofocus();
        }
    }

    internal virtual void _handleFocusChanged()
    {
        bool hasPrimaryFocusLocal = focusNode.hasPrimaryFocus;
        bool canRequestFocusLocal = focusNode.canRequestFocus;
        bool descendantsAreFocusableLocal = focusNode.descendantsAreFocusable;
        bool descendantsAreTraversableLocal = focusNode.descendantsAreTraversable;
        widget.onFocusChange?.Invoke(focusNode.hasFocus);
        if (_hadPrimaryFocus != hasPrimaryFocusLocal)
        {
            setState(() =>
            {
                _hadPrimaryFocus = hasPrimaryFocusLocal;
            });
        }
        if (_couldRequestFocus != canRequestFocusLocal)
        {
            setState(() =>
            {
                _couldRequestFocus = canRequestFocusLocal;
            });
        }
        if (_descendantsWereFocusable != descendantsAreFocusableLocal)
        {
            setState(() =>
            {
                _descendantsWereFocusable = descendantsAreFocusableLocal;
            });
        }
        if (_descendantsWereTraversable != descendantsAreTraversableLocal)
        {
            setState(() =>
            {
                _descendantsWereTraversable = descendantsAreTraversableLocal;
            });
        }
    }

    public override Widget build(BuildContext context)
    {
        _focusAttachment!.reparent(parent: widget.parentNode);
        Widget childLocal = widget.child;
        if (widget.includeSemantics)
        {
            childLocal = DartRuntimePrimitives.ConvertValue<Widget>(new Semantics(
                onFocus: Focus.CreateSemanticsFocusAction(
                    PlatformLibrary.defaultTargetPlatform,
                    _couldRequestFocus,
                    focusNode),
                focusable: _couldRequestFocus,
                focused: _couldRequestFocus ? _hadPrimaryFocus : null,
                child: widget.child));
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if (DebugLibrary.debugPaintFocusBoxes)
                {
                    childLocal = DartRuntimePrimitives.ConvertValue<Widget>(new _DebugFocusBorder__focus_scope(node: focusNode, child: childLocal));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return new _FocusInheritedScope__focus_scope(node: focusNode, child: childLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class FocusScope : Focus
{
    public FocusScope(Key? key = null, FocusScopeNode? node = null, FocusNode? parentNode = null, Widget child = default!, bool autofocus = false, Action<bool>? onFocusChange = null, bool? canRequestFocus = null, bool? skipTraversal = null, Func<FocusNode, KeyEvent, KeyEventResult>? onKeyEvent = null, Func<FocusNode, RawKeyEvent, KeyEventResult>? onKey = null, string? debugLabel = null, bool includeSemantics = true, bool? descendantsAreFocusable = null, bool? descendantsAreTraversable = null) : base(key: key, parentNode: parentNode, child: child, autofocus: autofocus, onFocusChange: onFocusChange, canRequestFocus: canRequestFocus, skipTraversal: skipTraversal, onKeyEvent: onKeyEvent, onKey: onKey, debugLabel: debugLabel, includeSemantics: includeSemantics, descendantsAreFocusable: descendantsAreFocusable, descendantsAreTraversable: descendantsAreTraversable, focusNode: node)
    {
    }

    public static FocusScope CreateWithExternalFocusNode(Key? key = null, Widget child = default!, FocusScopeNode focusScopeNode = default!, FocusNode? parentNode = null, bool autofocus = default!, bool includeSemantics = default!, Action<bool>? onFocusChange = null)
        => new _FocusScopeWithExternalFocusNode__focus_scope(key, child, focusScopeNode, parentNode, autofocus, includeSemantics, onFocusChange);

    public static FocusScopeNode of(BuildContext context, bool createDependency = true)
    {
        return maybeOf(context, scopeOk: true, createDependency: createDependency)?.nearestScope ?? context.owner!.focusManager.rootScope;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override State<Focus> createState() => DartRuntimePrimitives.ConvertValue<State<Focus>>(new _FocusScopeState__focus_scope());
}

internal class _FocusScopeWithExternalFocusNode__focus_scope : FocusScope
{
    internal _FocusScopeWithExternalFocusNode__focus_scope(Key? key = null, Widget child = default!, FocusScopeNode focusScopeNode = default!, FocusNode? parentNode = null, bool autofocus = false, bool includeSemantics = true, Action<bool>? onFocusChange = null) : base(key: key, child: child, parentNode: parentNode, autofocus: autofocus, includeSemantics: includeSemantics, onFocusChange: onFocusChange, node: focusScopeNode)
    {
    }

    internal override bool _usingExternalFocus => true;
    public override Func<FocusNode, KeyEvent, KeyEventResult>? onKeyEvent => focusNode!.onKeyEvent;
    public override Func<FocusNode, RawKeyEvent, KeyEventResult>? onKey => focusNode!.onKey;
    public override bool canRequestFocus => focusNode!.canRequestFocus;
    public override bool skipTraversal => focusNode!.skipTraversal;
    public override bool descendantsAreFocusable => focusNode!.descendantsAreFocusable;
    public override bool descendantsAreTraversable => focusNode!.descendantsAreTraversable;
    public override string? debugLabel => focusNode!.debugLabel;
}

internal class _FocusScopeState__focus_scope : _FocusState__focus_scope
{
    internal override FocusScopeNode _createNode()
    {
        return new FocusScopeNode(debugLabel: widget.debugLabel, canRequestFocus: widget.canRequestFocus, skipTraversal: widget.skipTraversal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        _focusAttachment!.reparent(parent: widget.parentNode);
        Widget result = new _FocusInheritedScope__focus_scope(node: focusNode, child: widget.child);
        if (widget.includeSemantics)
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(new Semantics(explicitChildNodes: true, child: result));
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DebugFocusBorder__focus_scope : StatelessWidget
{
    public virtual FocusNode node { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    internal _DebugFocusBorder__focus_scope(FocusNode node, Widget child)
    {
        this.node = node;
        this.child = child;
    }

    internal virtual Ui.Color _borderColor
    {
        get
        {
            if (node.hasPrimaryFocus)
            {
                return new Ui.Color(4026597120L);
            }
            else
            {
                if (node.hasFocus)
                {
                    return new Ui.Color(4026532095L);
                }
                else
                {
                    if (!node.canRequestFocus)
                    {
                        return new Ui.Color(4043243520L);
                    }
                    else
                    {
                        if (node.skipTraversal)
                        {
                            return new Ui.Color(4043308800L);
                        }
                        else
                        {
                            return new Ui.Color(4026597375L);
                        }
                    }
                }
            }
        }
    }
    public override Widget build(BuildContext context)
    {
        return new ListenableBuilder(listenable: node, builder: (context, _) =>
        {
            return new DecoratedBox(decoration: new BoxDecoration(border: Border.CreateAll(color: _borderColor, width: 3.0)), position: DecorationPosition.foreground, child: child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _FocusInheritedScope__focus_scope : InheritedNotifier<FocusNode>
{
    internal _FocusInheritedScope__focus_scope(FocusNode node, Widget child) : base(child: child, notifier: node)
    {
    }

}

public class ExcludeFocus : StatelessWidget
{
    public virtual bool excluding { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public ExcludeFocus(Key? key = null, bool excluding = true, Widget child = default!) : base(key: key)
    {
        this.excluding = excluding;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return new Focus(canRequestFocus: false, skipTraversal: true, includeSemantics: false, descendantsAreFocusable: !excluding, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
