using Doroti.Framework.Widgets;
using Doroti.Runtime;

if (args.Contains("--native") && System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported)
    throw new Exception("Expected NativeAOT.");
var service = WidgetInspectorService.instance;
var callbacks = new Dictionary<string, Func<DartMap<string, string>, Future<DartMap<string, object>>>>();
RegisterServiceExtensionCallback register = (callback, name) => callbacks.Add(name, callback);
service._registerSignalServiceExtension("sync", () => 42, register);
service._registerSignalServiceExtension("future", () => Future<string>.value("ready"), register);
service._registerObjectGroupServiceExtension("group", group => group + "-value", register);
service._registerServiceExtensionWithArg("arg", (arg, group) => new Payload(arg, group), register);
service._registerSignalServiceExtension<string?>("nullable", () => null, register);
service._registerSignalServiceExtension<int>("error", () => throw new FormatException("callback"), register);
var parameters = new DartMap<string, string> { ["arg"] = "selected", ["objectGroup"] = "test" };
Require(Equals((await callbacks["inspector.sync"](parameters))["result"], 42), "synchronous value result");
Require(Equals((await callbacks["inspector.future"](parameters))["result"], "ready"), "asynchronous generic future result");
Require(Equals((await callbacks["inspector.group"](parameters))["result"], "test-value"), "group argument");
Require(Equals((await callbacks["inspector.arg"](parameters))["result"], new Payload("selected", "test")), "external value-type argument callback");
Require((await callbacks["inspector.nullable"](parameters))["result"] is null, "null result");
try { await callbacks["inspector.error"](parameters); throw new Exception("exception swallowed"); }
catch (FormatException exception) when (exception.Message == "callback") { }
var location = new CreationLocation("external.cs", 12, 7, "ExternalWidget");
var json = Dart_convertLibrary.jsonEncode(location.toJsonMap());
Require(json.Contains("external.cs") && json.Contains("ExternalWidget") && json.Contains("12"), "explicit creation-location JSON");
Console.WriteLine("NativeAOT inspector: sync/future/null/error results, group/argument callbacks, external value payload and creation-location JSON PASS");
static void Require(bool condition, string name) { if (!condition) throw new Exception(name); }
readonly record struct Payload(string? Argument, string Group);
