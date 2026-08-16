namespace MauiSampleApp;

public sealed class DorotiMauiPage : ContentPage
{
    public DorotiMauiPage()
    {
        Title = "Doroti MAUI GPU Feasibility";
        Content = new DorotiSkiaView();
    }
}
