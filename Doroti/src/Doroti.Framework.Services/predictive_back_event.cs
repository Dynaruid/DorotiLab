#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/predictive_back_event.dart
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

public enum SwipeEdge
{
    left,
    right
}

public class PredictiveBackEvent
{
    public virtual Offset? touchOffset { get; private set; }
    public virtual double progress { get; private set; } = default!;
    public virtual SwipeEdge swipeEdge { get; private set; } = default!;

    public PredictiveBackEvent(Offset? touchOffset, double progress, SwipeEdge swipeEdge)
    {
        this.touchOffset = touchOffset;
        this.progress = progress;
        this.swipeEdge = swipeEdge;
        System.Diagnostics.Debug.Assert(((progress >= 0.0) && (progress <= 1.0)));
    }

    public static PredictiveBackEvent CreateFromMap(DartMap<string?, object?> map)
    {
        var touchOffset = ((List<object?>?)map.GetValueOrDefault("touchOffset"))!;
        return new PredictiveBackEvent(touchOffset: ((touchOffset is null) ? null : new global::Doroti.Ui.Offset((((double)touchOffset[(int)(0L)]!)).toDouble(), (((double)touchOffset[(int)(1L)]!)).toDouble())), progress: (((double)map.GetValueOrDefault("progress")!)).toDouble(), swipeEdge: System.Enum.GetValues<SwipeEdge>().ToList()[(int)(((long)map.GetValueOrDefault("swipeEdge")!))]);
    }

    public virtual bool isButtonEvent => ((touchOffset is null) || (((progress == 0.0) && (object.Equals(touchOffset, Offset.zero)))));
    public override bool Equals(object? other)
    {
        var __other = other as PredictiveBackEvent;
        if (__other is null) return false;
        if (ReferenceEquals(this, __other))
        {
            return true;
        }
        if ((!object.Equals(__other.GetType(), this.GetType())))
        {
            return false;
        }
        return ((((__other is PredictiveBackEvent) && (object.Equals(touchOffset, ((PredictiveBackEvent)__other).touchOffset))) && (progress == ((PredictiveBackEvent)__other).progress)) && (object.Equals(swipeEdge, ((PredictiveBackEvent)__other).swipeEdge)));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(touchOffset, progress, swipeEdge);
    public override string ToString()
    {
        return $"PredictiveBackEvent{{touchOffset: {touchOffset}, progress: {progress}, swipeEdge: {swipeEdge}}}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

