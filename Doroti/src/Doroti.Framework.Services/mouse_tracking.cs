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

namespace Doroti.Framework.Services;

public delegate void PointerEnterEventListener(PointerEnterEvent @event);

public delegate void PointerExitEventListener(PointerExitEvent @event);

public delegate void PointerHoverEventListener(PointerHoverEvent @event);

public interface IMouseTrackerCallback
{
    void Invoke(object? @event);
}

public sealed class MouseTrackerCallback<TEvent> : IMouseTrackerCallback
{
    private readonly Action<TEvent> _callback;

    public MouseTrackerCallback(Action<TEvent> callback) => _callback = callback;

    public void Invoke(object? @event)
    {
        if (@event is not TEvent typedEvent)
        {
            throw new InvalidCastException($"Mouse tracker callback expected {typeof(TEvent).FullName}.");
        }

        _callback(typedEvent);
    }
}

public interface IMouseTrackerAnnotation
{
    IMouseTrackerCallback? onEnter { get; }
    IMouseTrackerCallback? onExit { get; }
    MouseCursor cursor { get; }
    bool validForMouseTracker { get; }
}

public class MouseTrackerAnnotation : Diagnosticable, IMouseTrackerAnnotation
{
    public virtual IMouseTrackerCallback? onEnter { get; private set; }
    public virtual IMouseTrackerCallback? onExit { get; private set; }
    public virtual MouseCursor cursor { get; private set; } = default!;
    public virtual bool validForMouseTracker { get; private set; } = default!;

    public MouseTrackerAnnotation(global::System.Action<PointerEnterEvent>? onEnter = null, global::System.Action<PointerExitEvent>? onExit = null, MouseCursor cursor = default!, bool validForMouseTracker = true)
    {
        MouseCursor __cursor = cursor ?? MouseCursor.defer;
        this.onEnter = onEnter is null ? null : new MouseTrackerCallback<PointerEnterEvent>(onEnter);
        this.onExit = onExit is null ? null : new MouseTrackerCallback<PointerExitEvent>(onExit);
        this.cursor = __cursor;
        this.validForMouseTracker = validForMouseTracker;
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new FlagsSummary<IMouseTrackerCallback?>("callbacks", new DartMap<string, IMouseTrackerCallback?> { ["enter"] = this.onEnter, ["exit"] = this.onExit }, ifEmpty: "<none>"));
        properties.add(new DiagnosticsProperty<MouseCursor>("cursor", this.cursor, defaultValue: MouseCursor.defer));
    }

}
