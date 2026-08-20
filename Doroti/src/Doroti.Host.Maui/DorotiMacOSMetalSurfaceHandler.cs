#if MACOS
using Microsoft.Maui.Platforms.MacOS.Handlers;

namespace Doroti.Host.Maui;

public sealed class DorotiMacOSMetalSurfaceHandler
    : MacOSViewHandler<DorotiMacOSMetalSurface, DorotiMacOSMetalView>
{
    public static readonly IPropertyMapper<DorotiMacOSMetalSurface, DorotiMacOSMetalSurfaceHandler> Mapper =
        new PropertyMapper<DorotiMacOSMetalSurface, DorotiMacOSMetalSurfaceHandler>(ViewMapper);

    public DorotiMacOSMetalSurfaceHandler() : base(Mapper) { }

    protected override DorotiMacOSMetalView CreatePlatformView() => new();

    protected override void ConnectHandler(DorotiMacOSMetalView platformView)
    {
        base.ConnectHandler(platformView);
        VirtualView.Connect(platformView);
    }

    protected override void DisconnectHandler(DorotiMacOSMetalView platformView)
    {
        VirtualView.Disconnect(platformView);
        base.DisconnectHandler(platformView);
    }
}
#endif
