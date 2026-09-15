// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/unique_widget.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Widgets;

public abstract class UniqueWidget<T> : StatefulWidget where T : IState
{
    protected UniqueWidget(GlobalKey<T> key) : base(key: key)
    {
    }

    public abstract override IState createState();
    public virtual T? currentState
    {
        get
        {
            var globalKey = ((GlobalKey<T>?)(object?)this.key!)!;
            return ((GlobalKey<T>)globalKey).currentState;
        }
    }
}

