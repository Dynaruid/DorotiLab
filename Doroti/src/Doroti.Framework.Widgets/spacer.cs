// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/spacer.dart
#pragma warning disable CS8600, CS8603
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

public class Spacer : StatelessWidget
{
    public virtual long flex { get; private set; } = default!;

    public Spacer(global::Doroti.Framework.Foundation.Key? key = null, long flex = 1) : base(key: key)
    {
        this.flex = flex;
        System.Diagnostics.Debug.Assert((flex > 0L));
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new Expanded(flex: this.flex, child: SizedBox.CreateShrink()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

