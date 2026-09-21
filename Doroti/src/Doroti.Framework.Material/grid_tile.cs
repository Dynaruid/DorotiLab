// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/grid_tile.dart

using Doroti.Runtime;

namespace Doroti.Framework.Material;

public class GridTile : StatelessWidget
{
    public virtual Widget? header { get; private set; }
    public virtual Widget? footer { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public GridTile(
        Key? key = null,
        Widget? header = null,
        Widget? footer = null,
        Widget child = default!
    )
        : base(key: key)
    {
        this.header = header;
        this.footer = footer;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        if ((header is null) && (footer is null))
        {
            return child;
        }
        return new Stack(
            children: (
                (Func<List<Widget>>)(
                    () =>
                    {
                        var __collection1501 = new List<Widget>();
                        __collection1501.Add(
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                Positioned.CreateFill(child: child)
                            )
                        );
                        if (header is not null)
                        {
                            __collection1501.Add(
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    new Positioned(top: 0.0, left: 0.0, right: 0.0, child: header!)
                                )
                            );
                        }
                        if (footer is not null)
                        {
                            __collection1501.Add(
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    new Positioned(
                                        left: 0.0,
                                        bottom: 0.0,
                                        right: 0.0,
                                        child: footer!
                                    )
                                )
                            );
                        }
                        return __collection1501;
                    }
                )
            )()
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
