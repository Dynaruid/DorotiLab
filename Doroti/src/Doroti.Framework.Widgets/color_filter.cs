// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/color_filter.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class ColorFiltered : SingleChildRenderObjectWidget
{
    public virtual ColorFilter colorFilter { get; private set; } = default!;

    public ColorFiltered(ColorFilter colorFilter, Widget? child = null, global::Doroti.Framework.Foundation.Key? key = null) : base(child: child, key: key)
    {
        this.colorFilter = colorFilter;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(new _ColorFilterRenderObject__color_filter(colorFilter));
    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        ((_ColorFilterRenderObject__color_filter?)renderObject)!.colorFilter = colorFilter;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.ColorFilter>("colorFilter", colorFilter));
    }

}

internal class _ColorFilterRenderObject__color_filter : global::Doroti.Framework.Rendering.RenderProxyBox
{
    internal virtual ColorFilter _colorFilter { get; set; } = default!;

    internal _ColorFilterRenderObject__color_filter(ColorFilter _colorFilter)
    {
        this._colorFilter = _colorFilter;
    }

    public virtual global::Doroti.Ui.ColorFilter colorFilter
    {
        get => _colorFilter;
        set
        {
            var __value = value;
            if (!Equals(__value, _colorFilter))
            {
                _colorFilter = __value;
                markNeedsPaint();
            }
        }
    }
    public override bool alwaysNeedsCompositing => DartRuntimePrimitives.ConvertValue<bool>(child is not null);
    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        layer = context.pushColorFilter(offset, colorFilter, base.paint, oldLayer: ((global::Doroti.Framework.Rendering.ColorFilterLayer?)layer)!);
        DartRuntimePrimitives.Assert(() =>
            {
                layer!.debugCreator = debugCreator;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

}

