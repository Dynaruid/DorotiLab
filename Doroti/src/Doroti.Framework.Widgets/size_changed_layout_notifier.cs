// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/size_changed_layout_notifier.dart
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class SizeChangedLayoutNotification : LayoutChangedNotification
{
    public SizeChangedLayoutNotification() { }
}

public class SizeChangedLayoutNotifier : SingleChildRenderObjectWidget
{
    public SizeChangedLayoutNotifier(Key? key = null, Widget? child = null)
        : base(key: key, child: child) { }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderSizeChangedWithCallback__size_changed_layout_notifier(
            onLayoutChangedCallback: () =>
            {
                new SizeChangedLayoutNotification().dispatch(context);
            }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _RenderSizeChangedWithCallback__size_changed_layout_notifier : RenderProxyBox
{
    public virtual Action onLayoutChangedCallback { get; private set; } = default!;
    internal virtual Size? _oldSize { get; set; } = default;

    internal _RenderSizeChangedWithCallback__size_changed_layout_notifier(
        RenderBox? child = null,
        Action onLayoutChangedCallback = default!
    )
        : base(child)
    {
        this.onLayoutChangedCallback = onLayoutChangedCallback;
    }

    public override void performLayout()
    {
        base.performLayout();
        if ((_oldSize is not null) && (!Equals(size, _oldSize)))
        {
            onLayoutChangedCallback();
        }
        _oldSize = size;
    }
}
