// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/magnifier.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

public class TextMagnifier : global::Doroti.Framework.Widgets.StatefulWidget
{
    public static global::Doroti.Framework.Widgets.TextMagnifierConfiguration adaptiveMagnifierConfiguration = new global::Doroti.Framework.Widgets.TextMagnifierConfiguration(shouldDisplayHandlesInMagnifier: (object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS)), magnifierBuilder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.MagnifierController, global::Doroti.Framework.Foundation.ValueNotifier<global::Doroti.Framework.Widgets.MagnifierInfo>, global::Doroti.Framework.Widgets.Widget?>?)((context, controller, magnifierInfo) =>
    {
        switch (global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                {
                    return ((global::Doroti.Framework.Widgets.Widget?)(object?)new CupertinoTextMagnifier(controller: controller, magnifierInfo: magnifierInfo));
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
                {
                    return ((global::Doroti.Framework.Widgets.Widget?)(object?)new TextMagnifier(magnifierInfo: magnifierInfo));
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    return null;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart closure completed without a value.");
    })));
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

    internal virtual bool _positionShouldBeAnimated => DartRuntimePrimitives.ConvertValue<bool>((this._positionShouldBeAnimatedTimer is not null));
    public override void initState()
    {
        base.initState();
        _magnifierInfoListener = this._determineMagnifierPositionAndFocalPoint;
        ((TextMagnifier)this.widget).magnifierInfo.addListener(this._magnifierInfoListener);
    }

    public override void dispose()
    {
        ((TextMagnifier)this.widget).magnifierInfo.removeListener(this._magnifierInfoListener);
        this._positionShouldBeAnimatedTimer?.cancel();
        base.dispose();
    }

    public override void didChangeDependencies()
    {
        _determineMagnifierPositionAndFocalPoint();
        base.didChangeDependencies();
    }

    public override void didUpdateWidget(TextMagnifier oldWidget)
    {
        if ((!object.Equals(((TextMagnifier)oldWidget).magnifierInfo, ((TextMagnifier)this.widget).magnifierInfo)))
        {
            ((TextMagnifier)oldWidget).magnifierInfo.removeListener(this._magnifierInfoListener);
            ((TextMagnifier)this.widget).magnifierInfo.addListener(this._magnifierInfoListener);
        }
        base.didUpdateWidget(oldWidget);
    }

    internal virtual void _determineMagnifierPositionAndFocalPoint()
    {
        global::Doroti.Framework.Widgets.MagnifierInfo selectionInfo = ((TextMagnifier)this.widget).magnifierInfo.value;
        global::Doroti.Ui.Rect screenRect = ((global::Doroti.Ui.Rect)(object?)(Offset.zero & MediaQuery.sizeOf(this.context)));
        var basicMagnifierOffset = new global::Doroti.Ui.Offset((Magnifier.kDefaultMagnifierSize.width / 2L), (Magnifier.kDefaultMagnifierSize.height + Magnifier.kStandardVerticalFocalPointShift));
        double magnifierX = Dart_uiLibrary.clampDouble(((global::Doroti.Framework.Widgets.MagnifierInfo)selectionInfo).globalGesturePosition.dx, ((global::Doroti.Framework.Widgets.MagnifierInfo)selectionInfo).currentLineBoundaries.left, ((global::Doroti.Framework.Widgets.MagnifierInfo)selectionInfo).currentLineBoundaries.right);
        global::Doroti.Ui.Rect unadjustedMagnifierRect = ((global::Doroti.Ui.Rect)(object?)((new global::Doroti.Ui.Offset(magnifierX, ((Offset)((dynamic)((global::Doroti.Framework.Widgets.MagnifierInfo)selectionInfo).caretRect).center).dy) - basicMagnifierOffset) & Magnifier.kDefaultMagnifierSize));
        global::Doroti.Ui.Rect screenBoundsAdjustedMagnifierRect = ((global::Doroti.Ui.Rect)(object?)MagnifierController.shiftWithinBounds(bounds: screenRect, rect: unadjustedMagnifierRect));
        global::Doroti.Ui.Offset finalMagnifierPosition = ((global::Doroti.Ui.Offset)(object?)screenBoundsAdjustedMagnifierRect.topLeft);
        double horizontalMaxFocalPointEdgeInsets = (((Magnifier.kDefaultMagnifierSize.width / 2L)) / Magnifier._magnification);
        double newGlobalFocalPointX = default!;
        if ((((global::Doroti.Framework.Widgets.MagnifierInfo)selectionInfo).fieldBounds.width < (horizontalMaxFocalPointEdgeInsets * 2L)))
        {
            newGlobalFocalPointX = ((Offset)((dynamic)((global::Doroti.Framework.Widgets.MagnifierInfo)selectionInfo).fieldBounds).center).dx;
        }
        else
        {
            newGlobalFocalPointX = Dart_uiLibrary.clampDouble(((Offset)((dynamic)screenBoundsAdjustedMagnifierRect).center).dx, (((global::Doroti.Framework.Widgets.MagnifierInfo)selectionInfo).fieldBounds.left + horizontalMaxFocalPointEdgeInsets), (((global::Doroti.Framework.Widgets.MagnifierInfo)selectionInfo).fieldBounds.right - horizontalMaxFocalPointEdgeInsets));
        }
        double newRelativeFocalPointX = (newGlobalFocalPointX - ((Offset)((dynamic)screenBoundsAdjustedMagnifierRect).center).dx);
        var focalPointAdjustmentForScreenBoundsAdjustment = new global::Doroti.Ui.Offset(newRelativeFocalPointX, (unadjustedMagnifierRect.top - screenBoundsAdjustedMagnifierRect.top));
        Timer? positionShouldBeAnimated = this._positionShouldBeAnimatedTimer;
        if (((this._magnifierPosition is not null) && (finalMagnifierPosition.dy != DartRuntimePrimitives.RequireValue(this._magnifierPosition).dy)))
        {
            if (((this._positionShouldBeAnimatedTimer is not null) && this._positionShouldBeAnimatedTimer!.isActive))
            {
                this._positionShouldBeAnimatedTimer!.cancel();
            }
            positionShouldBeAnimated = new Timer(TextMagnifier.jumpBetweenLinesAnimationDuration, (() =>
            {
                setState(((global::System.Action)(() =>
                {
                    _positionShouldBeAnimatedTimer = null;
                })));
            }));
        }
        setState(((global::System.Action)(() =>
        {
            _magnifierPosition = finalMagnifierPosition;
            _positionShouldBeAnimatedTimer = positionShouldBeAnimated;
            _extraFocalPointOffset = focalPointAdjustmentForScreenBoundsAdjustment;
        })));
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => (this._magnifierPosition is not null), () => (object?)"Magnifier position should only be null before the first build.");
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.AnimatedPositioned(top: DartRuntimePrimitives.RequireValue(this._magnifierPosition).dy, left: DartRuntimePrimitives.RequireValue(this._magnifierPosition).dx, duration: (this._positionShouldBeAnimated ? TextMagnifier.jumpBetweenLinesAnimationDuration : Duration.zero), child: new Magnifier(additionalFocalPointOffset: this._extraFocalPointOffset)));
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
        global::Doroti.Framework.Painting.BorderRadius __borderRadius = borderRadius ?? global::Doroti.Framework.Painting.BorderRadius.CreateAll(Radius.circular(_borderRadius));
        Color __filmColor = filmColor ?? Color.CreateFromARGB(8, 158, 158, 158);
        List<global::Doroti.Framework.Painting.BoxShadow> __shadows = shadows ?? new List<global::Doroti.Framework.Painting.BoxShadow> { new global::Doroti.Framework.Painting.BoxShadow(blurRadius: 1.5, offset: new Offset(0.0, 2.0), spreadRadius: 0.75, color: Color.fromARGB(25, 0, 0, 0)) };
        Size __size = size ?? Magnifier.kDefaultMagnifierSize;
        this.additionalFocalPointOffset = additionalFocalPointOffset;
        this.borderRadius = __borderRadius;
        this.filmColor = __filmColor;
        this.shadows = __shadows;
        this.clipBehavior = clipBehavior;
        this.size = __size;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.RawMagnifier(decoration: new global::Doroti.Framework.Widgets.MagnifierDecoration(shape: new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: this.borderRadius), shadows: this.shadows), clipBehavior: this.clipBehavior, magnificationScale: _magnification, focalPointOffset: (this.additionalFocalPointOffset + new global::Doroti.Ui.Offset(0, (kStandardVerticalFocalPointShift + (kDefaultMagnifierSize.height / 2L)))), size: DartRuntimePrimitives.RequireValue(this.size), child: new global::Doroti.Framework.Widgets.ColoredBox(color: this.filmColor)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
