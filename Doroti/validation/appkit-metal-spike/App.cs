namespace Doroti.Validation.AppKitMetalSpike;

public sealed class App : Application
{
    protected override Window CreateWindow(IActivationState? activationState)
    {
        _ = activationState;
        return new Window(new ContentPage
        {
            Title = "Doroti AppKit Metal Spike",
            Content = new DorotiMetalSurface(),
        });
    }
}
