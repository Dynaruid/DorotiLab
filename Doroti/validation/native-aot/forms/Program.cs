using Doroti.Framework.Widgets;

if (args.Contains("--native") && System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported)
    throw new InvalidOperationException("Expected NativeAOT.");
SliverContract.Run();
var form = new FormState();
((IState)form)._widget = new Form(child: new SizedBox());
var saved = new List<object?>();
var text = new ExternalField<string>();
var number = new ExternalField<long>();
Initialize(text, "initial", value => saved.Add(value));
Initialize(number, 7L, value => saved.Add(value));
form._register(text);
form._register(number);
form._register(text);
Require(form.fields.Count() == 2 && form.fields.Contains(text) && form.fields.Contains(number), "heterogeneous fields and registration identity");
text.setValue("changed");
number.setValue(42L);
form.save();
Require(saved.Count == 2 && saved.Contains("changed") && saved.Contains(42L), "typed saves retain values");
var invalid = new HashSet<object>();
number.Valid = false;
Require(!form._validate(null!, invalid) && invalid.SetEquals([number]), "granular validation retains actual invalid field");
Require(text.ValidationCount == 1 && number.ValidationCount == 1, "virtual validation invoked once per field");
number.Valid = true;
Require(form._validate(null!), "corrected fields validate");
form._unregister(text);
form.save();
Require(saved.Count == 3 && Equals(saved.Last(), 42L), "removed field is not saved");
((IFormFieldState)number).clearErrorInternal();
Require(!number.hasInteractedByUser && !number.hasError, "static clear bridge resets restoration state");
form._unregister(number);
text.dispose();
number.dispose();
Require(!form.fields.Any(), "all fields detached");
Console.WriteLine("NativeAOT form contract: external string/integer fields, registration identity, typed save, virtual validation, granular failures, clear and disposal PASS");

static void Initialize<T>(ExternalField<T> state, T value, System.Action<T?> onSaved)
{
    ((IState)state)._widget = new FormField<T>(initialValue: value, onSaved: onSaved, builder: _ => new SizedBox());
    state.initState();
    state.restoreState(null, true);
    ((IState)state)._debugLifecycleState = _StateLifecycle__framework.ready;
}
static void Require(bool value, string message) { if (!value) throw new Exception(message); }
sealed class ExternalField<T> : FormFieldState<T>
{
    public bool Valid = true;
    public int ValidationCount;
    public override bool validate() { ValidationCount++; return Valid; }
}
