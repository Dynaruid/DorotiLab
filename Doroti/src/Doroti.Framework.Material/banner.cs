// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/banner.dart
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

public static partial class BannerLibrary
{
    internal static Duration _materialBannerTransitionDuration = Duration.Create(milliseconds: 250L);
}

public static partial class BannerLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _materialBannerHeightCurve = ((global::Doroti.Framework.Animation.Curve)(object?)global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
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
    remove
}

public class MaterialBanner : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget content { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle? contentTextStyle { get; private set; }
    public virtual List<global::Doroti.Framework.Widgets.Widget> actions { get; private set; } = default!;
    public virtual double? elevation { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? leading { get; private set; }
    public virtual double minActionBarHeight { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? dividerColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? leadingPadding { get; private set; }
    public virtual bool forceActionsBelow { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.OverflowBarAlignment overflowAlignment { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double>? animation { get; private set; }
    public virtual global::System.Action? onVisible { get; private set; }

    public MaterialBanner(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget content = default!, global::Doroti.Framework.Painting.TextStyle? contentTextStyle = null, List<global::Doroti.Framework.Widgets.Widget> actions = default!, double? elevation = null, global::Doroti.Framework.Widgets.Widget? leading = null, Color? backgroundColor = null, Color? surfaceTintColor = null, Color? shadowColor = null, Color? dividerColor = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? leadingPadding = null, bool forceActionsBelow = false, global::Doroti.Framework.Widgets.OverflowBarAlignment overflowAlignment = global::Doroti.Framework.Widgets.OverflowBarAlignment.end, global::Doroti.Framework.Animation.Animation<double>? animation = null, global::System.Action? onVisible = null, double minActionBarHeight = 52.0) : base(key: key)
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
        System.Diagnostics.Debug.Assert(((elevation is null) || (elevation >= 0.0)));
    }

    public static global::Doroti.Framework.Animation.AnimationController createAnimationController(global::Doroti.Framework.Scheduler.TickerProvider vsync)
    {
        return new global::Doroti.Framework.Animation.AnimationController(duration: BannerLibrary._materialBannerTransitionDuration, debugLabel: "MaterialBanner", vsync: vsync);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual MaterialBanner withAnimation(global::Doroti.Framework.Animation.Animation<double> newAnimation, global::Doroti.Framework.Foundation.Key? fallbackKey = null)
    {
        return new MaterialBanner(key: (this.key ?? fallbackKey), content: this.content, contentTextStyle: this.contentTextStyle, actions: this.actions, elevation: this.elevation, leading: this.leading, minActionBarHeight: this.minActionBarHeight, backgroundColor: this.backgroundColor, surfaceTintColor: this.surfaceTintColor, shadowColor: this.shadowColor, dividerColor: this.dividerColor, padding: this.padding, margin: this.margin, leadingPadding: this.leadingPadding, forceActionsBelow: this.forceActionsBelow, overflowAlignment: this.overflowAlignment, animation: newAnimation, onVisible: this.onVisible);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _MaterialBannerState__banner());
}

internal class _MaterialBannerState__banner : global::Doroti.Framework.Widgets.State<MaterialBanner>
{
    internal virtual bool _wasVisible { get; set; } = false;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _heightAnimation { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _slideOutCurvedAnimation { get; set; } = default;

    public override void initState()
    {
        base.initState();
        ((MaterialBanner)this.widget).animation?.addStatusListener((AnimationStatusListener)this._onAnimationStatusChanged);
        _setCurvedAnimations();
    }

    public override void didUpdateWidget(MaterialBanner oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((MaterialBanner)this.widget).animation, ((MaterialBanner)oldWidget).animation)))
        {
            ((MaterialBanner)oldWidget).animation?.removeStatusListener((AnimationStatusListener)this._onAnimationStatusChanged);
            ((MaterialBanner)this.widget).animation?.addStatusListener((AnimationStatusListener)this._onAnimationStatusChanged);
            _setCurvedAnimations();
        }
    }

    internal virtual void _setCurvedAnimations()
    {
        this._heightAnimation?.dispose();
        this._slideOutCurvedAnimation?.dispose();
        if ((((MaterialBanner)this.widget).animation is not null))
        {
            _heightAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((MaterialBanner)this.widget).animation!, curve: BannerLibrary._materialBannerHeightCurve);
            _slideOutCurvedAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((MaterialBanner)this.widget).animation!, curve: new global::Doroti.Framework.Animation.Threshold(0.0));
        }
        else
        {
            _heightAnimation = null;
            _slideOutCurvedAnimation = null;
        }
    }

    public override void dispose()
    {
        ((MaterialBanner)this.widget).animation?.removeStatusListener((AnimationStatusListener)this._onAnimationStatusChanged);
        this._heightAnimation?.dispose();
        this._slideOutCurvedAnimation?.dispose();
        base.dispose();
    }

    internal virtual void _onAnimationStatusChanged(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        if (global::Doroti.Framework.Animation.AnimationStatusMembers.isCompleted(status))
        {
            if (((((MaterialBanner)this.widget).onVisible is not null) && !this._wasVisible))
            {
                ((MaterialBanner)this.widget).onVisible!();
            }
            _wasVisible = true;
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        bool accessibleNavigation = MediaQuery.accessibleNavigationOf(context);
        DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(((MaterialBanner)this.widget).actions));
        ThemeData theme = Theme.of(context);
        MaterialBannerThemeData bannerTheme = MaterialBannerTheme.of(context);
        MaterialBannerThemeData defaults = (theme.useMaterial3 ? new _BannerDefaultsM3__banner(context) : new _BannerDefaultsM2__banner(context));
        bool isSingleRow = ((checked((long)(((MaterialBanner)this.widget).actions.Count)) == 1L) && !((MaterialBanner)this.widget).forceActionsBelow);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry paddingLocal = ((((MaterialBanner)this.widget).padding ?? bannerTheme.padding) ?? ((isSingleRow ? global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 16.0, top: 2.0) : global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 16.0, top: 24.0, end: 16.0, bottom: 4.0))));
        global::Doroti.Framework.Painting.EdgeInsetsGeometry leadingPaddingLocal = ((((MaterialBanner)this.widget).leadingPadding ?? bannerTheme.leadingPadding) ?? global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(end: 16.0));
        global::Doroti.Framework.Widgets.Widget actionsBar = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minHeight: ((MaterialBanner)this.widget).minActionBarHeight), child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 8), child: new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.AlignmentDirectional.centerEnd, child: new global::Doroti.Framework.Widgets.OverflowBar(overflowAlignment: ((MaterialBanner)this.widget).overflowAlignment, spacing: 8, children: ((MaterialBanner)this.widget).actions)))));
        double elevationLocal = ((((MaterialBanner)this.widget).elevation ?? bannerTheme.elevation) ?? 0.0);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry marginLocal = (((MaterialBanner)this.widget).margin ?? global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(bottom: ((elevationLocal > 0L) ? 10.0 : 0.0)));
        global::Doroti.Ui.Color backgroundColorLocal = ((global::Doroti.Ui.Color)(object?)((((MaterialBanner)this.widget).backgroundColor ?? bannerTheme.backgroundColor) ?? defaults.backgroundColor!));
        global::Doroti.Ui.Color? surfaceTintColorLocal = ((global::Doroti.Ui.Color?)(object?)((((MaterialBanner)this.widget).surfaceTintColor ?? bannerTheme.surfaceTintColor) ?? defaults.surfaceTintColor));
        global::Doroti.Ui.Color? shadowColorLocal = ((global::Doroti.Ui.Color?)(object?)(((MaterialBanner)this.widget).shadowColor ?? bannerTheme.shadowColor));
        global::Doroti.Ui.Color? dividerColorLocal = ((global::Doroti.Ui.Color?)(object?)((((MaterialBanner)this.widget).dividerColor ?? bannerTheme.dividerColor) ?? defaults.dividerColor));
        global::Doroti.Framework.Painting.TextStyle? textStyle = ((((MaterialBanner)this.widget).contentTextStyle ?? bannerTheme.contentTextStyle) ?? defaults.contentTextStyle);
        global::Doroti.Framework.Widgets.Widget materialBanner = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: marginLocal, child: new Material(elevation: elevationLocal, color: backgroundColorLocal, surfaceTintColor: surfaceTintColorLocal, shadowColor: shadowColorLocal, child: new global::Doroti.Framework.Widgets.Column(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection14138 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection14138.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: paddingLocal, child: new global::Doroti.Framework.Widgets.Row(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection14253 = new List<global::Doroti.Framework.Widgets.Widget>(); if ((((MaterialBanner)this.widget).leading is not null)) { __collection14253.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: leadingPaddingLocal, child: ((MaterialBanner)this.widget).leading))); } __collection14253.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(MediaQuery.withClampedTextScaling(maxScaleFactor: BannerLibrary._kMaxContentTextScaleFactor, child: new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: textStyle!, child: ((MaterialBanner)this.widget).content))))); if (isSingleRow) { __collection14253.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(MediaQuery.withClampedTextScaling(maxScaleFactor: BannerLibrary._kMaxContentTextScaleFactor, child: actionsBar))); } return __collection14253; }))())))); if (!isSingleRow) { __collection14138.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(actionsBar)); } if ((elevationLocal == 0L)) { __collection14138.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new Divider(height: 0, color: dividerColorLocal))); } return __collection14138; }))()))));
        if ((((MaterialBanner)this.widget).animation is null))
        {
            return materialBanner;
        }
        materialBanner = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SafeArea(child: materialBanner));
        global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Offset> slideOutAnimation = ((global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Offset>)(object?)new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: new global::Doroti.Ui.Offset(0.0, -1.0), end: Offset.zero).animate(this._slideOutCurvedAnimation!));
        materialBanner = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(container: true, liveRegion: true, onDismiss: ((global::System.Action)(() =>
        {
            ScaffoldMessenger.of(context).removeCurrentMaterialBanner(reason: MaterialBannerClosedReason.dismiss);
        })), child: (accessibleNavigation ? materialBanner : new global::Doroti.Framework.Widgets.SlideTransition(position: slideOutAnimation, child: materialBanner))));
        global::Doroti.Framework.Widgets.Widget materialBannerTransition = default!;
        if (accessibleNavigation)
        {
            materialBannerTransition = materialBanner;
        }
        else
        {
            materialBannerTransition = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: this._heightAnimation!, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((context, child) =>
            {
                return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.AlignmentDirectional.bottomStart, heightFactor: this._heightAnimation!.value, child: child));
                throw new InvalidOperationException("Dart closure completed without a value.");
            })), child: materialBanner));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Hero(tag: $"<MaterialBanner Hero tag - {((MaterialBanner)this.widget).content}>", child: new global::Doroti.Framework.Widgets.ClipRect(child: materialBannerTransition)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _BannerDefaultsM2__banner : MaterialBannerThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    internal virtual ThemeData _theme { get; private set; } = default!;

    internal _BannerDefaultsM2__banner(global::Doroti.Framework.Widgets.BuildContext context) : base(elevation: 0.0)
    {
        this.context = context;
        this._theme = Theme.of(context);
    }

    public virtual global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._theme.colorScheme.surface);
    public override global::Doroti.Framework.Painting.TextStyle? contentTextStyle => this._theme.textTheme.bodyMedium;
}

internal class _BannerDefaultsM3__banner : MaterialBannerThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = Theme.of(this.context).colorScheme;
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
                __late__textTheme = Theme.of(this.context).textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }

    internal _BannerDefaultsM3__banner(global::Doroti.Framework.Widgets.BuildContext context) : base(elevation: 1.0)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.surfaceContainerLow);
    public virtual global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public virtual global::Doroti.Ui.Color? dividerColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.outlineVariant);
    public override global::Doroti.Framework.Painting.TextStyle? contentTextStyle => this._textTheme.bodyMedium;
}
