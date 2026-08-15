// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/semantics/semantics_event.dart
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

namespace Doroti.Generated.Framework.Semantics;

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
        var @event__1959 = new DartMap<string, object> { ["type"] = this.type, ["data"] = getDataMap() };
        if ((nodeId is not null))
        {
            long nodeId__value2030 = DartRuntimePrimitives.RequireValue(nodeId);
            @event__1959["nodeId"] = DartRuntimePrimitives.RequireValue(nodeId__value2030);
        }
        return @event__1959;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract DartMap<string, object> getDataMap();
    public override string ToString()
    {
        var pairs__2231 = new List<string>();
        DartMap<string, object> dataMap__2282 = getDataMap();
        List<string> sortedKeys__2329 = ((Func<List<string>>)(() =>
{
    var __cascade = dataMap__2282.Keys.ToList();
    __cascade.sort();
    return __cascade;
}))();
        foreach (var key__2388 in sortedKeys__2329)
        {
            pairs__2231.Add($"{key__2388}: {dataMap__2282.GetValueOrDefault(key__2388)}");
        }
        return $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "SemanticsEvent"))}({string.Join(", ", pairs__2231)})";
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

