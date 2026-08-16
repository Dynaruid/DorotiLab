// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/focus_scope.dart
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

public class Focus : StatefulWidget
{
    public virtual FocusNode? parentNode { get; private set; }
    public virtual Widget child { get; private set; } = default!;
    public virtual FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual global::System.Action<bool>? onFocusChange { get; private set; }
    internal virtual global::System.Func<FocusNode, global::Doroti.Framework.Services.KeyEvent, KeyEventResult>? _onKeyEvent { get; private set; }
    internal virtual global::System.Func<FocusNode, global::Doroti.Framework.Services.RawKeyEvent, KeyEventResult>? _onKey { get; private set; }
    internal virtual bool? _canRequestFocus { get; private set; }
    internal virtual bool? _skipTraversal { get; private set; }
    internal virtual bool? _descendantsAreFocusable { get; private set; }
    internal virtual bool? _descendantsAreTraversable { get; private set; }
    public virtual bool includeSemantics { get; private set; } = default!;
    internal virtual string? _debugLabel { get; private set; }

    public Focus(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!, FocusNode? focusNode = null, FocusNode? parentNode = null, bool autofocus = false, global::System.Action<bool>? onFocusChange = null, global::System.Func<FocusNode, global::Doroti.Framework.Services.KeyEvent, KeyEventResult>? onKeyEvent = null, global::System.Func<FocusNode, global::Doroti.Framework.Services.RawKeyEvent, KeyEventResult>? onKey = null, bool? canRequestFocus = null, bool? skipTraversal = null, bool? descendantsAreFocusable = null, bool? descendantsAreTraversable = null, bool includeSemantics = true, string? debugLabel = null) : base(key: key)
    {
        this.child = child;
        this.focusNode = focusNode;
        this.parentNode = parentNode;
        this.autofocus = autofocus;
        this.onFocusChange = onFocusChange;
        this.includeSemantics = includeSemantics;
        this._onKeyEvent = onKeyEvent;
        this._onKey = onKey;
        this._canRequestFocus = canRequestFocus;
        this._skipTraversal = skipTraversal;
        this._descendantsAreFocusable = descendantsAreFocusable;
        this._descendantsAreTraversable = descendantsAreTraversable;
        this._debugLabel = debugLabel;
    }

    public static Focus CreateWithExternalFocusNode(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!, FocusNode focusNode = default!, FocusNode? parentNode = null, bool autofocus = default!, global::System.Action<bool>? onFocusChange = null, bool includeSemantics = default!)
        => ((Focus)(object?)new _FocusWithExternalFocusNode__focus_scope(key, child, focusNode, parentNode, autofocus, onFocusChange, includeSemantics));

    internal virtual bool _usingExternalFocus => false;
    public virtual global::System.Func<FocusNode, global::Doroti.Framework.Services.KeyEvent, KeyEventResult>? onKeyEvent => DartRuntimePrimitives.ConvertValue<global::System.Func<FocusNode, global::Doroti.Framework.Services.KeyEvent, KeyEventResult>>(((this._onKeyEvent ?? (global::System.Func<FocusNode, global::Doroti.Framework.Services.KeyEvent, KeyEventResult>)this.focusNode?.onKeyEvent)));
    public virtual global::System.Func<FocusNode, global::Doroti.Framework.Services.RawKeyEvent, KeyEventResult>? onKey => DartRuntimePrimitives.ConvertValue<global::System.Func<FocusNode, global::Doroti.Framework.Services.RawKeyEvent, KeyEventResult>>(((this._onKey ?? (global::System.Func<FocusNode, global::Doroti.Framework.Services.RawKeyEvent, KeyEventResult>)this.focusNode?.onKey)));
    public virtual bool canRequestFocus => DartRuntimePrimitives.ConvertValue<bool>(((this._canRequestFocus ?? this.focusNode?.canRequestFocus) ?? true));
    public virtual bool skipTraversal => DartRuntimePrimitives.ConvertValue<bool>(((this._skipTraversal ?? this.focusNode?.skipTraversal) ?? false));
    public virtual bool descendantsAreFocusable => DartRuntimePrimitives.ConvertValue<bool>(((this._descendantsAreFocusable ?? this.focusNode?.descendantsAreFocusable) ?? true));
    public virtual bool descendantsAreTraversable => DartRuntimePrimitives.ConvertValue<bool>(((this._descendantsAreTraversable ?? this.focusNode?.descendantsAreTraversable) ?? true));
    public virtual string? debugLabel => DartRuntimePrimitives.ConvertValue<string>((this._debugLabel ?? this.focusNode?.debugLabel));
    public static FocusNode of(BuildContext context, bool scopeOk = false, bool createDependency = true)
    {
        FocusNode? node__17435 = ((FocusNode?)(object?)Focus.maybeOf(context, scopeOk: scopeOk, createDependency: createDependency));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((node__17435 is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("Focus.of() was called with a context that does not contain a Focus widget.\n" + "No Focus widget ancestor could be found starting from the context that was passed to " + "Focus.of(). This can happen because you are using a widget that looks for a Focus " + "ancestor, and do not have a Focus widget descendant in the nearest FocusScope.\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() =>
            {
                if ((!scopeOk && (node__17435 is FocusScopeNode)))
                {
                    FocusScopeNode node__17435__as18136 = (FocusScopeNode)node__17435;
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("Focus.of() was called with a context that does not contain a Focus between the given " + "context and the nearest FocusScope widget.\n" + "No Focus ancestor could be found starting from the context that was passed to " + "Focus.of() to the point where it found the nearest FocusScope widget. This can happen " + "because you are using a widget that looks for a Focus ancestor, and do not have a " + "Focus widget ancestor in the current FocusScope.\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return node__17435!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static FocusNode? maybeOf(BuildContext context, bool scopeOk = false, bool createDependency = true)
    {
        _FocusInheritedScope__focus_scope? scope__19715 = (createDependency ? context.dependOnInheritedWidgetOfExactType<_FocusInheritedScope__focus_scope>() : context.getInheritedWidgetOfExactType<_FocusInheritedScope__focus_scope>());
        return (scope__19715?.notifier switch { null => DartRuntimePrimitives.ConvertValue<FocusNode>(null), FocusScopeNode __object19955 when (!scopeOk) => DartRuntimePrimitives.ConvertValue<FocusNode>(null), FocusNode node__20017 => node__20017 });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool isAt(BuildContext context) => DartRuntimePrimitives.ConvertValue<bool>((Focus.maybeOf(context)?.hasFocus ?? false));
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.StringProperty("debugLabel", this.debugLabel, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("autofocus", value: this.autofocus, ifTrue: "AUTOFOCUS", defaultValue: false));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("canRequestFocus", value: this.canRequestFocus, ifFalse: "NOT FOCUSABLE", defaultValue: false));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("descendantsAreFocusable", value: this.descendantsAreFocusable, ifFalse: "DESCENDANTS UNFOCUSABLE", defaultValue: true));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("descendantsAreTraversable", value: this.descendantsAreTraversable, ifFalse: "DESCENDANTS UNTRAVERSABLE", defaultValue: true));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<FocusNode>("focusNode", this.focusNode, defaultValue: null));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _FocusState__focus_scope());
}

internal class _FocusWithExternalFocusNode__focus_scope : Focus
{
    internal _FocusWithExternalFocusNode__focus_scope(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!, FocusNode focusNode = default!, FocusNode? parentNode = null, bool autofocus = false, global::System.Action<bool>? onFocusChange = null, bool includeSemantics = true) : base(key: key, child: child, focusNode: focusNode, parentNode: parentNode, autofocus: autofocus, onFocusChange: onFocusChange, includeSemantics: includeSemantics)
    {
    }

    internal override bool _usingExternalFocus => true;
    public override global::System.Func<FocusNode, global::Doroti.Framework.Services.KeyEvent, KeyEventResult>? onKeyEvent => this.focusNode!.onKeyEvent;
    public override global::System.Func<FocusNode, global::Doroti.Framework.Services.RawKeyEvent, KeyEventResult>? onKey => this.focusNode!.onKey;
    public override bool canRequestFocus => this.focusNode!.canRequestFocus;
    public override bool skipTraversal => this.focusNode!.skipTraversal;
    public override bool descendantsAreFocusable => this.focusNode!.descendantsAreFocusable;
    internal override bool? _descendantsAreTraversable => this.focusNode!.descendantsAreTraversable;
    public override string? debugLabel => this.focusNode!.debugLabel;
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

    public virtual FocusNode focusNode => DartRuntimePrimitives.ConvertValue<FocusNode>((((Focus)this.widget).focusNode ?? (_internalNode ??= _createNode())));
    public override void initState()
    {
        base.initState();
        _initNode();
    }

    internal virtual void _initNode()
    {
        if (!((Focus)this.widget)._usingExternalFocus)
        {
            this.focusNode.descendantsAreFocusable = ((Focus)this.widget).descendantsAreFocusable;
            this.focusNode.descendantsAreTraversable = ((Focus)this.widget).descendantsAreTraversable;
            this.focusNode.skipTraversal = ((Focus)this.widget).skipTraversal;
            if ((((Focus)this.widget)._canRequestFocus is not null))
            {
                this.focusNode.canRequestFocus = DartRuntimePrimitives.RequireValue(((Focus)this.widget)._canRequestFocus);
            }
        }
        _couldRequestFocus = ((FocusNode)this.focusNode).canRequestFocus;
        _descendantsWereFocusable = ((FocusNode)this.focusNode).descendantsAreFocusable;
        _descendantsWereTraversable = ((FocusNode)this.focusNode).descendantsAreTraversable;
        _hadPrimaryFocus = ((FocusNode)this.focusNode).hasPrimaryFocus;
        _focusAttachment = this.focusNode.attach(this.context, onKeyEvent: (global::System.Func<FocusNode, global::Doroti.Framework.Services.KeyEvent, KeyEventResult>?)((Focus)this.widget).onKeyEvent, onKey: (global::System.Func<FocusNode, global::Doroti.Framework.Services.RawKeyEvent, KeyEventResult>?)((Focus)this.widget).onKey);
        this.focusNode.addListener(this._handleFocusChanged);
    }

    internal virtual FocusNode _createNode()
    {
        return new FocusNode(debugLabel: ((Focus)this.widget).debugLabel, canRequestFocus: ((Focus)this.widget).canRequestFocus, descendantsAreFocusable: ((Focus)this.widget).descendantsAreFocusable, descendantsAreTraversable: ((Focus)this.widget).descendantsAreTraversable, skipTraversal: ((Focus)this.widget).skipTraversal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        this.focusNode.removeListener(this._handleFocusChanged);
        this._focusAttachment!.detach();
        this._internalNode?.dispose();
        base.dispose();
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        this._focusAttachment?.reparent();
        _handleAutofocus();
    }

    internal virtual void _handleAutofocus()
    {
        if ((!this._didAutofocus && ((Focus)this.widget).autofocus))
        {
            FocusScope.of(this.context).autofocus(this.focusNode);
            _didAutofocus = true;
        }
    }

    public override void deactivate()
    {
        base.deactivate();
        this._focusAttachment?.reparent();
        _didAutofocus = false;
    }

    public override void didUpdateWidget(Focus oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        DartRuntimePrimitives.Assert(() =>
            {
                if ((((object.Equals(((Focus)oldWidget).focusNode, ((Focus)this.widget).focusNode)) && !((Focus)this.widget)._usingExternalFocus) && (((Focus)oldWidget).debugLabel != ((Focus)this.widget).debugLabel)))
                {
                    this.focusNode.debugLabel = ((Focus)this.widget).debugLabel;
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        if ((object.Equals(((Focus)oldWidget).focusNode, ((Focus)this.widget).focusNode)))
        {
            if (!((Focus)this.widget)._usingExternalFocus)
            {
                if ((!object.Equals((global::System.Func<FocusNode, global::Doroti.Framework.Services.RawKeyEvent, KeyEventResult>?)((Focus)this.widget).onKey, (global::System.Func<FocusNode, global::Doroti.Framework.Services.RawKeyEvent, KeyEventResult>?)((FocusNode)this.focusNode).onKey)))
                {
                    this.focusNode.onKey = (global::System.Func<FocusNode, global::Doroti.Framework.Services.RawKeyEvent, KeyEventResult>)((Focus)this.widget).onKey;
                }
                if ((!object.Equals((global::System.Func<FocusNode, global::Doroti.Framework.Services.KeyEvent, KeyEventResult>?)((Focus)this.widget).onKeyEvent, (global::System.Func<FocusNode, global::Doroti.Framework.Services.KeyEvent, KeyEventResult>?)((FocusNode)this.focusNode).onKeyEvent)))
                {
                    this.focusNode.onKeyEvent = (global::System.Func<FocusNode, global::Doroti.Framework.Services.KeyEvent, KeyEventResult>)((Focus)this.widget).onKeyEvent;
                }
                this.focusNode.skipTraversal = ((Focus)this.widget).skipTraversal;
                if ((((Focus)this.widget)._canRequestFocus is not null))
                {
                    this.focusNode.canRequestFocus = DartRuntimePrimitives.RequireValue(((Focus)this.widget)._canRequestFocus);
                }
                this.focusNode.descendantsAreFocusable = ((Focus)this.widget).descendantsAreFocusable;
                this.focusNode.descendantsAreTraversable = ((Focus)this.widget).descendantsAreTraversable;
            }
        }
        else
        {
            this._focusAttachment!.detach();
            ((Focus)oldWidget).focusNode?.removeListener(this._handleFocusChanged);
            _initNode();
        }
        if ((((Focus)oldWidget).autofocus != ((Focus)this.widget).autofocus))
        {
            _handleAutofocus();
        }
    }

    internal virtual void _handleFocusChanged()
    {
        bool hasPrimaryFocus__27109 = ((FocusNode)this.focusNode).hasPrimaryFocus;
        bool canRequestFocus__27169 = ((FocusNode)this.focusNode).canRequestFocus;
        bool descendantsAreFocusable__27229 = ((FocusNode)this.focusNode).descendantsAreFocusable;
        bool descendantsAreTraversable__27305 = ((FocusNode)this.focusNode).descendantsAreTraversable;
        ((Focus)this.widget).onFocusChange?.Invoke(((FocusNode)this.focusNode).hasFocus);
        if ((this._hadPrimaryFocus != hasPrimaryFocus__27109))
        {
            setState(((global::System.Action)(() => {
_hadPrimaryFocus = hasPrimaryFocus__27109;
})));
        }
        if ((this._couldRequestFocus != canRequestFocus__27169))
        {
            setState(((global::System.Action)(() => {
_couldRequestFocus = canRequestFocus__27169;
})));
        }
        if ((this._descendantsWereFocusable != descendantsAreFocusable__27229))
        {
            setState(((global::System.Action)(() => {
_descendantsWereFocusable = descendantsAreFocusable__27229;
})));
        }
        if ((this._descendantsWereTraversable != descendantsAreTraversable__27305))
        {
            setState(((global::System.Action)(() => {
_descendantsWereTraversable = descendantsAreTraversable__27305;
})));
        }
    }

    public override Widget build(BuildContext context)
    {
        this._focusAttachment!.reparent(parent: ((Focus)this.widget).parentNode);
        Widget child__28232 = ((Focus)this.widget).child;
        if (((Focus)this.widget).includeSemantics)
        {
            child__28232 = DartRuntimePrimitives.ConvertValue<Widget>(new Semantics(onFocus: () => ((global::System.Action<FocusNode?>)(((!object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS)) && this._couldRequestFocus) ? ((FocusNode)this.focusNode).requestFocus : null))(default), focusable: this._couldRequestFocus, focused: (this._couldRequestFocus ? this._hadPrimaryFocus : null), child: ((Focus)this.widget).child));
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Framework.Widgets.DebugLibrary.debugPaintFocusBoxes)
                {
                    child__28232 = DartRuntimePrimitives.ConvertValue<Widget>(new _DebugFocusBorder__focus_scope(node: this.focusNode, child: child__28232));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((Widget)(object?)new _FocusInheritedScope__focus_scope(node: this.focusNode, child: child__28232));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class FocusScope : Focus
{
    public FocusScope(global::Doroti.Framework.Foundation.Key? key = null, FocusScopeNode? node = null, FocusNode? parentNode = null, Widget child = default!, bool autofocus = false, global::System.Action<bool>? onFocusChange = null, bool? canRequestFocus = null, bool? skipTraversal = null, global::System.Func<FocusNode, global::Doroti.Framework.Services.KeyEvent, KeyEventResult>? onKeyEvent = null, global::System.Func<FocusNode, global::Doroti.Framework.Services.RawKeyEvent, KeyEventResult>? onKey = null, string? debugLabel = null, bool includeSemantics = true, bool? descendantsAreFocusable = null, bool? descendantsAreTraversable = null) : base(key: key, parentNode: parentNode, child: child, autofocus: autofocus, onFocusChange: onFocusChange, canRequestFocus: canRequestFocus, skipTraversal: skipTraversal, onKeyEvent: onKeyEvent, onKey: onKey, debugLabel: debugLabel, includeSemantics: includeSemantics, descendantsAreFocusable: descendantsAreFocusable, descendantsAreTraversable: descendantsAreTraversable, focusNode: node)
    {
    }

    public static FocusScope CreateWithExternalFocusNode(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!, FocusScopeNode focusScopeNode = default!, FocusNode? parentNode = null, bool autofocus = default!, bool includeSemantics = default!, global::System.Action<bool>? onFocusChange = null)
        => ((FocusScope)(object?)new _FocusScopeWithExternalFocusNode__focus_scope(key, child, focusScopeNode, parentNode, autofocus, includeSemantics, onFocusChange));

    public static FocusScopeNode of(BuildContext context, bool createDependency = true)
    {
        return (Focus.maybeOf(context, scopeOk: true, createDependency: createDependency)?.nearestScope ?? ((BuildContext)context).owner!.focusManager.rootScope);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override State<Focus> createState() => DartRuntimePrimitives.ConvertValue<State<Focus>>(new _FocusScopeState__focus_scope());
}

internal class _FocusScopeWithExternalFocusNode__focus_scope : FocusScope
{
    internal _FocusScopeWithExternalFocusNode__focus_scope(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!, FocusScopeNode focusScopeNode = default!, FocusNode? parentNode = null, bool autofocus = false, bool includeSemantics = true, global::System.Action<bool>? onFocusChange = null) : base(key: key, child: child, parentNode: parentNode, autofocus: autofocus, includeSemantics: includeSemantics, onFocusChange: onFocusChange, node: focusScopeNode)
    {
    }

    internal override bool _usingExternalFocus => true;
    public override global::System.Func<FocusNode, global::Doroti.Framework.Services.KeyEvent, KeyEventResult>? onKeyEvent => this.focusNode!.onKeyEvent;
    public override global::System.Func<FocusNode, global::Doroti.Framework.Services.RawKeyEvent, KeyEventResult>? onKey => this.focusNode!.onKey;
    public override bool canRequestFocus => this.focusNode!.canRequestFocus;
    public override bool skipTraversal => this.focusNode!.skipTraversal;
    public override bool descendantsAreFocusable => this.focusNode!.descendantsAreFocusable;
    public override bool descendantsAreTraversable => this.focusNode!.descendantsAreTraversable;
    public override string? debugLabel => this.focusNode!.debugLabel;
}

internal class _FocusScopeState__focus_scope : _FocusState__focus_scope
{
    internal override FocusScopeNode _createNode()
    {
        return new FocusScopeNode(debugLabel: this.widget.debugLabel, canRequestFocus: this.widget.canRequestFocus, skipTraversal: this.widget.skipTraversal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        this._focusAttachment!.reparent(parent: this.widget.parentNode);
        Widget result__35353 = ((Widget)(object?)new _FocusInheritedScope__focus_scope(node: this.focusNode, child: ((Widget)((dynamic)this.widget).child)));
        if (this.widget.includeSemantics)
        {
            result__35353 = DartRuntimePrimitives.ConvertValue<Widget>(new Semantics(explicitChildNodes: true, child: result__35353));
        }
        return result__35353;
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

    internal virtual global::Doroti.Ui.Color _borderColor
    {
        get
        {
            if (((FocusNode)this.node).hasPrimaryFocus)
            {
                return new global::Doroti.Ui.Color(4026597120L);
            }
            else
            {
                if (((FocusNode)this.node).hasFocus)
                {
                    return new global::Doroti.Ui.Color(4026532095L);
                }
                else
                {
                    if (!((FocusNode)this.node).canRequestFocus)
                    {
                        return new global::Doroti.Ui.Color(4043243520L);
                    }
                    else
                    {
                        if (((FocusNode)this.node).skipTraversal)
                        {
                            return new global::Doroti.Ui.Color(4043308800L);
                        }
                        else
                        {
                            return new global::Doroti.Ui.Color(4026597375L);
                        }
                    }
                }
            }
            return default!;
        }
    }
    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new ListenableBuilder(listenable: this.node, builder: ((global::System.Func<BuildContext, Widget?, Widget>)((context, _) => {
return ((Widget)(object?)new DecoratedBox(decoration: new global::Doroti.Framework.Painting.BoxDecoration(border: global::Doroti.Framework.Painting.Border.CreateAll(color: this._borderColor, width: 3.0)), position: global::Doroti.Framework.Rendering.DecorationPosition.foreground, child: this.child));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
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

    public ExcludeFocus(global::Doroti.Framework.Foundation.Key? key = null, bool excluding = true, Widget child = default!) : base(key: key)
    {
        this.excluding = excluding;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new Focus(canRequestFocus: false, skipTraversal: true, includeSemantics: false, descendantsAreFocusable: !this.excluding, child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
