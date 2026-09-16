// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/animated_icons/animated_icons_data.dart

using Doroti.Ui;

namespace Doroti.Framework.Material;

public abstract class AnimatedIcons
{
    public static AnimatedIconData add_event = Animated_iconsLibrary.__add_event;
    public static AnimatedIconData arrow_menu = Animated_iconsLibrary.__arrow_menu;
    public static AnimatedIconData close_menu = Animated_iconsLibrary.__close_menu;
    public static AnimatedIconData ellipsis_search = Animated_iconsLibrary.__ellipsis_search;
    public static AnimatedIconData event_add = Animated_iconsLibrary.__event_add;
    public static AnimatedIconData home_menu = Animated_iconsLibrary.__home_menu;
    public static AnimatedIconData list_view = Animated_iconsLibrary.__list_view;
    public static AnimatedIconData menu_arrow = Animated_iconsLibrary.__menu_arrow;
    public static AnimatedIconData menu_close = Animated_iconsLibrary.__menu_close;
    public static AnimatedIconData menu_home = Animated_iconsLibrary.__menu_home;
    public static AnimatedIconData pause_play = Animated_iconsLibrary.__pause_play;
    public static AnimatedIconData play_pause = Animated_iconsLibrary.__play_pause;
    public static AnimatedIconData search_ellipsis = Animated_iconsLibrary.__search_ellipsis;
    public static AnimatedIconData view_list = Animated_iconsLibrary.__view_list;

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
