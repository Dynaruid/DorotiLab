// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/scrollbar.dart
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

public static partial class ScrollbarLibrary
{
    internal static double _kScrollbarThickness = 8.0;
}

public static partial class ScrollbarLibrary
{
    internal static double _kScrollbarThicknessWithTrack = 12.0;
}

public static partial class ScrollbarLibrary
{
    internal static double _kScrollbarMargin = 2.0;
}

public static partial class ScrollbarLibrary
{
    internal static double _kScrollbarMinLength = 48.0;
}

public static partial class ScrollbarLibrary
{
    internal static Radius _kScrollbarRadius = global::Doroti.Ui.Radius.circular(8.0);
}

public static partial class ScrollbarLibrary
{
    internal static Duration _kScrollbarFadeDuration = Duration.Create(milliseconds: 300L);
}

public static partial class ScrollbarLibrary
{
    internal static Duration _kScrollbarTimeToFade = Duration.Create(milliseconds: 600L);
}

public class Scrollbar : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.ScrollController? controller { get; private set; }
    public virtual bool? thumbVisibility { get; private set; }
    public virtual bool? trackVisibility { get; private set; }
    public virtual double? thickness { get; private set; }
    public virtual Radius? radius { get; private set; }
    public virtual bool? interactive { get; private set; }
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.ScrollNotification, bool>? notificationPredicate { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.ScrollbarOrientation? scrollbarOrientation { get; private set; }

    public Scrollbar(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget child = default!, global::Doroti.Generated.Framework.Widgets.ScrollController? controller = null, bool? thumbVisibility = null, bool? trackVisibility = null, double? thickness = null, Radius? radius = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.ScrollNotification, bool>? notificationPredicate = null, bool? interactive = null, global::Doroti.Generated.Framework.Widgets.ScrollbarOrientation? scrollbarOrientation = null) : base(key: key)
    {
        this.child = child;
        this.controller = controller;
        this.thumbVisibility = thumbVisibility;
        this.trackVisibility = trackVisibility;
        this.thickness = thickness;
        this.radius = radius;
        this.notificationPredicate = notificationPredicate;
        this.interactive = interactive;
        this.scrollbarOrientation = scrollbarOrientation;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        if ((object.Equals(Theme.of(context).platform, global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS)))
        {
            return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new CupertinoScrollbar(thumbVisibility: (this.thumbVisibility ?? false), thickness: (this.thickness ?? CupertinoScrollbar.defaultThickness), thicknessWhileDragging: (this.thickness ?? CupertinoScrollbar.defaultThicknessWhileDragging), radius: (this.radius ?? CupertinoScrollbar.defaultRadius), radiusWhileDragging: (this.radius ?? CupertinoScrollbar.defaultRadiusWhileDragging), controller: this.controller, notificationPredicate: this.notificationPredicate, scrollbarOrientation: this.scrollbarOrientation, child: this.child));
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _MaterialScrollbar__scrollbar(controller: this.controller, thumbVisibility: this.thumbVisibility, trackVisibility: this.trackVisibility, thickness: this.thickness, radius: this.radius, notificationPredicate: (global::System.Func<global::Doroti.Generated.Framework.Widgets.ScrollNotification, bool>?)this.notificationPredicate, interactive: this.interactive, scrollbarOrientation: this.scrollbarOrientation, child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MaterialScrollbar__scrollbar : global::Doroti.Generated.Framework.Widgets.RawScrollbar
{
    internal _MaterialScrollbar__scrollbar(global::Doroti.Generated.Framework.Widgets.Widget child, global::Doroti.Generated.Framework.Widgets.ScrollController? controller = null, bool? thumbVisibility = null, bool? trackVisibility = null, double? thickness = null, Radius? radius = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.ScrollNotification, bool>? notificationPredicate = null, bool? interactive = null, global::Doroti.Generated.Framework.Widgets.ScrollbarOrientation? scrollbarOrientation = null) : base(child: child, controller: controller, thumbVisibility: thumbVisibility, trackVisibility: trackVisibility, thickness: thickness, radius: radius, interactive: interactive, scrollbarOrientation: scrollbarOrientation, fadeDuration: ScrollbarLibrary._kScrollbarFadeDuration, timeToFade: ScrollbarLibrary._kScrollbarTimeToFade, pressDuration: Duration.zero, notificationPredicate: ((notificationPredicate ?? (global::System.Func<global::Doroti.Generated.Framework.Widgets.ScrollNotification, bool>)global::Doroti.Generated.Framework.Widgets.Scroll_notificationLibrary.defaultScrollNotificationPredicate)))
    {
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _MaterialScrollbarState__scrollbar());
}

internal class _MaterialScrollbarState__scrollbar : global::Doroti.Generated.Framework.Widgets.RawScrollbarState<_MaterialScrollbar__scrollbar>
{
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationController _hoverAnimationController { get; set; } = default!;
    internal virtual bool _dragIsActive { get; set; } = false;
    internal virtual bool _hoverIsActive { get; set; } = false;
    internal virtual ColorScheme _colorScheme { get; set; } = default!;
    internal virtual ScrollbarThemeData _scrollbarTheme { get; set; } = default!;
    internal virtual bool _useAndroidScrollbar { get; set; } = default!;

    public override bool showScrollbar => this.widget.thumbVisibility ?? this._scrollbarTheme.thumbVisibility?.resolve(this._states) ?? false;
    public override bool enableGestures => DartRuntimePrimitives.ConvertValue<bool>(((this.widget.interactive ?? this._scrollbarTheme.interactive) ?? !this._useAndroidScrollbar));
    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<bool> _trackVisibility => WidgetStateProperty.resolveWith(((global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, bool>)((states) => {
return this.widget.trackVisibility ?? this._scrollbarTheme.trackVisibility?.resolve(states) ?? false;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
    internal virtual HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> _states => ((Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>>)(() => { var __collection8725 = new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>(); if (this._dragIsActive) { __collection8725.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.dragged); } if (this._hoverIsActive) { __collection8725.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered); } return __collection8725; }))();
    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> _thumbColor
    {
        get
        {
            global::Doroti.Ui.Color onSurface__8898 = ((global::Doroti.Ui.Color)(object?)this._colorScheme.onSurface);
            global::Doroti.Ui.Brightness brightness__8955 = this._colorScheme.brightness;
            global::Doroti.Ui.Color dragColor__9008 = default!;
            global::Doroti.Ui.Color hoverColor__9034 = default!;
            global::Doroti.Ui.Color idleColor__9061 = default!;
            switch (brightness__8955)
            {
                case Brightness.light:
                    {
                        dragColor__9008 = onSurface__8898.withOpacity(0.6);
                        hoverColor__9034 = onSurface__8898.withOpacity(0.5);
                        idleColor__9061 = (this._useAndroidScrollbar ? Theme.of(this.context).highlightColor.withOpacity(1.0) : onSurface__8898.withOpacity(0.1));
                        break;
                    }
                case Brightness.dark:
                    {
                        dragColor__9008 = onSurface__8898.withOpacity(0.75);
                        hoverColor__9034 = onSurface__8898.withOpacity(0.65);
                        idleColor__9061 = (this._useAndroidScrollbar ? Theme.of(this.context).highlightColor.withOpacity(1.0) : onSurface__8898.withOpacity(0.3));
                        break;
                    }
            }
            return WidgetStateProperty.resolveWith<Color>(((global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, Color>)((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.dragged))
{
    return this._scrollbarTheme.thumbColor?.resolve(states) ?? dragColor__9008;
}
if (this._trackVisibility.resolve(states))
{
    return this._scrollbarTheme.thumbColor?.resolve(states) ?? hoverColor__9034;
}
return Dart_uiLibrary.Color.lerp((this._scrollbarTheme.thumbColor?.resolve(states) ?? idleColor__9061), (this._scrollbarTheme.thumbColor?.resolve(states) ?? hoverColor__9034), ((global::Doroti.Generated.Framework.Animation.AnimationController)this._hoverAnimationController).value)!;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
            return default!;
        }
    }
    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> _trackColor
    {
        get
        {
            global::Doroti.Ui.Color onSurface__10380 = ((global::Doroti.Ui.Color)(object?)this._colorScheme.onSurface);
            global::Doroti.Ui.Brightness brightness__10437 = this._colorScheme.brightness;
            return WidgetStateProperty.resolveWith<Color>(((global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, Color>)((states) => {
if ((this.showScrollbar && this._trackVisibility.resolve(states)))
{
    return this._scrollbarTheme.trackColor?.resolve(states) ?? (brightness__10437 switch { Brightness.light => onSurface__10380.withOpacity(0.03), Brightness.dark => onSurface__10380.withOpacity(0.05), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
}
return new global::Doroti.Ui.Color(0L);
throw new InvalidOperationException("Dart closure completed without a value.");
})));
            return default!;
        }
    }
    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> _trackBorderColor
    {
        get
        {
            global::Doroti.Ui.Color onSurface__10973 = ((global::Doroti.Ui.Color)(object?)this._colorScheme.onSurface);
            global::Doroti.Ui.Brightness brightness__11030 = this._colorScheme.brightness;
            return WidgetStateProperty.resolveWith<Color>(((global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, Color>)((states) => {
if ((this.showScrollbar && this._trackVisibility.resolve(states)))
{
    return this._scrollbarTheme.trackBorderColor?.resolve(states) ?? (brightness__11030 switch { Brightness.light => onSurface__10973.withOpacity(0.1), Brightness.dark => onSurface__10973.withOpacity(0.25), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
}
return new global::Doroti.Ui.Color(0L);
throw new InvalidOperationException("Dart closure completed without a value.");
})));
            return default!;
        }
    }
    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double> _thickness
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>)(object?)WidgetStateProperty.resolveWith(((global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, double>)((states) => {
if ((states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered) && this._trackVisibility.resolve(states)))
{
    return this.widget.thickness ?? this._scrollbarTheme.thickness?.resolve(states) ?? ScrollbarLibrary._kScrollbarThicknessWithTrack;
}
return this.widget.thickness ?? this._scrollbarTheme.thickness?.resolve(states) ?? ScrollbarLibrary._kScrollbarThickness;
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
            return default!;
        }
    }
    public override void initState()
    {
        base.initState();
        _hoverAnimationController = new global::Doroti.Generated.Framework.Animation.AnimationController(vsync: this, duration: Duration.Create(milliseconds: 200L));
        this._hoverAnimationController.addListener(((global::System.Action)(() => {
updateScrollbarPainter();
})));
    }

    public override void didChangeDependencies()
    {
        ThemeData theme__12425 = Theme.of(this.context);
        _colorScheme = theme__12425.colorScheme;
        _scrollbarTheme = ScrollbarTheme.of(this.context);
        switch (theme__12425.platform)
        {
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
                {
                    _useAndroidScrollbar = true;
                    break;
                }
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
                {
                    _useAndroidScrollbar = false;
                    break;
                }
        }
        base.didChangeDependencies();
    }

    public override void updateScrollbarPainter()
    {
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Widgets.ScrollbarPainter>)(() =>
{            var __cascade = this.scrollbarPainter;
            __cascade.color = this._thumbColor.resolve(this._states);
            __cascade.trackColor = this._trackColor.resolve(this._states);
            __cascade.trackBorderColor = this._trackBorderColor.resolve(this._states);
            __cascade.textDirection = Directionality.of(this.context);
            __cascade.thickness = this._thickness.resolve(this._states);
            __cascade.radius = ((this.widget.radius ?? this._scrollbarTheme.radius) ?? ((this._useAndroidScrollbar ? null : ScrollbarLibrary._kScrollbarRadius)));
            __cascade.crossAxisMargin = (this._scrollbarTheme.crossAxisMargin ?? ((this._useAndroidScrollbar ? 0.0 : ScrollbarLibrary._kScrollbarMargin)));
            __cascade.mainAxisMargin = (this._scrollbarTheme.mainAxisMargin ?? 0.0);
            __cascade.minLength = (this._scrollbarTheme.minThumbLength ?? ScrollbarLibrary._kScrollbarMinLength);
            __cascade.padding = MediaQuery.paddingOf(this.context);
            __cascade.scrollbarOrientation = this.widget.scrollbarOrientation;
            __cascade.ignorePointer = !this.enableGestures;
            return __cascade;        }))());
    }

    public override void handleThumbPressStart(Offset localPosition)
    {
        base.handleThumbPressStart(localPosition);
        setState(((global::System.Action)(() => {
_dragIsActive = true;
})));
    }

    public override void handleThumbPressEnd(Offset localPosition, global::Doroti.Generated.Framework.Gestures.Velocity velocity)
    {
        base.handleThumbPressEnd(localPosition, velocity);
        setState(((global::System.Action)(() => {
_dragIsActive = false;
})));
    }

    public override void handleHover(global::Doroti.Generated.Framework.Gestures.PointerHoverEvent @event)
    {
        base.handleHover(@event);
        if (isPointerOverScrollbar(@event.position, @event.kind, forHover: true))
        {
            setState(((global::System.Action)(() => {
_hoverIsActive = true;
})));
            this._hoverAnimationController.forward();
        }
        else
        {
            if (this._hoverIsActive)
            {
                setState(((global::System.Action)(() => {
_hoverIsActive = false;
})));
                this._hoverAnimationController.reverse();
            }
        }
    }

    public override void handleHoverExit(global::Doroti.Generated.Framework.Gestures.PointerExitEvent @event)
    {
        base.handleHoverExit(@event);
        setState(((global::System.Action)(() => {
_hoverIsActive = false;
})));
        this._hoverAnimationController.reverse();
    }

    public override void dispose()
    {
        this._hoverAnimationController.dispose();
        base.dispose();
    }

}
