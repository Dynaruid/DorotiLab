// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/scheduler/priority.dart
using Doroti.Runtime;

namespace Doroti.Framework.Scheduler;

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

