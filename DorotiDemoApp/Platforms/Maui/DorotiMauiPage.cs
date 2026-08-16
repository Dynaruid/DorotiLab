using Doroti.Host.Maui;

namespace DorotiDemoApp.Platforms.Maui;

public sealed class DorotiMauiPage : ContentPage
{
    public DorotiMauiPage()
    {
        Title = "Doroti Material Demo";
        Content = new DorotiMauiSurface(App.Definition, App.ViewConfiguration);
    }
}
