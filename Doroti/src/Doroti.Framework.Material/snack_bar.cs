// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/snack_bar.dart

using Doroti.Runtime;
using Doroti.Ui;

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
    internal static global::Doroti.Framework.Animation.Curve _snackBarHeightCurve = Curves.fastOutSlowIn;
}

public static partial class Snack_barLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _snackBarM3HeightCurve = Curves.easeInOutQuart;
}

public static partial class Snack_barLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _snackBarFadeInCurve = new global::Doroti.Framework.Animation.Interval(0.4, 1.0);
}

public static partial class Snack_barLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _snackBarM3FadeInCurve = new global::Doroti.Framework.Animation.Interval(0.4, 0.6, curve: Curves.easeInCirc);
}

public static partial class Snack_barLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _snackBarFadeOutCurve = new global::Doroti.Framework.Animation.Interval(0.72, 1.0, curve: Curves.fastOutSlowIn);
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
        System.Diagnostics.Debug.Assert((backgroundColor is not WidgetStateColor) || (disabledBackgroundColor is null));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SnackBarActionState__snack_bar());
}

internal class _SnackBarActionState__snack_bar : global::Doroti.Framework.Widgets.State<SnackBarAction>
{
    internal virtual bool _haveTriggeredAction { get; set; } = false;

    internal virtual void _handlePressed()
    {
        if (_haveTriggeredAction)
        {
            return;
        }
        setState(() =>
        {
            _haveTriggeredAction = true;
        });
        widget.onPressed();
        ScaffoldMessenger.of(context).hideCurrentSnackBar(reason: SnackBarClosedReason.action);
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        SnackBarThemeData defaults = new _SnackbarDefaultsM3__snack_bar(context);
        SnackBarThemeData snackBarTheme = SnackBarTheme.of(context);
        global::Doroti.Framework.Widgets.WidgetStateColor resolveForegroundColor()
        {
            if (widget.textColor is not null)
            {
                if (widget.textColor is global::Doroti.Framework.Widgets.WidgetStateColor)
                {
                    return ((global::Doroti.Framework.Widgets.WidgetStateColor?)widget.textColor!)!;
                }
            }
            else
            {
                if (snackBarTheme.actionTextColor is not null)
                {
                    if (snackBarTheme.actionTextColor is global::Doroti.Framework.Widgets.WidgetStateColor)
                    {
                        return ((global::Doroti.Framework.Widgets.WidgetStateColor?)snackBarTheme.actionTextColor!)!;
                    }
                }
                else
                {
                    if (defaults.actionTextColor is not null)
                    {
                        if (defaults.actionTextColor is global::Doroti.Framework.Widgets.WidgetStateColor)
                        {
                            return ((global::Doroti.Framework.Widgets.WidgetStateColor?)defaults.actionTextColor!)!;
                        }
                    }
                }
            }
            return WidgetStateColor.CreateResolveWith((states) =>
            {
                if (states.Contains(WidgetState.disabled))
                {
                    return (widget.disabledTextColor ?? snackBarTheme.disabledActionTextColor) ?? defaults.disabledActionTextColor!;
                }
                return (widget.textColor ?? snackBarTheme.actionTextColor) ?? defaults.actionTextColor!;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::Doroti.Framework.Widgets.WidgetStateColor? resolveBackgroundColor()
        {
            if (widget.backgroundColor is global::Doroti.Framework.Widgets.WidgetStateColor)
            {
                return ((global::Doroti.Framework.Widgets.WidgetStateColor?)widget.backgroundColor!)!;
            }
            if (snackBarTheme.actionBackgroundColor is global::Doroti.Framework.Widgets.WidgetStateColor)
            {
                return ((global::Doroti.Framework.Widgets.WidgetStateColor?)snackBarTheme.actionBackgroundColor!)!;
            }
            return WidgetStateColor.CreateResolveWith((states) =>
            {
                if (states.Contains(WidgetState.disabled))
                {
                    return (widget.disabledBackgroundColor ?? snackBarTheme.disabledActionBackgroundColor) ?? Colors.transparent;
                }
                return (widget.backgroundColor ?? snackBarTheme.actionBackgroundColor) ?? Colors.transparent;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        return new TextButton(style: TextButton.styleFrom(overlayColor: resolveForegroundColor()).copyWith(foregroundColor: resolveForegroundColor(), backgroundColor: resolveBackgroundColor()), onPressed: _haveTriggeredAction ? null : _handlePressed, child: new global::Doroti.Framework.Widgets.Text(widget.label));
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
        this.persist = persist ?? (action is not null);
        System.Diagnostics.Debug.Assert((elevation is null) || (elevation >= 0.0));
        System.Diagnostics.Debug.Assert((width is null) || (margin is null));
        System.Diagnostics.Debug.Assert((actionOverflowThreshold is null) || (actionOverflowThreshold >= 0L) && (actionOverflowThreshold <= 1L));
    }

    public static global::Doroti.Framework.Animation.AnimationController createAnimationController(global::Doroti.Framework.Scheduler.TickerProvider vsync, Duration? duration = null, Duration? reverseDuration = null)
    {
        return new global::Doroti.Framework.Animation.AnimationController(duration: duration ?? Snack_barLibrary._snackBarTransitionDuration, reverseDuration: reverseDuration, debugLabel: "SnackBar", vsync: vsync);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SnackBar withAnimation(global::Doroti.Framework.Animation.Animation<double> newAnimation, global::Doroti.Framework.Foundation.Key? fallbackKey = null)
    {
        return new SnackBar(key: key ?? fallbackKey, content: content, backgroundColor: backgroundColor, elevation: elevation, margin: margin, padding: padding, width: width, shape: shape, hitTestBehavior: hitTestBehavior, behavior: behavior, action: action, actionOverflowThreshold: actionOverflowThreshold, showCloseIcon: showCloseIcon, closeIconColor: closeIconColor, duration: DartRuntimePrimitives.RequireValue(duration), persist: persist, animation: newAnimation, onVisible: onVisible, dismissDirection: dismissDirection, clipBehavior: clipBehavior);
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
    internal virtual global::Doroti.Framework.Foundation.Key _dismissibleKey { get; private set; } = new global::Doroti.Framework.Foundation.UniqueKey();

    public override void initState()
    {
        base.initState();
        widget.animation!.addStatusListener(_onAnimationStatusChanged);
        _setAnimations();
    }

    public override void didUpdateWidget(SnackBar oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.animation, oldWidget.animation))
        {
            oldWidget.animation!.removeStatusListener(_onAnimationStatusChanged);
            widget.animation!.addStatusListener(_onAnimationStatusChanged);
            _disposeAnimations();
            _setAnimations();
        }
    }

    internal virtual void _setAnimations()
    {
        DartRuntimePrimitives.Assert(() => widget.animation is not null);
        _heightAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: widget.animation!, curve: Snack_barLibrary._snackBarHeightCurve);
        _fadeInAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: widget.animation!, curve: Snack_barLibrary._snackBarFadeInCurve);
        _fadeInM3Animation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: widget.animation!, curve: Snack_barLibrary._snackBarM3FadeInCurve);
        _fadeOutAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: widget.animation!, curve: Snack_barLibrary._snackBarFadeOutCurve, reverseCurve: new global::Doroti.Framework.Animation.Threshold(0.0));
        _heightM3Animation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: widget.animation!, curve: Snack_barLibrary._snackBarM3HeightCurve, reverseCurve: new global::Doroti.Framework.Animation.Threshold(0.0));
    }

    internal virtual void _disposeAnimations()
    {
        _heightAnimation?.dispose();
        _fadeInAnimation?.dispose();
        _fadeInM3Animation?.dispose();
        _fadeOutAnimation?.dispose();
        _heightM3Animation?.dispose();
        _heightAnimation = null;
        _fadeInAnimation = null;
        _fadeInM3Animation = null;
        _fadeOutAnimation = null;
        _heightM3Animation = null;
    }

    public override void dispose()
    {
        widget.animation!.removeStatusListener(_onAnimationStatusChanged);
        _disposeAnimations();
        base.dispose();
    }

    internal virtual void _onAnimationStatusChanged(global::Doroti.Framework.Animation.AnimationStatus animationStatus)
    {
        if (AnimationStatusMembers.isCompleted(animationStatus))
        {
            if ((widget.onVisible is not null) && !_wasVisible)
            {
                widget.onVisible!();
            }
            _wasVisible = true;
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        bool accessibleNavigation = MediaQuery.accessibleNavigationOf(context);
        DartRuntimePrimitives.Assert(() => widget.animation is not null);
        ThemeData theme = Theme.of(context);
        ColorScheme colorSchemeLocal = theme.colorScheme;
        SnackBarThemeData snackBarTheme = SnackBarTheme.of(context);
        var isThemeDark = Equals(theme.brightness, Brightness.dark);
        global::Doroti.Ui.Color buttonColor = isThemeDark ? colorSchemeLocal.primary : colorSchemeLocal.secondary;
        SnackBarThemeData defaults = new _SnackbarDefaultsM3__snack_bar(context);
        global::Doroti.Ui.Brightness brightnessLocal = isThemeDark ? Brightness.light : Brightness.dark;
        ThemeData effectiveTheme = theme;
        global::Doroti.Framework.Painting.TextStyle? contentTextStyleLocal = snackBarTheme.contentTextStyle ?? defaults.contentTextStyle;
        SnackBarBehavior snackBarBehavior = DartRuntimePrimitives.RequireValue(widget.behavior ?? snackBarTheme.behavior ?? defaults.behavior);
        double? widthLocal = widget.width ?? snackBarTheme.width;
        DartRuntimePrimitives.Assert(() =>
            {
                if (!Equals(snackBarBehavior, SnackBarBehavior.floating))
                {
                    string message(string parameter)
                    {
                        var prefix = $"{parameter} can only be used with floating behavior.";
                        if (widget.behavior is not null)
                        {
                            return $"{prefix} SnackBarBehavior.fixed was set in the SnackBar constructor.";
                        }
                        else
                        {
                            if (snackBarTheme.behavior is not null)
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
                    DartRuntimePrimitives.Assert(() => widget.margin is null, () => (object?)message("Margin"));
                    DartRuntimePrimitives.Assert(() => widthLocal is null, () => (object?)message("Width"));
                }
                return true;
            });
        bool showCloseIconLocal = (widget.showCloseIcon ?? snackBarTheme.showCloseIcon) ?? DartRuntimePrimitives.RequireValue(defaults.showCloseIcon);
        var isFloatingSnackBar = Equals(snackBarBehavior, SnackBarBehavior.floating);
        var horizontalPadding = isFloatingSnackBar ? 16.0 : 24.0;
        global::Doroti.Framework.Painting.EdgeInsetsGeometry paddingLocal = widget.padding ?? EdgeInsetsDirectional.CreateOnly(start: horizontalPadding, end: ((widget.action is not null) || showCloseIconLocal) ? 0 : horizontalPadding);
        double actionHorizontalMargin = (widget.padding?.resolve(TextDirection.ltr).right ?? horizontalPadding) / 2L;
        double iconHorizontalMargin = (widget.padding?.resolve(TextDirection.ltr).right ?? horizontalPadding) / 12.0;
        IconButton? iconButton = showCloseIconLocal ? new IconButton(key: StandardComponentTypeMembers.key(StandardComponentType.closeButton), icon: new global::Doroti.Framework.Widgets.Icon(Icons.close), iconSize: 24.0, color: (widget.closeIconColor ?? snackBarTheme.closeIconColor) ?? defaults.closeIconColor, onPressed: () => { ScaffoldMessenger.of(context).hideCurrentSnackBar(reason: SnackBarClosedReason.dismiss); }, tooltip: MaterialLocalizations.of(context).closeButtonTooltip) : null;
        var actionTextPainter = ((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = new global::Doroti.Framework.Painting.TextPainter(text: new global::Doroti.Framework.Painting.TextSpan(text: widget.action?.label ?? "", style: Theme.of(context).textTheme.labelLarge), maxLines: 1L, textDirection: TextDirection.ltr);
    __cascade.layout();
    return __cascade;
}))();
        double actionAndIconWidth = actionTextPainter.size.width + ((widget.action is not null) ? actionHorizontalMargin : 0L) + (showCloseIconLocal ? (iconButton?.iconSize ?? (0L + iconHorizontalMargin)) : 0L);
        actionTextPainter.dispose();
        global::Doroti.Framework.Painting.EdgeInsets marginLocal = (widget.margin?.resolve(TextDirection.ltr) ?? snackBarTheme.insetPadding) ?? defaults.insetPadding!;
        double snackBarWidth = widget.width ?? (MediaQuery.widthOf(context) - (marginLocal.left + marginLocal.right));
        double actionOverflowThresholdLocal = (widget.actionOverflowThreshold ?? snackBarTheme.actionOverflowThreshold) ?? DartRuntimePrimitives.RequireValue(defaults.actionOverflowThreshold);
        bool willOverflowAction = (actionAndIconWidth / snackBarWidth) > actionOverflowThresholdLocal;
        var maybeActionAndIcon = ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection27629 = new List<global::Doroti.Framework.Widgets.Widget>(); if (widget.action is not null) { __collection27629.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsets.CreateSymmetric(horizontal: actionHorizontalMargin), child: new TextButtonTheme(data: new TextButtonThemeData(style: TextButton.styleFrom(foregroundColor: buttonColor, padding: EdgeInsets.CreateSymmetric(horizontal: horizontalPadding))), child: widget.action!)))); } if (showCloseIconLocal) { __collection27629.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsets.CreateSymmetric(horizontal: iconHorizontalMargin), child: iconButton))); } return __collection27629; }))();
        global::Doroti.Framework.Widgets.Widget snackBar = new global::Doroti.Framework.Widgets.Padding(padding: paddingLocal, child: new global::Doroti.Framework.Widgets.Wrap(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection28354 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection28354.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Row(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection28401 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection28401.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.Padding(padding: (widget.padding is null) ? EdgeInsets.CreateSymmetric(vertical: Snack_barLibrary._singleLineVerticalPadding) : EdgeInsets.zero, child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: contentTextStyleLocal!, child: widget.content))))); if (!willOverflowAction) { __collection28401.AddRange(maybeActionAndIcon); } if (willOverflowAction) { __collection28401.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(width: snackBarWidth * 0.4))); } return __collection28401; }))()))); if (willOverflowAction) { __collection28354.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsets.CreateOnly(bottom: Snack_barLibrary._singleLineVerticalPadding), child: new global::Doroti.Framework.Widgets.Row(mainAxisAlignment: MainAxisAlignment.end, children: maybeActionAndIcon)))); } return __collection28354; }))()));
        if (!isFloatingSnackBar)
        {
            snackBar = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SafeArea(top: false, child: snackBar));
        }
        double elevationLocal = (widget.elevation ?? snackBarTheme.elevation) ?? DartRuntimePrimitives.RequireValue(defaults.elevation);
        global::Doroti.Ui.Color backgroundColorLocal = (widget.backgroundColor ?? snackBarTheme.backgroundColor) ?? defaults.backgroundColor!;
        global::Doroti.Framework.Painting.ShapeBorder? shapeLocal = (widget.shape ?? snackBarTheme.shape) ?? (isFloatingSnackBar ? defaults.shape : null);
        global::Doroti.Framework.Widgets.DismissDirection dismissDirectionLocal = (widget.dismissDirection ?? snackBarTheme.dismissDirection) ?? DismissDirection.down;
        snackBar = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new Material(shape: shapeLocal, elevation: elevationLocal, color: backgroundColorLocal, clipBehavior: widget.clipBehavior, child: new Theme(data: effectiveTheme, child: snackBar)));
        if (isFloatingSnackBar)
        {
            if (widthLocal is not null)
            {
                double width__24238__value30275 = DartRuntimePrimitives.RequireValue(widthLocal);
                snackBar = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsets.CreateOnly(top: marginLocal.top, bottom: marginLocal.bottom), child: new global::Doroti.Framework.Widgets.SizedBox(width: DartRuntimePrimitives.RequireValue(width__24238__value30275), child: snackBar)));
            }
            else
            {
                snackBar = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: marginLocal, child: snackBar));
            }
            snackBar = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SafeArea(top: false, bottom: false, child: snackBar));
        }
        snackBar = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(container: true, liveRegion: true, onDismiss: () =>
        {
            ScaffoldMessenger.of(context).removeCurrentSnackBar(reason: SnackBarClosedReason.dismiss);
        }, child: new global::Doroti.Framework.Widgets.Dismissible(key: _dismissibleKey, direction: dismissDirectionLocal, resizeDuration: null, behavior: widget.hitTestBehavior ?? (((widget.margin is not null) || (snackBarTheme.insetPadding is not null)) ? HitTestBehavior.deferToChild : HitTestBehavior.opaque), onDismissed: (direction) =>
        {
            ScaffoldMessenger.of(context).removeCurrentSnackBar(reason: SnackBarClosedReason.swipe);
        }, child: snackBar)));
        global::Doroti.Framework.Widgets.Widget snackBarTransition = default!;
        if (accessibleNavigation)
        {
            snackBarTransition = snackBar;
        }
        else
        {
            if (isFloatingSnackBar && false)
            {
                snackBarTransition = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.FadeTransition(opacity: _fadeInAnimation!, child: snackBar));
            }
            else
            {
                if (isFloatingSnackBar)
                {
                    snackBarTransition = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.FadeTransition(opacity: _fadeInM3Animation!, child: new global::Doroti.Framework.Widgets.ValueListenableBuilder<double>(valueListenable: _heightM3Animation!, builder: (context, value, child) =>
                    {
                        return new global::Doroti.Framework.Widgets.Align(alignment: Alignment.bottomLeft, heightFactor: value, child: child);
                        throw new InvalidOperationException("Dart closure completed without a value.");
                    }, child: snackBar)));
                }
                else
                {
                    snackBarTransition = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ValueListenableBuilder<double>(valueListenable: _heightAnimation!, builder: (context, value, child) =>
                    {
                        return new global::Doroti.Framework.Widgets.Align(alignment: AlignmentDirectional.topStart, heightFactor: value, child: child);
                        throw new InvalidOperationException("Dart closure completed without a value.");
                    }, child: snackBar));
                }
            }
        }
        return new global::Doroti.Framework.Widgets.Hero(tag: $"<SnackBar Hero tag - {widget.content}>", transitionOnUserGestures: true, child: new global::Doroti.Framework.Widgets.ClipRect(clipBehavior: widget.clipBehavior, child: snackBarTransition));
    }

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

    internal _SnackbarDefaultsM3__snack_bar(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public override global::Doroti.Ui.Color backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.inverseSurface);
    public override global::Doroti.Ui.Color actionTextColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(WidgetStateColor.CreateResolveWith((states) =>
    {
        if (states.Contains(WidgetState.disabled))
        {
            return _colors.inversePrimary;
        }
        if (states.Contains(WidgetState.pressed))
        {
            return _colors.inversePrimary;
        }
        if (states.Contains(WidgetState.hovered))
        {
            return _colors.inversePrimary;
        }
        if (states.Contains(WidgetState.focused))
        {
            return _colors.inversePrimary;
        }
        return _colors.inversePrimary;
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override global::Doroti.Ui.Color disabledActionTextColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.inversePrimary);
    public override global::Doroti.Framework.Painting.TextStyle contentTextStyle => Theme.of(context).textTheme.bodyMedium!.copyWith(color: _colors.onInverseSurface);
    public override double? elevation => 6.0;
    public override global::Doroti.Framework.Painting.ShapeBorder shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.ShapeBorder>(new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(4.0))));
    public override SnackBarBehavior? behavior => SnackBarBehavior.@fixed;
    public override global::Doroti.Framework.Painting.EdgeInsets insetPadding => new global::Doroti.Framework.Painting.EdgeInsets(15.0, 5.0, 15.0, 10.0);
    public override bool? showCloseIcon => false;
    public override global::Doroti.Ui.Color? closeIconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.onInverseSurface);
    public override double? actionOverflowThreshold => 0.25;
}
