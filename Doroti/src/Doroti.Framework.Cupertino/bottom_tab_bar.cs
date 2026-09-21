// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/bottom_tab_bar.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class Bottom_tab_barLibrary
{
    internal static double _kTabBarHeight = 50.0;
}

public static partial class Bottom_tab_barLibrary
{
    internal static Color _kDefaultTabBarBorderColor = new CupertinoDynamicColor(
        color: new Color(1291845632L),
        darkColor: new Color(687865856L)
    );
}

public static partial class Bottom_tab_barLibrary
{
    internal static Color _kDefaultTabBarInactiveColor = CupertinoColors.inactiveGray;
}

public class CupertinoTabBar : StatelessWidget, PreferredSizeWidget
{
    public virtual List<BottomNavigationBarItem> items { get; private set; } = default!;
    public virtual Action<long>? onTap { get; private set; }
    public virtual long currentIndex { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? activeColor { get; private set; }
    public virtual Color inactiveColor { get; private set; } = default!;
    public virtual double iconSize { get; private set; } = default!;
    public virtual double height { get; private set; } = default!;
    public virtual Border? border { get; private set; }

    public CupertinoTabBar(
        Key? key = null,
        List<BottomNavigationBarItem> items = default!,
        Action<long>? onTap = null,
        long currentIndex = 0,
        Color? backgroundColor = null,
        Color? activeColor = null,
        Color inactiveColor = default!,
        double iconSize = 30.0,
        double? height = null,
        Border? border = default!
    )
        : base(key: key)
    {
        Color __inactiveColor = inactiveColor ?? Bottom_tab_barLibrary._kDefaultTabBarInactiveColor;
        double __height = height ?? Bottom_tab_barLibrary._kTabBarHeight;
        Border? __border =
            border
            ?? new Border(
                top: new BorderSide(
                    color: Bottom_tab_barLibrary._kDefaultTabBarBorderColor,
                    width: 0.0
                )
            );
        this.items = items;
        this.onTap = onTap;
        this.currentIndex = currentIndex;
        this.backgroundColor = backgroundColor;
        this.activeColor = activeColor;
        this.inactiveColor = __inactiveColor;
        this.iconSize = iconSize;
        this.height = __height;
        this.border = __border;
        System.Diagnostics.Debug.Assert(checked(items.Count) >= 2L);
        System.Diagnostics.Debug.Assert(
            (0L <= (currentIndex)) && ((currentIndex) < checked(items.Count))
        );
        System.Diagnostics.Debug.Assert(__height >= 0.0);
    }

    public virtual Size preferredSize => new Size((height));

    public virtual bool opaque(BuildContext context)
    {
        Color backgroundColorLocal =
            backgroundColor ?? CupertinoTheme.of(context).barBackgroundColor;
        return CupertinoDynamicColor.resolve(backgroundColorLocal, context).alpha == 255L;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        double bottomPadding = MediaQuery.viewPaddingOf(context).bottom;
        Color backgroundColorLocal = CupertinoDynamicColor.resolve(
            backgroundColor ?? CupertinoTheme.of(context).barBackgroundColor,
            context
        );
        BorderSide resolveBorderSide(BorderSide side)
        {
            return Equals(side, BorderSide.none)
                ? side
                : side.copyWith(color: CupertinoDynamicColor.resolve(side.color, context));
            throw new InvalidOperationException(
                "Control flow completed without returning a value."
            );
        }
        Border? resolvedBorder =
            (
                (border is null)
                || (!Equals(DartRuntimePrimitives.RuntimeType(border), typeof(Border)))
            )
                ? border
                : new Border(
                    top: resolveBorderSide(border!.top),
                    left: resolveBorderSide(border!.left),
                    bottom: resolveBorderSide(border!.bottom),
                    right: resolveBorderSide(border!.right)
                );
        Color inactive = CupertinoDynamicColor.resolve(inactiveColor, context);
        Widget result = new DecoratedBox(
            decoration: new BoxDecoration(border: resolvedBorder, color: backgroundColorLocal),
            child: new SizedBox(
                height: height + bottomPadding,
                child: IconTheme.merge(
                    data: new IconThemeData(color: inactive, size: iconSize),
                    child: new DefaultTextStyle(
                        style: CupertinoTheme
                            .of(context)
                            .textTheme.tabLabelTextStyle.copyWith(color: inactive),
                        child: new Padding(
                            padding: EdgeInsets.CreateOnly(bottom: bottomPadding),
                            child: new Widgets.Semantics(
                                explicitChildNodes: true,
                                child: new Row(
                                    crossAxisAlignment: CrossAxisAlignment.end,
                                    children: _buildTabItems(context)
                                )
                            )
                        )
                    )
                )
            )
        );
        if (!opaque(context))
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(
                new ClipRect(
                    child: new BackdropFilter(
                        filter: new ImageFilter(sigmaX: 10.0, sigmaY: 10.0),
                        child: result
                    )
                )
            );
        }
        return result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual List<Widget> _buildTabItems(BuildContext context)
    {
        var result = new List<Widget>();
        CupertinoLocalizations localizations = CupertinoLocalizations.of(context);
        for (var index = 0L; index < checked(items.Count); index += 1L)
        {
            var activeLocal = index == currentIndex;
            result.Add(
                _wrapActiveItem(
                    context,
                    new Expanded(
                        child: new TextFieldTapRegion(
                            child: new Widgets.Semantics(
                                selected: activeLocal,
                                hint: localizations.tabSemanticsLabel(
                                    tabIndex: index + 1L,
                                    tabCount: checked(items.Count)
                                ),
                                child: new MouseRegion(
                                    cursor: Foundation.ConstantsLibrary.kIsWeb
                                        ? SystemMouseCursors.click
                                        : MouseCursor.defer,
                                    child: new GestureDetector(
                                        behavior: HitTestBehavior.opaque,
                                        onTap: (onTap is null)
                                            ? null
                                            : (
                                                () =>
                                                {
                                                    onTap!(index);
                                                }
                                            ),
                                        child: new Padding(
                                            padding: EdgeInsets.CreateOnly(bottom: 4.0),
                                            child: new Column(
                                                mainAxisAlignment: MainAxisAlignment.end,
                                                children: _buildSingleTabItem(
                                                    items[(int)index],
                                                    activeLocal
                                                )
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    ),
                    active: activeLocal
                )
            );
        }
        return result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual List<Widget> _buildSingleTabItem(BottomNavigationBarItem item, bool active)
    {
        return (
            (Func<List<Widget>>)(
                () =>
                {
                    var __collection9241 = new List<Widget>();
                    __collection9241.Add(
                        DartRuntimePrimitives.ConvertValue<Widget>(
                            new Expanded(
                                child: new Center(child: active ? item.activeIcon : item.icon)
                            )
                        )
                    );
                    if (item.label is not null)
                    {
                        __collection9241.Add(
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                new Text(item.label!, semanticsLabel: item.semanticsLabel)
                            )
                        );
                    }
                    return __collection9241;
                }
            )
        )();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _wrapActiveItem(BuildContext context, Widget item, bool active)
    {
        if (!active)
        {
            return item;
        }
        Color activeColorLocal = CupertinoDynamicColor.resolve(
            activeColor ?? CupertinoTheme.of(context).primaryColor,
            context
        );
        return IconTheme.merge(
            data: new IconThemeData(color: activeColorLocal),
            child: DefaultTextStyle.merge(
                style: new TextStyle(color: activeColorLocal),
                child: item
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual CupertinoTabBar copyWith(
        Key? key = null,
        List<BottomNavigationBarItem>? items = null,
        Color? backgroundColor = null,
        Color? activeColor = null,
        Color? inactiveColor = null,
        double? iconSize = null,
        double? height = null,
        Border? border = null,
        long? currentIndex = null,
        Action<long>? onTap = null
    )
    {
        return new CupertinoTabBar(
            key: key ?? this.key,
            items: items ?? this.items,
            backgroundColor: backgroundColor ?? this.backgroundColor,
            activeColor: activeColor ?? this.activeColor,
            inactiveColor: inactiveColor ?? this.inactiveColor,
            iconSize: iconSize ?? this.iconSize,
            height: height ?? this.height,
            border: border ?? this.border,
            currentIndex: currentIndex ?? this.currentIndex,
            onTap: onTap ?? this.onTap
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
