// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/dialog.dart
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

public static partial class DialogLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsets _defaultInsetPadding = global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 40.0, vertical: 24.0);
}

public class Dialog : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual Duration insetAnimationDuration { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Curve insetAnimationCurve { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsets? insetPadding { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry? alignment { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }
    internal virtual bool _fullscreen { get; private set; } = default!;
    public virtual SemanticsRole semanticsRole { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? constraints { get; private set; }

    public Dialog(global::Doroti.Framework.Foundation.Key? key = null, Color? backgroundColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, Duration? insetAnimationDuration = null, global::Doroti.Framework.Animation.Curve insetAnimationCurve = default!, global::Doroti.Framework.Painting.EdgeInsets? insetPadding = null, Clip? clipBehavior = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, global::Doroti.Framework.Widgets.Widget? child = null, SemanticsRole semanticsRole = SemanticsRole.dialog, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null) : base(key: key)
    {
        Duration __insetAnimationDuration = insetAnimationDuration ?? Duration.Create(milliseconds: 100);
        global::Doroti.Framework.Animation.Curve __insetAnimationCurve = insetAnimationCurve ?? global::Doroti.Framework.Animation.Curves.decelerate;
        this.backgroundColor = backgroundColor;
        this.elevation = elevation;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.insetAnimationDuration = __insetAnimationDuration;
        this.insetAnimationCurve = __insetAnimationCurve;
        this.insetPadding = insetPadding;
        this.clipBehavior = clipBehavior;
        this.shape = shape;
        this.alignment = alignment;
        this.child = child;
        this.semanticsRole = semanticsRole;
        this.constraints = constraints;
        this._fullscreen = false;
        System.Diagnostics.Debug.Assert(((elevation is null) || (elevation >= 0.0)));
    }

    public static Dialog CreateFullscreen(global::Doroti.Framework.Foundation.Key? key = null, Color? backgroundColor = null, Duration insetAnimationDuration = default, global::Doroti.Framework.Animation.Curve insetAnimationCurve = default!, global::Doroti.Framework.Widgets.Widget? child = null, SemanticsRole semanticsRole = SemanticsRole.dialog)
    {
        var __instance = new Dialog(key: key, backgroundColor: backgroundColor, insetAnimationDuration: insetAnimationDuration, insetAnimationCurve: insetAnimationCurve, child: child, semanticsRole: semanticsRole);
        global::Doroti.Framework.Animation.Curve __insetAnimationCurve = insetAnimationCurve ?? global::Doroti.Framework.Animation.Curves.decelerate;
        __instance.backgroundColor = backgroundColor;
        __instance.insetAnimationDuration = insetAnimationDuration;
        __instance.insetAnimationCurve = __insetAnimationCurve;
        __instance.child = child;
        __instance.semanticsRole = semanticsRole;
        __instance.elevation = 0;
        __instance.shadowColor = null;
        __instance.surfaceTintColor = null;
        __instance.insetPadding = global::Doroti.Framework.Painting.EdgeInsets.zero;
        __instance.clipBehavior = Clip.none;
        __instance.shape = null;
        __instance.alignment = null;
        __instance.constraints = null;
        __instance._fullscreen = true;
        return __instance;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        DialogThemeData dialogTheme = DialogTheme.of(context);
        global::Doroti.Framework.Painting.EdgeInsets effectivePadding = (MediaQuery.viewInsetsOf(context).op_Add((((this.insetPadding ?? dialogTheme.insetPadding) ?? DialogLibrary._defaultInsetPadding))));
        DialogThemeData defaults = (theme.useMaterial3 ? ((this._fullscreen ? new _DialogFullscreenDefaultsM3__dialog(context) : new _DialogDefaultsM3__dialog(context))) : new _DialogDefaultsM2__dialog(context));
        global::Doroti.Framework.Rendering.BoxConstraints boxConstraints = ((this.constraints ?? dialogTheme.constraints) ?? new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: 280.0));
        global::Doroti.Framework.Widgets.Widget dialogChild = default!;
        if (this._fullscreen)
        {
            dialogChild = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new Material(color: ((this.backgroundColor ?? dialogTheme.backgroundColor) ?? defaults.backgroundColor), child: this.child));
        }
        else
        {
            dialogChild = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Align(alignment: ((this.alignment ?? dialogTheme.alignment) ?? defaults.alignment!), child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: boxConstraints, child: new Material(color: ((this.backgroundColor ?? dialogTheme.backgroundColor) ?? defaults.backgroundColor), elevation: ((this.elevation ?? dialogTheme.elevation) ?? DartRuntimePrimitives.RequireValue(defaults.elevation)), shadowColor: ((this.shadowColor ?? dialogTheme.shadowColor) ?? defaults.shadowColor), surfaceTintColor: ((this.surfaceTintColor ?? dialogTheme.surfaceTintColor) ?? defaults.surfaceTintColor), shape: ((this.shape ?? dialogTheme.shape) ?? defaults.shape!), type: MaterialType.card, clipBehavior: ((this.clipBehavior ?? dialogTheme.clipBehavior) ?? DartRuntimePrimitives.RequireValue(defaults.clipBehavior)), child: this.child))));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(role: this.semanticsRole, child: new global::Doroti.Framework.Widgets.AnimatedPadding(padding: effectivePadding, duration: DartRuntimePrimitives.RequireValue(this.insetAnimationDuration), curve: this.insetAnimationCurve, child: global::Doroti.Framework.Widgets.MediaQuery.CreateRemoveViewInsets(removeLeft: true, removeTop: true, removeRight: true, removeBottom: true, context: context, child: dialogChild))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class AlertDialog : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget? icon { get; private set; }
    public virtual Color? iconColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? iconPadding { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? title { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? titlePadding { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? titleTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? content { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? contentTextStyle { get; private set; }
    public virtual List<global::Doroti.Framework.Widgets.Widget>? actions { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? actionsPadding { get; private set; }
    public virtual global::Doroti.Framework.Rendering.MainAxisAlignment? actionsAlignment { get; private set; }
    public virtual global::Doroti.Framework.Widgets.OverflowBarAlignment? actionsOverflowAlignment { get; private set; }
    public virtual global::Doroti.Framework.Painting.VerticalDirection? actionsOverflowDirection { get; private set; }
    public virtual double? actionsOverflowButtonSpacing { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? buttonPadding { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual string? semanticLabel { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsets? insetPadding { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry? alignment { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? constraints { get; private set; }
    public virtual bool scrollable { get; private set; } = default!;

    public AlertDialog(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget? icon = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? iconPadding = null, Color? iconColor = null, global::Doroti.Framework.Widgets.Widget? title = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? titlePadding = null, global::Doroti.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Framework.Widgets.Widget? content = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, global::Doroti.Framework.Painting.TextStyle? contentTextStyle = null, List<global::Doroti.Framework.Widgets.Widget>? actions = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? actionsPadding = null, global::Doroti.Framework.Rendering.MainAxisAlignment? actionsAlignment = null, global::Doroti.Framework.Widgets.OverflowBarAlignment? actionsOverflowAlignment = null, global::Doroti.Framework.Painting.VerticalDirection? actionsOverflowDirection = null, double? actionsOverflowButtonSpacing = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? buttonPadding = null, Color? backgroundColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, string? semanticLabel = null, global::Doroti.Framework.Painting.EdgeInsets? insetPadding = null, Clip? clipBehavior = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, bool scrollable = false) : base(key: key)
    {
        this.icon = icon;
        this.iconPadding = iconPadding;
        this.iconColor = iconColor;
        this.title = title;
        this.titlePadding = titlePadding;
        this.titleTextStyle = titleTextStyle;
        this.content = content;
        this.contentPadding = contentPadding;
        this.contentTextStyle = contentTextStyle;
        this.actions = actions;
        this.actionsPadding = actionsPadding;
        this.actionsAlignment = actionsAlignment;
        this.actionsOverflowAlignment = actionsOverflowAlignment;
        this.actionsOverflowDirection = actionsOverflowDirection;
        this.actionsOverflowButtonSpacing = actionsOverflowButtonSpacing;
        this.buttonPadding = buttonPadding;
        this.backgroundColor = backgroundColor;
        this.elevation = elevation;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.semanticLabel = semanticLabel;
        this.insetPadding = insetPadding;
        this.clipBehavior = clipBehavior;
        this.shape = shape;
        this.alignment = alignment;
        this.constraints = constraints;
        this.scrollable = scrollable;
    }

    public static AlertDialog CreateAdaptive(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget? icon = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? iconPadding = null, Color? iconColor = null, global::Doroti.Framework.Widgets.Widget? title = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? titlePadding = null, global::Doroti.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Framework.Widgets.Widget? content = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, global::Doroti.Framework.Painting.TextStyle? contentTextStyle = null, List<global::Doroti.Framework.Widgets.Widget>? actions = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? actionsPadding = null, global::Doroti.Framework.Rendering.MainAxisAlignment? actionsAlignment = null, global::Doroti.Framework.Widgets.OverflowBarAlignment? actionsOverflowAlignment = null, global::Doroti.Framework.Painting.VerticalDirection? actionsOverflowDirection = null, double? actionsOverflowButtonSpacing = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? buttonPadding = null, Color? backgroundColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, string? semanticLabel = null, global::Doroti.Framework.Painting.EdgeInsets insetPadding = default!, Clip? clipBehavior = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, bool scrollable = default!, global::Doroti.Framework.Widgets.ScrollController? scrollController = null, global::Doroti.Framework.Widgets.ScrollController? actionScrollController = null, Duration insetAnimationDuration = default!, global::Doroti.Framework.Animation.Curve insetAnimationCurve = default!)
        => ((AlertDialog)(object?)new _AdaptiveAlertDialog__dialog(key, icon, iconPadding, iconColor, title, titlePadding, titleTextStyle, content, contentPadding, contentTextStyle, actions, actionsPadding, actionsAlignment, actionsOverflowAlignment, actionsOverflowDirection, actionsOverflowButtonSpacing, buttonPadding, backgroundColor, elevation, shadowColor, surfaceTintColor, semanticLabel, insetPadding, clipBehavior, shape, alignment, constraints, scrollable, scrollController, actionScrollController, insetAnimationDuration, insetAnimationCurve));

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        ThemeData theme = Theme.of(context);
        DialogThemeData dialogTheme = DialogTheme.of(context);
        DialogThemeData defaults = (theme.useMaterial3 ? new _DialogDefaultsM3__dialog(context) : new _DialogDefaultsM2__dialog(context));
        string? labelLocal = (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform switch { global::Doroti.Framework.Foundation.TargetPlatform.iOS => this.semanticLabel, global::Doroti.Framework.Foundation.TargetPlatform.macOS => this.semanticLabel, global::Doroti.Framework.Foundation.TargetPlatform.android or global::Doroti.Framework.Foundation.TargetPlatform.fuchsia or global::Doroti.Framework.Foundation.TargetPlatform.linux => (this.semanticLabel ?? MaterialLocalizations.of(context).alertDialogLabel), global::Doroti.Framework.Foundation.TargetPlatform.windows => (this.semanticLabel ?? MaterialLocalizations.of(context).alertDialogLabel), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        var fontSizeToScale = 14.0;
        double effectiveTextScale = (MediaQuery.textScalerOf(context).scale(fontSizeToScale) / fontSizeToScale);
        double paddingScaleFactor = DialogLibrary._scalePadding(effectiveTextScale);
        global::Doroti.Ui.TextDirection? textDirection = Directionality.maybeOf(context);
        global::Doroti.Framework.Widgets.Widget? iconWidget = default!;
        global::Doroti.Framework.Widgets.Widget? titleWidget = default!;
        global::Doroti.Framework.Widgets.Widget? contentWidget = default!;
        global::Doroti.Framework.Widgets.Widget? actionsWidget = default!;
        if ((this.icon is not null))
        {
            var belowIsTitle = (this.title is not null);
            bool belowIsContent = (!belowIsTitle && (this.content is not null));
            var defaultIconPadding = global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(left: 24.0, top: 24.0, right: 24.0, bottom: (belowIsTitle ? 16.0 : (belowIsContent ? 0.0 : 24.0)));
            global::Doroti.Framework.Painting.EdgeInsets effectiveIconPadding = (this.iconPadding?.resolve(textDirection) ?? defaultIconPadding);
            iconWidget = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(left: (((global::Doroti.Framework.Painting.EdgeInsets)effectiveIconPadding).left * paddingScaleFactor), right: (((global::Doroti.Framework.Painting.EdgeInsets)effectiveIconPadding).right * paddingScaleFactor), top: (((global::Doroti.Framework.Painting.EdgeInsets)effectiveIconPadding).top * paddingScaleFactor), bottom: ((global::Doroti.Framework.Painting.EdgeInsets)effectiveIconPadding).bottom), child: new global::Doroti.Framework.Widgets.IconTheme(data: new global::Doroti.Framework.Widgets.IconThemeData(color: ((this.iconColor ?? dialogTheme.iconColor) ?? defaults.iconColor)), child: this.icon!)));
        }
        if ((this.title is not null))
        {
            var defaultTitlePadding = global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(left: 24.0, top: ((this.icon is null) ? 24.0 : 0.0), right: 24.0, bottom: ((this.content is null) ? 20.0 : 0.0));
            global::Doroti.Framework.Painting.EdgeInsets effectiveTitlePadding = (this.titlePadding?.resolve(textDirection) ?? defaultTitlePadding);
            titleWidget = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(left: (((global::Doroti.Framework.Painting.EdgeInsets)effectiveTitlePadding).left * paddingScaleFactor), right: (((global::Doroti.Framework.Painting.EdgeInsets)effectiveTitlePadding).right * paddingScaleFactor), top: ((this.icon is null) ? (((global::Doroti.Framework.Painting.EdgeInsets)effectiveTitlePadding).top * paddingScaleFactor) : ((global::Doroti.Framework.Painting.EdgeInsets)effectiveTitlePadding).top), bottom: ((global::Doroti.Framework.Painting.EdgeInsets)effectiveTitlePadding).bottom), child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: ((this.titleTextStyle ?? dialogTheme.titleTextStyle) ?? defaults.titleTextStyle!), textAlign: ((this.icon is null) ? global::Doroti.Ui.TextAlign.start : global::Doroti.Ui.TextAlign.center), child: new global::Doroti.Framework.Widgets.Semantics(namesRoute: ((labelLocal is null) && (!object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS))), container: true, child: this.title))));
        }
        if ((this.content is not null))
        {
            var defaultContentPadding = global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(left: 24.0, top: (theme.useMaterial3 ? 16.0 : 20.0), right: 24.0, bottom: 24.0);
            global::Doroti.Framework.Painting.EdgeInsets effectiveContentPadding = (this.contentPadding?.resolve(textDirection) ?? defaultContentPadding);
            contentWidget = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(left: (((global::Doroti.Framework.Painting.EdgeInsets)effectiveContentPadding).left * paddingScaleFactor), right: (((global::Doroti.Framework.Painting.EdgeInsets)effectiveContentPadding).right * paddingScaleFactor), top: (((this.title is null) && (this.icon is null)) ? (((global::Doroti.Framework.Painting.EdgeInsets)effectiveContentPadding).top * paddingScaleFactor) : ((global::Doroti.Framework.Painting.EdgeInsets)effectiveContentPadding).top), bottom: ((global::Doroti.Framework.Painting.EdgeInsets)effectiveContentPadding).bottom), child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: ((this.contentTextStyle ?? dialogTheme.contentTextStyle) ?? defaults.contentTextStyle!), child: new global::Doroti.Framework.Widgets.Semantics(container: true, explicitChildNodes: true, child: this.content))));
        }
        if ((this.actions is not null))
        {
            double spacingLocal = (((this.buttonPadding?.horizontal ?? 16)) / 2L);
            actionsWidget = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: ((this.actionsPadding ?? dialogTheme.actionsPadding) ?? ((theme.useMaterial3 ? defaults.actionsPadding! : defaults.actionsPadding!.add(global::Doroti.Framework.Painting.EdgeInsets.CreateAll(spacingLocal))))), child: new global::Doroti.Framework.Widgets.OverflowBar(alignment: (this.actionsAlignment ?? global::Doroti.Framework.Rendering.MainAxisAlignment.end), spacing: spacingLocal, overflowAlignment: (this.actionsOverflowAlignment ?? global::Doroti.Framework.Widgets.OverflowBarAlignment.end), overflowDirection: (this.actionsOverflowDirection ?? global::Doroti.Framework.Painting.VerticalDirection.down), overflowSpacing: (this.actionsOverflowButtonSpacing ?? 0), children: this.actions!)));
        }
        List<global::Doroti.Framework.Widgets.Widget> columnChildren = default!;
        if (this.scrollable)
        {
            columnChildren = ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection34259 = new List<global::Doroti.Framework.Widgets.Widget>(); if (((this.title is not null) || (this.content is not null))) { __collection34259.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: new global::Doroti.Framework.Widgets.SingleChildScrollView(child: new global::Doroti.Framework.Widgets.Column(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.stretch, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection34544 = new List<global::Doroti.Framework.Widgets.Widget>(); var __collectionElement34553 = iconWidget; if (__collectionElement34553 is { } __nonNullCollectionElement34553) { __collection34544.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement34553)); } var __collectionElement34566 = titleWidget; if (__collectionElement34566 is { } __nonNullCollectionElement34566) { __collection34544.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement34566)); } var __collectionElement34580 = contentWidget; if (__collectionElement34580 is { } __nonNullCollectionElement34580) { __collection34544.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement34580)); } return __collection34544; }))()))))); } var __collectionElement34650 = actionsWidget; if (__collectionElement34650 is { } __nonNullCollectionElement34650) { __collection34259.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement34650)); } return __collection34259; }))();
        }
        else
        {
            columnChildren = ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection34711 = new List<global::Doroti.Framework.Widgets.Widget>(); var __collectionElement34729 = iconWidget; if (__collectionElement34729 is { } __nonNullCollectionElement34729) { __collection34711.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement34729)); } var __collectionElement34750 = titleWidget; if (__collectionElement34750 is { } __nonNullCollectionElement34750) { __collection34711.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement34750)); } if ((contentWidget is not null)) { __collection34711.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: contentWidget))); } var __collectionElement34839 = actionsWidget; if (__collectionElement34839 is { } __nonNullCollectionElement34839) { __collection34711.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement34839)); } return __collection34711; }))();
        }
        global::Doroti.Framework.Widgets.Widget dialogChild = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.IntrinsicWidth(child: new global::Doroti.Framework.Widgets.Column(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.stretch, children: columnChildren)));
        if ((labelLocal is not null))
        {
            dialogChild = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(scopesRoute: true, explicitChildNodes: true, namesRoute: true, label: labelLocal, child: dialogChild));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new Dialog(backgroundColor: this.backgroundColor, elevation: this.elevation, shadowColor: this.shadowColor, surfaceTintColor: this.surfaceTintColor, insetPadding: this.insetPadding, clipBehavior: this.clipBehavior, shape: this.shape, alignment: this.alignment, constraints: this.constraints, semanticsRole: SemanticsRole.alertDialog, child: dialogChild));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AdaptiveAlertDialog__dialog : AlertDialog
{
    public virtual global::Doroti.Framework.Widgets.ScrollController? scrollController { get; private set; }
    public virtual global::Doroti.Framework.Widgets.ScrollController? actionScrollController { get; private set; }
    public virtual Duration insetAnimationDuration { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Curve insetAnimationCurve { get; private set; } = default!;

    internal _AdaptiveAlertDialog__dialog(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget? icon = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? iconPadding = null, Color? iconColor = null, global::Doroti.Framework.Widgets.Widget? title = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? titlePadding = null, global::Doroti.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Framework.Widgets.Widget? content = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, global::Doroti.Framework.Painting.TextStyle? contentTextStyle = null, List<global::Doroti.Framework.Widgets.Widget>? actions = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? actionsPadding = null, global::Doroti.Framework.Rendering.MainAxisAlignment? actionsAlignment = null, global::Doroti.Framework.Widgets.OverflowBarAlignment? actionsOverflowAlignment = null, global::Doroti.Framework.Painting.VerticalDirection? actionsOverflowDirection = null, double? actionsOverflowButtonSpacing = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? buttonPadding = null, Color? backgroundColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, string? semanticLabel = null, global::Doroti.Framework.Painting.EdgeInsets? insetPadding = null, Clip? clipBehavior = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, bool scrollable = false, global::Doroti.Framework.Widgets.ScrollController? scrollController = null, global::Doroti.Framework.Widgets.ScrollController? actionScrollController = null, Duration? insetAnimationDuration = null, global::Doroti.Framework.Animation.Curve insetAnimationCurve = default!) : base(key: key, icon: icon, iconPadding: iconPadding, iconColor: iconColor, title: title, titlePadding: titlePadding, titleTextStyle: titleTextStyle, content: content, contentPadding: contentPadding, contentTextStyle: contentTextStyle, actions: actions, actionsPadding: actionsPadding, actionsAlignment: actionsAlignment, actionsOverflowAlignment: actionsOverflowAlignment, actionsOverflowDirection: actionsOverflowDirection, actionsOverflowButtonSpacing: actionsOverflowButtonSpacing, buttonPadding: buttonPadding, backgroundColor: backgroundColor, elevation: elevation, shadowColor: shadowColor, surfaceTintColor: surfaceTintColor, semanticLabel: semanticLabel, insetPadding: insetPadding, clipBehavior: clipBehavior, shape: shape, alignment: alignment, constraints: constraints, scrollable: scrollable)
    {
        Duration __insetAnimationDuration = insetAnimationDuration ?? Duration.Create(milliseconds: 100);
        global::Doroti.Framework.Animation.Curve __insetAnimationCurve = insetAnimationCurve ?? global::Doroti.Framework.Animation.Curves.decelerate;
        this.scrollController = scrollController;
        this.actionScrollController = actionScrollController;
        this.insetAnimationDuration = __insetAnimationDuration;
        this.insetAnimationCurve = __insetAnimationCurve;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        switch (theme.platform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                {
                    return ((global::Doroti.Framework.Widgets.Widget)(object?)new CupertinoAlertDialog(title: this.title, content: this.content, actions: (this.actions ?? new List<global::Doroti.Framework.Widgets.Widget>()), scrollController: this.scrollController, actionScrollController: this.actionScrollController, insetAnimationDuration: DartRuntimePrimitives.RequireValue(this.insetAnimationDuration), insetAnimationCurve: this.insetAnimationCurve));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)base.build(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SimpleDialogOption : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsets? padding { get; private set; }

    public SimpleDialogOption(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action? onPressed = null, global::Doroti.Framework.Painting.EdgeInsets? padding = null, global::Doroti.Framework.Widgets.Widget? child = null) : base(key: key)
    {
        this.onPressed = onPressed;
        this.padding = padding;
        this.child = child;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new InkWell(onTap: this.onPressed, child: new global::Doroti.Framework.Widgets.Padding(padding: (this.padding ?? global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(vertical: 8.0, horizontal: 24.0)), child: this.child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SimpleDialog : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget? title { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry titlePadding { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle? titleTextStyle { get; private set; }
    public virtual List<global::Doroti.Framework.Widgets.Widget>? children { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry contentPadding { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? contentTextStyle { get; private set; }
    public virtual string? semanticLabel { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsets? insetPadding { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry? alignment { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? constraints { get; private set; }

    public SimpleDialog(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget? title = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry titlePadding = default!, global::Doroti.Framework.Painting.TextStyle? titleTextStyle = null, List<global::Doroti.Framework.Widgets.Widget>? children = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry contentPadding = default!, global::Doroti.Framework.Painting.TextStyle? contentTextStyle = null, Color? backgroundColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, string? semanticLabel = null, global::Doroti.Framework.Painting.EdgeInsets? insetPadding = null, Clip? clipBehavior = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null) : base(key: key)
    {
        global::Doroti.Framework.Painting.EdgeInsetsGeometry __titlePadding = titlePadding ?? new global::Doroti.Framework.Painting.EdgeInsets(24.0, 24.0, 24.0, 0.0);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry __contentPadding = contentPadding ?? new global::Doroti.Framework.Painting.EdgeInsets(0.0, 12.0, 0.0, 16.0);
        this.title = title;
        this.titlePadding = __titlePadding;
        this.titleTextStyle = titleTextStyle;
        this.children = children;
        this.contentPadding = __contentPadding;
        this.contentTextStyle = contentTextStyle;
        this.backgroundColor = backgroundColor;
        this.elevation = elevation;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.semanticLabel = semanticLabel;
        this.insetPadding = insetPadding;
        this.clipBehavior = clipBehavior;
        this.shape = shape;
        this.alignment = alignment;
        this.constraints = constraints;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        ThemeData theme = Theme.of(context);
        DialogThemeData dialogTheme = DialogTheme.of(context);
        DialogThemeData defaults = (theme.useMaterial3 ? new _DialogDefaultsM3__dialog(context) : new _DialogDefaultsM2__dialog(context));
        string? labelLocal = this.semanticLabel;
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                {
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    labelLocal ??= MaterialLocalizations.of(context).dialogLabel;
                    break;
                }
        }
        global::Doroti.Framework.Painting.TextStyle effectiveTitleTextStyle = ((this.titleTextStyle ?? dialogTheme.titleTextStyle) ?? theme.textTheme.titleLarge!);
        double fontSizeLocal = (((global::Doroti.Framework.Painting.TextStyle)effectiveTitleTextStyle).fontSize ?? global::Doroti.Framework.Painting.Text_painterLibrary.kDefaultFontSize);
        double fontSizeToScale = ((fontSizeLocal == 0.0) ? global::Doroti.Framework.Painting.Text_painterLibrary.kDefaultFontSize : fontSizeLocal);
        double effectiveTextScale = (MediaQuery.textScalerOf(context).scale(fontSizeToScale) / fontSizeToScale);
        double paddingScaleFactor = DialogLibrary._scalePadding(effectiveTextScale);
        global::Doroti.Ui.TextDirection? textDirection = Directionality.maybeOf(context);
        global::Doroti.Framework.Widgets.Widget? titleWidget = default!;
        if ((this.title is not null))
        {
            global::Doroti.Framework.Painting.EdgeInsets effectiveTitlePadding = ((global::Doroti.Framework.Painting.EdgeInsets)(object?)this.titlePadding.resolve(textDirection));
            titleWidget = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(left: (((global::Doroti.Framework.Painting.EdgeInsets)effectiveTitlePadding).left * paddingScaleFactor), right: (((global::Doroti.Framework.Painting.EdgeInsets)effectiveTitlePadding).right * paddingScaleFactor), top: (((global::Doroti.Framework.Painting.EdgeInsets)effectiveTitlePadding).top * paddingScaleFactor), bottom: ((this.children is null) ? (((global::Doroti.Framework.Painting.EdgeInsets)effectiveTitlePadding).bottom * paddingScaleFactor) : ((global::Doroti.Framework.Painting.EdgeInsets)effectiveTitlePadding).bottom)), child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: effectiveTitleTextStyle, child: new global::Doroti.Framework.Widgets.Semantics(namesRoute: ((labelLocal is null) && (!object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS))), container: true, child: this.title))));
        }
        global::Doroti.Framework.Widgets.Widget? contentWidget = default!;
        if ((this.children is not null))
        {
            global::Doroti.Framework.Painting.EdgeInsets effectiveContentPadding = ((global::Doroti.Framework.Painting.EdgeInsets)(object?)this.contentPadding.resolve(textDirection));
            contentWidget = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: new global::Doroti.Framework.Widgets.SingleChildScrollView(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(left: (((global::Doroti.Framework.Painting.EdgeInsets)effectiveContentPadding).left * paddingScaleFactor), right: (((global::Doroti.Framework.Painting.EdgeInsets)effectiveContentPadding).right * paddingScaleFactor), top: ((this.title is null) ? (((global::Doroti.Framework.Painting.EdgeInsets)effectiveContentPadding).top * paddingScaleFactor) : ((global::Doroti.Framework.Painting.EdgeInsets)effectiveContentPadding).top), bottom: (((global::Doroti.Framework.Painting.EdgeInsets)effectiveContentPadding).bottom * paddingScaleFactor)), child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: ((this.contentTextStyle ?? dialogTheme.contentTextStyle) ?? defaults.contentTextStyle!), child: new global::Doroti.Framework.Widgets.ListBody(children: this.children!)))));
        }
        global::Doroti.Framework.Widgets.Widget dialogChild = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.IntrinsicWidth(stepWidth: 56.0, child: new global::Doroti.Framework.Widgets.Column(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.stretch, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection49860 = new List<global::Doroti.Framework.Widgets.Widget>(); var __collectionElement49869 = titleWidget; if (__collectionElement49869 is { } __nonNullCollectionElement49869) { __collection49860.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement49869)); } var __collectionElement49883 = contentWidget; if (__collectionElement49883 is { } __nonNullCollectionElement49883) { __collection49860.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement49883)); } return __collection49860; }))())));
        if ((labelLocal is not null))
        {
            dialogChild = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(scopesRoute: true, explicitChildNodes: true, namesRoute: true, label: labelLocal, child: dialogChild));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new Dialog(backgroundColor: this.backgroundColor, elevation: this.elevation, shadowColor: this.shadowColor, surfaceTintColor: this.surfaceTintColor, insetPadding: this.insetPadding, clipBehavior: this.clipBehavior, shape: this.shape, alignment: this.alignment, constraints: this.constraints, child: dialogChild));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class DialogLibrary
{
    internal static global::Doroti.Framework.Widgets.Widget _buildMaterialDialogTransitions(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget child)
    {
        return child;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _FullWindowDialogWrapper__dialog : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _FullWindowDialogWrapper__dialog(global::Doroti.Framework.Widgets.Widget child)
    {
        this.child = child;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DialogThemeData windowDialogTheme = DialogTheme.of(context).copyWith(insetPadding: global::Doroti.Framework.Painting.EdgeInsets.zero, shape: new global::Doroti.Framework.Painting.RoundedRectangleBorder(), alignment: global::Doroti.Framework.Painting.Alignment.topLeft, constraints: global::Doroti.Framework.Rendering.BoxConstraints.CreateExpand());
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new DialogTheme(data: windowDialogTheme, child: global::Doroti.Framework.Widgets.MediaQuery.CreateRemoveViewInsets(removeLeft: true, removeTop: true, removeRight: true, removeBottom: true, context: context, child: global::Doroti.Framework.Widgets.MediaQuery.CreateRemoveViewPadding(removeLeft: true, removeTop: true, removeRight: true, removeBottom: true, context: context, child: this.child))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DialogPopScope__dialog : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual global::System.Action<object>? onPop { get; private set; }

    internal _DialogPopScope__dialog(global::Doroti.Framework.Widgets.Widget child, global::System.Action<object>? onPop = null)
    {
        this.child = child;
        this.onPop = onPop;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.PopScope<object>(canPop: false, onPopInvokedWithResult: ((global::System.Action<bool, object>)((didPop, result) =>
        {
            if (!didPop)
            {
                this.onPop?.Invoke(result);
            }
        })), child: new global::Doroti.Framework.Widgets.Builder(builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)((context) =>
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new _NavigatorShim__dialog(onPop: (global::System.Action<object>?)this.onPop, child: this.child));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _NavigatorShim__dialog : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::System.Action<object>? onPop { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _NavigatorShim__dialog(global::Doroti.Framework.Widgets.Widget child, global::System.Action<object>? onPop = null)
    {
        this.child = child;
        this.onPop = onPop;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.HeroControllerScope.CreateNone(child: new global::Doroti.Framework.Widgets.Navigator(pages: new List<global::Doroti.Framework.Widgets.Page<object?>> { new _DialogContentPage__dialog(child: this.child) }.Cast<global::Doroti.Framework.Widgets.Page<object>>().ToList(), onPopPage: ((global::System.Func<dynamic, object, bool>?)((route, result) =>
        {
            this.onPop?.Invoke(result);
            return false;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DialogContentPage__dialog : global::Doroti.Framework.Widgets.Page<object?>
{
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _DialogContentPage__dialog(global::Doroti.Framework.Widgets.Widget child)
    {
        this.child = child;
    }

    public override global::Doroti.Framework.Widgets.Route<object?> createRoute(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Route<object?>)(object?)new global::Doroti.Framework.Widgets.PageRouteBuilder<object?>(settings: this, pageBuilder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget>)((context, animation, secondaryAnimation) =>
        {
            return this.child;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), transitionDuration: Duration.zero, reverseTransitionDuration: Duration.zero));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class DialogLibrary
{
    public static Future<T?> showDialog<T>(global::Doroti.Framework.Widgets.BuildContext context, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> builder, bool barrierDismissible = true, Color? barrierColor = null, string? barrierLabel = null, bool useSafeArea = true, bool useRootNavigator = true, global::Doroti.Framework.Widgets.RouteSettings? routeSettings = null, Offset? anchorPoint = null, global::Doroti.Framework.Widgets.TraversalEdgeBehavior? traversalEdgeBehavior = null, bool fullscreenDialog = false, bool? requestFocus = null, global::Doroti.Framework.Animation.AnimationStyle? animationStyle = null)
    {
        DartRuntimePrimitives.Assert(() => DialogLibrary._debugIsActive(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        global::Doroti.Framework.Widgets.CapturedThemes themesLocal = ((global::Doroti.Framework.Widgets.CapturedThemes)(object?)InheritedTheme.capture(from: context, to: Navigator.of(context, rootNavigator: useRootNavigator).context));
        global::Doroti.Framework.Widgets.NavigatorState navigator = ((global::Doroti.Framework.Widgets.NavigatorState)(object?)Navigator.of(context, rootNavigator: useRootNavigator));
        return global::Doroti.Framework.Widgets.DialogLibrary.showRawDialog(context: context, useRootNavigator: useRootNavigator, routeSettings: routeSettings, fullscreenDialog: fullscreenDialog, routeBuilder: ((routeContext, _) =>
        {
            return new DialogRoute<T>(context: routeContext, builder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)builder, barrierColor: (((barrierColor ?? DialogTheme.of(context).barrierColor) ?? Theme.of(context).dialogTheme.barrierColor) ?? Colors.black54), barrierDismissible: barrierDismissible, barrierLabel: barrierLabel, useSafeArea: useSafeArea, settings: routeSettings, themes: themesLocal, anchorPoint: anchorPoint, traversalEdgeBehavior: (traversalEdgeBehavior ?? global::Doroti.Framework.Widgets.TraversalEdgeBehavior.closedLoop), requestFocus: requestFocus, animationStyle: animationStyle, fullscreenDialog: fullscreenDialog);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }), builder: ((routeContext) =>
        {
            global::Doroti.Ui.TextDirection textDirectionLocal = Directionality.of(context);
            ThemeData themeData = Theme.of(context);
            global::Doroti.Framework.Widgets.MediaQueryData mediaQuery = ((global::Doroti.Framework.Widgets.MediaQueryData)(object?)MediaQuery.of(context));
            global::Doroti.Framework.Widgets.Widget dialogContent = ((global::Doroti.Framework.Widgets.Widget)(object?)new _DialogPopScope__dialog(onPop: Navigator.of(navigator.context).pop, child: new global::Doroti.Framework.Widgets.Builder(builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)((innerContext) =>
            {
                return ((global::Doroti.Framework.Widgets.Widget)(object?)new _FullWindowDialogWrapper__dialog(child: builder(innerContext)));
                throw new InvalidOperationException("Dart closure completed without a value.");
            })))));
            return new global::Doroti.Framework.Widgets.Directionality(textDirection: textDirectionLocal, child: new Theme(data: themeData, child: new global::Doroti.Framework.Widgets.MediaQuery(data: mediaQuery, child: dialogContent)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class DialogLibrary
{
    public static Future<T?> showAdaptiveDialog<T>(global::Doroti.Framework.Widgets.BuildContext context, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> builder, bool? barrierDismissible = null, Color? barrierColor = null, string? barrierLabel = null, bool useSafeArea = true, bool useRootNavigator = true, global::Doroti.Framework.Widgets.RouteSettings? routeSettings = null, Offset? anchorPoint = null, global::Doroti.Framework.Widgets.TraversalEdgeBehavior? traversalEdgeBehavior = null, bool? requestFocus = null, global::Doroti.Framework.Animation.AnimationStyle? animationStyle = null)
    {
        ThemeData theme = Theme.of(context);
        switch (theme.platform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    return DialogLibrary.showDialog<T>(context: context, builder: builder, barrierDismissible: (barrierDismissible ?? true), barrierColor: barrierColor, barrierLabel: barrierLabel, useSafeArea: useSafeArea, useRootNavigator: useRootNavigator, routeSettings: routeSettings, anchorPoint: anchorPoint, traversalEdgeBehavior: traversalEdgeBehavior, requestFocus: requestFocus, animationStyle: animationStyle);
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                {
                    return RouteLibrary.showCupertinoDialog<T>(context: context, builder: builder, barrierDismissible: (barrierDismissible ?? false), barrierLabel: barrierLabel, useRootNavigator: useRootNavigator, anchorPoint: anchorPoint, routeSettings: routeSettings, requestFocus: requestFocus);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class DialogLibrary
{
    internal static bool _debugIsActive(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if (((context is global::Doroti.Framework.Widgets.Element) && !((global::Doroti.Framework.Widgets.Element)context).debugIsActive))
        {
            global::Doroti.Framework.Widgets.Element context__as64315 = (global::Doroti.Framework.Widgets.Element)context;
            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("This BuildContext is no longer valid."), new global::Doroti.Framework.Foundation.ErrorDescription("The showDialog function context parameter is a BuildContext that is no longer valid."), new global::Doroti.Framework.Foundation.ErrorHint("This can commonly occur when the showDialog function is called after awaiting a Future. " + "In this situation the BuildContext might refer to a widget that has already been disposed during the await. " + "Consider using a parent context instead.") }));
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class DialogRoute<T> : global::Doroti.Framework.Widgets.RawDialogRoute<T>
{
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _curvedAnimation { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.AnimationStyle? _animationStyle { get; private set; }

    public DialogRoute(global::Doroti.Framework.Widgets.BuildContext context, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget> builder, global::Doroti.Framework.Widgets.CapturedThemes? themes = null, Color? barrierColor = default!, bool barrierDismissible = true, string? barrierLabel = null, bool useSafeArea = true, global::Doroti.Framework.Widgets.RouteSettings? settings = null, bool? requestFocus = null, Offset? anchorPoint = null, global::Doroti.Framework.Widgets.TraversalEdgeBehavior? traversalEdgeBehavior = null, bool fullscreenDialog = false, global::Doroti.Framework.Animation.AnimationStyle? animationStyle = null) : base(barrierColor: barrierColor ?? Colors.black54, barrierDismissible: barrierDismissible, settings: settings, requestFocus: requestFocus, anchorPoint: anchorPoint, traversalEdgeBehavior: traversalEdgeBehavior, fullscreenDialog: fullscreenDialog, pageBuilder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget>)((buildContext, animation, secondaryAnimation) =>
    {
        global::Doroti.Framework.Widgets.Widget pageChild = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Builder(builder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)builder));
        global::Doroti.Framework.Widgets.Widget dialog = (themes?.wrap(pageChild) ?? pageChild);
        if (useSafeArea)
        {
            dialog = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SafeArea(child: dialog));
        }
        dialog = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(hitTestBehavior: SemanticsHitTestBehavior.opaque, child: dialog));
        return dialog;
        throw new InvalidOperationException("Dart closure completed without a value.");
    })), barrierLabel: (barrierLabel ?? MaterialLocalizations.of(context).modalBarrierDismissLabel), transitionDuration: (animationStyle?.duration ?? Duration.Create(milliseconds: 150L)), transitionBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Widget>)DialogLibrary._buildMaterialDialogTransitions)
    {
        this._animationStyle = animationStyle;
    }

    internal virtual void _setAnimation(global::Doroti.Framework.Animation.Animation<double> animation)
    {
        if ((!object.Equals(this._curvedAnimation?.parent, animation)))
        {
            this._curvedAnimation?.dispose();
            _curvedAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: animation, curve: (this._animationStyle?.curve ?? global::Doroti.Framework.Animation.Curves.easeOut), reverseCurve: (this._animationStyle?.reverseCurve ?? global::Doroti.Framework.Animation.Curves.easeOut));
        }
    }

    public override global::Doroti.Framework.Widgets.Widget buildTransitions(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget child)
    {
        _setAnimation(animation);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.FadeTransition(opacity: this._curvedAnimation!, child: base.buildTransitions(context, animation, secondaryAnimation, child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        this._curvedAnimation?.dispose();
        base.dispose();
    }

}

public static partial class DialogLibrary
{
    internal static double _scalePadding(double textScaleFactor)
    {
        double clampedTextScaleFactor = Dart_uiLibrary.clampDouble(textScaleFactor, 1.0, 2.0);
        return DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(1.0, (1.0 / 3.0), (clampedTextScaleFactor - 1.0)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _DialogDefaultsM2__dialog : DialogThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late_theme_initialized;
    private ThemeData __late_theme = default!;
    public virtual ThemeData theme
    {
        get
        {
            if (!__late_theme_initialized)
            {
                __late_theme = Theme.of(this.context);
                __late_theme_initialized = true;
            }
            return __late_theme;
        }
    }
    private bool __late_textTheme_initialized;
    private TextTheme __late_textTheme = default!;
    public virtual TextTheme textTheme
    {
        get
        {
            if (!__late_textTheme_initialized)
            {
                __late_textTheme = this.theme.textTheme;
                __late_textTheme_initialized = true;
            }
            return __late_textTheme;
        }
    }
    private bool __late_iconTheme_initialized;
    private global::Doroti.Framework.Widgets.IconThemeData __late_iconTheme = default!;
    public virtual global::Doroti.Framework.Widgets.IconThemeData iconTheme
    {
        get
        {
            if (!__late_iconTheme_initialized)
            {
                __late_iconTheme = this.theme.iconTheme;
                __late_iconTheme_initialized = true;
            }
            return __late_iconTheme;
        }
    }

    internal _DialogDefaultsM2__dialog(global::Doroti.Framework.Widgets.BuildContext context) : base(alignment: global::Doroti.Framework.Painting.Alignment.center, elevation: 24.0, shape: new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(4.0))), clipBehavior: Clip.none)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? iconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((global::Doroti.Framework.Widgets.IconThemeData)this.iconTheme).color);
    public virtual global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((object.Equals(this.theme.brightness, Brightness.dark)) ? Colors.grey[800L]! : Colors.white));
    public virtual global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this.theme.shadowColor);
    public override global::Doroti.Framework.Painting.TextStyle? titleTextStyle => this.textTheme.titleLarge;
    public override global::Doroti.Framework.Painting.TextStyle? contentTextStyle => this.textTheme.titleMedium;
    public override global::Doroti.Framework.Painting.EdgeInsetsGeometry? actionsPadding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Framework.Painting.EdgeInsets.zero);
}

internal class _DialogFullscreenDefaultsM3__dialog : DialogThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;

    internal _DialogFullscreenDefaultsM3__dialog(global::Doroti.Framework.Widgets.BuildContext context) : base(clipBehavior: Clip.none)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Theme.of(this.context).colorScheme.surface);
}

internal class _DialogDefaultsM3__dialog : DialogThemeData
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

    internal _DialogDefaultsM3__dialog(global::Doroti.Framework.Widgets.BuildContext context) : base(alignment: global::Doroti.Framework.Painting.Alignment.center, elevation: 6.0, shape: new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(28.0))), clipBehavior: Clip.none)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? iconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.secondary);
    public virtual global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.surfaceContainerHigh);
    public virtual global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public virtual global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public override global::Doroti.Framework.Painting.TextStyle? titleTextStyle => this._textTheme.headlineSmall;
    public override global::Doroti.Framework.Painting.TextStyle? contentTextStyle => this._textTheme.bodyMedium;
    public override global::Doroti.Framework.Painting.EdgeInsetsGeometry? actionsPadding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(left: 24.0, right: 24.0, bottom: 24.0));
}
