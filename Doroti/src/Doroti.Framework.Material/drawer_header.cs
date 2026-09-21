// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/drawer_header.dart

using Doroti.Runtime;

namespace Doroti.Framework.Material;

public static partial class Drawer_headerLibrary
{
    internal static double _kDrawerHeaderHeight = 160.0 + 1.0;
}

public class DrawerHeader : StatelessWidget
{
    public virtual Decoration? decoration { get; private set; }
    public virtual EdgeInsetsGeometry padding { get; private set; } = default!;
    public virtual EdgeInsetsGeometry? margin { get; private set; }
    public virtual Duration duration { get; private set; } = default!;
    public virtual Curve curve { get; private set; } = default!;
    public virtual Widget? child { get; private set; }

    public DrawerHeader(
        Key? key = null,
        Decoration? decoration = null,
        EdgeInsetsGeometry? margin = default!,
        EdgeInsetsGeometry padding = default!,
        Duration? duration = null,
        Curve curve = default!,
        Widget? child = default!
    )
        : base(key: key)
    {
        EdgeInsetsGeometry? __margin = margin ?? EdgeInsets.CreateOnly(bottom: 8.0);
        EdgeInsetsGeometry __padding = padding ?? new EdgeInsets(16.0, 16.0, 16.0, 8.0);
        Duration __duration = duration ?? Duration.Create(milliseconds: 250);
        Curve __curve = curve ?? Curves.fastOutSlowIn;
        this.decoration = decoration;
        this.margin = __margin;
        this.padding = __padding;
        this.duration = __duration;
        this.curve = __curve;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        ThemeData theme = Theme.of(context);
        double statusBarHeight = MediaQuery.paddingOf(context).top;
        return new Container(
            height: statusBarHeight + Drawer_headerLibrary._kDrawerHeaderHeight,
            margin: margin,
            decoration: new BoxDecoration(
                border: new Border(bottom: Divider.createBorderSide(context))
            ),
            child: new AnimatedContainer(
                padding: padding.add(EdgeInsets.CreateOnly(top: statusBarHeight)),
                decoration: decoration,
                duration: (duration),
                curve: curve,
                child: (child is null)
                    ? null
                    : new DefaultTextStyle(
                        style: theme.textTheme.bodyLarge!,
                        child: MediaQuery.CreateRemovePadding(
                            context: context,
                            removeTop: true,
                            child: child!
                        )
                    )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
