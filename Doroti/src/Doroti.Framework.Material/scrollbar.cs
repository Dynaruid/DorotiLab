// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/scrollbar.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

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
    internal static Radius _kScrollbarRadius = Radius.circular(8.0);
}

public static partial class ScrollbarLibrary
{
    internal static Duration _kScrollbarFadeDuration = Duration.Create(milliseconds: 300L);
}

public static partial class ScrollbarLibrary
{
    internal static Duration _kScrollbarTimeToFade = Duration.Create(milliseconds: 600L);
}

public class Scrollbar : StatelessWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual ScrollController? controller { get; private set; }
    public virtual bool? thumbVisibility { get; private set; }
    public virtual bool? trackVisibility { get; private set; }
    public virtual double? thickness { get; private set; }
    public virtual Radius? radius { get; private set; }
    public virtual bool? interactive { get; private set; }
    public virtual Func<ScrollNotification, bool>? notificationPredicate { get; private set; }
    public virtual ScrollbarOrientation? scrollbarOrientation { get; private set; }

    public Scrollbar(
        Key? key = null,
        Widget child = default!,
        ScrollController? controller = null,
        bool? thumbVisibility = null,
        bool? trackVisibility = null,
        double? thickness = null,
        Radius? radius = null,
        Func<ScrollNotification, bool>? notificationPredicate = null,
        bool? interactive = null,
        ScrollbarOrientation? scrollbarOrientation = null
    )
        : base(key: key)
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

    public override Widget build(BuildContext context)
    {
        if (Equals(Theme.of(context).platform, TargetPlatform.iOS))
        {
            return new CupertinoScrollbar(
                thumbVisibility: thumbVisibility ?? false,
                thickness: thickness ?? CupertinoScrollbar.defaultThickness,
                thicknessWhileDragging: thickness
                    ?? CupertinoScrollbar.defaultThicknessWhileDragging,
                radius: radius ?? CupertinoScrollbar.defaultRadius,
                radiusWhileDragging: radius ?? CupertinoScrollbar.defaultRadiusWhileDragging,
                controller: controller,
                notificationPredicate: notificationPredicate,
                scrollbarOrientation: scrollbarOrientation,
                child: child
            );
        }
        return new _MaterialScrollbar__scrollbar(
            controller: controller,
            thumbVisibility: thumbVisibility,
            trackVisibility: trackVisibility,
            thickness: thickness,
            radius: radius,
            notificationPredicate: notificationPredicate,
            interactive: interactive,
            scrollbarOrientation: scrollbarOrientation,
            child: child
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _MaterialScrollbar__scrollbar : RawScrollbar
{
    internal _MaterialScrollbar__scrollbar(
        Widget child,
        ScrollController? controller = null,
        bool? thumbVisibility = null,
        bool? trackVisibility = null,
        double? thickness = null,
        Radius? radius = null,
        Func<ScrollNotification, bool>? notificationPredicate = null,
        bool? interactive = null,
        ScrollbarOrientation? scrollbarOrientation = null
    )
        : base(
            child: child,
            controller: controller,
            thumbVisibility: thumbVisibility,
            trackVisibility: trackVisibility,
            thickness: thickness,
            radius: radius,
            interactive: interactive,
            scrollbarOrientation: scrollbarOrientation,
            fadeDuration: ScrollbarLibrary._kScrollbarFadeDuration,
            timeToFade: ScrollbarLibrary._kScrollbarTimeToFade,
            pressDuration: Duration.zero,
            notificationPredicate: notificationPredicate
                ?? Scroll_notificationLibrary.defaultScrollNotificationPredicate
        ) { }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _MaterialScrollbarState__scrollbar());
}

internal class _MaterialScrollbarState__scrollbar : RawScrollbarState<_MaterialScrollbar__scrollbar>
{
    internal virtual AnimationController _hoverAnimationController { get; set; } = default!;
    internal virtual bool _dragIsActive { get; set; } = false;

    // Dart library-private member: distinct from the same name in the base library.
    internal new virtual bool _hoverIsActive { get; set; } = false;
    internal virtual ColorScheme _colorScheme { get; set; } = default!;
    internal virtual ScrollbarThemeData _scrollbarTheme { get; set; } = default!;
    internal virtual bool _useAndroidScrollbar { get; set; } = default!;

    public override bool showScrollbar =>
        widget.thumbVisibility ?? _scrollbarTheme.thumbVisibility?.resolve(_states) ?? false;
    public override bool enableGestures =>
        DartRuntimePrimitives.ConvertValue<bool>(
            (widget.interactive ?? _scrollbarTheme.interactive) ?? !_useAndroidScrollbar
        );
    internal virtual WidgetStateProperty<bool> _trackVisibility =>
        WidgetStateProperty.resolveWith(
            (states) =>
            {
                return widget.trackVisibility
                    ?? _scrollbarTheme.trackVisibility?.resolve(states)
                    ?? false;
                throw new InvalidOperationException("Dart closure completed without a value.");
            }
        );
    internal virtual HashSet<WidgetState> _states =>
        (
            (Func<HashSet<WidgetState>>)(
                () =>
                {
                    var __collection8725 = new HashSet<WidgetState>();
                    if (_dragIsActive)
                    {
                        __collection8725.Add(WidgetState.dragged);
                    }
                    if (_hoverIsActive)
                    {
                        __collection8725.Add(WidgetState.hovered);
                    }
                    return __collection8725;
                }
            )
        )();
    internal virtual WidgetStateProperty<Color> _thumbColor
    {
        get
        {
            Color onSurfaceLocal = _colorScheme.onSurface;
            Brightness brightnessLocal = _colorScheme.brightness;
            Color dragColor = default!;
            Color hoverColor = default!;
            Color idleColor = default!;
            switch (brightnessLocal)
            {
                case Brightness.light:
                {
                    dragColor = onSurfaceLocal.withOpacity(0.6);
                    hoverColor = onSurfaceLocal.withOpacity(0.5);
                    idleColor = _useAndroidScrollbar
                        ? Theme.of(context).highlightColor.withOpacity(1.0)
                        : onSurfaceLocal.withOpacity(0.1);
                    break;
                }
                case Brightness.dark:
                {
                    dragColor = onSurfaceLocal.withOpacity(0.75);
                    hoverColor = onSurfaceLocal.withOpacity(0.65);
                    idleColor = _useAndroidScrollbar
                        ? Theme.of(context).highlightColor.withOpacity(1.0)
                        : onSurfaceLocal.withOpacity(0.3);
                    break;
                }
            }
            return WidgetStateProperty.resolveWith(
                (states) =>
                {
                    if (states.Contains(WidgetState.dragged))
                    {
                        return _scrollbarTheme.thumbColor?.resolve(states) ?? dragColor;
                    }
                    if (_trackVisibility.resolve(states))
                    {
                        return _scrollbarTheme.thumbColor?.resolve(states) ?? hoverColor;
                    }
                    return Dart_uiLibrary.Color.lerp(
                        _scrollbarTheme.thumbColor?.resolve(states) ?? idleColor,
                        _scrollbarTheme.thumbColor?.resolve(states) ?? hoverColor,
                        _hoverAnimationController.value
                    )!;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            );
        }
    }
    internal virtual WidgetStateProperty<Color> _trackColor
    {
        get
        {
            Color onSurfaceLocal = _colorScheme.onSurface;
            Brightness brightnessLocal = _colorScheme.brightness;
            return WidgetStateProperty.resolveWith(
                (states) =>
                {
                    if (showScrollbar && _trackVisibility.resolve(states))
                    {
                        return _scrollbarTheme.trackColor?.resolve(states)
                            ?? (
                                brightnessLocal switch
                                {
                                    Brightness.light => onSurfaceLocal.withOpacity(0.03),
                                    Brightness.dark => onSurfaceLocal.withOpacity(0.05),
                                    _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                                        throw new InvalidOperationException(
                                            "Non-exhaustive Dart switch value."
                                        ),
                                }
                            );
                    }
                    return new Color(0L);
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            );
        }
    }
    internal virtual WidgetStateProperty<Color> _trackBorderColor
    {
        get
        {
            Color onSurfaceLocal = _colorScheme.onSurface;
            Brightness brightnessLocal = _colorScheme.brightness;
            return WidgetStateProperty.resolveWith(
                (states) =>
                {
                    if (showScrollbar && _trackVisibility.resolve(states))
                    {
                        return _scrollbarTheme.trackBorderColor?.resolve(states)
                            ?? (
                                brightnessLocal switch
                                {
                                    Brightness.light => onSurfaceLocal.withOpacity(0.1),
                                    Brightness.dark => onSurfaceLocal.withOpacity(0.25),
                                    _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                                        throw new InvalidOperationException(
                                            "Non-exhaustive Dart switch value."
                                        ),
                                }
                            );
                    }
                    return new Color(0L);
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            );
        }
    }
    internal virtual WidgetStateProperty<double> _thickness
    {
        get
        {
            return WidgetStateProperty.resolveWith(
                (states) =>
                {
                    if (states.Contains(WidgetState.hovered) && _trackVisibility.resolve(states))
                    {
                        return widget.thickness
                            ?? _scrollbarTheme.thickness?.resolve(states)
                            ?? ScrollbarLibrary._kScrollbarThicknessWithTrack;
                    }
                    return widget.thickness
                        ?? _scrollbarTheme.thickness?.resolve(states)
                        ?? ScrollbarLibrary._kScrollbarThickness;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            );
        }
    }

    public override void initState()
    {
        base.initState();
        _hoverAnimationController = new AnimationController(
            vsync: this,
            duration: Duration.Create(milliseconds: 200L)
        );
        _hoverAnimationController.addListener(() =>
        {
            updateScrollbarPainter();
        });
    }

    public override void didChangeDependencies()
    {
        ThemeData theme = Theme.of(context);
        _colorScheme = theme.colorScheme;
        _scrollbarTheme = ScrollbarTheme.of(context);
        switch (theme.platform)
        {
            case TargetPlatform.android:
            {
                _useAndroidScrollbar = true;
                break;
            }
            case TargetPlatform.iOS:
            case TargetPlatform.linux:
            case TargetPlatform.fuchsia:
            case TargetPlatform.macOS:
            case TargetPlatform.windows:
            {
                _useAndroidScrollbar = false;
                break;
            }
        }
        base.didChangeDependencies();
    }

    public override void updateScrollbarPainter()
    {
        DartRuntimePrimitives.Ignore(
            (
                (Func<ScrollbarPainter>)(
                    () =>
                    {
                        var __cascade = scrollbarPainter;
                        __cascade.color = _thumbColor.resolve(_states);
                        __cascade.trackColor = _trackColor.resolve(_states);
                        __cascade.trackBorderColor = _trackBorderColor.resolve(_states);
                        __cascade.textDirection = Directionality.of(context);
                        __cascade.thickness = _thickness.resolve(_states);
                        __cascade.radius =
                            (widget.radius ?? _scrollbarTheme.radius)
                            ?? (_useAndroidScrollbar ? null : ScrollbarLibrary._kScrollbarRadius);
                        __cascade.crossAxisMargin =
                            _scrollbarTheme.crossAxisMargin
                            ?? (_useAndroidScrollbar ? 0.0 : ScrollbarLibrary._kScrollbarMargin);
                        __cascade.mainAxisMargin = _scrollbarTheme.mainAxisMargin ?? 0.0;
                        __cascade.minLength =
                            _scrollbarTheme.minThumbLength ?? ScrollbarLibrary._kScrollbarMinLength;
                        __cascade.padding = MediaQuery.paddingOf(context);
                        __cascade.scrollbarOrientation = widget.scrollbarOrientation;
                        __cascade.ignorePointer = !enableGestures;
                        return __cascade;
                    }
                )
            )()
        );
    }

    public override void handleThumbPressStart(Offset localPosition)
    {
        base.handleThumbPressStart(localPosition);
        setState(() =>
        {
            _dragIsActive = true;
        });
    }

    public override void handleThumbPressEnd(Offset localPosition, Gestures.Velocity velocity)
    {
        base.handleThumbPressEnd(localPosition, velocity);
        setState(() =>
        {
            _dragIsActive = false;
        });
    }

    public override void handleHover(Gestures.PointerHoverEvent @event)
    {
        base.handleHover(@event);
        if (isPointerOverScrollbar(@event.position, @event.kind, forHover: true))
        {
            setState(() =>
            {
                _hoverIsActive = true;
            });
            _hoverAnimationController.forward();
        }
        else
        {
            if (_hoverIsActive)
            {
                setState(() =>
                {
                    _hoverIsActive = false;
                });
                _hoverAnimationController.reverse();
            }
        }
    }

    public override void handleHoverExit(Gestures.PointerExitEvent @event)
    {
        base.handleHoverExit(@event);
        setState(() =>
        {
            _hoverIsActive = false;
        });
        _hoverAnimationController.reverse();
    }

    public override void dispose()
    {
        _hoverAnimationController.dispose();
        base.dispose();
    }
}
