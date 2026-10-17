using System.Reflection;
using System.Runtime.CompilerServices;
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

/// <summary>A bundled icon face, registered lazily once for each View that uses it.</summary>
public sealed class IconFont(Assembly assembly, string resourceName, string family)
{
    private readonly ConditionalWeakTable<DorotiView, Future<bool>> _loads = new();

    internal Future<bool> EnsureLoaded(DorotiView view) => _loads.GetValue(view,
        _ => Future<bool>.fromTask(LoadAsync(view)));

    private async Task<bool> LoadAsync(DorotiView view)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Missing icon font '{resourceName}'.");
        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);
        await view.RequireCapability<IFontHostCapability>(DorotiCapabilityIds.GraphicsFont,
            DorotiUiInvocation.Managed("IconFont.Load"))
            .RegisterFontAsync(buffer.ToArray(), family);
        return true;
    }
}
