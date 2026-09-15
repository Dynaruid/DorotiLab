// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/preferred_size.dart
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

public interface PreferredSizeWidget
{
    public global::Doroti.Ui.Size preferredSize { get; }
}

public class PreferredSize : StatelessWidget, PreferredSizeWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual Size preferredSize { get; private set; } = default!;

    public PreferredSize(global::Doroti.Framework.Foundation.Key? key = null, Size preferredSize = default!, Widget child = default!) : base(key: key)
    {
        this.preferredSize = preferredSize;
        this.child = child;
    }

    public override Widget build(BuildContext context) => this.child;
}

