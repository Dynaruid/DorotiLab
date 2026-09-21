// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/popup_menu.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class Popup_menuLibrary
{
    internal static Duration _kMenuDuration = Duration.Create(milliseconds: 300L);
}

public static partial class Popup_menuLibrary
{
    internal static double _kMenuCloseIntervalEnd = 2.0 / 3.0;
}

public static partial class Popup_menuLibrary
{
    internal static double _kMenuDividerHeight = 16.0;
}

public static partial class Popup_menuLibrary
{
    internal const double _kMenuMaxWidth = 5.0 * _kMenuWidthStep;
}

public static partial class Popup_menuLibrary
{
    internal const double _kMenuMinWidth = 2.0 * _kMenuWidthStep;
}

public static partial class Popup_menuLibrary
{
    internal const double _kMenuWidthStep = 56.0;
}

public static partial class Popup_menuLibrary
{
    internal static double _kMenuScreenPadding = 8.0;
}

public abstract class PopupMenuEntry<T> : StatefulWidget
{
    protected PopupMenuEntry(Key? key = null)
        : base(key: key) { }

    public abstract double height { get; }
    public abstract bool represents(T? value);
}

public class PopupMenuDivider : PopupMenuEntry<dynamic>
{
    private double __field_height = default!;
    public override double height
    {
        get => __field_height;
    }
    public virtual double? thickness { get; private set; }
    public virtual double? indent { get; private set; }
    public virtual double? endIndent { get; private set; }
    public virtual BorderRadiusGeometry? radius { get; private set; }
    public virtual Color? color { get; private set; }

    public PopupMenuDivider(
        Key? key = null,
        double? height = null,
        double? thickness = null,
        double? indent = null,
        double? endIndent = null,
        BorderRadiusGeometry? radius = null,
        Color? color = null
    )
        : base(key: key)
    {
        double __height = height ?? Popup_menuLibrary._kMenuDividerHeight;
        __field_height = __height;
        this.thickness = thickness;
        this.indent = indent;
        this.endIndent = endIndent;
        this.radius = radius;
        this.color = color;
    }

    public override bool represents(dynamic? value) => false;

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _PopupMenuDividerState__popup_menu());
}

internal class _PopupMenuDividerState__popup_menu : State<PopupMenuDivider>
{
    public override Widget build(BuildContext context)
    {
        return new Divider(
            height: widget.height,
            thickness: widget.thickness,
            indent: widget.indent,
            color: widget.color,
            endIndent: widget.endIndent,
            radius: widget.radius
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _MenuItem__popup_menu : SingleChildRenderObjectWidget
{
    public virtual Action<Size> onLayout { get; private set; } = default!;

    internal _MenuItem__popup_menu(Action<Size> onLayout, Widget? child)
        : base(child: child)
    {
        this.onLayout = onLayout;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderMenuItem__popup_menu(onLayout);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderMenuItem__popup_menu)renderObject;
        __renderObject.onLayout = onLayout;
    }
}

public class _RenderMenuItem__popup_menu : RenderShiftedBox
{
    public virtual Action<Size> onLayout { get; set; } = default!;

    internal _RenderMenuItem__popup_menu(Action<Size> onLayout, RenderBox? child = null)
        : base(child)
    {
        this.onLayout = onLayout;
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return child?.getDryLayout(constraints) ?? Size.zero;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        return child?.getDryBaseline(constraints, baseline);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void performLayout()
    {
        if (child is null)
        {
            size = Size.zero;
        }
        else
        {
            child!.layout(constraints, parentUsesSize: true);
            size = constraints.constrain(child!.size);
            var childParentData = ((BoxParentData?)child!.parentData!)!;
            childParentData.offset = Offset.zero;
        }
        onLayout(size);
    }
}

public class PopupMenuItem<T> : PopupMenuEntry<T>
{
    public virtual T? value { get; private set; }
    public virtual Action? onTap { get; private set; }
    public virtual bool enabled { get; private set; } = default!;
    private double __field_height = default!;
    public override double height
    {
        get => __field_height;
    }
    public virtual EdgeInsets? padding { get; private set; }
    public virtual TextStyle? textStyle { get; private set; }
    public virtual WidgetStateProperty<TextStyle?>? labelTextStyle { get; private set; }
    public virtual MouseCursor? mouseCursor { get; private set; }
    public virtual Widget? child { get; private set; }

    public PopupMenuItem(
        Key? key = null,
        T? value = default,
        Action? onTap = null,
        bool enabled = true,
        double? height = null,
        EdgeInsets? padding = null,
        TextStyle? textStyle = null,
        WidgetStateProperty<TextStyle?>? labelTextStyle = null,
        MouseCursor? mouseCursor = null,
        Widget? child = default!
    )
        : base(key: key)
    {
        double __height = height ?? ConstantsLibrary.kMinInteractiveDimension;
        this.value = value;
        this.onTap = onTap;
        this.enabled = enabled;
        __field_height = __height;
        this.padding = padding;
        this.textStyle = textStyle;
        this.labelTextStyle = labelTextStyle;
        this.mouseCursor = mouseCursor;
        this.child = child;
    }

    public override bool represents(T? value) =>
        DartRuntimePrimitives.ConvertValue<bool>(
            EqualityComparer<T>.Default.Equals(value, this.value)
        );

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new PopupMenuItemState<T, PopupMenuItem<T>>());
}

public class PopupMenuItemState<T, W> : State<W>
    where W : PopupMenuItem<T>
{
    public virtual Widget? buildChild() => widget.child;

    public virtual void handleTap()
    {
        Navigator.pop(context, widget.value);
        widget.onTap?.Invoke();
    }

    public override Widget build(BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        PopupMenuThemeData popupMenuTheme = PopupMenuTheme.of(context);
        PopupMenuThemeData defaults = new _PopupMenuDefaultsM3__popup_menu(context);
        var states = (
            (Func<HashSet<WidgetState>>)(
                () =>
                {
                    var __collection14434 = new HashSet<WidgetState>();
                    if (!widget.enabled)
                    {
                        __collection14434.Add(WidgetState.disabled);
                    }
                    return __collection14434;
                }
            )
        )();
        TextStyle styleLocal =
            (
                widget.labelTextStyle?.resolve(states)
                ?? popupMenuTheme.labelTextStyle?.resolve(states)!
            ) ?? defaults.labelTextStyle!.resolve(states)!;
        if (!widget.enabled && false)
        {
            styleLocal = styleLocal.copyWith(color: theme.disabledColor);
        }
        EdgeInsetsGeometry paddingLocal =
            widget.padding ?? _PopupMenuDefaultsM3__popup_menu.menuItemPadding;
        Widget item = new AnimatedDefaultTextStyle(
            style: styleLocal,
            duration: ConstantsLibrary.kThemeChangeDuration,
            child: new ConstrainedBox(
                constraints: new BoxConstraints(minHeight: widget.height),
                child: new Padding(
                    padding: paddingLocal,
                    child: new Align(
                        alignment: AlignmentDirectional.centerStart,
                        child: buildChild()
                    )
                )
            )
        );
        if (!widget.enabled)
        {
            var isDark = Equals(theme.brightness, Brightness.dark);
            item = IconTheme.merge(
                data: new IconThemeData(opacity: isDark ? 0.5 : 0.38),
                child: item
            );
        }
        return new MergeSemantics(
            child: buildSemantics(
                child: new InkWell(
                    onTap: widget.enabled ? handleTap : null,
                    canRequestFocus: widget.enabled,
                    mouseCursor: new _EffectiveMouseCursor__popup_menu(
                        widget.mouseCursor,
                        popupMenuTheme.mouseCursor
                    ),
                    child: ListTileTheme.merge(
                        contentPadding: EdgeInsets.zero,
                        titleTextStyle: styleLocal,
                        child: item
                    )
                )
            )
        );
    }

    public virtual Widget buildSemantics(Widget child)
    {
        return new Widgets.Semantics(
            role: SemanticsRole.menuItem,
            enabled: widget.enabled,
            button: true,
            child: child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class CheckedPopupMenuItem<T> : PopupMenuItem<T>
{
    public virtual bool @checked { get; private set; } = default!;

    public CheckedPopupMenuItem(
        Key? key = null,
        T? value = default,
        bool @checked = false,
        bool enabled = true,
        EdgeInsets? padding = null,
        double? height = null,
        WidgetStateProperty<TextStyle?>? labelTextStyle = null,
        MouseCursor? mouseCursor = null,
        Widget? child = null,
        Action? onTap = null
    )
        : base(
            key: key,
            value: value,
            enabled: enabled,
            padding: padding,
            height: height ?? ConstantsLibrary.kMinInteractiveDimension,
            labelTextStyle: labelTextStyle,
            mouseCursor: mouseCursor,
            child: child,
            onTap: onTap
        )
    {
        this.@checked = @checked;
    }

    public override Widget? child => base.child;

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _CheckedPopupMenuItemState__popup_menu<T>());
}

internal class _CheckedPopupMenuItemState__popup_menu<T>
    : PopupMenuItemState<T, CheckedPopupMenuItem<T>>,
        SingleTickerProviderStateMixin<CheckedPopupMenuItem<T>>
{
    internal static Duration _fadeDuration = Duration.Create(milliseconds: 150L);
    internal virtual AnimationController _controller { get; set; } = default!;
    public virtual Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual Animation<double> _opacity => _controller.view;

    public override void initState()
    {
        base.initState();
        _controller = (
            (Func<AnimationController>)(
                () =>
                {
                    var __cascade = new AnimationController(duration: _fadeDuration, vsync: this);
                    __cascade.value = widget.@checked ? 1.0 : 0.0;
                    __cascade.addListener(() =>
                    {
                        setState(() => { });
                    });
                    return __cascade;
                }
            )
        )();
    }

    public override void dispose()
    {
        _controller.dispose();
        DartRuntimePrimitives.Assert(() =>
        {
            if ((_ticker is null) || !_ticker!.isActive)
            {
                return true;
            }
            throw DartRuntimePrimitives.AsException(
                new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary($"{this} was disposed with an active Ticker."),
                        new ErrorDescription(
                            $"{GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time "
                                + "dispose() was called on the mixin, that Ticker was still active. The Ticker must "
                                + "be disposed before calling super.dispose()."
                        ),
                        new ErrorHint(
                            "Tickers used by AnimationControllers "
                                + "should be disposed by calling dispose() on the AnimationController itself. "
                                + "Otherwise, the ticker will leak."
                        ),
                        _ticker!.describeForError("The offending ticker was"),
                    }
                )
            );
        });
        _tickerModeNotifier?.removeListener(_updateTicker);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override void handleTap()
    {
        if (widget.@checked)
        {
            _controller.reverse();
        }
        else
        {
            _controller.forward();
        }
        base.handleTap();
    }

    public override Widget buildSemantics(Widget child)
    {
        return new Widgets.Semantics(
            role: SemanticsRole.menuItemCheckbox,
            enabled: widget.enabled,
            @checked: widget.@checked,
            button: true,
            child: child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget? buildChild()
    {
        ThemeData theme = Theme.of(context);
        PopupMenuThemeData popupMenuTheme = PopupMenuTheme.of(context);
        PopupMenuThemeData defaults = new _PopupMenuDefaultsM3__popup_menu(context);
        var states = (
            (Func<HashSet<WidgetState>>)(
                () =>
                {
                    var __collection22101 = new HashSet<WidgetState>();
                    if (widget.@checked)
                    {
                        __collection22101.Add(WidgetState.selected);
                    }
                    return __collection22101;
                }
            )
        )();
        WidgetStateProperty<TextStyle?>? effectiveLabelTextStyle =
            (widget.labelTextStyle ?? popupMenuTheme.labelTextStyle) ?? defaults.labelTextStyle;
        return (Widget?)
            new IgnorePointer(
                child: ListTileTheme.merge(
                    contentPadding: EdgeInsets.zero,
                    child: new ListTile(
                        enabled: widget.enabled,
                        titleTextStyle: effectiveLabelTextStyle?.resolve(states),
                        leading: new FadeTransition(
                            opacity: _opacity,
                            child: new Icon(_controller.isDismissed ? null : Icons.done)
                        ),
                        title: widget.child
                    )
                )
            );
    }

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (_ticker is null)
            {
                return true;
            }
            throw DartRuntimePrimitives.AsException(
                new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary(
                            $"{GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."
                        ),
                        new ErrorDescription(
                            "A SingleTickerProviderStateMixin can only be used as a TickerProvider once."
                        ),
                        new ErrorHint(
                            "If a State is used for multiple AnimationController objects, or if it is passed to other "
                                + "objects and those objects might use it more than one time in total, then instead of "
                                + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin."
                        ),
                    }
                )
            );
        });
        _ticker = new Scheduler.Ticker(
            onTick,
            debugLabel: Foundation.ConstantsLibrary.kDebugMode
                ? $"created by {DiagnosticsLibrary.describeIdentity(this)}"
                : null
        );
        _updateTickerModeNotifier();
        _updateTicker();
        return _ticker!;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTicker();
    }

    public virtual void _updateTicker()
    {
        TickerModeData values = _tickerModeNotifier!.value;
        if (_ticker is not null)
        {
            _ticker!.muted = !values.enabled;
            _ticker!.forceFrames = values.forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTicker);
        newNotifier.addListener(_updateTicker);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription = (_ticker?.isActive, _ticker?.muted) switch
        {
            (true, true) => "active but muted",
            (true, _) => "active",
            (false, true) => "inactive and muted",
            (false, _) => "inactive",
            (null, _) => DartRuntimePrimitives.ConvertValue<string>(null),
        };
        properties.add(
            new DiagnosticsProperty<Scheduler.Ticker>(
                "ticker",
                _ticker,
                description: tickerDescription,
                showSeparator: false,
                defaultValue: default
            )
        );
    }
}

public class _PopupMenu__popup_menu<T> : StatefulWidget
{
    public virtual List<GlobalKey<IState>> itemKeys { get; private set; } = default!;
    public virtual _PopupMenuRoute__popup_menu<T> route { get; private set; } = default!;
    public virtual string? semanticLabel { get; private set; }
    public virtual BoxConstraints? constraints { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;

    internal _PopupMenu__popup_menu(
        Key? key = null,
        List<GlobalKey<IState>> itemKeys = default!,
        _PopupMenuRoute__popup_menu<T> route = default!,
        string? semanticLabel = default!,
        BoxConstraints? constraints = null,
        Clip clipBehavior = default!
    )
        : base(key: key)
    {
        this.itemKeys = itemKeys;
        this.route = route;
        this.semanticLabel = semanticLabel;
        this.constraints = constraints;
        this.clipBehavior = clipBehavior;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _PopupMenuState__popup_menu<T>());
}

internal class _PopupMenuState__popup_menu<T> : State<_PopupMenu__popup_menu<T>>
{
    internal virtual List<CurvedAnimation> _opacities { get; set; } = new List<CurvedAnimation>();

    public override void initState()
    {
        base.initState();
        _setOpacities();
    }

    public override void didUpdateWidget(_PopupMenu__popup_menu<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (
            (checked(oldWidget.route.items.Count) != checked((long)widget.route.items.Count))
            || (!Equals(oldWidget.route.animation, widget.route.animation))
        )
        {
            _setOpacities();
        }
    }

    internal virtual void _setOpacities()
    {
        foreach (CurvedAnimation opacity in _opacities)
        {
            opacity.dispose();
        }
        var newOpacities = new List<CurvedAnimation>();
        double unit = 1.0 / (checked(widget.route.items.Count) + 1.5);
        for (var i = 0L; i < checked(widget.route.items.Count); i += 1L)
        {
            double start = (i + 1L) * unit;
            double end = DorotiUiLibrary.clampDouble(start + (1.5 * unit), 0.0, 1.0);
            var opacityLocal = new CurvedAnimation(
                parent: widget.route.animation!,
                curve: new Interval(start, end)
            );
            newOpacities.Add(opacityLocal);
        }
        _opacities = newOpacities;
    }

    public override void dispose()
    {
        foreach (CurvedAnimation opacity in _opacities)
        {
            opacity.dispose();
        }
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        double unit = 1.0 / (checked(widget.route.items.Count) + 1.5);
        var childrenLocal = new List<Widget>();
        ThemeData theme = Theme.of(context);
        PopupMenuThemeData popupMenuTheme = PopupMenuTheme.of(context);
        PopupMenuThemeData defaults = new _PopupMenuDefaultsM3__popup_menu(context);
        for (var i = 0L; i < checked(widget.route.items.Count); i += 1L)
        {
            // Layout runs after this loop. Capture the entry index rather than
            // the shared C# loop variable, which has advanced past the list.
            var itemIndex = checked((int)i);
            CurvedAnimation opacityLocal = _opacities[(int)i];
            Widget item = widget.route.items[(int)i];
            if (
                (widget.route.initialValue is not null)
                && widget.route.items[(int)i].represents(widget.route.initialValue)
            )
            {
                item = DartRuntimePrimitives.ConvertValue<Widget>(
                    new ColoredBox(color: Theme.of(context).highlightColor, child: item)
                );
            }
            childrenLocal.Add(
                new _MenuItem__popup_menu(
                    onLayout: (size) =>
                    {
                        widget.route.itemSizes[itemIndex] = size;
                    },
                    child: new FadeTransition(
                        key: widget.itemKeys[(int)i],
                        opacity: opacityLocal,
                        child: item
                    )
                )
            );
        }
        var opacityAlternate = new CurveTween(curve: new Interval(0.0, 1.0 / 3.0));
        var width = new CurveTween(curve: new Interval(0.0, unit));
        var height = new CurveTween(
            curve: new Interval(0.0, unit * checked(widget.route.items.Count))
        );
        Widget childLocal = new ConstrainedBox(
            constraints: widget.constraints
                ?? new BoxConstraints(
                    minWidth: Popup_menuLibrary._kMenuMinWidth,
                    maxWidth: Popup_menuLibrary._kMenuMaxWidth
                ),
            child: new IntrinsicWidth(
                stepWidth: Popup_menuLibrary._kMenuWidthStep,
                child: new Widgets.Semantics(
                    role: SemanticsRole.menu,
                    scopesRoute: true,
                    namesRoute: true,
                    explicitChildNodes: true,
                    label: widget.semanticLabel,
                    child: new SingleChildScrollView(
                        padding: (widget.route.menuPadding ?? popupMenuTheme.menuPadding)
                            ?? defaults.menuPadding,
                        child: new ListBody(children: childrenLocal)
                    )
                )
            )
        );
        return new AnimatedBuilder(
            animation: widget.route.animation!,
            builder: (context, child) =>
            {
                return new FadeTransition(
                    opacity: opacityAlternate.animate(widget.route.animation!),
                    child: new Material(
                        shape: (widget.route.shape ?? popupMenuTheme.shape) ?? defaults.shape,
                        color: (widget.route.color ?? popupMenuTheme.color) ?? defaults.color,
                        clipBehavior: widget.clipBehavior,
                        type: MaterialType.card,
                        elevation: (widget.route.elevation ?? popupMenuTheme.elevation)
                            ?? (
                                defaults.elevation
                                ?? throw new global::System.NullReferenceException(
                                    "A required value was null."
                                )
                            ),
                        shadowColor: (widget.route.shadowColor ?? popupMenuTheme.shadowColor)
                            ?? defaults.shadowColor,
                        surfaceTintColor: (
                            widget.route.surfaceTintColor ?? popupMenuTheme.surfaceTintColor
                        ) ?? defaults.surfaceTintColor,
                        child: new Align(
                            alignment: AlignmentDirectional.topEnd,
                            widthFactor: width.evaluate(widget.route.animation!),
                            heightFactor: height.evaluate(widget.route.animation!),
                            child: child
                        )
                    )
                );
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            },
            child: childLocal
        );
    }
}

internal class _PopupMenuRouteLayout__popup_menu : SingleChildLayoutDelegate
{
    public virtual RelativeRect position { get; private set; } = default!;
    public virtual List<Size?> itemSizes { get; set; } = default!;
    public virtual long? selectedItemIndex { get; private set; }
    public virtual TextDirection textDirection { get; private set; } = default!;
    public virtual EdgeInsets padding { get; set; } = default!;
    public virtual HashSet<Rect> avoidBounds { get; private set; } = default!;

    internal _PopupMenuRouteLayout__popup_menu(
        RelativeRect position,
        List<Size?> itemSizes,
        long? selectedItemIndex,
        TextDirection textDirection,
        EdgeInsets padding,
        HashSet<Rect> avoidBounds
    )
    {
        this.position = position;
        this.itemSizes = itemSizes;
        this.selectedItemIndex = selectedItemIndex;
        this.textDirection = textDirection;
        this.padding = padding;
        this.avoidBounds = avoidBounds;
    }

    public override BoxConstraints getConstraintsForChild(BoxConstraints constraints)
    {
        return BoxConstraints
            .CreateLoose(constraints.biggest)
            .deflate(EdgeInsets.CreateAll(Popup_menuLibrary._kMenuScreenPadding).op_Add(padding));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Offset getPositionForChild(Size size, Size childSize)
    {
        double y = position.top;
        double x = default!;
        if (position.left > position.right)
        {
            x = size.width - position.right - childSize.width;
        }
        else
        {
            if (position.left < position.right)
            {
                x = position.left;
            }
            else
            {
                x = textDirection switch
                {
                    TextDirection.rtl => size.width - position.right - childSize.width,
                    TextDirection.ltr => position.left,
                    _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                        throw new InvalidOperationException(
                            "Switch expression did not handle the supplied value."
                        ),
                };
            }
        }
        var wantedPosition = new Offset(x, y);
        Offset originCenter = position.toRect(Offset.zero & size).center;
        IEnumerable<Rect> subScreens = DisplayFeatureSubScreen.subScreensInBounds(
            Offset.zero & size,
            avoidBounds
        );
        Rect subScreen = _closestScreen(subScreens.Cast<Rect>(), originCenter);
        return _fitInsideScreen(subScreen, childSize, wantedPosition);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Rect _closestScreen(IEnumerable<Rect> screens, Offset point)
    {
        Rect closest = screens.First();
        foreach (var screen in screens)
        {
            if ((screen.center - point).distance < (closest.center - point).distance)
            {
                closest = screen;
            }
        }
        return closest;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Offset _fitInsideScreen(Rect screen, Size childSize, Offset wantedPosition)
    {
        double x = wantedPosition.dx;
        double y = wantedPosition.dy;
        if (x < (screen.left + Popup_menuLibrary._kMenuScreenPadding + padding.left))
        {
            x = screen.left + Popup_menuLibrary._kMenuScreenPadding + padding.left;
        }
        else
        {
            if (
                (x + childSize.width)
                > (screen.right - Popup_menuLibrary._kMenuScreenPadding - padding.right)
            )
            {
                x =
                    screen.right
                    - childSize.width
                    - Popup_menuLibrary._kMenuScreenPadding
                    - padding.right;
            }
        }
        if (y < (screen.top + Popup_menuLibrary._kMenuScreenPadding + padding.top))
        {
            y = Popup_menuLibrary._kMenuScreenPadding + padding.top;
        }
        else
        {
            if (
                (y + childSize.height)
                > (screen.bottom - Popup_menuLibrary._kMenuScreenPadding - padding.bottom)
            )
            {
                y =
                    screen.bottom
                    - childSize.height
                    - Popup_menuLibrary._kMenuScreenPadding
                    - padding.bottom;
            }
        }
        return new Offset(x, y);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool shouldRelayout(SingleChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (_PopupMenuRouteLayout__popup_menu)oldDelegate;
        DartRuntimePrimitives.Assert(() =>
            checked(itemSizes.Count) == checked((long)__oldDelegate.itemSizes.Count)
        );
        return (!Equals(position, __oldDelegate.position))
            || (selectedItemIndex != __oldDelegate.selectedItemIndex)
            || (!Equals(textDirection, __oldDelegate.textDirection))
            || !CollectionsLibrary.listEquals(itemSizes, __oldDelegate.itemSizes)
            || (!Equals(padding, __oldDelegate.padding))
            || !CollectionsLibrary.setEquals(avoidBounds, __oldDelegate.avoidBounds);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class _PopupMenuRoute__popup_menu<T> : PopupRoute<T>
{
    public virtual RelativeRect? position { get; private set; }
    public virtual Func<BuildContext, BoxConstraints, RelativeRect>? positionBuilder
    {
        get;
        private set;
    }
    public virtual List<PopupMenuEntry<T>> items { get; private set; } = default!;
    public virtual List<GlobalKey<IState>> itemKeys { get; private set; } = default!;
    public virtual List<Size?> itemSizes { get; private set; } = default!;
    public virtual T? initialValue { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual string? semanticLabel { get; private set; }
    public virtual ShapeBorder? shape { get; private set; }
    public virtual EdgeInsetsGeometry? menuPadding { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual CapturedThemes capturedThemes { get; private set; } = default!;
    public virtual BoxConstraints? constraints { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual AnimationStyle? popUpAnimationStyle { get; private set; }

    // Dart library-private member: distinct from the same name in the base library.
    internal new virtual CurvedAnimation? _animation { get; set; } = default;
    private string? __field_barrierLabel = default!;
    public override string? barrierLabel
    {
        get => __field_barrierLabel;
    }

    internal _PopupMenuRoute__popup_menu(
        RelativeRect? position = null,
        Func<BuildContext, BoxConstraints, RelativeRect>? positionBuilder = null,
        List<PopupMenuEntry<T>> items = default!,
        List<GlobalKey<IState>> itemKeys = default!,
        T? initialValue = default,
        double? elevation = null,
        Color? surfaceTintColor = null,
        Color? shadowColor = null,
        string barrierLabel = default!,
        string? semanticLabel = null,
        ShapeBorder? shape = null,
        EdgeInsetsGeometry? menuPadding = null,
        Color? color = null,
        CapturedThemes capturedThemes = default!,
        BoxConstraints? constraints = null,
        Clip clipBehavior = default!,
        RouteSettings? settings = null,
        bool? requestFocus = null,
        AnimationStyle? popUpAnimationStyle = null
    )
        : base(
            settings: settings,
            requestFocus: requestFocus,
            traversalEdgeBehavior: TraversalEdgeBehavior.closedLoop
        )
    {
        this.position = position;
        this.positionBuilder = positionBuilder;
        this.items = items;
        this.itemKeys = itemKeys;
        this.initialValue = initialValue;
        this.elevation = elevation;
        this.surfaceTintColor = surfaceTintColor;
        this.shadowColor = shadowColor;
        __field_barrierLabel = barrierLabel;
        this.semanticLabel = semanticLabel;
        this.shape = shape;
        this.menuPadding = menuPadding;
        this.color = color;
        this.capturedThemes = capturedThemes;
        this.constraints = constraints;
        this.clipBehavior = clipBehavior;
        this.popUpAnimationStyle = popUpAnimationStyle;
        itemSizes = new List<Size?>(
            Enumerable.Repeat<Size?>(null, checked((int)checked((long)items.Count)))
        );
        System.Diagnostics.Debug.Assert((position is not null) != (positionBuilder is not null));
    }

    public override Animation<double> createAnimation()
    {
        if (!Equals(popUpAnimationStyle, AnimationStyle.noAnimation))
        {
            return _animation ??= new CurvedAnimation(
                parent: base.createAnimation(),
                curve: popUpAnimationStyle?.curve ?? Curves.linear,
                reverseCurve: popUpAnimationStyle?.reverseCurve
                    ?? new Interval(0.0, Popup_menuLibrary._kMenuCloseIntervalEnd)
            );
        }
        return base.createAnimation();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void scrollTo(long selectedItemIndex)
    {
        Scheduler.SchedulerBinding.instance.addPostFrameCallback(
            (_) =>
            {
                if (itemKeys[(int)(selectedItemIndex)].currentContext is not null)
                {
                    DartRuntimePrimitives.Ignore(
                        Scrollable.ensureVisible(itemKeys[(int)(selectedItemIndex)].currentContext!)
                    );
                }
            }
        );
    }

    public override Duration transitionDuration =>
        DartRuntimePrimitives.ConvertValue<Duration>(
            popUpAnimationStyle?.duration ?? Popup_menuLibrary._kMenuDuration
        );
    public override bool barrierDismissible => true;
    public override Color? barrierColor => DartRuntimePrimitives.ConvertValue<Color>(null);

    public override Widget buildPage(
        BuildContext context,
        Animation<double> animation,
        Animation<double> secondaryAnimation
    )
    {
        long? selectedItemIndex = default!;
        if (initialValue is not null)
        {
            for (
                var index = 0L;
                (selectedItemIndex is null) && (index < checked(items.Count));
                index += 1L
            )
            {
                if (items[(int)index].represents(initialValue))
                {
                    selectedItemIndex = index;
                }
            }
        }
        if (selectedItemIndex is not null)
        {
            long selectedItemIndex__34930__value35194 = (
                selectedItemIndex
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            scrollTo(((selectedItemIndex__34930__value35194)));
        }
        Widget menu = new _PopupMenu__popup_menu<T>(
            route: this,
            itemKeys: itemKeys,
            semanticLabel: semanticLabel,
            constraints: constraints,
            clipBehavior: clipBehavior
        );
        MediaQueryData mediaQuery = MediaQuery.of(context);
        return MediaQuery.CreateRemovePadding(
            context: context,
            removeTop: true,
            removeBottom: true,
            removeLeft: true,
            removeRight: true,
            child: new LayoutBuilder(
                builder: (context, constraints) =>
                {
                    return new CustomSingleChildLayout(
                        @delegate: new _PopupMenuRouteLayout__popup_menu(
                            positionBuilder is null
                                ? position!
                                : positionBuilder.Invoke(context, constraints),
                            itemSizes,
                            selectedItemIndex,
                            Directionality.of(context),
                            mediaQuery.padding,
                            _avoidBounds(mediaQuery)
                        ),
                        child: capturedThemes.wrap(menu)
                    );
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual HashSet<Rect> _avoidBounds(MediaQueryData mediaQuery)
    {
        return DisplayFeatureSubScreen.avoidBounds(mediaQuery).toSet();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void dispose()
    {
        _animation?.dispose();
        base.dispose();
    }
}

public delegate RelativeRect PopupMenuPositionBuilder(
    BuildContext context,
    BoxConstraints constraints
);

public static partial class Popup_menuLibrary
{
    public static Future<T?> showMenu<T>(
        BuildContext context,
        RelativeRect? position = null,
        Func<BuildContext, BoxConstraints, RelativeRect>? positionBuilder = null,
        List<PopupMenuEntry<T>> items = default!,
        T? initialValue = default,
        double? elevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        string? semanticLabel = null,
        ShapeBorder? shape = null,
        EdgeInsetsGeometry? menuPadding = null,
        Color? color = null,
        bool useRootNavigator = false,
        BoxConstraints? constraints = null,
        Clip clipBehavior = Clip.none,
        RouteSettings? routeSettings = null,
        AnimationStyle? popUpAnimationStyle = null,
        bool? requestFocus = null
    )
    {
        DartRuntimePrimitives.Assert(() => Enumerable.Any(items));
        DartRuntimePrimitives.Assert(() =>
            DebugLibrary.debugCheckHasMaterialLocalizations(context)
        );
        DartRuntimePrimitives.Assert(
            () => (position is not null) != (positionBuilder is not null),
            () => (object?)"Either position or positionBuilder must be provided."
        );
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
            {
                break;
            }
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
            {
                semanticLabel ??= MaterialLocalizations.of(context).popupMenuLabel;
                break;
            }
        }
        var menuItemKeys = DartRuntimePrimitives.CreateList(
            checked(items.Count),
            (index) => GlobalKey<IState>.Create()
        );
        NavigatorState navigator = Navigator.of(context, rootNavigator: useRootNavigator);
        return navigator.push(
            new _PopupMenuRoute__popup_menu<T>(
                position: position,
                positionBuilder: positionBuilder,
                items: items,
                itemKeys: menuItemKeys,
                initialValue: initialValue,
                elevation: elevation,
                shadowColor: shadowColor,
                surfaceTintColor: surfaceTintColor,
                semanticLabel: semanticLabel,
                barrierLabel: MaterialLocalizations.of(context).menuDismissLabel,
                shape: shape,
                menuPadding: menuPadding,
                color: color,
                capturedThemes: InheritedTheme.capture(from: context, to: navigator.context),
                constraints: constraints,
                clipBehavior: clipBehavior,
                settings: routeSettings,
                popUpAnimationStyle: popUpAnimationStyle,
                requestFocus: requestFocus
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public delegate void PopupMenuItemSelected<T>(T value);

public delegate void PopupMenuCanceled();

public delegate List<PopupMenuEntry<T>> PopupMenuItemBuilder<T>(BuildContext context);

public class PopupMenuButton<T> : StatefulWidget
{
    public virtual Func<BuildContext, List<PopupMenuEntry<T>>> itemBuilder { get; private set; } =
        default!;
    public virtual T? initialValue { get; private set; }
    public virtual Action? onOpened { get; private set; }
    public virtual Action<T>? onSelected { get; private set; }
    public virtual Action? onCanceled { get; private set; }
    public virtual string? tooltip { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual EdgeInsetsGeometry padding { get; private set; } = default!;
    public virtual EdgeInsetsGeometry? menuPadding { get; private set; }
    public virtual double? splashRadius { get; private set; }
    public virtual Widget? child { get; private set; }
    public virtual BorderRadius? borderRadius { get; private set; }
    public virtual Widget? icon { get; private set; }
    public virtual Offset offset { get; private set; } = default!;
    public virtual bool enabled { get; private set; } = default!;
    public virtual ShapeBorder? shape { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual Color? iconColor { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual double? iconSize { get; private set; }
    public virtual BoxConstraints? constraints { get; private set; }
    public virtual PopupMenuPosition? position { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual bool useRootNavigator { get; private set; } = default!;
    public virtual AnimationStyle? popUpAnimationStyle { get; private set; }
    public virtual RouteSettings? routeSettings { get; private set; }
    public virtual ButtonStyle? style { get; private set; }
    public virtual bool? requestFocus { get; private set; }

    public PopupMenuButton(
        Key? key = null,
        Func<BuildContext, List<PopupMenuEntry<T>>> itemBuilder = default!,
        T? initialValue = default,
        Action? onOpened = null,
        Action<T>? onSelected = null,
        Action? onCanceled = null,
        string? tooltip = null,
        double? elevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        EdgeInsetsGeometry padding = default!,
        EdgeInsetsGeometry? menuPadding = null,
        Widget? child = null,
        BorderRadius? borderRadius = null,
        double? splashRadius = null,
        Widget? icon = null,
        double? iconSize = null,
        Offset offset = default,
        bool enabled = true,
        ShapeBorder? shape = null,
        Color? color = null,
        Color? iconColor = null,
        bool? enableFeedback = null,
        BoxConstraints? constraints = null,
        PopupMenuPosition? position = null,
        Clip clipBehavior = Clip.none,
        bool useRootNavigator = false,
        AnimationStyle? popUpAnimationStyle = null,
        RouteSettings? routeSettings = null,
        ButtonStyle? style = null,
        bool? requestFocus = null
    )
        : base(key: key)
    {
        EdgeInsetsGeometry __padding = padding ?? EdgeInsets.CreateAll(8.0);
        this.itemBuilder = itemBuilder;
        this.initialValue = initialValue;
        this.onOpened = onOpened;
        this.onSelected = onSelected;
        this.onCanceled = onCanceled;
        this.tooltip = tooltip;
        this.elevation = elevation;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.padding = __padding;
        this.menuPadding = menuPadding;
        this.child = child;
        this.borderRadius = borderRadius;
        this.splashRadius = splashRadius;
        this.icon = icon;
        this.iconSize = iconSize;
        this.offset = offset;
        this.enabled = enabled;
        this.shape = shape;
        this.color = color;
        this.iconColor = iconColor;
        this.enableFeedback = enableFeedback;
        this.constraints = constraints;
        this.position = position;
        this.clipBehavior = clipBehavior;
        this.useRootNavigator = useRootNavigator;
        this.popUpAnimationStyle = popUpAnimationStyle;
        this.routeSettings = routeSettings;
        this.style = style;
        this.requestFocus = requestFocus;
        System.Diagnostics.Debug.Assert(!((child is not null) && (icon is not null)));
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new PopupMenuButtonState<T>());
}

public class PopupMenuButtonState<T> : State<PopupMenuButton<T>>
{
    internal virtual bool _isMenuExpanded { get; set; } = false;
    internal virtual RelativeRect? _lastPosition { get; set; } = default;
    internal virtual PopupMenuThemeData _popupMenuTheme { get; set; } = default!;
    internal virtual RenderBox? _cachedButtonRenderBox { get; set; } = default;
    internal virtual RenderBox? _cachedOverlayRenderBox { get; set; } = default;

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _updateCachedObjects();
    }

    internal virtual void _updateCachedObjects()
    {
        if (mounted)
        {
            _popupMenuTheme = PopupMenuTheme.of(context);
            RenderObject? buttonRenderObject = context.findRenderObject();
            if (buttonRenderObject is RenderBox)
            {
                RenderBox buttonRenderObject__57178__as57237 = (RenderBox)buttonRenderObject;
                _cachedButtonRenderBox = buttonRenderObject__57178__as57237;
            }
            try
            {
                NavigatorState navigator = Navigator.of(
                    context,
                    rootNavigator: widget.useRootNavigator
                );
                RenderObject? overlayRenderObject = navigator.overlay?.context.findRenderObject();
                if (overlayRenderObject is RenderBox)
                {
                    RenderBox overlayRenderObject__57508__as57589 = (RenderBox)overlayRenderObject;
                    _cachedOverlayRenderBox = overlayRenderObject__57508__as57589;
                }
            }
            catch (Exception)
            {
                _cachedButtonRenderBox = null;
                _cachedOverlayRenderBox = null;
            }
        }
    }

    internal virtual RelativeRect _getDefaultPosition(BoxConstraints constraints)
    {
        return _lastPosition ?? RelativeRect.CreateFromSize(Rect.zero, constraints.biggest);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual RelativeRect _positionBuilder(
        BuildContext __unused0,
        BoxConstraints constraints
    )
    {
        if (!mounted)
        {
            return _getDefaultPosition(constraints);
        }
        PopupMenuThemeData popupMenuTheme = _popupMenuTheme;
        RenderBox? button = _cachedButtonRenderBox;
        RenderBox? overlay = _cachedOverlayRenderBox;
        if ((button is null) || (overlay is null) || !button.attached || !overlay.attached)
        {
            return _getDefaultPosition(constraints);
        }
        PopupMenuPosition popupMenuPosition =
            (widget.position ?? popupMenuTheme.position) ?? PopupMenuPosition.over;
        Offset offsetLocal = default!;
        switch (popupMenuPosition)
        {
            case var __constant59105 when Equals(__constant59105, PopupMenuPosition.over):
            {
                offsetLocal = widget.offset;
                break;
            }
            case var __constant59172 when Equals(__constant59172, PopupMenuPosition.under):
            {
                offsetLocal = new Offset(0.0, button.size.height) + widget.offset;
                if (widget.child is null)
                {
                    offsetLocal -= new Offset(0.0, widget.padding.vertical / 2L);
                }
                break;
            }
        }
        var positionLocal = RelativeRect.CreateFromRect(
            Rect.fromPoints(
                button.localToGlobal(offsetLocal, ancestor: overlay),
                button.localToGlobal(
                    button.size.bottomRight(Offset.zero) + offsetLocal,
                    ancestor: overlay
                )
            ),
            Offset.zero & overlay.size
        );
        return _lastPosition = positionLocal;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void showButtonMenu()
    {
        _updateCachedObjects();
        List<PopupMenuEntry<T>> itemsLocal = widget.itemBuilder(context).ToList();
        if (Enumerable.Any(itemsLocal))
        {
            widget.onOpened?.Invoke();
            setState(() =>
            {
                _isMenuExpanded = true;
            });
            DartRuntimePrimitives.Ignore(
                Popup_menuLibrary
                    .showMenu(
                        context: context,
                        elevation: widget.elevation,
                        shadowColor: widget.shadowColor,
                        surfaceTintColor: widget.surfaceTintColor,
                        items: itemsLocal,
                        initialValue: widget.initialValue,
                        positionBuilder: _positionBuilder,
                        shape: widget.shape,
                        menuPadding: widget.menuPadding,
                        color: widget.color,
                        constraints: widget.constraints,
                        clipBehavior: widget.clipBehavior,
                        useRootNavigator: widget.useRootNavigator,
                        popUpAnimationStyle: widget.popUpAnimationStyle,
                        routeSettings: widget.routeSettings,
                        requestFocus: widget.requestFocus
                    )
                    .then(
                        (newValue) =>
                        {
                            if (!mounted)
                            {
                                _ = (object?)null;
                                return;
                            }
                            setState(() =>
                            {
                                _isMenuExpanded = false;
                            });
                            if (newValue is null)
                            {
                                widget.onCanceled?.Invoke();
                                _ = (object?)null;
                                return;
                            }
                            widget.onSelected?.Invoke(newValue);
                        }
                    )
            );
        }
    }

    internal virtual bool _canRequestFocus
    {
        get
        {
            NavigationMode mode =
                MediaQuery.maybeNavigationModeOf(context) ?? NavigationMode.traditional;
            return mode switch
            {
                NavigationMode.traditional => widget.enabled,
                NavigationMode.directional => true,
                _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                    throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
            };
        }
    }

    public override Widget build(BuildContext context)
    {
        IconThemeData iconTheme = IconTheme.of(context);
        PopupMenuThemeData popupMenuTheme = PopupMenuTheme.of(context);
        bool enableFeedbackLocal =
            (widget.enableFeedback ?? PopupMenuTheme.of(context).enableFeedback) ?? true;
        DartRuntimePrimitives.Assert(() =>
            DebugLibrary.debugCheckHasMaterialLocalizations(context)
        );
        if (widget.child is not null)
        {
            Widget childLocal = new Tooltip(
                message: widget.tooltip ?? MaterialLocalizations.of(context).showMenuTooltip,
                child: new InkWell(
                    borderRadius: widget.borderRadius,
                    onTap: widget.enabled ? showButtonMenu : null,
                    canRequestFocus: _canRequestFocus,
                    radius: widget.splashRadius,
                    enableFeedback: enableFeedbackLocal,
                    child: widget.child
                )
            );
            MaterialTapTargetSize tapTargetSizeLocal =
                widget.style?.tapTargetSize ?? MaterialTapTargetSize.shrinkWrap;
            if (Equals(tapTargetSizeLocal, MaterialTapTargetSize.padded))
            {
                return new ConstrainedBox(
                    constraints: new BoxConstraints(
                        minWidth: Widgets.ConstantsLibrary.kMinInteractiveDimension,
                        minHeight: Widgets.ConstantsLibrary.kMinInteractiveDimension
                    ),
                    child: childLocal
                );
            }
            return new Widgets.Semantics(expanded: _isMenuExpanded, child: childLocal);
        }
        return new Widgets.Semantics(
            child: new IconButton(
                key: StandardComponentTypeMembers.key(StandardComponentType.moreButton),
                icon: new Widgets.Semantics(
                    expanded: _isMenuExpanded,
                    child: widget.icon ?? new Icon(Icons.adaptive.more)
                ),
                padding: widget.padding,
                splashRadius: widget.splashRadius,
                iconSize: (widget.iconSize ?? popupMenuTheme.iconSize) ?? iconTheme.size,
                color: (widget.iconColor ?? popupMenuTheme.iconColor) ?? iconTheme.color,
                tooltip: widget.tooltip ?? MaterialLocalizations.of(context).showMenuTooltip,
                onPressed: widget.enabled ? showButtonMenu : null,
                enableFeedback: enableFeedbackLocal,
                style: widget.style
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _EffectiveMouseCursor__popup_menu : WidgetStateMouseCursor
{
    public virtual MouseCursor? widgetCursor { get; private set; }
    public virtual WidgetStateProperty<MouseCursor?>? themeCursor { get; private set; }

    internal _EffectiveMouseCursor__popup_menu(
        MouseCursor? widgetCursor,
        WidgetStateProperty<MouseCursor?>? themeCursor
    )
    {
        this.widgetCursor = widgetCursor;
        this.themeCursor = themeCursor;
    }

    public override MouseCursor resolve(HashSet<WidgetState> states)
    {
        return (
                WidgetStateProperty.resolveAs(widgetCursor, states)
                ?? (themeCursor?.resolve(states))
            ) ?? adaptiveClickable.resolve(states);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override string debugDescription => "WidgetStateMouseCursor(PopupMenuItemState)";
}

internal class _PopupMenuDefaultsM3__popup_menu : PopupMenuThemeData
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
    public static EdgeInsets menuItemPadding = EdgeInsets.CreateSymmetric(horizontal: 12.0);

    internal _PopupMenuDefaultsM3__popup_menu(BuildContext context)
        : base(elevation: 3.0)
    {
        this.context = context;
    }

    public override WidgetStateProperty<TextStyle?>? labelTextStyle
    {
        get
        {
            return (WidgetStateProperty<TextStyle?>?)
                WidgetStateProperty.resolveWith(
                    (states) =>
                    {
                        TextStyle style = _textTheme.labelLarge!;
                        if (states.Contains(WidgetState.disabled))
                        {
                            return style.apply(color: _colors.onSurface.withOpacity(0.38));
                        }
                        return style.apply(color: _colors.onSurface);
                        throw new InvalidOperationException(
                            "Callback completed without returning a value."
                        );
                    }
                );
        }
    }
    public override Color? color =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.surfaceContainer);
    public override Color? shadowColor => DartRuntimePrimitives.ConvertValue<Color>(_colors.shadow);
    public override Color? surfaceTintColor =>
        DartRuntimePrimitives.ConvertValue<Color>(Colors.transparent);
    public override ShapeBorder? shape =>
        DartRuntimePrimitives.ConvertValue<ShapeBorder>(
            new RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(4.0)))
        );
    public override EdgeInsets? menuPadding => EdgeInsets.CreateSymmetric(vertical: 8.0);
}
