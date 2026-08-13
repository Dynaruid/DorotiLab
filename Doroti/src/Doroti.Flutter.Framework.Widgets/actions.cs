// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/actions.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public static partial class ActionsLibrary
{
    internal static BuildContext _getParent(BuildContext context)
    {
        BuildContext parent__1462 = default!;
        context.visitAncestorElements(((global::System.Func<Element, bool>)((ancestor) => {
parent__1462 = DartRuntimePrimitives.ConvertValue<BuildContext>(ancestor);
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        return parent__1462;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public abstract class Intent : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public static DoNothingIntent doNothing = new DoNothingIntent();

    protected Intent()
    {
    }

    public virtual string toStringShort() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((fullString__105654 ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
    }

}

public delegate void ActionListenerCallback(dynamic action);

public interface IActionListenerSource
{
    void addActionListener(global::System.Action<dynamic> listener);
    void removeActionListener(global::System.Action<dynamic> listener);
}

public abstract class Action<T> : global::Doroti.Generated.Framework.Foundation.Diagnosticable, IActionListenerSource where T : Intent
{
    internal virtual global::Doroti.Generated.Framework.Foundation.ObserverList<global::System.Action<dynamic>> _listeners { get; private set; } = new global::Doroti.Generated.Framework.Foundation.ObserverList<global::System.Action<dynamic>>();
    internal virtual dynamic _currentCallingAction { get; set; } = default!;

    protected Action()
    {
    }

    public static Action<T> CreateOverridable(Action<T> defaultAction, BuildContext context)
    {
        return ((Action<T>)(object?)defaultAction._makeOverridableAction(context));
    }

    internal virtual void _updateCallingAction(dynamic value)
    {
        _currentCallingAction = value;
    }

    internal virtual bool _debugCanHandleIntent<I>(I? intent) where I : Intent
    {
        object? badIntentString__8900 = (intent switch { T __object8942 => (object?)null, object runtimeType__8975 => (object?)runtimeType__8975, null when ((new List<I>() is List<T>)) => (object?)null, null => (object?)typeof(I).ToString() });
        DartRuntimePrimitives.Assert(() => (badIntentString__8900 is null), () => (object?)$"An Intent of type {badIntentString__8900} cannot be handled by {this.GetType()}: the Intent must be of a subtype of {typeof(T)}.");
        return (badIntentString__8900 is null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Action<T>? callingAction => ((Action<T>?)(object?)this._currentCallingAction)!;
    public virtual Type intentType => typeof(T);
    public virtual bool isEnabled(T intent, BuildContext? context = null) => this.isActionEnabled;
    internal virtual bool _isEnabled(T intent, BuildContext? context) => (this switch { ContextAction<T> action__11816 => action__11816.isEnabled(intent, context), _ => isEnabled(intent) });
    public virtual bool isActionEnabled => true;
    public virtual bool consumesKey(T intent) => true;
    public virtual KeyEventResult toKeyEventResult(T intent, object? invokeResult)
    {
        return (consumesKey(intent) ? KeyEventResult.handled : KeyEventResult.skipRemainingHandlers);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract object? invoke(T intent, BuildContext? context = null);
    internal virtual object? _invoke(T intent, BuildContext? context) => (this switch { ContextAction<T> action__15245 => (object?)action__15245.invoke(intent, context), _ => (object?)invoke(intent) });
    public virtual void addActionListener(global::System.Action<dynamic> listener) => this._listeners.add((global::System.Action<dynamic>)listener);
    public virtual void removeActionListener(global::System.Action<dynamic> listener) => this._listeners.remove((global::System.Action<dynamic>)listener);
    public virtual void notifyActionListeners()
    {
        if (!System.Linq.Enumerable.Any(this._listeners))
        {
            return;
        }
        var localListeners__18249 = new List<global::System.Action<dynamic>>(this._listeners);
        foreach (var listener__18326 in localListeners__18249)
        {
            InformationCollector? collector__18384 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    collector__18384 = (() => new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<Action<T>>($"The {this.GetType()} sending notification was", this, style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) });
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            try
            {
                if (this._listeners.contains((global::System.Action<dynamic>)listener__18326))
                {
                    listener__18326(this);
                }
            }
            catch (Exception exception__18795)
            {
                var stack__18806 = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: exception__18795, stack: stack__18806, library: "widgets library", context: new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"while dispatching notifications for {this.GetType()}"), informationCollector: (InformationCollector?)collector__18384));
            }
        }
    }

    internal virtual Action<T> _makeOverridableAction(BuildContext context)
    {
        return ((Action<T>)(object?)new _OverridableAction__actions<T>(defaultAction: this, lookupContext: context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string toStringShort() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((fullString__105654 ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
    }

}

public class ActionListener : StatefulWidget
{
    public virtual global::System.Action<dynamic> listener { get; private set; } = default!;
    public virtual dynamic action { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public ActionListener(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Action<dynamic> listener = default!, dynamic action = default!, Widget child = default!) : base(key: key)
    {
        this.listener = listener;
        this.action = action;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ActionListenerState__actions());
}

internal class _ActionListenerState__actions : State<ActionListener>
{
    public override void initState()
    {
        base.initState();
        ((dynamic)((ActionListener)(object)this.widget).action).addActionListener((global::System.Action<dynamic>)((ActionListener)(object)this.widget).listener);
    }

    public override void didUpdateWidget(ActionListener oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (((object.Equals(((ActionListener)oldWidget).action, ((ActionListener)(object)this.widget).action)) && (object.Equals((global::System.Action<dynamic>)((ActionListener)oldWidget).listener, (global::System.Action<dynamic>)((ActionListener)(object)this.widget).listener))))
        {
            return;
        }
        ((dynamic)((ActionListener)oldWidget).action).removeActionListener((global::System.Action<dynamic>)((ActionListener)oldWidget).listener);
        ((dynamic)((ActionListener)(object)this.widget).action).addActionListener((global::System.Action<dynamic>)((ActionListener)(object)this.widget).listener);
    }

    public override void dispose()
    {
        ((dynamic)((ActionListener)(object)this.widget).action).removeActionListener((global::System.Action<dynamic>)((ActionListener)(object)this.widget).listener);
        base.dispose();
    }

    public override Widget build(BuildContext context) => ((ActionListener)(object)this.widget).child;
}

public abstract class ContextAction<T> : Action<T> where T : Intent
{
    public override bool isEnabled(T intent, BuildContext? context = null) => base.isEnabled(intent);
    public abstract override object? invoke(T intent, BuildContext? context = null);
    internal override ContextAction<T> _makeOverridableAction(BuildContext context)
    {
        return ((ContextAction<T>)(object?)new _OverridableContextAction__actions<T>(defaultAction: this, lookupContext: context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public delegate object? OnInvokeCallback<T>(T intent) where T : Intent;

public class CallbackAction<T> : Action<T> where T : Intent
{
    public virtual global::System.Func<T, object?> onInvoke { get; private set; } = default!;

    public CallbackAction(global::System.Func<T, object?> onInvoke)
    {
        this.onInvoke = onInvoke;
    }

    public CallbackAction(global::System.Action<Intent?> onInvoke)
    {
        this.onInvoke = intent => { onInvoke(intent); return null; };
    }

    public CallbackAction(global::System.Action<T> onInvoke)
    {
        this.onInvoke = intent => { onInvoke(intent); return null; };
    }

    public override object? invoke(T intent, BuildContext? context = null) => this.onInvoke(intent);
}

public class ActionDispatcher : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{

    public ActionDispatcher()
    {
    }

    public virtual object? invokeAction(dynamic action, Intent intent, BuildContext? context = null)
    {
        BuildContext? target__26983 = (context ?? global::Doroti.Generated.Framework.Widgets.Focus_managerLibrary.primaryFocus?.context);
        DartRuntimePrimitives.Assert(() => ((bool)((dynamic)action)._isEnabled(intent, target__26983)), () => (object?)"Action must be enabled when calling invokeAction");
        return ((dynamic)action)._invoke(intent, target__26983);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual (bool, object?) invokeActionIfEnabled(dynamic action, Intent intent, BuildContext? context = null)
    {
        BuildContext? target__28148 = (context ?? global::Doroti.Generated.Framework.Widgets.Focus_managerLibrary.primaryFocus?.context);
        if (((bool)((dynamic)action)._isEnabled(intent, target__28148)))
        {
            return (true, ((dynamic)action)._invoke(intent, target__28148));
        }
        return (false, null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string toStringShort() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((fullString__105654 ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
    }

}

public class Actions : StatefulWidget
{
    public virtual ActionDispatcher? dispatcher { get; private set; }
    public virtual DartMap<Type, dynamic> actions { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public Actions(global::Doroti.Generated.Framework.Foundation.Key? key = null, ActionDispatcher? dispatcher = null, DartMap<Type, dynamic> actions = default!, Widget child = default!) : base(key: key)
    {
        this.dispatcher = dispatcher;
        this.actions = actions;
        this.child = child;
    }

    internal static bool _visitActionsAncestors(BuildContext context, global::System.Func<InheritedElement, bool> visitor)
    {
        if (!((BuildContext)context).mounted)
        {
            return false;
        }
        InheritedElement? actionsElement__31546 = ((InheritedElement?)(object?)context.getElementForInheritedWidgetOfExactType<_ActionsScope__actions>());
        while ((actionsElement__31546 is not null))
        {
            if (visitor(actionsElement__31546))
            {
                break;
            }
            BuildContext parent__31929 = ActionsLibrary._getParent(actionsElement__31546);
            actionsElement__31546 = parent__31929.getElementForInheritedWidgetOfExactType<_ActionsScope__actions>();
        }
        return (actionsElement__31546 is not null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static ActionDispatcher _findDispatcher(BuildContext context)
    {
        ActionDispatcher? dispatcher__32285 = default!;
        Actions._visitActionsAncestors(context, ((global::System.Func<InheritedElement, bool>)((element) => {
ActionDispatcher? found__32392 = (((_ActionsScope__actions?)(object?)element.widget)!).dispatcher;
if ((found__32392 is not null))
{
    dispatcher__32285 = found__32392;
    return true;
}
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        return (dispatcher__32285 ?? new ActionDispatcher());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::System.Action? handler<T>(BuildContext context, T intent) where T : Intent
    {
        dynamic action__33514 = Actions.maybeFind<T>(context);
        if (((action__33514 is not null) && ((bool)((dynamic)action__33514)._isEnabled(intent, context))))
        {
            return ((global::System.Action)(() => {
if (((bool)((dynamic)action__33514)._isEnabled(intent, context)))
{
    Actions.of(context).invokeAction(action__33514, intent, context);
}
}));
        }
        return ((global::System.Action)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Action<T> find<T>(BuildContext context, T? intent = default) where T : Intent
    {
        Action<T>? action__34857 = ((Action<T>?)(object?)Actions.maybeFind<T>(context, intent: intent));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((action__34857 is null))
                {
                    Type type__34966 = (DartRuntimePrimitives.RuntimeType(intent) ?? typeof(T));
                    throw DartRuntimePrimitives.AsException(global::Doroti.Generated.Framework.Foundation.FlutterError.Create($"Unable to find an action for a {type__34966} in an {typeof(Actions)} widget " + "in the given context.\n" + $"{typeof(Actions)}.find() was called on a context that doesn't contain an " + $"{typeof(Actions)} widget with a mapping for the given intent type.\n" + "The context used was:\n" + $"  {context}\n" + "The intent type requested was:\n" + $"  {type__34966}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return action__34857!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Action<T>? maybeFind<T>(BuildContext context, T? intent = default) where T : Intent
    {
        dynamic action__37058 = default!;
        Actions._visitActionsAncestors(context, ((global::System.Func<InheritedElement, bool>)((element) => {
var actions__37143 = ((_ActionsScope__actions?)(object?)element.widget)!;
dynamic result__37214 = Actions._getActionForIntent<T>(actions__37143, intent);
if ((result__37214 is not null))
{
    context.dependOnInheritedElement(element);
    action__37058 = result__37214;
    return true;
}
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        if (action__37058 is Action<T> action__37464)
        {
            return action__37464;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"An {DartRuntimePrimitives.RuntimeType(action__37058)} cannot be cast to an Action<{typeof(T)}>."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"A valid action {action__37058} was found but could not be returned by Actions.maybeFind<{typeof(T)}>."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("This is a current limitation of the Actions widget, " + "see https://github.com/flutter/flutter/issues/180871 for more details. " + "As a workaround, consider using Actions.invoke or Actions.maybeInvoke instead, " + "or explicitly set the type parameter to Intent: " + "Actions.maybeFind<Intent>(context, intent)") }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((Action<T>)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static dynamic _maybeFindWithoutDependingOn<T>(BuildContext context, T? intent) where T : Intent
    {
        dynamic action__38359 = default!;
        Actions._visitActionsAncestors(context, ((global::System.Func<InheritedElement, bool>)((element) => {
var actions__38445 = ((_ActionsScope__actions?)(object?)element.widget)!;
dynamic result__38516 = Actions._getActionForIntent<T>(actions__38445, intent);
if ((result__38516 is not null))
{
    action__38359 = result__38516;
    return true;
}
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        return action__38359;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static dynamic _getActionForIntent<T>(_ActionsScope__actions actionsMarker, T? intent) where T : Intent
    {
        dynamic mappedAction__38845 = ((_ActionsScope__actions)actionsMarker).actions.GetValueOrDefault((DartRuntimePrimitives.RuntimeType(intent) ?? typeof(T)));
        DartRuntimePrimitives.Assert(() => (((bool?)((dynamic)mappedAction__38845)?._debugCanHandleIntent(intent)) ?? true));
        return mappedAction__38845;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ActionDispatcher of(BuildContext context)
    {
        _ActionsScope__actions? marker__39321 = ((_ActionsScope__actions?)(object?)context.dependOnInheritedWidgetOfExactType<_ActionsScope__actions>());
        return ((marker__39321?.dispatcher ?? (ActionDispatcher)Actions._findDispatcher(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static object? invoke<T>(BuildContext context, T intent) where T : Intent
    {
        object? returnValue__40136 = default!;
        bool actionFound__40165 = Actions._visitActionsAncestors(context, ((global::System.Func<InheritedElement, bool>)((element) => {
var actions__40252 = ((_ActionsScope__actions?)(object?)element.widget)!;
dynamic result__40323 = Actions._getActionForIntent(actions__40252, intent);
if (((result__40323 is not null) && ((bool)((dynamic)result__40323)._isEnabled(intent, context))))
{
    returnValue__40136 = Actions._findDispatcher(element).invokeAction(result__40323, intent, context);
}
return (result__40323 is not null);
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        DartRuntimePrimitives.Assert(() =>
            {
                if (!actionFound__40165)
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Generated.Framework.Foundation.FlutterError.Create("Unable to find an action for an Intent with type " + $"{DartRuntimePrimitives.RuntimeType(intent)} in an {typeof(Actions)} widget in the given context.\n" + $"{typeof(Actions)}.invoke() was unable to find an {typeof(Actions)} widget that " + "contained a mapping for the given intent, or the intent type isn't the " + $"same as the type argument to invoke (which is {typeof(T)} - try supplying a " + "type argument to invoke if one was not given)\n" + "The context used was:\n" + $"  {context}\n" + "The intent type requested was:\n" + $"  {DartRuntimePrimitives.RuntimeType(intent)}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return returnValue__40136;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static object? maybeInvoke<T>(BuildContext context, T intent) where T : Intent
    {
        object? returnValue__42255 = default!;
        Actions._visitActionsAncestors(context, ((global::System.Func<InheritedElement, bool>)((element) => {
var actions__42345 = ((_ActionsScope__actions?)(object?)element.widget)!;
dynamic result__42416 = Actions._getActionForIntent(actions__42345, intent);
if (((result__42416 is not null) && ((bool)((dynamic)result__42416)._isEnabled(intent, context))))
{
    returnValue__42255 = Actions._findDispatcher(element).invokeAction(result__42416, intent, context);
}
return (result__42416 is not null);
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        return returnValue__42255;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ActionsState__actions());
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<ActionDispatcher>("dispatcher", this.dispatcher));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<DartMap<Type, object>>("actions", this.actions));
    }

}

internal class _ActionsState__actions : State<Actions>
{
    public virtual HashSet<dynamic>? listenedActions { get; set; } = new HashSet<dynamic>();
    public virtual object rebuildKey { get; set; } = new object();

    public override void initState()
    {
        base.initState();
        _updateActionListeners();
    }

    internal virtual void _handleActionChanged(dynamic action)
    {
        setState(((global::System.Action)(() => {
rebuildKey = new object();
})));
    }

    internal virtual void _updateActionListeners()
    {
        HashSet<object> widgetActions__43828 = ((HashSet<object>)(object?)((Actions)(object)this.widget).actions.Values.toSet());
        HashSet<object> removedActions__43905 = ((HashSet<object>)(object?)this.listenedActions!.difference<dynamic>(widgetActions__43828));
        HashSet<object> addedActions__43996 = ((HashSet<object>)(object?)widgetActions__43828.difference<dynamic>(this.listenedActions!));
        foreach (var action__44071 in removedActions__43905)
        {
            if (action__44071 is IActionListenerSource source)
                source.removeActionListener((global::System.Action<dynamic>)this._handleActionChanged);
        }
        foreach (var action__44177 in addedActions__43996)
        {
            if (action__44177 is IActionListenerSource source)
                source.addActionListener((global::System.Action<dynamic>)this._handleActionChanged);
        }
        listenedActions = widgetActions__43828;
    }

    public override void didUpdateWidget(Actions oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        _updateActionListeners();
    }

    public override void dispose()
    {
        base.dispose();
        foreach (dynamic action__44516 in this.listenedActions!)
        {
            if (action__44516 is IActionListenerSource source)
                source.removeActionListener((global::System.Action<dynamic>)this._handleActionChanged);
        }
        listenedActions = null;
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new _ActionsScope__actions(actions: ((Actions)(object)this.widget).actions, dispatcher: ((Actions)(object)this.widget).dispatcher, rebuildKey: this.rebuildKey, child: ((Actions)(object)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ActionsScope__actions : InheritedWidget
{
    public virtual ActionDispatcher? dispatcher { get; private set; }
    public virtual DartMap<Type, dynamic> actions { get; private set; } = default!;
    public virtual object rebuildKey { get; private set; } = default!;

    internal _ActionsScope__actions(ActionDispatcher? dispatcher, DartMap<Type, dynamic> actions, object rebuildKey, Widget child) : base(child: child)
    {
        this.dispatcher = dispatcher;
        this.actions = actions;
        this.rebuildKey = rebuildKey;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (_ActionsScope__actions)(object)oldWidget;
        return (((!object.Equals(this.rebuildKey, ((_ActionsScope__actions)__oldWidget).rebuildKey)) || (!object.Equals(((_ActionsScope__actions)__oldWidget).dispatcher, this.dispatcher))) || !global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.mapEquals<Type, object>(((_ActionsScope__actions)__oldWidget).actions, this.actions));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class FocusableActionDetector : StatefulWidget
{
    public virtual bool enabled { get; private set; } = default!;
    public virtual FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual bool descendantsAreFocusable { get; private set; } = default!;
    public virtual bool descendantsAreTraversable { get; private set; } = default!;
    public virtual DartMap<Type, dynamic>? actions { get; private set; }
    public virtual DartMap<ShortcutActivator, Intent>? shortcuts { get; private set; }
    public virtual global::System.Action<bool>? onShowFocusHighlight { get; private set; }
    public virtual global::System.Action<bool>? onShowHoverHighlight { get; private set; }
    public virtual global::System.Action<bool>? onFocusChange { get; private set; }
    public virtual global::Doroti.Generated.Framework.Services.MouseCursor mouseCursor { get; private set; } = default!;
    public virtual bool includeFocusSemantics { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public FocusableActionDetector(global::Doroti.Generated.Framework.Foundation.Key? key = null, bool enabled = true, FocusNode? focusNode = null, bool autofocus = false, bool descendantsAreFocusable = true, bool descendantsAreTraversable = true, DartMap<ShortcutActivator, Intent>? shortcuts = null, DartMap<Type, dynamic>? actions = null, global::System.Action<bool>? onShowFocusHighlight = null, global::System.Action<bool>? onShowHoverHighlight = null, global::System.Action<bool>? onFocusChange = null, global::Doroti.Generated.Framework.Services.MouseCursor mouseCursor = default!, bool includeFocusSemantics = true, Widget child = default!) : base(key: key)
    {
        global::Doroti.Generated.Framework.Services.MouseCursor __mouseCursor = mouseCursor ?? global::Doroti.Generated.Framework.Services.MouseCursor.defer;
        this.enabled = enabled;
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.descendantsAreFocusable = descendantsAreFocusable;
        this.descendantsAreTraversable = descendantsAreTraversable;
        this.shortcuts = shortcuts;
        this.actions = actions;
        this.onShowFocusHighlight = onShowFocusHighlight;
        this.onShowHoverHighlight = onShowHoverHighlight;
        this.onFocusChange = onFocusChange;
        this.mouseCursor = __mouseCursor;
        this.includeFocusSemantics = includeFocusSemantics;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _FocusableActionDetectorState__actions());
}

internal class _FocusableActionDetectorState__actions : State<FocusableActionDetector>
{
    internal virtual bool _canShowHighlight { get; set; } = false;
    internal virtual bool _hovering { get; set; } = false;
    internal virtual bool _focused { get; set; } = false;
    internal virtual GlobalKey<IState> _mouseRegionKey { get; private set; } = GlobalKey<IState>.Create();

    public override void initState()
    {
        base.initState();
        global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((duration) => {
_updateHighlightMode(FocusManager.instance.highlightMode);
})), debugLabel: "FocusableActionDetector.updateHighlightMode");
        FocusManager.instance.addHighlightModeListener((global::System.Action<FocusHighlightMode>)this._handleFocusHighlightModeChange);
    }

    public override void dispose()
    {
        FocusManager.instance.removeHighlightModeListener((global::System.Action<FocusHighlightMode>)this._handleFocusHighlightModeChange);
        base.dispose();
    }

    internal virtual void _updateHighlightMode(FocusHighlightMode mode)
    {
        _mayTriggerCallback(task: ((global::System.Action)(() => {
_canShowHighlight = (FocusManager.instance.highlightMode switch { FocusHighlightMode.touch => false, FocusHighlightMode.traditional => true, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
})));
    }

    internal virtual void _handleFocusHighlightModeChange(FocusHighlightMode mode)
    {
        if (!this.mounted)
        {
            return;
        }
        _updateHighlightMode(mode);
    }

    internal virtual void _handleMouseEnter(global::Doroti.Generated.Framework.Gestures.PointerEnterEvent @event)
    {
        if (!this._hovering)
        {
            _mayTriggerCallback(task: ((global::System.Action)(() => {
_hovering = true;
})));
        }
    }

    internal virtual void _handleMouseExit(global::Doroti.Generated.Framework.Gestures.PointerExitEvent @event)
    {
        if (this._hovering)
        {
            _mayTriggerCallback(task: ((global::System.Action)(() => {
_hovering = false;
})));
        }
    }

    internal virtual void _handleFocusChange(bool focused)
    {
        if ((this._focused != focused))
        {
            _mayTriggerCallback(task: ((global::System.Action)(() => {
_focused = focused;
})));
            ((FocusableActionDetector)(object)this.widget).onFocusChange?.Invoke(this._focused);
        }
    }

    internal virtual void _mayTriggerCallback(global::System.Action? task = null, FocusableActionDetector? oldWidget = null)
    {
        bool shouldShowHoverHighlight(FocusableActionDetector target)
        {
            return ((this._hovering && ((FocusableActionDetector)target).enabled) && this._canShowHighlight);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        bool canRequestFocus(FocusableActionDetector target)
        {
            return (MediaQuery.maybeNavigationModeOf(this.context) switch { NavigationMode.traditional => ((FocusableActionDetector)target).enabled, null => ((FocusableActionDetector)target).enabled, NavigationMode.directional => true, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        bool shouldShowFocusHighlight(FocusableActionDetector target)
        {
            return ((this._focused && this._canShowHighlight) && canRequestFocus(target));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        DartRuntimePrimitives.Assert(() => (!object.Equals(global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase, global::Doroti.Generated.Framework.Scheduler.SchedulerPhase.persistentCallbacks)));
        FocusableActionDetector oldTarget__52743 = ((oldWidget ?? (FocusableActionDetector)this.widget));
        bool didShowHoverHighlight__52791 = shouldShowHoverHighlight(oldTarget__52743);
        bool didShowFocusHighlight__52867 = shouldShowFocusHighlight(oldTarget__52743);
        task?.Invoke();
        bool doShowHoverHighlight__52961 = shouldShowHoverHighlight(this.widget);
        bool doShowFocusHighlight__53033 = shouldShowFocusHighlight(this.widget);
        if ((didShowFocusHighlight__52867 != doShowFocusHighlight__53033))
        {
            ((FocusableActionDetector)(object)this.widget).onShowFocusHighlight?.Invoke(doShowFocusHighlight__53033);
        }
        if ((didShowHoverHighlight__52791 != doShowHoverHighlight__52961))
        {
            ((FocusableActionDetector)(object)this.widget).onShowHoverHighlight?.Invoke(doShowHoverHighlight__52961);
        }
    }

    public override void didUpdateWidget(FocusableActionDetector oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((((FocusableActionDetector)(object)this.widget).enabled != ((FocusableActionDetector)oldWidget).enabled))
        {
            global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((duration) => {
_mayTriggerCallback(oldWidget: oldWidget);
})), debugLabel: "FocusableActionDetector.mayTriggerCallback");
        }
    }

    internal virtual bool _canRequestFocus => (MediaQuery.maybeNavigationModeOf(this.context) switch { NavigationMode.traditional => ((FocusableActionDetector)(object)this.widget).enabled, null => ((FocusableActionDetector)(object)this.widget).enabled, NavigationMode.directional => true, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    public override Widget build(BuildContext context)
    {
        Widget child__54279 = ((Widget)(object?)new MouseRegion(key: this._mouseRegionKey, onEnter: (global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerEnterEvent>)this._handleMouseEnter, onExit: (global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerExitEvent>)this._handleMouseExit, cursor: ((FocusableActionDetector)(object)this.widget).mouseCursor, child: new Focus(focusNode: ((FocusableActionDetector)(object)this.widget).focusNode, autofocus: ((FocusableActionDetector)(object)this.widget).autofocus, descendantsAreFocusable: ((FocusableActionDetector)(object)this.widget).descendantsAreFocusable, descendantsAreTraversable: ((FocusableActionDetector)(object)this.widget).descendantsAreTraversable, canRequestFocus: this._canRequestFocus, onFocusChange: (global::System.Action<bool>)this._handleFocusChange, includeSemantics: ((FocusableActionDetector)(object)this.widget).includeFocusSemantics, child: ((FocusableActionDetector)(object)this.widget).child)));
        if (((((FocusableActionDetector)(object)this.widget).enabled && (((FocusableActionDetector)(object)this.widget).actions is not null)) && System.Linq.Enumerable.Any(((FocusableActionDetector)(object)this.widget).actions!)))
        {
            child__54279 = DartRuntimePrimitives.ConvertValue<Widget>(new Actions(actions: ((FocusableActionDetector)(object)this.widget).actions!, child: child__54279));
        }
        if (((((FocusableActionDetector)(object)this.widget).enabled && (((FocusableActionDetector)(object)this.widget).shortcuts is not null)) && System.Linq.Enumerable.Any(((FocusableActionDetector)(object)this.widget).shortcuts!)))
        {
            child__54279 = DartRuntimePrimitives.ConvertValue<Widget>(new Shortcuts(shortcuts: ((FocusableActionDetector)(object)this.widget).shortcuts!, child: child__54279));
        }
        return child__54279;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class VoidCallbackIntent : Intent
{
    public virtual global::System.Action callback { get; private set; } = default!;

    public VoidCallbackIntent(global::System.Action callback)
    {
        this.callback = callback;
    }

}

public class VoidCallbackAction : Action<VoidCallbackIntent>
{
    public override object? invoke(VoidCallbackIntent intent, BuildContext? context = null)
    {
        intent.callback();
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DoNothingIntent : Intent
{
    public static DoNothingIntent Create()
        => new DoNothingIntent();

    public DoNothingIntent()
    {
    }

}

public class DoNothingAndStopPropagationIntent : Intent
{
    public static DoNothingAndStopPropagationIntent Create()
        => new DoNothingAndStopPropagationIntent();

    public DoNothingAndStopPropagationIntent()
    {
    }

}

public class DoNothingAction : Action<Intent>
{
    internal virtual bool _consumesKey { get; private set; } = default!;

    public DoNothingAction(bool consumesKey = true)
    {
        this._consumesKey = consumesKey;
    }

    public override bool consumesKey(Intent intent) => this._consumesKey;
    public override object? invoke(Intent intent, BuildContext? context = null)
    {
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ActivateIntent : Intent
{
    public ActivateIntent()
    {
    }

}

public class ButtonActivateIntent : Intent
{
    public ButtonActivateIntent()
    {
    }

}

public abstract class ActivateAction : Action<ActivateIntent>
{
}

public class SelectIntent : Intent
{
    public SelectIntent()
    {
    }

}

public abstract class SelectAction : Action<SelectIntent>
{
}

public class DismissIntent : Intent
{
    public DismissIntent()
    {
    }

}

public abstract class DismissAction : Action<DismissIntent>
{
}

public class PrioritizedIntents : Intent
{
    public virtual List<Intent> orderedIntents { get; private set; } = default!;

    public PrioritizedIntents(List<Intent> orderedIntents)
    {
        this.orderedIntents = orderedIntents;
    }

}

public class PrioritizedAction : ContextAction<PrioritizedIntents>
{
    internal virtual Action<Intent> _selectedAction { get; set; } = default!;
    internal virtual Intent _selectedIntent { get; set; } = default!;

    public override bool isEnabled(PrioritizedIntents intent, BuildContext? context = null)
    {
        FocusNode? focus__63783 = global::Doroti.Generated.Framework.Widgets.Focus_managerLibrary.primaryFocus;
        if (((focus__63783 is null) || (((FocusNode)focus__63783).context is null)))
        {
            return false;
        }
        foreach (Intent candidateIntent__63903 in ((PrioritizedIntents)intent).orderedIntents)
        {
            dynamic candidateAction__63975 = Actions.maybeFind(((FocusNode)focus__63783).context!, intent: DartRuntimePrimitives.ConvertValue<PrioritizedIntents>(candidateIntent__63903));
            if (((candidateAction__63975 is not null) && ((bool)((dynamic)candidateAction__63975)._isEnabled(candidateIntent__63903, context))))
            {
                _selectedAction = DartRuntimePrimitives.ConvertValue<Action<Intent>>(candidateAction__63975);
                _selectedIntent = candidateIntent__63903;
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override object? invoke(PrioritizedIntents intent, BuildContext? context = null)
    {
        this._selectedAction._invoke(this._selectedIntent, context);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal interface _OverridableActionMixin__actions<T> where T : Intent
{
    bool _debugAssertMutuallyRecursive { get; set; }
    bool _debugAssertIsActionEnabledMutuallyRecursive { get; set; }
    bool _debugAssertIsEnabledMutuallyRecursive { get; set; }
    bool _debugAssertConsumeKeyMutuallyRecursive { get; set; }

    public Action<T> _defaultAction { get; }
    public BuildContext _lookupContext { get; }
    public object? _invokeDefaultAction(T intent, dynamic fromAction, BuildContext? context);
    public dynamic _getOverrideAction<U>(U? intent, bool declareDependency = false);
    public void _updateCallingAction(dynamic value);
    public object? _invokeOverride(dynamic overrideAction, T intent, BuildContext? context);
    public object? invoke(T intent, BuildContext? context = null);
    public bool _isOverrideActionEnabled(dynamic overrideAction);
    public bool isActionEnabled { get; }
    public bool isEnabled(T intent, BuildContext? context = null);
    public bool consumesKey(T intent);
    public void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties);
}

internal class _OverridableAction__actions<T> : ContextAction<T> where T : Intent
{
    internal virtual Action<T> _defaultAction { get; private set; } = default!;
    internal virtual BuildContext _lookupContext { get; private set; } = default!;
    public virtual bool _debugAssertMutuallyRecursive { get; set; } = false;
    public virtual bool _debugAssertIsActionEnabledMutuallyRecursive { get; set; } = false;
    public virtual bool _debugAssertIsEnabledMutuallyRecursive { get; set; } = false;
    public virtual bool _debugAssertConsumeKeyMutuallyRecursive { get; set; } = false;

    internal _OverridableAction__actions(Action<T> defaultAction, BuildContext lookupContext)
    {
        this._lookupContext = lookupContext;
        this._defaultAction = defaultAction;
    }

    public virtual object? _invokeDefaultAction(T intent, dynamic fromAction, BuildContext? context)
    {
        if ((fromAction is null))
        {
            return this._defaultAction.invoke(intent);
        }
        else
        {
            object? returnValue__69692 = this._defaultAction.invoke(intent);
            return returnValue__69692;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override ContextAction<T> _makeOverridableAction(BuildContext context)
    {
        return ((ContextAction<T>)(object?)new _OverridableAction__actions<T>(defaultAction: this._defaultAction, lookupContext: context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual dynamic _getOverrideAction<U>(U? intent, bool declareDependency = false) where U : Intent
    {
        dynamic @override__65515 = (declareDependency ? Actions.maybeFind<U>(this._lookupContext, intent: intent) : Actions._maybeFindWithoutDependingOn(this._lookupContext, intent));
        DartRuntimePrimitives.Assert(() => !DartRuntimePrimitives.Identical(@override__65515, this));
        return @override__65515;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override void _updateCallingAction(dynamic value)
    {
        base._updateCallingAction((object?)value);
        this._defaultAction._updateCallingAction(value);
    }

    public virtual object? _invokeOverride(dynamic overrideAction, T intent, BuildContext? context)
    {
        DartRuntimePrimitives.Assert(() => !this._debugAssertMutuallyRecursive);
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugAssertMutuallyRecursive = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        ((dynamic)overrideAction)._updateCallingAction(this._defaultAction);
        object? returnValue__66199 = ((dynamic)overrideAction)._invoke(intent, context);
        ((dynamic)overrideAction)._updateCallingAction(null);
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugAssertMutuallyRecursive = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return returnValue__66199;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override object? invoke(T intent, BuildContext? context = null)
    {
        dynamic overrideAction__66512 = _getOverrideAction(intent);
        object? returnValue__66575 = ((overrideAction__66512 is null) ? _invokeDefaultAction(intent, this._currentCallingAction, context) : _invokeOverride(overrideAction__66512, intent, context));
        return returnValue__66575;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _isOverrideActionEnabled(dynamic overrideAction)
    {
        DartRuntimePrimitives.Assert(() => !this._debugAssertIsActionEnabledMutuallyRecursive);
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugAssertIsActionEnabledMutuallyRecursive = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        ((dynamic)overrideAction)._updateCallingAction(this._defaultAction);
        bool isOverrideEnabled__67072 = ((bool)((dynamic)overrideAction).isActionEnabled);
        ((dynamic)overrideAction)._updateCallingAction(null);
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugAssertIsActionEnabledMutuallyRecursive = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return isOverrideEnabled__67072;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isActionEnabled
    {
        get
        {
            dynamic overrideAction__67378 = _getOverrideAction<T>(default, declareDependency: true);
            bool returnValue__67464 = ((overrideAction__67378 is not null) ? _isOverrideActionEnabled(overrideAction__67378) : ((Action<T>)this._defaultAction).isActionEnabled);
            return returnValue__67464;
            return default!;
        }
    }
    public override bool isEnabled(T intent, BuildContext? context = null)
    {
        DartRuntimePrimitives.Assert(() => !this._debugAssertIsEnabledMutuallyRecursive);
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugAssertIsEnabledMutuallyRecursive = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        dynamic overrideAction__67867 = _getOverrideAction(intent);
        DartRuntimePrimitives.Assert(() => (((bool?)((dynamic)overrideAction__67867)?._debugCanHandleIntent(intent)) ?? true));
        ((dynamic)overrideAction__67867)?._updateCallingAction(this._defaultAction);
        bool returnValue__68052 = ((bool)((dynamic)((((object?)overrideAction__67867 ?? (object?)this._defaultAction))))._isEnabled(intent, context));
        ((dynamic)overrideAction__67867)?._updateCallingAction(null);
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugAssertIsEnabledMutuallyRecursive = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return returnValue__68052;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool consumesKey(T intent)
    {
        DartRuntimePrimitives.Assert(() => !this._debugAssertConsumeKeyMutuallyRecursive);
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugAssertConsumeKeyMutuallyRecursive = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        dynamic overrideAction__68528 = _getOverrideAction(intent);
        ((dynamic)overrideAction__68528)?._updateCallingAction(this._defaultAction);
        bool isEnabled__68646 = ((bool)((dynamic)((((object?)overrideAction__68528 ?? (object?)this._defaultAction)))).consumesKey(intent));
        ((dynamic)overrideAction__68528)?._updateCallingAction(null);
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugAssertConsumeKeyMutuallyRecursive = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return isEnabled__68646;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<Action<T>>("defaultAction", this._defaultAction));
    }

}

internal class _OverridableContextAction__actions<T> : ContextAction<T> where T : Intent
{
    internal virtual ContextAction<T> _defaultAction { get; private set; } = default!;
    internal virtual BuildContext _lookupContext { get; private set; } = default!;
    public virtual bool _debugAssertMutuallyRecursive { get; set; } = false;
    public virtual bool _debugAssertIsActionEnabledMutuallyRecursive { get; set; } = false;
    public virtual bool _debugAssertIsEnabledMutuallyRecursive { get; set; } = false;
    public virtual bool _debugAssertConsumeKeyMutuallyRecursive { get; set; } = false;

    internal _OverridableContextAction__actions(ContextAction<T> defaultAction, BuildContext lookupContext)
    {
        this._lookupContext = lookupContext;
        this._defaultAction = defaultAction;
    }

    public virtual object? _invokeOverride(dynamic overrideAction, T intent, BuildContext? context)
    {
        DartRuntimePrimitives.Assert(() => (context is not null));
        DartRuntimePrimitives.Assert(() => !this._debugAssertMutuallyRecursive);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugAssertMutuallyRecursive = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => ((bool)((dynamic)overrideAction)._debugCanHandleIntent(intent)));
        Action<T> wrappedDefault__70883 = ((Action<T>)(object?)new _ContextActionToActionAdapter__actions<T>(invokeContext: context!, action: this._defaultAction));
        ((dynamic)overrideAction)._updateCallingAction(wrappedDefault__70883);
        object? returnValue__71077 = ((dynamic)overrideAction)._invoke(intent, context);
        ((dynamic)overrideAction)._updateCallingAction(null);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugAssertMutuallyRecursive = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return returnValue__71077;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual object? _invokeDefaultAction(T intent, dynamic fromAction, BuildContext? context)
    {
        if ((fromAction is null))
        {
            return this._defaultAction.invoke(intent, context);
        }
        else
        {
            object? returnValue__71521 = this._defaultAction.invoke(intent, context);
            return returnValue__71521;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override ContextAction<T> _makeOverridableAction(BuildContext context)
    {
        return ((ContextAction<T>)(object?)new _OverridableContextAction__actions<T>(defaultAction: this._defaultAction, lookupContext: context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual dynamic _getOverrideAction<U>(U? intent, bool declareDependency = false) where U : Intent
    {
        dynamic @override__65515 = (declareDependency ? Actions.maybeFind<U>(this._lookupContext, intent: intent) : Actions._maybeFindWithoutDependingOn(this._lookupContext, intent));
        DartRuntimePrimitives.Assert(() => !DartRuntimePrimitives.Identical(@override__65515, this));
        return @override__65515;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override void _updateCallingAction(dynamic value)
    {
        base._updateCallingAction((object?)value);
        this._defaultAction._updateCallingAction(value);
    }

    public override object? invoke(T intent, BuildContext? context = null)
    {
        dynamic overrideAction__66512 = _getOverrideAction(intent);
        object? returnValue__66575 = ((overrideAction__66512 is null) ? _invokeDefaultAction(intent, this._currentCallingAction, context) : _invokeOverride(overrideAction__66512, intent, context));
        return returnValue__66575;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _isOverrideActionEnabled(dynamic overrideAction)
    {
        DartRuntimePrimitives.Assert(() => !this._debugAssertIsActionEnabledMutuallyRecursive);
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugAssertIsActionEnabledMutuallyRecursive = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        ((dynamic)overrideAction)._updateCallingAction(this._defaultAction);
        bool isOverrideEnabled__67072 = ((bool)((dynamic)overrideAction).isActionEnabled);
        ((dynamic)overrideAction)._updateCallingAction(null);
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugAssertIsActionEnabledMutuallyRecursive = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return isOverrideEnabled__67072;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isActionEnabled
    {
        get
        {
            dynamic overrideAction__67378 = _getOverrideAction<T>(default, declareDependency: true);
            bool returnValue__67464 = ((overrideAction__67378 is not null) ? _isOverrideActionEnabled(overrideAction__67378) : ((Action<T>)this._defaultAction).isActionEnabled);
            return returnValue__67464;
            return default!;
        }
    }
    public override bool isEnabled(T intent, BuildContext? context = null)
    {
        DartRuntimePrimitives.Assert(() => !this._debugAssertIsEnabledMutuallyRecursive);
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugAssertIsEnabledMutuallyRecursive = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        dynamic overrideAction__67867 = _getOverrideAction(intent);
        DartRuntimePrimitives.Assert(() => (((bool?)((dynamic)overrideAction__67867)?._debugCanHandleIntent(intent)) ?? true));
        ((dynamic)overrideAction__67867)?._updateCallingAction(this._defaultAction);
        bool returnValue__68052 = ((bool)((dynamic)((((object?)overrideAction__67867 ?? (object?)this._defaultAction))))._isEnabled(intent, context));
        ((dynamic)overrideAction__67867)?._updateCallingAction(null);
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugAssertIsEnabledMutuallyRecursive = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return returnValue__68052;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool consumesKey(T intent)
    {
        DartRuntimePrimitives.Assert(() => !this._debugAssertConsumeKeyMutuallyRecursive);
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugAssertConsumeKeyMutuallyRecursive = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        dynamic overrideAction__68528 = _getOverrideAction(intent);
        ((dynamic)overrideAction__68528)?._updateCallingAction(this._defaultAction);
        bool isEnabled__68646 = ((bool)((dynamic)((((object?)overrideAction__68528 ?? (object?)this._defaultAction)))).consumesKey(intent));
        ((dynamic)overrideAction__68528)?._updateCallingAction(null);
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugAssertConsumeKeyMutuallyRecursive = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return isEnabled__68646;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<Action<T>>("defaultAction", this._defaultAction));
    }

}

internal class _ContextActionToActionAdapter__actions<T> : Action<T> where T : Intent
{
    public virtual BuildContext invokeContext { get; private set; } = default!;
    public virtual ContextAction<T> action { get; private set; } = default!;

    internal _ContextActionToActionAdapter__actions(BuildContext invokeContext, ContextAction<T> action)
    {
        this.invokeContext = invokeContext;
        this.action = action;
    }

    internal override void _updateCallingAction(dynamic value)
    {
        this.action._updateCallingAction(value);
    }

    public override Action<T>? callingAction => this.action.callingAction;
    public override bool isEnabled(T intent, BuildContext? context = null) => this.action.isEnabled(intent, this.invokeContext);
    public override bool isActionEnabled => this.action.isActionEnabled;
    public override bool consumesKey(T intent) => this.action.consumesKey(intent);
    public override void addActionListener(global::System.Action<dynamic> listener)
    {
        base.addActionListener((global::System.Action<dynamic>)listener);
        this.action.addActionListener((global::System.Action<dynamic>)listener);
    }

    public override void removeActionListener(global::System.Action<dynamic> listener)
    {
        base.removeActionListener((global::System.Action<dynamic>)listener);
        this.action.removeActionListener((global::System.Action<dynamic>)listener);
    }

    public override void notifyActionListeners() => this.action.notifyActionListeners();
    public override object? invoke(T intent, BuildContext? context = null) => this.action.invoke(intent, this.invokeContext);
}
