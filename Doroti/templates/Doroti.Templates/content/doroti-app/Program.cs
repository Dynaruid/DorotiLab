# if DOROTI_BROWSER
[assembly: System.Runtime.Versioning.SupportedOSPlatform("browser")]

await DorotiApp.Platforms.Web.PlatformBootstrap.RunAsync(args);
# elif MACCATALYST
DorotiApp.Platforms.MacCatalyst.PlatformBootstrap.Run(args);
# endif
