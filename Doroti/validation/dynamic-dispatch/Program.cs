using System.Reflection;
using System.Runtime.CompilerServices;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Runtime;

VerifyRenderChildContracts();
await VerifyTypedRouteResults();
VerifyActionsContracts();
VerifySelectedOwnersHaveNoCallSites();

int widgetsCallSites = CountCallSites(typeof(Actions).Assembly);
Console.WriteLine($"Dynamic dispatch focused contract: PASS (configuration={ConfigurationName()}, widgets-call-sites={widgetsCallSites})");

static void VerifyRenderChildContracts()
{
    var constraints = new BoxConstraints();
    var parent = new RenderConstrainedBox(additionalConstraints: constraints);
    var child = new RenderConstrainedBox(additionalConstraints: constraints);
    var single = (IRenderObjectWithChild)parent;

    Require(single.debugValidateChild(child), "single-child contract validates the concrete render child");
    single.child = child;
    Require(ReferenceEquals(parent.child, child) && ReferenceEquals(child.parent, parent),
        "single-child contract adopts and orders its child");
    single.child = null;
    Require(parent.child is null && child.parent is null,
        "single-child contract removes its child");

    var flex = new RenderFlex();
    var first = new RenderConstrainedBox(additionalConstraints: constraints);
    var second = new RenderConstrainedBox(additionalConstraints: constraints);
    var container = (IContainerRenderObject)flex;

    Require(container.debugValidateChild(first) && container.debugValidateChild(second),
        "multi-child contract validates concrete render children");
    container.insert(first);
    container.insert(second, after: first);
    Require(ReferenceEquals(flex.firstChild, first) && ReferenceEquals(flex.lastChild, second),
        "multi-child insert preserves sibling order");
    container.move(second, after: null);
    Require(ReferenceEquals(flex.firstChild, second) && ReferenceEquals(flex.lastChild, first),
        "multi-child move updates sibling order");
    container.remove(second);
    container.remove(first);
    Require(flex.childCount == 0 && first.parent is null && second.parent is null,
        "multi-child remove releases every child");
}

static async Task VerifyTypedRouteResults()
{
    var intRoute = new ContractRoute<int>();
    var stringRoute = new ContractRoute<string>();
    var objectRoute = new ContractRoute<object?>();
    var mixedHistory = new List<RouteBase> { intRoute, stringRoute, objectRoute };

    Require(mixedHistory.Select(route => route.settings.name).SequenceEqual(["Int32", "String", "Object"]),
        "different Route<T> result types share one non-generic history contract");

    Require(intRoute.didPop(42), "int route accepts its typed result");
    Require(stringRoute.didPop("done"), "string route accepts its typed result");
    object marker = new();
    Require(objectRoute.didPop(marker), "object route accepts its typed result");

    Require(await intRoute.popped == 42, "int route preserves its pop result");
    Require(await stringRoute.popped == "done", "string route preserves its pop result");
    Require(ReferenceEquals(await objectRoute.popped, marker), "object route preserves its pop result");
}

static void VerifyActionsContracts()
{
    var first = new CountingAction<FirstIntent>();
    var second = new CountingAction<SecondIntent>();
    var map = new DartMap<Type, dynamic>
    {
        [typeof(FirstIntent)] = first,
        [typeof(SecondIntent)] = second,
    };
    _ = new Actions(actions: map, child: SizedBox.CreateShrink());

    ((IIntentAction)first).InvokeIntent(new FirstIntent(), context: null);
    ((IIntentAction)second).InvokeIntent(new SecondIntent(), context: null);
    Require(first.Invocations == 1 && second.Invocations == 1,
        "dispatcher invokes different intent subtypes through IIntentAction");

    IIntentAction firstContract = first;
    IIntentAction callingContract = new CountingAction<FirstIntent>();
    firstContract.UpdateCallingAction(callingContract);
    Require(ReferenceEquals(firstContract.CallingAction, callingContract),
        "calling-action override state uses the non-generic action contract");
    firstContract.UpdateCallingAction(null);

    var notifications = 0;
    void Listener(object action)
    {
        Require(ReferenceEquals(action, first), "listener receives the notifying action instance");
        notifications++;
    }
    first.addActionListener(Listener);
    first.notifyActionListeners();
    first.removeActionListener(Listener);
    first.notifyActionListeners();
    Require(notifications == 1, "action listener add/remove/notify is exactly once");

    var compatibleBaseMap = new DartMap<Type, dynamic>
    {
        [typeof(FirstIntent)] = new CountingAction<Intent>(),
    };
    _ = new Actions(actions: compatibleBaseMap, child: SizedBox.CreateShrink());

    var invalidMap = new DartMap<Type, dynamic>
    {
        [typeof(FirstIntent)] = new CountingAction<SecondIntent>(),
    };
    RequireThrows<ArgumentException>(() => _ = new Actions(actions: invalidMap, child: SizedBox.CreateShrink()),
        "Actions rejects a map whose value cannot handle the keyed intent type");
}

static void VerifySelectedOwnersHaveNoCallSites()
{
    Assembly widgetsAssembly = typeof(Actions).Assembly;
    Type[] owners =
    [
        typeof(ProxyElement),
        typeof(RenderObjectElement),
        typeof(SingleChildRenderObjectElement),
        typeof(MultiChildRenderObjectElement),
        typeof(WidgetsFlutterBinding),
        typeof(Route<>),
        typeof(TransitionRoute<>),
        typeof(Doroti.Framework.Widgets.Action<>),
        typeof(ActionListener),
        typeof(ActionDispatcher),
        typeof(Actions),
    ];

    foreach (Type owner in owners)
    {
        int count = widgetsAssembly.GetTypes()
            .Where(type => IsNestedUnder(type, owner))
            .SelectMany(type => type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            .Count(field => field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(CallSite<>));
        Require(count == 0, $"{owner.Name} has no compiler-generated DLR CallSite fields");
    }

    Type routeEntry = widgetsAssembly.GetType("Doroti.Framework.Widgets._RouteEntry__navigator", throwOnError: true)!;
    Type defaultTransitionDelegate = typeof(DefaultTransitionDelegate<>);
    foreach (Type owner in new[] { routeEntry, defaultTransitionDelegate })
    {
        int count = widgetsAssembly.GetTypes()
            .Where(type => IsNestedUnder(type, owner))
            .SelectMany(type => type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            .Count(field => field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(CallSite<>));
        Require(count == 0, $"{owner.Name} has no compiler-generated DLR CallSite fields");
    }
}

static bool IsNestedUnder(Type type, Type owner)
{
    for (Type? current = type.DeclaringType; current is not null; current = current.DeclaringType)
    {
        if (current == owner)
        {
            return true;
        }
    }
    return false;
}

static int CountCallSites(Assembly assembly) => assembly.GetTypes()
    .SelectMany(type => type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
    .Count(field => field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(CallSite<>));

static void Require(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}

static void RequireThrows<TException>(Action action, string message) where TException : Exception
{
    try
    {
        action();
    }
    catch (TException)
    {
        return;
    }
    throw new InvalidOperationException(message);
}

static string ConfigurationName() =>
#if DEBUG
    "Debug";
#else
    "Release";
#endif

sealed class ContractRoute<T> : Route<T>
{
    public ContractRoute() : base(new RouteSettings(name: typeof(T).Name)) { }
}

sealed class FirstIntent : Intent;
sealed class SecondIntent : Intent;

sealed class CountingAction<T> : Doroti.Framework.Widgets.Action<T> where T : Intent
{
    public int Invocations { get; private set; }

    public override object? invoke(T intent, BuildContext? context = null)
    {
        Invocations++;
        return Invocations;
    }
}
