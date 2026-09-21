// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/banner.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class BannerLibrary
{
    internal static Duration _materialBannerTransitionDuration = Duration.Create(
        milliseconds: 250L
    );
}

public static partial class BannerLibrary
{
    internal static Curve _materialBannerHeightCurve = Curves.fastOutSlowIn;
}

public static partial class BannerLibrary
{
    internal static double _kMaxContentTextScaleFactor = 1.5;
}

public enum MaterialBannerClosedReason
{
    dismiss,
    swipe,
    hide,
    remove,
}

public class MaterialBanner : StatefulWidget
{
    public virtual Widget content { get; private set; } = default!;
    public virtual TextStyle? contentTextStyle { get; private set; }
    public virtual List<Widget> actions { get; private set; } = default!;
    public virtual double? elevation { get; private set; }
    public virtual Widget? leading { get; private set; }
    public virtual double minActionBarHeight { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? dividerColor { get; private set; }
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual EdgeInsetsGeometry? margin { get; private set; }
    public virtual EdgeInsetsGeometry? leadingPadding { get; private set; }
    public virtual bool forceActionsBelow { get; private set; } = default!;
    public virtual OverflowBarAlignment overflowAlignment { get; private set; } = default!;
    public virtual Animation<double>? animation { get; private set; }
    public virtual Action? onVisible { get; private set; }

    public MaterialBanner(
        Key? key = null,
        Widget content = default!,
        TextStyle? contentTextStyle = null,
        List<Widget> actions = default!,
        double? elevation = null,
        Widget? leading = null,
        Color? backgroundColor = null,
        Color? surfaceTintColor = null,
        Color? shadowColor = null,
        Color? dividerColor = null,
        EdgeInsetsGeometry? padding = null,
        EdgeInsetsGeometry? margin = null,
        EdgeInsetsGeometry? leadingPadding = null,
        bool forceActionsBelow = false,
        OverflowBarAlignment overflowAlignment = OverflowBarAlignment.end,
        Animation<double>? animation = null,
        Action? onVisible = null,
        double minActionBarHeight = 52.0
    )
        : base(key: key)
    {
        this.content = content;
        this.contentTextStyle = contentTextStyle;
        this.actions = actions;
        this.elevation = elevation;
        this.leading = leading;
        this.backgroundColor = backgroundColor;
        this.surfaceTintColor = surfaceTintColor;
        this.shadowColor = shadowColor;
        this.dividerColor = dividerColor;
        this.padding = padding;
        this.margin = margin;
        this.leadingPadding = leadingPadding;
        this.forceActionsBelow = forceActionsBelow;
        this.overflowAlignment = overflowAlignment;
        this.animation = animation;
        this.onVisible = onVisible;
        this.minActionBarHeight = minActionBarHeight;
        System.Diagnostics.Debug.Assert((elevation is null) || (elevation >= 0.0));
    }

    public static AnimationController createAnimationController(Scheduler.TickerProvider vsync)
    {
        return new AnimationController(
            duration: BannerLibrary._materialBannerTransitionDuration,
            debugLabel: "MaterialBanner",
            vsync: vsync
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual MaterialBanner withAnimation(
        Animation<double> newAnimation,
        Key? fallbackKey = null
    )
    {
        return new MaterialBanner(
            key: key ?? fallbackKey,
            content: content,
            contentTextStyle: contentTextStyle,
            actions: actions,
            elevation: elevation,
            leading: leading,
            minActionBarHeight: minActionBarHeight,
            backgroundColor: backgroundColor,
            surfaceTintColor: surfaceTintColor,
            shadowColor: shadowColor,
            dividerColor: dividerColor,
            padding: padding,
            margin: margin,
            leadingPadding: leadingPadding,
            forceActionsBelow: forceActionsBelow,
            overflowAlignment: overflowAlignment,
            animation: newAnimation,
            onVisible: onVisible
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _MaterialBannerState__banner());
}

internal class _MaterialBannerState__banner : State<MaterialBanner>
{
    internal virtual bool _wasVisible { get; set; } = false;
    internal virtual CurvedAnimation? _heightAnimation { get; set; } = default;
    internal virtual CurvedAnimation? _slideOutCurvedAnimation { get; set; } = default;

    public override void initState()
    {
        base.initState();
        widget.animation?.addStatusListener(_onAnimationStatusChanged);
        _setCurvedAnimations();
    }

    public override void didUpdateWidget(MaterialBanner oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.animation, oldWidget.animation))
        {
            oldWidget.animation?.removeStatusListener(_onAnimationStatusChanged);
            widget.animation?.addStatusListener(_onAnimationStatusChanged);
            _setCurvedAnimations();
        }
    }

    internal virtual void _setCurvedAnimations()
    {
        _heightAnimation?.dispose();
        _slideOutCurvedAnimation?.dispose();
        if (widget.animation is not null)
        {
            _heightAnimation = new CurvedAnimation(
                parent: widget.animation!,
                curve: BannerLibrary._materialBannerHeightCurve
            );
            _slideOutCurvedAnimation = new CurvedAnimation(
                parent: widget.animation!,
                curve: new Threshold(0.0)
            );
        }
        else
        {
            _heightAnimation = null;
            _slideOutCurvedAnimation = null;
        }
    }

    public override void dispose()
    {
        widget.animation?.removeStatusListener(_onAnimationStatusChanged);
        _heightAnimation?.dispose();
        _slideOutCurvedAnimation?.dispose();
        base.dispose();
    }

    internal virtual void _onAnimationStatusChanged(AnimationStatus status)
    {
        if (AnimationStatusMembers.isCompleted(status))
        {
            if ((widget.onVisible is not null) && !_wasVisible)
            {
                widget.onVisible!();
            }
            _wasVisible = true;
        }
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        bool accessibleNavigation = MediaQuery.accessibleNavigationOf(context);
        DartRuntimePrimitives.Assert(() => Enumerable.Any(widget.actions));
        ThemeData theme = Theme.of(context);
        MaterialBannerThemeData bannerTheme = MaterialBannerTheme.of(context);
        MaterialBannerThemeData defaults = new _BannerDefaultsM3__banner(context);
        bool isSingleRow = (checked(widget.actions.Count) == 1L) && !widget.forceActionsBelow;
        EdgeInsetsGeometry paddingLocal =
            (widget.padding ?? bannerTheme.padding)
            ?? (
                isSingleRow
                    ? EdgeInsetsDirectional.CreateOnly(start: 16.0, top: 2.0)
                    : EdgeInsetsDirectional.CreateOnly(
                        start: 16.0,
                        top: 24.0,
                        end: 16.0,
                        bottom: 4.0
                    )
            );
        EdgeInsetsGeometry leadingPaddingLocal =
            (widget.leadingPadding ?? bannerTheme.leadingPadding)
            ?? EdgeInsetsDirectional.CreateOnly(end: 16.0);
        Widget actionsBar = new ConstrainedBox(
            constraints: new BoxConstraints(minHeight: widget.minActionBarHeight),
            child: new Padding(
                padding: EdgeInsets.CreateSymmetric(horizontal: 8),
                child: new Align(
                    alignment: AlignmentDirectional.centerEnd,
                    child: new OverflowBar(
                        overflowAlignment: widget.overflowAlignment,
                        spacing: 8,
                        children: widget.actions
                    )
                )
            )
        );
        double elevationLocal = (widget.elevation ?? bannerTheme.elevation) ?? 0.0;
        EdgeInsetsGeometry marginLocal =
            widget.margin ?? EdgeInsets.CreateOnly(bottom: (elevationLocal > 0L) ? 10.0 : 0.0);
        Color backgroundColorLocal =
            (widget.backgroundColor ?? bannerTheme.backgroundColor) ?? defaults.backgroundColor!;
        Color? surfaceTintColorLocal =
            (widget.surfaceTintColor ?? bannerTheme.surfaceTintColor) ?? defaults.surfaceTintColor;
        Color? shadowColorLocal = widget.shadowColor ?? bannerTheme.shadowColor;
        Color? dividerColorLocal =
            (widget.dividerColor ?? bannerTheme.dividerColor) ?? defaults.dividerColor;
        TextStyle? textStyle =
            (widget.contentTextStyle ?? bannerTheme.contentTextStyle) ?? defaults.contentTextStyle;
        Widget materialBanner = new Padding(
            padding: marginLocal,
            child: new Material(
                elevation: elevationLocal,
                color: backgroundColorLocal,
                surfaceTintColor: surfaceTintColorLocal,
                shadowColor: shadowColorLocal,
                child: new Column(
                    mainAxisSize: MainAxisSize.min,
                    children: (
                        (Func<List<Widget>>)(
                            () =>
                            {
                                var __collection14138 = new List<Widget>();
                                __collection14138.Add(
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        new Padding(
                                            padding: paddingLocal,
                                            child: new Row(
                                                children: (
                                                    (Func<List<Widget>>)(
                                                        () =>
                                                        {
                                                            var __collection14253 =
                                                                new List<Widget>();
                                                            if (widget.leading is not null)
                                                            {
                                                                __collection14253.Add(
                                                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                                                        new Padding(
                                                                            padding: leadingPaddingLocal,
                                                                            child: widget.leading
                                                                        )
                                                                    )
                                                                );
                                                            }
                                                            __collection14253.Add(
                                                                DartRuntimePrimitives.ConvertValue<Widget>(
                                                                    MediaQuery.withClampedTextScaling(
                                                                        maxScaleFactor: BannerLibrary._kMaxContentTextScaleFactor,
                                                                        child: new Expanded(
                                                                            child: new DefaultTextStyle(
                                                                                style: textStyle!,
                                                                                child: widget.content
                                                                            )
                                                                        )
                                                                    )
                                                                )
                                                            );
                                                            if (isSingleRow)
                                                            {
                                                                __collection14253.Add(
                                                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                                                        MediaQuery.withClampedTextScaling(
                                                                            maxScaleFactor: BannerLibrary._kMaxContentTextScaleFactor,
                                                                            child: actionsBar
                                                                        )
                                                                    )
                                                                );
                                                            }
                                                            return __collection14253;
                                                        }
                                                    )
                                                )()
                                            )
                                        )
                                    )
                                );
                                if (!isSingleRow)
                                {
                                    __collection14138.Add(
                                        DartRuntimePrimitives.ConvertValue<Widget>(actionsBar)
                                    );
                                }
                                if (elevationLocal == 0L)
                                {
                                    __collection14138.Add(
                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                            new Divider(height: 0, color: dividerColorLocal)
                                        )
                                    );
                                }
                                return __collection14138;
                            }
                        )
                    )()
                )
            )
        );
        if (widget.animation is null)
        {
            return materialBanner;
        }
        materialBanner = DartRuntimePrimitives.ConvertValue<Widget>(
            new SafeArea(child: materialBanner)
        );
        Animation<Offset> slideOutAnimation = new Tween<Offset>(
            begin: new Offset(0.0, -1.0),
            end: Offset.zero
        ).animate(_slideOutCurvedAnimation!);
        materialBanner = DartRuntimePrimitives.ConvertValue<Widget>(
            new Widgets.Semantics(
                container: true,
                liveRegion: true,
                onDismiss: () =>
                {
                    ScaffoldMessenger
                        .of(context)
                        .removeCurrentMaterialBanner(reason: MaterialBannerClosedReason.dismiss);
                },
                child: accessibleNavigation
                    ? materialBanner
                    : new SlideTransition(position: slideOutAnimation, child: materialBanner)
            )
        );
        Widget materialBannerTransition = default!;
        if (accessibleNavigation)
        {
            materialBannerTransition = materialBanner;
        }
        else
        {
            materialBannerTransition = DartRuntimePrimitives.ConvertValue<Widget>(
                new AnimatedBuilder(
                    animation: _heightAnimation!,
                    builder: (context, child) =>
                    {
                        return new Align(
                            alignment: AlignmentDirectional.bottomStart,
                            heightFactor: _heightAnimation!.value,
                            child: child
                        );
                        throw new InvalidOperationException(
                            "Callback completed without returning a value."
                        );
                    },
                    child: materialBanner
                )
            );
        }
        return new Hero(
            tag: $"<MaterialBanner Hero tag - {widget.content}>",
            child: new ClipRect(child: materialBannerTransition)
        );
    }
}

internal class _BannerDefaultsM3__banner : MaterialBannerThemeData
{
    public virtual BuildContext context { get; private set; } = default!;
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = Theme.of(context).colorScheme;
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
                __late__textTheme = Theme.of(context).textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }

    internal _BannerDefaultsM3__banner(BuildContext context)
        : base(elevation: 1.0)
    {
        this.context = context;
    }

    public override Color? backgroundColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.surfaceContainerLow);
    public override Color? surfaceTintColor =>
        DartRuntimePrimitives.ConvertValue<Color>(Colors.transparent);
    public override Color? dividerColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.outlineVariant);
    public override TextStyle? contentTextStyle => _textTheme.bodyMedium;
}
