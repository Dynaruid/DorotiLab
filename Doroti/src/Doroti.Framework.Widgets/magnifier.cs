// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/magnifier.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public delegate Widget? MagnifierBuilder(
    BuildContext context,
    MagnifierController controller,
    ValueNotifier<MagnifierInfo> magnifierInfo
);

public class MagnifierInfo
{
    public static MagnifierInfo empty = new MagnifierInfo(
        globalGesturePosition: Offset.zero,
        caretRect: Rect.zero,
        currentLineBoundaries: Rect.zero,
        fieldBounds: Rect.zero
    );
    public virtual Offset globalGesturePosition { get; private set; } = default!;
    public virtual Rect currentLineBoundaries { get; private set; } = default!;
    public virtual Rect caretRect { get; private set; } = default!;
    public virtual Rect fieldBounds { get; private set; } = default!;

    public MagnifierInfo(
        Offset globalGesturePosition,
        Rect caretRect,
        Rect fieldBounds,
        Rect currentLineBoundaries
    )
    {
        this.globalGesturePosition = globalGesturePosition;
        this.caretRect = caretRect;
        this.fieldBounds = fieldBounds;
        this.currentLineBoundaries = currentLineBoundaries;
    }

    public override bool Equals(object? other)
    {
        var __other = other as MagnifierInfo;
        if (__other is null)
        {
            return false;
        }

        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is MagnifierInfo)
            && Equals(__other.globalGesturePosition, globalGesturePosition)
            && Equals(__other.caretRect, caretRect)
            && Equals(__other.currentLineBoundaries, currentLineBoundaries)
            && Equals(__other.fieldBounds, fieldBounds);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(
                globalGesturePosition,
                caretRect,
                fieldBounds,
                currentLineBoundaries
            )
        );

    public override string ToString()
    {
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "MagnifierInfo")}("
            + $"position: {globalGesturePosition}, "
            + $"line: {currentLineBoundaries}, "
            + $"caret: {caretRect}, "
            + $"field: {fieldBounds}"
            + ")";
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class TextMagnifierConfiguration
{
    internal virtual Func<
        BuildContext,
        MagnifierController,
        ValueNotifier<MagnifierInfo>,
        Widget?
    >? _magnifierBuilder { get; private set; }
    public virtual bool shouldDisplayHandlesInMagnifier { get; private set; } = default!;
    public static TextMagnifierConfiguration disabled = new TextMagnifierConfiguration();

    public TextMagnifierConfiguration(
        Func<
            BuildContext,
            MagnifierController,
            ValueNotifier<MagnifierInfo>,
            Widget?
        >? magnifierBuilder = null,
        bool shouldDisplayHandlesInMagnifier = true
    )
    {
        this.shouldDisplayHandlesInMagnifier = shouldDisplayHandlesInMagnifier;
        _magnifierBuilder = magnifierBuilder;
    }

    public virtual Func<
        BuildContext,
        MagnifierController,
        ValueNotifier<MagnifierInfo>,
        Widget?
    > magnifierBuilder =>
        DartRuntimePrimitives.ConvertValue<
            Func<BuildContext, MagnifierController, ValueNotifier<MagnifierInfo>, Widget?>
        >(_magnifierBuilder ?? _none);

    internal static Widget? _none(
        BuildContext context,
        MagnifierController controller,
        ValueNotifier<MagnifierInfo> magnifierInfo
    ) => DartRuntimePrimitives.ConvertValue<Widget>(null);
}

public class MagnifierController
{
    public virtual AnimationController? animationController { get; set; } = default;
    internal virtual OverlayEntry? _overlayEntry { get; set; } = default;

    public MagnifierController(AnimationController? animationController = null)
    {
        this.animationController = animationController;
    }

    public virtual OverlayEntry? overlayEntry => _overlayEntry;
    public virtual bool shown =>
        DartRuntimePrimitives.ConvertValue<bool>(
            (overlayEntry is not null) && (animationController?.isForwardOrCompleted ?? true)
        );

    public virtual async Future show(
        BuildContext context,
        Func<BuildContext, Widget> builder,
        Widget? debugRequiredFor = null,
        OverlayEntry? below = null
    )
    {
        _overlayEntry?.remove();
        _overlayEntry?.dispose();
        OverlayState overlayState = Overlay.of(
            context,
            rootOverlay: true,
            debugRequiredFor: debugRequiredFor
        );
        CapturedThemes capturedThemes = InheritedTheme.capture(
            from: context,
            to: Navigator.maybeOf(context)?.context
        );
        _overlayEntry = new OverlayEntry(
            builder: (context) => capturedThemes.wrap(builder(context))
        );
        overlayState.insert(overlayEntry!, below: below);
        if (animationController is not null)
        {
            animationController?.forward();
        }
    }

    public virtual async Future hide(bool removeFromOverlay = true)
    {
        if (overlayEntry is null)
        {
            return;
        }
        if (animationController is not null)
        {
            animationController?.reverse();
        }
        if (removeFromOverlay)
        {
            this.removeFromOverlay();
        }
    }

    public virtual void removeFromOverlay()
    {
        _overlayEntry?.remove();
        _overlayEntry?.dispose();
        _overlayEntry = null;
    }

    public static Rect shiftWithinBounds(Rect rect, Rect bounds)
    {
        DartRuntimePrimitives.Assert(
            () => rect.width <= bounds.width,
            () =>
                (object?)
                    $"attempted to shift {rect} within {bounds}, but the rect has a greater width."
        );
        DartRuntimePrimitives.Assert(
            () => rect.height <= bounds.height,
            () =>
                (object?)
                    $"attempted to shift {rect} within {bounds}, but the rect has a greater height."
        );
        Offset rectShift = Offset.zero;
        if (rect.left < bounds.left)
        {
            rectShift += new Offset(bounds.left - rect.left, 0);
        }
        else
        {
            if (rect.right > bounds.right)
            {
                rectShift += new Offset(bounds.right - rect.right, 0);
            }
        }
        if (rect.top < bounds.top)
        {
            rectShift += new Offset(0, bounds.top - rect.top);
        }
        else
        {
            if (rect.bottom > bounds.bottom)
            {
                rectShift += new Offset(0, bounds.bottom - rect.bottom);
            }
        }
        return rect.shift(rectShift);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class MagnifierDecoration
{
    public virtual double opacity { get; private set; } = default!;
    public virtual List<BoxShadow>? shadows { get; private set; }
    public virtual ShapeBorder shape { get; private set; } = default!;

    public MagnifierDecoration(
        double opacity = 1.0,
        List<BoxShadow>? shadows = null,
        ShapeBorder shape = default!
    )
    {
        ShapeBorder __shape = shape ?? new RoundedRectangleBorder();
        this.opacity = opacity;
        this.shadows = shadows;
        this.shape = __shape;
    }

    public override bool Equals(object? other)
    {
        var __other = other as MagnifierDecoration;
        if (__other is null)
        {
            return false;
        }

        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is MagnifierDecoration)
            && (__other.opacity == opacity)
            && CollectionsLibrary.listEquals(__other.shadows, shadows)
            && Equals(__other.shape, shape);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(
                opacity,
                shape,
                (shadows is null) ? null : FoundationRuntimePorts.ObjectHashAll(shadows!)
            )
        );
}

public class RawMagnifier : StatelessWidget
{
    public virtual Widget? child { get; private set; }
    public virtual MagnifierDecoration decoration { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual Offset focalPointOffset { get; private set; } = default!;
    public virtual double magnificationScale { get; private set; } = default!;
    public virtual Size size { get; private set; } = default!;

    public RawMagnifier(
        Key? key = null,
        Widget? child = null,
        MagnifierDecoration decoration = default!,
        Clip clipBehavior = Clip.none,
        Offset focalPointOffset = default,
        double magnificationScale = 1,
        Size size = default!
    )
        : base(key: key)
    {
        MagnifierDecoration __decoration = decoration ?? new MagnifierDecoration();
        this.child = child;
        this.decoration = __decoration;
        this.clipBehavior = clipBehavior;
        this.focalPointOffset = focalPointOffset;
        this.magnificationScale = magnificationScale;
        this.size = size;
        System.Diagnostics.Debug.Assert(magnificationScale != 0L);
    }

    public override Widget build(BuildContext context)
    {
        return new Stack(
            clipBehavior: Clip.none,
            alignment: Alignment.center,
            children: new List<Widget>
            {
                ClipPath.shape(
                    shape: decoration.shape,
                    child: new Opacity(
                        opacity: decoration.opacity,
                        child: new _Magnifier__magnifier(
                            focalPointOffset: focalPointOffset,
                            magnificationScale: magnificationScale,
                            child: SizedBox.CreateFromSize(size: size, child: child)
                        )
                    )
                ),
                new IgnorePointer(
                    child: new Opacity(
                        opacity: decoration.opacity,
                        child: new ClipPath(
                            clipBehavior: clipBehavior,
                            clipper: new _NegativeClip__magnifier(shape: decoration.shape),
                            child: new DecoratedBox(
                                decoration: new ShapeDecoration(
                                    shape: decoration.shape,
                                    shadows: decoration.shadows
                                ),
                                child: SizedBox.CreateFromSize(size: size)
                            )
                        )
                    )
                ),
            }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _NegativeClip__magnifier : CustomClipper<Path>
{
    public virtual ShapeBorder shape { get; private set; } = default!;

    internal _NegativeClip__magnifier(ShapeBorder shape)
    {
        this.shape = shape;
    }

    public override Path getClip(Size size)
    {
        return (
            (Func<Path>)(
                () =>
                {
                    var __cascade = new Path();
                    __cascade.fillType = PathFillType.evenOdd;
                    __cascade.addRect(Rect.largest);
                    __cascade.addPath(shape.getInnerPath(Offset.zero & size), Offset.zero);
                    return __cascade;
                }
            )
        )();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool shouldReclip(CustomClipper<Path> oldClipper) =>
        DartRuntimePrimitives.ConvertValue<bool>(
            !Equals(((_NegativeClip__magnifier)oldClipper).shape, shape)
        );
}

internal class _Magnifier__magnifier : SingleChildRenderObjectWidget
{
    public virtual Offset focalPointOffset { get; private set; } = default!;
    public virtual double magnificationScale { get; private set; } = default!;

    internal _Magnifier__magnifier(
        Widget? child = null,
        double magnificationScale = 1,
        Offset focalPointOffset = default
    )
        : base(child: child)
    {
        this.magnificationScale = magnificationScale;
        this.focalPointOffset = focalPointOffset;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderMagnification__magnifier(focalPointOffset, magnificationScale);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderMagnification__magnifier)renderObject;
        DartRuntimePrimitives.Ignore(
            (
                (Func<_RenderMagnification__magnifier>)(
                    () =>
                    {
                        var __cascade = __renderObject;
                        __cascade.focalPointOffset = focalPointOffset;
                        __cascade.magnificationScale = magnificationScale;
                        return __cascade;
                    }
                )
            )()
        );
    }
}

public class _RenderMagnification__magnifier : RenderProxyBox
{
    internal virtual Offset _focalPointOffset { get; set; } = default!;
    internal virtual double _magnificationScale { get; set; } = default!;

    internal _RenderMagnification__magnifier(
        Offset _focalPointOffset,
        double _magnificationScale,
        RenderBox? child = null
    )
        : base(child)
    {
        this._focalPointOffset = _focalPointOffset;
        this._magnificationScale = _magnificationScale;
    }

    public virtual Offset focalPointOffset
    {
        get => _focalPointOffset;
        set
        {
            var __value = value;
            if (Equals(_focalPointOffset, __value))
            {
                return;
            }
            _focalPointOffset = __value;
            markNeedsPaint();
        }
    }
    public virtual double magnificationScale
    {
        get => _magnificationScale;
        set
        {
            var __value = value;
            if (_magnificationScale == __value)
            {
                return;
            }
            _magnificationScale = __value;
            markNeedsPaint();
        }
    }
    public override bool alwaysNeedsCompositing => true;
    public override BackdropFilterLayer? layer => ((BackdropFilterLayer?)base.layer)!;

    public override void paint(PaintingContext context, Offset offset)
    {
        Offset thisCenter = Alignment.center.alongSize(size) + offset;
        var matrix = (
            (Func<Matrix4>)(
                () =>
                {
                    var __cascade = Matrix4.identity();
                    __cascade.translateByDouble(
                        (magnificationScale * ((focalPointOffset.dx * -1L) - thisCenter.dx))
                            + thisCenter.dx,
                        (magnificationScale * ((focalPointOffset.dy * -1L) - thisCenter.dy))
                            + thisCenter.dy,
                        0,
                        1
                    );
                    __cascade.scaleByDouble(
                        magnificationScale,
                        magnificationScale,
                        magnificationScale,
                        1
                    );
                    return __cascade;
                }
            )
        )();
        var filterLocal = new ImageFilter(matrix.storage, filterQuality: FilterQuality.high);
        if (layer is null)
        {
            layer = new BackdropFilterLayer(filter: filterLocal);
        }
        else
        {
            layer!.filter = filterLocal;
        }
        context.pushLayer(layer!, base.paint, offset);
    }
}
