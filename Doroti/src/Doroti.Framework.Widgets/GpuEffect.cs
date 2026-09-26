using Doroti.Ui;

namespace Doroti.Framework.Widgets;

/// <summary>Processes a child image on the selected host GPU. Visual changes do
/// not change layout, hit testing, or semantic bounds. Unsupported hosts report an error.</summary>
public class GpuEffect : SingleChildRenderObjectWidget
{
    public GpuEffectProgram effect { get; }
    public GpuEffectParameters parameters { get; }
    public bool enabled { get; }
    public GpuEffectController? controller { get; }
    private readonly bool _backdrop;

    public GpuEffect(GpuEffectProgram effect, Widget? child = null,
        GpuEffectParameters? parameters = null, bool enabled = true, Key? key = null, GpuEffectController? controller = null)
        : this(effect, child, parameters, enabled, key, false, controller) { }

    protected GpuEffect(GpuEffectProgram effect, Widget? child,
        GpuEffectParameters? parameters, bool enabled, Key? key, bool backdrop, GpuEffectController? controller)
        : base(key: key, child: child)
    {
        this.effect = effect ?? throw new ArgumentNullException(nameof(effect));
        this.parameters = parameters ?? GpuEffectParameters.Empty;
        this.enabled = enabled;
        _backdrop = backdrop;
        this.controller = controller;
    }

    public override RenderObject createRenderObject(BuildContext context) => new RenderGpuEffect(this);
    public override void updateRenderObject(BuildContext context, RenderObject renderObject) =>
        ((RenderGpuEffect)renderObject).Update(this);

    private sealed class RenderGpuEffect(GpuEffect widget) : RenderProxyBox
    {
        private GpuEffect _widget = widget;
        public override bool alwaysNeedsCompositing => child is not null && _widget.enabled;
        public override bool isRepaintBoundary => alwaysNeedsCompositing;
        public override void attach(PipelineOwner owner)
        {
            base.attach(owner);
            _widget.controller?.addListener(ParametersChanged);
        }
        public override void detach()
        {
            _widget.controller?.removeListener(ParametersChanged);
            base.detach();
        }
        private void ParametersChanged()
        {
            if (isRepaintBoundary) markNeedsCompositedLayerUpdate();
        }

        internal void Update(GpuEffect next)
        {
            var boundary = isRepaintBoundary;
            var previous = _widget;
            if (attached && !ReferenceEquals(previous.controller, next.controller))
            {
                previous.controller?.removeListener(ParametersChanged);
                next.controller?.addListener(ParametersChanged);
            }
            _widget = next;
            if (boundary != isRepaintBoundary)
            {
                markNeedsCompositingBitsUpdate();
                markNeedsPaint();
            }
            else if (next.enabled && (!ReferenceEquals(previous.effect, next.effect) ||
                !ReferenceEquals(previous.parameters, next.parameters) || !ReferenceEquals(previous.controller, next.controller) || previous._backdrop != next._backdrop))
                markNeedsCompositedLayerUpdate();
        }

        public override OffsetLayer updateCompositedLayer(OffsetLayer? oldLayer)
        {
            var result = oldLayer as GpuEffectLayer ?? new GpuEffectLayer();
            result.Update(_widget.effect, _widget.controller?.Parameters ?? _widget.parameters, paintBounds, _widget._backdrop);
            return result;
        }
    }
}

/// <summary>Filters preceding Skia content in the current layer, then paints the
/// child. Native platform surfaces outside that layer are not captured.</summary>
public sealed class GpuBackdropEffect : GpuEffect
{
    public GpuBackdropEffect(GpuEffectProgram effect, Widget? child = null,
        GpuEffectParameters? parameters = null, bool enabled = true, Key? key = null, GpuEffectController? controller = null)
        : base(effect, child, parameters, enabled, key, true, controller) { }
}
