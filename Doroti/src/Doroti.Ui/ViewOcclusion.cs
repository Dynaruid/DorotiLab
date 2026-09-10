namespace Doroti.Ui;

/// <summary>Geometry helpers; all arguments must use the same view/window coordinate units.</summary>
public static class ViewOcclusion
{
    public static ViewPadding SafeEdges(Rect view, Rect container, ViewPadding padding) => new(
        padding.left > 0 && view.left >= container.left ? Math.Clamp(container.left + padding.left - view.left, 0, Math.Max(0, view.width)) : 0,
        padding.top > 0 && view.top >= container.top ? Math.Clamp(container.top + padding.top - view.top, 0, Math.Max(0, view.height)) : 0,
        padding.right > 0 && view.right <= container.right ? Math.Clamp(view.right - container.right + padding.right, 0, Math.Max(0, view.width)) : 0,
        padding.bottom > 0 && view.bottom <= container.bottom ? Math.Clamp(view.bottom - container.bottom + padding.bottom, 0, Math.Max(0, view.height)) : 0);

    public static ViewPadding EdgeInsets(Rect view, Rect occlusion)
    {
        var left = Math.Max(view.left, occlusion.left);
        var top = Math.Max(view.top, occlusion.top);
        var right = Math.Min(view.right, occlusion.right);
        var bottom = Math.Min(view.bottom, occlusion.bottom);
        if (left >= right || top >= bottom) return ViewPadding.zero;
        if (left <= view.left && right >= view.right)
        {
            if (bottom >= view.bottom) return new(0, 0, 0, view.bottom - top);
            if (top <= view.top) return new(0, bottom - view.top, 0, 0);
        }
        else if (top <= view.top && bottom >= view.bottom)
        {
            if (left <= view.left) return new(right - view.left, 0, 0, 0);
            if (right >= view.right) return new(0, 0, view.right - left, 0);
        }
        return ViewPadding.zero;
    }

    public static ViewPadding Scale(ViewPadding value, double scale) =>
        new(value.left * scale, value.top * scale, value.right * scale, value.bottom * scale);
}
