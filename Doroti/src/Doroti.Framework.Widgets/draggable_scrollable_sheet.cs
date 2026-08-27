// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/draggable_scrollable_sheet.dart
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

public delegate Widget ScrollableWidgetBuilder(BuildContext context, ScrollController scrollController);

public class DraggableScrollableController : global::Doroti.Framework.Foundation.ChangeNotifier
{
    internal virtual _DraggableScrollableSheetScrollController__draggable_scrollable_sheet? _attachedController { get; set; } = default;
    internal virtual HashSet<global::Doroti.Framework.Animation.AnimationController> _animationControllers { get; private set; } = new HashSet<global::Doroti.Framework.Animation.AnimationController>();

    public DraggableScrollableController()
    {
    }

    public virtual double size
    {
        get
        {
            _assertAttached();
            return this._attachedController!.extent.currentSize;
            return default!;
        }
    }
    public virtual double pixels
    {
        get
        {
            _assertAttached();
            return this._attachedController!.extent.currentPixels;
            return default!;
        }
    }
    public virtual double sizeToPixels(double size)
    {
        _assertAttached();
        return this._attachedController!.extent.sizeToPixels(size);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isAttached => DartRuntimePrimitives.ConvertValue<bool>(((this._attachedController is not null) && this._attachedController!.hasClients));
    public virtual double pixelsToSize(double pixels)
    {
        _assertAttached();
        return this._attachedController!.extent.pixelsToSize(pixels);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future animateTo(double size, Duration duration, global::Doroti.Framework.Animation.Curve curve)
    {
        _assertAttached();
        DartRuntimePrimitives.Assert(() => ((size >= 0L) && (size <= 1L)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(duration, Duration.zero)));
        var animationController = global::Doroti.Framework.Animation.AnimationController.CreateUnbounded(vsync: this._attachedController!.position.context.vsync, value: this._attachedController!.extent.currentSize);
        this._animationControllers.Add(animationController);
        this._attachedController!.position.goIdle();
        this._attachedController!.extent.hasDragged = false;
        this._attachedController!.extent.hasChanged = true;
        this._attachedController!.extent.startActivity(onCanceled: ((global::System.Action)(() =>
        {
            if (((global::Doroti.Framework.Animation.AnimationController)animationController).isAnimating)
            {
                animationController.stop();
            }
        })));
        animationController.addListener(((global::System.Action)(() =>
        {
            this._attachedController!.extent.updateSize(((global::Doroti.Framework.Animation.AnimationController)animationController).value, this._attachedController!.position.context.notificationContext!);
        })));
        animationController.animateTo(Dart_uiLibrary.clampDouble(size, this._attachedController!.extent.minSize, this._attachedController!.extent.maxSize), duration: duration, curve: curve);
    }

    public virtual void jumpTo(double size)
    {
        _assertAttached();
        DartRuntimePrimitives.Assert(() => ((size >= 0L) && (size <= 1L)));
        this._attachedController!.extent.startActivity(onCanceled: ((global::System.Action)(() =>
        {
        })));
        this._attachedController!.position.goIdle();
        this._attachedController!.extent.hasDragged = false;
        this._attachedController!.extent.hasChanged = true;
        this._attachedController!.extent.updateSize(size, this._attachedController!.position.context.notificationContext!);
    }

    public virtual void reset()
    {
        _assertAttached();
        this._attachedController!.reset();
    }

    internal virtual void _assertAttached()
    {
        DartRuntimePrimitives.Assert(() => this.isAttached, () => (object?)"DraggableScrollableController is not attached to a sheet. A DraggableScrollableController " + "must be used in a DraggableScrollableSheet before any of its methods are called.");
    }

    internal virtual void _attach(_DraggableScrollableSheetScrollController__draggable_scrollable_sheet scrollController)
    {
        DartRuntimePrimitives.Assert(() => (this._attachedController is null), () => (object?)"Draggable scrollable controller is already attached to a sheet.");
        _attachedController = scrollController;
        this._attachedController!.extent._currentSize.addListener(this.notifyListeners);
        this._attachedController!.onPositionDetached = (global::System.Action)this._disposeAnimationControllers;
    }

    internal virtual void _onExtentReplaced(_DraggableSheetExtent__draggable_scrollable_sheet previousExtent)
    {
        this._attachedController!.extent._currentSize.addListener(this.notifyListeners);
        if ((((_DraggableSheetExtent__draggable_scrollable_sheet)previousExtent).currentSize != this._attachedController!.extent.currentSize))
        {
            notifyListeners();
        }
    }

    internal virtual void _detach(bool disposeExtent = false)
    {
        if (disposeExtent)
        {
            this._attachedController?.extent.dispose();
        }
        else
        {
            this._attachedController?.extent._currentSize.removeListener(this.notifyListeners);
        }
        _disposeAnimationControllers();
        _attachedController = null;
    }

    internal virtual void _disposeAnimationControllers()
    {
        foreach (global::Doroti.Framework.Animation.AnimationController animationController in this._animationControllers)
        {
            animationController.dispose();
        }
        this._animationControllers.Clear();
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
    public virtual global::System.Func<BuildContext, ScrollController, Widget> builder { get; private set; } = default!;

    public DraggableScrollableSheet(global::Doroti.Framework.Foundation.Key? key = null, double initialChildSize = 0.5, double minChildSize = 0.25, double maxChildSize = 1.0, bool expand = true, bool snap = false, List<double>? snapSizes = null, Duration? snapAnimationDuration = null, DraggableScrollableController? controller = null, bool shouldCloseOnMinExtent = true, global::System.Func<BuildContext, ScrollController, Widget> builder = default!) : base(key: key)
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
        System.Diagnostics.Debug.Assert((minChildSize >= 0.0));
        System.Diagnostics.Debug.Assert((maxChildSize <= 1.0));
        System.Diagnostics.Debug.Assert((minChildSize <= initialChildSize));
        System.Diagnostics.Debug.Assert((initialChildSize <= maxChildSize));
        System.Diagnostics.Debug.Assert(((snapAnimationDuration is null) || (DartRuntimePrimitives.RequireValue(snapAnimationDuration) > Duration.zero)));
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
        System.Diagnostics.Debug.Assert((0.0 <= minExtent));
        System.Diagnostics.Debug.Assert((maxExtent <= 1.0));
        System.Diagnostics.Debug.Assert((minExtent <= extent));
        System.Diagnostics.Debug.Assert((minExtent <= initialExtent));
        System.Diagnostics.Debug.Assert((extent <= maxExtent));
        System.Diagnostics.Debug.Assert((initialExtent <= maxExtent));
    }

    public override void debugFillDescription(List<string> description)
    {
        base.debugFillDescription(description);
        description.Add($"depth: {this.depth} ({((this.depth == 0L) ? "local" : "remote")})");
        description.Add($"minExtent: {this.minExtent}, extent: {this.extent}, maxExtent: {this.maxExtent}, initialExtent: {this.initialExtent}");
    }

    public virtual long depth => this._depth;
}

public class _DraggableSheetExtent__draggable_scrollable_sheet
{
    internal virtual global::System.Action? _cancelActivity { get; set; } = default;
    public virtual double minSize { get; private set; } = default!;
    public virtual double maxSize { get; private set; } = default!;
    public virtual bool snap { get; private set; } = default!;
    public virtual List<double> snapSizes { get; private set; } = default!;
    public virtual Duration? snapAnimationDuration { get; private set; }
    public virtual double initialSize { get; private set; } = default!;
    public virtual bool shouldCloseOnMinExtent { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Foundation.ValueNotifier<double> _currentSize { get; private set; } = default!;
    public virtual double availablePixels { get; set; } = default!;
    public virtual bool hasDragged { get; set; } = default!;
    public virtual bool hasChanged { get; set; } = default!;

    internal _DraggableSheetExtent__draggable_scrollable_sheet(double minSize, double maxSize, bool snap, List<double> snapSizes, double initialSize, Duration? snapAnimationDuration = null, global::Doroti.Framework.Foundation.ValueNotifier<double>? currentSize = null, bool? hasDragged = null, bool? hasChanged = null, bool shouldCloseOnMinExtent = true)
    {
        this.minSize = minSize;
        this.maxSize = maxSize;
        this.snap = snap;
        this.snapSizes = snapSizes;
        this.initialSize = initialSize;
        this.snapAnimationDuration = snapAnimationDuration;
        this.shouldCloseOnMinExtent = shouldCloseOnMinExtent;
        this._currentSize = (currentSize ?? new global::Doroti.Framework.Foundation.ValueNotifier<double>(initialSize));
        this.availablePixels = double.PositiveInfinity;
        this.hasDragged = (hasDragged ?? false);
        this.hasChanged = (hasChanged ?? false);
        System.Diagnostics.Debug.Assert((minSize >= 0L));
        System.Diagnostics.Debug.Assert((maxSize <= 1L));
        System.Diagnostics.Debug.Assert((minSize <= initialSize));
        System.Diagnostics.Debug.Assert((initialSize <= maxSize));
    }

    public virtual bool isAtMin => DartRuntimePrimitives.ConvertValue<bool>((this.minSize >= ((global::Doroti.Framework.Foundation.ValueNotifier<double>)this._currentSize).value));
    public virtual bool isAtMax => DartRuntimePrimitives.ConvertValue<bool>((this.maxSize <= ((global::Doroti.Framework.Foundation.ValueNotifier<double>)this._currentSize).value));
    public virtual double currentSize => ((global::Doroti.Framework.Foundation.ValueNotifier<double>)this._currentSize).value;
    public virtual double currentPixels => sizeToPixels(((global::Doroti.Framework.Foundation.ValueNotifier<double>)this._currentSize).value);
    public virtual List<double> pixelSnapSizes => this.snapSizes.map<double, double>(this.sizeToPixels).ToList();
    public virtual void startActivity(global::System.Action onCanceled)
    {
        this._cancelActivity?.Invoke();
        _cancelActivity = (global::System.Action)onCanceled;
    }

    public virtual void addPixelDelta(double delta, BuildContext context)
    {
        this._cancelActivity?.Invoke();
        _cancelActivity = null;
        hasDragged = true;
        hasChanged = true;
        if ((this.availablePixels == 0L))
        {
            return;
        }
        updateSize((this.currentSize + pixelsToSize(delta)), context);
    }

    public virtual void updateSize(double newSize, BuildContext context)
    {
        double clampedSize = Dart_uiLibrary.clampDouble(newSize, this.minSize, this.maxSize);
        if ((((global::Doroti.Framework.Foundation.ValueNotifier<double>)this._currentSize).value == clampedSize))
        {
            return;
        }
        this._currentSize.value = clampedSize;
        new DraggableScrollableNotification(minExtent: this.minSize, maxExtent: this.maxSize, extent: DartRuntimePrimitives.RequireValue(this.currentSize), initialExtent: this.initialSize, context: context, shouldCloseOnMinExtent: this.shouldCloseOnMinExtent).dispatch(context);
    }

    public virtual double pixelsToSize(double pixels)
    {
        return ((pixels / this.availablePixels) * this.maxSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double sizeToPixels(double size)
    {
        return ((size / this.maxSize) * this.availablePixels);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        this._currentSize.dispose();
    }

    public virtual _DraggableSheetExtent__draggable_scrollable_sheet copyWith(double minSize, double maxSize, bool snap, List<double> snapSizes, double initialSize, Duration? snapAnimationDuration, bool shouldCloseOnMinExtent)
    {
        return new _DraggableSheetExtent__draggable_scrollable_sheet(minSize: minSize, maxSize: maxSize, snap: snap, snapSizes: snapSizes, snapAnimationDuration: snapAnimationDuration, initialSize: initialSize, currentSize: new global::Doroti.Framework.Foundation.ValueNotifier<double>((this.hasChanged ? Dart_uiLibrary.clampDouble(((global::Doroti.Framework.Foundation.ValueNotifier<double>)this._currentSize).value, minSize, maxSize) : initialSize)), hasDragged: this.hasDragged, hasChanged: this.hasChanged, shouldCloseOnMinExtent: shouldCloseOnMinExtent);
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
        _extent = new _DraggableSheetExtent__draggable_scrollable_sheet(minSize: ((DraggableScrollableSheet)this.widget).minChildSize, maxSize: ((DraggableScrollableSheet)this.widget).maxChildSize, snap: ((DraggableScrollableSheet)this.widget).snap, snapSizes: _impliedSnapSizes(), snapAnimationDuration: ((DraggableScrollableSheet)this.widget).snapAnimationDuration, initialSize: ((DraggableScrollableSheet)this.widget).initialChildSize, shouldCloseOnMinExtent: ((DraggableScrollableSheet)this.widget).shouldCloseOnMinExtent);
        _scrollController = new _DraggableScrollableSheetScrollController__draggable_scrollable_sheet(extent: this._extent);
        ((DraggableScrollableSheet)this.widget).controller?._attach(this._scrollController);
    }

    internal virtual List<double> _impliedSnapSizes()
    {
        for (var index = 0L; (index < ((((long?)(((DraggableScrollableSheet)this.widget).snapSizes?.Count)) ?? 0L))); index += 1L)
        {
            double snapSize = ((DraggableScrollableSheet)this.widget).snapSizes![(int)(index)];
            DartRuntimePrimitives.Assert(() => ((snapSize >= ((DraggableScrollableSheet)this.widget).minChildSize) && (snapSize <= ((DraggableScrollableSheet)this.widget).maxChildSize)), () => (object?)$"{_snapSizeErrorMessage(index)}\nSnap sizes must be between `minChildSize` and `maxChildSize`. ");
            DartRuntimePrimitives.Assert(() => ((index == 0L) || (snapSize > ((DraggableScrollableSheet)this.widget).snapSizes![(int)((index - 1L))])), () => (object?)$"{_snapSizeErrorMessage(index)}\nSnap sizes must be in ascending order. ");
        }
        if (((((DraggableScrollableSheet)this.widget).snapSizes is null) || !System.Linq.Enumerable.Any(((DraggableScrollableSheet)this.widget).snapSizes!)))
        {
            return new List<double> { ((DraggableScrollableSheet)this.widget).minChildSize, ((DraggableScrollableSheet)this.widget).maxChildSize };
        }
        return new List<double>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void didUpdateWidget(DraggableScrollableSheet oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((DraggableScrollableSheet)this.widget).controller, ((DraggableScrollableSheet)oldWidget).controller)))
        {
            ((DraggableScrollableSheet)oldWidget).controller?._detach();
            ((DraggableScrollableSheet)this.widget).controller?._attach(this._scrollController);
        }
        _replaceExtent(oldWidget);
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        if (_InheritedResetNotifier__draggable_scrollable_sheet.shouldReset(this.context))
        {
            this._scrollController.reset();
        }
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new ValueListenableBuilder<double>(valueListenable: ((_DraggableSheetExtent__draggable_scrollable_sheet)this._extent)._currentSize, builder: ((global::System.Func<BuildContext, double, Widget?, Widget>)((context, currentSize, child) => new LayoutBuilder(builder: ((global::System.Func<BuildContext, global::Doroti.Framework.Rendering.BoxConstraints, Widget>)((context, constraints) =>
        {
            this._extent.availablePixels = (((DraggableScrollableSheet)this.widget).maxChildSize * ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).biggest.height);
            Widget sheet = ((Widget)(object?)new FractionallySizedBox(heightFactor: currentSize, alignment: global::Doroti.Framework.Painting.Alignment.bottomCenter, child: child));
            return (((DraggableScrollableSheet)this.widget).expand ? SizedBox.CreateExpand(child: sheet) : sheet);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))))), child: this.widget.builder(context, this._scrollController)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        if ((((DraggableScrollableSheet)this.widget).controller is null))
        {
            this._extent.dispose();
        }
        else
        {
            ((DraggableScrollableSheet)this.widget).controller!._detach(disposeExtent: true);
        }
        this._scrollController.dispose();
        base.dispose();
    }

    internal virtual void _replaceExtent(DraggableScrollableSheet oldWidget)
    {
        _DraggableSheetExtent__draggable_scrollable_sheet previousExtent = this._extent;
        _extent = previousExtent.copyWith(minSize: ((DraggableScrollableSheet)this.widget).minChildSize, maxSize: ((DraggableScrollableSheet)this.widget).maxChildSize, snap: ((DraggableScrollableSheet)this.widget).snap, snapSizes: _impliedSnapSizes(), snapAnimationDuration: ((DraggableScrollableSheet)this.widget).snapAnimationDuration, initialSize: ((DraggableScrollableSheet)this.widget).initialChildSize, shouldCloseOnMinExtent: ((DraggableScrollableSheet)this.widget).shouldCloseOnMinExtent);
        this._scrollController.extent = this._extent;
        ((DraggableScrollableSheet)this.widget).controller?._onExtentReplaced(previousExtent);
        previousExtent.dispose();
        if (((((DraggableScrollableSheet)this.widget).snap && (((((DraggableScrollableSheet)this.widget).snap != ((DraggableScrollableSheet)oldWidget).snap) || (!object.Equals(((DraggableScrollableSheet)this.widget).snapSizes, ((DraggableScrollableSheet)oldWidget).snapSizes))))) && this._scrollController.hasClients))
        {
            WidgetsBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timeStamp) =>
            {
                for (var index = 0L; (index < this._scrollController.positions.Count()); index++)
                {
                    var position = ((_DraggableScrollableSheetScrollPosition__draggable_scrollable_sheet?)(object?)this._scrollController.positions.elementAt(index))!;
                    position.goBallistic(0);
                }
            })), debugLabel: "DraggableScrollableSheet.snap");
        }
    }

    internal virtual string _snapSizeErrorMessage(long invalidIndex)
    {
        List<string> snapSizesWithIndicator = ((DraggableScrollableSheet)this.widget).snapSizes!.asMap().Keys.map<long, string>(((index) =>
        {
            var snapSizeString = ((DraggableScrollableSheet)this.widget).snapSizes![(int)(index)].ToString();
            if ((index == invalidIndex))
            {
                return $">>> {snapSizeString} <<<";
            }
            return snapSizeString;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })).ToList().ToList();
        return $"Invalid snapSize '{((DraggableScrollableSheet)this.widget).snapSizes![(int)(invalidIndex)]}' at index {invalidIndex} of:\n" + $"  {snapSizesWithIndicator}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DraggableScrollableSheetScrollController__draggable_scrollable_sheet : ScrollController
{
    public virtual _DraggableSheetExtent__draggable_scrollable_sheet extent { get; set; } = default!;
    public virtual global::System.Action? onPositionDetached { get; set; } = default;

    internal _DraggableScrollableSheetScrollController__draggable_scrollable_sheet(_DraggableSheetExtent__draggable_scrollable_sheet extent)
    {
        this.extent = extent;
    }

    public override _DraggableScrollableSheetScrollPosition__draggable_scrollable_sheet createScrollPosition(ScrollPhysics physics, ScrollContext context, ScrollPosition? oldPosition)
    {
        return new _DraggableScrollableSheetScrollPosition__draggable_scrollable_sheet(physics: physics.applyTo(new AlwaysScrollableScrollPhysics()), context: context, oldPosition: oldPosition, getExtent: ((global::System.Func<_DraggableSheetExtent__draggable_scrollable_sheet>)(() => this.extent)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillDescription(List<string> description)
    {
        base.debugFillDescription(description);
        description.Add($"extent: {this.extent}");
    }

    public override _DraggableScrollableSheetScrollPosition__draggable_scrollable_sheet position => ((_DraggableScrollableSheetScrollPosition__draggable_scrollable_sheet?)(object?)base.position)!;
    public virtual void reset()
    {
        ((_DraggableSheetExtent__draggable_scrollable_sheet)this.extent)._cancelActivity?.Invoke();
        this.extent.hasDragged = false;
        this.extent.hasChanged = false;
        if ((this.offset != 0.0))
        {
            DartRuntimePrimitives.Ignore(animateTo(0.0, duration: Duration.Create(milliseconds: 1L), curve: global::Doroti.Framework.Animation.Curves.linear));
        }
        this.extent.updateSize(((_DraggableSheetExtent__draggable_scrollable_sheet)this.extent).initialSize, this.position.context.notificationContext!);
    }

    public override void detach(ScrollPosition position)
    {
        this.onPositionDetached?.Invoke();
        base.detach(position);
    }

}

public class _DraggableScrollableSheetScrollPosition__draggable_scrollable_sheet : ScrollPositionWithSingleContext
{
    internal virtual global::System.Action? _dragCancelCallback { get; set; } = default;
    public virtual global::System.Func<_DraggableSheetExtent__draggable_scrollable_sheet> getExtent { get; private set; } = default!;
    internal virtual HashSet<global::Doroti.Framework.Animation.AnimationController> _ballisticControllers { get; private set; } = new HashSet<global::Doroti.Framework.Animation.AnimationController>();

    internal _DraggableScrollableSheetScrollPosition__draggable_scrollable_sheet(ScrollPhysics physics, ScrollContext context, ScrollPosition? oldPosition = null, global::System.Func<_DraggableSheetExtent__draggable_scrollable_sheet> getExtent = default!) : base(physics: physics, context: context, oldPosition: oldPosition)
    {
        this.getExtent = getExtent;
    }

    public virtual bool listShouldScroll => DartRuntimePrimitives.ConvertValue<bool>((this.pixels > 0.0));
    public virtual _DraggableSheetExtent__draggable_scrollable_sheet extent => this.getExtent();
    public override void absorb(ScrollPosition other)
    {
        base.absorb(other);
        DartRuntimePrimitives.Assert(() => (this._dragCancelCallback is null));
        if ((other is not _DraggableScrollableSheetScrollPosition__draggable_scrollable_sheet))
        {
            return;
        }
        if ((((_DraggableScrollableSheetScrollPosition__draggable_scrollable_sheet)((_DraggableScrollableSheetScrollPosition__draggable_scrollable_sheet)other))._dragCancelCallback is not null))
        {
            _dragCancelCallback = (global::System.Action)((_DraggableScrollableSheetScrollPosition__draggable_scrollable_sheet)((_DraggableScrollableSheetScrollPosition__draggable_scrollable_sheet)other))._dragCancelCallback;
            ((dynamic)other)._dragCancelCallback = null;
        }
    }

    public override void beginActivity(ScrollActivity? newActivity)
    {
        foreach (global::Doroti.Framework.Animation.AnimationController ballisticController in this._ballisticControllers)
        {
            ballisticController.stop();
        }
        base.beginActivity(newActivity);
    }

    public override void applyUserOffset(double delta)
    {
        if ((!this.listShouldScroll && (((!((((_DraggableSheetExtent__draggable_scrollable_sheet)this.extent).isAtMin || ((_DraggableSheetExtent__draggable_scrollable_sheet)this.extent).isAtMax)) || ((((_DraggableSheetExtent__draggable_scrollable_sheet)this.extent).isAtMin && (delta < 0L)))) || ((((_DraggableSheetExtent__draggable_scrollable_sheet)this.extent).isAtMax && (delta > 0L)))))))
        {
            this.extent.addPixelDelta(-delta, ((ScrollContext)this.context).notificationContext!);
        }
        else
        {
            base.applyUserOffset(delta);
        }
    }

    internal virtual double? _getCurrentSnapSize()
    {
        return ((_DraggableSheetExtent__draggable_scrollable_sheet)this.extent).snapSizes.firstWhereOrNull(((snapSize) =>
        {
            return (((((_DraggableSheetExtent__draggable_scrollable_sheet)this.extent).currentSize - snapSize)).abs() <= this.extent.pixelsToSize(this.physics.toleranceFor(this).distance));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isAtSnapSize() => DartRuntimePrimitives.ConvertValue<bool>((_getCurrentSnapSize() is not null));
    internal virtual bool _shouldSnap() => DartRuntimePrimitives.ConvertValue<bool>(((((_DraggableSheetExtent__draggable_scrollable_sheet)this.extent).snap && ((_DraggableSheetExtent__draggable_scrollable_sheet)this.extent).hasDragged) && !_isAtSnapSize()));
    public override void dispose()
    {
        foreach (global::Doroti.Framework.Animation.AnimationController ballisticController in this._ballisticControllers)
        {
            ballisticController.dispose();
        }
        this._ballisticControllers.Clear();
        base.dispose();
    }

    public override void goBallistic(double velocity)
    {
        if ((((((velocity == 0.0) && !_shouldSnap())) || (((velocity < 0.0) && this.listShouldScroll))) || (((velocity > 0.0) && ((_DraggableSheetExtent__draggable_scrollable_sheet)this.extent).isAtMax))))
        {
            base.goBallistic(velocity);
            return;
        }
        this._dragCancelCallback?.Invoke();
        _dragCancelCallback = null;
        global::Doroti.Framework.Physics.Simulation simulation = default!;
        if (((_DraggableSheetExtent__draggable_scrollable_sheet)this.extent).snap)
        {
            simulation = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Physics.Simulation>(new _SnappingSimulation__draggable_scrollable_sheet(position: ((_DraggableSheetExtent__draggable_scrollable_sheet)this.extent).currentPixels, initialVelocity: velocity, pixelSnapSize: ((_DraggableSheetExtent__draggable_scrollable_sheet)this.extent).pixelSnapSizes, snapAnimationDuration: ((_DraggableSheetExtent__draggable_scrollable_sheet)this.extent).snapAnimationDuration, tolerance: this.physics.toleranceFor(this)));
        }
        else
        {
            simulation = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Physics.Simulation>(new ClampingScrollSimulation(position: ((_DraggableSheetExtent__draggable_scrollable_sheet)this.extent).currentPixels, velocity: velocity, tolerance: this.physics.toleranceFor(this)));
        }
        var ballisticController = global::Doroti.Framework.Animation.AnimationController.CreateUnbounded(debugLabel: global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "_DraggableScrollableSheetPosition"), vsync: ((ScrollContext)this.context).vsync);
        this._ballisticControllers.Add(ballisticController);
        double lastPosition = ((_DraggableSheetExtent__draggable_scrollable_sheet)this.extent).currentPixels;
        void tick()
        {
            double delta = (((global::Doroti.Framework.Animation.AnimationController)ballisticController).value - lastPosition);
            lastPosition = ((global::Doroti.Framework.Animation.AnimationController)ballisticController).value;
            this.extent.addPixelDelta(delta, ((ScrollContext)this.context).notificationContext!);
            if (((((velocity > 0L) && ((_DraggableSheetExtent__draggable_scrollable_sheet)this.extent).isAtMax)) || (((velocity < 0L) && ((_DraggableSheetExtent__draggable_scrollable_sheet)this.extent).isAtMin))))
            {
                velocity = (((global::Doroti.Framework.Animation.AnimationController)ballisticController).velocity + ((this.physics.toleranceFor(this).velocity * Math.Sign(((global::Doroti.Framework.Animation.AnimationController)ballisticController).velocity))));
                base.goBallistic(velocity);
                ballisticController.stop();
            }
            else
            {
                if (ballisticController.isCompleted)
                {
                    double? snapSize = _getCurrentSnapSize();
                    if ((snapSize is not null))
                    {
                        double snapSize__39055__value39101 = DartRuntimePrimitives.RequireValue(snapSize);
                        this.extent.updateSize(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(snapSize__39055__value39101)), ((ScrollContext)this.context).notificationContext!);
                    }
                    base.goBallistic(0);
                }
            }
        }
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = ballisticController;
    __cascade.addListener(() => tick());
    __cascade.animateWith(simulation).whenCompleteOrCancel(((global::System.Action)(() =>
    {
        if (this._ballisticControllers.Contains(ballisticController))
        {
            this._ballisticControllers.Remove(ballisticController);
            ballisticController.dispose();
        }
    })));
    return __cascade;
}))());
    }

    public override global::Doroti.Framework.Gestures.Drag drag(global::Doroti.Framework.Gestures.DragStartDetails details, global::System.Action dragCancelCallback)
    {
        _dragCancelCallback = (global::System.Action)dragCancelCallback;
        return ((global::Doroti.Framework.Gestures.Drag)(object?)base.drag(details, () => dragCancelCallback()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DraggableScrollableActuator : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;

    public DraggableScrollableActuator(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!) : base(key: key)
    {
        this.child = child;
    }

    public static bool reset(BuildContext context)
    {
        _InheritedResetNotifier__draggable_scrollable_sheet? notifier = ((_InheritedResetNotifier__draggable_scrollable_sheet?)(object?)context.dependOnInheritedWidgetOfExactType<_InheritedResetNotifier__draggable_scrollable_sheet>());
        return (notifier?._sendReset() ?? false);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DraggableScrollableActuatorState__draggable_scrollable_sheet());
}

internal class _DraggableScrollableActuatorState__draggable_scrollable_sheet : State<DraggableScrollableActuator>
{
    internal virtual _ResetNotifier__draggable_scrollable_sheet _notifier { get; private set; } = new _ResetNotifier__draggable_scrollable_sheet();

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new _InheritedResetNotifier__draggable_scrollable_sheet(notifier: this._notifier, child: ((DraggableScrollableActuator)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        this._notifier.dispose();
        base.dispose();
    }

}

public class _ResetNotifier__draggable_scrollable_sheet : global::Doroti.Framework.Foundation.ChangeNotifier
{
    internal virtual bool _wasCalled { get; set; } = false;

    internal _ResetNotifier__draggable_scrollable_sheet()
    {
    }

    public virtual bool sendReset()
    {
        if (!this.hasListeners)
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

    internal virtual bool _sendReset() => this.notifier!.sendReset();
    public static bool shouldReset(BuildContext context)
    {
        InheritedWidget? widget = ((InheritedWidget?)(object?)context.dependOnInheritedWidgetOfExactType<_InheritedResetNotifier__draggable_scrollable_sheet>());
        if ((widget is null))
        {
            return false;
        }
        DartRuntimePrimitives.Assert(() => (widget is _InheritedResetNotifier__draggable_scrollable_sheet));
        var inheritedNotifier = ((_InheritedResetNotifier__draggable_scrollable_sheet?)(object?)widget)!;
        bool wasCalled = inheritedNotifier.notifier!._wasCalled;
        inheritedNotifier.notifier!._wasCalled = false;
        return wasCalled;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SnappingSimulation__draggable_scrollable_sheet : global::Doroti.Framework.Physics.Simulation
{
    public virtual double position { get; private set; } = default!;
    public virtual double velocity { get; private set; } = default!;
    public const double minimumSpeed = 1600.0;
    internal virtual double _pixelSnapSize { get; private set; } = default!;

    internal _SnappingSimulation__draggable_scrollable_sheet(double position, double initialVelocity, List<double> pixelSnapSize, Duration? snapAnimationDuration = null, global::Doroti.Framework.Physics.Tolerance tolerance = default!) : base(tolerance: tolerance ?? global::Doroti.Framework.Physics.Tolerance.defaultTolerance)
    {
        this.position = position;
    }

    public override double dx(double time)
    {
        if (isDone(time))
        {
            return 0;
        }
        return this.velocity;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isDone(double time)
    {
        return (x(time) == this._pixelSnapSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double x(double time)
    {
        double newPosition = (this.position + (this.velocity * time));
        if (((((this.velocity >= 0L) && (newPosition > this._pixelSnapSize))) || (((this.velocity < 0L) && (newPosition < this._pixelSnapSize)))))
        {
            return this._pixelSnapSize;
        }
        return newPosition;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getSnapSize(double initialVelocity, List<double> pixelSnapSizes)
    {
        long indexOfNextSize = pixelSnapSizes.indexWhere(((size) => (size >= this.position)));
        if ((indexOfNextSize == 0L))
        {
            return pixelSnapSizes.First();
        }
        double nextSize = pixelSnapSizes[(int)(indexOfNextSize)];
        if ((nextSize == this.position))
        {
            return nextSize;
        }
        double previousSize = pixelSnapSizes[(int)((indexOfNextSize - 1L))];
        if ((initialVelocity.abs() <= ((global::Doroti.Framework.Physics.Tolerance)this.tolerance).velocity))
        {
            if (((this.position - previousSize) < (nextSize - this.position)))
            {
                return previousSize;
            }
            else
            {
                return nextSize;
            }
        }
        if ((initialVelocity < 0.0))
        {
            return pixelSnapSizes[(int)((indexOfNextSize - 1L))];
        }
        return pixelSnapSizes[(int)(indexOfNextSize)];
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
