// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scroll_configuration.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Widgets;

public static partial class Scroll_configurationLibrary
{
    internal static Color _kDefaultGlowColor = new global::Doroti.Ui.Color(4294967295L);
}

public static partial class Scroll_configurationLibrary
{
    internal static HashSet<PointerDeviceKind> _kTouchLikeDeviceTypes = new HashSet<PointerDeviceKind> { PointerDeviceKind.touch, PointerDeviceKind.stylus, PointerDeviceKind.invertedStylus, PointerDeviceKind.trackpad, PointerDeviceKind.unknown };
}

public enum AndroidOverscrollIndicator
{
    stretch,
    glow
}

public class ScrollBehavior
{
    internal static ScrollPhysics _bouncingPhysics = ((ScrollPhysics)(object?)new BouncingScrollPhysics(parent: new RangeMaintainingScrollPhysics()));
    internal static ScrollPhysics _bouncingDesktopPhysics = ((ScrollPhysics)(object?)new BouncingScrollPhysics(decelerationRate: ScrollDecelerationRate.fast, parent: new RangeMaintainingScrollPhysics()));
    internal static ScrollPhysics _clampingPhysics = ((ScrollPhysics)(object?)new ClampingScrollPhysics(parent: new RangeMaintainingScrollPhysics()));

    public ScrollBehavior()
    {
    }

    public virtual ScrollBehavior copyWith(bool? scrollbars = null, bool? overscroll = null, HashSet<PointerDeviceKind>? dragDevices = null, global::Doroti.Framework.Gestures.MultitouchDragStrategy? multitouchDragStrategy = null, HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey>? pointerAxisModifiers = null, ScrollPhysics? physics = null, global::Doroti.Framework.Foundation.TargetPlatform? platform = null, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null)
    {
        return ((ScrollBehavior)(object?)new _WrappedScrollBehavior__scroll_configuration(@delegate: this, scrollbars: (scrollbars ?? true), overscroll: (overscroll ?? true), dragDevices: dragDevices, multitouchDragStrategy: multitouchDragStrategy, pointerAxisModifiers: pointerAxisModifiers, physics: physics, platform: platform, keyboardDismissBehavior: keyboardDismissBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Foundation.TargetPlatform getPlatform(BuildContext context) => global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform;
    public virtual HashSet<global::Doroti.Ui.PointerDeviceKind> dragDevices => DartRuntimePrimitives.ConvertValue<HashSet<global::Doroti.Ui.PointerDeviceKind>>(Scroll_configurationLibrary._kTouchLikeDeviceTypes);
    public virtual global::Doroti.Framework.Gestures.MultitouchDragStrategy getMultitouchDragStrategy(BuildContext context)
    {
        switch (getPlatform(context))
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                {
                    return global::Doroti.Framework.Gestures.MultitouchDragStrategy.averageBoundaryPointers;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                {
                    return global::Doroti.Framework.Gestures.MultitouchDragStrategy.latestPointer;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> pointerAxisModifiers => new HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> { global::Doroti.Framework.Services.LogicalKeyboardKey.shiftLeft, global::Doroti.Framework.Services.LogicalKeyboardKey.shiftRight };
    public virtual Widget buildScrollbar(BuildContext context, Widget child, ScrollableDetails details)
    {
        switch (getPlatform(context))
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    DartRuntimePrimitives.Assert(() => (((ScrollableDetails)details).controller is not null));
                    return ((Widget)(object?)new RawScrollbar(controller: ((ScrollableDetails)details).controller, child: child));
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                {
                    return child;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Widget buildOverscrollIndicator(BuildContext context, Widget child, ScrollableDetails details)
    {
        switch (getPlatform(context))
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    return child;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                {
                    return ((Widget)(object?)new GlowingOverscrollIndicator(axisDirection: ((ScrollableDetails)details).direction, color: Scroll_configurationLibrary._kDefaultGlowColor, child: child));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::System.Func<global::Doroti.Framework.Gestures.PointerEvent, global::Doroti.Framework.Gestures.VelocityTracker> velocityTrackerBuilder(BuildContext context)
    {
        switch (getPlatform(context))
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                {
                    return ((global::System.Func<global::Doroti.Framework.Gestures.PointerEvent, global::Doroti.Framework.Gestures.VelocityTracker>)((@event) => new global::Doroti.Framework.Gestures.IOSScrollViewFlingVelocityTracker(((global::Doroti.Framework.Gestures.PointerEvent)@event).kind)));
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                {
                    return ((global::System.Func<global::Doroti.Framework.Gestures.PointerEvent, global::Doroti.Framework.Gestures.VelocityTracker>)((@event) => new global::Doroti.Framework.Gestures.MacOSScrollViewFlingVelocityTracker(((global::Doroti.Framework.Gestures.PointerEvent)@event).kind)));
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    return ((global::System.Func<global::Doroti.Framework.Gestures.PointerEvent, global::Doroti.Framework.Gestures.VelocityTracker>)((@event) => new global::Doroti.Framework.Gestures.VelocityTracker(((global::Doroti.Framework.Gestures.PointerEvent)@event).kind)));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ScrollPhysics getScrollPhysics(BuildContext context)
    {
        switch (getPlatform(context))
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                {
                    return _bouncingPhysics;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                {
                    return _bouncingDesktopPhysics;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    return _clampingPhysics;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool shouldNotify(ScrollBehavior oldDelegate) => false;
    public virtual ScrollViewKeyboardDismissBehavior getKeyboardDismissBehavior(BuildContext context) => ScrollViewKeyboardDismissBehavior.manual;
    public override string ToString() => global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "ScrollBehavior");
}

internal class _WrappedScrollBehavior__scroll_configuration : ScrollBehavior
{
    public virtual ScrollBehavior @delegate { get; private set; } = default!;
    public virtual bool scrollbars { get; private set; } = default!;
    public virtual bool overscroll { get; private set; } = default!;
    public virtual ScrollPhysics? physics { get; private set; }
    public virtual global::Doroti.Framework.Foundation.TargetPlatform? platform { get; private set; }
    public virtual ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior { get; private set; }
    internal virtual HashSet<PointerDeviceKind>? _dragDevices { get; private set; }
    public virtual global::Doroti.Framework.Gestures.MultitouchDragStrategy? multitouchDragStrategy { get; private set; }
    internal virtual HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey>? _pointerAxisModifiers { get; private set; }

    internal _WrappedScrollBehavior__scroll_configuration(ScrollBehavior @delegate, bool scrollbars = true, bool overscroll = true, HashSet<PointerDeviceKind>? dragDevices = null, global::Doroti.Framework.Gestures.MultitouchDragStrategy? multitouchDragStrategy = null, HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey>? pointerAxisModifiers = null, ScrollPhysics? physics = null, global::Doroti.Framework.Foundation.TargetPlatform? platform = null, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null)
    {
        this.@delegate = @delegate;
        this.scrollbars = scrollbars;
        this.overscroll = overscroll;
        this.multitouchDragStrategy = multitouchDragStrategy;
        this.physics = physics;
        this.platform = platform;
        this.keyboardDismissBehavior = keyboardDismissBehavior;
        this._dragDevices = dragDevices;
        this._pointerAxisModifiers = pointerAxisModifiers;
    }

    public override HashSet<PointerDeviceKind> dragDevices => DartRuntimePrimitives.ConvertValue<HashSet<PointerDeviceKind>>(((this._dragDevices ?? (HashSet<PointerDeviceKind>)((ScrollBehavior)this.@delegate).dragDevices)));
    public override HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> pointerAxisModifiers => DartRuntimePrimitives.ConvertValue<HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey>>(((this._pointerAxisModifiers ?? (HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey>)((ScrollBehavior)this.@delegate).pointerAxisModifiers)));
    public override global::Doroti.Framework.Gestures.MultitouchDragStrategy getMultitouchDragStrategy(BuildContext context)
    {
        return ((this.multitouchDragStrategy ?? (global::Doroti.Framework.Gestures.MultitouchDragStrategy)this.@delegate.getMultitouchDragStrategy(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget buildOverscrollIndicator(BuildContext context, Widget child, ScrollableDetails details)
    {
        if (this.overscroll)
        {
            return ((Widget)(object?)this.@delegate.buildOverscrollIndicator(context, child, details));
        }
        return child;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget buildScrollbar(BuildContext context, Widget child, ScrollableDetails details)
    {
        if (this.scrollbars)
        {
            return ((Widget)(object?)this.@delegate.buildScrollbar(context, child, details));
        }
        return child;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ScrollBehavior copyWith(bool? scrollbars = null, bool? overscroll = null, HashSet<PointerDeviceKind>? dragDevices = null, global::Doroti.Framework.Gestures.MultitouchDragStrategy? multitouchDragStrategy = null, HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey>? pointerAxisModifiers = null, ScrollPhysics? physics = null, global::Doroti.Framework.Foundation.TargetPlatform? platform = null, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null)
    {
        return ((ScrollBehavior)(object?)this.@delegate.copyWith(scrollbars: (scrollbars ?? this.scrollbars), overscroll: (overscroll ?? this.overscroll), dragDevices: (dragDevices ?? this.dragDevices), multitouchDragStrategy: (multitouchDragStrategy ?? this.multitouchDragStrategy), pointerAxisModifiers: (pointerAxisModifiers ?? this.pointerAxisModifiers), physics: (physics ?? this.physics), platform: (platform ?? this.platform), keyboardDismissBehavior: (keyboardDismissBehavior ?? this.keyboardDismissBehavior)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Foundation.TargetPlatform getPlatform(BuildContext context)
    {
        return ((this.platform ?? (global::Doroti.Framework.Foundation.TargetPlatform)this.@delegate.getPlatform(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ScrollPhysics getScrollPhysics(BuildContext context)
    {
        return ((this.physics ?? (ScrollPhysics)this.@delegate.getScrollPhysics(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ScrollViewKeyboardDismissBehavior getKeyboardDismissBehavior(BuildContext context)
    {
        return ((this.keyboardDismissBehavior ?? (ScrollViewKeyboardDismissBehavior)this.@delegate.getKeyboardDismissBehavior(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldNotify(ScrollBehavior oldDelegate)
    {
        var __oldDelegate = (_WrappedScrollBehavior__scroll_configuration)(object)oldDelegate;
        return (((((((((!object.Equals(DartRuntimePrimitives.RuntimeType(((_WrappedScrollBehavior__scroll_configuration)__oldDelegate).@delegate), DartRuntimePrimitives.RuntimeType(this.@delegate))) || (((_WrappedScrollBehavior__scroll_configuration)__oldDelegate).scrollbars != this.scrollbars)) || (((_WrappedScrollBehavior__scroll_configuration)__oldDelegate).overscroll != this.overscroll)) || !global::Doroti.Framework.Foundation.CollectionsLibrary.setEquals<global::Doroti.Ui.PointerDeviceKind>(((_WrappedScrollBehavior__scroll_configuration)__oldDelegate).dragDevices, this.dragDevices)) || (!object.Equals(((_WrappedScrollBehavior__scroll_configuration)__oldDelegate).multitouchDragStrategy, this.multitouchDragStrategy))) || !global::Doroti.Framework.Foundation.CollectionsLibrary.setEquals<global::Doroti.Framework.Services.LogicalKeyboardKey>(((_WrappedScrollBehavior__scroll_configuration)__oldDelegate).pointerAxisModifiers, this.pointerAxisModifiers)) || (!object.Equals(((_WrappedScrollBehavior__scroll_configuration)__oldDelegate).physics, this.physics))) || (!object.Equals(((_WrappedScrollBehavior__scroll_configuration)__oldDelegate).platform, this.platform))) || this.@delegate.shouldNotify(((_WrappedScrollBehavior__scroll_configuration)__oldDelegate).@delegate));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::System.Func<global::Doroti.Framework.Gestures.PointerEvent, global::Doroti.Framework.Gestures.VelocityTracker> velocityTrackerBuilder(BuildContext context)
    {
        return ((global::System.Func<global::Doroti.Framework.Gestures.PointerEvent, global::Doroti.Framework.Gestures.VelocityTracker>)this.@delegate.velocityTrackerBuilder(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "_WrappedScrollBehavior");
}

public class ScrollConfiguration : InheritedWidget
{
    public virtual ScrollBehavior behavior { get; private set; } = default!;

    public ScrollConfiguration(global::Doroti.Framework.Foundation.Key? key = null, ScrollBehavior behavior = default!, Widget child = default!) : base(key: key, child: child)
    {
        this.behavior = behavior;
    }

    public static ScrollBehavior of(BuildContext context)
    {
        ScrollConfiguration? configuration__15968 = ((ScrollConfiguration?)(object?)context.dependOnInheritedWidgetOfExactType<ScrollConfiguration>());
        return (configuration__15968?.behavior ?? new ScrollBehavior());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (ScrollConfiguration)(object)oldWidget;
        return ((!object.Equals(DartRuntimePrimitives.RuntimeType(this.behavior), DartRuntimePrimitives.RuntimeType(((ScrollConfiguration)__oldWidget).behavior))) || (((!object.Equals(this.behavior, ((ScrollConfiguration)__oldWidget).behavior)) && this.behavior.shouldNotify(((ScrollConfiguration)__oldWidget).behavior))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ScrollBehavior>("behavior", this.behavior));
    }

}

