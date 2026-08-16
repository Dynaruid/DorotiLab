using Doroti.Host.Maui;

namespace DorotiApp.Platforms.Maui;

public sealed class DorotiMauiPage : ContentPage
{
    public DorotiMauiPage() => Content = new DorotiMauiSurface(App.Definition, App.ViewConfiguration);
}
