// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/grid_tile.dart

using Doroti.Runtime;

namespace Doroti.Framework.Material;

public class GridTile : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget? header { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? footer { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;

    public GridTile(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget? header = null, global::Doroti.Framework.Widgets.Widget? footer = null, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key)
    {
        this.header = header;
        this.footer = footer;
        this.child = child;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if ((header is null) && (footer is null))
        {
            return child;
        }
        return new global::Doroti.Framework.Widgets.Stack(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection1501 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection1501.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(Positioned.CreateFill(child: child))); if (header is not null) { __collection1501.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Positioned(top: 0.0, left: 0.0, right: 0.0, child: header!))); } if (footer is not null) { __collection1501.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Positioned(left: 0.0, bottom: 0.0, right: 0.0, child: footer!))); } return __collection1501; }))());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
