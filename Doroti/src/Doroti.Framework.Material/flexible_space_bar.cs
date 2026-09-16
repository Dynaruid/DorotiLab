// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/flexible_space_bar.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public enum CollapseMode
{
    parallax,
    pin,
    none
}

public enum StretchMode
{
    zoomBackground,
    blurBackground,
    fadeTitle
}

public class FlexibleSpaceBar : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget? title { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? background { get; private set; }
    public virtual bool? centerTitle { get; private set; }
    public virtual CollapseMode collapseMode { get; private set; } = default!;
    public virtual List<StretchMode> stretchModes { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? titlePadding { get; private set; }
    public virtual double expandedTitleScale { get; private set; } = default!;

    public FlexibleSpaceBar(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget? title = null, global::Doroti.Framework.Widgets.Widget? background = null, bool? centerTitle = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? titlePadding = null, CollapseMode collapseMode = CollapseMode.parallax, List<StretchMode> stretchModes = default!, double expandedTitleScale = 1.5) : base(key: key)
    {
        List<StretchMode> __stretchModes = stretchModes ?? new List<StretchMode> { StretchMode.zoomBackground };
        this.title = title;
        this.background = background;
        this.centerTitle = centerTitle;
        this.titlePadding = titlePadding;
        this.collapseMode = collapseMode;
        this.stretchModes = __stretchModes;
        this.expandedTitleScale = expandedTitleScale;
        System.Diagnostics.Debug.Assert(expandedTitleScale >= 1L);
    }

    public static global::Doroti.Framework.Widgets.Widget createSettings(double? toolbarOpacity = null, double? minExtent = null, double? maxExtent = null, bool? isScrolledUnder = null, bool? hasLeading = null, double currentExtent = default!, global::Doroti.Framework.Widgets.Widget child = default!)
    {
        return new FlexibleSpaceBarSettings(toolbarOpacity: toolbarOpacity ?? 1.0, minExtent: minExtent ?? currentExtent, maxExtent: maxExtent ?? currentExtent, isScrolledUnder: isScrolledUnder, hasLeading: hasLeading, currentExtent: currentExtent, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _FlexibleSpaceBarState__flexible_space_bar());
}

internal class _FlexibleSpaceBarState__flexible_space_bar : global::Doroti.Framework.Widgets.State<FlexibleSpaceBar>
{
    internal virtual bool _getEffectiveCenterTitle(ThemeData theme)
    {
        return widget.centerTitle ?? (theme.platform switch { TargetPlatform.android or TargetPlatform.fuchsia or TargetPlatform.linux => false, TargetPlatform.windows => false, TargetPlatform.iOS => true, TargetPlatform.macOS => true, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.Alignment _getTitleAlignment(bool effectiveCenterTitle)
    {
        if (effectiveCenterTitle)
        {
            return Alignment.bottomCenter;
        }
        return Directionality.of(context) switch { TextDirection.rtl => Alignment.bottomRight, TextDirection.ltr => Alignment.bottomLeft, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getCollapsePadding(double t, FlexibleSpaceBarSettings settings)
    {
        switch (widget.collapseMode)
        {
            case CollapseMode.pin:
                {
                    return -(settings.maxExtent - settings.currentExtent);
                }
            case CollapseMode.none:
                {
                    return 0.0;
                }
            case CollapseMode.parallax:
                {
                    double deltaExtent = settings.maxExtent - settings.minExtent;
                    return -new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: deltaExtent / 4.0).transform(t);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.LayoutBuilder(builder: (context, constraints) =>
        {
            FlexibleSpaceBarSettings settings = context.dependOnInheritedWidgetOfExactType<FlexibleSpaceBarSettings>()!;
            var childrenLocal = new List<global::Doroti.Framework.Widgets.Widget>();
            double deltaExtent = settings.maxExtent - settings.minExtent;
            double t = Dart_uiLibrary.clampDouble(1.0 - ((settings.currentExtent - settings.minExtent) / deltaExtent), 0.0, 1.0);
            if (widget.background is not null)
            {
                double fadeStart = Math.Max(0.0, 1.0 - (ConstantsLibrary.kToolbarHeight / deltaExtent));
                var fadeEnd = 1.0;
                DartRuntimePrimitives.Assert(() => fadeStart <= fadeEnd);
                double opacityLocal = (settings.maxExtent == settings.minExtent) ? 1.0 : (1.0 - new global::Doroti.Framework.Animation.Interval(fadeStart, fadeEnd).transform(t));
                double heightLocal = settings.maxExtent;
                if (widget.stretchModes.Contains(StretchMode.zoomBackground) && (constraints.maxHeight > heightLocal))
                {
                    heightLocal = constraints.maxHeight;
                }
                double topPadding = _getCollapsePadding(t, settings);
                childrenLocal.Add(new global::Doroti.Framework.Widgets.Positioned(top: topPadding, left: 0.0, right: 0.0, height: heightLocal, child: new _FlexibleSpaceHeaderOpacity__flexible_space_bar(alwaysIncludeSemantics: true, opacity: opacityLocal, child: widget.background)));
                if (widget.stretchModes.Contains(StretchMode.blurBackground) && (constraints.maxHeight > settings.maxExtent))
                {
                    double blurAmount = (constraints.maxHeight - settings.maxExtent) / 10L;
                    childrenLocal.Add(Positioned.CreateFill(child: new global::Doroti.Framework.Widgets.BackdropFilter(filter: new global::Doroti.Ui.ImageFilter(sigmaX: blurAmount, sigmaY: blurAmount), child: new global::Doroti.Framework.Widgets.ColoredBox(color: Colors.transparent))));
                }
            }
            if (widget.title is not null)
            {
                ThemeData theme = Theme.of(context);
                global::Doroti.Framework.Widgets.Widget? titleLocal = default!;
                switch (theme.platform)
                {
                    case TargetPlatform.iOS:
                    case TargetPlatform.macOS:
                        {
                            titleLocal = widget.title;
                            break;
                        }
                    case TargetPlatform.android:
                    case TargetPlatform.fuchsia:
                    case TargetPlatform.linux:
                    case TargetPlatform.windows:
                        {
                            titleLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(namesRoute: true, child: widget.title));
                            break;
                        }
                }
                if (widget.stretchModes.Contains(StretchMode.fadeTitle) && (constraints.maxHeight > settings.maxExtent))
                {
                    double stretchOpacity = 1L - Dart_uiLibrary.clampDouble((constraints.maxHeight - settings.maxExtent) / 100L, 0.0, 1.0);
                    titleLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Opacity(opacity: stretchOpacity, child: titleLocal));
                }
                double opacityAlternate = settings.toolbarOpacity;
                if (opacityAlternate > 0.0)
                {
                    global::Doroti.Framework.Painting.TextStyle titleStyle = theme.textTheme.titleLarge!;
                    titleStyle = titleStyle.copyWith(color: titleStyle.color!.withOpacity(opacityAlternate));
                    bool effectiveCenterTitle = _getEffectiveCenterTitle(theme);
                    var leadingPadding = (settings.hasLeading ?? true) ? 72.0 : 0.0;
                    global::Doroti.Framework.Painting.EdgeInsetsGeometry paddingLocal = widget.titlePadding ?? EdgeInsetsDirectional.CreateOnly(start: effectiveCenterTitle ? 0.0 : leadingPadding, bottom: 16.0);
                    double scaleValue = new global::Doroti.Framework.Animation.Tween<double>(begin: widget.expandedTitleScale, end: 1.0).transform(t);
                    var scaleTransform = ((Func<Matrix4>)(() =>
            {
                var __cascade = Matrix4.identity();
                __cascade.scaleByDouble(scaleValue, scaleValue, 1.0, 1);
                return __cascade;
            }))();
                    global::Doroti.Framework.Painting.Alignment titleAlignment = _getTitleAlignment(effectiveCenterTitle);
                    childrenLocal.Add(new global::Doroti.Framework.Widgets.Padding(padding: paddingLocal, child: new global::Doroti.Framework.Widgets.Transform(alignment: titleAlignment, transform: scaleTransform, child: new global::Doroti.Framework.Widgets.Align(alignment: titleAlignment, child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: titleStyle, child: new global::Doroti.Framework.Widgets.LayoutBuilder(builder: (context, constraints) =>
                    {
                        return new global::Doroti.Framework.Widgets.SizedBox(width: constraints.maxWidth / scaleValue, child: new global::Doroti.Framework.Widgets.Align(alignment: titleAlignment, child: titleLocal));
                        throw new InvalidOperationException("Dart closure completed without a value.");
                    }))))));
                }
            }
            return new global::Doroti.Framework.Widgets.ClipRect(child: new global::Doroti.Framework.Widgets.Stack(children: childrenLocal));
        });
    }

}

public class FlexibleSpaceBarSettings : global::Doroti.Framework.Widgets.InheritedWidget
{
    public virtual double toolbarOpacity { get; private set; } = default!;
    public virtual double minExtent { get; private set; } = default!;
    public virtual double maxExtent { get; private set; } = default!;
    public virtual double currentExtent { get; private set; } = default!;
    public virtual bool? isScrolledUnder { get; private set; }
    public virtual bool? hasLeading { get; private set; }

    public FlexibleSpaceBarSettings(global::Doroti.Framework.Foundation.Key? key = null, double toolbarOpacity = default!, double minExtent = default!, double maxExtent = default!, double currentExtent = default!, global::Doroti.Framework.Widgets.Widget child = default!, bool? isScrolledUnder = null, bool? hasLeading = null) : base(key: key, child: child)
    {
        this.toolbarOpacity = toolbarOpacity;
        this.minExtent = minExtent;
        this.maxExtent = maxExtent;
        this.currentExtent = currentExtent;
        this.isScrolledUnder = isScrolledUnder;
        this.hasLeading = hasLeading;
        System.Diagnostics.Debug.Assert(minExtent >= 0L);
        System.Diagnostics.Debug.Assert(maxExtent >= 0L);
        System.Diagnostics.Debug.Assert(currentExtent >= 0L);
        System.Diagnostics.Debug.Assert(toolbarOpacity >= 0.0);
        System.Diagnostics.Debug.Assert(minExtent <= maxExtent);
        System.Diagnostics.Debug.Assert(minExtent <= currentExtent);
        System.Diagnostics.Debug.Assert(currentExtent <= maxExtent);
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget)
    {
        var __oldWidget = (FlexibleSpaceBarSettings)oldWidget;
        return (toolbarOpacity != __oldWidget.toolbarOpacity) || (minExtent != __oldWidget.minExtent) || (maxExtent != __oldWidget.maxExtent) || (currentExtent != __oldWidget.currentExtent) || (isScrolledUnder != __oldWidget.isScrolledUnder) || (hasLeading != __oldWidget.hasLeading);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _FlexibleSpaceHeaderOpacity__flexible_space_bar : global::Doroti.Framework.Widgets.SingleChildRenderObjectWidget
{
    public virtual double opacity { get; private set; } = default!;
    public virtual bool alwaysIncludeSemantics { get; private set; } = default!;

    internal _FlexibleSpaceHeaderOpacity__flexible_space_bar(double opacity, global::Doroti.Framework.Widgets.Widget? child, bool alwaysIncludeSemantics) : base(child: child)
    {
        this.opacity = opacity;
        this.alwaysIncludeSemantics = alwaysIncludeSemantics;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new _RenderFlexibleSpaceHeaderOpacity__flexible_space_bar(opacity: opacity, alwaysIncludeSemantics: alwaysIncludeSemantics);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderFlexibleSpaceHeaderOpacity__flexible_space_bar)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderFlexibleSpaceHeaderOpacity__flexible_space_bar>)(() =>
{
    var __cascade = __renderObject;
    __cascade.alwaysIncludeSemantics = alwaysIncludeSemantics;
    __cascade.opacity = opacity;
    return __cascade;
}))());
    }

}

public class _RenderFlexibleSpaceHeaderOpacity__flexible_space_bar : global::Doroti.Framework.Rendering.RenderOpacity
{
    internal _RenderFlexibleSpaceHeaderOpacity__flexible_space_bar(double opacity = 1.0, bool alwaysIncludeSemantics = false) : base(opacity: opacity, alwaysIncludeSemantics: alwaysIncludeSemantics)
    {
    }

    public override bool isRepaintBoundary => false;
    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        if (child is null)
        {
            return;
        }
        if ((opacity * 255L).roundToDouble() <= 0L)
        {
            layer = null;
            return;
        }
        DartRuntimePrimitives.Assert(() => needsCompositing);
        layer = context.pushOpacity(offset, (opacity * 255L).round(), base.paint, oldLayer: ((global::Doroti.Framework.Rendering.OpacityLayer?)layer)!);
        DartRuntimePrimitives.Assert(() =>
            {
                layer!.debugCreator = debugCreator;
                return true;
            });
    }

}
