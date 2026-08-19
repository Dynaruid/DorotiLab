using System;
using Foundation;

namespace DorotiTemplateApp.MacCatalyst.Native;

[BaseType(typeof(NSObject))]
interface DorotiNativeInterop
{
    [Static]
    [Export("platformInfo")]
    string PlatformInfo();

    [Static]
    [Export("echo:")]
    string Echo(string value);

    [Static]
    [Export("echoOnMainThreadWithValue:completion:")]
    void EchoOnMainThread(string value, Action<string> completion);
}
