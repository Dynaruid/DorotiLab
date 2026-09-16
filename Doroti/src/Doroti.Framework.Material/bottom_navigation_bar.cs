// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/bottom_navigation_bar.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public enum BottomNavigationBarType
{
    @fixed,
    shifting
}

public enum BottomNavigationBarLandscapeLayout
{
    spread,
    centered,
    linear
}

public class BottomNavigationBar : StatefulWidget
{
    public virtual List<BottomNavigationBarItem> items { get; private set; } = default!;
    public virtual System.Action<long>? onTap { get; private set; }
    public virtual long currentIndex { get; private set; } = default!;
    public virtual double? elevation { get; private set; }
    public virtual BottomNavigationBarType? type { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual double iconSize { get; private set; } = default!;
    public virtual Color? selectedItemColor { get; private set; }
    public virtual Color? unselectedItemColor { get; private set; }
    public virtual IconThemeData? selectedIconTheme { get; private set; }
    public virtual IconThemeData? unselectedIconTheme { get; private set; }
    public virtual TextStyle? selectedLabelStyle { get; private set; }
    public virtual TextStyle? unselectedLabelStyle { get; private set; }
    public virtual double selectedFontSize { get; private set; } = default!;
    public virtual double unselectedFontSize { get; private set; } = default!;
    public virtual bool? showUnselectedLabels { get; private set; }
    public virtual bool? showSelectedLabels { get; private set; }
    public virtual MouseCursor? mouseCursor { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual BottomNavigationBarLandscapeLayout? landscapeLayout { get; private set; }
    public virtual bool useLegacyColorScheme { get; private set; } = default!;

    public BottomNavigationBar(Key? key = null, List<BottomNavigationBarItem> items = default!, System.Action<long>? onTap = null, long currentIndex = 0, double? elevation = null, BottomNavigationBarType? type = null, Color? fixedColor = null, Color? backgroundColor = null, double iconSize = 24.0, Color? selectedItemColor = null, Color? unselectedItemColor = null, IconThemeData? selectedIconTheme = null, IconThemeData? unselectedIconTheme = null, double selectedFontSize = 14.0, double unselectedFontSize = 12.0, TextStyle? selectedLabelStyle = null, TextStyle? unselectedLabelStyle = null, bool? showSelectedLabels = null, bool? showUnselectedLabels = null, MouseCursor? mouseCursor = null, bool? enableFeedback = null, BottomNavigationBarLandscapeLayout? landscapeLayout = null, bool useLegacyColorScheme = true) : base(key: key)
    {
        this.items = items;
        this.onTap = onTap;
        this.currentIndex = currentIndex;
        this.elevation = elevation;
        this.type = type;
        this.backgroundColor = backgroundColor;
        this.iconSize = iconSize;
        this.unselectedItemColor = unselectedItemColor;
        this.selectedIconTheme = selectedIconTheme;
        this.unselectedIconTheme = unselectedIconTheme;
        this.selectedFontSize = selectedFontSize;
        this.unselectedFontSize = unselectedFontSize;
        this.selectedLabelStyle = selectedLabelStyle;
        this.unselectedLabelStyle = unselectedLabelStyle;
        this.showSelectedLabels = showSelectedLabels;
        this.showUnselectedLabels = showUnselectedLabels;
        this.mouseCursor = mouseCursor;
        this.enableFeedback = enableFeedback;
        this.landscapeLayout = landscapeLayout;
        this.useLegacyColorScheme = useLegacyColorScheme;
        this.selectedItemColor = selectedItemColor ?? fixedColor;
        System.Diagnostics.Debug.Assert(checked(items.Count) >= 2L);
        System.Diagnostics.Debug.Assert(items.All((item) => item.label is not null));
        System.Diagnostics.Debug.Assert((0L <= currentIndex) && (currentIndex < checked(items.Count)));
        System.Diagnostics.Debug.Assert((elevation is null) || (elevation >= 0.0));
        System.Diagnostics.Debug.Assert(iconSize >= 0.0);
        System.Diagnostics.Debug.Assert((selectedItemColor is null) || (fixedColor is null));
        System.Diagnostics.Debug.Assert(selectedFontSize >= 0.0);
        System.Diagnostics.Debug.Assert(unselectedFontSize >= 0.0);
    }

    public virtual Color? fixedColor => DartRuntimePrimitives.ConvertValue<Color>(selectedItemColor);
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _BottomNavigationBarState__bottom_navigation_bar());
}

internal class _BottomNavigationTile__bottom_navigation_bar : StatelessWidget
{
    public virtual BottomNavigationBarType type { get; private set; } = default!;
    public virtual BottomNavigationBarItem item { get; private set; } = default!;
    public virtual Animation<double> animation { get; private set; } = default!;
    public virtual double iconSize { get; private set; } = default!;
    public virtual Action? onTap { get; private set; }
    public virtual ColorTween? labelColorTween { get; private set; }
    public virtual ColorTween? iconColorTween { get; private set; }
    public virtual double? flex { get; private set; }
    public virtual bool selected { get; private set; } = default!;
    public virtual IconThemeData? selectedIconTheme { get; private set; }
    public virtual IconThemeData? unselectedIconTheme { get; private set; }
    public virtual TextStyle selectedLabelStyle { get; private set; } = default!;
    public virtual TextStyle unselectedLabelStyle { get; private set; } = default!;
    public virtual string? indexLabel { get; private set; }
    public virtual bool showSelectedLabels { get; private set; } = default!;
    public virtual bool showUnselectedLabels { get; private set; } = default!;
    public virtual MouseCursor mouseCursor { get; private set; } = default!;
    public virtual bool enableFeedback { get; private set; } = default!;
    public virtual BottomNavigationBarLandscapeLayout layout { get; private set; } = default!;

    internal _BottomNavigationTile__bottom_navigation_bar(BottomNavigationBarType type, BottomNavigationBarItem item, Animation<double> animation, double iconSize, Key? key = null, Action? onTap = null, ColorTween? labelColorTween = null, ColorTween? iconColorTween = null, double? flex = null, bool selected = false, TextStyle selectedLabelStyle = default!, TextStyle unselectedLabelStyle = default!, IconThemeData? selectedIconTheme = default!, IconThemeData? unselectedIconTheme = default!, bool showSelectedLabels = default!, bool showUnselectedLabels = default!, string? indexLabel = null, MouseCursor mouseCursor = default!, bool enableFeedback = default!, BottomNavigationBarLandscapeLayout layout = default!) : base(key: key)
    {
        this.type = type;
        this.item = item;
        this.animation = animation;
        this.iconSize = iconSize;
        this.onTap = onTap;
        this.labelColorTween = labelColorTween;
        this.iconColorTween = iconColorTween;
        this.flex = flex;
        this.selected = selected;
        this.selectedLabelStyle = selectedLabelStyle;
        this.unselectedLabelStyle = unselectedLabelStyle;
        this.selectedIconTheme = selectedIconTheme;
        this.unselectedIconTheme = unselectedIconTheme;
        this.showSelectedLabels = showSelectedLabels;
        this.showUnselectedLabels = showUnselectedLabels;
        this.indexLabel = indexLabel;
        this.mouseCursor = mouseCursor;
        this.enableFeedback = enableFeedback;
        this.layout = layout;
    }

    public override Widget build(BuildContext context)
    {
        long sizeLocal = default!;
        double selectedFontSize = DartRuntimePrimitives.RequireValue(selectedLabelStyle.fontSize);
        double selectedIconSize = selectedIconTheme?.size ?? iconSize;
        double unselectedIconSize = unselectedIconTheme?.size ?? iconSize;
        double selectedIconDiff = Math.Max(selectedIconSize - unselectedIconSize, 0);
        double unselectedIconDiff = Math.Max(unselectedIconSize - selectedIconSize, 0);
        string? effectiveTooltip = (item.tooltip == "") ? null : item.tooltip;
        double bottomPadding = default!;
        double topPadding = default!;
        if (showSelectedLabels && !showUnselectedLabels)
        {
            bottomPadding = new Tween<double>(begin: selectedIconDiff / 2.0, end: (selectedFontSize / 2.0) - (unselectedIconDiff / 2.0)).evaluate(animation);
            topPadding = new Tween<double>(begin: selectedFontSize + (selectedIconDiff / 2.0), end: (selectedFontSize / 2.0) - (unselectedIconDiff / 2.0)).evaluate(animation);
        }
        else
        {
            if (!showSelectedLabels && !showUnselectedLabels)
            {
                bottomPadding = new Tween<double>(begin: selectedIconDiff / 2.0, end: unselectedIconDiff / 2.0).evaluate(animation);
                topPadding = new Tween<double>(begin: selectedFontSize + (selectedIconDiff / 2.0), end: selectedFontSize + (unselectedIconDiff / 2.0)).evaluate(animation);
            }
            else
            {
                bottomPadding = new Tween<double>(begin: (selectedFontSize / 2.0) + (selectedIconDiff / 2.0), end: (selectedFontSize / 2.0) + (unselectedIconDiff / 2.0)).evaluate(animation);
                topPadding = new Tween<double>(begin: (selectedFontSize / 2.0) + (selectedIconDiff / 2.0), end: (selectedFontSize / 2.0) + (unselectedIconDiff / 2.0)).evaluate(animation);
            }
        }
        sizeLocal = type switch { BottomNavigationBarType.@fixed => 1L, BottomNavigationBarType.shifting => (DartRuntimePrimitives.RequireValue(flex) * 1000.0).round(), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        Widget result = new InkResponse(onTap: onTap, mouseCursor: mouseCursor, enableFeedback: enableFeedback, child: new Padding(padding: EdgeInsets.CreateOnly(top: topPadding, bottom: bottomPadding), child: new _Tile__bottom_navigation_bar(layout: layout, icon: new _TileIcon__bottom_navigation_bar(colorTween: iconColorTween!, animation: animation, iconSize: iconSize, selected: selected, item: item, selectedIconTheme: selectedIconTheme, unselectedIconTheme: unselectedIconTheme), label: new _Label__bottom_navigation_bar(colorTween: labelColorTween!, animation: animation, item: item, selectedLabelStyle: selectedLabelStyle, unselectedLabelStyle: unselectedLabelStyle, showSelectedLabels: showSelectedLabels, showUnselectedLabels: showUnselectedLabels))));
        if (effectiveTooltip is not null)
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(new Tooltip(message: effectiveTooltip, preferBelow: false, verticalOffset: selectedIconSize + selectedFontSize, excludeFromSemantics: true, child: result));
        }
        result = DartRuntimePrimitives.ConvertValue<Widget>(new Widgets.Semantics(selected: selected, button: true, container: true, child: new Stack(children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(result), DartRuntimePrimitives.ConvertValue<Widget>(new Widgets.Semantics(label: indexLabel)) })));
        return new Expanded(flex: sizeLocal, child: result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _Tile__bottom_navigation_bar : StatelessWidget
{
    public virtual BottomNavigationBarLandscapeLayout layout { get; private set; } = default!;
    public virtual Widget icon { get; private set; } = default!;
    public virtual Widget label { get; private set; } = default!;

    internal _Tile__bottom_navigation_bar(BottomNavigationBarLandscapeLayout layout, Widget icon, Widget label)
    {
        this.layout = layout;
        this.icon = icon;
        this.label = label;
    }

    public override Widget build(BuildContext context)
    {
        if (Equals(MediaQuery.orientationOf(context), Orientation.landscape) && Equals(layout, BottomNavigationBarLandscapeLayout.linear))
        {
            return new Align(heightFactor: 1, child: new Row(mainAxisSize: MainAxisSize.min, spacing: 8, children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(icon), DartRuntimePrimitives.ConvertValue<Widget>(new Flexible(child: new IntrinsicWidth(child: label))) }));
        }
        return new Column(mainAxisAlignment: MainAxisAlignment.spaceBetween, mainAxisSize: MainAxisSize.min, children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(icon), DartRuntimePrimitives.ConvertValue<Widget>(label) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TileIcon__bottom_navigation_bar : StatelessWidget
{
    public virtual ColorTween colorTween { get; private set; } = default!;
    public virtual Animation<double> animation { get; private set; } = default!;
    public virtual double iconSize { get; private set; } = default!;
    public virtual bool selected { get; private set; } = default!;
    public virtual BottomNavigationBarItem item { get; private set; } = default!;
    public virtual IconThemeData? selectedIconTheme { get; private set; }
    public virtual IconThemeData? unselectedIconTheme { get; private set; }

    internal _TileIcon__bottom_navigation_bar(ColorTween colorTween, Animation<double> animation, double iconSize, bool selected, BottomNavigationBarItem item, IconThemeData? selectedIconTheme, IconThemeData? unselectedIconTheme)
    {
        this.colorTween = colorTween;
        this.animation = animation;
        this.iconSize = iconSize;
        this.selected = selected;
        this.item = item;
        this.selectedIconTheme = selectedIconTheme;
        this.unselectedIconTheme = unselectedIconTheme;
    }

    public override Widget build(BuildContext context)
    {
        Color? iconColor = colorTween.evaluate(animation);
        var defaultIconTheme = new IconThemeData(color: iconColor, size: iconSize);
        IconThemeData iconThemeData = IconThemeData.lerp(defaultIconTheme.merge(unselectedIconTheme), defaultIconTheme.merge(selectedIconTheme), animation.value);
        return new Align(alignment: Alignment.topCenter, heightFactor: 1.0, child: new IconTheme(data: iconThemeData, child: selected ? item.activeIcon : item.icon));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _Label__bottom_navigation_bar : StatelessWidget
{
    public virtual ColorTween colorTween { get; private set; } = default!;
    public virtual Animation<double> animation { get; private set; } = default!;
    public virtual BottomNavigationBarItem item { get; private set; } = default!;
    public virtual TextStyle selectedLabelStyle { get; private set; } = default!;
    public virtual TextStyle unselectedLabelStyle { get; private set; } = default!;
    public virtual bool showSelectedLabels { get; private set; } = default!;
    public virtual bool showUnselectedLabels { get; private set; } = default!;

    internal _Label__bottom_navigation_bar(ColorTween colorTween, Animation<double> animation, BottomNavigationBarItem item, TextStyle selectedLabelStyle, TextStyle unselectedLabelStyle, bool showSelectedLabels, bool showUnselectedLabels)
    {
        this.colorTween = colorTween;
        this.animation = animation;
        this.item = item;
        this.selectedLabelStyle = selectedLabelStyle;
        this.unselectedLabelStyle = unselectedLabelStyle;
        this.showSelectedLabels = showSelectedLabels;
        this.showUnselectedLabels = showUnselectedLabels;
    }

    public override Widget build(BuildContext context)
    {
        double? selectedFontSize = selectedLabelStyle.fontSize;
        double? unselectedFontSize = unselectedLabelStyle.fontSize;
        TextStyle customStyle = TextStyle.lerp(unselectedLabelStyle, selectedLabelStyle, animation.value)!;
        Widget text = DefaultTextStyle.merge(style: customStyle.copyWith(fontSize: selectedFontSize, color: colorTween.evaluate(animation)), child: new Transform(transform: Matrix4.diagonal3(new Vector3(new Tween<double>(begin: DartRuntimePrimitives.RequireValue(unselectedFontSize) / DartRuntimePrimitives.RequireValue(selectedFontSize), end: 1.0).evaluate(animation))), alignment: Alignment.bottomCenter, child: new Text(item.label!, semanticsLabel: item.semanticsLabel)));
        if (!showUnselectedLabels && !showSelectedLabels)
        {
            text = DartRuntimePrimitives.ConvertValue<Widget>(Visibility.CreateMaintain(visible: false, child: text));
        }
        else
        {
            if (!showUnselectedLabels)
            {
                text = DartRuntimePrimitives.ConvertValue<Widget>(new FadeTransition(alwaysIncludeSemantics: true, opacity: animation, child: text));
            }
            else
            {
                if (!showSelectedLabels)
                {
                    text = DartRuntimePrimitives.ConvertValue<Widget>(new FadeTransition(alwaysIncludeSemantics: true, opacity: new Tween<double>(begin: 1.0, end: 0.0).animate(animation), child: text));
                }
            }
        }
        text = DartRuntimePrimitives.ConvertValue<Widget>(new Align(alignment: Alignment.bottomCenter, heightFactor: 1.0, child: text));
        if (item.label is not null)
        {
            text = MediaQuery.withClampedTextScaling(maxScaleFactor: 1.0, child: text);
        }
        return text;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _BottomNavigationBarState__bottom_navigation_bar : State<BottomNavigationBar>, TickerProviderStateMixin<BottomNavigationBar>
{
    internal virtual List<AnimationController> _controllers { get; set; } = new List<AnimationController>();
    internal virtual List<CurvedAnimation> _animations { get; set; } = new List<CurvedAnimation>();
    internal virtual Queue<_Circle__bottom_navigation_bar> _circles { get; private set; } = new Queue<_Circle__bottom_navigation_bar>();
    internal virtual Color? _backgroundColor { get; set; } = default;
    internal static Animatable<double> _flexTween = new Tween<double>(begin: 1.0, end: 1.5);
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual void _resetState()
    {
        foreach (AnimationController controller in _controllers)
        {
            controller.dispose();
        }
        foreach (_Circle__bottom_navigation_bar circle in _circles)
        {
            circle.dispose();
        }
        foreach (CurvedAnimation animation in _animations)
        {
            animation.dispose();
        }
        _circles.Clear();
        _controllers = new List<AnimationController>(Enumerable.Select(Enumerable.Range(0, checked((int)checked((long)widget.items.Count))), (index) =>
        {
            return ((Func<AnimationController>)(() =>
            {
                var __cascade = new AnimationController(duration: ThemeLibrary.kThemeAnimationDuration, vsync: this);
                __cascade.addListener(_rebuild);
                return __cascade;
            }))();
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        _animations = new List<CurvedAnimation>(Enumerable.Select(Enumerable.Range(0, checked((int)checked((long)widget.items.Count))), (index) =>
        {
            return new CurvedAnimation(parent: _controllers[index], curve: Curves.fastOutSlowIn, reverseCurve: Curves.fastOutSlowIn.flipped);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        _controllers[(int)widget.currentIndex].value = 1.0;
        _backgroundColor = widget.items[(int)widget.currentIndex].backgroundColor;
    }

    internal virtual BottomNavigationBarType _effectiveType
    {
        get
        {
            return (widget.type ?? BottomNavigationBarTheme.of(context).type) ?? ((checked(widget.items.Count) <= 3L) ? BottomNavigationBarType.@fixed : BottomNavigationBarType.shifting);
        }
    }
    internal virtual bool _defaultShowUnselected => _effectiveType switch { BottomNavigationBarType.shifting => false, BottomNavigationBarType.@fixed => true, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
    public override void initState()
    {
        base.initState();
        _resetState();
    }

    internal virtual void _rebuild()
    {
        setState(() =>
        {
        });
    }

    public override void dispose()
    {
        foreach (AnimationController controller in _controllers)
        {
            controller.dispose();
        }
        foreach (_Circle__bottom_navigation_bar circle in _circles)
        {
            circle.dispose();
        }
        foreach (CurvedAnimation animation in _animations)
        {
            animation.dispose();
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if (_tickers is not null)
                {
                    foreach (Scheduler.Ticker ticker in _tickers!)
                    {
                        if (ticker.isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{this} was disposed with an active Ticker."), new ErrorDescription($"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual double _evaluateFlex(Animation<double> animation) => _flexTween.evaluate(animation);
    internal virtual void _pushCircle(long index)
    {
        if (widget.items[(int)index].backgroundColor is not null)
        {
            _circles.Enqueue(((Func<_Circle__bottom_navigation_bar>)(() =>
{
    var __cascade = new _Circle__bottom_navigation_bar(state: this, index: index, color: widget.items[(int)index].backgroundColor!, vsync: this);
    __cascade.controller.addStatusListener((status) =>
    {
        if (AnimationStatusMembers.isCompleted(status))
        {
            setState(() =>
            {
                _Circle__bottom_navigation_bar circle = _circles.Dequeue();
                _backgroundColor = circle.color;
                circle.dispose();
            });
        }
    });
    return __cascade;
}))());
        }
    }

    public override void didUpdateWidget(BottomNavigationBar oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (checked(widget.items.Count) != checked((long)oldWidget.items.Count))
        {
            _resetState();
            return;
        }
        if (widget.currentIndex != oldWidget.currentIndex)
        {
            switch (_effectiveType)
            {
                case BottomNavigationBarType.@fixed:
                    {
                        break;
                    }
                case BottomNavigationBarType.shifting:
                    {
                        _pushCircle(widget.currentIndex);
                        break;
                    }
            }
            _controllers[(int)oldWidget.currentIndex].reverse();
            _controllers[(int)widget.currentIndex].forward();
        }
        else
        {
            if (!Equals(_backgroundColor, widget.items[(int)widget.currentIndex].backgroundColor))
            {
                _backgroundColor = widget.items[(int)widget.currentIndex].backgroundColor;
            }
        }
    }

    internal static TextStyle _effectiveTextStyle(TextStyle? textStyle, double fontSize)
    {
        textStyle ??= new TextStyle();
        return (textStyle.fontSize is null) ? textStyle.copyWith(fontSize: fontSize) : textStyle;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static IconThemeData _effectiveIconTheme(IconThemeData? iconTheme, Color? itemColor)
    {
        return iconTheme ?? new IconThemeData(color: itemColor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<Widget> _createTiles(BottomNavigationBarLandscapeLayout layout)
    {
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        ThemeData themeData = Theme.of(context);
        BottomNavigationBarThemeData bottomTheme = BottomNavigationBarTheme.of(context);
        Color themeColor = themeData.brightness switch { Brightness.light => themeData.colorScheme.primary, Brightness.dark => themeData.colorScheme.secondary, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        TextStyle effectiveSelectedLabelStyle = _effectiveTextStyle(widget.selectedLabelStyle ?? bottomTheme.selectedLabelStyle, widget.selectedFontSize);
        TextStyle effectiveUnselectedLabelStyle = _effectiveTextStyle(widget.unselectedLabelStyle ?? bottomTheme.unselectedLabelStyle, widget.unselectedFontSize);
        IconThemeData effectiveSelectedIconTheme = _effectiveIconTheme(widget.selectedIconTheme ?? bottomTheme.selectedIconTheme, (widget.selectedItemColor ?? bottomTheme.selectedItemColor) ?? themeColor);
        IconThemeData effectiveUnselectedIconTheme = _effectiveIconTheme(widget.unselectedIconTheme ?? bottomTheme.unselectedIconTheme, (widget.unselectedItemColor ?? bottomTheme.unselectedItemColor) ?? themeData.unselectedWidgetColor);
        ColorTween colorTween = default!;
        switch (_effectiveType)
        {
            case BottomNavigationBarType.@fixed:
                {
                    colorTween = new ColorTween(begin: (widget.unselectedItemColor ?? bottomTheme.unselectedItemColor) ?? themeData.unselectedWidgetColor, end: ((widget.selectedItemColor ?? bottomTheme.selectedItemColor) ?? widget.fixedColor) ?? themeColor);
                    break;
                }
            case BottomNavigationBarType.shifting:
                {
                    colorTween = new ColorTween(begin: (widget.unselectedItemColor ?? bottomTheme.unselectedItemColor) ?? themeData.colorScheme.surface, end: (widget.selectedItemColor ?? bottomTheme.selectedItemColor) ?? themeData.colorScheme.surface);
                    break;
                }
        }
        ColorTween labelColorTweenLocal = default!;
        switch (_effectiveType)
        {
            case BottomNavigationBarType.@fixed:
                {
                    labelColorTweenLocal = new ColorTween(begin: ((effectiveUnselectedLabelStyle.color ?? widget.unselectedItemColor) ?? bottomTheme.unselectedItemColor) ?? themeData.unselectedWidgetColor, end: (((effectiveSelectedLabelStyle.color ?? widget.selectedItemColor) ?? bottomTheme.selectedItemColor) ?? widget.fixedColor) ?? themeColor);
                    break;
                }
            case BottomNavigationBarType.shifting:
                {
                    labelColorTweenLocal = new ColorTween(begin: ((effectiveUnselectedLabelStyle.color ?? widget.unselectedItemColor) ?? bottomTheme.unselectedItemColor) ?? themeData.colorScheme.surface, end: ((effectiveSelectedLabelStyle.color ?? widget.selectedItemColor) ?? bottomTheme.selectedItemColor) ?? themeColor);
                    break;
                }
        }
        ColorTween iconColorTweenLocal = default!;
        switch (_effectiveType)
        {
            case BottomNavigationBarType.@fixed:
                {
                    iconColorTweenLocal = new ColorTween(begin: ((effectiveSelectedIconTheme.color ?? widget.unselectedItemColor) ?? bottomTheme.unselectedItemColor) ?? themeData.unselectedWidgetColor, end: (((effectiveUnselectedIconTheme.color ?? widget.selectedItemColor) ?? bottomTheme.selectedItemColor) ?? widget.fixedColor) ?? themeColor);
                    break;
                }
            case BottomNavigationBarType.shifting:
                {
                    iconColorTweenLocal = new ColorTween(begin: ((effectiveUnselectedIconTheme.color ?? widget.unselectedItemColor) ?? bottomTheme.unselectedItemColor) ?? themeData.colorScheme.surface, end: ((effectiveSelectedIconTheme.color ?? widget.selectedItemColor) ?? bottomTheme.selectedItemColor) ?? themeColor);
                    break;
                }
        }
        var tiles = new List<Widget>();
        for (var i = 0L; i < checked(widget.items.Count); i++)
        {
            var states = ((Func<HashSet<WidgetState>>)(() => { var __collection39372 = new HashSet<WidgetState>(); if (i == widget.currentIndex) { __collection39372.Add(WidgetState.selected); } return __collection39372; }))();
            MouseCursor effectiveMouseCursor = (WidgetStateProperty.resolveAs(widget.mouseCursor, states) ?? (bottomTheme.mouseCursor?.resolve(states))) ?? WidgetStateMouseCursor.clickable.resolve(states);
            tiles.Add(new _BottomNavigationTile__bottom_navigation_bar(_effectiveType, widget.items[(int)i], _animations[(int)i], widget.iconSize, key: widget.items[(int)i].key, selectedIconTheme: widget.useLegacyColorScheme ? (widget.selectedIconTheme ?? bottomTheme.selectedIconTheme) : effectiveSelectedIconTheme, unselectedIconTheme: widget.useLegacyColorScheme ? (widget.unselectedIconTheme ?? bottomTheme.unselectedIconTheme) : effectiveUnselectedIconTheme, selectedLabelStyle: effectiveSelectedLabelStyle, unselectedLabelStyle: effectiveUnselectedLabelStyle, enableFeedback: (widget.enableFeedback ?? bottomTheme.enableFeedback) ?? true, onTap: () =>
            {
                widget.onTap?.Invoke(i);
            }, labelColorTween: widget.useLegacyColorScheme ? colorTween : labelColorTweenLocal, iconColorTween: widget.useLegacyColorScheme ? colorTween : iconColorTweenLocal, flex: _evaluateFlex(_animations[(int)i]), selected: i == widget.currentIndex, showSelectedLabels: (widget.showSelectedLabels ?? bottomTheme.showSelectedLabels) ?? true, showUnselectedLabels: (widget.showUnselectedLabels ?? bottomTheme.showUnselectedLabels) ?? _defaultShowUnselected, indexLabel: localizations.tabLabel(tabIndex: i + 1L, tabCount: checked(widget.items.Count)), mouseCursor: effectiveMouseCursor, layout: layout));
        }
        return tiles;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasOverlay(context));
        BottomNavigationBarThemeData bottomTheme = BottomNavigationBarTheme.of(context);
        BottomNavigationBarLandscapeLayout layoutLocal = (widget.landscapeLayout ?? bottomTheme.landscapeLayout) ?? BottomNavigationBarLandscapeLayout.spread;
        double additionalBottomPadding = MediaQuery.viewPaddingOf(context).bottom;
        Color? backgroundColorLocal = _effectiveType switch { BottomNavigationBarType.@fixed => widget.backgroundColor ?? bottomTheme.backgroundColor, BottomNavigationBarType.shifting => _backgroundColor, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        return new Widgets.Semantics(explicitChildNodes: true, child: new _Bar__bottom_navigation_bar(layout: layoutLocal, elevation: (widget.elevation ?? bottomTheme.elevation) ?? 8.0, color: backgroundColorLocal, child: new ConstrainedBox(constraints: new BoxConstraints(minHeight: ConstantsLibrary.kBottomNavigationBarHeight + additionalBottomPadding), child: new CustomPaint(painter: new _RadialPainter__bottom_navigation_bar(circles: _circles.ToList(), textDirection: Directionality.of(context)), child: new Material(type: MaterialType.transparency, child: new Padding(padding: EdgeInsets.CreateOnly(bottom: additionalBottomPadding), child: MediaQuery.CreateRemovePadding(context: context, removeBottom: true, child: DefaultTextStyle.merge(overflow: TextOverflow.ellipsis, child: new Row(mainAxisAlignment: MainAxisAlignment.spaceBetween, children: _createTiles(layoutLocal))))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Scheduler.Ticker createTicker(System.Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = ((Func<_WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider(onTick, this, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
    __cascade.muted = !values.enabled;
    __cascade.forceFrames = values.forceFrames;
    return __cascade;
}))();
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

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<HashSet<Scheduler.Ticker>>("tickers", _tickers, description: (_tickers is not null) ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}" : null, defaultValue: default));
    }

}

internal class _Bar__bottom_navigation_bar : StatelessWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual BottomNavigationBarLandscapeLayout layout { get; private set; } = default!;
    public virtual double elevation { get; private set; } = default!;
    public virtual Color? color { get; private set; }

    internal _Bar__bottom_navigation_bar(Widget child, BottomNavigationBarLandscapeLayout layout, double elevation, Color? color)
    {
        this.child = child;
        this.layout = layout;
        this.elevation = elevation;
        this.color = color;
    }

    public override Widget build(BuildContext context)
    {
        Widget alignedChild = child;
        if (Equals(MediaQuery.orientationOf(context), Orientation.landscape) && Equals(layout, BottomNavigationBarLandscapeLayout.centered))
        {
            alignedChild = DartRuntimePrimitives.ConvertValue<Widget>(new Align(alignment: Alignment.bottomCenter, heightFactor: 1, child: new SizedBox(width: MediaQuery.heightOf(context), child: child)));
        }
        return new Material(elevation: elevation, color: color, child: alignedChild);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _Circle__bottom_navigation_bar
{
    public virtual _BottomNavigationBarState__bottom_navigation_bar state { get; private set; } = default!;
    public virtual long index { get; private set; } = default!;
    public virtual Color color { get; private set; } = default!;
    public virtual AnimationController controller { get; set; } = default!;
    public virtual CurvedAnimation animation { get; set; } = default!;

    internal _Circle__bottom_navigation_bar(_BottomNavigationBarState__bottom_navigation_bar state, long index, Color color, Scheduler.TickerProvider vsync)
    {
        this.state = state;
        this.index = index;
        this.color = color;
        controller = new AnimationController(duration: ThemeLibrary.kThemeAnimationDuration, vsync: vsync);
        animation = new CurvedAnimation(parent: controller, curve: Curves.fastOutSlowIn);
        controller.forward();
    }

    public virtual double horizontalLeadingOffset
    {
        get
        {
            double weightSum(IEnumerable<Animation<double>> animations)
            {
                return Enumerable.Aggregate(animations.map(state._evaluateFlex), (double)0.0, (sum, value) => sum + value);
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            double allWeights = weightSum(state._animations.Cast<Animation<double>>());
            double leadingWeights = weightSum(state._animations.GetRange(0L, index).Cast<Animation<double>>());
            return (leadingWeights + (state._evaluateFlex(state._animations[(int)index]) / 2.0)) / allWeights;
        }
    }
    public virtual void dispose()
    {
        controller.dispose();
        animation.dispose();
    }

}

internal class _RadialPainter__bottom_navigation_bar : CustomPainter
{
    public virtual List<_Circle__bottom_navigation_bar> circles { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;

    internal _RadialPainter__bottom_navigation_bar(List<_Circle__bottom_navigation_bar> circles, TextDirection textDirection)
    {
        this.circles = circles;
        this.textDirection = textDirection;
    }

    internal static double _maxRadius(Offset center, Size size)
    {
        double maxX = Math.Max(center.dx, size.width - center.dx);
        double maxY = Math.Max(center.dy, size.height - center.dy);
        return Dart_mathLibrary.sqrt((maxX * maxX) + (maxY * maxY));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRepaint(CustomPainter oldDelegate)
    {
        var __oldPainter = (_RadialPainter__bottom_navigation_bar)oldDelegate;
        if (!Equals(textDirection, __oldPainter.textDirection))
        {
            return true;
        }
        if (Equals(circles, __oldPainter.circles))
        {
            return false;
        }
        if (checked(circles.Count) != checked((long)__oldPainter.circles.Count))
        {
            return true;
        }
        for (var i = 0L; i < checked(circles.Count); i += 1L)
        {
            if (!Equals(circles[(int)i], __oldPainter.circles[(int)i]))
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(Canvas canvas, Size size)
    {
        foreach (_Circle__bottom_navigation_bar circle in circles)
        {
            var paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = new Paint();
    __cascade.color = circle.color;
    return __cascade;
}))();
            var rect = Rect.fromLTWH(0.0, 0.0, size.width, size.height);
            canvas.clipRect(rect);
            double leftFraction = textDirection switch { TextDirection.rtl => 1.0 - circle.horizontalLeadingOffset, TextDirection.ltr => circle.horizontalLeadingOffset, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            var center = new Offset(leftFraction * size.width, size.height / 2.0);
            var radiusTween = new Tween<double>(begin: 0.0, end: _maxRadius(center, size));
            canvas.drawCircle(center, radiusTween.transform(circle.animation.value), paintLocal);
        }
    }

}
