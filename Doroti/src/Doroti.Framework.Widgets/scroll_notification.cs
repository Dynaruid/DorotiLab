// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/scroll_notification.dart
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

public interface ViewportNotificationMixin
{
    long _depth { get; set; }

    public long depth { get; }
    public void debugFillDescription(List<string> description);
}

public interface ViewportElementMixin
{
    public bool onNotification(Notification notification);
}

public abstract class ScrollNotification : LayoutChangedNotification, ViewportNotificationMixin
{
    public virtual ScrollMetrics metrics { get; private set; } = default!;
    public virtual BuildContext? context { get; private set; }
    public virtual long _depth { get; set; } = 0L;

    protected ScrollNotification(ScrollMetrics metrics, BuildContext? context)
    {
        this.metrics = metrics;
        this.context = context;
    }

    public override void debugFillDescription(List<string> description)
    {
        base.debugFillDescription(description);
        description.Add($"depth: {this.depth} ({((this.depth == 0L) ? "local" : "remote")})");
        description.Add($"{this.metrics}");
    }

    public virtual long depth => this._depth;
}

public class ScrollStartNotification : ScrollNotification
{
    public virtual global::Doroti.Generated.Framework.Gestures.DragStartDetails? dragDetails { get; private set; }

    public ScrollStartNotification(ScrollMetrics metrics, BuildContext? context, global::Doroti.Generated.Framework.Gestures.DragStartDetails? dragDetails = null) : base(metrics: metrics, context: context)
    {
        this.dragDetails = dragDetails;
    }

    public override void debugFillDescription(List<string> description)
    {
        base.debugFillDescription(description);
        if ((this.dragDetails is not null))
        {
            description.Add($"{this.dragDetails}");
        }
    }

}

public class ScrollUpdateNotification : ScrollNotification
{
    public virtual global::Doroti.Generated.Framework.Gestures.DragUpdateDetails? dragDetails { get; private set; }
    public virtual double? scrollDelta { get; private set; }

    public ScrollUpdateNotification(ScrollMetrics metrics, BuildContext context, global::Doroti.Generated.Framework.Gestures.DragUpdateDetails? dragDetails = null, double? scrollDelta = null, long? depth = null) : base(metrics: metrics, context: context)
    {
        this.dragDetails = dragDetails;
        this.scrollDelta = scrollDelta;
    }

    public override void debugFillDescription(List<string> description)
    {
        base.debugFillDescription(description);
        description.Add($"scrollDelta: {this.scrollDelta}");
        if ((this.dragDetails is not null))
        {
            description.Add($"{this.dragDetails}");
        }
    }

}

public class OverscrollNotification : ScrollNotification
{
    public virtual global::Doroti.Generated.Framework.Gestures.DragUpdateDetails? dragDetails { get; private set; }
    public virtual double overscroll { get; private set; } = default!;
    public virtual double velocity { get; private set; } = default!;

    public OverscrollNotification(ScrollMetrics metrics, BuildContext context, global::Doroti.Generated.Framework.Gestures.DragUpdateDetails? dragDetails = null, double overscroll = default!, double velocity = 0.0) : base(metrics: metrics, context: context)
    {
        this.dragDetails = dragDetails;
        this.overscroll = overscroll;
        this.velocity = velocity;
        System.Diagnostics.Debug.Assert(double.IsFinite(overscroll));
        System.Diagnostics.Debug.Assert((overscroll != 0.0));
    }

    public override void debugFillDescription(List<string> description)
    {
        base.debugFillDescription(description);
        description.Add($"overscroll: {this.overscroll.toStringAsFixed(1L)}");
        description.Add($"velocity: {this.velocity.toStringAsFixed(1L)}");
        if ((this.dragDetails is not null))
        {
            description.Add($"{this.dragDetails}");
        }
    }

}

public class ScrollEndNotification : ScrollNotification
{
    public virtual global::Doroti.Generated.Framework.Gestures.DragEndDetails? dragDetails { get; private set; }

    public ScrollEndNotification(ScrollMetrics metrics, BuildContext context, global::Doroti.Generated.Framework.Gestures.DragEndDetails? dragDetails = null) : base(metrics: metrics, context: context)
    {
        this.dragDetails = dragDetails;
    }

    public override void debugFillDescription(List<string> description)
    {
        base.debugFillDescription(description);
        if ((this.dragDetails is not null))
        {
            description.Add($"{this.dragDetails}");
        }
    }

}

public class UserScrollNotification : ScrollNotification
{
    public virtual global::Doroti.Generated.Framework.Rendering.ScrollDirection direction { get; private set; } = default!;

    public UserScrollNotification(ScrollMetrics metrics, BuildContext context, global::Doroti.Generated.Framework.Rendering.ScrollDirection direction) : base(metrics: metrics, context: context)
    {
        this.direction = direction;
    }

    public override void debugFillDescription(List<string> description)
    {
        base.debugFillDescription(description);
        description.Add($"direction: {this.direction}");
    }

}

public delegate bool ScrollNotificationPredicate(ScrollNotification notification);

public static partial class Scroll_notificationLibrary
{
    public static bool defaultScrollNotificationPredicate(ScrollNotification notification)
    {
        return (notification.depth == 0L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

