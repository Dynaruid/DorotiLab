namespace Doroti.Ui;

/// <summary>The application surface color and brightness for a unified window caption.</summary>
public sealed record WindowTitlebarTheme(Color BackgroundColor, Brightness Brightness);

/// <summary>Optional host support for matching a client caption to the application theme.</summary>
public interface IWindowTitlebarHostCapability
{
    void SetTheme(WindowTitlebarTheme theme);
}
