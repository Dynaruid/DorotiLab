using Doroti.Ui;

namespace Doroti.Framework.Rendering;

internal sealed class VariableBlurImageFilterConfig : ImageFilterConfig
{
    private readonly ImageFilter _template;
    private readonly bool _bounded;
    private readonly double _resolutionScale;
    private readonly bool _adaptiveResolution;
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
        bool bounded,
        double resolutionScale,
        bool adaptiveResolution
    )
    {
        _template = ImageFilter.variableBlur(
            start,
            end,
            startSigma,
            endSigma,
            maxSamples,
            tileMode,
            resolutionScale: resolutionScale,
            adaptiveResolution: adaptiveResolution
        );
        _bounded = bounded;
        _resolutionScale = resolutionScale;
        _adaptiveResolution = adaptiveResolution;
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
            _bounded ? bounds : null,
            _resolutionScale,
            _adaptiveResolution
        );
    }

    public override string debugShortDescription => "variableBlur";

    public override bool Equals(object? obj) =>
        obj is VariableBlurImageFilterConfig other
        && _template == other._template
        && _bounded == other._bounded;

    public override int GetHashCode() => HashCode.Combine(_template, _bounded);
}
