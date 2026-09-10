using Doroti.Framework.Widgets;

if (args.Contains("--native") && System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported)
    throw new InvalidOperationException("Expected a published NativeAOT executable.");

var owner = new ExternalOwner();
var property = new ExternalProperty();
IRestorableProperty contract = property;
contract._register("external", owner);
Require(contract.isRegistered && ReferenceEquals(contract._owner, owner), "registration identity");
contract.initWithValueObject(contract.createDefaultValueObject());
Require(property.Value == new ExternalValue(7), "typed default survives boxing");
contract.initWithValueObject(contract.fromPrimitivesObject(42L));
Require(property.Value == new ExternalValue(42) && Equals(contract.toPrimitives(), 42L), "external serialization roundtrip");
var notifications = 0;
Action listener = () => notifications++;
contract.addListener(listener);
property.Update(new ExternalValue(43));
Require(notifications == 1, "listener invoked through static property contract");
contract.removeListener(listener);
property.Update(new ExternalValue(44));
Require(notifications == 1, "removed listener stays detached");
try { contract.initWithValueObject("wrong value"); throw new Exception("accepted incompatible value"); }
catch (InvalidCastException) { }
try { contract.fromPrimitivesObject("invalid serialized value"); throw new Exception("swallowed decode error"); }
catch (FormatException) { }
Require(property.Value == new ExternalValue(44), "failure retains last valid value");
contract._unregister();
Require(!contract.isRegistered && contract._owner is null, "unregister clears ownership");
contract._register("external-again", owner);
property.dispose();
Require(owner.Removed == 1 && contract._disposed && contract._restorationId is null, "dispose unregisters exactly once");
var optional = new NullableProperty();
IRestorableProperty optionalContract = optional;
optionalContract._register("nullable", owner);
optionalContract.initWithValueObject(null);
Require(optional.Value is null && optionalContract.toPrimitives() is null, "nullable values retained");
optionalContract.initWithValueObject("restored");
Require(optional.Value == "restored", "reference value restored");
optional.dispose();
Require(owner.Removed == 2, "independent external property lifetime");
Console.WriteLine("External restoration contract: registration, typed default/restore, value/reference/null, errors, listeners and disposal PASS");

static void Require(bool condition, string message) { if (!condition) throw new Exception(message); }
readonly record struct ExternalValue(long Number);
sealed class ExternalOwner : RestorationPropertyOwner
{
    public int Removed;
    public void _unregister(IRestorableProperty property) { Removed++; property._unregister(); }
}
sealed class ExternalProperty : RestorableProperty<ExternalValue>
{
    public ExternalValue Value;
    public override ExternalValue createDefaultValue() => new(7);
    public override ExternalValue fromPrimitives(object? data) => data is long number ? new(number) : throw new FormatException("invalid primitive");
    public override void initWithValue(ExternalValue value) => Value = value;
    public override object? toPrimitives() => Value.Number;
    public void Update(ExternalValue value) { Value = value; notifyListeners(); }
}
sealed class NullableProperty : RestorableProperty<string?>
{
    public string? Value;
    public override string? createDefaultValue() => null;
    public override string? fromPrimitives(object? data) => (string?)data;
    public override void initWithValue(string? value) => Value = value;
    public override object? toPrimitives() => Value;
}
