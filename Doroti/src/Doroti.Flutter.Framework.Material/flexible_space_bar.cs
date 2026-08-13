// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/flexible_space_bar.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

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

public class FlexibleSpaceBar : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? title { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? background { get; private set; }
    public virtual bool? centerTitle { get; private set; }
    public virtual CollapseMode collapseMode { get; private set; } = default!;
    public virtual List<StretchMode> stretchModes { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? titlePadding { get; private set; }
    public virtual double expandedTitleScale { get; private set; } = default!;

    public FlexibleSpaceBar(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget? title = null, global::Doroti.Generated.Framework.Widgets.Widget? background = null, bool? centerTitle = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? titlePadding = null, CollapseMode collapseMode = CollapseMode.parallax, List<StretchMode> stretchModes = default!, double expandedTitleScale = 1.5) : base(key: key)
    {
        List<StretchMode> __stretchModes = stretchModes ?? new List<StretchMode> { StretchMode.zoomBackground };
        this.title = title;
        this.background = background;
        this.centerTitle = centerTitle;
        this.titlePadding = titlePadding;
        this.collapseMode = collapseMode;
        this.stretchModes = __stretchModes;
        this.expandedTitleScale = expandedTitleScale;
        System.Diagnostics.Debug.Assert((expandedTitleScale >= 1L));
    }

    public static global::Doroti.Generated.Framework.Widgets.Widget createSettings(double? toolbarOpacity = null, double? minExtent = null, double? maxExtent = null, bool? isScrolledUnder = null, bool? hasLeading = null, double currentExtent = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new FlexibleSpaceBarSettings(toolbarOpacity: (toolbarOpacity ?? 1.0), minExtent: (minExtent ?? currentExtent), maxExtent: (maxExtent ?? currentExtent), isScrolledUnder: isScrolledUnder, hasLeading: hasLeading, currentExtent: currentExtent, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _FlexibleSpaceBarState__flexible_space_bar());
}

internal class _FlexibleSpaceBarState__flexible_space_bar : global::Doroti.Generated.Framework.Widgets.State<FlexibleSpaceBar>
{
    internal virtual bool _getEffectiveCenterTitle(ThemeData theme)
    {
        return (((FlexibleSpaceBar)this.widget).centerTitle ?? (theme.platform switch { global::Doroti.Generated.Framework.Foundation.TargetPlatform.android or global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia or global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux => false, global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows => false, global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS => true, global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS => true, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Painting.Alignment _getTitleAlignment(bool effectiveCenterTitle)
    {
        if (effectiveCenterTitle)
        {
            return global::Doroti.Generated.Framework.Painting.Alignment.bottomCenter;
        }
        return (Directionality.of(this.context) switch { TextDirection.rtl => global::Doroti.Generated.Framework.Painting.Alignment.bottomRight, TextDirection.ltr => global::Doroti.Generated.Framework.Painting.Alignment.bottomLeft, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getCollapsePadding(double t, FlexibleSpaceBarSettings settings)
    {
        switch (((FlexibleSpaceBar)this.widget).collapseMode)
        {
            case CollapseMode.pin:
                {
                    return -((((FlexibleSpaceBarSettings)settings).maxExtent - ((FlexibleSpaceBarSettings)settings).currentExtent));
                }
            case CollapseMode.none:
                {
                    return 0.0;
                }
            case CollapseMode.parallax:
                {
                    double deltaExtent__8054 = (((FlexibleSpaceBarSettings)settings).maxExtent - ((FlexibleSpaceBarSettings)settings).minExtent);
                    return -new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 0.0, end: (deltaExtent__8054 / 4.0)).transform(t);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.LayoutBuilder(builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Rendering.BoxConstraints, global::Doroti.Generated.Framework.Widgets.Widget>)((context, constraints) => {
FlexibleSpaceBarSettings settings__8384 = context.dependOnInheritedWidgetOfExactType<FlexibleSpaceBarSettings>()!;
var children__8496 = new List<global::Doroti.Generated.Framework.Widgets.Widget>();
double deltaExtent__8541 = (((FlexibleSpaceBarSettings)settings__8384).maxExtent - ((FlexibleSpaceBarSettings)settings__8384).minExtent);
double t__8684 = Dart_uiLibrary.clampDouble((1.0 - (((((FlexibleSpaceBarSettings)settings__8384).currentExtent - ((FlexibleSpaceBarSettings)settings__8384).minExtent)) / deltaExtent__8541)), 0.0, 1.0);
if ((((FlexibleSpaceBar)this.widget).background is not null))
{
    double fadeStart__8906 = Math.Max(0.0, (1.0 - (ConstantsLibrary.kToolbarHeight / deltaExtent__8541)));
    var fadeEnd__8985 = 1.0;
    DartRuntimePrimitives.Assert(() => (fadeStart__8906 <= fadeEnd__8985));
    double opacity__9208 = ((((FlexibleSpaceBarSettings)settings__8384).maxExtent == ((FlexibleSpaceBarSettings)settings__8384).minExtent) ? 1.0 : (1.0 - new global::Doroti.Generated.Framework.Animation.Interval(fadeStart__8906, fadeEnd__8985).transform(t__8684)));
    double height__9361 = ((FlexibleSpaceBarSettings)settings__8384).maxExtent;
    if ((((FlexibleSpaceBar)this.widget).stretchModes.Contains(StretchMode.zoomBackground) && (((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxHeight > height__9361)))
    {
        height__9361 = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxHeight;
    }
    double topPadding__9632 = _getCollapsePadding(t__8684, settings__8384);
    children__8496.Add(new global::Doroti.Generated.Framework.Widgets.Positioned(top: topPadding__9632, left: 0.0, right: 0.0, height: height__9361, child: new _FlexibleSpaceHeaderOpacity__flexible_space_bar(alwaysIncludeSemantics: true, opacity: opacity__9208, child: ((FlexibleSpaceBar)this.widget).background)));
    if ((((FlexibleSpaceBar)this.widget).stretchModes.Contains(StretchMode.blurBackground) && (((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxHeight > ((FlexibleSpaceBarSettings)settings__8384).maxExtent)))
    {
        double blurAmount__10396 = (((((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxHeight - ((FlexibleSpaceBarSettings)settings__8384).maxExtent)) / 10L);
        children__8496.Add(global::Doroti.Generated.Framework.Widgets.Positioned.CreateFill(child: new global::Doroti.Generated.Framework.Widgets.BackdropFilter(filter: new global::Doroti.Flutter.Ui.ImageFilter(sigmaX: blurAmount__10396, sigmaY: blurAmount__10396), child: new global::Doroti.Generated.Framework.Widgets.ColoredBox(color: Colors.transparent))));
    }
}
if ((((FlexibleSpaceBar)this.widget).title is not null))
{
    ThemeData theme__10866 = Theme.of(context);
    global::Doroti.Generated.Framework.Widgets.Widget? title__10912 = default!;
    switch (theme__10866.platform)
    {
        case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
        case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
            {
                title__10912 = ((FlexibleSpaceBar)this.widget).title;
                break;
            }
        case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
        case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
        case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
        case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
            {
                title__10912 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Semantics(namesRoute: true, child: ((FlexibleSpaceBar)this.widget).title));
                break;
            }
    }
    if ((((FlexibleSpaceBar)this.widget).stretchModes.Contains(StretchMode.fadeTitle) && (((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxHeight > ((FlexibleSpaceBarSettings)settings__8384).maxExtent)))
    {
        double stretchOpacity__11503 = (1L - Dart_uiLibrary.clampDouble((((((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxHeight - ((FlexibleSpaceBarSettings)settings__8384).maxExtent)) / 100L), 0.0, 1.0));
        title__10912 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Opacity(opacity: stretchOpacity__11503, child: title__10912));
    }
    double opacity__11719 = ((FlexibleSpaceBarSettings)settings__8384).toolbarOpacity;
    if ((opacity__11719 > 0.0))
    {
        global::Doroti.Generated.Framework.Painting.TextStyle titleStyle__11807 = (theme__10866.useMaterial3 ? theme__10866.textTheme.titleLarge! : theme__10866.primaryTextTheme.titleLarge!);
        titleStyle__11807 = titleStyle__11807.copyWith(color: ((global::Doroti.Generated.Framework.Painting.TextStyle)titleStyle__11807).color!.withOpacity(opacity__11719));
        bool effectiveCenterTitle__12055 = _getEffectiveCenterTitle(theme__10866);
        var leadingPadding__12129 = (((((FlexibleSpaceBarSettings)settings__8384).hasLeading ?? true)) ? 72.0 : 0.0);
        global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding__12227 = (((FlexibleSpaceBar)this.widget).titlePadding ?? global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: (effectiveCenterTitle__12055 ? 0.0 : leadingPadding__12129), bottom: 16.0));
        double scaleValue__12466 = new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: ((FlexibleSpaceBar)this.widget).expandedTitleScale, end: 1.0).transform(t__8684);
        var scaleTransform__12612 = ((Func<Matrix4>)(() =>
{            var __cascade = Matrix4.identity();
            __cascade.scaleByDouble(scaleValue__12466, scaleValue__12466, 1.0, 1);
            return __cascade;        }))();
        global::Doroti.Generated.Framework.Painting.Alignment titleAlignment__12739 = ((global::Doroti.Generated.Framework.Painting.Alignment)(object?)_getTitleAlignment(effectiveCenterTitle__12055));
        children__8496.Add(new global::Doroti.Generated.Framework.Widgets.Padding(padding: padding__12227, child: new global::Doroti.Generated.Framework.Widgets.Transform(alignment: titleAlignment__12739, transform: scaleTransform__12612, child: new global::Doroti.Generated.Framework.Widgets.Align(alignment: titleAlignment__12739, child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: titleStyle__11807, child: new global::Doroti.Generated.Framework.Widgets.LayoutBuilder(builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Rendering.BoxConstraints, global::Doroti.Generated.Framework.Widgets.Widget>)((context, constraints) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.SizedBox(width: (((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxWidth / scaleValue__12466), child: new global::Doroti.Generated.Framework.Widgets.Align(alignment: titleAlignment__12739, child: title__10912)));
throw new InvalidOperationException("Dart closure completed without a value.");
}))))))));
    }
}
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.ClipRect(child: new global::Doroti.Generated.Framework.Widgets.Stack(children: children__8496)));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class FlexibleSpaceBarSettings : global::Doroti.Generated.Framework.Widgets.InheritedWidget
{
    public virtual double toolbarOpacity { get; private set; } = default!;
    public virtual double minExtent { get; private set; } = default!;
    public virtual double maxExtent { get; private set; } = default!;
    public virtual double currentExtent { get; private set; } = default!;
    public virtual bool? isScrolledUnder { get; private set; }
    public virtual bool? hasLeading { get; private set; }

    public FlexibleSpaceBarSettings(global::Doroti.Generated.Framework.Foundation.Key? key = null, double toolbarOpacity = default!, double minExtent = default!, double maxExtent = default!, double currentExtent = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!, bool? isScrolledUnder = null, bool? hasLeading = null) : base(key: key, child: child)
    {
        this.toolbarOpacity = toolbarOpacity;
        this.minExtent = minExtent;
        this.maxExtent = maxExtent;
        this.currentExtent = currentExtent;
        this.isScrolledUnder = isScrolledUnder;
        this.hasLeading = hasLeading;
        System.Diagnostics.Debug.Assert((minExtent >= 0L));
        System.Diagnostics.Debug.Assert((maxExtent >= 0L));
        System.Diagnostics.Debug.Assert((currentExtent >= 0L));
        System.Diagnostics.Debug.Assert((toolbarOpacity >= 0.0));
        System.Diagnostics.Debug.Assert((minExtent <= maxExtent));
        System.Diagnostics.Debug.Assert((minExtent <= currentExtent));
        System.Diagnostics.Debug.Assert((currentExtent <= maxExtent));
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget)
    {
        var __oldWidget = (FlexibleSpaceBarSettings)(object)oldWidget;
        return ((((((this.toolbarOpacity != ((FlexibleSpaceBarSettings)__oldWidget).toolbarOpacity) || (this.minExtent != ((FlexibleSpaceBarSettings)__oldWidget).minExtent)) || (this.maxExtent != ((FlexibleSpaceBarSettings)__oldWidget).maxExtent)) || (this.currentExtent != ((FlexibleSpaceBarSettings)__oldWidget).currentExtent)) || (this.isScrolledUnder != ((FlexibleSpaceBarSettings)__oldWidget).isScrolledUnder)) || (this.hasLeading != ((FlexibleSpaceBarSettings)__oldWidget).hasLeading));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _FlexibleSpaceHeaderOpacity__flexible_space_bar : global::Doroti.Generated.Framework.Widgets.SingleChildRenderObjectWidget
{
    public virtual double opacity { get; private set; } = default!;
    public virtual bool alwaysIncludeSemantics { get; private set; } = default!;

    internal _FlexibleSpaceHeaderOpacity__flexible_space_bar(double opacity, global::Doroti.Generated.Framework.Widgets.Widget? child, bool alwaysIncludeSemantics) : base(child: child)
    {
        this.opacity = opacity;
        this.alwaysIncludeSemantics = alwaysIncludeSemantics;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderFlexibleSpaceHeaderOpacity__flexible_space_bar(opacity: this.opacity, alwaysIncludeSemantics: this.alwaysIncludeSemantics));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderFlexibleSpaceHeaderOpacity__flexible_space_bar)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderFlexibleSpaceHeaderOpacity__flexible_space_bar>)(() =>
{            var __cascade = __renderObject;
            __cascade.alwaysIncludeSemantics = this.alwaysIncludeSemantics;
            __cascade.opacity = this.opacity;
            return __cascade;        }))());
    }

}

public class _RenderFlexibleSpaceHeaderOpacity__flexible_space_bar : global::Doroti.Generated.Framework.Rendering.RenderOpacity
{
    internal _RenderFlexibleSpaceHeaderOpacity__flexible_space_bar(double opacity = 1.0, bool alwaysIncludeSemantics = false) : base(opacity: opacity, alwaysIncludeSemantics: alwaysIncludeSemantics)
    {
    }

    public override bool isRepaintBoundary => false;
    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        if ((this.child is null))
        {
            return;
        }
        if ((((this.opacity * 255L)).roundToDouble() <= 0L))
        {
            layer = null;
            return;
        }
        DartRuntimePrimitives.Assert(() => this.needsCompositing);
        layer = context.pushOpacity(offset, ((this.opacity * 255L)).round(), (global::System.Action<global::Doroti.Generated.Framework.Rendering.PaintingContext, Offset>)base.paint, oldLayer: ((global::Doroti.Generated.Framework.Rendering.OpacityLayer?)(object?)this.layer)!);
        DartRuntimePrimitives.Assert(() =>
            {
                this.layer!.debugCreator = this.debugCreator;
                return true;
            });
    }

}
