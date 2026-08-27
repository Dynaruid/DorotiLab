// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/snack_bar.dart
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

public static partial class Snack_barLibrary
{
    internal static double _singleLineVerticalPadding = 14.0;
}

public static partial class Snack_barLibrary
{
    internal static Duration _snackBarTransitionDuration = Duration.Create(milliseconds: 250L);
}

public static partial class Snack_barLibrary
{
    internal static Duration _snackBarDisplayDuration = Duration.Create(milliseconds: 4000L);
}

public static partial class Snack_barLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _snackBarHeightCurve = ((global::Doroti.Framework.Animation.Curve)(object?)global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
}

public static partial class Snack_barLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _snackBarM3HeightCurve = ((global::Doroti.Framework.Animation.Curve)(object?)global::Doroti.Framework.Animation.Curves.easeInOutQuart);
}

public static partial class Snack_barLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _snackBarFadeInCurve = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Interval(0.4, 1.0));
}

public static partial class Snack_barLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _snackBarM3FadeInCurve = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Interval(0.4, 0.6, curve: global::Doroti.Framework.Animation.Curves.easeInCirc));
}

public static partial class Snack_barLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _snackBarFadeOutCurve = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Interval(0.72, 1.0, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn));
}

public enum SnackBarClosedReason
{
    action,
    dismiss,
    swipe,
    hide,
    remove,
    timeout
}

public class SnackBarAction : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual Color? textColor { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? disabledTextColor { get; private set; }
    public virtual Color? disabledBackgroundColor { get; private set; }
    public virtual string label { get; private set; } = default!;
    public virtual global::System.Action onPressed { get; private set; } = default!;

    public SnackBarAction(global::Doroti.Framework.Foundation.Key? key = null, Color? textColor = null, Color? disabledTextColor = null, Color? backgroundColor = null, Color? disabledBackgroundColor = null, string label = default!, global::System.Action onPressed = default!) : base(key: key)
    {
        this.textColor = textColor;
        this.disabledTextColor = disabledTextColor;
        this.backgroundColor = backgroundColor;
        this.disabledBackgroundColor = disabledBackgroundColor;
        this.label = label;
        this.onPressed = onPressed;
        System.Diagnostics.Debug.Assert(((backgroundColor is not global::Doroti.Framework.Widgets.WidgetStateColor) || (disabledBackgroundColor is null)));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SnackBarActionState__snack_bar());
}

internal class _SnackBarActionState__snack_bar : global::Doroti.Framework.Widgets.State<SnackBarAction>
{
    internal virtual bool _haveTriggeredAction { get; set; } = false;

    internal virtual void _handlePressed()
    {
        if (this._haveTriggeredAction)
        {
            return;
        }
        setState(((global::System.Action)(() =>
        {
            _haveTriggeredAction = true;
        })));
        this.widget.onPressed();
        ScaffoldMessenger.of(this.context).hideCurrentSnackBar(reason: SnackBarClosedReason.action);
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        SnackBarThemeData defaults = (Theme.of(context).useMaterial3 ? new _SnackbarDefaultsM3__snack_bar(context) : new _SnackbarDefaultsM2__snack_bar(context));
        SnackBarThemeData snackBarTheme = SnackBarTheme.of(context);
        global::Doroti.Framework.Widgets.WidgetStateColor resolveForegroundColor()
        {
            if ((((SnackBarAction)this.widget).textColor is not null))
            {
                if ((((SnackBarAction)this.widget).textColor is global::Doroti.Framework.Widgets.WidgetStateColor))
                {
                    return ((global::Doroti.Framework.Widgets.WidgetStateColor?)(object?)((SnackBarAction)this.widget).textColor!)!;
                }
            }
            else
            {
                if ((snackBarTheme.actionTextColor is not null))
                {
                    if ((snackBarTheme.actionTextColor is global::Doroti.Framework.Widgets.WidgetStateColor))
                    {
                        return ((global::Doroti.Framework.Widgets.WidgetStateColor?)(object?)snackBarTheme.actionTextColor!)!;
                    }
                }
                else
                {
                    if ((defaults.actionTextColor is not null))
                    {
                        if ((defaults.actionTextColor is global::Doroti.Framework.Widgets.WidgetStateColor))
                        {
                            return ((global::Doroti.Framework.Widgets.WidgetStateColor?)(object?)defaults.actionTextColor!)!;
                        }
                    }
                }
            }
            return global::Doroti.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) =>
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
                {
                    return ((((SnackBarAction)this.widget).disabledTextColor ?? snackBarTheme.disabledActionTextColor) ?? defaults.disabledActionTextColor!);
                }
                return ((((SnackBarAction)this.widget).textColor ?? snackBarTheme.actionTextColor) ?? defaults.actionTextColor!);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::Doroti.Framework.Widgets.WidgetStateColor? resolveBackgroundColor()
        {
            if ((((SnackBarAction)this.widget).backgroundColor is global::Doroti.Framework.Widgets.WidgetStateColor))
            {
                return ((global::Doroti.Framework.Widgets.WidgetStateColor?)(object?)((SnackBarAction)this.widget).backgroundColor!)!;
            }
            if ((snackBarTheme.actionBackgroundColor is global::Doroti.Framework.Widgets.WidgetStateColor))
            {
                return ((global::Doroti.Framework.Widgets.WidgetStateColor?)(object?)snackBarTheme.actionBackgroundColor!)!;
            }
            return global::Doroti.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) =>
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
                {
                    return ((((SnackBarAction)this.widget).disabledBackgroundColor ?? snackBarTheme.disabledActionBackgroundColor) ?? Colors.transparent);
                }
                return ((((SnackBarAction)this.widget).backgroundColor ?? snackBarTheme.actionBackgroundColor) ?? Colors.transparent);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new TextButton(style: TextButton.styleFrom(overlayColor: resolveForegroundColor()).copyWith(foregroundColor: resolveForegroundColor(), backgroundColor: resolveBackgroundColor()), onPressed: ((global::System.Action)(this._haveTriggeredAction ? null : this._handlePressed)), child: new global::Doroti.Framework.Widgets.Text(((SnackBarAction)this.widget).label)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SnackBar : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget content { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual double? width { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual global::Doroti.Framework.Rendering.HitTestBehavior? hitTestBehavior { get; private set; }
    public virtual SnackBarBehavior? behavior { get; private set; }
    public virtual SnackBarAction? action { get; private set; }
    public virtual double? actionOverflowThreshold { get; private set; }
    public virtual bool? showCloseIcon { get; private set; }
    public virtual Color? closeIconColor { get; private set; }
    public virtual Duration duration { get; private set; } = default!;
    public virtual bool persist { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double>? animation { get; private set; }
    public virtual global::System.Action? onVisible { get; private set; }
    public virtual global::Doroti.Framework.Widgets.DismissDirection? dismissDirection { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;

    public SnackBar(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget content = default!, Color? backgroundColor = null, double? elevation = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, double? width = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Framework.Rendering.HitTestBehavior? hitTestBehavior = null, SnackBarBehavior? behavior = null, SnackBarAction? action = null, double? actionOverflowThreshold = null, bool? showCloseIcon = null, Color? closeIconColor = null, Duration? duration = null, bool? persist = null, global::Doroti.Framework.Animation.Animation<double>? animation = null, global::System.Action? onVisible = null, global::Doroti.Framework.Widgets.DismissDirection? dismissDirection = null, Clip clipBehavior = Clip.hardEdge) : base(key: key)
    {
        Duration __duration = duration ?? Snack_barLibrary._snackBarDisplayDuration;
        this.content = content;
        this.backgroundColor = backgroundColor;
        this.elevation = elevation;
        this.margin = margin;
        this.padding = padding;
        this.width = width;
        this.shape = shape;
        this.hitTestBehavior = hitTestBehavior;
        this.behavior = behavior;
        this.action = action;
        this.actionOverflowThreshold = actionOverflowThreshold;
        this.showCloseIcon = showCloseIcon;
        this.closeIconColor = closeIconColor;
        this.duration = __duration;
        this.animation = animation;
        this.onVisible = onVisible;
        this.dismissDirection = dismissDirection;
        this.clipBehavior = clipBehavior;
        this.persist = (persist ?? (action is not null));
        System.Diagnostics.Debug.Assert(((elevation is null) || (elevation >= 0.0)));
        System.Diagnostics.Debug.Assert(((width is null) || (margin is null)));
        System.Diagnostics.Debug.Assert(((actionOverflowThreshold is null) || (((actionOverflowThreshold >= 0L) && (actionOverflowThreshold <= 1L)))));
    }

    public static global::Doroti.Framework.Animation.AnimationController createAnimationController(global::Doroti.Framework.Scheduler.TickerProvider vsync, Duration? duration = null, Duration? reverseDuration = null)
    {
        return new global::Doroti.Framework.Animation.AnimationController(duration: (duration ?? Snack_barLibrary._snackBarTransitionDuration), reverseDuration: reverseDuration, debugLabel: "SnackBar", vsync: vsync);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SnackBar withAnimation(global::Doroti.Framework.Animation.Animation<double> newAnimation, global::Doroti.Framework.Foundation.Key? fallbackKey = null)
    {
        return new SnackBar(key: (this.key ?? fallbackKey), content: this.content, backgroundColor: this.backgroundColor, elevation: this.elevation, margin: this.margin, padding: this.padding, width: this.width, shape: this.shape, hitTestBehavior: this.hitTestBehavior, behavior: this.behavior, action: this.action, actionOverflowThreshold: this.actionOverflowThreshold, showCloseIcon: this.showCloseIcon, closeIconColor: this.closeIconColor, duration: DartRuntimePrimitives.RequireValue(this.duration), persist: this.persist, animation: newAnimation, onVisible: this.onVisible, dismissDirection: this.dismissDirection, clipBehavior: this.clipBehavior);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SnackBarState__snack_bar());
}

internal class _SnackBarState__snack_bar : global::Doroti.Framework.Widgets.State<SnackBar>
{
    internal virtual bool _wasVisible { get; set; } = false;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _heightAnimation { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _fadeInAnimation { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _fadeInM3Animation { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _fadeOutAnimation { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _heightM3Animation { get; set; } = default;
    internal virtual global::Doroti.Framework.Foundation.Key _dismissibleKey { get; private set; } = ((global::Doroti.Framework.Foundation.Key)(object?)new global::Doroti.Framework.Foundation.UniqueKey());

    public override void initState()
    {
        base.initState();
        ((SnackBar)this.widget).animation!.addStatusListener((AnimationStatusListener)this._onAnimationStatusChanged);
        _setAnimations();
    }

    public override void didUpdateWidget(SnackBar oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((SnackBar)this.widget).animation, ((SnackBar)oldWidget).animation)))
        {
            ((SnackBar)oldWidget).animation!.removeStatusListener((AnimationStatusListener)this._onAnimationStatusChanged);
            ((SnackBar)this.widget).animation!.addStatusListener((AnimationStatusListener)this._onAnimationStatusChanged);
            _disposeAnimations();
            _setAnimations();
        }
    }

    internal virtual void _setAnimations()
    {
        DartRuntimePrimitives.Assert(() => (((SnackBar)this.widget).animation is not null));
        _heightAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((SnackBar)this.widget).animation!, curve: Snack_barLibrary._snackBarHeightCurve);
        _fadeInAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((SnackBar)this.widget).animation!, curve: Snack_barLibrary._snackBarFadeInCurve);
        _fadeInM3Animation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((SnackBar)this.widget).animation!, curve: Snack_barLibrary._snackBarM3FadeInCurve);
        _fadeOutAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((SnackBar)this.widget).animation!, curve: Snack_barLibrary._snackBarFadeOutCurve, reverseCurve: new global::Doroti.Framework.Animation.Threshold(0.0));
        _heightM3Animation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((SnackBar)this.widget).animation!, curve: Snack_barLibrary._snackBarM3HeightCurve, reverseCurve: new global::Doroti.Framework.Animation.Threshold(0.0));
    }

    internal virtual void _disposeAnimations()
    {
        this._heightAnimation?.dispose();
        this._fadeInAnimation?.dispose();
        this._fadeInM3Animation?.dispose();
        this._fadeOutAnimation?.dispose();
        this._heightM3Animation?.dispose();
        _heightAnimation = null;
        _fadeInAnimation = null;
        _fadeInM3Animation = null;
        _fadeOutAnimation = null;
        _heightM3Animation = null;
    }

    public override void dispose()
    {
        ((SnackBar)this.widget).animation!.removeStatusListener((AnimationStatusListener)this._onAnimationStatusChanged);
        _disposeAnimations();
        base.dispose();
    }

    internal virtual void _onAnimationStatusChanged(global::Doroti.Framework.Animation.AnimationStatus animationStatus)
    {
        if (global::Doroti.Framework.Animation.AnimationStatusMembers.isCompleted(animationStatus))
        {
            if (((((SnackBar)this.widget).onVisible is not null) && !this._wasVisible))
            {
                ((SnackBar)this.widget).onVisible!();
            }
            _wasVisible = true;
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        bool accessibleNavigation = MediaQuery.accessibleNavigationOf(context);
        DartRuntimePrimitives.Assert(() => (((SnackBar)this.widget).animation is not null));
        ThemeData theme = Theme.of(context);
        ColorScheme colorSchemeLocal = theme.colorScheme;
        SnackBarThemeData snackBarTheme = SnackBarTheme.of(context);
        var isThemeDark = (object.Equals(theme.brightness, Brightness.dark));
        global::Doroti.Ui.Color buttonColor = ((global::Doroti.Ui.Color)(object?)(isThemeDark ? colorSchemeLocal.primary : colorSchemeLocal.secondary));
        SnackBarThemeData defaults = (theme.useMaterial3 ? new _SnackbarDefaultsM3__snack_bar(context) : new _SnackbarDefaultsM2__snack_bar(context));
        global::Doroti.Ui.Brightness brightnessLocal = (isThemeDark ? Brightness.light : Brightness.dark);
        ThemeData effectiveTheme = (theme.useMaterial3 ? theme : theme.copyWith(colorScheme: new ColorScheme(primary: colorSchemeLocal.onPrimary, secondary: buttonColor, surface: colorSchemeLocal.onSurface, background: defaults.backgroundColor, error: colorSchemeLocal.onError, onPrimary: colorSchemeLocal.primary, onSecondary: colorSchemeLocal.secondary, onSurface: colorSchemeLocal.surface, onBackground: colorSchemeLocal.background, onError: colorSchemeLocal.error, brightness: brightnessLocal)));
        global::Doroti.Framework.Painting.TextStyle? contentTextStyleLocal = (snackBarTheme.contentTextStyle ?? defaults.contentTextStyle);
        SnackBarBehavior snackBarBehavior = DartRuntimePrimitives.RequireValue(((SnackBar)this.widget).behavior ?? snackBarTheme.behavior ?? defaults.behavior);
        double? widthLocal = (((SnackBar)this.widget).width ?? snackBarTheme.width);
        DartRuntimePrimitives.Assert(() =>
            {
                if ((!object.Equals(snackBarBehavior, SnackBarBehavior.floating)))
                {
                    string message(string parameter)
                    {
                        var prefix = $"{parameter} can only be used with floating behavior.";
                        if ((((SnackBar)this.widget).behavior is not null))
                        {
                            return $"{prefix} SnackBarBehavior.fixed was set in the SnackBar constructor.";
                        }
                        else
                        {
                            if ((snackBarTheme.behavior is not null))
                            {
                                return $"{prefix} SnackBarBehavior.fixed was set by the inherited SnackBarThemeData.";
                            }
                            else
                            {
                                return $"{prefix} SnackBarBehavior.fixed was set by default.";
                            }
                        }
                        throw new InvalidOperationException("Dart control flow completed without a value.");
                    }
                    DartRuntimePrimitives.Assert(() => (((SnackBar)this.widget).margin is null), () => (object?)message("Margin"));
                    DartRuntimePrimitives.Assert(() => (widthLocal is null), () => (object?)message("Width"));
                }
                return true;
            });
        bool showCloseIconLocal = ((((SnackBar)this.widget).showCloseIcon ?? snackBarTheme.showCloseIcon) ?? DartRuntimePrimitives.RequireValue(defaults.showCloseIcon));
        var isFloatingSnackBar = (object.Equals(snackBarBehavior, SnackBarBehavior.floating));
        var horizontalPadding = (isFloatingSnackBar ? 16.0 : 24.0);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry paddingLocal = (((SnackBar)this.widget).padding ?? global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: horizontalPadding, end: (((((SnackBar)this.widget).action is not null) || showCloseIconLocal) ? 0 : horizontalPadding)));
        double actionHorizontalMargin = (((((SnackBar)this.widget).padding?.resolve(TextDirection.ltr).right ?? horizontalPadding)) / 2L);
        double iconHorizontalMargin = (((((SnackBar)this.widget).padding?.resolve(TextDirection.ltr).right ?? horizontalPadding)) / 12.0);
        IconButton? iconButton = (showCloseIconLocal ? new IconButton(key: StandardComponentTypeMembers.key(global::Doroti.Framework.Widgets.StandardComponentType.closeButton), icon: new global::Doroti.Framework.Widgets.Icon(Icons.close), iconSize: 24.0, color: ((((SnackBar)this.widget).closeIconColor ?? snackBarTheme.closeIconColor) ?? defaults.closeIconColor), onPressed: (() => { ScaffoldMessenger.of(context).hideCurrentSnackBar(reason: SnackBarClosedReason.dismiss); }), tooltip: MaterialLocalizations.of(context).closeButtonTooltip) : null);
        var actionTextPainter = ((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = new global::Doroti.Framework.Painting.TextPainter(text: new global::Doroti.Framework.Painting.TextSpan(text: (((SnackBar)this.widget).action?.label ?? ""), style: Theme.of(context).textTheme.labelLarge), maxLines: 1L, textDirection: TextDirection.ltr);
    __cascade.layout();
    return __cascade;
}))();
        double actionAndIconWidth = ((((global::Doroti.Framework.Painting.TextPainter)actionTextPainter).size.width + (((((SnackBar)this.widget).action is not null) ? actionHorizontalMargin : 0L))) + ((showCloseIconLocal ? ((iconButton?.iconSize ?? (0L + iconHorizontalMargin))) : 0L)));
        actionTextPainter.dispose();
        global::Doroti.Framework.Painting.EdgeInsets marginLocal = ((((SnackBar)this.widget).margin?.resolve(TextDirection.ltr) ?? snackBarTheme.insetPadding) ?? defaults.insetPadding!);
        double snackBarWidth = (((SnackBar)this.widget).width ?? (MediaQuery.widthOf(context) - ((((global::Doroti.Framework.Painting.EdgeInsets)marginLocal).left + ((global::Doroti.Framework.Painting.EdgeInsets)marginLocal).right))));
        double actionOverflowThresholdLocal = ((((SnackBar)this.widget).actionOverflowThreshold ?? snackBarTheme.actionOverflowThreshold) ?? DartRuntimePrimitives.RequireValue(defaults.actionOverflowThreshold));
        bool willOverflowAction = ((actionAndIconWidth / snackBarWidth) > actionOverflowThresholdLocal);
        var maybeActionAndIcon = ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection27629 = new List<global::Doroti.Framework.Widgets.Widget>(); if ((((SnackBar)this.widget).action is not null)) { __collection27629.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: actionHorizontalMargin), child: new TextButtonTheme(data: new TextButtonThemeData(style: TextButton.styleFrom(foregroundColor: buttonColor, padding: global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: horizontalPadding))), child: ((SnackBar)this.widget).action!)))); } if (showCloseIconLocal) { __collection27629.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: iconHorizontalMargin), child: iconButton))); } return __collection27629; }))();
        global::Doroti.Framework.Widgets.Widget snackBar = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: paddingLocal, child: new global::Doroti.Framework.Widgets.Wrap(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection28354 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection28354.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Row(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection28401 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection28401.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.Padding(padding: ((((SnackBar)this.widget).padding is null) ? global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(vertical: Snack_barLibrary._singleLineVerticalPadding) : global::Doroti.Framework.Painting.EdgeInsets.zero), child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: contentTextStyleLocal!, child: ((SnackBar)this.widget).content))))); if (!willOverflowAction) { __collection28401.AddRange(maybeActionAndIcon); } if (willOverflowAction) { __collection28401.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(width: (snackBarWidth * 0.4)))); } return __collection28401; }))()))); if (willOverflowAction) { __collection28354.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(bottom: Snack_barLibrary._singleLineVerticalPadding), child: new global::Doroti.Framework.Widgets.Row(mainAxisAlignment: global::Doroti.Framework.Rendering.MainAxisAlignment.end, children: maybeActionAndIcon)))); } return __collection28354; }))())));
        if (!isFloatingSnackBar)
        {
            snackBar = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SafeArea(top: false, child: snackBar));
        }
        double elevationLocal = ((((SnackBar)this.widget).elevation ?? snackBarTheme.elevation) ?? DartRuntimePrimitives.RequireValue(defaults.elevation));
        global::Doroti.Ui.Color backgroundColorLocal = ((global::Doroti.Ui.Color)(object?)((((SnackBar)this.widget).backgroundColor ?? snackBarTheme.backgroundColor) ?? defaults.backgroundColor!));
        global::Doroti.Framework.Painting.ShapeBorder? shapeLocal = ((((SnackBar)this.widget).shape ?? snackBarTheme.shape) ?? ((isFloatingSnackBar ? defaults.shape : null)));
        global::Doroti.Framework.Widgets.DismissDirection dismissDirectionLocal = ((((SnackBar)this.widget).dismissDirection ?? snackBarTheme.dismissDirection) ?? global::Doroti.Framework.Widgets.DismissDirection.down);
        snackBar = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new Material(shape: shapeLocal, elevation: elevationLocal, color: backgroundColorLocal, clipBehavior: ((SnackBar)this.widget).clipBehavior, child: new Theme(data: effectiveTheme, child: ((accessibleNavigation || theme.useMaterial3) ? snackBar : new global::Doroti.Framework.Widgets.FadeTransition(opacity: this._fadeOutAnimation!, child: snackBar)))));
        if (isFloatingSnackBar)
        {
            if ((widthLocal is not null))
            {
                double width__24238__value30275 = DartRuntimePrimitives.RequireValue(widthLocal);
                snackBar = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(top: ((global::Doroti.Framework.Painting.EdgeInsets)marginLocal).top, bottom: ((global::Doroti.Framework.Painting.EdgeInsets)marginLocal).bottom), child: new global::Doroti.Framework.Widgets.SizedBox(width: DartRuntimePrimitives.RequireValue(width__24238__value30275), child: snackBar)));
            }
            else
            {
                snackBar = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: marginLocal, child: snackBar));
            }
            snackBar = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SafeArea(top: false, bottom: false, child: snackBar));
        }
        snackBar = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(container: true, liveRegion: true, onDismiss: ((global::System.Action)(() =>
        {
            ScaffoldMessenger.of(context).removeCurrentSnackBar(reason: SnackBarClosedReason.dismiss);
        })), child: new global::Doroti.Framework.Widgets.Dismissible(key: this._dismissibleKey, direction: dismissDirectionLocal, resizeDuration: null, behavior: (((SnackBar)this.widget).hitTestBehavior ?? ((((((SnackBar)this.widget).margin is not null) || (snackBarTheme.insetPadding is not null)) ? global::Doroti.Framework.Rendering.HitTestBehavior.deferToChild : global::Doroti.Framework.Rendering.HitTestBehavior.opaque))), onDismissed: ((global::System.Action<global::Doroti.Framework.Widgets.DismissDirection>)((direction) =>
        {
            ScaffoldMessenger.of(context).removeCurrentSnackBar(reason: SnackBarClosedReason.swipe);
        })), child: snackBar)));
        global::Doroti.Framework.Widgets.Widget snackBarTransition = default!;
        if (accessibleNavigation)
        {
            snackBarTransition = snackBar;
        }
        else
        {
            if ((isFloatingSnackBar && !theme.useMaterial3))
            {
                snackBarTransition = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.FadeTransition(opacity: this._fadeInAnimation!, child: snackBar));
            }
            else
            {
                if ((isFloatingSnackBar && theme.useMaterial3))
                {
                    snackBarTransition = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.FadeTransition(opacity: this._fadeInM3Animation!, child: new global::Doroti.Framework.Widgets.ValueListenableBuilder<double>(valueListenable: this._heightM3Animation!, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, double, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((context, value, child) =>
                    {
                        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.Alignment.bottomLeft, heightFactor: value, child: child));
                        throw new InvalidOperationException("Dart closure completed without a value.");
                    })), child: snackBar)));
                }
                else
                {
                    snackBarTransition = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ValueListenableBuilder<double>(valueListenable: this._heightAnimation!, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, double, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((context, value, child) =>
                    {
                        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.AlignmentDirectional.topStart, heightFactor: value, child: child));
                        throw new InvalidOperationException("Dart closure completed without a value.");
                    })), child: snackBar));
                }
            }
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Hero(tag: $"<SnackBar Hero tag - {((SnackBar)this.widget).content}>", transitionOnUserGestures: true, child: new global::Doroti.Framework.Widgets.ClipRect(clipBehavior: ((SnackBar)this.widget).clipBehavior, child: snackBarTransition)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SnackbarDefaultsM2__snack_bar : SnackBarThemeData
{
    internal virtual ThemeData _theme { get; private set; } = default!;
    internal virtual ColorScheme _colors { get; private set; } = default!;

    internal _SnackbarDefaultsM2__snack_bar(global::Doroti.Framework.Widgets.BuildContext context) : base(elevation: 6.0)
    {
        this._theme = Theme.of(context);
        this._colors = Theme.of(context).colorScheme;
    }

    public virtual global::Doroti.Ui.Color backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((object.Equals(this._theme.brightness, Brightness.light)) ? Dart_uiLibrary.Color.alphaBlend(this._colors.onSurface.withOpacity(0.8), this._colors.surface) : this._colors.onSurface));
    public override global::Doroti.Framework.Painting.TextStyle? contentTextStyle => ThemeData.Create(useMaterial3: this._theme.useMaterial3, brightness: ((object.Equals(this._theme.brightness, Brightness.light)) ? Brightness.dark : Brightness.light)).textTheme.titleMedium;
    public virtual SnackBarBehavior behavior => SnackBarBehavior.@fixed;
    public virtual global::Doroti.Ui.Color actionTextColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.secondary);
    public virtual global::Doroti.Ui.Color disabledActionTextColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface.withOpacity(((object.Equals(this._theme.brightness, Brightness.light)) ? 0.38 : 0.3)));
    public virtual global::Doroti.Framework.Painting.ShapeBorder shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.ShapeBorder>(new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(4.0))));
    public virtual global::Doroti.Framework.Painting.EdgeInsets insetPadding => new global::Doroti.Framework.Painting.EdgeInsets(15.0, 5.0, 15.0, 10.0);
    public virtual bool showCloseIcon => false;
    public virtual global::Doroti.Ui.Color closeIconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface);
    public virtual double actionOverflowThreshold => 0.25;
}

internal class _SnackbarDefaultsM3__snack_bar : SnackBarThemeData
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

    internal _SnackbarDefaultsM3__snack_bar(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.inverseSurface);
    public virtual global::Doroti.Ui.Color actionTextColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(global::Doroti.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) =>
    {
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
        {
            return this._colors.inversePrimary;
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
        {
            return this._colors.inversePrimary;
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
        {
            return this._colors.inversePrimary;
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
        {
            return this._colors.inversePrimary;
        }
        return this._colors.inversePrimary;
        throw new InvalidOperationException("Dart closure completed without a value.");
    })));
    public virtual global::Doroti.Ui.Color disabledActionTextColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.inversePrimary);
    public virtual global::Doroti.Framework.Painting.TextStyle contentTextStyle => Theme.of(this.context).textTheme.bodyMedium!.copyWith(color: this._colors.onInverseSurface);
    public virtual double elevation => 6.0;
    public virtual global::Doroti.Framework.Painting.ShapeBorder shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.ShapeBorder>(new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(4.0))));
    public virtual SnackBarBehavior behavior => SnackBarBehavior.@fixed;
    public virtual global::Doroti.Framework.Painting.EdgeInsets insetPadding => new global::Doroti.Framework.Painting.EdgeInsets(15.0, 5.0, 15.0, 10.0);
    public virtual bool showCloseIcon => false;
    public virtual global::Doroti.Ui.Color? closeIconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onInverseSurface);
    public virtual double actionOverflowThreshold => 0.25;
}
