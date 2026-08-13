using System;
using System.Collections.Generic;
using Doroti.Generated.Framework.Foundation;
using Doroti.Flutter.Runtime;

var key = Key.Create("pilot");
var valueKey = new ValueKey<string>("pilot");
if (!Equals(key, valueKey) || key.GetHashCode() != valueKey.GetHashCode())
{
    Console.Error.WriteLine("KEY-PARITY-FAIL");
    return 2;
}

if (!RunNotifierParity())
{
    Console.Error.WriteLine("NOTIFIER-PARITY-FAIL");
    return 3;
}

var left = new List<int> { 1, 2, 3 };
var right = new List<int> { 1, 2, 3 };
if (!CollectionsLibrary.listEquals(left, right))
{
    Console.Error.WriteLine("COLLECTIONS-PARITY-FAIL");
    return 4;
}

var observers = new ObserverList<string>();
observers.add("a");
observers.add("b");
if (!observers.contains("a") || observers.isEmpty)
{
    Console.Error.WriteLine("OBSERVER-PARITY-FAIL");
    return 5;
}

Console.WriteLine("G3-B0-FOUNDATION-CONSUMER-PASS");
return 0;

static bool RunNotifierParity()
{
    var calls = 0;
    var errors = 0;
    Action first = () => calls++;
    Action second = () => calls++;
    Action failing = () => throw new InvalidOperationException("listener-fail");

    FlutterError.onError += _ => errors++;

    var notifier = new ChangeNotifier();
    notifier.addListener(first);
    notifier.addListener(first);
    notifier.notifyListeners();
    if (calls != 2)
    {
        return false;
    }

    calls = 0;
    notifier.removeListener(first);
    notifier.notifyListeners();
    if (calls != 1)
    {
        return false;
    }

    calls = 0;
    notifier = new ChangeNotifier();
    Action removing = () => { };
    removing = () =>
    {
        calls++;
        notifier.removeListener(removing);
    };
    notifier.addListener(removing);
    notifier.addListener(second);
    notifier.notifyListeners();
    if (calls != 2)
    {
        return false;
    }

    calls = 0;
    notifier.notifyListeners();
    if (calls != 1)
    {
        return false;
    }

    calls = 0;
    errors = 0;
    notifier = new ChangeNotifier();
    Action recursive = () => { };
    recursive = () =>
    {
        calls++;
        if (calls == 1)
        {
            notifier.notifyListeners();
        }
    };
    notifier.addListener(failing);
    notifier.addListener(recursive);
    notifier.addListener(second);
    notifier.notifyListeners();
    return errors >= 1 && calls >= 2;
}
