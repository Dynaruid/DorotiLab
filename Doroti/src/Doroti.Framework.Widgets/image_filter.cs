// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/image_filter.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class ImageFiltered : SingleChildRenderObjectWidget
{
    public virtual ImageFilter imageFilter { get; private set; } = default!;
    public virtual bool enabled { get; private set; } = default!;

    public ImageFiltered(global::Doroti.Framework.Foundation.Key? key = null, ImageFilter imageFilter = default!, Widget? child = null, bool enabled = true) : base(key: key, child: child)
    {
        this.imageFilter = imageFilter;
        this.enabled = enabled;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(new _ImageFilterRenderObject__image_filter(imageFilter, enabled));
    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        DartRuntimePrimitives.Ignore(((Func<_ImageFilterRenderObject__image_filter>)(() =>
{
    var __cascade = ((_ImageFilterRenderObject__image_filter?)renderObject)!;
    __cascade.enabled = enabled;
    __cascade.imageFilter = imageFilter;
    return __cascade;
}))());
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.ImageFilter>("imageFilter", imageFilter));
    }

}

internal class _ImageFilterRenderObject__image_filter : global::Doroti.Framework.Rendering.RenderProxyBox
{
    internal virtual bool _enabled { get; set; } = default!;
    internal virtual ImageFilter _imageFilter { get; set; } = default!;

    internal _ImageFilterRenderObject__image_filter(ImageFilter _imageFilter, bool _enabled)
    {
        this._imageFilter = _imageFilter;
        this._enabled = _enabled;
    }

    public virtual bool enabled
    {
        get => _enabled;
        set
        {
            var __value = value;
            if (enabled == __value)
            {
                return;
            }
            bool wasRepaintBoundary = isRepaintBoundary;
            _enabled = __value;
            if (isRepaintBoundary != wasRepaintBoundary)
            {
                markNeedsCompositingBitsUpdate();
            }
            markNeedsPaint();
        }
    }
    public virtual global::Doroti.Ui.ImageFilter imageFilter
    {
        get => _imageFilter;
        set
        {
            var __value = value;
            if (!Equals(__value, _imageFilter))
            {
                _imageFilter = __value;
                markNeedsCompositedLayerUpdate();
            }
        }
    }
    public override bool alwaysNeedsCompositing => DartRuntimePrimitives.ConvertValue<bool>((child is not null) && enabled);
    public override bool isRepaintBoundary => alwaysNeedsCompositing;
    public override global::Doroti.Framework.Rendering.OffsetLayer updateCompositedLayer(global::Doroti.Framework.Rendering.OffsetLayer? oldLayer)
    {
        var __oldLayer = oldLayer is null ? null : (global::Doroti.Framework.Rendering.ImageFilterLayer)oldLayer;
        global::Doroti.Framework.Rendering.ImageFilterLayer layer = __oldLayer ?? new global::Doroti.Framework.Rendering.ImageFilterLayer();
        layer.imageFilter = imageFilter;
        layer.bounds = paintBounds;
        return layer;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
