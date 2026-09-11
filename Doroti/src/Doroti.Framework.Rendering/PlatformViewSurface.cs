using Doroti.Ui;

namespace Doroti.Framework.Rendering;

/// <summary>Bounded native scene leaf. Native input goes directly to the attachment.</summary>
public sealed class RenderPlatformView : RenderBox
{
    private PlatformViewHandle? _handle;
    public PlatformViewHandle? Handle
    {
        get => _handle;
        set { if (_handle == value) return; _handle = value; markNeedsPaint(); markNeedsSemanticsUpdate(); }
    }
    public override bool sizedByParent => true;
    public override bool alwaysNeedsCompositing => true;
    public override Size computeDryLayout(BoxConstraints constraints)
    {
        var value = constraints.biggest;
        if (!value.isFinite) throw new InvalidOperationException("PlatformView requires bounded width and height.");
        return value;
    }
    public override void paint(PaintingContext context, Offset offset)
    {
        if (_handle is { } handle && !size.isEmpty) context.addLayer(new TypedPlatformViewLayer(handle, offset & size));
    }
    public override void describeSemanticsConfiguration(Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        if (_handle is not { } handle) return;
        config.isSemanticBoundary = true;
        config.platformViewId = handle.InstanceId;
        config.hitTestBehavior = Dart_uiLibrary.SemanticsHitTestBehavior.transparent;
    }
}

public sealed class TypedPlatformViewLayer(PlatformViewHandle handle, Rect bounds) : Layer
{
    public override bool supportsRasterization() => false;
    public override void addToScene(SceneBuilder builder) => builder.addPlatformView(handle, bounds.topLeft, bounds.width, bounds.height);
}

public sealed class RenderPointerInterceptor : RenderProxyBox
{
    private bool _intercepting;
    private bool _debug;
    public bool Intercepting { get => _intercepting; set { if (_intercepting == value) return; _intercepting = value; markNeedsCompositingBitsUpdate(); markNeedsPaint(); } }
    public bool Debug { get => _debug; set { if (_debug == value) return; _debug = value; markNeedsPaint(); } }
    public override bool alwaysNeedsCompositing => Intercepting;
    public override void paint(PaintingContext context, Offset offset)
    {
        if (Intercepting && !size.isEmpty) context.addLayer(new InputShieldLayer(offset & size, Debug));
        base.paint(context, offset);
    }
}

public sealed class InputShieldLayer(Rect bounds, bool debug) : Layer
{
    public override bool supportsRasterization() => false;
    public override void addToScene(SceneBuilder builder) => builder.addInputShield(bounds, debug);
}
