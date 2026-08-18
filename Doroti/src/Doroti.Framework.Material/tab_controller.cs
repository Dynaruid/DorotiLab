// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/tab_controller.dart
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

public class TabController : global::Doroti.Framework.Foundation.ChangeNotifier
{
    internal virtual global::Doroti.Framework.Animation.AnimationController? _animationController { get; set; } = default;
    internal virtual Duration _animationDuration { get; private set; } = default!;
    public virtual long length { get; private set; } = default!;
    internal virtual long _index { get; set; } = default!;
    internal virtual long _previousIndex { get; set; } = default!;
    internal virtual long _indexIsChangingCount { get; set; } = 0L;

    public TabController(long initialIndex = 0, Duration? animationDuration = null, long length = default!, global::Doroti.Framework.Scheduler.TickerProvider vsync = default!)
    {
        this.length = length;
        this._index = initialIndex;
        this._previousIndex = initialIndex;
        this._animationDuration = (animationDuration ?? ConstantsLibrary.kTabScrollDuration);
        this._animationController = global::Doroti.Framework.Animation.AnimationController.CreateUnbounded(value: initialIndex.toDouble(), vsync: vsync);
        System.Diagnostics.Debug.Assert((length >= 0L));
        System.Diagnostics.Debug.Assert(((initialIndex >= 0L) && (((DartRuntimePrimitives.RequireValue(length) == 0L) || (initialIndex < DartRuntimePrimitives.RequireValue(length))))));
        if (global::Doroti.Framework.Foundation.MemoryAllocationsLibrary.kFlutterMemoryAllocationsEnabled)
        {
            ChangeNotifier.maybeDispatchObjectCreation(this);
        }
    }

    public static TabController Create_(long index, long previousIndex, global::Doroti.Framework.Animation.AnimationController? animationController, Duration animationDuration, long length)
    {
        var __instance = new TabController(animationDuration: animationDuration, length: length, vsync: default!);
        __instance.length = length;
        __instance._index = DartRuntimePrimitives.RequireValue(index);
        __instance._previousIndex = DartRuntimePrimitives.RequireValue(previousIndex);
        __instance._animationController = animationController;
        __instance._animationDuration = DartRuntimePrimitives.RequireValue(animationDuration);
        if (global::Doroti.Framework.Foundation.MemoryAllocationsLibrary.kFlutterMemoryAllocationsEnabled)
        {
            ChangeNotifier.maybeDispatchObjectCreation(__instance);
        }
        return __instance;
    }

    internal virtual TabController _copyWithAndDispose(long? index, long? length, long? previousIndex, Duration? animationDuration)
    {
        if ((index is not null))
        {
            long index__value5320 = DartRuntimePrimitives.RequireValue(index);
            this._animationController!.value = DartRuntimePrimitives.RequireValue(index__value5320).toDouble();
        }
        var result__5407 = TabController.Create_(index: (index ?? this._index), length: (length ?? this.length), animationController: this._animationController, previousIndex: (previousIndex ?? this._previousIndex), animationDuration: (animationDuration ?? this._animationDuration));
        _animationController = null;
        dispose();
        return result__5407;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Animation.Animation<double>? animation => this._animationController?.view;
    public virtual Duration animationDuration => this._animationDuration;
    internal virtual void _changeIndex(long value, Duration? duration = null, global::Doroti.Framework.Animation.Curve? curve = null)
    {
        DartRuntimePrimitives.Assert(() => ((value >= 0L) && (((value < this.length) || (this.length == 0L)))));
        DartRuntimePrimitives.Assert(() => ((duration is not null) || (curve is null)));
        DartRuntimePrimitives.Assert(() => (this._indexIsChangingCount >= 0L));
        if (((value == this._index) || (this.length < 2L)))
        {
            return;
        }
        _previousIndex = this.index;
        _index = value;
        if (((duration is not null) && (DartRuntimePrimitives.RequireValue(duration) > Duration.zero)))
        {
            Duration duration__value7060 = DartRuntimePrimitives.RequireValue(duration);
            _indexIsChangingCount += 1L;
            notifyListeners();
            this._animationController!.animateTo(this._index.toDouble(), duration: DartRuntimePrimitives.RequireValue(duration__value7060), curve: curve!).whenCompleteOrCancel(((global::System.Action)(() =>
            {
                if ((this._animationController is not null))
                {
                    _indexIsChangingCount -= 1L;
                    notifyListeners();
                }
            })));
        }
        else
        {
            _indexIsChangingCount += 1L;
            this._animationController!.value = this._index.toDouble();
            _indexIsChangingCount -= 1L;
            notifyListeners();
        }
    }

    public virtual long index
    {
        get => this._index;
        set
        {
            var __value = value;
            _changeIndex(__value);
        }
    }
    public virtual long previousIndex => this._previousIndex;
    public virtual bool indexIsChanging => DartRuntimePrimitives.ConvertValue<bool>((this._indexIsChangingCount != 0L));
    public virtual void animateTo(long value, Duration? duration = null, global::Doroti.Framework.Animation.Curve curve = default!)
    {
        _changeIndex(value, duration: (duration ?? this._animationDuration), curve: curve);
    }

    public virtual double offset
    {
        get => (this._animationController!.value - this._index.toDouble());
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => ((__value >= -1.0) && (__value <= 1.0)));
            DartRuntimePrimitives.Assert(() => !this.indexIsChanging);
            if ((__value == this.offset))
            {
                return;
            }
            this._animationController!.value = (__value + this._index.toDouble());
        }
    }
    public virtual void dispose()
    {
        this._animationController?.dispose();
        _animationController = null;
        base.dispose();
    }

}

internal class _TabControllerScope__tab_controller : global::Doroti.Framework.Widgets.InheritedWidget
{
    public virtual TabController controller { get; private set; } = default!;
    public virtual bool enabled { get; private set; } = default!;

    internal _TabControllerScope__tab_controller(TabController controller, bool enabled, global::Doroti.Framework.Widgets.Widget child) : base(child: child)
    {
        this.controller = controller;
        this.enabled = enabled;
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget)
    {
        var __old = (_TabControllerScope__tab_controller)(object)oldWidget;
        return ((this.enabled != ((_TabControllerScope__tab_controller)__old).enabled) || (!object.Equals(this.controller, ((_TabControllerScope__tab_controller)__old).controller)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DefaultTabController : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual long length { get; private set; } = default!;
    public virtual long initialIndex { get; private set; } = default!;
    public virtual Duration? animationDuration { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;

    public DefaultTabController(global::Doroti.Framework.Foundation.Key? key = null, long length = default!, long initialIndex = 0, global::Doroti.Framework.Widgets.Widget child = default!, Duration? animationDuration = null) : base(key: key)
    {
        this.length = length;
        this.initialIndex = initialIndex;
        this.child = child;
        this.animationDuration = animationDuration;
        System.Diagnostics.Debug.Assert((length >= 0L));
        System.Diagnostics.Debug.Assert(((length == 0L) || (((initialIndex >= 0L) && (initialIndex < length)))));
    }

    public static TabController? maybeOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<_TabControllerScope__tab_controller>()?.controller;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TabController of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        TabController? controller__14638 = ((TabController?)(object?)DefaultTabController.maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((controller__14638 is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("DefaultTabController.of() was called with a context that does not " + "contain a DefaultTabController widget.\n" + "No DefaultTabController widget ancestor could be found starting from " + "the context that was passed to DefaultTabController.of(). This can " + "happen because you are using a widget that looks for a DefaultTabController " + "ancestor, but no such ancestor exists.\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
            });
        return controller__14638!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DefaultTabControllerState__tab_controller());
}

internal class _DefaultTabControllerState__tab_controller : global::Doroti.Framework.Widgets.State<DefaultTabController>, global::Doroti.Framework.Widgets.SingleTickerProviderStateMixin<DefaultTabController>
{
    internal virtual TabController _controller { get; set; } = default!;
    public virtual global::Doroti.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _controller = new TabController(vsync: this, length: ((DefaultTabController)this.widget).length, initialIndex: ((DefaultTabController)this.widget).initialIndex, animationDuration: ((DefaultTabController)this.widget).animationDuration);
    }

    public override void dispose()
    {
        this._controller.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if (((this._ticker is null) || !this._ticker!.isActive))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), this._ticker!.describeForError("The offending ticker was") }));
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _TabControllerScope__tab_controller(controller: this._controller, enabled: TickerMode.of(context), child: ((DefaultTabController)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void didUpdateWidget(DefaultTabController oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((((DefaultTabController)oldWidget).length != ((DefaultTabController)this.widget).length))
        {
            long? newIndex__16396 = default!;
            long previousIndex__16416 = ((TabController)this._controller).previousIndex;
            if ((((TabController)this._controller).index >= ((DefaultTabController)this.widget).length))
            {
                newIndex__16396 = Math.Max(0L, (((DefaultTabController)this.widget).length - 1L));
                previousIndex__16416 = ((TabController)this._controller).index;
            }
            _controller = this._controller._copyWithAndDispose(length: ((DefaultTabController)this.widget).length, animationDuration: ((DefaultTabController)this.widget).animationDuration, index: newIndex__16396, previousIndex: previousIndex__16416);
        }
        if ((!object.Equals(((DefaultTabController)oldWidget).animationDuration, ((DefaultTabController)this.widget).animationDuration)))
        {
            _controller = this._controller._copyWithAndDispose(length: ((DefaultTabController)this.widget).length, animationDuration: ((DefaultTabController)this.widget).animationDuration, index: ((TabController)this._controller).index, previousIndex: ((TabController)this._controller).previousIndex);
        }
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._ticker is null))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this.GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new global::Doroti.Framework.Foundation.ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new global::Doroti.Framework.Foundation.ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
            });
        this._ticker = new global::Doroti.Framework.Scheduler.Ticker((global::System.Action<Duration>)onTick, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
        _updateTickerModeNotifier();
        _updateTicker();
        return this._ticker!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTicker();
    }

    public virtual void _updateTicker()
    {
        TickerModeData values__15157 = this._tickerModeNotifier!.value;
        if ((this._ticker is not null))
        {
            this._ticker!.muted = !((TickerModeData)values__15157).enabled;
            this._ticker!.forceFrames = ((TickerModeData)values__15157).forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__15400 = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__15400, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        newNotifier__15400.addListener(() => this._updateTicker());
        this._tickerModeNotifier = newNotifier__15400;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription__15805 = ((this._ticker?.isActive, this._ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) });
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Scheduler.Ticker>("ticker", this._ticker, description: tickerDescription__15805, showSeparator: false, defaultValue: default));
    }

}
