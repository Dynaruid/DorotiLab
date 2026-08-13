// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/notification_listener.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

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
        var description__2885 = new List<string>();
        debugFillDescription(description__2885);
        return $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "Notification"))}({string.Join(", ", description__2885)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillDescription(List<string> description)
    {
    }

}

public class NotificationListener<T> : ProxyWidget where T : Notification
{
    public virtual global::System.Func<T, bool>? onNotification { get; private set; }

    public NotificationListener(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!, global::System.Func<T, bool>? onNotification = null) : base(key: key, child: child)
    {
        this.onNotification = onNotification;
    }

    public override Element createElement()
    {
        return ((Element)(object?)new _NotificationElement__notification_listener<T>(this));
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
        var listener__5485 = ((NotificationListener<T>?)(object?)this.widget)!;
        if (((((NotificationListener<T>)listener__5485).onNotification is not null) && (notification is T)))
        {
            T notification__as5574 = (T)(object)notification;
            return ((NotificationListener<T>)listener__5485).onNotification!(notification__as5574);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void notifyClients(ProxyWidget oldWidget)
    {
    }

    public override void attachNotificationTree()
    {
        _notificationTree = new _NotificationNode__framework(this._parent?._notificationTree, this);
    }

}

public class LayoutChangedNotification : Notification
{
    public LayoutChangedNotification()
    {
    }

}

