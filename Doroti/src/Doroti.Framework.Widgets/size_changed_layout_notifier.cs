// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/size_changed_layout_notifier.dart
#pragma warning disable CS8600, CS8603
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class SizeChangedLayoutNotification : LayoutChangedNotification
{
    public SizeChangedLayoutNotification()
    {
    }

}

public class SizeChangedLayoutNotifier : SingleChildRenderObjectWidget
{
    public SizeChangedLayoutNotifier(global::Doroti.Framework.Foundation.Key? key = null, Widget? child = null) : base(key: key, child: child)
    {
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderSizeChangedWithCallback__size_changed_layout_notifier(onLayoutChangedCallback: ((global::System.Action)(() =>
        {
            new SizeChangedLayoutNotification().dispatch(context);
        }))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _RenderSizeChangedWithCallback__size_changed_layout_notifier : global::Doroti.Framework.Rendering.RenderProxyBox
{
    public virtual global::System.Action onLayoutChangedCallback { get; private set; } = default!;
    internal virtual Size? _oldSize { get; set; } = default;

    internal _RenderSizeChangedWithCallback__size_changed_layout_notifier(global::Doroti.Framework.Rendering.RenderBox? child = null, global::System.Action onLayoutChangedCallback = default!) : base(child)
    {
        this.onLayoutChangedCallback = onLayoutChangedCallback;
    }

    public override void performLayout()
    {
        base.performLayout();
        if (((this._oldSize is not null) && (!object.Equals(this.size, this._oldSize))))
        {
            this.onLayoutChangedCallback();
        }
        _oldSize = this.size;
    }

}

