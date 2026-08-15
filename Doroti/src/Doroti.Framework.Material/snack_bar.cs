// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/snack_bar.dart
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
    internal static global::Doroti.Generated.Framework.Animation.Curve _snackBarHeightCurve = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)global::Doroti.Generated.Framework.Animation.Curves.fastOutSlowIn);
}

public static partial class Snack_barLibrary
{
    internal static global::Doroti.Generated.Framework.Animation.Curve _snackBarM3HeightCurve = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)global::Doroti.Generated.Framework.Animation.Curves.easeInOutQuart);
}

public static partial class Snack_barLibrary
{
    internal static global::Doroti.Generated.Framework.Animation.Curve _snackBarFadeInCurve = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)new global::Doroti.Generated.Framework.Animation.Interval(0.4, 1.0));
}

public static partial class Snack_barLibrary
{
    internal static global::Doroti.Generated.Framework.Animation.Curve _snackBarM3FadeInCurve = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)new global::Doroti.Generated.Framework.Animation.Interval(0.4, 0.6, curve: global::Doroti.Generated.Framework.Animation.Curves.easeInCirc));
}

public static partial class Snack_barLibrary
{
    internal static global::Doroti.Generated.Framework.Animation.Curve _snackBarFadeOutCurve = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)new global::Doroti.Generated.Framework.Animation.Interval(0.72, 1.0, curve: global::Doroti.Generated.Framework.Animation.Curves.fastOutSlowIn));
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

public class SnackBarAction : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual Color? textColor { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? disabledTextColor { get; private set; }
    public virtual Color? disabledBackgroundColor { get; private set; }
    public virtual string label { get; private set; } = default!;
    public virtual global::System.Action onPressed { get; private set; } = default!;

    public SnackBarAction(global::Doroti.Generated.Framework.Foundation.Key? key = null, Color? textColor = null, Color? disabledTextColor = null, Color? backgroundColor = null, Color? disabledBackgroundColor = null, string label = default!, global::System.Action onPressed = default!) : base(key: key)
    {
        this.textColor = textColor;
        this.disabledTextColor = disabledTextColor;
        this.backgroundColor = backgroundColor;
        this.disabledBackgroundColor = disabledBackgroundColor;
        this.label = label;
        this.onPressed = onPressed;
        System.Diagnostics.Debug.Assert(((backgroundColor is not global::Doroti.Generated.Framework.Widgets.WidgetStateColor) || (disabledBackgroundColor is null)));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SnackBarActionState__snack_bar());
}

internal class _SnackBarActionState__snack_bar : global::Doroti.Generated.Framework.Widgets.State<SnackBarAction>
{
    internal virtual bool _haveTriggeredAction { get; set; } = false;

    internal virtual void _handlePressed()
    {
        if (this._haveTriggeredAction)
        {
            return;
        }
        setState(((global::System.Action)(() => {
_haveTriggeredAction = true;
})));
        this.widget.onPressed();
        ScaffoldMessenger.of(this.context).hideCurrentSnackBar(reason: SnackBarClosedReason.action);
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        SnackBarThemeData defaults__5209 = (Theme.of(context).useMaterial3 ? new _SnackbarDefaultsM3__snack_bar(context) : new _SnackbarDefaultsM2__snack_bar(context));
        SnackBarThemeData snackBarTheme__5358 = SnackBarTheme.of(context);
        global::Doroti.Generated.Framework.Widgets.WidgetStateColor resolveForegroundColor()
        {
            if ((((SnackBarAction)this.widget).textColor is not null))
            {
                if ((((SnackBarAction)this.widget).textColor is global::Doroti.Generated.Framework.Widgets.WidgetStateColor))
                {
                    return ((global::Doroti.Generated.Framework.Widgets.WidgetStateColor?)(object?)((SnackBarAction)this.widget).textColor!)!;
                }
            }
            else
            {
                if ((snackBarTheme__5358.actionTextColor is not null))
                {
                    if ((snackBarTheme__5358.actionTextColor is global::Doroti.Generated.Framework.Widgets.WidgetStateColor))
                    {
                        return ((global::Doroti.Generated.Framework.Widgets.WidgetStateColor?)(object?)snackBarTheme__5358.actionTextColor!)!;
                    }
                }
                else
                {
                    if ((defaults__5209.actionTextColor is not null))
                    {
                        if ((defaults__5209.actionTextColor is global::Doroti.Generated.Framework.Widgets.WidgetStateColor))
                        {
                            return ((global::Doroti.Generated.Framework.Widgets.WidgetStateColor?)(object?)defaults__5209.actionTextColor!)!;
                        }
                    }
                }
            }
            return global::Doroti.Generated.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return ((((SnackBarAction)this.widget).disabledTextColor ?? snackBarTheme__5358.disabledActionTextColor) ?? defaults__5209.disabledActionTextColor!);
}
return ((((SnackBarAction)this.widget).textColor ?? snackBarTheme__5358.actionTextColor) ?? defaults__5209.actionTextColor!);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::Doroti.Generated.Framework.Widgets.WidgetStateColor? resolveBackgroundColor()
        {
            if ((((SnackBarAction)this.widget).backgroundColor is global::Doroti.Generated.Framework.Widgets.WidgetStateColor))
            {
                return ((global::Doroti.Generated.Framework.Widgets.WidgetStateColor?)(object?)((SnackBarAction)this.widget).backgroundColor!)!;
            }
            if ((snackBarTheme__5358.actionBackgroundColor is global::Doroti.Generated.Framework.Widgets.WidgetStateColor))
            {
                return ((global::Doroti.Generated.Framework.Widgets.WidgetStateColor?)(object?)snackBarTheme__5358.actionBackgroundColor!)!;
            }
            return global::Doroti.Generated.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return ((((SnackBarAction)this.widget).disabledBackgroundColor ?? snackBarTheme__5358.disabledActionBackgroundColor) ?? Colors.transparent);
}
return ((((SnackBarAction)this.widget).backgroundColor ?? snackBarTheme__5358.actionBackgroundColor) ?? Colors.transparent);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new TextButton(style: TextButton.styleFrom(overlayColor: resolveForegroundColor()).copyWith(foregroundColor: resolveForegroundColor(), backgroundColor: resolveBackgroundColor()), onPressed: ((global::System.Action)(this._haveTriggeredAction ? null : this._handlePressed)), child: new global::Doroti.Generated.Framework.Widgets.Text(((SnackBarAction)this.widget).label)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SnackBar : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget content { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? margin { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual double? width { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.HitTestBehavior? hitTestBehavior { get; private set; }
    public virtual SnackBarBehavior? behavior { get; private set; }
    public virtual SnackBarAction? action { get; private set; }
    public virtual double? actionOverflowThreshold { get; private set; }
    public virtual bool? showCloseIcon { get; private set; }
    public virtual Color? closeIconColor { get; private set; }
    public virtual Duration duration { get; private set; } = default!;
    public virtual bool persist { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double>? animation { get; private set; }
    public virtual global::System.Action? onVisible { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.DismissDirection? dismissDirection { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;

    public SnackBar(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget content = default!, Color? backgroundColor = null, double? elevation = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? margin = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, double? width = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Generated.Framework.Rendering.HitTestBehavior? hitTestBehavior = null, SnackBarBehavior? behavior = null, SnackBarAction? action = null, double? actionOverflowThreshold = null, bool? showCloseIcon = null, Color? closeIconColor = null, Duration? duration = null, bool? persist = null, global::Doroti.Generated.Framework.Animation.Animation<double>? animation = null, global::System.Action? onVisible = null, global::Doroti.Generated.Framework.Widgets.DismissDirection? dismissDirection = null, Clip clipBehavior = Clip.hardEdge) : base(key: key)
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

    public static global::Doroti.Generated.Framework.Animation.AnimationController createAnimationController(global::Doroti.Generated.Framework.Scheduler.TickerProvider vsync, Duration? duration = null, Duration? reverseDuration = null)
    {
        return new global::Doroti.Generated.Framework.Animation.AnimationController(duration: (duration ?? Snack_barLibrary._snackBarTransitionDuration), reverseDuration: reverseDuration, debugLabel: "SnackBar", vsync: vsync);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SnackBar withAnimation(global::Doroti.Generated.Framework.Animation.Animation<double> newAnimation, global::Doroti.Generated.Framework.Foundation.Key? fallbackKey = null)
    {
        return new SnackBar(key: (this.key ?? fallbackKey), content: this.content, backgroundColor: this.backgroundColor, elevation: this.elevation, margin: this.margin, padding: this.padding, width: this.width, shape: this.shape, hitTestBehavior: this.hitTestBehavior, behavior: this.behavior, action: this.action, actionOverflowThreshold: this.actionOverflowThreshold, showCloseIcon: this.showCloseIcon, closeIconColor: this.closeIconColor, duration: DartRuntimePrimitives.RequireValue(this.duration), persist: this.persist, animation: newAnimation, onVisible: this.onVisible, dismissDirection: this.dismissDirection, clipBehavior: this.clipBehavior);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SnackBarState__snack_bar());
}

internal class _SnackBarState__snack_bar : global::Doroti.Generated.Framework.Widgets.State<SnackBar>
{
    internal virtual bool _wasVisible { get; set; } = false;
    internal virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation? _heightAnimation { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation? _fadeInAnimation { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation? _fadeInM3Animation { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation? _fadeOutAnimation { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation? _heightM3Animation { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Foundation.Key _dismissibleKey { get; private set; } = ((global::Doroti.Generated.Framework.Foundation.Key)(object?)new global::Doroti.Generated.Framework.Foundation.UniqueKey());

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
        _heightAnimation = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: ((SnackBar)this.widget).animation!, curve: Snack_barLibrary._snackBarHeightCurve);
        _fadeInAnimation = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: ((SnackBar)this.widget).animation!, curve: Snack_barLibrary._snackBarFadeInCurve);
        _fadeInM3Animation = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: ((SnackBar)this.widget).animation!, curve: Snack_barLibrary._snackBarM3FadeInCurve);
        _fadeOutAnimation = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: ((SnackBar)this.widget).animation!, curve: Snack_barLibrary._snackBarFadeOutCurve, reverseCurve: new global::Doroti.Generated.Framework.Animation.Threshold(0.0));
        _heightM3Animation = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: ((SnackBar)this.widget).animation!, curve: Snack_barLibrary._snackBarM3HeightCurve, reverseCurve: new global::Doroti.Generated.Framework.Animation.Threshold(0.0));
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

    internal virtual void _onAnimationStatusChanged(global::Doroti.Generated.Framework.Animation.AnimationStatus animationStatus)
    {
        if (global::Doroti.Generated.Framework.Animation.AnimationStatusMembers.isCompleted(animationStatus))
        {
            if (((((SnackBar)this.widget).onVisible is not null) && !this._wasVisible))
            {
                ((SnackBar)this.widget).onVisible!();
            }
            _wasVisible = true;
        }
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        bool accessibleNavigation__22487 = MediaQuery.accessibleNavigationOf(context);
        DartRuntimePrimitives.Assert(() => (((SnackBar)this.widget).animation is not null));
        ThemeData theme__22612 = Theme.of(context);
        ColorScheme colorScheme__22661 = theme__22612.colorScheme;
        SnackBarThemeData snackBarTheme__22722 = SnackBarTheme.of(context);
        var isThemeDark__22775 = (object.Equals(theme__22612.brightness, Brightness.dark));
        global::Doroti.Ui.Color buttonColor__22842 = ((global::Doroti.Ui.Color)(object?)(isThemeDark__22775 ? colorScheme__22661.primary : colorScheme__22661.secondary));
        SnackBarThemeData defaults__22943 = (theme__22612.useMaterial3 ? new _SnackbarDefaultsM3__snack_bar(context) : new _SnackbarDefaultsM2__snack_bar(context));
        global::Doroti.Ui.Brightness brightness__23170 = (isThemeDark__22775 ? Brightness.light : Brightness.dark);
        ThemeData effectiveTheme__23357 = (theme__22612.useMaterial3 ? theme__22612 : theme__22612.copyWith(colorScheme: new ColorScheme(primary: colorScheme__22661.onPrimary, secondary: buttonColor__22842, surface: colorScheme__22661.onSurface, background: defaults__22943.backgroundColor, error: colorScheme__22661.onError, onPrimary: colorScheme__22661.primary, onSecondary: colorScheme__22661.secondary, onSurface: colorScheme__22661.surface, onBackground: colorScheme__22661.background, onError: colorScheme__22661.error, brightness: brightness__23170)));
        global::Doroti.Generated.Framework.Painting.TextStyle? contentTextStyle__24021 = (snackBarTheme__22722.contentTextStyle ?? defaults__22943.contentTextStyle);
        SnackBarBehavior snackBarBehavior__24128 = DartRuntimePrimitives.RequireValue(((SnackBar)this.widget).behavior ?? snackBarTheme__22722.behavior ?? defaults__22943.behavior);
        double? width__24238 = (((SnackBar)this.widget).width ?? snackBarTheme__22722.width);
        DartRuntimePrimitives.Assert(() =>
            {
                if ((!object.Equals(snackBarBehavior__24128, SnackBarBehavior.floating)))
                {
                    string message(string parameter)
                    {
                        var prefix__24555 = $"{parameter} can only be used with floating behavior.";
                        if ((((SnackBar)this.widget).behavior is not null))
                        {
                            return $"{prefix__24555} SnackBarBehavior.fixed was set in the SnackBar constructor.";
                        }
                        else
                        {
                            if ((snackBarTheme__22722.behavior is not null))
                            {
                                return $"{prefix__24555} SnackBarBehavior.fixed was set by the inherited SnackBarThemeData.";
                            }
                            else
                            {
                                return $"{prefix__24555} SnackBarBehavior.fixed was set by default.";
                            }
                        }
                        throw new InvalidOperationException("Dart control flow completed without a value.");
                    }
                    DartRuntimePrimitives.Assert(() => (((SnackBar)this.widget).margin is null), () => (object?)message("Margin"));
                    DartRuntimePrimitives.Assert(() => (width__24238 is null), () => (object?)message("Width"));
                }
                return true;
            });
        bool showCloseIcon__25177 = ((((SnackBar)this.widget).showCloseIcon ?? snackBarTheme__22722.showCloseIcon) ?? DartRuntimePrimitives.RequireValue(defaults__22943.showCloseIcon));
        var isFloatingSnackBar__25292 = (object.Equals(snackBarBehavior__24128, SnackBarBehavior.floating));
        var horizontalPadding__25370 = (isFloatingSnackBar__25292 ? 16.0 : 24.0);
        global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding__25453 = (((SnackBar)this.widget).padding ?? global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: horizontalPadding__25370, end: (((((SnackBar)this.widget).action is not null) || showCloseIcon__25177) ? 0 : horizontalPadding__25370)));
        double actionHorizontalMargin__25669 = (((((SnackBar)this.widget).padding?.resolve(TextDirection.ltr).right ?? horizontalPadding__25370)) / 2L);
        double iconHorizontalMargin__25796 = (((((SnackBar)this.widget).padding?.resolve(TextDirection.ltr).right ?? horizontalPadding__25370)) / 12.0);
        IconButton? iconButton__25930 = (showCloseIcon__25177 ? new IconButton(key: StandardComponentTypeMembers.key(global::Doroti.Generated.Framework.Widgets.StandardComponentType.closeButton), icon: new global::Doroti.Generated.Framework.Widgets.Icon(Icons.close), iconSize: 24.0, color: ((((SnackBar)this.widget).closeIconColor ?? snackBarTheme__22722.closeIconColor) ?? defaults__22943.closeIconColor), onPressed: (() => { ScaffoldMessenger.of(context).hideCurrentSnackBar(reason: SnackBarClosedReason.dismiss); }), tooltip: MaterialLocalizations.of(context).closeButtonTooltip) : null);
        var actionTextPainter__26557 = ((Func<global::Doroti.Generated.Framework.Painting.TextPainter>)(() =>
{            var __cascade = new global::Doroti.Generated.Framework.Painting.TextPainter(text: new global::Doroti.Generated.Framework.Painting.TextSpan(text: (((SnackBar)this.widget).action?.label ?? ""), style: Theme.of(context).textTheme.labelLarge), maxLines: 1L, textDirection: TextDirection.ltr);
            __cascade.layout();
            return __cascade;        }))();
        double actionAndIconWidth__26811 = ((((global::Doroti.Generated.Framework.Painting.TextPainter)actionTextPainter__26557).size.width + (((((SnackBar)this.widget).action is not null) ? actionHorizontalMargin__25669 : 0L))) + ((showCloseIcon__25177 ? ((iconButton__25930?.iconSize ?? (0L + iconHorizontalMargin__25796))) : 0L)));
        actionTextPainter__26557.dispose();
        global::Doroti.Generated.Framework.Painting.EdgeInsets margin__27071 = ((((SnackBar)this.widget).margin?.resolve(TextDirection.ltr) ?? snackBarTheme__22722.insetPadding) ?? defaults__22943.insetPadding!);
        double snackBarWidth__27221 = (((SnackBar)this.widget).width ?? (MediaQuery.widthOf(context) - ((((global::Doroti.Generated.Framework.Painting.EdgeInsets)margin__27071).left + ((global::Doroti.Generated.Framework.Painting.EdgeInsets)margin__27071).right))));
        double actionOverflowThreshold__27338 = ((((SnackBar)this.widget).actionOverflowThreshold ?? snackBarTheme__22722.actionOverflowThreshold) ?? DartRuntimePrimitives.RequireValue(defaults__22943.actionOverflowThreshold));
        bool willOverflowAction__27514 = ((actionAndIconWidth__26811 / snackBarWidth__27221) > actionOverflowThreshold__27338);
        var maybeActionAndIcon__27608 = ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection27629 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); if ((((SnackBar)this.widget).action is not null)) { __collection27629.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: actionHorizontalMargin__25669), child: new TextButtonTheme(data: new TextButtonThemeData(style: TextButton.styleFrom(foregroundColor: buttonColor__22842, padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: horizontalPadding__25370))), child: ((SnackBar)this.widget).action!)))); } if (showCloseIcon__25177) { __collection27629.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: iconHorizontalMargin__25796), child: iconButton__25930))); } return __collection27629; }))();
        global::Doroti.Generated.Framework.Widgets.Widget snackBar__28273 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Padding(padding: padding__25453, child: new global::Doroti.Generated.Framework.Widgets.Wrap(children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection28354 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection28354.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Row(children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection28401 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection28401.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Expanded(child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: ((((SnackBar)this.widget).padding is null) ? global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(vertical: Snack_barLibrary._singleLineVerticalPadding) : global::Doroti.Generated.Framework.Painting.EdgeInsets.zero), child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: contentTextStyle__24021!, child: ((SnackBar)this.widget).content))))); if (!willOverflowAction__27514) { __collection28401.AddRange(maybeActionAndIcon__27608); } if (willOverflowAction__27514) { __collection28401.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(width: (snackBarWidth__27221 * 0.4)))); } return __collection28401; }))()))); if (willOverflowAction__27514) { __collection28354.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(bottom: Snack_barLibrary._singleLineVerticalPadding), child: new global::Doroti.Generated.Framework.Widgets.Row(mainAxisAlignment: global::Doroti.Generated.Framework.Rendering.MainAxisAlignment.end, children: maybeActionAndIcon__27608)))); } return __collection28354; }))())));
        if (!isFloatingSnackBar__25292)
        {
            snackBar__28273 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SafeArea(top: false, child: snackBar__28273));
        }
        double elevation__29330 = ((((SnackBar)this.widget).elevation ?? snackBarTheme__22722.elevation) ?? DartRuntimePrimitives.RequireValue(defaults__22943.elevation));
        global::Doroti.Ui.Color backgroundColor__29426 = ((global::Doroti.Ui.Color)(object?)((((SnackBar)this.widget).backgroundColor ?? snackBarTheme__22722.backgroundColor) ?? defaults__22943.backgroundColor!));
        global::Doroti.Generated.Framework.Painting.ShapeBorder? shape__29561 = ((((SnackBar)this.widget).shape ?? snackBarTheme__22722.shape) ?? ((isFloatingSnackBar__25292 ? defaults__22943.shape : null)));
        global::Doroti.Generated.Framework.Widgets.DismissDirection dismissDirection__29689 = ((((SnackBar)this.widget).dismissDirection ?? snackBarTheme__22722.dismissDirection) ?? global::Doroti.Generated.Framework.Widgets.DismissDirection.down);
        snackBar__28273 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new Material(shape: shape__29561, elevation: elevation__29330, color: backgroundColor__29426, clipBehavior: ((SnackBar)this.widget).clipBehavior, child: new Theme(data: effectiveTheme__23357, child: ((accessibleNavigation__22487 || theme__22612.useMaterial3) ? snackBar__28273 : new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: this._fadeOutAnimation!, child: snackBar__28273)))));
        if (isFloatingSnackBar__25292)
        {
            if ((width__24238 is not null))
            {
                double width__24238__value30275 = DartRuntimePrimitives.RequireValue(width__24238);
                snackBar__28273 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(top: ((global::Doroti.Generated.Framework.Painting.EdgeInsets)margin__27071).top, bottom: ((global::Doroti.Generated.Framework.Painting.EdgeInsets)margin__27071).bottom), child: new global::Doroti.Generated.Framework.Widgets.SizedBox(width: DartRuntimePrimitives.RequireValue(width__24238__value30275), child: snackBar__28273)));
            }
            else
            {
                snackBar__28273 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: margin__27071, child: snackBar__28273));
            }
            snackBar__28273 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SafeArea(top: false, bottom: false, child: snackBar__28273));
        }
        snackBar__28273 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Semantics(container: true, liveRegion: true, onDismiss: ((global::System.Action)(() => {
ScaffoldMessenger.of(context).removeCurrentSnackBar(reason: SnackBarClosedReason.dismiss);
})), child: new global::Doroti.Generated.Framework.Widgets.Dismissible(key: this._dismissibleKey, direction: dismissDirection__29689, resizeDuration: null, behavior: (((SnackBar)this.widget).hitTestBehavior ?? ((((((SnackBar)this.widget).margin is not null) || (snackBarTheme__22722.insetPadding is not null)) ? global::Doroti.Generated.Framework.Rendering.HitTestBehavior.deferToChild : global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque))), onDismissed: ((global::System.Action<global::Doroti.Generated.Framework.Widgets.DismissDirection>)((direction) => {
ScaffoldMessenger.of(context).removeCurrentSnackBar(reason: SnackBarClosedReason.swipe);
})), child: snackBar__28273)));
        global::Doroti.Generated.Framework.Widgets.Widget snackBarTransition__31394 = default!;
        if (accessibleNavigation__22487)
        {
            snackBarTransition__31394 = snackBar__28273;
        }
        else
        {
            if ((isFloatingSnackBar__25292 && !theme__22612.useMaterial3))
            {
                snackBarTransition__31394 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: this._fadeInAnimation!, child: snackBar__28273));
            }
            else
            {
                if ((isFloatingSnackBar__25292 && theme__22612.useMaterial3))
                {
                    snackBarTransition__31394 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: this._fadeInM3Animation!, child: new global::Doroti.Generated.Framework.Widgets.ValueListenableBuilder<double>(valueListenable: this._heightM3Animation!, builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, double, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, value, child) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Align(alignment: global::Doroti.Generated.Framework.Painting.Alignment.bottomLeft, heightFactor: value, child: child));
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: snackBar__28273)));
                }
                else
                {
                    snackBarTransition__31394 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.ValueListenableBuilder<double>(valueListenable: this._heightAnimation!, builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, double, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, value, child) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Align(alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.topStart, heightFactor: value, child: child));
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: snackBar__28273));
                }
            }
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Hero(tag: $"<SnackBar Hero tag - {((SnackBar)this.widget).content}>", transitionOnUserGestures: true, child: new global::Doroti.Generated.Framework.Widgets.ClipRect(clipBehavior: ((SnackBar)this.widget).clipBehavior, child: snackBarTransition__31394)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SnackbarDefaultsM2__snack_bar : SnackBarThemeData
{
    internal virtual ThemeData _theme { get; private set; } = default!;
    internal virtual ColorScheme _colors { get; private set; } = default!;

    internal _SnackbarDefaultsM2__snack_bar(global::Doroti.Generated.Framework.Widgets.BuildContext context) : base(elevation: 6.0)
    {
        this._theme = Theme.of(context);
        this._colors = Theme.of(context).colorScheme;
    }

    public virtual global::Doroti.Ui.Color backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((object.Equals(this._theme.brightness, Brightness.light)) ? Dart_uiLibrary.Color.alphaBlend(this._colors.onSurface.withOpacity(0.8), this._colors.surface) : this._colors.onSurface));
    public override global::Doroti.Generated.Framework.Painting.TextStyle? contentTextStyle => ThemeData.Create(useMaterial3: this._theme.useMaterial3, brightness: ((object.Equals(this._theme.brightness, Brightness.light)) ? Brightness.dark : Brightness.light)).textTheme.titleMedium;
    public virtual SnackBarBehavior behavior => SnackBarBehavior.@fixed;
    public virtual global::Doroti.Ui.Color actionTextColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.secondary);
    public virtual global::Doroti.Ui.Color disabledActionTextColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface.withOpacity(((object.Equals(this._theme.brightness, Brightness.light)) ? 0.38 : 0.3)));
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.ShapeBorder>(new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(4.0))));
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets insetPadding => new global::Doroti.Generated.Framework.Painting.EdgeInsets(15.0, 5.0, 15.0, 10.0);
    public virtual bool showCloseIcon => false;
    public virtual global::Doroti.Ui.Color closeIconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface);
    public virtual double actionOverflowThreshold => 0.25;
}

internal class _SnackbarDefaultsM3__snack_bar : SnackBarThemeData
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;
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

    internal _SnackbarDefaultsM3__snack_bar(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.inverseSurface);
    public virtual global::Doroti.Ui.Color actionTextColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(global::Doroti.Generated.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return this._colors.inversePrimary;
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
{
    return this._colors.inversePrimary;
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
{
    return this._colors.inversePrimary;
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
{
    return this._colors.inversePrimary;
}
return this._colors.inversePrimary;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
    public virtual global::Doroti.Ui.Color disabledActionTextColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.inversePrimary);
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle contentTextStyle => Theme.of(this.context).textTheme.bodyMedium!.copyWith(color: this._colors.onInverseSurface);
    public virtual double elevation => 6.0;
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.ShapeBorder>(new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(4.0))));
    public virtual SnackBarBehavior behavior => SnackBarBehavior.@fixed;
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets insetPadding => new global::Doroti.Generated.Framework.Painting.EdgeInsets(15.0, 5.0, 15.0, 10.0);
    public virtual bool showCloseIcon => false;
    public virtual global::Doroti.Ui.Color? closeIconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onInverseSurface);
    public virtual double actionOverflowThreshold => 0.25;
}
