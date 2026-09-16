// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/expansion_tile.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class Expansion_tileLibrary
{
    internal static Duration _kExpand = Duration.Create(milliseconds: 200L);
}

public delegate void ExpansionTileController();

public class ExpansionTile : StatefulWidget
{
    public virtual Widget? leading { get; private set; }
    public virtual Widget title { get; private set; } = default!;
    public virtual Widget? subtitle { get; private set; }
    public virtual System.Action<bool>? onExpansionChanged { get; private set; }
    public virtual List<Widget> children { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? collapsedBackgroundColor { get; private set; }
    public virtual Widget? trailing { get; private set; }
    public virtual bool showTrailingIcon { get; private set; } = default!;
    public virtual bool initiallyExpanded { get; private set; } = default!;
    public virtual bool maintainState { get; private set; } = default!;
    public virtual EdgeInsetsGeometry? tilePadding { get; private set; }
    public virtual AlignmentGeometry? expandedAlignment { get; private set; }
    public virtual CrossAxisAlignment? expandedCrossAxisAlignment { get; private set; }
    public virtual EdgeInsetsGeometry? childrenPadding { get; private set; }
    public virtual Color? iconColor { get; private set; }
    public virtual Color? collapsedIconColor { get; private set; }
    public virtual Color? textColor { get; private set; }
    public virtual Color? collapsedTextColor { get; private set; }
    public virtual ShapeBorder? shape { get; private set; }
    public virtual ShapeBorder? collapsedShape { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual ListTileControlAffinity? controlAffinity { get; private set; }
    public virtual ExpansibleController? controller { get; private set; }
    public virtual bool? dense { get; private set; }
    public virtual Color? splashColor { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual double? minTileHeight { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual bool enabled { get; private set; } = default!;
    public virtual AnimationStyle? expansionAnimationStyle { get; private set; }
    public virtual bool internalAddSemanticForOnTap { get; private set; } = default!;
    public virtual WidgetStatesController? statesController { get; private set; }

    public ExpansionTile(Key? key = null, Widget? leading = null, Widget title = default!, Widget? subtitle = null, System.Action<bool>? onExpansionChanged = null, List<Widget> children = default!, Widget? trailing = null, bool showTrailingIcon = true, bool initiallyExpanded = false, bool maintainState = false, EdgeInsetsGeometry? tilePadding = null, CrossAxisAlignment? expandedCrossAxisAlignment = null, AlignmentGeometry? expandedAlignment = null, EdgeInsetsGeometry? childrenPadding = null, Color? backgroundColor = null, Color? collapsedBackgroundColor = null, Color? textColor = null, Color? collapsedTextColor = null, Color? iconColor = null, Color? collapsedIconColor = null, ShapeBorder? shape = null, ShapeBorder? collapsedShape = null, Clip? clipBehavior = null, ListTileControlAffinity? controlAffinity = null, ExpansibleController? controller = null, bool? dense = null, Color? splashColor = null, VisualDensity? visualDensity = null, double? minTileHeight = null, bool? enableFeedback = true, bool enabled = true, AnimationStyle? expansionAnimationStyle = null, bool internalAddSemanticForOnTap = false, WidgetStatesController? statesController = null) : base(key: key)
    {
        List<Widget> __children = children ?? new List<Widget>();
        this.leading = leading;
        this.title = title;
        this.subtitle = subtitle;
        this.onExpansionChanged = onExpansionChanged;
        this.children = __children;
        this.trailing = trailing;
        this.showTrailingIcon = showTrailingIcon;
        this.initiallyExpanded = initiallyExpanded;
        this.maintainState = maintainState;
        this.tilePadding = tilePadding;
        this.expandedCrossAxisAlignment = expandedCrossAxisAlignment;
        this.expandedAlignment = expandedAlignment;
        this.childrenPadding = childrenPadding;
        this.backgroundColor = backgroundColor;
        this.collapsedBackgroundColor = collapsedBackgroundColor;
        this.textColor = textColor;
        this.collapsedTextColor = collapsedTextColor;
        this.iconColor = iconColor;
        this.collapsedIconColor = collapsedIconColor;
        this.shape = shape;
        this.collapsedShape = collapsedShape;
        this.clipBehavior = clipBehavior;
        this.controlAffinity = controlAffinity;
        this.controller = controller;
        this.dense = dense;
        this.splashColor = splashColor;
        this.visualDensity = visualDensity;
        this.minTileHeight = minTileHeight;
        this.enableFeedback = enableFeedback;
        this.enabled = enabled;
        this.expansionAnimationStyle = expansionAnimationStyle;
        this.internalAddSemanticForOnTap = internalAddSemanticForOnTap;
        this.statesController = statesController;
        System.Diagnostics.Debug.Assert(!Equals(expandedCrossAxisAlignment, CrossAxisAlignment.baseline));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ExpansionTileState__expansion_tile());
}

internal class _ExpansionTileState__expansion_tile : State<ExpansionTile>
{
    internal static Animatable<double> _easeInTween = new CurveTween(curve: Curves.easeIn);
    internal static Animatable<double> _easeOutTween = new CurveTween(curve: Curves.easeOut);
    internal static Animatable<double> _halfTween = new Tween<double>(begin: 0.0, end: 0.5);
    internal virtual ShapeBorderTween _borderTween { get; private set; } = new ShapeBorderTween();
    internal virtual ColorTween _headerColorTween { get; private set; } = new ColorTween();
    internal virtual ColorTween _iconColorTween { get; private set; } = new ColorTween();
    internal virtual ColorTween _backgroundColorTween { get; private set; } = new ColorTween();
    internal virtual Animation<double> _iconTurns { get; set; } = default!;
    internal virtual Animation<ShapeBorder?> _border { get; set; } = default!;
    internal virtual Animation<Color?> _headerColor { get; set; } = default!;
    internal virtual Animation<Color?> _iconColor { get; set; } = default!;
    internal virtual Animation<Color?> _backgroundColor { get; set; } = default!;
    internal virtual ExpansionTileThemeData _expansionTileTheme { get; set; } = default!;
    internal virtual ExpansibleController _tileController { get; set; } = default!;
    internal virtual Timer? _timer { get; set; } = default;
    internal virtual Curve _curve { get; set; } = default!;
    internal virtual Curve? _reverseCurve { get; set; } = default;
    internal virtual Duration _duration { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        _curve = DartRuntimePrimitives.ConvertValue<Curve>(Curves.easeIn);
        _duration = Expansion_tileLibrary._kExpand;
        _tileController = widget.controller ?? new ExpansibleController();
        if (widget.initiallyExpanded)
        {
            _tileController.expand();
        }
        _tileController.addListener(_onExpansionChanged);
    }

    public override void dispose()
    {
        _tileController.removeListener(_onExpansionChanged);
        if (widget.controller is null)
        {
            _tileController.dispose();
        }
        _timer?.cancel();
        _timer = null;
        base.dispose();
    }

    internal virtual void _onExpansionChanged()
    {
        TextDirection textDirectionLocal = WidgetsLocalizations.of(context).textDirection;
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        string stateHint = _tileController.isExpanded ? localizations.collapsedHint : localizations.expandedHint;
        if (Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.iOS))
        {
            _timer?.cancel();
            _timer = new Timer(Duration.Create(seconds: 1L), () =>
            {
                DartRuntimePrimitives.Ignore(SemanticsService.sendAnnouncement(View.of(context), stateHint, textDirectionLocal).catchError((exception, stack) =>
                {
                    FlutterError.reportError(new FlutterErrorDetails(exception: exception, stack: stack, library: "material library", context: new ErrorDescription("while sending semantics announcement")));
                }));
                _timer?.cancel();
                _timer = null;
            });
        }
        else
        {
            if (!Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.android))
            {
                DartRuntimePrimitives.Ignore(SemanticsService.sendAnnouncement(View.of(context), stateHint, textDirectionLocal).catchError((exception, stack) =>
                {
                    FlutterError.reportError(new FlutterErrorDetails(exception: exception, stack: stack, library: "material library", context: new ErrorDescription("while sending semantics announcement")));
                }));
            }
        }
        widget.onExpansionChanged?.Invoke(_tileController.isExpanded);
    }

    internal virtual ListTileControlAffinity _effectiveAffinity()
    {
        ListTileThemeData listTileTheme = ListTileTheme.of(context);
        ListTileControlAffinity affinity = (widget.controlAffinity ?? listTileTheme.controlAffinity) ?? ListTileControlAffinity.trailing;
        switch (affinity)
        {
            case ListTileControlAffinity.leading:
                {
                    return ListTileControlAffinity.leading;
                }
            case ListTileControlAffinity.trailing:
            case ListTileControlAffinity.platform:
                {
                    return ListTileControlAffinity.trailing;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget? _buildIcon(BuildContext context, Animation<double> animation)
    {
        _iconTurns = animation.drive(_halfTween.chain(_easeInTween));
        return (Widget?)new RotationTransition(turns: _iconTurns, child: new Icon(Icons.expand_more));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget? _buildLeadingIcon(BuildContext context, Animation<double> animation)
    {
        if (!Equals(_effectiveAffinity(), ListTileControlAffinity.leading))
        {
            return null;
        }
        return _buildIcon(context, animation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget? _buildTrailingIcon(BuildContext context, Animation<double> animation)
    {
        if (!Equals(_effectiveAffinity(), ListTileControlAffinity.trailing))
        {
            return null;
        }
        return _buildIcon(context, animation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildHeader(BuildContext context, Animation<double> animation)
    {
        _iconColor = animation.drive(_iconColorTween.chain(_easeInTween));
        _headerColor = animation.drive(_headerColorTween.chain(_easeInTween));
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        string onTapHintLocal = _tileController.isExpanded ? localizations.expansionTileExpandedTapHint : localizations.expansionTileCollapsedTapHint;
        string semanticsHint = PlatformLibrary.defaultTargetPlatform switch { TargetPlatform.iOS => _tileController.isExpanded ? $"{localizations.collapsedHint}\n {localizations.expansionTileExpandedHint}" : $"{localizations.expandedHint}\n {localizations.expansionTileCollapsedHint}", TargetPlatform.macOS => _tileController.isExpanded ? $"{localizations.collapsedHint}\n {localizations.expansionTileExpandedHint}" : $"{localizations.expandedHint}\n {localizations.expansionTileCollapsedHint}", _ => _tileController.isExpanded ? localizations.collapsedHint : localizations.expandedHint };
        Widget childLocal = ListTileTheme.merge(iconColor: _iconColor.value ?? _expansionTileTheme.iconColor, textColor: _headerColor.value, child: new ListTile(enabled: widget.enabled, onTap: _tileController.isExpanded ? _tileController.collapse : _tileController.expand, dense: widget.dense, splashColor: widget.splashColor, visualDensity: widget.visualDensity, enableFeedback: widget.enableFeedback, contentPadding: widget.tilePadding ?? _expansionTileTheme.tilePadding, leading: widget.leading ?? _buildLeadingIcon(context, animation), title: widget.title, subtitle: widget.subtitle, trailing: widget.showTrailingIcon ? (widget.trailing ?? _buildTrailingIcon(context, animation)) : null, minTileHeight: widget.minTileHeight, internalAddSemanticForOnTap: widget.internalAddSemanticForOnTap, statesController: widget.statesController));
        if (Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.android))
        {
            return new Widgets.Semantics(label: semanticsHint, liveRegion: true, accessibilityFocusBlockType: AccessibilityFocusBlockType.blockNode, child: new Widgets.Semantics(hint: semanticsHint, onTapHint: onTapHintLocal, child: childLocal));
        }
        return new Widgets.Semantics(hint: semanticsHint, onTapHint: onTapHintLocal, child: childLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildBody(BuildContext context, Animation<double> animation)
    {
        return new Align(alignment: (widget.expandedAlignment ?? _expansionTileTheme.expandedAlignment) ?? Alignment.center, child: new Padding(padding: (widget.childrenPadding ?? _expansionTileTheme.childrenPadding) ?? EdgeInsets.zero, child: new Column(crossAxisAlignment: widget.expandedCrossAxisAlignment ?? CrossAxisAlignment.center, children: widget.children)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildExpansible(BuildContext context, Widget header, Widget body, Animation<double> animation)
    {
        _backgroundColor = animation.drive(_backgroundColorTween.chain(_easeOutTween));
        _border = animation.drive(_borderTween.chain(_easeOutTween));
        Color backgroundColorLocal = (_backgroundColor.value ?? _expansionTileTheme.backgroundColor) ?? Colors.transparent;
        ShapeBorder expansionTileBorder = _border.value ?? new Border(top: new BorderSide(color: Colors.transparent), bottom: new BorderSide(color: Colors.transparent));
        Clip clipBehaviorLocal = (widget.clipBehavior ?? _expansionTileTheme.clipBehavior) ?? Clip.antiAlias;
        Decoration decorationLocal = new ShapeDecoration(color: backgroundColorLocal, shape: expansionTileBorder);
        Widget tile = new Padding(padding: decorationLocal.padding, child: new Column(mainAxisSize: MainAxisSize.min, children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(header), DartRuntimePrimitives.ConvertValue<Widget>(body) }));
        bool isShapeProvided = (widget.shape is not null) || (_expansionTileTheme.shape is not null) || (widget.collapsedShape is not null) || (_expansionTileTheme.collapsedShape is not null);
        if (isShapeProvided)
        {
            return new Material(clipBehavior: clipBehaviorLocal, color: backgroundColorLocal, shape: expansionTileBorder, child: tile);
        }
        if (backgroundColorLocal.a > 0L)
        {
            tile = DartRuntimePrimitives.ConvertValue<Widget>(new Material(type: MaterialType.transparency, child: tile));
        }
        return new DecoratedBox(decoration: decorationLocal, child: tile);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void didUpdateWidget(ExpansionTile oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        ThemeData theme = Theme.of(context);
        _expansionTileTheme = ExpansionTileTheme.of(context);
        ExpansionTileThemeData defaults = new _ExpansionTileDefaultsM3__expansion_tile(context);
        if ((!Equals(widget.collapsedShape, oldWidget.collapsedShape)) || (!Equals(widget.shape, oldWidget.shape)))
        {
            _updateShapeBorder(theme);
        }
        if ((!Equals(widget.collapsedTextColor, oldWidget.collapsedTextColor)) || (!Equals(widget.textColor, oldWidget.textColor)))
        {
            _updateHeaderColor(defaults);
        }
        if ((!Equals(widget.collapsedIconColor, oldWidget.collapsedIconColor)) || (!Equals(widget.iconColor, oldWidget.iconColor)))
        {
            _updateIconColor(defaults);
        }
        if ((!Equals(widget.backgroundColor, oldWidget.backgroundColor)) || (!Equals(widget.collapsedBackgroundColor, oldWidget.collapsedBackgroundColor)))
        {
            _updateBackgroundColor();
        }
        if (!Equals(widget.expansionAnimationStyle, oldWidget.expansionAnimationStyle))
        {
            _updateAnimationDuration();
            _updateHeightFactorCurve();
        }
        if (!Equals(widget.controller, oldWidget.controller))
        {
            _tileController.removeListener(_onExpansionChanged);
            if (oldWidget.controller is null)
            {
                _tileController.dispose();
            }
            _tileController = widget.controller ?? new ExpansibleController();
            _tileController.addListener(_onExpansionChanged);
        }
    }

    public override void didChangeDependencies()
    {
        ThemeData theme = Theme.of(context);
        _expansionTileTheme = ExpansionTileTheme.of(context);
        ExpansionTileThemeData defaults = new _ExpansionTileDefaultsM3__expansion_tile(context);
        _updateAnimationDuration();
        _updateShapeBorder(theme);
        _updateHeaderColor(defaults);
        _updateIconColor(defaults);
        _updateBackgroundColor();
        _updateHeightFactorCurve();
        base.didChangeDependencies();
    }

    internal virtual void _updateAnimationDuration()
    {
        _duration = (widget.expansionAnimationStyle?.duration ?? _expansionTileTheme.expansionAnimationStyle?.duration) ?? Duration.Create(milliseconds: 200L);
    }

    internal virtual void _updateShapeBorder(ThemeData theme)
    {
        DartRuntimePrimitives.Ignore(((Func<ShapeBorderTween>)(() =>
{
    var __cascade = _borderTween;
    __cascade.begin = (widget.collapsedShape ?? _expansionTileTheme.collapsedShape) ?? new Border(top: new BorderSide(color: Colors.transparent), bottom: new BorderSide(color: Colors.transparent));
    __cascade.end = (widget.shape ?? _expansionTileTheme.shape) ?? new Border(top: new BorderSide(color: theme.dividerColor), bottom: new BorderSide(color: theme.dividerColor));
    return __cascade;
}))());
    }

    internal virtual void _updateHeaderColor(ExpansionTileThemeData defaults)
    {
        DartRuntimePrimitives.Ignore(((Func<ColorTween>)(() =>
{
    var __cascade = _headerColorTween;
    __cascade.begin = (widget.collapsedTextColor ?? _expansionTileTheme.collapsedTextColor) ?? defaults.collapsedTextColor;
    __cascade.end = (widget.textColor ?? _expansionTileTheme.textColor) ?? defaults.textColor;
    return __cascade;
}))());
    }

    internal virtual void _updateIconColor(ExpansionTileThemeData defaults)
    {
        DartRuntimePrimitives.Ignore(((Func<ColorTween>)(() =>
{
    var __cascade = _iconColorTween;
    __cascade.begin = (widget.collapsedIconColor ?? _expansionTileTheme.collapsedIconColor) ?? defaults.collapsedIconColor;
    __cascade.end = (widget.iconColor ?? _expansionTileTheme.iconColor) ?? defaults.iconColor;
    return __cascade;
}))());
    }

    internal virtual void _updateBackgroundColor()
    {
        DartRuntimePrimitives.Ignore(((Func<ColorTween>)(() =>
{
    var __cascade = _backgroundColorTween;
    __cascade.begin = widget.collapsedBackgroundColor ?? _expansionTileTheme.collapsedBackgroundColor;
    __cascade.end = widget.backgroundColor ?? _expansionTileTheme.backgroundColor;
    return __cascade;
}))());
    }

    internal virtual void _updateHeightFactorCurve()
    {
        _curve = (widget.expansionAnimationStyle?.curve ?? _expansionTileTheme.expansionAnimationStyle?.curve) ?? Curves.easeIn;
        _reverseCurve = widget.expansionAnimationStyle?.reverseCurve ?? _expansionTileTheme.expansionAnimationStyle?.reverseCurve;
    }

    public override Widget build(BuildContext context)
    {
        return new Expansible(controller: _tileController, curve: _curve, duration: _duration, reverseCurve: _reverseCurve, maintainState: widget.maintainState, headerBuilder: _buildHeader, bodyBuilder: _buildBody, expansibleBuilder: _buildExpansible);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ExpansionTileDefaultsM3__expansion_tile : ExpansionTileThemeData
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

    internal _ExpansionTileDefaultsM3__expansion_tile(BuildContext context)
    {
        this.context = context;
    }

    public override Color? textColor => DartRuntimePrimitives.ConvertValue<Color>(_colors.onSurface);
    public override Color? iconColor => DartRuntimePrimitives.ConvertValue<Color>(_colors.primary);
    public override Color? collapsedTextColor => DartRuntimePrimitives.ConvertValue<Color>(_colors.onSurface);
    public override Color? collapsedIconColor => DartRuntimePrimitives.ConvertValue<Color>(_colors.onSurfaceVariant);
}
