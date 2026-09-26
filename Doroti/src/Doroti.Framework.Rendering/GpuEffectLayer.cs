using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public sealed class GpuEffectLayer : OffsetLayer
{
    private GpuEffectProgram? _program;
    private GpuEffectParameters _parameters = GpuEffectParameters.Empty;
    private Rect _bounds;
    private bool _backdrop;

    public void Update(GpuEffectProgram program, GpuEffectParameters parameters, Rect bounds, bool isBackdrop = false)
    {
        if (ReferenceEquals(_program, program) && ReferenceEquals(_parameters, parameters) && Equals(_bounds, bounds) && _backdrop == isBackdrop) return;
        _program = program;
        _parameters = parameters;
        _bounds = bounds;
        _backdrop = isBackdrop;
        markNeedsAddToScene();
    }

    public override void addToScene(SceneBuilder builder)
    {
        engineLayer = builder.pushGpuEffect(_program ?? throw new InvalidOperationException("Missing effect program."),
            _parameters, _bounds, offset, _engineLayer as GpuEffectEngineLayer, _backdrop);
        addChildrenToScene(builder);
        builder.pop();
    }
}
