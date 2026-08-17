namespace MauiSampleApp;

public sealed class DorotiMauiApplication : Application
{
    protected override Window CreateWindow(IActivationState? activationState) => new(new DorotiOcrPage());
}
