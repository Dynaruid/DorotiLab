using Doroti.Framework.Foundation;
using Doroti.Framework.Widgets;
using Doroti.Runtime;
using Doroti.Ui;

var locale = new Locale("ko");
var immediate = new ExternalDelegate<string>(() => new SynchronousFuture<string>("한국어"));
var duplicate = new ExternalDelegate<string>(() => throw new Exception("Duplicate delegate was loaded."));
var synchronous = false;
_ = LocalizationsLibrary._loadAll(locale, [immediate, duplicate]).then(result =>
{
    Require((string)result[typeof(string)] == "한국어", "First supported delegate wins.");
    synchronous = true;
    return result;
});
Require(synchronous, "SynchronousFuture must retain synchronous localization delivery across the untyped bridge.");

var completion = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
var delayed = new ExternalDelegate<int>(() => Future<int>.fromTask(completion.Task));
var pending = LocalizationsLibrary._loadAll(locale, [immediate, delayed]);
Require(!pending.asTask().IsCompleted, "Mixed results must wait for the delayed delegate.");
completion.SetResult(42);
var values = await pending;
Require((string)values[typeof(string)] == "한국어" && (int)values[typeof(int)] == 42, "Heterogeneous result types are preserved.");
ILocalizationsDelegate bridge = immediate;
Require(!bridge.shouldReload(duplicate), "Same concrete delegate can keep its resources.");
Require(bridge.shouldReload(delayed), "Different generic delegate reloads without an invalid generic cast.");
Require(bridge.shouldReload(new AlternateStringDelegate()), "Different concrete delegate reloads.");
var failed = new ExternalDelegate<int>(() => Future<int>.error(new ApplicationException("locale")));
try
{
    await LocalizationsLibrary._loadAll(locale, [failed]);
    throw new Exception("Failed locale silently recovered.");
}
catch (ApplicationException error) { Require(error.Message == "locale", "Original load failure is propagated."); }
try
{
    _ = LocalizationsLibrary._loadAll(locale, [new object()]);
    throw new Exception("Unsupported delegate silently accepted.");
}
catch (ArgumentException) { }
Console.WriteLine("NativeAOT external localization contracts: PASS");

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

sealed class ExternalDelegate<T>(Func<Future<T>> loader) : LocalizationsDelegate<T>
{
    public override bool isSupported(Locale locale) => locale.languageCode == "ko";
    public override Future<T> load(Locale locale) => loader();
    public override bool shouldReload(LocalizationsDelegate<T> old) => false;
}

sealed class AlternateStringDelegate : LocalizationsDelegate<string>
{
    public override bool isSupported(Locale locale) => true;
    public override Future<string> load(Locale locale) => new SynchronousFuture<string>("alternate");
    public override bool shouldReload(LocalizationsDelegate<string> old) => false;
}
