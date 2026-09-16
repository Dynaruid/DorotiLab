// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/semantics/semantics_event.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Semantics;

public enum Assertiveness
{
    polite,
    assertive
}

public abstract class SemanticsEvent
{
    public virtual string type { get; private set; } = default!;

    protected SemanticsEvent(string type)
    {
        this.type = type;
    }

    public virtual DartMap<string, object> toMap(long? nodeId = null)
    {
        var @event = new DartMap<string, object> { ["type"] = this.type, ["data"] = getDataMap() };
        if ((nodeId is not null))
        {
            long nodeId__value2030 = DartRuntimePrimitives.RequireValue(nodeId);
            @event["nodeId"] = DartRuntimePrimitives.RequireValue(nodeId__value2030);
        }
        return @event;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract DartMap<string, object> getDataMap();
    public override string ToString()
    {
        var pairs = new List<string>();
        DartMap<string, object> dataMap = getDataMap();
        List<string> sortedKeys = ((Func<List<string>>)(() =>
{
    var __cascade = dataMap.Keys.ToList();
    __cascade.sort();
    return __cascade;
}))();
        foreach (var key in sortedKeys)
        {
            pairs.Add($"{key}: {dataMap.GetValueOrDefault(key)}");
        }
        return $"{(objectRuntimeTypeFunctions.objectRuntimeType(this, "SemanticsEvent"))}({string.Join(", ", pairs)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class AnnounceSemanticsEvent : SemanticsEvent
{
    public virtual long viewId { get; private set; } = default!;
    public virtual string message { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;
    public virtual Assertiveness assertiveness { get; private set; } = default!;

    public AnnounceSemanticsEvent(string message, TextDirection textDirection, long viewId, Assertiveness assertiveness = Assertiveness.polite) : base("announce")
    {
        this.message = message;
        this.textDirection = textDirection;
        this.viewId = viewId;
        this.assertiveness = assertiveness;
    }

    public override DartMap<string, object> getDataMap()
    {
        return new DartMap<string, object> { ["viewId"] = this.viewId, ["message"] = this.message, ["textDirection"] = FoundationRuntimePorts.EnumIndex(this.textDirection) };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class TooltipSemanticsEvent : SemanticsEvent
{
    public virtual string message { get; private set; } = default!;

    public TooltipSemanticsEvent(string message) : base("tooltip")
    {
        this.message = message;
    }

    public override DartMap<string, object> getDataMap()
    {
        return new DartMap<string, object> { ["message"] = this.message };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class LongPressSemanticsEvent : SemanticsEvent
{
    public LongPressSemanticsEvent() : base("longPress")
    {
    }

    public override DartMap<string, object> getDataMap() => new DartMap<string, object>();
}

public class TapSemanticEvent : SemanticsEvent
{
    public TapSemanticEvent() : base("tap")
    {
    }

    public override DartMap<string, object> getDataMap() => new DartMap<string, object>();
}

public class FocusSemanticEvent : SemanticsEvent
{
    public FocusSemanticEvent() : base("focus")
    {
    }

    public override DartMap<string, object> getDataMap() => new DartMap<string, object>();
}

