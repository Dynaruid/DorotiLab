// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/autofill.dart
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

public enum AutofillContextAction
{
    commit,
    cancel
}

public class AutofillGroup : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual AutofillContextAction onDisposeAction { get; private set; } = default!;

    public AutofillGroup(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!, AutofillContextAction onDisposeAction = AutofillContextAction.commit) : base(key: key)
    {
        this.child = child;
        this.onDisposeAction = onDisposeAction;
    }

    public static AutofillGroupState? maybeOf(BuildContext context)
    {
        _AutofillScope__autofill? scope__3693 = ((_AutofillScope__autofill?)(object?)context.dependOnInheritedWidgetOfExactType<_AutofillScope__autofill>());
        return scope__3693?._scope;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static AutofillGroupState of(BuildContext context)
    {
        AutofillGroupState? groupState__4552 = ((AutofillGroupState?)(object?)AutofillGroup.maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((groupState__4552 is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("AutofillGroup.of() was called with a context that does not contain an " + "AutofillGroup widget.\n" + "No AutofillGroup widget ancestor could be found starting from the " + "context that was passed to AutofillGroup.of(). This can happen " + "because you are using a widget that looks for an AutofillGroup " + "ancestor, but no such ancestor exists.\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return groupState__4552!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new AutofillGroupState());
}

public class AutofillGroupState : State<AutofillGroup>, global::Doroti.Framework.Services.AutofillScopeMixin
{
    internal virtual DartMap<string, global::Doroti.Framework.Services.AutofillClient> _clients { get; private set; } = new DartMap<string, global::Doroti.Framework.Services.AutofillClient>();
    internal virtual bool _isTopmostAutofillGroup { get; set; } = false;

    public virtual global::Doroti.Framework.Services.AutofillClient? getAutofillClient(string autofillId) => this._clients.GetValueOrDefault(autofillId);
    public virtual IEnumerable<global::Doroti.Framework.Services.AutofillClient> autofillClients
    {
        get
        {
            return this._clients.Values.where(((client) => ((global::Doroti.Framework.Services.AutofillClient)client).textInputConfiguration.autofillConfiguration.enabled));
            return default!;
        }
    }
    public virtual void register(global::Doroti.Framework.Services.AutofillClient client)
    {
        this._clients.putIfAbsent(((global::Doroti.Framework.Services.AutofillClient)client).autofillId, (() => client));
    }

    public virtual void unregister(string autofillId)
    {
        DartRuntimePrimitives.Assert(() => this._clients.ContainsKey(autofillId));
        this._clients.remove(autofillId);
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _isTopmostAutofillGroup = (AutofillGroup.maybeOf(this.context) is null);
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new _AutofillScope__autofill(autofillScopeState: this, child: ((AutofillGroup)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        base.dispose();
        if (!this._isTopmostAutofillGroup)
        {
            return;
        }
        switch (((AutofillGroup)this.widget).onDisposeAction)
        {
            case AutofillContextAction.cancel:
                {
                    TextInput.finishAutofillContext(shouldSave: false);
                    break;
                }
            case AutofillContextAction.commit:
                {
                    TextInput.finishAutofillContext();
                    break;
                }
        }
    }

    public virtual TextInputConnection attach(TextInputClient trigger, TextInputConfiguration configuration)
    {
        DartRuntimePrimitives.Assert(() => !this.autofillClients.any(((client) => !((AutofillClient)client).textInputConfiguration.autofillConfiguration.enabled)), () => (object?)"Every client in AutofillScope.autofillClients must enable autofill");
        TextInputConfiguration inputConfiguration__46968 = ((TextInputConfiguration)(object?)new _AutofillScopeTextInputConfiguration__autofill(allConfigurations: this.autofillClients.map<AutofillClient, TextInputConfiguration>(((client) => ((AutofillClient)client).textInputConfiguration)).Cast<TextInputConfiguration>(), currentClientConfiguration: configuration));
        return ((TextInputConnection)(object?)TextInput.attach(trigger, inputConfiguration__46968));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AutofillScope__autofill : InheritedWidget
{
    internal virtual AutofillGroupState? _scope { get; private set; }

    internal _AutofillScope__autofill(Widget child, AutofillGroupState? autofillScopeState = null) : base(child: child)
    {
        this._scope = autofillScopeState;
    }

    public virtual AutofillGroup client => this._scope!.widget;
    public override bool updateShouldNotify(InheritedWidget oldWidget) => (!object.Equals(this._scope, ((_AutofillScope__autofill)oldWidget)._scope));
}

