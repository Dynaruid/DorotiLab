// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/snapshot_widget.dart
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
        this._allowSnapshotting = allowSnapshotting;
    }

    public virtual void clear()
    {
        notifyListeners();
    }

    public virtual bool allowSnapshotting
    {
        get => this._allowSnapshotting;
        set
        {
            var __value = value;
            if ((__value == this.allowSnapshotting))
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
        global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context);
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderSnapshotWidget__snapshot_widget(controller: this.controller, mode: this.mode, devicePixelRatio: MediaQuery.devicePixelRatioOf(context), painter: this.painter, autoresize: this.autoresize));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context);
        DartRuntimePrimitives.Ignore(((Func<_RenderSnapshotWidget__snapshot_widget>)(() =>
{
    var __cascade = (((_RenderSnapshotWidget__snapshot_widget?)(object?)renderObject)!);
    __cascade.controller = this.controller;
    __cascade.mode = this.mode;
    __cascade.devicePixelRatio = MediaQuery.devicePixelRatioOf(context);
    __cascade.painter = this.painter;
    __cascade.autoresize = this.autoresize;
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
        this._devicePixelRatio = devicePixelRatio;
        this._controller = controller;
        this._mode = mode;
        this._painter = painter;
        this._autoresize = autoresize;
    }

    public virtual double devicePixelRatio
    {
        get => this._devicePixelRatio;
        set
        {
            var __value = value;
            if ((__value == this.devicePixelRatio))
            {
                return;
            }
            _devicePixelRatio = __value;
            if ((this._childRaster is null))
            {
                return;
            }
            else
            {
                this._childRaster?.dispose();
                _childRaster = null;
                markNeedsPaint();
            }
        }
    }
    public virtual SnapshotPainter painter
    {
        get => this._painter;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this.painter)))
            {
                return;
            }
            SnapshotPainter oldPainter = this.painter;
            oldPainter.removeListener(() => this.markNeedsPaint());
            _painter = __value;
            if (((!object.Equals(DartRuntimePrimitives.RuntimeType(oldPainter), DartRuntimePrimitives.RuntimeType(this.painter))) || this.painter.shouldRepaint(oldPainter)))
            {
                markNeedsPaint();
            }
            if (this.attached)
            {
                this.painter.addListener(() => this.markNeedsPaint());
            }
        }
    }
    public virtual SnapshotController controller
    {
        get => this._controller;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this.controller)))
            {
                return;
            }
            this.controller.removeListener(() => this._onRasterValueChanged());
            bool oldValue = ((SnapshotController)this.controller).allowSnapshotting;
            _controller = __value;
            if (this.attached)
            {
                this.controller.addListener(() => this._onRasterValueChanged());
                if ((oldValue != ((SnapshotController)this.controller).allowSnapshotting))
                {
                    _onRasterValueChanged();
                }
            }
        }
    }
    public virtual SnapshotMode mode
    {
        get => this._mode;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._mode)))
            {
                return;
            }
            _mode = __value;
            markNeedsPaint();
        }
    }
    public virtual bool autoresize
    {
        get => this._autoresize;
        set
        {
            var __value = value;
            if ((__value == this.autoresize))
            {
                return;
            }
            _autoresize = __value;
            markNeedsPaint();
        }
    }
    public virtual void attach(global::Doroti.Framework.Rendering.PipelineOwner owner)
    {
        this.controller.addListener(() => this._onRasterValueChanged());
        this.painter.addListener(() => this.markNeedsPaint());
        base.attach(owner);
    }

    public virtual void detach()
    {
        _disableSnapshotAttempt = false;
        this.controller.removeListener(() => this._onRasterValueChanged());
        this.painter.removeListener(() => this.markNeedsPaint());
        this._childRaster?.dispose();
        _childRaster = null;
        _childRasterSize = null;
        base.detach();
    }

    public override void dispose()
    {
        this.controller.removeListener(() => this._onRasterValueChanged());
        this.painter.removeListener(() => this.markNeedsPaint());
        this._childRaster?.dispose();
        _childRaster = null;
        _childRasterSize = null;
        base.dispose();
    }

    internal virtual void _onRasterValueChanged()
    {
        _disableSnapshotAttempt = false;
        this._childRaster?.dispose();
        _childRaster = null;
        _childRasterSize = null;
        markNeedsPaint();
    }

    internal virtual global::Doroti.Ui.Image? _paintAndDetachToImage()
    {
        var offsetLayer = new global::Doroti.Framework.Rendering.OffsetLayer();
        var context = new global::Doroti.Framework.Rendering.PaintingContext(offsetLayer, (Offset.zero & this.size));
        base.paint(context, Offset.zero);
        context.stopRecordingIfNeeded();
        if (((!object.Equals(this.mode, SnapshotMode.forced)) && !offsetLayer.supportsRasterization()))
        {
            offsetLayer.dispose();
            if ((object.Equals(this.mode, SnapshotMode.normal)))
            {
                throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("SnapshotWidget used with a child that contains a PlatformView."));
            }
            _disableSnapshotAttempt = true;
            return ((global::Doroti.Ui.Image)(object)null);
        }
        global::Doroti.Ui.Image image = ((global::Doroti.Ui.Image)(object?)offsetLayer.toImageSync((Offset.zero & this.size), pixelRatio: this.devicePixelRatio));
        offsetLayer.dispose();
        _lastCachedSize = this.size;
        return image;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        if (this.size.isEmpty)
        {
            this._childRaster?.dispose();
            _childRaster = null;
            _childRasterSize = null;
            return;
        }
        if ((!((SnapshotController)this.controller).allowSnapshotting || this._disableSnapshotAttempt))
        {
            this._childRaster?.dispose();
            _childRaster = null;
            _childRasterSize = null;
            this.painter.paint(context, offset, this.size, (global::System.Action<global::Doroti.Framework.Rendering.PaintingContext, Offset>)base.paint);
            return;
        }
        if (((this.autoresize && (!object.Equals(this.size, this._lastCachedSize))) && (this._lastCachedSize is not null)))
        {
            this._childRaster?.dispose();
            _childRaster = null;
        }
        if ((this._childRaster is null))
        {
            _childRaster = _paintAndDetachToImage();
            _childRasterSize = (this.size * this.devicePixelRatio);
        }
        if ((this._childRaster is null))
        {
            this.painter.paint(context, offset, this.size, (global::System.Action<global::Doroti.Framework.Rendering.PaintingContext, Offset>)base.paint);
        }
        else
        {
            this.painter.paintSnapshot(context, offset, this.size, this._childRaster!, DartRuntimePrimitives.RequireValue(this._childRasterSize), this.devicePixelRatio);
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

    public virtual void addListener(global::System.Action listener)
    {
    }

    public virtual void dispose()
    {
    }

    public virtual bool hasListeners => false;
    public virtual void notifyListeners()
    {
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset, Size size, global::System.Action<global::Doroti.Framework.Rendering.PaintingContext, Offset> painter)
    {
        painter(context, offset);
    }

    public override void paintSnapshot(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset, Size size, global::Doroti.Ui.Image image, Size sourceSize, double pixelRatio)
    {
        var src = global::Doroti.Ui.Rect.fromLTWH(0, 0, sourceSize.width, sourceSize.height);
        var dst = global::Doroti.Ui.Rect.fromLTWH(offset.dx, offset.dy, size.width, size.height);
        var paint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.filterQuality = FilterQuality.medium;
    return __cascade;
}))();
        ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawImageRect(image, src, dst, paint);
    }

    public virtual void removeListener(global::System.Action listener)
    {
    }

    public override bool shouldRepaint(SnapshotPainter oldPainter) => false;
}

