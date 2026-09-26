using Doroti.Ui;

namespace Doroti.Framework.Rendering;

internal sealed class VariableBlurImageFilterConfig : ImageFilterConfig
{
    private readonly ImageFilter _template;
    private readonly bool _bounded;
    private readonly (
        Offset Start,
        Offset End,
        double StartSigma,
        double EndSigma,
        int MaxSamples
    ) _settings;

    internal VariableBlurImageFilterConfig(
        Offset start,
        Offset end,
        double startSigma,
        double endSigma,
        int maxSamples,
        TileMode tileMode,
        bool bounded
    )
    {
        _template = ImageFilter.variableBlur(
            start,
            end,
            startSigma,
            endSigma,
            maxSamples,
            tileMode
        );
        _bounded = bounded;
        _settings = (start, end, startSigma, endSigma, maxSamples);
    }

    public override ImageFilter resolve(ImageFilterContext context)
    {
        var bounds = context.bounds;
        var settings = _settings;
        // Empty render boxes have no visible output, but must resolve safely.
        var width = bounds.width > 0 ? bounds.width : 1;
        var height = bounds.height > 0 ? bounds.height : 1;
        Offset Point(Offset p) => new(bounds.left + p.dx * width, bounds.top + p.dy * height);
        return ImageFilter.variableBlur(
            Point(settings.Start),
            Point(settings.End),
            settings.StartSigma,
            settings.EndSigma,
            settings.MaxSamples,
            _template.tileMode,
            _bounded ? bounds : null
        );
    }

    public override string debugShortDescription => "variableBlur";

    public override bool Equals(object? obj) =>
        obj is VariableBlurImageFilterConfig other
        && _template == other._template
        && _bounded == other._bounded;

    public override int GetHashCode() => HashCode.Combine(_template, _bounded);
}
