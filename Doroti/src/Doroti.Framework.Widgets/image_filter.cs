// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/image_filter.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

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

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(new _ImageFilterRenderObject__image_filter(this.imageFilter, this.enabled));
    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        DartRuntimePrimitives.Ignore(((Func<_ImageFilterRenderObject__image_filter>)(() =>
{
    var __cascade = (((_ImageFilterRenderObject__image_filter?)(object?)renderObject)!);
    __cascade.enabled = this.enabled;
    __cascade.imageFilter = this.imageFilter;
    return __cascade;
}))());
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.ImageFilter>("imageFilter", this.imageFilter));
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
        get => this._enabled;
        set
        {
            var __value = value;
            if ((this.enabled == __value))
            {
                return;
            }
            bool wasRepaintBoundary__2837 = this.isRepaintBoundary;
            _enabled = __value;
            if ((this.isRepaintBoundary != wasRepaintBoundary__2837))
            {
                markNeedsCompositingBitsUpdate();
            }
            markNeedsPaint();
        }
    }
    public virtual global::Doroti.Ui.ImageFilter imageFilter
    {
        get => this._imageFilter;
        set
        {
            var __value = (ImageFilter)(object)value;
            if ((!object.Equals(__value, this._imageFilter)))
            {
                _imageFilter = __value;
                markNeedsCompositedLayerUpdate();
            }
        }
    }
    public override bool alwaysNeedsCompositing => DartRuntimePrimitives.ConvertValue<bool>(((this.child is not null) && this.enabled));
    public override bool isRepaintBoundary => this.alwaysNeedsCompositing;
    public override global::Doroti.Framework.Rendering.OffsetLayer updateCompositedLayer(global::Doroti.Framework.Rendering.OffsetLayer? oldLayer)
    {
        var __oldLayer = oldLayer is null ? null : (global::Doroti.Framework.Rendering.ImageFilterLayer)(object)oldLayer;
        global::Doroti.Framework.Rendering.ImageFilterLayer layer__3520 = (__oldLayer ?? new global::Doroti.Framework.Rendering.ImageFilterLayer());
        layer__3520.imageFilter = this.imageFilter;
        layer__3520.bounds = this.paintBounds;
        return ((global::Doroti.Framework.Rendering.OffsetLayer)(object?)layer__3520);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
