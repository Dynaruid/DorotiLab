// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/clipboard.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Services;

public class ClipboardData
{
    public virtual string? text { get; private set; }

    public ClipboardData(string text)
    {
        this.text = text;
    }
}

public abstract class Clipboard
{
    public const string kTextPlain = "text/plain";

    public static async Future setData(ClipboardData data)
    {
        await Invoke("setData", async (host, token) => { await host.SetClipboardTextAsync(data.text ?? string.Empty, token).ConfigureAwait(false); return true; });
    }

    public static async Future<ClipboardData?> getData(string format)
    {
        if (format != kTextPlain)
        {
            return null;
        }

        var text = await Invoke("getData", (host, token) => host.GetClipboardTextAsync(token));
        return text is null ? null : new ClipboardData(text);
    }

    public static async Future<bool> hasStrings()
    {
        return await Invoke("hasStrings", (host, token) => host.HasClipboardTextAsync(token));
    }

    public static async Future<ClipboardTextAvailability> queryTextAvailability() =>
        await Invoke("queryTextAvailability", (host, token) => host.QueryClipboardTextAsync(token));

    private static ValueTask<T> Invoke<T>(string operation, Func<IPlatformServicesHostCapability, CancellationToken, ValueTask<T>> call)
    {
        var invocation = DorotiUiInvocation.Managed($"package:flutter/services.dart#Clipboard.{operation}");
        var view = PlatformDispatcher.instance.RequireInvocationView(invocation);
        return view.InvokeCapabilityAsync(DorotiCapabilityIds.PlatformServices, invocation, call);
    }
}
