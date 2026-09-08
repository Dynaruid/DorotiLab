using Doroti.Framework.Widgets;

namespace Doroti.Framework.Material;

// Keep the inherited metric transform at the slot boundary. Its stable child
// still receives every changed MediaQuery aspect and the actual constraints.
internal sealed class ScaffoldSlotMediaQuery(
    Widget child,
    bool removeLeftPadding,
    bool removeTopPadding,
    bool removeRightPadding,
    bool removeBottomPadding,
    bool removeBottomInset,
    bool maintainBottomViewPadding) : StatelessWidget
{
    public override Widget build(BuildContext context)
    {
        var data = MediaQuery.of(context).removePadding(
            removeLeft: removeLeftPadding, removeTop: removeTopPadding,
            removeRight: removeRightPadding, removeBottom: removeBottomPadding);
        if (removeBottomInset) data = data.removeViewInsets(removeBottom: true);
        if (maintainBottomViewPadding && data.viewInsets.bottom != 0)
            data = data.copyWith(padding: data.padding.copyWith(bottom: data.viewPadding.bottom));
        return new MediaQuery(data: data, child: child);
    }
}
