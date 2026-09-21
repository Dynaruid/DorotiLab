// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/media_query.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public enum Orientation
{
    portrait,
    landscape,
}

public enum _MediaQueryAspect__media_query
{
    size,
    width,
    height,
    orientation,
    devicePixelRatio,
    textScaleFactor,
    textScaler,
    platformBrightness,
    padding,
    viewInsets,
    systemGestureInsets,
    viewPadding,
    alwaysUse24HourFormat,
    accessibleNavigation,
    invertColors,
    highContrast,
    onOffSwitchLabels,
    disableAnimations,
    reduceMotion,
    boldText,
    supportsAnnounce,
    navigationMode,
    gestureSettings,
    displayFeatures,
    supportsShowingSystemContextMenu,
    lineHeightScaleFactorOverride,
    letterSpacingOverride,
    wordSpacingOverride,
    paragraphSpacingOverride,
    displayCornerRadii,
}

public class MediaQueryData
{
    public virtual Size size { get; private set; } = default!;
    public virtual double devicePixelRatio { get; private set; } = default!;
    internal virtual double _textScaleFactor { get; private set; } = default!;
    internal virtual TextScaler _textScaler { get; private set; } = default!;
    public virtual Brightness platformBrightness { get; private set; } = default!;
    public virtual EdgeInsets viewInsets { get; private set; } = default!;
    public virtual EdgeInsets padding { get; private set; } = default!;
    public virtual EdgeInsets viewPadding { get; private set; } = default!;
    public virtual EdgeInsets systemGestureInsets { get; private set; } = default!;
    public virtual bool alwaysUse24HourFormat { get; private set; } = default!;
    public virtual bool accessibleNavigation { get; private set; } = default!;
    public virtual bool invertColors { get; private set; } = default!;
    public virtual bool highContrast { get; private set; } = default!;
    public virtual bool onOffSwitchLabels { get; private set; } = default!;
    public virtual bool disableAnimations { get; private set; } = default!;
    public virtual bool reduceMotion { get; private set; } = default!;
    public virtual bool boldText { get; private set; } = default!;
    public virtual bool supportsAnnounce { get; private set; } = default!;
    public virtual NavigationMode navigationMode { get; private set; } = default!;
    public virtual DeviceGestureSettings gestureSettings { get; private set; } = default!;
    public virtual List<DisplayFeature> displayFeatures { get; private set; } = default!;
    public virtual bool supportsShowingSystemContextMenu { get; private set; } = default!;
    public virtual double? lineHeightScaleFactorOverride { get; private set; }
    public virtual double? letterSpacingOverride { get; private set; }
    public virtual double? wordSpacingOverride { get; private set; }
    public virtual double? paragraphSpacingOverride { get; private set; }
    public virtual BorderRadius? displayCornerRadii { get; private set; }

    public MediaQueryData(
        Size? size = null,
        double devicePixelRatio = 1.0,
        double textScaleFactor = 1.0,
        TextScaler textScaler = default!,
        Brightness platformBrightness = Brightness.light,
        EdgeInsets padding = default!,
        EdgeInsets viewInsets = default!,
        EdgeInsets systemGestureInsets = default!,
        EdgeInsets viewPadding = default!,
        bool alwaysUse24HourFormat = false,
        bool accessibleNavigation = false,
        bool invertColors = false,
        bool highContrast = false,
        bool onOffSwitchLabels = false,
        bool disableAnimations = false,
        bool reduceMotion = false,
        bool boldText = false,
        bool supportsAnnounce = false,
        NavigationMode navigationMode = NavigationMode.traditional,
        DeviceGestureSettings gestureSettings = default!,
        List<DisplayFeature> displayFeatures = default!,
        bool supportsShowingSystemContextMenu = false,
        double? lineHeightScaleFactorOverride = null,
        double? letterSpacingOverride = null,
        double? wordSpacingOverride = null,
        double? paragraphSpacingOverride = null,
        BorderRadius? displayCornerRadii = null
    )
    {
        TextScaler __textScaler = textScaler ?? Media_queryLibrary._kUnspecifiedTextScaler;
        EdgeInsets __padding = padding ?? EdgeInsets.zero;
        EdgeInsets __viewInsets = viewInsets ?? EdgeInsets.zero;
        EdgeInsets __systemGestureInsets = systemGestureInsets ?? EdgeInsets.zero;
        EdgeInsets __viewPadding = viewPadding ?? EdgeInsets.zero;
        DeviceGestureSettings __gestureSettings =
            gestureSettings
            ?? new DeviceGestureSettings(touchSlop: Gestures.ConstantsLibrary.kTouchSlop);
        List<DisplayFeature> __displayFeatures = displayFeatures ?? new List<DisplayFeature>();
        this.size = size ?? Size.zero;
        this.devicePixelRatio = devicePixelRatio;
        this.platformBrightness = platformBrightness;
        this.padding = __padding;
        this.viewInsets = __viewInsets;
        this.systemGestureInsets = __systemGestureInsets;
        this.viewPadding = __viewPadding;
        this.alwaysUse24HourFormat = alwaysUse24HourFormat;
        this.accessibleNavigation = accessibleNavigation;
        this.invertColors = invertColors;
        this.highContrast = highContrast;
        this.onOffSwitchLabels = onOffSwitchLabels;
        this.disableAnimations = disableAnimations;
        this.reduceMotion = reduceMotion;
        this.boldText = boldText;
        this.supportsAnnounce = supportsAnnounce;
        this.navigationMode = navigationMode;
        this.gestureSettings = __gestureSettings;
        this.displayFeatures = __displayFeatures;
        this.supportsShowingSystemContextMenu = supportsShowingSystemContextMenu;
        this.lineHeightScaleFactorOverride = lineHeightScaleFactorOverride;
        this.letterSpacingOverride = letterSpacingOverride;
        this.wordSpacingOverride = wordSpacingOverride;
        this.paragraphSpacingOverride = paragraphSpacingOverride;
        this.displayCornerRadii = displayCornerRadii;
        _textScaleFactor = DartRuntimePrimitives.RequireValue(textScaleFactor);
        _textScaler = __textScaler;
        System.Diagnostics.Debug.Assert(
            DartRuntimePrimitives.Identical(
                __textScaler,
                Media_queryLibrary._kUnspecifiedTextScaler
            ) || (DartRuntimePrimitives.RequireValue(textScaleFactor) == 1.0)
        );
    }

    public static MediaQueryData CreateFromWindow(DorotiView window) => CreateFromView(window);

    public static MediaQueryData CreateFromView(
        DorotiView view,
        MediaQueryData? platformData = null
    )
    {
        var metrics = view.metrics;
        var configuration =
            view.environmentConfiguration ?? view.platformDispatcher.configurationSnapshot;
        var accessibility =
            configuration?.accessibilityFeatures ?? view.platformDispatcher.accessibilityFeatures;
        var __instance = new MediaQueryData(
            default!,
            default!,
            default!,
            default!,
            default!,
            default!,
            default!,
            default!,
            default!,
            default!,
            default!,
            default!,
            default!,
            default!,
            default!,
            default!,
            default!,
            default!,
            default!,
            default!,
            default!,
            default!,
            default!,
            default!,
            default!,
            default!,
            default!
        );
        __instance.size = metrics.physicalSize / metrics.devicePixelRatio;
        __instance.devicePixelRatio = metrics.devicePixelRatio;
        __instance._textScaleFactor = 1.0;
        __instance._textScaler = _textScalerFromView(view, platformData);
        __instance.platformBrightness =
            platformData?.platformBrightness
            ?? configuration?.platformBrightness
            ?? view.platformDispatcher.platformBrightness;
        __instance.padding = EdgeInsets.CreateFromViewPadding(
            metrics.padding,
            metrics.devicePixelRatio
        );
        __instance.viewPadding = EdgeInsets.CreateFromViewPadding(
            metrics.viewPadding,
            metrics.devicePixelRatio
        );
        __instance.viewInsets = EdgeInsets.CreateFromViewPadding(
            metrics.viewInsets,
            metrics.devicePixelRatio
        );
        __instance.systemGestureInsets = EdgeInsets.CreateFromViewPadding(
            metrics.systemGestureInsets,
            metrics.devicePixelRatio
        );
        __instance.accessibleNavigation =
            platformData?.accessibleNavigation ?? accessibility.accessibleNavigation;
        __instance.invertColors = platformData?.invertColors ?? accessibility.invertColors;
        __instance.disableAnimations =
            platformData?.disableAnimations ?? accessibility.disableAnimations;
        __instance.reduceMotion = platformData?.reduceMotion ?? accessibility.reduceMotion;
        __instance.boldText = platformData?.boldText ?? accessibility.boldText;
        __instance.supportsAnnounce =
            platformData?.supportsAnnounce ?? accessibility.supportsAnnounce;
        __instance.highContrast = platformData?.highContrast ?? accessibility.highContrast;
        __instance.onOffSwitchLabels =
            platformData?.onOffSwitchLabels ?? accessibility.onOffSwitchLabels;
        __instance.alwaysUse24HourFormat =
            platformData?.alwaysUse24HourFormat
            ?? configuration?.alwaysUse24HourFormat
            ?? view.platformDispatcher.alwaysUse24HourFormat;
        __instance.navigationMode = platformData?.navigationMode ?? NavigationMode.traditional;
        __instance.gestureSettings = new DeviceGestureSettings(
            metrics.gestureSettings.physicalTouchSlop / metrics.devicePixelRatio
        );
        __instance.displayFeatures = metrics.displayFeatures.ToList();
        __instance.supportsShowingSystemContextMenu =
            platformData?.supportsShowingSystemContextMenu
            ?? configuration?.supportsShowingSystemContextMenu
            ?? view.platformDispatcher.supportsShowingSystemContextMenu;
        __instance.lineHeightScaleFactorOverride =
            platformData?.lineHeightScaleFactorOverride
            ?? configuration?.lineHeightScaleFactorOverride
            ?? view.platformDispatcher.lineHeightScaleFactorOverride;
        __instance.letterSpacingOverride =
            platformData?.letterSpacingOverride
            ?? configuration?.letterSpacingOverride
            ?? view.platformDispatcher.letterSpacingOverride;
        __instance.wordSpacingOverride =
            platformData?.wordSpacingOverride
            ?? configuration?.wordSpacingOverride
            ?? view.platformDispatcher.wordSpacingOverride;
        __instance.paragraphSpacingOverride =
            platformData?.paragraphSpacingOverride
            ?? configuration?.paragraphSpacingOverride
            ?? view.platformDispatcher.paragraphSpacingOverride;
        __instance.displayCornerRadii = _displayCornerRadiiFromMetrics(metrics);
        return __instance;
    }

    internal static TextScaler _textScalerFromView(DorotiView view, MediaQueryData? platformData)
    {
        return platformData?.textScaler
            ?? new SystemTextScaler(
                view.environmentConfiguration ?? view.platformDispatcher.configurationSnapshot,
                view.platformDispatcher
            );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static BorderRadius? _displayCornerRadiiFromView(DorotiView view)
    {
        return _displayCornerRadiiFromMetrics(view.metrics);
    }

    private static BorderRadius? _displayCornerRadiiFromMetrics(ViewMetrics metrics)
    {
        var displayCornerRadiiLocal = metrics.displayCornerRadii;
        if (displayCornerRadiiLocal is null)
        {
            return null;
        }
        double devicePixelRatioLocal = metrics.devicePixelRatio;
        return new BorderRadius(
            topLeft: Radius.circular(
                displayCornerRadiiLocal.topLeft
                    / DartRuntimePrimitives.RequireValue(devicePixelRatioLocal)
            ),
            topRight: Radius.circular(
                displayCornerRadiiLocal.topRight
                    / DartRuntimePrimitives.RequireValue(devicePixelRatioLocal)
            ),
            bottomRight: Radius.circular(
                displayCornerRadiiLocal.bottomRight
                    / DartRuntimePrimitives.RequireValue(devicePixelRatioLocal)
            ),
            bottomLeft: Radius.circular(
                displayCornerRadiiLocal.bottomLeft
                    / DartRuntimePrimitives.RequireValue(devicePixelRatioLocal)
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double textScaleFactor => textScaler.textScaleFactor;
    public virtual TextScaler textScaler
    {
        get
        {
            if (
                !DartRuntimePrimitives.Identical(
                    Media_queryLibrary._kUnspecifiedTextScaler,
                    _textScaler
                )
            )
            {
                return _textScaler;
            }
            return (_textScaleFactor == 1.0)
                ? TextScaler.noScaling
                : TextScaler.CreateLinear(_textScaleFactor);
        }
    }
    public virtual Orientation orientation
    {
        get { return (size.width > size.height) ? Orientation.landscape : Orientation.portrait; }
    }

    public virtual MediaQueryData copyWith(
        Size? size = null,
        double? devicePixelRatio = null,
        double? textScaleFactor = null,
        TextScaler? textScaler = null,
        Brightness? platformBrightness = null,
        EdgeInsets? padding = null,
        EdgeInsets? viewPadding = null,
        EdgeInsets? viewInsets = null,
        EdgeInsets? systemGestureInsets = null,
        bool? alwaysUse24HourFormat = null,
        bool? highContrast = null,
        bool? onOffSwitchLabels = null,
        bool? disableAnimations = null,
        bool? reduceMotion = null,
        bool? invertColors = null,
        bool? accessibleNavigation = null,
        bool? boldText = null,
        bool? supportsAnnounce = null,
        NavigationMode? navigationMode = null,
        DeviceGestureSettings? gestureSettings = null,
        List<DisplayFeature>? displayFeatures = null,
        bool? supportsShowingSystemContextMenu = null
    )
    {
        DartRuntimePrimitives.Assert(() => (textScaleFactor is null) || (textScaler is null));
        if (textScaleFactor is not null)
        {
            double textScaleFactor__value39066 = DartRuntimePrimitives.RequireValue(
                textScaleFactor
            );
            textScaler ??= TextScaler.CreateLinear(
                DartRuntimePrimitives.RequireValue(
                    DartRuntimePrimitives.RequireValue(textScaleFactor__value39066)
                )
            );
        }
        return new MediaQueryData(
            size: size ?? this.size,
            devicePixelRatio: devicePixelRatio ?? this.devicePixelRatio,
            textScaler: textScaler ?? this.textScaler,
            platformBrightness: platformBrightness ?? this.platformBrightness,
            padding: padding ?? this.padding,
            viewPadding: viewPadding ?? this.viewPadding,
            viewInsets: viewInsets ?? this.viewInsets,
            systemGestureInsets: systemGestureInsets ?? this.systemGestureInsets,
            alwaysUse24HourFormat: alwaysUse24HourFormat ?? this.alwaysUse24HourFormat,
            invertColors: invertColors ?? this.invertColors,
            highContrast: highContrast ?? this.highContrast,
            onOffSwitchLabels: onOffSwitchLabels ?? this.onOffSwitchLabels,
            disableAnimations: disableAnimations ?? this.disableAnimations,
            reduceMotion: reduceMotion ?? this.reduceMotion,
            accessibleNavigation: accessibleNavigation ?? this.accessibleNavigation,
            boldText: boldText ?? this.boldText,
            supportsAnnounce: supportsAnnounce ?? this.supportsAnnounce,
            navigationMode: navigationMode ?? this.navigationMode,
            gestureSettings: gestureSettings ?? this.gestureSettings,
            displayFeatures: displayFeatures ?? this.displayFeatures,
            supportsShowingSystemContextMenu: supportsShowingSystemContextMenu
                ?? this.supportsShowingSystemContextMenu,
            lineHeightScaleFactorOverride: lineHeightScaleFactorOverride,
            letterSpacingOverride: letterSpacingOverride,
            wordSpacingOverride: wordSpacingOverride,
            paragraphSpacingOverride: paragraphSpacingOverride,
            displayCornerRadii: displayCornerRadii
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual MediaQueryData applyTextStyleOverrides(
        double? lineHeightScaleFactorOverride,
        double? letterSpacingOverride,
        double? wordSpacingOverride,
        double? paragraphSpacingOverride
    )
    {
        return new MediaQueryData(
            size: DartRuntimePrimitives.RequireValue(size),
            devicePixelRatio: DartRuntimePrimitives.RequireValue(devicePixelRatio),
            textScaler: textScaler,
            platformBrightness: DartRuntimePrimitives.RequireValue(platformBrightness),
            padding: padding,
            viewPadding: viewPadding,
            viewInsets: viewInsets,
            systemGestureInsets: systemGestureInsets,
            alwaysUse24HourFormat: DartRuntimePrimitives.RequireValue(alwaysUse24HourFormat),
            invertColors: DartRuntimePrimitives.RequireValue(invertColors),
            highContrast: DartRuntimePrimitives.RequireValue(highContrast),
            onOffSwitchLabels: DartRuntimePrimitives.RequireValue(onOffSwitchLabels),
            disableAnimations: DartRuntimePrimitives.RequireValue(disableAnimations),
            reduceMotion: DartRuntimePrimitives.RequireValue(reduceMotion),
            accessibleNavigation: DartRuntimePrimitives.RequireValue(accessibleNavigation),
            boldText: DartRuntimePrimitives.RequireValue(boldText),
            supportsAnnounce: DartRuntimePrimitives.RequireValue(supportsAnnounce),
            navigationMode: DartRuntimePrimitives.RequireValue(navigationMode),
            gestureSettings: gestureSettings,
            displayFeatures: displayFeatures,
            supportsShowingSystemContextMenu: DartRuntimePrimitives.RequireValue(
                supportsShowingSystemContextMenu
            ),
            lineHeightScaleFactorOverride: lineHeightScaleFactorOverride,
            letterSpacingOverride: letterSpacingOverride,
            wordSpacingOverride: wordSpacingOverride,
            paragraphSpacingOverride: paragraphSpacingOverride,
            displayCornerRadii: displayCornerRadii
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual MediaQueryData applyDisplayCornerRadii(BorderRadius? displayCornerRadii)
    {
        return new MediaQueryData(
            size: DartRuntimePrimitives.RequireValue(size),
            devicePixelRatio: DartRuntimePrimitives.RequireValue(devicePixelRatio),
            textScaler: textScaler,
            platformBrightness: DartRuntimePrimitives.RequireValue(platformBrightness),
            padding: padding,
            viewPadding: viewPadding,
            viewInsets: viewInsets,
            systemGestureInsets: systemGestureInsets,
            alwaysUse24HourFormat: DartRuntimePrimitives.RequireValue(alwaysUse24HourFormat),
            invertColors: DartRuntimePrimitives.RequireValue(invertColors),
            highContrast: DartRuntimePrimitives.RequireValue(highContrast),
            onOffSwitchLabels: DartRuntimePrimitives.RequireValue(onOffSwitchLabels),
            disableAnimations: DartRuntimePrimitives.RequireValue(disableAnimations),
            reduceMotion: DartRuntimePrimitives.RequireValue(reduceMotion),
            accessibleNavigation: DartRuntimePrimitives.RequireValue(accessibleNavigation),
            boldText: DartRuntimePrimitives.RequireValue(boldText),
            supportsAnnounce: DartRuntimePrimitives.RequireValue(supportsAnnounce),
            navigationMode: DartRuntimePrimitives.RequireValue(navigationMode),
            gestureSettings: gestureSettings,
            displayFeatures: displayFeatures,
            supportsShowingSystemContextMenu: DartRuntimePrimitives.RequireValue(
                supportsShowingSystemContextMenu
            ),
            lineHeightScaleFactorOverride: lineHeightScaleFactorOverride,
            letterSpacingOverride: letterSpacingOverride,
            wordSpacingOverride: wordSpacingOverride,
            paragraphSpacingOverride: paragraphSpacingOverride,
            displayCornerRadii: displayCornerRadii
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual MediaQueryData removePadding(
        bool removeLeft = false,
        bool removeTop = false,
        bool removeRight = false,
        bool removeBottom = false
    )
    {
        if (!(removeLeft || removeTop || removeRight || removeBottom))
        {
            return this;
        }
        return copyWith(
            padding: padding.copyWith(
                left: removeLeft ? 0.0 : null,
                top: removeTop ? 0.0 : null,
                right: removeRight ? 0.0 : null,
                bottom: removeBottom ? 0.0 : null
            ),
            viewPadding: viewPadding.copyWith(
                left: removeLeft ? Math.Max(0.0, viewPadding.left - padding.left) : null,
                top: removeTop ? Math.Max(0.0, viewPadding.top - padding.top) : null,
                right: removeRight ? Math.Max(0.0, viewPadding.right - padding.right) : null,
                bottom: removeBottom ? Math.Max(0.0, viewPadding.bottom - padding.bottom) : null
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual MediaQueryData removeViewInsets(
        bool removeLeft = false,
        bool removeTop = false,
        bool removeRight = false,
        bool removeBottom = false
    )
    {
        if (!(removeLeft || removeTop || removeRight || removeBottom))
        {
            return this;
        }
        return copyWith(
            viewPadding: viewPadding.copyWith(
                left: removeLeft ? Math.Max(0.0, viewPadding.left - viewInsets.left) : null,
                top: removeTop ? Math.Max(0.0, viewPadding.top - viewInsets.top) : null,
                right: removeRight ? Math.Max(0.0, viewPadding.right - viewInsets.right) : null,
                bottom: removeBottom ? Math.Max(0.0, viewPadding.bottom - viewInsets.bottom) : null
            ),
            viewInsets: viewInsets.copyWith(
                left: removeLeft ? 0.0 : null,
                top: removeTop ? 0.0 : null,
                right: removeRight ? 0.0 : null,
                bottom: removeBottom ? 0.0 : null
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual MediaQueryData removeViewPadding(
        bool removeLeft = false,
        bool removeTop = false,
        bool removeRight = false,
        bool removeBottom = false
    )
    {
        if (!(removeLeft || removeTop || removeRight || removeBottom))
        {
            return this;
        }
        return copyWith(
            padding: padding.copyWith(
                left: removeLeft ? 0.0 : null,
                top: removeTop ? 0.0 : null,
                right: removeRight ? 0.0 : null,
                bottom: removeBottom ? 0.0 : null
            ),
            viewPadding: viewPadding.copyWith(
                left: removeLeft ? 0.0 : null,
                top: removeTop ? 0.0 : null,
                right: removeRight ? 0.0 : null,
                bottom: removeBottom ? 0.0 : null
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual MediaQueryData removeDisplayFeatures(Rect subScreen)
    {
        DartRuntimePrimitives.Assert(
            () =>
                (subScreen.left >= 0.0)
                && (subScreen.top >= 0.0)
                && (subScreen.right <= size.width)
                && (subScreen.bottom <= size.height),
            () => (object?)"'subScreen' argument cannot be outside the bounds of the screen"
        );
        if (Equals(subScreen.size, size) && Equals(subScreen.topLeft, Offset.zero))
        {
            return this;
        }
        double rightInset = size.width - subScreen.right;
        double bottomInset = size.height - subScreen.bottom;
        return copyWith(
            padding: EdgeInsets.CreateOnly(
                left: Math.Max(0.0, padding.left - subScreen.left),
                top: Math.Max(0.0, padding.top - subScreen.top),
                right: Math.Max(0.0, padding.right - rightInset),
                bottom: Math.Max(0.0, padding.bottom - bottomInset)
            ),
            viewPadding: EdgeInsets.CreateOnly(
                left: Math.Max(0.0, viewPadding.left - subScreen.left),
                top: Math.Max(0.0, viewPadding.top - subScreen.top),
                right: Math.Max(0.0, viewPadding.right - rightInset),
                bottom: Math.Max(0.0, viewPadding.bottom - bottomInset)
            ),
            viewInsets: EdgeInsets.CreateOnly(
                left: Math.Max(0.0, viewInsets.left - subScreen.left),
                top: Math.Max(0.0, viewInsets.top - subScreen.top),
                right: Math.Max(0.0, viewInsets.right - rightInset),
                bottom: Math.Max(0.0, viewInsets.bottom - bottomInset)
            ),
            displayFeatures: displayFeatures
                .where((displayFeature) => subScreen.overlaps(displayFeature.bounds))
                .ToList()
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as MediaQueryData;
        if (__other is null)
        {
            return false;
        }

        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is MediaQueryData)
            && Equals(__other.size, size)
            && (__other.devicePixelRatio == devicePixelRatio)
            && (__other.textScaleFactor == textScaleFactor)
            && Equals(__other.platformBrightness, platformBrightness)
            && Equals(__other.padding, padding)
            && Equals(__other.viewPadding, viewPadding)
            && Equals(__other.viewInsets, viewInsets)
            && Equals(__other.systemGestureInsets, systemGestureInsets)
            && (__other.alwaysUse24HourFormat == alwaysUse24HourFormat)
            && (__other.highContrast == highContrast)
            && (__other.onOffSwitchLabels == onOffSwitchLabels)
            && (__other.disableAnimations == disableAnimations)
            && (__other.reduceMotion == reduceMotion)
            && (__other.invertColors == invertColors)
            && (__other.accessibleNavigation == accessibleNavigation)
            && (__other.boldText == boldText)
            && (__other.supportsAnnounce == supportsAnnounce)
            && Equals(__other.navigationMode, navigationMode)
            && Equals(__other.gestureSettings, gestureSettings)
            && CollectionsLibrary.listEquals(__other.displayFeatures, displayFeatures)
            && (__other.supportsShowingSystemContextMenu == supportsShowingSystemContextMenu)
            && (__other.lineHeightScaleFactorOverride == lineHeightScaleFactorOverride)
            && (__other.letterSpacingOverride == letterSpacingOverride)
            && (__other.wordSpacingOverride == wordSpacingOverride)
            && (__other.paragraphSpacingOverride == paragraphSpacingOverride)
            && Equals(__other.displayCornerRadii, displayCornerRadii);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(
                size,
                devicePixelRatio,
                textScaleFactor,
                platformBrightness,
                padding,
                viewPadding,
                viewInsets,
                alwaysUse24HourFormat,
                highContrast,
                onOffSwitchLabels,
                disableAnimations,
                reduceMotion,
                invertColors,
                accessibleNavigation,
                boldText,
                navigationMode,
                gestureSettings,
                FoundationRuntimePorts.ObjectHashAll(displayFeatures),
                supportsShowingSystemContextMenu,
                FoundationRuntimePorts.ObjectHash(
                    lineHeightScaleFactorOverride,
                    letterSpacingOverride,
                    wordSpacingOverride,
                    paragraphSpacingOverride,
                    displayCornerRadii
                )
            )
        );

    public override string ToString()
    {
        var properties = new List<string>
        {
            $"size: {size}",
            $"devicePixelRatio: {devicePixelRatio.toStringAsFixed(1L)}",
            $"textScaler: {textScaler}",
            $"platformBrightness: {platformBrightness}",
            $"padding: {padding}",
            $"viewPadding: {viewPadding}",
            $"viewInsets: {viewInsets}",
            $"systemGestureInsets: {systemGestureInsets}",
            $"alwaysUse24HourFormat: {alwaysUse24HourFormat}",
            $"accessibleNavigation: {accessibleNavigation}",
            $"highContrast: {highContrast}",
            $"onOffSwitchLabels: {onOffSwitchLabels}",
            $"disableAnimations: {disableAnimations}",
            $"reduceMotion: {reduceMotion}",
            $"invertColors: {invertColors}",
            $"boldText: {boldText}",
            $"navigationMode: {navigationMode.ToString()}",
            $"gestureSettings: {gestureSettings}",
            $"displayFeatures: {displayFeatures}",
            $"supportsShowingSystemContextMenu: {supportsShowingSystemContextMenu}",
            $"lineHeightScaleFactorOverride: {lineHeightScaleFactorOverride}",
            $"letterSpacingOverride: {letterSpacingOverride}",
            $"wordSpacingOverride: {wordSpacingOverride}",
            $"paragraphSpacingOverride: {paragraphSpacingOverride}",
            $"displayCornerRadii: {displayCornerRadii}",
        };
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "MediaQueryData")}({string.Join(", ", properties)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class MediaQuery : InheritedModel<_MediaQueryAspect__media_query>
{
    public virtual MediaQueryData data { get; private set; } = default!;

    public MediaQuery(Key? key = null, MediaQueryData data = default!, Widget child = default!)
        : base(key: key, child: child)
    {
        this.data = data;
    }

    public static MediaQuery CreateRemovePadding(
        Key? key = null,
        BuildContext context = default!,
        bool removeLeft = false,
        bool removeTop = false,
        bool removeRight = false,
        bool removeBottom = false,
        Widget child = default!
    )
    {
        var __instance = new MediaQuery(key: key, child: child);
        __instance.data = of(context)
            .removePadding(
                removeLeft: removeLeft,
                removeTop: removeTop,
                removeRight: removeRight,
                removeBottom: removeBottom
            );
        return __instance;
    }

    public static MediaQuery CreateRemoveViewInsets(
        Key? key = null,
        BuildContext context = default!,
        bool removeLeft = false,
        bool removeTop = false,
        bool removeRight = false,
        bool removeBottom = false,
        Widget child = default!
    )
    {
        var __instance = new MediaQuery(key: key, child: child);
        __instance.data = of(context)
            .removeViewInsets(
                removeLeft: removeLeft,
                removeTop: removeTop,
                removeRight: removeRight,
                removeBottom: removeBottom
            );
        return __instance;
    }

    public static MediaQuery CreateRemoveViewPadding(
        Key? key = null,
        BuildContext context = default!,
        bool removeLeft = false,
        bool removeTop = false,
        bool removeRight = false,
        bool removeBottom = false,
        Widget child = default!
    )
    {
        var __instance = new MediaQuery(key: key, child: child);
        __instance.data = of(context)
            .removeViewPadding(
                removeLeft: removeLeft,
                removeTop: removeTop,
                removeRight: removeRight,
                removeBottom: removeBottom
            );
        return __instance;
    }

    public static Widget applyTextStyleOverrides(
        Key? key = null,
        double? lineHeightScaleFactorOverride = default!,
        double? letterSpacingOverride = default!,
        double? wordSpacingOverride = default!,
        double? paragraphSpacingOverride = default!,
        Widget child = default!
    )
    {
        return new Builder(
            key: key,
            builder: (context) =>
            {
                DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMediaQuery(context));
                return new MediaQuery(
                    data: of(context)
                        .applyTextStyleOverrides(
                            lineHeightScaleFactorOverride: lineHeightScaleFactorOverride,
                            letterSpacingOverride: letterSpacingOverride,
                            wordSpacingOverride: wordSpacingOverride,
                            paragraphSpacingOverride: paragraphSpacingOverride
                        ),
                    child: child
                );
                throw new InvalidOperationException("Dart closure completed without a value.");
            }
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Widget fromWindow(Key? key = null, Widget child = default!)
    {
        return new _MediaQueryFromView__media_query(
            key: key,
            view: WidgetsBinding.instance.window,
            ignoreParentData: true,
            child: child
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Widget fromView(
        Key? key = null,
        DorotiView view = default!,
        Widget child = default!
    )
    {
        return new _MediaQueryFromView__media_query(key: key, view: view, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Widget withNoTextScaling(Key? key = null, Widget child = default!)
    {
        return new Builder(
            key: key,
            builder: (context) =>
            {
                DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMediaQuery(context));
                return new MediaQuery(
                    data: of(context).copyWith(textScaler: TextScaler.noScaling),
                    child: child
                );
                throw new InvalidOperationException("Dart closure completed without a value.");
            }
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Widget withClampedTextScaling(
        Key? key = null,
        double minScaleFactor = 0.0,
        double maxScaleFactor = double.PositiveInfinity,
        Widget child = default!
    )
    {
        DartRuntimePrimitives.Assert(() => maxScaleFactor >= minScaleFactor);
        DartRuntimePrimitives.Assert(() => !double.IsNaN(maxScaleFactor));
        DartRuntimePrimitives.Assert(() => double.IsFinite(minScaleFactor));
        DartRuntimePrimitives.Assert(() => minScaleFactor >= 0L);
        return new Builder(
            builder: (context) =>
            {
                DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMediaQuery(context));
                MediaQueryData dataLocal = of(context);
                return new MediaQuery(
                    data: dataLocal.copyWith(
                        textScaler: dataLocal.textScaler.clamp(
                            minScaleFactor: minScaleFactor,
                            maxScaleFactor: maxScaleFactor
                        )
                    ),
                    child: child
                );
                throw new InvalidOperationException("Dart closure completed without a value.");
            }
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static MediaQueryData of(BuildContext context)
    {
        return _of(context);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static MediaQueryData _of(
        BuildContext context,
        _MediaQueryAspect__media_query? aspect = null
    )
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMediaQuery(context));
        return InheritedModel<object>.inheritFrom<MediaQuery>(context, aspect: aspect)!.data;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static MediaQueryData? maybeOf(BuildContext context)
    {
        return _maybeOf(context);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static MediaQueryData? _maybeOf(
        BuildContext context,
        _MediaQueryAspect__media_query? aspect = null
    )
    {
        return InheritedModel<object>.inheritFrom<MediaQuery>(context, aspect: aspect)?.data;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Size sizeOf(BuildContext context) =>
        DartRuntimePrimitives.ConvertValue<Size>(
            _of(context, _MediaQueryAspect__media_query.size).size
        );

    public static Size? maybeSizeOf(BuildContext context) =>
        DartRuntimePrimitives.ConvertValue<Size>(
            _maybeOf(context, _MediaQueryAspect__media_query.size)?.size
        );

    public static double widthOf(BuildContext context) =>
        _of(context, _MediaQueryAspect__media_query.width).size.width;

    public static double? maybeWidthOf(BuildContext context) =>
        _maybeOf(context, _MediaQueryAspect__media_query.width)?.size.width;

    public static double heightOf(BuildContext context) =>
        _of(context, _MediaQueryAspect__media_query.height).size.height;

    public static double? maybeHeightOf(BuildContext context) =>
        _maybeOf(context, _MediaQueryAspect__media_query.height)?.size.height;

    public static Orientation orientationOf(BuildContext context) =>
        _of(context, _MediaQueryAspect__media_query.orientation).orientation;

    public static Orientation? maybeOrientationOf(BuildContext context) =>
        _maybeOf(context, _MediaQueryAspect__media_query.orientation)?.orientation;

    public static double devicePixelRatioOf(BuildContext context) =>
        _of(context, _MediaQueryAspect__media_query.devicePixelRatio).devicePixelRatio;

    public static double? maybeDevicePixelRatioOf(BuildContext context) =>
        _maybeOf(context, _MediaQueryAspect__media_query.devicePixelRatio)?.devicePixelRatio;

    public static double textScaleFactorOf(BuildContext context) =>
        DartRuntimePrimitives.ConvertValue<double>(maybeTextScaleFactorOf(context) ?? 1.0);

    public static double? maybeTextScaleFactorOf(BuildContext context) =>
        _maybeOf(context, _MediaQueryAspect__media_query.textScaleFactor)?.textScaleFactor;

    public static TextScaler textScalerOf(BuildContext context) =>
        DartRuntimePrimitives.ConvertValue<TextScaler>(
            maybeTextScalerOf(context) ?? TextScaler.noScaling
        );

    public static TextScaler? maybeTextScalerOf(BuildContext context) =>
        _maybeOf(context, _MediaQueryAspect__media_query.textScaler)?.textScaler;

    public static Brightness platformBrightnessOf(BuildContext context) =>
        DartRuntimePrimitives.ConvertValue<Brightness>(
            maybePlatformBrightnessOf(context) ?? Brightness.light
        );

    public static Brightness? maybePlatformBrightnessOf(BuildContext context) =>
        DartRuntimePrimitives.ConvertValue<Brightness>(
            _maybeOf(context, _MediaQueryAspect__media_query.platformBrightness)?.platformBrightness
        );

    public static EdgeInsets paddingOf(BuildContext context) =>
        _of(context, _MediaQueryAspect__media_query.padding).padding;

    public static EdgeInsets? maybePaddingOf(BuildContext context) =>
        _maybeOf(context, _MediaQueryAspect__media_query.padding)?.padding;

    public static EdgeInsets viewInsetsOf(BuildContext context) =>
        _of(context, _MediaQueryAspect__media_query.viewInsets).viewInsets;

    public static EdgeInsets? maybeViewInsetsOf(BuildContext context) =>
        _maybeOf(context, _MediaQueryAspect__media_query.viewInsets)?.viewInsets;

    public static EdgeInsets systemGestureInsetsOf(BuildContext context) =>
        _of(context, _MediaQueryAspect__media_query.systemGestureInsets).systemGestureInsets;

    public static EdgeInsets? maybeSystemGestureInsetsOf(BuildContext context) =>
        _maybeOf(context, _MediaQueryAspect__media_query.systemGestureInsets)?.systemGestureInsets;

    public static EdgeInsets viewPaddingOf(BuildContext context) =>
        _of(context, _MediaQueryAspect__media_query.viewPadding).viewPadding;

    public static EdgeInsets? maybeViewPaddingOf(BuildContext context) =>
        _maybeOf(context, _MediaQueryAspect__media_query.viewPadding)?.viewPadding;

    public static bool alwaysUse24HourFormatOf(BuildContext context) =>
        _of(context, _MediaQueryAspect__media_query.alwaysUse24HourFormat).alwaysUse24HourFormat;

    public static bool? maybeAlwaysUse24HourFormatOf(BuildContext context) =>
        _maybeOf(
            context,
            _MediaQueryAspect__media_query.alwaysUse24HourFormat
        )?.alwaysUse24HourFormat;

    public static bool accessibleNavigationOf(BuildContext context) =>
        _of(context, _MediaQueryAspect__media_query.accessibleNavigation).accessibleNavigation;

    public static bool? maybeAccessibleNavigationOf(BuildContext context) =>
        _maybeOf(
            context,
            _MediaQueryAspect__media_query.accessibleNavigation
        )?.accessibleNavigation;

    public static bool invertColorsOf(BuildContext context) =>
        _of(context, _MediaQueryAspect__media_query.invertColors).invertColors;

    public static bool? maybeInvertColorsOf(BuildContext context) =>
        _maybeOf(context, _MediaQueryAspect__media_query.invertColors)?.invertColors;

    public static bool highContrastOf(BuildContext context) =>
        DartRuntimePrimitives.ConvertValue<bool>(maybeHighContrastOf(context) ?? false);

    public static bool? maybeHighContrastOf(BuildContext context) =>
        _maybeOf(context, _MediaQueryAspect__media_query.highContrast)?.highContrast;

    public static bool onOffSwitchLabelsOf(BuildContext context) =>
        DartRuntimePrimitives.ConvertValue<bool>(maybeOnOffSwitchLabelsOf(context) ?? false);

    public static bool? maybeOnOffSwitchLabelsOf(BuildContext context) =>
        _maybeOf(context, _MediaQueryAspect__media_query.onOffSwitchLabels)?.onOffSwitchLabels;

    public static bool disableAnimationsOf(BuildContext context) =>
        _of(context, _MediaQueryAspect__media_query.disableAnimations).disableAnimations;

    public static bool? maybeDisableAnimationsOf(BuildContext context) =>
        _maybeOf(context, _MediaQueryAspect__media_query.disableAnimations)?.disableAnimations;

    public static bool reduceMotionOf(BuildContext context) =>
        _of(context, _MediaQueryAspect__media_query.reduceMotion).reduceMotion;

    public static bool? maybeReduceMotionOf(BuildContext context) =>
        _maybeOf(context, _MediaQueryAspect__media_query.reduceMotion)?.reduceMotion;

    public static bool boldTextOf(BuildContext context) =>
        DartRuntimePrimitives.ConvertValue<bool>(maybeBoldTextOf(context) ?? false);

    public static bool? maybeBoldTextOf(BuildContext context) =>
        _maybeOf(context, _MediaQueryAspect__media_query.boldText)?.boldText;

    public static bool supportsAnnounceOf(BuildContext context) =>
        DartRuntimePrimitives.ConvertValue<bool>(maybeSupportsAnnounceOf(context) ?? false);

    public static bool? maybeSupportsAnnounceOf(BuildContext context) =>
        _maybeOf(context, _MediaQueryAspect__media_query.supportsAnnounce)?.supportsAnnounce;

    public static NavigationMode navigationModeOf(BuildContext context) =>
        _of(context, _MediaQueryAspect__media_query.navigationMode).navigationMode;

    public static NavigationMode? maybeNavigationModeOf(BuildContext context) =>
        _maybeOf(context, _MediaQueryAspect__media_query.navigationMode)?.navigationMode;

    public static DeviceGestureSettings gestureSettingsOf(BuildContext context) =>
        _of(context, _MediaQueryAspect__media_query.gestureSettings).gestureSettings;

    public static DeviceGestureSettings? maybeGestureSettingsOf(BuildContext context) =>
        _maybeOf(context, _MediaQueryAspect__media_query.gestureSettings)?.gestureSettings;

    public static List<DisplayFeature> displayFeaturesOf(BuildContext context) =>
        _of(context, _MediaQueryAspect__media_query.displayFeatures).displayFeatures;

    public static List<DisplayFeature>? maybeDisplayFeaturesOf(BuildContext context) =>
        _maybeOf(context, _MediaQueryAspect__media_query.displayFeatures)?.displayFeatures;

    public static bool supportsShowingSystemContextMenu(BuildContext context) =>
        _of(
            context,
            _MediaQueryAspect__media_query.supportsShowingSystemContextMenu
        ).supportsShowingSystemContextMenu;

    public static bool? maybeSupportsShowingSystemContextMenu(BuildContext context) =>
        _maybeOf(
            context,
            _MediaQueryAspect__media_query.supportsShowingSystemContextMenu
        )?.supportsShowingSystemContextMenu;

    public static double? maybeLineHeightScaleFactorOverrideOf(BuildContext context) =>
        _maybeOf(
            context,
            _MediaQueryAspect__media_query.lineHeightScaleFactorOverride
        )?.lineHeightScaleFactorOverride;

    public static double? maybeLetterSpacingOverrideOf(BuildContext context) =>
        _maybeOf(
            context,
            _MediaQueryAspect__media_query.letterSpacingOverride
        )?.letterSpacingOverride;

    public static double? maybeWordSpacingOverrideOf(BuildContext context) =>
        _maybeOf(context, _MediaQueryAspect__media_query.wordSpacingOverride)?.wordSpacingOverride;

    public static double? maybeParagraphSpacingOverrideOf(BuildContext context) =>
        _maybeOf(
            context,
            _MediaQueryAspect__media_query.paragraphSpacingOverride
        )?.paragraphSpacingOverride;

    public static BorderRadius? displayCornerRadiiOf(BuildContext context) =>
        _of(context, _MediaQueryAspect__media_query.displayCornerRadii).displayCornerRadii;

    public static BorderRadius? maybeDisplayCornerRadiiOf(BuildContext context) =>
        _maybeOf(context, _MediaQueryAspect__media_query.displayCornerRadii)?.displayCornerRadii;

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((MediaQuery)oldWidget).data));

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<MediaQueryData>("data", data, showName: false));
    }

    public override bool updateShouldNotifyDependent(
        InheritedModel<_MediaQueryAspect__media_query> oldWidget,
        HashSet<_MediaQueryAspect__media_query> dependencies
    )
    {
        FrameworkWorkCounters.Add(FrameworkWork.MediaDependentCheck);
        FrameworkWorkCounters.Add(FrameworkWork.MediaAspectSubscriptions, dependencies.Count);
        var previous = ((MediaQuery)oldWidget).data;
        // The override already receives typed aspects. Preserve short-circuit
        // semantics without cloning the set or boxing every enum value.
        foreach (var dependency in dependencies)
        {
            var notified = dependency switch
            {
                _MediaQueryAspect__media_query.size => !Equals(data.size, previous.size),
                _MediaQueryAspect__media_query.width => data.size.width != previous.size.width,
                _MediaQueryAspect__media_query.height => data.size.height != previous.size.height,
                _MediaQueryAspect__media_query.orientation => !Equals(
                    data.orientation,
                    previous.orientation
                ),
                _MediaQueryAspect__media_query.devicePixelRatio => data.devicePixelRatio
                    != previous.devicePixelRatio,
                _MediaQueryAspect__media_query.textScaleFactor => data.textScaleFactor
                    != previous.textScaleFactor,
                _MediaQueryAspect__media_query.textScaler => !Equals(
                    data.textScaler,
                    previous.textScaler
                ),
                _MediaQueryAspect__media_query.platformBrightness => !Equals(
                    data.platformBrightness,
                    previous.platformBrightness
                ),
                _MediaQueryAspect__media_query.padding => !Equals(data.padding, previous.padding),
                _MediaQueryAspect__media_query.viewInsets => !Equals(
                    data.viewInsets,
                    previous.viewInsets
                ),
                _MediaQueryAspect__media_query.viewPadding => !Equals(
                    data.viewPadding,
                    previous.viewPadding
                ),
                _MediaQueryAspect__media_query.invertColors => data.invertColors
                    != previous.invertColors,
                _MediaQueryAspect__media_query.highContrast => data.highContrast
                    != previous.highContrast,
                _MediaQueryAspect__media_query.onOffSwitchLabels => data.onOffSwitchLabels
                    != previous.onOffSwitchLabels,
                _MediaQueryAspect__media_query.disableAnimations => data.disableAnimations
                    != previous.disableAnimations,
                _MediaQueryAspect__media_query.reduceMotion => data.reduceMotion
                    != previous.reduceMotion,
                _MediaQueryAspect__media_query.boldText => data.boldText != previous.boldText,
                _MediaQueryAspect__media_query.supportsAnnounce => data.supportsAnnounce
                    != previous.supportsAnnounce,
                _MediaQueryAspect__media_query.navigationMode => !Equals(
                    data.navigationMode,
                    previous.navigationMode
                ),
                _MediaQueryAspect__media_query.gestureSettings => !Equals(
                    data.gestureSettings,
                    previous.gestureSettings
                ),
                _MediaQueryAspect__media_query.displayFeatures => !Equals(
                    data.displayFeatures,
                    previous.displayFeatures
                ),
                _MediaQueryAspect__media_query.systemGestureInsets => !Equals(
                    data.systemGestureInsets,
                    previous.systemGestureInsets
                ),
                _MediaQueryAspect__media_query.accessibleNavigation => data.accessibleNavigation
                    != previous.accessibleNavigation,
                _MediaQueryAspect__media_query.alwaysUse24HourFormat => data.alwaysUse24HourFormat
                    != previous.alwaysUse24HourFormat,
                _MediaQueryAspect__media_query.supportsShowingSystemContextMenu =>
                    data.supportsShowingSystemContextMenu
                        != previous.supportsShowingSystemContextMenu,
                _MediaQueryAspect__media_query.lineHeightScaleFactorOverride =>
                    data.lineHeightScaleFactorOverride != previous.lineHeightScaleFactorOverride,
                _MediaQueryAspect__media_query.letterSpacingOverride => data.letterSpacingOverride
                    != previous.letterSpacingOverride,
                _MediaQueryAspect__media_query.wordSpacingOverride => data.wordSpacingOverride
                    != previous.wordSpacingOverride,
                _MediaQueryAspect__media_query.paragraphSpacingOverride =>
                    data.paragraphSpacingOverride != previous.paragraphSpacingOverride,
                _MediaQueryAspect__media_query.displayCornerRadii => !Equals(
                    data.displayCornerRadii,
                    previous.displayCornerRadii
                ),
                _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
            };
            if (notified)
            {
                FrameworkWorkCounters.Add(FrameworkWork.MediaDependentNotified);
                return true;
            }
        }
        return false;
    }
}

public enum NavigationMode
{
    traditional,
    directional,
}

public class _MediaQueryFromView__media_query : StatefulWidget
{
    public virtual DorotiView view { get; private set; } = default!;
    public virtual bool ignoreParentData { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    internal _MediaQueryFromView__media_query(
        Key? key = null,
        DorotiView view = default!,
        bool ignoreParentData = false,
        Widget child = default!
    )
        : base(key: key)
    {
        this.view = view;
        this.ignoreParentData = ignoreParentData;
        this.child = child;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _MediaQueryFromViewState__media_query());
}

internal class _MediaQueryFromViewState__media_query
    : State<_MediaQueryFromView__media_query>,
        WidgetsBindingObserver
{
    internal virtual MediaQueryData? _parentData { get; set; } = default;
    internal virtual MediaQueryData? _data { get; set; } = default;

    public override void initState()
    {
        base.initState();
        WidgetsBinding.instance.addObserver(this);
        PlatformDispatcher.instance.onPlatformConfigurationChanged += HandleEnvironmentChanged;
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _updateParentData();
        _updateData();
        DartRuntimePrimitives.Assert(() => _data is not null);
    }

    public override void didUpdateWidget(_MediaQueryFromView__media_query oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (widget.ignoreParentData != oldWidget.ignoreParentData)
        {
            _updateParentData();
        }
        if ((_data is null) || (!Equals(oldWidget.view, widget.view)))
        {
            _updateData();
        }
        DartRuntimePrimitives.Assert(() => _data is not null);
    }

    internal virtual void _updateParentData()
    {
        _parentData = widget.ignoreParentData ? null : MediaQuery.maybeOf(context);
        _data = null;
    }

    internal virtual void _updateData()
    {
        FrameworkWorkCounters.Add(FrameworkWork.MediaUpdate);
        var newData = MediaQueryData.CreateFromView(widget.view, platformData: _parentData);
        if (_data is not null && newData.displayFeatures.SequenceEqual(_data.displayFeatures))
        {
            newData = newData.copyWith(displayFeatures: _data.displayFeatures);
        }

        if (!Equals(newData, _data))
        {
            FrameworkWorkCounters.Add(FrameworkWork.MediaChanged);
            setState(() =>
            {
                _data = newData;
            });
        }
    }

    public override void didChangeAccessibilityFeatures()
    {
        if (_parentData is null)
        {
            _updateData();
        }
    }

    public virtual void didChangeMetrics()
    {
        _updateData();
    }

    public virtual void didChangeTextScaleFactor()
    {
        if (_parentData is null)
        {
            _updateData();
        }
    }

    public virtual void didChangePlatformBrightness()
    {
        if (_parentData is null)
        {
            _updateData();
        }
    }

    private void HandleEnvironmentChanged(DorotiView view, PlatformConfiguration configuration)
    {
        if (view == widget.view && _parentData is null)
        {
            _updateData();
        }
    }

    public override void dispose()
    {
        PlatformDispatcher.instance.onPlatformConfigurationChanged -= HandleEnvironmentChanged;
        WidgetsBinding.instance.removeObserver(this);
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        MediaQueryData effectiveData = _data!;
        if (
            !Foundation.ConstantsLibrary.kReleaseMode
            && (_parentData is null)
            && (
                !Equals(
                    effectiveData.platformBrightness,
                    Foundation.DebugLibrary.debugBrightnessOverride
                )
            )
        )
        {
            effectiveData = effectiveData.copyWith(
                platformBrightness: Foundation.DebugLibrary.debugBrightnessOverride
            );
        }
        return new MediaQuery(data: effectiveData, child: widget.child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Media_queryLibrary
{
    internal static TextScaler _kUnspecifiedTextScaler = new _UnspecifiedTextScaler__media_query();
}

internal class _UnspecifiedTextScaler__media_query : TextScaler
{
    internal _UnspecifiedTextScaler__media_query() { }

    public override TextScaler clamp(
        double minScaleFactor = 0,
        double maxScaleFactor = double.PositiveInfinity
    ) => throw new NotImplementedException();

    public override double scale(double fontSize) => throw new NotImplementedException();

    public override double textScaleFactor => throw new NotImplementedException();
}

public class SystemTextScaler : TextScaler
{
    internal virtual PlatformDispatcher _platformDispatcher { get; private set; } = default!;
    private double __field_textScaleFactor = default!;
    public override double textScaleFactor
    {
        get => __field_textScaleFactor;
    }

    private readonly Func<double, double>? _fontSizeScaler;

    public SystemTextScaler(PlatformDispatcher dispatcher)
        : this(dispatcher.configurationSnapshot, dispatcher) { }

    public SystemTextScaler(
        PlatformConfiguration? configuration,
        PlatformDispatcher _platformDispatcher
    )
    {
        this._platformDispatcher = _platformDispatcher;
        __field_textScaleFactor =
            configuration?.textScaleFactor ?? _platformDispatcher.textScaleFactor;
        _fontSizeScaler = configuration?.fontSizeScaler;
    }

    public override double scale(double fontSize)
    {
        if (!double.IsFinite(fontSize) || fontSize < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fontSize));
        }

        return _fontSizeScaler?.Invoke(fontSize) ?? (fontSize * textScaleFactor);
    }

    public override bool Equals(object? other)
    {
        if (DartRuntimePrimitives.Identical(this, other))
        {
            return true;
        }
        return other switch
        {
            SystemTextScaler { textScaleFactor: double textScaleFactorLocal } => textScaleFactor
                == textScaleFactorLocal,
            var noScaling when Equals(noScaling, TextScaler.noScaling) => textScaleFactor == 1.0,
            _ => false,
        };
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(textScaleFactor.GetHashCode());

    public override string ToString() =>
        $"SystemTextScaler ({((textScaleFactor == 1.0) ? "no scaling" : $"{textScaleFactor}x")})";
}
