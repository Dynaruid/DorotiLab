// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/autofill.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public enum AutofillContextAction
{
    commit,
    cancel,
}

public class AutofillGroup : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual AutofillContextAction onDisposeAction { get; private set; } = default!;

    public AutofillGroup(
        Key? key = null,
        Widget child = default!,
        AutofillContextAction onDisposeAction = AutofillContextAction.commit
    )
        : base(key: key)
    {
        this.child = child;
        this.onDisposeAction = onDisposeAction;
    }

    public static AutofillGroupState? maybeOf(BuildContext context)
    {
        _AutofillScope__autofill? scope =
            context.dependOnInheritedWidgetOfExactType<_AutofillScope__autofill>();
        return scope?._scope;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static AutofillGroupState of(BuildContext context)
    {
        AutofillGroupState? groupState = maybeOf(context);
        DartRuntimePrimitives.Assert(() =>
        {
            if (groupState is null)
            {
                throw DartRuntimePrimitives.AsException(
                    FlutterError.Create(
                        "AutofillGroup.of() was called with a context that does not contain an "
                            + "AutofillGroup widget.\n"
                            + "No AutofillGroup widget ancestor could be found starting from the "
                            + "context that was passed to AutofillGroup.of(). This can happen "
                            + "because you are using a widget that looks for an AutofillGroup "
                            + "ancestor, but no such ancestor exists.\n"
                            + "The context used was:\n"
                            + $"  {context}"
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        return groupState!;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new AutofillGroupState());
}

public class AutofillGroupState : State<AutofillGroup>, AutofillScopeMixin
{
    internal virtual DartMap<string, AutofillClient> _clients { get; private set; } =
        new DartMap<string, AutofillClient>();
    internal virtual bool _isTopmostAutofillGroup { get; set; } = false;

    public virtual AutofillClient? getAutofillClient(string autofillId) =>
        _clients.GetValueOrDefault(autofillId);

    public virtual IEnumerable<AutofillClient> autofillClients
    {
        get
        {
            return _clients.Values.where(
                (client) => client.textInputConfiguration.autofillConfiguration.enabled
            );
        }
    }

    public virtual void register(AutofillClient client)
    {
        _clients.putIfAbsent(client.autofillId, () => client);
    }

    public virtual void unregister(string autofillId)
    {
        DartRuntimePrimitives.Assert(() => _clients.ContainsKey(autofillId));
        _clients.remove(autofillId);
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _isTopmostAutofillGroup = AutofillGroup.maybeOf(context) is null;
    }

    public override Widget build(BuildContext context)
    {
        return new _AutofillScope__autofill(autofillScopeState: this, child: widget.child);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void dispose()
    {
        base.dispose();
        if (!_isTopmostAutofillGroup)
        {
            return;
        }
        switch (widget.onDisposeAction)
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

    public virtual TextInputConnection attach(
        TextInputClient trigger,
        TextInputConfiguration configuration
    )
    {
        DartRuntimePrimitives.Assert(
            () =>
                !autofillClients.any(
                    (client) => !client.textInputConfiguration.autofillConfiguration.enabled
                ),
            () => (object?)"Every client in AutofillScope.autofillClients must enable autofill"
        );
        TextInputConfiguration inputConfiguration =
            new _AutofillScopeTextInputConfiguration__autofill(
                allConfigurations: autofillClients
                    .map((client) => client.textInputConfiguration)
                    .Cast<TextInputConfiguration>(),
                currentClientConfiguration: configuration
            );
        return TextInput.attach(trigger, inputConfiguration);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _AutofillScope__autofill : InheritedWidget
{
    internal virtual AutofillGroupState? _scope { get; private set; }

    internal _AutofillScope__autofill(Widget child, AutofillGroupState? autofillScopeState = null)
        : base(child: child)
    {
        _scope = autofillScopeState;
    }

    public virtual AutofillGroup client => _scope!.widget;

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        !Equals(_scope, ((_AutofillScope__autofill)oldWidget)._scope);
}
