// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/dialog.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class DialogLibrary
{
    internal static EdgeInsets _defaultInsetPadding = EdgeInsets.CreateSymmetric(
        horizontal: 40.0,
        vertical: 24.0
    );
}

public class Dialog : StatelessWidget
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual Duration insetAnimationDuration { get; private set; } = default!;
    public virtual Curve insetAnimationCurve { get; private set; } = default!;
    public virtual EdgeInsets? insetPadding { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual ShapeBorder? shape { get; private set; }
    public virtual AlignmentGeometry? alignment { get; private set; }
    public virtual Widget? child { get; private set; }
    internal virtual bool _fullscreen { get; private set; } = default!;
    public virtual SemanticsRole semanticsRole { get; private set; } = default!;
    public virtual BoxConstraints? constraints { get; private set; }

    public Dialog(
        Key? key = null,
        Color? backgroundColor = null,
        double? elevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        Duration? insetAnimationDuration = null,
        Curve insetAnimationCurve = default!,
        EdgeInsets? insetPadding = null,
        Clip? clipBehavior = null,
        ShapeBorder? shape = null,
        AlignmentGeometry? alignment = null,
        Widget? child = null,
        SemanticsRole semanticsRole = SemanticsRole.dialog,
        BoxConstraints? constraints = null
    )
        : base(key: key)
    {
        Duration __insetAnimationDuration =
            insetAnimationDuration ?? Duration.Create(milliseconds: 100);
        Curve __insetAnimationCurve = insetAnimationCurve ?? Curves.decelerate;
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
        _fullscreen = false;
        System.Diagnostics.Debug.Assert((elevation is null) || (elevation >= 0.0));
    }

    public static Dialog CreateFullscreen(
        Key? key = null,
        Color? backgroundColor = null,
        Duration insetAnimationDuration = default,
        Curve insetAnimationCurve = default!,
        Widget? child = null,
        SemanticsRole semanticsRole = SemanticsRole.dialog
    )
    {
        var __instance = new Dialog(
            key: key,
            backgroundColor: backgroundColor,
            insetAnimationDuration: insetAnimationDuration,
            insetAnimationCurve: insetAnimationCurve,
            child: child,
            semanticsRole: semanticsRole
        );
        Curve __insetAnimationCurve = insetAnimationCurve ?? Curves.decelerate;
        __instance.backgroundColor = backgroundColor;
        __instance.insetAnimationDuration = insetAnimationDuration;
        __instance.insetAnimationCurve = __insetAnimationCurve;
        __instance.child = child;
        __instance.semanticsRole = semanticsRole;
        __instance.elevation = 0;
        __instance.shadowColor = null;
        __instance.surfaceTintColor = null;
        __instance.insetPadding = EdgeInsets.zero;
        __instance.clipBehavior = Clip.none;
        __instance.shape = null;
        __instance.alignment = null;
        __instance.constraints = null;
        __instance._fullscreen = true;
        return __instance;
    }

    public override Widget build(BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        DialogThemeData dialogTheme = DialogTheme.of(context);
        EdgeInsets effectivePadding = MediaQuery
            .viewInsetsOf(context)
            .op_Add(
                (insetPadding ?? dialogTheme.insetPadding) ?? DialogLibrary._defaultInsetPadding
            );
        DialogThemeData defaults = _fullscreen
            ? new _DialogFullscreenDefaultsM3__dialog(context)
            : new _DialogDefaultsM3__dialog(context);
        BoxConstraints boxConstraints =
            (constraints ?? dialogTheme.constraints) ?? new BoxConstraints(minWidth: 280.0);
        Widget dialogChild = default!;
        if (_fullscreen)
        {
            dialogChild = DartRuntimePrimitives.ConvertValue<Widget>(
                new Material(
                    color: (backgroundColor ?? dialogTheme.backgroundColor)
                        ?? defaults.backgroundColor,
                    child: child
                )
            );
        }
        else
        {
            dialogChild = DartRuntimePrimitives.ConvertValue<Widget>(
                new Align(
                    alignment: (alignment ?? dialogTheme.alignment) ?? defaults.alignment!,
                    child: new ConstrainedBox(
                        constraints: boxConstraints,
                        child: new Material(
                            color: (backgroundColor ?? dialogTheme.backgroundColor)
                                ?? defaults.backgroundColor,
                            elevation: (elevation ?? dialogTheme.elevation)
                                ?? (
                                    defaults.elevation
                                    ?? throw new global::System.NullReferenceException(
                                        "A required value was null."
                                    )
                                ),
                            shadowColor: (shadowColor ?? dialogTheme.shadowColor)
                                ?? defaults.shadowColor,
                            surfaceTintColor: (surfaceTintColor ?? dialogTheme.surfaceTintColor)
                                ?? defaults.surfaceTintColor,
                            shape: (shape ?? dialogTheme.shape) ?? defaults.shape!,
                            type: MaterialType.card,
                            clipBehavior: (clipBehavior ?? dialogTheme.clipBehavior)
                                ?? (
                                    defaults.clipBehavior
                                    ?? throw new global::System.NullReferenceException(
                                        "A required value was null."
                                    )
                                ),
                            child: child
                        )
                    )
                )
            );
        }
        return new Widgets.Semantics(
            role: semanticsRole,
            child: new AnimatedPadding(
                padding: effectivePadding,
                duration: (insetAnimationDuration),
                curve: insetAnimationCurve,
                child: MediaQuery.CreateRemoveViewInsets(
                    removeLeft: true,
                    removeTop: true,
                    removeRight: true,
                    removeBottom: true,
                    context: context,
                    child: dialogChild
                )
            )
        );
    }
}

public class AlertDialog : StatelessWidget
{
    public virtual Widget? icon { get; private set; }
    public virtual Color? iconColor { get; private set; }
    public virtual EdgeInsetsGeometry? iconPadding { get; private set; }
    public virtual Widget? title { get; private set; }
    public virtual EdgeInsetsGeometry? titlePadding { get; private set; }
    public virtual TextStyle? titleTextStyle { get; private set; }
    public virtual Widget? content { get; private set; }
    public virtual EdgeInsetsGeometry? contentPadding { get; private set; }
    public virtual TextStyle? contentTextStyle { get; private set; }
    public virtual List<Widget>? actions { get; private set; }
    public virtual EdgeInsetsGeometry? actionsPadding { get; private set; }
    public virtual MainAxisAlignment? actionsAlignment { get; private set; }
    public virtual OverflowBarAlignment? actionsOverflowAlignment { get; private set; }
    public virtual VerticalDirection? actionsOverflowDirection { get; private set; }
    public virtual double? actionsOverflowButtonSpacing { get; private set; }
    public virtual EdgeInsetsGeometry? buttonPadding { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual string? semanticLabel { get; private set; }
    public virtual EdgeInsets? insetPadding { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual ShapeBorder? shape { get; private set; }
    public virtual AlignmentGeometry? alignment { get; private set; }
    public virtual BoxConstraints? constraints { get; private set; }
    public virtual bool scrollable { get; private set; } = default!;

    public AlertDialog(
        Key? key = null,
        Widget? icon = null,
        EdgeInsetsGeometry? iconPadding = null,
        Color? iconColor = null,
        Widget? title = null,
        EdgeInsetsGeometry? titlePadding = null,
        TextStyle? titleTextStyle = null,
        Widget? content = null,
        EdgeInsetsGeometry? contentPadding = null,
        TextStyle? contentTextStyle = null,
        List<Widget>? actions = null,
        EdgeInsetsGeometry? actionsPadding = null,
        MainAxisAlignment? actionsAlignment = null,
        OverflowBarAlignment? actionsOverflowAlignment = null,
        VerticalDirection? actionsOverflowDirection = null,
        double? actionsOverflowButtonSpacing = null,
        EdgeInsetsGeometry? buttonPadding = null,
        Color? backgroundColor = null,
        double? elevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        string? semanticLabel = null,
        EdgeInsets? insetPadding = null,
        Clip? clipBehavior = null,
        ShapeBorder? shape = null,
        AlignmentGeometry? alignment = null,
        BoxConstraints? constraints = null,
        bool scrollable = false
    )
        : base(key: key)
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

    public static AlertDialog CreateAdaptive(
        Key? key = null,
        Widget? icon = null,
        EdgeInsetsGeometry? iconPadding = null,
        Color? iconColor = null,
        Widget? title = null,
        EdgeInsetsGeometry? titlePadding = null,
        TextStyle? titleTextStyle = null,
        Widget? content = null,
        EdgeInsetsGeometry? contentPadding = null,
        TextStyle? contentTextStyle = null,
        List<Widget>? actions = null,
        EdgeInsetsGeometry? actionsPadding = null,
        MainAxisAlignment? actionsAlignment = null,
        OverflowBarAlignment? actionsOverflowAlignment = null,
        VerticalDirection? actionsOverflowDirection = null,
        double? actionsOverflowButtonSpacing = null,
        EdgeInsetsGeometry? buttonPadding = null,
        Color? backgroundColor = null,
        double? elevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        string? semanticLabel = null,
        EdgeInsets insetPadding = default!,
        Clip? clipBehavior = null,
        ShapeBorder? shape = null,
        AlignmentGeometry? alignment = null,
        BoxConstraints? constraints = null,
        bool scrollable = default!,
        ScrollController? scrollController = null,
        ScrollController? actionScrollController = null,
        Duration insetAnimationDuration = default!,
        Curve insetAnimationCurve = default!
    ) =>
        new _AdaptiveAlertDialog__dialog(
            key,
            icon,
            iconPadding,
            iconColor,
            title,
            titlePadding,
            titleTextStyle,
            content,
            contentPadding,
            contentTextStyle,
            actions,
            actionsPadding,
            actionsAlignment,
            actionsOverflowAlignment,
            actionsOverflowDirection,
            actionsOverflowButtonSpacing,
            buttonPadding,
            backgroundColor,
            elevation,
            shadowColor,
            surfaceTintColor,
            semanticLabel,
            insetPadding,
            clipBehavior,
            shape,
            alignment,
            constraints,
            scrollable,
            scrollController,
            actionScrollController,
            insetAnimationDuration,
            insetAnimationCurve
        );

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            DebugLibrary.debugCheckHasMaterialLocalizations(context)
        );
        ThemeData theme = Theme.of(context);
        DialogThemeData dialogTheme = DialogTheme.of(context);
        DialogThemeData defaults = new _DialogDefaultsM3__dialog(context);
        string? labelLocal = PlatformLibrary.defaultTargetPlatform switch
        {
            TargetPlatform.iOS => semanticLabel,
            TargetPlatform.macOS => semanticLabel,
            TargetPlatform.android or TargetPlatform.fuchsia or TargetPlatform.linux =>
                semanticLabel ?? MaterialLocalizations.of(context).alertDialogLabel,
            TargetPlatform.windows => semanticLabel
                ?? MaterialLocalizations.of(context).alertDialogLabel,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        var fontSizeToScale = 14.0;
        double effectiveTextScale =
            MediaQuery.textScalerOf(context).scale(fontSizeToScale) / fontSizeToScale;
        double paddingScaleFactor = DialogLibrary._scalePadding(effectiveTextScale);
        TextDirection? textDirection = Directionality.maybeOf(context);
        Widget? iconWidget = default!;
        Widget? titleWidget = default!;
        Widget? contentWidget = default!;
        Widget? actionsWidget = default!;
        if (icon is not null)
        {
            var belowIsTitle = title is not null;
            bool belowIsContent = !belowIsTitle && (content is not null);
            var defaultIconPadding = EdgeInsets.CreateOnly(
                left: 24.0,
                top: 24.0,
                right: 24.0,
                bottom: belowIsTitle ? 16.0 : (belowIsContent ? 0.0 : 24.0)
            );
            EdgeInsets effectiveIconPadding =
                iconPadding?.resolve(textDirection) ?? defaultIconPadding;
            iconWidget = DartRuntimePrimitives.ConvertValue<Widget>(
                new Padding(
                    padding: EdgeInsets.CreateOnly(
                        left: effectiveIconPadding.left * paddingScaleFactor,
                        right: effectiveIconPadding.right * paddingScaleFactor,
                        top: effectiveIconPadding.top * paddingScaleFactor,
                        bottom: effectiveIconPadding.bottom
                    ),
                    child: new IconTheme(
                        data: new IconThemeData(
                            color: (iconColor ?? dialogTheme.iconColor) ?? defaults.iconColor
                        ),
                        child: icon!
                    )
                )
            );
        }
        if (title is not null)
        {
            var defaultTitlePadding = EdgeInsets.CreateOnly(
                left: 24.0,
                top: (icon is null) ? 24.0 : 0.0,
                right: 24.0,
                bottom: (content is null) ? 20.0 : 0.0
            );
            EdgeInsets effectiveTitlePadding =
                titlePadding?.resolve(textDirection) ?? defaultTitlePadding;
            titleWidget = DartRuntimePrimitives.ConvertValue<Widget>(
                new Padding(
                    padding: EdgeInsets.CreateOnly(
                        left: effectiveTitlePadding.left * paddingScaleFactor,
                        right: effectiveTitlePadding.right * paddingScaleFactor,
                        top: (icon is null)
                            ? (effectiveTitlePadding.top * paddingScaleFactor)
                            : effectiveTitlePadding.top,
                        bottom: effectiveTitlePadding.bottom
                    ),
                    child: new DefaultTextStyle(
                        style: (titleTextStyle ?? dialogTheme.titleTextStyle)
                            ?? defaults.titleTextStyle!,
                        textAlign: (icon is null) ? TextAlign.start : TextAlign.center,
                        child: new Widgets.Semantics(
                            namesRoute: (labelLocal is null)
                                && (
                                    !Equals(
                                        PlatformLibrary.defaultTargetPlatform,
                                        TargetPlatform.iOS
                                    )
                                ),
                            container: true,
                            child: title
                        )
                    )
                )
            );
        }
        if (content is not null)
        {
            var defaultContentPadding = EdgeInsets.CreateOnly(
                left: 24.0,
                top: 16.0,
                right: 24.0,
                bottom: 24.0
            );
            EdgeInsets effectiveContentPadding =
                contentPadding?.resolve(textDirection) ?? defaultContentPadding;
            contentWidget = DartRuntimePrimitives.ConvertValue<Widget>(
                new Padding(
                    padding: EdgeInsets.CreateOnly(
                        left: effectiveContentPadding.left * paddingScaleFactor,
                        right: effectiveContentPadding.right * paddingScaleFactor,
                        top: ((title is null) && (icon is null))
                            ? (effectiveContentPadding.top * paddingScaleFactor)
                            : effectiveContentPadding.top,
                        bottom: effectiveContentPadding.bottom
                    ),
                    child: new DefaultTextStyle(
                        style: (contentTextStyle ?? dialogTheme.contentTextStyle)
                            ?? defaults.contentTextStyle!,
                        child: new Widgets.Semantics(
                            container: true,
                            explicitChildNodes: true,
                            child: content
                        )
                    )
                )
            );
        }
        if (actions is not null)
        {
            double spacingLocal = (buttonPadding?.horizontal ?? 16) / 2L;
            actionsWidget = DartRuntimePrimitives.ConvertValue<Widget>(
                new Padding(
                    padding: (actionsPadding ?? dialogTheme.actionsPadding)
                        ?? defaults.actionsPadding!,
                    child: new OverflowBar(
                        alignment: actionsAlignment ?? MainAxisAlignment.end,
                        spacing: spacingLocal,
                        overflowAlignment: actionsOverflowAlignment ?? OverflowBarAlignment.end,
                        overflowDirection: actionsOverflowDirection ?? VerticalDirection.down,
                        overflowSpacing: actionsOverflowButtonSpacing ?? 0,
                        children: actions!
                    )
                )
            );
        }
        List<Widget> columnChildren = default!;
        if (scrollable)
        {
            columnChildren = (
                (Func<List<Widget>>)(
                    () =>
                    {
                        var __collection34259 = new List<Widget>();
                        if ((title is not null) || (content is not null))
                        {
                            __collection34259.Add(
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    new Flexible(
                                        child: new SingleChildScrollView(
                                            child: new Column(
                                                mainAxisSize: MainAxisSize.min,
                                                crossAxisAlignment: CrossAxisAlignment.stretch,
                                                children: (
                                                    (Func<List<Widget>>)(
                                                        () =>
                                                        {
                                                            var __collection34544 =
                                                                new List<Widget>();
                                                            var __collectionElement34553 =
                                                                iconWidget;
                                                            if (
                                                                __collectionElement34553 is
                                                                { } __nonNullCollectionElement34553
                                                            )
                                                            {
                                                                __collection34544.Add(
                                                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                                                        __nonNullCollectionElement34553
                                                                    )
                                                                );
                                                            }
                                                            var __collectionElement34566 =
                                                                titleWidget;
                                                            if (
                                                                __collectionElement34566 is
                                                                { } __nonNullCollectionElement34566
                                                            )
                                                            {
                                                                __collection34544.Add(
                                                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                                                        __nonNullCollectionElement34566
                                                                    )
                                                                );
                                                            }
                                                            var __collectionElement34580 =
                                                                contentWidget;
                                                            if (
                                                                __collectionElement34580 is
                                                                { } __nonNullCollectionElement34580
                                                            )
                                                            {
                                                                __collection34544.Add(
                                                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                                                        __nonNullCollectionElement34580
                                                                    )
                                                                );
                                                            }
                                                            return __collection34544;
                                                        }
                                                    )
                                                )()
                                            )
                                        )
                                    )
                                )
                            );
                        }
                        var __collectionElement34650 = actionsWidget;
                        if (__collectionElement34650 is { } __nonNullCollectionElement34650)
                        {
                            __collection34259.Add(
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    __nonNullCollectionElement34650
                                )
                            );
                        }
                        return __collection34259;
                    }
                )
            )();
        }
        else
        {
            columnChildren = (
                (Func<List<Widget>>)(
                    () =>
                    {
                        var __collection34711 = new List<Widget>();
                        var __collectionElement34729 = iconWidget;
                        if (__collectionElement34729 is { } __nonNullCollectionElement34729)
                        {
                            __collection34711.Add(
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    __nonNullCollectionElement34729
                                )
                            );
                        }
                        var __collectionElement34750 = titleWidget;
                        if (__collectionElement34750 is { } __nonNullCollectionElement34750)
                        {
                            __collection34711.Add(
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    __nonNullCollectionElement34750
                                )
                            );
                        }
                        if (contentWidget is not null)
                        {
                            __collection34711.Add(
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    new Flexible(child: contentWidget)
                                )
                            );
                        }
                        var __collectionElement34839 = actionsWidget;
                        if (__collectionElement34839 is { } __nonNullCollectionElement34839)
                        {
                            __collection34711.Add(
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    __nonNullCollectionElement34839
                                )
                            );
                        }
                        return __collection34711;
                    }
                )
            )();
        }
        Widget dialogChild = new IntrinsicWidth(
            child: new Column(
                mainAxisSize: MainAxisSize.min,
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: columnChildren
            )
        );
        if (labelLocal is not null)
        {
            dialogChild = DartRuntimePrimitives.ConvertValue<Widget>(
                new Widgets.Semantics(
                    scopesRoute: true,
                    explicitChildNodes: true,
                    namesRoute: true,
                    label: labelLocal,
                    child: dialogChild
                )
            );
        }
        return new Dialog(
            backgroundColor: backgroundColor,
            elevation: elevation,
            shadowColor: shadowColor,
            surfaceTintColor: surfaceTintColor,
            insetPadding: insetPadding,
            clipBehavior: clipBehavior,
            shape: shape,
            alignment: alignment,
            constraints: constraints,
            semanticsRole: SemanticsRole.alertDialog,
            child: dialogChild
        );
    }
}

internal class _AdaptiveAlertDialog__dialog : AlertDialog
{
    public virtual ScrollController? scrollController { get; private set; }
    public virtual ScrollController? actionScrollController { get; private set; }
    public virtual Duration insetAnimationDuration { get; private set; } = default!;
    public virtual Curve insetAnimationCurve { get; private set; } = default!;

    internal _AdaptiveAlertDialog__dialog(
        Key? key = null,
        Widget? icon = null,
        EdgeInsetsGeometry? iconPadding = null,
        Color? iconColor = null,
        Widget? title = null,
        EdgeInsetsGeometry? titlePadding = null,
        TextStyle? titleTextStyle = null,
        Widget? content = null,
        EdgeInsetsGeometry? contentPadding = null,
        TextStyle? contentTextStyle = null,
        List<Widget>? actions = null,
        EdgeInsetsGeometry? actionsPadding = null,
        MainAxisAlignment? actionsAlignment = null,
        OverflowBarAlignment? actionsOverflowAlignment = null,
        VerticalDirection? actionsOverflowDirection = null,
        double? actionsOverflowButtonSpacing = null,
        EdgeInsetsGeometry? buttonPadding = null,
        Color? backgroundColor = null,
        double? elevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        string? semanticLabel = null,
        EdgeInsets? insetPadding = null,
        Clip? clipBehavior = null,
        ShapeBorder? shape = null,
        AlignmentGeometry? alignment = null,
        BoxConstraints? constraints = null,
        bool scrollable = false,
        ScrollController? scrollController = null,
        ScrollController? actionScrollController = null,
        Duration? insetAnimationDuration = null,
        Curve insetAnimationCurve = default!
    )
        : base(
            key: key,
            icon: icon,
            iconPadding: iconPadding,
            iconColor: iconColor,
            title: title,
            titlePadding: titlePadding,
            titleTextStyle: titleTextStyle,
            content: content,
            contentPadding: contentPadding,
            contentTextStyle: contentTextStyle,
            actions: actions,
            actionsPadding: actionsPadding,
            actionsAlignment: actionsAlignment,
            actionsOverflowAlignment: actionsOverflowAlignment,
            actionsOverflowDirection: actionsOverflowDirection,
            actionsOverflowButtonSpacing: actionsOverflowButtonSpacing,
            buttonPadding: buttonPadding,
            backgroundColor: backgroundColor,
            elevation: elevation,
            shadowColor: shadowColor,
            surfaceTintColor: surfaceTintColor,
            semanticLabel: semanticLabel,
            insetPadding: insetPadding,
            clipBehavior: clipBehavior,
            shape: shape,
            alignment: alignment,
            constraints: constraints,
            scrollable: scrollable
        )
    {
        Duration __insetAnimationDuration =
            insetAnimationDuration ?? Duration.Create(milliseconds: 100);
        Curve __insetAnimationCurve = insetAnimationCurve ?? Curves.decelerate;
        this.scrollController = scrollController;
        this.actionScrollController = actionScrollController;
        this.insetAnimationDuration = __insetAnimationDuration;
        this.insetAnimationCurve = __insetAnimationCurve;
    }

    public override Widget build(BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        switch (theme.platform)
        {
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
            {
                break;
            }
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
            {
                return new CupertinoAlertDialog(
                    title: title,
                    content: content,
                    actions: actions ?? new List<Widget>(),
                    scrollController: scrollController,
                    actionScrollController: actionScrollController,
                    insetAnimationDuration: (insetAnimationDuration),
                    insetAnimationCurve: insetAnimationCurve
                );
            }
            default:
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                );
        }
        return base.build(context);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class SimpleDialogOption : StatelessWidget
{
    public virtual Action? onPressed { get; private set; }
    public virtual Widget? child { get; private set; }
    public virtual EdgeInsets? padding { get; private set; }

    public SimpleDialogOption(
        Key? key = null,
        Action? onPressed = null,
        EdgeInsets? padding = null,
        Widget? child = null
    )
        : base(key: key)
    {
        this.onPressed = onPressed;
        this.padding = padding;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return new InkWell(
            onTap: onPressed,
            child: new Padding(
                padding: padding ?? EdgeInsets.CreateSymmetric(vertical: 8.0, horizontal: 24.0),
                child: child
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class SimpleDialog : StatelessWidget
{
    public virtual Widget? title { get; private set; }
    public virtual EdgeInsetsGeometry titlePadding { get; private set; } = default!;
    public virtual TextStyle? titleTextStyle { get; private set; }
    public virtual List<Widget>? children { get; private set; }
    public virtual EdgeInsetsGeometry contentPadding { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual TextStyle? contentTextStyle { get; private set; }
    public virtual string? semanticLabel { get; private set; }
    public virtual EdgeInsets? insetPadding { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual ShapeBorder? shape { get; private set; }
    public virtual AlignmentGeometry? alignment { get; private set; }
    public virtual BoxConstraints? constraints { get; private set; }

    public SimpleDialog(
        Key? key = null,
        Widget? title = null,
        EdgeInsetsGeometry titlePadding = default!,
        TextStyle? titleTextStyle = null,
        List<Widget>? children = null,
        EdgeInsetsGeometry contentPadding = default!,
        TextStyle? contentTextStyle = null,
        Color? backgroundColor = null,
        double? elevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        string? semanticLabel = null,
        EdgeInsets? insetPadding = null,
        Clip? clipBehavior = null,
        ShapeBorder? shape = null,
        AlignmentGeometry? alignment = null,
        BoxConstraints? constraints = null
    )
        : base(key: key)
    {
        EdgeInsetsGeometry __titlePadding = titlePadding ?? new EdgeInsets(24.0, 24.0, 24.0, 0.0);
        EdgeInsetsGeometry __contentPadding =
            contentPadding ?? new EdgeInsets(0.0, 12.0, 0.0, 16.0);
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

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            DebugLibrary.debugCheckHasMaterialLocalizations(context)
        );
        ThemeData theme = Theme.of(context);
        DialogThemeData dialogTheme = DialogTheme.of(context);
        DialogThemeData defaults = new _DialogDefaultsM3__dialog(context);
        string? labelLocal = semanticLabel;
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.macOS:
            case TargetPlatform.iOS:
            {
                break;
            }
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
            {
                labelLocal ??= MaterialLocalizations.of(context).dialogLabel;
                break;
            }
        }
        TextStyle effectiveTitleTextStyle =
            (titleTextStyle ?? dialogTheme.titleTextStyle) ?? theme.textTheme.titleLarge!;
        double fontSizeLocal =
            effectiveTitleTextStyle.fontSize ?? Text_painterLibrary.kDefaultFontSize;
        double fontSizeToScale =
            (fontSizeLocal == 0.0) ? Text_painterLibrary.kDefaultFontSize : fontSizeLocal;
        double effectiveTextScale =
            MediaQuery.textScalerOf(context).scale(fontSizeToScale) / fontSizeToScale;
        double paddingScaleFactor = DialogLibrary._scalePadding(effectiveTextScale);
        TextDirection? textDirection = Directionality.maybeOf(context);
        Widget? titleWidget = default!;
        if (title is not null)
        {
            EdgeInsets effectiveTitlePadding = titlePadding.resolve(textDirection);
            titleWidget = DartRuntimePrimitives.ConvertValue<Widget>(
                new Padding(
                    padding: EdgeInsets.CreateOnly(
                        left: effectiveTitlePadding.left * paddingScaleFactor,
                        right: effectiveTitlePadding.right * paddingScaleFactor,
                        top: effectiveTitlePadding.top * paddingScaleFactor,
                        bottom: (children is null)
                            ? (effectiveTitlePadding.bottom * paddingScaleFactor)
                            : effectiveTitlePadding.bottom
                    ),
                    child: new DefaultTextStyle(
                        style: effectiveTitleTextStyle,
                        child: new Widgets.Semantics(
                            namesRoute: (labelLocal is null)
                                && (
                                    !Equals(
                                        PlatformLibrary.defaultTargetPlatform,
                                        TargetPlatform.iOS
                                    )
                                ),
                            container: true,
                            child: title
                        )
                    )
                )
            );
        }
        Widget? contentWidget = default!;
        if (children is not null)
        {
            EdgeInsets effectiveContentPadding = contentPadding.resolve(textDirection);
            contentWidget = DartRuntimePrimitives.ConvertValue<Widget>(
                new Flexible(
                    child: new SingleChildScrollView(
                        padding: EdgeInsets.CreateOnly(
                            left: effectiveContentPadding.left * paddingScaleFactor,
                            right: effectiveContentPadding.right * paddingScaleFactor,
                            top: (title is null)
                                ? (effectiveContentPadding.top * paddingScaleFactor)
                                : effectiveContentPadding.top,
                            bottom: effectiveContentPadding.bottom * paddingScaleFactor
                        ),
                        child: new DefaultTextStyle(
                            style: (contentTextStyle ?? dialogTheme.contentTextStyle)
                                ?? defaults.contentTextStyle!,
                            child: new ListBody(children: children!)
                        )
                    )
                )
            );
        }
        Widget dialogChild = new IntrinsicWidth(
            stepWidth: 56.0,
            child: new Column(
                mainAxisSize: MainAxisSize.min,
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: (
                    (Func<List<Widget>>)(
                        () =>
                        {
                            var __collection49860 = new List<Widget>();
                            var __collectionElement49869 = titleWidget;
                            if (__collectionElement49869 is { } __nonNullCollectionElement49869)
                            {
                                __collection49860.Add(
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        __nonNullCollectionElement49869
                                    )
                                );
                            }
                            var __collectionElement49883 = contentWidget;
                            if (__collectionElement49883 is { } __nonNullCollectionElement49883)
                            {
                                __collection49860.Add(
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        __nonNullCollectionElement49883
                                    )
                                );
                            }
                            return __collection49860;
                        }
                    )
                )()
            )
        );
        if (labelLocal is not null)
        {
            dialogChild = DartRuntimePrimitives.ConvertValue<Widget>(
                new Widgets.Semantics(
                    scopesRoute: true,
                    explicitChildNodes: true,
                    namesRoute: true,
                    label: labelLocal,
                    child: dialogChild
                )
            );
        }
        return new Dialog(
            backgroundColor: backgroundColor,
            elevation: elevation,
            shadowColor: shadowColor,
            surfaceTintColor: surfaceTintColor,
            insetPadding: insetPadding,
            clipBehavior: clipBehavior,
            shape: shape,
            alignment: alignment,
            constraints: constraints,
            child: dialogChild
        );
    }
}

public static partial class DialogLibrary
{
    internal static Widget _buildMaterialDialogTransitions(
        BuildContext context,
        Animation<double> animation,
        Animation<double> secondaryAnimation,
        Widget child
    )
    {
        return child;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _FullWindowDialogWrapper__dialog : StatelessWidget
{
    public virtual Widget child { get; private set; } = default!;

    internal _FullWindowDialogWrapper__dialog(Widget child)
    {
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        DialogThemeData windowDialogTheme = DialogTheme
            .of(context)
            .copyWith(
                insetPadding: EdgeInsets.zero,
                shape: new RoundedRectangleBorder(),
                alignment: Alignment.topLeft,
                constraints: BoxConstraints.CreateExpand()
            );
        return new DialogTheme(
            data: windowDialogTheme,
            child: MediaQuery.CreateRemoveViewInsets(
                removeLeft: true,
                removeTop: true,
                removeRight: true,
                removeBottom: true,
                context: context,
                child: MediaQuery.CreateRemoveViewPadding(
                    removeLeft: true,
                    removeTop: true,
                    removeRight: true,
                    removeBottom: true,
                    context: context,
                    child: child
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _DialogPopScope__dialog : StatelessWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual Action<object?>? onPop { get; private set; }

    internal _DialogPopScope__dialog(Widget child, Action<object?>? onPop = null)
    {
        this.child = child;
        this.onPop = onPop;
    }

    public override Widget build(BuildContext context)
    {
        return new PopScope<object>(
            canPop: false,
            onPopInvokedWithResult: (didPop, result) =>
            {
                if (!didPop)
                {
                    onPop?.Invoke(result);
                }
            },
            child: new Builder(
                builder: (context) =>
                {
                    return new _NavigatorShim__dialog(onPop: onPop, child: child);
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _NavigatorShim__dialog : StatelessWidget
{
    public virtual Action<object?>? onPop { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    internal _NavigatorShim__dialog(Widget child, Action<object?>? onPop = null)
    {
        this.child = child;
        this.onPop = onPop;
    }

    public override Widget build(BuildContext context)
    {
        return HeroControllerScope.CreateNone(
            child: new Navigator(
                pages: new List<Page<object?>> { new _DialogContentPage__dialog(child: child) }
                    .Cast<Page<object?>>()
                    .ToList(),
                onPopPage: (route, result) =>
                {
                    onPop?.Invoke(result);
                    return false;
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _DialogContentPage__dialog : Page<object?>
{
    public virtual Widget child { get; private set; } = default!;

    internal _DialogContentPage__dialog(Widget child)
    {
        this.child = child;
    }

    public override Route<object?> createRoute(BuildContext context)
    {
        return new PageRouteBuilder<object?>(
            settings: this,
            pageBuilder: (context, animation, secondaryAnimation) =>
            {
                return child;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            },
            transitionDuration: Duration.zero,
            reverseTransitionDuration: Duration.zero
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class DialogLibrary
{
    public static Future<T?> showDialog<T>(
        BuildContext context,
        Func<BuildContext, Widget> builder,
        bool barrierDismissible = true,
        Color? barrierColor = null,
        string? barrierLabel = null,
        bool useSafeArea = true,
        bool useRootNavigator = true,
        RouteSettings? routeSettings = null,
        Offset? anchorPoint = null,
        TraversalEdgeBehavior? traversalEdgeBehavior = null,
        bool fullscreenDialog = false,
        bool? requestFocus = null,
        AnimationStyle? animationStyle = null
    )
    {
        DartRuntimePrimitives.Assert(() => _debugIsActive(context));
        DartRuntimePrimitives.Assert(() =>
            DebugLibrary.debugCheckHasMaterialLocalizations(context)
        );
        CapturedThemes themesLocal = InheritedTheme.capture(
            from: context,
            to: Navigator.of(context, rootNavigator: useRootNavigator).context
        );
        NavigatorState navigator = Navigator.of(context, rootNavigator: useRootNavigator);
        return Widgets.DialogLibrary.showRawDialog(
            context: context,
            useRootNavigator: useRootNavigator,
            routeSettings: routeSettings,
            fullscreenDialog: fullscreenDialog,
            routeBuilder: (routeContext, _) =>
            {
                return new DialogRoute<T>(
                    context: routeContext,
                    builder: builder,
                    barrierColor: (
                        (barrierColor ?? DialogTheme.of(context).barrierColor)
                        ?? Theme.of(context).dialogTheme.barrierColor
                    ) ?? Colors.black54,
                    barrierDismissible: barrierDismissible,
                    barrierLabel: barrierLabel,
                    useSafeArea: useSafeArea,
                    settings: routeSettings,
                    themes: themesLocal,
                    anchorPoint: anchorPoint,
                    traversalEdgeBehavior: traversalEdgeBehavior
                        ?? TraversalEdgeBehavior.closedLoop,
                    requestFocus: requestFocus,
                    animationStyle: animationStyle,
                    fullscreenDialog: fullscreenDialog
                );
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            },
            builder: (routeContext) =>
            {
                TextDirection textDirectionLocal = Directionality.of(context);
                ThemeData themeData = Theme.of(context);
                MediaQueryData mediaQuery = MediaQuery.of(context);
                Widget dialogContent = new _DialogPopScope__dialog(
                    onPop: Navigator.of(navigator.context).pop,
                    child: new Builder(
                        builder: (innerContext) =>
                        {
                            return new _FullWindowDialogWrapper__dialog(
                                child: builder(innerContext)
                            );
                            throw new InvalidOperationException(
                                "Callback completed without returning a value."
                            );
                        }
                    )
                );
                return new Directionality(
                    textDirection: textDirectionLocal,
                    child: new Theme(
                        data: themeData,
                        child: new MediaQuery(data: mediaQuery, child: dialogContent)
                    )
                );
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class DialogLibrary
{
    public static Future<T?> showAdaptiveDialog<T>(
        BuildContext context,
        Func<BuildContext, Widget> builder,
        bool? barrierDismissible = null,
        Color? barrierColor = null,
        string? barrierLabel = null,
        bool useSafeArea = true,
        bool useRootNavigator = true,
        RouteSettings? routeSettings = null,
        Offset? anchorPoint = null,
        TraversalEdgeBehavior? traversalEdgeBehavior = null,
        bool? requestFocus = null,
        AnimationStyle? animationStyle = null
    )
    {
        ThemeData theme = Theme.of(context);
        switch (theme.platform)
        {
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
            {
                return showDialog<T>(
                    context: context,
                    builder: builder,
                    barrierDismissible: barrierDismissible ?? true,
                    barrierColor: barrierColor,
                    barrierLabel: barrierLabel,
                    useSafeArea: useSafeArea,
                    useRootNavigator: useRootNavigator,
                    routeSettings: routeSettings,
                    anchorPoint: anchorPoint,
                    traversalEdgeBehavior: traversalEdgeBehavior,
                    requestFocus: requestFocus,
                    animationStyle: animationStyle
                );
            }
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
            {
                return RouteLibrary.showCupertinoDialog<T>(
                    context: context,
                    builder: builder,
                    barrierDismissible: barrierDismissible ?? false,
                    barrierLabel: barrierLabel,
                    useRootNavigator: useRootNavigator,
                    anchorPoint: anchorPoint,
                    routeSettings: routeSettings,
                    requestFocus: requestFocus
                );
            }
            default:
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                );
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class DialogLibrary
{
    internal static bool _debugIsActive(BuildContext context)
    {
        if ((context is Element) && !((Element)context).debugIsActive)
        {
            Element context__as64315 = (Element)context;
            throw DartRuntimePrimitives.AsException(
                new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary("This BuildContext is no longer valid."),
                        new ErrorDescription(
                            "The showDialog function context parameter is a BuildContext that is no longer valid."
                        ),
                        new ErrorHint(
                            "This can commonly occur when the showDialog function is called after awaiting a Future. "
                                + "In this situation the BuildContext might refer to a widget that has already been disposed during the await. "
                                + "Consider using a parent context instead."
                        ),
                    }
                )
            );
        }
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class DialogRoute<T> : RawDialogRoute<T>
{
    internal virtual CurvedAnimation? _curvedAnimation { get; set; } = default;
    internal virtual AnimationStyle? _animationStyle { get; private set; }

    public DialogRoute(
        BuildContext context,
        Func<BuildContext, Widget> builder,
        CapturedThemes? themes = null,
        Color? barrierColor = default!,
        bool barrierDismissible = true,
        string? barrierLabel = null,
        bool useSafeArea = true,
        RouteSettings? settings = null,
        bool? requestFocus = null,
        Offset? anchorPoint = null,
        TraversalEdgeBehavior? traversalEdgeBehavior = null,
        bool fullscreenDialog = false,
        AnimationStyle? animationStyle = null
    )
        : base(
            barrierColor: barrierColor ?? Colors.black54,
            barrierDismissible: barrierDismissible,
            settings: settings,
            requestFocus: requestFocus,
            anchorPoint: anchorPoint,
            traversalEdgeBehavior: traversalEdgeBehavior,
            fullscreenDialog: fullscreenDialog,
            pageBuilder: (buildContext, animation, secondaryAnimation) =>
            {
                Widget pageChild = new Builder(builder: builder);
                Widget dialog = themes?.wrap(pageChild) ?? pageChild;
                if (useSafeArea)
                {
                    dialog = DartRuntimePrimitives.ConvertValue<Widget>(
                        new SafeArea(child: dialog)
                    );
                }
                dialog = DartRuntimePrimitives.ConvertValue<Widget>(
                    new Widgets.Semantics(
                        hitTestBehavior: SemanticsHitTestBehavior.opaque,
                        child: dialog
                    )
                );
                return dialog;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            },
            barrierLabel: barrierLabel
                ?? MaterialLocalizations.of(context).modalBarrierDismissLabel,
            transitionDuration: animationStyle?.duration ?? Duration.Create(milliseconds: 150L),
            transitionBuilder: DialogLibrary._buildMaterialDialogTransitions
        )
    {
        _animationStyle = animationStyle;
    }

    internal virtual void _setAnimation(Animation<double> animation)
    {
        if (!Equals(_curvedAnimation?.parent, animation))
        {
            _curvedAnimation?.dispose();
            _curvedAnimation = new CurvedAnimation(
                parent: animation,
                curve: _animationStyle?.curve ?? Curves.easeOut,
                reverseCurve: _animationStyle?.reverseCurve ?? Curves.easeOut
            );
        }
    }

    public override Widget buildTransitions(
        BuildContext context,
        Animation<double> animation,
        Animation<double> secondaryAnimation,
        Widget child
    )
    {
        _setAnimation(animation);
        return new FadeTransition(
            opacity: _curvedAnimation!,
            child: base.buildTransitions(context, animation, secondaryAnimation, child)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void dispose()
    {
        _curvedAnimation?.dispose();
        base.dispose();
    }
}

public static partial class DialogLibrary
{
    internal static double _scalePadding(double textScaleFactor)
    {
        double clampedTextScaleFactor = Dart_uiLibrary.clampDouble(textScaleFactor, 1.0, 2.0);
        return (
            Dart_uiLibrary.lerpDouble(1.0, 1.0 / 3.0, clampedTextScaleFactor - 1.0)
            ?? throw new global::System.NullReferenceException("A required value was null.")
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _DialogFullscreenDefaultsM3__dialog : DialogThemeData
{
    public virtual BuildContext context { get; private set; } = default!;

    internal _DialogFullscreenDefaultsM3__dialog(BuildContext context)
        : base(clipBehavior: Clip.none)
    {
        this.context = context;
    }

    public override Color? backgroundColor =>
        DartRuntimePrimitives.ConvertValue<Color>(Theme.of(context).colorScheme.surface);
}

internal class _DialogDefaultsM3__dialog : DialogThemeData
{
    public virtual BuildContext context { get; private set; } = default!;
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = Theme.of(context).colorScheme;
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
                __late__textTheme = Theme.of(context).textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }

    internal _DialogDefaultsM3__dialog(BuildContext context)
        : base(
            alignment: Alignment.center,
            elevation: 6.0,
            shape: new RoundedRectangleBorder(
                borderRadius: BorderRadius.CreateAll(Radius.circular(28.0))
            ),
            clipBehavior: Clip.none
        )
    {
        this.context = context;
    }

    public override Color? iconColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.secondary);
    public override Color? backgroundColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.surfaceContainerHigh);
    public override Color? shadowColor =>
        DartRuntimePrimitives.ConvertValue<Color>(Colors.transparent);
    public override Color? surfaceTintColor =>
        DartRuntimePrimitives.ConvertValue<Color>(Colors.transparent);
    public override TextStyle? titleTextStyle => _textTheme.headlineSmall;
    public override TextStyle? contentTextStyle => _textTheme.bodyMedium;
    public override EdgeInsetsGeometry? actionsPadding =>
        DartRuntimePrimitives.ConvertValue<EdgeInsetsGeometry>(
            EdgeInsets.CreateOnly(left: 24.0, right: 24.0, bottom: 24.0)
        );
}
