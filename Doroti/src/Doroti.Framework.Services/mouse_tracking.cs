#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/mouse_tracking.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Services;

public delegate void PointerEnterEventListener(PointerEnterEvent @event);

public delegate void PointerExitEventListener(PointerExitEvent @event);

public delegate void PointerHoverEventListener(PointerHoverEvent @event);

public interface IMouseTrackerAnnotation
{
    dynamic onEnter => ((dynamic)this).onEnter;
    dynamic onExit => ((dynamic)this).onExit;
    MouseCursor cursor => (MouseCursor)((dynamic)this).cursor;
    bool validForMouseTracker => (bool)((dynamic)this).validForMouseTracker;
}

public class MouseTrackerAnnotation : Diagnosticable, IMouseTrackerAnnotation
{
    public virtual global::System.Action<PointerEnterEvent>? onEnter { get; private set; }
    public virtual global::System.Action<PointerExitEvent>? onExit { get; private set; }
    public virtual MouseCursor cursor { get; private set; } = default!;
    public virtual bool validForMouseTracker { get; private set; } = default!;

    public MouseTrackerAnnotation(global::System.Action<PointerEnterEvent>? onEnter = null, global::System.Action<PointerExitEvent>? onExit = null, MouseCursor cursor = default!, bool validForMouseTracker = true)
    {
        MouseCursor __cursor = cursor ?? MouseCursor.defer;
        this.onEnter = onEnter;
        this.onExit = onExit;
        this.cursor = __cursor;
        this.validForMouseTracker = validForMouseTracker;
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new FlagsSummary<Delegate?>("callbacks", new DartMap<string, Delegate?> { ["enter"] = ((Delegate?)(object?)this.onEnter), ["exit"] = ((Delegate?)(object?)this.onExit) }, ifEmpty: "<none>"));
        properties.add(new DiagnosticsProperty<MouseCursor>("cursor", this.cursor, defaultValue: MouseCursor.defer));
    }

}
