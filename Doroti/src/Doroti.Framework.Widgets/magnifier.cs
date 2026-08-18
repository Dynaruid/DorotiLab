// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/magnifier.dart
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

public delegate Widget? MagnifierBuilder(BuildContext context, MagnifierController controller, global::Doroti.Framework.Foundation.ValueNotifier<MagnifierInfo> magnifierInfo);

public class MagnifierInfo
{
    public static MagnifierInfo empty = new MagnifierInfo(globalGesturePosition: Offset.zero, caretRect: Rect.zero, currentLineBoundaries: Rect.zero, fieldBounds: Rect.zero);
    public virtual Offset globalGesturePosition { get; private set; } = default!;
    public virtual Rect currentLineBoundaries { get; private set; } = default!;
    public virtual Rect caretRect { get; private set; } = default!;
    public virtual Rect fieldBounds { get; private set; } = default!;

    public MagnifierInfo(Offset globalGesturePosition, Rect caretRect, Rect fieldBounds, Rect currentLineBoundaries)
    {
        this.globalGesturePosition = globalGesturePosition;
        this.caretRect = caretRect;
        this.fieldBounds = fieldBounds;
        this.currentLineBoundaries = currentLineBoundaries;
    }

    public override bool Equals(object? other)
    {
        var __other = other as MagnifierInfo;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((__other is MagnifierInfo) && (object.Equals(((MagnifierInfo)((MagnifierInfo)__other)).globalGesturePosition, this.globalGesturePosition))) && (object.Equals(((MagnifierInfo)((MagnifierInfo)__other)).caretRect, this.caretRect))) && (object.Equals(((MagnifierInfo)((MagnifierInfo)__other)).currentLineBoundaries, this.currentLineBoundaries))) && (object.Equals(((MagnifierInfo)((MagnifierInfo)__other)).fieldBounds, this.fieldBounds)));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.globalGesturePosition, this.caretRect, this.fieldBounds, this.currentLineBoundaries));
    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "MagnifierInfo"))}(" + $"position: {this.globalGesturePosition}, " + $"line: {this.currentLineBoundaries}, " + $"caret: {this.caretRect}, " + $"field: {this.fieldBounds}" + ")";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class TextMagnifierConfiguration
{
    internal virtual global::System.Func<BuildContext, MagnifierController, global::Doroti.Framework.Foundation.ValueNotifier<MagnifierInfo>, Widget?>? _magnifierBuilder { get; private set; }
    public virtual bool shouldDisplayHandlesInMagnifier { get; private set; } = default!;
    public static TextMagnifierConfiguration disabled = new TextMagnifierConfiguration();

    public TextMagnifierConfiguration(global::System.Func<BuildContext, MagnifierController, global::Doroti.Framework.Foundation.ValueNotifier<MagnifierInfo>, Widget?>? magnifierBuilder = null, bool shouldDisplayHandlesInMagnifier = true)
    {
        this.shouldDisplayHandlesInMagnifier = shouldDisplayHandlesInMagnifier;
        this._magnifierBuilder = magnifierBuilder;
    }

    public virtual global::System.Func<BuildContext, MagnifierController, global::Doroti.Framework.Foundation.ValueNotifier<MagnifierInfo>, Widget?> magnifierBuilder => DartRuntimePrimitives.ConvertValue<global::System.Func<BuildContext, MagnifierController, global::Doroti.Framework.Foundation.ValueNotifier<MagnifierInfo>, Widget?>>(((this._magnifierBuilder ?? (global::System.Func<BuildContext, MagnifierController, global::Doroti.Framework.Foundation.ValueNotifier<MagnifierInfo>, Widget?>)_none)));
    internal static Widget? _none(BuildContext context, MagnifierController controller, global::Doroti.Framework.Foundation.ValueNotifier<MagnifierInfo> magnifierInfo) => DartRuntimePrimitives.ConvertValue<Widget>(null);
}

public class MagnifierController
{
    public virtual global::Doroti.Framework.Animation.AnimationController? animationController { get; set; } = default;
    internal virtual OverlayEntry? _overlayEntry { get; set; } = default;

    public MagnifierController(global::Doroti.Framework.Animation.AnimationController? animationController = null)
    {
        this.animationController = animationController;
    }

    public virtual OverlayEntry? overlayEntry => this._overlayEntry;
    public virtual bool shown => DartRuntimePrimitives.ConvertValue<bool>(((this.overlayEntry is not null) && ((this.animationController?.isForwardOrCompleted ?? true))));
    public async virtual Future show(BuildContext context, global::System.Func<BuildContext, Widget> builder, Widget? debugRequiredFor = null, OverlayEntry? below = null)
    {
        this._overlayEntry?.remove();
        this._overlayEntry?.dispose();
        OverlayState overlayState__9315 = ((OverlayState)(object?)Overlay.of(context, rootOverlay: true, debugRequiredFor: debugRequiredFor));
        CapturedThemes capturedThemes__9457 = ((CapturedThemes)(object?)InheritedTheme.capture(from: context, to: Navigator.maybeOf(context)?.context));
        _overlayEntry = new OverlayEntry(builder: ((global::System.Func<BuildContext, Widget>)((context) => capturedThemes__9457.wrap(builder(context)))));
        overlayState__9315.insert(this.overlayEntry!, below: below);
        if ((this.animationController is not null))
        {
            this.animationController?.forward();
        }
    }

    public async virtual Future hide(bool removeFromOverlay = true)
    {
        if ((this.overlayEntry is null))
        {
            return;
        }
        if ((this.animationController is not null))
        {
            this.animationController?.reverse();
        }
        if (removeFromOverlay)
        {
            this.removeFromOverlay();
        }
    }

    public virtual void removeFromOverlay()
    {
        this._overlayEntry?.remove();
        this._overlayEntry?.dispose();
        _overlayEntry = null;
    }

    public static global::Doroti.Ui.Rect shiftWithinBounds(Rect rect, Rect bounds)
    {
        DartRuntimePrimitives.Assert(() => (rect.width <= bounds.width), () => (object?)$"attempted to shift {rect} within {bounds}, but the rect has a greater width.");
        DartRuntimePrimitives.Assert(() => (rect.height <= bounds.height), () => (object?)$"attempted to shift {rect} within {bounds}, but the rect has a greater height.");
        global::Doroti.Ui.Offset rectShift__12514 = ((global::Doroti.Ui.Offset)(object?)Offset.zero);
        if ((rect.left < bounds.left))
        {
            rectShift__12514 += new global::Doroti.Ui.Offset((bounds.left - rect.left), 0);
        }
        else
        {
            if ((rect.right > bounds.right))
            {
                rectShift__12514 += new global::Doroti.Ui.Offset((bounds.right - rect.right), 0);
            }
        }
        if ((rect.top < bounds.top))
        {
            rectShift__12514 += new global::Doroti.Ui.Offset(0, (bounds.top - rect.top));
        }
        else
        {
            if ((rect.bottom > bounds.bottom))
            {
                rectShift__12514 += new global::Doroti.Ui.Offset(0, (bounds.bottom - rect.bottom));
            }
        }
        return rect.shift(rectShift__12514);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class MagnifierDecoration
{
    public virtual double opacity { get; private set; } = default!;
    public virtual List<global::Doroti.Framework.Painting.BoxShadow>? shadows { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder shape { get; private set; } = default!;

    public MagnifierDecoration(double opacity = 1.0, List<global::Doroti.Framework.Painting.BoxShadow>? shadows = null, global::Doroti.Framework.Painting.ShapeBorder shape = default!)
    {
        global::Doroti.Framework.Painting.ShapeBorder __shape = shape ?? new global::Doroti.Framework.Painting.RoundedRectangleBorder();
        this.opacity = opacity;
        this.shadows = shadows;
        this.shape = __shape;
    }

    public override bool Equals(object? other)
    {
        var __other = other as MagnifierDecoration;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((__other is MagnifierDecoration) && (((MagnifierDecoration)((MagnifierDecoration)__other)).opacity == this.opacity)) && global::Doroti.Framework.Foundation.CollectionsLibrary.listEquals<global::Doroti.Framework.Painting.BoxShadow>(((MagnifierDecoration)((MagnifierDecoration)__other)).shadows, this.shadows)) && (object.Equals(((MagnifierDecoration)((MagnifierDecoration)__other)).shape, this.shape)));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.opacity, this.shape, ((this.shadows is null) ? null : FoundationRuntimePorts.ObjectHashAll(this.shadows!))));
}

public class RawMagnifier : StatelessWidget
{
    public virtual Widget? child { get; private set; }
    public virtual MagnifierDecoration decoration { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual Offset focalPointOffset { get; private set; } = default!;
    public virtual double magnificationScale { get; private set; } = default!;
    public virtual Size size { get; private set; } = default!;

    public RawMagnifier(global::Doroti.Framework.Foundation.Key? key = null, Widget? child = null, MagnifierDecoration decoration = default!, Clip clipBehavior = Clip.none, Offset focalPointOffset = default, double magnificationScale = 1, Size size = default!) : base(key: key)
    {
        MagnifierDecoration __decoration = decoration ?? new MagnifierDecoration();
        this.child = child;
        this.decoration = __decoration;
        this.clipBehavior = clipBehavior;
        this.focalPointOffset = focalPointOffset;
        this.magnificationScale = magnificationScale;
        this.size = size;
        System.Diagnostics.Debug.Assert((magnificationScale != 0L));
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new Stack(clipBehavior: Clip.none, alignment: global::Doroti.Framework.Painting.Alignment.center, children: new List<Widget> { ClipPath.shape(shape: ((MagnifierDecoration)this.decoration).shape, child: new Opacity(opacity: ((MagnifierDecoration)this.decoration).opacity, child: new _Magnifier__magnifier(focalPointOffset: this.focalPointOffset, magnificationScale: this.magnificationScale, child: SizedBox.CreateFromSize(size: this.size, child: this.child)))), new IgnorePointer(child: new Opacity(opacity: ((MagnifierDecoration)this.decoration).opacity, child: new ClipPath(clipBehavior: this.clipBehavior, clipper: new _NegativeClip__magnifier(shape: ((MagnifierDecoration)this.decoration).shape), child: new DecoratedBox(decoration: new global::Doroti.Framework.Painting.ShapeDecoration(shape: ((MagnifierDecoration)this.decoration).shape, shadows: ((MagnifierDecoration)this.decoration).shadows), child: SizedBox.CreateFromSize(size: this.size))))) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _NegativeClip__magnifier : global::Doroti.Framework.Rendering.CustomClipper<Path>
{
    public virtual global::Doroti.Framework.Painting.ShapeBorder shape { get; private set; } = default!;

    internal _NegativeClip__magnifier(global::Doroti.Framework.Painting.ShapeBorder shape)
    {
        this.shape = shape;
    }

    public override Path getClip(Size size)
    {
        return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.fillType = PathFillType.evenOdd;
    __cascade.addRect(Rect.largest);
    __cascade.addPath(this.shape.getInnerPath((Offset.zero & size)), Offset.zero);
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldReclip(global::Doroti.Framework.Rendering.CustomClipper<Path> oldClipper) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(((_NegativeClip__magnifier)oldClipper).shape, this.shape)));
}

internal class _Magnifier__magnifier : SingleChildRenderObjectWidget
{
    public virtual Offset focalPointOffset { get; private set; } = default!;
    public virtual double magnificationScale { get; private set; } = default!;

    internal _Magnifier__magnifier(Widget? child = null, double magnificationScale = 1, Offset focalPointOffset = default) : base(child: child)
    {
        this.magnificationScale = magnificationScale;
        this.focalPointOffset = focalPointOffset;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderMagnification__magnifier(this.focalPointOffset, this.magnificationScale));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderMagnification__magnifier)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderMagnification__magnifier>)(() =>
{
    var __cascade = __renderObject;
    __cascade.focalPointOffset = this.focalPointOffset;
    __cascade.magnificationScale = this.magnificationScale;
    return __cascade;
}))());
    }

}

public class _RenderMagnification__magnifier : global::Doroti.Framework.Rendering.RenderProxyBox
{
    internal virtual Offset _focalPointOffset { get; set; } = default!;
    internal virtual double _magnificationScale { get; set; } = default!;

    internal _RenderMagnification__magnifier(Offset _focalPointOffset, double _magnificationScale, global::Doroti.Framework.Rendering.RenderBox? child = null) : base(child)
    {
        this._focalPointOffset = _focalPointOffset;
        this._magnificationScale = _magnificationScale;
    }

    public virtual global::Doroti.Ui.Offset focalPointOffset
    {
        get => this._focalPointOffset;
        set
        {
            var __value = value;
            if ((object.Equals(this._focalPointOffset, __value)))
            {
                return;
            }
            _focalPointOffset = __value;
            markNeedsPaint();
        }
    }
    public virtual double magnificationScale
    {
        get => this._magnificationScale;
        set
        {
            var __value = value;
            if ((this._magnificationScale == __value))
            {
                return;
            }
            _magnificationScale = __value;
            markNeedsPaint();
        }
    }
    public override bool alwaysNeedsCompositing => true;
    public override global::Doroti.Framework.Rendering.BackdropFilterLayer? layer => ((global::Doroti.Framework.Rendering.BackdropFilterLayer?)(object?)base.layer)!;
    public virtual void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        global::Doroti.Ui.Offset thisCenter__22916 = ((global::Doroti.Ui.Offset)(object?)(global::Doroti.Framework.Painting.Alignment.center.alongSize(this.size) + offset));
        var matrix__22982 = ((Func<Matrix4>)(() =>
{
    var __cascade = Matrix4.identity();
    __cascade.translateByDouble(((this.magnificationScale * ((((this.focalPointOffset.dx * -1L)) - thisCenter__22916.dx))) + thisCenter__22916.dx), ((this.magnificationScale * ((((this.focalPointOffset.dy * -1L)) - thisCenter__22916.dy))) + thisCenter__22916.dy), 0, 1);
    __cascade.scaleByDouble(this.magnificationScale, this.magnificationScale, this.magnificationScale, 1);
    return __cascade;
}))();
        var filter__23345 = new global::Doroti.Ui.ImageFilter(matrix__22982.storage, filterQuality: FilterQuality.high);
        if ((this.layer is null))
        {
            layer = new global::Doroti.Framework.Rendering.BackdropFilterLayer(filter: filter__23345);
        }
        else
        {
            this.layer!.filter = filter__23345;
        }
        context.pushLayer(this.layer!, (global::System.Action<global::Doroti.Framework.Rendering.PaintingContext, Offset>)base.paint, offset);
    }

}

