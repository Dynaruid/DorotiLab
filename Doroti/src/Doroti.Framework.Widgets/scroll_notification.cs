// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scroll_notification.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

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
        description.Add($"depth: {depth} ({((depth == 0L) ? "local" : "remote")})");
        description.Add($"{metrics}");
    }

    public virtual long depth => _depth;
}

public class ScrollStartNotification : ScrollNotification
{
    public virtual global::Doroti.Framework.Gestures.DragStartDetails? dragDetails { get; private set; }

    public ScrollStartNotification(ScrollMetrics metrics, BuildContext? context, global::Doroti.Framework.Gestures.DragStartDetails? dragDetails = null) : base(metrics: metrics, context: context)
    {
        this.dragDetails = dragDetails;
    }

    public override void debugFillDescription(List<string> description)
    {
        base.debugFillDescription(description);
        if (dragDetails is not null)
        {
            description.Add($"{dragDetails}");
        }
    }

}

public class ScrollUpdateNotification : ScrollNotification
{
    public virtual global::Doroti.Framework.Gestures.DragUpdateDetails? dragDetails { get; private set; }
    public virtual double? scrollDelta { get; private set; }

    public ScrollUpdateNotification(ScrollMetrics metrics, BuildContext context, global::Doroti.Framework.Gestures.DragUpdateDetails? dragDetails = null, double? scrollDelta = null, long? depth = null) : base(metrics: metrics, context: context)
    {
        this.dragDetails = dragDetails;
        this.scrollDelta = scrollDelta;
        if (depth is not null)
        {
            long depth__value8172 = DartRuntimePrimitives.RequireValue(depth);
            _depth = DartRuntimePrimitives.RequireValue(depth__value8172);
        }
    }

    public override void debugFillDescription(List<string> description)
    {
        base.debugFillDescription(description);
        description.Add($"scrollDelta: {scrollDelta}");
        if (dragDetails is not null)
        {
            description.Add($"{dragDetails}");
        }
    }

}

public class OverscrollNotification : ScrollNotification
{
    public virtual global::Doroti.Framework.Gestures.DragUpdateDetails? dragDetails { get; private set; }
    public virtual double overscroll { get; private set; } = default!;
    public virtual double velocity { get; private set; } = default!;

    public OverscrollNotification(ScrollMetrics metrics, BuildContext context, global::Doroti.Framework.Gestures.DragUpdateDetails? dragDetails = null, double overscroll = default!, double velocity = 0.0) : base(metrics: metrics, context: context)
    {
        this.dragDetails = dragDetails;
        this.overscroll = overscroll;
        this.velocity = velocity;
        System.Diagnostics.Debug.Assert(double.IsFinite(overscroll));
        System.Diagnostics.Debug.Assert(overscroll != 0.0);
    }

    public override void debugFillDescription(List<string> description)
    {
        base.debugFillDescription(description);
        description.Add($"overscroll: {overscroll.toStringAsFixed(1L)}");
        description.Add($"velocity: {velocity.toStringAsFixed(1L)}");
        if (dragDetails is not null)
        {
            description.Add($"{dragDetails}");
        }
    }

}

public class ScrollEndNotification : ScrollNotification
{
    public virtual global::Doroti.Framework.Gestures.DragEndDetails? dragDetails { get; private set; }

    public ScrollEndNotification(ScrollMetrics metrics, BuildContext context, global::Doroti.Framework.Gestures.DragEndDetails? dragDetails = null) : base(metrics: metrics, context: context)
    {
        this.dragDetails = dragDetails;
    }

    public override void debugFillDescription(List<string> description)
    {
        base.debugFillDescription(description);
        if (dragDetails is not null)
        {
            description.Add($"{dragDetails}");
        }
    }

}

public class UserScrollNotification : ScrollNotification
{
    public virtual global::Doroti.Framework.Rendering.ScrollDirection direction { get; private set; } = default!;

    public UserScrollNotification(ScrollMetrics metrics, BuildContext context, global::Doroti.Framework.Rendering.ScrollDirection direction) : base(metrics: metrics, context: context)
    {
        this.direction = direction;
    }

    public override void debugFillDescription(List<string> description)
    {
        base.debugFillDescription(description);
        description.Add($"direction: {direction}");
    }

}

public delegate bool ScrollNotificationPredicate(ScrollNotification notification);

public static partial class Scroll_notificationLibrary
{
    public static bool defaultScrollNotificationPredicate(ScrollNotification notification)
    {
        return notification.depth == 0L;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
