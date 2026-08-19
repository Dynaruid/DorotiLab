using Doroti.Hosting;
using Microsoft.Maui.ApplicationModel;

namespace DorotiTemplateApp.Android;

internal sealed class DorotiNativePlatformBridge : DorotiNativePlatformBridgeBase
{
    public override DorotiNativePlatformInfo PlatformInfo() =>
        DorotiNativePlatformInfo.Parse(Native.DorotiNativeBridgeBinding.PlatformInfo());

    public override string Echo(string value) => Native.DorotiNativeBridgeBinding.Echo(value);

    public override async ValueTask<string> EchoOnUiThreadAsync(
        string value,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var activity = Platform.CurrentActivity
            ?? throw new InvalidOperationException("The Android native bridge requires a current Activity.");
        return await Native.DorotiNativeBridgeBinding.EchoOnUiThreadAsync(activity, value)
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
