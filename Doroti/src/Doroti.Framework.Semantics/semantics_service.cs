// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/semantics/semantics_service.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Semantics;

public abstract class SemanticsService
{
    public static async Future announce(string message, TextDirection textDirection, Assertiveness assertiveness = Assertiveness.polite)
    {
        global::Doroti.Ui.DorotiView? view = PlatformDispatcher.instance.implicitView;
        DartRuntimePrimitives.Assert(() => view is not null);
        var @event = new AnnounceSemanticsEvent(message, textDirection, checked((long)view!.viewId), assertiveness: assertiveness);
        await SystemChannels.accessibility.send(@event.toMap());
    }

    public static async Future sendAnnouncement(DorotiView view, string message, TextDirection textDirection, Assertiveness assertiveness = Assertiveness.polite)
    {
        var @event = new AnnounceSemanticsEvent(message, textDirection, checked((long)view.viewId), assertiveness: assertiveness);
        await SystemChannels.accessibility.send(@event.toMap());
    }

    public static async Future tooltip(string message)
    {
        var @event = new TooltipSemanticsEvent(message);
        await SystemChannels.accessibility.send(@event.toMap());
    }

}

