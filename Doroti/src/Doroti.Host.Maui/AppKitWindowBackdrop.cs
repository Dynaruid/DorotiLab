#if MACOS
using System.Runtime.Versioning;
using AppKit;
using CoreGraphics;
using Doroti.Ui;

namespace Doroti.Host.Maui;

/// <summary>Owns the native background sibling and restores window state on detachment.</summary>
internal sealed class AppKitWindowBackdrop : IDisposable
{
    // Background effects must never intercept the renderer's pointer or scroll input.
    private sealed class BlurView : NSVisualEffectView
    {
        public override NSView? HitTest(CGPoint point) => null;
    }

    [SupportedOSPlatform("macos26.0")]
    private sealed class GlassView : NSGlassEffectView
    {
        public override NSView? HitTest(CGPoint point) => null;
    }

    private readonly DorotiMacOSMetalView _surface;
    private WindowBackdropOptions _options = new();
    private NSWindow? _window;
    private NSColor? _originalBackground;
    private bool _originalOpaque;
    private bool _changedWindow;
    private NSView? _effect;

    internal AppKitWindowBackdrop(DorotiMacOSMetalView surface) => _surface = surface;
    internal WindowBackdropMode AppliedMode { get; private set; }

    internal void Configure(WindowBackdropOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (_options == options) { Synchronize(); return; }
        Detach();
        _options = options;
        Synchronize();
    }

    internal void Synchronize()
    {
        if (_window != _surface.Window) Detach();
        if (_surface.Window is not { } window || _surface.Superview is not { } parent) return;
        if (_window is null)
        {
            _window = window;
            _originalOpaque = window.IsOpaque;
            _originalBackground = window.BackgroundColor;
            AppliedMode = _options.mode;
            if (AppliedMode == WindowBackdropMode.liquidGlass && !OperatingSystem.IsMacOSVersionAtLeast(26))
                AppliedMode = WindowBackdropMode.acrylic;

            switch (AppliedMode)
            {
                case WindowBackdropMode.acrylic:
                case WindowBackdropMode.experimentalAcrylic:
                    _effect = CreateBlur();
                    break;
                case WindowBackdropMode.liquidGlass when OperatingSystem.IsMacOSVersionAtLeast(26):
                    _effect = CreateGlass();
                    break;
                case WindowBackdropMode.system:
                case WindowBackdropMode.solid:
                case WindowBackdropMode.transparent:
                    break;
                default:
                    AppliedMode = _options.fallback == WindowBackdropFallback.solid
                        ? WindowBackdropMode.solid : WindowBackdropMode.transparent;
                    break;
            }
            if (AppliedMode != WindowBackdropMode.system)
            {
                _changedWindow = true;
                window.IsOpaque = AppliedMode == WindowBackdropMode.solid;
                window.BackgroundColor = window.IsOpaque ? NSColor.WindowBackground : NSColor.Clear;
            }
            if (_effect is not null)
            {
                _effect.Identifier = "doroti-window-backdrop";
                _effect.Appearance = _options.theme switch
                {
                    WindowBackdropTheme.light => NSAppearance.GetAppearance(NSAppearance.NameAqua),
                    WindowBackdropTheme.dark => NSAppearance.GetAppearance(NSAppearance.NameDarkAqua),
                    _ => null,
                };
            }
        }
        if (_effect is not { } effect) return;
        if (effect.Superview != parent)
        {
            effect.RemoveFromSuperview();
            parent.AddSubview(effect, NSWindowOrderingMode.Below, _surface);
        }
        effect.Frame = _surface.Frame;
    }

    private NSView CreateBlur() => new BlurView
    {
        BlendingMode = NSVisualEffectBlendingMode.BehindWindow,
        State = NSVisualEffectState.FollowsWindowActiveState,
        Material = _options.acrylicKind switch
        {
            WindowAcrylicKind.thin => NSVisualEffectMaterial.UnderWindowBackground,
            WindowAcrylicKind.@base => NSVisualEffectMaterial.WindowBackground,
            _ => NSVisualEffectMaterial.Sidebar,
        },
    };

    [SupportedOSPlatform("macos26.0")]
    private NSView CreateGlass()
    {
        var glass = new GlassView { CornerRadius = 0 };
        if (_options.tintColor is { } tint)
            glass.TintColor = NSColor.FromRgba((nfloat)tint.r, (nfloat)tint.g, (nfloat)tint.b,
                (nfloat)Math.Clamp(_options.tintOpacity ?? tint.a, 0, 1));
        return glass;
    }

    private void Detach()
    {
        _effect?.RemoveFromSuperview();
        _effect?.Dispose();
        _effect = null;
        if (_changedWindow && _window is { } window)
        {
            window.IsOpaque = _originalOpaque;
            if (_originalBackground is { } background) window.BackgroundColor = background;
        }
        _window = null;
        _changedWindow = false;
        _originalBackground = null;
        AppliedMode = WindowBackdropMode.system;
    }

    public void Dispose() => Detach();
}
#endif
