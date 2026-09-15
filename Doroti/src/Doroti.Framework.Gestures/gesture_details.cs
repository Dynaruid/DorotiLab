// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/gesture_details.dart
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

public abstract class PositionedGestureDetails
{
    public virtual Offset globalPosition { get; private set; } = default!;
    public virtual Offset localPosition { get; private set; } = default!;
    public PositionedGestureDetails() { }


    protected PositionedGestureDetails(Offset globalPosition, Offset localPosition)
    {
        this.globalPosition = globalPosition;
        this.localPosition = localPosition;
    }

}

