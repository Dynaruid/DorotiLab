// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/spacer.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public class Spacer : StatelessWidget
{
    public virtual long flex { get; private set; } = default!;

    public Spacer(global::Doroti.Generated.Framework.Foundation.Key? key = null, long flex = 1) : base(key: key)
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

