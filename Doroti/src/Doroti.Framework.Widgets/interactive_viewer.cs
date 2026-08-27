// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/interactive_viewer.dart
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

public delegate Widget InteractiveViewerWidgetBuilder(BuildContext context, global::Doroti.Ui.Quad viewport);

public class InteractiveViewer : StatefulWidget
{
    public virtual global::Doroti.Framework.Painting.Alignment? alignment { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual PanAxis panAxis { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsets boundaryMargin { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, global::Doroti.Ui.Quad, Widget>? builder { get; private set; }
    public virtual Widget? child { get; private set; }
    public virtual bool constrained { get; private set; } = default!;
    public virtual bool panEnabled { get; private set; } = default!;
    public virtual bool scaleEnabled { get; private set; } = default!;
    public virtual bool trackpadScrollCausesScale { get; private set; } = default!;
    public virtual double scaleFactor { get; private set; } = default!;
    public virtual double maxScale { get; private set; } = default!;
    public virtual double minScale { get; private set; } = default!;
    public virtual double interactionEndFrictionCoefficient { get; private set; } = default!;
    public virtual global::System.Action<global::Doroti.Framework.Gestures.ScaleEndDetails>? onInteractionEnd { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.ScaleStartDetails>? onInteractionStart { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.ScaleUpdateDetails>? onInteractionUpdate { get; private set; }
    public virtual TransformationController? transformationController { get; private set; }
    internal const double _kDrag = 0.0000135;

    public InteractiveViewer(global::Doroti.Framework.Foundation.Key? key = null, Clip clipBehavior = Clip.hardEdge, PanAxis panAxis = PanAxis.free, global::Doroti.Framework.Painting.EdgeInsets boundaryMargin = default!, bool constrained = true, double maxScale = 2.5, double minScale = 0.8, double? interactionEndFrictionCoefficient = null, global::System.Action<global::Doroti.Framework.Gestures.ScaleEndDetails>? onInteractionEnd = null, global::System.Action<global::Doroti.Framework.Gestures.ScaleStartDetails>? onInteractionStart = null, global::System.Action<global::Doroti.Framework.Gestures.ScaleUpdateDetails>? onInteractionUpdate = null, bool panEnabled = true, bool scaleEnabled = true, double? scaleFactor = null, TransformationController? transformationController = null, global::Doroti.Framework.Painting.Alignment? alignment = null, bool trackpadScrollCausesScale = false, Widget child = default!) : base(key: key)
    {
        global::Doroti.Framework.Painting.EdgeInsets __boundaryMargin = boundaryMargin ?? global::Doroti.Framework.Painting.EdgeInsets.zero;
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
        this.builder = null;
        System.Diagnostics.Debug.Assert((minScale > 0L));
        System.Diagnostics.Debug.Assert((__interactionEndFrictionCoefficient > 0L));
        System.Diagnostics.Debug.Assert(double.IsFinite(minScale));
        System.Diagnostics.Debug.Assert((maxScale > 0L));
        System.Diagnostics.Debug.Assert(!double.IsNaN(maxScale));
        System.Diagnostics.Debug.Assert((maxScale >= minScale));
        System.Diagnostics.Debug.Assert((((double.IsInfinity(__boundaryMargin.horizontal) && double.IsInfinity(__boundaryMargin.vertical))) || ((((double.IsFinite(((global::Doroti.Framework.Painting.EdgeInsets)__boundaryMargin).top) && double.IsFinite(((global::Doroti.Framework.Painting.EdgeInsets)__boundaryMargin).right)) && double.IsFinite(((global::Doroti.Framework.Painting.EdgeInsets)__boundaryMargin).bottom)) && double.IsFinite(((global::Doroti.Framework.Painting.EdgeInsets)__boundaryMargin).left)))));
    }

    public static InteractiveViewer CreateBuilder(global::Doroti.Framework.Foundation.Key? key = null, Clip clipBehavior = Clip.hardEdge, PanAxis panAxis = PanAxis.free, global::Doroti.Framework.Painting.EdgeInsets boundaryMargin = default!, double maxScale = 2.5, double minScale = 0.8, double? interactionEndFrictionCoefficient = null, global::System.Action<global::Doroti.Framework.Gestures.ScaleEndDetails>? onInteractionEnd = null, global::System.Action<global::Doroti.Framework.Gestures.ScaleStartDetails>? onInteractionStart = null, global::System.Action<global::Doroti.Framework.Gestures.ScaleUpdateDetails>? onInteractionUpdate = null, bool panEnabled = true, bool scaleEnabled = true, double scaleFactor = 200.0, TransformationController? transformationController = null, global::Doroti.Framework.Painting.Alignment? alignment = null, bool trackpadScrollCausesScale = false, global::System.Func<BuildContext, global::Doroti.Ui.Quad, Widget> builder = default!)
    {
        var __instance = new InteractiveViewer(default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!);
        global::Doroti.Framework.Painting.EdgeInsets __boundaryMargin = boundaryMargin ?? global::Doroti.Framework.Painting.EdgeInsets.zero;
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
        double lengthSquared = (global::Doroti.Runtime.Dart_mathLibrary.pow((l2.x - l1.x), 2.0).toDouble() + global::Doroti.Runtime.Dart_mathLibrary.pow((l2.y - l1.y), 2.0).toDouble());
        if ((lengthSquared == 0L))
        {
            return l1;
        }
        Vector3 l1P = (point - l1);
        Vector3 l1L2 = (l2 - l1);
        double fraction = Dart_uiLibrary.clampDouble((l1P.dot(l1L2) / lengthSquared), 0.0, 1.0);
        return (l1 + (l1L2 * fraction));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Ui.Quad getAxisAlignedBoundingBox(global::Doroti.Ui.Quad quad)
    {
        double minX = Math.Min(quad.point0.x, Math.Min(quad.point1.x, Math.Min(quad.point2.x, quad.point3.x)));
        double minY = Math.Min(quad.point0.y, Math.Min(quad.point1.y, Math.Min(quad.point2.y, quad.point3.y)));
        double maxX = Math.Max(quad.point0.x, Math.Max(quad.point1.x, Math.Max(quad.point2.x, quad.point3.x)));
        double maxY = Math.Max(quad.point0.y, Math.Max(quad.point1.y, Math.Max(quad.point2.y, quad.point3.y)));
        return new global::Doroti.Ui.Quad(new Vector3(minX, minY, 0), new Vector3(maxX, minY, 0), new Vector3(maxX, maxY, 0), new Vector3(minX, maxY, 0));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool pointIsInside(Vector3 point, global::Doroti.Ui.Quad quad)
    {
        Vector3 aM = (point - quad.point0);
        Vector3 aB = (quad.point1 - quad.point0);
        Vector3 aD = (quad.point3 - quad.point0);
        double aMAB = aM.dot(aB);
        double aBAB = aB.dot(aB);
        double aMAD = aM.dot(aD);
        double aDAD = aD.dot(aD);
        return ((((0L <= aMAB) && (aMAB <= aBAB)) && (0L <= aMAD)) && (aMAD <= aDAD));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Vector3 getNearestPointInside(Vector3 point, global::Doroti.Ui.Quad quad)
    {
        if (InteractiveViewer.pointIsInside(point, quad))
        {
            return point;
        }
        var closestPoints = new List<Vector3> { InteractiveViewer.getNearestPointOnLine(point, quad.point0, quad.point1), InteractiveViewer.getNearestPointOnLine(point, quad.point1, quad.point2), InteractiveViewer.getNearestPointOnLine(point, quad.point2, quad.point3), InteractiveViewer.getNearestPointOnLine(point, quad.point3, quad.point0) };
        double minDistance = double.PositiveInfinity;
        Vector3 closestOverall = default!;
        foreach (var closePoint in closestPoints)
        {
            double distance = global::Doroti.Runtime.Dart_mathLibrary.sqrt((global::Doroti.Runtime.Dart_mathLibrary.pow((point.x - closePoint.x), 2L) + global::Doroti.Runtime.Dart_mathLibrary.pow((point.y - closePoint.y), 2L)));
            if ((distance < minDistance))
            {
                minDistance = distance;
                closestOverall = closePoint;
            }
        }
        return closestOverall;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _InteractiveViewerState__interactive_viewer());
}

internal class _InteractiveViewerState__interactive_viewer : State<InteractiveViewer>, TickerProviderStateMixin<InteractiveViewer>
{
    private bool __late__transformer_initialized;
    private TransformationController __late__transformer = default!;
    internal virtual TransformationController _transformer
    {
        get
        {
            if (!__late__transformer_initialized)
            {
                __late__transformer = (((InteractiveViewer)this.widget).transformationController ?? new TransformationController());
                __late__transformer_initialized = true;
            }
            return __late__transformer;
        }
        set { __late__transformer = value; __late__transformer_initialized = true; }
    }
    internal virtual GlobalKey<IState> _childKey { get; private set; } = GlobalKey<IState>.Create();
    internal virtual GlobalKey<IState> _parentKey { get; private set; } = GlobalKey<IState>.Create();
    internal virtual global::Doroti.Framework.Animation.Animation<Offset>? _animation { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.Animation<double>? _scaleAnimation { get; set; } = default;
    internal virtual Offset _scaleAnimationFocalPoint { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.AnimationController _controller { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.AnimationController _scaleController { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.Axis? _currentAxis { get; set; } = default;
    internal virtual Offset? _referenceFocalPoint { get; set; } = default;
    internal virtual double? _scaleStart { get; set; } = default;
    internal virtual double? _rotationStart { get; set; } = 0.0;
    internal virtual double _currentRotation { get; set; } = 0.0;
    internal virtual _GestureType__interactive_viewer? _gestureType { get; set; } = default;
    internal virtual bool _rotateEnabled { get; private set; } = false;
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual global::Doroti.Ui.Rect _boundaryRect
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (((GlobalKey<IState>)this._childKey).currentContext is not null));
            DartRuntimePrimitives.Assert(() => !double.IsNaN(((InteractiveViewer)this.widget).boundaryMargin.left));
            DartRuntimePrimitives.Assert(() => !double.IsNaN(((InteractiveViewer)this.widget).boundaryMargin.right));
            DartRuntimePrimitives.Assert(() => !double.IsNaN(((InteractiveViewer)this.widget).boundaryMargin.top));
            DartRuntimePrimitives.Assert(() => !double.IsNaN(((InteractiveViewer)this.widget).boundaryMargin.bottom));
            var childRenderBox = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((GlobalKey<IState>)this._childKey).currentContext!.findRenderObject()!)!;
            global::Doroti.Ui.Size childSize = ((global::Doroti.Ui.Size)(object?)((global::Doroti.Framework.Rendering.RenderBox)childRenderBox).size);
            global::Doroti.Ui.Rect boundaryRect = ((global::Doroti.Ui.Rect)(object?)((InteractiveViewer)this.widget).boundaryMargin.inflateRect((Offset.zero & childSize)));
            DartRuntimePrimitives.Assert(() => !boundaryRect.isEmpty, () => (object?)"InteractiveViewer's child must have nonzero dimensions.");
            DartRuntimePrimitives.Assert(() => (boundaryRect.isFinite || ((((double.IsInfinity(boundaryRect.left) && double.IsInfinity(boundaryRect.top)) && double.IsInfinity(boundaryRect.right)) && double.IsInfinity(boundaryRect.bottom)))), () => (object?)"boundaryRect must either be infinite in all directions or finite in all directions.");
            return boundaryRect;
            return default!;
        }
    }
    internal virtual global::Doroti.Ui.Rect _viewport
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (((GlobalKey<IState>)this._parentKey).currentContext is not null));
            var parentRenderBox = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((GlobalKey<IState>)this._parentKey).currentContext!.findRenderObject()!)!;
            return (Offset.zero & ((global::Doroti.Framework.Rendering.RenderBox)parentRenderBox).size);
            return default!;
        }
    }
    internal virtual Matrix4 _matrixTranslate(Matrix4 matrix, Offset translation)
    {
        if ((object.Equals(translation, Offset.zero)))
        {
            return matrix.clone();
        }
        global::Doroti.Ui.Offset alignedTranslation = default!;
        if ((this._currentAxis is not null))
        {
            alignedTranslation = (((InteractiveViewer)this.widget).panAxis switch { PanAxis.horizontal => Interactive_viewerLibrary._alignAxis(translation, global::Doroti.Framework.Painting.Axis.horizontal), PanAxis.vertical => Interactive_viewerLibrary._alignAxis(translation, global::Doroti.Framework.Painting.Axis.vertical), PanAxis.aligned => Interactive_viewerLibrary._alignAxis(translation, DartRuntimePrimitives.RequireValue(this._currentAxis)), PanAxis.free => translation, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        }
        else
        {
            alignedTranslation = translation;
        }
        Matrix4 nextMatrix = ((Func<Matrix4>)(() =>
{
    var __cascade = matrix.clone();
    __cascade.translateByDouble(alignedTranslation.dx, alignedTranslation.dy, 0, 1);
    return __cascade;
}))();
        global::Doroti.Ui.Quad nextViewport = Interactive_viewerLibrary._transformViewport(nextMatrix, this._viewport);
        if (this._boundaryRect.isInfinite)
        {
            return nextMatrix;
        }
        global::Doroti.Ui.Quad boundariesAabbQuad = Interactive_viewerLibrary._getAxisAlignedBoundingBoxWithRotation(this._boundaryRect, this._currentRotation);
        global::Doroti.Ui.Offset offendingDistance = ((global::Doroti.Ui.Offset)(object?)Interactive_viewerLibrary._exceedsBy(boundariesAabbQuad, nextViewport));
        if ((object.Equals(offendingDistance, Offset.zero)))
        {
            return nextMatrix;
        }
        global::Doroti.Ui.Offset nextTotalTranslation = ((global::Doroti.Ui.Offset)(object?)Interactive_viewerLibrary._getMatrixTranslation(nextMatrix));
        double currentScale = matrix.getMaxScaleOnAxis();
        var correctedTotalTranslation = new global::Doroti.Ui.Offset((nextTotalTranslation.dx - (offendingDistance.dx * currentScale)), (nextTotalTranslation.dy - (offendingDistance.dy * currentScale)));
        Matrix4 correctedMatrix = ((Func<Matrix4>)(() =>
{
    var __cascade = matrix.clone();
    __cascade.setTranslation(new Vector3(correctedTotalTranslation.dx, correctedTotalTranslation.dy, 0.0));
    return __cascade;
}))();
        global::Doroti.Ui.Quad correctedViewport = Interactive_viewerLibrary._transformViewport(correctedMatrix, this._viewport);
        global::Doroti.Ui.Offset offendingCorrectedDistance = ((global::Doroti.Ui.Offset)(object?)Interactive_viewerLibrary._exceedsBy(boundariesAabbQuad, correctedViewport));
        if ((object.Equals(offendingCorrectedDistance, Offset.zero)))
        {
            return correctedMatrix;
        }
        if (((offendingCorrectedDistance.dx != 0.0) && (offendingCorrectedDistance.dy != 0.0)))
        {
            return matrix.clone();
        }
        var unidirectionalCorrectedTotalTranslation = new global::Doroti.Ui.Offset(((offendingCorrectedDistance.dx == 0.0) ? correctedTotalTranslation.dx : 0.0), ((offendingCorrectedDistance.dy == 0.0) ? correctedTotalTranslation.dy : 0.0));
        return ((Func<Matrix4>)(() =>
{
    var __cascade = matrix.clone();
    __cascade.setTranslation(new Vector3(unidirectionalCorrectedTotalTranslation.dx, unidirectionalCorrectedTotalTranslation.dy, 0.0));
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Matrix4 _matrixScale(Matrix4 matrix, double scale)
    {
        if ((scale == 1.0))
        {
            return matrix.clone();
        }
        DartRuntimePrimitives.Assert(() => (scale != 0.0));
        double currentScale = this._transformer.value.getMaxScaleOnAxis();
        double totalScale = Math.Max((currentScale * scale), Math.Max((this._viewport.width / this._boundaryRect.width), (this._viewport.height / this._boundaryRect.height)));
        double clampedTotalScale = Dart_uiLibrary.clampDouble(totalScale, ((InteractiveViewer)this.widget).minScale, ((InteractiveViewer)this.widget).maxScale);
        double clampedScale = (clampedTotalScale / currentScale);
        return ((Func<Matrix4>)(() =>
{
    var __cascade = matrix.clone();
    __cascade.scaleByDouble(clampedScale, clampedScale, clampedScale, 1);
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Matrix4 _matrixRotate(Matrix4 matrix, double rotation, Offset focalPoint)
    {
        if ((rotation == 0L))
        {
            return matrix.clone();
        }
        global::Doroti.Ui.Offset focalPointScene = ((global::Doroti.Ui.Offset)(object?)this._transformer.toScene(focalPoint));
        return ((Func<Matrix4>)(() =>
{
    var __cascade = matrix.clone();
    __cascade.translateByDouble(focalPointScene.dx, focalPointScene.dy, 0, 1);
    __cascade.rotateZ(-rotation);
    __cascade.translateByDouble(-focalPointScene.dx, -focalPointScene.dy, 0, 1);
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _gestureIsSupported(_GestureType__interactive_viewer? gestureType)
    {
        return (gestureType switch { _GestureType__interactive_viewer.rotate => this._rotateEnabled, _GestureType__interactive_viewer.scale => ((InteractiveViewer)this.widget).scaleEnabled, _GestureType__interactive_viewer.pan => ((InteractiveViewer)this.widget).panEnabled, null => ((InteractiveViewer)this.widget).panEnabled, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual _GestureType__interactive_viewer _getGestureType(global::Doroti.Framework.Gestures.ScaleUpdateDetails details)
    {
        double scaleLocal = (!((InteractiveViewer)this.widget).scaleEnabled ? 1.0 : ((global::Doroti.Framework.Gestures.ScaleUpdateDetails)details).scale);
        double rotationLocal = (!this._rotateEnabled ? 0.0 : ((global::Doroti.Framework.Gestures.ScaleUpdateDetails)details).rotation);
        if ((((scaleLocal - 1L)).abs() > rotationLocal.abs()))
        {
            return _GestureType__interactive_viewer.scale;
        }
        else
        {
            if ((rotationLocal != 0.0))
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

    internal virtual void _onScaleStart(global::Doroti.Framework.Gestures.ScaleStartDetails details)
    {
        ((InteractiveViewer)this.widget).onInteractionStart?.Invoke(details);
        if (((global::Doroti.Framework.Animation.AnimationController)this._controller).isAnimating)
        {
            this._controller.stop();
            this._controller.reset();
            this._animation?.removeListener(() => this._handleInertiaAnimation());
            _animation = null;
        }
        if (((global::Doroti.Framework.Animation.AnimationController)this._scaleController).isAnimating)
        {
            this._scaleController.stop();
            this._scaleController.reset();
            this._scaleAnimation?.removeListener(() => this._handleScaleAnimation());
            _scaleAnimation = null;
        }
        _gestureType = null;
        _currentAxis = null;
        _scaleStart = this._transformer.value.getMaxScaleOnAxis();
        _referenceFocalPoint = this._transformer.toScene(((global::Doroti.Framework.Gestures.ScaleStartDetails)details).localFocalPoint);
        _rotationStart = this._currentRotation;
    }

    internal virtual void _onScaleUpdate(global::Doroti.Framework.Gestures.ScaleUpdateDetails details)
    {
        double scaleLocal = this._transformer.value.getMaxScaleOnAxis();
        _scaleAnimationFocalPoint = ((global::Doroti.Framework.Gestures.ScaleUpdateDetails)details).localFocalPoint;
        global::Doroti.Ui.Offset focalPointScene = ((global::Doroti.Ui.Offset)(object?)this._transformer.toScene(((global::Doroti.Framework.Gestures.ScaleUpdateDetails)details).localFocalPoint));
        if ((object.Equals(this._gestureType, _GestureType__interactive_viewer.pan)))
        {
            _gestureType = _getGestureType(details);
        }
        else
        {
            _gestureType ??= _getGestureType(details);
        }
        if (!_gestureIsSupported(this._gestureType))
        {
            ((InteractiveViewer)this.widget).onInteractionUpdate?.Invoke(details);
            return;
        }
        switch (DartRuntimePrimitives.RequireValue(this._gestureType))
        {
            case _GestureType__interactive_viewer.scale:
                {
                    DartRuntimePrimitives.Assert(() => (this._scaleStart is not null));
                    double desiredScale = (DartRuntimePrimitives.RequireValue(this._scaleStart) * ((global::Doroti.Framework.Gestures.ScaleUpdateDetails)details).scale);
                    double scaleChange = (desiredScale / scaleLocal);
                    this._transformer.value = _matrixScale(this._transformer.value, scaleChange);
                    global::Doroti.Ui.Offset focalPointSceneScaled = ((global::Doroti.Ui.Offset)(object?)this._transformer.toScene(((global::Doroti.Framework.Gestures.ScaleUpdateDetails)details).localFocalPoint));
                    this._transformer.value = _matrixTranslate(this._transformer.value, (focalPointSceneScaled - DartRuntimePrimitives.RequireValue(this._referenceFocalPoint)));
                    global::Doroti.Ui.Offset focalPointSceneCheck = ((global::Doroti.Ui.Offset)(object?)this._transformer.toScene(((global::Doroti.Framework.Gestures.ScaleUpdateDetails)details).localFocalPoint));
                    if ((!object.Equals(Interactive_viewerLibrary._round(DartRuntimePrimitives.RequireValue(this._referenceFocalPoint)), Interactive_viewerLibrary._round(focalPointSceneCheck))))
                    {
                        _referenceFocalPoint = focalPointSceneCheck;
                    }
                    break;
                }
            case _GestureType__interactive_viewer.rotate:
                {
                    if ((((global::Doroti.Framework.Gestures.ScaleUpdateDetails)details).rotation == 0.0))
                    {
                        ((InteractiveViewer)this.widget).onInteractionUpdate?.Invoke(details);
                        return;
                    }
                    double desiredRotation = (DartRuntimePrimitives.RequireValue(this._rotationStart) + ((global::Doroti.Framework.Gestures.ScaleUpdateDetails)details).rotation);
                    this._transformer.value = _matrixRotate(this._transformer.value, (this._currentRotation - desiredRotation), ((global::Doroti.Framework.Gestures.ScaleUpdateDetails)details).localFocalPoint);
                    _currentRotation = desiredRotation;
                    break;
                }
            case _GestureType__interactive_viewer.pan:
                {
                    DartRuntimePrimitives.Assert(() => (this._referenceFocalPoint is not null));
                    if ((((global::Doroti.Framework.Gestures.ScaleUpdateDetails)details).scale != 1.0))
                    {
                        ((InteractiveViewer)this.widget).onInteractionUpdate?.Invoke(details);
                        return;
                    }
                    _currentAxis ??= Interactive_viewerLibrary._getPanAxis(DartRuntimePrimitives.RequireValue(this._referenceFocalPoint), focalPointScene);
                    global::Doroti.Ui.Offset translationChange = ((global::Doroti.Ui.Offset)(object?)(focalPointScene - DartRuntimePrimitives.RequireValue(this._referenceFocalPoint)));
                    this._transformer.value = _matrixTranslate(this._transformer.value, translationChange);
                    _referenceFocalPoint = this._transformer.toScene(((global::Doroti.Framework.Gestures.ScaleUpdateDetails)details).localFocalPoint);
                    break;
                }
        }
        ((InteractiveViewer)this.widget).onInteractionUpdate?.Invoke(details);
    }

    internal virtual void _onScaleEnd(global::Doroti.Framework.Gestures.ScaleEndDetails details)
    {
        ((InteractiveViewer)this.widget).onInteractionEnd?.Invoke(details);
        _scaleStart = null;
        _rotationStart = null;
        _referenceFocalPoint = null;
        this._animation?.removeListener(() => this._handleInertiaAnimation());
        this._scaleAnimation?.removeListener(() => this._handleScaleAnimation());
        this._controller.reset();
        this._scaleController.reset();
        if (!_gestureIsSupported(this._gestureType))
        {
            _currentAxis = null;
            return;
        }
        switch (this._gestureType)
        {
            case _GestureType__interactive_viewer.pan:
                {
                    if ((((global::Doroti.Framework.Gestures.ScaleEndDetails)details).velocity.pixelsPerSecond.distance < global::Doroti.Framework.Gestures.ConstantsLibrary.kMinFlingVelocity))
                    {
                        _currentAxis = null;
                        return;
                    }
                    Vector3 translationVector = this._transformer.value.getTranslation();
                    var translation = new global::Doroti.Ui.Offset(translationVector.x, translationVector.y);
                    var frictionSimulationX = new global::Doroti.Framework.Physics.FrictionSimulation(((InteractiveViewer)this.widget).interactionEndFrictionCoefficient, translation.dx, ((global::Doroti.Framework.Gestures.ScaleEndDetails)details).velocity.pixelsPerSecond.dx);
                    var frictionSimulationY = new global::Doroti.Framework.Physics.FrictionSimulation(((InteractiveViewer)this.widget).interactionEndFrictionCoefficient, translation.dy, ((global::Doroti.Framework.Gestures.ScaleEndDetails)details).velocity.pixelsPerSecond.dy);
                    double tFinal = Interactive_viewerLibrary._getFinalTime(((global::Doroti.Framework.Gestures.ScaleEndDetails)details).velocity.pixelsPerSecond.distance, ((InteractiveViewer)this.widget).interactionEndFrictionCoefficient);
                    _animation = new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: translation, end: new global::Doroti.Ui.Offset(((global::Doroti.Framework.Physics.FrictionSimulation)frictionSimulationX).finalX, ((global::Doroti.Framework.Physics.FrictionSimulation)frictionSimulationY).finalX)).chain(new global::Doroti.Framework.Animation.CurveTween(curve: global::Doroti.Framework.Animation.Curves.decelerate)).animate(this._controller);
                    this._controller.duration = Duration.Create(milliseconds: ((tFinal * 1000L)).round());
                    this._animation!.addListener(() => this._handleInertiaAnimation());
                    this._controller.forward();
                    break;
                }
            case _GestureType__interactive_viewer.scale:
                {
                    if ((((global::Doroti.Framework.Gestures.ScaleEndDetails)details).scaleVelocity.abs() < 0.1))
                    {
                        _currentAxis = null;
                        return;
                    }
                    double scaleLocal = this._transformer.value.getMaxScaleOnAxis();
                    var frictionSimulation = new global::Doroti.Framework.Physics.FrictionSimulation((((InteractiveViewer)this.widget).interactionEndFrictionCoefficient * ((InteractiveViewer)this.widget).scaleFactor), scaleLocal, (((global::Doroti.Framework.Gestures.ScaleEndDetails)details).scaleVelocity / 10L));
                    double tFinalLocal = Interactive_viewerLibrary._getFinalTime(((global::Doroti.Framework.Gestures.ScaleEndDetails)details).scaleVelocity.abs(), ((InteractiveViewer)this.widget).interactionEndFrictionCoefficient, effectivelyMotionless: 0.1);
                    _scaleAnimation = new global::Doroti.Framework.Animation.Tween<double>(begin: scaleLocal, end: frictionSimulation.x(tFinalLocal)).chain(new global::Doroti.Framework.Animation.CurveTween(curve: global::Doroti.Framework.Animation.Curves.decelerate)).animate(this._scaleController);
                    this._scaleController.duration = Duration.Create(milliseconds: ((tFinalLocal * 1000L)).round());
                    this._scaleAnimation!.addListener(() => this._handleScaleAnimation());
                    this._scaleController.forward();
                    break;
                }
            case _GestureType__interactive_viewer.rotate or null:
                {
                    break;
                }
        }
    }

    internal virtual void _receivedPointerSignal(global::Doroti.Framework.Gestures.PointerSignalEvent @event)
    {
        global::Doroti.Ui.Offset local = ((global::Doroti.Ui.Offset)(object?)@event.localPosition);
        global::Doroti.Ui.Offset @global = ((global::Doroti.Ui.Offset)(object?)@event.position);
        double scaleChange = default!;
        if ((@event is global::Doroti.Framework.Gestures.PointerScrollEvent))
        {
            global::Doroti.Framework.Gestures.PointerScrollEvent @event__as35966 = (global::Doroti.Framework.Gestures.PointerScrollEvent)@event;
            if (((object.Equals(((global::Doroti.Framework.Gestures.PointerScrollEvent)@event__as35966).kind, PointerDeviceKind.trackpad)) && !((InteractiveViewer)this.widget).trackpadScrollCausesScale))
            {
                ((InteractiveViewer)this.widget).onInteractionStart?.Invoke(new global::Doroti.Framework.Gestures.ScaleStartDetails(focalPoint: @global, localFocalPoint: local));
                global::Doroti.Ui.Offset localDelta = ((global::Doroti.Ui.Offset)(object?)PointerEvent.transformDeltaViaPositions(untransformedEndPosition: (@global + ((global::Doroti.Framework.Gestures.PointerScrollEvent)((global::Doroti.Framework.Gestures.PointerScrollEvent)@event__as35966)).scrollDelta), untransformedDelta: ((global::Doroti.Framework.Gestures.PointerScrollEvent)((global::Doroti.Framework.Gestures.PointerScrollEvent)@event__as35966)).scrollDelta, transform: ((global::Doroti.Framework.Gestures.PointerScrollEvent)@event__as35966).transform));
                if (!_gestureIsSupported(_GestureType__interactive_viewer.pan))
                {
                    ((InteractiveViewer)this.widget).onInteractionUpdate?.Invoke(new global::Doroti.Framework.Gestures.ScaleUpdateDetails(focalPoint: (@global - ((global::Doroti.Framework.Gestures.PointerScrollEvent)((global::Doroti.Framework.Gestures.PointerScrollEvent)@event__as35966)).scrollDelta), localFocalPoint: (local - ((global::Doroti.Framework.Gestures.PointerScrollEvent)((global::Doroti.Framework.Gestures.PointerScrollEvent)@event__as35966)).scrollDelta), focalPointDelta: -localDelta));
                    ((InteractiveViewer)this.widget).onInteractionEnd?.Invoke(new global::Doroti.Framework.Gestures.ScaleEndDetails());
                    return;
                }
                global::Doroti.Ui.Offset focalPointScene = ((global::Doroti.Ui.Offset)(object?)this._transformer.toScene(local));
                global::Doroti.Ui.Offset newFocalPointScene = ((global::Doroti.Ui.Offset)(object?)this._transformer.toScene((local - localDelta)));
                this._transformer.value = _matrixTranslate(this._transformer.value, (newFocalPointScene - focalPointScene));
                ((InteractiveViewer)this.widget).onInteractionUpdate?.Invoke(new global::Doroti.Framework.Gestures.ScaleUpdateDetails(focalPoint: (@global - ((global::Doroti.Framework.Gestures.PointerScrollEvent)((global::Doroti.Framework.Gestures.PointerScrollEvent)@event__as35966)).scrollDelta), localFocalPoint: (local - localDelta), focalPointDelta: -localDelta));
                ((InteractiveViewer)this.widget).onInteractionEnd?.Invoke(new global::Doroti.Framework.Gestures.ScaleEndDetails());
                return;
            }
            if ((((global::Doroti.Framework.Gestures.PointerScrollEvent)((global::Doroti.Framework.Gestures.PointerScrollEvent)@event__as35966)).scrollDelta.dy == 0.0))
            {
                return;
            }
            scaleChange = global::Doroti.Runtime.Dart_mathLibrary.exp((-((global::Doroti.Framework.Gestures.PointerScrollEvent)((global::Doroti.Framework.Gestures.PointerScrollEvent)@event__as35966)).scrollDelta.dy / ((InteractiveViewer)this.widget).scaleFactor));
        }
        else
        {
            if ((@event is global::Doroti.Framework.Gestures.PointerScaleEvent))
            {
                global::Doroti.Framework.Gestures.PointerScaleEvent @event__as37721 = (global::Doroti.Framework.Gestures.PointerScaleEvent)@event;
                scaleChange = ((global::Doroti.Framework.Gestures.PointerScaleEvent)((global::Doroti.Framework.Gestures.PointerScaleEvent)@event__as37721)).scale;
            }
            else
            {
                return;
            }
        }
        ((InteractiveViewer)this.widget).onInteractionStart?.Invoke(new global::Doroti.Framework.Gestures.ScaleStartDetails(focalPoint: @global, localFocalPoint: local));
        if (!_gestureIsSupported(_GestureType__interactive_viewer.scale))
        {
            ((InteractiveViewer)this.widget).onInteractionUpdate?.Invoke(new global::Doroti.Framework.Gestures.ScaleUpdateDetails(focalPoint: @global, localFocalPoint: local, scale: scaleChange));
            ((InteractiveViewer)this.widget).onInteractionEnd?.Invoke(new global::Doroti.Framework.Gestures.ScaleEndDetails());
            return;
        }
        global::Doroti.Ui.Offset focalPointSceneLocal = ((global::Doroti.Ui.Offset)(object?)this._transformer.toScene(local));
        this._transformer.value = _matrixScale(this._transformer.value, scaleChange);
        global::Doroti.Ui.Offset focalPointSceneScaled = ((global::Doroti.Ui.Offset)(object?)this._transformer.toScene(local));
        this._transformer.value = _matrixTranslate(this._transformer.value, (focalPointSceneScaled - focalPointSceneLocal));
        ((InteractiveViewer)this.widget).onInteractionUpdate?.Invoke(new global::Doroti.Framework.Gestures.ScaleUpdateDetails(focalPoint: @global, localFocalPoint: local, scale: scaleChange));
        ((InteractiveViewer)this.widget).onInteractionEnd?.Invoke(new global::Doroti.Framework.Gestures.ScaleEndDetails());
    }

    internal virtual void _handleInertiaAnimation()
    {
        if (!((global::Doroti.Framework.Animation.AnimationController)this._controller).isAnimating)
        {
            _currentAxis = null;
            this._animation?.removeListener(() => this._handleInertiaAnimation());
            _animation = null;
            this._controller.reset();
            return;
        }
        Vector3 translationVector = this._transformer.value.getTranslation();
        var translation = new global::Doroti.Ui.Offset(translationVector.x, translationVector.y);
        this._transformer.value = _matrixTranslate(this._transformer.value, (this._transformer.toScene(this._animation!.value) - this._transformer.toScene(translation)));
    }

    internal virtual void _handleScaleAnimation()
    {
        if (!((global::Doroti.Framework.Animation.AnimationController)this._scaleController).isAnimating)
        {
            _currentAxis = null;
            this._scaleAnimation?.removeListener(() => this._handleScaleAnimation());
            _scaleAnimation = null;
            this._scaleController.reset();
            return;
        }
        double desiredScale = this._scaleAnimation!.value;
        double scaleChange = (desiredScale / this._transformer.value.getMaxScaleOnAxis());
        global::Doroti.Ui.Offset referenceFocalPoint = ((global::Doroti.Ui.Offset)(object?)this._transformer.toScene(this._scaleAnimationFocalPoint));
        this._transformer.value = _matrixScale(this._transformer.value, scaleChange);
        global::Doroti.Ui.Offset focalPointSceneScaled = ((global::Doroti.Ui.Offset)(object?)this._transformer.toScene(this._scaleAnimationFocalPoint));
        this._transformer.value = _matrixTranslate(this._transformer.value, (focalPointSceneScaled - referenceFocalPoint));
    }

    internal virtual void _handleTransformation()
    {
        setState(((global::System.Action)(() =>
        {
        })));
    }

    public override void initState()
    {
        base.initState();
        _controller = new global::Doroti.Framework.Animation.AnimationController(vsync: this);
        _scaleController = new global::Doroti.Framework.Animation.AnimationController(vsync: this);
        this._transformer.addListener(() => this._handleTransformation());
    }

    public override void didUpdateWidget(InteractiveViewer oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        TransformationController? newController = ((InteractiveViewer)this.widget).transformationController;
        if ((object.Equals(newController, ((InteractiveViewer)oldWidget).transformationController)))
        {
            return;
        }
        this._transformer.removeListener(() => this._handleTransformation());
        if ((((InteractiveViewer)oldWidget).transformationController is null))
        {
            this._transformer.dispose();
        }
        _transformer = (newController ?? new TransformationController());
        this._transformer.addListener(() => this._handleTransformation());
    }

    public override void dispose()
    {
        this._controller.dispose();
        this._scaleController.dispose();
        this._transformer.removeListener(() => this._handleTransformation());
        if ((((InteractiveViewer)this.widget).transformationController is null))
        {
            this._transformer.dispose();
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        Widget childLocal = default!;
        if ((((InteractiveViewer)this.widget).child is not null))
        {
            childLocal = DartRuntimePrimitives.ConvertValue<Widget>(new _InteractiveViewerBuilt__interactive_viewer(childKey: this._childKey, clipBehavior: ((InteractiveViewer)this.widget).clipBehavior, constrained: ((InteractiveViewer)this.widget).constrained, matrix: this._transformer.value, alignment: ((InteractiveViewer)this.widget).alignment, child: ((InteractiveViewer)this.widget).child!));
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (((InteractiveViewer)this.widget).builder is not null));
            DartRuntimePrimitives.Assert(() => !((InteractiveViewer)this.widget).constrained);
            childLocal = DartRuntimePrimitives.ConvertValue<Widget>(new LayoutBuilder(builder: ((global::System.Func<BuildContext, global::Doroti.Framework.Rendering.BoxConstraints, Widget>)((context, constraints) =>
            {
                Matrix4 matrixLocal = ((Matrix4)(object?)this._transformer.value);
                return ((Widget)(object?)new _InteractiveViewerBuilt__interactive_viewer(childKey: this._childKey, clipBehavior: ((InteractiveViewer)this.widget).clipBehavior, constrained: ((InteractiveViewer)this.widget).constrained, alignment: ((InteractiveViewer)this.widget).alignment, matrix: matrixLocal, child: ((InteractiveViewer)this.widget).builder!(context, Interactive_viewerLibrary._transformViewport(matrixLocal, (Offset.zero & ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).biggest)))));
                throw new InvalidOperationException("Dart closure completed without a value.");
            }))));
        }
        return ((Widget)(object?)new Listener(key: this._parentKey, onPointerSignal: (global::System.Action<global::Doroti.Framework.Gestures.PointerSignalEvent>)this._receivedPointerSignal, child: new GestureDetector(behavior: global::Doroti.Framework.Rendering.HitTestBehavior.opaque, onScaleEnd: (global::System.Action<global::Doroti.Framework.Gestures.ScaleEndDetails>)this._onScaleEnd, onScaleStart: (global::System.Action<global::Doroti.Framework.Gestures.ScaleStartDetails>)this._onScaleStart, onScaleUpdate: (global::System.Action<global::Doroti.Framework.Gestures.ScaleUpdateDetails>)this._onScaleUpdate, trackpadScrollCausesScale: ((InteractiveViewer)this.widget).trackpadScrollCausesScale, trackpadScrollToScaleFactor: new global::Doroti.Ui.Offset(0, (-1L / ((InteractiveViewer)this.widget).scaleFactor)), child: childLocal)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = this._tickerModeNotifier!.value;
        var result = ((Func<_WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values).enabled;
    __cascade.forceFrames = ((TickerModeData)values).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => (this._tickers is not null));
        DartRuntimePrimitives.Assert(() => this._tickers!.Contains(ticker));
        this._tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if ((this._tickers is not null))
        {
            TickerModeData values = this._tickerModeNotifier!.value;
            bool mutedLocal = !((TickerModeData)values).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = ((TickerModeData)values).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

internal class _InteractiveViewerBuilt__interactive_viewer : StatelessWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual GlobalKey<IState> childKey { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual bool constrained { get; private set; } = default!;
    public virtual Matrix4 matrix { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.Alignment? alignment { get; private set; }

    internal _InteractiveViewerBuilt__interactive_viewer(Widget child, GlobalKey<IState> childKey, Clip clipBehavior, bool constrained, Matrix4 matrix, global::Doroti.Framework.Painting.Alignment? alignment)
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
        Widget childLocal = ((Widget)(object?)new Transform(transform: this.matrix, alignment: this.alignment, child: new KeyedSubtree(key: this.childKey, child: ((Widget)((dynamic)this).child))));
        if (!this.constrained)
        {
            childLocal = DartRuntimePrimitives.ConvertValue<Widget>(new OverflowBox(alignment: global::Doroti.Framework.Painting.Alignment.topLeft, minWidth: 0.0, minHeight: 0.0, maxWidth: double.PositiveInfinity, maxHeight: double.PositiveInfinity, child: childLocal));
        }
        return ((Widget)(object?)new ClipRect(clipBehavior: this.clipBehavior, child: childLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class TransformationController : global::Doroti.Framework.Foundation.ValueNotifier<Matrix4>
{
    public TransformationController(Matrix4? value = null) : base((value ?? Matrix4.identity()))
    {
    }

    public virtual global::Doroti.Ui.Offset toScene(Offset viewportPoint)
    {
        var inverseMatrix = Matrix4.inverted(this.value);
        Vector3 untransformed = inverseMatrix.transform3(new Vector3(viewportPoint.dx, viewportPoint.dy, 0));
        return new global::Doroti.Ui.Offset(untransformed.x, untransformed.y);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal enum _GestureType__interactive_viewer
{
    pan,
    scale,
    rotate
}

public static partial class Interactive_viewerLibrary
{
    internal static double _getFinalTime(double velocity, double drag, double effectivelyMotionless = 10)
    {
        return (global::Doroti.Runtime.Dart_mathLibrary.log((effectivelyMotionless / velocity)) / global::Doroti.Runtime.Dart_mathLibrary.log((drag / 100L)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Interactive_viewerLibrary
{
    internal static Offset _getMatrixTranslation(Matrix4 matrix)
    {
        Vector3 nextTranslation = matrix.getTranslation();
        return new global::Doroti.Ui.Offset(nextTranslation.x, nextTranslation.y);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Interactive_viewerLibrary
{
    internal static global::Doroti.Ui.Quad _transformViewport(Matrix4 matrix, Rect viewport)
    {
        Matrix4 inverseMatrix = ((Func<Matrix4>)(() =>
{
    var __cascade = matrix.clone();
    __cascade.invert();
    return __cascade;
}))();
        return new global::Doroti.Ui.Quad(inverseMatrix.transform3(new Vector3(viewport.topLeft.dx, viewport.topLeft.dy, 0.0)), inverseMatrix.transform3(new Vector3(viewport.topRight.dx, viewport.topRight.dy, 0.0)), inverseMatrix.transform3(new Vector3(viewport.bottomRight.dx, viewport.bottomRight.dy, 0.0)), inverseMatrix.transform3(new Vector3(viewport.bottomLeft.dx, viewport.bottomLeft.dy, 0.0)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Interactive_viewerLibrary
{
    internal static global::Doroti.Ui.Quad _getAxisAlignedBoundingBoxWithRotation(Rect rect, double rotation)
    {
        var rotationMatrix = ((Func<Matrix4>)(() =>
{
    var __cascade = Matrix4.identity();
    __cascade.translateByDouble((rect.size.width / 2L), (rect.size.height / 2L), 0, 1);
    __cascade.rotateZ(rotation);
    __cascade.translateByDouble((-rect.size.width / 2L), (-rect.size.height / 2L), 0, 1);
    return __cascade;
}))();
        var boundariesRotated = new global::Doroti.Ui.Quad(rotationMatrix.transform3(new Vector3(rect.left, rect.top, 0.0)), rotationMatrix.transform3(new Vector3(rect.right, rect.top, 0.0)), rotationMatrix.transform3(new Vector3(rect.right, rect.bottom, 0.0)), rotationMatrix.transform3(new Vector3(rect.left, rect.bottom, 0.0)));
        return ((global::Doroti.Ui.Quad)(object?)InteractiveViewer.getAxisAlignedBoundingBox(boundariesRotated));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Interactive_viewerLibrary
{
    internal static Offset _exceedsBy(global::Doroti.Ui.Quad boundary, global::Doroti.Ui.Quad viewport)
    {
        var viewportPoints = new List<Vector3> { viewport.point0, viewport.point1, viewport.point2, viewport.point3 };
        global::Doroti.Ui.Offset largestExcess = ((global::Doroti.Ui.Offset)(object?)Offset.zero);
        foreach (var point in viewportPoints)
        {
            Vector3 pointInside = ((Vector3)(object?)InteractiveViewer.getNearestPointInside(point, boundary));
            var excess = new global::Doroti.Ui.Offset((pointInside.x - point.x), (pointInside.y - point.y));
            if ((excess.dx.abs() > largestExcess.dx.abs()))
            {
                largestExcess = new global::Doroti.Ui.Offset(excess.dx, largestExcess.dy);
            }
            if ((excess.dy.abs() > largestExcess.dy.abs()))
            {
                largestExcess = new global::Doroti.Ui.Offset(largestExcess.dx, excess.dy);
            }
        }
        return Interactive_viewerLibrary._round(largestExcess);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Interactive_viewerLibrary
{
    internal static Offset _round(Offset offset)
    {
        return new global::Doroti.Ui.Offset(Dart_coreLibrary.parse(offset.dx.toStringAsFixed(9L)), Dart_coreLibrary.parse(offset.dy.toStringAsFixed(9L)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Interactive_viewerLibrary
{
    internal static Offset _alignAxis(Offset offset, global::Doroti.Framework.Painting.Axis axis)
    {
        return (axis switch { global::Doroti.Framework.Painting.Axis.horizontal => new global::Doroti.Ui.Offset(offset.dx, 0.0), global::Doroti.Framework.Painting.Axis.vertical => new global::Doroti.Ui.Offset(0.0, offset.dy), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Interactive_viewerLibrary
{
    internal static global::Doroti.Framework.Painting.Axis? _getPanAxis(Offset point1, Offset point2)
    {
        if ((object.Equals(point1, point2)))
        {
            return ((global::Doroti.Framework.Painting.Axis)(object)null);
        }
        double x = (point2.dx - point1.dx);
        double y = (point2.dy - point1.dy);
        return ((x.abs() > y.abs()) ? global::Doroti.Framework.Painting.Axis.horizontal : global::Doroti.Framework.Painting.Axis.vertical);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public enum PanAxis
{
    horizontal,
    vertical,
    aligned,
    free
}

