using System.Text;
using Doroti.Flutter.Hosting;
using Doroti.Flutter.Ui;
using Doroti.Generated.Framework.Widgets;
using Doroti.Plugin.G55Echo.WinX64;
using MaterialAssets = Doroti.Generated.Application.G55MaterialAssets.Framework;
using MaterialPlugin = Doroti.Generated.Application.G55MaterialPlugin.Framework;
using CupertinoLocalized = Doroti.Generated.Application.G55CupertinoLocalized.Framework;
using WidgetsBase = Doroti.Generated.Application.G55WidgetsBase.Framework;

var materialAssets = new MaterialAssets.G55MaterialAssetsApp();
var materialPlugin = new MaterialPlugin.G55MaterialPluginApp();
var cupertino = new CupertinoLocalized.G55CupertinoLocalizedApp();
var widgets = new WidgetsBase.G55WidgetsBaseApp();
Execute(materialAssets);
Execute(materialPlugin);
Execute(cupertino);
Execute(widgets);
if (materialPlugin.echoChannel.name != "g55/echo")
    throw new InvalidOperationException("Generated Dart MethodChannel API/codec binding drifted.");

using var boundary = FlutterApplicationBoundary.Load(
    typeof(MaterialPlugin.G55MaterialPluginApp).Assembly,
    "win-x64",
    [new EchoPluginHandler()]);
var capabilities = new FlutterViewCapabilities("win-x64/g5-5-external-consumer");
boundary.Configure(capabilities);
var resourceCapability = capabilities.Require<IApplicationResourceHostCapability>(
    55,
    FlutterCapabilityIds.ApplicationResources,
    DartUiInvocation.Managed("g5-5-external-consumer"));
var asset = await resourceCapability.LoadAsync("assets/brand.txt");
if (Encoding.UTF8.GetString(asset.Span).Trim() != "doroti-g5-5-brand-asset")
    throw new InvalidOperationException("Generated application asset payload drifted.");
if (resourceCapability.ResolveFont("Doroti Sans").Key != "fonts/DorotiSans")
    throw new InvalidOperationException("Generated application font manifest drifted.");
var localization = resourceCapability.ResolveLocalization("en");
var localizationBytes = await resourceCapability.LoadAsync(localization.Key);
if (!Encoding.UTF8.GetString(localizationBytes.Span).Contains("Doroti application", StringComparison.Ordinal))
    throw new InvalidOperationException("Generated application localization manifest drifted.");

var messaging = capabilities.Require<IPlatformMessageHostCapability>(
    55,
    FlutterCapabilityIds.PlatformMessaging,
    DartUiInvocation.Managed("g5-5-external-consumer"));
var response = await messaging.SendAsync("g55/echo", Encoding.UTF8.GetBytes("doroti"));
if (response is null || Encoding.UTF8.GetString(response.Value.Span) != "win-x64:doroti")
    throw new InvalidOperationException("RID native plugin ABI execution drifted.");
try
{
    await messaging.SendAsync("g55/missing", null);
    throw new InvalidOperationException("Unsupported plugin silently succeeded.");
}
catch (FlutterCapabilityException exception) when (exception.CapabilityId == FlutterCapabilityIds.PlatformPlugins)
{
}

Console.WriteLine("G5-5-EXTERNAL-APPLICATION-CONSUMER-PASS");

static void Execute(StatelessWidget app)
{
    var result = app.build(new StatelessElement(app));
    if (result is null) throw new InvalidOperationException($"Generated app returned no widget: {app.GetType().FullName}");
}
