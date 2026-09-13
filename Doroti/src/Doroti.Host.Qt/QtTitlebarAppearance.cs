using Doroti.Ui;

namespace Doroti.Host.Qt;

internal sealed class QtTitlebarAppearance(Action invalidate) : IWindowTitlebarHostCapability
{
    private WindowTitlebarTheme? _theme;

    internal WindowTitlebarTheme? Theme => Volatile.Read(ref _theme);

    public void SetTheme(WindowTitlebarTheme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);
        if (Interlocked.Exchange(ref _theme, theme) != theme) invalidate();
    }
}
