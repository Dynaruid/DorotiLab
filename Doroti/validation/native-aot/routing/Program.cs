using Doroti.Framework.Foundation;
using Doroti.Framework.Widgets;
using Doroti.Runtime;
using C = Doroti.Framework.Cupertino;

if (args.Contains("--native") && System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported)
    throw new InvalidOperationException("Expected NativeAOT.");
var scheduler = new TestScheduler();
var integer = new ExternalRoute<int>("Integer");
var text = new ExternalRoute<string>("String");
IModalRoute modal = integer;
Require(ReferenceEquals(modal.routeBase, integer), "modal bridge retains route identity");
var numberEntry = new ExternalEntry<int>();
var objectEntry = new ExternalEntry<object?>();
modal.registerPopEntry(numberEntry);
modal.registerPopEntry(objectEntry);
numberEntry.Notifier.value = false;
Require(integer.popDisposition == RoutePopDisposition.doNotPop, "pop entry veto");
integer.onPopInvokedWithResult(false, 42);
Require(numberEntry.Result == 42 && Equals(objectEntry.Result, 42) && numberEntry.Calls == 1, "heterogeneous pop callback types and result values");
modal.unregisterPopEntry(numberEntry);
integer.onPopInvokedWithResult(true, 7);
Require(numberEntry.Calls == 1 && Equals(objectEntry.Result, 7), "removed entry is not invoked");
modal.unregisterPopEntry(objectEntry);
var incompatible = (IPopEntry)new ExternalEntry<string>();
try { incompatible.onPopInvokedWithResultObject(true, 42); throw new Exception("incompatible result was coerced"); }
catch (InvalidCastException) { }
text.didChangePrevious(integer);
Require(((C.ICupertinoRouteTitle)text).previousTitle.value == "Integer", "previous title crosses generic route types");
Require(integer.canTransitionTo(text) && text.canTransitionFrom(integer), "page transition metadata ignores result type");
var removed = 0;
var history = new LocalHistoryEntry(() => removed++);
integer.addLocalHistoryEntry(history);
Require(integer.willHandlePopInternally, "local history attaches to integer route");
history.remove();
Require(removed == 1 && !integer.willHandlePopInternally, "local history detaches without object-route cast");
integer.dispose(); text.dispose();
numberEntry.Notifier.dispose(); objectEntry.Notifier.dispose();

var parser = new ExternalParser();
var routerDelegate = new ExternalRouterDelegate();
var widget = ((IRouterDelegate)routerDelegate).createRouterWidget(null, parser, null, "custom");
Require(widget is Router<ExternalConfiguration> router && ReferenceEquals(router.routerDelegate, routerDelegate) && ReferenceEquals(router.routeInformationParser, parser), "composition retains external configuration type");
IRouterConfig config = new RouterConfig<ExternalConfiguration>(routerDelegate: routerDelegate);
Require(config.createRouterWidget() is Router<ExternalConfiguration>, "configuration factory retains type");
try { ((IRouterDelegate)routerDelegate).createRouterWidget(null, new WrongParser(), null, null); throw new Exception("incompatible parser was accepted"); }
catch (ArgumentException) { }
var value = await parser.parseRouteInformation(new RouteInformation(location: "/external"));
await routerDelegate.setNewRoutePath(value);
Require(routerDelegate.currentConfiguration == value, "external parser and route configuration value");
Console.WriteLine("NativeAOT routing: pop veto/results/unregistration/type rejection, cross-generic title/transitions/local history, external Router configuration and parser PASS");

static void Require(bool value, string message) { if (!value) throw new Exception(message); }
sealed class ExternalRoute<T>(string title) : C.CupertinoPageRoute<T>(_ => new SizedBox(), title: title)
{
    // Lifecycle scheduling belongs to the separately mounted route fixture.
    public override void changedInternalState() { }
}
sealed class ExternalEntry<T> : PopEntry<T>
{
    public readonly ValueNotifier<bool> Notifier = new(true);
    public int Calls;
    public T? Result;
    public override ValueListenable<bool> canPopNotifier => Notifier;
    public override void onPopInvokedWithResult(bool didPop, T? result) { Calls++; Result = result; }
}
readonly record struct ExternalConfiguration(string Path);
sealed class ExternalParser : RouteInformationParser<ExternalConfiguration>
{
    public override Future<ExternalConfiguration> parseRouteInformation(RouteInformation information) => new SynchronousFuture<ExternalConfiguration>(new(information.location));
}
sealed class WrongParser : RouteInformationParser<string> { }
sealed class ExternalRouterDelegate : RouterDelegate<ExternalConfiguration>
{
    private ExternalConfiguration _configuration;
    public override ExternalConfiguration currentConfiguration => _configuration;
    public override Future setNewRoutePath(ExternalConfiguration configuration) { _configuration = configuration; return Future.value(); }
    public override Future<bool> popRoute() => new SynchronousFuture<bool>(false);
    public override Widget build(BuildContext context) => new SizedBox();
    public override void addListener(System.Action listener) { }
    public override void removeListener(System.Action listener) { }
}

sealed class TestScheduler : Doroti.Framework.Scheduler.SchedulerBinding { }
