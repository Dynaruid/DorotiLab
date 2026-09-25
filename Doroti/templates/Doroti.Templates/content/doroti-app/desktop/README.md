# Optional desktop companion

This assembly is intentionally separate from the common app. It is currently
available to a **Windows MAUI** runner. The template's default WindowsAppSDK
runner remains unchanged; its desktop controller adapter is not implemented yet.

In a Windows MAUI runner, set:

```xml
<DorotiDesktopProject>../desktop/DorotiTemplateApp.Desktop.csproj</DorotiDesktopProject>
<DorotiDesktopStartupType>DorotiTemplateApp.Desktop.DesktopStartup</DorotiDesktopStartupType>
```

The SDK registers the companion startup and references its assembly. Never add
this project to the common app, Web, Android, iOS or MacCatalyst runner. These
references fail with a `DOROTIDESKTOP` diagnostic. Custom/hidden title bars and
other native desktop adapters remain pending.
