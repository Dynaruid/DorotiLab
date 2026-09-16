// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/snapshot_widget.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public enum SnapshotMode
{
    permissive,
    normal,
    forced
}

public class SnapshotController : global::Doroti.Framework.Foundation.ChangeNotifier
{
    internal virtual bool _allowSnapshotting { get; set; } = default!;

    public SnapshotController(bool allowSnapshotting = false)
    {
        _allowSnapshotting = allowSnapshotting;
    }

    public virtual void clear()
    {
        notifyListeners();
    }

    public virtual bool allowSnapshotting
    {
        get => _allowSnapshotting;
        set
        {
            var __value = value;
            if (__value == allowSnapshotting)
            {
                return;
            }
            _allowSnapshotting = __value;
            notifyListeners();
        }
    }
}

public class SnapshotWidget : SingleChildRenderObjectWidget
{
    public virtual SnapshotController controller { get; private set; } = default!;
    public virtual SnapshotMode mode { get; private set; } = default!;
    public virtual bool autoresize { get; private set; } = default!;
    public virtual SnapshotPainter painter { get; private set; } = default!;

    public SnapshotWidget(global::Doroti.Framework.Foundation.Key? key = null, SnapshotMode mode = SnapshotMode.normal, SnapshotPainter painter = default!, bool autoresize = false, SnapshotController controller = default!, Widget? child = default!) : base(key: key, child: child)
    {
        SnapshotPainter __painter = painter ?? new _DefaultSnapshotPainter__snapshot_widget();
        this.mode = mode;
        this.painter = __painter;
        this.autoresize = autoresize;
        this.controller = controller;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        DebugLibrary.debugCheckHasMediaQuery(context);
        return new _RenderSnapshotWidget__snapshot_widget(controller: controller, mode: mode, devicePixelRatio: MediaQuery.devicePixelRatioOf(context), painter: painter, autoresize: autoresize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        DebugLibrary.debugCheckHasMediaQuery(context);
        DartRuntimePrimitives.Ignore(((Func<_RenderSnapshotWidget__snapshot_widget>)(() =>
{
    var __cascade = ((_RenderSnapshotWidget__snapshot_widget?)renderObject)!;
    __cascade.controller = controller;
    __cascade.mode = mode;
    __cascade.devicePixelRatio = MediaQuery.devicePixelRatioOf(context);
    __cascade.painter = painter;
    __cascade.autoresize = autoresize;
    return __cascade;
}))());
    }

}

internal class _RenderSnapshotWidget__snapshot_widget : global::Doroti.Framework.Rendering.RenderProxyBox
{
    internal virtual double _devicePixelRatio { get; set; } = default!;
    internal virtual SnapshotPainter _painter { get; set; } = default!;
    internal virtual SnapshotController _controller { get; set; } = default!;
    internal virtual SnapshotMode _mode { get; set; } = default!;
    internal virtual bool _autoresize { get; set; } = default!;
    internal virtual global::Doroti.Ui.Image? _childRaster { get; set; } = default;
    internal virtual Size? _childRasterSize { get; set; } = default;
    internal virtual bool _disableSnapshotAttempt { get; set; } = false;
    internal virtual Size? _lastCachedSize { get; set; } = default;

    internal _RenderSnapshotWidget__snapshot_widget(double devicePixelRatio, SnapshotController controller, SnapshotMode mode, SnapshotPainter painter, bool autoresize)
    {
        _devicePixelRatio = devicePixelRatio;
        _controller = controller;
        _mode = mode;
        _painter = painter;
        _autoresize = autoresize;
    }

    public virtual double devicePixelRatio
    {
        get => _devicePixelRatio;
        set
        {
            var __value = value;
            if (__value == devicePixelRatio)
            {
                return;
            }
            _devicePixelRatio = __value;
            if (_childRaster is null)
            {
                return;
            }
            else
            {
                _childRaster?.dispose();
                _childRaster = null;
                markNeedsPaint();
            }
        }
    }
    public virtual SnapshotPainter painter
    {
        get => _painter;
        set
        {
            var __value = value;
            if (Equals(__value, painter))
            {
                return;
            }
            SnapshotPainter oldPainter = painter;
            oldPainter.removeListener(markNeedsPaint);
            _painter = __value;
            if ((!Equals(DartRuntimePrimitives.RuntimeType(oldPainter), DartRuntimePrimitives.RuntimeType(painter))) || painter.shouldRepaint(oldPainter))
            {
                markNeedsPaint();
            }
            if (attached)
            {
                painter.addListener(markNeedsPaint);
            }
        }
    }
    public virtual SnapshotController controller
    {
        get => _controller;
        set
        {
            var __value = value;
            if (Equals(__value, controller))
            {
                return;
            }
            controller.removeListener(_onRasterValueChanged);
            bool oldValue = controller.allowSnapshotting;
            _controller = __value;
            if (attached)
            {
                controller.addListener(_onRasterValueChanged);
                if (oldValue != controller.allowSnapshotting)
                {
                    _onRasterValueChanged();
                }
            }
        }
    }
    public virtual SnapshotMode mode
    {
        get => _mode;
        set
        {
            var __value = value;
            if (Equals(__value, _mode))
            {
                return;
            }
            _mode = __value;
            markNeedsPaint();
        }
    }
    public virtual bool autoresize
    {
        get => _autoresize;
        set
        {
            var __value = value;
            if (__value == autoresize)
            {
                return;
            }
            _autoresize = __value;
            markNeedsPaint();
        }
    }
    public override void attach(global::Doroti.Framework.Rendering.PipelineOwner owner)
    {
        controller.addListener(_onRasterValueChanged);
        painter.addListener(markNeedsPaint);
        base.attach(owner);
    }

    public override void detach()
    {
        _disableSnapshotAttempt = false;
        controller.removeListener(_onRasterValueChanged);
        painter.removeListener(markNeedsPaint);
        _childRaster?.dispose();
        _childRaster = null;
        _childRasterSize = null;
        base.detach();
    }

    public override void dispose()
    {
        controller.removeListener(_onRasterValueChanged);
        painter.removeListener(markNeedsPaint);
        _childRaster?.dispose();
        _childRaster = null;
        _childRasterSize = null;
        base.dispose();
    }

    internal virtual void _onRasterValueChanged()
    {
        _disableSnapshotAttempt = false;
        _childRaster?.dispose();
        _childRaster = null;
        _childRasterSize = null;
        markNeedsPaint();
    }

    internal virtual global::Doroti.Ui.Image? _paintAndDetachToImage()
    {
        var offsetLayer = new global::Doroti.Framework.Rendering.OffsetLayer();
        var context = new global::Doroti.Framework.Rendering.PaintingContext(offsetLayer, Offset.zero & size);
        base.paint(context, Offset.zero);
        context.stopRecordingIfNeeded();
        if ((!Equals(mode, SnapshotMode.forced)) && !offsetLayer.supportsRasterization())
        {
            offsetLayer.dispose();
            if (Equals(mode, SnapshotMode.normal))
            {
                throw DartRuntimePrimitives.AsException(FlutterError.Create("SnapshotWidget used with a child that contains a PlatformView."));
            }
            _disableSnapshotAttempt = true;
            return null;
        }
        global::Doroti.Ui.Image image = offsetLayer.toImageSync(Offset.zero & size, pixelRatio: devicePixelRatio);
        offsetLayer.dispose();
        _lastCachedSize = size;
        return image;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        if (size.isEmpty)
        {
            _childRaster?.dispose();
            _childRaster = null;
            _childRasterSize = null;
            return;
        }
        if (!controller.allowSnapshotting || _disableSnapshotAttempt)
        {
            _childRaster?.dispose();
            _childRaster = null;
            _childRasterSize = null;
            painter.paint(context, offset, size, base.paint);
            return;
        }
        if (autoresize && (!Equals(size, _lastCachedSize)) && (_lastCachedSize is not null))
        {
            _childRaster?.dispose();
            _childRaster = null;
        }
        if (_childRaster is null)
        {
            _childRaster = _paintAndDetachToImage();
            _childRasterSize = size * devicePixelRatio;
        }
        if (_childRaster is null)
        {
            painter.paint(context, offset, size, base.paint);
        }
        else
        {
            painter.paintSnapshot(context, offset, size, _childRaster!, DartRuntimePrimitives.RequireValue(_childRasterSize), devicePixelRatio);
        }
    }

}

public abstract class SnapshotPainter : global::Doroti.Framework.Foundation.ChangeNotifier
{
    protected SnapshotPainter()
    {
    }

    public abstract void paintSnapshot(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset, Size size, global::Doroti.Ui.Image image, Size sourceSize, double pixelRatio);
    public abstract void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset, Size size, global::System.Action<global::Doroti.Framework.Rendering.PaintingContext, Offset> painter);
    public abstract bool shouldRepaint(SnapshotPainter oldPainter);
}

internal class _DefaultSnapshotPainter__snapshot_widget : SnapshotPainter
{
    internal _DefaultSnapshotPainter__snapshot_widget()
    {
    }

    public override void addListener(global::System.Action listener)
    {
    }

    public override void dispose()
    {
    }

    public new virtual bool hasListeners => false;
    public new virtual void notifyListeners()
    {
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset, Size size, global::System.Action<global::Doroti.Framework.Rendering.PaintingContext, Offset> painter)
    {
        painter(context, offset);
    }

    public override void paintSnapshot(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset, Size size, global::Doroti.Ui.Image image, Size sourceSize, double pixelRatio)
    {
        var src = Rect.fromLTWH(0, 0, sourceSize.width, sourceSize.height);
        var dst = Rect.fromLTWH(offset.dx, offset.dy, size.width, size.height);
        var paint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.filterQuality = FilterQuality.medium;
    return __cascade;
}))();
        context.canvas.drawImageRect(image, src, dst, paint);
    }

    public override void removeListener(global::System.Action listener)
    {
    }

    public override bool shouldRepaint(SnapshotPainter oldPainter) => false;
}

