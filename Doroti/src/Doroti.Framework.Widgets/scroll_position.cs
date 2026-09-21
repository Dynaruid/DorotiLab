// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scroll_position.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public enum ScrollPositionAlignmentPolicy
{
    @explicit,
    keepVisibleAtEnd,
    keepVisibleAtStart,
}

public abstract class ScrollPosition : ViewportOffset, ScrollMetrics
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
    internal virtual Axis? _lastAxis { get; set; } = default;
    internal virtual HashSet<SemanticsAction>? _semanticActions { get; set; } = default;
    public virtual ValueNotifier<bool> isScrollingNotifier { get; private set; } =
        new ValueNotifier<bool>(false);
    internal virtual ScrollActivity? _activity { get; set; } = default;

    protected ScrollPosition(
        ScrollPhysics physics,
        ScrollContext context,
        bool keepScrollOffset = true,
        ScrollPosition? oldPosition = null,
        string? debugLabel = null
    )
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

    public virtual double minScrollExtent =>
        DartRuntimePrimitives.ConvertValue<double>(
            (
                _minScrollExtent
                ?? throw new global::System.NullReferenceException("A required value was null.")
            )
        );
    public virtual double maxScrollExtent =>
        DartRuntimePrimitives.ConvertValue<double>(
            (
                _maxScrollExtent
                ?? throw new global::System.NullReferenceException("A required value was null.")
            )
        );
    public virtual bool hasContentDimensions =>
        DartRuntimePrimitives.ConvertValue<bool>(
            (_minScrollExtent is not null) && (_maxScrollExtent is not null)
        );
    public override double pixels =>
        DartRuntimePrimitives.ConvertValue<double>(
            (
                _pixels
                ?? throw new global::System.NullReferenceException("A required value was null.")
            )
        );
    public override bool hasPixels => DartRuntimePrimitives.ConvertValue<bool>(_pixels is not null);
    public virtual double viewportDimension =>
        DartRuntimePrimitives.ConvertValue<double>(
            (
                _viewportDimension
                ?? throw new global::System.NullReferenceException("A required value was null.")
            )
        );
    public virtual bool hasViewportDimension =>
        DartRuntimePrimitives.ConvertValue<bool>(_viewportDimension is not null);
    public virtual bool haveDimensions => _haveDimensions;
    public virtual bool shouldIgnorePointer =>
        DartRuntimePrimitives.ConvertValue<bool>(
            !outOfRange && (activity?.shouldIgnorePointer ?? true)
        );

    public virtual void absorb(ScrollPosition other)
    {
        DartRuntimePrimitives.Assert(() => Equals(other.context, context));
        DartRuntimePrimitives.Assert(() => _pixels is null);
        if (other.hasContentDimensions)
        {
            _minScrollExtent = other.minScrollExtent;
            _maxScrollExtent = other.maxScrollExtent;
        }
        if (other.hasPixels)
        {
            _pixels = other.pixels;
        }
        if (other.hasViewportDimension)
        {
            _viewportDimension = other.viewportDimension;
        }
        DartRuntimePrimitives.Assert(() => activity is null);
        DartRuntimePrimitives.Assert(() => other.activity is not null);
        _activity = other.activity;
        other._activity = null;
        if (!Equals(DartRuntimePrimitives.RuntimeType(other), GetType()))
        {
            activity!.resetActivity();
        }
        context.setIgnorePointer(activity!.shouldIgnorePointer);
        isScrollingNotifier.value = activity!.isScrolling;
    }

    public virtual double devicePixelRatio => context.devicePixelRatio;

    public virtual double setPixels(double newPixels)
    {
        DartRuntimePrimitives.Assert(() => hasPixels);
        DartRuntimePrimitives.Assert(
            () =>
                !Equals(
                    Scheduler.SchedulerBinding.instance.schedulerPhase,
                    Scheduler.SchedulerPhase.persistentCallbacks
                ),
            () =>
                (object?)
                    "A scrollable's position should not change during the build, layout, and paint phases, otherwise the rendering will be confused."
        );
        if (newPixels != pixels)
        {
            double overscroll = applyBoundaryConditions(newPixels);
            DartRuntimePrimitives.Assert(() =>
            {
                double delta = newPixels - pixels;
                if (overscroll.abs() > delta.abs())
                {
                    throw DartRuntimePrimitives.AsException(
                        FlutterError.Create(
                            $"{GetType()}.applyBoundaryConditions returned invalid overscroll value.\n"
                                + $"setPixels() was called to change the scroll offset from {pixels} to {newPixels}.\n"
                                + $"That is a delta of {delta} units.\n"
                                + $"{GetType()}.applyBoundaryConditions reported an overscroll of {overscroll} units."
                        )
                    );
                }
                return true;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            });
            double oldPixels = pixels;
            _pixels = newPixels - overscroll;
            if (_pixels != oldPixels)
            {
                if (outOfRange)
                {
                    context.setIgnorePointer(false);
                }
                notifyListeners();
                didUpdateScrollPositionBy(pixels - oldPixels);
            }
            if (overscroll.abs() > Foundation.ConstantsLibrary.precisionErrorTolerance)
            {
                didOverscrollBy(overscroll);
                return overscroll;
            }
        }
        return 0.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void correctPixels(double value)
    {
        _pixels = value;
    }

    public override void correctBy(double correction)
    {
        DartRuntimePrimitives.Assert(
            () => hasPixels,
            () =>
                (object?)
                    "An initial pixels value must exist by calling correctPixels on the ScrollPosition"
        );
        _pixels =
            (
                _pixels
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ) + correction;
        _didChangeViewportDimensionOrReceiveCorrection = true;
    }

    public virtual void forcePixels(double value)
    {
        DartRuntimePrimitives.Assert(() => hasPixels);
        _impliedVelocity = value - pixels;
        _pixels = value;
        notifyListeners();
        Scheduler.SchedulerBinding.instance.addPostFrameCallback(
            (timeStamp) =>
            {
                _impliedVelocity = 0;
            },
            debugLabel: "ScrollPosition.resetVelocity"
        );
    }

    public virtual void saveScrollOffset()
    {
        PageStorage.maybeOf(context.storageContext)?.writeState(context.storageContext, pixels);
    }

    public virtual void restoreScrollOffset()
    {
        if (!hasPixels)
        {
            var value = (double?)
                PageStorage.maybeOf(context.storageContext)?.readState(context.storageContext);
            if (value is not null)
            {
                double value__23743__value23862 = (
                    value
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
                correctPixels(((value__23743__value23862)));
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
        DartRuntimePrimitives.Assert(() => hasPixels);
        context.saveOffset(pixels);
    }

    public virtual double applyBoundaryConditions(double value)
    {
        double result = physics.applyBoundaryConditions(this, value);
        DartRuntimePrimitives.Assert(() =>
        {
            double delta = value - pixels;
            if (result.abs() > delta.abs())
            {
                throw DartRuntimePrimitives.AsException(
                    FlutterError.Create(
                        $"{DartRuntimePrimitives.RuntimeType(physics)}.applyBoundaryConditions returned invalid overscroll value.\n"
                            + $"The method was called to consider a change from {pixels} to {value}, which is a "
                            + $"delta of {delta.toStringAsFixed(1L)} units. However, it returned an overscroll of "
                            + $"{result.toStringAsFixed(1L)} units, which has a greater magnitude than the delta. "
                            + "The applyBoundaryConditions method is only supposed to reduce the possible range "
                            + "of movement, not increase it.\n"
                            + $"The scroll extents are {minScrollExtent} .. {maxScrollExtent}, and the "
                            + $"viewport dimension is {viewportDimension}."
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        return result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool applyViewportDimension(double viewportDimension)
    {
        if (_viewportDimension != viewportDimension)
        {
            _viewportDimension = viewportDimension;
            _didChangeViewportDimensionOrReceiveCorrection = true;
        }
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual bool _isMetricsChanged()
    {
        DartRuntimePrimitives.Assert(() => haveDimensions);
        ScrollMetrics currentMetrics = copyWith();
        return (_lastMetrics is null)
            || !(
                (currentMetrics.extentBefore == _lastMetrics!.extentBefore)
                && (currentMetrics.extentInside == _lastMetrics!.extentInside)
                && (currentMetrics.extentAfter == _lastMetrics!.extentAfter)
                && Equals(currentMetrics.axisDirection, _lastMetrics!.axisDirection)
            );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool applyContentDimensions(double minScrollExtent, double maxScrollExtent)
    {
        DartRuntimePrimitives.Assert(() => haveDimensions == (_lastMetrics is not null));
        if (
            !Physics.UtilsLibrary.nearEqual(
                _minScrollExtent,
                minScrollExtent,
                Physics.Tolerance.defaultTolerance.distance
            )
            || !Physics.UtilsLibrary.nearEqual(
                _maxScrollExtent,
                maxScrollExtent,
                Physics.Tolerance.defaultTolerance.distance
            )
            || _didChangeViewportDimensionOrReceiveCorrection
            || (!Equals(_lastAxis, axis))
        )
        {
            DartRuntimePrimitives.Assert(() => minScrollExtent <= maxScrollExtent);
            _minScrollExtent = minScrollExtent;
            _maxScrollExtent = maxScrollExtent;
            _lastAxis = axis;
            ScrollMetrics? currentMetrics = haveDimensions ? copyWith() : null;
            _didChangeViewportDimensionOrReceiveCorrection = false;
            _pendingDimensions = true;
            if (haveDimensions && !correctForNewDimensions(_lastMetrics!, currentMetrics!))
            {
                return false;
            }
            _haveDimensions = true;
        }
        DartRuntimePrimitives.Assert(() => haveDimensions);
        if (_pendingDimensions)
        {
            applyNewDimensions();
            _pendingDimensions = false;
        }
        DartRuntimePrimitives.Assert(
            () => !_didChangeViewportDimensionOrReceiveCorrection,
            () =>
                (object?)
                    "Use correctForNewDimensions() (and return true) to change the scroll offset during applyContentDimensions()."
        );
        if (_isMetricsChanged())
        {
            if (!_haveScheduledUpdateNotification)
            {
                DartAsyncRuntime.scheduleMicrotask(didUpdateScrollMetrics);
                _haveScheduledUpdateNotification = true;
            }
            _lastMetrics = copyWith();
        }
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool correctForNewDimensions(
        ScrollMetrics oldPosition,
        ScrollMetrics newPosition
    )
    {
        double newPixels = physics.adjustPositionForNewDimensions(
            oldPosition: oldPosition,
            newPosition: newPosition,
            isScrolling: activity!.isScrolling,
            velocity: activity!.velocity
        );
        if (newPixels != pixels)
        {
            correctPixels(newPixels);
            return false;
        }
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void applyNewDimensions()
    {
        DartRuntimePrimitives.Assert(() => hasPixels);
        DartRuntimePrimitives.Assert(() => _pendingDimensions);
        activity!.applyNewDimensions();
        _updateSemanticActions();
    }

    internal virtual void _updateSemanticActions()
    {
        var (forward, backward) = axisDirection switch
        {
            AxisDirection.up => (SemanticsAction.scrollDown, SemanticsAction.scrollUp),
            AxisDirection.down => (SemanticsAction.scrollUp, SemanticsAction.scrollDown),
            AxisDirection.left => (SemanticsAction.scrollRight, SemanticsAction.scrollLeft),
            AxisDirection.right => (SemanticsAction.scrollLeft, SemanticsAction.scrollRight),
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        var actions = new HashSet<SemanticsAction>();
        if (CollectionsLibrary.setEquals(actions, _semanticActions))
        {
            return;
        }
        _semanticActions = actions;
        context.setSemanticsActions(_semanticActions!);
    }

    internal virtual ScrollPositionAlignmentPolicy _maybeFlipAlignment(
        ScrollPositionAlignmentPolicy alignmentPolicy
    )
    {
        return alignmentPolicy switch
        {
            ScrollPositionAlignmentPolicy.@explicit => alignmentPolicy,
            ScrollPositionAlignmentPolicy.keepVisibleAtEnd =>
                ScrollPositionAlignmentPolicy.keepVisibleAtStart,
            ScrollPositionAlignmentPolicy.keepVisibleAtStart =>
                ScrollPositionAlignmentPolicy.keepVisibleAtEnd,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual ScrollPositionAlignmentPolicy _applyAxisDirectionToAlignmentPolicy(
        ScrollPositionAlignmentPolicy alignmentPolicy
    )
    {
        return axisDirection switch
        {
            AxisDirection.up => _maybeFlipAlignment(alignmentPolicy),
            AxisDirection.left => _maybeFlipAlignment(alignmentPolicy),
            AxisDirection.down => alignmentPolicy,
            AxisDirection.right => alignmentPolicy,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual async Future ensureVisible(
        RenderObject @object,
        double alignment = 0.0,
        Duration duration = default,
        Curve curve = default!,
        ScrollPositionAlignmentPolicy alignmentPolicy = ScrollPositionAlignmentPolicy.@explicit,
        RenderObject? targetRenderObject = null
    )
    {
        DartRuntimePrimitives.Assert(() => @object.attached);
        RenderAbstractViewport? viewport = RenderAbstractViewport.maybeOf(@object);
        if (viewport is null)
        {
            return;
        }
        Rect? targetRect = default!;
        if ((targetRenderObject is not null) && (!Equals(targetRenderObject, @object)))
        {
            targetRect = MatrixUtils.transformRect(
                targetRenderObject.getTransformTo(@object),
                @object.paintBounds.intersect(targetRenderObject.paintBounds)
            );
        }
        double target = default!;
        switch (_applyAxisDirectionToAlignmentPolicy(alignmentPolicy))
        {
            case ScrollPositionAlignmentPolicy.@explicit:
            {
                target = viewport
                    .getOffsetToReveal(@object, alignment, rect: targetRect, axis: axis)
                    .offset;
                target = Dart_uiLibrary.clampDouble(target, minScrollExtent, maxScrollExtent);
                break;
            }
            case ScrollPositionAlignmentPolicy.keepVisibleAtEnd:
            {
                target = viewport
                    .getOffsetToReveal(@object, 1.0, rect: targetRect, axis: axis)
                    .offset;
                target = Dart_uiLibrary.clampDouble(target, minScrollExtent, maxScrollExtent);
                if (target < pixels)
                {
                    target = pixels;
                }
                break;
            }
            case ScrollPositionAlignmentPolicy.keepVisibleAtStart:
            {
                target = viewport
                    .getOffsetToReveal(@object, 0.0, rect: targetRect, axis: axis)
                    .offset;
                target = Dart_uiLibrary.clampDouble(target, minScrollExtent, maxScrollExtent);
                if (target > pixels)
                {
                    target = pixels;
                }
                break;
            }
        }
        if (target == pixels)
        {
            return;
        }
        if (Equals((duration), Duration.zero))
        {
            jumpTo(target);
            return;
        }
        await animateTo(target, duration: ((duration)), curve: curve);
        return;
    }

    public abstract override Future animateTo(double to, Duration duration, Curve curve);
    public abstract override void jumpTo(double pixels);
    public abstract void pointerScroll(double delta);

    public override Future moveTo(
        double to,
        Duration? duration = null,
        Curve? curve = null,
        bool? clamp = true
    )
    {
        // C# binds optional arguments at the call site's static type. A call
        // through ViewportOffset supplies null; Dart selects this override's true.
        clamp ??= true;
        DartRuntimePrimitives.Assert(() => clamp is not null);
        if (
            (clamp ?? throw new global::System.NullReferenceException("A required value was null."))
        )
        {
            to = Dart_uiLibrary.clampDouble(to, minScrollExtent, maxScrollExtent);
        }
        return base.moveTo(to, duration: duration, curve: curve);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool allowImplicitScrolling => physics.allowImplicitScrolling;
    public abstract void jumpToWithoutSettling(double value);
    public abstract ScrollHoldController hold(Action holdCancelCallback);
    public abstract Drag drag(DragStartDetails details, Action dragCancelCallback);
    public virtual ScrollActivity? activity => _activity;

    public virtual void beginActivity(ScrollActivity? newActivity)
    {
        if (newActivity is null)
        {
            return;
        }
        bool wasScrolling = default!;
        bool oldIgnorePointer = default!;
        if (_activity is not null)
        {
            oldIgnorePointer = _activity!.shouldIgnorePointer;
            wasScrolling = _activity!.isScrolling;
            if (wasScrolling && !newActivity.isScrolling)
            {
                didEndScroll();
            }
            _activity!.dispose();
        }
        else
        {
            oldIgnorePointer = false;
            wasScrolling = false;
        }
        _activity = newActivity;
        if (oldIgnorePointer != activity!.shouldIgnorePointer)
        {
            context.setIgnorePointer(activity!.shouldIgnorePointer);
        }
        isScrollingNotifier.value = activity!.isScrolling;
        if (!wasScrolling && _activity!.isScrolling)
        {
            didStartScroll();
        }
    }

    public virtual void didStartScroll()
    {
        recordScrollTrace(DorotiFramePhase.scrollStart);
        activity!.dispatchScrollStartNotification(copyWith(), context.notificationContext);
    }

    public virtual void didUpdateScrollPositionBy(double delta)
    {
        recordScrollTrace(DorotiFramePhase.scrollUpdate, delta);
        activity!.dispatchScrollUpdateNotification(copyWith(), context.notificationContext!, delta);
    }

    public virtual void didEndScroll()
    {
        recordScrollTrace(DorotiFramePhase.scrollEnd);
        activity!.dispatchScrollEndNotification(copyWith(), context.notificationContext!);
        saveOffset();
        if (keepScrollOffset)
        {
            saveScrollOffset();
        }
    }

    private void recordScrollTrace(DorotiFramePhase phase, double? delta = null)
    {
        var dispatcher = PlatformDispatcher.instance;
        dispatcher.frameTrace.RecordScroll(
            phase,
            dispatcher.implicitView?.viewId ?? 0,
            System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this),
            pixels,
            delta,
            activity?.GetType().Name ?? "none",
            hasContentDimensions ? minScrollExtent : null,
            hasContentDimensions ? maxScrollExtent : null
        );
    }

    public virtual void didOverscrollBy(double value)
    {
        DartRuntimePrimitives.Assert(() => activity!.isScrolling);
        activity!.dispatchOverscrollNotification(copyWith(), context.notificationContext!, value);
    }

    public virtual void didUpdateScrollDirection(ScrollDirection direction)
    {
        new UserScrollNotification(
            metrics: copyWith(),
            context: context.notificationContext!,
            direction: direction
        ).dispatch(context.notificationContext);
    }

    public virtual void didUpdateScrollMetrics()
    {
        DartRuntimePrimitives.Assert(() =>
            !Equals(
                Scheduler.SchedulerBinding.instance.schedulerPhase,
                Scheduler.SchedulerPhase.persistentCallbacks
            )
        );
        DartRuntimePrimitives.Assert(() => _haveScheduledUpdateNotification);
        _haveScheduledUpdateNotification = false;
        if (context.notificationContext is not null)
        {
            new ScrollMetricsNotification(
                metrics: copyWith(),
                context: context.notificationContext!
            ).dispatch(context.notificationContext);
        }
    }

    public virtual bool recommendDeferredLoading(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => activity is not null);
        return physics.recommendDeferredLoading(
            activity!.velocity + _impliedVelocity,
            copyWith(),
            context
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void dispose()
    {
        activity?.dispose();
        _activity = null;
        isScrollingNotifier.dispose();
        base.dispose();
    }

    public new virtual void notifyListeners()
    {
        _updateSemanticActions();
        base.notifyListeners();
    }

    public override void debugFillDescription(List<string> description)
    {
        if (debugLabel is not null)
        {
            description.Add(debugLabel!);
        }
        base.debugFillDescription(description);
        description.Add(
            $"range: {_minScrollExtent?.toStringAsFixed(1L)}..{_maxScrollExtent?.toStringAsFixed(1L)}"
        );
        description.Add($"viewport: {_viewportDimension?.toStringAsFixed(1L)}");
    }

    public abstract AxisDirection axisDirection { get; }

    public virtual ScrollMetrics copyWith(
        double? minScrollExtent = null,
        double? maxScrollExtent = null,
        double? pixels = null,
        double? viewportDimension = null,
        AxisDirection? axisDirection = null,
        double? devicePixelRatio = null,
        long? itemIndex = null,
        double? minRange = null,
        double? maxRange = null,
        double? correctionOffset = null,
        double? viewportFraction = null
    )
    {
        return new FixedScrollMetrics(
            minScrollExtent: minScrollExtent
                ?? (hasContentDimensions ? this.minScrollExtent : null),
            maxScrollExtent: maxScrollExtent
                ?? (hasContentDimensions ? this.maxScrollExtent : null),
            pixels: pixels ?? (hasPixels ? this.pixels : null),
            viewportDimension: viewportDimension
                ?? (hasViewportDimension ? this.viewportDimension : null),
            axisDirection: axisDirection ?? this.axisDirection,
            devicePixelRatio: devicePixelRatio ?? this.devicePixelRatio
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Axis axis => Basic_typesLibrary.axisDirectionToAxis((axisDirection));
    public virtual bool outOfRange =>
        DartRuntimePrimitives.ConvertValue<bool>(
            (pixels < minScrollExtent) || (pixels > maxScrollExtent)
        );
    public virtual bool atEdge =>
        DartRuntimePrimitives.ConvertValue<bool>(
            (pixels == minScrollExtent) || (pixels == maxScrollExtent)
        );
    public virtual double extentBefore => Math.Max(pixels - minScrollExtent, 0.0);
    public virtual double extentInside
    {
        get
        {
            DartRuntimePrimitives.Assert(() => minScrollExtent <= maxScrollExtent);
            return viewportDimension
                - Dart_uiLibrary.clampDouble(minScrollExtent - pixels, 0, viewportDimension)
                - Dart_uiLibrary.clampDouble(pixels - maxScrollExtent, 0, viewportDimension);
        }
    }
    public virtual double extentAfter => Math.Max(maxScrollExtent - pixels, 0.0);
    public virtual double extentTotal =>
        DartRuntimePrimitives.ConvertValue<double>(
            maxScrollExtent - minScrollExtent + viewportDimension
        );
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
        return new ScrollUpdateNotification(metrics: metrics, context: context, depth: depth);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void debugFillDescription(List<string> description)
    {
        base.debugFillDescription(description);
        description.Add($"depth: {depth} ({((depth == 0L) ? "local" : "remote")})");
        description.Add($"{metrics}");
    }

    public virtual long depth => _depth;
}
