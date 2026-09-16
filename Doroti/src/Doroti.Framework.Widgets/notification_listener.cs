// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/notification_listener.dart
namespace Doroti.Framework.Widgets;

public delegate bool NotificationListenerCallback<T>(T notification) where T : Notification;

public abstract class Notification
{
    protected Notification()
    {
    }

    public virtual void dispatch(BuildContext? target)
    {
        target?.dispatchNotification(this);
    }

    public override string ToString()
    {
        var description = new List<string>();
        debugFillDescription(description);
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "Notification")}({string.Join(", ", description)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillDescription(List<string> description)
    {
    }

}

public class NotificationListener<T> : ProxyWidget where T : Notification
{
    public virtual Func<T, bool>? onNotification { get; private set; }

    public NotificationListener(Key? key = null, Widget child = default!, Func<T, bool>? onNotification = null) : base(key: key, child: child)
    {
        this.onNotification = onNotification;
    }

    public override Element createElement()
    {
        return new _NotificationElement__notification_listener<T>(this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _NotificationElement__notification_listener<T> : ProxyElement, NotifiableElementMixin where T : Notification
{

    internal _NotificationElement__notification_listener(NotificationListener<T> widget) : base(widget)
    {
    }

    public virtual bool onNotification(Notification notification)
    {
        var listener = ((NotificationListener<T>?)widget)!;
        if ((listener.onNotification is not null) && (notification is T))
        {
            T notification__as5574 = (T)notification;
            return listener.onNotification!(notification__as5574);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void notifyClients(ProxyWidget oldWidget)
    {
    }

    public override void attachNotificationTree()
    {
        _notificationTree = new _NotificationNode__framework(_parent?._notificationTree, this);
    }

}

public class LayoutChangedNotification : Notification
{
    public LayoutChangedNotification()
    {
    }

}

