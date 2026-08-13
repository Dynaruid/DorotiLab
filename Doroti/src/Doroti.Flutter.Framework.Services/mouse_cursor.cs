#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/mouse_cursor.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Services;

public class MouseCursorManager
{
    public virtual MouseCursor fallbackMouseCursor { get; private set; } = default!;
    internal virtual DartMap<long, MouseCursorSession> _lastSession { get; private set; } = new DartMap<long, MouseCursorSession>();

    public MouseCursorManager(MouseCursor fallbackMouseCursor)
    {
        this.fallbackMouseCursor = fallbackMouseCursor;
        System.Diagnostics.Debug.Assert((!object.Equals(fallbackMouseCursor, MouseCursor.defer)));
    }

    public virtual MouseCursor? debugDeviceActiveCursor(long device)
    {
        MouseCursor? result = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                result = _lastSession.GetValueOrDefault(device)?.cursor;
                return true;
            });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleDeviceCursorUpdate(long device, IPointerEvent triggeringEvent, IEnumerable<MouseCursor> cursorCandidates)
    {
        if (triggeringEvent is IPointerRemovedEvent)
        {
            _lastSession.remove(device);
            return;
        }
        MouseCursorSession? lastSession = _lastSession.GetValueOrDefault(device);
        MouseCursor nextCursor = (_DeferringMouseCursor.firstNonDeferred(cursorCandidates) ?? fallbackMouseCursor);
        DartRuntimePrimitives.Assert(() => (nextCursor is not _DeferringMouseCursor));
        if ((object.Equals(lastSession?.cursor, nextCursor)))
        {
            return;
        }
        MouseCursorSession nextSession = nextCursor.createSession(device);
        _lastSession[device] = nextSession;
        lastSession?.dispose();
        _ = nextSession.activate();
    }

}

public abstract class MouseCursorSession
{
    public virtual MouseCursor cursor { get; private set; } = default!;
    public virtual long device { get; private set; } = default!;

    protected MouseCursorSession(MouseCursor cursor, long device)
    {
        this.cursor = cursor;
        this.device = device;
    }

    public abstract Future activate();
    public abstract void dispose();
}

public abstract class MouseCursor : Diagnosticable
{
    public static MouseCursor defer = new _DeferringMouseCursor();
    public static MouseCursor uncontrolled = new _NoopMouseCursor();

    protected MouseCursor()
    {
    }

    public abstract MouseCursorSession createSession(long device);
    public abstract string debugDescription { get; }
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string debugDescription = this.debugDescription;
        if ((FoundationRuntimePorts.EnumIndex(minLevel) >= FoundationRuntimePorts.EnumIndex(DiagnosticLevel.info)))
        {
            return debugDescription;
        }
        return base.ToString();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DeferringMouseCursor : MouseCursor
{
    internal _DeferringMouseCursor()
    {
    }

    public override MouseCursorSession createSession(long device)
    {
        DartRuntimePrimitives.Assert(() => false);
        throw new NotImplementedException();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string debugDescription => "defer";
    public static MouseCursor? firstNonDeferred(IEnumerable<MouseCursor> cursors)
    {
        foreach (var cursor in cursors)
        {
            if ((!object.Equals(cursor, MouseCursor.defer)))
            {
                return cursor;
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _NoopMouseCursorSession : MouseCursorSession
{
    internal _NoopMouseCursorSession(_NoopMouseCursor cursor, long device) : base(cursor: cursor, device: device)
    {
    }

    public async override Future activate()
    {
    }

    public override void dispose()
    {
    }

}

internal class _NoopMouseCursor : MouseCursor
{
    internal _NoopMouseCursor()
    {
    }

    public override MouseCursorSession createSession(long device) => new _NoopMouseCursorSession(this, device);
    public override string debugDescription => "uncontrolled";
}

internal class _SystemMouseCursorSession : MouseCursorSession
{
    internal _SystemMouseCursorSession(SystemMouseCursor cursor, long device) : base(cursor: cursor, device: device)
    {
    }

    public override MouseCursor cursor => ((SystemMouseCursor?)base.cursor)!;
    public override Future activate()
    {
        return SystemChannels.mouseCursor.invokeMethod<object?>("activateSystemCursor", new DartMap<object, object> { ["device"] = device, ["kind"] = ((SystemMouseCursor)cursor).kind });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
    }

}

public class SystemMouseCursor : MouseCursor
{
    public virtual string kind { get; private set; } = default!;

    public SystemMouseCursor(string kind)
    {
        this.kind = kind;
    }

    public override string debugDescription => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "SystemMouseCursor"))}({kind})";
    public override MouseCursorSession createSession(long device) => new _SystemMouseCursorSession(this, device);
    public override bool Equals(object? other)
    {
        var __other = other as SystemMouseCursor;
        if (__other is null) return false;
        if ((!object.Equals(__other.GetType(), this.GetType())))
        {
            return false;
        }
        return ((__other is SystemMouseCursor) && (((SystemMouseCursor)((SystemMouseCursor)__other)).kind == kind));
    }

    public override int GetHashCode() => kind.GetHashCode();
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.Add(new DiagnosticsProperty<string>("kind", kind, level: DiagnosticLevel.debug));
    }

}

public abstract class SystemMouseCursors
{
    public static SystemMouseCursor none = new SystemMouseCursor(kind: "none");
    public static SystemMouseCursor basic = new SystemMouseCursor(kind: "basic");
    public static SystemMouseCursor click = new SystemMouseCursor(kind: "click");
    public static SystemMouseCursor forbidden = new SystemMouseCursor(kind: "forbidden");
    public static SystemMouseCursor wait = new SystemMouseCursor(kind: "wait");
    public static SystemMouseCursor progress = new SystemMouseCursor(kind: "progress");
    public static SystemMouseCursor contextMenu = new SystemMouseCursor(kind: "contextMenu");
    public static SystemMouseCursor help = new SystemMouseCursor(kind: "help");
    public static SystemMouseCursor text = new SystemMouseCursor(kind: "text");
    public static SystemMouseCursor verticalText = new SystemMouseCursor(kind: "verticalText");
    public static SystemMouseCursor cell = new SystemMouseCursor(kind: "cell");
    public static SystemMouseCursor precise = new SystemMouseCursor(kind: "precise");
    public static SystemMouseCursor move = new SystemMouseCursor(kind: "move");
    public static SystemMouseCursor grab = new SystemMouseCursor(kind: "grab");
    public static SystemMouseCursor grabbing = new SystemMouseCursor(kind: "grabbing");
    public static SystemMouseCursor noDrop = new SystemMouseCursor(kind: "noDrop");
    public static SystemMouseCursor alias = new SystemMouseCursor(kind: "alias");
    public static SystemMouseCursor copy = new SystemMouseCursor(kind: "copy");
    public static SystemMouseCursor disappearing = new SystemMouseCursor(kind: "disappearing");
    public static SystemMouseCursor allScroll = new SystemMouseCursor(kind: "allScroll");
    public static SystemMouseCursor resizeLeftRight = new SystemMouseCursor(kind: "resizeLeftRight");
    public static SystemMouseCursor resizeUpDown = new SystemMouseCursor(kind: "resizeUpDown");
    public static SystemMouseCursor resizeUpLeftDownRight = new SystemMouseCursor(kind: "resizeUpLeftDownRight");
    public static SystemMouseCursor resizeUpRightDownLeft = new SystemMouseCursor(kind: "resizeUpRightDownLeft");
    public static SystemMouseCursor resizeUp = new SystemMouseCursor(kind: "resizeUp");
    public static SystemMouseCursor resizeDown = new SystemMouseCursor(kind: "resizeDown");
    public static SystemMouseCursor resizeLeft = new SystemMouseCursor(kind: "resizeLeft");
    public static SystemMouseCursor resizeRight = new SystemMouseCursor(kind: "resizeRight");
    public static SystemMouseCursor resizeUpLeft = new SystemMouseCursor(kind: "resizeUpLeft");
    public static SystemMouseCursor resizeUpRight = new SystemMouseCursor(kind: "resizeUpRight");
    public static SystemMouseCursor resizeDownLeft = new SystemMouseCursor(kind: "resizeDownLeft");
    public static SystemMouseCursor resizeDownRight = new SystemMouseCursor(kind: "resizeDownRight");
    public static SystemMouseCursor resizeColumn = new SystemMouseCursor(kind: "resizeColumn");
    public static SystemMouseCursor resizeRow = new SystemMouseCursor(kind: "resizeRow");
    public static SystemMouseCursor zoomIn = new SystemMouseCursor(kind: "zoomIn");
    public static SystemMouseCursor zoomOut = new SystemMouseCursor(kind: "zoomOut");

}
