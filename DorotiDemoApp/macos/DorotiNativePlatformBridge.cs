using Doroti.Hosting;

namespace DorotiDemoApp.MacCatalyst;

internal sealed class DorotiNativePlatformBridge : DorotiNativePlatformBridgeBase
{
    public override DorotiNativePlatformInfo PlatformInfo() =>
        DorotiNativePlatformInfo.Parse(Native.DorotiNativeInterop.PlatformInfo());

    public override string Echo(string value) => Native.DorotiNativeInterop.Echo(value);

    public override async ValueTask<string> EchoOnUiThreadAsync(
        string value,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var completion = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        Native.DorotiNativeInterop.EchoOnMainThread(value, result => completion.TrySetResult(result));
        return await completion.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
    }
}
