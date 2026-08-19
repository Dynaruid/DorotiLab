using Android.App;

namespace DorotiDemoApp.Android.Native;

public static class DorotiNativeBridgeBinding
{
    [System.Diagnostics.CodeAnalysis.DynamicDependency(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicMethods, typeof(global::Dev.Doroti.Bridge.DorotiNativeBridge))]
    public static string PlatformInfo() =>
        global::Dev.Doroti.Bridge.DorotiNativeBridge.PlatformInfo() ?? string.Empty;

    [System.Diagnostics.CodeAnalysis.DynamicDependency(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicMethods, typeof(global::Dev.Doroti.Bridge.DorotiNativeBridge))]
    public static string Echo(string value) =>
        global::Dev.Doroti.Bridge.DorotiNativeBridge.Echo(value) ?? string.Empty;

    [System.Diagnostics.CodeAnalysis.DynamicDependency(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicMethods, typeof(global::Dev.Doroti.Bridge.DorotiNativeBridge))]
    public static Task<string> EchoOnUiThreadAsync(Activity activity, string value)
    {
        ArgumentNullException.ThrowIfNull(activity);
        var completion = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        global::Dev.Doroti.Bridge.DorotiNativeBridge.EchoOnUiThread(
            activity,
            value,
            new Callback(result => completion.TrySetResult(result ?? string.Empty)));
        return completion.Task;
    }

    private sealed class Callback(Action<string?> callback) :
        Java.Lang.Object,
        global::Dev.Doroti.Bridge.IDorotiResultCallback
    {
        public void OnResult(string? value) => callback(value);
    }
}
