// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/actions.dart
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

public static partial class ActionsLibrary
{
    internal static BuildContext _getParent(BuildContext context)
    {
        BuildContext parent = default!;
        context.visitAncestorElements(((global::System.Func<Element, bool>)((ancestor) =>
        {
            parent = DartRuntimePrimitives.ConvertValue<BuildContext>(ancestor);
            return false;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        return parent;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public abstract class Intent : global::Doroti.Framework.Foundation.Diagnosticable
{
    public static DoNothingIntent doNothing = new DoNothingIntent();

    protected Intent()
    {
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((fullString ?? (string)toStringShort()));
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

public delegate void ActionListenerCallback(object action);

public interface IActionListenerSource
{
    void addActionListener(global::System.Action<object> listener);
    void removeActionListener(global::System.Action<object> listener);
}

public interface IIntentAction : IActionListenerSource
{
    Type IntentType { get; }
    IIntentAction? CallingAction { get; }
    bool IsActionEnabled { get; }
    bool DebugCanHandleIntent(Intent? intent);
    bool IsEnabledForIntent(Intent intent, BuildContext? context);
    object? InvokeIntent(Intent intent, BuildContext? context);
    bool ConsumesKeyForIntent(Intent intent);
    void UpdateCallingAction(IIntentAction? value);
}

public abstract class Action<T> : global::Doroti.Framework.Foundation.Diagnosticable, IActionListenerSource, IIntentAction where T : Intent
{
    internal virtual global::Doroti.Framework.Foundation.ObserverList<global::System.Action<object>> _listeners { get; private set; } = new global::Doroti.Framework.Foundation.ObserverList<global::System.Action<object>>();
    internal virtual IIntentAction? _currentCallingAction { get; set; }

    protected Action()
    {
    }

    public static Action<T> CreateOverridable(Action<T> defaultAction, BuildContext context)
    {
        return ((Action<T>)(object?)defaultAction._makeOverridableAction(context));
    }

    internal virtual void _updateCallingAction(IIntentAction? value)
    {
        _currentCallingAction = value;
    }

    internal virtual bool _debugCanHandleIntent<I>(I? intent) where I : Intent
    {
        object? badIntentString = (intent switch { T __object8942 => (object?)null, object runtimeType => (object?)runtimeType, null when ((new List<I>() is List<T>)) => (object?)null, null => (object?)typeof(I).ToString() });
        DartRuntimePrimitives.Assert(() => (badIntentString is null), () => (object?)$"An Intent of type {badIntentString} cannot be handled by {this.GetType()}: the Intent must be of a subtype of {typeof(T)}.");
        return (badIntentString is null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Action<T>? callingAction => ((Action<T>?)(object?)this._currentCallingAction)!;
    public virtual Type intentType => typeof(T);
    public virtual bool isEnabled(T intent, BuildContext? context = null) => this.isActionEnabled;
    internal virtual bool _isEnabled(T intent, BuildContext? context) => (this switch { ContextAction<T> action => action.isEnabled(intent, context), _ => isEnabled(intent) });
    public virtual bool isActionEnabled => true;
    public virtual bool consumesKey(T intent) => true;
    public virtual KeyEventResult toKeyEventResult(T intent, object? invokeResult)
    {
        return (consumesKey(intent) ? KeyEventResult.handled : KeyEventResult.skipRemainingHandlers);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract object? invoke(T intent, BuildContext? context = null);
    internal virtual object? _invoke(T intent, BuildContext? context) => (this switch { ContextAction<T> action => (object?)action.invoke(intent, context), _ => (object?)invoke(intent) });
    bool IIntentAction.IsEnabledForIntent(Intent intent, BuildContext? context) => _isEnabled((T)intent, context);
    object? IIntentAction.InvokeIntent(Intent intent, BuildContext? context) => _invoke((T)intent, context);
    Type IIntentAction.IntentType => intentType;
    IIntentAction? IIntentAction.CallingAction => this._currentCallingAction;
    bool IIntentAction.IsActionEnabled => isActionEnabled;
    bool IIntentAction.DebugCanHandleIntent(Intent? intent) => _debugCanHandleIntent(intent);
    bool IIntentAction.ConsumesKeyForIntent(Intent intent) => consumesKey((T)intent);
    void IIntentAction.UpdateCallingAction(IIntentAction? value) => _updateCallingAction(value);
    public virtual void addActionListener(global::System.Action<object> listener) => this._listeners.add(listener);
    public virtual void removeActionListener(global::System.Action<object> listener) => this._listeners.remove(listener);
    public virtual void notifyActionListeners()
    {
        if (!System.Linq.Enumerable.Any(this._listeners))
        {
            return;
        }
        var localListeners = new List<global::System.Action<object>>(this._listeners);
        foreach (var listener in localListeners)
        {
            InformationCollector? collector = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    collector = (() => new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.DiagnosticsProperty<Action<T>>($"The {this.GetType()} sending notification was", this, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) });
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            try
            {
                if (this._listeners.contains(listener))
                {
                    listener(this);
                }
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exceptionLocal, stack: stackLocal, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription($"while dispatching notifications for {this.GetType()}"), informationCollector: (InformationCollector?)collector));
            }
        }
    }

    internal virtual Action<T> _makeOverridableAction(BuildContext context)
    {
        return ((Action<T>)(object?)new _OverridableAction__actions<T>(defaultAction: this, lookupContext: context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((fullString ?? (string)toStringShort()));
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
    public virtual global::System.Action<object> listener { get; private set; } = default!;
    public virtual dynamic action { get; private set; } = default!;
    internal virtual IActionListenerSource listenerSource { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public ActionListener(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action<object> listener = default!, dynamic action = default!, Widget child = default!) : base(key: key)
    {
        this.listener = listener;
        this.action = action;
        this.listenerSource = (object?)action as IActionListenerSource ?? throw new ArgumentException("ActionListener.action must implement IActionListenerSource.", nameof(action));
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ActionListenerState__actions());
}

internal class _ActionListenerState__actions : State<ActionListener>
{
    public override void initState()
    {
        base.initState();
        ((ActionListener)(object)this.widget).listenerSource.addActionListener(((ActionListener)(object)this.widget).listener);
    }

    public override void didUpdateWidget(ActionListener oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (((object.Equals(((ActionListener)oldWidget).listenerSource, ((ActionListener)(object)this.widget).listenerSource)) && (object.Equals(((ActionListener)oldWidget).listener, ((ActionListener)(object)this.widget).listener))))
        {
            return;
        }
        ((ActionListener)oldWidget).listenerSource.removeActionListener(((ActionListener)oldWidget).listener);
        ((ActionListener)(object)this.widget).listenerSource.addActionListener(((ActionListener)(object)this.widget).listener);
    }

    public override void dispose()
    {
        ((ActionListener)(object)this.widget).listenerSource.removeActionListener(((ActionListener)(object)this.widget).listener);
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

public class ActionDispatcher : global::Doroti.Framework.Foundation.Diagnosticable
{

    public ActionDispatcher()
    {
    }

    public virtual object? invokeAction(dynamic action, Intent intent, BuildContext? context = null)
    {
        BuildContext? target = (context ?? global::Doroti.Framework.Widgets.Focus_managerLibrary.primaryFocus?.context);
        var intentAction = Actions._requireAction((object?)action);
        DartRuntimePrimitives.Assert(() => intentAction.IsEnabledForIntent(intent, target), () => (object?)"Action must be enabled when calling invokeAction");
        return intentAction.InvokeIntent(intent, target);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual (bool, object?) invokeActionIfEnabled(dynamic action, Intent intent, BuildContext? context = null)
    {
        BuildContext? target = (context ?? global::Doroti.Framework.Widgets.Focus_managerLibrary.primaryFocus?.context);
        var intentAction = Actions._requireAction((object?)action);
        if (intentAction.IsEnabledForIntent(intent, target))
        {
            return (true, intentAction.InvokeIntent(intent, target));
        }
        return (false, null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((fullString ?? (string)toStringShort()));
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
    internal virtual DartMap<Type, IIntentAction> typedActions { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public Actions(global::Doroti.Framework.Foundation.Key? key = null, ActionDispatcher? dispatcher = null, DartMap<Type, dynamic> actions = default!, Widget child = default!) : base(key: key)
    {
        this.dispatcher = dispatcher;
        this.actions = actions;
        this.typedActions = _normalizeActions(actions);
        this.child = child;
    }

    internal static IIntentAction _requireAction(object? action, Type? intentType = null)
    {
        if (action is IIntentAction typedAction)
        {
            if (intentType is not null && !typedAction.IntentType.IsAssignableFrom(intentType))
            {
                throw new ArgumentException($"Action for {intentType} declares intent type {typedAction.IntentType}.", nameof(action));
            }
            return typedAction;
        }
        throw new ArgumentException("Actions values must derive from Action<T>.", nameof(action));
    }

    internal static DartMap<Type, IIntentAction> _normalizeActions(DartMap<Type, dynamic> actions)
    {
        var result = new DartMap<Type, IIntentAction>();
        foreach (KeyValuePair<Type, dynamic> entry in actions)
        {
            result[entry.Key] = _requireAction((object?)entry.Value, entry.Key);
        }
        return result;
    }

    internal static bool _visitActionsAncestors(BuildContext context, global::System.Func<InheritedElement, bool> visitor)
    {
        if (!((BuildContext)context).mounted)
        {
            return false;
        }
        InheritedElement? actionsElement = ((InheritedElement?)(object?)context.getElementForInheritedWidgetOfExactType<_ActionsScope__actions>());
        while ((actionsElement is not null))
        {
            if (visitor(actionsElement))
            {
                break;
            }
            BuildContext parent = ActionsLibrary._getParent(actionsElement);
            actionsElement = parent.getElementForInheritedWidgetOfExactType<_ActionsScope__actions>();
        }
        return (actionsElement is not null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static ActionDispatcher _findDispatcher(BuildContext context)
    {
        ActionDispatcher? dispatcherLocal = default!;
        Actions._visitActionsAncestors(context, ((global::System.Func<InheritedElement, bool>)((element) =>
        {
            ActionDispatcher? found = (((_ActionsScope__actions?)(object?)element.widget)!).dispatcher;
            if ((found is not null))
            {
                dispatcherLocal = found;
                return true;
            }
            return false;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        return (dispatcherLocal ?? new ActionDispatcher());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::System.Action? handler<T>(BuildContext context, T intent) where T : Intent
    {
        IIntentAction? action = Actions.maybeFind<T>(context, intent);
        if (((action is not null) && action.IsEnabledForIntent(intent, context)))
        {
            return ((global::System.Action)(() =>
            {
                if (action.IsEnabledForIntent(intent, context))
                {
                    Actions.of(context).invokeAction(action, intent, context);
                }
            }));
        }
        return ((global::System.Action)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Action<T> find<T>(BuildContext context, T? intent = default) where T : Intent
    {
        Action<T>? action = ((Action<T>?)(object?)Actions.maybeFind<T>(context, intent: intent));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((action is null))
                {
                    Type @type = (DartRuntimePrimitives.RuntimeType(intent) ?? typeof(T));
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create($"Unable to find an action for a {@type} in an {typeof(Actions)} widget " + "in the given context.\n" + $"{typeof(Actions)}.find() was called on a context that doesn't contain an " + $"{typeof(Actions)} widget with a mapping for the given intent type.\n" + "The context used was:\n" + $"  {context}\n" + "The intent type requested was:\n" + $"  {@type}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return action!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Action<T>? maybeFind<T>(BuildContext context, T? intent = default) where T : Intent
    {
        IIntentAction? action = default;
        Actions._visitActionsAncestors(context, ((global::System.Func<InheritedElement, bool>)((element) =>
        {
            var actions = ((_ActionsScope__actions?)(object?)element.widget)!;
            IIntentAction? result = Actions._getActionForIntent<T>(actions, intent);
            if ((result is not null))
            {
                context.dependOnInheritedElement(element);
                action = result;
                return true;
            }
            return false;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        if (action is Action<T> actionLocal)
        {
            return actionLocal;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"An {DartRuntimePrimitives.RuntimeType(action)} cannot be cast to an Action<{typeof(T)}>."), new global::Doroti.Framework.Foundation.ErrorDescription($"A valid action {action} was found but could not be returned by Actions.maybeFind<{typeof(T)}>."), new global::Doroti.Framework.Foundation.ErrorHint("This is a current limitation of the Actions widget, " + "see https://github.com/flutter/flutter/issues/180871 for more details. " + "As a workaround, consider using Actions.invoke or Actions.maybeInvoke instead, " + "or explicitly set the type parameter to Intent: " + "Actions.maybeFind<Intent>(context, intent)") }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return ((Action<T>)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static IIntentAction? _maybeFindWithoutDependingOn<T>(BuildContext context, T? intent, bool declareDependency = false) where T : Intent
    {
        IIntentAction? action = default;
        Actions._visitActionsAncestors(context, ((global::System.Func<InheritedElement, bool>)((element) =>
        {
            var actions = ((_ActionsScope__actions?)(object?)element.widget)!;
            IIntentAction? result = Actions._getActionForIntent<T>(actions, intent);
            if ((result is not null))
            {
                if (declareDependency)
                {
                    context.dependOnInheritedElement(element);
                }
                action = result;
                return true;
            }
            return false;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        return action;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static IIntentAction? _getActionForIntent<T>(_ActionsScope__actions actionsMarker, T? intent) where T : Intent
    {
        IIntentAction? mappedAction = ((_ActionsScope__actions)actionsMarker).actions.GetValueOrDefault((DartRuntimePrimitives.RuntimeType(intent) ?? typeof(T)));
        DartRuntimePrimitives.Assert(() => mappedAction?.DebugCanHandleIntent(intent) ?? true);
        return mappedAction;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ActionDispatcher of(BuildContext context)
    {
        _ActionsScope__actions? marker = ((_ActionsScope__actions?)(object?)context.dependOnInheritedWidgetOfExactType<_ActionsScope__actions>());
        return ((marker?.dispatcher ?? (ActionDispatcher)Actions._findDispatcher(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static object? invoke<T>(BuildContext context, T intent) where T : Intent
    {
        object? returnValue = default!;
        bool actionFound = Actions._visitActionsAncestors(context, ((global::System.Func<InheritedElement, bool>)((element) =>
        {
            var actions = ((_ActionsScope__actions?)(object?)element.widget)!;
            IIntentAction? result = Actions._getActionForIntent(actions, intent);
            if (((result is not null) && result.IsEnabledForIntent(intent, context)))
            {
                returnValue = Actions._findDispatcher(element).invokeAction(result, intent, context);
            }
            return (result is not null);
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        DartRuntimePrimitives.Assert(() =>
            {
                if (!actionFound)
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("Unable to find an action for an Intent with type " + $"{DartRuntimePrimitives.RuntimeType(intent)} in an {typeof(Actions)} widget in the given context.\n" + $"{typeof(Actions)}.invoke() was unable to find an {typeof(Actions)} widget that " + "contained a mapping for the given intent, or the intent type isn't the " + $"same as the type argument to invoke (which is {typeof(T)} - try supplying a " + "type argument to invoke if one was not given)\n" + "The context used was:\n" + $"  {context}\n" + "The intent type requested was:\n" + $"  {DartRuntimePrimitives.RuntimeType(intent)}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return returnValue;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static object? maybeInvoke<T>(BuildContext context, T intent) where T : Intent
    {
        object? returnValue = default!;
        Actions._visitActionsAncestors(context, ((global::System.Func<InheritedElement, bool>)((element) =>
        {
            var actions = ((_ActionsScope__actions?)(object?)element.widget)!;
            IIntentAction? result = Actions._getActionForIntent(actions, intent);
            if (((result is not null) && result.IsEnabledForIntent(intent, context)))
            {
                returnValue = Actions._findDispatcher(element).invokeAction(result, intent, context);
            }
            return (result is not null);
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        return returnValue;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ActionsState__actions());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ActionDispatcher>("dispatcher", this.dispatcher));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<DartMap<Type, object>>("actions", this.actions));
    }

}

internal class _ActionsState__actions : State<Actions>
{
    public virtual HashSet<IIntentAction>? listenedActions { get; set; } = new HashSet<IIntentAction>();
    public virtual object rebuildKey { get; set; } = new object();

    public override void initState()
    {
        base.initState();
        _updateActionListeners();
    }

    internal virtual void _handleActionChanged(object action)
    {
        setState(((global::System.Action)(() =>
        {
            rebuildKey = new object();
        })));
    }

    internal virtual void _updateActionListeners()
    {
        HashSet<IIntentAction> widgetActions = ((Actions)(object)this.widget).typedActions.Values.ToHashSet();
        HashSet<IIntentAction> removedActions = this.listenedActions!.Except(widgetActions).ToHashSet();
        HashSet<IIntentAction> addedActions = widgetActions.Except(this.listenedActions!).ToHashSet();
        foreach (var action in removedActions)
        {
            action.removeActionListener(this._handleActionChanged);
        }
        foreach (var actionLocal in addedActions)
        {
            actionLocal.addActionListener(this._handleActionChanged);
        }
        listenedActions = widgetActions;
    }

    public override void didUpdateWidget(Actions oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        _updateActionListeners();
    }

    public override void dispose()
    {
        base.dispose();
        foreach (IIntentAction action in this.listenedActions!)
        {
            action.removeActionListener(this._handleActionChanged);
        }
        listenedActions = null;
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new _ActionsScope__actions(actions: ((Actions)(object)this.widget).typedActions, dispatcher: ((Actions)(object)this.widget).dispatcher, rebuildKey: this.rebuildKey, child: ((Actions)(object)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ActionsScope__actions : InheritedWidget
{
    public virtual ActionDispatcher? dispatcher { get; private set; }
    public virtual DartMap<Type, IIntentAction> actions { get; private set; } = default!;
    public virtual object rebuildKey { get; private set; } = default!;

    internal _ActionsScope__actions(ActionDispatcher? dispatcher, DartMap<Type, IIntentAction> actions, object rebuildKey, Widget child) : base(child: child)
    {
        this.dispatcher = dispatcher;
        this.actions = actions;
        this.rebuildKey = rebuildKey;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (_ActionsScope__actions)(object)oldWidget;
        bool actionsEqual = ((_ActionsScope__actions)__oldWidget).actions.Count == this.actions.Count && ((_ActionsScope__actions)__oldWidget).actions.All(entry => this.actions.ContainsKey(entry.Key) && object.Equals(this.actions.GetValueOrDefault(entry.Key), entry.Value));
        return (((!object.Equals(this.rebuildKey, ((_ActionsScope__actions)__oldWidget).rebuildKey)) || (!object.Equals(((_ActionsScope__actions)__oldWidget).dispatcher, this.dispatcher))) || !actionsEqual);
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
    public virtual global::Doroti.Framework.Services.MouseCursor mouseCursor { get; private set; } = default!;
    public virtual bool includeFocusSemantics { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public FocusableActionDetector(global::Doroti.Framework.Foundation.Key? key = null, bool enabled = true, FocusNode? focusNode = null, bool autofocus = false, bool descendantsAreFocusable = true, bool descendantsAreTraversable = true, DartMap<ShortcutActivator, Intent>? shortcuts = null, DartMap<Type, dynamic>? actions = null, global::System.Action<bool>? onShowFocusHighlight = null, global::System.Action<bool>? onShowHoverHighlight = null, global::System.Action<bool>? onFocusChange = null, global::Doroti.Framework.Services.MouseCursor mouseCursor = default!, bool includeFocusSemantics = true, Widget child = default!) : base(key: key)
    {
        global::Doroti.Framework.Services.MouseCursor __mouseCursor = mouseCursor ?? global::Doroti.Framework.Services.MouseCursor.defer;
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
        global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((duration) =>
        {
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
        _mayTriggerCallback(task: ((global::System.Action)(() =>
        {
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

    internal virtual void _handleMouseEnter(global::Doroti.Framework.Gestures.PointerEnterEvent @event)
    {
        if (!this._hovering)
        {
            _mayTriggerCallback(task: ((global::System.Action)(() =>
            {
                _hovering = true;
            })));
        }
    }

    internal virtual void _handleMouseExit(global::Doroti.Framework.Gestures.PointerExitEvent @event)
    {
        if (this._hovering)
        {
            _mayTriggerCallback(task: ((global::System.Action)(() =>
            {
                _hovering = false;
            })));
        }
    }

    internal virtual void _handleFocusChange(bool focused)
    {
        if ((this._focused != focused))
        {
            _mayTriggerCallback(task: ((global::System.Action)(() =>
            {
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
        DartRuntimePrimitives.Assert(() => (!object.Equals(global::Doroti.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase, global::Doroti.Framework.Scheduler.SchedulerPhase.persistentCallbacks)));
        FocusableActionDetector oldTarget = ((oldWidget ?? (FocusableActionDetector)this.widget));
        bool didShowHoverHighlight = shouldShowHoverHighlight(oldTarget);
        bool didShowFocusHighlight = shouldShowFocusHighlight(oldTarget);
        task?.Invoke();
        bool doShowHoverHighlight = shouldShowHoverHighlight(this.widget);
        bool doShowFocusHighlight = shouldShowFocusHighlight(this.widget);
        if ((didShowFocusHighlight != doShowFocusHighlight))
        {
            ((FocusableActionDetector)(object)this.widget).onShowFocusHighlight?.Invoke(doShowFocusHighlight);
        }
        if ((didShowHoverHighlight != doShowHoverHighlight))
        {
            ((FocusableActionDetector)(object)this.widget).onShowHoverHighlight?.Invoke(doShowHoverHighlight);
        }
    }

    public override void didUpdateWidget(FocusableActionDetector oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((((FocusableActionDetector)(object)this.widget).enabled != ((FocusableActionDetector)oldWidget).enabled))
        {
            global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((duration) =>
            {
                _mayTriggerCallback(oldWidget: oldWidget);
            })), debugLabel: "FocusableActionDetector.mayTriggerCallback");
        }
    }

    internal virtual bool _canRequestFocus => (MediaQuery.maybeNavigationModeOf(this.context) switch { NavigationMode.traditional => ((FocusableActionDetector)(object)this.widget).enabled, null => ((FocusableActionDetector)(object)this.widget).enabled, NavigationMode.directional => true, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    public override Widget build(BuildContext context)
    {
        Widget childLocal = ((Widget)(object?)new MouseRegion(key: this._mouseRegionKey, onEnter: (global::System.Action<global::Doroti.Framework.Gestures.PointerEnterEvent>)this._handleMouseEnter, onExit: (global::System.Action<global::Doroti.Framework.Gestures.PointerExitEvent>)this._handleMouseExit, cursor: ((FocusableActionDetector)(object)this.widget).mouseCursor, child: new Focus(focusNode: ((FocusableActionDetector)(object)this.widget).focusNode, autofocus: ((FocusableActionDetector)(object)this.widget).autofocus, descendantsAreFocusable: ((FocusableActionDetector)(object)this.widget).descendantsAreFocusable, descendantsAreTraversable: ((FocusableActionDetector)(object)this.widget).descendantsAreTraversable, canRequestFocus: this._canRequestFocus, onFocusChange: (global::System.Action<bool>)this._handleFocusChange, includeSemantics: ((FocusableActionDetector)(object)this.widget).includeFocusSemantics, child: ((FocusableActionDetector)(object)this.widget).child)));
        if (((((FocusableActionDetector)(object)this.widget).enabled && (((FocusableActionDetector)(object)this.widget).actions is not null)) && System.Linq.Enumerable.Any(((FocusableActionDetector)(object)this.widget).actions!)))
        {
            childLocal = DartRuntimePrimitives.ConvertValue<Widget>(new Actions(actions: ((FocusableActionDetector)(object)this.widget).actions!, child: childLocal));
        }
        if (((((FocusableActionDetector)(object)this.widget).enabled && (((FocusableActionDetector)(object)this.widget).shortcuts is not null)) && System.Linq.Enumerable.Any(((FocusableActionDetector)(object)this.widget).shortcuts!)))
        {
            childLocal = DartRuntimePrimitives.ConvertValue<Widget>(new Shortcuts(shortcuts: ((FocusableActionDetector)(object)this.widget).shortcuts!, child: childLocal));
        }
        return childLocal;
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
    internal virtual IIntentAction _selectedAction { get; set; } = default!;
    internal virtual Intent _selectedIntent { get; set; } = default!;

    public override bool isEnabled(PrioritizedIntents intent, BuildContext? context = null)
    {
        FocusNode? focus = global::Doroti.Framework.Widgets.Focus_managerLibrary.primaryFocus;
        if (((focus is null) || (((FocusNode)focus).context is null)))
        {
            return false;
        }
        foreach (Intent candidateIntent in ((PrioritizedIntents)intent).orderedIntents)
        {
            IIntentAction? candidateAction = Actions._maybeFindWithoutDependingOn(((FocusNode)focus).context!, candidateIntent, declareDependency: true);
            if (((candidateAction is not null) && candidateAction.IsEnabledForIntent(candidateIntent, context)))
            {
                _selectedAction = candidateAction;
                _selectedIntent = candidateIntent;
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override object? invoke(PrioritizedIntents intent, BuildContext? context = null)
    {
        this._selectedAction.InvokeIntent(this._selectedIntent, context);
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
    public object? _invokeDefaultAction(T intent, IIntentAction? fromAction, BuildContext? context);
    public IIntentAction? _getOverrideAction<U>(U? intent, bool declareDependency = false);
    public void _updateCallingAction(IIntentAction? value);
    public object? _invokeOverride(IIntentAction overrideAction, T intent, BuildContext? context);
    public object? invoke(T intent, BuildContext? context = null);
    public bool _isOverrideActionEnabled(IIntentAction overrideAction);
    public bool isActionEnabled { get; }
    public bool isEnabled(T intent, BuildContext? context = null);
    public bool consumesKey(T intent);
    public void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties);
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

    public virtual object? _invokeDefaultAction(T intent, IIntentAction? fromAction, BuildContext? context)
    {
        if ((fromAction is null))
        {
            return this._defaultAction.invoke(intent);
        }
        else
        {
            object? returnValue = this._defaultAction.invoke(intent);
            return returnValue;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override ContextAction<T> _makeOverridableAction(BuildContext context)
    {
        return ((ContextAction<T>)(object?)new _OverridableAction__actions<T>(defaultAction: this._defaultAction, lookupContext: context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual IIntentAction? _getOverrideAction<U>(U? intent, bool declareDependency = false) where U : Intent
    {
        IIntentAction? @override = (declareDependency ? Actions.maybeFind<U>(this._lookupContext, intent: intent) : Actions._maybeFindWithoutDependingOn(this._lookupContext, intent));
        DartRuntimePrimitives.Assert(() => !DartRuntimePrimitives.Identical(@override, this));
        return @override;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override void _updateCallingAction(IIntentAction? value)
    {
        base._updateCallingAction(value);
        this._defaultAction._updateCallingAction(value);
    }

    public virtual object? _invokeOverride(IIntentAction overrideAction, T intent, BuildContext? context)
    {
        DartRuntimePrimitives.Assert(() => !this._debugAssertMutuallyRecursive);
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugAssertMutuallyRecursive = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        overrideAction.UpdateCallingAction(this._defaultAction);
        object? returnValue = overrideAction.InvokeIntent(intent, context);
        overrideAction.UpdateCallingAction(null);
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugAssertMutuallyRecursive = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return returnValue;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override object? invoke(T intent, BuildContext? context = null)
    {
        IIntentAction? overrideAction = _getOverrideAction(intent);
        object? returnValue = ((overrideAction is null) ? _invokeDefaultAction(intent, this._currentCallingAction, context) : _invokeOverride(overrideAction, intent, context));
        return returnValue;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _isOverrideActionEnabled(IIntentAction overrideAction)
    {
        DartRuntimePrimitives.Assert(() => !this._debugAssertIsActionEnabledMutuallyRecursive);
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugAssertIsActionEnabledMutuallyRecursive = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        overrideAction.UpdateCallingAction(this._defaultAction);
        bool isOverrideEnabled = overrideAction.IsActionEnabled;
        overrideAction.UpdateCallingAction(null);
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugAssertIsActionEnabledMutuallyRecursive = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return isOverrideEnabled;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isActionEnabled
    {
        get
        {
            IIntentAction? overrideAction = _getOverrideAction<T>(default, declareDependency: true);
            bool returnValue = ((overrideAction is not null) ? _isOverrideActionEnabled(overrideAction) : ((Action<T>)this._defaultAction).isActionEnabled);
            return returnValue;
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
        IIntentAction? overrideAction = _getOverrideAction(intent);
        DartRuntimePrimitives.Assert(() => overrideAction?.DebugCanHandleIntent(intent) ?? true);
        overrideAction?.UpdateCallingAction(this._defaultAction);
        bool returnValue = (overrideAction ?? this._defaultAction).IsEnabledForIntent(intent, context);
        overrideAction?.UpdateCallingAction(null);
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugAssertIsEnabledMutuallyRecursive = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return returnValue;
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
        IIntentAction? overrideAction = _getOverrideAction(intent);
        overrideAction?.UpdateCallingAction(this._defaultAction);
        bool isEnabled = (overrideAction ?? this._defaultAction).ConsumesKeyForIntent(intent);
        overrideAction?.UpdateCallingAction(null);
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugAssertConsumeKeyMutuallyRecursive = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return isEnabled;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Action<T>>("defaultAction", this._defaultAction));
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

    public virtual object? _invokeOverride(IIntentAction overrideAction, T intent, BuildContext? context)
    {
        DartRuntimePrimitives.Assert(() => (context is not null));
        DartRuntimePrimitives.Assert(() => !this._debugAssertMutuallyRecursive);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugAssertMutuallyRecursive = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        DartRuntimePrimitives.Assert(() => overrideAction.DebugCanHandleIntent(intent));
        Action<T> wrappedDefault = ((Action<T>)(object?)new _ContextActionToActionAdapter__actions<T>(invokeContext: context!, action: this._defaultAction));
        overrideAction.UpdateCallingAction(wrappedDefault);
        object? returnValue = overrideAction.InvokeIntent(intent, context);
        overrideAction.UpdateCallingAction(null);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugAssertMutuallyRecursive = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return returnValue;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual object? _invokeDefaultAction(T intent, IIntentAction? fromAction, BuildContext? context)
    {
        if ((fromAction is null))
        {
            return this._defaultAction.invoke(intent, context);
        }
        else
        {
            object? returnValue = this._defaultAction.invoke(intent, context);
            return returnValue;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override ContextAction<T> _makeOverridableAction(BuildContext context)
    {
        return ((ContextAction<T>)(object?)new _OverridableContextAction__actions<T>(defaultAction: this._defaultAction, lookupContext: context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual IIntentAction? _getOverrideAction<U>(U? intent, bool declareDependency = false) where U : Intent
    {
        IIntentAction? @override = (declareDependency ? Actions.maybeFind<U>(this._lookupContext, intent: intent) : Actions._maybeFindWithoutDependingOn(this._lookupContext, intent));
        DartRuntimePrimitives.Assert(() => !DartRuntimePrimitives.Identical(@override, this));
        return @override;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override void _updateCallingAction(IIntentAction? value)
    {
        base._updateCallingAction(value);
        this._defaultAction._updateCallingAction(value);
    }

    public override object? invoke(T intent, BuildContext? context = null)
    {
        IIntentAction? overrideAction = _getOverrideAction(intent);
        object? returnValue = ((overrideAction is null) ? _invokeDefaultAction(intent, this._currentCallingAction, context) : _invokeOverride(overrideAction, intent, context));
        return returnValue;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _isOverrideActionEnabled(IIntentAction overrideAction)
    {
        DartRuntimePrimitives.Assert(() => !this._debugAssertIsActionEnabledMutuallyRecursive);
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugAssertIsActionEnabledMutuallyRecursive = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        overrideAction.UpdateCallingAction(this._defaultAction);
        bool isOverrideEnabled = overrideAction.IsActionEnabled;
        overrideAction.UpdateCallingAction(null);
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugAssertIsActionEnabledMutuallyRecursive = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return isOverrideEnabled;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isActionEnabled
    {
        get
        {
            IIntentAction? overrideAction = _getOverrideAction<T>(default, declareDependency: true);
            bool returnValue = ((overrideAction is not null) ? _isOverrideActionEnabled(overrideAction) : ((Action<T>)this._defaultAction).isActionEnabled);
            return returnValue;
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
        IIntentAction? overrideAction = _getOverrideAction(intent);
        DartRuntimePrimitives.Assert(() => overrideAction?.DebugCanHandleIntent(intent) ?? true);
        overrideAction?.UpdateCallingAction(this._defaultAction);
        bool returnValue = (overrideAction ?? this._defaultAction).IsEnabledForIntent(intent, context);
        overrideAction?.UpdateCallingAction(null);
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugAssertIsEnabledMutuallyRecursive = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return returnValue;
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
        IIntentAction? overrideAction = _getOverrideAction(intent);
        overrideAction?.UpdateCallingAction(this._defaultAction);
        bool isEnabled = (overrideAction ?? this._defaultAction).ConsumesKeyForIntent(intent);
        overrideAction?.UpdateCallingAction(null);
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugAssertConsumeKeyMutuallyRecursive = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return isEnabled;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Action<T>>("defaultAction", this._defaultAction));
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

    internal override void _updateCallingAction(IIntentAction? value)
    {
        this.action._updateCallingAction(value);
    }

    public override Action<T>? callingAction => this.action.callingAction;
    public override bool isEnabled(T intent, BuildContext? context = null) => this.action.isEnabled(intent, this.invokeContext);
    public override bool isActionEnabled => this.action.isActionEnabled;
    public override bool consumesKey(T intent) => this.action.consumesKey(intent);
    public override void addActionListener(global::System.Action<object> listener)
    {
        base.addActionListener(listener);
        this.action.addActionListener(listener);
    }

    public override void removeActionListener(global::System.Action<object> listener)
    {
        base.removeActionListener(listener);
        this.action.removeActionListener(listener);
    }

    public override void notifyActionListeners() => this.action.notifyActionListeners();
    public override object? invoke(T intent, BuildContext? context = null) => this.action.invoke(intent, this.invokeContext);
}
