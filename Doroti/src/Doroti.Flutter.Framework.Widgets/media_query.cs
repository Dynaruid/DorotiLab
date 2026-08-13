// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/media_query.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public enum Orientation
{
    portrait,
    landscape
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
    displayCornerRadii
}

public class MediaQueryData
{
    public virtual Size size { get; private set; } = default!;
    public virtual double devicePixelRatio { get; private set; } = default!;
    internal virtual double _textScaleFactor { get; private set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Painting.TextScaler _textScaler { get; private set; } = default!;
    public virtual Brightness platformBrightness { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets viewInsets { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets padding { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets viewPadding { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets systemGestureInsets { get; private set; } = default!;
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
    public virtual global::Doroti.Generated.Framework.Gestures.DeviceGestureSettings gestureSettings { get; private set; } = default!;
    public virtual List<global::Doroti.Flutter.Ui.DisplayFeature> displayFeatures { get; private set; } = default!;
    public virtual bool supportsShowingSystemContextMenu { get; private set; } = default!;
    public virtual double? lineHeightScaleFactorOverride { get; private set; }
    public virtual double? letterSpacingOverride { get; private set; }
    public virtual double? wordSpacingOverride { get; private set; }
    public virtual double? paragraphSpacingOverride { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadius? displayCornerRadii { get; private set; }

    public MediaQueryData(Size size = default, double devicePixelRatio = 1.0, double textScaleFactor = 1.0, global::Doroti.Generated.Framework.Painting.TextScaler textScaler = default!, Brightness platformBrightness = Brightness.light, global::Doroti.Generated.Framework.Painting.EdgeInsets padding = default!, global::Doroti.Generated.Framework.Painting.EdgeInsets viewInsets = default!, global::Doroti.Generated.Framework.Painting.EdgeInsets systemGestureInsets = default!, global::Doroti.Generated.Framework.Painting.EdgeInsets viewPadding = default!, bool alwaysUse24HourFormat = false, bool accessibleNavigation = false, bool invertColors = false, bool highContrast = false, bool onOffSwitchLabels = false, bool disableAnimations = false, bool reduceMotion = false, bool boldText = false, bool supportsAnnounce = false, NavigationMode navigationMode = NavigationMode.traditional, global::Doroti.Generated.Framework.Gestures.DeviceGestureSettings gestureSettings = default!, List<global::Doroti.Flutter.Ui.DisplayFeature> displayFeatures = default!, bool supportsShowingSystemContextMenu = false, double? lineHeightScaleFactorOverride = null, double? letterSpacingOverride = null, double? wordSpacingOverride = null, double? paragraphSpacingOverride = null, global::Doroti.Generated.Framework.Painting.BorderRadius? displayCornerRadii = null)
    {
        global::Doroti.Generated.Framework.Painting.TextScaler __textScaler = textScaler ?? Media_queryLibrary._kUnspecifiedTextScaler;
        global::Doroti.Generated.Framework.Painting.EdgeInsets __padding = padding ?? global::Doroti.Generated.Framework.Painting.EdgeInsets.zero;
        global::Doroti.Generated.Framework.Painting.EdgeInsets __viewInsets = viewInsets ?? global::Doroti.Generated.Framework.Painting.EdgeInsets.zero;
        global::Doroti.Generated.Framework.Painting.EdgeInsets __systemGestureInsets = systemGestureInsets ?? global::Doroti.Generated.Framework.Painting.EdgeInsets.zero;
        global::Doroti.Generated.Framework.Painting.EdgeInsets __viewPadding = viewPadding ?? global::Doroti.Generated.Framework.Painting.EdgeInsets.zero;
        global::Doroti.Generated.Framework.Gestures.DeviceGestureSettings __gestureSettings = gestureSettings ?? new global::Doroti.Generated.Framework.Gestures.DeviceGestureSettings(touchSlop: global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kTouchSlop);
        List<global::Doroti.Flutter.Ui.DisplayFeature> __displayFeatures = displayFeatures ?? new List<global::Doroti.Flutter.Ui.DisplayFeature>();
        this.size = size;
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
        this._textScaleFactor = DartRuntimePrimitives.RequireValue(textScaleFactor);
        this._textScaler = __textScaler;
        System.Diagnostics.Debug.Assert((DartRuntimePrimitives.Identical(__textScaler, Media_queryLibrary._kUnspecifiedTextScaler) || (DartRuntimePrimitives.RequireValue(textScaleFactor) == 1.0)));
    }

    public static MediaQueryData CreateFromWindow(FlutterView window)
        => MediaQueryData.CreateFromView(window);

    public static MediaQueryData CreateFromView(FlutterView view, MediaQueryData? platformData = null)
    {
        var __instance = new MediaQueryData(default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!);
        __instance.size = (view.physicalSize / view.devicePixelRatio);
        __instance.devicePixelRatio = view.devicePixelRatio;
        __instance._textScaleFactor = 1.0;
        __instance._textScaler = MediaQueryData._textScalerFromView(view, platformData);
        __instance.platformBrightness = (platformData?.platformBrightness ?? view.platformDispatcher.platformBrightness);
        __instance.padding = global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateFromViewPadding(view.padding, view.devicePixelRatio);
        __instance.viewPadding = global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateFromViewPadding(view.viewPadding, view.devicePixelRatio);
        __instance.viewInsets = global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateFromViewPadding(view.viewInsets, view.devicePixelRatio);
        __instance.systemGestureInsets = global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateFromViewPadding(view.systemGestureInsets, view.devicePixelRatio);
        __instance.accessibleNavigation = (platformData?.accessibleNavigation ?? view.platformDispatcher.accessibilityFeatures.accessibleNavigation);
        __instance.invertColors = (platformData?.invertColors ?? view.platformDispatcher.accessibilityFeatures.invertColors);
        __instance.disableAnimations = (platformData?.disableAnimations ?? view.platformDispatcher.accessibilityFeatures.disableAnimations);
        __instance.reduceMotion = (platformData?.reduceMotion ?? view.platformDispatcher.accessibilityFeatures.reduceMotion);
        __instance.boldText = (platformData?.boldText ?? view.platformDispatcher.accessibilityFeatures.boldText);
        __instance.supportsAnnounce = (platformData?.supportsAnnounce ?? view.platformDispatcher.accessibilityFeatures.supportsAnnounce);
        __instance.highContrast = (platformData?.highContrast ?? view.platformDispatcher.accessibilityFeatures.highContrast);
        __instance.onOffSwitchLabels = (platformData?.onOffSwitchLabels ?? view.platformDispatcher.accessibilityFeatures.onOffSwitchLabels);
        __instance.alwaysUse24HourFormat = (platformData?.alwaysUse24HourFormat ?? view.platformDispatcher.alwaysUse24HourFormat);
        __instance.navigationMode = (platformData?.navigationMode ?? NavigationMode.traditional);
        __instance.gestureSettings = global::Doroti.Generated.Framework.Gestures.DeviceGestureSettings.CreateFromView(view);
        __instance.displayFeatures = view.displayFeatures.ToList();
        __instance.supportsShowingSystemContextMenu = (platformData?.supportsShowingSystemContextMenu ?? view.platformDispatcher.supportsShowingSystemContextMenu);
        __instance.lineHeightScaleFactorOverride = (platformData?.lineHeightScaleFactorOverride ?? view.platformDispatcher.lineHeightScaleFactorOverride);
        __instance.letterSpacingOverride = (platformData?.letterSpacingOverride ?? view.platformDispatcher.letterSpacingOverride);
        __instance.wordSpacingOverride = (platformData?.wordSpacingOverride ?? view.platformDispatcher.wordSpacingOverride);
        __instance.paragraphSpacingOverride = (platformData?.paragraphSpacingOverride ?? view.platformDispatcher.paragraphSpacingOverride);
        __instance.displayCornerRadii = MediaQueryData._displayCornerRadiiFromView(view);
        return __instance;
    }

    internal static global::Doroti.Generated.Framework.Painting.TextScaler _textScalerFromView(FlutterView view, MediaQueryData? platformData)
    {
        return (platformData?.textScaler ?? new SystemTextScaler(view.platformDispatcher));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Generated.Framework.Painting.BorderRadius? _displayCornerRadiiFromView(FlutterView view)
    {
        global::Doroti.Flutter.Ui.DisplayCornerRadii? displayCornerRadii__15687 = ((global::Doroti.Flutter.Ui.DisplayCornerRadii?)(object?)view.displayCornerRadii);
        if ((displayCornerRadii__15687 is null))
        {
            return ((global::Doroti.Generated.Framework.Painting.BorderRadius)(object)null);
        }
        double devicePixelRatio__15813 = view.devicePixelRatio;
        return new global::Doroti.Generated.Framework.Painting.BorderRadius(topLeft: global::Doroti.Flutter.Ui.Radius.circular((displayCornerRadii__15687.topLeft / DartRuntimePrimitives.RequireValue(devicePixelRatio__15813))), topRight: global::Doroti.Flutter.Ui.Radius.circular((displayCornerRadii__15687.topRight / DartRuntimePrimitives.RequireValue(devicePixelRatio__15813))), bottomRight: global::Doroti.Flutter.Ui.Radius.circular((displayCornerRadii__15687.bottomRight / DartRuntimePrimitives.RequireValue(devicePixelRatio__15813))), bottomLeft: global::Doroti.Flutter.Ui.Radius.circular((displayCornerRadii__15687.bottomLeft / DartRuntimePrimitives.RequireValue(devicePixelRatio__15813))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double textScaleFactor => ((global::Doroti.Generated.Framework.Painting.TextScaler)this.textScaler).textScaleFactor;
    public virtual global::Doroti.Generated.Framework.Painting.TextScaler textScaler
    {
        get
        {
            if (!DartRuntimePrimitives.Identical(Media_queryLibrary._kUnspecifiedTextScaler, this._textScaler))
            {
                return this._textScaler;
            }
            return ((this._textScaleFactor == 1.0) ? global::Doroti.Generated.Framework.Painting.TextScaler.noScaling : global::Doroti.Generated.Framework.Painting.TextScaler.CreateLinear(this._textScaleFactor));
            return default!;
        }
    }
    public virtual Orientation orientation
    {
        get
        {
            return ((this.size.width > this.size.height) ? Orientation.landscape : Orientation.portrait);
            return default!;
        }
    }
    public virtual MediaQueryData copyWith(Size? size = null, double? devicePixelRatio = null, double? textScaleFactor = null, global::Doroti.Generated.Framework.Painting.TextScaler? textScaler = null, Brightness? platformBrightness = null, global::Doroti.Generated.Framework.Painting.EdgeInsets? padding = null, global::Doroti.Generated.Framework.Painting.EdgeInsets? viewPadding = null, global::Doroti.Generated.Framework.Painting.EdgeInsets? viewInsets = null, global::Doroti.Generated.Framework.Painting.EdgeInsets? systemGestureInsets = null, bool? alwaysUse24HourFormat = null, bool? highContrast = null, bool? onOffSwitchLabels = null, bool? disableAnimations = null, bool? reduceMotion = null, bool? invertColors = null, bool? accessibleNavigation = null, bool? boldText = null, bool? supportsAnnounce = null, NavigationMode? navigationMode = null, global::Doroti.Generated.Framework.Gestures.DeviceGestureSettings? gestureSettings = null, List<global::Doroti.Flutter.Ui.DisplayFeature>? displayFeatures = null, bool? supportsShowingSystemContextMenu = null)
    {
        DartRuntimePrimitives.Assert(() => ((textScaleFactor is null) || (textScaler is null)));
        if ((textScaleFactor is not null))
        {
            double textScaleFactor__value39066 = DartRuntimePrimitives.RequireValue(textScaleFactor);
            textScaler ??= global::Doroti.Generated.Framework.Painting.TextScaler.CreateLinear(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(textScaleFactor__value39066)));
        }
        return new MediaQueryData(size: (size ?? this.size), devicePixelRatio: (devicePixelRatio ?? this.devicePixelRatio), textScaler: (textScaler ?? this.textScaler), platformBrightness: (platformBrightness ?? this.platformBrightness), padding: (padding ?? this.padding), viewPadding: (viewPadding ?? this.viewPadding), viewInsets: (viewInsets ?? this.viewInsets), systemGestureInsets: (systemGestureInsets ?? this.systemGestureInsets), alwaysUse24HourFormat: (alwaysUse24HourFormat ?? this.alwaysUse24HourFormat), invertColors: (invertColors ?? this.invertColors), highContrast: (highContrast ?? this.highContrast), onOffSwitchLabels: (onOffSwitchLabels ?? this.onOffSwitchLabels), disableAnimations: (disableAnimations ?? this.disableAnimations), reduceMotion: (reduceMotion ?? this.reduceMotion), accessibleNavigation: (accessibleNavigation ?? this.accessibleNavigation), boldText: (boldText ?? this.boldText), supportsAnnounce: (supportsAnnounce ?? this.supportsAnnounce), navigationMode: (navigationMode ?? this.navigationMode), gestureSettings: (gestureSettings ?? this.gestureSettings), displayFeatures: (displayFeatures ?? this.displayFeatures), supportsShowingSystemContextMenu: (supportsShowingSystemContextMenu ?? this.supportsShowingSystemContextMenu), lineHeightScaleFactorOverride: this.lineHeightScaleFactorOverride, letterSpacingOverride: this.letterSpacingOverride, wordSpacingOverride: this.wordSpacingOverride, paragraphSpacingOverride: this.paragraphSpacingOverride, displayCornerRadii: this.displayCornerRadii);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual MediaQueryData applyTextStyleOverrides(double? lineHeightScaleFactorOverride, double? letterSpacingOverride, double? wordSpacingOverride, double? paragraphSpacingOverride)
    {
        return new MediaQueryData(size: DartRuntimePrimitives.RequireValue(this.size), devicePixelRatio: DartRuntimePrimitives.RequireValue(this.devicePixelRatio), textScaler: this.textScaler, platformBrightness: DartRuntimePrimitives.RequireValue(this.platformBrightness), padding: this.padding, viewPadding: this.viewPadding, viewInsets: this.viewInsets, systemGestureInsets: this.systemGestureInsets, alwaysUse24HourFormat: DartRuntimePrimitives.RequireValue(this.alwaysUse24HourFormat), invertColors: DartRuntimePrimitives.RequireValue(this.invertColors), highContrast: DartRuntimePrimitives.RequireValue(this.highContrast), onOffSwitchLabels: DartRuntimePrimitives.RequireValue(this.onOffSwitchLabels), disableAnimations: DartRuntimePrimitives.RequireValue(this.disableAnimations), reduceMotion: DartRuntimePrimitives.RequireValue(this.reduceMotion), accessibleNavigation: DartRuntimePrimitives.RequireValue(this.accessibleNavigation), boldText: DartRuntimePrimitives.RequireValue(this.boldText), supportsAnnounce: DartRuntimePrimitives.RequireValue(this.supportsAnnounce), navigationMode: DartRuntimePrimitives.RequireValue(this.navigationMode), gestureSettings: this.gestureSettings, displayFeatures: this.displayFeatures, supportsShowingSystemContextMenu: DartRuntimePrimitives.RequireValue(this.supportsShowingSystemContextMenu), lineHeightScaleFactorOverride: lineHeightScaleFactorOverride, letterSpacingOverride: letterSpacingOverride, wordSpacingOverride: wordSpacingOverride, paragraphSpacingOverride: paragraphSpacingOverride, displayCornerRadii: this.displayCornerRadii);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual MediaQueryData applyDisplayCornerRadii(global::Doroti.Generated.Framework.Painting.BorderRadius? displayCornerRadii)
    {
        return new MediaQueryData(size: DartRuntimePrimitives.RequireValue(this.size), devicePixelRatio: DartRuntimePrimitives.RequireValue(this.devicePixelRatio), textScaler: this.textScaler, platformBrightness: DartRuntimePrimitives.RequireValue(this.platformBrightness), padding: this.padding, viewPadding: this.viewPadding, viewInsets: this.viewInsets, systemGestureInsets: this.systemGestureInsets, alwaysUse24HourFormat: DartRuntimePrimitives.RequireValue(this.alwaysUse24HourFormat), invertColors: DartRuntimePrimitives.RequireValue(this.invertColors), highContrast: DartRuntimePrimitives.RequireValue(this.highContrast), onOffSwitchLabels: DartRuntimePrimitives.RequireValue(this.onOffSwitchLabels), disableAnimations: DartRuntimePrimitives.RequireValue(this.disableAnimations), reduceMotion: DartRuntimePrimitives.RequireValue(this.reduceMotion), accessibleNavigation: DartRuntimePrimitives.RequireValue(this.accessibleNavigation), boldText: DartRuntimePrimitives.RequireValue(this.boldText), supportsAnnounce: DartRuntimePrimitives.RequireValue(this.supportsAnnounce), navigationMode: DartRuntimePrimitives.RequireValue(this.navigationMode), gestureSettings: this.gestureSettings, displayFeatures: this.displayFeatures, supportsShowingSystemContextMenu: DartRuntimePrimitives.RequireValue(this.supportsShowingSystemContextMenu), lineHeightScaleFactorOverride: this.lineHeightScaleFactorOverride, letterSpacingOverride: this.letterSpacingOverride, wordSpacingOverride: this.wordSpacingOverride, paragraphSpacingOverride: this.paragraphSpacingOverride, displayCornerRadii: displayCornerRadii);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual MediaQueryData removePadding(bool removeLeft = false, bool removeTop = false, bool removeRight = false, bool removeBottom = false)
    {
        if (!((((removeLeft || removeTop) || removeRight) || removeBottom)))
        {
            return this;
        }
        return ((MediaQueryData)(object?)copyWith(padding: this.padding.copyWith(left: (removeLeft ? 0.0 : null), top: (removeTop ? 0.0 : null), right: (removeRight ? 0.0 : null), bottom: (removeBottom ? 0.0 : null)), viewPadding: this.viewPadding.copyWith(left: (removeLeft ? Math.Max(0.0, (((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.viewPadding).left - ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.padding).left)) : null), top: (removeTop ? Math.Max(0.0, (((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.viewPadding).top - ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.padding).top)) : null), right: (removeRight ? Math.Max(0.0, (((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.viewPadding).right - ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.padding).right)) : null), bottom: (removeBottom ? Math.Max(0.0, (((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.viewPadding).bottom - ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.padding).bottom)) : null))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual MediaQueryData removeViewInsets(bool removeLeft = false, bool removeTop = false, bool removeRight = false, bool removeBottom = false)
    {
        if (!((((removeLeft || removeTop) || removeRight) || removeBottom)))
        {
            return this;
        }
        return ((MediaQueryData)(object?)copyWith(viewPadding: this.viewPadding.copyWith(left: (removeLeft ? Math.Max(0.0, (((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.viewPadding).left - ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.viewInsets).left)) : null), top: (removeTop ? Math.Max(0.0, (((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.viewPadding).top - ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.viewInsets).top)) : null), right: (removeRight ? Math.Max(0.0, (((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.viewPadding).right - ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.viewInsets).right)) : null), bottom: (removeBottom ? Math.Max(0.0, (((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.viewPadding).bottom - ((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.viewInsets).bottom)) : null)), viewInsets: this.viewInsets.copyWith(left: (removeLeft ? 0.0 : null), top: (removeTop ? 0.0 : null), right: (removeRight ? 0.0 : null), bottom: (removeBottom ? 0.0 : null))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual MediaQueryData removeViewPadding(bool removeLeft = false, bool removeTop = false, bool removeRight = false, bool removeBottom = false)
    {
        if (!((((removeLeft || removeTop) || removeRight) || removeBottom)))
        {
            return this;
        }
        return ((MediaQueryData)(object?)copyWith(padding: this.padding.copyWith(left: (removeLeft ? 0.0 : null), top: (removeTop ? 0.0 : null), right: (removeRight ? 0.0 : null), bottom: (removeBottom ? 0.0 : null)), viewPadding: this.viewPadding.copyWith(left: (removeLeft ? 0.0 : null), top: (removeTop ? 0.0 : null), right: (removeRight ? 0.0 : null), bottom: (removeBottom ? 0.0 : null))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual MediaQueryData removeDisplayFeatures(Rect subScreen)
    {
        DartRuntimePrimitives.Assert(() => ((((subScreen.left >= 0.0) && (subScreen.top >= 0.0)) && (subScreen.right <= this.size.width)) && (subScreen.bottom <= this.size.height)), () => (object?)"'subScreen' argument cannot be outside the bounds of the screen");
        if (((object.Equals(subScreen.size, this.size)) && (object.Equals(subScreen.topLeft, Offset.zero))))
        {
            return this;
        }
        double rightInset__49607 = (this.size.width - subScreen.right);
        double bottomInset__49667 = (this.size.height - subScreen.bottom);
        return ((MediaQueryData)(object?)copyWith(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(left: Math.Max(0.0, (((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.padding).left - subScreen.left)), top: Math.Max(0.0, (((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.padding).top - subScreen.top)), right: Math.Max(0.0, (((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.padding).right - rightInset__49607)), bottom: Math.Max(0.0, (((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.padding).bottom - bottomInset__49667))), viewPadding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(left: Math.Max(0.0, (((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.viewPadding).left - subScreen.left)), top: Math.Max(0.0, (((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.viewPadding).top - subScreen.top)), right: Math.Max(0.0, (((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.viewPadding).right - rightInset__49607)), bottom: Math.Max(0.0, (((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.viewPadding).bottom - bottomInset__49667))), viewInsets: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(left: Math.Max(0.0, (((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.viewInsets).left - subScreen.left)), top: Math.Max(0.0, (((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.viewInsets).top - subScreen.top)), right: Math.Max(0.0, (((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.viewInsets).right - rightInset__49607)), bottom: Math.Max(0.0, (((global::Doroti.Generated.Framework.Painting.EdgeInsets)this.viewInsets).bottom - bottomInset__49667))), displayFeatures: this.displayFeatures.where(((displayFeature) => subScreen.overlaps(displayFeature.bounds))).ToList()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as MediaQueryData;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((((((((((((((((((((((__other is MediaQueryData) && (object.Equals(((MediaQueryData)((MediaQueryData)__other)).size, this.size))) && (((MediaQueryData)((MediaQueryData)__other)).devicePixelRatio == this.devicePixelRatio)) && (((MediaQueryData)((MediaQueryData)__other)).textScaleFactor == this.textScaleFactor)) && (object.Equals(((MediaQueryData)((MediaQueryData)__other)).platformBrightness, this.platformBrightness))) && (object.Equals(((MediaQueryData)((MediaQueryData)__other)).padding, this.padding))) && (object.Equals(((MediaQueryData)((MediaQueryData)__other)).viewPadding, this.viewPadding))) && (object.Equals(((MediaQueryData)((MediaQueryData)__other)).viewInsets, this.viewInsets))) && (object.Equals(((MediaQueryData)((MediaQueryData)__other)).systemGestureInsets, this.systemGestureInsets))) && (((MediaQueryData)((MediaQueryData)__other)).alwaysUse24HourFormat == this.alwaysUse24HourFormat)) && (((MediaQueryData)((MediaQueryData)__other)).highContrast == this.highContrast)) && (((MediaQueryData)((MediaQueryData)__other)).onOffSwitchLabels == this.onOffSwitchLabels)) && (((MediaQueryData)((MediaQueryData)__other)).disableAnimations == this.disableAnimations)) && (((MediaQueryData)((MediaQueryData)__other)).reduceMotion == this.reduceMotion)) && (((MediaQueryData)((MediaQueryData)__other)).invertColors == this.invertColors)) && (((MediaQueryData)((MediaQueryData)__other)).accessibleNavigation == this.accessibleNavigation)) && (((MediaQueryData)((MediaQueryData)__other)).boldText == this.boldText)) && (((MediaQueryData)((MediaQueryData)__other)).supportsAnnounce == this.supportsAnnounce)) && (object.Equals(((MediaQueryData)((MediaQueryData)__other)).navigationMode, this.navigationMode))) && (object.Equals(((MediaQueryData)((MediaQueryData)__other)).gestureSettings, this.gestureSettings))) && global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals(((MediaQueryData)((MediaQueryData)__other)).displayFeatures, this.displayFeatures)) && (((MediaQueryData)((MediaQueryData)__other)).supportsShowingSystemContextMenu == this.supportsShowingSystemContextMenu)) && (((MediaQueryData)((MediaQueryData)__other)).lineHeightScaleFactorOverride == this.lineHeightScaleFactorOverride)) && (((MediaQueryData)((MediaQueryData)__other)).letterSpacingOverride == this.letterSpacingOverride)) && (((MediaQueryData)((MediaQueryData)__other)).wordSpacingOverride == this.wordSpacingOverride)) && (((MediaQueryData)((MediaQueryData)__other)).paragraphSpacingOverride == this.paragraphSpacingOverride)) && (object.Equals(((MediaQueryData)((MediaQueryData)__other)).displayCornerRadii, this.displayCornerRadii)));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.size, this.devicePixelRatio, this.textScaleFactor, this.platformBrightness, this.padding, this.viewPadding, this.viewInsets, this.alwaysUse24HourFormat, this.highContrast, this.onOffSwitchLabels, this.disableAnimations, this.reduceMotion, this.invertColors, this.accessibleNavigation, this.boldText, this.navigationMode, this.gestureSettings, FoundationRuntimePorts.ObjectHashAll(this.displayFeatures), this.supportsShowingSystemContextMenu, FoundationRuntimePorts.ObjectHash(this.lineHeightScaleFactorOverride, this.letterSpacingOverride, this.wordSpacingOverride, this.paragraphSpacingOverride, this.displayCornerRadii)));
    public override string ToString()
    {
        var properties__53032 = new List<string> { $"size: {this.size}", $"devicePixelRatio: {this.devicePixelRatio.toStringAsFixed(1L)}", $"textScaler: {this.textScaler}", $"platformBrightness: {this.platformBrightness}", $"padding: {this.padding}", $"viewPadding: {this.viewPadding}", $"viewInsets: {this.viewInsets}", $"systemGestureInsets: {this.systemGestureInsets}", $"alwaysUse24HourFormat: {this.alwaysUse24HourFormat}", $"accessibleNavigation: {this.accessibleNavigation}", $"highContrast: {this.highContrast}", $"onOffSwitchLabels: {this.onOffSwitchLabels}", $"disableAnimations: {this.disableAnimations}", $"reduceMotion: {this.reduceMotion}", $"invertColors: {this.invertColors}", $"boldText: {this.boldText}", $"navigationMode: {this.navigationMode.ToString()}", $"gestureSettings: {this.gestureSettings}", $"displayFeatures: {this.displayFeatures}", $"supportsShowingSystemContextMenu: {this.supportsShowingSystemContextMenu}", $"lineHeightScaleFactorOverride: {this.lineHeightScaleFactorOverride}", $"letterSpacingOverride: {this.letterSpacingOverride}", $"wordSpacingOverride: {this.wordSpacingOverride}", $"paragraphSpacingOverride: {this.paragraphSpacingOverride}", $"displayCornerRadii: {this.displayCornerRadii}" };
        return $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "MediaQueryData"))}({string.Join(", ", properties__53032)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class MediaQuery : InheritedModel<_MediaQueryAspect__media_query>
{
    public virtual MediaQueryData data { get; private set; } = default!;

    public MediaQuery(global::Doroti.Generated.Framework.Foundation.Key? key = null, MediaQueryData data = default!, Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static MediaQuery CreateRemovePadding(global::Doroti.Generated.Framework.Foundation.Key? key = null, BuildContext context = default!, bool removeLeft = false, bool removeTop = false, bool removeRight = false, bool removeBottom = false, Widget child = default!)
    {
        var __instance = new MediaQuery(key: key, child: child);
        __instance.data = MediaQuery.of(context).removePadding(removeLeft: removeLeft, removeTop: removeTop, removeRight: removeRight, removeBottom: removeBottom);
        return __instance;
    }

    public static MediaQuery CreateRemoveViewInsets(global::Doroti.Generated.Framework.Foundation.Key? key = null, BuildContext context = default!, bool removeLeft = false, bool removeTop = false, bool removeRight = false, bool removeBottom = false, Widget child = default!)
    {
        var __instance = new MediaQuery(key: key, child: child);
        __instance.data = MediaQuery.of(context).removeViewInsets(removeLeft: removeLeft, removeTop: removeTop, removeRight: removeRight, removeBottom: removeBottom);
        return __instance;
    }

    public static MediaQuery CreateRemoveViewPadding(global::Doroti.Generated.Framework.Foundation.Key? key = null, BuildContext context = default!, bool removeLeft = false, bool removeTop = false, bool removeRight = false, bool removeBottom = false, Widget child = default!)
    {
        var __instance = new MediaQuery(key: key, child: child);
        __instance.data = MediaQuery.of(context).removeViewPadding(removeLeft: removeLeft, removeTop: removeTop, removeRight: removeRight, removeBottom: removeBottom);
        return __instance;
    }

    public static Widget applyTextStyleOverrides(global::Doroti.Generated.Framework.Foundation.Key? key = null, double? lineHeightScaleFactorOverride = default!, double? letterSpacingOverride = default!, double? wordSpacingOverride = default!, double? paragraphSpacingOverride = default!, Widget child = default!)
    {
        return ((Widget)(object?)new Builder(key: key, builder: ((global::System.Func<BuildContext, Widget>)((context) => {
DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
return ((Widget)(object?)new MediaQuery(data: MediaQuery.of(context).applyTextStyleOverrides(lineHeightScaleFactorOverride: lineHeightScaleFactorOverride, letterSpacingOverride: letterSpacingOverride, wordSpacingOverride: wordSpacingOverride, paragraphSpacingOverride: paragraphSpacingOverride), child: child));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Widget fromWindow(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!)
    {
        return ((Widget)(object?)new _MediaQueryFromView__media_query(key: key, view: WidgetsBinding.instance.window, ignoreParentData: true, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Widget fromView(global::Doroti.Generated.Framework.Foundation.Key? key = null, FlutterView view = default!, Widget child = default!)
    {
        return ((Widget)(object?)new _MediaQueryFromView__media_query(key: key, view: view, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Widget withNoTextScaling(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!)
    {
        return ((Widget)(object?)new Builder(key: key, builder: ((global::System.Func<BuildContext, Widget>)((context) => {
DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
return ((Widget)(object?)new MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: global::Doroti.Generated.Framework.Painting.TextScaler.noScaling), child: child));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Widget withClampedTextScaling(global::Doroti.Generated.Framework.Foundation.Key? key = null, double minScaleFactor = 0.0, double maxScaleFactor = double.PositiveInfinity, Widget child = default!)
    {
        DartRuntimePrimitives.Assert(() => (maxScaleFactor >= minScaleFactor));
        DartRuntimePrimitives.Assert(() => !double.IsNaN(maxScaleFactor));
        DartRuntimePrimitives.Assert(() => double.IsFinite(minScaleFactor));
        DartRuntimePrimitives.Assert(() => (minScaleFactor >= 0L));
        return ((Widget)(object?)new Builder(builder: ((global::System.Func<BuildContext, Widget>)((context) => {
DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
MediaQueryData data__65806 = ((MediaQueryData)(object?)MediaQuery.of(context));
return ((Widget)(object?)new MediaQuery(data: data__65806.copyWith(textScaler: ((MediaQueryData)data__65806).textScaler.clamp(minScaleFactor: minScaleFactor, maxScaleFactor: maxScaleFactor)), child: child));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static MediaQueryData of(BuildContext context)
    {
        return ((MediaQueryData)(object?)MediaQuery._of(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static MediaQueryData _of(BuildContext context, _MediaQueryAspect__media_query? aspect = null)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        return InheritedModel<object>.inheritFrom<MediaQuery>(context, aspect: aspect)!.data;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static MediaQueryData? maybeOf(BuildContext context)
    {
        return ((MediaQueryData?)(object?)MediaQuery._maybeOf(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static MediaQueryData? _maybeOf(BuildContext context, _MediaQueryAspect__media_query? aspect = null)
    {
        return InheritedModel<object>.inheritFrom<MediaQuery>(context, aspect: aspect)?.data;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Flutter.Ui.Size sizeOf(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Size>(MediaQuery._of(context, _MediaQueryAspect__media_query.size).size);
    public static global::Doroti.Flutter.Ui.Size? maybeSizeOf(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Size>(MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.size)?.size);
    public static double widthOf(BuildContext context) => MediaQuery._of(context, _MediaQueryAspect__media_query.width).size.width;
    public static double? maybeWidthOf(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.width)?.size.width;
    public static double heightOf(BuildContext context) => MediaQuery._of(context, _MediaQueryAspect__media_query.height).size.height;
    public static double? maybeHeightOf(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.height)?.size.height;
    public static Orientation orientationOf(BuildContext context) => MediaQuery._of(context, _MediaQueryAspect__media_query.orientation).orientation;
    public static Orientation? maybeOrientationOf(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.orientation)?.orientation;
    public static double devicePixelRatioOf(BuildContext context) => MediaQuery._of(context, _MediaQueryAspect__media_query.devicePixelRatio).devicePixelRatio;
    public static double? maybeDevicePixelRatioOf(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.devicePixelRatio)?.devicePixelRatio;
    public static double textScaleFactorOf(BuildContext context) => DartRuntimePrimitives.ConvertValue<double>((MediaQuery.maybeTextScaleFactorOf(context) ?? 1.0));
    public static double? maybeTextScaleFactorOf(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.textScaleFactor)?.textScaleFactor;
    public static global::Doroti.Generated.Framework.Painting.TextScaler textScalerOf(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.TextScaler>((MediaQuery.maybeTextScalerOf(context) ?? global::Doroti.Generated.Framework.Painting.TextScaler.noScaling));
    public static global::Doroti.Generated.Framework.Painting.TextScaler? maybeTextScalerOf(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.textScaler)?.textScaler;
    public static global::Doroti.Flutter.Ui.Brightness platformBrightnessOf(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Brightness>((MediaQuery.maybePlatformBrightnessOf(context) ?? Brightness.light));
    public static global::Doroti.Flutter.Ui.Brightness? maybePlatformBrightnessOf(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Brightness>(MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.platformBrightness)?.platformBrightness);
    public static global::Doroti.Generated.Framework.Painting.EdgeInsets paddingOf(BuildContext context) => MediaQuery._of(context, _MediaQueryAspect__media_query.padding).padding;
    public static global::Doroti.Generated.Framework.Painting.EdgeInsets? maybePaddingOf(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.padding)?.padding;
    public static global::Doroti.Generated.Framework.Painting.EdgeInsets viewInsetsOf(BuildContext context) => MediaQuery._of(context, _MediaQueryAspect__media_query.viewInsets).viewInsets;
    public static global::Doroti.Generated.Framework.Painting.EdgeInsets? maybeViewInsetsOf(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.viewInsets)?.viewInsets;
    public static global::Doroti.Generated.Framework.Painting.EdgeInsets systemGestureInsetsOf(BuildContext context) => MediaQuery._of(context, _MediaQueryAspect__media_query.systemGestureInsets).systemGestureInsets;
    public static global::Doroti.Generated.Framework.Painting.EdgeInsets? maybeSystemGestureInsetsOf(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.systemGestureInsets)?.systemGestureInsets;
    public static global::Doroti.Generated.Framework.Painting.EdgeInsets viewPaddingOf(BuildContext context) => MediaQuery._of(context, _MediaQueryAspect__media_query.viewPadding).viewPadding;
    public static global::Doroti.Generated.Framework.Painting.EdgeInsets? maybeViewPaddingOf(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.viewPadding)?.viewPadding;
    public static bool alwaysUse24HourFormatOf(BuildContext context) => MediaQuery._of(context, _MediaQueryAspect__media_query.alwaysUse24HourFormat).alwaysUse24HourFormat;
    public static bool? maybeAlwaysUse24HourFormatOf(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.alwaysUse24HourFormat)?.alwaysUse24HourFormat;
    public static bool accessibleNavigationOf(BuildContext context) => MediaQuery._of(context, _MediaQueryAspect__media_query.accessibleNavigation).accessibleNavigation;
    public static bool? maybeAccessibleNavigationOf(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.accessibleNavigation)?.accessibleNavigation;
    public static bool invertColorsOf(BuildContext context) => MediaQuery._of(context, _MediaQueryAspect__media_query.invertColors).invertColors;
    public static bool? maybeInvertColorsOf(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.invertColors)?.invertColors;
    public static bool highContrastOf(BuildContext context) => DartRuntimePrimitives.ConvertValue<bool>((MediaQuery.maybeHighContrastOf(context) ?? false));
    public static bool? maybeHighContrastOf(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.highContrast)?.highContrast;
    public static bool onOffSwitchLabelsOf(BuildContext context) => DartRuntimePrimitives.ConvertValue<bool>((MediaQuery.maybeOnOffSwitchLabelsOf(context) ?? false));
    public static bool? maybeOnOffSwitchLabelsOf(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.onOffSwitchLabels)?.onOffSwitchLabels;
    public static bool disableAnimationsOf(BuildContext context) => MediaQuery._of(context, _MediaQueryAspect__media_query.disableAnimations).disableAnimations;
    public static bool? maybeDisableAnimationsOf(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.disableAnimations)?.disableAnimations;
    public static bool reduceMotionOf(BuildContext context) => MediaQuery._of(context, _MediaQueryAspect__media_query.reduceMotion).reduceMotion;
    public static bool? maybeReduceMotionOf(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.reduceMotion)?.reduceMotion;
    public static bool boldTextOf(BuildContext context) => DartRuntimePrimitives.ConvertValue<bool>((MediaQuery.maybeBoldTextOf(context) ?? false));
    public static bool? maybeBoldTextOf(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.boldText)?.boldText;
    public static bool supportsAnnounceOf(BuildContext context) => DartRuntimePrimitives.ConvertValue<bool>((MediaQuery.maybeSupportsAnnounceOf(context) ?? false));
    public static bool? maybeSupportsAnnounceOf(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.supportsAnnounce)?.supportsAnnounce;
    public static NavigationMode navigationModeOf(BuildContext context) => MediaQuery._of(context, _MediaQueryAspect__media_query.navigationMode).navigationMode;
    public static NavigationMode? maybeNavigationModeOf(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.navigationMode)?.navigationMode;
    public static global::Doroti.Generated.Framework.Gestures.DeviceGestureSettings gestureSettingsOf(BuildContext context) => MediaQuery._of(context, _MediaQueryAspect__media_query.gestureSettings).gestureSettings;
    public static global::Doroti.Generated.Framework.Gestures.DeviceGestureSettings? maybeGestureSettingsOf(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.gestureSettings)?.gestureSettings;
    public static List<global::Doroti.Flutter.Ui.DisplayFeature> displayFeaturesOf(BuildContext context) => MediaQuery._of(context, _MediaQueryAspect__media_query.displayFeatures).displayFeatures;
    public static List<global::Doroti.Flutter.Ui.DisplayFeature>? maybeDisplayFeaturesOf(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.displayFeatures)?.displayFeatures;
    public static bool supportsShowingSystemContextMenu(BuildContext context) => MediaQuery._of(context, _MediaQueryAspect__media_query.supportsShowingSystemContextMenu).supportsShowingSystemContextMenu;
    public static bool? maybeSupportsShowingSystemContextMenu(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.supportsShowingSystemContextMenu)?.supportsShowingSystemContextMenu;
    public static double? maybeLineHeightScaleFactorOverrideOf(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.lineHeightScaleFactorOverride)?.lineHeightScaleFactorOverride;
    public static double? maybeLetterSpacingOverrideOf(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.letterSpacingOverride)?.letterSpacingOverride;
    public static double? maybeWordSpacingOverrideOf(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.wordSpacingOverride)?.wordSpacingOverride;
    public static double? maybeParagraphSpacingOverrideOf(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.paragraphSpacingOverride)?.paragraphSpacingOverride;
    public static global::Doroti.Generated.Framework.Painting.BorderRadius? displayCornerRadiiOf(BuildContext context) => MediaQuery._of(context, _MediaQueryAspect__media_query.displayCornerRadii).displayCornerRadii;
    public static global::Doroti.Generated.Framework.Painting.BorderRadius? maybeDisplayCornerRadiiOf(BuildContext context) => MediaQuery._maybeOf(context, _MediaQueryAspect__media_query.displayCornerRadii)?.displayCornerRadii;
    public override bool updateShouldNotify(InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((MediaQuery)oldWidget).data)));
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<MediaQueryData>("data", this.data, showName: false));
    }

    public override bool updateShouldNotifyDependent(InheritedModel<_MediaQueryAspect__media_query> oldWidget, HashSet<_MediaQueryAspect__media_query> dependencies)
    {
        var __oldWidget = (MediaQuery)(object)oldWidget;
        var __dependencies = (HashSet<object>)(object)dependencies;
        return __dependencies.any(((dependency) => ((dependency is _MediaQueryAspect__media_query) && (((_MediaQueryAspect__media_query)dependency) switch { _MediaQueryAspect__media_query.size => (!object.Equals(((MediaQueryData)this.data).size, ((MediaQuery)__oldWidget).data.size)), _MediaQueryAspect__media_query.width => (((MediaQueryData)this.data).size.width != ((MediaQuery)__oldWidget).data.size.width), _MediaQueryAspect__media_query.height => (((MediaQueryData)this.data).size.height != ((MediaQuery)__oldWidget).data.size.height), _MediaQueryAspect__media_query.orientation => (!object.Equals(((MediaQueryData)this.data).orientation, ((MediaQuery)__oldWidget).data.orientation)), _MediaQueryAspect__media_query.devicePixelRatio => (((MediaQueryData)this.data).devicePixelRatio != ((MediaQuery)__oldWidget).data.devicePixelRatio), _MediaQueryAspect__media_query.textScaleFactor => (((MediaQueryData)this.data).textScaleFactor != ((MediaQuery)__oldWidget).data.textScaleFactor), _MediaQueryAspect__media_query.textScaler => (!object.Equals(((MediaQueryData)this.data).textScaler, ((MediaQuery)__oldWidget).data.textScaler)), _MediaQueryAspect__media_query.platformBrightness => (!object.Equals(((MediaQueryData)this.data).platformBrightness, ((MediaQuery)__oldWidget).data.platformBrightness)), _MediaQueryAspect__media_query.padding => (!object.Equals(((MediaQueryData)this.data).padding, ((MediaQuery)__oldWidget).data.padding)), _MediaQueryAspect__media_query.viewInsets => (!object.Equals(((MediaQueryData)this.data).viewInsets, ((MediaQuery)__oldWidget).data.viewInsets)), _MediaQueryAspect__media_query.viewPadding => (!object.Equals(((MediaQueryData)this.data).viewPadding, ((MediaQuery)__oldWidget).data.viewPadding)), _MediaQueryAspect__media_query.invertColors => (((MediaQueryData)this.data).invertColors != ((MediaQuery)__oldWidget).data.invertColors), _MediaQueryAspect__media_query.highContrast => (((MediaQueryData)this.data).highContrast != ((MediaQuery)__oldWidget).data.highContrast), _MediaQueryAspect__media_query.onOffSwitchLabels => (((MediaQueryData)this.data).onOffSwitchLabels != ((MediaQuery)__oldWidget).data.onOffSwitchLabels), _MediaQueryAspect__media_query.disableAnimations => (((MediaQueryData)this.data).disableAnimations != ((MediaQuery)__oldWidget).data.disableAnimations), _MediaQueryAspect__media_query.reduceMotion => (((MediaQueryData)this.data).reduceMotion != ((MediaQuery)__oldWidget).data.reduceMotion), _MediaQueryAspect__media_query.boldText => (((MediaQueryData)this.data).boldText != ((MediaQuery)__oldWidget).data.boldText), _MediaQueryAspect__media_query.supportsAnnounce => (((MediaQueryData)this.data).supportsAnnounce != ((MediaQuery)__oldWidget).data.supportsAnnounce), _MediaQueryAspect__media_query.navigationMode => (!object.Equals(((MediaQueryData)this.data).navigationMode, ((MediaQuery)__oldWidget).data.navigationMode)), _MediaQueryAspect__media_query.gestureSettings => (!object.Equals(((MediaQueryData)this.data).gestureSettings, ((MediaQuery)__oldWidget).data.gestureSettings)), _MediaQueryAspect__media_query.displayFeatures => (!object.Equals(((MediaQueryData)this.data).displayFeatures, ((MediaQuery)__oldWidget).data.displayFeatures)), _MediaQueryAspect__media_query.systemGestureInsets => (!object.Equals(((MediaQueryData)this.data).systemGestureInsets, ((MediaQuery)__oldWidget).data.systemGestureInsets)), _MediaQueryAspect__media_query.accessibleNavigation => (((MediaQueryData)this.data).accessibleNavigation != ((MediaQuery)__oldWidget).data.accessibleNavigation), _MediaQueryAspect__media_query.alwaysUse24HourFormat => (((MediaQueryData)this.data).alwaysUse24HourFormat != ((MediaQuery)__oldWidget).data.alwaysUse24HourFormat), _MediaQueryAspect__media_query.supportsShowingSystemContextMenu => (((MediaQueryData)this.data).supportsShowingSystemContextMenu != ((MediaQuery)__oldWidget).data.supportsShowingSystemContextMenu), _MediaQueryAspect__media_query.lineHeightScaleFactorOverride => (((MediaQueryData)this.data).lineHeightScaleFactorOverride != ((MediaQuery)__oldWidget).data.lineHeightScaleFactorOverride), _MediaQueryAspect__media_query.letterSpacingOverride => (((MediaQueryData)this.data).letterSpacingOverride != ((MediaQuery)__oldWidget).data.letterSpacingOverride), _MediaQueryAspect__media_query.wordSpacingOverride => (((MediaQueryData)this.data).wordSpacingOverride != ((MediaQuery)__oldWidget).data.wordSpacingOverride), _MediaQueryAspect__media_query.paragraphSpacingOverride => (((MediaQueryData)this.data).paragraphSpacingOverride != ((MediaQuery)__oldWidget).data.paragraphSpacingOverride), _MediaQueryAspect__media_query.displayCornerRadii => (!object.Equals(((MediaQueryData)this.data).displayCornerRadii, ((MediaQuery)__oldWidget).data.displayCornerRadii)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public enum NavigationMode
{
    traditional,
    directional
}

public class _MediaQueryFromView__media_query : StatefulWidget
{
    public virtual FlutterView view { get; private set; } = default!;
    public virtual bool ignoreParentData { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    internal _MediaQueryFromView__media_query(global::Doroti.Generated.Framework.Foundation.Key? key = null, FlutterView view = default!, bool ignoreParentData = false, Widget child = default!) : base(key: key)
    {
        this.view = view;
        this.ignoreParentData = ignoreParentData;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _MediaQueryFromViewState__media_query());
}

internal class _MediaQueryFromViewState__media_query : State<_MediaQueryFromView__media_query>, WidgetsBindingObserver
{
    internal virtual MediaQueryData? _parentData { get; set; } = default;
    internal virtual MediaQueryData? _data { get; set; } = default;

    public override void initState()
    {
        base.initState();
        WidgetsBinding.instance.addObserver(this);
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _updateParentData();
        _updateData();
        DartRuntimePrimitives.Assert(() => (this._data is not null));
    }

    public override void didUpdateWidget(_MediaQueryFromView__media_query oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((((_MediaQueryFromView__media_query)this.widget).ignoreParentData != ((_MediaQueryFromView__media_query)oldWidget).ignoreParentData))
        {
            _updateParentData();
        }
        if (((this._data is null) || (!object.Equals(((_MediaQueryFromView__media_query)oldWidget).view, ((_MediaQueryFromView__media_query)this.widget).view))))
        {
            _updateData();
        }
        DartRuntimePrimitives.Assert(() => (this._data is not null));
    }

    internal virtual void _updateParentData()
    {
        _parentData = (((_MediaQueryFromView__media_query)this.widget).ignoreParentData ? null : MediaQuery.maybeOf(this.context));
        _data = null;
    }

    internal virtual void _updateData()
    {
        var newData__107642 = MediaQueryData.CreateFromView(((_MediaQueryFromView__media_query)this.widget).view, platformData: this._parentData);
        if ((!object.Equals(newData__107642, this._data)))
        {
            setState(((global::System.Action)(() => {
_data = newData__107642;
})));
        }
    }

    public virtual void didChangeAccessibilityFeatures()
    {
        if ((this._parentData is null))
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
        if ((this._parentData is null))
        {
            _updateData();
        }
    }

    public virtual void didChangePlatformBrightness()
    {
        if ((this._parentData is null))
        {
            _updateData();
        }
    }

    public override void dispose()
    {
        WidgetsBinding.instance.removeObserver(this);
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        MediaQueryData effectiveData__109246 = this._data!;
        if (((!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode && (this._parentData is null)) && (!object.Equals(((MediaQueryData)effectiveData__109246).platformBrightness, global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugBrightnessOverride))))
        {
            effectiveData__109246 = effectiveData__109246.copyWith(platformBrightness: global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugBrightnessOverride);
        }
        return ((Widget)(object?)new MediaQuery(data: effectiveData__109246, child: ((_MediaQueryFromView__media_query)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Media_queryLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.TextScaler _kUnspecifiedTextScaler = ((global::Doroti.Generated.Framework.Painting.TextScaler)(object?)new _UnspecifiedTextScaler__media_query());
}

internal class _UnspecifiedTextScaler__media_query : global::Doroti.Generated.Framework.Painting.TextScaler
{
    internal _UnspecifiedTextScaler__media_query()
    {
    }

    public override global::Doroti.Generated.Framework.Painting.TextScaler clamp(double minScaleFactor = 0, double maxScaleFactor = double.PositiveInfinity) => throw new NotImplementedException();
    public override double scale(double fontSize) => throw new NotImplementedException();
    public override double textScaleFactor => throw new NotImplementedException();
}

public class SystemTextScaler : global::Doroti.Generated.Framework.Painting.TextScaler
{
    internal virtual PlatformDispatcher _platformDispatcher { get; private set; } = default!;
    private double __field_textScaleFactor = default!;
    public override double textScaleFactor { get => __field_textScaleFactor; }

    public SystemTextScaler(PlatformDispatcher _platformDispatcher)
    {
        this._platformDispatcher = _platformDispatcher;
        this.__field_textScaleFactor = _platformDispatcher.textScaleFactor;
    }

    public override double scale(double fontSize) => this._platformDispatcher.scaleFontSize(fontSize);
    public override bool Equals(object? other)
    {
        var __other = other as SystemTextScaler;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        return (__other switch { SystemTextScaler { textScaleFactor: double textScaleFactor__111756 } __object111725 => (this.textScaleFactor == textScaleFactor__111756), var __constant111920 when (object.Equals(__constant111920, global::Doroti.Generated.Framework.Painting.TextScaler.noScaling)) => (this.textScaleFactor == 1.0), _ => false });
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(this.textScaleFactor.GetHashCode());
    public override string ToString() => $"SystemTextScaler ({((this.textScaleFactor == 1.0) ? "no scaling" : $"{this.textScaleFactor}x")})";
}
