using Microsoft.Maui.Platforms.MacOS.Handlers;

namespace Doroti.Validation.AppKitMetalSpike;

internal sealed class DorotiMetalSurfaceHandler
    : MacOSViewHandler<DorotiMetalSurface, DorotiMetalView>
{
    public static readonly IPropertyMapper<DorotiMetalSurface, DorotiMetalSurfaceHandler> Mapper =
        new PropertyMapper<DorotiMetalSurface, DorotiMetalSurfaceHandler>(ViewMapper);

    public DorotiMetalSurfaceHandler() : base(Mapper) { }

    protected override DorotiMetalView CreatePlatformView() => new();

    protected override void ConnectHandler(DorotiMetalView platformView)
    {
        base.ConnectHandler(platformView);
        platformView.Connect(VirtualView);
    }

    protected override void DisconnectHandler(DorotiMetalView platformView)
    {
        platformView.Disconnect();
        base.DisconnectHandler(platformView);
    }
}
