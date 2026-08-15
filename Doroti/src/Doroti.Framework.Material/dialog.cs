// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/dialog.dart
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

public static partial class DialogLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.EdgeInsets _defaultInsetPadding = global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 40.0, vertical: 24.0);
}

public class Dialog : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual Duration insetAnimationDuration { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Curve insetAnimationCurve { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets? insetPadding { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }
    internal virtual bool _fullscreen { get; private set; } = default!;
    public virtual SemanticsRole semanticsRole { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints { get; private set; }

    public Dialog(global::Doroti.Generated.Framework.Foundation.Key? key = null, Color? backgroundColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, Duration? insetAnimationDuration = null, global::Doroti.Generated.Framework.Animation.Curve insetAnimationCurve = default!, global::Doroti.Generated.Framework.Painting.EdgeInsets? insetPadding = null, Clip? clipBehavior = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = null, global::Doroti.Generated.Framework.Widgets.Widget? child = null, SemanticsRole semanticsRole = SemanticsRole.dialog, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null) : base(key: key)
    {
        Duration __insetAnimationDuration = insetAnimationDuration ?? Duration.Create(milliseconds: 100);
        global::Doroti.Generated.Framework.Animation.Curve __insetAnimationCurve = insetAnimationCurve ?? global::Doroti.Generated.Framework.Animation.Curves.decelerate;
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

    public static Dialog CreateFullscreen(global::Doroti.Generated.Framework.Foundation.Key? key = null, Color? backgroundColor = null, Duration insetAnimationDuration = default, global::Doroti.Generated.Framework.Animation.Curve insetAnimationCurve = default!, global::Doroti.Generated.Framework.Widgets.Widget? child = null, SemanticsRole semanticsRole = SemanticsRole.dialog)
    {
        var __instance = new Dialog(key: key, backgroundColor: backgroundColor, insetAnimationDuration: insetAnimationDuration, insetAnimationCurve: insetAnimationCurve, child: child, semanticsRole: semanticsRole);
        global::Doroti.Generated.Framework.Animation.Curve __insetAnimationCurve = insetAnimationCurve ?? global::Doroti.Generated.Framework.Animation.Curves.decelerate;
        __instance.backgroundColor = backgroundColor;
        __instance.insetAnimationDuration = insetAnimationDuration;
        __instance.insetAnimationCurve = __insetAnimationCurve;
        __instance.child = child;
        __instance.semanticsRole = semanticsRole;
        __instance.elevation = 0;
        __instance.shadowColor = null;
        __instance.surfaceTintColor = null;
        __instance.insetPadding = global::Doroti.Generated.Framework.Painting.EdgeInsets.zero;
        __instance.clipBehavior = Clip.none;
        __instance.shape = null;
        __instance.alignment = null;
        __instance.constraints = null;
        __instance._fullscreen = true;
        return __instance;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__9765 = Theme.of(context);
        DialogThemeData dialogTheme__9818 = DialogTheme.of(context);
        global::Doroti.Generated.Framework.Painting.EdgeInsets effectivePadding__9878 = (MediaQuery.viewInsetsOf(context).op_Add((((this.insetPadding ?? dialogTheme__9818.insetPadding) ?? DialogLibrary._defaultInsetPadding))));
        DialogThemeData defaults__10042 = (theme__9765.useMaterial3 ? ((this._fullscreen ? new _DialogFullscreenDefaultsM3__dialog(context) : new _DialogDefaultsM3__dialog(context))) : new _DialogDefaultsM2__dialog(context));
        global::Doroti.Generated.Framework.Rendering.BoxConstraints boxConstraints__10228 = ((this.constraints ?? dialogTheme__9818.constraints) ?? new global::Doroti.Generated.Framework.Rendering.BoxConstraints(minWidth: 280.0));
        global::Doroti.Generated.Framework.Widgets.Widget dialogChild__10346 = default!;
        if (this._fullscreen)
        {
            dialogChild__10346 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new Material(color: ((this.backgroundColor ?? dialogTheme__9818.backgroundColor) ?? defaults__10042.backgroundColor), child: this.child));
        }
        else
        {
            dialogChild__10346 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Align(alignment: ((this.alignment ?? dialogTheme__9818.alignment) ?? defaults__10042.alignment!), child: new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: boxConstraints__10228, child: new Material(color: ((this.backgroundColor ?? dialogTheme__9818.backgroundColor) ?? defaults__10042.backgroundColor), elevation: ((this.elevation ?? dialogTheme__9818.elevation) ?? DartRuntimePrimitives.RequireValue(defaults__10042.elevation)), shadowColor: ((this.shadowColor ?? dialogTheme__9818.shadowColor) ?? defaults__10042.shadowColor), surfaceTintColor: ((this.surfaceTintColor ?? dialogTheme__9818.surfaceTintColor) ?? defaults__10042.surfaceTintColor), shape: ((this.shape ?? dialogTheme__9818.shape) ?? defaults__10042.shape!), type: MaterialType.card, clipBehavior: ((this.clipBehavior ?? dialogTheme__9818.clipBehavior) ?? DartRuntimePrimitives.RequireValue(defaults__10042.clipBehavior)), child: this.child))));
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Semantics(role: this.semanticsRole, child: new global::Doroti.Generated.Framework.Widgets.AnimatedPadding(padding: effectivePadding__9878, duration: DartRuntimePrimitives.RequireValue(this.insetAnimationDuration), curve: this.insetAnimationCurve, child: global::Doroti.Generated.Framework.Widgets.MediaQuery.CreateRemoveViewInsets(removeLeft: true, removeTop: true, removeRight: true, removeBottom: true, context: context, child: dialogChild__10346))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class AlertDialog : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? icon { get; private set; }
    public virtual Color? iconColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? iconPadding { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? title { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? titlePadding { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? content { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? contentPadding { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? contentTextStyle { get; private set; }
    public virtual List<global::Doroti.Generated.Framework.Widgets.Widget>? actions { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? actionsPadding { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.MainAxisAlignment? actionsAlignment { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.OverflowBarAlignment? actionsOverflowAlignment { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.VerticalDirection? actionsOverflowDirection { get; private set; }
    public virtual double? actionsOverflowButtonSpacing { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? buttonPadding { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual string? semanticLabel { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets? insetPadding { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints { get; private set; }
    public virtual bool scrollable { get; private set; } = default!;

    public AlertDialog(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget? icon = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? iconPadding = null, Color? iconColor = null, global::Doroti.Generated.Framework.Widgets.Widget? title = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? titlePadding = null, global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Generated.Framework.Widgets.Widget? content = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, global::Doroti.Generated.Framework.Painting.TextStyle? contentTextStyle = null, List<global::Doroti.Generated.Framework.Widgets.Widget>? actions = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? actionsPadding = null, global::Doroti.Generated.Framework.Rendering.MainAxisAlignment? actionsAlignment = null, global::Doroti.Generated.Framework.Widgets.OverflowBarAlignment? actionsOverflowAlignment = null, global::Doroti.Generated.Framework.Painting.VerticalDirection? actionsOverflowDirection = null, double? actionsOverflowButtonSpacing = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? buttonPadding = null, Color? backgroundColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, string? semanticLabel = null, global::Doroti.Generated.Framework.Painting.EdgeInsets? insetPadding = null, Clip? clipBehavior = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, bool scrollable = false) : base(key: key)
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

    public static AlertDialog CreateAdaptive(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget? icon = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? iconPadding = null, Color? iconColor = null, global::Doroti.Generated.Framework.Widgets.Widget? title = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? titlePadding = null, global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Generated.Framework.Widgets.Widget? content = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, global::Doroti.Generated.Framework.Painting.TextStyle? contentTextStyle = null, List<global::Doroti.Generated.Framework.Widgets.Widget>? actions = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? actionsPadding = null, global::Doroti.Generated.Framework.Rendering.MainAxisAlignment? actionsAlignment = null, global::Doroti.Generated.Framework.Widgets.OverflowBarAlignment? actionsOverflowAlignment = null, global::Doroti.Generated.Framework.Painting.VerticalDirection? actionsOverflowDirection = null, double? actionsOverflowButtonSpacing = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? buttonPadding = null, Color? backgroundColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, string? semanticLabel = null, global::Doroti.Generated.Framework.Painting.EdgeInsets insetPadding = default!, Clip? clipBehavior = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, bool scrollable = default!, global::Doroti.Generated.Framework.Widgets.ScrollController? scrollController = null, global::Doroti.Generated.Framework.Widgets.ScrollController? actionScrollController = null, Duration insetAnimationDuration = default!, global::Doroti.Generated.Framework.Animation.Curve insetAnimationCurve = default!)
        => ((AlertDialog)(object?)new _AdaptiveAlertDialog__dialog(key, icon, iconPadding, iconColor, title, titlePadding, titleTextStyle, content, contentPadding, contentTextStyle, actions, actionsPadding, actionsAlignment, actionsOverflowAlignment, actionsOverflowDirection, actionsOverflowButtonSpacing, buttonPadding, backgroundColor, elevation, shadowColor, surfaceTintColor, semanticLabel, insetPadding, clipBehavior, shape, alignment, constraints, scrollable, scrollController, actionScrollController, insetAnimationDuration, insetAnimationCurve));

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        ThemeData theme__29144 = Theme.of(context);
        DialogThemeData dialogTheme__29198 = DialogTheme.of(context);
        DialogThemeData defaults__29263 = (theme__29144.useMaterial3 ? new _DialogDefaultsM3__dialog(context) : new _DialogDefaultsM2__dialog(context));
        string? label__29387 = (global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform switch { global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS => this.semanticLabel, global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS => this.semanticLabel, global::Doroti.Generated.Framework.Foundation.TargetPlatform.android or global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia or global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux => (this.semanticLabel ?? MaterialLocalizations.of(context).alertDialogLabel), global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows => (this.semanticLabel ?? MaterialLocalizations.of(context).alertDialogLabel), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        var fontSizeToScale__29797 = 14.0;
        double effectiveTextScale__29838 = (MediaQuery.textScalerOf(context).scale(fontSizeToScale__29797) / fontSizeToScale__29797);
        double paddingScaleFactor__29959 = DialogLibrary._scalePadding(effectiveTextScale__29838);
        global::Doroti.Ui.TextDirection? textDirection__30040 = Directionality.maybeOf(context);
        global::Doroti.Generated.Framework.Widgets.Widget? iconWidget__30102 = default!;
        global::Doroti.Generated.Framework.Widgets.Widget? titleWidget__30126 = default!;
        global::Doroti.Generated.Framework.Widgets.Widget? contentWidget__30151 = default!;
        global::Doroti.Generated.Framework.Widgets.Widget? actionsWidget__30178 = default!;
        if ((this.icon is not null))
        {
            var belowIsTitle__30230 = (this.title is not null);
            bool belowIsContent__30277 = (!belowIsTitle__30230 && (this.content is not null));
            var defaultIconPadding__30340 = global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(left: 24.0, top: 24.0, right: 24.0, bottom: (belowIsTitle__30230 ? 16.0 : (belowIsContent__30277 ? 0.0 : 24.0)));
            global::Doroti.Generated.Framework.Painting.EdgeInsets effectiveIconPadding__30585 = (this.iconPadding?.resolve(textDirection__30040) ?? defaultIconPadding__30340);
            iconWidget__30102 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(left: (((global::Doroti.Generated.Framework.Painting.EdgeInsets)effectiveIconPadding__30585).left * paddingScaleFactor__29959), right: (((global::Doroti.Generated.Framework.Painting.EdgeInsets)effectiveIconPadding__30585).right * paddingScaleFactor__29959), top: (((global::Doroti.Generated.Framework.Painting.EdgeInsets)effectiveIconPadding__30585).top * paddingScaleFactor__29959), bottom: ((global::Doroti.Generated.Framework.Painting.EdgeInsets)effectiveIconPadding__30585).bottom), child: new global::Doroti.Generated.Framework.Widgets.IconTheme(data: new global::Doroti.Generated.Framework.Widgets.IconThemeData(color: ((this.iconColor ?? dialogTheme__29198.iconColor) ?? defaults__29263.iconColor)), child: this.icon!)));
        }
        if ((this.title is not null))
        {
            var defaultTitlePadding__31199 = global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(left: 24.0, top: ((this.icon is null) ? 24.0 : 0.0), right: 24.0, bottom: ((this.content is null) ? 20.0 : 0.0));
            global::Doroti.Generated.Framework.Painting.EdgeInsets effectiveTitlePadding__31397 = (this.titlePadding?.resolve(textDirection__30040) ?? defaultTitlePadding__31199);
            titleWidget__30126 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(left: (((global::Doroti.Generated.Framework.Painting.EdgeInsets)effectiveTitlePadding__31397).left * paddingScaleFactor__29959), right: (((global::Doroti.Generated.Framework.Painting.EdgeInsets)effectiveTitlePadding__31397).right * paddingScaleFactor__29959), top: ((this.icon is null) ? (((global::Doroti.Generated.Framework.Painting.EdgeInsets)effectiveTitlePadding__31397).top * paddingScaleFactor__29959) : ((global::Doroti.Generated.Framework.Painting.EdgeInsets)effectiveTitlePadding__31397).top), bottom: ((global::Doroti.Generated.Framework.Painting.EdgeInsets)effectiveTitlePadding__31397).bottom), child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: ((this.titleTextStyle ?? dialogTheme__29198.titleTextStyle) ?? defaults__29263.titleTextStyle!), textAlign: ((this.icon is null) ? global::Doroti.Ui.TextAlign.start : global::Doroti.Ui.TextAlign.center), child: new global::Doroti.Generated.Framework.Widgets.Semantics(namesRoute: ((label__29387 is null) && (!object.Equals(global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS))), container: true, child: this.title))));
        }
        if ((this.content is not null))
        {
            var defaultContentPadding__32469 = global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(left: 24.0, top: (theme__29144.useMaterial3 ? 16.0 : 20.0), right: 24.0, bottom: 24.0);
            global::Doroti.Generated.Framework.Painting.EdgeInsets effectiveContentPadding__32652 = (this.contentPadding?.resolve(textDirection__30040) ?? defaultContentPadding__32469);
            contentWidget__30151 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(left: (((global::Doroti.Generated.Framework.Painting.EdgeInsets)effectiveContentPadding__32652).left * paddingScaleFactor__29959), right: (((global::Doroti.Generated.Framework.Painting.EdgeInsets)effectiveContentPadding__32652).right * paddingScaleFactor__29959), top: (((this.title is null) && (this.icon is null)) ? (((global::Doroti.Generated.Framework.Painting.EdgeInsets)effectiveContentPadding__32652).top * paddingScaleFactor__29959) : ((global::Doroti.Generated.Framework.Painting.EdgeInsets)effectiveContentPadding__32652).top), bottom: ((global::Doroti.Generated.Framework.Painting.EdgeInsets)effectiveContentPadding__32652).bottom), child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: ((this.contentTextStyle ?? dialogTheme__29198.contentTextStyle) ?? defaults__29263.contentTextStyle!), child: new global::Doroti.Generated.Framework.Widgets.Semantics(container: true, explicitChildNodes: true, child: this.content))));
        }
        if ((this.actions is not null))
        {
            double spacing__33460 = (((this.buttonPadding?.horizontal ?? 16)) / 2L);
            actionsWidget__30178 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: ((this.actionsPadding ?? dialogTheme__29198.actionsPadding) ?? ((theme__29144.useMaterial3 ? defaults__29263.actionsPadding! : defaults__29263.actionsPadding!.add(global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(spacing__33460))))), child: new global::Doroti.Generated.Framework.Widgets.OverflowBar(alignment: (this.actionsAlignment ?? global::Doroti.Generated.Framework.Rendering.MainAxisAlignment.end), spacing: spacing__33460, overflowAlignment: (this.actionsOverflowAlignment ?? global::Doroti.Generated.Framework.Widgets.OverflowBarAlignment.end), overflowDirection: (this.actionsOverflowDirection ?? global::Doroti.Generated.Framework.Painting.VerticalDirection.down), overflowSpacing: (this.actionsOverflowButtonSpacing ?? 0), children: this.actions!)));
        }
        List<global::Doroti.Generated.Framework.Widgets.Widget> columnChildren__34198 = default!;
        if (this.scrollable)
        {
            columnChildren__34198 = ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection34259 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); if (((this.title is not null) || (this.content is not null))) { __collection34259.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Flexible(child: new global::Doroti.Generated.Framework.Widgets.SingleChildScrollView(child: new global::Doroti.Generated.Framework.Widgets.Column(mainAxisSize: global::Doroti.Generated.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.stretch, children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection34544 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); var __collectionElement34553 = iconWidget__30102; if (__collectionElement34553 is { } __nonNullCollectionElement34553) { __collection34544.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement34553)); } var __collectionElement34566 = titleWidget__30126; if (__collectionElement34566 is { } __nonNullCollectionElement34566) { __collection34544.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement34566)); } var __collectionElement34580 = contentWidget__30151; if (__collectionElement34580 is { } __nonNullCollectionElement34580) { __collection34544.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement34580)); } return __collection34544; }))()))))); } var __collectionElement34650 = actionsWidget__30178; if (__collectionElement34650 is { } __nonNullCollectionElement34650) { __collection34259.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement34650)); } return __collection34259; }))();
        }
        else
        {
            columnChildren__34198 = ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection34711 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); var __collectionElement34729 = iconWidget__30102; if (__collectionElement34729 is { } __nonNullCollectionElement34729) { __collection34711.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement34729)); } var __collectionElement34750 = titleWidget__30126; if (__collectionElement34750 is { } __nonNullCollectionElement34750) { __collection34711.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement34750)); } if ((contentWidget__30151 is not null)) { __collection34711.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Flexible(child: contentWidget__30151))); } var __collectionElement34839 = actionsWidget__30178; if (__collectionElement34839 is { } __nonNullCollectionElement34839) { __collection34711.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement34839)); } return __collection34711; }))();
        }
        global::Doroti.Generated.Framework.Widgets.Widget dialogChild__34882 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.IntrinsicWidth(child: new global::Doroti.Generated.Framework.Widgets.Column(mainAxisSize: global::Doroti.Generated.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.stretch, children: columnChildren__34198)));
        if ((label__29387 is not null))
        {
            dialogChild__34882 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Semantics(scopesRoute: true, explicitChildNodes: true, namesRoute: true, label: label__29387, child: dialogChild__34882));
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new Dialog(backgroundColor: this.backgroundColor, elevation: this.elevation, shadowColor: this.shadowColor, surfaceTintColor: this.surfaceTintColor, insetPadding: this.insetPadding, clipBehavior: this.clipBehavior, shape: this.shape, alignment: this.alignment, constraints: this.constraints, semanticsRole: SemanticsRole.alertDialog, child: dialogChild__34882));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AdaptiveAlertDialog__dialog : AlertDialog
{
    public virtual global::Doroti.Generated.Framework.Widgets.ScrollController? scrollController { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.ScrollController? actionScrollController { get; private set; }
    public virtual Duration insetAnimationDuration { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Curve insetAnimationCurve { get; private set; } = default!;

    internal _AdaptiveAlertDialog__dialog(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget? icon = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? iconPadding = null, Color? iconColor = null, global::Doroti.Generated.Framework.Widgets.Widget? title = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? titlePadding = null, global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Generated.Framework.Widgets.Widget? content = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, global::Doroti.Generated.Framework.Painting.TextStyle? contentTextStyle = null, List<global::Doroti.Generated.Framework.Widgets.Widget>? actions = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? actionsPadding = null, global::Doroti.Generated.Framework.Rendering.MainAxisAlignment? actionsAlignment = null, global::Doroti.Generated.Framework.Widgets.OverflowBarAlignment? actionsOverflowAlignment = null, global::Doroti.Generated.Framework.Painting.VerticalDirection? actionsOverflowDirection = null, double? actionsOverflowButtonSpacing = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? buttonPadding = null, Color? backgroundColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, string? semanticLabel = null, global::Doroti.Generated.Framework.Painting.EdgeInsets? insetPadding = null, Clip? clipBehavior = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, bool scrollable = false, global::Doroti.Generated.Framework.Widgets.ScrollController? scrollController = null, global::Doroti.Generated.Framework.Widgets.ScrollController? actionScrollController = null, Duration? insetAnimationDuration = null, global::Doroti.Generated.Framework.Animation.Curve insetAnimationCurve = default!) : base(key: key, icon: icon, iconPadding: iconPadding, iconColor: iconColor, title: title, titlePadding: titlePadding, titleTextStyle: titleTextStyle, content: content, contentPadding: contentPadding, contentTextStyle: contentTextStyle, actions: actions, actionsPadding: actionsPadding, actionsAlignment: actionsAlignment, actionsOverflowAlignment: actionsOverflowAlignment, actionsOverflowDirection: actionsOverflowDirection, actionsOverflowButtonSpacing: actionsOverflowButtonSpacing, buttonPadding: buttonPadding, backgroundColor: backgroundColor, elevation: elevation, shadowColor: shadowColor, surfaceTintColor: surfaceTintColor, semanticLabel: semanticLabel, insetPadding: insetPadding, clipBehavior: clipBehavior, shape: shape, alignment: alignment, constraints: constraints, scrollable: scrollable)
    {
        Duration __insetAnimationDuration = insetAnimationDuration ?? Duration.Create(milliseconds: 100);
        global::Doroti.Generated.Framework.Animation.Curve __insetAnimationCurve = insetAnimationCurve ?? global::Doroti.Generated.Framework.Animation.Curves.decelerate;
        this.scrollController = scrollController;
        this.actionScrollController = actionScrollController;
        this.insetAnimationDuration = __insetAnimationDuration;
        this.insetAnimationCurve = __insetAnimationCurve;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__36882 = Theme.of(context);
        switch (theme__36882.platform)
        {
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
                {
                    break;
                }
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
                {
                    return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new CupertinoAlertDialog(title: this.title, content: this.content, actions: (this.actions ?? new List<global::Doroti.Generated.Framework.Widgets.Widget>()), scrollController: this.scrollController, actionScrollController: this.actionScrollController, insetAnimationDuration: DartRuntimePrimitives.RequireValue(this.insetAnimationDuration), insetAnimationCurve: this.insetAnimationCurve));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)base.build(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SimpleDialogOption : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets? padding { get; private set; }

    public SimpleDialogOption(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Action? onPressed = null, global::Doroti.Generated.Framework.Painting.EdgeInsets? padding = null, global::Doroti.Generated.Framework.Widgets.Widget? child = null) : base(key: key)
    {
        this.onPressed = onPressed;
        this.padding = padding;
        this.child = child;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new InkWell(onTap: this.onPressed, child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: (this.padding ?? global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(vertical: 8.0, horizontal: 24.0)), child: this.child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SimpleDialog : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? title { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry titlePadding { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle { get; private set; }
    public virtual List<global::Doroti.Generated.Framework.Widgets.Widget>? children { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry contentPadding { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? contentTextStyle { get; private set; }
    public virtual string? semanticLabel { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets? insetPadding { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints { get; private set; }

    public SimpleDialog(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget? title = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry titlePadding = default!, global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle = null, List<global::Doroti.Generated.Framework.Widgets.Widget>? children = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry contentPadding = default!, global::Doroti.Generated.Framework.Painting.TextStyle? contentTextStyle = null, Color? backgroundColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, string? semanticLabel = null, global::Doroti.Generated.Framework.Painting.EdgeInsets? insetPadding = null, Clip? clipBehavior = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null) : base(key: key)
    {
        global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry __titlePadding = titlePadding ?? new global::Doroti.Generated.Framework.Painting.EdgeInsets(24.0, 24.0, 24.0, 0.0);
        global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry __contentPadding = contentPadding ?? new global::Doroti.Generated.Framework.Painting.EdgeInsets(0.0, 12.0, 0.0, 16.0);
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

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        ThemeData theme__46601 = Theme.of(context);
        DialogThemeData dialogTheme__46655 = DialogTheme.of(context);
        DialogThemeData defaults__46720 = (theme__46601.useMaterial3 ? new _DialogDefaultsM3__dialog(context) : new _DialogDefaultsM2__dialog(context));
        string? label__46838 = this.semanticLabel;
        switch (global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
                {
                    break;
                }
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
                {
                    label__46838 ??= MaterialLocalizations.of(context).dialogLabel;
                    break;
                }
        }
        global::Doroti.Generated.Framework.Painting.TextStyle effectiveTitleTextStyle__47294 = ((this.titleTextStyle ?? dialogTheme__46655.titleTextStyle) ?? theme__46601.textTheme.titleLarge!);
        double fontSize__47422 = (((global::Doroti.Generated.Framework.Painting.TextStyle)effectiveTitleTextStyle__47294).fontSize ?? global::Doroti.Generated.Framework.Painting.Text_painterLibrary.kDefaultFontSize);
        double fontSizeToScale__47504 = ((fontSize__47422 == 0.0) ? global::Doroti.Generated.Framework.Painting.Text_painterLibrary.kDefaultFontSize : fontSize__47422);
        double effectiveTextScale__47586 = (MediaQuery.textScalerOf(context).scale(fontSizeToScale__47504) / fontSizeToScale__47504);
        double paddingScaleFactor__47707 = DialogLibrary._scalePadding(effectiveTextScale__47586);
        global::Doroti.Ui.TextDirection? textDirection__47788 = Directionality.maybeOf(context);
        global::Doroti.Generated.Framework.Widgets.Widget? titleWidget__47850 = default!;
        if ((this.title is not null))
        {
            global::Doroti.Generated.Framework.Painting.EdgeInsets effectiveTitlePadding__47911 = ((global::Doroti.Generated.Framework.Painting.EdgeInsets)(object?)this.titlePadding.resolve(textDirection__47788));
            titleWidget__47850 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(left: (((global::Doroti.Generated.Framework.Painting.EdgeInsets)effectiveTitlePadding__47911).left * paddingScaleFactor__47707), right: (((global::Doroti.Generated.Framework.Painting.EdgeInsets)effectiveTitlePadding__47911).right * paddingScaleFactor__47707), top: (((global::Doroti.Generated.Framework.Painting.EdgeInsets)effectiveTitlePadding__47911).top * paddingScaleFactor__47707), bottom: ((this.children is null) ? (((global::Doroti.Generated.Framework.Painting.EdgeInsets)effectiveTitlePadding__47911).bottom * paddingScaleFactor__47707) : ((global::Doroti.Generated.Framework.Painting.EdgeInsets)effectiveTitlePadding__47911).bottom)), child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: effectiveTitleTextStyle__47294, child: new global::Doroti.Generated.Framework.Widgets.Semantics(namesRoute: ((label__46838 is null) && (!object.Equals(global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS))), container: true, child: this.title))));
        }
        global::Doroti.Generated.Framework.Widgets.Widget? contentWidget__48829 = default!;
        if ((this.children is not null))
        {
            global::Doroti.Generated.Framework.Painting.EdgeInsets effectiveContentPadding__48895 = ((global::Doroti.Generated.Framework.Painting.EdgeInsets)(object?)this.contentPadding.resolve(textDirection__47788));
            contentWidget__48829 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Flexible(child: new global::Doroti.Generated.Framework.Widgets.SingleChildScrollView(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(left: (((global::Doroti.Generated.Framework.Painting.EdgeInsets)effectiveContentPadding__48895).left * paddingScaleFactor__47707), right: (((global::Doroti.Generated.Framework.Painting.EdgeInsets)effectiveContentPadding__48895).right * paddingScaleFactor__47707), top: ((this.title is null) ? (((global::Doroti.Generated.Framework.Painting.EdgeInsets)effectiveContentPadding__48895).top * paddingScaleFactor__47707) : ((global::Doroti.Generated.Framework.Painting.EdgeInsets)effectiveContentPadding__48895).top), bottom: (((global::Doroti.Generated.Framework.Painting.EdgeInsets)effectiveContentPadding__48895).bottom * paddingScaleFactor__47707)), child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: ((this.contentTextStyle ?? dialogTheme__46655.contentTextStyle) ?? defaults__46720.contentTextStyle!), child: new global::Doroti.Generated.Framework.Widgets.ListBody(children: this.children!)))));
        }
        global::Doroti.Generated.Framework.Widgets.Widget dialogChild__49672 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.IntrinsicWidth(stepWidth: 56.0, child: new global::Doroti.Generated.Framework.Widgets.Column(mainAxisSize: global::Doroti.Generated.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.stretch, children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection49860 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); var __collectionElement49869 = titleWidget__47850; if (__collectionElement49869 is { } __nonNullCollectionElement49869) { __collection49860.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement49869)); } var __collectionElement49883 = contentWidget__48829; if (__collectionElement49883 is { } __nonNullCollectionElement49883) { __collection49860.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement49883)); } return __collection49860; }))())));
        if ((label__46838 is not null))
        {
            dialogChild__49672 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Semantics(scopesRoute: true, explicitChildNodes: true, namesRoute: true, label: label__46838, child: dialogChild__49672));
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new Dialog(backgroundColor: this.backgroundColor, elevation: this.elevation, shadowColor: this.shadowColor, surfaceTintColor: this.surfaceTintColor, insetPadding: this.insetPadding, clipBehavior: this.clipBehavior, shape: this.shape, alignment: this.alignment, constraints: this.constraints, child: dialogChild__49672));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class DialogLibrary
{
    internal static global::Doroti.Generated.Framework.Widgets.Widget _buildMaterialDialogTransitions(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return child;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _FullWindowDialogWrapper__dialog : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _FullWindowDialogWrapper__dialog(global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        this.child = child;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DialogThemeData windowDialogTheme__50951 = DialogTheme.of(context).copyWith(insetPadding: global::Doroti.Generated.Framework.Painting.EdgeInsets.zero, shape: new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(), alignment: global::Doroti.Generated.Framework.Painting.Alignment.topLeft, constraints: global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateExpand());
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new DialogTheme(data: windowDialogTheme__50951, child: global::Doroti.Generated.Framework.Widgets.MediaQuery.CreateRemoveViewInsets(removeLeft: true, removeTop: true, removeRight: true, removeBottom: true, context: context, child: global::Doroti.Generated.Framework.Widgets.MediaQuery.CreateRemoveViewPadding(removeLeft: true, removeTop: true, removeRight: true, removeBottom: true, context: context, child: this.child))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DialogPopScope__dialog : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual global::System.Action<object>? onPop { get; private set; }

    internal _DialogPopScope__dialog(global::Doroti.Generated.Framework.Widgets.Widget child, global::System.Action<object>? onPop = null)
    {
        this.child = child;
        this.onPop = onPop;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.PopScope<object>(canPop: false, onPopInvokedWithResult: ((global::System.Action<bool, object>)((didPop, result) => {
if (!didPop)
{
    this.onPop?.Invoke(result);
}
})), child: new global::Doroti.Generated.Framework.Widgets.Builder(builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>)((context) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _NavigatorShim__dialog(onPop: (global::System.Action<object>?)this.onPop, child: this.child));
throw new InvalidOperationException("Dart closure completed without a value.");
})))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _NavigatorShim__dialog : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::System.Action<object>? onPop { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _NavigatorShim__dialog(global::Doroti.Generated.Framework.Widgets.Widget child, global::System.Action<object>? onPop = null)
    {
        this.child = child;
        this.onPop = onPop;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)global::Doroti.Generated.Framework.Widgets.HeroControllerScope.CreateNone(child: new global::Doroti.Generated.Framework.Widgets.Navigator(pages: new List<global::Doroti.Generated.Framework.Widgets.Page<object?>> { new _DialogContentPage__dialog(child: this.child) }.Cast<global::Doroti.Generated.Framework.Widgets.Page<object>>().ToList(), onPopPage: ((global::System.Func<dynamic, object, bool>?)((route, result) => {
this.onPop?.Invoke(result);
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
})))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DialogContentPage__dialog : global::Doroti.Generated.Framework.Widgets.Page<object?>
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _DialogContentPage__dialog(global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        this.child = child;
    }

    public override global::Doroti.Generated.Framework.Widgets.Route<object?> createRoute(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Route<object?>)(object?)new global::Doroti.Generated.Framework.Widgets.PageRouteBuilder<object?>(settings: this, pageBuilder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.Widget>)((context, animation, secondaryAnimation) => {
return this.child;
throw new InvalidOperationException("Dart closure completed without a value.");
})), transitionDuration: Duration.zero, reverseTransitionDuration: Duration.zero));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class DialogLibrary
{
    public static Future<T?> showDialog<T>(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget> builder, bool barrierDismissible = true, Color? barrierColor = null, string? barrierLabel = null, bool useSafeArea = true, bool useRootNavigator = true, global::Doroti.Generated.Framework.Widgets.RouteSettings? routeSettings = null, Offset? anchorPoint = null, global::Doroti.Generated.Framework.Widgets.TraversalEdgeBehavior? traversalEdgeBehavior = null, bool fullscreenDialog = false, bool? requestFocus = null, global::Doroti.Generated.Framework.Animation.AnimationStyle? animationStyle = null)
    {
        DartRuntimePrimitives.Assert(() => DialogLibrary._debugIsActive(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        global::Doroti.Generated.Framework.Widgets.CapturedThemes themes__60281 = ((global::Doroti.Generated.Framework.Widgets.CapturedThemes)(object?)InheritedTheme.capture(from: context, to: Navigator.of(context, rootNavigator: useRootNavigator).context));
        global::Doroti.Generated.Framework.Widgets.NavigatorState navigator__60433 = ((global::Doroti.Generated.Framework.Widgets.NavigatorState)(object?)Navigator.of(context, rootNavigator: useRootNavigator));
        return global::Doroti.Generated.Framework.Widgets.DialogLibrary.showRawDialog(context: context, useRootNavigator: useRootNavigator, routeSettings: routeSettings, fullscreenDialog: fullscreenDialog, routeBuilder: ((routeContext, _) => {
return new DialogRoute<T>(context: routeContext, builder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>)builder, barrierColor: (((barrierColor ?? DialogTheme.of(context).barrierColor) ?? Theme.of(context).dialogTheme.barrierColor) ?? Colors.black54), barrierDismissible: barrierDismissible, barrierLabel: barrierLabel, useSafeArea: useSafeArea, settings: routeSettings, themes: themes__60281, anchorPoint: anchorPoint, traversalEdgeBehavior: (traversalEdgeBehavior ?? global::Doroti.Generated.Framework.Widgets.TraversalEdgeBehavior.closedLoop), requestFocus: requestFocus, animationStyle: animationStyle, fullscreenDialog: fullscreenDialog);
throw new InvalidOperationException("Dart closure completed without a value.");
}), builder: ((routeContext) => {
global::Doroti.Ui.TextDirection textDirection__61736 = Directionality.of(context);
ThemeData themeData__61802 = Theme.of(context);
global::Doroti.Generated.Framework.Widgets.MediaQueryData mediaQuery__61860 = ((global::Doroti.Generated.Framework.Widgets.MediaQueryData)(object?)MediaQuery.of(context));
global::Doroti.Generated.Framework.Widgets.Widget dialogContent__61916 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _DialogPopScope__dialog(onPop: Navigator.of(navigator__60433.context).pop, child: new global::Doroti.Generated.Framework.Widgets.Builder(builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>)((innerContext) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _FullWindowDialogWrapper__dialog(child: builder(innerContext)));
throw new InvalidOperationException("Dart closure completed without a value.");
})))));
return new global::Doroti.Generated.Framework.Widgets.Directionality(textDirection: textDirection__61736, child: new Theme(data: themeData__61802, child: new global::Doroti.Generated.Framework.Widgets.MediaQuery(data: mediaQuery__61860, child: dialogContent__61916)));
throw new InvalidOperationException("Dart closure completed without a value.");
}));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class DialogLibrary
{
    public static Future<T?> showAdaptiveDialog<T>(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget> builder, bool? barrierDismissible = null, Color? barrierColor = null, string? barrierLabel = null, bool useSafeArea = true, bool useRootNavigator = true, global::Doroti.Generated.Framework.Widgets.RouteSettings? routeSettings = null, Offset? anchorPoint = null, global::Doroti.Generated.Framework.Widgets.TraversalEdgeBehavior? traversalEdgeBehavior = null, bool? requestFocus = null, global::Doroti.Generated.Framework.Animation.AnimationStyle? animationStyle = null)
    {
        ThemeData theme__63173 = Theme.of(context);
        switch (theme__63173.platform)
        {
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
                {
                    return DialogLibrary.showDialog<T>(context: context, builder: builder, barrierDismissible: (barrierDismissible ?? true), barrierColor: barrierColor, barrierLabel: barrierLabel, useSafeArea: useSafeArea, useRootNavigator: useRootNavigator, routeSettings: routeSettings, anchorPoint: anchorPoint, traversalEdgeBehavior: traversalEdgeBehavior, requestFocus: requestFocus, animationStyle: animationStyle);
                }
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
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
    internal static bool _debugIsActive(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        if (((context is global::Doroti.Generated.Framework.Widgets.Element) && !((global::Doroti.Generated.Framework.Widgets.Element)context).debugIsActive))
        {
            global::Doroti.Generated.Framework.Widgets.Element context__as64315 = (global::Doroti.Generated.Framework.Widgets.Element)context;
            throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("This BuildContext is no longer valid."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("The showDialog function context parameter is a BuildContext that is no longer valid."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("This can commonly occur when the showDialog function is called after awaiting a Future. " + "In this situation the BuildContext might refer to a widget that has already been disposed during the await. " + "Consider using a parent context instead.") }));
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class DialogRoute<T> : global::Doroti.Generated.Framework.Widgets.RawDialogRoute<T>
{
    internal virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation? _curvedAnimation { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationStyle? _animationStyle { get; private set; }

    public DialogRoute(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget> builder, global::Doroti.Generated.Framework.Widgets.CapturedThemes? themes = null, Color? barrierColor = default!, bool barrierDismissible = true, string? barrierLabel = null, bool useSafeArea = true, global::Doroti.Generated.Framework.Widgets.RouteSettings? settings = null, bool? requestFocus = null, Offset? anchorPoint = null, global::Doroti.Generated.Framework.Widgets.TraversalEdgeBehavior? traversalEdgeBehavior = null, bool fullscreenDialog = false, global::Doroti.Generated.Framework.Animation.AnimationStyle? animationStyle = null) : base(barrierColor: barrierColor ?? Colors.black54, barrierDismissible: barrierDismissible, settings: settings, requestFocus: requestFocus, anchorPoint: anchorPoint, traversalEdgeBehavior: traversalEdgeBehavior, fullscreenDialog: fullscreenDialog, pageBuilder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.Widget>)((buildContext, animation, secondaryAnimation) => {
global::Doroti.Generated.Framework.Widgets.Widget pageChild__68121 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Builder(builder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>)builder));
global::Doroti.Generated.Framework.Widgets.Widget dialog__68182 = (themes?.wrap(pageChild__68121) ?? pageChild__68121);
if (useSafeArea)
{
    dialog__68182 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SafeArea(child: dialog__68182));
}
dialog__68182 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Semantics(hitTestBehavior: SemanticsHitTestBehavior.opaque, child: dialog__68182));
return dialog__68182;
throw new InvalidOperationException("Dart closure completed without a value.");
})), barrierLabel: (barrierLabel ?? MaterialLocalizations.of(context).modalBarrierDismissLabel), transitionDuration: (animationStyle?.duration ?? Duration.Create(milliseconds: 150L)), transitionBuilder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Widgets.Widget>)DialogLibrary._buildMaterialDialogTransitions)
    {
        this._animationStyle = animationStyle;
    }

    internal virtual void _setAnimation(global::Doroti.Generated.Framework.Animation.Animation<double> animation)
    {
        if ((!object.Equals(this._curvedAnimation?.parent, animation)))
        {
            this._curvedAnimation?.dispose();
            _curvedAnimation = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: animation, curve: (this._animationStyle?.curve ?? global::Doroti.Generated.Framework.Animation.Curves.easeOut), reverseCurve: (this._animationStyle?.reverseCurve ?? global::Doroti.Generated.Framework.Animation.Curves.easeOut));
        }
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget buildTransitions(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        _setAnimation(animation);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: this._curvedAnimation!, child: base.buildTransitions(context, animation, secondaryAnimation, child)));
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
        double clampedTextScaleFactor__69764 = Dart_uiLibrary.clampDouble(textScaleFactor, 1.0, 2.0);
        return DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(1.0, (1.0 / 3.0), (clampedTextScaleFactor__69764 - 1.0)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _DialogDefaultsM2__dialog : DialogThemeData
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;
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
    private global::Doroti.Generated.Framework.Widgets.IconThemeData __late_iconTheme = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.IconThemeData iconTheme
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

    internal _DialogDefaultsM2__dialog(global::Doroti.Generated.Framework.Widgets.BuildContext context) : base(alignment: global::Doroti.Generated.Framework.Painting.Alignment.center, elevation: 24.0, shape: new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(4.0))), clipBehavior: Clip.none)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? iconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((global::Doroti.Generated.Framework.Widgets.IconThemeData)this.iconTheme).color);
    public virtual global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((object.Equals(this.theme.brightness, Brightness.dark)) ? Colors.grey[800L]! : Colors.white));
    public virtual global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this.theme.shadowColor);
    public override global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle => this.textTheme.titleLarge;
    public override global::Doroti.Generated.Framework.Painting.TextStyle? contentTextStyle => this.textTheme.titleMedium;
    public override global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? actionsPadding => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Generated.Framework.Painting.EdgeInsets.zero);
}

internal class _DialogFullscreenDefaultsM3__dialog : DialogThemeData
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;

    internal _DialogFullscreenDefaultsM3__dialog(global::Doroti.Generated.Framework.Widgets.BuildContext context) : base(clipBehavior: Clip.none)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Theme.of(this.context).colorScheme.surface);
}

internal class _DialogDefaultsM3__dialog : DialogThemeData
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;
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

    internal _DialogDefaultsM3__dialog(global::Doroti.Generated.Framework.Widgets.BuildContext context) : base(alignment: global::Doroti.Generated.Framework.Painting.Alignment.center, elevation: 6.0, shape: new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(28.0))), clipBehavior: Clip.none)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? iconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.secondary);
    public virtual global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.surfaceContainerHigh);
    public virtual global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public virtual global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public override global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle => this._textTheme.headlineSmall;
    public override global::Doroti.Generated.Framework.Painting.TextStyle? contentTextStyle => this._textTheme.bodyMedium;
    public override global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? actionsPadding => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(left: 24.0, right: 24.0, bottom: 24.0));
}
