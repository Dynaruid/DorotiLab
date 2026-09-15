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
        if (Style.Saturation != 1)
            throw new NotSupportedException("Saturation adjustment is not yet supported by the common native effect adapters.");
        Widget foreground = new Container(color: new Color(Style.Tint), child: Child);
        return new ClipRect(child: Style.Sigma == 0 ? foreground : new BackdropFilter(
            filterConfig: ImageFilterConfig.CreateBlur(sigmaX: Style.Sigma, sigmaY: Style.Sigma,
                tileMode: TileMode.clamp, bounded: true), child: foreground));
    });
}
