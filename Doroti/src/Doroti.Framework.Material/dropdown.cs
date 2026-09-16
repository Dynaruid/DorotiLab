// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/dropdown.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

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
    internal static global::Doroti.Framework.Painting.EdgeInsets _kMenuItemPadding = EdgeInsets.CreateSymmetric(horizontal: 16.0);
}

public static partial class DropdownLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsetsGeometry _kAlignedButtonPadding = EdgeInsetsDirectional.CreateOnly(start: 16.0, end: 4.0);
}

public static partial class DropdownLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsets _kUnalignedButtonPadding = EdgeInsets.zero;
}

public static partial class DropdownLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsets _kAlignedMenuMargin = EdgeInsets.zero;
}

public static partial class DropdownLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsetsGeometry _kUnalignedMenuMargin = EdgeInsetsDirectional.CreateOnly(start: 16.0, end: 24.0);
}

public delegate List<global::Doroti.Framework.Widgets.Widget> DropdownButtonBuilder(global::Doroti.Framework.Widgets.BuildContext context);

internal class _DropdownMenuPainter__dropdown : global::Doroti.Framework.Rendering.CustomPainter
{
    public virtual Color? color { get; private set; }
    public virtual long? elevation { get; private set; }
    public virtual long? selectedIndex { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderRadius? borderRadius { get; private set; }
    public virtual global::Doroti.Framework.Animation.Animation<double> resize { get; private set; } = default!;
    public virtual global::System.Func<double> getSelectedItemOffset { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Painting.BoxPainter _painter { get; private set; } = default!;

    internal _DropdownMenuPainter__dropdown(Color? color = null, long? elevation = null, long? selectedIndex = null, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Framework.Animation.Animation<double> resize = default!, global::System.Func<double> getSelectedItemOffset = default!) : base(repaint: resize)
    {
        this.color = color;
        this.elevation = elevation;
        this.selectedIndex = selectedIndex;
        this.borderRadius = borderRadius;
        this.resize = resize;
        this.getSelectedItemOffset = getSelectedItemOffset;
        _painter = new global::Doroti.Framework.Painting.BoxDecoration(color: color, borderRadius: borderRadius ?? BorderRadius.CreateAll(Radius.circular(2.0)), boxShadow: ShadowsLibrary.kElevationToShadow.GetValueOrDefault(elevation)).createBoxPainter();
    }

    public override void paint(Canvas canvas, Size size)
    {
        double selectedItemOffset = getSelectedItemOffset();
        var top = new global::Doroti.Framework.Animation.Tween<double>(begin: Dart_uiLibrary.clampDouble(selectedItemOffset, 0.0, Math.Max(size.height - DropdownLibrary._kMenuItemHeight, 0.0)), end: 0.0);
        var bottom = new global::Doroti.Framework.Animation.Tween<double>(begin: Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(top.begin) + DropdownLibrary._kMenuItemHeight, Math.Min(DropdownLibrary._kMenuItemHeight, size.height), size.height), end: size.height);
        var rect = Rect.fromLTRB(0.0, top.evaluate(resize), size.width, bottom.evaluate(resize));
        _painter.paint(canvas, rect.topLeft, new global::Doroti.Framework.Painting.ImageConfiguration(size: rect.size));
    }

    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldPainter = (_DropdownMenuPainter__dropdown)oldDelegate;
        return (!Equals(__oldPainter.color, color)) || (__oldPainter.elevation != elevation) || (__oldPainter.selectedIndex != selectedIndex) || (!Equals(__oldPainter.borderRadius, borderRadius)) || (!Equals(__oldPainter.resize, resize));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _DropdownMenuItemButton__dropdown<T> : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual _DropdownRoute__dropdown<T> route { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.ScrollController scrollController { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsets? padding { get; private set; }
    public virtual Rect buttonRect { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.BoxConstraints constraints { get; private set; } = default!;
    public virtual long itemIndex { get; private set; } = default!;
    public virtual bool enableFeedback { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }

    internal _DropdownMenuItemButton__dropdown(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.EdgeInsets? padding = null, _DropdownRoute__dropdown<T> route = default!, Rect buttonRect = default!, global::Doroti.Framework.Rendering.BoxConstraints constraints = default!, long itemIndex = default!, bool enableFeedback = default!, global::Doroti.Framework.Widgets.ScrollController scrollController = default!, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null) : base(key: key)
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

public class _DropdownMenuItemButtonState__dropdown<T> : global::Doroti.Framework.Widgets.State<_DropdownMenuItemButton__dropdown<T>>
{
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _opacityAnimation { get; set; } = default!;
    internal static DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent> _webShortcuts = new DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent> { [new global::Doroti.Framework.Widgets.SingleActivator(LogicalKeyboardKey.arrowDown)] = new global::Doroti.Framework.Widgets.DirectionalFocusIntent(TraversalDirection.down), [new global::Doroti.Framework.Widgets.SingleActivator(LogicalKeyboardKey.arrowUp)] = new global::Doroti.Framework.Widgets.DirectionalFocusIntent(TraversalDirection.up) };

    public override void initState()
    {
        base.initState();
        _setOpacityAnimation();
    }

    public override void didUpdateWidget(_DropdownMenuItemButton__dropdown<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((oldWidget.itemIndex != widget.itemIndex) || (!Equals(oldWidget.route.animation, widget.route.animation)) || (oldWidget.route.selectedIndex != widget.route.selectedIndex) || (checked(widget.route.items.Count) != checked((long)oldWidget.route.items.Count)))
        {
            _opacityAnimation.dispose();
            _setOpacityAnimation();
        }
    }

    internal virtual void _setOpacityAnimation()
    {
        double unit = 0.5 / (checked(widget.route.items.Count) + 1.5);
        if (widget.itemIndex == widget.route.selectedIndex)
        {
            _opacityAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: widget.route.animation!, curve: new global::Doroti.Framework.Animation.Threshold(0.0));
        }
        else
        {
            double start = Dart_uiLibrary.clampDouble(0.5 + ((widget.itemIndex + 1L) * unit), 0.0, 1.0);
            double end = Dart_uiLibrary.clampDouble(start + (1.5 * unit), 0.0, 1.0);
            _opacityAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: widget.route.animation!, curve: new global::Doroti.Framework.Animation.Interval(start, end));
        }
    }

    internal virtual void _handleFocusChange(bool focused)
    {
        bool inTraditionalMode = FocusManager.instance.highlightMode switch { FocusHighlightMode.touch => false, FocusHighlightMode.traditional => true, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        if (focused && inTraditionalMode)
        {
            _MenuLimits__dropdown menuLimits = widget.route.getMenuLimits(widget.buttonRect, widget.constraints.maxHeight, widget.itemIndex);
            DartRuntimePrimitives.Ignore(widget.scrollController.animateTo(menuLimits.scrollOffset, curve: Curves.easeInOut, duration: Duration.Create(milliseconds: 100L)));
        }
    }

    internal virtual void _handleOnTap()
    {
        DropdownMenuItem<T> dropdownMenuItem = widget.route.items[(int)widget.itemIndex].item!;
        dropdownMenuItem.onTap?.Invoke();
        Navigator.pop<object>(context, new _DropdownRouteResult__dropdown<T>(dropdownMenuItem.value));
    }

    public override void dispose()
    {
        _opacityAnimation.dispose();
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DropdownMenuItem<T> dropdownMenuItem = widget.route.items[(int)widget.itemIndex].item!;
        global::Doroti.Framework.Widgets.Widget childLocal = widget.route.items[(int)widget.itemIndex];
        if (widget.padding is global::Doroti.Framework.Painting.EdgeInsetsGeometry paddingLocal)
        {
            childLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: paddingLocal, child: childLocal));
        }
        childLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(height: widget.route.itemHeight, child: childLocal));
        var isSelected = widget.itemIndex == widget.route.selectedIndex;
        global::Doroti.Framework.Widgets.FocusHighlightMode highlightModeLocal = FocusManager.instance.highlightMode;
        if (dropdownMenuItem.enabled)
        {
            childLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new InkWell(autofocus: isSelected, enableFeedback: widget.enableFeedback, onTap: _handleOnTap, onFocusChange: _handleFocusChange, mouseCursor: widget.mouseCursor, child: Equals(highlightModeLocal, FocusHighlightMode.touch) ? new Ink(color: isSelected ? Theme.of(context).focusColor : null, child: childLocal) : childLocal));
        }
        childLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.FadeTransition(opacity: _opacityAnimation, child: childLocal));
        if (Foundation.ConstantsLibrary.kIsWeb && dropdownMenuItem.enabled)
        {
            childLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Shortcuts(shortcuts: _webShortcuts, child: childLocal));
        }
        return new global::Doroti.Framework.Widgets.Semantics(role: SemanticsRole.menuItem, child: childLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DropdownMenu__dropdown<T> : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual _DropdownRoute__dropdown<T> route { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsets? padding { get; private set; }
    public virtual Rect buttonRect { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.BoxConstraints constraints { get; private set; } = default!;
    public virtual Color? dropdownColor { get; private set; }
    public virtual bool enableFeedback { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.BorderRadius? borderRadius { get; private set; }
    public virtual global::Doroti.Framework.Widgets.ScrollController scrollController { get; private set; } = default!;
    public virtual double? menuWidth { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }

    internal _DropdownMenu__dropdown(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.EdgeInsets? padding = null, _DropdownRoute__dropdown<T> route = default!, Rect buttonRect = default!, global::Doroti.Framework.Rendering.BoxConstraints constraints = default!, Color? dropdownColor = null, bool enableFeedback = default!, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Framework.Widgets.ScrollController scrollController = default!, double? menuWidth = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null) : base(key: key)
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

internal class _DropdownMenuState__dropdown<T> : global::Doroti.Framework.Widgets.State<_DropdownMenu__dropdown<T>>
{
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _fadeOpacity { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _resize { get; private set; } = default!;

    public override void initState()
    {
        base.initState();
        _fadeOpacity = new global::Doroti.Framework.Animation.CurvedAnimation(parent: widget.route.animation!, curve: new global::Doroti.Framework.Animation.Interval(0.0, 0.25), reverseCurve: new global::Doroti.Framework.Animation.Interval(0.75, 1.0));
        _resize = new global::Doroti.Framework.Animation.CurvedAnimation(parent: widget.route.animation!, curve: new global::Doroti.Framework.Animation.Interval(0.25, 0.5), reverseCurve: new global::Doroti.Framework.Animation.Threshold(0.0));
    }

    public override void dispose()
    {
        _fadeOpacity.dispose();
        _resize.dispose();
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        _DropdownRoute__dropdown<T> routeLocal = widget.route;
        var childrenLocal = ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection10886 = new List<global::Doroti.Framework.Widgets.Widget>(); for (long itemIndexLocal = 0L; itemIndexLocal < checked(routeLocal.items.Count); ++itemIndexLocal) { __collection10886.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _DropdownMenuItemButton__dropdown<T>(route: widget.route, padding: widget.padding, buttonRect: widget.buttonRect, constraints: widget.constraints, itemIndex: itemIndexLocal, enableFeedback: widget.enableFeedback, scrollController: widget.scrollController, mouseCursor: widget.mouseCursor))); } return __collection10886; }))();
        return new global::Doroti.Framework.Widgets.FadeTransition(opacity: _fadeOpacity, child: new global::Doroti.Framework.Widgets.CustomPaint(painter: new _DropdownMenuPainter__dropdown(color: widget.dropdownColor ?? Theme.of(context).canvasColor, elevation: routeLocal.elevation, selectedIndex: routeLocal.selectedIndex, resize: _resize, borderRadius: widget.borderRadius, getSelectedItemOffset: () => routeLocal.getItemOffset(routeLocal.selectedIndex)), child: new global::Doroti.Framework.Widgets.Semantics(role: SemanticsRole.menu, scopesRoute: true, namesRoute: true, explicitChildNodes: true, label: localizations.popupMenuLabel, child: new global::Doroti.Framework.Widgets.ClipRRect(borderRadius: widget.borderRadius ?? BorderRadius.zero, clipBehavior: (widget.borderRadius is not null) ? Clip.antiAlias : Clip.none, child: new Material(type: MaterialType.transparency, textStyle: routeLocal.style, child: new global::Doroti.Framework.Widgets.ScrollConfiguration(behavior: ScrollConfiguration.of(context).copyWith(scrollbars: false, overscroll: false, physics: new global::Doroti.Framework.Widgets.ClampingScrollPhysics(), platform: Theme.of(context).platform), child: new global::Doroti.Framework.Widgets.PrimaryScrollController(controller: widget.scrollController, child: new Scrollbar(thumbVisibility: true, child: new global::Doroti.Framework.Widgets.ListView(primary: true, padding: ConstantsLibrary.kMaterialListPadding, shrinkWrap: true, children: childrenLocal)))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DropdownMenuRouteLayout__dropdown<T> : global::Doroti.Framework.Rendering.SingleChildLayoutDelegate
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

    public override global::Doroti.Framework.Rendering.BoxConstraints getConstraintsForChild(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        double maxHeightLocal = Math.Max(0.0, constraints.maxHeight - (2L * DropdownLibrary._kMenuItemHeight));
        if ((route.menuMaxHeight is not null) && (DartRuntimePrimitives.RequireValue(route.menuMaxHeight) <= maxHeightLocal))
        {
            maxHeightLocal = DartRuntimePrimitives.RequireValue(route.menuMaxHeight);
        }
        double widthLocal = Math.Min(constraints.maxWidth, menuWidth ?? buttonRect.width);
        return new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: widthLocal, maxWidth: widthLocal, maxHeight: maxHeightLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Offset getPositionForChild(Size size, Size childSize)
    {
        _MenuLimits__dropdown menuLimits = route.getMenuLimits(buttonRect, size.height, route.selectedIndex);
        DartRuntimePrimitives.Assert(() =>
            {
                global::Doroti.Ui.Rect container = Offset.zero & size;
                if (Equals(container.intersect(buttonRect), buttonRect))
                {
                    DartRuntimePrimitives.Assert(() => menuLimits.top >= 0.0);
                    DartRuntimePrimitives.Assert(() => (menuLimits.top + menuLimits.height) <= size.height);
                }
                return true;
            });
        DartRuntimePrimitives.Assert(() => textDirection is not null);
        double leftLocal = DartRuntimePrimitives.RequireValue(textDirection) switch { TextDirection.rtl => Dart_uiLibrary.clampDouble(buttonRect.right, 0.0, size.width) - childSize.width, TextDirection.ltr => Dart_uiLibrary.clampDouble(buttonRect.left, 0.0, size.width - childSize.width), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        return new global::Doroti.Ui.Offset(leftLocal, menuLimits.top);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRelayout(global::Doroti.Framework.Rendering.SingleChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (_DropdownMenuRouteLayout__dropdown<T>)oldDelegate;
        return (!Equals(buttonRect, __oldDelegate.buttonRect)) || (!Equals(textDirection, __oldDelegate.textDirection));
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
        return (__other is _DropdownRouteResult__dropdown<T>) && EqualityComparer<T>.Default.Equals(__other.result, result);
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(result?.GetHashCode() ?? 0);
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

public class _DropdownRoute__dropdown<T> : global::Doroti.Framework.Widgets.PopupRoute<_DropdownRouteResult__dropdown<T>>
{
    public virtual List<_MenuItem__dropdown<T>> items { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry padding { get; private set; } = default!;
    public virtual Rect buttonRect { get; private set; } = default!;
    public virtual long selectedIndex { get; private set; } = default!;
    public virtual long elevation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.CapturedThemes capturedThemes { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle style { get; private set; } = default!;
    public virtual double? itemHeight { get; private set; }
    public virtual double? menuWidth { get; private set; }
    public virtual Color? dropdownColor { get; private set; }
    public virtual double? menuMaxHeight { get; private set; }
    public virtual bool enableFeedback { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.BorderRadius? borderRadius { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? dropdownMenuItemMouseCursor { get; private set; }
    public virtual List<double> itemHeights { get; private set; } = default!;
    private bool __field_barrierDismissible = default!;
    public override bool barrierDismissible { get => __field_barrierDismissible; }
    private string? __field_barrierLabel = default!;
    public override string? barrierLabel { get => __field_barrierLabel; }

    internal _DropdownRoute__dropdown(List<_MenuItem__dropdown<T>> items, global::Doroti.Framework.Painting.EdgeInsetsGeometry padding, Rect buttonRect, long selectedIndex, long elevation = 8, global::Doroti.Framework.Widgets.CapturedThemes capturedThemes = default!, global::Doroti.Framework.Painting.TextStyle style = default!, string? barrierLabel = null, double? itemHeight = null, double? menuWidth = null, Color? dropdownColor = null, double? menuMaxHeight = null, bool enableFeedback = default!, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, bool barrierDismissible = true, global::Doroti.Framework.Services.MouseCursor? dropdownMenuItemMouseCursor = null)
    {
        this.items = items;
        this.padding = padding;
        this.buttonRect = buttonRect;
        this.selectedIndex = selectedIndex;
        this.elevation = elevation;
        this.capturedThemes = capturedThemes;
        this.style = style;
        __field_barrierLabel = barrierLabel;
        this.itemHeight = itemHeight;
        this.menuWidth = menuWidth;
        this.dropdownColor = dropdownColor;
        this.menuMaxHeight = menuMaxHeight;
        this.enableFeedback = enableFeedback;
        this.borderRadius = borderRadius;
        __field_barrierDismissible = barrierDismissible;
        this.dropdownMenuItemMouseCursor = dropdownMenuItemMouseCursor;
        itemHeights = new List<double>(Enumerable.Repeat<double>(itemHeight ?? ConstantsLibrary.kMinInteractiveDimension, checked((int)checked((long)items.Count))));
    }

    public override Duration transitionDuration => DropdownLibrary._kDropdownMenuDuration;
    public override Color? barrierColor => DartRuntimePrimitives.ConvertValue<Color>(null);
    public override global::Doroti.Framework.Widgets.Widget buildPage(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation)
    {
        return new global::Doroti.Framework.Widgets.LayoutBuilder(builder: (context, constraints) =>
        {
            return new _DropdownRoutePage__dropdown<T>(route: this, constraints: constraints, items: items, padding: padding, buttonRect: buttonRect, selectedIndex: selectedIndex, elevation: elevation, capturedThemes: capturedThemes, style: style, dropdownColor: dropdownColor, enableFeedback: enableFeedback, borderRadius: borderRadius, menuWidth: menuWidth, mouseCursor: dropdownMenuItemMouseCursor);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _dismiss()
    {
        if (isActive)
        {
            navigator?.removeRoute(this);
        }
    }

    public virtual double getItemOffset(long index)
    {
        double offset = ConstantsLibrary.kMaterialListPadding.top;
        if (Enumerable.Any(items) && (index > 0L))
        {
            DartRuntimePrimitives.Assert(() => checked(items.Count) == checked((long)itemHeights.Count));
            offset += itemHeights.GetRange(0L, index).reduce((total, height) => total + height);
        }
        return offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual _MenuLimits__dropdown getMenuLimits(Rect buttonRect, double availableHeight, long index)
    {
        double computedMaxHeight = availableHeight - (2.0 * DropdownLibrary._kMenuItemHeight);
        if (menuMaxHeight is not null)
        {
            double menuMaxHeight__value19574 = DartRuntimePrimitives.RequireValue(menuMaxHeight);
            computedMaxHeight = Math.Min(computedMaxHeight, DartRuntimePrimitives.RequireValue(menuMaxHeight));
        }
        double buttonTop = buttonRect.top;
        double buttonBottom = Math.Min(buttonRect.bottom, availableHeight);
        double selectedItemOffset = getItemOffset(index);
        double topLimit = Math.Min(DropdownLibrary._kMenuItemHeight, buttonTop);
        double bottomLimit = Math.Max(availableHeight - DropdownLibrary._kMenuItemHeight, buttonBottom);
        double menuTop = buttonTop - selectedItemOffset - ((itemHeights[(int)selectedIndex] - buttonRect.height) / 2.0);
        double preferredMenuHeight = ConstantsLibrary.kMaterialListPadding.vertical;
        if (Enumerable.Any(items))
        {
            preferredMenuHeight += itemHeights.reduce((total, height) => total + height);
        }
        double menuHeight = Math.Min(computedMaxHeight, preferredMenuHeight);
        double menuBottom = menuTop + menuHeight;
        if (menuTop < topLimit)
        {
            menuTop = Math.Min(buttonTop, topLimit);
            menuBottom = menuTop + menuHeight;
        }
        if (menuBottom > bottomLimit)
        {
            menuBottom = Math.Max(buttonBottom, bottomLimit);
            menuTop = menuBottom - menuHeight;
        }
        if ((menuBottom - (itemHeights[(int)selectedIndex] / 2.0)) < (buttonBottom - (buttonRect.height / 2.0)))
        {
            menuBottom = buttonBottom - (buttonRect.height / 2.0) + (itemHeights[(int)selectedIndex] / 2.0);
            menuTop = menuBottom - menuHeight;
        }
        double scrollOffset = 0;
        if (preferredMenuHeight > computedMaxHeight)
        {
            scrollOffset = Math.Max(0.0, selectedItemOffset - (buttonTop - menuTop));
            scrollOffset = Math.Min(scrollOffset, preferredMenuHeight - menuHeight);
        }
        DartRuntimePrimitives.Assert(() => (menuBottom - menuTop - menuHeight).abs() < Foundation.ConstantsLibrary.precisionErrorTolerance);
        return new _MenuLimits__dropdown(menuTop, menuBottom, menuHeight, scrollOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DropdownRoutePage__dropdown<T> : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual _DropdownRoute__dropdown<T> route { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.BoxConstraints constraints { get; private set; } = default!;
    public virtual List<_MenuItem__dropdown<T>>? items { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry padding { get; private set; } = default!;
    public virtual Rect buttonRect { get; private set; } = default!;
    public virtual long selectedIndex { get; private set; } = default!;
    public virtual long elevation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.CapturedThemes capturedThemes { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle? style { get; private set; }
    public virtual Color? dropdownColor { get; private set; }
    public virtual bool enableFeedback { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.BorderRadius? borderRadius { get; private set; }
    public virtual double? menuWidth { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }

    internal _DropdownRoutePage__dropdown(global::Doroti.Framework.Foundation.Key? key = null, _DropdownRoute__dropdown<T> route = default!, global::Doroti.Framework.Rendering.BoxConstraints constraints = default!, List<_MenuItem__dropdown<T>>? items = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry padding = default!, Rect buttonRect = default!, long selectedIndex = default!, long elevation = 8, global::Doroti.Framework.Widgets.CapturedThemes capturedThemes = default!, global::Doroti.Framework.Painting.TextStyle? style = null, Color? dropdownColor = default!, bool enableFeedback = default!, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, double? menuWidth = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null) : base(key: key)
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

internal class _DropdownRoutePageState__dropdown<T> : global::Doroti.Framework.Widgets.State<_DropdownRoutePage__dropdown<T>>
{
    internal virtual global::Doroti.Framework.Widgets.ScrollController _scrollController { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        _MenuLimits__dropdown menuLimits = widget.route.getMenuLimits(widget.buttonRect, widget.constraints.maxHeight, widget.selectedIndex);
        _scrollController = new global::Doroti.Framework.Widgets.ScrollController(initialScrollOffset: menuLimits.scrollOffset);
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        global::Doroti.Ui.TextDirection? textDirectionLocal = Directionality.maybeOf(context);
        global::Doroti.Framework.Widgets.Widget menu = new _DropdownMenu__dropdown<T>(route: widget.route, padding: widget.padding.resolve(textDirectionLocal), buttonRect: widget.buttonRect, constraints: widget.constraints, dropdownColor: widget.dropdownColor, enableFeedback: widget.enableFeedback, borderRadius: widget.borderRadius, scrollController: _scrollController, mouseCursor: widget.mouseCursor);
        return MediaQuery.CreateRemovePadding(context: context, removeTop: true, removeBottom: true, removeLeft: true, removeRight: true, child: new global::Doroti.Framework.Widgets.Builder(builder: (context) =>
        {
            return new global::Doroti.Framework.Widgets.CustomSingleChildLayout(@delegate: new _DropdownMenuRouteLayout__dropdown<T>(buttonRect: widget.buttonRect, route: widget.route, textDirection: textDirectionLocal, menuWidth: widget.menuWidth), child: widget.capturedThemes.wrap(menu));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        _scrollController.dispose();
        base.dispose();
    }

}

public class _MenuItem__dropdown<T> : global::Doroti.Framework.Widgets.SingleChildRenderObjectWidget
{
    public virtual global::System.Action<Size> onLayout { get; private set; } = default!;
    public virtual DropdownMenuItem<T>? item { get; private set; }

    internal _MenuItem__dropdown(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action<Size> onLayout = default!, DropdownMenuItem<T>? item = default!) : base(key: key, child: item)
    {
        this.onLayout = onLayout;
        this.item = item;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new _RenderMenuItem__dropdown(onLayout);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderMenuItem__dropdown)renderObject;
        __renderObject.onLayout = onLayout;
    }

}

public class _RenderMenuItem__dropdown : global::Doroti.Framework.Rendering.RenderProxyBox
{
    public virtual global::System.Action<Size> onLayout { get; set; } = default!;

    internal _RenderMenuItem__dropdown(global::System.Action<Size> onLayout, global::Doroti.Framework.Rendering.RenderBox? child = null) : base(child)
    {
        this.onLayout = onLayout;
    }

    public override void performLayout()
    {
        base.performLayout();
        onLayout(size);
    }

}

public class _DropdownMenuItemContainer__dropdown : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;

    internal _DropdownMenuItemContainer__dropdown(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key)
    {
        global::Doroti.Framework.Painting.AlignmentGeometry __alignment = alignment ?? AlignmentDirectional.centerStart;
        this.alignment = __alignment;
        this.child = child;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.Semantics(button: true, child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minHeight: DropdownLibrary._kMenuItemHeight), child: new global::Doroti.Framework.Widgets.Align(alignment: alignment, child: child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DropdownMenuItem<T> : _DropdownMenuItemContainer__dropdown
{
    public virtual global::System.Action? onTap { get; private set; }
    public virtual T? value { get; private set; }
    public virtual bool enabled { get; private set; } = default!;

    public DropdownMenuItem(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action? onTap = null, T? value = default, bool enabled = true, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, alignment: alignment ?? AlignmentDirectional.centerStart, child: child)
    {
        this.onTap = onTap;
        this.value = value;
        this.enabled = enabled;
    }

}

public class DropdownButtonHideUnderline : global::Doroti.Framework.Widgets.InheritedWidget
{
    public DropdownButtonHideUnderline(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
    }

    public static bool at(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<DropdownButtonHideUnderline>() is not null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => false;
}

public class DropdownButton<T> : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual List<DropdownMenuItem<T>>? items { get; private set; }
    public virtual T? value { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? hint { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? disabledHint { get; private set; }
    public virtual global::System.Action<T?>? onChanged { get; private set; }
    public virtual global::System.Action? onTap { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, List<global::Doroti.Framework.Widgets.Widget>>? selectedItemBuilder { get; private set; }
    public virtual long elevation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle? style { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? underline { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? icon { get; private set; }
    public virtual Color? iconDisabledColor { get; private set; }
    public virtual Color? iconEnabledColor { get; private set; }
    public virtual double iconSize { get; private set; } = default!;
    public virtual bool isDense { get; private set; } = default!;
    public virtual bool isExpanded { get; private set; } = default!;
    public virtual double? itemHeight { get; private set; }
    public virtual double? menuWidth { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual Color? dropdownColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual double? menuMaxHeight { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.BorderRadius? borderRadius { get; private set; }
    public virtual bool barrierDismissible { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? dropdownMenuItemMouseCursor { get; private set; }
    internal virtual InputDecoration? _inputDecoration { get; private set; }
    internal virtual bool _isEmpty { get; private set; } = default!;

    public DropdownButton(global::Doroti.Framework.Foundation.Key? key = null, List<DropdownMenuItem<T>>? items = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, List<global::Doroti.Framework.Widgets.Widget>>? selectedItemBuilder = null, T? value = default, global::Doroti.Framework.Widgets.Widget? hint = null, global::Doroti.Framework.Widgets.Widget? disabledHint = null, global::System.Action<T?>? onChanged = default!, global::System.Action? onTap = null, long elevation = 8, global::Doroti.Framework.Painting.TextStyle? style = null, global::Doroti.Framework.Widgets.Widget? underline = null, global::Doroti.Framework.Widgets.Widget? icon = null, Color? iconDisabledColor = null, Color? iconEnabledColor = null, double iconSize = 24.0, bool isDense = false, bool isExpanded = false, double? itemHeight = null, double? menuWidth = null, Color? focusColor = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, Color? dropdownColor = null, double? menuMaxHeight = null, bool? enableFeedback = null, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, bool barrierDismissible = true, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Framework.Services.MouseCursor? dropdownMenuItemMouseCursor = null) : base(key: key)
    {
        double? __itemHeight = itemHeight ?? ConstantsLibrary.kMinInteractiveDimension;
        global::Doroti.Framework.Painting.AlignmentGeometry __alignment = alignment ?? AlignmentDirectional.centerStart;
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
        _inputDecoration = null;
        _isEmpty = false;
        System.Diagnostics.Debug.Assert((items is null) || !Enumerable.Any(items) || (value is null) || (items.where((item) =>
        {
            return Equals(item.value, value);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }).Count() == 1L));
        System.Diagnostics.Debug.Assert((__itemHeight is null) || (__itemHeight >= ConstantsLibrary.kMinInteractiveDimension));
    }

    public static DropdownButton<T> Create_formField(global::Doroti.Framework.Foundation.Key? key = null, List<DropdownMenuItem<T>>? items = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, List<global::Doroti.Framework.Widgets.Widget>>? selectedItemBuilder = null, T? value = default, global::Doroti.Framework.Widgets.Widget? hint = null, global::Doroti.Framework.Widgets.Widget? disabledHint = null, global::System.Action<T?>? onChanged = default!, global::System.Action? onTap = null, long elevation = 8, global::Doroti.Framework.Painting.TextStyle? style = null, global::Doroti.Framework.Widgets.Widget? underline = null, global::Doroti.Framework.Widgets.Widget? icon = null, Color? iconDisabledColor = null, Color? iconEnabledColor = null, double iconSize = 24.0, bool isDense = false, bool isExpanded = false, double? itemHeight = null, double? menuWidth = null, Color? focusColor = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, Color? dropdownColor = null, double? menuMaxHeight = null, bool? enableFeedback = null, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, bool barrierDismissible = true, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Framework.Services.MouseCursor? dropdownMenuItemMouseCursor = null, InputDecoration inputDecoration = default!, bool isEmpty = default!)
    {
        var __instance = new DropdownButton<T>(key: key, items: items, selectedItemBuilder: selectedItemBuilder, value: value, hint: hint, disabledHint: disabledHint, onChanged: onChanged, onTap: onTap, elevation: elevation, style: style, underline: underline, icon: icon, iconDisabledColor: iconDisabledColor, iconEnabledColor: iconEnabledColor, iconSize: iconSize, isDense: isDense, isExpanded: isExpanded, itemHeight: itemHeight, menuWidth: menuWidth, focusColor: focusColor, focusNode: focusNode, autofocus: autofocus, dropdownColor: dropdownColor, menuMaxHeight: menuMaxHeight, enableFeedback: enableFeedback, alignment: alignment, borderRadius: borderRadius, padding: padding, barrierDismissible: barrierDismissible, mouseCursor: mouseCursor, dropdownMenuItemMouseCursor: dropdownMenuItemMouseCursor);
        double? __itemHeight = itemHeight ?? ConstantsLibrary.kMinInteractiveDimension;
        global::Doroti.Framework.Painting.AlignmentGeometry __alignment = alignment ?? AlignmentDirectional.centerStart;
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

internal class _DropdownButtonState__dropdown<T> : global::Doroti.Framework.Widgets.State<DropdownButton<T>>, global::Doroti.Framework.Widgets.WidgetsBindingObserver
{
    internal virtual long? _selectedIndex { get; set; } = default;
    internal virtual _DropdownRoute__dropdown<T>? _dropdownRoute { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.Orientation? _lastOrientation { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.FocusNode? _internalNode { get; set; } = default;
    internal virtual DartMap<Type, dynamic> _actionMap { get; set; } = default!;
    internal virtual bool _isHovering { get; set; } = false;
    internal virtual bool _hasPrimaryFocus { get; set; } = false;
    internal virtual bool _isMenuExpanded { get; set; } = false;

    public virtual global::Doroti.Framework.Widgets.FocusNode focusNode => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.FocusNode>(widget.focusNode ?? _internalNode!);
    internal virtual global::Doroti.Framework.Widgets.FocusNode _createFocusNode()
    {
        return new global::Doroti.Framework.Widgets.FocusNode(debugLabel: $"{DartRuntimePrimitives.RuntimeType(widget)}");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void initState()
    {
        base.initState();
        _updateSelectedIndex();
        if (widget.focusNode is null)
        {
            _internalNode ??= _createFocusNode();
        }
        _actionMap = new DartMap<Type, dynamic> { [typeof(global::Doroti.Framework.Widgets.ActivateIntent)] = new global::Doroti.Framework.Widgets.CallbackAction<global::Doroti.Framework.Widgets.ActivateIntent>(onInvoke: (global::System.Action<global::Doroti.Framework.Widgets.ActivateIntent>)((intent) => { _handleTap(); })), [typeof(global::Doroti.Framework.Widgets.ButtonActivateIntent)] = new global::Doroti.Framework.Widgets.CallbackAction<global::Doroti.Framework.Widgets.ButtonActivateIntent>(onInvoke: (global::System.Action<global::Doroti.Framework.Widgets.ButtonActivateIntent>)((intent) => { _handleTap(); })) };
        focusNode.addListener(_handleFocusChanged);
    }

    public override void dispose()
    {
        WidgetsBinding.instance.removeObserver(this);
        _removeDropdownRoute();
        focusNode.removeListener(_handleFocusChanged);
        _internalNode?.dispose();
        base.dispose();
    }

    internal virtual void _handleFocusChanged()
    {
        if (_hasPrimaryFocus != focusNode.hasPrimaryFocus)
        {
            setState(() =>
            {
                _hasPrimaryFocus = focusNode.hasPrimaryFocus;
            });
        }
    }

    internal virtual void _removeDropdownRoute()
    {
        _dropdownRoute?._dismiss();
        _dropdownRoute = null;
        _lastOrientation = null;
    }

    public override void didUpdateWidget(DropdownButton<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.focusNode, oldWidget.focusNode))
        {
            oldWidget.focusNode?.removeListener(_handleFocusChanged);
            if ((_internalNode is not null) && (widget.focusNode is not null))
            {
                _internalNode!.dispose();
                _internalNode = null;
            }
            if (widget.focusNode is null)
            {
                _internalNode ??= _createFocusNode();
            }
            _hasPrimaryFocus = focusNode.hasPrimaryFocus;
            focusNode.addListener(_handleFocusChanged);
        }
        _updateSelectedIndex();
    }

    internal virtual void _updateSelectedIndex()
    {
        if ((widget.items is null) || !Enumerable.Any(widget.items!) || (widget.value is null) && !Enumerable.Any(widget.items!.where((item) => item.enabled && EqualityComparer<T>.Default.Equals(item.value, widget.value))))
        {
            _selectedIndex = null;
            return;
        }
        DartRuntimePrimitives.Assert(() => widget.items!.where((item) => EqualityComparer<T>.Default.Equals(item.value, widget.value)).Count() == 1L);
        for (var itemIndex = 0L; itemIndex < checked(widget.items!.Count); itemIndex++)
        {
            if (EqualityComparer<T>.Default.Equals(widget.items![(int)itemIndex].value, widget.value))
            {
                _selectedIndex = itemIndex;
                return;
            }
        }
    }

    internal virtual global::Doroti.Framework.Painting.TextStyle? _textStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.TextStyle>(widget.style ?? Theme.of(context).textTheme.titleMedium);
    internal virtual void _handleTap()
    {
        global::Doroti.Ui.TextDirection? textDirection = Directionality.maybeOf(context);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry menuMargin = ButtonTheme.of(context).alignedDropdown ? DropdownLibrary._kAlignedMenuMargin : DropdownLibrary._kUnalignedMenuMargin;
        var menuItems = ((Func<List<_MenuItem__dropdown<T>>>)(() =>
        {
            var __collection51034 = new List<_MenuItem__dropdown<T>>(); for (long index = 0L; index < checked(widget.items!.Count); index += 1L)
            {
                __collection51034.Add(new _MenuItem__dropdown<T>(item: widget.items![(int)index], onLayout: (size) =>
                {
                    if (_dropdownRoute is null)
                    {
                        return;
                    }
                    _dropdownRoute!.itemHeights[(int)index] = size.height;
                }));
            }
            return __collection51034;
        }))();
        global::Doroti.Framework.Widgets.NavigatorState navigator = Navigator.of(context);
        DartRuntimePrimitives.Assert(() => _dropdownRoute is null);
        var itemBox = ((global::Doroti.Framework.Rendering.RenderBox?)context.findRenderObject()!)!;
        global::Doroti.Ui.Rect itemRect = itemBox.localToGlobal(Offset.zero, ancestor: navigator.context.findRenderObject()) & itemBox.size;
        _dropdownRoute = new _DropdownRoute__dropdown<T>(items: menuItems, buttonRect: menuMargin.resolve(textDirection).inflateRect(itemRect), padding: DropdownLibrary._kMenuItemPadding.resolve(textDirection), selectedIndex: _selectedIndex ?? 0L, elevation: widget.elevation, capturedThemes: InheritedTheme.capture(from: context, to: navigator.context), style: _textStyle!, barrierLabel: MaterialLocalizations.of(context).modalBarrierDismissLabel, itemHeight: widget.itemHeight, menuWidth: widget.menuWidth, dropdownColor: widget.dropdownColor, menuMaxHeight: widget.menuMaxHeight, enableFeedback: widget.enableFeedback ?? true, borderRadius: widget.borderRadius, barrierDismissible: widget.barrierDismissible, dropdownMenuItemMouseCursor: widget.dropdownMenuItemMouseCursor);
        focusNode.requestFocus();
        DartRuntimePrimitives.Ignore(navigator.push(_dropdownRoute!).then((newValue) =>
        {
            _removeDropdownRoute();
            if (mounted)
            {
                setState(() =>
                {
                    _isMenuExpanded = false;
                });
            }
            if (!mounted || (newValue is null))
            {
                return;
            }
            widget.onChanged?.Invoke(newValue.result);
        }));
        widget.onTap?.Invoke();
        setState(() =>
        {
            _isMenuExpanded = true;
        });
    }

    internal virtual double _denseButtonHeight
    {
        get
        {
            double fontSizeLocal = _textStyle!.fontSize ?? DartRuntimePrimitives.RequireValue(Theme.of(context).textTheme.titleMedium!.fontSize);
            double lineHeight = (_textStyle!.height ?? Theme.of(context).textTheme.titleMedium!.height) ?? 1.0;
            double scaledFontSize = MediaQuery.textScalerOf(context).scale(fontSizeLocal * lineHeight);
            return Math.Max(scaledFontSize, Math.Max(widget.iconSize, DropdownLibrary._kDenseButtonHeight));
        }
    }
    internal virtual global::Doroti.Ui.Color _iconColor
    {
        get
        {
            global::Doroti.Ui.Brightness brightness = Theme.brightnessOf(context);
            if (_enabled)
            {
                return widget.iconEnabledColor ?? (brightness switch { Brightness.light => Colors.grey.shade700, Brightness.dark => Colors.white70, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            }
            else
            {
                return widget.iconDisabledColor ?? (brightness switch { Brightness.light => Colors.grey.shade400, Brightness.dark => Colors.white10, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            }
        }
    }
    internal virtual bool _enabled => DartRuntimePrimitives.ConvertValue<bool>((widget.items is not null) && Enumerable.Any(widget.items!) && (widget.onChanged is not null));
    internal virtual global::Doroti.Framework.Widgets.Orientation _getOrientation(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Widgets.Orientation? result = MediaQuery.maybeOrientationOf(context);
        if (result is null)
        {
            global::Doroti.Ui.Size size = View.of(context).physicalSize;
            result = (size.width > size.height) ? Orientation.landscape : Orientation.portrait;
        }
        return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(result));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        global::Doroti.Framework.Widgets.Orientation newOrientation = _getOrientation(context);
        _lastOrientation ??= newOrientation;
        if (!Equals(newOrientation, _lastOrientation))
        {
            _removeDropdownRoute();
            _lastOrientation = newOrientation;
        }
        List<global::Doroti.Framework.Widgets.Widget> itemsLocal = default!;
        if (widget.selectedItemBuilder is not null)
        {
            List<global::Doroti.Framework.Widgets.Widget> selectedItems = widget.selectedItemBuilder!(context).ToList();
            DartRuntimePrimitives.Assert(() => (widget.items is null) || (checked(selectedItems.Count) == checked((long)widget.items!.Count)), () => (object?)"The selectedItemBuilder must return a list of widgets with the same length as the items list.\n" + $"Currently, selectedItemBuilder returns a list of length {checked((long)selectedItems.Count)}, " + $"but items has length {checked((long)widget.items!.Count)}.");
            itemsLocal = new List<global::Doroti.Framework.Widgets.Widget>(DartRuntimePrimitives.ConvertEnumerable<global::Doroti.Framework.Widgets.Widget>(selectedItems));
        }
        else
        {
            itemsLocal = (widget.items is not null) ? new List<global::Doroti.Framework.Widgets.Widget>(DartRuntimePrimitives.ConvertEnumerable<global::Doroti.Framework.Widgets.Widget>(widget.items!)) : new List<global::Doroti.Framework.Widgets.Widget>();
        }
        long? hintIndex = default!;
        if ((widget.hint is not null) || !_enabled && (widget.disabledHint is not null))
        {
            global::Doroti.Framework.Widgets.Widget displayedHint = _enabled ? widget.hint! : (widget.disabledHint ?? widget.hint!);
            hintIndex = checked(itemsLocal.Count);
            itemsLocal.Add(new global::Doroti.Framework.Widgets.DefaultTextStyle(style: _textStyle!.copyWith(color: Theme.of(context).hintColor), child: new global::Doroti.Framework.Widgets.IgnorePointer(child: new _DropdownMenuItemContainer__dropdown(alignment: widget.alignment, child: displayedHint))));
        }
        global::Doroti.Framework.Painting.EdgeInsetsGeometry paddingLocal = (ButtonTheme.of(context).alignedDropdown && (widget._inputDecoration is null)) ? DropdownLibrary._kAlignedButtonPadding : DropdownLibrary._kUnalignedButtonPadding;
        global::Doroti.Framework.Widgets.Widget innerItemsWidget = default!;
        if (!Enumerable.Any(itemsLocal))
        {
            innerItemsWidget = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(SizedBox.CreateShrink());
        }
        else
        {
            innerItemsWidget = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.IndexedStack(index: _selectedIndex ?? hintIndex, alignment: widget.alignment, children: widget.isDense ? itemsLocal : itemsLocal.map<global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.RenderObjectWidget>((item) =>
            {
                return (widget.itemHeight is not null) ? new global::Doroti.Framework.Widgets.SizedBox(height: widget.itemHeight, child: item) : new global::Doroti.Framework.Widgets.Column(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(item) });
                throw new InvalidOperationException("Dart closure completed without a value.");
            }).Cast<global::Doroti.Framework.Widgets.Widget>().ToList()));
        }
        var defaultIcon = new global::Doroti.Framework.Widgets.Icon(Icons.arrow_drop_down);
        global::Doroti.Framework.Widgets.Widget effectiveSuffixIcon = new global::Doroti.Framework.Widgets.IconTheme(data: new global::Doroti.Framework.Widgets.IconThemeData(color: _iconColor, size: widget.iconSize), child: (widget.icon ?? widget._inputDecoration?.suffixIcon) ?? defaultIcon);
        global::Doroti.Framework.Widgets.Widget result = new global::Doroti.Framework.Widgets.DefaultTextStyle(style: _enabled ? _textStyle! : _textStyle!.copyWith(color: Theme.of(context).disabledColor), child: new global::Doroti.Framework.Widgets.SizedBox(height: widget.isDense ? _denseButtonHeight : null, child: new global::Doroti.Framework.Widgets.Padding(padding: paddingLocal.resolve(Directionality.of(context)), child: new global::Doroti.Framework.Widgets.Row(mainAxisAlignment: MainAxisAlignment.spaceBetween, mainAxisSize: MainAxisSize.min, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection58789 = new List<global::Doroti.Framework.Widgets.Widget>(); if (widget.isExpanded) { __collection58789.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: innerItemsWidget))); } else { __collection58789.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(innerItemsWidget)); } if (widget._inputDecoration is null) { __collection58789.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(effectiveSuffixIcon)); } return __collection58789; }))()))));
        if (!DropdownButtonHideUnderline.at(context))
        {
            var bottomLocal = (widget.isDense || (widget.itemHeight is null)) ? 0.0 : 8.0;
            result = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(result), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Positioned(left: 0.0, right: 0.0, bottom: bottomLocal, child: widget.underline ?? new global::Doroti.Framework.Widgets.Container(height: 1.0, decoration: new global::Doroti.Framework.Painting.BoxDecoration(border: new global::Doroti.Framework.Painting.Border(bottom: new global::Doroti.Framework.Painting.BorderSide(color: new global::Doroti.Ui.Color(4290624957L), width: 0.0)))))) }));
        }
        global::Doroti.Framework.Services.MouseCursor effectiveMouseCursor = WidgetStateProperty.resolveAs<global::Doroti.Framework.Services.MouseCursor>(widget.mouseCursor ?? WidgetStateMouseCursor.adaptiveClickable, ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection59821 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (!_enabled) { __collection59821.Add(WidgetState.disabled); } return __collection59821; }))());
        if (widget._inputDecoration is not null)
        {
            bool filledLocal = widget._inputDecoration?.filled ?? InputDecorationTheme.of(context).filled;
            bool oulined = (widget._inputDecoration?.border?.isOutline ?? InputDecorationTheme.of(context).border?.isOutline) ?? false;
            var suffixIconEndMargin = (filledLocal || oulined) ? 12.0 : 0.0;
            InputDecoration effectiveDecoration = widget._inputDecoration!.copyWith(suffixIconConstraints: new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: widget.iconSize + suffixIconEndMargin, minHeight: widget.iconSize), suffixIcon: new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsetsGeometry.CreateDirectional(end: suffixIconEndMargin), child: effectiveSuffixIcon));
            if (_hasPrimaryFocus)
            {
                global::Doroti.Ui.Color? focusColorLocal = widget.focusColor ?? effectiveDecoration.focusColor;
                if (focusColorLocal is not null)
                {
                    effectiveDecoration = effectiveDecoration.copyWith(fillColor: focusColorLocal);
                }
            }
            result = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Focus(canRequestFocus: _enabled, focusNode: focusNode, autofocus: widget.autofocus, child: new global::Doroti.Framework.Widgets.MouseRegion(onEnter: (@event) =>
            {
                if (!_isHovering)
                {
                    setState(() =>
                    {
                        _isHovering = true;
                    });
                }
            }, onExit: (@event) =>
            {
                if (_isHovering)
                {
                    setState(() =>
                    {
                        _isHovering = false;
                    });
                }
            }, cursor: effectiveMouseCursor, child: new global::Doroti.Framework.Widgets.GestureDetector(onTap: _enabled ? _handleTap : null, behavior: HitTestBehavior.opaque, child: new InputDecorator(decoration: effectiveDecoration, isEmpty: widget._isEmpty, isFocused: _hasPrimaryFocus, isHovering: _isHovering, child: (widget.padding is null) ? result : new global::Doroti.Framework.Widgets.Padding(padding: widget.padding!, child: result))))));
        }
        else
        {
            result = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new InkWell(mouseCursor: effectiveMouseCursor, onTap: _enabled ? _handleTap : null, canRequestFocus: _enabled, borderRadius: widget.borderRadius, focusNode: focusNode, autofocus: widget.autofocus, focusColor: widget.focusColor ?? Theme.of(context).focusColor, enableFeedback: false, child: (widget.padding is null) ? result : new global::Doroti.Framework.Widgets.Padding(padding: widget.padding!, child: result)));
        }
        bool childHasButtonSemantic = (hintIndex is not null) || (_selectedIndex is not null) && (widget.selectedItemBuilder is null);
        return new global::Doroti.Framework.Widgets.Semantics(button: !childHasButtonSemantic, expanded: _isMenuExpanded, child: new global::Doroti.Framework.Widgets.Actions(actions: _actionMap, child: result));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DropdownButtonFormField<T> : global::Doroti.Framework.Widgets.FormField<T>
{
    public virtual global::System.Action<T?>? onChanged { get; private set; }
    public virtual InputDecoration decoration { get; private set; } = default!;
    public virtual bool barrierDismissible { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? dropdownMenuItemMouseCursor { get; private set; }

    public DropdownButtonFormField(global::Doroti.Framework.Foundation.Key? key = null, List<DropdownMenuItem<T>>? items = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, List<global::Doroti.Framework.Widgets.Widget>>? selectedItemBuilder = null, T? value = default, T? initialValue = default, global::Doroti.Framework.Widgets.Widget? hint = null, global::Doroti.Framework.Widgets.Widget? disabledHint = null, global::System.Action<T?>? onChanged = default!, global::System.Action? onTap = null, long elevation = 8, global::Doroti.Framework.Painting.TextStyle? style = null, global::Doroti.Framework.Widgets.Widget? icon = null, Color? iconDisabledColor = null, Color? iconEnabledColor = null, double iconSize = 24.0, bool isDense = true, bool isExpanded = false, double? itemHeight = null, Color? focusColor = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, Color? dropdownColor = null, InputDecoration? decoration = null, global::System.Action<T?>? onSaved = null, global::System.Func<T?, string?>? validator = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, string, global::Doroti.Framework.Widgets.Widget>? errorBuilder = null, string? forceErrorText = null, global::Doroti.Framework.Widgets.AutovalidateMode? autovalidateMode = null, double? menuMaxHeight = null, bool? enableFeedback = null, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, bool barrierDismissible = true, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Framework.Services.MouseCursor? dropdownMenuItemMouseCursor = null) : base(key: key, onSaved: onSaved, validator: validator, errorBuilder: errorBuilder, forceErrorText: forceErrorText, initialValue: initialValue ?? value, autovalidateMode: autovalidateMode ?? AutovalidateMode.disabled, builder: (field) =>
    {
        var state = ((_DropdownButtonFormFieldState__dropdown<T>?)field)!;
        InputDecoration effectiveDecoration = (decoration ?? new InputDecoration()).applyDefaults(InputDecorationTheme.of(((_DropdownButtonFormFieldState__dropdown<T>)field).context));
        bool showSelectedItem = (items is not null) && Enumerable.Any(items.where((item) => EqualityComparer<T>.Default.Equals(item.value, state.value)));
        bool isDropdownEnabled = (onChanged is not null) && (items is not null) && Enumerable.Any(items);
        global::Doroti.Framework.Widgets.Widget? decorationHint = (effectiveDecoration.hintText is not null) ? new global::Doroti.Framework.Widgets.Text(effectiveDecoration.hintText!) : null;
        global::Doroti.Framework.Widgets.Widget? effectiveHint = hint ?? decorationHint;
        global::Doroti.Framework.Widgets.Widget? effectiveDisabledHint = disabledHint ?? effectiveHint;
        bool isHintOrDisabledHintAvailable = isDropdownEnabled ? (effectiveHint is not null) : ((effectiveHint is not null) || (effectiveDisabledHint is not null));
        bool isEmptyLocal = !showSelectedItem && !isHintOrDisabledHintAvailable;
        if ((((_DropdownButtonFormFieldState__dropdown<T>)field).errorText is not null) || (effectiveDecoration.hintText is not null))
        {
            global::Doroti.Framework.Widgets.Widget? errorLocal = ((((_DropdownButtonFormFieldState__dropdown<T>)field).errorText is not null) && (errorBuilder is not null)) ? errorBuilder(state.context, ((_DropdownButtonFormFieldState__dropdown<T>)field).errorText!) : null;
            string? errorTextLocal = (errorLocal is null) ? ((_DropdownButtonFormFieldState__dropdown<T>)field).errorText : null;
            string? hintTextLocal = (effectiveDecoration.hintText is not null) ? "" : null;
            effectiveDecoration = effectiveDecoration.copyWith(error: errorLocal, errorText: errorTextLocal, hintText: hintTextLocal);
        }
        return new global::Doroti.Framework.Widgets.Focus(canRequestFocus: false, skipTraversal: true, child: new DropdownButtonHideUnderline(child: DropdownButton<T>.Create_formField(items: items, selectedItemBuilder: selectedItemBuilder, value: state.value, hint: effectiveHint, disabledHint: effectiveDisabledHint, onChanged: (onChanged is null) ? null : state.didChange, onTap: onTap, elevation: elevation, style: style, icon: icon, iconDisabledColor: iconDisabledColor, iconEnabledColor: iconEnabledColor, iconSize: iconSize, isDense: isDense, isExpanded: isExpanded, itemHeight: itemHeight, focusColor: focusColor, focusNode: focusNode, autofocus: autofocus, dropdownColor: dropdownColor, menuMaxHeight: menuMaxHeight, enableFeedback: enableFeedback, alignment: alignment ?? AlignmentDirectional.centerStart, borderRadius: borderRadius, inputDecoration: effectiveDecoration, isEmpty: isEmptyLocal, padding: padding, barrierDismissible: barrierDismissible, mouseCursor: mouseCursor, dropdownMenuItemMouseCursor: dropdownMenuItemMouseCursor)));
        throw new InvalidOperationException("Dart closure completed without a value.");
    })
    {
        global::Doroti.Framework.Painting.AlignmentGeometry __alignment = alignment ?? AlignmentDirectional.centerStart;
        this.onChanged = onChanged;
        this.barrierDismissible = barrierDismissible;
        this.mouseCursor = mouseCursor;
        this.dropdownMenuItemMouseCursor = dropdownMenuItemMouseCursor;
        this.decoration = decoration ?? new InputDecoration();
        System.Diagnostics.Debug.Assert((items is null) || !Enumerable.Any(items) || (initialValue is null) && (value is null) || (items.where((item) => EqualityComparer<T>.Default.Equals(item.value, initialValue ?? value)).Count() == 1L));
        System.Diagnostics.Debug.Assert((itemHeight is null) || (itemHeight >= ConstantsLibrary.kMinInteractiveDimension));
        System.Diagnostics.Debug.Assert((errorBuilder is null) || (decoration?.errorText is null));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DropdownButtonFormFieldState__dropdown<T>());
}

internal class _DropdownButtonFormFieldState__dropdown<T> : global::Doroti.Framework.Widgets.FormFieldState<T>
{
    internal virtual DropdownButtonFormField<T> _dropdownButtonFormField => ((DropdownButtonFormField<T>?)widget)!;
    public override void didChange(T? value)
    {
        base.didChange(value);
        _dropdownButtonFormField.onChanged?.Invoke(value);
    }

    public override void didUpdateWidget(global::Doroti.Framework.Widgets.FormField<T> oldWidget)
    {
        var __oldWidget = (DropdownButtonFormField<T>)oldWidget;
        base.didUpdateWidget(__oldWidget);
        if (!EqualityComparer<T>.Default.Equals(__oldWidget.initialValue, widget.initialValue))
        {
            setValue(widget.initialValue);
        }
    }

    public override void reset()
    {
        base.reset();
        _dropdownButtonFormField.onChanged?.Invoke(value);
    }

}
