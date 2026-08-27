// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/app_bar.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

internal delegate _ScrollUnderFlexibleConfig__app_bar _FlexibleConfigBuilder__app_bar(global::Doroti.Framework.Widgets.BuildContext __unused0);

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
    large
}

internal class _ToolbarContainerLayout__app_bar : global::Doroti.Framework.Rendering.SingleChildLayoutDelegate
{
    public virtual double toolbarHeight { get; private set; } = default!;

    internal _ToolbarContainerLayout__app_bar(double toolbarHeight)
    {
        this.toolbarHeight = toolbarHeight;
    }

    public override global::Doroti.Framework.Rendering.BoxConstraints getConstraintsForChild(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)constraints.tighten(height: this.toolbarHeight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size getSize(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return new global::Doroti.Ui.Size(((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth, this.toolbarHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Offset getPositionForChild(Size size, Size childSize)
    {
        return new global::Doroti.Ui.Offset(0.0, (size.height - childSize.height));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRelayout(global::Doroti.Framework.Rendering.SingleChildLayoutDelegate oldDelegate) => DartRuntimePrimitives.ConvertValue<bool>((this.toolbarHeight != ((_ToolbarContainerLayout__app_bar)oldDelegate).toolbarHeight));
}

internal class _PreferredAppBarSize__app_bar : Size
{
    public virtual double? toolbarHeight { get; private set; }
    public virtual double? bottomHeight { get; private set; }

    internal _PreferredAppBarSize__app_bar(double? toolbarHeight, double? bottomHeight) : base((((toolbarHeight ?? ConstantsLibrary.kToolbarHeight)) + ((bottomHeight ?? 0L))))
    {
        this.toolbarHeight = toolbarHeight;
        this.bottomHeight = bottomHeight;
    }

}

public class AppBar : global::Doroti.Framework.Widgets.StatefulWidget, global::Doroti.Framework.Widgets.PreferredSizeWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget? leading { get; private set; }
    public virtual bool automaticallyImplyLeading { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? title { get; private set; }
    public virtual List<global::Doroti.Framework.Widgets.Widget>? actions { get; private set; }
    public virtual bool automaticallyImplyActions { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? flexibleSpace { get; private set; }
    public virtual global::Doroti.Framework.Widgets.PreferredSizeWidget? bottom { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual double? scrolledUnderElevation { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Widgets.ScrollNotification, bool> notificationPredicate { get; private set; } = default!;
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? foregroundColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.IconThemeData? iconTheme { get; private set; }
    public virtual global::Doroti.Framework.Widgets.IconThemeData? actionsIconTheme { get; private set; }
    public virtual bool primary { get; private set; } = default!;
    public virtual bool? centerTitle { get; private set; }
    public virtual bool excludeHeaderSemantics { get; private set; } = default!;
    public virtual double? titleSpacing { get; private set; }
    public virtual double toolbarOpacity { get; private set; } = default!;
    public virtual double bottomOpacity { get; private set; } = default!;
    public virtual Size preferredSize { get; private set; } = default!;
    public virtual double? toolbarHeight { get; private set; }
    public virtual double? leadingWidth { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? toolbarTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? titleTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Services.SystemUiOverlayStyle? systemOverlayStyle { get; private set; }
    public virtual bool forceMaterialTransparency { get; private set; } = default!;
    public virtual bool useDefaultSemanticsOrder { get; private set; } = default!;
    public virtual Clip? clipBehavior { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? actionsPadding { get; private set; }
    public virtual bool animateColor { get; private set; } = default!;

    public AppBar(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget? leading = null, bool automaticallyImplyLeading = true, global::Doroti.Framework.Widgets.Widget? title = null, List<global::Doroti.Framework.Widgets.Widget>? actions = null, bool automaticallyImplyActions = true, global::Doroti.Framework.Widgets.Widget? flexibleSpace = null, global::Doroti.Framework.Widgets.PreferredSizeWidget? bottom = null, double? elevation = null, double? scrolledUnderElevation = null, global::System.Func<global::Doroti.Framework.Widgets.ScrollNotification, bool> notificationPredicate = default!, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, Color? backgroundColor = null, Color? foregroundColor = null, global::Doroti.Framework.Widgets.IconThemeData? iconTheme = null, global::Doroti.Framework.Widgets.IconThemeData? actionsIconTheme = null, bool primary = true, bool? centerTitle = null, bool excludeHeaderSemantics = false, double? titleSpacing = null, double toolbarOpacity = 1.0, double bottomOpacity = 1.0, double? toolbarHeight = null, double? leadingWidth = null, global::Doroti.Framework.Painting.TextStyle? toolbarTextStyle = null, global::Doroti.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Framework.Services.SystemUiOverlayStyle? systemOverlayStyle = null, bool forceMaterialTransparency = false, bool useDefaultSemanticsOrder = true, Clip? clipBehavior = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? actionsPadding = null, bool animateColor = false) : base(key: key)
    {
        global::System.Func<global::Doroti.Framework.Widgets.ScrollNotification, bool> __notificationPredicate = notificationPredicate ?? Scroll_notificationLibrary.defaultScrollNotificationPredicate;
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
        this.preferredSize = new _PreferredAppBarSize__app_bar(toolbarHeight, bottom?.preferredSize.height);
        System.Diagnostics.Debug.Assert(((elevation is null) || (elevation >= 0.0)));
    }

    public static double preferredHeightFor(global::Doroti.Framework.Widgets.BuildContext context, Size preferredSize)
    {
        if (((preferredSize is _PreferredAppBarSize__app_bar preferredAppBarSize) && (preferredAppBarSize.toolbarHeight is null)))
        {
            _PreferredAppBarSize__app_bar preferredSize__as9579 = (_PreferredAppBarSize__app_bar)preferredSize;
            return (((AppBarTheme.of(context).toolbarHeight ?? ConstantsLibrary.kToolbarHeight)) + ((preferredSize__as9579.bottomHeight ?? 0L)));
        }
        return preferredSize.height;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _getEffectiveCenterTitle(ThemeData theme, AppBarThemeData appbarTheme)
    {
        bool platformCenter()
        {
            return (theme.platform switch { global::Doroti.Framework.Foundation.TargetPlatform.iOS => ((this.actions is null) || (checked((long)(this.actions!.Count)) < 2L)), global::Doroti.Framework.Foundation.TargetPlatform.macOS => ((this.actions is null) || (checked((long)(this.actions!.Count)) < 2L)), global::Doroti.Framework.Foundation.TargetPlatform.android or global::Doroti.Framework.Foundation.TargetPlatform.fuchsia or global::Doroti.Framework.Foundation.TargetPlatform.linux => false, global::Doroti.Framework.Foundation.TargetPlatform.windows => false, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        return ((this.centerTitle ?? appbarTheme.centerTitle) ?? platformCenter());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _AppBarState__app_bar());
}

internal class _AppBarState__app_bar : global::Doroti.Framework.Widgets.State<AppBar>
{
    internal virtual global::Doroti.Framework.Widgets.ScrollNotificationObserverState? _scrollNotificationObserver { get; set; } = default;
    internal virtual bool _scrolledUnder { get; set; } = false;

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        this._scrollNotificationObserver?.removeListener((global::System.Action<global::Doroti.Framework.Widgets.ScrollNotification>)this._handleScrollNotification);
        ScaffoldState? scaffoldState = Scaffold.maybeOf(this.context);
        if (((scaffoldState is not null) && ((scaffoldState.isDrawerOpen || scaffoldState.isEndDrawerOpen))))
        {
            return;
        }
        _scrollNotificationObserver = ScrollNotificationObserver.maybeOf(this.context);
        this._scrollNotificationObserver?.addListener((global::System.Action<global::Doroti.Framework.Widgets.ScrollNotification>)this._handleScrollNotification);
    }

    public override void dispose()
    {
        if ((this._scrollNotificationObserver is not null))
        {
            this._scrollNotificationObserver!.removeListener((global::System.Action<global::Doroti.Framework.Widgets.ScrollNotification>)this._handleScrollNotification);
            _scrollNotificationObserver = null;
        }
        base.dispose();
    }

    internal virtual void _handleScrollNotification(global::Doroti.Framework.Widgets.ScrollNotification notification)
    {
        if (((notification is global::Doroti.Framework.Widgets.ScrollUpdateNotification) && this.widget.notificationPredicate(((global::Doroti.Framework.Widgets.ScrollUpdateNotification)notification))))
        {
            global::Doroti.Framework.Widgets.ScrollUpdateNotification notification__as34351 = (global::Doroti.Framework.Widgets.ScrollUpdateNotification)notification;
            bool oldScrolledUnder = this._scrolledUnder;
            global::Doroti.Framework.Widgets.ScrollMetrics metricsLocal = ((global::Doroti.Framework.Widgets.ScrollUpdateNotification)notification__as34351).metrics;
            switch (((global::Doroti.Framework.Widgets.ScrollMetrics)metricsLocal).axisDirection)
            {
                case global::Doroti.Framework.Painting.AxisDirection.up:
                    {
                        _scrolledUnder = (((global::Doroti.Framework.Widgets.ScrollMetrics)metricsLocal).extentAfter > 0L);
                        break;
                    }
                case global::Doroti.Framework.Painting.AxisDirection.down:
                    {
                        _scrolledUnder = (((global::Doroti.Framework.Widgets.ScrollMetrics)metricsLocal).extentBefore > 0L);
                        break;
                    }
                case global::Doroti.Framework.Painting.AxisDirection.right:
                case global::Doroti.Framework.Painting.AxisDirection.left:
                    {
                        break;
                    }
            }
            if ((this._scrolledUnder != oldScrolledUnder))
            {
                setState(((global::System.Action)(() =>
                {
                })));
            }
        }
    }

    internal virtual global::Doroti.Ui.Color _resolveColor(HashSet<global::Doroti.Framework.Widgets.WidgetState> states, Color? widgetColor, Color? themeColor, Color defaultColor)
    {
        return ((global::Doroti.Ui.Color)(object?)((((WidgetStateProperty.resolveAs<global::Doroti.Ui.Color?>(widgetColor, states) ?? (Color)WidgetStateProperty.resolveAs<global::Doroti.Ui.Color?>(themeColor, states))) ?? (Color)WidgetStateProperty.resolveAs<global::Doroti.Ui.Color>(defaultColor, states))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Services.SystemUiOverlayStyle _systemOverlayStyleForBrightness(Brightness brightness, Color? backgroundColor = null)
    {
        global::Doroti.Framework.Services.SystemUiOverlayStyle style = ((object.Equals(brightness, Brightness.dark)) ? global::Doroti.Framework.Services.SystemUiOverlayStyle.light : global::Doroti.Framework.Services.SystemUiOverlayStyle.dark);
        return new global::Doroti.Framework.Services.SystemUiOverlayStyle(statusBarColor: backgroundColor, statusBarBrightness: ((global::Doroti.Framework.Services.SystemUiOverlayStyle)style).statusBarBrightness, statusBarIconBrightness: ((global::Doroti.Framework.Services.SystemUiOverlayStyle)style).statusBarIconBrightness, systemStatusBarContrastEnforced: ((global::Doroti.Framework.Services.SystemUiOverlayStyle)style).systemStatusBarContrastEnforced);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => (!((AppBar)this.widget).primary || global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context)));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        ThemeData theme = Theme.of(context);
        IconButtonThemeData iconButtonTheme = IconButtonTheme.of(context);
        AppBarThemeData appBarTheme = AppBarTheme.of(context);
        AppBarThemeData defaults = (theme.useMaterial3 ? new _AppBarDefaultsM3__app_bar(context) : new _AppBarDefaultsM2__app_bar(context));
        ScaffoldState? scaffold = Scaffold.maybeOf(context);
        dynamic parentRoute = global::Doroti.Framework.Widgets.ModalRoute<object>.of<object>(context);
        FlexibleSpaceBarSettings? settings = ((FlexibleSpaceBarSettings?)(object?)context.dependOnInheritedWidgetOfExactType<FlexibleSpaceBarSettings>());
        var states = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection37017 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if ((settings?.isScrolledUnder ?? this._scrolledUnder)) { __collection37017.Add(global::Doroti.Framework.Widgets.WidgetState.scrolledUnder); } return __collection37017; }))();
        bool hasDrawerLocal = (scaffold?.hasDrawer ?? false);
        bool hasEndDrawerLocal = (scaffold?.hasEndDrawer ?? false);
        bool useCloseButton = (((bool?)((dynamic)parentRoute)?.fullscreenDialog) ?? false);
        double toolbarHeightLocal = ((((AppBar)this.widget).toolbarHeight ?? appBarTheme.toolbarHeight) ?? ConstantsLibrary.kToolbarHeight);
        global::Doroti.Ui.Color backgroundColorLocal = ((global::Doroti.Ui.Color)(object?)_resolveColor(states, ((AppBar)this.widget).backgroundColor, appBarTheme.backgroundColor, (defaults.backgroundColor ?? Theme.of(context).colorScheme.surface)));
        global::Doroti.Ui.Color scrolledUnderBackground = ((global::Doroti.Ui.Color)(object?)_resolveColor(states, ((AppBar)this.widget).backgroundColor, appBarTheme.backgroundColor, Theme.of(context).colorScheme.surfaceContainer));
        var effectiveBackgroundColor = (states.Contains(global::Doroti.Framework.Widgets.WidgetState.scrolledUnder) ? scrolledUnderBackground : backgroundColorLocal);
        global::Doroti.Ui.Color foregroundColorLocal = ((global::Doroti.Ui.Color)(object?)((((AppBar)this.widget).foregroundColor ?? appBarTheme.foregroundColor) ?? defaults.foregroundColor!));
        double elevationLocal = ((((AppBar)this.widget).elevation ?? appBarTheme.elevation) ?? DartRuntimePrimitives.RequireValue(defaults.elevation));
        double effectiveElevation = (states.Contains(global::Doroti.Framework.Widgets.WidgetState.scrolledUnder) ? (((((AppBar)this.widget).scrolledUnderElevation ?? appBarTheme.scrolledUnderElevation) ?? defaults.scrolledUnderElevation) ?? elevationLocal) : elevationLocal);
        global::Doroti.Framework.Widgets.IconThemeData overallIconTheme = (((((AppBar)this.widget).iconTheme ?? appBarTheme.iconTheme) ?? (global::Doroti.Framework.Widgets.IconThemeData)defaults.iconTheme!.copyWith(color: foregroundColorLocal)));
        global::Doroti.Ui.Color? actionForegroundColor = ((global::Doroti.Ui.Color?)(object?)(((AppBar)this.widget).foregroundColor ?? appBarTheme.foregroundColor));
        global::Doroti.Framework.Widgets.IconThemeData actionsIconThemeLocal = ((((((((AppBar)this.widget).actionsIconTheme ?? appBarTheme.actionsIconTheme) ?? ((AppBar)this.widget).iconTheme) ?? appBarTheme.iconTheme) ?? (global::Doroti.Framework.Widgets.IconThemeData)defaults.actionsIconTheme?.copyWith(color: actionForegroundColor))) ?? overallIconTheme);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry actionsPaddingLocal = ((((AppBar)this.widget).actionsPadding ?? appBarTheme.actionsPadding) ?? defaults.actionsPadding!);
        global::Doroti.Framework.Painting.TextStyle? toolbarTextStyleLocal = (((((AppBar)this.widget).toolbarTextStyle ?? appBarTheme.toolbarTextStyle) ?? (global::Doroti.Framework.Painting.TextStyle)defaults.toolbarTextStyle?.copyWith(color: foregroundColorLocal)));
        global::Doroti.Framework.Painting.TextStyle? titleTextStyleLocal = (((((AppBar)this.widget).titleTextStyle ?? appBarTheme.titleTextStyle) ?? (global::Doroti.Framework.Painting.TextStyle)defaults.titleTextStyle?.copyWith(color: foregroundColorLocal)));
        if ((((AppBar)this.widget).toolbarOpacity != 1.0))
        {
            double opacityLocal = new global::Doroti.Framework.Animation.Interval(0.25, 1.0, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn).transform(((AppBar)this.widget).toolbarOpacity);
            if ((titleTextStyleLocal?.color is not null))
            {
                titleTextStyleLocal = titleTextStyleLocal!.copyWith(color: ((global::Doroti.Framework.Painting.TextStyle)titleTextStyleLocal).color!.withOpacity(opacityLocal));
            }
            if ((toolbarTextStyleLocal?.color is not null))
            {
                toolbarTextStyleLocal = toolbarTextStyleLocal!.copyWith(color: ((global::Doroti.Framework.Painting.TextStyle)toolbarTextStyleLocal).color!.withOpacity(opacityLocal));
            }
            overallIconTheme = overallIconTheme.copyWith(opacity: (opacityLocal * ((((global::Doroti.Framework.Widgets.IconThemeData)overallIconTheme).opacity ?? 1.0))));
            actionsIconThemeLocal = actionsIconThemeLocal.copyWith(opacity: (opacityLocal * ((((global::Doroti.Framework.Widgets.IconThemeData)actionsIconThemeLocal).opacity ?? 1.0))));
        }
        global::Doroti.Framework.Widgets.Widget? leadingLocal = ((AppBar)this.widget).leading;
        if (((leadingLocal is null) && ((AppBar)this.widget).automaticallyImplyLeading))
        {
            if (hasDrawerLocal)
            {
                leadingLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new DrawerButton(style: IconButton.styleFrom(iconSize: (((global::Doroti.Framework.Widgets.IconThemeData)overallIconTheme).size ?? 24))));
            }
            else
            {
                if ((((bool?)((dynamic)parentRoute)?.impliesAppBarDismissal) ?? false))
                {
                    leadingLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>((useCloseButton ? new CloseButton() : new BackButton()));
                }
            }
        }
        if ((leadingLocal is not null))
        {
            if (theme.useMaterial3)
            {
                IconButtonThemeData effectiveIconButtonTheme = default!;
                if ((object.Equals(overallIconTheme, defaults.iconTheme)))
                {
                    effectiveIconButtonTheme = iconButtonTheme;
                }
                else
                {
                    ButtonStyle leadingIconButtonStyle = ((ButtonStyle)(object?)IconButton.styleFrom(foregroundColor: ((global::Doroti.Framework.Widgets.IconThemeData)overallIconTheme).color, iconSize: ((global::Doroti.Framework.Widgets.IconThemeData)overallIconTheme).size));
                    effectiveIconButtonTheme = new IconButtonThemeData(style: iconButtonTheme.style?.copyWith(foregroundColor: leadingIconButtonStyle.foregroundColor, overlayColor: leadingIconButtonStyle.overlayColor, iconSize: leadingIconButtonStyle.iconSize));
                }
                leadingLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new IconButtonTheme(data: effectiveIconButtonTheme, child: ((leadingLocal is IconButton) ? new global::Doroti.Framework.Widgets.Center(child: ((IconButton)leadingLocal)) : leadingLocal)));
                leadingLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: global::Doroti.Framework.Rendering.BoxConstraints.CreateTightFor(width: ((((AppBar)this.widget).leadingWidth ?? appBarTheme.leadingWidth) ?? App_barLibrary._kLeadingWidth)), child: leadingLocal));
            }
            else
            {
                leadingLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: global::Doroti.Framework.Rendering.BoxConstraints.CreateTightFor(width: ((((AppBar)this.widget).leadingWidth ?? appBarTheme.leadingWidth) ?? App_barLibrary._kLeadingWidth)), child: leadingLocal));
            }
        }
        global::Doroti.Framework.Widgets.Widget? titleLocal = ((AppBar)this.widget).title;
        if ((titleLocal is not null))
        {
            titleLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _AppBarTitleBox__app_bar(child: titleLocal));
            if (!((AppBar)this.widget).excludeHeaderSemantics)
            {
                titleLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(namesRoute: (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform switch { global::Doroti.Framework.Foundation.TargetPlatform.android or global::Doroti.Framework.Foundation.TargetPlatform.fuchsia or global::Doroti.Framework.Foundation.TargetPlatform.linux => true, global::Doroti.Framework.Foundation.TargetPlatform.windows => true, global::Doroti.Framework.Foundation.TargetPlatform.iOS => DartRuntimePrimitives.ConvertValue<bool>(null), global::Doroti.Framework.Foundation.TargetPlatform.macOS => DartRuntimePrimitives.ConvertValue<bool>(null), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }), header: true, child: titleLocal));
            }
            titleLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.DefaultTextStyle(style: titleTextStyleLocal!, softWrap: false, overflow: global::Doroti.Framework.Painting.TextOverflow.ellipsis, child: titleLocal));
            titleLocal = MediaQuery.withClampedTextScaling(maxScaleFactor: App_barLibrary._kMaxTitleTextScaleFactor, child: titleLocal);
        }
        global::Doroti.Framework.Widgets.Widget? actionsLocal = default!;
        if (((((AppBar)this.widget).actions is not null) && System.Linq.Enumerable.Any(((AppBar)this.widget).actions!)))
        {
            actionsLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: actionsPaddingLocal, child: new global::Doroti.Framework.Widgets.Row(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: (theme.useMaterial3 ? global::Doroti.Framework.Rendering.CrossAxisAlignment.center : global::Doroti.Framework.Rendering.CrossAxisAlignment.stretch), children: ((AppBar)this.widget).actions!)));
        }
        else
        {
            if ((hasEndDrawerLocal && ((AppBar)this.widget).automaticallyImplyActions))
            {
                actionsLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new EndDrawerButton(style: IconButton.styleFrom(iconSize: (((global::Doroti.Framework.Widgets.IconThemeData)overallIconTheme).size ?? 24))));
            }
        }
        if ((actionsLocal is not null))
        {
            IconButtonThemeData effectiveActionsIconButtonTheme = default!;
            if ((object.Equals(actionsIconThemeLocal, defaults.actionsIconTheme)))
            {
                effectiveActionsIconButtonTheme = iconButtonTheme;
            }
            else
            {
                ButtonStyle actionsIconButtonStyle = ((ButtonStyle)(object?)IconButton.styleFrom(foregroundColor: ((global::Doroti.Framework.Widgets.IconThemeData)actionsIconThemeLocal).color, iconSize: ((global::Doroti.Framework.Widgets.IconThemeData)actionsIconThemeLocal).size));
                effectiveActionsIconButtonTheme = new IconButtonThemeData(style: iconButtonTheme.style?.copyWith(foregroundColor: actionsIconButtonStyle.foregroundColor, overlayColor: actionsIconButtonStyle.overlayColor, iconSize: actionsIconButtonStyle.iconSize));
            }
            actionsLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new IconButtonTheme(data: effectiveActionsIconButtonTheme, child: IconTheme.merge(data: actionsIconThemeLocal, child: actionsLocal)));
        }
        global::Doroti.Framework.Widgets.Widget toolbar = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.NavigationToolbar(leading: leadingLocal, middle: titleLocal, trailing: actionsLocal, centerMiddle: this.widget._getEffectiveCenterTitle(theme, appBarTheme), middleSpacing: ((((AppBar)this.widget).titleSpacing ?? appBarTheme.titleSpacing) ?? global::Doroti.Framework.Widgets.NavigationToolbar.kMiddleSpacing)));
        global::Doroti.Framework.Widgets.Widget appBar = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ClipRect(clipBehavior: (((AppBar)this.widget).clipBehavior ?? Clip.hardEdge), child: new global::Doroti.Framework.Widgets.CustomSingleChildLayout(@delegate: new _ToolbarContainerLayout__app_bar(toolbarHeightLocal), child: IconTheme.merge(data: overallIconTheme, child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: toolbarTextStyleLocal!, child: toolbar)))));
        if ((((AppBar)this.widget).bottom is not null))
        {
            appBar = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Column(mainAxisAlignment: global::Doroti.Framework.Rendering.MainAxisAlignment.spaceBetween, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection46394 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection46394.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(maxHeight: toolbarHeightLocal), child: appBar)))); if ((((AppBar)this.widget).bottomOpacity == 1.0)) { __collection46394.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(((AppBar)this.widget).bottom!)); } else { __collection46394.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Opacity(opacity: new global::Doroti.Framework.Animation.Interval(0.25, 1.0, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn).transform(((AppBar)this.widget).bottomOpacity), child: DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(((AppBar)this.widget).bottom)))); } return __collection46394; }))()));
        }
        if (((AppBar)this.widget).primary)
        {
            appBar = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SafeArea(bottom: false, child: appBar));
        }
        appBar = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.Alignment.topCenter, child: appBar));
        if ((((AppBar)this.widget).flexibleSpace is not null))
        {
            appBar = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Stack(fit: global::Doroti.Framework.Rendering.StackFit.passthrough, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(sortKey: (((AppBar)this.widget).useDefaultSemanticsOrder ? new global::Doroti.Framework.Semantics.OrdinalSortKey(1.0) : null), explicitChildNodes: true, child: ((AppBar)this.widget).flexibleSpace)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(sortKey: (((AppBar)this.widget).useDefaultSemanticsOrder ? new global::Doroti.Framework.Semantics.OrdinalSortKey(0.0) : null), explicitChildNodes: true, child: new Material(type: MaterialType.transparency, child: appBar))) }));
        }
        global::Doroti.Framework.Services.SystemUiOverlayStyle overlayStyle = ((((((AppBar)this.widget).systemOverlayStyle ?? appBarTheme.systemOverlayStyle) ?? defaults.systemOverlayStyle) ?? (global::Doroti.Framework.Services.SystemUiOverlayStyle)_systemOverlayStyleForBrightness(ThemeData.estimateBrightnessForColor(effectiveBackgroundColor), (theme.useMaterial3 ? new global::Doroti.Ui.Color(0L) : null))));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(container: true, child: new global::Doroti.Framework.Widgets.AnnotatedRegion<global::Doroti.Framework.Services.SystemUiOverlayStyle>(value: overlayStyle, child: new Material(color: (theme.useMaterial3 ? effectiveBackgroundColor : backgroundColorLocal), elevation: effectiveElevation, type: (((AppBar)this.widget).forceMaterialTransparency ? MaterialType.transparency : MaterialType.canvas), shadowColor: ((((AppBar)this.widget).shadowColor ?? appBarTheme.shadowColor) ?? defaults.shadowColor), surfaceTintColor: ((((AppBar)this.widget).surfaceTintColor ?? appBarTheme.surfaceTintColor) ?? ((theme.useMaterial3 ? theme.colorScheme.surfaceTint : null))), shape: ((((AppBar)this.widget).shape ?? appBarTheme.shape) ?? defaults.shape), animateColor: ((AppBar)this.widget).animateColor, child: new global::Doroti.Framework.Widgets.Semantics(explicitChildNodes: true, child: appBar)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SliverAppBarDelegate__app_bar : global::Doroti.Framework.Widgets.SliverPersistentHeaderDelegate
{
    public virtual global::Doroti.Framework.Widgets.Widget? leading { get; private set; }
    public virtual bool automaticallyImplyLeading { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? title { get; private set; }
    public virtual List<global::Doroti.Framework.Widgets.Widget>? actions { get; private set; }
    public virtual bool automaticallyImplyActions { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? flexibleSpace { get; private set; }
    public virtual global::Doroti.Framework.Widgets.PreferredSizeWidget? bottom { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual double? scrolledUnderElevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual bool forceElevated { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? foregroundColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.IconThemeData? iconTheme { get; private set; }
    public virtual global::Doroti.Framework.Widgets.IconThemeData? actionsIconTheme { get; private set; }
    public virtual bool primary { get; private set; } = default!;
    public virtual bool? centerTitle { get; private set; }
    public virtual bool excludeHeaderSemantics { get; private set; } = default!;
    public virtual double? titleSpacing { get; private set; }
    public virtual double? expandedHeight { get; private set; }
    public virtual double collapsedHeight { get; private set; } = default!;
    public virtual double topPadding { get; private set; } = default!;
    public virtual bool floating { get; private set; } = default!;
    public virtual bool pinned { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual double? toolbarHeight { get; private set; }
    public virtual double? leadingWidth { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? toolbarTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? titleTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Services.SystemUiOverlayStyle? systemOverlayStyle { get; private set; }
    internal virtual double _bottomHeight { get; private set; } = default!;
    public virtual bool forceMaterialTransparency { get; private set; } = default!;
    public virtual bool useDefaultSemanticsOrder { get; private set; } = default!;
    public virtual Clip? clipBehavior { get; private set; }
    public virtual _SliverAppVariant__app_bar variant { get; private set; } = default!;
    public virtual bool accessibleNavigation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? actionsPadding { get; private set; }
    private global::Doroti.Framework.Scheduler.TickerProvider? __field_vsync = default!;
    public override global::Doroti.Framework.Scheduler.TickerProvider? vsync { get => __field_vsync; }
    private global::Doroti.Framework.Rendering.FloatingHeaderSnapConfiguration? __field_snapConfiguration = default!;
    public override global::Doroti.Framework.Rendering.FloatingHeaderSnapConfiguration? snapConfiguration { get => __field_snapConfiguration; }
    private global::Doroti.Framework.Rendering.OverScrollHeaderStretchConfiguration? __field_stretchConfiguration = default!;
    public override global::Doroti.Framework.Rendering.OverScrollHeaderStretchConfiguration? stretchConfiguration { get => __field_stretchConfiguration; }
    private global::Doroti.Framework.Rendering.PersistentHeaderShowOnScreenConfiguration? __field_showOnScreenConfiguration = default!;
    public override global::Doroti.Framework.Rendering.PersistentHeaderShowOnScreenConfiguration? showOnScreenConfiguration { get => __field_showOnScreenConfiguration; }

    internal _SliverAppBarDelegate__app_bar(global::Doroti.Framework.Widgets.Widget? leading, bool automaticallyImplyLeading, global::Doroti.Framework.Widgets.Widget? title, List<global::Doroti.Framework.Widgets.Widget>? actions, bool automaticallyImplyActions, global::Doroti.Framework.Widgets.Widget? flexibleSpace, global::Doroti.Framework.Widgets.PreferredSizeWidget? bottom, double? elevation, double? scrolledUnderElevation, Color? shadowColor, Color? surfaceTintColor, bool forceElevated, Color? backgroundColor, Color? foregroundColor, global::Doroti.Framework.Widgets.IconThemeData? iconTheme, global::Doroti.Framework.Widgets.IconThemeData? actionsIconTheme, bool primary, bool? centerTitle, bool excludeHeaderSemantics, double? titleSpacing, double? expandedHeight, double collapsedHeight, double topPadding, bool floating, bool pinned, global::Doroti.Framework.Scheduler.TickerProvider vsync, global::Doroti.Framework.Rendering.FloatingHeaderSnapConfiguration? snapConfiguration, global::Doroti.Framework.Rendering.OverScrollHeaderStretchConfiguration? stretchConfiguration, global::Doroti.Framework.Rendering.PersistentHeaderShowOnScreenConfiguration? showOnScreenConfiguration, global::Doroti.Framework.Painting.ShapeBorder? shape, double? toolbarHeight, double? leadingWidth, global::Doroti.Framework.Painting.TextStyle? toolbarTextStyle, global::Doroti.Framework.Painting.TextStyle? titleTextStyle, global::Doroti.Framework.Services.SystemUiOverlayStyle? systemOverlayStyle, bool forceMaterialTransparency, bool useDefaultSemanticsOrder, Clip? clipBehavior, _SliverAppVariant__app_bar variant, bool accessibleNavigation, global::Doroti.Framework.Painting.EdgeInsetsGeometry? actionsPadding)
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
        this.__field_vsync = vsync;
        this.__field_snapConfiguration = snapConfiguration;
        this.__field_stretchConfiguration = stretchConfiguration;
        this.__field_showOnScreenConfiguration = showOnScreenConfiguration;
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
        this._bottomHeight = (bottom?.preferredSize.height ?? 0.0);
        System.Diagnostics.Debug.Assert((primary || (topPadding == 0.0)));
    }

    public override double minExtent => this.collapsedHeight;
    public override double maxExtent => Math.Max((this.topPadding + ((this.expandedHeight ?? (((this.toolbarHeight ?? ConstantsLibrary.kToolbarHeight)) + this._bottomHeight)))), this.minExtent);
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context, double shrinkOffset, bool overlapsContent)
    {
        double visibleMainHeight = ((this.maxExtent - shrinkOffset) - this.topPadding);
        double extraToolbarHeight = Math.Max((((this.minExtent - this._bottomHeight) - this.topPadding) - ((this.toolbarHeight ?? ConstantsLibrary.kToolbarHeight))), 0.0);
        double visibleToolbarHeight = ((visibleMainHeight - this._bottomHeight) - extraToolbarHeight);
        bool isScrolledUnderLocal = ((overlapsContent || this.forceElevated) || ((this.pinned && (shrinkOffset > (this.maxExtent - this.minExtent)))));
        bool isPinnedWithOpacityFade = (((this.pinned && this.floating) && (this.bottom is not null)) && (extraToolbarHeight == 0.0));
        double toolbarOpacityLocal = ((!this.accessibleNavigation && ((!this.pinned || isPinnedWithOpacityFade))) ? Dart_uiLibrary.clampDouble((visibleToolbarHeight / ((this.toolbarHeight ?? ConstantsLibrary.kToolbarHeight))), 0.0, 1.0) : 1.0);
        global::Doroti.Framework.Widgets.Widget? effectiveTitle = (this.variant switch { _SliverAppVariant__app_bar.small => this.title, _SliverAppVariant__app_bar.medium => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.AnimatedOpacity(opacity: (isScrolledUnderLocal ? 1 : 0), duration: Duration.Create(milliseconds: 500L), curve: new global::Doroti.Framework.Animation.Cubic(0.2, 0.0, 0.0, 1.0), child: this.title)), _SliverAppVariant__app_bar.large => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.AnimatedOpacity(opacity: (isScrolledUnderLocal ? 1 : 0), duration: Duration.Create(milliseconds: 500L), curve: new global::Doroti.Framework.Animation.Cubic(0.2, 0.0, 0.0, 1.0), child: this.title)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Framework.Widgets.Widget appBar = ((global::Doroti.Framework.Widgets.Widget)(object?)FlexibleSpaceBar.createSettings(minExtent: this.minExtent, maxExtent: this.maxExtent, currentExtent: Math.Max(this.minExtent, (this.maxExtent - shrinkOffset)), toolbarOpacity: toolbarOpacityLocal, isScrolledUnder: isScrolledUnderLocal, hasLeading: ((this.leading is not null) || this.automaticallyImplyLeading), child: new AppBar(clipBehavior: this.clipBehavior, leading: this.leading, automaticallyImplyLeading: this.automaticallyImplyLeading, title: effectiveTitle, actions: this.actions, automaticallyImplyActions: this.automaticallyImplyActions, flexibleSpace: (((((this.title is null) && (this.flexibleSpace is not null)) && !this.excludeHeaderSemantics)) ? new global::Doroti.Framework.Widgets.Semantics(header: true, child: this.flexibleSpace) : this.flexibleSpace), bottom: this.bottom, elevation: (isScrolledUnderLocal ? this.elevation : 0.0), scrolledUnderElevation: this.scrolledUnderElevation, shadowColor: this.shadowColor, surfaceTintColor: this.surfaceTintColor, backgroundColor: this.backgroundColor, foregroundColor: this.foregroundColor, iconTheme: this.iconTheme, actionsIconTheme: this.actionsIconTheme, primary: this.primary, centerTitle: this.centerTitle, excludeHeaderSemantics: this.excludeHeaderSemantics, titleSpacing: this.titleSpacing, shape: this.shape, toolbarOpacity: toolbarOpacityLocal, bottomOpacity: (this.pinned ? 1.0 : Dart_uiLibrary.clampDouble((visibleMainHeight / this._bottomHeight), 0.0, 1.0)), toolbarHeight: this.toolbarHeight, leadingWidth: this.leadingWidth, toolbarTextStyle: this.toolbarTextStyle, titleTextStyle: this.titleTextStyle, systemOverlayStyle: this.systemOverlayStyle, forceMaterialTransparency: this.forceMaterialTransparency, useDefaultSemanticsOrder: this.useDefaultSemanticsOrder, actionsPadding: this.actionsPadding)));
        return appBar;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRebuild(global::Doroti.Framework.Widgets.SliverPersistentHeaderDelegate oldDelegate)
    {
        var __oldDelegate = (_SliverAppBarDelegate__app_bar)(object)oldDelegate;
        return (((((((((((((((((((((((((((((((((((!object.Equals(this.leading, ((_SliverAppBarDelegate__app_bar)__oldDelegate).leading)) || (this.automaticallyImplyLeading != ((_SliverAppBarDelegate__app_bar)__oldDelegate).automaticallyImplyLeading)) || (!object.Equals(this.title, ((_SliverAppBarDelegate__app_bar)__oldDelegate).title))) || (!object.Equals(this.actions, ((_SliverAppBarDelegate__app_bar)__oldDelegate).actions))) || (this.automaticallyImplyActions != ((_SliverAppBarDelegate__app_bar)__oldDelegate).automaticallyImplyActions)) || (!object.Equals(this.flexibleSpace, ((_SliverAppBarDelegate__app_bar)__oldDelegate).flexibleSpace))) || (!object.Equals(this.bottom, ((_SliverAppBarDelegate__app_bar)__oldDelegate).bottom))) || (this._bottomHeight != ((_SliverAppBarDelegate__app_bar)__oldDelegate)._bottomHeight)) || (this.elevation != ((_SliverAppBarDelegate__app_bar)__oldDelegate).elevation)) || (!object.Equals(this.shadowColor, ((_SliverAppBarDelegate__app_bar)__oldDelegate).shadowColor))) || (!object.Equals(this.backgroundColor, ((_SliverAppBarDelegate__app_bar)__oldDelegate).backgroundColor))) || (!object.Equals(this.foregroundColor, ((_SliverAppBarDelegate__app_bar)__oldDelegate).foregroundColor))) || (!object.Equals(this.iconTheme, ((_SliverAppBarDelegate__app_bar)__oldDelegate).iconTheme))) || (!object.Equals(this.actionsIconTheme, ((_SliverAppBarDelegate__app_bar)__oldDelegate).actionsIconTheme))) || (this.primary != ((_SliverAppBarDelegate__app_bar)__oldDelegate).primary)) || (this.centerTitle != ((_SliverAppBarDelegate__app_bar)__oldDelegate).centerTitle)) || (this.titleSpacing != ((_SliverAppBarDelegate__app_bar)__oldDelegate).titleSpacing)) || (this.expandedHeight != ((_SliverAppBarDelegate__app_bar)__oldDelegate).expandedHeight)) || (this.topPadding != ((_SliverAppBarDelegate__app_bar)__oldDelegate).topPadding)) || (this.pinned != ((_SliverAppBarDelegate__app_bar)__oldDelegate).pinned)) || (this.floating != ((_SliverAppBarDelegate__app_bar)__oldDelegate).floating)) || (!object.Equals(this.vsync, ((_SliverAppBarDelegate__app_bar)__oldDelegate).vsync))) || (!object.Equals(this.snapConfiguration, ((_SliverAppBarDelegate__app_bar)__oldDelegate).snapConfiguration))) || (!object.Equals(this.stretchConfiguration, ((_SliverAppBarDelegate__app_bar)__oldDelegate).stretchConfiguration))) || (!object.Equals(this.showOnScreenConfiguration, ((_SliverAppBarDelegate__app_bar)__oldDelegate).showOnScreenConfiguration))) || (this.forceElevated != ((_SliverAppBarDelegate__app_bar)__oldDelegate).forceElevated)) || (this.toolbarHeight != ((_SliverAppBarDelegate__app_bar)__oldDelegate).toolbarHeight)) || (this.leadingWidth != ((_SliverAppBarDelegate__app_bar)__oldDelegate).leadingWidth)) || (!object.Equals(this.toolbarTextStyle, ((_SliverAppBarDelegate__app_bar)__oldDelegate).toolbarTextStyle))) || (!object.Equals(this.titleTextStyle, ((_SliverAppBarDelegate__app_bar)__oldDelegate).titleTextStyle))) || (!object.Equals(this.systemOverlayStyle, ((_SliverAppBarDelegate__app_bar)__oldDelegate).systemOverlayStyle))) || (this.forceMaterialTransparency != ((_SliverAppBarDelegate__app_bar)__oldDelegate).forceMaterialTransparency)) || (this.useDefaultSemanticsOrder != ((_SliverAppBarDelegate__app_bar)__oldDelegate).useDefaultSemanticsOrder)) || (this.accessibleNavigation != ((_SliverAppBarDelegate__app_bar)__oldDelegate).accessibleNavigation)) || (!object.Equals(this.actionsPadding, ((_SliverAppBarDelegate__app_bar)__oldDelegate).actionsPadding)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}(topPadding: {this.topPadding.toStringAsFixed(1L)}, bottomHeight: {this._bottomHeight.toStringAsFixed(1L)}, ...)";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverAppBar : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget? leading { get; private set; }
    public virtual bool automaticallyImplyLeading { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? title { get; private set; }
    public virtual List<global::Doroti.Framework.Widgets.Widget>? actions { get; private set; }
    public virtual bool automaticallyImplyActions { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? flexibleSpace { get; private set; }
    public virtual global::Doroti.Framework.Widgets.PreferredSizeWidget? bottom { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual double? scrolledUnderElevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual bool forceElevated { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? foregroundColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.IconThemeData? iconTheme { get; private set; }
    public virtual global::Doroti.Framework.Widgets.IconThemeData? actionsIconTheme { get; private set; }
    public virtual bool primary { get; private set; } = default!;
    public virtual bool? centerTitle { get; private set; }
    public virtual bool excludeHeaderSemantics { get; private set; } = default!;
    public virtual double? titleSpacing { get; private set; }
    public virtual double? collapsedHeight { get; private set; }
    public virtual double? expandedHeight { get; private set; }
    public virtual bool floating { get; private set; } = default!;
    public virtual bool pinned { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual bool snap { get; private set; } = default!;
    public virtual bool stretch { get; private set; } = default!;
    public virtual double stretchTriggerOffset { get; private set; } = default!;
    public virtual global::System.Func<Future>? onStretchTrigger { get; private set; }
    public virtual double toolbarHeight { get; private set; } = default!;
    public virtual double? leadingWidth { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? toolbarTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? titleTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Services.SystemUiOverlayStyle? systemOverlayStyle { get; private set; }
    public virtual bool forceMaterialTransparency { get; private set; } = default!;
    public virtual bool useDefaultSemanticsOrder { get; private set; } = default!;
    public virtual Clip? clipBehavior { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? actionsPadding { get; private set; }
    internal virtual _SliverAppVariant__app_bar _variant { get; private set; } = default!;

    public SliverAppBar(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget? leading = null, bool automaticallyImplyLeading = true, global::Doroti.Framework.Widgets.Widget? title = null, List<global::Doroti.Framework.Widgets.Widget>? actions = null, bool automaticallyImplyActions = true, global::Doroti.Framework.Widgets.Widget? flexibleSpace = null, global::Doroti.Framework.Widgets.PreferredSizeWidget? bottom = null, double? elevation = null, double? scrolledUnderElevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, bool forceElevated = false, Color? backgroundColor = null, Color? foregroundColor = null, global::Doroti.Framework.Widgets.IconThemeData? iconTheme = null, global::Doroti.Framework.Widgets.IconThemeData? actionsIconTheme = null, bool primary = true, bool? centerTitle = null, bool excludeHeaderSemantics = false, double? titleSpacing = null, double? collapsedHeight = null, double? expandedHeight = null, bool floating = false, bool pinned = false, bool snap = false, bool stretch = false, double stretchTriggerOffset = 100.0, global::System.Func<Future>? onStretchTrigger = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, double? toolbarHeight = null, double? leadingWidth = null, global::Doroti.Framework.Painting.TextStyle? toolbarTextStyle = null, global::Doroti.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Framework.Services.SystemUiOverlayStyle? systemOverlayStyle = null, bool forceMaterialTransparency = false, bool useDefaultSemanticsOrder = true, Clip? clipBehavior = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? actionsPadding = null) : base(key: key)
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
        this._variant = _SliverAppVariant__app_bar.small;
        System.Diagnostics.Debug.Assert((floating || !snap));
        System.Diagnostics.Debug.Assert((stretchTriggerOffset > 0.0));
        System.Diagnostics.Debug.Assert(((collapsedHeight is null) || (collapsedHeight >= __toolbarHeight)));
    }

    public static SliverAppBar CreateMedium(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget? leading = null, bool automaticallyImplyLeading = true, global::Doroti.Framework.Widgets.Widget? title = null, List<global::Doroti.Framework.Widgets.Widget>? actions = null, bool automaticallyImplyActions = true, global::Doroti.Framework.Widgets.Widget? flexibleSpace = null, global::Doroti.Framework.Widgets.PreferredSizeWidget? bottom = null, double? elevation = null, double? scrolledUnderElevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, bool forceElevated = false, Color? backgroundColor = null, Color? foregroundColor = null, global::Doroti.Framework.Widgets.IconThemeData? iconTheme = null, global::Doroti.Framework.Widgets.IconThemeData? actionsIconTheme = null, bool primary = true, bool? centerTitle = null, bool excludeHeaderSemantics = false, double? titleSpacing = null, double? collapsedHeight = null, double? expandedHeight = null, bool floating = false, bool pinned = true, bool snap = false, bool stretch = false, double stretchTriggerOffset = 100.0, global::System.Func<Future>? onStretchTrigger = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, double? toolbarHeight = null, double? leadingWidth = null, global::Doroti.Framework.Painting.TextStyle? toolbarTextStyle = null, global::Doroti.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Framework.Services.SystemUiOverlayStyle? systemOverlayStyle = null, bool forceMaterialTransparency = false, bool useDefaultSemanticsOrder = true, Clip? clipBehavior = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? actionsPadding = null)
    {
        var __instance = new SliverAppBar(key: key, leading: leading, automaticallyImplyLeading: automaticallyImplyLeading, title: title, actions: actions, automaticallyImplyActions: automaticallyImplyActions, flexibleSpace: flexibleSpace, bottom: bottom, elevation: elevation, scrolledUnderElevation: scrolledUnderElevation, shadowColor: shadowColor, surfaceTintColor: surfaceTintColor, forceElevated: forceElevated, backgroundColor: backgroundColor, foregroundColor: foregroundColor, iconTheme: iconTheme, actionsIconTheme: actionsIconTheme, primary: primary, centerTitle: centerTitle, excludeHeaderSemantics: excludeHeaderSemantics, titleSpacing: titleSpacing, collapsedHeight: collapsedHeight, expandedHeight: expandedHeight, floating: floating, pinned: pinned, snap: snap, stretch: stretch, stretchTriggerOffset: stretchTriggerOffset, onStretchTrigger: onStretchTrigger, shape: shape, toolbarHeight: toolbarHeight, leadingWidth: leadingWidth, toolbarTextStyle: toolbarTextStyle, titleTextStyle: titleTextStyle, systemOverlayStyle: systemOverlayStyle, forceMaterialTransparency: forceMaterialTransparency, useDefaultSemanticsOrder: useDefaultSemanticsOrder, clipBehavior: clipBehavior, actionsPadding: actionsPadding);
        double __toolbarHeight = toolbarHeight ?? _MediumScrollUnderFlexibleConfig__app_bar.collapsedHeight;
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

    public static SliverAppBar CreateLarge(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget? leading = null, bool automaticallyImplyLeading = true, global::Doroti.Framework.Widgets.Widget? title = null, List<global::Doroti.Framework.Widgets.Widget>? actions = null, bool automaticallyImplyActions = true, global::Doroti.Framework.Widgets.Widget? flexibleSpace = null, global::Doroti.Framework.Widgets.PreferredSizeWidget? bottom = null, double? elevation = null, double? scrolledUnderElevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, bool forceElevated = false, Color? backgroundColor = null, Color? foregroundColor = null, global::Doroti.Framework.Widgets.IconThemeData? iconTheme = null, global::Doroti.Framework.Widgets.IconThemeData? actionsIconTheme = null, bool primary = true, bool? centerTitle = null, bool excludeHeaderSemantics = false, double? titleSpacing = null, double? collapsedHeight = null, double? expandedHeight = null, bool floating = false, bool pinned = true, bool snap = false, bool stretch = false, double stretchTriggerOffset = 100.0, global::System.Func<Future>? onStretchTrigger = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, double? toolbarHeight = null, double? leadingWidth = null, global::Doroti.Framework.Painting.TextStyle? toolbarTextStyle = null, global::Doroti.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Framework.Services.SystemUiOverlayStyle? systemOverlayStyle = null, bool forceMaterialTransparency = false, bool useDefaultSemanticsOrder = true, Clip? clipBehavior = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? actionsPadding = null)
    {
        var __instance = new SliverAppBar(key: key, leading: leading, automaticallyImplyLeading: automaticallyImplyLeading, title: title, actions: actions, automaticallyImplyActions: automaticallyImplyActions, flexibleSpace: flexibleSpace, bottom: bottom, elevation: elevation, scrolledUnderElevation: scrolledUnderElevation, shadowColor: shadowColor, surfaceTintColor: surfaceTintColor, forceElevated: forceElevated, backgroundColor: backgroundColor, foregroundColor: foregroundColor, iconTheme: iconTheme, actionsIconTheme: actionsIconTheme, primary: primary, centerTitle: centerTitle, excludeHeaderSemantics: excludeHeaderSemantics, titleSpacing: titleSpacing, collapsedHeight: collapsedHeight, expandedHeight: expandedHeight, floating: floating, pinned: pinned, snap: snap, stretch: stretch, stretchTriggerOffset: stretchTriggerOffset, onStretchTrigger: onStretchTrigger, shape: shape, toolbarHeight: toolbarHeight, leadingWidth: leadingWidth, toolbarTextStyle: toolbarTextStyle, titleTextStyle: titleTextStyle, systemOverlayStyle: systemOverlayStyle, forceMaterialTransparency: forceMaterialTransparency, useDefaultSemanticsOrder: useDefaultSemanticsOrder, clipBehavior: clipBehavior, actionsPadding: actionsPadding);
        double __toolbarHeight = toolbarHeight ?? _LargeScrollUnderFlexibleConfig__app_bar.collapsedHeight;
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

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SliverAppBarState__app_bar());
}

internal class _SliverAppBarState__app_bar : global::Doroti.Framework.Widgets.State<SliverAppBar>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<SliverAppBar>
{
    internal virtual global::Doroti.Framework.Rendering.FloatingHeaderSnapConfiguration? _snapConfiguration { get; set; } = default;
    internal virtual global::Doroti.Framework.Rendering.OverScrollHeaderStretchConfiguration? _stretchConfiguration { get; set; } = default;
    internal virtual global::Doroti.Framework.Rendering.PersistentHeaderShowOnScreenConfiguration? _showOnScreenConfiguration { get; set; } = default;
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual void _updateSnapConfiguration()
    {
        if ((((SliverAppBar)this.widget).snap && ((SliverAppBar)this.widget).floating))
        {
            _snapConfiguration = new global::Doroti.Framework.Rendering.FloatingHeaderSnapConfiguration(curve: global::Doroti.Framework.Animation.Curves.easeOut, duration: Duration.Create(milliseconds: 200L));
        }
        else
        {
            _snapConfiguration = null;
        }
        _showOnScreenConfiguration = ((((SliverAppBar)this.widget).floating & ((SliverAppBar)this.widget).snap) ? new global::Doroti.Framework.Rendering.PersistentHeaderShowOnScreenConfiguration(minShowOnScreenExtent: double.PositiveInfinity) : null);
    }

    internal virtual void _updateStretchConfiguration()
    {
        if (((SliverAppBar)this.widget).stretch)
        {
            _stretchConfiguration = new global::Doroti.Framework.Rendering.OverScrollHeaderStretchConfiguration(stretchTriggerOffset: ((SliverAppBar)this.widget).stretchTriggerOffset, onStretchTrigger: (global::System.Func<Future>?)((SliverAppBar)this.widget).onStretchTrigger);
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
        if (((((SliverAppBar)this.widget).snap != ((SliverAppBar)oldWidget).snap) || (((SliverAppBar)this.widget).floating != ((SliverAppBar)oldWidget).floating)))
        {
            _updateSnapConfiguration();
        }
        if ((((SliverAppBar)this.widget).stretch != ((SliverAppBar)oldWidget).stretch))
        {
            _updateStretchConfiguration();
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => (!((SliverAppBar)this.widget).primary || global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context)));
        double bottomHeightLocal = (((SliverAppBar)this.widget).bottom?.preferredSize.height ?? 0.0);
        double topPaddingLocal = (((SliverAppBar)this.widget).primary ? MediaQuery.paddingOf(context).top : 0.0);
        double collapsedHeightLocal = ((((((SliverAppBar)this.widget).pinned && ((SliverAppBar)this.widget).floating) && (((SliverAppBar)this.widget).bottom is not null))) ? ((((((SliverAppBar)this.widget).collapsedHeight ?? 0.0)) + bottomHeightLocal) + topPaddingLocal) : ((((((SliverAppBar)this.widget).collapsedHeight ?? ((SliverAppBar)this.widget).toolbarHeight)) + bottomHeightLocal) + topPaddingLocal));
        double? effectiveExpandedHeight = default!;
        double effectiveCollapsedHeight = default!;
        global::Doroti.Framework.Widgets.Widget? effectiveFlexibleSpace = default!;
        switch (((SliverAppBar)this.widget)._variant)
        {
            case _SliverAppVariant__app_bar.small:
                {
                    effectiveExpandedHeight = ((SliverAppBar)this.widget).expandedHeight;
                    effectiveCollapsedHeight = collapsedHeightLocal;
                    effectiveFlexibleSpace = ((SliverAppBar)this.widget).flexibleSpace;
                    break;
                }
            case _SliverAppVariant__app_bar.medium:
                {
                    effectiveExpandedHeight = (((SliverAppBar)this.widget).expandedHeight ?? (_MediumScrollUnderFlexibleConfig__app_bar.expandedHeight + bottomHeightLocal));
                    effectiveCollapsedHeight = (((SliverAppBar)this.widget).collapsedHeight ?? ((topPaddingLocal + _MediumScrollUnderFlexibleConfig__app_bar.collapsedHeight) + bottomHeightLocal));
                    effectiveFlexibleSpace = (((SliverAppBar)this.widget).flexibleSpace ?? new _ScrollUnderFlexibleSpace__app_bar(title: ((SliverAppBar)this.widget).title, foregroundColor: ((SliverAppBar)this.widget).foregroundColor, configBuilder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, _MediumScrollUnderFlexibleConfig__app_bar>)((arg0) => new _MediumScrollUnderFlexibleConfig__app_bar(arg0))), titleTextStyle: ((SliverAppBar)this.widget).titleTextStyle, bottomHeight: bottomHeightLocal));
                    break;
                }
            case _SliverAppVariant__app_bar.large:
                {
                    effectiveExpandedHeight = (((SliverAppBar)this.widget).expandedHeight ?? (_LargeScrollUnderFlexibleConfig__app_bar.expandedHeight + bottomHeightLocal));
                    effectiveCollapsedHeight = (((SliverAppBar)this.widget).collapsedHeight ?? ((topPaddingLocal + _LargeScrollUnderFlexibleConfig__app_bar.collapsedHeight) + bottomHeightLocal));
                    effectiveFlexibleSpace = (((SliverAppBar)this.widget).flexibleSpace ?? new _ScrollUnderFlexibleSpace__app_bar(title: ((SliverAppBar)this.widget).title, foregroundColor: ((SliverAppBar)this.widget).foregroundColor, configBuilder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, _LargeScrollUnderFlexibleConfig__app_bar>)((arg0) => new _LargeScrollUnderFlexibleConfig__app_bar(arg0))), titleTextStyle: ((SliverAppBar)this.widget).titleTextStyle, bottomHeight: bottomHeightLocal));
                    break;
                }
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.MediaQuery.CreateRemovePadding(context: context, removeBottom: true, child: new global::Doroti.Framework.Widgets.SliverPersistentHeader(floating: ((SliverAppBar)this.widget).floating, pinned: ((SliverAppBar)this.widget).pinned, @delegate: new _SliverAppBarDelegate__app_bar(vsync: this, leading: ((SliverAppBar)this.widget).leading, automaticallyImplyLeading: ((SliverAppBar)this.widget).automaticallyImplyLeading, title: ((SliverAppBar)this.widget).title, actions: ((SliverAppBar)this.widget).actions, automaticallyImplyActions: ((SliverAppBar)this.widget).automaticallyImplyActions, flexibleSpace: effectiveFlexibleSpace, bottom: ((SliverAppBar)this.widget).bottom, elevation: ((SliverAppBar)this.widget).elevation, scrolledUnderElevation: ((SliverAppBar)this.widget).scrolledUnderElevation, shadowColor: ((SliverAppBar)this.widget).shadowColor, surfaceTintColor: ((SliverAppBar)this.widget).surfaceTintColor, forceElevated: ((SliverAppBar)this.widget).forceElevated, backgroundColor: ((SliverAppBar)this.widget).backgroundColor, foregroundColor: ((SliverAppBar)this.widget).foregroundColor, iconTheme: ((SliverAppBar)this.widget).iconTheme, actionsIconTheme: ((SliverAppBar)this.widget).actionsIconTheme, primary: ((SliverAppBar)this.widget).primary, centerTitle: ((SliverAppBar)this.widget).centerTitle, excludeHeaderSemantics: ((SliverAppBar)this.widget).excludeHeaderSemantics, titleSpacing: ((SliverAppBar)this.widget).titleSpacing, expandedHeight: effectiveExpandedHeight, collapsedHeight: effectiveCollapsedHeight, topPadding: topPaddingLocal, floating: ((SliverAppBar)this.widget).floating, pinned: ((SliverAppBar)this.widget).pinned, shape: ((SliverAppBar)this.widget).shape, snapConfiguration: this._snapConfiguration, stretchConfiguration: this._stretchConfiguration, showOnScreenConfiguration: this._showOnScreenConfiguration, toolbarHeight: ((SliverAppBar)this.widget).toolbarHeight, leadingWidth: ((SliverAppBar)this.widget).leadingWidth, toolbarTextStyle: ((SliverAppBar)this.widget).toolbarTextStyle, titleTextStyle: ((SliverAppBar)this.widget).titleTextStyle, systemOverlayStyle: ((SliverAppBar)this.widget).systemOverlayStyle, forceMaterialTransparency: ((SliverAppBar)this.widget).forceMaterialTransparency, useDefaultSemanticsOrder: ((SliverAppBar)this.widget).useDefaultSemanticsOrder, clipBehavior: ((SliverAppBar)this.widget).clipBehavior, variant: ((SliverAppBar)this.widget)._variant, accessibleNavigation: MediaQuery.of(context).accessibleNavigation, actionsPadding: ((SliverAppBar)this.widget).actionsPadding))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = this._tickerModeNotifier!.value;
        var result = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values).enabled;
    __cascade.forceFrames = ((TickerModeData)values).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => (this._tickers is not null));
        DartRuntimePrimitives.Assert(() => this._tickers!.Contains(ticker));
        this._tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if ((this._tickers is not null))
        {
            TickerModeData values = this._tickerModeNotifier!.value;
            bool mutedLocal = !((TickerModeData)values).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = ((TickerModeData)values).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier;
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        this._tickerModeNotifier = null;
        base.dispose();
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

internal class _AppBarTitleBox__app_bar : global::Doroti.Framework.Widgets.SingleChildRenderObjectWidget
{
    internal _AppBarTitleBox__app_bar(global::Doroti.Framework.Widgets.Widget child) : base(child: child)
    {
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderAppBarTitleBox__app_bar(textDirection: Directionality.of(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderAppBarTitleBox__app_bar)(object)renderObject;
        ((dynamic)__renderObject).textDirection = Directionality.of(context);
    }

}

public class _RenderAppBarTitleBox__app_bar : global::Doroti.Framework.Rendering.RenderAligningShiftedBox
{
    internal _RenderAppBarTitleBox__app_bar(TextDirection? textDirection = null) : base(textDirection: textDirection, alignment: global::Doroti.Framework.Painting.Alignment.center)
    {
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        global::Doroti.Framework.Rendering.BoxConstraints innerConstraints = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)constraints.copyWith(maxHeight: double.PositiveInfinity));
        global::Doroti.Ui.Size childSize = ((global::Doroti.Ui.Size)(object?)this.child!.getDryLayout(innerConstraints));
        return constraints.constrain(childSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Framework.Rendering.BoxConstraints innerConstraints = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)constraints.copyWith(maxHeight: double.PositiveInfinity));
        global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((childLocal is null))
        {
            return null;
        }
        double? result = childLocal.getDryBaseline(innerConstraints, baseline);
        if ((result is null))
        {
            return null;
        }
        global::Doroti.Ui.Size childSize = ((global::Doroti.Ui.Size)(object?)childLocal.getDryLayout(innerConstraints));
        return (DartRuntimePrimitives.RequireValue(result) + this.resolvedAlignment.alongOffset((getDryLayout(constraints) - childSize)).dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Framework.Rendering.BoxConstraints innerConstraints = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)this.constraints.copyWith(maxHeight: double.PositiveInfinity));
        this.child!.layout(innerConstraints, parentUsesSize: true);
        size = this.constraints.constrain(this.child!.size);
        alignChild();
    }

}

internal class _ScrollUnderFlexibleSpace__app_bar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget? title { get; private set; }
    public virtual Color? foregroundColor { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, _ScrollUnderFlexibleConfig__app_bar> configBuilder { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle? titleTextStyle { get; private set; }
    public virtual double bottomHeight { get; private set; } = default!;

    internal _ScrollUnderFlexibleSpace__app_bar(global::Doroti.Framework.Widgets.Widget? title = null, Color? foregroundColor = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, _ScrollUnderFlexibleConfig__app_bar> configBuilder = default!, global::Doroti.Framework.Painting.TextStyle? titleTextStyle = null, double bottomHeight = default!)
    {
        this.title = title;
        this.foregroundColor = foregroundColor;
        this.configBuilder = configBuilder;
        this.titleTextStyle = titleTextStyle;
        this.bottomHeight = bottomHeight;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        AppBarThemeData appBarTheme = AppBarTheme.of(context);
        AppBarThemeData defaults = (Theme.of(context).useMaterial3 ? new _AppBarDefaultsM3__app_bar(context) : new _AppBarDefaultsM2__app_bar(context));
        FlexibleSpaceBarSettings settings = context.dependOnInheritedWidgetOfExactType<FlexibleSpaceBarSettings>()!;
        _ScrollUnderFlexibleConfig__app_bar config = this.configBuilder(context);
        DartRuntimePrimitives.Assert(() => ((_ScrollUnderFlexibleConfig__app_bar)config).expandedTitlePadding.isNonNegative, () => (object?)"The _ExpandedTitleWithPadding widget assumes that the expanded title padding is non-negative. " + "Update its implementation to handle negative padding.");
        global::Doroti.Framework.Painting.TextStyle? expandedTextStyleLocal = (((this.titleTextStyle ?? appBarTheme.titleTextStyle) ?? (global::Doroti.Framework.Painting.TextStyle)((_ScrollUnderFlexibleConfig__app_bar)config).expandedTextStyle?.copyWith(color: ((this.foregroundColor ?? appBarTheme.foregroundColor) ?? defaults.foregroundColor))));
        global::Doroti.Framework.Widgets.Widget? expandedTitle = ((this.title, expandedTextStyleLocal) switch { (null, _) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(null), (global::Doroti.Framework.Widgets.Widget titleLocal, null) => titleLocal, (global::Doroti.Framework.Widgets.Widget titleAlternate, global::Doroti.Framework.Painting.TextStyle textStyle) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.DefaultTextStyle(style: textStyle, child: titleAlternate)) });
        global::Doroti.Framework.Painting.EdgeInsets resolvedTitlePadding = ((global::Doroti.Framework.Painting.EdgeInsets)(object?)((_ScrollUnderFlexibleConfig__app_bar)config).expandedTitlePadding.resolve(Directionality.of(context)));
        global::Doroti.Framework.Painting.EdgeInsetsGeometry expandedTitlePaddingLocal = ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)(object?)((this.bottomHeight > 0L) ? resolvedTitlePadding.copyWith(bottom: 0) : resolvedTitlePadding));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)MediaQuery.withClampedTextScaling(maxScaleFactor: App_barLibrary._kMaxTitleTextScaleFactor, child: new global::Doroti.Framework.Widgets.Column(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection89388 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection89388.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(top: (((FlexibleSpaceBarSettings)settings).minExtent - this.bottomHeight))))); __collection89388.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: new global::Doroti.Framework.Widgets.ClipRect(child: new _ExpandedTitleWithPadding__app_bar(padding: expandedTitlePaddingLocal, maxExtent: (((FlexibleSpaceBarSettings)settings).maxExtent - ((FlexibleSpaceBarSettings)settings).minExtent), child: expandedTitle))))); if ((this.bottomHeight > 0L)) { __collection89388.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(bottom: this.bottomHeight)))); } return __collection89388; }))())));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ExpandedTitleWithPadding__app_bar : global::Doroti.Framework.Widgets.SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry padding { get; private set; } = default!;
    public virtual double maxExtent { get; private set; } = default!;

    internal _ExpandedTitleWithPadding__app_bar(global::Doroti.Framework.Painting.EdgeInsetsGeometry padding, double maxExtent, global::Doroti.Framework.Widgets.Widget? child = null) : base(child: child)
    {
        this.padding = padding;
        this.maxExtent = maxExtent;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.TextDirection textDirection = Directionality.of(context);
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderExpandedTitleBox__app_bar(this.padding.resolve(textDirection), global::Doroti.Framework.Painting.AlignmentDirectional.bottomStart.resolve(textDirection), this.maxExtent, null));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderExpandedTitleBox__app_bar)(object)renderObject;
        global::Doroti.Ui.TextDirection textDirection = Directionality.of(context);
        DartRuntimePrimitives.Ignore(((Func<_RenderExpandedTitleBox__app_bar>)(() =>
{
    var __cascade = __renderObject;
    __cascade.padding = this.padding.resolve(textDirection);
    __cascade.titleAlignment = global::Doroti.Framework.Painting.AlignmentDirectional.bottomStart.resolve(textDirection);
    __cascade.maxExtent = this.maxExtent;
    return __cascade;
}))());
    }

}

public class _RenderExpandedTitleBox__app_bar : global::Doroti.Framework.Rendering.RenderShiftedBox
{
    internal virtual global::Doroti.Framework.Painting.EdgeInsets _padding { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.Alignment _titleAlignment { get; set; } = default!;
    internal virtual double _maxExtent { get; set; } = default!;

    internal _RenderExpandedTitleBox__app_bar(global::Doroti.Framework.Painting.EdgeInsets _padding, global::Doroti.Framework.Painting.Alignment _titleAlignment, double _maxExtent, global::Doroti.Framework.Rendering.RenderBox? child) : base(child)
    {
        this._padding = _padding;
        this._titleAlignment = _titleAlignment;
        this._maxExtent = _maxExtent;
    }

    public virtual global::Doroti.Framework.Painting.EdgeInsets padding
    {
        get => this._padding;
        set
        {
            var __value = value;
            if ((object.Equals(this._padding, __value)))
            {
                return;
            }
            DartRuntimePrimitives.Assert(() => __value.isNonNegative);
            _padding = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Painting.Alignment titleAlignment
    {
        get => this._titleAlignment;
        set
        {
            var __value = value;
            if ((object.Equals(this._titleAlignment, __value)))
            {
                return;
            }
            _titleAlignment = __value;
            markNeedsLayout();
        }
    }
    public virtual double maxExtent
    {
        get => this._maxExtent;
        set
        {
            var __value = value;
            if ((this._maxExtent == __value))
            {
                return;
            }
            _maxExtent = __value;
            markNeedsLayout();
        }
    }
    public override double computeMaxIntrinsicHeight(double width)
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        return ((childLocal is null) ? 0.0 : (childLocal.getMaxIntrinsicHeight(Math.Max(0, (width - this.padding.horizontal))) + this.padding.vertical));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        return ((childLocal is null) ? 0.0 : (childLocal.getMaxIntrinsicWidth(double.PositiveInfinity) + this.padding.horizontal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        return ((childLocal is null) ? 0.0 : (childLocal.getMinIntrinsicHeight(Math.Max(0, (width - this.padding.horizontal))) + this.padding.vertical));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        return ((childLocal is null) ? 0.0 : (childLocal.getMinIntrinsicWidth(double.PositiveInfinity) + this.padding.horizontal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints) => ((this.child is null) ? Size.zero : ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).biggest);
    internal virtual global::Doroti.Ui.Offset _childOffsetFromSize(Size childSize, Size size)
    {
        DartRuntimePrimitives.Assert(() => (this.child is not null));
        DartRuntimePrimitives.Assert(() => this.padding.isNonNegative);
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Painting.Alignment)this.titleAlignment).y == 1.0));
        double yAdjustment = Dart_uiLibrary.clampDouble(((childSize.height + ((global::Doroti.Framework.Painting.EdgeInsets)this.padding).bottom) - this.maxExtent), 0, ((global::Doroti.Framework.Painting.EdgeInsets)this.padding).bottom);
        double offsetX = (((((((global::Doroti.Framework.Painting.Alignment)this.titleAlignment).x + 1L)) / 2L) * (((size.width - this.padding.horizontal) - childSize.width))) + ((global::Doroti.Framework.Painting.EdgeInsets)this.padding).left);
        double offsetY = (((size.height - childSize.height) - ((global::Doroti.Framework.Painting.EdgeInsets)this.padding).bottom) + yAdjustment);
        return new global::Doroti.Ui.Offset(offsetX, offsetY);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((childLocal is null))
        {
            return null;
        }
        global::Doroti.Framework.Rendering.BoxConstraints childConstraints = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)constraints.widthConstraints().deflate(this.padding));
        global::Doroti.Framework.Rendering.BaselineOffset result = (new global::Doroti.Framework.Rendering.BaselineOffset(childLocal.getDryBaseline(childConstraints, baseline)).op_Add(_childOffsetFromSize(childLocal.getDryLayout(childConstraints), getDryLayout(constraints)).dy));
        return result.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((childLocal is null))
        {
            size = ((global::Doroti.Framework.Rendering.BoxConstraints)this.constraints).smallest;
            return;
        }
        size = ((global::Doroti.Framework.Rendering.BoxConstraints)this.constraints).biggest;
        childLocal.layout(this.constraints.widthConstraints().deflate(this.padding), parentUsesSize: true);
        var childParentData = ((global::Doroti.Framework.Rendering.BoxParentData?)(object?)childLocal.parentData!)!;
        childParentData.offset = _childOffsetFromSize(((global::Doroti.Framework.Rendering.RenderBox)childLocal).size, this.size);
    }

}

internal interface _ScrollUnderFlexibleConfig__app_bar
{
    public global::Doroti.Framework.Painting.TextStyle? collapsedTextStyle { get; }
    public global::Doroti.Framework.Painting.TextStyle? expandedTextStyle { get; }
    public global::Doroti.Framework.Painting.EdgeInsetsGeometry expandedTitlePadding { get; }
}

internal class _AppBarDefaultsM2__app_bar : AppBarThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__theme_initialized;
    private ThemeData __late__theme = default!;
    internal virtual ThemeData _theme
    {
        get
        {
            if (!__late__theme_initialized)
            {
                __late__theme = Theme.of(this.context);
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
                __late__colors = this._theme.colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _AppBarDefaultsM2__app_bar(global::Doroti.Framework.Widgets.BuildContext context) : base(elevation: 4.0, shadowColor: new global::Doroti.Ui.Color(4278190080L), titleSpacing: global::Doroti.Framework.Widgets.NavigationToolbar.kMiddleSpacing, toolbarHeight: ConstantsLibrary.kToolbarHeight)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((object.Equals(this._colors.brightness, Brightness.dark)) ? this._colors.surface : this._colors.primary));
    public virtual global::Doroti.Ui.Color? foregroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((object.Equals(this._colors.brightness, Brightness.dark)) ? this._colors.onSurface : this._colors.onPrimary));
    public override global::Doroti.Framework.Widgets.IconThemeData? iconTheme => this._theme.iconTheme;
    public override global::Doroti.Framework.Painting.TextStyle? toolbarTextStyle => this._theme.textTheme.bodyMedium;
    public override global::Doroti.Framework.Painting.TextStyle? titleTextStyle => this._theme.textTheme.titleLarge;
    public virtual global::Doroti.Framework.Painting.EdgeInsets? actionsPadding => global::Doroti.Framework.Painting.EdgeInsets.zero;
}

internal class _AppBarDefaultsM3__app_bar : AppBarThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__theme_initialized;
    private ThemeData __late__theme = default!;
    internal virtual ThemeData _theme
    {
        get
        {
            if (!__late__theme_initialized)
            {
                __late__theme = Theme.of(this.context);
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
                __late__colors = this._theme.colorScheme;
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
                __late__textTheme = this._theme.textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }

    internal _AppBarDefaultsM3__app_bar(global::Doroti.Framework.Widgets.BuildContext context) : base(elevation: 0.0, scrolledUnderElevation: 3.0, titleSpacing: global::Doroti.Framework.Widgets.NavigationToolbar.kMiddleSpacing, toolbarHeight: 64.0)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.surface);
    public virtual global::Doroti.Ui.Color? foregroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface);
    public virtual global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public virtual global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public override global::Doroti.Framework.Widgets.IconThemeData? iconTheme => new global::Doroti.Framework.Widgets.IconThemeData(color: this._colors.onSurface, size: 24.0);
    public override global::Doroti.Framework.Widgets.IconThemeData? actionsIconTheme => new global::Doroti.Framework.Widgets.IconThemeData(color: this._colors.onSurfaceVariant, size: 24.0);
    public override global::Doroti.Framework.Painting.TextStyle? toolbarTextStyle => this._textTheme.bodyMedium;
    public override global::Doroti.Framework.Painting.TextStyle? titleTextStyle => this._textTheme.titleLarge;
    public virtual global::Doroti.Framework.Painting.EdgeInsets? actionsPadding => global::Doroti.Framework.Painting.EdgeInsets.zero;
}

internal class _MediumScrollUnderFlexibleConfig__app_bar : _ScrollUnderFlexibleConfig__app_bar
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__theme_initialized;
    private ThemeData __late__theme = default!;
    internal virtual ThemeData _theme
    {
        get
        {
            if (!__late__theme_initialized)
            {
                __late__theme = Theme.of(this.context);
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
                __late__colors = this._theme.colorScheme;
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
                __late__textTheme = this._theme.textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }
    public const double collapsedHeight = 64.0;
    public const double expandedHeight = 112.0;

    internal _MediumScrollUnderFlexibleConfig__app_bar(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public virtual global::Doroti.Framework.Painting.TextStyle? collapsedTextStyle => this._textTheme.titleLarge?.apply(color: this._colors.onSurface);
    public virtual global::Doroti.Framework.Painting.TextStyle? expandedTextStyle => this._textTheme.headlineSmall?.apply(color: this._colors.onSurface);
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry expandedTitlePadding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(new global::Doroti.Framework.Painting.EdgeInsets(16, 0, 16, 20));
}

internal class _LargeScrollUnderFlexibleConfig__app_bar : _ScrollUnderFlexibleConfig__app_bar
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__theme_initialized;
    private ThemeData __late__theme = default!;
    internal virtual ThemeData _theme
    {
        get
        {
            if (!__late__theme_initialized)
            {
                __late__theme = Theme.of(this.context);
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
                __late__colors = this._theme.colorScheme;
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
                __late__textTheme = this._theme.textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }
    public const double collapsedHeight = 64.0;
    public const double expandedHeight = 152.0;

    internal _LargeScrollUnderFlexibleConfig__app_bar(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public virtual global::Doroti.Framework.Painting.TextStyle? collapsedTextStyle => this._textTheme.titleLarge?.apply(color: this._colors.onSurface);
    public virtual global::Doroti.Framework.Painting.TextStyle? expandedTextStyle => this._textTheme.headlineMedium?.apply(color: this._colors.onSurface);
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry expandedTitlePadding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(new global::Doroti.Framework.Painting.EdgeInsets(16, 0, 16, 28));
}
