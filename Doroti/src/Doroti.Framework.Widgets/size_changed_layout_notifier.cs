// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/size_changed_layout_notifier.dart
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
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderSizeChangedWithCallback__size_changed_layout_notifier(onLayoutChangedCallback: ((global::System.Action)(() => {
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

    public virtual void performLayout()
    {
        base.performLayout();
        if (((this._oldSize is not null) && (!object.Equals(this.size, this._oldSize))))
        {
            this.onLayoutChangedCallback();
        }
        _oldSize = this.size;
    }

}

