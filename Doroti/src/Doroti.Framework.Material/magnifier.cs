// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/magnifier.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class TextMagnifier : global::Doroti.Framework.Widgets.StatefulWidget
{
    public static global::Doroti.Framework.Widgets.TextMagnifierConfiguration adaptiveMagnifierConfiguration = new global::Doroti.Framework.Widgets.TextMagnifierConfiguration(shouldDisplayHandlesInMagnifier: Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.iOS), magnifierBuilder: (context, controller, magnifierInfo) =>
    {
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.iOS:
                {
                    return (global::Doroti.Framework.Widgets.Widget?)new CupertinoTextMagnifier(controller: controller, magnifierInfo: magnifierInfo);
                }
            case TargetPlatform.android:
                {
                    return (global::Doroti.Framework.Widgets.Widget?)new TextMagnifier(magnifierInfo: magnifierInfo);
                }
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.macOS:
            case TargetPlatform.windows:
                {
                    return null;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart closure completed without a value.");
    });
    public static Duration jumpBetweenLinesAnimationDuration = Duration.Create(milliseconds: 70L);
    public virtual global::Doroti.Framework.Foundation.ValueNotifier<global::Doroti.Framework.Widgets.MagnifierInfo> magnifierInfo { get; private set; } = default!;

    public TextMagnifier(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Foundation.ValueNotifier<global::Doroti.Framework.Widgets.MagnifierInfo> magnifierInfo = default!) : base(key: key)
    {
        this.magnifierInfo = magnifierInfo;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _TextMagnifierState__magnifier());
}

internal class _TextMagnifierState__magnifier : global::Doroti.Framework.Widgets.State<TextMagnifier>
{
    internal virtual global::System.Action _magnifierInfoListener { get; set; } = default!;
    internal virtual Offset? _magnifierPosition { get; set; } = default;
    internal virtual Timer? _positionShouldBeAnimatedTimer { get; set; } = default;
    internal virtual Offset _extraFocalPointOffset { get; set; } = Offset.zero;

    internal virtual bool _positionShouldBeAnimated => DartRuntimePrimitives.ConvertValue<bool>(_positionShouldBeAnimatedTimer is not null);
    public override void initState()
    {
        base.initState();
        _magnifierInfoListener = _determineMagnifierPositionAndFocalPoint;
        widget.magnifierInfo.addListener(_magnifierInfoListener);
    }

    public override void dispose()
    {
        widget.magnifierInfo.removeListener(_magnifierInfoListener);
        _positionShouldBeAnimatedTimer?.cancel();
        base.dispose();
    }

    public override void didChangeDependencies()
    {
        _determineMagnifierPositionAndFocalPoint();
        base.didChangeDependencies();
    }

    public override void didUpdateWidget(TextMagnifier oldWidget)
    {
        if (!Equals(oldWidget.magnifierInfo, widget.magnifierInfo))
        {
            oldWidget.magnifierInfo.removeListener(_magnifierInfoListener);
            widget.magnifierInfo.addListener(_magnifierInfoListener);
        }
        base.didUpdateWidget(oldWidget);
    }

    internal virtual void _determineMagnifierPositionAndFocalPoint()
    {
        global::Doroti.Framework.Widgets.MagnifierInfo selectionInfo = widget.magnifierInfo.value;
        global::Doroti.Ui.Rect screenRect = Offset.zero & MediaQuery.sizeOf(context);
        var basicMagnifierOffset = new global::Doroti.Ui.Offset(Magnifier.kDefaultMagnifierSize.width / 2L, Magnifier.kDefaultMagnifierSize.height + Magnifier.kStandardVerticalFocalPointShift);
        double magnifierX = Dart_uiLibrary.clampDouble(selectionInfo.globalGesturePosition.dx, selectionInfo.currentLineBoundaries.left, selectionInfo.currentLineBoundaries.right);
        global::Doroti.Ui.Rect unadjustedMagnifierRect = (new global::Doroti.Ui.Offset(magnifierX, selectionInfo.caretRect.center.dy) - basicMagnifierOffset) & Magnifier.kDefaultMagnifierSize;
        global::Doroti.Ui.Rect screenBoundsAdjustedMagnifierRect = MagnifierController.shiftWithinBounds(bounds: screenRect, rect: unadjustedMagnifierRect);
        global::Doroti.Ui.Offset finalMagnifierPosition = screenBoundsAdjustedMagnifierRect.topLeft;
        double horizontalMaxFocalPointEdgeInsets = Magnifier.kDefaultMagnifierSize.width / 2L / Magnifier._magnification;
        double newGlobalFocalPointX = default!;
        if (selectionInfo.fieldBounds.width < (horizontalMaxFocalPointEdgeInsets * 2L))
        {
            newGlobalFocalPointX = selectionInfo.fieldBounds.center.dx;
        }
        else
        {
            newGlobalFocalPointX = Dart_uiLibrary.clampDouble(screenBoundsAdjustedMagnifierRect.center.dx, selectionInfo.fieldBounds.left + horizontalMaxFocalPointEdgeInsets, selectionInfo.fieldBounds.right - horizontalMaxFocalPointEdgeInsets);
        }
        double newRelativeFocalPointX = newGlobalFocalPointX - screenBoundsAdjustedMagnifierRect.center.dx;
        var focalPointAdjustmentForScreenBoundsAdjustment = new global::Doroti.Ui.Offset(newRelativeFocalPointX, unadjustedMagnifierRect.top - screenBoundsAdjustedMagnifierRect.top);
        Timer? positionShouldBeAnimated = _positionShouldBeAnimatedTimer;
        if ((_magnifierPosition is not null) && (finalMagnifierPosition.dy != DartRuntimePrimitives.RequireValue(_magnifierPosition).dy))
        {
            if ((_positionShouldBeAnimatedTimer is not null) && _positionShouldBeAnimatedTimer!.isActive)
            {
                _positionShouldBeAnimatedTimer!.cancel();
            }
            positionShouldBeAnimated = new Timer(TextMagnifier.jumpBetweenLinesAnimationDuration, () =>
            {
                setState(() =>
                {
                    _positionShouldBeAnimatedTimer = null;
                });
            });
        }
        setState(() =>
        {
            _magnifierPosition = finalMagnifierPosition;
            _positionShouldBeAnimatedTimer = positionShouldBeAnimated;
            _extraFocalPointOffset = focalPointAdjustmentForScreenBoundsAdjustment;
        });
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => _magnifierPosition is not null, () => (object?)"Magnifier position should only be null before the first build.");
        return new global::Doroti.Framework.Widgets.AnimatedPositioned(top: DartRuntimePrimitives.RequireValue(_magnifierPosition).dy, left: DartRuntimePrimitives.RequireValue(_magnifierPosition).dx, duration: _positionShouldBeAnimated ? TextMagnifier.jumpBetweenLinesAnimationDuration : Duration.zero, child: new Magnifier(additionalFocalPointOffset: _extraFocalPointOffset));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class Magnifier : global::Doroti.Framework.Widgets.StatelessWidget
{
    public static Size kDefaultMagnifierSize = new global::Doroti.Ui.Size(77.37, 37.9);
    public const double kStandardVerticalFocalPointShift = 22.0;
    internal const double _borderRadius = 40;
    internal const double _magnification = 1.25;
    public virtual Offset additionalFocalPointOffset { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.BorderRadius borderRadius { get; private set; } = default!;
    public virtual Color filmColor { get; private set; } = default!;
    public virtual List<global::Doroti.Framework.Painting.BoxShadow> shadows { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual Size size { get; private set; } = default!;

    public Magnifier(global::Doroti.Framework.Foundation.Key? key = null, Offset additionalFocalPointOffset = default, global::Doroti.Framework.Painting.BorderRadius borderRadius = default!, Color filmColor = default!, List<global::Doroti.Framework.Painting.BoxShadow> shadows = default!, Clip clipBehavior = Clip.hardEdge, Size? size = null) : base(key: key)
    {
        global::Doroti.Framework.Painting.BorderRadius __borderRadius = borderRadius ?? BorderRadius.CreateAll(Radius.circular(_borderRadius));
        Color __filmColor = filmColor ?? Color.CreateFromARGB(8, 158, 158, 158);
        List<global::Doroti.Framework.Painting.BoxShadow> __shadows = shadows ?? new List<global::Doroti.Framework.Painting.BoxShadow> { new global::Doroti.Framework.Painting.BoxShadow(blurRadius: 1.5, offset: new Offset(0.0, 2.0), spreadRadius: 0.75, color: Color.fromARGB(25, 0, 0, 0)) };
        Size __size = size ?? kDefaultMagnifierSize;
        this.additionalFocalPointOffset = additionalFocalPointOffset;
        this.borderRadius = __borderRadius;
        this.filmColor = __filmColor;
        this.shadows = __shadows;
        this.clipBehavior = clipBehavior;
        this.size = __size;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.RawMagnifier(decoration: new global::Doroti.Framework.Widgets.MagnifierDecoration(shape: new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: borderRadius), shadows: shadows), clipBehavior: clipBehavior, magnificationScale: _magnification, focalPointOffset: additionalFocalPointOffset + new global::Doroti.Ui.Offset(0, kStandardVerticalFocalPointShift + (kDefaultMagnifierSize.height / 2L)), size: DartRuntimePrimitives.RequireValue(size), child: new global::Doroti.Framework.Widgets.ColoredBox(color: filmColor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
