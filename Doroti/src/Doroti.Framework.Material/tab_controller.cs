// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/tab_controller.dart

using Doroti.Runtime;

namespace Doroti.Framework.Material;

public class TabController : ChangeNotifier
{
    internal virtual AnimationController? _animationController { get; set; } = default;
    internal virtual Duration _animationDuration { get; private set; } = default!;
    public virtual long length { get; private set; } = default!;
    internal virtual long _index { get; set; } = default!;
    internal virtual long _previousIndex { get; set; } = default!;
    internal virtual long _indexIsChangingCount { get; set; } = 0L;

    public TabController(long initialIndex = 0, Duration? animationDuration = null, long length = default!, Scheduler.TickerProvider vsync = default!)
    {
        this.length = length;
        _index = initialIndex;
        _previousIndex = initialIndex;
        _animationDuration = animationDuration ?? ConstantsLibrary.kTabScrollDuration;
        _animationController = AnimationController.CreateUnbounded(value: initialIndex.toDouble(), vsync: vsync);
        System.Diagnostics.Debug.Assert(length >= 0L);
        System.Diagnostics.Debug.Assert((initialIndex >= 0L) && ((DartRuntimePrimitives.RequireValue(length) == 0L) || (initialIndex < DartRuntimePrimitives.RequireValue(length))));
        if (MemoryAllocationsLibrary.kFlutterMemoryAllocationsEnabled)
        {
            maybeDispatchObjectCreation(this);
        }
    }

    public static TabController Create_(long index, long previousIndex, AnimationController? animationController, Duration animationDuration, long length)
    {
        var __instance = new TabController(animationDuration: animationDuration, length: length, vsync: default!);
        __instance.length = length;
        __instance._index = DartRuntimePrimitives.RequireValue(index);
        __instance._previousIndex = DartRuntimePrimitives.RequireValue(previousIndex);
        __instance._animationController = animationController;
        __instance._animationDuration = DartRuntimePrimitives.RequireValue(animationDuration);
        if (MemoryAllocationsLibrary.kFlutterMemoryAllocationsEnabled)
        {
            maybeDispatchObjectCreation(__instance);
        }
        return __instance;
    }

    internal virtual TabController _copyWithAndDispose(long? index, long? length, long? previousIndex, Duration? animationDuration)
    {
        if (index is not null)
        {
            long index__value5320 = DartRuntimePrimitives.RequireValue(index);
            _animationController!.value = DartRuntimePrimitives.RequireValue(index__value5320).toDouble();
        }
        var result = Create_(index: index ?? _index, length: length ?? this.length, animationController: _animationController, previousIndex: previousIndex ?? _previousIndex, animationDuration: animationDuration ?? _animationDuration);
        _animationController = null;
        dispose();
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Animation<double>? animation => _animationController?.view;
    public virtual Duration animationDuration => _animationDuration;
    internal virtual void _changeIndex(long value, Duration? duration = null, Curve? curve = null)
    {
        DartRuntimePrimitives.Assert(() => (value >= 0L) && ((value < length) || (length == 0L)));
        DartRuntimePrimitives.Assert(() => (duration is not null) || (curve is null));
        DartRuntimePrimitives.Assert(() => _indexIsChangingCount >= 0L);
        if ((value == _index) || (length < 2L))
        {
            return;
        }
        _previousIndex = index;
        _index = value;
        if ((duration is not null) && (DartRuntimePrimitives.RequireValue(duration) > Duration.zero))
        {
            Duration duration__value7060 = DartRuntimePrimitives.RequireValue(duration);
            _indexIsChangingCount += 1L;
            notifyListeners();
            _animationController!.animateTo(_index.toDouble(), duration: DartRuntimePrimitives.RequireValue(duration__value7060), curve: curve!).whenCompleteOrCancel(() =>
            {
                if (_animationController is not null)
                {
                    _indexIsChangingCount -= 1L;
                    notifyListeners();
                }
            });
        }
        else
        {
            _indexIsChangingCount += 1L;
            _animationController!.value = _index.toDouble();
            _indexIsChangingCount -= 1L;
            notifyListeners();
        }
    }

    public virtual long index
    {
        get => _index;
        set
        {
            var __value = value;
            _changeIndex(__value);
        }
    }
    public virtual long previousIndex => _previousIndex;
    public virtual bool indexIsChanging => DartRuntimePrimitives.ConvertValue<bool>(_indexIsChangingCount != 0L);
    public virtual void animateTo(long value, Duration? duration = null, Curve curve = default!)
    {
        _changeIndex(value, duration: duration ?? _animationDuration, curve: curve);
    }

    public virtual double offset
    {
        get => _animationController!.value - _index.toDouble();
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value >= -1.0) && (__value <= 1.0));
            DartRuntimePrimitives.Assert(() => !indexIsChanging);
            if (__value == offset)
            {
                return;
            }
            _animationController!.value = __value + _index.toDouble();
        }
    }
    public override void dispose()
    {
        _animationController?.dispose();
        _animationController = null;
        base.dispose();
    }

}

internal class _TabControllerScope__tab_controller : InheritedWidget
{
    public virtual TabController controller { get; private set; } = default!;
    public virtual bool enabled { get; private set; } = default!;

    internal _TabControllerScope__tab_controller(TabController controller, bool enabled, Widget child) : base(child: child)
    {
        this.controller = controller;
        this.enabled = enabled;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __old = (_TabControllerScope__tab_controller)oldWidget;
        return (enabled != __old.enabled) || (!Equals(controller, __old.controller));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DefaultTabController : StatefulWidget
{
    public virtual long length { get; private set; } = default!;
    public virtual long initialIndex { get; private set; } = default!;
    public virtual Duration? animationDuration { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public DefaultTabController(Key? key = null, long length = default!, long initialIndex = 0, Widget child = default!, Duration? animationDuration = null) : base(key: key)
    {
        this.length = length;
        this.initialIndex = initialIndex;
        this.child = child;
        this.animationDuration = animationDuration;
        System.Diagnostics.Debug.Assert(length >= 0L);
        System.Diagnostics.Debug.Assert((length == 0L) || (initialIndex >= 0L) && (initialIndex < length));
    }

    public static TabController? maybeOf(BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<_TabControllerScope__tab_controller>()?.controller;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TabController of(BuildContext context)
    {
        TabController? controller = maybeOf(context);
        DartRuntimePrimitives.Assert(() =>
            {
                if (controller is null)
                {
                    throw DartRuntimePrimitives.AsException(FlutterError.Create("DefaultTabController.of() was called with a context that does not " + "contain a DefaultTabController widget.\n" + "No DefaultTabController widget ancestor could be found starting from " + "the context that was passed to DefaultTabController.of(). This can " + "happen because you are using a widget that looks for a DefaultTabController " + "ancestor, but no such ancestor exists.\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
            });
        return controller!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DefaultTabControllerState__tab_controller());
}

internal class _DefaultTabControllerState__tab_controller : State<DefaultTabController>, SingleTickerProviderStateMixin<DefaultTabController>
{
    internal virtual TabController _controller { get; set; } = default!;
    public virtual Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _controller = new TabController(vsync: this, length: widget.length, initialIndex: widget.initialIndex, animationDuration: widget.animationDuration);
    }

    public override void dispose()
    {
        _controller.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((_ticker is null) || !_ticker!.isActive)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{this} was disposed with an active Ticker."), new ErrorDescription($"{GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), _ticker!.describeForError("The offending ticker was") }));
            });
        _tickerModeNotifier?.removeListener(_updateTicker);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return new _TabControllerScope__tab_controller(controller: _controller, enabled: TickerMode.of(context), child: widget.child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void didUpdateWidget(DefaultTabController oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (oldWidget.length != widget.length)
        {
            long? newIndex = default!;
            long previousIndexLocal = _controller.previousIndex;
            if (_controller.index >= widget.length)
            {
                newIndex = Math.Max(0L, widget.length - 1L);
                previousIndexLocal = _controller.index;
            }
            _controller = _controller._copyWithAndDispose(length: widget.length, animationDuration: widget.animationDuration, index: newIndex, previousIndex: previousIndexLocal);
        }
        if (!Equals(oldWidget.animationDuration, widget.animationDuration))
        {
            _controller = _controller._copyWithAndDispose(length: widget.length, animationDuration: widget.animationDuration, index: _controller.index, previousIndex: _controller.previousIndex);
        }
    }

    public virtual Scheduler.Ticker createTicker(System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (_ticker is null)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
            });
        _ticker = new Scheduler.Ticker(onTick, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
        _updateTickerModeNotifier();
        _updateTicker();
        return _ticker!;
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
        TickerModeData values = _tickerModeNotifier!.value;
        if (_ticker is not null)
        {
            _ticker!.muted = !values.enabled;
            _ticker!.forceFrames = values.forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTicker);
        newNotifier.addListener(_updateTicker);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription = (_ticker?.isActive, _ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) };
        properties.add(new DiagnosticsProperty<Scheduler.Ticker>("ticker", _ticker, description: tickerDescription, showSeparator: false, defaultValue: default));
    }

}
