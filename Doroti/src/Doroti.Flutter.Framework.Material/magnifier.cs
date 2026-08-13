// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/magnifier.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public class TextMagnifier : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public static global::Doroti.Generated.Framework.Widgets.TextMagnifierConfiguration adaptiveMagnifierConfiguration = new global::Doroti.Generated.Framework.Widgets.TextMagnifierConfiguration(shouldDisplayHandlesInMagnifier: (object.Equals(global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS)), magnifierBuilder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.MagnifierController, global::Doroti.Generated.Framework.Foundation.ValueNotifier<global::Doroti.Generated.Framework.Widgets.MagnifierInfo>, global::Doroti.Generated.Framework.Widgets.Widget?>?)((context, controller, magnifierInfo) => {
switch (global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
{
    case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
        {
            return ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)new CupertinoTextMagnifier(controller: controller, magnifierInfo: magnifierInfo));
        }
    case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
        {
            return ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)new TextMagnifier(magnifierInfo: magnifierInfo));
        }
    case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
    case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
    case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
    case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
        {
            return null;
        }
    default:
        throw new InvalidOperationException("Non-exhaustive Dart switch value.");
}
throw new InvalidOperationException("Dart closure completed without a value.");
})));
    public static Duration jumpBetweenLinesAnimationDuration = Duration.Create(milliseconds: 70L);
    public virtual global::Doroti.Generated.Framework.Foundation.ValueNotifier<global::Doroti.Generated.Framework.Widgets.MagnifierInfo> magnifierInfo { get; private set; } = default!;

    public TextMagnifier(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Foundation.ValueNotifier<global::Doroti.Generated.Framework.Widgets.MagnifierInfo> magnifierInfo = default!) : base(key: key)
    {
        this.magnifierInfo = magnifierInfo;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _TextMagnifierState__magnifier());
}

internal class _TextMagnifierState__magnifier : global::Doroti.Generated.Framework.Widgets.State<TextMagnifier>
{
    internal virtual Offset? _magnifierPosition { get; set; } = default;
    internal virtual Timer? _positionShouldBeAnimatedTimer { get; set; } = default;
    internal virtual Offset _extraFocalPointOffset { get; set; } = Offset.zero;

    internal virtual bool _positionShouldBeAnimated => DartRuntimePrimitives.ConvertValue<bool>((this._positionShouldBeAnimatedTimer is not null));
    public override void initState()
    {
        base.initState();
        ((TextMagnifier)this.widget).magnifierInfo.addListener(() => this._determineMagnifierPositionAndFocalPoint());
    }

    public override void dispose()
    {
        ((TextMagnifier)this.widget).magnifierInfo.removeListener(() => this._determineMagnifierPositionAndFocalPoint());
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
            ((TextMagnifier)oldWidget).magnifierInfo.removeListener(() => this._determineMagnifierPositionAndFocalPoint());
            ((TextMagnifier)this.widget).magnifierInfo.addListener(() => this._determineMagnifierPositionAndFocalPoint());
        }
        base.didUpdateWidget(oldWidget);
    }

    internal virtual void _determineMagnifierPositionAndFocalPoint()
    {
        global::Doroti.Generated.Framework.Widgets.MagnifierInfo selectionInfo__4826 = ((TextMagnifier)this.widget).magnifierInfo.value;
        global::Doroti.Flutter.Ui.Rect screenRect__4885 = ((global::Doroti.Flutter.Ui.Rect)(object?)(Offset.zero & MediaQuery.sizeOf(this.context)));
        var basicMagnifierOffset__5134 = new global::Doroti.Flutter.Ui.Offset((Magnifier.kDefaultMagnifierSize.width / 2L), (Magnifier.kDefaultMagnifierSize.height + Magnifier.kStandardVerticalFocalPointShift));
        double magnifierX__5527 = Dart_uiLibrary.clampDouble(((global::Doroti.Generated.Framework.Widgets.MagnifierInfo)selectionInfo__4826).globalGesturePosition.dx, ((global::Doroti.Generated.Framework.Widgets.MagnifierInfo)selectionInfo__4826).currentLineBoundaries.left, ((global::Doroti.Generated.Framework.Widgets.MagnifierInfo)selectionInfo__4826).currentLineBoundaries.right);
        global::Doroti.Flutter.Ui.Rect unadjustedMagnifierRect__5842 = ((global::Doroti.Flutter.Ui.Rect)(object?)((new global::Doroti.Flutter.Ui.Offset(magnifierX__5527, ((Offset)((dynamic)((global::Doroti.Generated.Framework.Widgets.MagnifierInfo)selectionInfo__4826).caretRect).center).dy) - basicMagnifierOffset__5134) & Magnifier.kDefaultMagnifierSize));
        global::Doroti.Flutter.Ui.Rect screenBoundsAdjustedMagnifierRect__6287 = ((global::Doroti.Flutter.Ui.Rect)(object?)MagnifierController.shiftWithinBounds(bounds: screenRect__4885, rect: unadjustedMagnifierRect__5842));
        global::Doroti.Flutter.Ui.Offset finalMagnifierPosition__6491 = ((global::Doroti.Flutter.Ui.Offset)(object?)screenBoundsAdjustedMagnifierRect__6287.topLeft);
        double horizontalMaxFocalPointEdgeInsets__6717 = (((Magnifier.kDefaultMagnifierSize.width / 2L)) / Magnifier._magnification);
        double newGlobalFocalPointX__6972 = default!;
        if ((((global::Doroti.Generated.Framework.Widgets.MagnifierInfo)selectionInfo__4826).fieldBounds.width < (horizontalMaxFocalPointEdgeInsets__6717 * 2L)))
        {
            newGlobalFocalPointX__6972 = ((Offset)((dynamic)((global::Doroti.Generated.Framework.Widgets.MagnifierInfo)selectionInfo__4826).fieldBounds).center).dx;
        }
        else
        {
            newGlobalFocalPointX__6972 = Dart_uiLibrary.clampDouble(((Offset)((dynamic)screenBoundsAdjustedMagnifierRect__6287).center).dx, (((global::Doroti.Generated.Framework.Widgets.MagnifierInfo)selectionInfo__4826).fieldBounds.left + horizontalMaxFocalPointEdgeInsets__6717), (((global::Doroti.Generated.Framework.Widgets.MagnifierInfo)selectionInfo__4826).fieldBounds.right - horizontalMaxFocalPointEdgeInsets__6717));
        }
        double newRelativeFocalPointX__7863 = (newGlobalFocalPointX__6972 - ((Offset)((dynamic)screenBoundsAdjustedMagnifierRect__6287).center).dx);
        var focalPointAdjustmentForScreenBoundsAdjustment__8407 = new global::Doroti.Flutter.Ui.Offset(newRelativeFocalPointX__7863, (unadjustedMagnifierRect__5842.top - screenBoundsAdjustedMagnifierRect__6287.top));
        Timer? positionShouldBeAnimated__8587 = this._positionShouldBeAnimatedTimer;
        if (((this._magnifierPosition is not null) && (finalMagnifierPosition__6491.dy != DartRuntimePrimitives.RequireValue(this._magnifierPosition).dy)))
        {
            if (((this._positionShouldBeAnimatedTimer is not null) && this._positionShouldBeAnimatedTimer!.isActive))
            {
                this._positionShouldBeAnimatedTimer!.cancel();
            }
            positionShouldBeAnimated__8587 = new Timer(TextMagnifier.jumpBetweenLinesAnimationDuration, (() => { setState(((global::System.Action)(() => {
_positionShouldBeAnimatedTimer = null;
}))); }));
        }
        setState(((global::System.Action)(() => {
_magnifierPosition = finalMagnifierPosition__6491;
_positionShouldBeAnimatedTimer = positionShouldBeAnimated__8587;
_extraFocalPointOffset = focalPointAdjustmentForScreenBoundsAdjustment__8407;
})));
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => (this._magnifierPosition is not null), () => (object?)"Magnifier position should only be null before the first build.");
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.AnimatedPositioned(top: DartRuntimePrimitives.RequireValue(this._magnifierPosition).dy, left: DartRuntimePrimitives.RequireValue(this._magnifierPosition).dx, duration: (this._positionShouldBeAnimated ? TextMagnifier.jumpBetweenLinesAnimationDuration : Duration.zero), child: new Magnifier(additionalFocalPointOffset: this._extraFocalPointOffset)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class Magnifier : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public static Size kDefaultMagnifierSize = new global::Doroti.Flutter.Ui.Size(77.37, 37.9);
    public const double kStandardVerticalFocalPointShift = 22.0;
    internal const double _borderRadius = 40;
    internal const double _magnification = 1.25;
    public virtual Offset additionalFocalPointOffset { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadius borderRadius { get; private set; } = default!;
    public virtual Color filmColor { get; private set; } = default!;
    public virtual List<global::Doroti.Generated.Framework.Painting.BoxShadow> shadows { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual Size size { get; private set; } = default!;

    public Magnifier(global::Doroti.Generated.Framework.Foundation.Key? key = null, Offset additionalFocalPointOffset = default, global::Doroti.Generated.Framework.Painting.BorderRadius borderRadius = default!, Color filmColor = default!, List<global::Doroti.Generated.Framework.Painting.BoxShadow> shadows = default!, Clip clipBehavior = Clip.hardEdge, Size? size = null) : base(key: key)
    {
        global::Doroti.Generated.Framework.Painting.BorderRadius __borderRadius = borderRadius ?? global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(Radius.circular(_borderRadius));
        Color __filmColor = filmColor ?? Color.CreateFromARGB(8, 158, 158, 158);
        List<global::Doroti.Generated.Framework.Painting.BoxShadow> __shadows = shadows ?? new List<global::Doroti.Generated.Framework.Painting.BoxShadow> { new global::Doroti.Generated.Framework.Painting.BoxShadow(blurRadius: 1.5, offset: new Offset(0.0, 2.0), spreadRadius: 0.75, color: Color.fromARGB(25, 0, 0, 0)) };
        Size __size = size ?? Magnifier.kDefaultMagnifierSize;
        this.additionalFocalPointOffset = additionalFocalPointOffset;
        this.borderRadius = __borderRadius;
        this.filmColor = __filmColor;
        this.shadows = __shadows;
        this.clipBehavior = clipBehavior;
        this.size = __size;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.RawMagnifier(decoration: new global::Doroti.Generated.Framework.Widgets.MagnifierDecoration(shape: new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: this.borderRadius), shadows: this.shadows), clipBehavior: this.clipBehavior, magnificationScale: _magnification, focalPointOffset: (this.additionalFocalPointOffset + new global::Doroti.Flutter.Ui.Offset(0, (kStandardVerticalFocalPointShift + (kDefaultMagnifierSize.height / 2L)))), size: DartRuntimePrimitives.RequireValue(this.size), child: new global::Doroti.Generated.Framework.Widgets.ColoredBox(color: this.filmColor)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
