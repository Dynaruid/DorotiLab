using Doroti.Ui;
using Locale = Doroti.Ui.Locale;
using Rect = Doroti.Ui.Rect;

namespace Doroti.Host.Maui;

/// <summary>Observes the actual render element. Layout consumption stays disabled separately.</summary>
internal sealed partial class MauiViewEnvironment : IDisposable
{
    private readonly View _element;
    private readonly List<Action> _detach = [];
    private bool _disposed;
    private int _refreshPosted;
    internal Doroti.Ui.Size? NativePhysicalSize { get; private set; }
    internal static double ValidScale(double value) => double.IsFinite(value) && value > 0 ? value : 1;
    internal ViewPadding Padding { get; private set; }
    internal ViewPadding Insets { get; private set; }
    internal ViewPadding Gestures { get; private set; }
    internal IReadOnlyList<DisplayFeature> Features { get; private set; } = Array.Empty<DisplayFeature>();
    internal GestureSettings GestureSettings { get; private set; } = new();
    internal DisplayCornerRadii? Corners { get; private set; }
    internal double TextScale { get; private set; } = 1;
    internal Func<double, double>? FontScaler { get; private set; }
    internal AccessibilityFeatures? Accessibility { get; private set; }
    internal bool? Use24Hour { get; private set; }
    internal IReadOnlyList<Locale>? Locales { get; private set; }
    private static Locale ParseLocale(string tag)
    {
        var parts = tag.Split('-');
        var script = parts.Skip(1).FirstOrDefault(part => part.Length == 4);
        return new(parts[0], parts.Skip(1).FirstOrDefault(part => part.Length is 2 or 3), script);
    }
    internal event Action? Changed;
    internal MauiViewEnvironment(View element)
    {
        _element = element;
        element.HandlerChanged += Attach;
        element.Loaded += Attach;
        element.Unloaded += Detach;
        element.SizeChanged += Refresh;
        Attach(null, EventArgs.Empty);
    }
    private void Attach(object? sender, EventArgs args)
    {
        Detach(sender, args);
        if (_disposed) return;
        AttachNative();
        CaptureNative();
        Changed?.Invoke();
    }
    private void Detach(object? sender, EventArgs args)
    {
        foreach (var detach in _detach) detach();
        _detach.Clear();
        NativePhysicalSize = null;
        Padding = Insets = Gestures = ViewPadding.zero;
        Features = Array.Empty<DisplayFeature>();
        Corners = null;
    }
    internal void Refresh(object? sender = null, EventArgs? args = null)
    {
        if (_disposed) return;
        if (_element.Dispatcher.IsDispatchRequired)
        {
            if (Interlocked.Exchange(ref _refreshPosted, 1) == 0)
                _element.Dispatcher.Dispatch(() => { Interlocked.Exchange(ref _refreshPosted, 0); Refresh(); });
            return;
        }
        var before = (Padding, Insets, Gestures, GestureSettings, Corners, TextScale, Accessibility, Use24Hour);
        var previousFeatures = Features;
        var previousLocales = Locales;
        CaptureNative();
        if (before != (Padding, Insets, Gestures, GestureSettings, Corners, TextScale, Accessibility, Use24Hour) ||
            !previousFeatures.SequenceEqual(Features) || !(previousLocales ?? []).SequenceEqual(Locales ?? [])) Changed?.Invoke();
    }
    partial void AttachNative();
    partial void CaptureNative();
    public void Dispose()
    {
        _disposed = true;
        Detach(null, EventArgs.Empty);
        _element.HandlerChanged -= Attach;
        _element.Loaded -= Attach;
        _element.Unloaded -= Detach;
        _element.SizeChanged -= Refresh;
    }
}
