// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/animated_icons/animated_icons_data.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public abstract class AnimatedIcons
{
    public static AnimatedIconData add_event = ((AnimatedIconData)(object?)Animated_iconsLibrary.__add_event);
    public static AnimatedIconData arrow_menu = ((AnimatedIconData)(object?)Animated_iconsLibrary.__arrow_menu);
    public static AnimatedIconData close_menu = ((AnimatedIconData)(object?)Animated_iconsLibrary.__close_menu);
    public static AnimatedIconData ellipsis_search = ((AnimatedIconData)(object?)Animated_iconsLibrary.__ellipsis_search);
    public static AnimatedIconData event_add = ((AnimatedIconData)(object?)Animated_iconsLibrary.__event_add);
    public static AnimatedIconData home_menu = ((AnimatedIconData)(object?)Animated_iconsLibrary.__home_menu);
    public static AnimatedIconData list_view = ((AnimatedIconData)(object?)Animated_iconsLibrary.__list_view);
    public static AnimatedIconData menu_arrow = ((AnimatedIconData)(object?)Animated_iconsLibrary.__menu_arrow);
    public static AnimatedIconData menu_close = ((AnimatedIconData)(object?)Animated_iconsLibrary.__menu_close);
    public static AnimatedIconData menu_home = ((AnimatedIconData)(object?)Animated_iconsLibrary.__menu_home);
    public static AnimatedIconData pause_play = ((AnimatedIconData)(object?)Animated_iconsLibrary.__pause_play);
    public static AnimatedIconData play_pause = ((AnimatedIconData)(object?)Animated_iconsLibrary.__play_pause);
    public static AnimatedIconData search_ellipsis = ((AnimatedIconData)(object?)Animated_iconsLibrary.__search_ellipsis);
    public static AnimatedIconData view_list = ((AnimatedIconData)(object?)Animated_iconsLibrary.__view_list);

}

public interface AnimatedIconData
{
    public bool matchTextDirection { get; }
}

internal class _AnimatedIconData__animated_icons_data : AnimatedIconData
{
    public virtual Size size { get; private set; } = default!;
    public virtual List<_PathFrames__animated_icons> paths { get; private set; } = default!;
    public virtual bool matchTextDirection { get; private set; } = default!;

    internal _AnimatedIconData__animated_icons_data(Size size, List<_PathFrames__animated_icons> paths, bool matchTextDirection = false)
    {
        this.size = size;
        this.paths = paths;
        this.matchTextDirection = matchTextDirection;
    }

}
