using Doroti.Host.Maui;
using Doroti.Hosting;

namespace InvalidRequiredRegistration;

internal sealed class InvalidApplication : DorotiMauiWinUIApplication
{
    protected override DorotiApplicationDescriptor CreateApplicationDescriptor() =>
        throw new NotSupportedException();

    // This is intentionally invalid: the host owns and seals mandatory Doroti registration.
    protected override MauiApp CreateMauiApp() => MauiApp.CreateBuilder().Build();
}
