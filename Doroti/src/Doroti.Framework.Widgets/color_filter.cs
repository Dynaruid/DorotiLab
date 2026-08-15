// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/color_filter.dart
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

namespace Doroti.Generated.Framework.Widgets;

public class ColorFiltered : SingleChildRenderObjectWidget
{
    public virtual ColorFilter colorFilter { get; private set; } = default!;

    public ColorFiltered(ColorFilter colorFilter, Widget? child = null, global::Doroti.Generated.Framework.Foundation.Key? key = null) : base(child: child, key: key)
    {
        this.colorFilter = colorFilter;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(new _ColorFilterRenderObject__color_filter(this.colorFilter));
    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        (((_ColorFilterRenderObject__color_filter?)(object?)renderObject)!).colorFilter = this.colorFilter;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.ColorFilter>("colorFilter", this.colorFilter));
    }

}

internal class _ColorFilterRenderObject__color_filter : global::Doroti.Generated.Framework.Rendering.RenderProxyBox
{
    internal virtual ColorFilter _colorFilter { get; set; } = default!;

    internal _ColorFilterRenderObject__color_filter(ColorFilter _colorFilter)
    {
        this._colorFilter = _colorFilter;
    }

    public virtual global::Doroti.Ui.ColorFilter colorFilter
    {
        get => this._colorFilter;
        set
        {
            var __value = (ColorFilter)(object)value;
            if ((!object.Equals(__value, this._colorFilter)))
            {
                _colorFilter = __value;
                markNeedsPaint();
            }
        }
    }
    public override bool alwaysNeedsCompositing => DartRuntimePrimitives.ConvertValue<bool>((this.child is not null));
    public virtual void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        layer = context.pushColorFilter(offset, this.colorFilter, (global::System.Action<global::Doroti.Generated.Framework.Rendering.PaintingContext, Offset>)base.paint, oldLayer: ((global::Doroti.Generated.Framework.Rendering.ColorFilterLayer?)(object?)this.layer)!);
        DartRuntimePrimitives.Assert(() =>
            {
                this.layer!.debugCreator = this.debugCreator;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

}

