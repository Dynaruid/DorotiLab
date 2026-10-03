using Doroti.Ui;

namespace Doroti.Skia.Rendering;

public sealed partial class SkiaSceneRenderer
{
    private ISkiaGpuEffectBackend? _featureGpuBackend;
    public GraphicsFeatureSupport SceneFeatures => new(_diagnosticsBackend,
        Interlocked.Read(ref _contextGeneration), SkSL: !_disposed, Wgsl: !_disposed && _featureGpuBackend is not null,
        VariableBlur: !_disposed, EffectBudgetBytes: _featureGpuBackend?.AvailableCaptureBytes ?? 0,
        Reason: _disposed ? "Renderer owner is retired." : _featureGpuBackend is null ? "WGSL requires an attached GPU effect backend." : null);
    GraphicsFeatureSupport ISceneHostCapability.Features => SceneFeatures;
    public SemanticsFeatureSupport SemanticsFeatures { get; set; } = new(Reason: "Detailed native semantics support has not been configured.");
    SemanticsFeatureSupport ISemanticsHostCapability.Features => _disposed
        ? new(Reason: "Semantics owner is retired.") : SemanticsFeatures;
}
