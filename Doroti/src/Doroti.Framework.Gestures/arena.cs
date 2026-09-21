// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/arena.dart
using Doroti.Runtime;

namespace Doroti.Framework.Gestures;

public enum GestureDisposition
{
    accepted,
    rejected,
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
        _arena._resolve(_pointer, _member, disposition);
    }
}

internal class _GestureArena__arena
{
    public virtual List<GestureArenaMember> members { get; private set; } =
        new List<GestureArenaMember>();
    public virtual bool isOpen { get; set; } = true;
    public virtual bool isHeld { get; set; } = false;
    public virtual bool hasPendingSweep { get; set; } = false;
    public virtual GestureArenaMember? eagerWinner { get; set; } = default;

    public virtual void add(GestureArenaMember member)
    {
        DartRuntimePrimitives.Assert(() => isOpen);
        members.Add(member);
    }

    public override string ToString()
    {
        var buffer = new StringBuffer();
        if (checked((long)members.Count) == 0)
        {
            buffer.write("<empty>");
        }
        else
        {
            buffer.write(
                string.Join(
                    ", ",
                    members.map(
                        (member) =>
                        {
                            if (Equals(member, eagerWinner))
                            {
                                return $"{member} (eager winner)";
                            }
                            return $"{member}";
                        }
                    )
                )
            );
        }
        if (isOpen)
        {
            buffer.write(" [open]");
        }
        if (isHeld)
        {
            buffer.write(" [held]");
        }
        if (hasPendingSweep)
        {
            buffer.write(" [hasPendingSweep]");
        }
        return buffer.ToString();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class GestureArenaManager
{
    internal virtual DartMap<long, _GestureArena__arena> _arenas { get; private set; } =
        new DartMap<long, _GestureArena__arena>();

    public virtual GestureArenaEntry add(long pointer, GestureArenaMember member)
    {
        _GestureArena__arena state = _arenas.putIfAbsent(
            pointer,
            () =>
            {
                DartRuntimePrimitives.Assert(() =>
                    _debugLogDiagnostic(pointer, "★ Opening new gesture arena.")
                );
                return new _GestureArena__arena();
            }
        );
        state.add(member);
        DartRuntimePrimitives.Assert(() => _debugLogDiagnostic(pointer, $"Adding: {member}"));
        return new GestureArenaEntry(this, pointer, member);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void close(long pointer)
    {
        _GestureArena__arena? state = _arenas.GetValueOrDefault(pointer);
        if (state is null)
        {
            return;
        }
        state.isOpen = false;
        DartRuntimePrimitives.Assert(() => _debugLogDiagnostic(pointer, "Closing", state));
        _tryToResolveArena(pointer, state);
    }

    public virtual void sweep(long pointer)
    {
        _GestureArena__arena? state = _arenas.GetValueOrDefault(pointer);
        if (state is null)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => !state.isOpen);
        if (state.isHeld)
        {
            state.hasPendingSweep = true;
            DartRuntimePrimitives.Assert(() =>
                _debugLogDiagnostic(pointer, "Delaying sweep", state)
            );
            return;
        }
        DartRuntimePrimitives.Assert(() => _debugLogDiagnostic(pointer, "Sweeping", state));
        _arenas.remove(pointer);
        if (checked((long)state.members.Count) != 0)
        {
            DartRuntimePrimitives.Assert(() =>
                _debugLogDiagnostic(pointer, $"Winner: {state.members.First()}")
            );
            state.members.First().acceptGesture(pointer);
            for (var i = 1L; i < checked(state.members.Count); i++)
            {
                state.members[(int)i].rejectGesture(pointer);
            }
        }
    }

    public virtual void hold(long pointer)
    {
        _GestureArena__arena? state = _arenas.GetValueOrDefault(pointer);
        if (state is null)
        {
            return;
        }
        state.isHeld = true;
        DartRuntimePrimitives.Assert(() => _debugLogDiagnostic(pointer, "Holding", state));
    }

    public virtual void release(long pointer)
    {
        _GestureArena__arena? state = _arenas.GetValueOrDefault(pointer);
        if (state is null)
        {
            return;
        }
        state.isHeld = false;
        DartRuntimePrimitives.Assert(() => _debugLogDiagnostic(pointer, "Releasing", state));
        if (state.hasPendingSweep)
        {
            sweep(pointer);
        }
    }

    internal virtual void _resolve(
        long pointer,
        GestureArenaMember member,
        GestureDisposition disposition
    )
    {
        _GestureArena__arena? state = _arenas.GetValueOrDefault(pointer);
        if (state is null)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => state.members.Contains(member));
        switch (disposition)
        {
            case GestureDisposition.accepted:
            {
                DartRuntimePrimitives.Assert(() =>
                    _debugLogDiagnostic(pointer, $"Accepting: {member}")
                );
                if (state.isOpen)
                {
                    state.eagerWinner ??= member;
                }
                else
                {
                    DartRuntimePrimitives.Assert(() =>
                        _debugLogDiagnostic(pointer, $"Self-declared winner: {member}")
                    );
                    _resolveInFavorOf(pointer, state, member);
                }
                break;
            }
            case GestureDisposition.rejected:
            {
                DartRuntimePrimitives.Assert(() =>
                    _debugLogDiagnostic(pointer, $"Rejecting: {member}")
                );
                if (Equals(state.eagerWinner, member))
                {
                    state.eagerWinner = null;
                }
                state.members.Remove(member);
                member.rejectGesture(pointer);
                if (!state.isOpen)
                {
                    _tryToResolveArena(pointer, state);
                }
                break;
            }
        }
    }

    internal virtual void _tryToResolveArena(long pointer, _GestureArena__arena state)
    {
        DartRuntimePrimitives.Assert(() => Equals(_arenas.GetValueOrDefault(pointer), state));
        DartRuntimePrimitives.Assert(() => !state.isOpen);
        if (checked(state.members.Count) == 1L)
        {
            DartAsyncRuntime.scheduleMicrotask(() => _resolveByDefault(pointer, state));
        }
        else
        {
            if (checked((long)state.members.Count) == 0)
            {
                _arenas.remove(pointer);
                DartRuntimePrimitives.Assert(() => _debugLogDiagnostic(pointer, "Arena empty."));
            }
            else
            {
                if (state.eagerWinner is not null)
                {
                    DartRuntimePrimitives.Assert(() =>
                        _debugLogDiagnostic(pointer, $"Eager winner: {state.eagerWinner}")
                    );
                    _resolveInFavorOf(pointer, state, state.eagerWinner!);
                }
            }
        }
    }

    internal virtual void _resolveByDefault(long pointer, _GestureArena__arena state)
    {
        if (!_arenas.ContainsKey(pointer))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => Equals(_arenas.GetValueOrDefault(pointer), state));
        DartRuntimePrimitives.Assert(() => !state.isOpen);
        List<GestureArenaMember> membersLocal = state.members;
        DartRuntimePrimitives.Assert(() => checked(membersLocal.Count) == 1L);
        _arenas.remove(pointer);
        DartRuntimePrimitives.Assert(() =>
            _debugLogDiagnostic(pointer, $"Default winner: {state.members.First()}")
        );
        state.members.First().acceptGesture(pointer);
    }

    internal virtual void _resolveInFavorOf(
        long pointer,
        _GestureArena__arena state,
        GestureArenaMember member
    )
    {
        DartRuntimePrimitives.Assert(() => Equals(state, _arenas.GetValueOrDefault(pointer)));
        DartRuntimePrimitives.Assert(() =>
            (state.eagerWinner is null) || Equals(state.eagerWinner, member)
        );
        DartRuntimePrimitives.Assert(() => !state.isOpen);
        _arenas.remove(pointer);
        foreach (GestureArenaMember rejectedMember in state.members)
        {
            if (!Equals(rejectedMember, member))
            {
                rejectedMember.rejectGesture(pointer);
            }
        }
        member.acceptGesture(pointer);
    }

    internal virtual bool _debugLogDiagnostic(
        long pointer,
        string message,
        _GestureArena__arena? state = null
    )
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (DebugLibrary.debugPrintGestureArenaDiagnostics)
            {
                long? count = state?.members?.Count;
                var s = (count != 1L) ? "s" : "";
                PrintLibrary.debugPrint(
                    $"Gesture arena {pointer.ToString().padRight(4L)} ❙ {message}{((count is not null) ? $" with {(count ?? throw new global::System.NullReferenceException("A required value was null."))} member{s}." : "")}"
                );
            }
            return true;
        });
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
