// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/drag.dart
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

public abstract class Drag
{
    public virtual void update(DragUpdateDetails details)
    {
    }

    public virtual void end(DragEndDetails details)
    {
    }

    public virtual void cancel()
    {
    }

}

