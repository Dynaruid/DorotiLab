namespace DorotiApp.Platforms.Maui;

public sealed class DorotiMauiApplication : Application
{
    protected override Window CreateWindow(IActivationState? activationState)
    {
        _ = activationState;
        return new(new DorotiMauiPage());
    }
}
