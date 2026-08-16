#if DOROTI_BROWSER
[assembly: System.Runtime.Versioning.SupportedOSPlatform("browser")]

await DorotiDemoApp.Platforms.Web.PlatformBootstrap.RunAsync(args);
#elif MACCATALYST
DorotiDemoApp.Platforms.MacCatalyst.PlatformBootstrap.Run(args);
#endif
