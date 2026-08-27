// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/team.dart
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

namespace Doroti.Framework.Gestures;

internal class _CombiningGestureArenaEntry__team : GestureArenaEntry
{
    internal virtual _CombiningGestureArenaMember__team _combiner { get; private set; } = default!;
    internal virtual GestureArenaMember _member { get; private set; } = default!;

    internal _CombiningGestureArenaEntry__team(_CombiningGestureArenaMember__team _combiner, GestureArenaMember _member)
    {
        this._combiner = _combiner;
        this._member = _member;
    }

    public override void resolve(GestureDisposition disposition)
    {
        this._combiner._resolve(this._member, disposition);
    }

}

public class _CombiningGestureArenaMember__team : GestureArenaMember
{
    internal virtual GestureArenaTeam _owner { get; private set; } = default!;
    internal virtual List<GestureArenaMember> _members { get; private set; } = new List<GestureArenaMember>();
    internal virtual long _pointer { get; private set; } = default!;
    internal virtual bool _resolved { get; set; } = false;
    internal virtual GestureArenaMember? _winner { get; set; } = default;
    internal virtual GestureArenaEntry? _entry { get; set; } = default;

    internal _CombiningGestureArenaMember__team(GestureArenaTeam _owner, long _pointer)
    {
        this._owner = _owner;
        this._pointer = _pointer;
    }

    public virtual void acceptGesture(long pointer)
    {
        DartRuntimePrimitives.Assert(() => (this._pointer == pointer));
        DartRuntimePrimitives.Assert(() => ((this._winner is not null) || (checked((long)(this._members.Count)) != 0)));
        _close();
        _winner ??= (((GestureArenaTeam)this._owner).captain ?? this._members[(int)(0L)]);
        foreach (GestureArenaMember member in this._members)
        {
            if ((!object.Equals(member, this._winner)))
            {
                member.rejectGesture(pointer);
            }
        }
        this._winner!.acceptGesture(pointer);
    }

    public virtual void rejectGesture(long pointer)
    {
        DartRuntimePrimitives.Assert(() => (this._pointer == pointer));
        _close();
        foreach (GestureArenaMember member in this._members)
        {
            member.rejectGesture(pointer);
        }
    }

    internal virtual void _close()
    {
        DartRuntimePrimitives.Assert(() => !this._resolved);
        _resolved = true;
        _CombiningGestureArenaMember__team? combiner = ((GestureArenaTeam)this._owner)._combiners.remove(this._pointer);
        DartRuntimePrimitives.Assert(() => (object.Equals(combiner, this)));
    }

    internal virtual GestureArenaEntry _add(long pointer, GestureArenaMember member)
    {
        DartRuntimePrimitives.Assert(() => !this._resolved);
        DartRuntimePrimitives.Assert(() => (this._pointer == pointer));
        this._members.Add(member);
        _entry ??= GestureBinding.instance.gestureArena.add(pointer, this);
        return new _CombiningGestureArenaEntry__team(this, member);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _resolve(GestureArenaMember member, GestureDisposition disposition)
    {
        if (this._resolved)
        {
            return;
        }
        switch (disposition)
        {
            case GestureDisposition.accepted:
                {
                    _winner ??= (((GestureArenaTeam)this._owner).captain ?? member);
                    this._entry!.resolve(disposition);
                    break;
                }
            case GestureDisposition.rejected:
                {
                    this._members.Remove(member);
                    member.rejectGesture(this._pointer);
                    if ((checked((long)(this._members.Count)) == 0))
                    {
                        this._entry!.resolve(disposition);
                    }
                    break;
                }
        }
    }

}

public class GestureArenaTeam
{
    internal virtual DartMap<long, _CombiningGestureArenaMember__team> _combiners { get; private set; } = new DartMap<long, _CombiningGestureArenaMember__team>();
    public virtual GestureArenaMember? captain { get; set; } = default;

    public virtual GestureArenaEntry add(long pointer, GestureArenaMember member)
    {
        _CombiningGestureArenaMember__team combiner = this._combiners.putIfAbsent(pointer, (() => new _CombiningGestureArenaMember__team(this, pointer)));
        return combiner._add(pointer, member);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

