#if WINDOWS
using Doroti.Ui;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace Doroti.Host.Maui;

/// <summary>One native material owner per MAUI window.</summary>
internal sealed class WindowsWindowBackdrop : IDisposable
{
    private readonly Microsoft.UI.Xaml.Window _window;
    private readonly SystemBackdrop? _previous;
    private readonly AcrylicMaterial? _material;
    private readonly WindowBackdropOptions _options;
    private readonly bool _nativeCaption;
    private readonly FrameworkElement? _themeOwner;
    private bool _disposed;

    internal WindowsWindowBackdrop(
        Microsoft.UI.Xaml.Window window,
        WindowBackdropOptions options,
        bool nativeCaption = false
    )
    {
        _window = window;
        _options = options;
        _nativeCaption = nativeCaption;
        _previous = window.SystemBackdrop;
        if (options.mode is WindowBackdropMode.acrylic or WindowBackdropMode.experimentalAcrylic)
        {
            _material = new(options);
            window.SystemBackdrop = _material;
        }
        if (_nativeCaption)
        {
            _themeOwner = window.Content as FrameworkElement;
            if (_themeOwner is not null)
                _themeOwner.ActualThemeChanged += HandleThemeChanged;
            ApplyCaption();
        }
    }

    private void HandleThemeChanged(FrameworkElement sender, object args) => ApplyCaption();

    private void ApplyCaption()
    {
        if (!_disposed && _nativeCaption)
            WindowsNativeCaption.ApplyTheme(_window, _options);
    }

    public void Dispose()
    {
        _disposed = true;
        if (_themeOwner is not null)
            _themeOwner.ActualThemeChanged -= HandleThemeChanged;
        if (_material is not null && ReferenceEquals(_window.SystemBackdrop, _material))
        {
            _window.SystemBackdrop = _previous;
        }
    }

    private sealed class AcrylicMaterial(WindowBackdropOptions options) : SystemBackdrop
    {
        private DesktopAcrylicController? _controller;
        private readonly SystemBackdropConfiguration _configuration = new();
        private Windows.UI.Composition.Compositor? _fallbackCompositor;
        private Windows.UI.Composition.CompositionColorBrush? _fallback;

        protected override void OnTargetConnected(
            ICompositionSupportsSystemBackdrop connectedTarget,
            XamlRoot xamlRoot
        )
        {
            base.OnTargetConnected(connectedTarget, xamlRoot);
            UpdateConfiguration(connectedTarget, xamlRoot);
            if (!DesktopAcrylicController.IsSupported())
            {
                _fallbackCompositor = new();
                _fallback = _fallbackCompositor.CreateColorBrush(
                    options.fallback == WindowBackdropFallback.transparent
                        ? Microsoft.UI.Colors.Transparent
                    : _configuration.Theme == SystemBackdropTheme.Dark ? Microsoft.UI.Colors.Black
                    : Microsoft.UI.Colors.White
                );
                connectedTarget.SystemBackdrop = _fallback;
                return;
            }

            _controller = new DesktopAcrylicController
            {
                Kind = options.acrylicKind switch
                {
                    WindowAcrylicKind.@base => DesktopAcrylicKind.Base,
                    WindowAcrylicKind.thin => DesktopAcrylicKind.Thin,
                    _ => DesktopAcrylicKind.Default,
                },
            };
            _controller.SetSystemBackdropConfiguration(_configuration);
            if (options.tintColor is { } tint)
            {
                var value = tint.value;
                _controller.TintColor = Windows.UI.Color.FromArgb(
                    (byte)(value >> 24),
                    (byte)(value >> 16),
                    (byte)(value >> 8),
                    (byte)value
                );
            }
            if (options.tintOpacity is { } tintOpacity)
            {
                _controller.TintOpacity = (float)tintOpacity;
            }
            if (options.luminosityOpacity is { } luminosityOpacity)
            {
                _controller.LuminosityOpacity = (float)luminosityOpacity;
            }
            _controller.AddSystemBackdropTarget(connectedTarget);
        }

        protected override void OnDefaultSystemBackdropConfigurationChanged(
            ICompositionSupportsSystemBackdrop target,
            XamlRoot xamlRoot
        )
        {
            base.OnDefaultSystemBackdropConfigurationChanged(target, xamlRoot);
            UpdateConfiguration(target, xamlRoot);
        }

        private void UpdateConfiguration(ICompositionSupportsSystemBackdrop target, XamlRoot root)
        {
            var defaults = GetDefaultSystemBackdropConfiguration(target, root);
            _configuration.IsInputActive = defaults.IsInputActive;
            _configuration.IsHighContrast = defaults.IsHighContrast;
            _configuration.Theme = options.theme switch
            {
                WindowBackdropTheme.light => SystemBackdropTheme.Light,
                WindowBackdropTheme.dark => SystemBackdropTheme.Dark,
                _ => defaults.Theme,
            };
        }

        protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop target)
        {
            _controller?.RemoveSystemBackdropTarget(target);
            _controller?.Dispose();
            _controller = null;
            if (_fallback is not null)
            {
                target.SystemBackdrop = null;
                _fallback.Dispose();
                _fallback = null;
                _fallbackCompositor?.Dispose();
                _fallbackCompositor = null;
            }
            base.OnTargetDisconnected(target);
        }
    }
}
#endif
