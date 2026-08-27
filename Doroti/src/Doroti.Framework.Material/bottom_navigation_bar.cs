// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/bottom_navigation_bar.dart
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

public class BottomNavigationBar : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual List<global::Doroti.Framework.Widgets.BottomNavigationBarItem> items { get; private set; } = default!;
    public virtual global::System.Action<long>? onTap { get; private set; }
    public virtual long currentIndex { get; private set; } = default!;
    public virtual double? elevation { get; private set; }
    public virtual BottomNavigationBarType? type { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual double iconSize { get; private set; } = default!;
    public virtual Color? selectedItemColor { get; private set; }
    public virtual Color? unselectedItemColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.IconThemeData? selectedIconTheme { get; private set; }
    public virtual global::Doroti.Framework.Widgets.IconThemeData? unselectedIconTheme { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? selectedLabelStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? unselectedLabelStyle { get; private set; }
    public virtual double selectedFontSize { get; private set; } = default!;
    public virtual double unselectedFontSize { get; private set; } = default!;
    public virtual bool? showUnselectedLabels { get; private set; }
    public virtual bool? showSelectedLabels { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual BottomNavigationBarLandscapeLayout? landscapeLayout { get; private set; }
    public virtual bool useLegacyColorScheme { get; private set; } = default!;

    public BottomNavigationBar(global::Doroti.Framework.Foundation.Key? key = null, List<global::Doroti.Framework.Widgets.BottomNavigationBarItem> items = default!, global::System.Action<long>? onTap = null, long currentIndex = 0, double? elevation = null, BottomNavigationBarType? type = null, Color? fixedColor = null, Color? backgroundColor = null, double iconSize = 24.0, Color? selectedItemColor = null, Color? unselectedItemColor = null, global::Doroti.Framework.Widgets.IconThemeData? selectedIconTheme = null, global::Doroti.Framework.Widgets.IconThemeData? unselectedIconTheme = null, double selectedFontSize = 14.0, double unselectedFontSize = 12.0, global::Doroti.Framework.Painting.TextStyle? selectedLabelStyle = null, global::Doroti.Framework.Painting.TextStyle? unselectedLabelStyle = null, bool? showSelectedLabels = null, bool? showUnselectedLabels = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, bool? enableFeedback = null, BottomNavigationBarLandscapeLayout? landscapeLayout = null, bool useLegacyColorScheme = true) : base(key: key)
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
        this.selectedItemColor = (selectedItemColor ?? fixedColor);
        System.Diagnostics.Debug.Assert((checked((long)(items.Count)) >= 2L));
        System.Diagnostics.Debug.Assert(items.All(((item) => (((global::Doroti.Framework.Widgets.BottomNavigationBarItem)item).label is not null))));
        System.Diagnostics.Debug.Assert(((0L <= currentIndex) && (currentIndex < checked((long)(items.Count)))));
        System.Diagnostics.Debug.Assert(((elevation is null) || (elevation >= 0.0)));
        System.Diagnostics.Debug.Assert((iconSize >= 0.0));
        System.Diagnostics.Debug.Assert(((selectedItemColor is null) || (fixedColor is null)));
        System.Diagnostics.Debug.Assert((selectedFontSize >= 0.0));
        System.Diagnostics.Debug.Assert((unselectedFontSize >= 0.0));
    }

    public virtual global::Doroti.Ui.Color? fixedColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this.selectedItemColor);
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _BottomNavigationBarState__bottom_navigation_bar());
}

internal class _BottomNavigationTile__bottom_navigation_bar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual BottomNavigationBarType type { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.BottomNavigationBarItem item { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual double iconSize { get; private set; } = default!;
    public virtual global::System.Action? onTap { get; private set; }
    public virtual global::Doroti.Framework.Animation.ColorTween? labelColorTween { get; private set; }
    public virtual global::Doroti.Framework.Animation.ColorTween? iconColorTween { get; private set; }
    public virtual double? flex { get; private set; }
    public virtual bool selected { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.IconThemeData? selectedIconTheme { get; private set; }
    public virtual global::Doroti.Framework.Widgets.IconThemeData? unselectedIconTheme { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle selectedLabelStyle { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle unselectedLabelStyle { get; private set; } = default!;
    public virtual string? indexLabel { get; private set; }
    public virtual bool showSelectedLabels { get; private set; } = default!;
    public virtual bool showUnselectedLabels { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.MouseCursor mouseCursor { get; private set; } = default!;
    public virtual bool enableFeedback { get; private set; } = default!;
    public virtual BottomNavigationBarLandscapeLayout layout { get; private set; } = default!;

    internal _BottomNavigationTile__bottom_navigation_bar(BottomNavigationBarType type, global::Doroti.Framework.Widgets.BottomNavigationBarItem item, global::Doroti.Framework.Animation.Animation<double> animation, double iconSize, global::Doroti.Framework.Foundation.Key? key = null, global::System.Action? onTap = null, global::Doroti.Framework.Animation.ColorTween? labelColorTween = null, global::Doroti.Framework.Animation.ColorTween? iconColorTween = null, double? flex = null, bool selected = false, global::Doroti.Framework.Painting.TextStyle selectedLabelStyle = default!, global::Doroti.Framework.Painting.TextStyle unselectedLabelStyle = default!, global::Doroti.Framework.Widgets.IconThemeData? selectedIconTheme = default!, global::Doroti.Framework.Widgets.IconThemeData? unselectedIconTheme = default!, bool showSelectedLabels = default!, bool showUnselectedLabels = default!, string? indexLabel = null, global::Doroti.Framework.Services.MouseCursor mouseCursor = default!, bool enableFeedback = default!, BottomNavigationBarLandscapeLayout layout = default!) : base(key: key)
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

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        long sizeLocal = default!;
        double selectedFontSize = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Painting.TextStyle)this.selectedLabelStyle).fontSize);
        double selectedIconSize = (this.selectedIconTheme?.size ?? this.iconSize);
        double unselectedIconSize = (this.unselectedIconTheme?.size ?? this.iconSize);
        double selectedIconDiff = Math.Max((selectedIconSize - unselectedIconSize), 0);
        double unselectedIconDiff = Math.Max((unselectedIconSize - selectedIconSize), 0);
        string? effectiveTooltip = ((((global::Doroti.Framework.Widgets.BottomNavigationBarItem)this.item).tooltip == "") ? null : ((global::Doroti.Framework.Widgets.BottomNavigationBarItem)this.item).tooltip);
        double bottomPadding = default!;
        double topPadding = default!;
        if ((this.showSelectedLabels && !this.showUnselectedLabels))
        {
            bottomPadding = new global::Doroti.Framework.Animation.Tween<double>(begin: (selectedIconDiff / 2.0), end: ((selectedFontSize / 2.0) - (unselectedIconDiff / 2.0))).evaluate(this.animation);
            topPadding = new global::Doroti.Framework.Animation.Tween<double>(begin: (selectedFontSize + (selectedIconDiff / 2.0)), end: ((selectedFontSize / 2.0) - (unselectedIconDiff / 2.0))).evaluate(this.animation);
        }
        else
        {
            if ((!this.showSelectedLabels && !this.showUnselectedLabels))
            {
                bottomPadding = new global::Doroti.Framework.Animation.Tween<double>(begin: (selectedIconDiff / 2.0), end: (unselectedIconDiff / 2.0)).evaluate(this.animation);
                topPadding = new global::Doroti.Framework.Animation.Tween<double>(begin: (selectedFontSize + (selectedIconDiff / 2.0)), end: (selectedFontSize + (unselectedIconDiff / 2.0))).evaluate(this.animation);
            }
            else
            {
                bottomPadding = new global::Doroti.Framework.Animation.Tween<double>(begin: ((selectedFontSize / 2.0) + (selectedIconDiff / 2.0)), end: ((selectedFontSize / 2.0) + (unselectedIconDiff / 2.0))).evaluate(this.animation);
                topPadding = new global::Doroti.Framework.Animation.Tween<double>(begin: ((selectedFontSize / 2.0) + (selectedIconDiff / 2.0)), end: ((selectedFontSize / 2.0) + (unselectedIconDiff / 2.0))).evaluate(this.animation);
            }
        }
        sizeLocal = (this.type switch { BottomNavigationBarType.@fixed => 1L, BottomNavigationBarType.shifting => ((DartRuntimePrimitives.RequireValue(this.flex) * 1000.0)).round(), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Framework.Widgets.Widget result = ((global::Doroti.Framework.Widgets.Widget)(object?)new InkResponse(onTap: this.onTap, mouseCursor: this.mouseCursor, enableFeedback: this.enableFeedback, child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(top: topPadding, bottom: bottomPadding), child: new _Tile__bottom_navigation_bar(layout: this.layout, icon: new _TileIcon__bottom_navigation_bar(colorTween: this.iconColorTween!, animation: this.animation, iconSize: this.iconSize, selected: this.selected, item: this.item, selectedIconTheme: this.selectedIconTheme, unselectedIconTheme: this.unselectedIconTheme), label: new _Label__bottom_navigation_bar(colorTween: this.labelColorTween!, animation: this.animation, item: this.item, selectedLabelStyle: this.selectedLabelStyle, unselectedLabelStyle: this.unselectedLabelStyle, showSelectedLabels: this.showSelectedLabels, showUnselectedLabels: this.showUnselectedLabels)))));
        if ((effectiveTooltip is not null))
        {
            result = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new Tooltip(message: effectiveTooltip, preferBelow: false, verticalOffset: (selectedIconSize + selectedFontSize), excludeFromSemantics: true, child: result));
        }
        result = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(selected: this.selected, button: true, container: true, child: new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(result), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(label: this.indexLabel)) })));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Expanded(flex: sizeLocal, child: result));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _Tile__bottom_navigation_bar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual BottomNavigationBarLandscapeLayout layout { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget icon { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget label { get; private set; } = default!;

    internal _Tile__bottom_navigation_bar(BottomNavigationBarLandscapeLayout layout, global::Doroti.Framework.Widgets.Widget icon, global::Doroti.Framework.Widgets.Widget label)
    {
        this.layout = layout;
        this.icon = icon;
        this.label = label;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if (((object.Equals(MediaQuery.orientationOf(context), global::Doroti.Framework.Widgets.Orientation.landscape)) && (object.Equals(this.layout, BottomNavigationBarLandscapeLayout.linear))))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Align(heightFactor: 1, child: new global::Doroti.Framework.Widgets.Row(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, spacing: 8, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(this.icon), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: new global::Doroti.Framework.Widgets.IntrinsicWidth(child: this.label))) })));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Column(mainAxisAlignment: global::Doroti.Framework.Rendering.MainAxisAlignment.spaceBetween, mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(this.icon), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(this.label) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TileIcon__bottom_navigation_bar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Animation.ColorTween colorTween { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual double iconSize { get; private set; } = default!;
    public virtual bool selected { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.BottomNavigationBarItem item { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.IconThemeData? selectedIconTheme { get; private set; }
    public virtual global::Doroti.Framework.Widgets.IconThemeData? unselectedIconTheme { get; private set; }

    internal _TileIcon__bottom_navigation_bar(global::Doroti.Framework.Animation.ColorTween colorTween, global::Doroti.Framework.Animation.Animation<double> animation, double iconSize, bool selected, global::Doroti.Framework.Widgets.BottomNavigationBarItem item, global::Doroti.Framework.Widgets.IconThemeData? selectedIconTheme, global::Doroti.Framework.Widgets.IconThemeData? unselectedIconTheme)
    {
        this.colorTween = colorTween;
        this.animation = animation;
        this.iconSize = iconSize;
        this.selected = selected;
        this.item = item;
        this.selectedIconTheme = selectedIconTheme;
        this.unselectedIconTheme = unselectedIconTheme;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color? iconColor = ((global::Doroti.Ui.Color?)(object?)this.colorTween.evaluate(this.animation));
        var defaultIconTheme = new global::Doroti.Framework.Widgets.IconThemeData(color: iconColor, size: this.iconSize);
        global::Doroti.Framework.Widgets.IconThemeData iconThemeData = ((global::Doroti.Framework.Widgets.IconThemeData)(object?)IconThemeData.lerp(defaultIconTheme.merge(this.unselectedIconTheme), defaultIconTheme.merge(this.selectedIconTheme), ((global::Doroti.Framework.Animation.Animation<double>)this.animation).value));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.Alignment.topCenter, heightFactor: 1.0, child: new global::Doroti.Framework.Widgets.IconTheme(data: iconThemeData, child: (this.selected ? ((global::Doroti.Framework.Widgets.BottomNavigationBarItem)this.item).activeIcon : ((global::Doroti.Framework.Widgets.BottomNavigationBarItem)this.item).icon))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _Label__bottom_navigation_bar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Animation.ColorTween colorTween { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.BottomNavigationBarItem item { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle selectedLabelStyle { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle unselectedLabelStyle { get; private set; } = default!;
    public virtual bool showSelectedLabels { get; private set; } = default!;
    public virtual bool showUnselectedLabels { get; private set; } = default!;

    internal _Label__bottom_navigation_bar(global::Doroti.Framework.Animation.ColorTween colorTween, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Widgets.BottomNavigationBarItem item, global::Doroti.Framework.Painting.TextStyle selectedLabelStyle, global::Doroti.Framework.Painting.TextStyle unselectedLabelStyle, bool showSelectedLabels, bool showUnselectedLabels)
    {
        this.colorTween = colorTween;
        this.animation = animation;
        this.item = item;
        this.selectedLabelStyle = selectedLabelStyle;
        this.unselectedLabelStyle = unselectedLabelStyle;
        this.showSelectedLabels = showSelectedLabels;
        this.showUnselectedLabels = showUnselectedLabels;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        double? selectedFontSize = ((global::Doroti.Framework.Painting.TextStyle)this.selectedLabelStyle).fontSize;
        double? unselectedFontSize = ((global::Doroti.Framework.Painting.TextStyle)this.unselectedLabelStyle).fontSize;
        global::Doroti.Framework.Painting.TextStyle customStyle = TextStyle.lerp(this.unselectedLabelStyle, this.selectedLabelStyle, ((global::Doroti.Framework.Animation.Animation<double>)this.animation).value)!;
        global::Doroti.Framework.Widgets.Widget text = ((global::Doroti.Framework.Widgets.Widget)(object?)DefaultTextStyle.merge(style: customStyle.copyWith(fontSize: selectedFontSize, color: this.colorTween.evaluate(this.animation)), child: new global::Doroti.Framework.Widgets.Transform(transform: Matrix4.diagonal3(new Vector3(new global::Doroti.Framework.Animation.Tween<double>(begin: (DartRuntimePrimitives.RequireValue(unselectedFontSize) / DartRuntimePrimitives.RequireValue(selectedFontSize)), end: 1.0).evaluate(this.animation))), alignment: global::Doroti.Framework.Painting.Alignment.bottomCenter, child: new global::Doroti.Framework.Widgets.Text(((global::Doroti.Framework.Widgets.BottomNavigationBarItem)this.item).label!, semanticsLabel: ((global::Doroti.Framework.Widgets.BottomNavigationBarItem)this.item).semanticsLabel))));
        if ((!this.showUnselectedLabels && !this.showSelectedLabels))
        {
            text = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(global::Doroti.Framework.Widgets.Visibility.CreateMaintain(visible: false, child: text));
        }
        else
        {
            if (!this.showUnselectedLabels)
            {
                text = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.FadeTransition(alwaysIncludeSemantics: true, opacity: this.animation, child: text));
            }
            else
            {
                if (!this.showSelectedLabels)
                {
                    text = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.FadeTransition(alwaysIncludeSemantics: true, opacity: new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0, end: 0.0).animate(this.animation), child: text));
                }
            }
        }
        text = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.Alignment.bottomCenter, heightFactor: 1.0, child: text));
        if ((((global::Doroti.Framework.Widgets.BottomNavigationBarItem)this.item).label is not null))
        {
            text = MediaQuery.withClampedTextScaling(maxScaleFactor: 1.0, child: text);
        }
        return text;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _BottomNavigationBarState__bottom_navigation_bar : global::Doroti.Framework.Widgets.State<BottomNavigationBar>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<BottomNavigationBar>
{
    internal virtual List<global::Doroti.Framework.Animation.AnimationController> _controllers { get; set; } = new List<global::Doroti.Framework.Animation.AnimationController>();
    internal virtual List<global::Doroti.Framework.Animation.CurvedAnimation> _animations { get; set; } = new List<global::Doroti.Framework.Animation.CurvedAnimation>();
    internal virtual Queue<_Circle__bottom_navigation_bar> _circles { get; private set; } = new Queue<_Circle__bottom_navigation_bar>();
    internal virtual Color? _backgroundColor { get; set; } = default;
    internal static global::Doroti.Framework.Animation.Animatable<double> _flexTween = ((global::Doroti.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0, end: 1.5));
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual void _resetState()
    {
        foreach (global::Doroti.Framework.Animation.AnimationController controller in this._controllers)
        {
            controller.dispose();
        }
        foreach (_Circle__bottom_navigation_bar circle in this._circles)
        {
            circle.dispose();
        }
        foreach (global::Doroti.Framework.Animation.CurvedAnimation animation in this._animations)
        {
            animation.dispose();
        }
        this._circles.Clear();
        _controllers = new List<global::Doroti.Framework.Animation.AnimationController>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)checked((long)(((BottomNavigationBar)this.widget).items.Count)))), ((index) =>
        {
            return ((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
            {
                var __cascade = new global::Doroti.Framework.Animation.AnimationController(duration: ThemeLibrary.kThemeAnimationDuration, vsync: this);
                __cascade.addListener(() => this._rebuild());
                return __cascade;
            }))();
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        _animations = new List<global::Doroti.Framework.Animation.CurvedAnimation>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)checked((long)(((BottomNavigationBar)this.widget).items.Count)))), ((index) =>
        {
            return new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._controllers[(int)(index)], curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn, reverseCurve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn.flipped);
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        this._controllers[(int)(((BottomNavigationBar)this.widget).currentIndex)].value = 1.0;
        _backgroundColor = ((BottomNavigationBar)this.widget).items[(int)(((BottomNavigationBar)this.widget).currentIndex)].backgroundColor;
    }

    internal virtual BottomNavigationBarType _effectiveType
    {
        get
        {
            return ((((BottomNavigationBar)this.widget).type ?? BottomNavigationBarTheme.of(this.context).type) ?? (((checked((long)(((BottomNavigationBar)this.widget).items.Count)) <= 3L) ? BottomNavigationBarType.@fixed : BottomNavigationBarType.shifting)));
            return default!;
        }
    }
    internal virtual bool _defaultShowUnselected => (this._effectiveType switch { BottomNavigationBarType.shifting => false, BottomNavigationBarType.@fixed => true, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    public override void initState()
    {
        base.initState();
        _resetState();
    }

    internal virtual void _rebuild()
    {
        setState(((global::System.Action)(() =>
        {
        })));
    }

    public override void dispose()
    {
        foreach (global::Doroti.Framework.Animation.AnimationController controller in this._controllers)
        {
            controller.dispose();
        }
        foreach (_Circle__bottom_navigation_bar circle in this._circles)
        {
            circle.dispose();
        }
        foreach (global::Doroti.Framework.Animation.CurvedAnimation animation in this._animations)
        {
            animation.dispose();
        }
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
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual double _evaluateFlex(global::Doroti.Framework.Animation.Animation<double> animation) => _flexTween.evaluate(animation);
    internal virtual void _pushCircle(long index)
    {
        if ((((BottomNavigationBar)this.widget).items[(int)(index)].backgroundColor is not null))
        {
            this._circles.Enqueue(((Func<_Circle__bottom_navigation_bar>)(() =>
{
    var __cascade = new _Circle__bottom_navigation_bar(state: this, index: index, color: ((BottomNavigationBar)this.widget).items[(int)(index)].backgroundColor!, vsync: this);
    __cascade.controller.addStatusListener(((AnimationStatusListener)((status) =>
    {
        if (global::Doroti.Framework.Animation.AnimationStatusMembers.isCompleted(status))
        {
            setState(((global::System.Action)(() =>
            {
                _Circle__bottom_navigation_bar circle = this._circles.Dequeue();
                _backgroundColor = ((_Circle__bottom_navigation_bar)circle).color;
                circle.dispose();
            })));
        }
    })));
    return __cascade;
}))());
        }
    }

    public override void didUpdateWidget(BottomNavigationBar oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((checked((long)(((BottomNavigationBar)this.widget).items.Count)) != checked((long)(((BottomNavigationBar)oldWidget).items.Count))))
        {
            _resetState();
            return;
        }
        if ((((BottomNavigationBar)this.widget).currentIndex != ((BottomNavigationBar)oldWidget).currentIndex))
        {
            switch (this._effectiveType)
            {
                case BottomNavigationBarType.@fixed:
                    {
                        break;
                    }
                case BottomNavigationBarType.shifting:
                    {
                        _pushCircle(((BottomNavigationBar)this.widget).currentIndex);
                        break;
                    }
            }
            this._controllers[(int)(((BottomNavigationBar)oldWidget).currentIndex)].reverse();
            this._controllers[(int)(((BottomNavigationBar)this.widget).currentIndex)].forward();
        }
        else
        {
            if ((!object.Equals(this._backgroundColor, ((BottomNavigationBar)this.widget).items[(int)(((BottomNavigationBar)this.widget).currentIndex)].backgroundColor)))
            {
                _backgroundColor = ((BottomNavigationBar)this.widget).items[(int)(((BottomNavigationBar)this.widget).currentIndex)].backgroundColor;
            }
        }
    }

    internal static global::Doroti.Framework.Painting.TextStyle _effectiveTextStyle(global::Doroti.Framework.Painting.TextStyle? textStyle, double fontSize)
    {
        textStyle ??= new global::Doroti.Framework.Painting.TextStyle();
        return ((((global::Doroti.Framework.Painting.TextStyle)textStyle).fontSize is null) ? textStyle.copyWith(fontSize: fontSize) : textStyle);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Framework.Widgets.IconThemeData _effectiveIconTheme(global::Doroti.Framework.Widgets.IconThemeData? iconTheme, Color? itemColor)
    {
        return (iconTheme ?? new global::Doroti.Framework.Widgets.IconThemeData(color: itemColor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<global::Doroti.Framework.Widgets.Widget> _createTiles(BottomNavigationBarLandscapeLayout layout)
    {
        MaterialLocalizations localizations = ((MaterialLocalizations)(object?)MaterialLocalizations.of(this.context));
        ThemeData themeData = Theme.of(this.context);
        BottomNavigationBarThemeData bottomTheme = BottomNavigationBarTheme.of(this.context);
        global::Doroti.Ui.Color themeColor = ((global::Doroti.Ui.Color)(object?)(themeData.brightness switch { Brightness.light => themeData.colorScheme.primary, Brightness.dark => themeData.colorScheme.secondary, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        global::Doroti.Framework.Painting.TextStyle effectiveSelectedLabelStyle = ((global::Doroti.Framework.Painting.TextStyle)(object?)_BottomNavigationBarState__bottom_navigation_bar._effectiveTextStyle((((BottomNavigationBar)this.widget).selectedLabelStyle ?? bottomTheme.selectedLabelStyle), ((BottomNavigationBar)this.widget).selectedFontSize));
        global::Doroti.Framework.Painting.TextStyle effectiveUnselectedLabelStyle = ((global::Doroti.Framework.Painting.TextStyle)(object?)_BottomNavigationBarState__bottom_navigation_bar._effectiveTextStyle((((BottomNavigationBar)this.widget).unselectedLabelStyle ?? bottomTheme.unselectedLabelStyle), ((BottomNavigationBar)this.widget).unselectedFontSize));
        global::Doroti.Framework.Widgets.IconThemeData effectiveSelectedIconTheme = ((global::Doroti.Framework.Widgets.IconThemeData)(object?)_BottomNavigationBarState__bottom_navigation_bar._effectiveIconTheme((((BottomNavigationBar)this.widget).selectedIconTheme ?? bottomTheme.selectedIconTheme), ((((BottomNavigationBar)this.widget).selectedItemColor ?? bottomTheme.selectedItemColor) ?? themeColor)));
        global::Doroti.Framework.Widgets.IconThemeData effectiveUnselectedIconTheme = ((global::Doroti.Framework.Widgets.IconThemeData)(object?)_BottomNavigationBarState__bottom_navigation_bar._effectiveIconTheme((((BottomNavigationBar)this.widget).unselectedIconTheme ?? bottomTheme.unselectedIconTheme), ((((BottomNavigationBar)this.widget).unselectedItemColor ?? bottomTheme.unselectedItemColor) ?? themeData.unselectedWidgetColor)));
        global::Doroti.Framework.Animation.ColorTween colorTween = default!;
        switch (this._effectiveType)
        {
            case BottomNavigationBarType.@fixed:
                {
                    colorTween = new global::Doroti.Framework.Animation.ColorTween(begin: ((((BottomNavigationBar)this.widget).unselectedItemColor ?? bottomTheme.unselectedItemColor) ?? themeData.unselectedWidgetColor), end: ((((((BottomNavigationBar)this.widget).selectedItemColor ?? bottomTheme.selectedItemColor) ?? (Color)((BottomNavigationBar)this.widget).fixedColor)) ?? themeColor));
                    break;
                }
            case BottomNavigationBarType.shifting:
                {
                    colorTween = new global::Doroti.Framework.Animation.ColorTween(begin: ((((BottomNavigationBar)this.widget).unselectedItemColor ?? bottomTheme.unselectedItemColor) ?? themeData.colorScheme.surface), end: ((((BottomNavigationBar)this.widget).selectedItemColor ?? bottomTheme.selectedItemColor) ?? themeData.colorScheme.surface));
                    break;
                }
        }
        global::Doroti.Framework.Animation.ColorTween labelColorTweenLocal = default!;
        switch (this._effectiveType)
        {
            case BottomNavigationBarType.@fixed:
                {
                    labelColorTweenLocal = new global::Doroti.Framework.Animation.ColorTween(begin: (((((global::Doroti.Framework.Painting.TextStyle)effectiveUnselectedLabelStyle).color ?? ((BottomNavigationBar)this.widget).unselectedItemColor) ?? bottomTheme.unselectedItemColor) ?? themeData.unselectedWidgetColor), end: (((((((global::Doroti.Framework.Painting.TextStyle)effectiveSelectedLabelStyle).color ?? ((BottomNavigationBar)this.widget).selectedItemColor) ?? bottomTheme.selectedItemColor) ?? (Color)((BottomNavigationBar)this.widget).fixedColor)) ?? themeColor));
                    break;
                }
            case BottomNavigationBarType.shifting:
                {
                    labelColorTweenLocal = new global::Doroti.Framework.Animation.ColorTween(begin: (((((global::Doroti.Framework.Painting.TextStyle)effectiveUnselectedLabelStyle).color ?? ((BottomNavigationBar)this.widget).unselectedItemColor) ?? bottomTheme.unselectedItemColor) ?? themeData.colorScheme.surface), end: (((((global::Doroti.Framework.Painting.TextStyle)effectiveSelectedLabelStyle).color ?? ((BottomNavigationBar)this.widget).selectedItemColor) ?? bottomTheme.selectedItemColor) ?? themeColor));
                    break;
                }
        }
        global::Doroti.Framework.Animation.ColorTween iconColorTweenLocal = default!;
        switch (this._effectiveType)
        {
            case BottomNavigationBarType.@fixed:
                {
                    iconColorTweenLocal = new global::Doroti.Framework.Animation.ColorTween(begin: (((((global::Doroti.Framework.Widgets.IconThemeData)effectiveSelectedIconTheme).color ?? ((BottomNavigationBar)this.widget).unselectedItemColor) ?? bottomTheme.unselectedItemColor) ?? themeData.unselectedWidgetColor), end: (((((((global::Doroti.Framework.Widgets.IconThemeData)effectiveUnselectedIconTheme).color ?? ((BottomNavigationBar)this.widget).selectedItemColor) ?? bottomTheme.selectedItemColor) ?? (Color)((BottomNavigationBar)this.widget).fixedColor)) ?? themeColor));
                    break;
                }
            case BottomNavigationBarType.shifting:
                {
                    iconColorTweenLocal = new global::Doroti.Framework.Animation.ColorTween(begin: (((((global::Doroti.Framework.Widgets.IconThemeData)effectiveUnselectedIconTheme).color ?? ((BottomNavigationBar)this.widget).unselectedItemColor) ?? bottomTheme.unselectedItemColor) ?? themeData.colorScheme.surface), end: (((((global::Doroti.Framework.Widgets.IconThemeData)effectiveSelectedIconTheme).color ?? ((BottomNavigationBar)this.widget).selectedItemColor) ?? bottomTheme.selectedItemColor) ?? themeColor));
                    break;
                }
        }
        var tiles = new List<global::Doroti.Framework.Widgets.Widget>();
        for (var i = 0L; (i < checked((long)(((BottomNavigationBar)this.widget).items.Count))); i++)
        {
            var states = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection39372 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if ((i == ((BottomNavigationBar)this.widget).currentIndex)) { __collection39372.Add(global::Doroti.Framework.Widgets.WidgetState.selected); } return __collection39372; }))();
            global::Doroti.Framework.Services.MouseCursor effectiveMouseCursor = ((((WidgetStateProperty.resolveAs<global::Doroti.Framework.Services.MouseCursor?>(((BottomNavigationBar)this.widget).mouseCursor, states) ?? (global::Doroti.Framework.Services.MouseCursor)bottomTheme.mouseCursor?.resolve(states))) ?? (global::Doroti.Framework.Services.MouseCursor)global::Doroti.Framework.Widgets.WidgetStateMouseCursor.clickable.resolve(states)));
            tiles.Add(new _BottomNavigationTile__bottom_navigation_bar(this._effectiveType, ((BottomNavigationBar)this.widget).items[(int)(i)], this._animations[(int)(i)], ((BottomNavigationBar)this.widget).iconSize, key: ((BottomNavigationBar)this.widget).items[(int)(i)].key, selectedIconTheme: (((BottomNavigationBar)this.widget).useLegacyColorScheme ? (((BottomNavigationBar)this.widget).selectedIconTheme ?? bottomTheme.selectedIconTheme) : effectiveSelectedIconTheme), unselectedIconTheme: (((BottomNavigationBar)this.widget).useLegacyColorScheme ? (((BottomNavigationBar)this.widget).unselectedIconTheme ?? bottomTheme.unselectedIconTheme) : effectiveUnselectedIconTheme), selectedLabelStyle: effectiveSelectedLabelStyle, unselectedLabelStyle: effectiveUnselectedLabelStyle, enableFeedback: ((((BottomNavigationBar)this.widget).enableFeedback ?? bottomTheme.enableFeedback) ?? true), onTap: ((global::System.Action)(() =>
            {
                ((BottomNavigationBar)this.widget).onTap?.Invoke(i);
            })), labelColorTween: (((BottomNavigationBar)this.widget).useLegacyColorScheme ? colorTween : labelColorTweenLocal), iconColorTween: (((BottomNavigationBar)this.widget).useLegacyColorScheme ? colorTween : iconColorTweenLocal), flex: _evaluateFlex(this._animations[(int)(i)]), selected: (i == ((BottomNavigationBar)this.widget).currentIndex), showSelectedLabels: ((((BottomNavigationBar)this.widget).showSelectedLabels ?? bottomTheme.showSelectedLabels) ?? true), showUnselectedLabels: (((((BottomNavigationBar)this.widget).showUnselectedLabels ?? bottomTheme.showUnselectedLabels) ?? (bool)this._defaultShowUnselected)), indexLabel: localizations.tabLabel(tabIndex: (i + 1L), tabCount: checked((long)(((BottomNavigationBar)this.widget).items.Count))), mouseCursor: effectiveMouseCursor, layout: layout));
        }
        return tiles;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasOverlay(context));
        BottomNavigationBarThemeData bottomTheme = BottomNavigationBarTheme.of(context);
        BottomNavigationBarLandscapeLayout layoutLocal = ((((BottomNavigationBar)this.widget).landscapeLayout ?? bottomTheme.landscapeLayout) ?? BottomNavigationBarLandscapeLayout.spread);
        double additionalBottomPadding = MediaQuery.viewPaddingOf(context).bottom;
        global::Doroti.Ui.Color? backgroundColorLocal = ((global::Doroti.Ui.Color?)(object?)(this._effectiveType switch { BottomNavigationBarType.@fixed => (((BottomNavigationBar)this.widget).backgroundColor ?? bottomTheme.backgroundColor), BottomNavigationBarType.shifting => this._backgroundColor, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(explicitChildNodes: true, child: new _Bar__bottom_navigation_bar(layout: layoutLocal, elevation: ((((BottomNavigationBar)this.widget).elevation ?? bottomTheme.elevation) ?? 8.0), color: backgroundColorLocal, child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minHeight: (ConstantsLibrary.kBottomNavigationBarHeight + additionalBottomPadding)), child: new global::Doroti.Framework.Widgets.CustomPaint(painter: new _RadialPainter__bottom_navigation_bar(circles: this._circles.ToList(), textDirection: Directionality.of(context)), child: new Material(type: MaterialType.transparency, child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(bottom: additionalBottomPadding), child: global::Doroti.Framework.Widgets.MediaQuery.CreateRemovePadding(context: context, removeBottom: true, child: DefaultTextStyle.merge(overflow: global::Doroti.Framework.Painting.TextOverflow.ellipsis, child: new global::Doroti.Framework.Widgets.Row(mainAxisAlignment: global::Doroti.Framework.Rendering.MainAxisAlignment.spaceBetween, children: _createTiles(layoutLocal)))))))))));
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

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

internal class _Bar__bottom_navigation_bar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual BottomNavigationBarLandscapeLayout layout { get; private set; } = default!;
    public virtual double elevation { get; private set; } = default!;
    public virtual Color? color { get; private set; }

    internal _Bar__bottom_navigation_bar(global::Doroti.Framework.Widgets.Widget child, BottomNavigationBarLandscapeLayout layout, double elevation, Color? color)
    {
        this.child = child;
        this.layout = layout;
        this.elevation = elevation;
        this.color = color;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Widgets.Widget alignedChild = this.child;
        if (((object.Equals(MediaQuery.orientationOf(context), global::Doroti.Framework.Widgets.Orientation.landscape)) && (object.Equals(this.layout, BottomNavigationBarLandscapeLayout.centered))))
        {
            alignedChild = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.Alignment.bottomCenter, heightFactor: 1, child: new global::Doroti.Framework.Widgets.SizedBox(width: MediaQuery.heightOf(context), child: this.child)));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new Material(elevation: this.elevation, color: this.color, child: alignedChild));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _Circle__bottom_navigation_bar
{
    public virtual _BottomNavigationBarState__bottom_navigation_bar state { get; private set; } = default!;
    public virtual long index { get; private set; } = default!;
    public virtual Color color { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.AnimationController controller { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation animation { get; set; } = default!;

    internal _Circle__bottom_navigation_bar(_BottomNavigationBarState__bottom_navigation_bar state, long index, Color color, global::Doroti.Framework.Scheduler.TickerProvider vsync)
    {
        this.state = state;
        this.index = index;
        this.color = color;
        controller = new global::Doroti.Framework.Animation.AnimationController(duration: ThemeLibrary.kThemeAnimationDuration, vsync: vsync);
        animation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this.controller, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
        this.controller.forward();
    }

    public virtual double horizontalLeadingOffset
    {
        get
        {
            double weightSum(IEnumerable<global::Doroti.Framework.Animation.Animation<double>> animations)
            {
                return System.Linq.Enumerable.Aggregate(animations.map<global::Doroti.Framework.Animation.Animation<double>, double>(((_BottomNavigationBarState__bottom_navigation_bar)this.state)._evaluateFlex), (double)0.0, ((sum, value) => (sum + value)));
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            double allWeights = weightSum(((_BottomNavigationBarState__bottom_navigation_bar)this.state)._animations.Cast<global::Doroti.Framework.Animation.Animation<double>>());
            double leadingWeights = weightSum(((_BottomNavigationBarState__bottom_navigation_bar)this.state)._animations.GetRange(0L, this.index).Cast<global::Doroti.Framework.Animation.Animation<double>>());
            return (((leadingWeights + (this.state._evaluateFlex(((_BottomNavigationBarState__bottom_navigation_bar)this.state)._animations[(int)(this.index)]) / 2.0))) / allWeights);
            return default!;
        }
    }
    public virtual void dispose()
    {
        this.controller.dispose();
        this.animation.dispose();
    }

}

internal class _RadialPainter__bottom_navigation_bar : global::Doroti.Framework.Rendering.CustomPainter
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
        double maxX = Math.Max(center.dx, (size.width - center.dx));
        double maxY = Math.Max(center.dy, (size.height - center.dy));
        return global::Doroti.Runtime.Dart_mathLibrary.sqrt(((maxX * maxX) + (maxY * maxY)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldPainter = (_RadialPainter__bottom_navigation_bar)(object)oldDelegate;
        if ((!object.Equals(this.textDirection, ((_RadialPainter__bottom_navigation_bar)__oldPainter).textDirection)))
        {
            return true;
        }
        if ((object.Equals(this.circles, ((_RadialPainter__bottom_navigation_bar)__oldPainter).circles)))
        {
            return false;
        }
        if ((checked((long)(this.circles.Count)) != checked((long)(((_RadialPainter__bottom_navigation_bar)__oldPainter).circles.Count))))
        {
            return true;
        }
        for (var i = 0L; (i < checked((long)(this.circles.Count))); i += 1L)
        {
            if ((!object.Equals(this.circles[(int)(i)], ((_RadialPainter__bottom_navigation_bar)__oldPainter).circles[(int)(i)])))
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(Canvas canvas, Size size)
    {
        foreach (_Circle__bottom_navigation_bar circle in this.circles)
        {
            var paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = ((_Circle__bottom_navigation_bar)circle).color;
    return __cascade;
}))();
            var rect = global::Doroti.Ui.Rect.fromLTWH(0.0, 0.0, size.width, size.height);
            canvas.clipRect(rect);
            double leftFraction = (this.textDirection switch { TextDirection.rtl => (1.0 - ((_Circle__bottom_navigation_bar)circle).horizontalLeadingOffset), TextDirection.ltr => ((_Circle__bottom_navigation_bar)circle).horizontalLeadingOffset, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            var center = new global::Doroti.Ui.Offset((leftFraction * size.width), (size.height / 2.0));
            var radiusTween = new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: _RadialPainter__bottom_navigation_bar._maxRadius(center, size));
            canvas.drawCircle(center, radiusTween.transform(((_Circle__bottom_navigation_bar)circle).animation.value), paintLocal);
        }
    }

}
