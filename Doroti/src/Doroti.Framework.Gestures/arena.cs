// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/arena.dart
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

namespace Doroti.Generated.Framework.Gestures;

public enum GestureDisposition
{
    accepted,
    rejected
}

public interface GestureArenaMember
{
    public void acceptGesture(long pointer);
    public void rejectGesture(long pointer);
}

public class GestureArenaEntry
{
    internal virtual GestureArenaManager _arena { get; private set; } = default!;
    internal virtual long _pointer { get; private set; } = default!;
    internal virtual GestureArenaMember _member { get; private set; } = default!;
    public GestureArenaEntry() { }


    public GestureArenaEntry(GestureArenaManager _arena, long _pointer, GestureArenaMember _member)
    {
        this._arena = _arena;
        this._pointer = _pointer;
        this._member = _member;
    }

    public virtual void resolve(GestureDisposition disposition)
    {
        this._arena._resolve(this._pointer, this._member, disposition);
    }

}

internal class _GestureArena__arena
{
    public virtual List<GestureArenaMember> members { get; private set; } = new List<GestureArenaMember>();
    public virtual bool isOpen { get; set; } = true;
    public virtual bool isHeld { get; set; } = false;
    public virtual bool hasPendingSweep { get; set; } = false;
    public virtual GestureArenaMember? eagerWinner { get; set; } = default;

    public virtual void add(GestureArenaMember member)
    {
        DartRuntimePrimitives.Assert(() => this.isOpen);
        this.members.Add(member);
    }

    public override string ToString()
    {
        var buffer__2503 = new StringBuffer();
        if ((checked((long)(this.members.Count)) == 0))
        {
            buffer__2503.write("<empty>");
        }
        else
        {
            buffer__2503.write(string.Join(", ", this.members.map<GestureArenaMember, string>(((member) =>
            {
                if ((object.Equals(member, this.eagerWinner)))
                {
                    return $"{member} (eager winner)";
                }
                return $"{member}";
                return default;
            }))));
        }
        if (this.isOpen)
        {
            buffer__2503.write(" [open]");
        }
        if (this.isHeld)
        {
            buffer__2503.write(" [held]");
        }
        if (this.hasPendingSweep)
        {
            buffer__2503.write(" [hasPendingSweep]");
        }
        return buffer__2503.ToString();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class GestureArenaManager
{
    internal virtual DartMap<long, _GestureArena__arena> _arenas { get; private set; } = new DartMap<long, _GestureArena__arena>();

    public virtual GestureArenaEntry add(long pointer, GestureArenaMember member)
    {
        _GestureArena__arena state__3811 = this._arenas.putIfAbsent(pointer, (() =>
        {
            DartRuntimePrimitives.Assert(() => _debugLogDiagnostic(pointer, "★ Opening new gesture arena."));
            return new _GestureArena__arena();
            return default;
        }));
        state__3811.add(member);
        DartRuntimePrimitives.Assert(() => _debugLogDiagnostic(pointer, $"Adding: {member}"));
        return new GestureArenaEntry(this, pointer, member);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void close(long pointer)
    {
        _GestureArena__arena? state__4304 = this._arenas.GetValueOrDefault(pointer);
        if ((state__4304 is null))
        {
            return;
        }
        state__4304.isOpen = false;
        DartRuntimePrimitives.Assert(() => _debugLogDiagnostic(pointer, "Closing", state__4304));
        _tryToResolveArena(pointer, state__4304);
    }

    public virtual void sweep(long pointer)
    {
        _GestureArena__arena? state__5140 = this._arenas.GetValueOrDefault(pointer);
        if ((state__5140 is null))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => !((_GestureArena__arena)state__5140).isOpen);
        if (((_GestureArena__arena)state__5140).isHeld)
        {
            state__5140.hasPendingSweep = true;
            DartRuntimePrimitives.Assert(() => _debugLogDiagnostic(pointer, "Delaying sweep", state__5140));
            return;
        }
        DartRuntimePrimitives.Assert(() => _debugLogDiagnostic(pointer, "Sweeping", state__5140));
        this._arenas.remove(pointer);
        if ((checked((long)(((_GestureArena__arena)state__5140).members.Count)) != 0))
        {
            DartRuntimePrimitives.Assert(() => _debugLogDiagnostic(pointer, $"Winner: {((_GestureArena__arena)state__5140).members.First()}"));
            ((_GestureArena__arena)state__5140).members.First().acceptGesture(pointer);
            for (var i__5844 = 1L; (i__5844 < checked((long)(((_GestureArena__arena)state__5140).members.Count))); i__5844++)
            {
                ((_GestureArena__arena)state__5140).members[(int)(i__5844)].rejectGesture(pointer);
            }
        }
    }

    public virtual void hold(long pointer)
    {
        _GestureArena__arena? state__6451 = this._arenas.GetValueOrDefault(pointer);
        if ((state__6451 is null))
        {
            return;
        }
        state__6451.isHeld = true;
        DartRuntimePrimitives.Assert(() => _debugLogDiagnostic(pointer, "Holding", state__6451));
    }

    public virtual void release(long pointer)
    {
        _GestureArena__arena? state__6935 = this._arenas.GetValueOrDefault(pointer);
        if ((state__6935 is null))
        {
            return;
        }
        state__6935.isHeld = false;
        DartRuntimePrimitives.Assert(() => _debugLogDiagnostic(pointer, "Releasing", state__6935));
        if (((_GestureArena__arena)state__6935).hasPendingSweep)
        {
            sweep(pointer);
        }
    }

    internal virtual void _resolve(long pointer, GestureArenaMember member, GestureDisposition disposition)
    {
        _GestureArena__arena? state__7478 = this._arenas.GetValueOrDefault(pointer);
        if ((state__7478 is null))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => ((_GestureArena__arena)state__7478).members.Contains(member));
        switch (disposition)
        {
            case GestureDisposition.accepted:
                {
                    DartRuntimePrimitives.Assert(() => _debugLogDiagnostic(pointer, $"Accepting: {member}"));
                    if (((_GestureArena__arena)state__7478).isOpen)
                    {
                        state__7478.eagerWinner ??= member;
                    }
                    else
                    {
                        DartRuntimePrimitives.Assert(() => _debugLogDiagnostic(pointer, $"Self-declared winner: {member}"));
                        _resolveInFavorOf(pointer, state__7478, member);
                    }
                    break;
                }
            case GestureDisposition.rejected:
                {
                    DartRuntimePrimitives.Assert(() => _debugLogDiagnostic(pointer, $"Rejecting: {member}"));
                    if ((object.Equals(((_GestureArena__arena)state__7478).eagerWinner, member)))
                    {
                        state__7478.eagerWinner = null;
                    }
                    ((_GestureArena__arena)state__7478).members.Remove(member);
                    member.rejectGesture(pointer);
                    if (!((_GestureArena__arena)state__7478).isOpen)
                    {
                        _tryToResolveArena(pointer, state__7478);
                    }
                    break;
                }
        }
    }

    internal virtual void _tryToResolveArena(long pointer, _GestureArena__arena state)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(this._arenas.GetValueOrDefault(pointer), state)));
        DartRuntimePrimitives.Assert(() => !((_GestureArena__arena)state).isOpen);
        if ((checked((long)(((_GestureArena__arena)state).members.Count)) == 1L))
        {
            DartAsyncRuntime.scheduleMicrotask((() => _resolveByDefault(pointer, state)));
        }
        else
        {
            if ((checked((long)(((_GestureArena__arena)state).members.Count)) == 0))
            {
                this._arenas.remove(pointer);
                DartRuntimePrimitives.Assert(() => _debugLogDiagnostic(pointer, "Arena empty."));
            }
            else
            {
                if ((((_GestureArena__arena)state).eagerWinner is not null))
                {
                    DartRuntimePrimitives.Assert(() => _debugLogDiagnostic(pointer, $"Eager winner: {((_GestureArena__arena)state).eagerWinner}"));
                    _resolveInFavorOf(pointer, state, ((_GestureArena__arena)state).eagerWinner!);
                }
            }
        }
    }

    internal virtual void _resolveByDefault(long pointer, _GestureArena__arena state)
    {
        if (!this._arenas.ContainsKey(pointer))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(this._arenas.GetValueOrDefault(pointer), state)));
        DartRuntimePrimitives.Assert(() => !((_GestureArena__arena)state).isOpen);
        List<GestureArenaMember> members__9182 = ((_GestureArena__arena)state).members;
        DartRuntimePrimitives.Assert(() => (checked((long)(members__9182.Count)) == 1L));
        this._arenas.remove(pointer);
        DartRuntimePrimitives.Assert(() => _debugLogDiagnostic(pointer, $"Default winner: {((_GestureArena__arena)state).members.First()}"));
        ((_GestureArena__arena)state).members.First().acceptGesture(pointer);
    }

    internal virtual void _resolveInFavorOf(long pointer, _GestureArena__arena state, GestureArenaMember member)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(state, this._arenas.GetValueOrDefault(pointer))));
        DartRuntimePrimitives.Assert(() => ((((_GestureArena__arena)state).eagerWinner is null) || (object.Equals(((_GestureArena__arena)state).eagerWinner, member))));
        DartRuntimePrimitives.Assert(() => !((_GestureArena__arena)state).isOpen);
        this._arenas.remove(pointer);
        foreach (GestureArenaMember rejectedMember__9693 in ((_GestureArena__arena)state).members)
        {
            if ((!object.Equals(rejectedMember__9693, member)))
            {
                rejectedMember__9693.rejectGesture(pointer);
            }
        }
        member.acceptGesture(pointer);
    }

    internal virtual bool _debugLogDiagnostic(long pointer, string message, _GestureArena__arena? state = null)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Generated.Framework.Gestures.DebugLibrary.debugPrintGestureArenaDiagnostics)
                {
                    long? count__10031 = ((long?)(state?.members?.Count));
                    var s__10076 = ((count__10031 != 1L) ? "s" : "");
                    global::Doroti.Generated.Framework.Foundation.PrintLibrary.debugPrint($"Gesture arena {pointer.ToString().padRight(4L)} ❙ {message}{((count__10031 is not null) ? $" with {DartRuntimePrimitives.RequireValue(count__10031)} member{s__10076}." : "")}");
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

