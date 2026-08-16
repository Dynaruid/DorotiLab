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
        ScaffoldState? scaffoldState__33726 = Scaffold.maybeOf(this.context);
        if (((scaffoldState__33726 is not null) && ((scaffoldState__33726.isDrawerOpen || scaffoldState__33726.isEndDrawerOpen))))
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
            bool oldScrolledUnder__34458 = this._scrolledUnder;
            global::Doroti.Framework.Widgets.ScrollMetrics metrics__34519 = ((global::Doroti.Framework.Widgets.ScrollUpdateNotification)notification__as34351).metrics;
            switch (((global::Doroti.Framework.Widgets.ScrollMetrics)metrics__34519).axisDirection)
            {
                case global::Doroti.Framework.Painting.AxisDirection.up:
                    {
                        _scrolledUnder = (((global::Doroti.Framework.Widgets.ScrollMetrics)metrics__34519).extentAfter > 0L);
                        break;
                    }
                case global::Doroti.Framework.Painting.AxisDirection.down:
                    {
                        _scrolledUnder = (((global::Doroti.Framework.Widgets.ScrollMetrics)metrics__34519).extentBefore > 0L);
                        break;
                    }
                case global::Doroti.Framework.Painting.AxisDirection.right:
                case global::Doroti.Framework.Painting.AxisDirection.left:
                    {
                        break;
                    }
            }
            if ((this._scrolledUnder != oldScrolledUnder__34458))
            {
                setState(((global::System.Action)(() => {
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
        global::Doroti.Framework.Services.SystemUiOverlayStyle style__35755 = ((object.Equals(brightness, Brightness.dark)) ? global::Doroti.Framework.Services.SystemUiOverlayStyle.light : global::Doroti.Framework.Services.SystemUiOverlayStyle.dark);
        return new global::Doroti.Framework.Services.SystemUiOverlayStyle(statusBarColor: backgroundColor, statusBarBrightness: ((global::Doroti.Framework.Services.SystemUiOverlayStyle)style__35755).statusBarBrightness, statusBarIconBrightness: ((global::Doroti.Framework.Services.SystemUiOverlayStyle)style__35755).statusBarIconBrightness, systemStatusBarContrastEnforced: ((global::Doroti.Framework.Services.SystemUiOverlayStyle)style__35755).systemStatusBarContrastEnforced);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => (!((AppBar)this.widget).primary || global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context)));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        ThemeData theme__36437 = Theme.of(context);
        IconButtonThemeData iconButtonTheme__36494 = IconButtonTheme.of(context);
        AppBarThemeData appBarTheme__36567 = AppBarTheme.of(context);
        AppBarThemeData defaults__36632 = (theme__36437.useMaterial3 ? new _AppBarDefaultsM3__app_bar(context) : new _AppBarDefaultsM2__app_bar(context));
        ScaffoldState? scaffold__36762 = Scaffold.maybeOf(context);
        dynamic parentRoute__36831 = global::Doroti.Framework.Widgets.ModalRoute<object>.of<object>(context);
        FlexibleSpaceBarSettings? settings__36906 = ((FlexibleSpaceBarSettings?)(object?)context.dependOnInheritedWidgetOfExactType<FlexibleSpaceBarSettings>());
        var states__37008 = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection37017 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if ((settings__36906?.isScrolledUnder ?? this._scrolledUnder)) { __collection37017.Add(global::Doroti.Framework.Widgets.WidgetState.scrolledUnder); } return __collection37017; }))();
        bool hasDrawer__37137 = (scaffold__36762?.hasDrawer ?? false);
        bool hasEndDrawer__37194 = (scaffold__36762?.hasEndDrawer ?? false);
        bool useCloseButton__37257 = (((bool?)((dynamic)parentRoute__36831)?.fullscreenDialog) ?? false);
        double toolbarHeight__37332 = ((((AppBar)this.widget).toolbarHeight ?? appBarTheme__36567.toolbarHeight) ?? ConstantsLibrary.kToolbarHeight);
        global::Doroti.Ui.Color backgroundColor__37442 = ((global::Doroti.Ui.Color)(object?)_resolveColor(states__37008, ((AppBar)this.widget).backgroundColor, appBarTheme__36567.backgroundColor, (defaults__36632.backgroundColor ?? Theme.of(context).colorScheme.surface)));
        global::Doroti.Ui.Color scrolledUnderBackground__37611 = ((global::Doroti.Ui.Color)(object?)_resolveColor(states__37008, ((AppBar)this.widget).backgroundColor, appBarTheme__36567.backgroundColor, Theme.of(context).colorScheme.surfaceContainer));
        var effectiveBackgroundColor__37803 = (states__37008.Contains(global::Doroti.Framework.Widgets.WidgetState.scrolledUnder) ? scrolledUnderBackground__37611 : backgroundColor__37442);
        global::Doroti.Ui.Color foregroundColor__37951 = ((global::Doroti.Ui.Color)(object?)((((AppBar)this.widget).foregroundColor ?? appBarTheme__36567.foregroundColor) ?? defaults__36632.foregroundColor!));
        double elevation__38079 = ((((AppBar)this.widget).elevation ?? appBarTheme__36567.elevation) ?? DartRuntimePrimitives.RequireValue(defaults__36632.elevation));
        double effectiveElevation__38175 = (states__37008.Contains(global::Doroti.Framework.Widgets.WidgetState.scrolledUnder) ? (((((AppBar)this.widget).scrolledUnderElevation ?? appBarTheme__36567.scrolledUnderElevation) ?? defaults__36632.scrolledUnderElevation) ?? elevation__38079) : elevation__38079);
        global::Doroti.Framework.Widgets.IconThemeData overallIconTheme__38447 = (((((AppBar)this.widget).iconTheme ?? appBarTheme__36567.iconTheme) ?? (global::Doroti.Framework.Widgets.IconThemeData)defaults__36632.iconTheme!.copyWith(color: foregroundColor__37951)));
        global::Doroti.Ui.Color? actionForegroundColor__38607 = ((global::Doroti.Ui.Color?)(object?)(((AppBar)this.widget).foregroundColor ?? appBarTheme__36567.foregroundColor));
        global::Doroti.Framework.Widgets.IconThemeData actionsIconTheme__38704 = ((((((((AppBar)this.widget).actionsIconTheme ?? appBarTheme__36567.actionsIconTheme) ?? ((AppBar)this.widget).iconTheme) ?? appBarTheme__36567.iconTheme) ?? (global::Doroti.Framework.Widgets.IconThemeData)defaults__36632.actionsIconTheme?.copyWith(color: actionForegroundColor__38607))) ?? overallIconTheme__38447);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry actionsPadding__38992 = ((((AppBar)this.widget).actionsPadding ?? appBarTheme__36567.actionsPadding) ?? defaults__36632.actionsPadding!);
        global::Doroti.Framework.Painting.TextStyle? toolbarTextStyle__39114 = (((((AppBar)this.widget).toolbarTextStyle ?? appBarTheme__36567.toolbarTextStyle) ?? (global::Doroti.Framework.Painting.TextStyle)defaults__36632.toolbarTextStyle?.copyWith(color: foregroundColor__37951)));
        global::Doroti.Framework.Painting.TextStyle? titleTextStyle__39293 = (((((AppBar)this.widget).titleTextStyle ?? appBarTheme__36567.titleTextStyle) ?? (global::Doroti.Framework.Painting.TextStyle)defaults__36632.titleTextStyle?.copyWith(color: foregroundColor__37951)));
        if ((((AppBar)this.widget).toolbarOpacity != 1.0))
        {
            double opacity__39508 = new global::Doroti.Framework.Animation.Interval(0.25, 1.0, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn).transform(((AppBar)this.widget).toolbarOpacity);
            if ((titleTextStyle__39293?.color is not null))
            {
                titleTextStyle__39293 = titleTextStyle__39293!.copyWith(color: ((global::Doroti.Framework.Painting.TextStyle)titleTextStyle__39293).color!.withOpacity(opacity__39508));
            }
            if ((toolbarTextStyle__39114?.color is not null))
            {
                toolbarTextStyle__39114 = toolbarTextStyle__39114!.copyWith(color: ((global::Doroti.Framework.Painting.TextStyle)toolbarTextStyle__39114).color!.withOpacity(opacity__39508));
            }
            overallIconTheme__38447 = overallIconTheme__38447.copyWith(opacity: (opacity__39508 * ((((global::Doroti.Framework.Widgets.IconThemeData)overallIconTheme__38447).opacity ?? 1.0))));
            actionsIconTheme__38704 = actionsIconTheme__38704.copyWith(opacity: (opacity__39508 * ((((global::Doroti.Framework.Widgets.IconThemeData)actionsIconTheme__38704).opacity ?? 1.0))));
        }
        global::Doroti.Framework.Widgets.Widget? leading__40261 = ((AppBar)this.widget).leading;
        if (((leading__40261 is null) && ((AppBar)this.widget).automaticallyImplyLeading))
        {
            if (hasDrawer__37137)
            {
                leading__40261 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new DrawerButton(style: IconButton.styleFrom(iconSize: (((global::Doroti.Framework.Widgets.IconThemeData)overallIconTheme__38447).size ?? 24))));
            }
            else
            {
                if ((((bool?)((dynamic)parentRoute__36831)?.impliesAppBarDismissal) ?? false))
                {
                    leading__40261 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>((useCloseButton__37257 ? new CloseButton() : new BackButton()));
                }
            }
        }
        if ((leading__40261 is not null))
        {
            if (theme__36437.useMaterial3)
            {
                IconButtonThemeData effectiveIconButtonTheme__40722 = default!;
                if ((object.Equals(overallIconTheme__38447, defaults__36632.iconTheme)))
                {
                    effectiveIconButtonTheme__40722 = iconButtonTheme__36494;
                }
                else
                {
                    ButtonStyle leadingIconButtonStyle__41450 = ((ButtonStyle)(object?)IconButton.styleFrom(foregroundColor: ((global::Doroti.Framework.Widgets.IconThemeData)overallIconTheme__38447).color, iconSize: ((global::Doroti.Framework.Widgets.IconThemeData)overallIconTheme__38447).size));
                    effectiveIconButtonTheme__40722 = new IconButtonThemeData(style: iconButtonTheme__36494.style?.copyWith(foregroundColor: leadingIconButtonStyle__41450.foregroundColor, overlayColor: leadingIconButtonStyle__41450.overlayColor, iconSize: leadingIconButtonStyle__41450.iconSize));
                }
                leading__40261 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new IconButtonTheme(data: effectiveIconButtonTheme__40722, child: ((leading__40261 is IconButton) ? new global::Doroti.Framework.Widgets.Center(child: ((IconButton)leading__40261)) : leading__40261)));
                leading__40261 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: global::Doroti.Framework.Rendering.BoxConstraints.CreateTightFor(width: ((((AppBar)this.widget).leadingWidth ?? appBarTheme__36567.leadingWidth) ?? App_barLibrary._kLeadingWidth)), child: leading__40261));
            }
            else
            {
                leading__40261 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: global::Doroti.Framework.Rendering.BoxConstraints.CreateTightFor(width: ((((AppBar)this.widget).leadingWidth ?? appBarTheme__36567.leadingWidth) ?? App_barLibrary._kLeadingWidth)), child: leading__40261));
            }
        }
        global::Doroti.Framework.Widgets.Widget? title__42825 = ((AppBar)this.widget).title;
        if ((title__42825 is not null))
        {
            title__42825 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _AppBarTitleBox__app_bar(child: title__42825));
            if (!((AppBar)this.widget).excludeHeaderSemantics)
            {
                title__42825 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(namesRoute: (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform switch { global::Doroti.Framework.Foundation.TargetPlatform.android or global::Doroti.Framework.Foundation.TargetPlatform.fuchsia or global::Doroti.Framework.Foundation.TargetPlatform.linux => true, global::Doroti.Framework.Foundation.TargetPlatform.windows => true, global::Doroti.Framework.Foundation.TargetPlatform.iOS => DartRuntimePrimitives.ConvertValue<bool>(null), global::Doroti.Framework.Foundation.TargetPlatform.macOS => DartRuntimePrimitives.ConvertValue<bool>(null), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }), header: true, child: title__42825));
            }
            title__42825 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.DefaultTextStyle(style: titleTextStyle__39293!, softWrap: false, overflow: global::Doroti.Framework.Painting.TextOverflow.ellipsis, child: title__42825));
            title__42825 = MediaQuery.withClampedTextScaling(maxScaleFactor: App_barLibrary._kMaxTitleTextScaleFactor, child: title__42825);
        }
        global::Doroti.Framework.Widgets.Widget? actions__43926 = default!;
        if (((((AppBar)this.widget).actions is not null) && System.Linq.Enumerable.Any(((AppBar)this.widget).actions!)))
        {
            actions__43926 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: actionsPadding__38992, child: new global::Doroti.Framework.Widgets.Row(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: (theme__36437.useMaterial3 ? global::Doroti.Framework.Rendering.CrossAxisAlignment.center : global::Doroti.Framework.Rendering.CrossAxisAlignment.stretch), children: ((AppBar)this.widget).actions!)));
        }
        else
        {
            if ((hasEndDrawer__37194 && ((AppBar)this.widget).automaticallyImplyActions))
            {
                actions__43926 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new EndDrawerButton(style: IconButton.styleFrom(iconSize: (((global::Doroti.Framework.Widgets.IconThemeData)overallIconTheme__38447).size ?? 24))));
            }
        }
        if ((actions__43926 is not null))
        {
            IconButtonThemeData effectiveActionsIconButtonTheme__44617 = default!;
            if ((object.Equals(actionsIconTheme__38704, defaults__36632.actionsIconTheme)))
            {
                effectiveActionsIconButtonTheme__44617 = iconButtonTheme__36494;
            }
            else
            {
                ButtonStyle actionsIconButtonStyle__44809 = ((ButtonStyle)(object?)IconButton.styleFrom(foregroundColor: ((global::Doroti.Framework.Widgets.IconThemeData)actionsIconTheme__38704).color, iconSize: ((global::Doroti.Framework.Widgets.IconThemeData)actionsIconTheme__38704).size));
                effectiveActionsIconButtonTheme__44617 = new IconButtonThemeData(style: iconButtonTheme__36494.style?.copyWith(foregroundColor: actionsIconButtonStyle__44809.foregroundColor, overlayColor: actionsIconButtonStyle__44809.overlayColor, iconSize: actionsIconButtonStyle__44809.iconSize));
            }
            actions__43926 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new IconButtonTheme(data: effectiveActionsIconButtonTheme__44617, child: IconTheme.merge(data: actionsIconTheme__38704, child: actions__43926)));
        }
        global::Doroti.Framework.Widgets.Widget toolbar__45480 = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.NavigationToolbar(leading: leading__40261, middle: title__42825, trailing: actions__43926, centerMiddle: this.widget._getEffectiveCenterTitle(theme__36437, appBarTheme__36567), middleSpacing: ((((AppBar)this.widget).titleSpacing ?? appBarTheme__36567.titleSpacing) ?? global::Doroti.Framework.Widgets.NavigationToolbar.kMiddleSpacing)));
        global::Doroti.Framework.Widgets.Widget appBar__45918 = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ClipRect(clipBehavior: (((AppBar)this.widget).clipBehavior ?? Clip.hardEdge), child: new global::Doroti.Framework.Widgets.CustomSingleChildLayout(@delegate: new _ToolbarContainerLayout__app_bar(toolbarHeight__37332), child: IconTheme.merge(data: overallIconTheme__38447, child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: toolbarTextStyle__39114!, child: toolbar__45480)))));
        if ((((AppBar)this.widget).bottom is not null))
        {
            appBar__45918 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Column(mainAxisAlignment: global::Doroti.Framework.Rendering.MainAxisAlignment.spaceBetween, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection46394 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection46394.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(maxHeight: toolbarHeight__37332), child: appBar__45918)))); if ((((AppBar)this.widget).bottomOpacity == 1.0)) { __collection46394.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(((AppBar)this.widget).bottom!)); } else { __collection46394.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Opacity(opacity: new global::Doroti.Framework.Animation.Interval(0.25, 1.0, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn).transform(((AppBar)this.widget).bottomOpacity), child: DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(((AppBar)this.widget).bottom)))); } return __collection46394; }))()));
        }
        if (((AppBar)this.widget).primary)
        {
            appBar__45918 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SafeArea(bottom: false, child: appBar__45918));
        }
        appBar__45918 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.Alignment.topCenter, child: appBar__45918));
        if ((((AppBar)this.widget).flexibleSpace is not null))
        {
            appBar__45918 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Stack(fit: global::Doroti.Framework.Rendering.StackFit.passthrough, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(sortKey: (((AppBar)this.widget).useDefaultSemanticsOrder ? new global::Doroti.Framework.Semantics.OrdinalSortKey(1.0) : null), explicitChildNodes: true, child: ((AppBar)this.widget).flexibleSpace)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(sortKey: (((AppBar)this.widget).useDefaultSemanticsOrder ? new global::Doroti.Framework.Semantics.OrdinalSortKey(0.0) : null), explicitChildNodes: true, child: new Material(type: MaterialType.transparency, child: appBar__45918))) }));
        }
        global::Doroti.Framework.Services.SystemUiOverlayStyle overlayStyle__47948 = ((((((AppBar)this.widget).systemOverlayStyle ?? appBarTheme__36567.systemOverlayStyle) ?? defaults__36632.systemOverlayStyle) ?? (global::Doroti.Framework.Services.SystemUiOverlayStyle)_systemOverlayStyleForBrightness(ThemeData.estimateBrightnessForColor(effectiveBackgroundColor__37803), (theme__36437.useMaterial3 ? new global::Doroti.Ui.Color(0L) : null))));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(container: true, child: new global::Doroti.Framework.Widgets.AnnotatedRegion<global::Doroti.Framework.Services.SystemUiOverlayStyle>(value: overlayStyle__47948, child: new Material(color: (theme__36437.useMaterial3 ? effectiveBackgroundColor__37803 : backgroundColor__37442), elevation: effectiveElevation__38175, type: (((AppBar)this.widget).forceMaterialTransparency ? MaterialType.transparency : MaterialType.canvas), shadowColor: ((((AppBar)this.widget).shadowColor ?? appBarTheme__36567.shadowColor) ?? defaults__36632.shadowColor), surfaceTintColor: ((((AppBar)this.widget).surfaceTintColor ?? appBarTheme__36567.surfaceTintColor) ?? ((theme__36437.useMaterial3 ? theme__36437.colorScheme.surfaceTint : null))), shape: ((((AppBar)this.widget).shape ?? appBarTheme__36567.shape) ?? defaults__36632.shape), animateColor: ((AppBar)this.widget).animateColor, child: new global::Doroti.Framework.Widgets.Semantics(explicitChildNodes: true, child: appBar__45918)))));
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
        double visibleMainHeight__52927 = ((this.maxExtent - shrinkOffset) - this.topPadding);
        double extraToolbarHeight__53003 = Math.Max((((this.minExtent - this._bottomHeight) - this.topPadding) - ((this.toolbarHeight ?? ConstantsLibrary.kToolbarHeight))), 0.0);
        double visibleToolbarHeight__53151 = ((visibleMainHeight__52927 - this._bottomHeight) - extraToolbarHeight__53003);
        bool isScrolledUnder__53246 = ((overlapsContent || this.forceElevated) || ((this.pinned && (shrinkOffset > (this.maxExtent - this.minExtent)))));
        bool isPinnedWithOpacityFade__53373 = (((this.pinned && this.floating) && (this.bottom is not null)) && (extraToolbarHeight__53003 == 0.0));
        double toolbarOpacity__53491 = ((!this.accessibleNavigation && ((!this.pinned || isPinnedWithOpacityFade__53373))) ? Dart_uiLibrary.clampDouble((visibleToolbarHeight__53151 / ((this.toolbarHeight ?? ConstantsLibrary.kToolbarHeight))), 0.0, 1.0) : 1.0);
        global::Doroti.Framework.Widgets.Widget? effectiveTitle__53693 = (this.variant switch { _SliverAppVariant__app_bar.small => this.title, _SliverAppVariant__app_bar.medium => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.AnimatedOpacity(opacity: (isScrolledUnder__53246 ? 1 : 0), duration: Duration.Create(milliseconds: 500L), curve: new global::Doroti.Framework.Animation.Cubic(0.2, 0.0, 0.0, 1.0), child: this.title)), _SliverAppVariant__app_bar.large => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.AnimatedOpacity(opacity: (isScrolledUnder__53246 ? 1 : 0), duration: Duration.Create(milliseconds: 500L), curve: new global::Doroti.Framework.Animation.Cubic(0.2, 0.0, 0.0, 1.0), child: this.title)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Framework.Widgets.Widget appBar__54046 = ((global::Doroti.Framework.Widgets.Widget)(object?)FlexibleSpaceBar.createSettings(minExtent: this.minExtent, maxExtent: this.maxExtent, currentExtent: Math.Max(this.minExtent, (this.maxExtent - shrinkOffset)), toolbarOpacity: toolbarOpacity__53491, isScrolledUnder: isScrolledUnder__53246, hasLeading: ((this.leading is not null) || this.automaticallyImplyLeading), child: new AppBar(clipBehavior: this.clipBehavior, leading: this.leading, automaticallyImplyLeading: this.automaticallyImplyLeading, title: effectiveTitle__53693, actions: this.actions, automaticallyImplyActions: this.automaticallyImplyActions, flexibleSpace: (((((this.title is null) && (this.flexibleSpace is not null)) && !this.excludeHeaderSemantics)) ? new global::Doroti.Framework.Widgets.Semantics(header: true, child: this.flexibleSpace) : this.flexibleSpace), bottom: this.bottom, elevation: (isScrolledUnder__53246 ? this.elevation : 0.0), scrolledUnderElevation: this.scrolledUnderElevation, shadowColor: this.shadowColor, surfaceTintColor: this.surfaceTintColor, backgroundColor: this.backgroundColor, foregroundColor: this.foregroundColor, iconTheme: this.iconTheme, actionsIconTheme: this.actionsIconTheme, primary: this.primary, centerTitle: this.centerTitle, excludeHeaderSemantics: this.excludeHeaderSemantics, titleSpacing: this.titleSpacing, shape: this.shape, toolbarOpacity: toolbarOpacity__53491, bottomOpacity: (this.pinned ? 1.0 : Dart_uiLibrary.clampDouble((visibleMainHeight__52927 / this._bottomHeight), 0.0, 1.0)), toolbarHeight: this.toolbarHeight, leadingWidth: this.leadingWidth, toolbarTextStyle: this.toolbarTextStyle, titleTextStyle: this.titleTextStyle, systemOverlayStyle: this.systemOverlayStyle, forceMaterialTransparency: this.forceMaterialTransparency, useDefaultSemanticsOrder: this.useDefaultSemanticsOrder, actionsPadding: this.actionsPadding)));
        return appBar__54046;
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
        double bottomHeight__80554 = (((SliverAppBar)this.widget).bottom?.preferredSize.height ?? 0.0);
        double topPadding__80630 = (((SliverAppBar)this.widget).primary ? MediaQuery.paddingOf(context).top : 0.0);
        double collapsedHeight__80718 = ((((((SliverAppBar)this.widget).pinned && ((SliverAppBar)this.widget).floating) && (((SliverAppBar)this.widget).bottom is not null))) ? ((((((SliverAppBar)this.widget).collapsedHeight ?? 0.0)) + bottomHeight__80554) + topPadding__80630) : ((((((SliverAppBar)this.widget).collapsedHeight ?? ((SliverAppBar)this.widget).toolbarHeight)) + bottomHeight__80554) + topPadding__80630));
        double? effectiveExpandedHeight__80972 = default!;
        double effectiveCollapsedHeight__81014 = default!;
        global::Doroti.Framework.Widgets.Widget? effectiveFlexibleSpace__81058 = default!;
        switch (((SliverAppBar)this.widget)._variant)
        {
            case _SliverAppVariant__app_bar.small:
                {
                    effectiveExpandedHeight__80972 = ((SliverAppBar)this.widget).expandedHeight;
                    effectiveCollapsedHeight__81014 = collapsedHeight__80718;
                    effectiveFlexibleSpace__81058 = ((SliverAppBar)this.widget).flexibleSpace;
                    break;
                }
            case _SliverAppVariant__app_bar.medium:
                {
                    effectiveExpandedHeight__80972 = (((SliverAppBar)this.widget).expandedHeight ?? (_MediumScrollUnderFlexibleConfig__app_bar.expandedHeight + bottomHeight__80554));
                    effectiveCollapsedHeight__81014 = (((SliverAppBar)this.widget).collapsedHeight ?? ((topPadding__80630 + _MediumScrollUnderFlexibleConfig__app_bar.collapsedHeight) + bottomHeight__80554));
                    effectiveFlexibleSpace__81058 = (((SliverAppBar)this.widget).flexibleSpace ?? new _ScrollUnderFlexibleSpace__app_bar(title: ((SliverAppBar)this.widget).title, foregroundColor: ((SliverAppBar)this.widget).foregroundColor, configBuilder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, _MediumScrollUnderFlexibleConfig__app_bar>)((arg0) => new _MediumScrollUnderFlexibleConfig__app_bar(arg0))), titleTextStyle: ((SliverAppBar)this.widget).titleTextStyle, bottomHeight: bottomHeight__80554));
                    break;
                }
            case _SliverAppVariant__app_bar.large:
                {
                    effectiveExpandedHeight__80972 = (((SliverAppBar)this.widget).expandedHeight ?? (_LargeScrollUnderFlexibleConfig__app_bar.expandedHeight + bottomHeight__80554));
                    effectiveCollapsedHeight__81014 = (((SliverAppBar)this.widget).collapsedHeight ?? ((topPadding__80630 + _LargeScrollUnderFlexibleConfig__app_bar.collapsedHeight) + bottomHeight__80554));
                    effectiveFlexibleSpace__81058 = (((SliverAppBar)this.widget).flexibleSpace ?? new _ScrollUnderFlexibleSpace__app_bar(title: ((SliverAppBar)this.widget).title, foregroundColor: ((SliverAppBar)this.widget).foregroundColor, configBuilder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, _LargeScrollUnderFlexibleConfig__app_bar>)((arg0) => new _LargeScrollUnderFlexibleConfig__app_bar(arg0))), titleTextStyle: ((SliverAppBar)this.widget).titleTextStyle, bottomHeight: bottomHeight__80554));
                    break;
                }
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.MediaQuery.CreateRemovePadding(context: context, removeBottom: true, child: new global::Doroti.Framework.Widgets.SliverPersistentHeader(floating: ((SliverAppBar)this.widget).floating, pinned: ((SliverAppBar)this.widget).pinned, @delegate: new _SliverAppBarDelegate__app_bar(vsync: this, leading: ((SliverAppBar)this.widget).leading, automaticallyImplyLeading: ((SliverAppBar)this.widget).automaticallyImplyLeading, title: ((SliverAppBar)this.widget).title, actions: ((SliverAppBar)this.widget).actions, automaticallyImplyActions: ((SliverAppBar)this.widget).automaticallyImplyActions, flexibleSpace: effectiveFlexibleSpace__81058, bottom: ((SliverAppBar)this.widget).bottom, elevation: ((SliverAppBar)this.widget).elevation, scrolledUnderElevation: ((SliverAppBar)this.widget).scrolledUnderElevation, shadowColor: ((SliverAppBar)this.widget).shadowColor, surfaceTintColor: ((SliverAppBar)this.widget).surfaceTintColor, forceElevated: ((SliverAppBar)this.widget).forceElevated, backgroundColor: ((SliverAppBar)this.widget).backgroundColor, foregroundColor: ((SliverAppBar)this.widget).foregroundColor, iconTheme: ((SliverAppBar)this.widget).iconTheme, actionsIconTheme: ((SliverAppBar)this.widget).actionsIconTheme, primary: ((SliverAppBar)this.widget).primary, centerTitle: ((SliverAppBar)this.widget).centerTitle, excludeHeaderSemantics: ((SliverAppBar)this.widget).excludeHeaderSemantics, titleSpacing: ((SliverAppBar)this.widget).titleSpacing, expandedHeight: effectiveExpandedHeight__80972, collapsedHeight: effectiveCollapsedHeight__81014, topPadding: topPadding__80630, floating: ((SliverAppBar)this.widget).floating, pinned: ((SliverAppBar)this.widget).pinned, shape: ((SliverAppBar)this.widget).shape, snapConfiguration: this._snapConfiguration, stretchConfiguration: this._stretchConfiguration, showOnScreenConfiguration: this._showOnScreenConfiguration, toolbarHeight: ((SliverAppBar)this.widget).toolbarHeight, leadingWidth: ((SliverAppBar)this.widget).leadingWidth, toolbarTextStyle: ((SliverAppBar)this.widget).toolbarTextStyle, titleTextStyle: ((SliverAppBar)this.widget).titleTextStyle, systemOverlayStyle: ((SliverAppBar)this.widget).systemOverlayStyle, forceMaterialTransparency: ((SliverAppBar)this.widget).forceMaterialTransparency, useDefaultSemanticsOrder: ((SliverAppBar)this.widget).useDefaultSemanticsOrder, clipBehavior: ((SliverAppBar)this.widget).clipBehavior, variant: ((SliverAppBar)this.widget)._variant, accessibleNavigation: MediaQuery.of(context).accessibleNavigation, actionsPadding: ((SliverAppBar)this.widget).actionsPadding))));
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
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{            var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
            __cascade.muted = !((TickerModeData)values__17506).enabled;
            __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
            return __cascade;        }))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result__17553);
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
            TickerModeData values__18318 = this._tickerModeNotifier!.value;
            bool muted__18372 = !((TickerModeData)values__18318).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker__18989 in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker__18989).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker__18989.describeForError("The offending ticker was") }));
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
        global::Doroti.Framework.Rendering.BoxConstraints innerConstraints__85853 = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)constraints.copyWith(maxHeight: double.PositiveInfinity));
        global::Doroti.Ui.Size childSize__85937 = ((global::Doroti.Ui.Size)(object?)this.child!.getDryLayout(innerConstraints__85853));
        return constraints.constrain(childSize__85937);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Framework.Rendering.BoxConstraints innerConstraints__86167 = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)constraints.copyWith(maxHeight: double.PositiveInfinity));
        global::Doroti.Framework.Rendering.RenderBox? child__86257 = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((child__86257 is null))
        {
            return null;
        }
        double? result__86345 = child__86257.getDryBaseline(innerConstraints__86167, baseline);
        if ((result__86345 is null))
        {
            return null;
        }
        global::Doroti.Ui.Size childSize__86470 = ((global::Doroti.Ui.Size)(object?)child__86257.getDryLayout(innerConstraints__86167));
        return (DartRuntimePrimitives.RequireValue(result__86345) + this.resolvedAlignment.alongOffset((getDryLayout(constraints) - childSize__86470)).dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Framework.Rendering.BoxConstraints innerConstraints__86698 = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)this.constraints.copyWith(maxHeight: double.PositiveInfinity));
        this.child!.layout(innerConstraints__86698, parentUsesSize: true);
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
        AppBarThemeData appBarTheme__87379 = AppBarTheme.of(context);
        AppBarThemeData defaults__87449 = (Theme.of(context).useMaterial3 ? new _AppBarDefaultsM3__app_bar(context) : new _AppBarDefaultsM2__app_bar(context));
        FlexibleSpaceBarSettings settings__87601 = context.dependOnInheritedWidgetOfExactType<FlexibleSpaceBarSettings>()!;
        _ScrollUnderFlexibleConfig__app_bar config__87731 = this.configBuilder(context);
        DartRuntimePrimitives.Assert(() => ((_ScrollUnderFlexibleConfig__app_bar)config__87731).expandedTitlePadding.isNonNegative, () => (object?)"The _ExpandedTitleWithPadding widget assumes that the expanded title padding is non-negative. " + "Update its implementation to handle negative padding.");
        global::Doroti.Framework.Painting.TextStyle? expandedTextStyle__88020 = (((this.titleTextStyle ?? appBarTheme__87379.titleTextStyle) ?? (global::Doroti.Framework.Painting.TextStyle)((_ScrollUnderFlexibleConfig__app_bar)config__87731).expandedTextStyle?.copyWith(color: ((this.foregroundColor ?? appBarTheme__87379.foregroundColor) ?? defaults__87449.foregroundColor))));
        global::Doroti.Framework.Widgets.Widget? expandedTitle__88271 = ((this.title, expandedTextStyle__88020) switch { (null, _) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(null), (global::Doroti.Framework.Widgets.Widget title__88370, null) => title__88370, (global::Doroti.Framework.Widgets.Widget title__88413, global::Doroti.Framework.Painting.TextStyle textStyle__88436) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.DefaultTextStyle(style: textStyle__88436, child: title__88413)) });
        global::Doroti.Framework.Painting.EdgeInsets resolvedTitlePadding__88554 = ((global::Doroti.Framework.Painting.EdgeInsets)(object?)((_ScrollUnderFlexibleConfig__app_bar)config__87731).expandedTitlePadding.resolve(Directionality.of(context)));
        global::Doroti.Framework.Painting.EdgeInsetsGeometry expandedTitlePadding__88684 = ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)(object?)((this.bottomHeight > 0L) ? resolvedTitlePadding__88554.copyWith(bottom: 0) : resolvedTitlePadding__88554));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)MediaQuery.withClampedTextScaling(maxScaleFactor: App_barLibrary._kMaxTitleTextScaleFactor, child: new global::Doroti.Framework.Widgets.Column(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection89388 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection89388.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(top: (((FlexibleSpaceBarSettings)settings__87601).minExtent - this.bottomHeight))))); __collection89388.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: new global::Doroti.Framework.Widgets.ClipRect(child: new _ExpandedTitleWithPadding__app_bar(padding: expandedTitlePadding__88684, maxExtent: (((FlexibleSpaceBarSettings)settings__87601).maxExtent - ((FlexibleSpaceBarSettings)settings__87601).minExtent), child: expandedTitle__88271))))); if ((this.bottomHeight > 0L)) { __collection89388.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(bottom: this.bottomHeight)))); } return __collection89388; }))())));
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
        global::Doroti.Ui.TextDirection textDirection__90771 = Directionality.of(context);
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderExpandedTitleBox__app_bar(this.padding.resolve(textDirection__90771), global::Doroti.Framework.Painting.AlignmentDirectional.bottomStart.resolve(textDirection__90771), this.maxExtent, null));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderExpandedTitleBox__app_bar)(object)renderObject;
        global::Doroti.Ui.TextDirection textDirection__91117 = Directionality.of(context);
        DartRuntimePrimitives.Ignore(((Func<_RenderExpandedTitleBox__app_bar>)(() =>
{            var __cascade = __renderObject;
            __cascade.padding = this.padding.resolve(textDirection__91117);
            __cascade.titleAlignment = global::Doroti.Framework.Painting.AlignmentDirectional.bottomStart.resolve(textDirection__91117);
            __cascade.maxExtent = this.maxExtent;
            return __cascade;        }))());
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
        global::Doroti.Framework.Rendering.RenderBox? child__92236 = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        return ((child__92236 is null) ? 0.0 : (child__92236.getMaxIntrinsicHeight(Math.Max(0, (width - this.padding.horizontal))) + this.padding.vertical));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        global::Doroti.Framework.Rendering.RenderBox? child__92483 = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        return ((child__92483 is null) ? 0.0 : (child__92483.getMaxIntrinsicWidth(double.PositiveInfinity) + this.padding.horizontal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        global::Doroti.Framework.Rendering.RenderBox? child__92691 = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        return ((child__92691 is null) ? 0.0 : (child__92691.getMinIntrinsicHeight(Math.Max(0, (width - this.padding.horizontal))) + this.padding.vertical));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        global::Doroti.Framework.Rendering.RenderBox? child__92938 = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        return ((child__92938 is null) ? 0.0 : (child__92938.getMinIntrinsicWidth(double.PositiveInfinity) + this.padding.horizontal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints) => ((this.child is null) ? Size.zero : ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).biggest);
    internal virtual global::Doroti.Ui.Offset _childOffsetFromSize(Size childSize, Size size)
    {
        DartRuntimePrimitives.Assert(() => (this.child is not null));
        DartRuntimePrimitives.Assert(() => this.padding.isNonNegative);
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Painting.Alignment)this.titleAlignment).y == 1.0));
        double yAdjustment__93766 = Dart_uiLibrary.clampDouble(((childSize.height + ((global::Doroti.Framework.Painting.EdgeInsets)this.padding).bottom) - this.maxExtent), 0, ((global::Doroti.Framework.Painting.EdgeInsets)this.padding).bottom);
        double offsetX__93901 = (((((((global::Doroti.Framework.Painting.Alignment)this.titleAlignment).x + 1L)) / 2L) * (((size.width - this.padding.horizontal) - childSize.width))) + ((global::Doroti.Framework.Painting.EdgeInsets)this.padding).left);
        double offsetY__94041 = (((size.height - childSize.height) - ((global::Doroti.Framework.Painting.EdgeInsets)this.padding).bottom) + yAdjustment__93766);
        return new global::Doroti.Ui.Offset(offsetX__93901, offsetY__94041);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Framework.Rendering.RenderBox? child__94281 = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((child__94281 is null))
        {
            return null;
        }
        global::Doroti.Framework.Rendering.BoxConstraints childConstraints__94376 = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)constraints.widthConstraints().deflate(this.padding));
        global::Doroti.Framework.Rendering.BaselineOffset result__94469 = (new global::Doroti.Framework.Rendering.BaselineOffset(child__94281.getDryBaseline(childConstraints__94376, baseline)).op_Add(_childOffsetFromSize(child__94281.getDryLayout(childConstraints__94376), getDryLayout(constraints)).dy));
        return result__94469.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Framework.Rendering.RenderBox? child__94740 = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((child__94740 is null))
        {
            size = ((global::Doroti.Framework.Rendering.BoxConstraints)this.constraints).smallest;
            return;
        }
        size = ((global::Doroti.Framework.Rendering.BoxConstraints)this.constraints).biggest;
        child__94740.layout(this.constraints.widthConstraints().deflate(this.padding), parentUsesSize: true);
        var childParentData__94971 = ((global::Doroti.Framework.Rendering.BoxParentData?)(object?)child__94740.parentData!)!;
        childParentData__94971.offset = _childOffsetFromSize(((global::Doroti.Framework.Rendering.RenderBox)child__94740).size, this.size);
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
