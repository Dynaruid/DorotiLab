// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/tooltip_visibility.dart

namespace Doroti.Framework.Material;

internal class _TooltipVisibilityScope__tooltip_visibility : InheritedWidget
{
    public virtual bool visible { get; private set; } = default!;

    internal _TooltipVisibilityScope__tooltip_visibility(Widget child, bool visible) : base(child: child)
    {
        this.visible = visible;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __old = (_TooltipVisibilityScope__tooltip_visibility)oldWidget;
        return __old.visible != visible;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class TooltipVisibility : StatelessWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual bool visible { get; private set; } = default!;

    public TooltipVisibility(Key? key = null, bool visible = default!, Widget child = default!) : base(key: key)
    {
        this.visible = visible;
        this.child = child;
    }

    public static bool of(BuildContext context)
    {
        _TooltipVisibilityScope__tooltip_visibility? visibility = context.dependOnInheritedWidgetOfExactType<_TooltipVisibilityScope__tooltip_visibility>();
        return visibility?.visible ?? true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        return new _TooltipVisibilityScope__tooltip_visibility(visible: visible, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
