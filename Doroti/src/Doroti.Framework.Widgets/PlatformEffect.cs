using Doroti.Ui;

namespace Doroti.Framework.Widgets;

/// <summary>Bounded inline backdrop with a sharp child. Native sampling is negotiated by the host;
/// unsupported native combinations fail explicitly. The effect adds no focus, semantics, or shield.</summary>
public sealed class PlatformEffect : StatelessWidget
{
    public PlatformEffect(PlatformEffectStyle? style = null, Widget? child = null, Key? key = null) : base(key: key)
    { Style = style ?? new(); Child = child; Style.Validate(); }
    public PlatformEffectStyle Style { get; }
    public Widget? Child { get; }
    public override Widget build(BuildContext context) => new LayoutBuilder(builder: (_, constraints) =>
    {
        if (!constraints.hasBoundedWidth || !constraints.hasBoundedHeight)
            throw new InvalidOperationException("PlatformEffect requires bounded width and height.");
        Widget foreground = new Container(color: new Color(Style.Tint), child: Child);
        return new ClipRect(child: (Style.Match == PlatformEffectMatchPolicy.SolidTint || Style.Sigma == 0 && Style.Saturation == 1) ? foreground : new BackdropFilter(
            filterConfig: new NativeEffectFilterConfig(Style), child: foreground));
    });

    private sealed class NativeEffectFilterConfig(PlatformEffectStyle style) : ImageFilterConfig
    {
        public override ImageFilter resolve(Doroti.Framework.Rendering.ImageFilterContext context) =>
            new(sigmaX: style.Sigma, sigmaY: style.Sigma, tileMode: TileMode.clamp, bounds: context.bounds)
            { PlatformEffectIntent = style };
        public override string debugShortDescription => "platformEffect";
        public override bool Equals(object? other) => other is NativeEffectFilterConfig config && style == config.Style;
        private PlatformEffectStyle Style => style;
        public override int GetHashCode() => style.GetHashCode();
    }
}
