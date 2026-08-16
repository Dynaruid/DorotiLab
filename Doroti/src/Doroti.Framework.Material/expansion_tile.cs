// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/expansion_tile.dart
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

public static partial class Expansion_tileLibrary
{
    internal static Duration _kExpand = Duration.Create(milliseconds: 200L);
}

public delegate void ExpansionTileController();

public class ExpansionTile : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget? leading { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget title { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? subtitle { get; private set; }
    public virtual global::System.Action<bool>? onExpansionChanged { get; private set; }
    public virtual List<global::Doroti.Framework.Widgets.Widget> children { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? collapsedBackgroundColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? trailing { get; private set; }
    public virtual bool showTrailingIcon { get; private set; } = default!;
    public virtual bool initiallyExpanded { get; private set; } = default!;
    public virtual bool maintainState { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? tilePadding { get; private set; }
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry? expandedAlignment { get; private set; }
    public virtual global::Doroti.Framework.Rendering.CrossAxisAlignment? expandedCrossAxisAlignment { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? childrenPadding { get; private set; }
    public virtual Color? iconColor { get; private set; }
    public virtual Color? collapsedIconColor { get; private set; }
    public virtual Color? textColor { get; private set; }
    public virtual Color? collapsedTextColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? collapsedShape { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual ListTileControlAffinity? controlAffinity { get; private set; }
    public virtual global::Doroti.Framework.Widgets.ExpansibleController? controller { get; private set; }
    public virtual bool? dense { get; private set; }
    public virtual Color? splashColor { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual double? minTileHeight { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual bool enabled { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.AnimationStyle? expansionAnimationStyle { get; private set; }
    public virtual bool internalAddSemanticForOnTap { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.WidgetStatesController? statesController { get; private set; }

    public ExpansionTile(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget? leading = null, global::Doroti.Framework.Widgets.Widget title = default!, global::Doroti.Framework.Widgets.Widget? subtitle = null, global::System.Action<bool>? onExpansionChanged = null, List<global::Doroti.Framework.Widgets.Widget> children = default!, global::Doroti.Framework.Widgets.Widget? trailing = null, bool showTrailingIcon = true, bool initiallyExpanded = false, bool maintainState = false, global::Doroti.Framework.Painting.EdgeInsetsGeometry? tilePadding = null, global::Doroti.Framework.Rendering.CrossAxisAlignment? expandedCrossAxisAlignment = null, global::Doroti.Framework.Painting.AlignmentGeometry? expandedAlignment = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? childrenPadding = null, Color? backgroundColor = null, Color? collapsedBackgroundColor = null, Color? textColor = null, Color? collapsedTextColor = null, Color? iconColor = null, Color? collapsedIconColor = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Framework.Painting.ShapeBorder? collapsedShape = null, Clip? clipBehavior = null, ListTileControlAffinity? controlAffinity = null, global::Doroti.Framework.Widgets.ExpansibleController? controller = null, bool? dense = null, Color? splashColor = null, VisualDensity? visualDensity = null, double? minTileHeight = null, bool? enableFeedback = true, bool enabled = true, global::Doroti.Framework.Animation.AnimationStyle? expansionAnimationStyle = null, bool internalAddSemanticForOnTap = false, global::Doroti.Framework.Widgets.WidgetStatesController? statesController = null) : base(key: key)
    {
        List<global::Doroti.Framework.Widgets.Widget> __children = children ?? new List<global::Doroti.Framework.Widgets.Widget>();
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
        System.Diagnostics.Debug.Assert((!object.Equals(expandedCrossAxisAlignment, global::Doroti.Framework.Rendering.CrossAxisAlignment.baseline)));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ExpansionTileState__expansion_tile());
}

internal class _ExpansionTileState__expansion_tile : global::Doroti.Framework.Widgets.State<ExpansionTile>
{
    internal static global::Doroti.Framework.Animation.Animatable<double> _easeInTween = ((global::Doroti.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Framework.Animation.CurveTween(curve: global::Doroti.Framework.Animation.Curves.easeIn));
    internal static global::Doroti.Framework.Animation.Animatable<double> _easeOutTween = ((global::Doroti.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Framework.Animation.CurveTween(curve: global::Doroti.Framework.Animation.Curves.easeOut));
    internal static global::Doroti.Framework.Animation.Animatable<double> _halfTween = ((global::Doroti.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: 0.5));
    internal virtual ShapeBorderTween _borderTween { get; private set; } = new ShapeBorderTween();
    internal virtual global::Doroti.Framework.Animation.ColorTween _headerColorTween { get; private set; } = new global::Doroti.Framework.Animation.ColorTween();
    internal virtual global::Doroti.Framework.Animation.ColorTween _iconColorTween { get; private set; } = new global::Doroti.Framework.Animation.ColorTween();
    internal virtual global::Doroti.Framework.Animation.ColorTween _backgroundColorTween { get; private set; } = new global::Doroti.Framework.Animation.ColorTween();
    internal virtual global::Doroti.Framework.Animation.Animation<double> _iconTurns { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Painting.ShapeBorder?> _border { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<Color?> _headerColor { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<Color?> _iconColor { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<Color?> _backgroundColor { get; set; } = default!;
    internal virtual ExpansionTileThemeData _expansionTileTheme { get; set; } = default!;
    internal virtual global::Doroti.Framework.Widgets.ExpansibleController _tileController { get; set; } = default!;
    internal virtual Timer? _timer { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.Curve _curve { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Curve? _reverseCurve { get; set; } = default;
    internal virtual Duration _duration { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        _curve = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Curve>(global::Doroti.Framework.Animation.Curves.easeIn);
        _duration = Expansion_tileLibrary._kExpand;
        _tileController = (((ExpansionTile)this.widget).controller ?? new global::Doroti.Framework.Widgets.ExpansibleController());
        if (((ExpansionTile)this.widget).initiallyExpanded)
        {
            this._tileController.expand();
        }
        this._tileController.addListener(() => this._onExpansionChanged());
    }

    public override void dispose()
    {
        this._tileController.removeListener(() => this._onExpansionChanged());
        if ((((ExpansionTile)this.widget).controller is null))
        {
            this._tileController.dispose();
        }
        this._timer?.cancel();
        _timer = null;
        base.dispose();
    }

    internal virtual void _onExpansionChanged()
    {
        global::Doroti.Ui.TextDirection textDirection__21799 = ((TextDirection)((dynamic)WidgetsLocalizations.of(this.context)).textDirection);
        MaterialLocalizations localizations__21895 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(this.context));
        string stateHint__21963 = (((global::Doroti.Framework.Widgets.ExpansibleController)this._tileController).isExpanded ? ((MaterialLocalizations)localizations__21895).collapsedHint : ((MaterialLocalizations)localizations__21895).expandedHint);
        if ((object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS)))
        {
            this._timer?.cancel();
            _timer = new Timer(Duration.Create(seconds: 1L), (() => {
DartRuntimePrimitives.Ignore(SemanticsService.sendAnnouncement(View.of(this.context), stateHint__21963, textDirection__21799).catchError(((global::System.Action<object, global::System.Diagnostics.StackTrace>)((exception, stack) => {
FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception, stack: stack, library: "material library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while sending semantics announcement")));
}))));
this._timer?.cancel();
_timer = null;
}));
        }
        else
        {
            if ((!object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.android)))
            {
                DartRuntimePrimitives.Ignore(SemanticsService.sendAnnouncement(View.of(this.context), stateHint__21963, textDirection__21799).catchError(((global::System.Action<object, global::System.Diagnostics.StackTrace>)((exception, stack) => {
FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception, stack: stack, library: "material library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while sending semantics announcement")));
}))));
            }
        }
        ((ExpansionTile)this.widget).onExpansionChanged?.Invoke(((global::Doroti.Framework.Widgets.ExpansibleController)this._tileController).isExpanded);
    }

    internal virtual ListTileControlAffinity _effectiveAffinity()
    {
        ListTileThemeData listTileTheme__23751 = ListTileTheme.of(this.context);
        ListTileControlAffinity affinity__23828 = ((((ExpansionTile)this.widget).controlAffinity ?? listTileTheme__23751.controlAffinity) ?? ListTileControlAffinity.trailing);
        switch (affinity__23828)
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

    internal virtual global::Doroti.Framework.Widgets.Widget? _buildIcon(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation)
    {
        _iconTurns = animation.drive(_halfTween.chain(_easeInTween));
        return ((global::Doroti.Framework.Widgets.Widget?)(object?)new global::Doroti.Framework.Widgets.RotationTransition(turns: this._iconTurns, child: new global::Doroti.Framework.Widgets.Icon(Icons.expand_more)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget? _buildLeadingIcon(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation)
    {
        if ((!object.Equals(_effectiveAffinity(), ListTileControlAffinity.leading)))
        {
            return null;
        }
        return ((global::Doroti.Framework.Widgets.Widget?)(object?)_buildIcon(context, animation));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget? _buildTrailingIcon(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation)
    {
        if ((!object.Equals(_effectiveAffinity(), ListTileControlAffinity.trailing)))
        {
            return null;
        }
        return ((global::Doroti.Framework.Widgets.Widget?)(object?)_buildIcon(context, animation));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildHeader(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation)
    {
        _iconColor = animation.drive(this._iconColorTween.chain(_easeInTween));
        _headerColor = animation.drive(this._headerColorTween.chain(_easeInTween));
        MaterialLocalizations localizations__25136 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        string onTapHint__25204 = (((global::Doroti.Framework.Widgets.ExpansibleController)this._tileController).isExpanded ? ((MaterialLocalizations)localizations__25136).expansionTileExpandedTapHint : ((MaterialLocalizations)localizations__25136).expansionTileCollapsedTapHint);
        string semanticsHint__25368 = (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform switch { global::Doroti.Framework.Foundation.TargetPlatform.iOS => (((global::Doroti.Framework.Widgets.ExpansibleController)this._tileController).isExpanded ? $"{((MaterialLocalizations)localizations__25136).collapsedHint}\n {((MaterialLocalizations)localizations__25136).expansionTileExpandedHint}" : $"{((MaterialLocalizations)localizations__25136).expandedHint}\n {((MaterialLocalizations)localizations__25136).expansionTileCollapsedHint}"), global::Doroti.Framework.Foundation.TargetPlatform.macOS => (((global::Doroti.Framework.Widgets.ExpansibleController)this._tileController).isExpanded ? $"{((MaterialLocalizations)localizations__25136).collapsedHint}\n {((MaterialLocalizations)localizations__25136).expansionTileExpandedHint}" : $"{((MaterialLocalizations)localizations__25136).expandedHint}\n {((MaterialLocalizations)localizations__25136).expansionTileCollapsedHint}"), _ => (((global::Doroti.Framework.Widgets.ExpansibleController)this._tileController).isExpanded ? ((MaterialLocalizations)localizations__25136).collapsedHint : ((MaterialLocalizations)localizations__25136).expandedHint) });
        global::Doroti.Framework.Widgets.Widget child__25812 = ListTileTheme.merge(iconColor: (((global::Doroti.Framework.Animation.Animation<Color?>)this._iconColor).value ?? this._expansionTileTheme.iconColor), textColor: ((global::Doroti.Framework.Animation.Animation<Color?>)this._headerColor).value, child: new ListTile(enabled: ((ExpansionTile)this.widget).enabled, onTap: ((global::System.Action)(((global::Doroti.Framework.Widgets.ExpansibleController)this._tileController).isExpanded ? ((global::Doroti.Framework.Widgets.ExpansibleController)this._tileController).collapse : ((global::Doroti.Framework.Widgets.ExpansibleController)this._tileController).expand)), dense: ((ExpansionTile)this.widget).dense, splashColor: ((ExpansionTile)this.widget).splashColor, visualDensity: ((ExpansionTile)this.widget).visualDensity, enableFeedback: ((ExpansionTile)this.widget).enableFeedback, contentPadding: (((ExpansionTile)this.widget).tilePadding ?? this._expansionTileTheme.tilePadding), leading: ((((ExpansionTile)this.widget).leading ?? (global::Doroti.Framework.Widgets.Widget)_buildLeadingIcon(context, animation))), title: ((ExpansionTile)this.widget).title, subtitle: ((ExpansionTile)this.widget).subtitle, trailing: (((ExpansionTile)this.widget).showTrailingIcon ? ((((ExpansionTile)this.widget).trailing ?? (global::Doroti.Framework.Widgets.Widget)_buildTrailingIcon(context, animation))) : null), minTileHeight: ((ExpansionTile)this.widget).minTileHeight, internalAddSemanticForOnTap: ((ExpansionTile)this.widget).internalAddSemanticForOnTap, statesController: ((ExpansionTile)this.widget).statesController));
        if ((object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.android)))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(label: semanticsHint__25368, liveRegion: true, accessibilityFocusBlockType: global::Doroti.Framework.Semantics.AccessibilityFocusBlockType.blockNode, child: new global::Doroti.Framework.Widgets.Semantics(hint: semanticsHint__25368, onTapHint: onTapHint__25204, child: child__25812)));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(hint: semanticsHint__25368, onTapHint: onTapHint__25204, child: child__25812));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildBody(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Align(alignment: ((((ExpansionTile)this.widget).expandedAlignment ?? this._expansionTileTheme.expandedAlignment) ?? global::Doroti.Framework.Painting.Alignment.center), child: new global::Doroti.Framework.Widgets.Padding(padding: ((((ExpansionTile)this.widget).childrenPadding ?? this._expansionTileTheme.childrenPadding) ?? global::Doroti.Framework.Painting.EdgeInsets.zero), child: new global::Doroti.Framework.Widgets.Column(crossAxisAlignment: (((ExpansionTile)this.widget).expandedCrossAxisAlignment ?? global::Doroti.Framework.Rendering.CrossAxisAlignment.center), children: ((ExpansionTile)this.widget).children))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildExpansible(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget header, global::Doroti.Framework.Widgets.Widget body, global::Doroti.Framework.Animation.Animation<double> animation)
    {
        _backgroundColor = animation.drive(this._backgroundColorTween.chain(_easeOutTween));
        _border = animation.drive(this._borderTween.chain(_easeOutTween));
        global::Doroti.Ui.Color backgroundColor__28200 = ((global::Doroti.Ui.Color)(object?)((((global::Doroti.Framework.Animation.Animation<Color?>)this._backgroundColor).value ?? this._expansionTileTheme.backgroundColor) ?? Colors.transparent));
        global::Doroti.Framework.Painting.ShapeBorder expansionTileBorder__28333 = (((global::Doroti.Framework.Animation.Animation<global::Doroti.Framework.Painting.ShapeBorder?>)this._border).value ?? new global::Doroti.Framework.Painting.Border(top: new global::Doroti.Framework.Painting.BorderSide(color: Colors.transparent), bottom: new global::Doroti.Framework.Painting.BorderSide(color: Colors.transparent)));
        global::Doroti.Ui.Clip clipBehavior__28539 = ((((ExpansionTile)this.widget).clipBehavior ?? this._expansionTileTheme.clipBehavior) ?? Clip.antiAlias);
        global::Doroti.Framework.Painting.Decoration decoration__28659 = ((global::Doroti.Framework.Painting.Decoration)(object?)new global::Doroti.Framework.Painting.ShapeDecoration(color: backgroundColor__28200, shape: expansionTileBorder__28333));
        global::Doroti.Framework.Widgets.Widget tile__28772 = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: ((global::Doroti.Framework.Painting.Decoration)decoration__28659).padding, child: new global::Doroti.Framework.Widgets.Column(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(header), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(body) })));
        bool isShapeProvided__28933 = ((((((ExpansionTile)this.widget).shape is not null) || (this._expansionTileTheme.shape is not null)) || (((ExpansionTile)this.widget).collapsedShape is not null)) || (this._expansionTileTheme.collapsedShape is not null));
        if (isShapeProvided__28933)
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new Material(clipBehavior: clipBehavior__28539, color: backgroundColor__28200, shape: expansionTileBorder__28333, child: tile__28772));
        }
        if ((backgroundColor__28200.a > 0L))
        {
            tile__28772 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new Material(type: MaterialType.transparency, child: tile__28772));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.DecoratedBox(decoration: decoration__28659, child: tile__28772));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void didUpdateWidget(ExpansionTile oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        ThemeData theme__29924 = Theme.of(this.context);
        _expansionTileTheme = ExpansionTileTheme.of(this.context);
        ExpansionTileThemeData defaults__30042 = (theme__29924.useMaterial3 ? new _ExpansionTileDefaultsM3__expansion_tile(this.context) : new _ExpansionTileDefaultsM2__expansion_tile(this.context));
        if (((!object.Equals(((ExpansionTile)this.widget).collapsedShape, ((ExpansionTile)oldWidget).collapsedShape)) || (!object.Equals(((ExpansionTile)this.widget).shape, ((ExpansionTile)oldWidget).shape))))
        {
            _updateShapeBorder(theme__29924);
        }
        if (((!object.Equals(((ExpansionTile)this.widget).collapsedTextColor, ((ExpansionTile)oldWidget).collapsedTextColor)) || (!object.Equals(((ExpansionTile)this.widget).textColor, ((ExpansionTile)oldWidget).textColor))))
        {
            _updateHeaderColor(defaults__30042);
        }
        if (((!object.Equals(((ExpansionTile)this.widget).collapsedIconColor, ((ExpansionTile)oldWidget).collapsedIconColor)) || (!object.Equals(((ExpansionTile)this.widget).iconColor, ((ExpansionTile)oldWidget).iconColor))))
        {
            _updateIconColor(defaults__30042);
        }
        if (((!object.Equals(((ExpansionTile)this.widget).backgroundColor, ((ExpansionTile)oldWidget).backgroundColor)) || (!object.Equals(((ExpansionTile)this.widget).collapsedBackgroundColor, ((ExpansionTile)oldWidget).collapsedBackgroundColor))))
        {
            _updateBackgroundColor();
        }
        if ((!object.Equals(((ExpansionTile)this.widget).expansionAnimationStyle, ((ExpansionTile)oldWidget).expansionAnimationStyle)))
        {
            _updateAnimationDuration();
            _updateHeightFactorCurve();
        }
        if ((!object.Equals(((ExpansionTile)this.widget).controller, ((ExpansionTile)oldWidget).controller)))
        {
            this._tileController.removeListener(() => this._onExpansionChanged());
            if ((((ExpansionTile)oldWidget).controller is null))
            {
                this._tileController.dispose();
            }
            _tileController = (((ExpansionTile)this.widget).controller ?? new global::Doroti.Framework.Widgets.ExpansibleController());
            this._tileController.addListener(() => this._onExpansionChanged());
        }
    }

    public override void didChangeDependencies()
    {
        ThemeData theme__31352 = Theme.of(this.context);
        _expansionTileTheme = ExpansionTileTheme.of(this.context);
        ExpansionTileThemeData defaults__31470 = (theme__31352.useMaterial3 ? new _ExpansionTileDefaultsM3__expansion_tile(this.context) : new _ExpansionTileDefaultsM2__expansion_tile(this.context));
        _updateAnimationDuration();
        _updateShapeBorder(theme__31352);
        _updateHeaderColor(defaults__31470);
        _updateIconColor(defaults__31470);
        _updateBackgroundColor();
        _updateHeightFactorCurve();
        base.didChangeDependencies();
    }

    internal virtual void _updateAnimationDuration()
    {
        _duration = ((((ExpansionTile)this.widget).expansionAnimationStyle?.duration ?? this._expansionTileTheme.expansionAnimationStyle?.duration) ?? Duration.Create(milliseconds: 200L));
    }

    internal virtual void _updateShapeBorder(ThemeData theme)
    {
        DartRuntimePrimitives.Ignore(((Func<ShapeBorderTween>)(() =>
{            var __cascade = this._borderTween;
            __cascade.begin = ((((ExpansionTile)this.widget).collapsedShape ?? this._expansionTileTheme.collapsedShape) ?? new global::Doroti.Framework.Painting.Border(top: new global::Doroti.Framework.Painting.BorderSide(color: Colors.transparent), bottom: new global::Doroti.Framework.Painting.BorderSide(color: Colors.transparent)));
            __cascade.end = ((((ExpansionTile)this.widget).shape ?? this._expansionTileTheme.shape) ?? new global::Doroti.Framework.Painting.Border(top: new global::Doroti.Framework.Painting.BorderSide(color: theme.dividerColor), bottom: new global::Doroti.Framework.Painting.BorderSide(color: theme.dividerColor)));
            return __cascade;        }))());
    }

    internal virtual void _updateHeaderColor(ExpansionTileThemeData defaults)
    {
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.ColorTween>)(() =>
{            var __cascade = this._headerColorTween;
            __cascade.begin = ((((ExpansionTile)this.widget).collapsedTextColor ?? this._expansionTileTheme.collapsedTextColor) ?? defaults.collapsedTextColor);
            __cascade.end = ((((ExpansionTile)this.widget).textColor ?? this._expansionTileTheme.textColor) ?? defaults.textColor);
            return __cascade;        }))());
    }

    internal virtual void _updateIconColor(ExpansionTileThemeData defaults)
    {
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.ColorTween>)(() =>
{            var __cascade = this._iconColorTween;
            __cascade.begin = ((((ExpansionTile)this.widget).collapsedIconColor ?? this._expansionTileTheme.collapsedIconColor) ?? defaults.collapsedIconColor);
            __cascade.end = ((((ExpansionTile)this.widget).iconColor ?? this._expansionTileTheme.iconColor) ?? defaults.iconColor);
            return __cascade;        }))());
    }

    internal virtual void _updateBackgroundColor()
    {
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.ColorTween>)(() =>
{            var __cascade = this._backgroundColorTween;
            __cascade.begin = (((ExpansionTile)this.widget).collapsedBackgroundColor ?? this._expansionTileTheme.collapsedBackgroundColor);
            __cascade.end = (((ExpansionTile)this.widget).backgroundColor ?? this._expansionTileTheme.backgroundColor);
            return __cascade;        }))());
    }

    internal virtual void _updateHeightFactorCurve()
    {
        _curve = ((((ExpansionTile)this.widget).expansionAnimationStyle?.curve ?? this._expansionTileTheme.expansionAnimationStyle?.curve) ?? global::Doroti.Framework.Animation.Curves.easeIn);
        _reverseCurve = (((ExpansionTile)this.widget).expansionAnimationStyle?.reverseCurve ?? this._expansionTileTheme.expansionAnimationStyle?.reverseCurve);
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Expansible(controller: this._tileController, curve: this._curve, duration: this._duration, reverseCurve: this._reverseCurve, maintainState: ((ExpansionTile)this.widget).maintainState, headerBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget>)this._buildHeader, bodyBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget>)this._buildBody, expansibleBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget>)this._buildExpansible));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ExpansionTileDefaultsM2__expansion_tile : ExpansionTileThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__theme_initialized;
    private ThemeData __late__theme = default!;
    internal virtual ThemeData _theme
    {
        get
        {
            if (!__late__theme_initialized)
            {
                __late__theme = Theme.of(this.context);
                __late__theme_initialized = true;
            }
            return __late__theme;
        }
    }
    private bool __late__colorScheme_initialized;
    private ColorScheme __late__colorScheme = default!;
    internal virtual ColorScheme _colorScheme
    {
        get
        {
            if (!__late__colorScheme_initialized)
            {
                __late__colorScheme = this._theme.colorScheme;
                __late__colorScheme_initialized = true;
            }
            return __late__colorScheme;
        }
    }

    internal _ExpansionTileDefaultsM2__expansion_tile(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? textColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colorScheme.primary);
    public virtual global::Doroti.Ui.Color? iconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colorScheme.primary);
    public virtual global::Doroti.Ui.Color? collapsedTextColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._theme.textTheme.titleMedium!.color);
    public virtual global::Doroti.Ui.Color? collapsedIconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._theme.unselectedWidgetColor);
}

internal class _ExpansionTileDefaultsM3__expansion_tile : ExpansionTileThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__theme_initialized;
    private ThemeData __late__theme = default!;
    internal virtual ThemeData _theme
    {
        get
        {
            if (!__late__theme_initialized)
            {
                __late__theme = Theme.of(this.context);
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
                __late__colors = this._theme.colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _ExpansionTileDefaultsM3__expansion_tile(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? textColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface);
    public virtual global::Doroti.Ui.Color? iconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary);
    public virtual global::Doroti.Ui.Color? collapsedTextColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface);
    public virtual global::Doroti.Ui.Color? collapsedIconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurfaceVariant);
}
