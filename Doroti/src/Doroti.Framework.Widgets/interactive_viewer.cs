// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/interactive_viewer.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public delegate Widget InteractiveViewerWidgetBuilder(BuildContext context, Quad viewport);

public class InteractiveViewer : StatefulWidget
{
    public virtual Alignment? alignment { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual PanAxis panAxis { get; private set; } = default!;
    public virtual EdgeInsets boundaryMargin { get; private set; } = default!;
    public virtual Func<BuildContext, Quad, Widget>? builder { get; private set; }
    public virtual Widget? child { get; private set; }
    public virtual bool constrained { get; private set; } = default!;
    public virtual bool panEnabled { get; private set; } = default!;
    public virtual bool scaleEnabled { get; private set; } = default!;
    public virtual bool trackpadScrollCausesScale { get; private set; } = default!;
    public virtual double scaleFactor { get; private set; } = default!;
    public virtual double maxScale { get; private set; } = default!;
    public virtual double minScale { get; private set; } = default!;
    public virtual double interactionEndFrictionCoefficient { get; private set; } = default!;
    public virtual Action<ScaleEndDetails>? onInteractionEnd { get; private set; }
    public virtual Action<ScaleStartDetails>? onInteractionStart { get; private set; }
    public virtual Action<ScaleUpdateDetails>? onInteractionUpdate { get; private set; }
    public virtual TransformationController? transformationController { get; private set; }
    internal const double _kDrag = 0.0000135;

    public InteractiveViewer(
        Key? key = null,
        Clip clipBehavior = Clip.hardEdge,
        PanAxis panAxis = PanAxis.free,
        EdgeInsets boundaryMargin = default!,
        bool constrained = true,
        double maxScale = 2.5,
        double minScale = 0.8,
        double? interactionEndFrictionCoefficient = null,
        Action<ScaleEndDetails>? onInteractionEnd = null,
        Action<ScaleStartDetails>? onInteractionStart = null,
        Action<ScaleUpdateDetails>? onInteractionUpdate = null,
        bool panEnabled = true,
        bool scaleEnabled = true,
        double? scaleFactor = null,
        TransformationController? transformationController = null,
        Alignment? alignment = null,
        bool trackpadScrollCausesScale = false,
        Widget child = default!
    )
        : base(key: key)
    {
        EdgeInsets __boundaryMargin = boundaryMargin ?? EdgeInsets.zero;
        double __interactionEndFrictionCoefficient = interactionEndFrictionCoefficient ?? _kDrag;
        double __scaleFactor = scaleFactor ?? ScaleLibrary.kDefaultMouseScrollToScaleFactor;
        this.clipBehavior = clipBehavior;
        this.panAxis = panAxis;
        this.boundaryMargin = __boundaryMargin;
        this.constrained = constrained;
        this.maxScale = maxScale;
        this.minScale = minScale;
        this.interactionEndFrictionCoefficient = __interactionEndFrictionCoefficient;
        this.onInteractionEnd = onInteractionEnd;
        this.onInteractionStart = onInteractionStart;
        this.onInteractionUpdate = onInteractionUpdate;
        this.panEnabled = panEnabled;
        this.scaleEnabled = scaleEnabled;
        this.scaleFactor = __scaleFactor;
        this.transformationController = transformationController;
        this.alignment = alignment;
        this.trackpadScrollCausesScale = trackpadScrollCausesScale;
        this.child = child;
        builder = null;
        System.Diagnostics.Debug.Assert(minScale > 0L);
        System.Diagnostics.Debug.Assert(__interactionEndFrictionCoefficient > 0L);
        System.Diagnostics.Debug.Assert(double.IsFinite(minScale));
        System.Diagnostics.Debug.Assert(maxScale > 0L);
        System.Diagnostics.Debug.Assert(!double.IsNaN(maxScale));
        System.Diagnostics.Debug.Assert(maxScale >= minScale);
        System.Diagnostics.Debug.Assert(
            (
                double.IsInfinity(__boundaryMargin.horizontal)
                && double.IsInfinity(__boundaryMargin.vertical)
            )
                || (
                    double.IsFinite(__boundaryMargin.top)
                    && double.IsFinite(__boundaryMargin.right)
                    && double.IsFinite(__boundaryMargin.bottom)
                    && double.IsFinite(__boundaryMargin.left)
                )
        );
    }

    public static InteractiveViewer CreateBuilder(
        Key? key = null,
        Clip clipBehavior = Clip.hardEdge,
        PanAxis panAxis = PanAxis.free,
        EdgeInsets boundaryMargin = default!,
        double maxScale = 2.5,
        double minScale = 0.8,
        double? interactionEndFrictionCoefficient = null,
        Action<ScaleEndDetails>? onInteractionEnd = null,
        Action<ScaleStartDetails>? onInteractionStart = null,
        Action<ScaleUpdateDetails>? onInteractionUpdate = null,
        bool panEnabled = true,
        bool scaleEnabled = true,
        double scaleFactor = 200.0,
        TransformationController? transformationController = null,
        Alignment? alignment = null,
        bool trackpadScrollCausesScale = false,
        Func<BuildContext, Quad, Widget> builder = default!
    )
    {
        var __instance = new InteractiveViewer(
            key,
            clipBehavior,
            panAxis,
            boundaryMargin,
            default!,
            maxScale,
            minScale,
            interactionEndFrictionCoefficient,
            onInteractionEnd,
            onInteractionStart,
            onInteractionUpdate,
            panEnabled,
            scaleEnabled,
            scaleFactor,
            transformationController,
            alignment,
            trackpadScrollCausesScale,
            default!
        );
        EdgeInsets __boundaryMargin = boundaryMargin ?? EdgeInsets.zero;
        double __interactionEndFrictionCoefficient = interactionEndFrictionCoefficient ?? _kDrag;
        __instance.clipBehavior = clipBehavior;
        __instance.panAxis = panAxis;
        __instance.boundaryMargin = __boundaryMargin;
        __instance.maxScale = maxScale;
        __instance.minScale = minScale;
        __instance.interactionEndFrictionCoefficient = __interactionEndFrictionCoefficient;
        __instance.onInteractionEnd = onInteractionEnd;
        __instance.onInteractionStart = onInteractionStart;
        __instance.onInteractionUpdate = onInteractionUpdate;
        __instance.panEnabled = panEnabled;
        __instance.scaleEnabled = scaleEnabled;
        __instance.scaleFactor = scaleFactor;
        __instance.transformationController = transformationController;
        __instance.alignment = alignment;
        __instance.trackpadScrollCausesScale = trackpadScrollCausesScale;
        __instance.builder = builder;
        __instance.constrained = false;
        __instance.child = null;
        return __instance;
    }

    public static Vector3 getNearestPointOnLine(Vector3 point, Vector3 l1, Vector3 l2)
    {
        double lengthSquared =
            Dart_mathLibrary.pow(l2.x - l1.x, 2.0).toDouble()
            + Dart_mathLibrary.pow(l2.y - l1.y, 2.0).toDouble();
        if (lengthSquared == 0L)
        {
            return l1;
        }
        Vector3 l1P = point - l1;
        Vector3 l1L2 = l2 - l1;
        double fraction = Dart_uiLibrary.clampDouble(l1P.dot(l1L2) / lengthSquared, 0.0, 1.0);
        return l1 + (l1L2 * fraction);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Quad getAxisAlignedBoundingBox(Quad quad)
    {
        double minX = Math.Min(
            quad.point0.x,
            Math.Min(quad.point1.x, Math.Min(quad.point2.x, quad.point3.x))
        );
        double minY = Math.Min(
            quad.point0.y,
            Math.Min(quad.point1.y, Math.Min(quad.point2.y, quad.point3.y))
        );
        double maxX = Math.Max(
            quad.point0.x,
            Math.Max(quad.point1.x, Math.Max(quad.point2.x, quad.point3.x))
        );
        double maxY = Math.Max(
            quad.point0.y,
            Math.Max(quad.point1.y, Math.Max(quad.point2.y, quad.point3.y))
        );
        return new Quad(
            new Vector3(minX, minY, 0),
            new Vector3(maxX, minY, 0),
            new Vector3(maxX, maxY, 0),
            new Vector3(minX, maxY, 0)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool pointIsInside(Vector3 point, Quad quad)
    {
        Vector3 aM = point - quad.point0;
        Vector3 aB = quad.point1 - quad.point0;
        Vector3 aD = quad.point3 - quad.point0;
        double aMAB = aM.dot(aB);
        double aBAB = aB.dot(aB);
        double aMAD = aM.dot(aD);
        double aDAD = aD.dot(aD);
        return (0L <= aMAB) && (aMAB <= aBAB) && (0L <= aMAD) && (aMAD <= aDAD);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Vector3 getNearestPointInside(Vector3 point, Quad quad)
    {
        if (pointIsInside(point, quad))
        {
            return point;
        }
        var closestPoints = new List<Vector3>
        {
            getNearestPointOnLine(point, quad.point0, quad.point1),
            getNearestPointOnLine(point, quad.point1, quad.point2),
            getNearestPointOnLine(point, quad.point2, quad.point3),
            getNearestPointOnLine(point, quad.point3, quad.point0),
        };
        double minDistance = double.PositiveInfinity;
        Vector3 closestOverall = default!;
        foreach (var closePoint in closestPoints)
        {
            double distance = Dart_mathLibrary.sqrt(
                Dart_mathLibrary.pow(point.x - closePoint.x, 2L)
                    + Dart_mathLibrary.pow(point.y - closePoint.y, 2L)
            );
            if (distance < minDistance)
            {
                minDistance = distance;
                closestOverall = closePoint;
            }
        }
        return closestOverall;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _InteractiveViewerState__interactive_viewer()
        );
}

internal class _InteractiveViewerState__interactive_viewer
    : State<InteractiveViewer>,
        TickerProviderStateMixin<InteractiveViewer>
{
    private bool __late__transformer_initialized;
    private TransformationController __late__transformer = default!;
    internal virtual TransformationController _transformer
    {
        get
        {
            if (!__late__transformer_initialized)
            {
                __late__transformer =
                    widget.transformationController ?? new TransformationController();
                __late__transformer_initialized = true;
            }
            return __late__transformer;
        }
        set
        {
            __late__transformer = value;
            __late__transformer_initialized = true;
        }
    }
    internal virtual GlobalKey<IState> _childKey { get; private set; } = GlobalKey<IState>.Create();
    internal virtual GlobalKey<IState> _parentKey { get; private set; } =
        GlobalKey<IState>.Create();
    internal virtual Animation<Offset>? _animation { get; set; } = default;
    internal virtual Animation<double>? _scaleAnimation { get; set; } = default;
    internal virtual Offset _scaleAnimationFocalPoint { get; set; } = default!;
    internal virtual AnimationController _controller { get; set; } = default!;
    internal virtual AnimationController _scaleController { get; set; } = default!;
    internal virtual Axis? _currentAxis { get; set; } = default;
    internal virtual Offset? _referenceFocalPoint { get; set; } = default;
    internal virtual double? _scaleStart { get; set; } = default;
    internal virtual double? _rotationStart { get; set; } = 0.0;
    internal virtual double _currentRotation { get; set; } = 0.0;
    internal virtual _GestureType__interactive_viewer? _gestureType { get; set; } = default;
    internal virtual bool _rotateEnabled { get; private set; } = false;
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual Rect _boundaryRect
    {
        get
        {
            DartRuntimePrimitives.Assert(() => _childKey.currentContext is not null);
            DartRuntimePrimitives.Assert(() => !double.IsNaN(widget.boundaryMargin.left));
            DartRuntimePrimitives.Assert(() => !double.IsNaN(widget.boundaryMargin.right));
            DartRuntimePrimitives.Assert(() => !double.IsNaN(widget.boundaryMargin.top));
            DartRuntimePrimitives.Assert(() => !double.IsNaN(widget.boundaryMargin.bottom));
            var childRenderBox = ((RenderBox?)_childKey.currentContext!.findRenderObject()!)!;
            Size childSize = childRenderBox.size;
            Rect boundaryRect = widget.boundaryMargin.inflateRect(Offset.zero & childSize);
            DartRuntimePrimitives.Assert(
                () => !boundaryRect.isEmpty,
                () => (object?)"InteractiveViewer's child must have nonzero dimensions."
            );
            DartRuntimePrimitives.Assert(
                () =>
                    boundaryRect.isFinite
                    || (
                        double.IsInfinity(boundaryRect.left)
                        && double.IsInfinity(boundaryRect.top)
                        && double.IsInfinity(boundaryRect.right)
                        && double.IsInfinity(boundaryRect.bottom)
                    ),
                () =>
                    (object?)
                        "boundaryRect must either be infinite in all directions or finite in all directions."
            );
            return boundaryRect;
        }
    }
    internal virtual Rect _viewport
    {
        get
        {
            DartRuntimePrimitives.Assert(() => _parentKey.currentContext is not null);
            var parentRenderBox = ((RenderBox?)_parentKey.currentContext!.findRenderObject()!)!;
            return Offset.zero & parentRenderBox.size;
        }
    }

    internal virtual Matrix4 _matrixTranslate(Matrix4 matrix, Offset translation)
    {
        if (Equals(translation, Offset.zero))
        {
            return matrix.clone();
        }
        Offset alignedTranslation = default!;
        if (_currentAxis is not null)
        {
            alignedTranslation = widget.panAxis switch
            {
                PanAxis.horizontal => Interactive_viewerLibrary._alignAxis(
                    translation,
                    Axis.horizontal
                ),
                PanAxis.vertical => Interactive_viewerLibrary._alignAxis(
                    translation,
                    Axis.vertical
                ),
                PanAxis.aligned => Interactive_viewerLibrary._alignAxis(
                    translation,
                    (
                        _currentAxis
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    )
                ),
                PanAxis.free => translation,
                _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
            };
        }
        else
        {
            alignedTranslation = translation;
        }
        Matrix4 nextMatrix = (
            (Func<Matrix4>)(
                () =>
                {
                    var __cascade = matrix.clone();
                    __cascade.translateByDouble(alignedTranslation.dx, alignedTranslation.dy, 0, 1);
                    return __cascade;
                }
            )
        )();
        Quad nextViewport = Interactive_viewerLibrary._transformViewport(nextMatrix, _viewport);
        if (_boundaryRect.isInfinite)
        {
            return nextMatrix;
        }
        Quad boundariesAabbQuad = Interactive_viewerLibrary._getAxisAlignedBoundingBoxWithRotation(
            _boundaryRect,
            _currentRotation
        );
        Offset offendingDistance = Interactive_viewerLibrary._exceedsBy(
            boundariesAabbQuad,
            nextViewport
        );
        if (Equals(offendingDistance, Offset.zero))
        {
            return nextMatrix;
        }
        Offset nextTotalTranslation = Interactive_viewerLibrary._getMatrixTranslation(nextMatrix);
        double currentScale = matrix.getMaxScaleOnAxis();
        var correctedTotalTranslation = new Offset(
            nextTotalTranslation.dx - (offendingDistance.dx * currentScale),
            nextTotalTranslation.dy - (offendingDistance.dy * currentScale)
        );
        Matrix4 correctedMatrix = (
            (Func<Matrix4>)(
                () =>
                {
                    var __cascade = matrix.clone();
                    __cascade.setTranslation(
                        new Vector3(correctedTotalTranslation.dx, correctedTotalTranslation.dy, 0.0)
                    );
                    return __cascade;
                }
            )
        )();
        Quad correctedViewport = Interactive_viewerLibrary._transformViewport(
            correctedMatrix,
            _viewport
        );
        Offset offendingCorrectedDistance = Interactive_viewerLibrary._exceedsBy(
            boundariesAabbQuad,
            correctedViewport
        );
        if (Equals(offendingCorrectedDistance, Offset.zero))
        {
            return correctedMatrix;
        }
        if ((offendingCorrectedDistance.dx != 0.0) && (offendingCorrectedDistance.dy != 0.0))
        {
            return matrix.clone();
        }
        var unidirectionalCorrectedTotalTranslation = new Offset(
            (offendingCorrectedDistance.dx == 0.0) ? correctedTotalTranslation.dx : 0.0,
            (offendingCorrectedDistance.dy == 0.0) ? correctedTotalTranslation.dy : 0.0
        );
        return (
            (Func<Matrix4>)(
                () =>
                {
                    var __cascade = matrix.clone();
                    __cascade.setTranslation(
                        new Vector3(
                            unidirectionalCorrectedTotalTranslation.dx,
                            unidirectionalCorrectedTotalTranslation.dy,
                            0.0
                        )
                    );
                    return __cascade;
                }
            )
        )();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Matrix4 _matrixScale(Matrix4 matrix, double scale)
    {
        if (scale == 1.0)
        {
            return matrix.clone();
        }
        DartRuntimePrimitives.Assert(() => scale != 0.0);
        double currentScale = _transformer.value.getMaxScaleOnAxis();
        double totalScale = Math.Max(
            currentScale * scale,
            Math.Max(_viewport.width / _boundaryRect.width, _viewport.height / _boundaryRect.height)
        );
        double clampedTotalScale = Dart_uiLibrary.clampDouble(
            totalScale,
            widget.minScale,
            widget.maxScale
        );
        double clampedScale = clampedTotalScale / currentScale;
        return (
            (Func<Matrix4>)(
                () =>
                {
                    var __cascade = matrix.clone();
                    __cascade.scaleByDouble(clampedScale, clampedScale, clampedScale, 1);
                    return __cascade;
                }
            )
        )();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Matrix4 _matrixRotate(Matrix4 matrix, double rotation, Offset focalPoint)
    {
        if (rotation == 0L)
        {
            return matrix.clone();
        }
        Offset focalPointScene = _transformer.toScene(focalPoint);
        return (
            (Func<Matrix4>)(
                () =>
                {
                    var __cascade = matrix.clone();
                    __cascade.translateByDouble(focalPointScene.dx, focalPointScene.dy, 0, 1);
                    __cascade.rotateZ(-rotation);
                    __cascade.translateByDouble(-focalPointScene.dx, -focalPointScene.dy, 0, 1);
                    return __cascade;
                }
            )
        )();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _gestureIsSupported(_GestureType__interactive_viewer? gestureType)
    {
        return gestureType switch
        {
            _GestureType__interactive_viewer.rotate => _rotateEnabled,
            _GestureType__interactive_viewer.scale => widget.scaleEnabled,
            _GestureType__interactive_viewer.pan => widget.panEnabled,
            null => widget.panEnabled,
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual _GestureType__interactive_viewer _getGestureType(ScaleUpdateDetails details)
    {
        double scaleLocal = !widget.scaleEnabled ? 1.0 : details.scale;
        double rotationLocal = !_rotateEnabled ? 0.0 : details.rotation;
        if ((scaleLocal - 1L).abs() > rotationLocal.abs())
        {
            return _GestureType__interactive_viewer.scale;
        }
        else
        {
            if (rotationLocal != 0.0)
            {
                return _GestureType__interactive_viewer.rotate;
            }
            else
            {
                return _GestureType__interactive_viewer.pan;
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _onScaleStart(ScaleStartDetails details)
    {
        widget.onInteractionStart?.Invoke(details);
        if (_controller.isAnimating)
        {
            _controller.stop();
            _controller.reset();
            _animation?.removeListener(_handleInertiaAnimation);
            _animation = null;
        }
        if (_scaleController.isAnimating)
        {
            _scaleController.stop();
            _scaleController.reset();
            _scaleAnimation?.removeListener(_handleScaleAnimation);
            _scaleAnimation = null;
        }
        _gestureType = null;
        _currentAxis = null;
        _scaleStart = _transformer.value.getMaxScaleOnAxis();
        _referenceFocalPoint = _transformer.toScene(details.localFocalPoint);
        _rotationStart = _currentRotation;
    }

    internal virtual void _onScaleUpdate(ScaleUpdateDetails details)
    {
        double scaleLocal = _transformer.value.getMaxScaleOnAxis();
        _scaleAnimationFocalPoint = details.localFocalPoint;
        Offset focalPointScene = _transformer.toScene(details.localFocalPoint);
        if (Equals(_gestureType, _GestureType__interactive_viewer.pan))
        {
            _gestureType = _getGestureType(details);
        }
        else
        {
            _gestureType ??= _getGestureType(details);
        }
        if (!_gestureIsSupported(_gestureType))
        {
            widget.onInteractionUpdate?.Invoke(details);
            return;
        }
        switch (
            (
                _gestureType
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            )
        )
        {
            case _GestureType__interactive_viewer.scale:
            {
                DartRuntimePrimitives.Assert(() => _scaleStart is not null);
                double desiredScale =
                    (
                        _scaleStart
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ) * details.scale;
                double scaleChange = desiredScale / scaleLocal;
                _transformer.value = _matrixScale(_transformer.value, scaleChange);
                Offset focalPointSceneScaled = _transformer.toScene(details.localFocalPoint);
                _transformer.value = _matrixTranslate(
                    _transformer.value,
                    focalPointSceneScaled
                        - (
                            _referenceFocalPoint
                            ?? throw new global::System.NullReferenceException(
                                "Dart null assertion failed."
                            )
                        )
                );
                Offset focalPointSceneCheck = _transformer.toScene(details.localFocalPoint);
                if (
                    !Equals(
                        Interactive_viewerLibrary._round(
                            (
                                _referenceFocalPoint
                                ?? throw new global::System.NullReferenceException(
                                    "Dart null assertion failed."
                                )
                            )
                        ),
                        Interactive_viewerLibrary._round(focalPointSceneCheck)
                    )
                )
                {
                    _referenceFocalPoint = focalPointSceneCheck;
                }
                break;
            }
            case _GestureType__interactive_viewer.rotate:
            {
                if (details.rotation == 0.0)
                {
                    widget.onInteractionUpdate?.Invoke(details);
                    return;
                }
                double desiredRotation =
                    (
                        _rotationStart
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ) + details.rotation;
                _transformer.value = _matrixRotate(
                    _transformer.value,
                    _currentRotation - desiredRotation,
                    details.localFocalPoint
                );
                _currentRotation = desiredRotation;
                break;
            }
            case _GestureType__interactive_viewer.pan:
            {
                DartRuntimePrimitives.Assert(() => _referenceFocalPoint is not null);
                if (details.scale != 1.0)
                {
                    widget.onInteractionUpdate?.Invoke(details);
                    return;
                }
                _currentAxis ??= Interactive_viewerLibrary._getPanAxis(
                    (
                        _referenceFocalPoint
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ),
                    focalPointScene
                );
                Offset translationChange =
                    focalPointScene
                    - (
                        _referenceFocalPoint
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    );
                _transformer.value = _matrixTranslate(_transformer.value, translationChange);
                _referenceFocalPoint = _transformer.toScene(details.localFocalPoint);
                break;
            }
        }
        widget.onInteractionUpdate?.Invoke(details);
    }

    internal virtual void _onScaleEnd(ScaleEndDetails details)
    {
        widget.onInteractionEnd?.Invoke(details);
        _scaleStart = null;
        _rotationStart = null;
        _referenceFocalPoint = null;
        _animation?.removeListener(_handleInertiaAnimation);
        _scaleAnimation?.removeListener(_handleScaleAnimation);
        _controller.reset();
        _scaleController.reset();
        if (!_gestureIsSupported(_gestureType))
        {
            _currentAxis = null;
            return;
        }
        switch (_gestureType)
        {
            case _GestureType__interactive_viewer.pan:
            {
                if (
                    details.velocity.pixelsPerSecond.distance
                    < Gestures.ConstantsLibrary.kMinFlingVelocity
                )
                {
                    _currentAxis = null;
                    return;
                }
                Vector3 translationVector = _transformer.value.getTranslation();
                var translation = new Offset(translationVector.x, translationVector.y);
                var frictionSimulationX = new Physics.FrictionSimulation(
                    widget.interactionEndFrictionCoefficient,
                    translation.dx,
                    details.velocity.pixelsPerSecond.dx
                );
                var frictionSimulationY = new Physics.FrictionSimulation(
                    widget.interactionEndFrictionCoefficient,
                    translation.dy,
                    details.velocity.pixelsPerSecond.dy
                );
                double tFinal = Interactive_viewerLibrary._getFinalTime(
                    details.velocity.pixelsPerSecond.distance,
                    widget.interactionEndFrictionCoefficient
                );
                _animation = new Tween<Offset>(
                    begin: translation,
                    end: new Offset(frictionSimulationX.finalX, frictionSimulationY.finalX)
                )
                    .chain(new CurveTween(curve: Curves.decelerate))
                    .animate(_controller);
                _controller.duration = Duration.Create(milliseconds: (tFinal * 1000L).round());
                _animation!.addListener(_handleInertiaAnimation);
                _controller.forward();
                break;
            }
            case _GestureType__interactive_viewer.scale:
            {
                if (details.scaleVelocity.abs() < 0.1)
                {
                    _currentAxis = null;
                    return;
                }
                double scaleLocal = _transformer.value.getMaxScaleOnAxis();
                var frictionSimulation = new Physics.FrictionSimulation(
                    widget.interactionEndFrictionCoefficient * widget.scaleFactor,
                    scaleLocal,
                    details.scaleVelocity / 10L
                );
                double tFinalLocal = Interactive_viewerLibrary._getFinalTime(
                    details.scaleVelocity.abs(),
                    widget.interactionEndFrictionCoefficient,
                    effectivelyMotionless: 0.1
                );
                _scaleAnimation = new Tween<double>(
                    begin: scaleLocal,
                    end: frictionSimulation.x(tFinalLocal)
                )
                    .chain(new CurveTween(curve: Curves.decelerate))
                    .animate(_scaleController);
                _scaleController.duration = Duration.Create(
                    milliseconds: (tFinalLocal * 1000L).round()
                );
                _scaleAnimation!.addListener(_handleScaleAnimation);
                _scaleController.forward();
                break;
            }
            case _GestureType__interactive_viewer.rotate or null:
            {
                break;
            }
        }
    }

    internal virtual void _receivedPointerSignal(PointerSignalEvent @event)
    {
        Offset local = @event.localPosition;
        Offset @global = @event.position;
        double scaleChange = default!;
        if (@event is PointerScrollEvent)
        {
            PointerScrollEvent @event__as35966 = (PointerScrollEvent)@event;
            if (
                Equals(@event__as35966.kind, PointerDeviceKind.trackpad)
                && !widget.trackpadScrollCausesScale
            )
            {
                widget.onInteractionStart?.Invoke(
                    new ScaleStartDetails(focalPoint: @global, localFocalPoint: local)
                );
                Offset localDelta = PointerEvent.transformDeltaViaPositions(
                    untransformedEndPosition: @global + @event__as35966.scrollDelta,
                    untransformedDelta: @event__as35966.scrollDelta,
                    transform: @event__as35966.transform
                );
                if (!_gestureIsSupported(_GestureType__interactive_viewer.pan))
                {
                    widget.onInteractionUpdate?.Invoke(
                        new ScaleUpdateDetails(
                            focalPoint: @global - @event__as35966.scrollDelta,
                            localFocalPoint: local - @event__as35966.scrollDelta,
                            focalPointDelta: -localDelta
                        )
                    );
                    widget.onInteractionEnd?.Invoke(new ScaleEndDetails());
                    return;
                }
                Offset focalPointScene = _transformer.toScene(local);
                Offset newFocalPointScene = _transformer.toScene(local - localDelta);
                _transformer.value = _matrixTranslate(
                    _transformer.value,
                    newFocalPointScene - focalPointScene
                );
                widget.onInteractionUpdate?.Invoke(
                    new ScaleUpdateDetails(
                        focalPoint: @global - @event__as35966.scrollDelta,
                        localFocalPoint: local - localDelta,
                        focalPointDelta: -localDelta
                    )
                );
                widget.onInteractionEnd?.Invoke(new ScaleEndDetails());
                return;
            }
            if (@event__as35966.scrollDelta.dy == 0.0)
            {
                return;
            }
            scaleChange = Dart_mathLibrary.exp(
                -@event__as35966.scrollDelta.dy / widget.scaleFactor
            );
        }
        else
        {
            if (@event is PointerScaleEvent)
            {
                PointerScaleEvent @event__as37721 = (PointerScaleEvent)@event;
                scaleChange = @event__as37721.scale;
            }
            else
            {
                return;
            }
        }
        widget.onInteractionStart?.Invoke(
            new ScaleStartDetails(focalPoint: @global, localFocalPoint: local)
        );
        if (!_gestureIsSupported(_GestureType__interactive_viewer.scale))
        {
            widget.onInteractionUpdate?.Invoke(
                new ScaleUpdateDetails(
                    focalPoint: @global,
                    localFocalPoint: local,
                    scale: scaleChange
                )
            );
            widget.onInteractionEnd?.Invoke(new ScaleEndDetails());
            return;
        }
        Offset focalPointSceneLocal = _transformer.toScene(local);
        _transformer.value = _matrixScale(_transformer.value, scaleChange);
        Offset focalPointSceneScaled = _transformer.toScene(local);
        _transformer.value = _matrixTranslate(
            _transformer.value,
            focalPointSceneScaled - focalPointSceneLocal
        );
        widget.onInteractionUpdate?.Invoke(
            new ScaleUpdateDetails(focalPoint: @global, localFocalPoint: local, scale: scaleChange)
        );
        widget.onInteractionEnd?.Invoke(new ScaleEndDetails());
    }

    internal virtual void _handleInertiaAnimation()
    {
        if (!_controller.isAnimating)
        {
            _currentAxis = null;
            _animation?.removeListener(_handleInertiaAnimation);
            _animation = null;
            _controller.reset();
            return;
        }
        Vector3 translationVector = _transformer.value.getTranslation();
        var translation = new Offset(translationVector.x, translationVector.y);
        _transformer.value = _matrixTranslate(
            _transformer.value,
            _transformer.toScene(_animation!.value) - _transformer.toScene(translation)
        );
    }

    internal virtual void _handleScaleAnimation()
    {
        if (!_scaleController.isAnimating)
        {
            _currentAxis = null;
            _scaleAnimation?.removeListener(_handleScaleAnimation);
            _scaleAnimation = null;
            _scaleController.reset();
            return;
        }
        double desiredScale = _scaleAnimation!.value;
        double scaleChange = desiredScale / _transformer.value.getMaxScaleOnAxis();
        Offset referenceFocalPoint = _transformer.toScene(_scaleAnimationFocalPoint);
        _transformer.value = _matrixScale(_transformer.value, scaleChange);
        Offset focalPointSceneScaled = _transformer.toScene(_scaleAnimationFocalPoint);
        _transformer.value = _matrixTranslate(
            _transformer.value,
            focalPointSceneScaled - referenceFocalPoint
        );
    }

    internal virtual void _handleTransformation()
    {
        setState(() => { });
    }

    public override void initState()
    {
        base.initState();
        _controller = new AnimationController(vsync: this);
        _scaleController = new AnimationController(vsync: this);
        _transformer.addListener(_handleTransformation);
    }

    public override void didUpdateWidget(InteractiveViewer oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        TransformationController? newController = widget.transformationController;
        if (Equals(newController, oldWidget.transformationController))
        {
            return;
        }
        _transformer.removeListener(_handleTransformation);
        if (oldWidget.transformationController is null)
        {
            _transformer.dispose();
        }
        _transformer = newController ?? new TransformationController();
        _transformer.addListener(_handleTransformation);
    }

    public override void dispose()
    {
        _controller.dispose();
        _scaleController.dispose();
        _transformer.removeListener(_handleTransformation);
        if (widget.transformationController is null)
        {
            _transformer.dispose();
        }
        DartRuntimePrimitives.Assert(() =>
        {
            if (_tickers is not null)
            {
                foreach (Scheduler.Ticker ticker in _tickers!)
                {
                    if (ticker.isActive)
                    {
                        throw DartRuntimePrimitives.AsException(
                            new FlutterError(
                                new List<DiagnosticsNode>
                                {
                                    new ErrorSummary($"{this} was disposed with an active Ticker."),
                                    new ErrorDescription(
                                        $"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time "
                                            + "dispose() was called on the mixin, that Ticker was still active. All Tickers must "
                                            + "be disposed before calling super.dispose()."
                                    ),
                                    new ErrorHint(
                                        "Tickers used by AnimationControllers "
                                            + "should be disposed by calling dispose() on the AnimationController itself. "
                                            + "Otherwise, the ticker will leak."
                                    ),
                                    ticker.describeForError("The offending ticker was"),
                                }
                            )
                        );
                    }
                }
            }
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        Widget childLocal = default!;
        if (widget.child is not null)
        {
            childLocal = DartRuntimePrimitives.ConvertValue<Widget>(
                new _InteractiveViewerBuilt__interactive_viewer(
                    childKey: _childKey,
                    clipBehavior: widget.clipBehavior,
                    constrained: widget.constrained,
                    matrix: _transformer.value,
                    alignment: widget.alignment,
                    child: widget.child!
                )
            );
        }
        else
        {
            DartRuntimePrimitives.Assert(() => widget.builder is not null);
            DartRuntimePrimitives.Assert(() => !widget.constrained);
            childLocal = DartRuntimePrimitives.ConvertValue<Widget>(
                new LayoutBuilder(
                    builder: (context, constraints) =>
                    {
                        Matrix4 matrixLocal = _transformer.value;
                        return new _InteractiveViewerBuilt__interactive_viewer(
                            childKey: _childKey,
                            clipBehavior: widget.clipBehavior,
                            constrained: widget.constrained,
                            alignment: widget.alignment,
                            matrix: matrixLocal,
                            child: widget.builder!(
                                context,
                                Interactive_viewerLibrary._transformViewport(
                                    matrixLocal,
                                    Offset.zero & constraints.biggest
                                )
                            )
                        );
                        throw new InvalidOperationException(
                            "Dart closure completed without a value."
                        );
                    }
                )
            );
        }
        return new Listener(
            key: _parentKey,
            onPointerSignal: _receivedPointerSignal,
            child: new GestureDetector(
                behavior: HitTestBehavior.opaque,
                onScaleEnd: _onScaleEnd,
                onScaleStart: _onScaleStart,
                onScaleUpdate: _onScaleUpdate,
                trackpadScrollCausesScale: widget.trackpadScrollCausesScale,
                trackpadScrollToScaleFactor: new Offset(0, -1L / widget.scaleFactor),
                child: childLocal
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = (
            (Func<_WidgetTicker__ticker_provider>)(
                () =>
                {
                    var __cascade = new _WidgetTicker__ticker_provider(
                        onTick,
                        this,
                        debugLabel: Foundation.ConstantsLibrary.kDebugMode
                            ? $"created by {DiagnosticsLibrary.describeIdentity(this)}"
                            : null
                    );
                    __cascade.muted = !values.enabled;
                    __cascade.forceFrames = values.forceFrames;
                    return __cascade;
                }
            )
        )();
        _tickers!.Add(result);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => _tickers is not null);
        DartRuntimePrimitives.Assert(() => _tickers!.Contains(ticker));
        _tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if (_tickers is not null)
        {
            TickerModeData values = _tickerModeNotifier!.value;
            bool mutedLocal = !values.enabled;
            foreach (Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new DiagnosticsProperty<HashSet<Scheduler.Ticker>>(
                "tickers",
                _tickers,
                description: (_tickers is not null)
                    ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}"
                    : null,
                defaultValue: default
            )
        );
    }
}

internal class _InteractiveViewerBuilt__interactive_viewer : StatelessWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual GlobalKey<IState> childKey { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual bool constrained { get; private set; } = default!;
    public virtual Matrix4 matrix { get; private set; } = default!;
    public virtual Alignment? alignment { get; private set; }

    internal _InteractiveViewerBuilt__interactive_viewer(
        Widget child,
        GlobalKey<IState> childKey,
        Clip clipBehavior,
        bool constrained,
        Matrix4 matrix,
        Alignment? alignment
    )
    {
        this.child = child;
        this.childKey = childKey;
        this.clipBehavior = clipBehavior;
        this.constrained = constrained;
        this.matrix = matrix;
        this.alignment = alignment;
    }

    public override Widget build(BuildContext context)
    {
        Widget childLocal = new Transform(
            transform: matrix,
            alignment: alignment,
            child: new KeyedSubtree(key: childKey, child: child)
        );
        if (!constrained)
        {
            childLocal = DartRuntimePrimitives.ConvertValue<Widget>(
                new OverflowBox(
                    alignment: Alignment.topLeft,
                    minWidth: 0.0,
                    minHeight: 0.0,
                    maxWidth: double.PositiveInfinity,
                    maxHeight: double.PositiveInfinity,
                    child: childLocal
                )
            );
        }
        return new ClipRect(clipBehavior: clipBehavior, child: childLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class TransformationController : ValueNotifier<Matrix4>
{
    public TransformationController(Matrix4? value = null)
        : base(value ?? Matrix4.identity()) { }

    public virtual Offset toScene(Offset viewportPoint)
    {
        var inverseMatrix = Matrix4.inverted(value);
        Vector3 untransformed = inverseMatrix.transform3(
            new Vector3(viewportPoint.dx, viewportPoint.dy, 0)
        );
        return new Offset(untransformed.x, untransformed.y);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal enum _GestureType__interactive_viewer
{
    pan,
    scale,
    rotate,
}

public static partial class Interactive_viewerLibrary
{
    internal static double _getFinalTime(
        double velocity,
        double drag,
        double effectivelyMotionless = 10
    )
    {
        return Dart_mathLibrary.log(effectivelyMotionless / velocity)
            / Dart_mathLibrary.log(drag / 100L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Interactive_viewerLibrary
{
    internal static Offset _getMatrixTranslation(Matrix4 matrix)
    {
        Vector3 nextTranslation = matrix.getTranslation();
        return new Offset(nextTranslation.x, nextTranslation.y);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Interactive_viewerLibrary
{
    internal static Quad _transformViewport(Matrix4 matrix, Rect viewport)
    {
        Matrix4 inverseMatrix = (
            (Func<Matrix4>)(
                () =>
                {
                    var __cascade = matrix.clone();
                    __cascade.invert();
                    return __cascade;
                }
            )
        )();
        return new Quad(
            inverseMatrix.transform3(new Vector3(viewport.topLeft.dx, viewport.topLeft.dy, 0.0)),
            inverseMatrix.transform3(new Vector3(viewport.topRight.dx, viewport.topRight.dy, 0.0)),
            inverseMatrix.transform3(
                new Vector3(viewport.bottomRight.dx, viewport.bottomRight.dy, 0.0)
            ),
            inverseMatrix.transform3(
                new Vector3(viewport.bottomLeft.dx, viewport.bottomLeft.dy, 0.0)
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Interactive_viewerLibrary
{
    internal static Quad _getAxisAlignedBoundingBoxWithRotation(Rect rect, double rotation)
    {
        var rotationMatrix = (
            (Func<Matrix4>)(
                () =>
                {
                    var __cascade = Matrix4.identity();
                    __cascade.translateByDouble(rect.size.width / 2L, rect.size.height / 2L, 0, 1);
                    __cascade.rotateZ(rotation);
                    __cascade.translateByDouble(
                        -rect.size.width / 2L,
                        -rect.size.height / 2L,
                        0,
                        1
                    );
                    return __cascade;
                }
            )
        )();
        var boundariesRotated = new Quad(
            rotationMatrix.transform3(new Vector3(rect.left, rect.top, 0.0)),
            rotationMatrix.transform3(new Vector3(rect.right, rect.top, 0.0)),
            rotationMatrix.transform3(new Vector3(rect.right, rect.bottom, 0.0)),
            rotationMatrix.transform3(new Vector3(rect.left, rect.bottom, 0.0))
        );
        return InteractiveViewer.getAxisAlignedBoundingBox(boundariesRotated);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Interactive_viewerLibrary
{
    internal static Offset _exceedsBy(Quad boundary, Quad viewport)
    {
        var viewportPoints = new List<Vector3>
        {
            viewport.point0,
            viewport.point1,
            viewport.point2,
            viewport.point3,
        };
        Offset largestExcess = Offset.zero;
        foreach (var point in viewportPoints)
        {
            Vector3 pointInside = InteractiveViewer.getNearestPointInside(point, boundary);
            var excess = new Offset(pointInside.x - point.x, pointInside.y - point.y);
            if (excess.dx.abs() > largestExcess.dx.abs())
            {
                largestExcess = new Offset(excess.dx, largestExcess.dy);
            }
            if (excess.dy.abs() > largestExcess.dy.abs())
            {
                largestExcess = new Offset(largestExcess.dx, excess.dy);
            }
        }
        return _round(largestExcess);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Interactive_viewerLibrary
{
    internal static Offset _round(Offset offset)
    {
        return new Offset(
            double.Parse(
                offset.dx.toStringAsFixed(9L),
                System.Globalization.CultureInfo.InvariantCulture
            ),
            double.Parse(
                offset.dy.toStringAsFixed(9L),
                System.Globalization.CultureInfo.InvariantCulture
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Interactive_viewerLibrary
{
    internal static Offset _alignAxis(Offset offset, Axis axis)
    {
        return axis switch
        {
            Axis.horizontal => new Offset(offset.dx, 0.0),
            Axis.vertical => new Offset(0.0, offset.dy),
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Interactive_viewerLibrary
{
    internal static Axis? _getPanAxis(Offset point1, Offset point2)
    {
        if (Equals(point1, point2))
        {
            return null;
        }
        double x = point2.dx - point1.dx;
        double y = point2.dy - point1.dy;
        return (x.abs() > y.abs()) ? Axis.horizontal : Axis.vertical;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public enum PanAxis
{
    horizontal,
    vertical,
    aligned,
    free,
}
