// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/bottom_navigation_bar_item.dart
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class BottomNavigationBarItem
{
    public virtual Key? key { get; private set; }
    public virtual Widget icon { get; private set; } = default!;
    public virtual Widget activeIcon { get; private set; } = default!;
    public virtual string? label { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual string? tooltip { get; private set; }
    public virtual string? semanticsLabel { get; private set; }

    public BottomNavigationBarItem(Key? key = null, Widget icon = default!, string? label = null, Widget? activeIcon = null, Color? backgroundColor = null, string? tooltip = null, string? semanticsLabel = null)
    {
        this.key = key;
        this.icon = icon;
        this.label = label;
        this.backgroundColor = backgroundColor;
        this.tooltip = tooltip;
        this.semanticsLabel = semanticsLabel;
        this.activeIcon = activeIcon ?? icon;
    }

}

