// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scroll_position.dart
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

public enum ScrollPositionAlignmentPolicy
{
    @explicit,
    keepVisibleAtEnd,
    keepVisibleAtStart
}

public abstract class ScrollPosition : global::Doroti.Framework.Rendering.ViewportOffset, ScrollMetrics
{
    public virtual ScrollPhysics physics { get; private set; } = default!;
    public virtual ScrollContext context { get; private set; } = default!;
    public virtual bool keepScrollOffset { get; private set; } = default!;
    public virtual string? debugLabel { get; private set; }
    internal virtual double? _minScrollExtent { get; set; } = default;
    internal virtual double? _maxScrollExtent { get; set; } = default;
    internal virtual double _impliedVelocity { get; set; } = 0;
    internal virtual double? _pixels { get; set; } = default;
    internal virtual double? _viewportDimension { get; set; } = default;
    internal virtual bool _haveDimensions { get; set; } = false;
    internal virtual bool _didChangeViewportDimensionOrReceiveCorrection { get; set; } = true;
    internal virtual bool _pendingDimensions { get; set; } = false;
    internal virtual ScrollMetrics? _lastMetrics { get; set; } = default;
    internal virtual bool _haveScheduledUpdateNotification { get; set; } = false;
    internal virtual global::Doroti.Framework.Painting.Axis? _lastAxis { get; set; } = default;
    internal virtual HashSet<SemanticsAction>? _semanticActions { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueNotifier<bool> isScrollingNotifier { get; private set; } = new global::Doroti.Framework.Foundation.ValueNotifier<bool>(false);
    internal virtual ScrollActivity? _activity { get; set; } = default;

    protected ScrollPosition(ScrollPhysics physics, ScrollContext context, bool keepScrollOffset = true, ScrollPosition? oldPosition = null, string? debugLabel = null)
    {
        this.physics = physics;
        this.context = context;
        this.keepScrollOffset = keepScrollOffset;
        this.debugLabel = debugLabel;
        if (oldPosition is not null)
        {
            absorb(oldPosition);
        }
        if (keepScrollOffset)
        {
            restoreScrollOffset();
        }
    }

    public virtual double minScrollExtent => DartRuntimePrimitives.ConvertValue<double>(DartRuntimePrimitives.RequireValue(this._minScrollExtent));
    public virtual double maxScrollExtent => DartRuntimePrimitives.ConvertValue<double>(DartRuntimePrimitives.RequireValue(this._maxScrollExtent));
    public virtual bool hasContentDimensions => DartRuntimePrimitives.ConvertValue<bool>(((this._minScrollExtent is not null) && (this._maxScrollExtent is not null)));
    public override double pixels => DartRuntimePrimitives.ConvertValue<double>(DartRuntimePrimitives.RequireValue(this._pixels));
    public override bool hasPixels => DartRuntimePrimitives.ConvertValue<bool>((this._pixels is not null));
    public virtual double viewportDimension => DartRuntimePrimitives.ConvertValue<double>(DartRuntimePrimitives.RequireValue(this._viewportDimension));
    public virtual bool hasViewportDimension => DartRuntimePrimitives.ConvertValue<bool>((this._viewportDimension is not null));
    public virtual bool haveDimensions => this._haveDimensions;
    public virtual bool shouldIgnorePointer => DartRuntimePrimitives.ConvertValue<bool>((!this.outOfRange && ((this.activity?.shouldIgnorePointer ?? true))));
    public virtual void absorb(ScrollPosition other)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((ScrollPosition)other).context, this.context)));
        DartRuntimePrimitives.Assert(() => (this._pixels is null));
        if (((ScrollPosition)other).hasContentDimensions)
        {
            _minScrollExtent = ((ScrollPosition)other).minScrollExtent;
            _maxScrollExtent = ((ScrollPosition)other).maxScrollExtent;
        }
        if (((ScrollPosition)other).hasPixels)
        {
            _pixels = ((ScrollPosition)other).pixels;
        }
        if (((ScrollPosition)other).hasViewportDimension)
        {
            _viewportDimension = ((ScrollPosition)other).viewportDimension;
        }
        DartRuntimePrimitives.Assert(() => (this.activity is null));
        DartRuntimePrimitives.Assert(() => (((ScrollPosition)other).activity is not null));
        _activity = ((ScrollPosition)other).activity;
        other._activity = null;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(other), this.GetType())))
        {
            this.activity!.resetActivity();
        }
        this.context.setIgnorePointer(this.activity!.shouldIgnorePointer);
        this.isScrollingNotifier.value = this.activity!.isScrolling;
    }

    public virtual double devicePixelRatio => ((ScrollContext)this.context).devicePixelRatio;
    public virtual double setPixels(double newPixels)
    {
        DartRuntimePrimitives.Assert(() => this.hasPixels);
        DartRuntimePrimitives.Assert(() => (!object.Equals(global::Doroti.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase, global::Doroti.Framework.Scheduler.SchedulerPhase.persistentCallbacks)), () => (object?)"A scrollable's position should not change during the build, layout, and paint phases, otherwise the rendering will be confused.");
        if ((newPixels != this.pixels))
        {
            double overscroll__16015 = applyBoundaryConditions(newPixels);
            DartRuntimePrimitives.Assert(() =>
                {
                    double delta__16103 = (newPixels - this.pixels);
                    if ((overscroll__16015.abs() > delta__16103.abs()))
                    {
                        throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create($"{this.GetType()}.applyBoundaryConditions returned invalid overscroll value.\n" + $"setPixels() was called to change the scroll offset from {this.pixels} to {newPixels}.\n" + $"That is a delta of {delta__16103} units.\n" + $"{this.GetType()}.applyBoundaryConditions reported an overscroll of {overscroll__16015} units."));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            double oldPixels__16611 = this.pixels;
            _pixels = (newPixels - overscroll__16015);
            if ((this._pixels != oldPixels__16611))
            {
                if (this.outOfRange)
                {
                    this.context.setIgnorePointer(false);
                }
                notifyListeners();
                didUpdateScrollPositionBy((this.pixels - oldPixels__16611));
            }
            if ((overscroll__16015.abs() > global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
            {
                didOverscrollBy(overscroll__16015);
                return overscroll__16015;
            }
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void correctPixels(double value)
    {
        _pixels = value;
    }

    public override void correctBy(double correction)
    {
        DartRuntimePrimitives.Assert(() => this.hasPixels, () => (object?)"An initial pixels value must exist by calling correctPixels on the ScrollPosition");
        _pixels = (DartRuntimePrimitives.RequireValue(this._pixels) + correction);
        _didChangeViewportDimensionOrReceiveCorrection = true;
    }

    public virtual void forcePixels(double value)
    {
        DartRuntimePrimitives.Assert(() => this.hasPixels);
        _impliedVelocity = (value - this.pixels);
        _pixels = value;
        notifyListeners();
        global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timeStamp) => {
_impliedVelocity = 0;
})), debugLabel: "ScrollPosition.resetVelocity");
    }

    public virtual void saveScrollOffset()
    {
        PageStorage.maybeOf(((ScrollContext)this.context).storageContext)?.writeState(((ScrollContext)this.context).storageContext, this.pixels);
    }

    public virtual void restoreScrollOffset()
    {
        if (!this.hasPixels)
        {
            var value__23743 = ((double?)PageStorage.maybeOf(((ScrollContext)this.context).storageContext)?.readState(((ScrollContext)this.context).storageContext));
            if ((value__23743 is not null))
            {
                double value__23743__value23862 = DartRuntimePrimitives.RequireValue(value__23743);
                correctPixels(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(value__23743__value23862)));
            }
        }
    }

    public virtual void restoreOffset(double offset, bool initialRestore = false)
    {
        if (initialRestore)
        {
            correctPixels(offset);
        }
        else
        {
            jumpTo(offset);
        }
    }

    public virtual void saveOffset()
    {
        DartRuntimePrimitives.Assert(() => this.hasPixels);
        this.context.saveOffset(this.pixels);
    }

    public virtual double applyBoundaryConditions(double value)
    {
        double result__25844 = this.physics.applyBoundaryConditions(this, value);
        DartRuntimePrimitives.Assert(() =>
            {
                double delta__25934 = (value - this.pixels);
                if ((result__25844.abs() > delta__25934.abs()))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create($"{DartRuntimePrimitives.RuntimeType(this.physics)}.applyBoundaryConditions returned invalid overscroll value.\n" + $"The method was called to consider a change from {this.pixels} to {value}, which is a " + $"delta of {delta__25934.toStringAsFixed(1L)} units. However, it returned an overscroll of " + $"{result__25844.toStringAsFixed(1L)} units, which has a greater magnitude than the delta. " + "The applyBoundaryConditions method is only supposed to reduce the possible range " + "of movement, not increase it.\n" + $"The scroll extents are {this.minScrollExtent} .. {this.maxScrollExtent}, and the " + $"viewport dimension is {this.viewportDimension}."));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result__25844;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool applyViewportDimension(double viewportDimension)
    {
        if ((this._viewportDimension != viewportDimension))
        {
            _viewportDimension = viewportDimension;
            _didChangeViewportDimensionOrReceiveCorrection = true;
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isMetricsChanged()
    {
        DartRuntimePrimitives.Assert(() => this.haveDimensions);
        ScrollMetrics currentMetrics__27599 = ((ScrollMetrics)(object?)copyWith());
        return ((this._lastMetrics is null) || !(((((((ScrollMetrics)currentMetrics__27599).extentBefore == this._lastMetrics!.extentBefore) && (((ScrollMetrics)currentMetrics__27599).extentInside == this._lastMetrics!.extentInside)) && (((ScrollMetrics)currentMetrics__27599).extentAfter == this._lastMetrics!.extentAfter)) && (object.Equals(((ScrollMetrics)currentMetrics__27599).axisDirection, this._lastMetrics!.axisDirection)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool applyContentDimensions(double minScrollExtent, double maxScrollExtent)
    {
        DartRuntimePrimitives.Assert(() => (this.haveDimensions == ((this._lastMetrics is not null))));
        if ((((!global::Doroti.Framework.Physics.UtilsLibrary.nearEqual(this._minScrollExtent, minScrollExtent, global::Doroti.Framework.Physics.Tolerance.defaultTolerance.distance) || !global::Doroti.Framework.Physics.UtilsLibrary.nearEqual(this._maxScrollExtent, maxScrollExtent, global::Doroti.Framework.Physics.Tolerance.defaultTolerance.distance)) || this._didChangeViewportDimensionOrReceiveCorrection) || (!object.Equals(this._lastAxis, this.axis))))
        {
            DartRuntimePrimitives.Assert(() => (minScrollExtent <= maxScrollExtent));
            _minScrollExtent = minScrollExtent;
            _maxScrollExtent = maxScrollExtent;
            _lastAxis = this.axis;
            ScrollMetrics? currentMetrics__28564 = (this.haveDimensions ? copyWith() : null);
            _didChangeViewportDimensionOrReceiveCorrection = false;
            _pendingDimensions = true;
            if ((this.haveDimensions && !correctForNewDimensions(this._lastMetrics!, currentMetrics__28564!)))
            {
                return false;
            }
            _haveDimensions = true;
        }
        DartRuntimePrimitives.Assert(() => this.haveDimensions);
        if (this._pendingDimensions)
        {
            applyNewDimensions();
            _pendingDimensions = false;
        }
        DartRuntimePrimitives.Assert(() => !this._didChangeViewportDimensionOrReceiveCorrection, () => (object?)"Use correctForNewDimensions() (and return true) to change the scroll offset during applyContentDimensions().");
        if (_isMetricsChanged())
        {
            if (!this._haveScheduledUpdateNotification)
            {
                DartAsyncRuntime.scheduleMicrotask(this.didUpdateScrollMetrics);
                _haveScheduledUpdateNotification = true;
            }
            _lastMetrics = copyWith();
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool correctForNewDimensions(ScrollMetrics oldPosition, ScrollMetrics newPosition)
    {
        double newPixels__30380 = this.physics.adjustPositionForNewDimensions(oldPosition: oldPosition, newPosition: newPosition, isScrolling: this.activity!.isScrolling, velocity: this.activity!.velocity);
        if ((newPixels__30380 != this.pixels))
        {
            correctPixels(newPixels__30380);
            return false;
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void applyNewDimensions()
    {
        DartRuntimePrimitives.Assert(() => this.hasPixels);
        DartRuntimePrimitives.Assert(() => this._pendingDimensions);
        this.activity!.applyNewDimensions();
        _updateSemanticActions();
    }

    internal virtual void _updateSemanticActions()
    {
        var (forward__32619, backward__32644) = (this.axisDirection switch { global::Doroti.Framework.Painting.AxisDirection.up => (((SemanticsAction, SemanticsAction))((SemanticsAction.scrollDown, SemanticsAction.scrollUp))), global::Doroti.Framework.Painting.AxisDirection.down => (((SemanticsAction, SemanticsAction))((SemanticsAction.scrollUp, SemanticsAction.scrollDown))), global::Doroti.Framework.Painting.AxisDirection.left => (((SemanticsAction, SemanticsAction))((SemanticsAction.scrollRight, SemanticsAction.scrollLeft))), global::Doroti.Framework.Painting.AxisDirection.right => (((SemanticsAction, SemanticsAction))((SemanticsAction.scrollLeft, SemanticsAction.scrollRight))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        var actions__33040 = new HashSet<SemanticsAction>();
        if (global::Doroti.Framework.Foundation.CollectionsLibrary.setEquals<global::Doroti.Ui.SemanticsAction>(actions__33040, this._semanticActions))
        {
            return;
        }
        _semanticActions = actions__33040;
        this.context.setSemanticsActions(this._semanticActions!);
    }

    internal virtual ScrollPositionAlignmentPolicy _maybeFlipAlignment(ScrollPositionAlignmentPolicy alignmentPolicy)
    {
        return (alignmentPolicy switch { ScrollPositionAlignmentPolicy.@explicit => alignmentPolicy, ScrollPositionAlignmentPolicy.keepVisibleAtEnd => ScrollPositionAlignmentPolicy.keepVisibleAtStart, ScrollPositionAlignmentPolicy.keepVisibleAtStart => ScrollPositionAlignmentPolicy.keepVisibleAtEnd, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual ScrollPositionAlignmentPolicy _applyAxisDirectionToAlignmentPolicy(ScrollPositionAlignmentPolicy alignmentPolicy)
    {
        return (this.axisDirection switch { global::Doroti.Framework.Painting.AxisDirection.up => _maybeFlipAlignment(alignmentPolicy), global::Doroti.Framework.Painting.AxisDirection.left => _maybeFlipAlignment(alignmentPolicy), global::Doroti.Framework.Painting.AxisDirection.down => alignmentPolicy, global::Doroti.Framework.Painting.AxisDirection.right => alignmentPolicy, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future ensureVisible(global::Doroti.Framework.Rendering.RenderObject @object, double alignment = 0.0, Duration duration = default, global::Doroti.Framework.Animation.Curve curve = default!, ScrollPositionAlignmentPolicy alignmentPolicy = ScrollPositionAlignmentPolicy.@explicit, global::Doroti.Framework.Rendering.RenderObject? targetRenderObject = null)
    {
        DartRuntimePrimitives.Assert(() => ((global::Doroti.Framework.Rendering.RenderObject)@object).attached);
        global::Doroti.Framework.Rendering.RenderAbstractViewport? viewport__35436 = ((global::Doroti.Framework.Rendering.RenderAbstractViewport?)(object?)RenderAbstractViewport.maybeOf(@object));
        if ((viewport__35436 is null))
        {
            return;
        }
        global::Doroti.Ui.Rect? targetRect__35586 = default!;
        if (((targetRenderObject is not null) && (!object.Equals(targetRenderObject, @object))))
        {
            targetRect__35586 = MatrixUtils.transformRect(((Matrix4)((dynamic)targetRenderObject).getTransformTo(@object)), ((global::Doroti.Framework.Rendering.RenderObject)@object).paintBounds.intersect(((global::Doroti.Framework.Rendering.RenderObject)targetRenderObject).paintBounds));
        }
        double target__35862 = default!;
        switch (_applyAxisDirectionToAlignmentPolicy(alignmentPolicy))
        {
            case ScrollPositionAlignmentPolicy.@explicit:
                {
                    target__35862 = viewport__35436.getOffsetToReveal(@object, alignment, rect: targetRect__35586, axis: this.axis).offset;
                    target__35862 = Dart_uiLibrary.clampDouble(target__35862, this.minScrollExtent, this.maxScrollExtent);
                    break;
                }
            case ScrollPositionAlignmentPolicy.keepVisibleAtEnd:
                {
                    target__35862 = viewport__35436.getOffsetToReveal(@object, 1.0, rect: targetRect__35586, axis: this.axis).offset;
                    target__35862 = Dart_uiLibrary.clampDouble(target__35862, this.minScrollExtent, this.maxScrollExtent);
                    if ((target__35862 < this.pixels))
                    {
                        target__35862 = this.pixels;
                    }
                    break;
                }
            case ScrollPositionAlignmentPolicy.keepVisibleAtStart:
                {
                    target__35862 = viewport__35436.getOffsetToReveal(@object, 0.0, rect: targetRect__35586, axis: this.axis).offset;
                    target__35862 = Dart_uiLibrary.clampDouble(target__35862, this.minScrollExtent, this.maxScrollExtent);
                    if ((target__35862 > this.pixels))
                    {
                        target__35862 = this.pixels;
                    }
                    break;
                }
        }
        if ((target__35862 == this.pixels))
        {
            return;
        }
        if ((object.Equals(DartRuntimePrimitives.RequireValue(duration), Duration.zero)))
        {
            jumpTo(target__35862);
            return;
        }
        await animateTo(target__35862, duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)), curve: curve);
        return;
    }

    public abstract override Future animateTo(double to, Duration duration, global::Doroti.Framework.Animation.Curve curve);
    public abstract override void jumpTo(double pixels);
    public abstract void pointerScroll(double delta);
    public override Future moveTo(double to, Duration? duration = null, global::Doroti.Framework.Animation.Curve? curve = null, bool? clamp = true)
    {
        DartRuntimePrimitives.Assert(() => (clamp is not null));
        if (DartRuntimePrimitives.RequireValue(clamp))
        {
            to = Dart_uiLibrary.clampDouble(to, this.minScrollExtent, this.maxScrollExtent);
        }
        return ((Future)(object?)base.moveTo(to, duration: duration, curve: curve));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool allowImplicitScrolling => ((ScrollPhysics)this.physics).allowImplicitScrolling;
    public abstract void jumpToWithoutSettling(double value);
    public abstract ScrollHoldController hold(global::System.Action holdCancelCallback);
    public abstract global::Doroti.Framework.Gestures.Drag drag(global::Doroti.Framework.Gestures.DragStartDetails details, global::System.Action dragCancelCallback);
    public virtual ScrollActivity? activity => this._activity;
    public virtual void beginActivity(ScrollActivity? newActivity)
    {
        if ((newActivity is null))
        {
            return;
        }
        bool wasScrolling__42964 = default!;
        bool oldIgnorePointer__42978 = default!;
        if ((this._activity is not null))
        {
            oldIgnorePointer__42978 = this._activity!.shouldIgnorePointer;
            wasScrolling__42964 = this._activity!.isScrolling;
            if ((wasScrolling__42964 && !((ScrollActivity)newActivity).isScrolling))
            {
                didEndScroll();
            }
            this._activity!.dispose();
        }
        else
        {
            oldIgnorePointer__42978 = false;
            wasScrolling__42964 = false;
        }
        _activity = newActivity;
        if ((oldIgnorePointer__42978 != this.activity!.shouldIgnorePointer))
        {
            this.context.setIgnorePointer(this.activity!.shouldIgnorePointer);
        }
        this.isScrollingNotifier.value = this.activity!.isScrolling;
        if ((!wasScrolling__42964 && this._activity!.isScrolling))
        {
            didStartScroll();
        }
    }

    public virtual void didStartScroll()
    {
        this.activity!.dispatchScrollStartNotification(copyWith(), ((ScrollContext)this.context).notificationContext);
    }

    public virtual void didUpdateScrollPositionBy(double delta)
    {
        this.activity!.dispatchScrollUpdateNotification(copyWith(), ((ScrollContext)this.context).notificationContext!, delta);
    }

    public virtual void didEndScroll()
    {
        this.activity!.dispatchScrollEndNotification(copyWith(), ((ScrollContext)this.context).notificationContext!);
        saveOffset();
        if (this.keepScrollOffset)
        {
            saveScrollOffset();
        }
    }

    public virtual void didOverscrollBy(double value)
    {
        DartRuntimePrimitives.Assert(() => this.activity!.isScrolling);
        this.activity!.dispatchOverscrollNotification(copyWith(), ((ScrollContext)this.context).notificationContext!, value);
    }

    public virtual void didUpdateScrollDirection(global::Doroti.Framework.Rendering.ScrollDirection direction)
    {
        new UserScrollNotification(metrics: copyWith(), context: ((ScrollContext)this.context).notificationContext!, direction: direction).dispatch(((ScrollContext)this.context).notificationContext);
    }

    public virtual void didUpdateScrollMetrics()
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(global::Doroti.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase, global::Doroti.Framework.Scheduler.SchedulerPhase.persistentCallbacks)));
        DartRuntimePrimitives.Assert(() => this._haveScheduledUpdateNotification);
        _haveScheduledUpdateNotification = false;
        if ((((ScrollContext)this.context).notificationContext is not null))
        {
            new ScrollMetricsNotification(metrics: copyWith(), context: ((ScrollContext)this.context).notificationContext!).dispatch(((ScrollContext)this.context).notificationContext);
        }
    }

    public virtual bool recommendDeferredLoading(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => (this.activity is not null));
        return this.physics.recommendDeferredLoading((this.activity!.velocity + this._impliedVelocity), copyWith(), context);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void dispose()
    {
        this.activity?.dispose();
        _activity = null;
        this.isScrollingNotifier.dispose();
        base.dispose();
    }

    public virtual void notifyListeners()
    {
        _updateSemanticActions();
        base.notifyListeners();
    }

    public override void debugFillDescription(List<string> description)
    {
        if ((this.debugLabel is not null))
        {
            description.Add(this.debugLabel!);
        }
        base.debugFillDescription(description);
        description.Add($"range: {this._minScrollExtent?.toStringAsFixed(1L)}..{this._maxScrollExtent?.toStringAsFixed(1L)}");
        description.Add($"viewport: {this._viewportDimension?.toStringAsFixed(1L)}");
    }

    public abstract AxisDirection axisDirection { get; }
    public virtual ScrollMetrics copyWith(double? minScrollExtent = null, double? maxScrollExtent = null, double? pixels = null, double? viewportDimension = null, global::Doroti.Framework.Painting.AxisDirection? axisDirection = null, double? devicePixelRatio = null, long? itemIndex = null, double? minRange = null, double? maxRange = null, double? correctionOffset = null, double? viewportFraction = null)
    {
        return ((ScrollMetrics)(object?)new FixedScrollMetrics(minScrollExtent: (minScrollExtent ?? ((this.hasContentDimensions ? this.minScrollExtent : null))), maxScrollExtent: (maxScrollExtent ?? ((this.hasContentDimensions ? this.maxScrollExtent : null))), pixels: (pixels ?? ((this.hasPixels ? this.pixels : null))), viewportDimension: (viewportDimension ?? ((this.hasViewportDimension ? this.viewportDimension : null))), axisDirection: (axisDirection ?? this.axisDirection), devicePixelRatio: (devicePixelRatio ?? this.devicePixelRatio)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Painting.Axis axis => global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(DartRuntimePrimitives.RequireValue(this.axisDirection));
    public virtual bool outOfRange => DartRuntimePrimitives.ConvertValue<bool>(((this.pixels < this.minScrollExtent) || (this.pixels > this.maxScrollExtent)));
    public virtual bool atEdge => DartRuntimePrimitives.ConvertValue<bool>(((this.pixels == this.minScrollExtent) || (this.pixels == this.maxScrollExtent)));
    public virtual double extentBefore => Math.Max((this.pixels - this.minScrollExtent), 0.0);
    public virtual double extentInside
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (this.minScrollExtent <= this.maxScrollExtent));
            return ((this.viewportDimension - Dart_uiLibrary.clampDouble((this.minScrollExtent - this.pixels), 0, this.viewportDimension)) - Dart_uiLibrary.clampDouble((this.pixels - this.maxScrollExtent), 0, this.viewportDimension));
            return default!;
        }
    }
    public virtual double extentAfter => Math.Max((this.maxScrollExtent - this.pixels), 0.0);
    public virtual double extentTotal => DartRuntimePrimitives.ConvertValue<double>(((this.maxScrollExtent - this.minScrollExtent) + this.viewportDimension));
}

public class ScrollMetricsNotification : Notification, ViewportNotificationMixin
{
    public virtual ScrollMetrics metrics { get; private set; } = default!;
    public virtual BuildContext context { get; private set; } = default!;
    public virtual long _depth { get; set; } = 0L;

    public ScrollMetricsNotification(ScrollMetrics metrics, BuildContext context)
    {
        this.metrics = metrics;
        this.context = context;
    }

    public virtual ScrollUpdateNotification asScrollUpdate()
    {
        return new ScrollUpdateNotification(metrics: this.metrics, context: this.context, depth: this.depth);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillDescription(List<string> description)
    {
        base.debugFillDescription(description);
        description.Add($"depth: {this.depth} ({((this.depth == 0L) ? "local" : "remote")})");
        description.Add($"{this.metrics}");
    }

    public virtual long depth => this._depth;
}
