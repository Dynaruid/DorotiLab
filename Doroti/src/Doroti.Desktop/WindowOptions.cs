using Doroti.Ui;

namespace Doroti.Desktop;

public enum WindowStartupVisibility
{
    WhenReady,
    Manual,

    /// <summary>The platform owns first visibility; readiness does not promise a hidden first frame.</summary>
    PlatformDefault,
}

public enum WindowLifetimePolicy
{
    OnLastWindowClosed,
    Explicit,
}

public enum WindowPresentationState
{
    Normal,
    Minimized,
    Maximized,
    FullScreen,
}

public enum WindowTitleBarStyle
{
    Normal,
    Hidden,
}

public enum WindowTitleBarBackground
{
    System,
    Solid,
    Backdrop,
}

public enum WindowFrame
{
    Standard,
    Frameless,
}

public enum WindowCaptionButtonMode
{
    Native,
    Custom,
}

public enum WindowThemeSource
{
    System,
    App,
    Explicit,
}

public enum WindowTheme
{
    Light,
    Dark,
}

public enum WindowBackdropMode
{
    System,
    Solid,
    Transparent,
    Acrylic,
    LiquidGlass,
}

public enum WindowBackdropFallback
{
    Transparent,
    Solid,
}

public enum WindowAcrylicKind
{
    Default,
    Base,
    Thin,
}

public enum WindowSupport
{
    Supported,
    Unsupported,
    RequiresRecreation,
}

public enum WindowApplyStatus
{
    Applied,
    Superseded,
    Rejected,
    Failed,
    Canceled,
}

public enum WindowCloseDecision
{
    Allow,
    Cancel,
}

public enum WindowResizeEdge
{
    Left,
    TopLeft,
    Top,
    TopRight,
    Right,
    BottomRight,
    Bottom,
    BottomLeft,
}

/// <summary>Opaque identity. It is neither a native handle nor a view id.</summary>
public readonly record struct WindowId(Guid Value)
{
    internal static WindowId New() => new(Guid.NewGuid());
}

public sealed record WindowBackdropOptions
{
    public WindowBackdropMode Mode { get; init; }
    public WindowBackdropFallback Fallback { get; init; } = WindowBackdropFallback.Solid;
    public WindowAcrylicKind AcrylicKind { get; init; }
    public Color? TintColor { get; init; }
    public double? TintOpacity { get; init; }
    public double? LuminosityOpacity { get; init; }
}

public sealed record WindowTitleBarOptions
{
    public WindowTitleBarStyle Style { get; init; }
    public WindowTitleBarBackground Background { get; init; }
    public WindowFrame Frame { get; init; }
    public WindowCaptionButtonMode Buttons { get; init; }
    public Color? BackgroundColor { get; init; }
}

public sealed record WindowAppearanceOptions
{
    public Color BackgroundColor { get; init; } = new(0xffffffff);
    public Color? DarkBackgroundColor { get; init; }
    public WindowThemeSource ThemeSource { get; init; }
    public WindowTheme Theme { get; init; }
    public WindowBackdropOptions Backdrop { get; init; } = new();
    public WindowBackdropOptions? MacOSBackdrop { get; init; }
    public WindowTitleBarOptions TitleBar { get; init; } = new();
}

public sealed record WindowOptions
{
    public string Title { get; init; } = "Doroti";

    /// <summary>Client area in logical units, excluding native non-client chrome.</summary>
    public Size Size { get; init; } = new(800, 600);
    public Size? MinimumSize { get; init; }
    public Size? MaximumSize { get; init; }

    /// <summary>Outer-frame origin in global physical screen pixels (supports negative monitors).</summary>
    public Offset? Position { get; init; }
    public bool Centered { get; init; }
    public bool AlwaysOnTop { get; init; }
    public bool SkipTaskbar { get; init; }
    public bool Resizable { get; init; } = true;
    public WindowPresentationState PresentationState { get; init; }
    public WindowStartupVisibility StartupVisibility { get; init; }
    public WindowAppearanceOptions Appearance { get; init; } = new();

    public void Validate()
    {
        ArgumentNullException.ThrowIfNull(Title);
        ValidateSize(Size);
        if (MinimumSize is { } min)
            ValidateSize(min);
        if (MaximumSize is { } max)
            ValidateSize(max);
        if (
            MinimumSize is { } a
            && MaximumSize is { } b
            && (a.width > b.width || a.height > b.height)
        )
            throw new ArgumentException("MinimumSize exceeds MaximumSize.");
        if (
            (MinimumSize is { } lo && (Size.width < lo.width || Size.height < lo.height))
            || (MaximumSize is { } hi && (Size.width > hi.width || Size.height > hi.height))
        )
            throw new ArgumentException("Size must be within the client size limits.");
        if (Position is { IsFinite: false } || (Centered && Position is not null))
            throw new ArgumentException(
                "Position must be finite and cannot be combined with Centered."
            );
        ValidateAppearance(Appearance);
        if (!Enum.IsDefined(StartupVisibility) || !Enum.IsDefined(PresentationState))
            throw new ArgumentException("Unknown window option value.");
    }

    internal static void ValidateSize(Size size)
    {
        ArgumentNullException.ThrowIfNull(size);
        if (!size.IsFinite || size.IsEmpty)
            throw new ArgumentOutOfRangeException(nameof(size));
    }

    internal static void ValidateAppearance(WindowAppearanceOptions appearance)
    {
        ArgumentNullException.ThrowIfNull(appearance);
        ArgumentNullException.ThrowIfNull(appearance.Backdrop);
        ArgumentNullException.ThrowIfNull(appearance.TitleBar);
        ArgumentNullException.ThrowIfNull(appearance.BackgroundColor);
        var bar = appearance.TitleBar;
        if (
            !Enum.IsDefined(bar.Style)
            || !Enum.IsDefined(bar.Frame)
            || !Enum.IsDefined(bar.Buttons)
            || !Enum.IsDefined(bar.Background)
            || !Enum.IsDefined(appearance.ThemeSource)
            || !Enum.IsDefined(appearance.Theme)
        )
            throw new ArgumentException("Unknown appearance option value.");
        if (
            bar.Style == WindowTitleBarStyle.Normal
            && (bar.Frame == WindowFrame.Frameless || bar.Buttons == WindowCaptionButtonMode.Custom)
        )
            throw new ArgumentException(
                "Normal title bars require the standard frame and native buttons."
            );
        foreach (var material in new[] { appearance.Backdrop, appearance.MacOSBackdrop })
        {
            if (material is null)
                continue;
            if (
                !Enum.IsDefined(material.Mode)
                || !Enum.IsDefined(material.Fallback)
                || !Enum.IsDefined(material.AcrylicKind)
            )
                throw new ArgumentException("Unknown backdrop option value.");
            foreach (var opacity in new[] { material.TintOpacity, material.LuminosityOpacity })
                if (opacity is { } value && (!double.IsFinite(value) || value < 0 || value > 1))
                    throw new ArgumentOutOfRangeException(
                        nameof(appearance),
                        "Material opacity must be finite and in [0,1]."
                    );
        }
    }
}

public sealed record WindowEvaluation(WindowSupport Support, string? Reason = null)
{
    public static WindowEvaluation Supported { get; } = new(WindowSupport.Supported);

    public void ThrowIfUnsupported()
    {
        if (Support != WindowSupport.Supported)
            throw new NotSupportedException($"{Support}: {Reason}");
    }
}

public sealed class WindowCapabilities(
    Func<WindowOptions, WindowOptions?, WindowEvaluation> evaluate,
    bool canCancelNativeClose
)
{
    public WindowCapabilities(Func<WindowOptions, WindowOptions?, WindowEvaluation> evaluate)
        : this(evaluate, true) { }

    /// <summary>Whether native close requests participate in RegisterClosing. API close remains cancellable.</summary>
    public bool CanCancelNativeClose { get; } = canCancelNativeClose;

    public WindowEvaluation Evaluate(WindowOptions options, WindowOptions? current = null)
    {
        options.Validate();
        return evaluate(options, current);
    }
}

public sealed record WindowManagerCapabilities(
    bool CanCreateAdditionalWindows,
    int? MaximumWindows = 1
);

public sealed record WindowChromeMetrics(
    double CaptionHeight,
    ViewPadding ContentInset,
    double LeftButtonInset,
    double RightButtonInset,
    long Revision
);

public sealed record WindowEffectiveAppearance(
    WindowAppearanceOptions Appearance,
    bool SystemPolicyFallback = false,
    string? Detail = null
);

public sealed record WindowState(
    Rect? Bounds,
    Size ClientSize,
    double Scale,
    bool Visible,
    bool Focused,
    WindowPresentationState PresentationState,
    WindowAppearanceOptions RequestedAppearance,
    WindowEffectiveAppearance EffectiveAppearance,
    WindowChromeMetrics Chrome,
    long Revision = 0,
    bool Closed = false
);

public sealed record WindowApplyResult(
    WindowApplyStatus Status,
    long Revision,
    WindowState State,
    Exception? Error = null
);

public sealed record WindowClosingContext(DorotiWindowController Window);
