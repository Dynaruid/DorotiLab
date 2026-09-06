using Doroti.Framework.Services;
using Doroti.Runtime;
using Doroti.Ui;

internal static class SampleHostContracts
{
    internal static async Task Verify()
    {
        using var dispatcher = new PlatformDispatcher();
        using var scope = dispatcher.EnterScope();
        var host = new Host();
        dispatcher.RegisterView(72, new DorotiViewCapabilities("sample-contract")
            .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, new ClipboardFixtureHost())
            .Register<IUrlLauncherHostCapability>(DorotiCapabilityIds.UrlLauncher, host)
            .Register<IFontHostCapability>(DorotiCapabilityIds.GraphicsFont, host));
        foreach (var url in new[] { "file:///tmp/test", "javascript:alert(1)", "/relative", "https://user:password@example.com" })
            Require((await UrlLauncher.launchUrl(url)).Status == UrlLaunchStatus.invalidUrl, "invalid URL must be rejected before reaching the host");
        Require(host.Url is null, "validation does not invoke host");
        Require((await UrlLauncher.launchUrl("https://pub.dev/packages/dynamic_color")).Status == UrlLaunchStatus.blocked, "popup failure survives Services boundary");
        Require(host.Url == "https://pub.dev/packages/dynamic_color", "host receives validated absolute URL");
        host.Result = new(UrlLaunchStatus.opened);
        Require((await UrlLauncher.launchUrl("https://example.com")).Succeeded, "accepted host request succeeds");
        var notifications = 0;
        dispatcher.channelBuffers.setListener("flutter/system", (data, reply) =>
        {
            Require(System.Text.Encoding.UTF8.GetString(data!.asMemory().Span) == "{\"type\":\"fontsChange\"}", "font notification uses system contract");
            notifications++; reply(null); return Future.value();
        });
        var buffer = new ByteBuffer([9, 1, 2, 3, 8]);
        var pending = Dart_uiLibrary.loadFontFromList(new Uint8List(buffer, 1, 3), "ExampleFamily");
        Require(host.Font!.SequenceEqual(new byte[] { 1, 2, 3 }) && host.Family == "ExampleFamily", "font loader honors byte view and family");
        Require(notifications == 0, "font notification waits for host completion");
        host.FontReady.SetResult();
        await pending;
        Require(notifications == 1, "font completion invalidates framework text layout once");
        Console.WriteLine("D05/URL-result and D11/font-byte-view: PASS");
    }
    private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
    private sealed class Host : IUrlLauncherHostCapability, IFontHostCapability
    {
        internal string? Url, Family;
        internal byte[]? Font;
        internal UrlLaunchResult Result = new(UrlLaunchStatus.blocked, "Fixture popup block");
        internal readonly TaskCompletionSource FontReady = new();
        public ValueTask<UrlLaunchResult> LaunchUrlAsync(string absoluteUrl, CancellationToken cancellationToken = default) { Url = absoluteUrl; return ValueTask.FromResult(Result); }
        public ValueTask RegisterFontAsync(ReadOnlyMemory<byte> bytes, string? family, CancellationToken cancellationToken = default)
        { Font = bytes.ToArray(); Family = family; return new(FontReady.Task); }
    }
}
