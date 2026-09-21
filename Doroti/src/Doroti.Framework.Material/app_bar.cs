// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/app_bar.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

internal delegate _ScrollUnderFlexibleConfig__app_bar _FlexibleConfigBuilder__app_bar(
    BuildContext __unused0
);

public static partial class App_barLibrary
{
    internal static double _kLeadingWidth = ConstantsLibrary.kToolbarHeight;
}

public static partial class App_barLibrary
{
    internal static double _kMaxTitleTextScaleFactor = 1.34;
}

internal enum _SliverAppVariant__app_bar
{
    small,
    medium,
    large,
}

internal class _ToolbarContainerLayout__app_bar : SingleChildLayoutDelegate
{
    public virtual double toolbarHeight { get; private set; } = default!;

    internal _ToolbarContainerLayout__app_bar(double toolbarHeight)
    {
        this.toolbarHeight = toolbarHeight;
    }

    public override BoxConstraints getConstraintsForChild(BoxConstraints constraints)
    {
        return constraints.tighten(height: toolbarHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size getSize(BoxConstraints constraints)
    {
        return new Size(constraints.maxWidth, toolbarHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Offset getPositionForChild(Size size, Size childSize)
    {
        return new Offset(0.0, size.height - childSize.height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRelayout(SingleChildLayoutDelegate oldDelegate) =>
        DartRuntimePrimitives.ConvertValue<bool>(
            toolbarHeight != ((_ToolbarContainerLayout__app_bar)oldDelegate).toolbarHeight
        );
}

internal class _PreferredAppBarSize__app_bar : Size
{
    public virtual double? toolbarHeight { get; private set; }
    public virtual double? bottomHeight { get; private set; }

    internal _PreferredAppBarSize__app_bar(double? toolbarHeight, double? bottomHeight)
        : base((toolbarHeight ?? ConstantsLibrary.kToolbarHeight) + (bottomHeight ?? 0L))
    {
        this.toolbarHeight = toolbarHeight;
        this.bottomHeight = bottomHeight;
    }
}

public class AppBar : StatefulWidget, PreferredSizeWidget
{
    public virtual Widget? leading { get; private set; }
    public virtual bool automaticallyImplyLeading { get; private set; } = default!;
    public virtual Widget? title { get; private set; }
    public virtual List<Widget>? actions { get; private set; }
    public virtual bool automaticallyImplyActions { get; private set; } = default!;
    public virtual Widget? flexibleSpace { get; private set; }
    public virtual PreferredSizeWidget? bottom { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual double? scrolledUnderElevation { get; private set; }
    public virtual Func<ScrollNotification, bool> notificationPredicate { get; private set; } =
        default!;
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual ShapeBorder? shape { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? foregroundColor { get; private set; }
    public virtual IconThemeData? iconTheme { get; private set; }
    public virtual IconThemeData? actionsIconTheme { get; private set; }
    public virtual bool primary { get; private set; } = default!;
    public virtual bool? centerTitle { get; private set; }
    public virtual bool excludeHeaderSemantics { get; private set; } = default!;
    public virtual double? titleSpacing { get; private set; }
    public virtual double toolbarOpacity { get; private set; } = default!;
    public virtual double bottomOpacity { get; private set; } = default!;
    public virtual Size preferredSize { get; private set; } = default!;
    public virtual double? toolbarHeight { get; private set; }
    public virtual double? leadingWidth { get; private set; }
    public virtual TextStyle? toolbarTextStyle { get; private set; }
    public virtual TextStyle? titleTextStyle { get; private set; }
    public virtual SystemUiOverlayStyle? systemOverlayStyle { get; private set; }
    public virtual bool forceMaterialTransparency { get; private set; } = default!;
    public virtual bool useDefaultSemanticsOrder { get; private set; } = default!;
    public virtual Clip? clipBehavior { get; private set; }
    public virtual EdgeInsetsGeometry? actionsPadding { get; private set; }
    public virtual bool animateColor { get; private set; } = default!;

    public AppBar(
        Key? key = null,
        Widget? leading = null,
        bool automaticallyImplyLeading = true,
        Widget? title = null,
        List<Widget>? actions = null,
        bool automaticallyImplyActions = true,
        Widget? flexibleSpace = null,
        PreferredSizeWidget? bottom = null,
        double? elevation = null,
        double? scrolledUnderElevation = null,
        Func<ScrollNotification, bool> notificationPredicate = default!,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        ShapeBorder? shape = null,
        Color? backgroundColor = null,
        Color? foregroundColor = null,
        IconThemeData? iconTheme = null,
        IconThemeData? actionsIconTheme = null,
        bool primary = true,
        bool? centerTitle = null,
        bool excludeHeaderSemantics = false,
        double? titleSpacing = null,
        double toolbarOpacity = 1.0,
        double bottomOpacity = 1.0,
        double? toolbarHeight = null,
        double? leadingWidth = null,
        TextStyle? toolbarTextStyle = null,
        TextStyle? titleTextStyle = null,
        SystemUiOverlayStyle? systemOverlayStyle = null,
        bool forceMaterialTransparency = false,
        bool useDefaultSemanticsOrder = true,
        Clip? clipBehavior = null,
        EdgeInsetsGeometry? actionsPadding = null,
        bool animateColor = false
    )
        : base(key: key)
    {
        Func<ScrollNotification, bool> __notificationPredicate =
            notificationPredicate ?? Scroll_notificationLibrary.defaultScrollNotificationPredicate;
        this.leading = leading;
        this.automaticallyImplyLeading = automaticallyImplyLeading;
        this.title = title;
        this.actions = actions;
        this.automaticallyImplyActions = automaticallyImplyActions;
        this.flexibleSpace = flexibleSpace;
        this.bottom = bottom;
        this.elevation = elevation;
        this.scrolledUnderElevation = scrolledUnderElevation;
        this.notificationPredicate = __notificationPredicate;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.shape = shape;
        this.backgroundColor = backgroundColor;
        this.foregroundColor = foregroundColor;
        this.iconTheme = iconTheme;
        this.actionsIconTheme = actionsIconTheme;
        this.primary = primary;
        this.centerTitle = centerTitle;
        this.excludeHeaderSemantics = excludeHeaderSemantics;
        this.titleSpacing = titleSpacing;
        this.toolbarOpacity = toolbarOpacity;
        this.bottomOpacity = bottomOpacity;
        this.toolbarHeight = toolbarHeight;
        this.leadingWidth = leadingWidth;
        this.toolbarTextStyle = toolbarTextStyle;
        this.titleTextStyle = titleTextStyle;
        this.systemOverlayStyle = systemOverlayStyle;
        this.forceMaterialTransparency = forceMaterialTransparency;
        this.useDefaultSemanticsOrder = useDefaultSemanticsOrder;
        this.clipBehavior = clipBehavior;
        this.actionsPadding = actionsPadding;
        this.animateColor = animateColor;
        preferredSize = new _PreferredAppBarSize__app_bar(
            toolbarHeight,
            bottom?.preferredSize.height
        );
        System.Diagnostics.Debug.Assert((elevation is null) || (elevation >= 0.0));
    }

    public static double preferredHeightFor(BuildContext context, Size preferredSize)
    {
        if (
            (preferredSize is _PreferredAppBarSize__app_bar preferredAppBarSize)
            && (preferredAppBarSize.toolbarHeight is null)
        )
        {
            _PreferredAppBarSize__app_bar preferredSize__as9579 =
                (_PreferredAppBarSize__app_bar)preferredSize;
            return (AppBarTheme.of(context).toolbarHeight ?? ConstantsLibrary.kToolbarHeight)
                + (preferredSize__as9579.bottomHeight ?? 0L);
        }
        return preferredSize.height;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _getEffectiveCenterTitle(ThemeData theme, AppBarThemeData appbarTheme)
    {
        bool platformCenter()
        {
            return theme.platform switch
            {
                TargetPlatform.iOS => (actions is null) || (checked(actions!.Count) < 2L),
                TargetPlatform.macOS => (actions is null) || (checked(actions!.Count) < 2L),
                TargetPlatform.android or TargetPlatform.fuchsia or TargetPlatform.linux => false,
                TargetPlatform.windows => false,
                _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                    throw new InvalidOperationException("Non-exhaustive Dart switch value."),
            };
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        return (centerTitle ?? appbarTheme.centerTitle) ?? platformCenter();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _AppBarState__app_bar());
}

internal class _AppBarState__app_bar : State<AppBar>
{
    internal virtual ScrollNotificationObserverState? _scrollNotificationObserver { get; set; } =
        default;
    internal virtual bool _scrolledUnder { get; set; } = false;

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _scrollNotificationObserver?.removeListener(_handleScrollNotification);
        ScaffoldState? scaffoldState = Scaffold.maybeOf(context);
        if (
            (scaffoldState is not null)
            && (scaffoldState.isDrawerOpen || scaffoldState.isEndDrawerOpen)
        )
        {
            return;
        }
        _scrollNotificationObserver = ScrollNotificationObserver.maybeOf(context);
        _scrollNotificationObserver?.addListener(_handleScrollNotification);
    }

    public override void dispose()
    {
        if (_scrollNotificationObserver is not null)
        {
            _scrollNotificationObserver!.removeListener(_handleScrollNotification);
            _scrollNotificationObserver = null;
        }
        base.dispose();
    }

    internal virtual void _handleScrollNotification(ScrollNotification notification)
    {
        if (
            (notification is ScrollUpdateNotification)
            && widget.notificationPredicate((ScrollUpdateNotification)notification)
        )
        {
            ScrollUpdateNotification notification__as34351 = (ScrollUpdateNotification)notification;
            bool oldScrolledUnder = _scrolledUnder;
            ScrollMetrics metricsLocal = notification__as34351.metrics;
            switch (metricsLocal.axisDirection)
            {
                case AxisDirection.up:
                {
                    _scrolledUnder = metricsLocal.extentAfter > 0L;
                    break;
                }
                case AxisDirection.down:
                {
                    _scrolledUnder = metricsLocal.extentBefore > 0L;
                    break;
                }
                case AxisDirection.right:
                case AxisDirection.left:
                {
                    break;
                }
            }
            if (_scrolledUnder != oldScrolledUnder)
            {
                setState(() => { });
            }
        }
    }

    internal virtual Color _resolveColor(
        HashSet<WidgetState> states,
        Color? widgetColor,
        Color? themeColor,
        Color defaultColor
    )
    {
        return (
                WidgetStateProperty.resolveAs(widgetColor, states)
                ?? WidgetStateProperty.resolveAs(themeColor, states)
            ) ?? WidgetStateProperty.resolveAs(defaultColor, states);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SystemUiOverlayStyle _systemOverlayStyleForBrightness(
        Brightness brightness,
        Color? backgroundColor = null
    )
    {
        SystemUiOverlayStyle style = Equals(brightness, Brightness.dark)
            ? SystemUiOverlayStyle.light
            : SystemUiOverlayStyle.dark;
        return new SystemUiOverlayStyle(
            statusBarColor: backgroundColor,
            statusBarBrightness: style.statusBarBrightness,
            statusBarIconBrightness: style.statusBarIconBrightness,
            systemStatusBarContrastEnforced: style.systemStatusBarContrastEnforced
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            !widget.primary || Widgets.DebugLibrary.debugCheckHasMediaQuery(context)
        );
        DartRuntimePrimitives.Assert(() =>
            DebugLibrary.debugCheckHasMaterialLocalizations(context)
        );
        ThemeData theme = Theme.of(context);
        IconButtonThemeData iconButtonTheme = IconButtonTheme.of(context);
        AppBarThemeData appBarTheme = AppBarTheme.of(context);
        AppBarThemeData defaults = new _AppBarDefaultsM3__app_bar(context);
        ScaffoldState? scaffold = Scaffold.maybeOf(context);
        IModalRoute? parentRoute = ModalRoute<object>.untypedOf(context);
        FlexibleSpaceBarSettings? settings =
            context.dependOnInheritedWidgetOfExactType<FlexibleSpaceBarSettings>();
        var states = (
            (Func<HashSet<WidgetState>>)(
                () =>
                {
                    var __collection37017 = new HashSet<WidgetState>();
                    if (settings?.isScrolledUnder ?? _scrolledUnder)
                    {
                        __collection37017.Add(WidgetState.scrolledUnder);
                    }
                    return __collection37017;
                }
            )
        )();
        bool hasDrawerLocal = scaffold?.hasDrawer ?? false;
        bool hasEndDrawerLocal = scaffold?.hasEndDrawer ?? false;
        bool useCloseButton = (parentRoute?.fullscreenDialog) ?? false;
        double toolbarHeightLocal =
            (widget.toolbarHeight ?? appBarTheme.toolbarHeight) ?? ConstantsLibrary.kToolbarHeight;
        Color backgroundColorLocal = _resolveColor(
            states,
            widget.backgroundColor,
            appBarTheme.backgroundColor,
            defaults.backgroundColor ?? Theme.of(context).colorScheme.surface
        );
        Color scrolledUnderBackground = _resolveColor(
            states,
            widget.backgroundColor,
            appBarTheme.backgroundColor,
            Theme.of(context).colorScheme.surfaceContainer
        );
        var effectiveBackgroundColor = states.Contains(WidgetState.scrolledUnder)
            ? scrolledUnderBackground
            : backgroundColorLocal;
        Color foregroundColorLocal =
            (widget.foregroundColor ?? appBarTheme.foregroundColor) ?? defaults.foregroundColor!;
        double elevationLocal =
            (widget.elevation ?? appBarTheme.elevation)
            ?? (
                defaults.elevation
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        double effectiveElevation = states.Contains(WidgetState.scrolledUnder)
            ? (
                (
                    (widget.scrolledUnderElevation ?? appBarTheme.scrolledUnderElevation)
                    ?? defaults.scrolledUnderElevation
                ) ?? elevationLocal
            )
            : elevationLocal;
        IconThemeData overallIconTheme =
            (widget.iconTheme ?? appBarTheme.iconTheme)
            ?? defaults.iconTheme!.copyWith(color: foregroundColorLocal);
        Color? actionForegroundColor = widget.foregroundColor ?? appBarTheme.foregroundColor;
        IconThemeData actionsIconThemeLocal =
            (
                (
                    ((widget.actionsIconTheme ?? appBarTheme.actionsIconTheme) ?? widget.iconTheme)
                    ?? appBarTheme.iconTheme
                ) ?? (defaults.actionsIconTheme?.copyWith(color: actionForegroundColor))
            ) ?? overallIconTheme;
        EdgeInsetsGeometry actionsPaddingLocal =
            (widget.actionsPadding ?? appBarTheme.actionsPadding) ?? defaults.actionsPadding!;
        TextStyle? toolbarTextStyleLocal =
            (widget.toolbarTextStyle ?? appBarTheme.toolbarTextStyle)
            ?? (defaults.toolbarTextStyle?.copyWith(color: foregroundColorLocal));
        TextStyle? titleTextStyleLocal =
            (widget.titleTextStyle ?? appBarTheme.titleTextStyle)
            ?? (defaults.titleTextStyle?.copyWith(color: foregroundColorLocal));
        if (widget.toolbarOpacity != 1.0)
        {
            double opacityLocal = new Interval(0.25, 1.0, curve: Curves.fastOutSlowIn).transform(
                widget.toolbarOpacity
            );
            if (titleTextStyleLocal?.color is not null)
            {
                titleTextStyleLocal = titleTextStyleLocal!.copyWith(
                    color: titleTextStyleLocal.color!.withOpacity(opacityLocal)
                );
            }
            if (toolbarTextStyleLocal?.color is not null)
            {
                toolbarTextStyleLocal = toolbarTextStyleLocal!.copyWith(
                    color: toolbarTextStyleLocal.color!.withOpacity(opacityLocal)
                );
            }
            overallIconTheme = overallIconTheme.copyWith(
                opacity: opacityLocal * (overallIconTheme.opacity ?? 1.0)
            );
            actionsIconThemeLocal = actionsIconThemeLocal.copyWith(
                opacity: opacityLocal * (actionsIconThemeLocal.opacity ?? 1.0)
            );
        }
        Widget? leadingLocal = widget.leading;
        if ((leadingLocal is null) && widget.automaticallyImplyLeading)
        {
            if (hasDrawerLocal)
            {
                leadingLocal = DartRuntimePrimitives.ConvertValue<Widget>(
                    new DrawerButton(
                        style: IconButton.styleFrom(iconSize: overallIconTheme.size ?? 24)
                    )
                );
            }
            else
            {
                if ((parentRoute?.impliesAppBarDismissal) ?? false)
                {
                    leadingLocal = DartRuntimePrimitives.ConvertValue<Widget>(
                        useCloseButton ? new CloseButton() : new BackButton()
                    );
                }
            }
        }
        if (leadingLocal is not null)
        {
            {
                IconButtonThemeData effectiveIconButtonTheme = default!;
                if (Equals(overallIconTheme, defaults.iconTheme))
                {
                    effectiveIconButtonTheme = iconButtonTheme;
                }
                else
                {
                    ButtonStyle leadingIconButtonStyle = IconButton.styleFrom(
                        foregroundColor: overallIconTheme.color,
                        iconSize: overallIconTheme.size
                    );
                    effectiveIconButtonTheme = new IconButtonThemeData(
                        style: iconButtonTheme.style?.copyWith(
                            foregroundColor: leadingIconButtonStyle.foregroundColor,
                            overlayColor: leadingIconButtonStyle.overlayColor,
                            iconSize: leadingIconButtonStyle.iconSize
                        )
                    );
                }
                leadingLocal = DartRuntimePrimitives.ConvertValue<Widget>(
                    new IconButtonTheme(
                        data: effectiveIconButtonTheme,
                        child: (leadingLocal is IconButton)
                            ? new Center(child: (IconButton)leadingLocal)
                            : leadingLocal
                    )
                );
                leadingLocal = DartRuntimePrimitives.ConvertValue<Widget>(
                    new ConstrainedBox(
                        constraints: BoxConstraints.CreateTightFor(
                            width: (widget.leadingWidth ?? appBarTheme.leadingWidth)
                                ?? App_barLibrary._kLeadingWidth
                        ),
                        child: leadingLocal
                    )
                );
            }
        }
        Widget? titleLocal = widget.title;
        if (titleLocal is not null)
        {
            titleLocal = DartRuntimePrimitives.ConvertValue<Widget>(
                new _AppBarTitleBox__app_bar(child: titleLocal)
            );
            if (!widget.excludeHeaderSemantics)
            {
                titleLocal = DartRuntimePrimitives.ConvertValue<Widget>(
                    new Widgets.Semantics(
                        namesRoute: PlatformLibrary.defaultTargetPlatform switch
                        {
                            TargetPlatform.android
                            or TargetPlatform.fuchsia
                            or TargetPlatform.linux => true,
                            TargetPlatform.windows => true,
                            TargetPlatform.iOS => DartRuntimePrimitives.ConvertValue<bool>(null),
                            TargetPlatform.macOS => DartRuntimePrimitives.ConvertValue<bool>(null),
                            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                                throw new InvalidOperationException(
                                    "Non-exhaustive Dart switch value."
                                ),
                        },
                        header: true,
                        child: titleLocal
                    )
                );
            }
            titleLocal = DartRuntimePrimitives.ConvertValue<Widget>(
                new DefaultTextStyle(
                    style: titleTextStyleLocal!,
                    softWrap: false,
                    overflow: TextOverflow.ellipsis,
                    child: titleLocal
                )
            );
            titleLocal = MediaQuery.withClampedTextScaling(
                maxScaleFactor: App_barLibrary._kMaxTitleTextScaleFactor,
                child: titleLocal
            );
        }
        Widget? actionsLocal = default!;
        if ((widget.actions is not null) && Enumerable.Any(widget.actions!))
        {
            actionsLocal = DartRuntimePrimitives.ConvertValue<Widget>(
                new Padding(
                    padding: actionsPaddingLocal,
                    child: new Row(
                        mainAxisSize: MainAxisSize.min,
                        crossAxisAlignment: CrossAxisAlignment.center,
                        children: widget.actions!
                    )
                )
            );
        }
        else
        {
            if (hasEndDrawerLocal && widget.automaticallyImplyActions)
            {
                actionsLocal = DartRuntimePrimitives.ConvertValue<Widget>(
                    new EndDrawerButton(
                        style: IconButton.styleFrom(iconSize: overallIconTheme.size ?? 24)
                    )
                );
            }
        }
        if (actionsLocal is not null)
        {
            IconButtonThemeData effectiveActionsIconButtonTheme = default!;
            if (Equals(actionsIconThemeLocal, defaults.actionsIconTheme))
            {
                effectiveActionsIconButtonTheme = iconButtonTheme;
            }
            else
            {
                ButtonStyle actionsIconButtonStyle = IconButton.styleFrom(
                    foregroundColor: actionsIconThemeLocal.color,
                    iconSize: actionsIconThemeLocal.size
                );
                effectiveActionsIconButtonTheme = new IconButtonThemeData(
                    style: iconButtonTheme.style?.copyWith(
                        foregroundColor: actionsIconButtonStyle.foregroundColor,
                        overlayColor: actionsIconButtonStyle.overlayColor,
                        iconSize: actionsIconButtonStyle.iconSize
                    )
                );
            }
            actionsLocal = DartRuntimePrimitives.ConvertValue<Widget>(
                new IconButtonTheme(
                    data: effectiveActionsIconButtonTheme,
                    child: IconTheme.merge(data: actionsIconThemeLocal, child: actionsLocal)
                )
            );
        }
        Widget toolbar = new NavigationToolbar(
            leading: leadingLocal,
            middle: titleLocal,
            trailing: actionsLocal,
            centerMiddle: widget._getEffectiveCenterTitle(theme, appBarTheme),
            middleSpacing: (widget.titleSpacing ?? appBarTheme.titleSpacing)
                ?? NavigationToolbar.kMiddleSpacing
        );
        Widget appBar = new ClipRect(
            clipBehavior: widget.clipBehavior ?? Clip.hardEdge,
            child: new CustomSingleChildLayout(
                @delegate: new _ToolbarContainerLayout__app_bar(toolbarHeightLocal),
                child: IconTheme.merge(
                    data: overallIconTheme,
                    child: new DefaultTextStyle(style: toolbarTextStyleLocal!, child: toolbar)
                )
            )
        );
        if (widget.bottom is not null)
        {
            appBar = DartRuntimePrimitives.ConvertValue<Widget>(
                new Column(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: (
                        (Func<List<Widget>>)(
                            () =>
                            {
                                var __collection46394 = new List<Widget>();
                                __collection46394.Add(
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        new Flexible(
                                            child: new ConstrainedBox(
                                                constraints: new BoxConstraints(
                                                    maxHeight: toolbarHeightLocal
                                                ),
                                                child: appBar
                                            )
                                        )
                                    )
                                );
                                if (widget.bottomOpacity == 1.0)
                                {
                                    __collection46394.Add(
                                        DartRuntimePrimitives.ConvertValue<Widget>(widget.bottom!)
                                    );
                                }
                                else
                                {
                                    __collection46394.Add(
                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                            new Opacity(
                                                opacity: new Interval(
                                                    0.25,
                                                    1.0,
                                                    curve: Curves.fastOutSlowIn
                                                ).transform(widget.bottomOpacity),
                                                child: DartRuntimePrimitives.ConvertValue<Widget>(
                                                    widget.bottom
                                                )
                                            )
                                        )
                                    );
                                }
                                return __collection46394;
                            }
                        )
                    )()
                )
            );
        }
        if (widget.primary)
        {
            appBar = DartRuntimePrimitives.ConvertValue<Widget>(
                new SafeArea(bottom: false, child: appBar)
            );
        }
        appBar = DartRuntimePrimitives.ConvertValue<Widget>(
            new Align(alignment: Alignment.topCenter, child: appBar)
        );
        if (widget.flexibleSpace is not null)
        {
            appBar = DartRuntimePrimitives.ConvertValue<Widget>(
                new Stack(
                    fit: StackFit.passthrough,
                    children: new List<Widget>
                    {
                        DartRuntimePrimitives.ConvertValue<Widget>(
                            new Widgets.Semantics(
                                sortKey: widget.useDefaultSemanticsOrder
                                    ? new OrdinalSortKey(1.0)
                                    : null,
                                explicitChildNodes: true,
                                child: widget.flexibleSpace
                            )
                        ),
                        DartRuntimePrimitives.ConvertValue<Widget>(
                            new Widgets.Semantics(
                                sortKey: widget.useDefaultSemanticsOrder
                                    ? new OrdinalSortKey(0.0)
                                    : null,
                                explicitChildNodes: true,
                                child: new Material(type: MaterialType.transparency, child: appBar)
                            )
                        ),
                    }
                )
            );
        }
        SystemUiOverlayStyle overlayStyle =
            (
                (widget.systemOverlayStyle ?? appBarTheme.systemOverlayStyle)
                ?? defaults.systemOverlayStyle
            )
            ?? _systemOverlayStyleForBrightness(
                ThemeData.estimateBrightnessForColor(effectiveBackgroundColor),
                new Color(0L)
            );
        return new Widgets.Semantics(
            container: true,
            child: new AnnotatedRegion<SystemUiOverlayStyle>(
                value: overlayStyle,
                child: new Material(
                    color: effectiveBackgroundColor,
                    elevation: effectiveElevation,
                    type: widget.forceMaterialTransparency
                        ? MaterialType.transparency
                        : MaterialType.canvas,
                    shadowColor: (widget.shadowColor ?? appBarTheme.shadowColor)
                        ?? defaults.shadowColor,
                    surfaceTintColor: (widget.surfaceTintColor ?? appBarTheme.surfaceTintColor)
                        ?? theme.colorScheme.surfaceTint,
                    shape: (widget.shape ?? appBarTheme.shape) ?? defaults.shape,
                    animateColor: widget.animateColor,
                    child: new Widgets.Semantics(explicitChildNodes: true, child: appBar)
                )
            )
        );
    }
}

internal class _SliverAppBarDelegate__app_bar : SliverPersistentHeaderDelegate
{
    public virtual Widget? leading { get; private set; }
    public virtual bool automaticallyImplyLeading { get; private set; } = default!;
    public virtual Widget? title { get; private set; }
    public virtual List<Widget>? actions { get; private set; }
    public virtual bool automaticallyImplyActions { get; private set; } = default!;
    public virtual Widget? flexibleSpace { get; private set; }
    public virtual PreferredSizeWidget? bottom { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual double? scrolledUnderElevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual bool forceElevated { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? foregroundColor { get; private set; }
    public virtual IconThemeData? iconTheme { get; private set; }
    public virtual IconThemeData? actionsIconTheme { get; private set; }
    public virtual bool primary { get; private set; } = default!;
    public virtual bool? centerTitle { get; private set; }
    public virtual bool excludeHeaderSemantics { get; private set; } = default!;
    public virtual double? titleSpacing { get; private set; }
    public virtual double? expandedHeight { get; private set; }
    public virtual double collapsedHeight { get; private set; } = default!;
    public virtual double topPadding { get; private set; } = default!;
    public virtual bool floating { get; private set; } = default!;
    public virtual bool pinned { get; private set; } = default!;
    public virtual ShapeBorder? shape { get; private set; }
    public virtual double? toolbarHeight { get; private set; }
    public virtual double? leadingWidth { get; private set; }
    public virtual TextStyle? toolbarTextStyle { get; private set; }
    public virtual TextStyle? titleTextStyle { get; private set; }
    public virtual SystemUiOverlayStyle? systemOverlayStyle { get; private set; }
    internal virtual double _bottomHeight { get; private set; } = default!;
    public virtual bool forceMaterialTransparency { get; private set; } = default!;
    public virtual bool useDefaultSemanticsOrder { get; private set; } = default!;
    public virtual Clip? clipBehavior { get; private set; }
    public virtual _SliverAppVariant__app_bar variant { get; private set; } = default!;
    public virtual bool accessibleNavigation { get; private set; } = default!;
    public virtual EdgeInsetsGeometry? actionsPadding { get; private set; }
    private Scheduler.TickerProvider? __field_vsync = default!;
    public override Scheduler.TickerProvider? vsync
    {
        get => __field_vsync;
    }
    private FloatingHeaderSnapConfiguration? __field_snapConfiguration = default!;
    public override FloatingHeaderSnapConfiguration? snapConfiguration
    {
        get => __field_snapConfiguration;
    }
    private OverScrollHeaderStretchConfiguration? __field_stretchConfiguration = default!;
    public override OverScrollHeaderStretchConfiguration? stretchConfiguration
    {
        get => __field_stretchConfiguration;
    }
    private PersistentHeaderShowOnScreenConfiguration? __field_showOnScreenConfiguration = default!;
    public override PersistentHeaderShowOnScreenConfiguration? showOnScreenConfiguration
    {
        get => __field_showOnScreenConfiguration;
    }

    internal _SliverAppBarDelegate__app_bar(
        Widget? leading,
        bool automaticallyImplyLeading,
        Widget? title,
        List<Widget>? actions,
        bool automaticallyImplyActions,
        Widget? flexibleSpace,
        PreferredSizeWidget? bottom,
        double? elevation,
        double? scrolledUnderElevation,
        Color? shadowColor,
        Color? surfaceTintColor,
        bool forceElevated,
        Color? backgroundColor,
        Color? foregroundColor,
        IconThemeData? iconTheme,
        IconThemeData? actionsIconTheme,
        bool primary,
        bool? centerTitle,
        bool excludeHeaderSemantics,
        double? titleSpacing,
        double? expandedHeight,
        double collapsedHeight,
        double topPadding,
        bool floating,
        bool pinned,
        Scheduler.TickerProvider vsync,
        FloatingHeaderSnapConfiguration? snapConfiguration,
        OverScrollHeaderStretchConfiguration? stretchConfiguration,
        PersistentHeaderShowOnScreenConfiguration? showOnScreenConfiguration,
        ShapeBorder? shape,
        double? toolbarHeight,
        double? leadingWidth,
        TextStyle? toolbarTextStyle,
        TextStyle? titleTextStyle,
        SystemUiOverlayStyle? systemOverlayStyle,
        bool forceMaterialTransparency,
        bool useDefaultSemanticsOrder,
        Clip? clipBehavior,
        _SliverAppVariant__app_bar variant,
        bool accessibleNavigation,
        EdgeInsetsGeometry? actionsPadding
    )
    {
        this.leading = leading;
        this.automaticallyImplyLeading = automaticallyImplyLeading;
        this.title = title;
        this.actions = actions;
        this.automaticallyImplyActions = automaticallyImplyActions;
        this.flexibleSpace = flexibleSpace;
        this.bottom = bottom;
        this.elevation = elevation;
        this.scrolledUnderElevation = scrolledUnderElevation;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.forceElevated = forceElevated;
        this.backgroundColor = backgroundColor;
        this.foregroundColor = foregroundColor;
        this.iconTheme = iconTheme;
        this.actionsIconTheme = actionsIconTheme;
        this.primary = primary;
        this.centerTitle = centerTitle;
        this.excludeHeaderSemantics = excludeHeaderSemantics;
        this.titleSpacing = titleSpacing;
        this.expandedHeight = expandedHeight;
        this.collapsedHeight = collapsedHeight;
        this.topPadding = topPadding;
        this.floating = floating;
        this.pinned = pinned;
        __field_vsync = vsync;
        __field_snapConfiguration = snapConfiguration;
        __field_stretchConfiguration = stretchConfiguration;
        __field_showOnScreenConfiguration = showOnScreenConfiguration;
        this.shape = shape;
        this.toolbarHeight = toolbarHeight;
        this.leadingWidth = leadingWidth;
        this.toolbarTextStyle = toolbarTextStyle;
        this.titleTextStyle = titleTextStyle;
        this.systemOverlayStyle = systemOverlayStyle;
        this.forceMaterialTransparency = forceMaterialTransparency;
        this.useDefaultSemanticsOrder = useDefaultSemanticsOrder;
        this.clipBehavior = clipBehavior;
        this.variant = variant;
        this.accessibleNavigation = accessibleNavigation;
        this.actionsPadding = actionsPadding;
        _bottomHeight = bottom?.preferredSize.height ?? 0.0;
        System.Diagnostics.Debug.Assert(primary || (topPadding == 0.0));
    }

    public override double minExtent => collapsedHeight;
    public override double maxExtent =>
        Math.Max(
            topPadding
                + (
                    expandedHeight
                    ?? ((toolbarHeight ?? ConstantsLibrary.kToolbarHeight) + _bottomHeight)
                ),
            minExtent
        );

    public override Widget build(BuildContext context, double shrinkOffset, bool overlapsContent)
    {
        double visibleMainHeight = maxExtent - shrinkOffset - topPadding;
        double extraToolbarHeight = Math.Max(
            minExtent
                - _bottomHeight
                - topPadding
                - (toolbarHeight ?? ConstantsLibrary.kToolbarHeight),
            0.0
        );
        double visibleToolbarHeight = visibleMainHeight - _bottomHeight - extraToolbarHeight;
        bool isScrolledUnderLocal =
            overlapsContent
            || forceElevated
            || (pinned && (shrinkOffset > (maxExtent - minExtent)));
        bool isPinnedWithOpacityFade =
            pinned && floating && (bottom is not null) && (extraToolbarHeight == 0.0);
        double toolbarOpacityLocal =
            (!accessibleNavigation && (!pinned || isPinnedWithOpacityFade))
                ? Dart_uiLibrary.clampDouble(
                    visibleToolbarHeight / (toolbarHeight ?? ConstantsLibrary.kToolbarHeight),
                    0.0,
                    1.0
                )
                : 1.0;
        Widget? effectiveTitle = variant switch
        {
            _SliverAppVariant__app_bar.small => title,
            _SliverAppVariant__app_bar.medium => DartRuntimePrimitives.ConvertValue<Widget>(
                new AnimatedOpacity(
                    opacity: isScrolledUnderLocal ? 1 : 0,
                    duration: Duration.Create(milliseconds: 500L),
                    curve: new Cubic(0.2, 0.0, 0.0, 1.0),
                    child: title
                )
            ),
            _SliverAppVariant__app_bar.large => DartRuntimePrimitives.ConvertValue<Widget>(
                new AnimatedOpacity(
                    opacity: isScrolledUnderLocal ? 1 : 0,
                    duration: Duration.Create(milliseconds: 500L),
                    curve: new Cubic(0.2, 0.0, 0.0, 1.0),
                    child: title
                )
            ),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        Widget appBar = FlexibleSpaceBar.createSettings(
            minExtent: minExtent,
            maxExtent: maxExtent,
            currentExtent: Math.Max(minExtent, maxExtent - shrinkOffset),
            toolbarOpacity: toolbarOpacityLocal,
            isScrolledUnder: isScrolledUnderLocal,
            hasLeading: (leading is not null) || automaticallyImplyLeading,
            child: new AppBar(
                clipBehavior: clipBehavior,
                leading: leading,
                automaticallyImplyLeading: automaticallyImplyLeading,
                title: effectiveTitle,
                actions: actions,
                automaticallyImplyActions: automaticallyImplyActions,
                flexibleSpace: (
                    (title is null) && (flexibleSpace is not null) && !excludeHeaderSemantics
                )
                    ? new Widgets.Semantics(header: true, child: flexibleSpace)
                    : flexibleSpace,
                bottom: bottom,
                elevation: isScrolledUnderLocal ? elevation : 0.0,
                scrolledUnderElevation: scrolledUnderElevation,
                shadowColor: shadowColor,
                surfaceTintColor: surfaceTintColor,
                backgroundColor: backgroundColor,
                foregroundColor: foregroundColor,
                iconTheme: iconTheme,
                actionsIconTheme: actionsIconTheme,
                primary: primary,
                centerTitle: centerTitle,
                excludeHeaderSemantics: excludeHeaderSemantics,
                titleSpacing: titleSpacing,
                shape: shape,
                toolbarOpacity: toolbarOpacityLocal,
                bottomOpacity: pinned
                    ? 1.0
                    : Dart_uiLibrary.clampDouble(visibleMainHeight / _bottomHeight, 0.0, 1.0),
                toolbarHeight: toolbarHeight,
                leadingWidth: leadingWidth,
                toolbarTextStyle: toolbarTextStyle,
                titleTextStyle: titleTextStyle,
                systemOverlayStyle: systemOverlayStyle,
                forceMaterialTransparency: forceMaterialTransparency,
                useDefaultSemanticsOrder: useDefaultSemanticsOrder,
                actionsPadding: actionsPadding
            )
        );
        return appBar;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRebuild(SliverPersistentHeaderDelegate oldDelegate)
    {
        var __oldDelegate = (_SliverAppBarDelegate__app_bar)oldDelegate;
        return (!Equals(leading, __oldDelegate.leading))
            || (automaticallyImplyLeading != __oldDelegate.automaticallyImplyLeading)
            || (!Equals(title, __oldDelegate.title))
            || (!Equals(actions, __oldDelegate.actions))
            || (automaticallyImplyActions != __oldDelegate.automaticallyImplyActions)
            || (!Equals(flexibleSpace, __oldDelegate.flexibleSpace))
            || (!Equals(bottom, __oldDelegate.bottom))
            || (_bottomHeight != __oldDelegate._bottomHeight)
            || (elevation != __oldDelegate.elevation)
            || (!Equals(shadowColor, __oldDelegate.shadowColor))
            || (!Equals(backgroundColor, __oldDelegate.backgroundColor))
            || (!Equals(foregroundColor, __oldDelegate.foregroundColor))
            || (!Equals(iconTheme, __oldDelegate.iconTheme))
            || (!Equals(actionsIconTheme, __oldDelegate.actionsIconTheme))
            || (primary != __oldDelegate.primary)
            || (centerTitle != __oldDelegate.centerTitle)
            || (titleSpacing != __oldDelegate.titleSpacing)
            || (expandedHeight != __oldDelegate.expandedHeight)
            || (topPadding != __oldDelegate.topPadding)
            || (pinned != __oldDelegate.pinned)
            || (floating != __oldDelegate.floating)
            || (!Equals(vsync, __oldDelegate.vsync))
            || (!Equals(snapConfiguration, __oldDelegate.snapConfiguration))
            || (!Equals(stretchConfiguration, __oldDelegate.stretchConfiguration))
            || (!Equals(showOnScreenConfiguration, __oldDelegate.showOnScreenConfiguration))
            || (forceElevated != __oldDelegate.forceElevated)
            || (toolbarHeight != __oldDelegate.toolbarHeight)
            || (leadingWidth != __oldDelegate.leadingWidth)
            || (!Equals(toolbarTextStyle, __oldDelegate.toolbarTextStyle))
            || (!Equals(titleTextStyle, __oldDelegate.titleTextStyle))
            || (!Equals(systemOverlayStyle, __oldDelegate.systemOverlayStyle))
            || (forceMaterialTransparency != __oldDelegate.forceMaterialTransparency)
            || (useDefaultSemanticsOrder != __oldDelegate.useDefaultSemanticsOrder)
            || (accessibleNavigation != __oldDelegate.accessibleNavigation)
            || (!Equals(actionsPadding, __oldDelegate.actionsPadding));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{DiagnosticsLibrary.describeIdentity(this)}(topPadding: {topPadding.toStringAsFixed(1L)}, bottomHeight: {_bottomHeight.toStringAsFixed(1L)}, ...)";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class SliverAppBar : StatefulWidget
{
    public virtual Widget? leading { get; private set; }
    public virtual bool automaticallyImplyLeading { get; private set; } = default!;
    public virtual Widget? title { get; private set; }
    public virtual List<Widget>? actions { get; private set; }
    public virtual bool automaticallyImplyActions { get; private set; } = default!;
    public virtual Widget? flexibleSpace { get; private set; }
    public virtual PreferredSizeWidget? bottom { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual double? scrolledUnderElevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual bool forceElevated { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? foregroundColor { get; private set; }
    public virtual IconThemeData? iconTheme { get; private set; }
    public virtual IconThemeData? actionsIconTheme { get; private set; }
    public virtual bool primary { get; private set; } = default!;
    public virtual bool? centerTitle { get; private set; }
    public virtual bool excludeHeaderSemantics { get; private set; } = default!;
    public virtual double? titleSpacing { get; private set; }
    public virtual double? collapsedHeight { get; private set; }
    public virtual double? expandedHeight { get; private set; }
    public virtual bool floating { get; private set; } = default!;
    public virtual bool pinned { get; private set; } = default!;
    public virtual ShapeBorder? shape { get; private set; }
    public virtual bool snap { get; private set; } = default!;
    public virtual bool stretch { get; private set; } = default!;
    public virtual double stretchTriggerOffset { get; private set; } = default!;
    public virtual Func<Future>? onStretchTrigger { get; private set; }
    public virtual double toolbarHeight { get; private set; } = default!;
    public virtual double? leadingWidth { get; private set; }
    public virtual TextStyle? toolbarTextStyle { get; private set; }
    public virtual TextStyle? titleTextStyle { get; private set; }
    public virtual SystemUiOverlayStyle? systemOverlayStyle { get; private set; }
    public virtual bool forceMaterialTransparency { get; private set; } = default!;
    public virtual bool useDefaultSemanticsOrder { get; private set; } = default!;
    public virtual Clip? clipBehavior { get; private set; }
    public virtual EdgeInsetsGeometry? actionsPadding { get; private set; }
    internal virtual _SliverAppVariant__app_bar _variant { get; private set; } = default!;

    public SliverAppBar(
        Key? key = null,
        Widget? leading = null,
        bool automaticallyImplyLeading = true,
        Widget? title = null,
        List<Widget>? actions = null,
        bool automaticallyImplyActions = true,
        Widget? flexibleSpace = null,
        PreferredSizeWidget? bottom = null,
        double? elevation = null,
        double? scrolledUnderElevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        bool forceElevated = false,
        Color? backgroundColor = null,
        Color? foregroundColor = null,
        IconThemeData? iconTheme = null,
        IconThemeData? actionsIconTheme = null,
        bool primary = true,
        bool? centerTitle = null,
        bool excludeHeaderSemantics = false,
        double? titleSpacing = null,
        double? collapsedHeight = null,
        double? expandedHeight = null,
        bool floating = false,
        bool pinned = false,
        bool snap = false,
        bool stretch = false,
        double stretchTriggerOffset = 100.0,
        Func<Future>? onStretchTrigger = null,
        ShapeBorder? shape = null,
        double? toolbarHeight = null,
        double? leadingWidth = null,
        TextStyle? toolbarTextStyle = null,
        TextStyle? titleTextStyle = null,
        SystemUiOverlayStyle? systemOverlayStyle = null,
        bool forceMaterialTransparency = false,
        bool useDefaultSemanticsOrder = true,
        Clip? clipBehavior = null,
        EdgeInsetsGeometry? actionsPadding = null
    )
        : base(key: key)
    {
        double __toolbarHeight = toolbarHeight ?? ConstantsLibrary.kToolbarHeight;
        this.leading = leading;
        this.automaticallyImplyLeading = automaticallyImplyLeading;
        this.title = title;
        this.actions = actions;
        this.automaticallyImplyActions = automaticallyImplyActions;
        this.flexibleSpace = flexibleSpace;
        this.bottom = bottom;
        this.elevation = elevation;
        this.scrolledUnderElevation = scrolledUnderElevation;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.forceElevated = forceElevated;
        this.backgroundColor = backgroundColor;
        this.foregroundColor = foregroundColor;
        this.iconTheme = iconTheme;
        this.actionsIconTheme = actionsIconTheme;
        this.primary = primary;
        this.centerTitle = centerTitle;
        this.excludeHeaderSemantics = excludeHeaderSemantics;
        this.titleSpacing = titleSpacing;
        this.collapsedHeight = collapsedHeight;
        this.expandedHeight = expandedHeight;
        this.floating = floating;
        this.pinned = pinned;
        this.snap = snap;
        this.stretch = stretch;
        this.stretchTriggerOffset = stretchTriggerOffset;
        this.onStretchTrigger = onStretchTrigger;
        this.shape = shape;
        this.toolbarHeight = __toolbarHeight;
        this.leadingWidth = leadingWidth;
        this.toolbarTextStyle = toolbarTextStyle;
        this.titleTextStyle = titleTextStyle;
        this.systemOverlayStyle = systemOverlayStyle;
        this.forceMaterialTransparency = forceMaterialTransparency;
        this.useDefaultSemanticsOrder = useDefaultSemanticsOrder;
        this.clipBehavior = clipBehavior;
        this.actionsPadding = actionsPadding;
        _variant = _SliverAppVariant__app_bar.small;
        System.Diagnostics.Debug.Assert(floating || !snap);
        System.Diagnostics.Debug.Assert(stretchTriggerOffset > 0.0);
        System.Diagnostics.Debug.Assert(
            (collapsedHeight is null) || (collapsedHeight >= __toolbarHeight)
        );
    }

    public static SliverAppBar CreateMedium(
        Key? key = null,
        Widget? leading = null,
        bool automaticallyImplyLeading = true,
        Widget? title = null,
        List<Widget>? actions = null,
        bool automaticallyImplyActions = true,
        Widget? flexibleSpace = null,
        PreferredSizeWidget? bottom = null,
        double? elevation = null,
        double? scrolledUnderElevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        bool forceElevated = false,
        Color? backgroundColor = null,
        Color? foregroundColor = null,
        IconThemeData? iconTheme = null,
        IconThemeData? actionsIconTheme = null,
        bool primary = true,
        bool? centerTitle = null,
        bool excludeHeaderSemantics = false,
        double? titleSpacing = null,
        double? collapsedHeight = null,
        double? expandedHeight = null,
        bool floating = false,
        bool pinned = true,
        bool snap = false,
        bool stretch = false,
        double stretchTriggerOffset = 100.0,
        Func<Future>? onStretchTrigger = null,
        ShapeBorder? shape = null,
        double? toolbarHeight = null,
        double? leadingWidth = null,
        TextStyle? toolbarTextStyle = null,
        TextStyle? titleTextStyle = null,
        SystemUiOverlayStyle? systemOverlayStyle = null,
        bool forceMaterialTransparency = false,
        bool useDefaultSemanticsOrder = true,
        Clip? clipBehavior = null,
        EdgeInsetsGeometry? actionsPadding = null
    )
    {
        var __instance = new SliverAppBar(
            key: key,
            leading: leading,
            automaticallyImplyLeading: automaticallyImplyLeading,
            title: title,
            actions: actions,
            automaticallyImplyActions: automaticallyImplyActions,
            flexibleSpace: flexibleSpace,
            bottom: bottom,
            elevation: elevation,
            scrolledUnderElevation: scrolledUnderElevation,
            shadowColor: shadowColor,
            surfaceTintColor: surfaceTintColor,
            forceElevated: forceElevated,
            backgroundColor: backgroundColor,
            foregroundColor: foregroundColor,
            iconTheme: iconTheme,
            actionsIconTheme: actionsIconTheme,
            primary: primary,
            centerTitle: centerTitle,
            excludeHeaderSemantics: excludeHeaderSemantics,
            titleSpacing: titleSpacing,
            collapsedHeight: collapsedHeight,
            expandedHeight: expandedHeight,
            floating: floating,
            pinned: pinned,
            snap: snap,
            stretch: stretch,
            stretchTriggerOffset: stretchTriggerOffset,
            onStretchTrigger: onStretchTrigger,
            shape: shape,
            toolbarHeight: toolbarHeight,
            leadingWidth: leadingWidth,
            toolbarTextStyle: toolbarTextStyle,
            titleTextStyle: titleTextStyle,
            systemOverlayStyle: systemOverlayStyle,
            forceMaterialTransparency: forceMaterialTransparency,
            useDefaultSemanticsOrder: useDefaultSemanticsOrder,
            clipBehavior: clipBehavior,
            actionsPadding: actionsPadding
        );
        double __toolbarHeight =
            toolbarHeight ?? _MediumScrollUnderFlexibleConfig__app_bar.collapsedHeight;
        __instance.leading = leading;
        __instance.automaticallyImplyLeading = automaticallyImplyLeading;
        __instance.title = title;
        __instance.actions = actions;
        __instance.automaticallyImplyActions = automaticallyImplyActions;
        __instance.flexibleSpace = flexibleSpace;
        __instance.bottom = bottom;
        __instance.elevation = elevation;
        __instance.scrolledUnderElevation = scrolledUnderElevation;
        __instance.shadowColor = shadowColor;
        __instance.surfaceTintColor = surfaceTintColor;
        __instance.forceElevated = forceElevated;
        __instance.backgroundColor = backgroundColor;
        __instance.foregroundColor = foregroundColor;
        __instance.iconTheme = iconTheme;
        __instance.actionsIconTheme = actionsIconTheme;
        __instance.primary = primary;
        __instance.centerTitle = centerTitle;
        __instance.excludeHeaderSemantics = excludeHeaderSemantics;
        __instance.titleSpacing = titleSpacing;
        __instance.collapsedHeight = collapsedHeight;
        __instance.expandedHeight = expandedHeight;
        __instance.floating = floating;
        __instance.pinned = pinned;
        __instance.snap = snap;
        __instance.stretch = stretch;
        __instance.stretchTriggerOffset = stretchTriggerOffset;
        __instance.onStretchTrigger = onStretchTrigger;
        __instance.shape = shape;
        __instance.toolbarHeight = __toolbarHeight;
        __instance.leadingWidth = leadingWidth;
        __instance.toolbarTextStyle = toolbarTextStyle;
        __instance.titleTextStyle = titleTextStyle;
        __instance.systemOverlayStyle = systemOverlayStyle;
        __instance.forceMaterialTransparency = forceMaterialTransparency;
        __instance.useDefaultSemanticsOrder = useDefaultSemanticsOrder;
        __instance.clipBehavior = clipBehavior;
        __instance.actionsPadding = actionsPadding;
        __instance._variant = _SliverAppVariant__app_bar.medium;
        return __instance;
    }

    public static SliverAppBar CreateLarge(
        Key? key = null,
        Widget? leading = null,
        bool automaticallyImplyLeading = true,
        Widget? title = null,
        List<Widget>? actions = null,
        bool automaticallyImplyActions = true,
        Widget? flexibleSpace = null,
        PreferredSizeWidget? bottom = null,
        double? elevation = null,
        double? scrolledUnderElevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        bool forceElevated = false,
        Color? backgroundColor = null,
        Color? foregroundColor = null,
        IconThemeData? iconTheme = null,
        IconThemeData? actionsIconTheme = null,
        bool primary = true,
        bool? centerTitle = null,
        bool excludeHeaderSemantics = false,
        double? titleSpacing = null,
        double? collapsedHeight = null,
        double? expandedHeight = null,
        bool floating = false,
        bool pinned = true,
        bool snap = false,
        bool stretch = false,
        double stretchTriggerOffset = 100.0,
        Func<Future>? onStretchTrigger = null,
        ShapeBorder? shape = null,
        double? toolbarHeight = null,
        double? leadingWidth = null,
        TextStyle? toolbarTextStyle = null,
        TextStyle? titleTextStyle = null,
        SystemUiOverlayStyle? systemOverlayStyle = null,
        bool forceMaterialTransparency = false,
        bool useDefaultSemanticsOrder = true,
        Clip? clipBehavior = null,
        EdgeInsetsGeometry? actionsPadding = null
    )
    {
        var __instance = new SliverAppBar(
            key: key,
            leading: leading,
            automaticallyImplyLeading: automaticallyImplyLeading,
            title: title,
            actions: actions,
            automaticallyImplyActions: automaticallyImplyActions,
            flexibleSpace: flexibleSpace,
            bottom: bottom,
            elevation: elevation,
            scrolledUnderElevation: scrolledUnderElevation,
            shadowColor: shadowColor,
            surfaceTintColor: surfaceTintColor,
            forceElevated: forceElevated,
            backgroundColor: backgroundColor,
            foregroundColor: foregroundColor,
            iconTheme: iconTheme,
            actionsIconTheme: actionsIconTheme,
            primary: primary,
            centerTitle: centerTitle,
            excludeHeaderSemantics: excludeHeaderSemantics,
            titleSpacing: titleSpacing,
            collapsedHeight: collapsedHeight,
            expandedHeight: expandedHeight,
            floating: floating,
            pinned: pinned,
            snap: snap,
            stretch: stretch,
            stretchTriggerOffset: stretchTriggerOffset,
            onStretchTrigger: onStretchTrigger,
            shape: shape,
            toolbarHeight: toolbarHeight,
            leadingWidth: leadingWidth,
            toolbarTextStyle: toolbarTextStyle,
            titleTextStyle: titleTextStyle,
            systemOverlayStyle: systemOverlayStyle,
            forceMaterialTransparency: forceMaterialTransparency,
            useDefaultSemanticsOrder: useDefaultSemanticsOrder,
            clipBehavior: clipBehavior,
            actionsPadding: actionsPadding
        );
        double __toolbarHeight =
            toolbarHeight ?? _LargeScrollUnderFlexibleConfig__app_bar.collapsedHeight;
        __instance.leading = leading;
        __instance.automaticallyImplyLeading = automaticallyImplyLeading;
        __instance.title = title;
        __instance.actions = actions;
        __instance.automaticallyImplyActions = automaticallyImplyActions;
        __instance.flexibleSpace = flexibleSpace;
        __instance.bottom = bottom;
        __instance.elevation = elevation;
        __instance.scrolledUnderElevation = scrolledUnderElevation;
        __instance.shadowColor = shadowColor;
        __instance.surfaceTintColor = surfaceTintColor;
        __instance.forceElevated = forceElevated;
        __instance.backgroundColor = backgroundColor;
        __instance.foregroundColor = foregroundColor;
        __instance.iconTheme = iconTheme;
        __instance.actionsIconTheme = actionsIconTheme;
        __instance.primary = primary;
        __instance.centerTitle = centerTitle;
        __instance.excludeHeaderSemantics = excludeHeaderSemantics;
        __instance.titleSpacing = titleSpacing;
        __instance.collapsedHeight = collapsedHeight;
        __instance.expandedHeight = expandedHeight;
        __instance.floating = floating;
        __instance.pinned = pinned;
        __instance.snap = snap;
        __instance.stretch = stretch;
        __instance.stretchTriggerOffset = stretchTriggerOffset;
        __instance.onStretchTrigger = onStretchTrigger;
        __instance.shape = shape;
        __instance.toolbarHeight = __toolbarHeight;
        __instance.leadingWidth = leadingWidth;
        __instance.toolbarTextStyle = toolbarTextStyle;
        __instance.titleTextStyle = titleTextStyle;
        __instance.systemOverlayStyle = systemOverlayStyle;
        __instance.forceMaterialTransparency = forceMaterialTransparency;
        __instance.useDefaultSemanticsOrder = useDefaultSemanticsOrder;
        __instance.clipBehavior = clipBehavior;
        __instance.actionsPadding = actionsPadding;
        __instance._variant = _SliverAppVariant__app_bar.large;
        return __instance;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _SliverAppBarState__app_bar());
}

internal class _SliverAppBarState__app_bar
    : State<SliverAppBar>,
        TickerProviderStateMixin<SliverAppBar>
{
    internal virtual FloatingHeaderSnapConfiguration? _snapConfiguration { get; set; } = default;
    internal virtual OverScrollHeaderStretchConfiguration? _stretchConfiguration { get; set; } =
        default;
    internal virtual PersistentHeaderShowOnScreenConfiguration? _showOnScreenConfiguration { get; set; } =
        default;
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual void _updateSnapConfiguration()
    {
        if (widget.snap && widget.floating)
        {
            _snapConfiguration = new FloatingHeaderSnapConfiguration(
                curve: Curves.easeOut,
                duration: Duration.Create(milliseconds: 200L)
            );
        }
        else
        {
            _snapConfiguration = null;
        }
        _showOnScreenConfiguration =
            (widget.floating & widget.snap)
                ? new PersistentHeaderShowOnScreenConfiguration(
                    minShowOnScreenExtent: double.PositiveInfinity
                )
                : null;
    }

    internal virtual void _updateStretchConfiguration()
    {
        if (widget.stretch)
        {
            _stretchConfiguration = new OverScrollHeaderStretchConfiguration(
                stretchTriggerOffset: widget.stretchTriggerOffset,
                onStretchTrigger: widget.onStretchTrigger
            );
        }
        else
        {
            _stretchConfiguration = null;
        }
    }

    public override void initState()
    {
        base.initState();
        _updateSnapConfiguration();
        _updateStretchConfiguration();
    }

    public override void didUpdateWidget(SliverAppBar oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((widget.snap != oldWidget.snap) || (widget.floating != oldWidget.floating))
        {
            _updateSnapConfiguration();
        }
        if (widget.stretch != oldWidget.stretch)
        {
            _updateStretchConfiguration();
        }
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            !widget.primary || Widgets.DebugLibrary.debugCheckHasMediaQuery(context)
        );
        double bottomHeightLocal = widget.bottom?.preferredSize.height ?? 0.0;
        double topPaddingLocal = widget.primary ? MediaQuery.paddingOf(context).top : 0.0;
        double collapsedHeightLocal =
            (widget.pinned && widget.floating && (widget.bottom is not null))
                ? ((widget.collapsedHeight ?? 0.0) + bottomHeightLocal + topPaddingLocal)
                : (
                    (widget.collapsedHeight ?? widget.toolbarHeight)
                    + bottomHeightLocal
                    + topPaddingLocal
                );
        double? effectiveExpandedHeight = default!;
        double effectiveCollapsedHeight = default!;
        Widget? effectiveFlexibleSpace = default!;
        switch (widget._variant)
        {
            case _SliverAppVariant__app_bar.small:
            {
                effectiveExpandedHeight = widget.expandedHeight;
                effectiveCollapsedHeight = collapsedHeightLocal;
                effectiveFlexibleSpace = widget.flexibleSpace;
                break;
            }
            case _SliverAppVariant__app_bar.medium:
            {
                effectiveExpandedHeight =
                    widget.expandedHeight
                    ?? (
                        _MediumScrollUnderFlexibleConfig__app_bar.expandedHeight + bottomHeightLocal
                    );
                effectiveCollapsedHeight =
                    widget.collapsedHeight
                    ?? (
                        topPaddingLocal
                        + _MediumScrollUnderFlexibleConfig__app_bar.collapsedHeight
                        + bottomHeightLocal
                    );
                effectiveFlexibleSpace =
                    widget.flexibleSpace
                    ?? new _ScrollUnderFlexibleSpace__app_bar(
                        title: widget.title,
                        foregroundColor: widget.foregroundColor,
                        configBuilder: (Func<
                            BuildContext,
                            _MediumScrollUnderFlexibleConfig__app_bar
                        >)((arg0) => new _MediumScrollUnderFlexibleConfig__app_bar(arg0)),
                        titleTextStyle: widget.titleTextStyle,
                        bottomHeight: bottomHeightLocal
                    );
                break;
            }
            case _SliverAppVariant__app_bar.large:
            {
                effectiveExpandedHeight =
                    widget.expandedHeight
                    ?? (
                        _LargeScrollUnderFlexibleConfig__app_bar.expandedHeight + bottomHeightLocal
                    );
                effectiveCollapsedHeight =
                    widget.collapsedHeight
                    ?? (
                        topPaddingLocal
                        + _LargeScrollUnderFlexibleConfig__app_bar.collapsedHeight
                        + bottomHeightLocal
                    );
                effectiveFlexibleSpace =
                    widget.flexibleSpace
                    ?? new _ScrollUnderFlexibleSpace__app_bar(
                        title: widget.title,
                        foregroundColor: widget.foregroundColor,
                        configBuilder: (Func<
                            BuildContext,
                            _LargeScrollUnderFlexibleConfig__app_bar
                        >)((arg0) => new _LargeScrollUnderFlexibleConfig__app_bar(arg0)),
                        titleTextStyle: widget.titleTextStyle,
                        bottomHeight: bottomHeightLocal
                    );
                break;
            }
        }
        return MediaQuery.CreateRemovePadding(
            context: context,
            removeBottom: true,
            child: new SliverPersistentHeader(
                floating: widget.floating,
                pinned: widget.pinned,
                @delegate: new _SliverAppBarDelegate__app_bar(
                    vsync: this,
                    leading: widget.leading,
                    automaticallyImplyLeading: widget.automaticallyImplyLeading,
                    title: widget.title,
                    actions: widget.actions,
                    automaticallyImplyActions: widget.automaticallyImplyActions,
                    flexibleSpace: effectiveFlexibleSpace,
                    bottom: widget.bottom,
                    elevation: widget.elevation,
                    scrolledUnderElevation: widget.scrolledUnderElevation,
                    shadowColor: widget.shadowColor,
                    surfaceTintColor: widget.surfaceTintColor,
                    forceElevated: widget.forceElevated,
                    backgroundColor: widget.backgroundColor,
                    foregroundColor: widget.foregroundColor,
                    iconTheme: widget.iconTheme,
                    actionsIconTheme: widget.actionsIconTheme,
                    primary: widget.primary,
                    centerTitle: widget.centerTitle,
                    excludeHeaderSemantics: widget.excludeHeaderSemantics,
                    titleSpacing: widget.titleSpacing,
                    expandedHeight: effectiveExpandedHeight,
                    collapsedHeight: effectiveCollapsedHeight,
                    topPadding: topPaddingLocal,
                    floating: widget.floating,
                    pinned: widget.pinned,
                    shape: widget.shape,
                    snapConfiguration: _snapConfiguration,
                    stretchConfiguration: _stretchConfiguration,
                    showOnScreenConfiguration: _showOnScreenConfiguration,
                    toolbarHeight: widget.toolbarHeight,
                    leadingWidth: widget.leadingWidth,
                    toolbarTextStyle: widget.toolbarTextStyle,
                    titleTextStyle: widget.titleTextStyle,
                    systemOverlayStyle: widget.systemOverlayStyle,
                    forceMaterialTransparency: widget.forceMaterialTransparency,
                    useDefaultSemanticsOrder: widget.useDefaultSemanticsOrder,
                    clipBehavior: widget.clipBehavior,
                    variant: widget._variant,
                    accessibleNavigation: MediaQuery.of(context).accessibleNavigation,
                    actionsPadding: widget.actionsPadding
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = (
            (Func<_WidgetTicker__ticker_provider>)(
                () =>
                {
                    var __cascade = new _WidgetTicker__ticker_provider(
                        onTick,
                        this,
                        debugLabel: Foundation.ConstantsLibrary.kDebugMode
                            ? $"created by {DiagnosticsLibrary.describeIdentity(this)}"
                            : null
                    );
                    __cascade.muted = !values.enabled;
                    __cascade.forceFrames = values.forceFrames;
                    return __cascade;
                }
            )
        )();
        _tickers!.Add(result);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => _tickers is not null);
        DartRuntimePrimitives.Assert(() => _tickers!.Contains(ticker));
        _tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if (_tickers is not null)
        {
            TickerModeData values = _tickerModeNotifier!.value;
            bool mutedLocal = !values.enabled;
            foreach (Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (_tickers is not null)
            {
                foreach (Scheduler.Ticker ticker in _tickers!)
                {
                    if (ticker.isActive)
                    {
                        throw DartRuntimePrimitives.AsException(
                            new FlutterError(
                                new List<DiagnosticsNode>
                                {
                                    new ErrorSummary($"{this} was disposed with an active Ticker."),
                                    new ErrorDescription(
                                        $"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time "
                                            + "dispose() was called on the mixin, that Ticker was still active. All Tickers must "
                                            + "be disposed before calling super.dispose()."
                                    ),
                                    new ErrorHint(
                                        "Tickers used by AnimationControllers "
                                            + "should be disposed by calling dispose() on the AnimationController itself. "
                                            + "Otherwise, the ticker will leak."
                                    ),
                                    ticker.describeForError("The offending ticker was"),
                                }
                            )
                        );
                    }
                }
            }
            return true;
        });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new DiagnosticsProperty<HashSet<Scheduler.Ticker>>(
                "tickers",
                _tickers,
                description: (_tickers is not null)
                    ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}"
                    : null,
                defaultValue: default
            )
        );
    }
}

internal class _AppBarTitleBox__app_bar : SingleChildRenderObjectWidget
{
    internal _AppBarTitleBox__app_bar(Widget child)
        : base(child: child) { }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderAppBarTitleBox__app_bar(textDirection: Directionality.of(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderAppBarTitleBox__app_bar)renderObject;
        __renderObject.textDirection = Directionality.of(context);
    }
}

public class _RenderAppBarTitleBox__app_bar : RenderAligningShiftedBox
{
    internal _RenderAppBarTitleBox__app_bar(TextDirection? textDirection = null)
        : base(textDirection: textDirection, alignment: Alignment.center) { }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        BoxConstraints innerConstraints = constraints.copyWith(maxHeight: double.PositiveInfinity);
        Size childSize = child!.getDryLayout(innerConstraints);
        return constraints.constrain(childSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        BoxConstraints innerConstraints = constraints.copyWith(maxHeight: double.PositiveInfinity);
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return null;
        }
        double? result = childLocal.getDryBaseline(innerConstraints, baseline);
        if (result is null)
        {
            return null;
        }
        Size childSize = childLocal.getDryLayout(innerConstraints);
        return (
                result
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ) + resolvedAlignment.alongOffset(getDryLayout(constraints) - childSize).dy;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        BoxConstraints innerConstraints = constraints.copyWith(maxHeight: double.PositiveInfinity);
        child!.layout(innerConstraints, parentUsesSize: true);
        size = constraints.constrain(child!.size);
        alignChild();
    }
}

internal class _ScrollUnderFlexibleSpace__app_bar : StatelessWidget
{
    public virtual Widget? title { get; private set; }
    public virtual Color? foregroundColor { get; private set; }
    public virtual Func<BuildContext, _ScrollUnderFlexibleConfig__app_bar> configBuilder
    {
        get;
        private set;
    } = default!;
    public virtual TextStyle? titleTextStyle { get; private set; }
    public virtual double bottomHeight { get; private set; } = default!;

    internal _ScrollUnderFlexibleSpace__app_bar(
        Widget? title = null,
        Color? foregroundColor = null,
        Func<BuildContext, _ScrollUnderFlexibleConfig__app_bar> configBuilder = default!,
        TextStyle? titleTextStyle = null,
        double bottomHeight = default!
    )
    {
        this.title = title;
        this.foregroundColor = foregroundColor;
        this.configBuilder = configBuilder;
        this.titleTextStyle = titleTextStyle;
        this.bottomHeight = bottomHeight;
    }

    public override Widget build(BuildContext context)
    {
        AppBarThemeData appBarTheme = AppBarTheme.of(context);
        AppBarThemeData defaults = new _AppBarDefaultsM3__app_bar(context);
        FlexibleSpaceBarSettings settings =
            context.dependOnInheritedWidgetOfExactType<FlexibleSpaceBarSettings>()!;
        _ScrollUnderFlexibleConfig__app_bar config = configBuilder(context);
        DartRuntimePrimitives.Assert(
            () => config.expandedTitlePadding.isNonNegative,
            () =>
                (object?)
                    "The _ExpandedTitleWithPadding widget assumes that the expanded title padding is non-negative. "
                + "Update its implementation to handle negative padding."
        );
        TextStyle? expandedTextStyleLocal =
            (titleTextStyle ?? appBarTheme.titleTextStyle)
            ?? (
                config.expandedTextStyle?.copyWith(
                    color: (foregroundColor ?? appBarTheme.foregroundColor)
                        ?? defaults.foregroundColor
                )
            );
        Widget? expandedTitle = (title, expandedTextStyleLocal) switch
        {
            (null, _) => DartRuntimePrimitives.ConvertValue<Widget>(null),
            (Widget titleLocal, null) => titleLocal,
            (Widget titleAlternate, TextStyle textStyle) =>
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new DefaultTextStyle(style: textStyle, child: titleAlternate)
                ),
        };
        EdgeInsets resolvedTitlePadding = config.expandedTitlePadding.resolve(
            Directionality.of(context)
        );
        EdgeInsetsGeometry expandedTitlePaddingLocal =
            (bottomHeight > 0L) ? resolvedTitlePadding.copyWith(bottom: 0) : resolvedTitlePadding;
        return MediaQuery.withClampedTextScaling(
            maxScaleFactor: App_barLibrary._kMaxTitleTextScaleFactor,
            child: new Column(
                children: (
                    (Func<List<Widget>>)(
                        () =>
                        {
                            var __collection89388 = new List<Widget>();
                            __collection89388.Add(
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    new Padding(
                                        padding: EdgeInsets.CreateOnly(
                                            top: settings.minExtent - bottomHeight
                                        )
                                    )
                                )
                            );
                            __collection89388.Add(
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    new Flexible(
                                        child: new ClipRect(
                                            child: new _ExpandedTitleWithPadding__app_bar(
                                                padding: expandedTitlePaddingLocal,
                                                maxExtent: settings.maxExtent - settings.minExtent,
                                                child: expandedTitle
                                            )
                                        )
                                    )
                                )
                            );
                            if (bottomHeight > 0L)
                            {
                                __collection89388.Add(
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        new Padding(
                                            padding: EdgeInsets.CreateOnly(bottom: bottomHeight)
                                        )
                                    )
                                );
                            }
                            return __collection89388;
                        }
                    )
                )()
            )
        );
    }
}

internal class _ExpandedTitleWithPadding__app_bar : SingleChildRenderObjectWidget
{
    public virtual EdgeInsetsGeometry padding { get; private set; } = default!;
    public virtual double maxExtent { get; private set; } = default!;

    internal _ExpandedTitleWithPadding__app_bar(
        EdgeInsetsGeometry padding,
        double maxExtent,
        Widget? child = null
    )
        : base(child: child)
    {
        this.padding = padding;
        this.maxExtent = maxExtent;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        TextDirection textDirection = Directionality.of(context);
        return new _RenderExpandedTitleBox__app_bar(
            padding.resolve(textDirection),
            AlignmentDirectional.bottomStart.resolve(textDirection),
            maxExtent,
            null
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderExpandedTitleBox__app_bar)renderObject;
        TextDirection textDirection = Directionality.of(context);
        DartRuntimePrimitives.Ignore(
            (
                (Func<_RenderExpandedTitleBox__app_bar>)(
                    () =>
                    {
                        var __cascade = __renderObject;
                        __cascade.padding = padding.resolve(textDirection);
                        __cascade.titleAlignment = AlignmentDirectional.bottomStart.resolve(
                            textDirection
                        );
                        __cascade.maxExtent = maxExtent;
                        return __cascade;
                    }
                )
            )()
        );
    }
}

public class _RenderExpandedTitleBox__app_bar : RenderShiftedBox
{
    internal virtual EdgeInsets _padding { get; set; } = default!;
    internal virtual Alignment _titleAlignment { get; set; } = default!;
    internal virtual double _maxExtent { get; set; } = default!;

    internal _RenderExpandedTitleBox__app_bar(
        EdgeInsets _padding,
        Alignment _titleAlignment,
        double _maxExtent,
        RenderBox? child
    )
        : base(child)
    {
        this._padding = _padding;
        this._titleAlignment = _titleAlignment;
        this._maxExtent = _maxExtent;
    }

    public virtual EdgeInsets padding
    {
        get => _padding;
        set
        {
            var __value = value;
            if (Equals(_padding, __value))
            {
                return;
            }
            DartRuntimePrimitives.Assert(() => __value.isNonNegative);
            _padding = __value;
            markNeedsLayout();
        }
    }
    public virtual Alignment titleAlignment
    {
        get => _titleAlignment;
        set
        {
            var __value = value;
            if (Equals(_titleAlignment, __value))
            {
                return;
            }
            _titleAlignment = __value;
            markNeedsLayout();
        }
    }
    public virtual double maxExtent
    {
        get => _maxExtent;
        set
        {
            var __value = value;
            if (_maxExtent == __value)
            {
                return;
            }
            _maxExtent = __value;
            markNeedsLayout();
        }
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        RenderBox? childLocal = child;
        return (childLocal is null)
            ? 0.0
            : (
                childLocal.getMaxIntrinsicHeight(Math.Max(0, width - padding.horizontal))
                + padding.vertical
            );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        RenderBox? childLocal = child;
        return (childLocal is null)
            ? 0.0
            : (childLocal.getMaxIntrinsicWidth(double.PositiveInfinity) + padding.horizontal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        RenderBox? childLocal = child;
        return (childLocal is null)
            ? 0.0
            : (
                childLocal.getMinIntrinsicHeight(Math.Max(0, width - padding.horizontal))
                + padding.vertical
            );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        RenderBox? childLocal = child;
        return (childLocal is null)
            ? 0.0
            : (childLocal.getMinIntrinsicWidth(double.PositiveInfinity) + padding.horizontal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints) =>
        (child is null) ? Size.zero : constraints.biggest;

    internal virtual Offset _childOffsetFromSize(Size childSize, Size size)
    {
        DartRuntimePrimitives.Assert(() => child is not null);
        DartRuntimePrimitives.Assert(() => padding.isNonNegative);
        DartRuntimePrimitives.Assert(() => titleAlignment.y == 1.0);
        double yAdjustment = Dart_uiLibrary.clampDouble(
            childSize.height + padding.bottom - maxExtent,
            0,
            padding.bottom
        );
        double offsetX =
            ((titleAlignment.x + 1L) / 2L * (size.width - padding.horizontal - childSize.width))
            + padding.left;
        double offsetY = size.height - childSize.height - padding.bottom + yAdjustment;
        return new Offset(offsetX, offsetY);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return null;
        }
        BoxConstraints childConstraints = constraints.widthConstraints().deflate(padding);
        BaselineOffset result = new BaselineOffset(
            childLocal.getDryBaseline(childConstraints, baseline)
        ).op_Add(
            _childOffsetFromSize(
                childLocal.getDryLayout(childConstraints),
                getDryLayout(constraints)
            ).dy
        );
        return result.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            size = constraints.smallest;
            return;
        }
        size = constraints.biggest;
        childLocal.layout(constraints.widthConstraints().deflate(padding), parentUsesSize: true);
        var childParentData = ((BoxParentData?)childLocal.parentData!)!;
        childParentData.offset = _childOffsetFromSize(childLocal.size, size);
    }
}

internal interface _ScrollUnderFlexibleConfig__app_bar
{
    public TextStyle? collapsedTextStyle { get; }
    public TextStyle? expandedTextStyle { get; }
    public EdgeInsetsGeometry expandedTitlePadding { get; }
}

internal class _AppBarDefaultsM3__app_bar : AppBarThemeData
{
    public virtual BuildContext context { get; private set; } = default!;
    private bool __late__theme_initialized;
    private ThemeData __late__theme = default!;
    internal virtual ThemeData _theme
    {
        get
        {
            if (!__late__theme_initialized)
            {
                __late__theme = Theme.of(context);
                __late__theme_initialized = true;
            }
            return __late__theme;
        }
    }
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = _theme.colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }
    private bool __late__textTheme_initialized;
    private TextTheme __late__textTheme = default!;
    internal virtual TextTheme _textTheme
    {
        get
        {
            if (!__late__textTheme_initialized)
            {
                __late__textTheme = _theme.textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }

    internal _AppBarDefaultsM3__app_bar(BuildContext context)
        : base(
            elevation: 0.0,
            scrolledUnderElevation: 3.0,
            titleSpacing: NavigationToolbar.kMiddleSpacing,
            toolbarHeight: 64.0
        )
    {
        this.context = context;
    }

    public override Color? backgroundColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.surface);
    public override Color? foregroundColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.onSurface);
    public override Color? shadowColor =>
        DartRuntimePrimitives.ConvertValue<Color>(Colors.transparent);
    public override Color? surfaceTintColor =>
        DartRuntimePrimitives.ConvertValue<Color>(Colors.transparent);
    public override IconThemeData? iconTheme =>
        new IconThemeData(color: _colors.onSurface, size: 24.0);
    public override IconThemeData? actionsIconTheme =>
        new IconThemeData(color: _colors.onSurfaceVariant, size: 24.0);
    public override TextStyle? toolbarTextStyle => _textTheme.bodyMedium;
    public override TextStyle? titleTextStyle => _textTheme.titleLarge;
    public override EdgeInsets? actionsPadding => EdgeInsets.zero;
}

internal class _MediumScrollUnderFlexibleConfig__app_bar : _ScrollUnderFlexibleConfig__app_bar
{
    public virtual BuildContext context { get; private set; } = default!;
    private bool __late__theme_initialized;
    private ThemeData __late__theme = default!;
    internal virtual ThemeData _theme
    {
        get
        {
            if (!__late__theme_initialized)
            {
                __late__theme = Theme.of(context);
                __late__theme_initialized = true;
            }
            return __late__theme;
        }
    }
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = _theme.colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }
    private bool __late__textTheme_initialized;
    private TextTheme __late__textTheme = default!;
    internal virtual TextTheme _textTheme
    {
        get
        {
            if (!__late__textTheme_initialized)
            {
                __late__textTheme = _theme.textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }
    public const double collapsedHeight = 64.0;
    public const double expandedHeight = 112.0;

    internal _MediumScrollUnderFlexibleConfig__app_bar(BuildContext context)
    {
        this.context = context;
    }

    public virtual TextStyle? collapsedTextStyle =>
        _textTheme.titleLarge?.apply(color: _colors.onSurface);
    public virtual TextStyle? expandedTextStyle =>
        _textTheme.headlineSmall?.apply(color: _colors.onSurface);
    public virtual EdgeInsetsGeometry expandedTitlePadding =>
        DartRuntimePrimitives.ConvertValue<EdgeInsetsGeometry>(new EdgeInsets(16, 0, 16, 20));
}

internal class _LargeScrollUnderFlexibleConfig__app_bar : _ScrollUnderFlexibleConfig__app_bar
{
    public virtual BuildContext context { get; private set; } = default!;
    private bool __late__theme_initialized;
    private ThemeData __late__theme = default!;
    internal virtual ThemeData _theme
    {
        get
        {
            if (!__late__theme_initialized)
            {
                __late__theme = Theme.of(context);
                __late__theme_initialized = true;
            }
            return __late__theme;
        }
    }
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = _theme.colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }
    private bool __late__textTheme_initialized;
    private TextTheme __late__textTheme = default!;
    internal virtual TextTheme _textTheme
    {
        get
        {
            if (!__late__textTheme_initialized)
            {
                __late__textTheme = _theme.textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }
    public const double collapsedHeight = 64.0;
    public const double expandedHeight = 152.0;

    internal _LargeScrollUnderFlexibleConfig__app_bar(BuildContext context)
    {
        this.context = context;
    }

    public virtual TextStyle? collapsedTextStyle =>
        _textTheme.titleLarge?.apply(color: _colors.onSurface);
    public virtual TextStyle? expandedTextStyle =>
        _textTheme.headlineMedium?.apply(color: _colors.onSurface);
    public virtual EdgeInsetsGeometry expandedTitlePadding =>
        DartRuntimePrimitives.ConvertValue<EdgeInsetsGeometry>(new EdgeInsets(16, 0, 16, 28));
}
