// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/semantics/semantics_service.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Semantics;

public abstract class SemanticsService
{
    public static async Future announce(string message, TextDirection textDirection, Assertiveness assertiveness = Assertiveness.polite)
    {
        global::Doroti.Ui.DorotiView? view = PlatformDispatcher.instance.implicitView;
        DartRuntimePrimitives.Assert(() => (view is not null));
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

