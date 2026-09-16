// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/draggable_scrollable_sheet.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public delegate Widget ScrollableWidgetBuilder(BuildContext context, ScrollController scrollController);

public class DraggableScrollableController : ChangeNotifier
{
    internal virtual _DraggableScrollableSheetScrollController__draggable_scrollable_sheet? _attachedController { get; set; } = default;
    internal virtual HashSet<AnimationController> _animationControllers { get; private set; } = new HashSet<AnimationController>();

    public DraggableScrollableController()
    {
    }

    public virtual double size
    {
        get
        {
            _assertAttached();
            return _attachedController!.extent.currentSize;
        }
    }
    public virtual double pixels
    {
        get
        {
            _assertAttached();
            return _attachedController!.extent.currentPixels;
        }
    }
    public virtual double sizeToPixels(double size)
    {
        _assertAttached();
        return _attachedController!.extent.sizeToPixels(size);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isAttached => DartRuntimePrimitives.ConvertValue<bool>((_attachedController is not null) && _attachedController!.hasClients);
    public virtual double pixelsToSize(double pixels)
    {
        _assertAttached();
        return _attachedController!.extent.pixelsToSize(pixels);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future animateTo(double size, Duration duration, Curve curve)
    {
        _assertAttached();
        DartRuntimePrimitives.Assert(() => (size >= 0L) && (size <= 1L));
        DartRuntimePrimitives.Assert(() => !Equals(duration, Duration.zero));
        var animationController = AnimationController.CreateUnbounded(vsync: _attachedController!.position.context.vsync, value: _attachedController!.extent.currentSize);
        _animationControllers.Add(animationController);
        _attachedController!.position.goIdle();
        _attachedController!.extent.hasDragged = false;
        _attachedController!.extent.hasChanged = true;
        _attachedController!.extent.startActivity(onCanceled: () =>
        {
            if (animationController.isAnimating)
            {
                animationController.stop();
            }
        });
        animationController.addListener(() =>
        {
            _attachedController!.extent.updateSize(animationController.value, _attachedController!.position.context.notificationContext!);
        });
        await animationController.animateTo(Dart_uiLibrary.clampDouble(size, _attachedController!.extent.minSize, _attachedController!.extent.maxSize), duration: duration, curve: curve);
    }

    public virtual void jumpTo(double size)
    {
        _assertAttached();
        DartRuntimePrimitives.Assert(() => (size >= 0L) && (size <= 1L));
        _attachedController!.extent.startActivity(onCanceled: () =>
        {
        });
        _attachedController!.position.goIdle();
        _attachedController!.extent.hasDragged = false;
        _attachedController!.extent.hasChanged = true;
        _attachedController!.extent.updateSize(size, _attachedController!.position.context.notificationContext!);
    }

    public virtual void reset()
    {
        _assertAttached();
        _attachedController!.reset();
    }

    internal virtual void _assertAttached()
    {
        DartRuntimePrimitives.Assert(() => isAttached, () => (object?)"DraggableScrollableController is not attached to a sheet. A DraggableScrollableController " + "must be used in a DraggableScrollableSheet before any of its methods are called.");
    }

    internal virtual void _attach(_DraggableScrollableSheetScrollController__draggable_scrollable_sheet scrollController)
    {
        DartRuntimePrimitives.Assert(() => _attachedController is null, () => (object?)"Draggable scrollable controller is already attached to a sheet.");
        _attachedController = scrollController;
        _attachedController!.extent._currentSize.addListener(notifyListeners);
        _attachedController!.onPositionDetached = _disposeAnimationControllers;
    }

    internal virtual void _onExtentReplaced(_DraggableSheetExtent__draggable_scrollable_sheet previousExtent)
    {
        _attachedController!.extent._currentSize.addListener(notifyListeners);
        if (previousExtent.currentSize != _attachedController!.extent.currentSize)
        {
            notifyListeners();
        }
    }

    internal virtual void _detach(bool disposeExtent = false)
    {
        if (disposeExtent)
        {
            _attachedController?.extent.dispose();
        }
        else
        {
            _attachedController?.extent._currentSize.removeListener(notifyListeners);
        }
        _disposeAnimationControllers();
        _attachedController = null;
    }

    internal virtual void _disposeAnimationControllers()
    {
        foreach (AnimationController animationController in _animationControllers)
        {
            animationController.dispose();
        }
        _animationControllers.Clear();
    }

}

public class DraggableScrollableSheet : StatefulWidget
{
    public virtual double initialChildSize { get; private set; } = default!;
    public virtual double minChildSize { get; private set; } = default!;
    public virtual double maxChildSize { get; private set; } = default!;
    public virtual bool expand { get; private set; } = default!;
    public virtual bool snap { get; private set; } = default!;
    public virtual List<double>? snapSizes { get; private set; }
    public virtual Duration? snapAnimationDuration { get; private set; }
    public virtual DraggableScrollableController? controller { get; private set; }
    public virtual bool shouldCloseOnMinExtent { get; private set; } = default!;
    public virtual Func<BuildContext, ScrollController, Widget> builder { get; private set; } = default!;

    public DraggableScrollableSheet(Key? key = null, double initialChildSize = 0.5, double minChildSize = 0.25, double maxChildSize = 1.0, bool expand = true, bool snap = false, List<double>? snapSizes = null, Duration? snapAnimationDuration = null, DraggableScrollableController? controller = null, bool shouldCloseOnMinExtent = true, Func<BuildContext, ScrollController, Widget> builder = default!) : base(key: key)
    {
        this.initialChildSize = initialChildSize;
        this.minChildSize = minChildSize;
        this.maxChildSize = maxChildSize;
        this.expand = expand;
        this.snap = snap;
        this.snapSizes = snapSizes;
        this.snapAnimationDuration = snapAnimationDuration;
        this.controller = controller;
        this.shouldCloseOnMinExtent = shouldCloseOnMinExtent;
        this.builder = builder;
        System.Diagnostics.Debug.Assert(minChildSize >= 0.0);
        System.Diagnostics.Debug.Assert(maxChildSize <= 1.0);
        System.Diagnostics.Debug.Assert(minChildSize <= initialChildSize);
        System.Diagnostics.Debug.Assert(initialChildSize <= maxChildSize);
        System.Diagnostics.Debug.Assert((snapAnimationDuration is null) || (DartRuntimePrimitives.RequireValue(snapAnimationDuration) > Duration.zero));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DraggableScrollableSheetState__draggable_scrollable_sheet());
}

public class DraggableScrollableNotification : Notification, ViewportNotificationMixin
{
    public virtual double extent { get; private set; } = default!;
    public virtual double minExtent { get; private set; } = default!;
    public virtual double maxExtent { get; private set; } = default!;
    public virtual double initialExtent { get; private set; } = default!;
    public virtual BuildContext context { get; private set; } = default!;
    public virtual bool shouldCloseOnMinExtent { get; private set; } = default!;
    public virtual long _depth { get; set; } = 0L;

    public DraggableScrollableNotification(double extent, double minExtent, double maxExtent, double initialExtent, BuildContext context, bool shouldCloseOnMinExtent = true)
    {
        this.extent = extent;
        this.minExtent = minExtent;
        this.maxExtent = maxExtent;
        this.initialExtent = initialExtent;
        this.context = context;
        this.shouldCloseOnMinExtent = shouldCloseOnMinExtent;
        System.Diagnostics.Debug.Assert(0.0 <= minExtent);
        System.Diagnostics.Debug.Assert(maxExtent <= 1.0);
        System.Diagnostics.Debug.Assert(minExtent <= extent);
        System.Diagnostics.Debug.Assert(minExtent <= initialExtent);
        System.Diagnostics.Debug.Assert(extent <= maxExtent);
        System.Diagnostics.Debug.Assert(initialExtent <= maxExtent);
    }

    public override void debugFillDescription(List<string> description)
    {
        base.debugFillDescription(description);
        description.Add($"depth: {depth} ({((depth == 0L) ? "local" : "remote")})");
        description.Add($"minExtent: {minExtent}, extent: {extent}, maxExtent: {maxExtent}, initialExtent: {initialExtent}");
    }

    public virtual long depth => _depth;
}

public class _DraggableSheetExtent__draggable_scrollable_sheet
{
    internal virtual Action? _cancelActivity { get; set; } = default;
    public virtual double minSize { get; private set; } = default!;
    public virtual double maxSize { get; private set; } = default!;
    public virtual bool snap { get; private set; } = default!;
    public virtual List<double> snapSizes { get; private set; } = default!;
    public virtual Duration? snapAnimationDuration { get; private set; }
    public virtual double initialSize { get; private set; } = default!;
    public virtual bool shouldCloseOnMinExtent { get; private set; } = default!;
    internal virtual ValueNotifier<double> _currentSize { get; private set; } = default!;
    public virtual double availablePixels { get; set; } = default!;
    public virtual bool hasDragged { get; set; } = default!;
    public virtual bool hasChanged { get; set; } = default!;

    internal _DraggableSheetExtent__draggable_scrollable_sheet(double minSize, double maxSize, bool snap, List<double> snapSizes, double initialSize, Duration? snapAnimationDuration = null, ValueNotifier<double>? currentSize = null, bool? hasDragged = null, bool? hasChanged = null, bool shouldCloseOnMinExtent = true)
    {
        this.minSize = minSize;
        this.maxSize = maxSize;
        this.snap = snap;
        this.snapSizes = snapSizes;
        this.initialSize = initialSize;
        this.snapAnimationDuration = snapAnimationDuration;
        this.shouldCloseOnMinExtent = shouldCloseOnMinExtent;
        _currentSize = currentSize ?? new ValueNotifier<double>(initialSize);
        availablePixels = double.PositiveInfinity;
        this.hasDragged = hasDragged ?? false;
        this.hasChanged = hasChanged ?? false;
        System.Diagnostics.Debug.Assert(minSize >= 0L);
        System.Diagnostics.Debug.Assert(maxSize <= 1L);
        System.Diagnostics.Debug.Assert(minSize <= initialSize);
        System.Diagnostics.Debug.Assert(initialSize <= maxSize);
    }

    public virtual bool isAtMin => DartRuntimePrimitives.ConvertValue<bool>(minSize >= _currentSize.value);
    public virtual bool isAtMax => DartRuntimePrimitives.ConvertValue<bool>(maxSize <= _currentSize.value);
    public virtual double currentSize => _currentSize.value;
    public virtual double currentPixels => sizeToPixels(_currentSize.value);
    public virtual List<double> pixelSnapSizes => snapSizes.map(sizeToPixels).ToList();
    public virtual void startActivity(Action onCanceled)
    {
        _cancelActivity?.Invoke();
        _cancelActivity = onCanceled;
    }

    public virtual void addPixelDelta(double delta, BuildContext context)
    {
        _cancelActivity?.Invoke();
        _cancelActivity = null;
        hasDragged = true;
        hasChanged = true;
        if (availablePixels == 0L)
        {
            return;
        }
        updateSize(currentSize + pixelsToSize(delta), context);
    }

    public virtual void updateSize(double newSize, BuildContext context)
    {
        double clampedSize = Dart_uiLibrary.clampDouble(newSize, minSize, maxSize);
        if (_currentSize.value == clampedSize)
        {
            return;
        }
        _currentSize.value = clampedSize;
        new DraggableScrollableNotification(minExtent: minSize, maxExtent: maxSize, extent: DartRuntimePrimitives.RequireValue(currentSize), initialExtent: initialSize, context: context, shouldCloseOnMinExtent: shouldCloseOnMinExtent).dispatch(context);
    }

    public virtual double pixelsToSize(double pixels)
    {
        return pixels / availablePixels * maxSize;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double sizeToPixels(double size)
    {
        return size / maxSize * availablePixels;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        _currentSize.dispose();
    }

    public virtual _DraggableSheetExtent__draggable_scrollable_sheet copyWith(double minSize, double maxSize, bool snap, List<double> snapSizes, double initialSize, Duration? snapAnimationDuration, bool shouldCloseOnMinExtent)
    {
        return new _DraggableSheetExtent__draggable_scrollable_sheet(minSize: minSize, maxSize: maxSize, snap: snap, snapSizes: snapSizes, snapAnimationDuration: snapAnimationDuration, initialSize: initialSize, currentSize: new ValueNotifier<double>(hasChanged ? Dart_uiLibrary.clampDouble(_currentSize.value, minSize, maxSize) : initialSize), hasDragged: hasDragged, hasChanged: hasChanged, shouldCloseOnMinExtent: shouldCloseOnMinExtent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DraggableScrollableSheetState__draggable_scrollable_sheet : State<DraggableScrollableSheet>
{
    internal virtual _DraggableScrollableSheetScrollController__draggable_scrollable_sheet _scrollController { get; set; } = default!;
    internal virtual _DraggableSheetExtent__draggable_scrollable_sheet _extent { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        _extent = new _DraggableSheetExtent__draggable_scrollable_sheet(minSize: widget.minChildSize, maxSize: widget.maxChildSize, snap: widget.snap, snapSizes: _impliedSnapSizes(), snapAnimationDuration: widget.snapAnimationDuration, initialSize: widget.initialChildSize, shouldCloseOnMinExtent: widget.shouldCloseOnMinExtent);
        _scrollController = new _DraggableScrollableSheetScrollController__draggable_scrollable_sheet(extent: _extent);
        widget.controller?._attach(_scrollController);
    }

    internal virtual List<double> _impliedSnapSizes()
    {
        for (var index = 0L; index < (((long?)(widget.snapSizes?.Count)) ?? 0L); index += 1L)
        {
            double snapSize = widget.snapSizes![(int)index];
            DartRuntimePrimitives.Assert(() => (snapSize >= widget.minChildSize) && (snapSize <= widget.maxChildSize), () => (object?)$"{_snapSizeErrorMessage(index)}\nSnap sizes must be between `minChildSize` and `maxChildSize`. ");
            DartRuntimePrimitives.Assert(() => (index == 0L) || (snapSize > widget.snapSizes![(int)(index - 1L)]), () => (object?)$"{_snapSizeErrorMessage(index)}\nSnap sizes must be in ascending order. ");
        }
        if ((widget.snapSizes is null) || !Enumerable.Any(widget.snapSizes!))
        {
            return new List<double> { widget.minChildSize, widget.maxChildSize };
        }
        return new List<double>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void didUpdateWidget(DraggableScrollableSheet oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.controller, oldWidget.controller))
        {
            oldWidget.controller?._detach();
            widget.controller?._attach(_scrollController);
        }
        _replaceExtent(oldWidget);
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        if (_InheritedResetNotifier__draggable_scrollable_sheet.shouldReset(context))
        {
            _scrollController.reset();
        }
    }

    public override Widget build(BuildContext context)
    {
        return new ValueListenableBuilder<double>(valueListenable: _extent._currentSize, builder: (context, currentSize, child) => new LayoutBuilder(builder: (context, constraints) =>
        {
            _extent.availablePixels = widget.maxChildSize * constraints.biggest.height;
            Widget sheet = new FractionallySizedBox(heightFactor: currentSize, alignment: Alignment.bottomCenter, child: child);
            return widget.expand ? SizedBox.CreateExpand(child: sheet) : sheet;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }), child: widget.builder(context, _scrollController));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        if (widget.controller is null)
        {
            _extent.dispose();
        }
        else
        {
            widget.controller!._detach(disposeExtent: true);
        }
        _scrollController.dispose();
        base.dispose();
    }

    internal virtual void _replaceExtent(DraggableScrollableSheet oldWidget)
    {
        _DraggableSheetExtent__draggable_scrollable_sheet previousExtent = _extent;
        _extent = previousExtent.copyWith(minSize: widget.minChildSize, maxSize: widget.maxChildSize, snap: widget.snap, snapSizes: _impliedSnapSizes(), snapAnimationDuration: widget.snapAnimationDuration, initialSize: widget.initialChildSize, shouldCloseOnMinExtent: widget.shouldCloseOnMinExtent);
        _scrollController.extent = _extent;
        widget.controller?._onExtentReplaced(previousExtent);
        previousExtent.dispose();
        if (widget.snap && ((widget.snap != oldWidget.snap) || (!Equals(widget.snapSizes, oldWidget.snapSizes))) && _scrollController.hasClients)
        {
            WidgetsBinding.instance.addPostFrameCallback((timeStamp) =>
            {
                for (var index = 0L; index < _scrollController.positions.Count(); index++)
                {
                    var position = ((_DraggableScrollableSheetScrollPosition__draggable_scrollable_sheet?)_scrollController.positions.elementAt(index))!;
                    position.goBallistic(0);
                }
            }, debugLabel: "DraggableScrollableSheet.snap");
        }
    }

    internal virtual string _snapSizeErrorMessage(long invalidIndex)
    {
        List<string> snapSizesWithIndicator = widget.snapSizes!.asMap().Keys.map((index) =>
        {
            var snapSizeString = widget.snapSizes![(int)index].ToString();
            if (index == invalidIndex)
            {
                return $">>> {snapSizeString} <<<";
            }
            return snapSizeString;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }).ToList().ToList();
        return $"Invalid snapSize '{widget.snapSizes![(int)invalidIndex]}' at index {invalidIndex} of:\n" + $"  {snapSizesWithIndicator}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DraggableScrollableSheetScrollController__draggable_scrollable_sheet : ScrollController
{
    public virtual _DraggableSheetExtent__draggable_scrollable_sheet extent { get; set; } = default!;
    public virtual Action? onPositionDetached { get; set; } = default;

    internal _DraggableScrollableSheetScrollController__draggable_scrollable_sheet(_DraggableSheetExtent__draggable_scrollable_sheet extent)
    {
        this.extent = extent;
    }

    public override _DraggableScrollableSheetScrollPosition__draggable_scrollable_sheet createScrollPosition(ScrollPhysics physics, ScrollContext context, ScrollPosition? oldPosition)
    {
        return new _DraggableScrollableSheetScrollPosition__draggable_scrollable_sheet(physics: physics.applyTo(new AlwaysScrollableScrollPhysics()), context: context, oldPosition: oldPosition, getExtent: () => extent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillDescription(List<string> description)
    {
        base.debugFillDescription(description);
        description.Add($"extent: {extent}");
    }

    public override _DraggableScrollableSheetScrollPosition__draggable_scrollable_sheet position => ((_DraggableScrollableSheetScrollPosition__draggable_scrollable_sheet?)base.position)!;
    public virtual void reset()
    {
        extent._cancelActivity?.Invoke();
        extent.hasDragged = false;
        extent.hasChanged = false;
        if (offset != 0.0)
        {
            DartRuntimePrimitives.Ignore(animateTo(0.0, duration: Duration.Create(milliseconds: 1L), curve: Curves.linear));
        }
        extent.updateSize(extent.initialSize, position.context.notificationContext!);
    }

    public override void detach(ScrollPosition position)
    {
        onPositionDetached?.Invoke();
        base.detach(position);
    }

}

public class _DraggableScrollableSheetScrollPosition__draggable_scrollable_sheet : ScrollPositionWithSingleContext
{
    internal virtual Action? _dragCancelCallback { get; set; } = default;
    public virtual Func<_DraggableSheetExtent__draggable_scrollable_sheet> getExtent { get; private set; } = default!;
    internal virtual HashSet<AnimationController> _ballisticControllers { get; private set; } = new HashSet<AnimationController>();

    internal _DraggableScrollableSheetScrollPosition__draggable_scrollable_sheet(ScrollPhysics physics, ScrollContext context, ScrollPosition? oldPosition = null, Func<_DraggableSheetExtent__draggable_scrollable_sheet> getExtent = default!) : base(physics: physics, context: context, oldPosition: oldPosition)
    {
        this.getExtent = getExtent;
    }

    public virtual bool listShouldScroll => DartRuntimePrimitives.ConvertValue<bool>(pixels > 0.0);
    public virtual _DraggableSheetExtent__draggable_scrollable_sheet extent => getExtent();
    public override void absorb(ScrollPosition other)
    {
        base.absorb(other);
        DartRuntimePrimitives.Assert(() => _dragCancelCallback is null);
        if (other is not _DraggableScrollableSheetScrollPosition__draggable_scrollable_sheet)
        {
            return;
        }
        if (((_DraggableScrollableSheetScrollPosition__draggable_scrollable_sheet)other)._dragCancelCallback is not null)
        {
            _dragCancelCallback = ((_DraggableScrollableSheetScrollPosition__draggable_scrollable_sheet)other)._dragCancelCallback;
            ((_DraggableScrollableSheetScrollPosition__draggable_scrollable_sheet)other)._dragCancelCallback = null;
        }
    }

    public override void beginActivity(ScrollActivity? newActivity)
    {
        foreach (AnimationController ballisticController in _ballisticControllers)
        {
            ballisticController.stop();
        }
        base.beginActivity(newActivity);
    }

    public override void applyUserOffset(double delta)
    {
        if (!listShouldScroll && (!(extent.isAtMin || extent.isAtMax) || extent.isAtMin && (delta < 0L) || extent.isAtMax && (delta > 0L)))
        {
            extent.addPixelDelta(-delta, context.notificationContext!);
        }
        else
        {
            base.applyUserOffset(delta);
        }
    }

    internal virtual double? _getCurrentSnapSize()
    {
        return extent.snapSizes.firstWhereOrNull((snapSize) =>
        {
            return (extent.currentSize - snapSize).abs() <= extent.pixelsToSize(physics.toleranceFor(this).distance);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isAtSnapSize() => DartRuntimePrimitives.ConvertValue<bool>(_getCurrentSnapSize() is not null);
    internal virtual bool _shouldSnap() => DartRuntimePrimitives.ConvertValue<bool>(extent.snap && extent.hasDragged && !_isAtSnapSize());
    public override void dispose()
    {
        foreach (AnimationController ballisticController in _ballisticControllers)
        {
            ballisticController.dispose();
        }
        _ballisticControllers.Clear();
        base.dispose();
    }

    public override void goBallistic(double velocity)
    {
        if ((velocity == 0.0) && !_shouldSnap() || (velocity < 0.0) && listShouldScroll || (velocity > 0.0) && extent.isAtMax)
        {
            base.goBallistic(velocity);
            return;
        }
        _dragCancelCallback?.Invoke();
        _dragCancelCallback = null;
        Physics.Simulation simulation = default!;
        if (extent.snap)
        {
            simulation = DartRuntimePrimitives.ConvertValue<Physics.Simulation>(new _SnappingSimulation__draggable_scrollable_sheet(position: extent.currentPixels, initialVelocity: velocity, pixelSnapSize: extent.pixelSnapSizes, snapAnimationDuration: extent.snapAnimationDuration, tolerance: physics.toleranceFor(this)));
        }
        else
        {
            simulation = DartRuntimePrimitives.ConvertValue<Physics.Simulation>(new ClampingScrollSimulation(position: extent.currentPixels, velocity: velocity, tolerance: physics.toleranceFor(this)));
        }
        var ballisticController = AnimationController.CreateUnbounded(debugLabel: objectRuntimeTypeFunctions.objectRuntimeType(this, "_DraggableScrollableSheetPosition"), vsync: context.vsync);
        _ballisticControllers.Add(ballisticController);
        double lastPosition = extent.currentPixels;
        void tick()
        {
            double delta = ballisticController.value - lastPosition;
            lastPosition = ballisticController.value;
            extent.addPixelDelta(delta, context.notificationContext!);
            if ((velocity > 0L) && extent.isAtMax || (velocity < 0L) && extent.isAtMin)
            {
                velocity = ballisticController.velocity + physics.toleranceFor(this).velocity * Math.Sign(ballisticController.velocity);
                base.goBallistic(velocity);
                ballisticController.stop();
            }
            else
            {
                if (ballisticController.isCompleted)
                {
                    double? snapSize = _getCurrentSnapSize();
                    if (snapSize is not null)
                    {
                        double snapSize__39055__value39101 = DartRuntimePrimitives.RequireValue(snapSize);
                        extent.updateSize(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(snapSize__39055__value39101)), context.notificationContext!);
                    }
                    base.goBallistic(0);
                }
            }
        }
        DartRuntimePrimitives.Ignore(((Func<AnimationController>)(() =>
{
    var __cascade = ballisticController;
    __cascade.addListener(tick);
    __cascade.animateWith(simulation).whenCompleteOrCancel(() =>
    {
        if (_ballisticControllers.Contains(ballisticController))
        {
            _ballisticControllers.Remove(ballisticController);
            ballisticController.dispose();
        }
    });
    return __cascade;
}))());
    }

    public override Drag drag(DragStartDetails details, Action dragCancelCallback)
    {
        _dragCancelCallback = dragCancelCallback;
        return base.drag(details, () => dragCancelCallback());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DraggableScrollableActuator : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;

    public DraggableScrollableActuator(Key? key = null, Widget child = default!) : base(key: key)
    {
        this.child = child;
    }

    public static bool reset(BuildContext context)
    {
        _InheritedResetNotifier__draggable_scrollable_sheet? notifier = context.dependOnInheritedWidgetOfExactType<_InheritedResetNotifier__draggable_scrollable_sheet>();
        return notifier?._sendReset() ?? false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DraggableScrollableActuatorState__draggable_scrollable_sheet());
}

internal class _DraggableScrollableActuatorState__draggable_scrollable_sheet : State<DraggableScrollableActuator>
{
    internal virtual _ResetNotifier__draggable_scrollable_sheet _notifier { get; private set; } = new _ResetNotifier__draggable_scrollable_sheet();

    public override Widget build(BuildContext context)
    {
        return new _InheritedResetNotifier__draggable_scrollable_sheet(notifier: _notifier, child: widget.child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        _notifier.dispose();
        base.dispose();
    }

}

public class _ResetNotifier__draggable_scrollable_sheet : ChangeNotifier
{
    internal virtual bool _wasCalled { get; set; } = false;

    internal _ResetNotifier__draggable_scrollable_sheet()
    {
    }

    public virtual bool sendReset()
    {
        if (!hasListeners)
        {
            return false;
        }
        _wasCalled = true;
        notifyListeners();
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _InheritedResetNotifier__draggable_scrollable_sheet : InheritedNotifier<_ResetNotifier__draggable_scrollable_sheet>
{
    internal _InheritedResetNotifier__draggable_scrollable_sheet(Widget child, _ResetNotifier__draggable_scrollable_sheet notifier) : base(child: child, notifier: notifier)
    {
    }

    internal virtual bool _sendReset() => notifier!.sendReset();
    public static bool shouldReset(BuildContext context)
    {
        InheritedWidget? widget = context.dependOnInheritedWidgetOfExactType<_InheritedResetNotifier__draggable_scrollable_sheet>();
        if (widget is null)
        {
            return false;
        }
        DartRuntimePrimitives.Assert(() => widget is _InheritedResetNotifier__draggable_scrollable_sheet);
        var inheritedNotifier = ((_InheritedResetNotifier__draggable_scrollable_sheet?)widget)!;
        bool wasCalled = inheritedNotifier.notifier!._wasCalled;
        inheritedNotifier.notifier!._wasCalled = false;
        return wasCalled;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SnappingSimulation__draggable_scrollable_sheet : Physics.Simulation
{
    public virtual double position { get; private set; } = default!;
    public virtual double velocity { get; private set; } = default!;
    public const double minimumSpeed = 1600.0;
    internal virtual double _pixelSnapSize { get; private set; } = default!;

    internal _SnappingSimulation__draggable_scrollable_sheet(double position, double initialVelocity, List<double> pixelSnapSize, Duration? snapAnimationDuration = null, Physics.Tolerance tolerance = default!) : base(tolerance: tolerance ?? Physics.Tolerance.defaultTolerance)
    {
        this.position = position;
    }

    public override double dx(double time)
    {
        if (isDone(time))
        {
            return 0;
        }
        return velocity;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isDone(double time)
    {
        return x(time) == _pixelSnapSize;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double x(double time)
    {
        double newPosition = position + (velocity * time);
        if ((velocity >= 0L) && (newPosition > _pixelSnapSize) || (velocity < 0L) && (newPosition < _pixelSnapSize))
        {
            return _pixelSnapSize;
        }
        return newPosition;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getSnapSize(double initialVelocity, List<double> pixelSnapSizes)
    {
        long indexOfNextSize = pixelSnapSizes.indexWhere((size) => size >= position);
        if (indexOfNextSize == 0L)
        {
            return pixelSnapSizes.First();
        }
        double nextSize = pixelSnapSizes[(int)indexOfNextSize];
        if (nextSize == position)
        {
            return nextSize;
        }
        double previousSize = pixelSnapSizes[(int)(indexOfNextSize - 1L)];
        if (initialVelocity.abs() <= tolerance.velocity)
        {
            if ((position - previousSize) < (nextSize - position))
            {
                return previousSize;
            }
            else
            {
                return nextSize;
            }
        }
        if (initialVelocity < 0.0)
        {
            return pixelSnapSizes[(int)(indexOfNextSize - 1L)];
        }
        return pixelSnapSizes[(int)indexOfNextSize];
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
