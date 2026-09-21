// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scroll_configuration.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public static partial class Scroll_configurationLibrary
{
    internal static Color _kDefaultGlowColor = new Color(4294967295L);
}

public static partial class Scroll_configurationLibrary
{
    internal static HashSet<PointerDeviceKind> _kTouchLikeDeviceTypes =
        new HashSet<PointerDeviceKind>
        {
            PointerDeviceKind.touch,
            PointerDeviceKind.stylus,
            PointerDeviceKind.invertedStylus,
            PointerDeviceKind.trackpad,
            PointerDeviceKind.unknown,
        };
}

public enum AndroidOverscrollIndicator
{
    stretch,
    glow,
}

public class ScrollBehavior
{
    internal static ScrollPhysics _bouncingPhysics = new BouncingScrollPhysics(
        parent: new RangeMaintainingScrollPhysics()
    );
    internal static ScrollPhysics _bouncingDesktopPhysics = new BouncingScrollPhysics(
        decelerationRate: ScrollDecelerationRate.fast,
        parent: new RangeMaintainingScrollPhysics()
    );
    internal static ScrollPhysics _clampingPhysics = new ClampingScrollPhysics(
        parent: new RangeMaintainingScrollPhysics()
    );

    public ScrollBehavior() { }

    public virtual ScrollBehavior copyWith(
        bool? scrollbars = null,
        bool? overscroll = null,
        HashSet<PointerDeviceKind>? dragDevices = null,
        MultitouchDragStrategy? multitouchDragStrategy = null,
        HashSet<LogicalKeyboardKey>? pointerAxisModifiers = null,
        ScrollPhysics? physics = null,
        TargetPlatform? platform = null,
        ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null
    )
    {
        return new _WrappedScrollBehavior__scroll_configuration(
            @delegate: this,
            scrollbars: scrollbars ?? true,
            overscroll: overscroll ?? true,
            dragDevices: dragDevices,
            multitouchDragStrategy: multitouchDragStrategy,
            pointerAxisModifiers: pointerAxisModifiers,
            physics: physics,
            platform: platform,
            keyboardDismissBehavior: keyboardDismissBehavior
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual TargetPlatform getPlatform(BuildContext context) =>
        PlatformLibrary.defaultTargetPlatform;

    public virtual HashSet<PointerDeviceKind> dragDevices =>
        DartRuntimePrimitives.ConvertValue<HashSet<PointerDeviceKind>>(
            Scroll_configurationLibrary._kTouchLikeDeviceTypes
        );

    public virtual MultitouchDragStrategy getMultitouchDragStrategy(BuildContext context)
    {
        switch (getPlatform(context))
        {
            case TargetPlatform.macOS:
            case TargetPlatform.iOS:
            {
                return MultitouchDragStrategy.averageBoundaryPointers;
            }
            case TargetPlatform.linux:
            case TargetPlatform.windows:
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            {
                return MultitouchDragStrategy.latestPointer;
            }
            default:
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                );
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual HashSet<LogicalKeyboardKey> pointerAxisModifiers =>
        new HashSet<LogicalKeyboardKey>
        {
            LogicalKeyboardKey.shiftLeft,
            LogicalKeyboardKey.shiftRight,
        };

    public virtual Widget buildScrollbar(
        BuildContext context,
        Widget child,
        ScrollableDetails details
    )
    {
        switch (getPlatform(context))
        {
            case TargetPlatform.linux:
            case TargetPlatform.macOS:
            case TargetPlatform.windows:
            {
                DartRuntimePrimitives.Assert(() => details.controller is not null);
                return new RawScrollbar(controller: details.controller, child: child);
            }
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.iOS:
            {
                return child;
            }
            default:
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                );
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Widget buildOverscrollIndicator(
        BuildContext context,
        Widget child,
        ScrollableDetails details
    )
    {
        switch (getPlatform(context))
        {
            case TargetPlatform.iOS:
            case TargetPlatform.linux:
            case TargetPlatform.macOS:
            case TargetPlatform.windows:
            {
                return child;
            }
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            {
                return new GlowingOverscrollIndicator(
                    axisDirection: details.direction,
                    color: Scroll_configurationLibrary._kDefaultGlowColor,
                    child: child
                );
            }
            default:
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                );
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Func<PointerEvent, VelocityTracker> velocityTrackerBuilder(BuildContext context)
    {
        switch (getPlatform(context))
        {
            case TargetPlatform.iOS:
            {
                return (@event) => new IOSScrollViewFlingVelocityTracker(@event.kind);
            }
            case TargetPlatform.macOS:
            {
                return (@event) => new MacOSScrollViewFlingVelocityTracker(@event.kind);
            }
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
            {
                return (@event) => new VelocityTracker(@event.kind);
            }
            default:
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                );
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual ScrollPhysics getScrollPhysics(BuildContext context)
    {
        switch (getPlatform(context))
        {
            case TargetPlatform.iOS:
            {
                return _bouncingPhysics;
            }
            case TargetPlatform.macOS:
            {
                return _bouncingDesktopPhysics;
            }
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
            {
                return _clampingPhysics;
            }
            default:
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                );
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool shouldNotify(ScrollBehavior oldDelegate) => false;

    public virtual ScrollViewKeyboardDismissBehavior getKeyboardDismissBehavior(
        BuildContext context
    ) => ScrollViewKeyboardDismissBehavior.manual;

    public override string ToString() =>
        objectRuntimeTypeFunctions.objectRuntimeType(this, "ScrollBehavior");
}

internal class _WrappedScrollBehavior__scroll_configuration : ScrollBehavior
{
    public virtual ScrollBehavior @delegate { get; private set; } = default!;
    public virtual bool scrollbars { get; private set; } = default!;
    public virtual bool overscroll { get; private set; } = default!;
    public virtual ScrollPhysics? physics { get; private set; }
    public virtual TargetPlatform? platform { get; private set; }
    public virtual ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior { get; private set; }
    internal virtual HashSet<PointerDeviceKind>? _dragDevices { get; private set; }
    public virtual MultitouchDragStrategy? multitouchDragStrategy { get; private set; }
    internal virtual HashSet<LogicalKeyboardKey>? _pointerAxisModifiers { get; private set; }

    internal _WrappedScrollBehavior__scroll_configuration(
        ScrollBehavior @delegate,
        bool scrollbars = true,
        bool overscroll = true,
        HashSet<PointerDeviceKind>? dragDevices = null,
        MultitouchDragStrategy? multitouchDragStrategy = null,
        HashSet<LogicalKeyboardKey>? pointerAxisModifiers = null,
        ScrollPhysics? physics = null,
        TargetPlatform? platform = null,
        ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null
    )
    {
        this.@delegate = @delegate;
        this.scrollbars = scrollbars;
        this.overscroll = overscroll;
        this.multitouchDragStrategy = multitouchDragStrategy;
        this.physics = physics;
        this.platform = platform;
        this.keyboardDismissBehavior = keyboardDismissBehavior;
        _dragDevices = dragDevices;
        _pointerAxisModifiers = pointerAxisModifiers;
    }

    public override HashSet<PointerDeviceKind> dragDevices =>
        DartRuntimePrimitives.ConvertValue<HashSet<PointerDeviceKind>>(
            _dragDevices ?? @delegate.dragDevices
        );
    public override HashSet<LogicalKeyboardKey> pointerAxisModifiers =>
        DartRuntimePrimitives.ConvertValue<HashSet<LogicalKeyboardKey>>(
            _pointerAxisModifiers ?? @delegate.pointerAxisModifiers
        );

    public override MultitouchDragStrategy getMultitouchDragStrategy(BuildContext context)
    {
        return multitouchDragStrategy ?? @delegate.getMultitouchDragStrategy(context);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget buildOverscrollIndicator(
        BuildContext context,
        Widget child,
        ScrollableDetails details
    )
    {
        if (overscroll)
        {
            return @delegate.buildOverscrollIndicator(context, child, details);
        }
        return child;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget buildScrollbar(
        BuildContext context,
        Widget child,
        ScrollableDetails details
    )
    {
        if (scrollbars)
        {
            return @delegate.buildScrollbar(context, child, details);
        }
        return child;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override ScrollBehavior copyWith(
        bool? scrollbars = null,
        bool? overscroll = null,
        HashSet<PointerDeviceKind>? dragDevices = null,
        MultitouchDragStrategy? multitouchDragStrategy = null,
        HashSet<LogicalKeyboardKey>? pointerAxisModifiers = null,
        ScrollPhysics? physics = null,
        TargetPlatform? platform = null,
        ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null
    )
    {
        return @delegate.copyWith(
            scrollbars: scrollbars ?? this.scrollbars,
            overscroll: overscroll ?? this.overscroll,
            dragDevices: dragDevices ?? this.dragDevices,
            multitouchDragStrategy: multitouchDragStrategy ?? this.multitouchDragStrategy,
            pointerAxisModifiers: pointerAxisModifiers ?? this.pointerAxisModifiers,
            physics: physics ?? this.physics,
            platform: platform ?? this.platform,
            keyboardDismissBehavior: keyboardDismissBehavior ?? this.keyboardDismissBehavior
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override TargetPlatform getPlatform(BuildContext context)
    {
        return platform ?? @delegate.getPlatform(context);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override ScrollPhysics getScrollPhysics(BuildContext context)
    {
        return physics ?? @delegate.getScrollPhysics(context);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override ScrollViewKeyboardDismissBehavior getKeyboardDismissBehavior(
        BuildContext context
    )
    {
        return keyboardDismissBehavior ?? @delegate.getKeyboardDismissBehavior(context);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool shouldNotify(ScrollBehavior oldDelegate)
    {
        var __oldDelegate = (_WrappedScrollBehavior__scroll_configuration)oldDelegate;
        return (
                !Equals(
                    DartRuntimePrimitives.RuntimeType(__oldDelegate.@delegate),
                    DartRuntimePrimitives.RuntimeType(@delegate)
                )
            )
            || (__oldDelegate.scrollbars != scrollbars)
            || (__oldDelegate.overscroll != overscroll)
            || !CollectionsLibrary.setEquals(__oldDelegate.dragDevices, dragDevices)
            || (!Equals(__oldDelegate.multitouchDragStrategy, multitouchDragStrategy))
            || !CollectionsLibrary.setEquals(
                __oldDelegate.pointerAxisModifiers,
                pointerAxisModifiers
            )
            || (!Equals(__oldDelegate.physics, physics))
            || (!Equals(__oldDelegate.platform, platform))
            || @delegate.shouldNotify(__oldDelegate.@delegate);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Func<PointerEvent, VelocityTracker> velocityTrackerBuilder(BuildContext context)
    {
        return @delegate.velocityTrackerBuilder(context);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override string ToString() =>
        objectRuntimeTypeFunctions.objectRuntimeType(this, "_WrappedScrollBehavior");
}

public class ScrollConfiguration : InheritedWidget
{
    public virtual ScrollBehavior behavior { get; private set; } = default!;

    public ScrollConfiguration(
        Key? key = null,
        ScrollBehavior behavior = default!,
        Widget child = default!
    )
        : base(key: key, child: child)
    {
        this.behavior = behavior;
    }

    public static ScrollBehavior of(BuildContext context)
    {
        ScrollConfiguration? configuration =
            context.dependOnInheritedWidgetOfExactType<ScrollConfiguration>();
        return configuration?.behavior ?? new ScrollBehavior();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (ScrollConfiguration)oldWidget;
        return (
                !Equals(
                    DartRuntimePrimitives.RuntimeType(behavior),
                    DartRuntimePrimitives.RuntimeType(__oldWidget.behavior)
                )
            )
            || (
                (!Equals(behavior, __oldWidget.behavior))
                && behavior.shouldNotify(__oldWidget.behavior)
            );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<ScrollBehavior>("behavior", behavior));
    }
}
