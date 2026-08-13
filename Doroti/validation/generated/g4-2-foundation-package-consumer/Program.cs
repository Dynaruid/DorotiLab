using Doroti.Generated.Framework.Foundation;
using Doroti.Flutter.Ui;

var notifier = new ChangeNotifier();
var notifications = 0;
notifier.addListener(() => notifications++);
notifier.notifyListeners();

var buffer = new WriteBuffer();
buffer.putInt32(42);
var read = new ReadBuffer(buffer.done());

using var environment = PlatformEnvironmentContext.Enter(new([], Brightness.light, false, false, HostOperatingSystem.windows));
if (notifications != 1 || read.getInt32() != 42 || PlatformLibrary.defaultTargetPlatform != TargetPlatform.windows)
{
    return 2;
}

Console.WriteLine("G4-2-FOUNDATION-PACKAGE-CONSUMER-PASS");
return 0;
