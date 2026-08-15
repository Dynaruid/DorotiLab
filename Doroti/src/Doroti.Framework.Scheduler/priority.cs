#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/scheduler/priority.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Scheduler;

public class Priority
{
    internal virtual long _value { get; private set; } = default!;
    public static Priority idle = new Priority(0L);
    public static Priority animation = new Priority(100000L);
    public static Priority touch = new Priority(200000L);
    public const long kMaxOffset = 10000L;

    public Priority(long _value)
    {
        this._value = _value;
    }

    public virtual long value => _value;
    public virtual Priority op_Add(long offset)
    {
        if ((offset.abs() > kMaxOffset))
        {
            offset = (kMaxOffset * Math.Sign(offset));
        }
        return new Priority((_value + offset));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Priority op_Subtract(long offset) => (this.op_Add((-offset)));
}

