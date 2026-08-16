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
        double lengthSquared__16123 = (global::Doroti.Runtime.Dart_mathLibrary.pow((l2.x - l1.x), 2.0).toDouble() + global::Doroti.Runtime.Dart_mathLibrary.pow((l2.y - l1.y), 2.0).toDouble());
        if ((lengthSquared__16123 == 0L))
        {
            return l1;
        }
        Vector3 l1P__16427 = (point - l1);
        Vector3 l1L2__16463 = (l2 - l1);
        double fraction__16496 = Dart_uiLibrary.clampDouble((l1P__16427.dot(l1L2__16463) / lengthSquared__16123), 0.0, 1.0);
        return (l1 + (l1L2__16463 * fraction__16496));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Ui.Quad getAxisAlignedBoundingBox(global::Doroti.Ui.Quad quad)
    {
        double minX__16748 = Math.Min(quad.point0.x, Math.Min(quad.point1.x, Math.Min(quad.point2.x, quad.point3.x)));
        double minY__16881 = Math.Min(quad.point0.y, Math.Min(quad.point1.y, Math.Min(quad.point2.y, quad.point3.y)));
        double maxX__17014 = Math.Max(quad.point0.x, Math.Max(quad.point1.x, Math.Max(quad.point2.x, quad.point3.x)));
        double maxY__17147 = Math.Max(quad.point0.y, Math.Max(quad.point1.y, Math.Max(quad.point2.y, quad.point3.y)));
        return new global::Doroti.Ui.Quad(new Vector3(minX__16748, minY__16881, 0), new Vector3(maxX__17014, minY__16881, 0), new Vector3(maxX__17014, maxY__17147, 0), new Vector3(minX__16748, maxY__17147, 0));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool pointIsInside(Vector3 point, global::Doroti.Ui.Quad quad)
    {
        Vector3 aM__17671 = (point - quad.point0);
        Vector3 aB__17715 = (quad.point1 - quad.point0);
        Vector3 aD__17765 = (quad.point3 - quad.point0);
        double aMAB__17815 = aM__17671.dot(aB__17715);
        double aBAB__17851 = aB__17715.dot(aB__17715);
        double aMAD__17887 = aM__17671.dot(aD__17765);
        double aDAD__17923 = aD__17765.dot(aD__17765);
        return ((((0L <= aMAB__17815) && (aMAB__17815 <= aBAB__17851)) && (0L <= aMAD__17887)) && (aMAD__17887 <= aDAD__17923));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Vector3 getNearestPointInside(Vector3 point, global::Doroti.Ui.Quad quad)
    {
        if (InteractiveViewer.pointIsInside(point, quad))
        {
            return point;
        }
        var closestPoints__18428 = new List<Vector3> { InteractiveViewer.getNearestPointOnLine(point, quad.point0, quad.point1), InteractiveViewer.getNearestPointOnLine(point, quad.point1, quad.point2), InteractiveViewer.getNearestPointOnLine(point, quad.point2, quad.point3), InteractiveViewer.getNearestPointOnLine(point, quad.point3, quad.point0) };
        double minDistance__18793 = double.PositiveInfinity;
        Vector3 closestOverall__18841 = default!;
        foreach (var closePoint__18872 in closestPoints__18428)
        {
            double distance__18922 = global::Doroti.Runtime.Dart_mathLibrary.sqrt((global::Doroti.Runtime.Dart_mathLibrary.pow((point.x - closePoint__18872.x), 2L) + global::Doroti.Runtime.Dart_mathLibrary.pow((point.y - closePoint__18872.y), 2L)));
            if ((distance__18922 < minDistance__18793))
            {
                minDistance__18793 = distance__18922;
                closestOverall__18841 = closePoint__18872;
            }
        }
        return closestOverall__18841;
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
            var childRenderBox__20744 = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((GlobalKey<IState>)this._childKey).currentContext!.findRenderObject()!)!;
            global::Doroti.Ui.Size childSize__20836 = ((global::Doroti.Ui.Size)(object?)((global::Doroti.Framework.Rendering.RenderBox)childRenderBox__20744).size);
            global::Doroti.Ui.Rect boundaryRect__20884 = ((global::Doroti.Ui.Rect)(object?)((InteractiveViewer)this.widget).boundaryMargin.inflateRect((Offset.zero & childSize__20836)));
            DartRuntimePrimitives.Assert(() => !boundaryRect__20884.isEmpty, () => (object?)"InteractiveViewer's child must have nonzero dimensions.");
            DartRuntimePrimitives.Assert(() => (boundaryRect__20884.isFinite || ((((double.IsInfinity(boundaryRect__20884.left) && double.IsInfinity(boundaryRect__20884.top)) && double.IsInfinity(boundaryRect__20884.right)) && double.IsInfinity(boundaryRect__20884.bottom)))), () => (object?)"boundaryRect must either be infinite in all directions or finite in all directions.");
            return boundaryRect__20884;
            return default!;
        }
    }
    internal virtual global::Doroti.Ui.Rect _viewport
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (((GlobalKey<IState>)this._parentKey).currentContext is not null));
            var parentRenderBox__21684 = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((GlobalKey<IState>)this._parentKey).currentContext!.findRenderObject()!)!;
            return (Offset.zero & ((global::Doroti.Framework.Rendering.RenderBox)parentRenderBox__21684).size);
            return default!;
        }
    }
    internal virtual Matrix4 _matrixTranslate(Matrix4 matrix, Offset translation)
    {
        if ((object.Equals(translation, Offset.zero)))
        {
            return matrix.clone();
        }
        global::Doroti.Ui.Offset alignedTranslation__22069 = default!;
        if ((this._currentAxis is not null))
        {
            alignedTranslation__22069 = (((InteractiveViewer)this.widget).panAxis switch { PanAxis.horizontal => Interactive_viewerLibrary._alignAxis(translation, global::Doroti.Framework.Painting.Axis.horizontal), PanAxis.vertical => Interactive_viewerLibrary._alignAxis(translation, global::Doroti.Framework.Painting.Axis.vertical), PanAxis.aligned => Interactive_viewerLibrary._alignAxis(translation, DartRuntimePrimitives.RequireValue(this._currentAxis)), PanAxis.free => translation, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        }
        else
        {
            alignedTranslation__22069 = translation;
        }
        Matrix4 nextMatrix__22506 = ((Func<Matrix4>)(() =>
{            var __cascade = matrix.clone();
            __cascade.translateByDouble(alignedTranslation__22069.dx, alignedTranslation__22069.dy, 0, 1);
            return __cascade;        }))();
        global::Doroti.Ui.Quad nextViewport__22748 = Interactive_viewerLibrary._transformViewport(nextMatrix__22506, this._viewport);
        if (this._boundaryRect.isInfinite)
        {
            return nextMatrix__22506;
        }
        global::Doroti.Ui.Quad boundariesAabbQuad__23279 = Interactive_viewerLibrary._getAxisAlignedBoundingBoxWithRotation(this._boundaryRect, this._currentRotation);
        global::Doroti.Ui.Offset offendingDistance__23491 = ((global::Doroti.Ui.Offset)(object?)Interactive_viewerLibrary._exceedsBy(boundariesAabbQuad__23279, nextViewport__22748));
        if ((object.Equals(offendingDistance__23491, Offset.zero)))
        {
            return nextMatrix__22506;
        }
        global::Doroti.Ui.Offset nextTotalTranslation__23757 = ((global::Doroti.Ui.Offset)(object?)Interactive_viewerLibrary._getMatrixTranslation(nextMatrix__22506));
        double currentScale__23832 = matrix.getMaxScaleOnAxis();
        var correctedTotalTranslation__23885 = new global::Doroti.Ui.Offset((nextTotalTranslation__23757.dx - (offendingDistance__23491.dx * currentScale__23832)), (nextTotalTranslation__23757.dy - (offendingDistance__23491.dy * currentScale__23832)));
        Matrix4 correctedMatrix__24417 = ((Func<Matrix4>)(() =>
{            var __cascade = matrix.clone();
            __cascade.setTranslation(new Vector3(correctedTotalTranslation__23885.dx, correctedTotalTranslation__23885.dy, 0.0));
            return __cascade;        }))();
        global::Doroti.Ui.Quad correctedViewport__24621 = Interactive_viewerLibrary._transformViewport(correctedMatrix__24417, this._viewport);
        global::Doroti.Ui.Offset offendingCorrectedDistance__24706 = ((global::Doroti.Ui.Offset)(object?)Interactive_viewerLibrary._exceedsBy(boundariesAabbQuad__23279, correctedViewport__24621));
        if ((object.Equals(offendingCorrectedDistance__24706, Offset.zero)))
        {
            return correctedMatrix__24417;
        }
        if (((offendingCorrectedDistance__24706.dx != 0.0) && (offendingCorrectedDistance__24706.dy != 0.0)))
        {
            return matrix.clone();
        }
        var unidirectionalCorrectedTotalTranslation__25349 = new global::Doroti.Ui.Offset(((offendingCorrectedDistance__24706.dx == 0.0) ? correctedTotalTranslation__23885.dx : 0.0), ((offendingCorrectedDistance__24706.dy == 0.0) ? correctedTotalTranslation__23885.dy : 0.0));
        return ((Func<Matrix4>)(() =>
{            var __cascade = matrix.clone();
            __cascade.setTranslation(new Vector3(unidirectionalCorrectedTotalTranslation__25349.dx, unidirectionalCorrectedTotalTranslation__25349.dy, 0.0));
            return __cascade;        }))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Matrix4 _matrixScale(Matrix4 matrix, double scale)
    {
        if ((scale == 1.0))
        {
            return matrix.clone();
        }
        DartRuntimePrimitives.Assert(() => (scale != 0.0));
        double currentScale__26103 = this._transformer.value.getMaxScaleOnAxis();
        double totalScale__26175 = Math.Max((currentScale__26103 * scale), Math.Max((this._viewport.width / this._boundaryRect.width), (this._viewport.height / this._boundaryRect.height)));
        double clampedTotalScale__26478 = Dart_uiLibrary.clampDouble(totalScale__26175, ((InteractiveViewer)this.widget).minScale, ((InteractiveViewer)this.widget).maxScale);
        double clampedScale__26574 = (clampedTotalScale__26478 / currentScale__26103);
        return ((Func<Matrix4>)(() =>
{            var __cascade = matrix.clone();
            __cascade.scaleByDouble(clampedScale__26574, clampedScale__26574, clampedScale__26574, 1);
            return __cascade;        }))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Matrix4 _matrixRotate(Matrix4 matrix, double rotation, Offset focalPoint)
    {
        if ((rotation == 0L))
        {
            return matrix.clone();
        }
        global::Doroti.Ui.Offset focalPointScene__26965 = ((global::Doroti.Ui.Offset)(object?)this._transformer.toScene(focalPoint));
        return ((Func<Matrix4>)(() =>
{            var __cascade = matrix.clone();
            __cascade.translateByDouble(focalPointScene__26965.dx, focalPointScene__26965.dy, 0, 1);
            __cascade.rotateZ(-rotation);
            __cascade.translateByDouble(-focalPointScene__26965.dx, -focalPointScene__26965.dy, 0, 1);
            return __cascade;        }))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _gestureIsSupported(_GestureType__interactive_viewer? gestureType)
    {
        return (gestureType switch { _GestureType__interactive_viewer.rotate => this._rotateEnabled, _GestureType__interactive_viewer.scale => ((InteractiveViewer)this.widget).scaleEnabled, _GestureType__interactive_viewer.pan => ((InteractiveViewer)this.widget).panEnabled, null => ((InteractiveViewer)this.widget).panEnabled, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual _GestureType__interactive_viewer _getGestureType(global::Doroti.Framework.Gestures.ScaleUpdateDetails details)
    {
        double scale__27849 = (!((InteractiveViewer)this.widget).scaleEnabled ? 1.0 : ((global::Doroti.Framework.Gestures.ScaleUpdateDetails)details).scale);
        double rotation__27918 = (!this._rotateEnabled ? 0.0 : ((global::Doroti.Framework.Gestures.ScaleUpdateDetails)details).rotation);
        if ((((scale__27849 - 1L)).abs() > rotation__27918.abs()))
        {
            return _GestureType__interactive_viewer.scale;
        }
        else
        {
            if ((rotation__27918 != 0.0))
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
        double scale__29196 = this._transformer.value.getMaxScaleOnAxis();
        _scaleAnimationFocalPoint = ((global::Doroti.Framework.Gestures.ScaleUpdateDetails)details).localFocalPoint;
        global::Doroti.Ui.Offset focalPointScene__29318 = ((global::Doroti.Ui.Offset)(object?)this._transformer.toScene(((global::Doroti.Framework.Gestures.ScaleUpdateDetails)details).localFocalPoint));
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
                    double desiredScale__30231 = (DartRuntimePrimitives.RequireValue(this._scaleStart) * ((global::Doroti.Framework.Gestures.ScaleUpdateDetails)details).scale);
                    double scaleChange__30297 = (desiredScale__30231 / scale__29196);
                    this._transformer.value = _matrixScale(this._transformer.value, scaleChange__30297);
                    global::Doroti.Ui.Offset focalPointSceneScaled__30685 = ((global::Doroti.Ui.Offset)(object?)this._transformer.toScene(((global::Doroti.Framework.Gestures.ScaleUpdateDetails)details).localFocalPoint));
                    this._transformer.value = _matrixTranslate(this._transformer.value, (focalPointSceneScaled__30685 - DartRuntimePrimitives.RequireValue(this._referenceFocalPoint)));
                    global::Doroti.Ui.Offset focalPointSceneCheck__31273 = ((global::Doroti.Ui.Offset)(object?)this._transformer.toScene(((global::Doroti.Framework.Gestures.ScaleUpdateDetails)details).localFocalPoint));
                    if ((!object.Equals(Interactive_viewerLibrary._round(DartRuntimePrimitives.RequireValue(this._referenceFocalPoint)), Interactive_viewerLibrary._round(focalPointSceneCheck__31273))))
                    {
                        _referenceFocalPoint = focalPointSceneCheck__31273;
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
                    double desiredRotation__31659 = (DartRuntimePrimitives.RequireValue(this._rotationStart) + ((global::Doroti.Framework.Gestures.ScaleUpdateDetails)details).rotation);
                    this._transformer.value = _matrixRotate(this._transformer.value, (this._currentRotation - desiredRotation__31659), ((global::Doroti.Framework.Gestures.ScaleUpdateDetails)details).localFocalPoint);
                    _currentRotation = desiredRotation__31659;
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
                    _currentAxis ??= Interactive_viewerLibrary._getPanAxis(DartRuntimePrimitives.RequireValue(this._referenceFocalPoint), focalPointScene__29318);
                    global::Doroti.Ui.Offset translationChange__32556 = ((global::Doroti.Ui.Offset)(object?)(focalPointScene__29318 - DartRuntimePrimitives.RequireValue(this._referenceFocalPoint)));
                    this._transformer.value = _matrixTranslate(this._transformer.value, translationChange__32556);
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
                    Vector3 translationVector__33631 = this._transformer.value.getTranslation();
                    var translation__33702 = new global::Doroti.Ui.Offset(translationVector__33631.x, translationVector__33631.y);
                    var frictionSimulationX__33780 = new global::Doroti.Framework.Physics.FrictionSimulation(((InteractiveViewer)this.widget).interactionEndFrictionCoefficient, translation__33702.dx, ((global::Doroti.Framework.Gestures.ScaleEndDetails)details).velocity.pixelsPerSecond.dx);
                    var frictionSimulationY__33972 = new global::Doroti.Framework.Physics.FrictionSimulation(((InteractiveViewer)this.widget).interactionEndFrictionCoefficient, translation__33702.dy, ((global::Doroti.Framework.Gestures.ScaleEndDetails)details).velocity.pixelsPerSecond.dy);
                    double tFinal__34171 = Interactive_viewerLibrary._getFinalTime(((global::Doroti.Framework.Gestures.ScaleEndDetails)details).velocity.pixelsPerSecond.distance, ((InteractiveViewer)this.widget).interactionEndFrictionCoefficient);
                    _animation = new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: translation__33702, end: new global::Doroti.Ui.Offset(((global::Doroti.Framework.Physics.FrictionSimulation)frictionSimulationX__33780).finalX, ((global::Doroti.Framework.Physics.FrictionSimulation)frictionSimulationY__33972).finalX)).chain(new global::Doroti.Framework.Animation.CurveTween(curve: global::Doroti.Framework.Animation.Curves.decelerate)).animate(this._controller);
                    this._controller.duration = Duration.Create(milliseconds: ((tFinal__34171 * 1000L)).round());
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
                    double scale__34861 = this._transformer.value.getMaxScaleOnAxis();
                    var frictionSimulation__34923 = new global::Doroti.Framework.Physics.FrictionSimulation((((InteractiveViewer)this.widget).interactionEndFrictionCoefficient * ((InteractiveViewer)this.widget).scaleFactor), scale__34861, (((global::Doroti.Framework.Gestures.ScaleEndDetails)details).scaleVelocity / 10L));
                    double tFinal__35124 = Interactive_viewerLibrary._getFinalTime(((global::Doroti.Framework.Gestures.ScaleEndDetails)details).scaleVelocity.abs(), ((InteractiveViewer)this.widget).interactionEndFrictionCoefficient, effectivelyMotionless: 0.1);
                    _scaleAnimation = new global::Doroti.Framework.Animation.Tween<double>(begin: scale__34861, end: frictionSimulation__34923.x(tFinal__35124)).chain(new global::Doroti.Framework.Animation.CurveTween(curve: global::Doroti.Framework.Animation.Curves.decelerate)).animate(this._scaleController);
                    this._scaleController.duration = Duration.Create(milliseconds: ((tFinal__35124 * 1000L)).round());
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
        global::Doroti.Ui.Offset local__35857 = ((global::Doroti.Ui.Offset)(object?)@event.localPosition);
        global::Doroti.Ui.Offset global__35903 = ((global::Doroti.Ui.Offset)(object?)@event.position);
        double scaleChange__35945 = default!;
        if ((@event is global::Doroti.Framework.Gestures.PointerScrollEvent))
        {
            global::Doroti.Framework.Gestures.PointerScrollEvent @event__as35966 = (global::Doroti.Framework.Gestures.PointerScrollEvent)@event;
            if (((object.Equals(((global::Doroti.Framework.Gestures.PointerScrollEvent)@event__as35966).kind, PointerDeviceKind.trackpad)) && !((InteractiveViewer)this.widget).trackpadScrollCausesScale))
            {
                ((InteractiveViewer)this.widget).onInteractionStart?.Invoke(new global::Doroti.Framework.Gestures.ScaleStartDetails(focalPoint: global__35903, localFocalPoint: local__35857));
                global::Doroti.Ui.Offset localDelta__36285 = ((global::Doroti.Ui.Offset)(object?)PointerEvent.transformDeltaViaPositions(untransformedEndPosition: (global__35903 + ((global::Doroti.Framework.Gestures.PointerScrollEvent)((global::Doroti.Framework.Gestures.PointerScrollEvent)@event__as35966)).scrollDelta), untransformedDelta: ((global::Doroti.Framework.Gestures.PointerScrollEvent)((global::Doroti.Framework.Gestures.PointerScrollEvent)@event__as35966)).scrollDelta, transform: ((global::Doroti.Framework.Gestures.PointerScrollEvent)@event__as35966).transform));
                if (!_gestureIsSupported(_GestureType__interactive_viewer.pan))
                {
                    ((InteractiveViewer)this.widget).onInteractionUpdate?.Invoke(new global::Doroti.Framework.Gestures.ScaleUpdateDetails(focalPoint: (global__35903 - ((global::Doroti.Framework.Gestures.PointerScrollEvent)((global::Doroti.Framework.Gestures.PointerScrollEvent)@event__as35966)).scrollDelta), localFocalPoint: (local__35857 - ((global::Doroti.Framework.Gestures.PointerScrollEvent)((global::Doroti.Framework.Gestures.PointerScrollEvent)@event__as35966)).scrollDelta), focalPointDelta: -localDelta__36285));
                    ((InteractiveViewer)this.widget).onInteractionEnd?.Invoke(new global::Doroti.Framework.Gestures.ScaleEndDetails());
                    return;
                }
                global::Doroti.Ui.Offset focalPointScene__36926 = ((global::Doroti.Ui.Offset)(object?)this._transformer.toScene(local__35857));
                global::Doroti.Ui.Offset newFocalPointScene__36994 = ((global::Doroti.Ui.Offset)(object?)this._transformer.toScene((local__35857 - localDelta__36285)));
                this._transformer.value = _matrixTranslate(this._transformer.value, (newFocalPointScene__36994 - focalPointScene__36926));
                ((InteractiveViewer)this.widget).onInteractionUpdate?.Invoke(new global::Doroti.Framework.Gestures.ScaleUpdateDetails(focalPoint: (global__35903 - ((global::Doroti.Framework.Gestures.PointerScrollEvent)((global::Doroti.Framework.Gestures.PointerScrollEvent)@event__as35966)).scrollDelta), localFocalPoint: (local__35857 - localDelta__36285), focalPointDelta: -localDelta__36285));
                ((InteractiveViewer)this.widget).onInteractionEnd?.Invoke(new global::Doroti.Framework.Gestures.ScaleEndDetails());
                return;
            }
            if ((((global::Doroti.Framework.Gestures.PointerScrollEvent)((global::Doroti.Framework.Gestures.PointerScrollEvent)@event__as35966)).scrollDelta.dy == 0.0))
            {
                return;
            }
            scaleChange__35945 = global::Doroti.Runtime.Dart_mathLibrary.exp((-((global::Doroti.Framework.Gestures.PointerScrollEvent)((global::Doroti.Framework.Gestures.PointerScrollEvent)@event__as35966)).scrollDelta.dy / ((InteractiveViewer)this.widget).scaleFactor));
        }
        else
        {
            if ((@event is global::Doroti.Framework.Gestures.PointerScaleEvent))
            {
                global::Doroti.Framework.Gestures.PointerScaleEvent @event__as37721 = (global::Doroti.Framework.Gestures.PointerScaleEvent)@event;
                scaleChange__35945 = ((global::Doroti.Framework.Gestures.PointerScaleEvent)((global::Doroti.Framework.Gestures.PointerScaleEvent)@event__as37721)).scale;
            }
            else
            {
                return;
            }
        }
        ((InteractiveViewer)this.widget).onInteractionStart?.Invoke(new global::Doroti.Framework.Gestures.ScaleStartDetails(focalPoint: global__35903, localFocalPoint: local__35857));
        if (!_gestureIsSupported(_GestureType__interactive_viewer.scale))
        {
            ((InteractiveViewer)this.widget).onInteractionUpdate?.Invoke(new global::Doroti.Framework.Gestures.ScaleUpdateDetails(focalPoint: global__35903, localFocalPoint: local__35857, scale: scaleChange__35945));
            ((InteractiveViewer)this.widget).onInteractionEnd?.Invoke(new global::Doroti.Framework.Gestures.ScaleEndDetails());
            return;
        }
        global::Doroti.Ui.Offset focalPointScene__38205 = ((global::Doroti.Ui.Offset)(object?)this._transformer.toScene(local__35857));
        this._transformer.value = _matrixScale(this._transformer.value, scaleChange__35945);
        global::Doroti.Ui.Offset focalPointSceneScaled__38467 = ((global::Doroti.Ui.Offset)(object?)this._transformer.toScene(local__35857));
        this._transformer.value = _matrixTranslate(this._transformer.value, (focalPointSceneScaled__38467 - focalPointScene__38205));
        ((InteractiveViewer)this.widget).onInteractionUpdate?.Invoke(new global::Doroti.Framework.Gestures.ScaleUpdateDetails(focalPoint: global__35903, localFocalPoint: local__35857, scale: scaleChange__35945));
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
        Vector3 translationVector__39159 = this._transformer.value.getTranslation();
        var translation__39226 = new global::Doroti.Ui.Offset(translationVector__39159.x, translationVector__39159.y);
        this._transformer.value = _matrixTranslate(this._transformer.value, (this._transformer.toScene(this._animation!.value) - this._transformer.toScene(translation__39226)));
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
        double desiredScale__39716 = this._scaleAnimation!.value;
        double scaleChange__39772 = (desiredScale__39716 / this._transformer.value.getMaxScaleOnAxis());
        global::Doroti.Ui.Offset referenceFocalPoint__39858 = ((global::Doroti.Ui.Offset)(object?)this._transformer.toScene(this._scaleAnimationFocalPoint));
        this._transformer.value = _matrixScale(this._transformer.value, scaleChange__39772);
        global::Doroti.Ui.Offset focalPointSceneScaled__40257 = ((global::Doroti.Ui.Offset)(object?)this._transformer.toScene(this._scaleAnimationFocalPoint));
        this._transformer.value = _matrixTranslate(this._transformer.value, (focalPointSceneScaled__40257 - referenceFocalPoint__39858));
    }

    internal virtual void _handleTransformation()
    {
        setState(((global::System.Action)(() => {
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
        TransformationController? newController__40975 = ((InteractiveViewer)this.widget).transformationController;
        if ((object.Equals(newController__40975, ((InteractiveViewer)oldWidget).transformationController)))
        {
            return;
        }
        this._transformer.removeListener(() => this._handleTransformation());
        if ((((InteractiveViewer)oldWidget).transformationController is null))
        {
            this._transformer.dispose();
        }
        _transformer = (newController__40975 ?? new TransformationController());
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
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker__18989 in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker__18989).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker__18989.describeForError("The offending ticker was") }));
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
        Widget child__41696 = default!;
        if ((((InteractiveViewer)this.widget).child is not null))
        {
            child__41696 = DartRuntimePrimitives.ConvertValue<Widget>(new _InteractiveViewerBuilt__interactive_viewer(childKey: this._childKey, clipBehavior: ((InteractiveViewer)this.widget).clipBehavior, constrained: ((InteractiveViewer)this.widget).constrained, matrix: this._transformer.value, alignment: ((InteractiveViewer)this.widget).alignment, child: ((InteractiveViewer)this.widget).child!));
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (((InteractiveViewer)this.widget).builder is not null));
            DartRuntimePrimitives.Assert(() => !((InteractiveViewer)this.widget).constrained);
            child__41696 = DartRuntimePrimitives.ConvertValue<Widget>(new LayoutBuilder(builder: ((global::System.Func<BuildContext, global::Doroti.Framework.Rendering.BoxConstraints, Widget>)((context, constraints) => {
Matrix4 matrix__42339 = ((Matrix4)(object?)this._transformer.value);
return ((Widget)(object?)new _InteractiveViewerBuilt__interactive_viewer(childKey: this._childKey, clipBehavior: ((InteractiveViewer)this.widget).clipBehavior, constrained: ((InteractiveViewer)this.widget).constrained, alignment: ((InteractiveViewer)this.widget).alignment, matrix: matrix__42339, child: ((InteractiveViewer)this.widget).builder!(context, Interactive_viewerLibrary._transformViewport(matrix__42339, (Offset.zero & ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).biggest)))));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        }
        return ((Widget)(object?)new Listener(key: this._parentKey, onPointerSignal: (global::System.Action<global::Doroti.Framework.Gestures.PointerSignalEvent>)this._receivedPointerSignal, child: new GestureDetector(behavior: global::Doroti.Framework.Rendering.HitTestBehavior.opaque, onScaleEnd: (global::System.Action<global::Doroti.Framework.Gestures.ScaleEndDetails>)this._onScaleEnd, onScaleStart: (global::System.Action<global::Doroti.Framework.Gestures.ScaleStartDetails>)this._onScaleStart, onScaleUpdate: (global::System.Action<global::Doroti.Framework.Gestures.ScaleUpdateDetails>)this._onScaleUpdate, trackpadScrollCausesScale: ((InteractiveViewer)this.widget).trackpadScrollCausesScale, trackpadScrollToScaleFactor: new global::Doroti.Ui.Offset(0, (-1L / ((InteractiveViewer)this.widget).scaleFactor)), child: child__41696)));
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
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<_WidgetTicker__ticker_provider>)(() =>
{            var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
            __cascade.muted = !((TickerModeData)values__17506).enabled;
            __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
            return __cascade;        }))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result__17553);
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
            TickerModeData values__18318 = this._tickerModeNotifier!.value;
            bool muted__18372 = !((TickerModeData)values__18318).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
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
        Widget child__43924 = ((Widget)(object?)new Transform(transform: this.matrix, alignment: this.alignment, child: new KeyedSubtree(key: this.childKey, child: ((Widget)((dynamic)this).child))));
        if (!this.constrained)
        {
            child__43924 = DartRuntimePrimitives.ConvertValue<Widget>(new OverflowBox(alignment: global::Doroti.Framework.Painting.Alignment.topLeft, minWidth: 0.0, minHeight: 0.0, maxWidth: double.PositiveInfinity, maxHeight: double.PositiveInfinity, child: child__43924));
        }
        return ((Widget)(object?)new ClipRect(clipBehavior: this.clipBehavior, child: child__43924));
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
        var inverseMatrix__46410 = Matrix4.inverted(this.value);
        Vector3 untransformed__46469 = inverseMatrix__46410.transform3(new Vector3(viewportPoint.dx, viewportPoint.dy, 0));
        return new global::Doroti.Ui.Offset(untransformed__46469.x, untransformed__46469.y);
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
        Vector3 nextTranslation__47225 = matrix.getTranslation();
        return new global::Doroti.Ui.Offset(nextTranslation__47225.x, nextTranslation__47225.y);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Interactive_viewerLibrary
{
    internal static global::Doroti.Ui.Quad _transformViewport(Matrix4 matrix, Rect viewport)
    {
        Matrix4 inverseMatrix__47697 = ((Func<Matrix4>)(() =>
{            var __cascade = matrix.clone();
            __cascade.invert();
            return __cascade;        }))();
        return new global::Doroti.Ui.Quad(inverseMatrix__47697.transform3(new Vector3(viewport.topLeft.dx, viewport.topLeft.dy, 0.0)), inverseMatrix__47697.transform3(new Vector3(viewport.topRight.dx, viewport.topRight.dy, 0.0)), inverseMatrix__47697.transform3(new Vector3(viewport.bottomRight.dx, viewport.bottomRight.dy, 0.0)), inverseMatrix__47697.transform3(new Vector3(viewport.bottomLeft.dx, viewport.bottomLeft.dy, 0.0)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Interactive_viewerLibrary
{
    internal static global::Doroti.Ui.Quad _getAxisAlignedBoundingBoxWithRotation(Rect rect, double rotation)
    {
        var rotationMatrix__48311 = ((Func<Matrix4>)(() =>
{            var __cascade = Matrix4.identity();
            __cascade.translateByDouble((rect.size.width / 2L), (rect.size.height / 2L), 0, 1);
            __cascade.rotateZ(rotation);
            __cascade.translateByDouble((-rect.size.width / 2L), (-rect.size.height / 2L), 0, 1);
            return __cascade;        }))();
        var boundariesRotated__48528 = new global::Doroti.Ui.Quad(rotationMatrix__48311.transform3(new Vector3(rect.left, rect.top, 0.0)), rotationMatrix__48311.transform3(new Vector3(rect.right, rect.top, 0.0)), rotationMatrix__48311.transform3(new Vector3(rect.right, rect.bottom, 0.0)), rotationMatrix__48311.transform3(new Vector3(rect.left, rect.bottom, 0.0)));
        return ((global::Doroti.Ui.Quad)(object?)InteractiveViewer.getAxisAlignedBoundingBox(boundariesRotated__48528));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Interactive_viewerLibrary
{
    internal static Offset _exceedsBy(global::Doroti.Ui.Quad boundary, global::Doroti.Ui.Quad viewport)
    {
        var viewportPoints__49140 = new List<Vector3> { viewport.point0, viewport.point1, viewport.point2, viewport.point3 };
        global::Doroti.Ui.Offset largestExcess__49266 = ((global::Doroti.Ui.Offset)(object?)Offset.zero);
        foreach (var point__49308 in viewportPoints__49140)
        {
            Vector3 pointInside__49353 = ((Vector3)(object?)InteractiveViewer.getNearestPointInside(point__49308, boundary));
            var excess__49435 = new global::Doroti.Ui.Offset((pointInside__49353.x - point__49308.x), (pointInside__49353.y - point__49308.y));
            if ((excess__49435.dx.abs() > largestExcess__49266.dx.abs()))
            {
                largestExcess__49266 = new global::Doroti.Ui.Offset(excess__49435.dx, largestExcess__49266.dy);
            }
            if ((excess__49435.dy.abs() > largestExcess__49266.dy.abs()))
            {
                largestExcess__49266 = new global::Doroti.Ui.Offset(largestExcess__49266.dx, excess__49435.dy);
            }
        }
        return Interactive_viewerLibrary._round(largestExcess__49266);
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
        double x__50576 = (point2.dx - point1.dx);
        double y__50618 = (point2.dy - point1.dy);
        return ((x__50576.abs() > y__50618.abs()) ? global::Doroti.Framework.Painting.Axis.horizontal : global::Doroti.Framework.Painting.Axis.vertical);
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

