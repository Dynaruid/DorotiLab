// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/dropdown.dart
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

public static partial class DropdownLibrary
{
    internal static Duration _kDropdownMenuDuration = Duration.Create(milliseconds: 300L);
}

public static partial class DropdownLibrary
{
    internal static double _kMenuItemHeight = ConstantsLibrary.kMinInteractiveDimension;
}

public static partial class DropdownLibrary
{
    internal static double _kDenseButtonHeight = 24.0;
}

public static partial class DropdownLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.EdgeInsets _kMenuItemPadding = global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 16.0);
}

public static partial class DropdownLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry _kAlignedButtonPadding = ((global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry)(object?)global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 16.0, end: 4.0));
}

public static partial class DropdownLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.EdgeInsets _kUnalignedButtonPadding = global::Doroti.Generated.Framework.Painting.EdgeInsets.zero;
}

public static partial class DropdownLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.EdgeInsets _kAlignedMenuMargin = global::Doroti.Generated.Framework.Painting.EdgeInsets.zero;
}

public static partial class DropdownLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry _kUnalignedMenuMargin = ((global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry)(object?)global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 16.0, end: 24.0));
}

public delegate List<global::Doroti.Generated.Framework.Widgets.Widget> DropdownButtonBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context);

internal class _DropdownMenuPainter__dropdown : global::Doroti.Generated.Framework.Rendering.CustomPainter
{
    public virtual Color? color { get; private set; }
    public virtual long? elevation { get; private set; }
    public virtual long? selectedIndex { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius { get; private set; }
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> resize { get; private set; } = default!;
    public virtual global::System.Func<double> getSelectedItemOffset { get; private set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Painting.BoxPainter _painter { get; private set; } = default!;

    internal _DropdownMenuPainter__dropdown(Color? color = null, long? elevation = null, long? selectedIndex = null, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Generated.Framework.Animation.Animation<double> resize = default!, global::System.Func<double> getSelectedItemOffset = default!) : base(repaint: resize)
    {
        this.color = color;
        this.elevation = elevation;
        this.selectedIndex = selectedIndex;
        this.borderRadius = borderRadius;
        this.resize = resize;
        this.getSelectedItemOffset = getSelectedItemOffset;
        this._painter = new global::Doroti.Generated.Framework.Painting.BoxDecoration(color: color, borderRadius: (borderRadius ?? global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(2.0))), boxShadow: ShadowsLibrary.kElevationToShadow.GetValueOrDefault(elevation)).createBoxPainter();
    }

    public override void paint(Canvas canvas, Size size)
    {
        double selectedItemOffset__2754 = this.getSelectedItemOffset();
        var top__2810 = new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: Dart_uiLibrary.clampDouble(selectedItemOffset__2754, 0.0, Math.Max((size.height - DropdownLibrary._kMenuItemHeight), 0.0)), end: 0.0);
        var bottom__2963 = new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: Dart_uiLibrary.clampDouble((DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Animation.Tween<double>)top__2810).begin) + DropdownLibrary._kMenuItemHeight), Math.Min(DropdownLibrary._kMenuItemHeight, size.height), size.height), end: size.height);
        var rect__3173 = global::Doroti.Ui.Rect.fromLTRB(0.0, top__2810.evaluate(this.resize), size.width, bottom__2963.evaluate(this.resize));
        this._painter.paint(canvas, rect__3173.topLeft, new global::Doroti.Generated.Framework.Painting.ImageConfiguration(size: rect__3173.size));
    }

    public override bool shouldRepaint(global::Doroti.Generated.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldPainter = (_DropdownMenuPainter__dropdown)(object)oldDelegate;
        return (((((!object.Equals(((_DropdownMenuPainter__dropdown)__oldPainter).color, this.color)) || (((_DropdownMenuPainter__dropdown)__oldPainter).elevation != this.elevation)) || (((_DropdownMenuPainter__dropdown)__oldPainter).selectedIndex != this.selectedIndex)) || (!object.Equals(((_DropdownMenuPainter__dropdown)__oldPainter).borderRadius, this.borderRadius))) || (!object.Equals(((_DropdownMenuPainter__dropdown)__oldPainter).resize, this.resize)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _DropdownMenuItemButton__dropdown<T> : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual _DropdownRoute__dropdown<T> route { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.ScrollController scrollController { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets? padding { get; private set; }
    public virtual Rect buttonRect { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints { get; private set; } = default!;
    public virtual long itemIndex { get; private set; } = default!;
    public virtual bool enableFeedback { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor { get; private set; }

    internal _DropdownMenuItemButton__dropdown(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.EdgeInsets? padding = null, _DropdownRoute__dropdown<T> route = default!, Rect buttonRect = default!, global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints = default!, long itemIndex = default!, bool enableFeedback = default!, global::Doroti.Generated.Framework.Widgets.ScrollController scrollController = default!, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null) : base(key: key)
    {
        this.padding = padding;
        this.route = route;
        this.buttonRect = buttonRect;
        this.constraints = constraints;
        this.itemIndex = itemIndex;
        this.enableFeedback = enableFeedback;
        this.scrollController = scrollController;
        this.mouseCursor = mouseCursor;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DropdownMenuItemButtonState__dropdown<T>());
}

public class _DropdownMenuItemButtonState__dropdown<T> : global::Doroti.Generated.Framework.Widgets.State<_DropdownMenuItemButton__dropdown<T>>
{
    internal virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation _opacityAnimation { get; set; } = default!;
    internal static DartMap<global::Doroti.Generated.Framework.Widgets.ShortcutActivator, global::Doroti.Generated.Framework.Widgets.Intent> _webShortcuts = new DartMap<global::Doroti.Generated.Framework.Widgets.ShortcutActivator, global::Doroti.Generated.Framework.Widgets.Intent> { [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowDown)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent(global::Doroti.Generated.Framework.Widgets.TraversalDirection.down)), [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowUp)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent(global::Doroti.Generated.Framework.Widgets.TraversalDirection.up)) };

    public override void initState()
    {
        base.initState();
        _setOpacityAnimation();
    }

    public override void didUpdateWidget(_DropdownMenuItemButton__dropdown<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (((((((_DropdownMenuItemButton__dropdown<T>)oldWidget).itemIndex != ((_DropdownMenuItemButton__dropdown<T>)(object)this.widget).itemIndex) || (!object.Equals(((_DropdownMenuItemButton__dropdown<T>)oldWidget).route.animation, ((_DropdownMenuItemButton__dropdown<T>)(object)this.widget).route.animation))) || (((_DropdownMenuItemButton__dropdown<T>)oldWidget).route.selectedIndex != ((_DropdownMenuItemButton__dropdown<T>)(object)this.widget).route.selectedIndex)) || (checked((long)(((_DropdownMenuItemButton__dropdown<T>)(object)this.widget).route.items.Count)) != checked((long)(((_DropdownMenuItemButton__dropdown<T>)oldWidget).route.items.Count)))))
        {
            this._opacityAnimation.dispose();
            _setOpacityAnimation();
        }
    }

    internal virtual void _setOpacityAnimation()
    {
        double unit__5102 = (0.5 / ((checked((long)(((_DropdownMenuItemButton__dropdown<T>)(object)this.widget).route.items.Count)) + 1.5)));
        if ((((_DropdownMenuItemButton__dropdown<T>)(object)this.widget).itemIndex == ((_DropdownMenuItemButton__dropdown<T>)(object)this.widget).route.selectedIndex))
        {
            _opacityAnimation = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: ((_DropdownMenuItemButton__dropdown<T>)(object)this.widget).route.animation!, curve: new global::Doroti.Generated.Framework.Animation.Threshold(0.0));
        }
        else
        {
            double start__5370 = Dart_uiLibrary.clampDouble((0.5 + (((((_DropdownMenuItemButton__dropdown<T>)(object)this.widget).itemIndex + 1L)) * unit__5102)), 0.0, 1.0);
            double end__5457 = Dart_uiLibrary.clampDouble((start__5370 + (1.5 * unit__5102)), 0.0, 1.0);
            _opacityAnimation = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: ((_DropdownMenuItemButton__dropdown<T>)(object)this.widget).route.animation!, curve: new global::Doroti.Generated.Framework.Animation.Interval(start__5370, end__5457));
        }
    }

    internal virtual void _handleFocusChange(bool focused)
    {
        bool inTraditionalMode__5704 = (global::Doroti.Generated.Framework.Widgets.FocusManager.instance.highlightMode switch { global::Doroti.Generated.Framework.Widgets.FocusHighlightMode.touch => false, global::Doroti.Generated.Framework.Widgets.FocusHighlightMode.traditional => true, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        if ((focused && inTraditionalMode__5704))
        {
            _MenuLimits__dropdown menuLimits__5930 = ((_MenuLimits__dropdown)(object?)((_DropdownMenuItemButton__dropdown<T>)(object)this.widget).route.getMenuLimits(((_DropdownMenuItemButton__dropdown<T>)(object)this.widget).buttonRect, ((_DropdownMenuItemButton__dropdown<T>)(object)this.widget).constraints.maxHeight, ((_DropdownMenuItemButton__dropdown<T>)(object)this.widget).itemIndex));
            DartRuntimePrimitives.Ignore(((_DropdownMenuItemButton__dropdown<T>)(object)this.widget).scrollController.animateTo(((_MenuLimits__dropdown)menuLimits__5930).scrollOffset, curve: global::Doroti.Generated.Framework.Animation.Curves.easeInOut, duration: Duration.Create(milliseconds: 100L)));
        }
    }

    internal virtual void _handleOnTap()
    {
        DropdownMenuItem<T> dropdownMenuItem__6305 = ((_DropdownMenuItemButton__dropdown<T>)(object)this.widget).route.items[(int)(((_DropdownMenuItemButton__dropdown<T>)(object)this.widget).itemIndex)].item!;
        ((DropdownMenuItem<T>)dropdownMenuItem__6305).onTap?.Invoke();
        Navigator.pop<object>(this.context, new _DropdownRouteResult__dropdown<T>(((DropdownMenuItem<T>)dropdownMenuItem__6305).value));
    }

    public override void dispose()
    {
        this._opacityAnimation.dispose();
        base.dispose();
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DropdownMenuItem<T> dropdownMenuItem__7073 = ((_DropdownMenuItemButton__dropdown<T>)(object)this.widget).route.items[(int)(((_DropdownMenuItemButton__dropdown<T>)(object)this.widget).itemIndex)].item!;
        global::Doroti.Generated.Framework.Widgets.Widget child__7147 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)((_DropdownMenuItemButton__dropdown<T>)(object)this.widget).route.items[(int)(((_DropdownMenuItemButton__dropdown<T>)(object)this.widget).itemIndex)]);
        if (((_DropdownMenuItemButton__dropdown<T>)(object)this.widget).padding is global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding__7246)
        {
            child__7147 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: padding__7246, child: child__7147));
        }
        child__7147 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(height: ((_DropdownMenuItemButton__dropdown<T>)(object)this.widget).route.itemHeight, child: child__7147));
        var isSelected__7398 = (((_DropdownMenuItemButton__dropdown<T>)(object)this.widget).itemIndex == ((_DropdownMenuItemButton__dropdown<T>)(object)this.widget).route.selectedIndex);
        global::Doroti.Generated.Framework.Widgets.FocusHighlightMode highlightMode__7488 = global::Doroti.Generated.Framework.Widgets.FocusManager.instance.highlightMode;
        if (((DropdownMenuItem<T>)dropdownMenuItem__7073).enabled)
        {
            child__7147 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new InkWell(autofocus: isSelected__7398, enableFeedback: ((_DropdownMenuItemButton__dropdown<T>)(object)this.widget).enableFeedback, onTap: this._handleOnTap, onFocusChange: this._handleFocusChange, mouseCursor: ((_DropdownMenuItemButton__dropdown<T>)(object)this.widget).mouseCursor, child: ((object.Equals(highlightMode__7488, global::Doroti.Generated.Framework.Widgets.FocusHighlightMode.touch)) ? new Ink(color: (isSelected__7398 ? Theme.of(context).focusColor : null), child: child__7147) : child__7147)));
        }
        child__7147 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: this._opacityAnimation, child: child__7147));
        if ((global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb && ((DropdownMenuItem<T>)dropdownMenuItem__7073).enabled))
        {
            child__7147 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Shortcuts(shortcuts: _webShortcuts, child: child__7147));
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Semantics(role: SemanticsRole.menuItem, child: child__7147));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DropdownMenu__dropdown<T> : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual _DropdownRoute__dropdown<T> route { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets? padding { get; private set; }
    public virtual Rect buttonRect { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints { get; private set; } = default!;
    public virtual Color? dropdownColor { get; private set; }
    public virtual bool enableFeedback { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.ScrollController scrollController { get; private set; } = default!;
    public virtual double? menuWidth { get; private set; }
    public virtual global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor { get; private set; }

    internal _DropdownMenu__dropdown(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.EdgeInsets? padding = null, _DropdownRoute__dropdown<T> route = default!, Rect buttonRect = default!, global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints = default!, Color? dropdownColor = null, bool enableFeedback = default!, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Generated.Framework.Widgets.ScrollController scrollController = default!, double? menuWidth = null, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null) : base(key: key)
    {
        this.padding = padding;
        this.route = route;
        this.buttonRect = buttonRect;
        this.constraints = constraints;
        this.dropdownColor = dropdownColor;
        this.enableFeedback = enableFeedback;
        this.borderRadius = borderRadius;
        this.scrollController = scrollController;
        this.menuWidth = menuWidth;
        this.mouseCursor = mouseCursor;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DropdownMenuState__dropdown<T>());
}

internal class _DropdownMenuState__dropdown<T> : global::Doroti.Generated.Framework.Widgets.State<_DropdownMenu__dropdown<T>>
{
    internal virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation _fadeOpacity { get; private set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation _resize { get; private set; } = default!;

    public override void initState()
    {
        base.initState();
        _fadeOpacity = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: ((_DropdownMenu__dropdown<T>)(object)this.widget).route.animation!, curve: new global::Doroti.Generated.Framework.Animation.Interval(0.0, 0.25), reverseCurve: new global::Doroti.Generated.Framework.Animation.Interval(0.75, 1.0));
        _resize = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: ((_DropdownMenu__dropdown<T>)(object)this.widget).route.animation!, curve: new global::Doroti.Generated.Framework.Animation.Interval(0.25, 0.5), reverseCurve: new global::Doroti.Generated.Framework.Animation.Threshold(0.0));
    }

    public override void dispose()
    {
        this._fadeOpacity.dispose();
        this._resize.dispose();
        base.dispose();
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        MaterialLocalizations localizations__10764 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        _DropdownRoute__dropdown<T> route__10843 = ((_DropdownMenu__dropdown<T>)(object)this.widget).route;
        var children__10875 = ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection10886 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); for (long itemIndex__10911 = 0L; (itemIndex__10911 < checked((long)(((_DropdownRoute__dropdown<T>)route__10843).items.Count))); ++itemIndex__10911) { __collection10886.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new _DropdownMenuItemButton__dropdown<T>(route: ((_DropdownMenu__dropdown<T>)(object)this.widget).route, padding: ((_DropdownMenu__dropdown<T>)(object)this.widget).padding, buttonRect: ((_DropdownMenu__dropdown<T>)(object)this.widget).buttonRect, constraints: ((_DropdownMenu__dropdown<T>)(object)this.widget).constraints, itemIndex: itemIndex__10911, enableFeedback: ((_DropdownMenu__dropdown<T>)(object)this.widget).enableFeedback, scrollController: ((_DropdownMenu__dropdown<T>)(object)this.widget).scrollController, mouseCursor: ((_DropdownMenu__dropdown<T>)(object)this.widget).mouseCursor))); } return __collection10886; }))();
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: this._fadeOpacity, child: new global::Doroti.Generated.Framework.Widgets.CustomPaint(painter: new _DropdownMenuPainter__dropdown(color: (((_DropdownMenu__dropdown<T>)(object)this.widget).dropdownColor ?? Theme.of(context).canvasColor), elevation: ((_DropdownRoute__dropdown<T>)route__10843).elevation, selectedIndex: ((_DropdownRoute__dropdown<T>)route__10843).selectedIndex, resize: this._resize, borderRadius: ((_DropdownMenu__dropdown<T>)(object)this.widget).borderRadius, getSelectedItemOffset: ((global::System.Func<double>)(() => route__10843.getItemOffset(((_DropdownRoute__dropdown<T>)route__10843).selectedIndex)))), child: new global::Doroti.Generated.Framework.Widgets.Semantics(role: SemanticsRole.menu, scopesRoute: true, namesRoute: true, explicitChildNodes: true, label: ((MaterialLocalizations)localizations__10764).popupMenuLabel, child: new global::Doroti.Generated.Framework.Widgets.ClipRRect(borderRadius: (((_DropdownMenu__dropdown<T>)(object)this.widget).borderRadius ?? global::Doroti.Generated.Framework.Painting.BorderRadius.zero), clipBehavior: ((((_DropdownMenu__dropdown<T>)(object)this.widget).borderRadius is not null) ? Clip.antiAlias : Clip.none), child: new Material(type: MaterialType.transparency, textStyle: ((_DropdownRoute__dropdown<T>)route__10843).style, child: new global::Doroti.Generated.Framework.Widgets.ScrollConfiguration(behavior: ScrollConfiguration.of(context).copyWith(scrollbars: false, overscroll: false, physics: new global::Doroti.Generated.Framework.Widgets.ClampingScrollPhysics(), platform: Theme.of(context).platform), child: new global::Doroti.Generated.Framework.Widgets.PrimaryScrollController(controller: ((_DropdownMenu__dropdown<T>)(object)this.widget).scrollController, child: new Scrollbar(thumbVisibility: true, child: new global::Doroti.Generated.Framework.Widgets.ListView(primary: true, padding: ConstantsLibrary.kMaterialListPadding, shrinkWrap: true, children: children__10875))))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DropdownMenuRouteLayout__dropdown<T> : global::Doroti.Generated.Framework.Rendering.SingleChildLayoutDelegate
{
    public virtual Rect buttonRect { get; private set; } = default!;
    public virtual _DropdownRoute__dropdown<T> route { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }
    public virtual double? menuWidth { get; private set; }

    internal _DropdownMenuRouteLayout__dropdown(Rect buttonRect, _DropdownRoute__dropdown<T> route, TextDirection? textDirection, double? menuWidth = null)
    {
        this.buttonRect = buttonRect;
        this.route = route;
        this.textDirection = textDirection;
        this.menuWidth = menuWidth;
    }

    public override global::Doroti.Generated.Framework.Rendering.BoxConstraints getConstraintsForChild(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        double maxHeight__14295 = Math.Max(0.0, (((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxHeight - (2L * DropdownLibrary._kMenuItemHeight)));
        if (((((_DropdownRoute__dropdown<T>)this.route).menuMaxHeight is not null) && (DartRuntimePrimitives.RequireValue(((_DropdownRoute__dropdown<T>)this.route).menuMaxHeight) <= maxHeight__14295)))
        {
            maxHeight__14295 = DartRuntimePrimitives.RequireValue(((_DropdownRoute__dropdown<T>)this.route).menuMaxHeight);
        }
        double width__14663 = Math.Min(((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxWidth, (this.menuWidth ?? this.buttonRect.width));
        return new global::Doroti.Generated.Framework.Rendering.BoxConstraints(minWidth: width__14663, maxWidth: width__14663, maxHeight: maxHeight__14295);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Offset getPositionForChild(Size size, Size childSize)
    {
        _MenuLimits__dropdown menuLimits__14914 = ((_MenuLimits__dropdown)(object?)this.route.getMenuLimits(this.buttonRect, size.height, ((_DropdownRoute__dropdown<T>)this.route).selectedIndex));
        DartRuntimePrimitives.Assert(() =>
            {
                global::Doroti.Ui.Rect container__15053 = ((global::Doroti.Ui.Rect)(object?)(Offset.zero & size));
                if ((object.Equals(container__15053.intersect(this.buttonRect), this.buttonRect)))
                {
                    DartRuntimePrimitives.Assert(() => (((_MenuLimits__dropdown)menuLimits__14914).top >= 0.0));
                    DartRuntimePrimitives.Assert(() => ((((_MenuLimits__dropdown)menuLimits__14914).top + ((_MenuLimits__dropdown)menuLimits__14914).height) <= size.height));
                }
                return true;
            });
        DartRuntimePrimitives.Assert(() => (this.textDirection is not null));
        double left__15506 = (DartRuntimePrimitives.RequireValue(this.textDirection) switch { TextDirection.rtl => (Dart_uiLibrary.clampDouble(this.buttonRect.right, 0.0, size.width) - childSize.width), TextDirection.ltr => Dart_uiLibrary.clampDouble(this.buttonRect.left, 0.0, (size.width - childSize.width)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return new global::Doroti.Ui.Offset(left__15506, ((_MenuLimits__dropdown)menuLimits__14914).top);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRelayout(global::Doroti.Generated.Framework.Rendering.SingleChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (_DropdownMenuRouteLayout__dropdown<T>)(object)oldDelegate;
        return ((!object.Equals(this.buttonRect, ((_DropdownMenuRouteLayout__dropdown<T>)__oldDelegate).buttonRect)) || (!object.Equals(this.textDirection, ((_DropdownMenuRouteLayout__dropdown<T>)__oldDelegate).textDirection)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _DropdownRouteResult__dropdown<T>
{
    public virtual T? result { get; private set; }

    internal _DropdownRouteResult__dropdown(T? result)
    {
        this.result = result;
    }

    public override bool Equals(object? other)
    {
        var __other = other as _DropdownRouteResult__dropdown<T>;
        if (__other is null) return false;
        return ((__other is _DropdownRouteResult__dropdown<T>) && EqualityComparer<T>.Default.Equals(((_DropdownRouteResult__dropdown<T>)((_DropdownRouteResult__dropdown<T>)__other)).result, this.result));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(this.result.GetHashCode());
}

public class _MenuLimits__dropdown
{
    public virtual double top { get; private set; } = default!;
    public virtual double bottom { get; private set; } = default!;
    public virtual double height { get; private set; } = default!;
    public virtual double scrollOffset { get; private set; } = default!;

    internal _MenuLimits__dropdown(double top, double bottom, double height, double scrollOffset)
    {
        this.top = top;
        this.bottom = bottom;
        this.height = height;
        this.scrollOffset = scrollOffset;
    }

}

public class _DropdownRoute__dropdown<T> : global::Doroti.Generated.Framework.Widgets.PopupRoute<_DropdownRouteResult__dropdown<T>>
{
    public virtual List<_MenuItem__dropdown<T>> items { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding { get; private set; } = default!;
    public virtual Rect buttonRect { get; private set; } = default!;
    public virtual long selectedIndex { get; private set; } = default!;
    public virtual long elevation { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.CapturedThemes capturedThemes { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle style { get; private set; } = default!;
    public virtual double? itemHeight { get; private set; }
    public virtual double? menuWidth { get; private set; }
    public virtual Color? dropdownColor { get; private set; }
    public virtual double? menuMaxHeight { get; private set; }
    public virtual bool enableFeedback { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius { get; private set; }
    public virtual global::Doroti.Generated.Framework.Services.MouseCursor? dropdownMenuItemMouseCursor { get; private set; }
    public virtual List<double> itemHeights { get; private set; } = default!;
    private bool __field_barrierDismissible = default!;
    public override bool barrierDismissible { get => __field_barrierDismissible; }
    private string? __field_barrierLabel = default!;
    public override string? barrierLabel { get => __field_barrierLabel; }

    internal _DropdownRoute__dropdown(List<_MenuItem__dropdown<T>> items, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding, Rect buttonRect, long selectedIndex, long elevation = 8, global::Doroti.Generated.Framework.Widgets.CapturedThemes capturedThemes = default!, global::Doroti.Generated.Framework.Painting.TextStyle style = default!, string? barrierLabel = null, double? itemHeight = null, double? menuWidth = null, Color? dropdownColor = null, double? menuMaxHeight = null, bool enableFeedback = default!, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null, bool barrierDismissible = true, global::Doroti.Generated.Framework.Services.MouseCursor? dropdownMenuItemMouseCursor = null)
    {
        this.items = items;
        this.padding = padding;
        this.buttonRect = buttonRect;
        this.selectedIndex = selectedIndex;
        this.elevation = elevation;
        this.capturedThemes = capturedThemes;
        this.style = style;
        this.__field_barrierLabel = barrierLabel;
        this.itemHeight = itemHeight;
        this.menuWidth = menuWidth;
        this.dropdownColor = dropdownColor;
        this.menuMaxHeight = menuMaxHeight;
        this.enableFeedback = enableFeedback;
        this.borderRadius = borderRadius;
        this.__field_barrierDismissible = barrierDismissible;
        this.dropdownMenuItemMouseCursor = dropdownMenuItemMouseCursor;
        this.itemHeights = new List<double>(System.Linq.Enumerable.Repeat<double>((itemHeight ?? ConstantsLibrary.kMinInteractiveDimension), checked((int)checked((long)(items.Count)))));
    }

    public override Duration transitionDuration => DropdownLibrary._kDropdownMenuDuration;
    public override Color? barrierColor => DartRuntimePrimitives.ConvertValue<Color>(null);
    public override global::Doroti.Generated.Framework.Widgets.Widget buildPage(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.LayoutBuilder(builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Rendering.BoxConstraints, global::Doroti.Generated.Framework.Widgets.Widget>)((context, constraints) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _DropdownRoutePage__dropdown<T>(route: this, constraints: constraints, items: this.items, padding: this.padding, buttonRect: this.buttonRect, selectedIndex: this.selectedIndex, elevation: this.elevation, capturedThemes: this.capturedThemes, style: this.style, dropdownColor: this.dropdownColor, enableFeedback: this.enableFeedback, borderRadius: this.borderRadius, menuWidth: this.menuWidth, mouseCursor: this.dropdownMenuItemMouseCursor));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _dismiss()
    {
        if (this.isActive)
        {
            this.navigator?.removeRoute(this);
        }
    }

    public virtual double getItemOffset(long index)
    {
        double offset__18858 = ((global::Doroti.Generated.Framework.Painting.EdgeInsets)ConstantsLibrary.kMaterialListPadding).top;
        if ((System.Linq.Enumerable.Any(this.items) && (index > 0L)))
        {
            DartRuntimePrimitives.Assert(() => (checked((long)(this.items.Count)) == checked((long)(this.itemHeights.Count))));
            offset__18858 += this.itemHeights.GetRange(0L, index).reduce(((total, height) => (total + height)));
        }
        return offset__18858;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual _MenuLimits__dropdown getMenuLimits(Rect buttonRect, double availableHeight, long index)
    {
        double computedMaxHeight__19504 = (availableHeight - (2.0 * DropdownLibrary._kMenuItemHeight));
        if ((this.menuMaxHeight is not null))
        {
            double menuMaxHeight__value19574 = DartRuntimePrimitives.RequireValue(menuMaxHeight);
            computedMaxHeight__19504 = Math.Min(computedMaxHeight__19504, DartRuntimePrimitives.RequireValue(this.menuMaxHeight));
        }
        double buttonTop__19693 = buttonRect.top;
        double buttonBottom__19738 = Math.Min(buttonRect.bottom, availableHeight);
        double selectedItemOffset__19816 = getItemOffset(index);
        double topLimit__20148 = Math.Min(DropdownLibrary._kMenuItemHeight, buttonTop__19693);
        double bottomLimit__20215 = Math.Max((availableHeight - DropdownLibrary._kMenuItemHeight), buttonBottom__19738);
        double menuTop__20301 = (((buttonTop__19693 - selectedItemOffset__19816)) - (((this.itemHeights[(int)(this.selectedIndex)] - buttonRect.height)) / 2.0));
        double preferredMenuHeight__20421 = ConstantsLibrary.kMaterialListPadding.vertical;
        if (System.Linq.Enumerable.Any(this.items))
        {
            preferredMenuHeight__20421 += this.itemHeights.reduce(((total, height) => (total + height)));
        }
        double menuHeight__20748 = Math.Min(computedMaxHeight__19504, preferredMenuHeight__20421);
        double menuBottom__20822 = (menuTop__20301 + menuHeight__20748);
        if ((menuTop__20301 < topLimit__20148))
        {
            menuTop__20301 = Math.Min(buttonTop__19693, topLimit__20148);
            menuBottom__20822 = (menuTop__20301 + menuHeight__20748);
        }
        if ((menuBottom__20822 > bottomLimit__20215))
        {
            menuBottom__20822 = Math.Max(buttonBottom__19738, bottomLimit__20215);
            menuTop__20301 = (menuBottom__20822 - menuHeight__20748);
        }
        if (((menuBottom__20822 - (this.itemHeights[(int)(this.selectedIndex)] / 2.0)) < (buttonBottom__19738 - (buttonRect.height / 2.0))))
        {
            menuBottom__20822 = ((buttonBottom__19738 - (buttonRect.height / 2.0)) + (this.itemHeights[(int)(this.selectedIndex)] / 2.0));
            menuTop__20301 = (menuBottom__20822 - menuHeight__20748);
        }
        double scrollOffset__21708 = 0;
        if ((preferredMenuHeight__20421 > computedMaxHeight__19504))
        {
            scrollOffset__21708 = Math.Max(0.0, (selectedItemOffset__19816 - ((buttonTop__19693 - menuTop__20301))));
            scrollOffset__21708 = Math.Min(scrollOffset__21708, (preferredMenuHeight__20421 - menuHeight__20748));
        }
        DartRuntimePrimitives.Assert(() => ((((menuBottom__20822 - menuTop__20301) - menuHeight__20748)).abs() < global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance));
        return new _MenuLimits__dropdown(menuTop__20301, menuBottom__20822, menuHeight__20748, scrollOffset__21708);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DropdownRoutePage__dropdown<T> : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual _DropdownRoute__dropdown<T> route { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints { get; private set; } = default!;
    public virtual List<_MenuItem__dropdown<T>>? items { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding { get; private set; } = default!;
    public virtual Rect buttonRect { get; private set; } = default!;
    public virtual long selectedIndex { get; private set; } = default!;
    public virtual long elevation { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.CapturedThemes capturedThemes { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? style { get; private set; }
    public virtual Color? dropdownColor { get; private set; }
    public virtual bool enableFeedback { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius { get; private set; }
    public virtual double? menuWidth { get; private set; }
    public virtual global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor { get; private set; }

    internal _DropdownRoutePage__dropdown(global::Doroti.Generated.Framework.Foundation.Key? key = null, _DropdownRoute__dropdown<T> route = default!, global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints = default!, List<_MenuItem__dropdown<T>>? items = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding = default!, Rect buttonRect = default!, long selectedIndex = default!, long elevation = 8, global::Doroti.Generated.Framework.Widgets.CapturedThemes capturedThemes = default!, global::Doroti.Generated.Framework.Painting.TextStyle? style = null, Color? dropdownColor = default!, bool enableFeedback = default!, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null, double? menuWidth = null, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null) : base(key: key)
    {
        this.route = route;
        this.constraints = constraints;
        this.items = items;
        this.padding = padding;
        this.buttonRect = buttonRect;
        this.selectedIndex = selectedIndex;
        this.elevation = elevation;
        this.capturedThemes = capturedThemes;
        this.style = style;
        this.dropdownColor = dropdownColor;
        this.enableFeedback = enableFeedback;
        this.borderRadius = borderRadius;
        this.menuWidth = menuWidth;
        this.mouseCursor = mouseCursor;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DropdownRoutePageState__dropdown<T>());
}

internal class _DropdownRoutePageState__dropdown<T> : global::Doroti.Generated.Framework.Widgets.State<_DropdownRoutePage__dropdown<T>>
{
    internal virtual global::Doroti.Generated.Framework.Widgets.ScrollController _scrollController { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        _MenuLimits__dropdown menuLimits__24511 = ((_MenuLimits__dropdown)(object?)((_DropdownRoutePage__dropdown<T>)(object)this.widget).route.getMenuLimits(((_DropdownRoutePage__dropdown<T>)(object)this.widget).buttonRect, ((_DropdownRoutePage__dropdown<T>)(object)this.widget).constraints.maxHeight, ((_DropdownRoutePage__dropdown<T>)(object)this.widget).selectedIndex));
        _scrollController = new global::Doroti.Generated.Framework.Widgets.ScrollController(initialScrollOffset: ((_MenuLimits__dropdown)menuLimits__24511).scrollOffset);
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        global::Doroti.Ui.TextDirection? textDirection__24868 = Directionality.maybeOf(context);
        global::Doroti.Generated.Framework.Widgets.Widget menu__24934 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _DropdownMenu__dropdown<T>(route: ((_DropdownRoutePage__dropdown<T>)(object)this.widget).route, padding: ((_DropdownRoutePage__dropdown<T>)(object)this.widget).padding.resolve(textDirection__24868), buttonRect: ((_DropdownRoutePage__dropdown<T>)(object)this.widget).buttonRect, constraints: ((_DropdownRoutePage__dropdown<T>)(object)this.widget).constraints, dropdownColor: ((_DropdownRoutePage__dropdown<T>)(object)this.widget).dropdownColor, enableFeedback: ((_DropdownRoutePage__dropdown<T>)(object)this.widget).enableFeedback, borderRadius: ((_DropdownRoutePage__dropdown<T>)(object)this.widget).borderRadius, scrollController: this._scrollController, mouseCursor: ((_DropdownRoutePage__dropdown<T>)(object)this.widget).mouseCursor));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)global::Doroti.Generated.Framework.Widgets.MediaQuery.CreateRemovePadding(context: context, removeTop: true, removeBottom: true, removeLeft: true, removeRight: true, child: new global::Doroti.Generated.Framework.Widgets.Builder(builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>)((context) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.CustomSingleChildLayout(@delegate: new _DropdownMenuRouteLayout__dropdown<T>(buttonRect: ((_DropdownRoutePage__dropdown<T>)(object)this.widget).buttonRect, route: ((_DropdownRoutePage__dropdown<T>)(object)this.widget).route, textDirection: textDirection__24868, menuWidth: ((_DropdownRoutePage__dropdown<T>)(object)this.widget).menuWidth), child: ((_DropdownRoutePage__dropdown<T>)(object)this.widget).capturedThemes.wrap(menu__24934)));
throw new InvalidOperationException("Dart closure completed without a value.");
})))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        this._scrollController.dispose();
        base.dispose();
    }

}

public class _MenuItem__dropdown<T> : global::Doroti.Generated.Framework.Widgets.SingleChildRenderObjectWidget
{
    public virtual global::System.Action<Size> onLayout { get; private set; } = default!;
    public virtual DropdownMenuItem<T>? item { get; private set; }

    internal _MenuItem__dropdown(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Action<Size> onLayout = default!, DropdownMenuItem<T>? item = default!) : base(key: key, child: item)
    {
        this.onLayout = onLayout;
        this.item = item;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderMenuItem__dropdown((global::System.Action<Size>)this.onLayout));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderMenuItem__dropdown)(object)renderObject;
        __renderObject.onLayout = (global::System.Action<Size>)this.onLayout;
    }

}

public class _RenderMenuItem__dropdown : global::Doroti.Generated.Framework.Rendering.RenderProxyBox
{
    public virtual global::System.Action<Size> onLayout { get; set; } = default!;

    internal _RenderMenuItem__dropdown(global::System.Action<Size> onLayout, global::Doroti.Generated.Framework.Rendering.RenderBox? child = null) : base(child)
    {
        this.onLayout = onLayout;
    }

    public virtual void performLayout()
    {
        base.performLayout();
        this.onLayout(this.size);
    }

}

public class _DropdownMenuItemContainer__dropdown : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;

    internal _DropdownMenuItemContainer__dropdown(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key)
    {
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerStart;
        this.alignment = __alignment;
        this.child = child;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Semantics(button: true, child: new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Generated.Framework.Rendering.BoxConstraints(minHeight: DropdownLibrary._kMenuItemHeight), child: new global::Doroti.Generated.Framework.Widgets.Align(alignment: this.alignment, child: this.child))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DropdownMenuItem<T> : _DropdownMenuItemContainer__dropdown
{
    public virtual global::System.Action? onTap { get; private set; }
    public virtual T? value { get; private set; }
    public virtual bool enabled { get; private set; } = default!;

    public DropdownMenuItem(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Action? onTap = null, T? value = default, bool enabled = true, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key, alignment: alignment ?? global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerStart, child: child)
    {
        this.onTap = onTap;
        this.value = value;
        this.enabled = enabled;
    }

}

public class DropdownButtonHideUnderline : global::Doroti.Generated.Framework.Widgets.InheritedWidget
{
    public DropdownButtonHideUnderline(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
    }

    public static bool at(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return (context.dependOnInheritedWidgetOfExactType<DropdownButtonHideUnderline>() is not null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => false;
}

public class DropdownButton<T> : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual List<DropdownMenuItem<T>>? items { get; private set; }
    public virtual T? value { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? hint { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? disabledHint { get; private set; }
    public virtual global::System.Action<T?>? onChanged { get; private set; }
    public virtual global::System.Action? onTap { get; private set; }
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, List<global::Doroti.Generated.Framework.Widgets.Widget>>? selectedItemBuilder { get; private set; }
    public virtual long elevation { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? style { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? underline { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? icon { get; private set; }
    public virtual Color? iconDisabledColor { get; private set; }
    public virtual Color? iconEnabledColor { get; private set; }
    public virtual double iconSize { get; private set; } = default!;
    public virtual bool isDense { get; private set; } = default!;
    public virtual bool isExpanded { get; private set; } = default!;
    public virtual double? itemHeight { get; private set; }
    public virtual double? menuWidth { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual Color? dropdownColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual double? menuMaxHeight { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius { get; private set; }
    public virtual bool barrierDismissible { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Services.MouseCursor? dropdownMenuItemMouseCursor { get; private set; }
    internal virtual InputDecoration? _inputDecoration { get; private set; }
    internal virtual bool _isEmpty { get; private set; } = default!;

    public DropdownButton(global::Doroti.Generated.Framework.Foundation.Key? key = null, List<DropdownMenuItem<T>>? items = default!, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, List<global::Doroti.Generated.Framework.Widgets.Widget>>? selectedItemBuilder = null, T? value = default, global::Doroti.Generated.Framework.Widgets.Widget? hint = null, global::Doroti.Generated.Framework.Widgets.Widget? disabledHint = null, global::System.Action<T?>? onChanged = default!, global::System.Action? onTap = null, long elevation = 8, global::Doroti.Generated.Framework.Painting.TextStyle? style = null, global::Doroti.Generated.Framework.Widgets.Widget? underline = null, global::Doroti.Generated.Framework.Widgets.Widget? icon = null, Color? iconDisabledColor = null, Color? iconEnabledColor = null, double iconSize = 24.0, bool isDense = false, bool isExpanded = false, double? itemHeight = null, double? menuWidth = null, Color? focusColor = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, Color? dropdownColor = null, double? menuMaxHeight = null, bool? enableFeedback = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, bool barrierDismissible = true, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Generated.Framework.Services.MouseCursor? dropdownMenuItemMouseCursor = null) : base(key: key)
    {
        double? __itemHeight = itemHeight ?? ConstantsLibrary.kMinInteractiveDimension;
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerStart;
        this.items = items;
        this.selectedItemBuilder = selectedItemBuilder;
        this.value = value;
        this.hint = hint;
        this.disabledHint = disabledHint;
        this.onChanged = onChanged;
        this.onTap = onTap;
        this.elevation = elevation;
        this.style = style;
        this.underline = underline;
        this.icon = icon;
        this.iconDisabledColor = iconDisabledColor;
        this.iconEnabledColor = iconEnabledColor;
        this.iconSize = iconSize;
        this.isDense = isDense;
        this.isExpanded = isExpanded;
        this.itemHeight = __itemHeight;
        this.menuWidth = menuWidth;
        this.focusColor = focusColor;
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.dropdownColor = dropdownColor;
        this.menuMaxHeight = menuMaxHeight;
        this.enableFeedback = enableFeedback;
        this.alignment = __alignment;
        this.borderRadius = borderRadius;
        this.padding = padding;
        this.barrierDismissible = barrierDismissible;
        this.mouseCursor = mouseCursor;
        this.dropdownMenuItemMouseCursor = dropdownMenuItemMouseCursor;
        this._inputDecoration = null;
        this._isEmpty = false;
        System.Diagnostics.Debug.Assert(((((items is null) || !System.Linq.Enumerable.Any(items)) || (value is null)) || (items.where(((item) => {
return object.Equals(((DropdownMenuItem<T>)item).value, value);
throw new InvalidOperationException("Dart closure completed without a value.");
})).Count() == 1L)));
        System.Diagnostics.Debug.Assert(((__itemHeight is null) || (__itemHeight >= ConstantsLibrary.kMinInteractiveDimension)));
    }

    public static DropdownButton<T> Create_formField(global::Doroti.Generated.Framework.Foundation.Key? key = null, List<DropdownMenuItem<T>>? items = default!, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, List<global::Doroti.Generated.Framework.Widgets.Widget>>? selectedItemBuilder = null, T? value = default, global::Doroti.Generated.Framework.Widgets.Widget? hint = null, global::Doroti.Generated.Framework.Widgets.Widget? disabledHint = null, global::System.Action<T?>? onChanged = default!, global::System.Action? onTap = null, long elevation = 8, global::Doroti.Generated.Framework.Painting.TextStyle? style = null, global::Doroti.Generated.Framework.Widgets.Widget? underline = null, global::Doroti.Generated.Framework.Widgets.Widget? icon = null, Color? iconDisabledColor = null, Color? iconEnabledColor = null, double iconSize = 24.0, bool isDense = false, bool isExpanded = false, double? itemHeight = null, double? menuWidth = null, Color? focusColor = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, Color? dropdownColor = null, double? menuMaxHeight = null, bool? enableFeedback = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, bool barrierDismissible = true, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Generated.Framework.Services.MouseCursor? dropdownMenuItemMouseCursor = null, InputDecoration inputDecoration = default!, bool isEmpty = default!)
    {
        var __instance = new DropdownButton<T>(key: key, items: items, selectedItemBuilder: selectedItemBuilder, value: value, hint: hint, disabledHint: disabledHint, onChanged: onChanged, onTap: onTap, elevation: elevation, style: style, underline: underline, icon: icon, iconDisabledColor: iconDisabledColor, iconEnabledColor: iconEnabledColor, iconSize: iconSize, isDense: isDense, isExpanded: isExpanded, itemHeight: itemHeight, menuWidth: menuWidth, focusColor: focusColor, focusNode: focusNode, autofocus: autofocus, dropdownColor: dropdownColor, menuMaxHeight: menuMaxHeight, enableFeedback: enableFeedback, alignment: alignment, borderRadius: borderRadius, padding: padding, barrierDismissible: barrierDismissible, mouseCursor: mouseCursor, dropdownMenuItemMouseCursor: dropdownMenuItemMouseCursor);
        double? __itemHeight = itemHeight ?? ConstantsLibrary.kMinInteractiveDimension;
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerStart;
        __instance.items = items;
        __instance.selectedItemBuilder = selectedItemBuilder;
        __instance.value = value;
        __instance.hint = hint;
        __instance.disabledHint = disabledHint;
        __instance.onChanged = onChanged;
        __instance.onTap = onTap;
        __instance.elevation = elevation;
        __instance.style = style;
        __instance.underline = underline;
        __instance.icon = icon;
        __instance.iconDisabledColor = iconDisabledColor;
        __instance.iconEnabledColor = iconEnabledColor;
        __instance.iconSize = iconSize;
        __instance.isDense = isDense;
        __instance.isExpanded = isExpanded;
        __instance.itemHeight = __itemHeight;
        __instance.menuWidth = menuWidth;
        __instance.focusColor = focusColor;
        __instance.focusNode = focusNode;
        __instance.autofocus = autofocus;
        __instance.dropdownColor = dropdownColor;
        __instance.menuMaxHeight = menuMaxHeight;
        __instance.enableFeedback = enableFeedback;
        __instance.alignment = __alignment;
        __instance.borderRadius = borderRadius;
        __instance.padding = padding;
        __instance.barrierDismissible = barrierDismissible;
        __instance.mouseCursor = mouseCursor;
        __instance.dropdownMenuItemMouseCursor = dropdownMenuItemMouseCursor;
        __instance._inputDecoration = inputDecoration;
        __instance._isEmpty = isEmpty;
        return __instance;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DropdownButtonState__dropdown<T>());
}

internal class _DropdownButtonState__dropdown<T> : global::Doroti.Generated.Framework.Widgets.State<DropdownButton<T>>, global::Doroti.Generated.Framework.Widgets.WidgetsBindingObserver
{
    internal virtual long? _selectedIndex { get; set; } = default;
    internal virtual _DropdownRoute__dropdown<T>? _dropdownRoute { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Widgets.Orientation? _lastOrientation { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Widgets.FocusNode? _internalNode { get; set; } = default;
    internal virtual DartMap<Type, dynamic> _actionMap { get; set; } = default!;
    internal virtual bool _isHovering { get; set; } = false;
    internal virtual bool _hasPrimaryFocus { get; set; } = false;
    internal virtual bool _isMenuExpanded { get; set; } = false;

    public virtual global::Doroti.Generated.Framework.Widgets.FocusNode focusNode => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.FocusNode>((((DropdownButton<T>)(object)this.widget).focusNode ?? this._internalNode!));
    internal virtual global::Doroti.Generated.Framework.Widgets.FocusNode _createFocusNode()
    {
        return new global::Doroti.Generated.Framework.Widgets.FocusNode(debugLabel: $"{DartRuntimePrimitives.RuntimeType(this.widget)}");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void initState()
    {
        base.initState();
        _updateSelectedIndex();
        if ((((DropdownButton<T>)(object)this.widget).focusNode is null))
        {
            _internalNode ??= _createFocusNode();
        }
        _actionMap = new DartMap<Type, dynamic> { [typeof(global::Doroti.Generated.Framework.Widgets.ActivateIntent)] = new global::Doroti.Generated.Framework.Widgets.CallbackAction<global::Doroti.Generated.Framework.Widgets.ActivateIntent>(onInvoke: ((global::System.Action<global::Doroti.Generated.Framework.Widgets.ActivateIntent>)((intent) => { _handleTap(); }))), [typeof(global::Doroti.Generated.Framework.Widgets.ButtonActivateIntent)] = new global::Doroti.Generated.Framework.Widgets.CallbackAction<global::Doroti.Generated.Framework.Widgets.ButtonActivateIntent>(onInvoke: ((global::System.Action<global::Doroti.Generated.Framework.Widgets.ButtonActivateIntent>)((intent) => { _handleTap(); }))) };
        this.focusNode.addListener(() => this._handleFocusChanged());
    }

    public override void dispose()
    {
        global::Doroti.Generated.Framework.Widgets.WidgetsBinding.instance.removeObserver(this);
        _removeDropdownRoute();
        this.focusNode.removeListener(() => this._handleFocusChanged());
        this._internalNode?.dispose();
        base.dispose();
    }

    internal virtual void _handleFocusChanged()
    {
        if ((this._hasPrimaryFocus != ((global::Doroti.Generated.Framework.Widgets.FocusNode)this.focusNode).hasPrimaryFocus))
        {
            setState(((global::System.Action)(() => {
_hasPrimaryFocus = ((global::Doroti.Generated.Framework.Widgets.FocusNode)this.focusNode).hasPrimaryFocus;
})));
        }
    }

    internal virtual void _removeDropdownRoute()
    {
        this._dropdownRoute?._dismiss();
        _dropdownRoute = null;
        _lastOrientation = null;
    }

    public override void didUpdateWidget(DropdownButton<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((DropdownButton<T>)(object)this.widget).focusNode, ((DropdownButton<T>)oldWidget).focusNode)))
        {
            ((DropdownButton<T>)oldWidget).focusNode?.removeListener(() => this._handleFocusChanged());
            if (((this._internalNode is not null) && (((DropdownButton<T>)(object)this.widget).focusNode is not null)))
            {
                this._internalNode!.dispose();
                _internalNode = null;
            }
            if ((((DropdownButton<T>)(object)this.widget).focusNode is null))
            {
                _internalNode ??= _createFocusNode();
            }
            _hasPrimaryFocus = ((global::Doroti.Generated.Framework.Widgets.FocusNode)this.focusNode).hasPrimaryFocus;
            this.focusNode.addListener(() => this._handleFocusChanged());
        }
        _updateSelectedIndex();
    }

    internal virtual void _updateSelectedIndex()
    {
        if ((((((DropdownButton<T>)(object)this.widget).items is null) || !System.Linq.Enumerable.Any(((DropdownButton<T>)(object)this.widget).items!)) || (((((DropdownButton<T>)(object)this.widget).value is null) && !System.Linq.Enumerable.Any(((DropdownButton<T>)(object)this.widget).items!.where(((item) => (((DropdownMenuItem<T>)((DropdownMenuItem<T>)item)).enabled && EqualityComparer<T>.Default.Equals(((DropdownMenuItem<T>)((DropdownMenuItem<T>)item)).value, ((DropdownButton<T>)(object)this.widget).value)))))))))
        {
            _selectedIndex = null;
            return;
        }
        DartRuntimePrimitives.Assert(() => (((DropdownButton<T>)(object)this.widget).items!.where(((item) => EqualityComparer<T>.Default.Equals(((DropdownMenuItem<T>)((DropdownMenuItem<T>)item)).value, ((DropdownButton<T>)(object)this.widget).value))).Count() == 1L));
        for (var itemIndex__50486 = 0L; (itemIndex__50486 < checked((long)(((DropdownButton<T>)(object)this.widget).items!.Count))); itemIndex__50486++)
        {
            if (EqualityComparer<T>.Default.Equals(((DropdownButton<T>)(object)this.widget).items![(int)(itemIndex__50486)].value, ((DropdownButton<T>)(object)this.widget).value))
            {
                _selectedIndex = itemIndex__50486;
                return;
            }
        }
    }

    internal virtual global::Doroti.Generated.Framework.Painting.TextStyle? _textStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.TextStyle>((((DropdownButton<T>)(object)this.widget).style ?? Theme.of(this.context).textTheme.titleMedium));
    internal virtual void _handleTap()
    {
        global::Doroti.Ui.TextDirection? textDirection__50817 = Directionality.maybeOf(this.context);
        global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry menuMargin__50895 = (ButtonTheme.of(this.context).alignedDropdown ? DropdownLibrary._kAlignedMenuMargin : DropdownLibrary._kUnalignedMenuMargin);
        var menuItems__51022 = ((Func<List<_MenuItem__dropdown<T>>>)(() => { var __collection51034 = new List<_MenuItem__dropdown<T>>(); for (long index__51065 = 0L; (index__51065 < checked((long)(((DropdownButton<T>)(object)this.widget).items!.Count))); index__51065 += 1L) { __collection51034.Add(new _MenuItem__dropdown<T>(item: ((DropdownButton<T>)(object)this.widget).items![(int)(index__51065)], onLayout: ((global::System.Action<Size>)((size) => {
if ((this._dropdownRoute is null))
{
    return;
}
this._dropdownRoute!.itemHeights[(int)(index__51065)] = size.height;
})))); } return __collection51034; }))();
        global::Doroti.Generated.Framework.Widgets.NavigatorState navigator__51933 = ((global::Doroti.Generated.Framework.Widgets.NavigatorState)(object?)Navigator.of(this.context));
        DartRuntimePrimitives.Assert(() => (this._dropdownRoute is null));
        var itemBox__52014 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)this.context.findRenderObject()!)!;
        global::Doroti.Ui.Rect itemRect__52081 = ((global::Doroti.Ui.Rect)(object?)(((Offset)((dynamic)itemBox__52014).localToGlobal(Offset.zero, ancestor: navigator__51933.context.findRenderObject())) & ((global::Doroti.Generated.Framework.Rendering.RenderBox)itemBox__52014).size));
        _dropdownRoute = new _DropdownRoute__dropdown<T>(items: menuItems__51022, buttonRect: menuMargin__50895.resolve(textDirection__50817).inflateRect(itemRect__52081), padding: DropdownLibrary._kMenuItemPadding.resolve(textDirection__50817), selectedIndex: (this._selectedIndex ?? 0L), elevation: ((DropdownButton<T>)(object)this.widget).elevation, capturedThemes: InheritedTheme.capture(from: this.context, to: navigator__51933.context), style: this._textStyle!, barrierLabel: MaterialLocalizations.of(this.context).modalBarrierDismissLabel, itemHeight: ((DropdownButton<T>)(object)this.widget).itemHeight, menuWidth: ((DropdownButton<T>)(object)this.widget).menuWidth, dropdownColor: ((DropdownButton<T>)(object)this.widget).dropdownColor, menuMaxHeight: ((DropdownButton<T>)(object)this.widget).menuMaxHeight, enableFeedback: (((DropdownButton<T>)(object)this.widget).enableFeedback ?? true), borderRadius: ((DropdownButton<T>)(object)this.widget).borderRadius, barrierDismissible: ((DropdownButton<T>)(object)this.widget).barrierDismissible, dropdownMenuItemMouseCursor: ((DropdownButton<T>)(object)this.widget).dropdownMenuItemMouseCursor);
        this.focusNode.requestFocus();
        DartRuntimePrimitives.Ignore(navigator__51933.push(this._dropdownRoute!).then((global::System.Action<_DropdownRouteResult__dropdown<T>?>)((newValue) => {
_removeDropdownRoute();
if (this.mounted)
{
    setState(((global::System.Action)(() => {
_isMenuExpanded = false;
})));
}
if ((!this.mounted || (newValue is null)))
{
    return;
}
((DropdownButton<T>)(object)this.widget).onChanged?.Invoke(((_DropdownRouteResult__dropdown<T>)newValue).result);
})));
        ((DropdownButton<T>)(object)this.widget).onTap?.Invoke();
        setState(((global::System.Action)(() => {
_isMenuExpanded = true;
})));
    }

    internal virtual double _denseButtonHeight
    {
        get
        {
            double fontSize__53828 = (this._textStyle!.fontSize ?? DartRuntimePrimitives.RequireValue(Theme.of(this.context).textTheme.titleMedium!.fontSize));
            double lineHeight__53940 = ((this._textStyle!.height ?? Theme.of(this.context).textTheme.titleMedium!.height) ?? 1.0);
            double scaledFontSize__54056 = MediaQuery.textScalerOf(this.context).scale((fontSize__53828 * lineHeight__53940));
            return Math.Max(scaledFontSize__54056, Math.Max(((DropdownButton<T>)(object)this.widget).iconSize, DropdownLibrary._kDenseButtonHeight));
            return default!;
        }
    }
    internal virtual global::Doroti.Ui.Color _iconColor
    {
        get
        {
            global::Doroti.Ui.Brightness brightness__54337 = Theme.brightnessOf(this.context);
            if (this._enabled)
            {
                return (((DropdownButton<T>)(object)this.widget).iconEnabledColor ?? (brightness__54337 switch { Brightness.light => Colors.grey.shade700, Brightness.dark => Colors.white70, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
            }
            else
            {
                return (((DropdownButton<T>)(object)this.widget).iconDisabledColor ?? (brightness__54337 switch { Brightness.light => Colors.grey.shade400, Brightness.dark => Colors.white10, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
            }
            return default!;
        }
    }
    internal virtual bool _enabled => DartRuntimePrimitives.ConvertValue<bool>((((((DropdownButton<T>)(object)this.widget).items is not null) && System.Linq.Enumerable.Any(((DropdownButton<T>)(object)this.widget).items!)) && (((DropdownButton<T>)(object)this.widget).onChanged is not null)));
    internal virtual global::Doroti.Generated.Framework.Widgets.Orientation _getOrientation(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Generated.Framework.Widgets.Orientation? result__54969 = MediaQuery.maybeOrientationOf(context);
        if ((result__54969 is null))
        {
            global::Doroti.Ui.Size size__55156 = ((global::Doroti.Ui.Size)(object?)View.of(context).physicalSize);
            result__54969 = ((size__55156.width > size__55156.height) ? global::Doroti.Generated.Framework.Widgets.Orientation.landscape : global::Doroti.Generated.Framework.Widgets.Orientation.portrait);
        }
        return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(result__54969));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        global::Doroti.Generated.Framework.Widgets.Orientation newOrientation__55486 = _getOrientation(context);
        _lastOrientation ??= newOrientation__55486;
        if ((!object.Equals(newOrientation__55486, this._lastOrientation)))
        {
            _removeDropdownRoute();
            _lastOrientation = newOrientation__55486;
        }
        List<global::Doroti.Generated.Framework.Widgets.Widget> items__56033 = default!;
        if ((((DropdownButton<T>)(object)this.widget).selectedItemBuilder is not null))
        {
            List<global::Doroti.Generated.Framework.Widgets.Widget> selectedItems__56111 = ((DropdownButton<T>)(object)this.widget).selectedItemBuilder!(context).ToList();
            DartRuntimePrimitives.Assert(() => ((((DropdownButton<T>)(object)this.widget).items is null) || (checked((long)(selectedItems__56111.Count)) == checked((long)(((DropdownButton<T>)(object)this.widget).items!.Count)))), () => (object?)"The selectedItemBuilder must return a list of widgets with the same length as the items list.\n" + $"Currently, selectedItemBuilder returns a list of length {checked((long)(selectedItems__56111.Count))}, " + $"but items has length {checked((long)(((DropdownButton<T>)(object)this.widget).items!.Count))}.");
            items__56033 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(DartRuntimePrimitives.ConvertEnumerable<global::Doroti.Generated.Framework.Widgets.Widget>(selectedItems__56111));
        }
        else
        {
            items__56033 = ((((DropdownButton<T>)(object)this.widget).items is not null) ? new List<global::Doroti.Generated.Framework.Widgets.Widget>(DartRuntimePrimitives.ConvertEnumerable<global::Doroti.Generated.Framework.Widgets.Widget>(((DropdownButton<T>)(object)this.widget).items!)) : new List<global::Doroti.Generated.Framework.Widgets.Widget>());
        }
        long? hintIndex__56678 = default!;
        if (((((DropdownButton<T>)(object)this.widget).hint is not null) || ((!this._enabled && (((DropdownButton<T>)(object)this.widget).disabledHint is not null)))))
        {
            global::Doroti.Generated.Framework.Widgets.Widget displayedHint__56785 = (this._enabled ? ((DropdownButton<T>)(object)this.widget).hint! : (((DropdownButton<T>)(object)this.widget).disabledHint ?? ((DropdownButton<T>)(object)this.widget).hint!));
            hintIndex__56678 = checked((long)(items__56033.Count));
            items__56033.Add(new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: this._textStyle!.copyWith(color: Theme.of(context).hintColor), child: new global::Doroti.Generated.Framework.Widgets.IgnorePointer(child: new _DropdownMenuItemContainer__dropdown(alignment: ((DropdownButton<T>)(object)this.widget).alignment, child: displayedHint__56785))));
        }
        global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding__57214 = ((ButtonTheme.of(context).alignedDropdown && (((DropdownButton<T>)(object)this.widget)._inputDecoration is null)) ? DropdownLibrary._kAlignedButtonPadding : DropdownLibrary._kUnalignedButtonPadding);
        global::Doroti.Generated.Framework.Widgets.Widget innerItemsWidget__57499 = default!;
        if (!System.Linq.Enumerable.Any(items__56033))
        {
            innerItemsWidget__57499 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(global::Doroti.Generated.Framework.Widgets.SizedBox.CreateShrink());
        }
        else
        {
            innerItemsWidget__57499 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.IndexedStack(index: (this._selectedIndex ?? hintIndex__56678), alignment: ((DropdownButton<T>)(object)this.widget).alignment, children: (((DropdownButton<T>)(object)this.widget).isDense ? items__56033 : items__56033.map<global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Widgets.RenderObjectWidget>(((item) => {
return ((((DropdownButton<T>)(object)this.widget).itemHeight is not null) ? new global::Doroti.Generated.Framework.Widgets.SizedBox(height: ((DropdownButton<T>)(object)this.widget).itemHeight, child: item) : new global::Doroti.Generated.Framework.Widgets.Column(mainAxisSize: global::Doroti.Generated.Framework.Rendering.MainAxisSize.min, children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(item) }));
throw new InvalidOperationException("Dart closure completed without a value.");
})).Cast<global::Doroti.Generated.Framework.Widgets.Widget>().ToList())));
        }
        var defaultIcon__58079 = new global::Doroti.Generated.Framework.Widgets.Icon(Icons.arrow_drop_down);
        global::Doroti.Generated.Framework.Widgets.Widget effectiveSuffixIcon__58139 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.IconTheme(data: new global::Doroti.Generated.Framework.Widgets.IconThemeData(color: this._iconColor, size: ((DropdownButton<T>)(object)this.widget).iconSize), child: ((((DropdownButton<T>)(object)this.widget).icon ?? ((DropdownButton<T>)(object)this.widget)._inputDecoration?.suffixIcon) ?? defaultIcon__58079)));
        global::Doroti.Generated.Framework.Widgets.Widget result__58340 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: (this._enabled ? this._textStyle! : this._textStyle!.copyWith(color: Theme.of(context).disabledColor)), child: new global::Doroti.Generated.Framework.Widgets.SizedBox(height: (((DropdownButton<T>)(object)this.widget).isDense ? this._denseButtonHeight : null), child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: padding__57214.resolve(Directionality.of(context)), child: new global::Doroti.Generated.Framework.Widgets.Row(mainAxisAlignment: global::Doroti.Generated.Framework.Rendering.MainAxisAlignment.spaceBetween, mainAxisSize: global::Doroti.Generated.Framework.Rendering.MainAxisSize.min, children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection58789 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); if (((DropdownButton<T>)(object)this.widget).isExpanded) { __collection58789.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Expanded(child: innerItemsWidget__57499))); } else { __collection58789.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(innerItemsWidget__57499)); } if ((((DropdownButton<T>)(object)this.widget)._inputDecoration is null)) { __collection58789.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(effectiveSuffixIcon__58139)); } return __collection58789; }))())))));
        if (!DropdownButtonHideUnderline.at(context))
        {
            var bottom__59085 = (((((DropdownButton<T>)(object)this.widget).isDense || (((DropdownButton<T>)(object)this.widget).itemHeight is null))) ? 0.0 : 8.0);
            result__58340 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Stack(children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(result__58340), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Positioned(left: 0.0, right: 0.0, bottom: bottom__59085, child: (((DropdownButton<T>)(object)this.widget).underline ?? new global::Doroti.Generated.Framework.Widgets.Container(height: 1.0, decoration: new global::Doroti.Generated.Framework.Painting.BoxDecoration(border: new global::Doroti.Generated.Framework.Painting.Border(bottom: new global::Doroti.Generated.Framework.Painting.BorderSide(color: new global::Doroti.Ui.Color(4290624957L), width: 0.0))))))) }));
        }
        global::Doroti.Generated.Framework.Services.MouseCursor effectiveMouseCursor__59678 = ((global::Doroti.Generated.Framework.Services.MouseCursor)(object?)WidgetStateProperty.resolveAs<global::Doroti.Generated.Framework.Services.MouseCursor>((((DropdownButton<T>)(object)this.widget).mouseCursor ?? global::Doroti.Generated.Framework.Widgets.WidgetStateMouseCursor.adaptiveClickable), ((Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>>)(() => { var __collection59821 = new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>(); if (!this._enabled) { __collection59821.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled); } return __collection59821; }))()));
        if ((((DropdownButton<T>)(object)this.widget)._inputDecoration is not null))
        {
            bool filled__60494 = (((DropdownButton<T>)(object)this.widget)._inputDecoration?.filled ?? InputDecorationTheme.of(context).filled);
            bool oulined__60606 = ((((DropdownButton<T>)(object)this.widget)._inputDecoration?.border?.isOutline ?? InputDecorationTheme.of(context).border?.isOutline) ?? false);
            var suffixIconEndMargin__60766 = (((filled__60494 || oulined__60606)) ? 12.0 : 0.0);
            InputDecoration effectiveDecoration__60844 = ((InputDecoration)(object?)((DropdownButton<T>)(object)this.widget)._inputDecoration!.copyWith(suffixIconConstraints: new global::Doroti.Generated.Framework.Rendering.BoxConstraints(minWidth: (((DropdownButton<T>)(object)this.widget).iconSize + suffixIconEndMargin__60766), minHeight: ((DropdownButton<T>)(object)this.widget).iconSize), suffixIcon: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry.CreateDirectional(end: suffixIconEndMargin__60766), child: effectiveSuffixIcon__58139)));
            if (this._hasPrimaryFocus)
            {
                global::Doroti.Ui.Color? focusColor__61427 = ((global::Doroti.Ui.Color?)(object?)(((DropdownButton<T>)(object)this.widget).focusColor ?? ((InputDecoration)effectiveDecoration__60844).focusColor));
                if ((focusColor__61427 is not null))
                {
                    effectiveDecoration__60844 = effectiveDecoration__60844.copyWith(fillColor: focusColor__61427);
                }
            }
            result__58340 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Focus(canRequestFocus: this._enabled, focusNode: this.focusNode, autofocus: ((DropdownButton<T>)(object)this.widget).autofocus, child: new global::Doroti.Generated.Framework.Widgets.MouseRegion(onEnter: ((global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerEnterEvent>)((@event) => {
if (!this._isHovering)
{
    setState(((global::System.Action)(() => {
_isHovering = true;
})));
}
})), onExit: ((global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerExitEvent>)((@event) => {
if (this._isHovering)
{
    setState(((global::System.Action)(() => {
_isHovering = false;
})));
}
})), cursor: effectiveMouseCursor__59678, child: new global::Doroti.Generated.Framework.Widgets.GestureDetector(onTap: ((global::System.Action)(this._enabled ? this._handleTap : null)), behavior: global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque, child: new InputDecorator(decoration: effectiveDecoration__60844, isEmpty: ((DropdownButton<T>)(object)this.widget)._isEmpty, isFocused: this._hasPrimaryFocus, isHovering: this._isHovering, child: ((((DropdownButton<T>)(object)this.widget).padding is null) ? result__58340 : new global::Doroti.Generated.Framework.Widgets.Padding(padding: ((DropdownButton<T>)(object)this.widget).padding!, child: result__58340)))))));
        }
        else
        {
            result__58340 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new InkWell(mouseCursor: effectiveMouseCursor__59678, onTap: ((global::System.Action)(this._enabled ? this._handleTap : null)), canRequestFocus: this._enabled, borderRadius: ((DropdownButton<T>)(object)this.widget).borderRadius, focusNode: this.focusNode, autofocus: ((DropdownButton<T>)(object)this.widget).autofocus, focusColor: (((DropdownButton<T>)(object)this.widget).focusColor ?? Theme.of(context).focusColor), enableFeedback: false, child: ((((DropdownButton<T>)(object)this.widget).padding is null) ? result__58340 : new global::Doroti.Generated.Framework.Widgets.Padding(padding: ((DropdownButton<T>)(object)this.widget).padding!, child: result__58340))));
        }
        bool childHasButtonSemantic__63298 = ((hintIndex__56678 is not null) || (((this._selectedIndex is not null) && (((DropdownButton<T>)(object)this.widget).selectedItemBuilder is null))));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Semantics(button: !childHasButtonSemantic__63298, expanded: this._isMenuExpanded, child: new global::Doroti.Generated.Framework.Widgets.Actions(actions: this._actionMap, child: result__58340)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DropdownButtonFormField<T> : global::Doroti.Generated.Framework.Widgets.FormField<T>
{
    public virtual global::System.Action<T?>? onChanged { get; private set; }
    public virtual InputDecoration decoration { get; private set; } = default!;
    public virtual bool barrierDismissible { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Services.MouseCursor? dropdownMenuItemMouseCursor { get; private set; }

    public DropdownButtonFormField(global::Doroti.Generated.Framework.Foundation.Key? key = null, List<DropdownMenuItem<T>>? items = default!, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, List<global::Doroti.Generated.Framework.Widgets.Widget>>? selectedItemBuilder = null, T? value = default, T? initialValue = default, global::Doroti.Generated.Framework.Widgets.Widget? hint = null, global::Doroti.Generated.Framework.Widgets.Widget? disabledHint = null, global::System.Action<T?>? onChanged = default!, global::System.Action? onTap = null, long elevation = 8, global::Doroti.Generated.Framework.Painting.TextStyle? style = null, global::Doroti.Generated.Framework.Widgets.Widget? icon = null, Color? iconDisabledColor = null, Color? iconEnabledColor = null, double iconSize = 24.0, bool isDense = true, bool isExpanded = false, double? itemHeight = null, Color? focusColor = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, Color? dropdownColor = null, InputDecoration? decoration = null, global::System.Action<T?>? onSaved = null, global::System.Func<T?, string?>? validator = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, string, global::Doroti.Generated.Framework.Widgets.Widget>? errorBuilder = null, string? forceErrorText = null, global::Doroti.Generated.Framework.Widgets.AutovalidateMode? autovalidateMode = null, double? menuMaxHeight = null, bool? enableFeedback = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, bool barrierDismissible = true, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Generated.Framework.Services.MouseCursor? dropdownMenuItemMouseCursor = null) : base(key: key, onSaved: onSaved, validator: validator, errorBuilder: errorBuilder, forceErrorText: forceErrorText, initialValue: (initialValue ?? value), autovalidateMode: (autovalidateMode ?? global::Doroti.Generated.Framework.Widgets.AutovalidateMode.disabled), builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.FormFieldState<T>, global::Doroti.Generated.Framework.Widgets.Widget>)((field) => {
var state__66866 = ((_DropdownButtonFormFieldState__dropdown<T>?)(object?)field)!;
InputDecoration effectiveDecoration__66944 = ((InputDecoration)(object?)((decoration ?? new InputDecoration())).applyDefaults(InputDecorationTheme.of(((_DropdownButtonFormFieldState__dropdown<T>)field).context)));
bool showSelectedItem__67100 = ((items is not null) && System.Linq.Enumerable.Any(items.where(((item) => EqualityComparer<T>.Default.Equals(((DropdownMenuItem<T>)item).value, state__66866.value)))));
bool isDropdownEnabled__67269 = (((onChanged is not null) && (items is not null)) && System.Linq.Enumerable.Any(items));
global::Doroti.Generated.Framework.Widgets.Widget? decorationHint__67480 = ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)((((InputDecoration)effectiveDecoration__66944).hintText is not null) ? new global::Doroti.Generated.Framework.Widgets.Text(((InputDecoration)effectiveDecoration__66944).hintText!) : null));
global::Doroti.Generated.Framework.Widgets.Widget? effectiveHint__67635 = (hint ?? decorationHint__67480);
global::Doroti.Generated.Framework.Widgets.Widget? effectiveDisabledHint__67700 = (disabledHint ?? effectiveHint__67635);
bool isHintOrDisabledHintAvailable__67777 = (isDropdownEnabled__67269 ? (effectiveHint__67635 is not null) : ((effectiveHint__67635 is not null) || (effectiveDisabledHint__67700 is not null)));
bool isEmpty__67961 = (!showSelectedItem__67100 && !isHintOrDisabledHintAvailable__67777);
if (((((_DropdownButtonFormFieldState__dropdown<T>)field).errorText is not null) || (((InputDecoration)effectiveDecoration__66944).hintText is not null)))
{
    global::Doroti.Generated.Framework.Widgets.Widget? error__68134 = (((((_DropdownButtonFormFieldState__dropdown<T>)field).errorText is not null) && (errorBuilder is not null)) ? errorBuilder(state__66866.context, ((_DropdownButtonFormFieldState__dropdown<T>)field).errorText!) : null);
    string? errorText__68307 = ((error__68134 is null) ? ((_DropdownButtonFormFieldState__dropdown<T>)field).errorText : null);
    string? hintText__68479 = ((((InputDecoration)effectiveDecoration__66944).hintText is not null) ? "" : null);
    effectiveDecoration__66944 = effectiveDecoration__66944.copyWith(error: error__68134, errorText: errorText__68307, hintText: hintText__68479);
}
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Focus(canRequestFocus: false, skipTraversal: true, child: new DropdownButtonHideUnderline(child: DropdownButton<T>.Create_formField(items: items, selectedItemBuilder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, List<global::Doroti.Generated.Framework.Widgets.Widget>>?)selectedItemBuilder, value: state__66866.value, hint: effectiveHint__67635, disabledHint: effectiveDisabledHint__67700, onChanged: ((global::System.Action<T?>)((onChanged is null) ? null : ((_DropdownButtonFormFieldState__dropdown<T>)state__66866).didChange)), onTap: onTap, elevation: elevation, style: style, icon: icon, iconDisabledColor: iconDisabledColor, iconEnabledColor: iconEnabledColor, iconSize: iconSize, isDense: isDense, isExpanded: isExpanded, itemHeight: itemHeight, focusColor: focusColor, focusNode: focusNode, autofocus: autofocus, dropdownColor: dropdownColor, menuMaxHeight: menuMaxHeight, enableFeedback: enableFeedback, alignment: alignment ?? global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerStart, borderRadius: borderRadius, inputDecoration: effectiveDecoration__66944, isEmpty: isEmpty__67961, padding: padding, barrierDismissible: barrierDismissible, mouseCursor: mouseCursor, dropdownMenuItemMouseCursor: dropdownMenuItemMouseCursor))));
throw new InvalidOperationException("Dart closure completed without a value.");
})))
    {
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerStart;
        this.onChanged = onChanged;
        this.barrierDismissible = barrierDismissible;
        this.mouseCursor = mouseCursor;
        this.dropdownMenuItemMouseCursor = dropdownMenuItemMouseCursor;
        this.decoration = (decoration ?? new InputDecoration());
        System.Diagnostics.Debug.Assert(((((items is null) || !System.Linq.Enumerable.Any(items)) || (((initialValue is null) && (value is null)))) || (items.where(((item) => EqualityComparer<T>.Default.Equals(((DropdownMenuItem<T>)item).value, ((initialValue ?? value))))).Count() == 1L)));
        System.Diagnostics.Debug.Assert(((itemHeight is null) || (itemHeight >= ConstantsLibrary.kMinInteractiveDimension)));
        System.Diagnostics.Debug.Assert(((errorBuilder is null) || (decoration?.errorText is null)));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DropdownButtonFormFieldState__dropdown<T>());
}

internal class _DropdownButtonFormFieldState__dropdown<T> : global::Doroti.Generated.Framework.Widgets.FormFieldState<T>
{
    internal virtual DropdownButtonFormField<T> _dropdownButtonFormField => ((DropdownButtonFormField<T>?)(object?)this.widget)!;
    public override void didChange(T? value)
    {
        base.didChange(value);
        ((DropdownButtonFormField<T>)this._dropdownButtonFormField).onChanged?.Invoke(value);
    }

    public override void didUpdateWidget(global::Doroti.Generated.Framework.Widgets.FormField<T> oldWidget)
    {
        var __oldWidget = (DropdownButtonFormField<T>)(object)oldWidget;
        base.didUpdateWidget(__oldWidget);
        if (!EqualityComparer<T>.Default.Equals(__oldWidget.initialValue, ((FormField<T>)(object)this.widget).initialValue))
        {
            setValue(((FormField<T>)(object)this.widget).initialValue);
        }
    }

    public override void reset()
    {
        base.reset();
        ((DropdownButtonFormField<T>)this._dropdownButtonFormField).onChanged?.Invoke(this.value);
    }

}
