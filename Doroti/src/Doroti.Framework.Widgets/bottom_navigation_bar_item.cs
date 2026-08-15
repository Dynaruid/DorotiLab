// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/bottom_navigation_bar_item.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public class BottomNavigationBarItem
{
    public virtual global::Doroti.Generated.Framework.Foundation.Key? key { get; private set; }
    public virtual Widget icon { get; private set; } = default!;
    public virtual Widget activeIcon { get; private set; } = default!;
    public virtual string? label { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual string? tooltip { get; private set; }
    public virtual string? semanticsLabel { get; private set; }

    public BottomNavigationBarItem(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget icon = default!, string? label = null, Widget? activeIcon = null, Color? backgroundColor = null, string? tooltip = null, string? semanticsLabel = null)
    {
        this.key = key;
        this.icon = icon;
        this.label = label;
        this.backgroundColor = backgroundColor;
        this.tooltip = tooltip;
        this.semanticsLabel = semanticsLabel;
        this.activeIcon = (activeIcon ?? icon);
    }

}

