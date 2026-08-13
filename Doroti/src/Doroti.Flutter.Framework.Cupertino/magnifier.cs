// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/cupertino/magnifier.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Cupertino;

public class CupertinoTextMagnifier : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Animation.Curve animationCurve { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.MagnifierController controller { get; private set; } = default!;
    public virtual double dragResistance { get; private set; } = default!;
    public virtual double hideBelowThreshold { get; private set; } = default!;
    public virtual double horizontalScreenEdgePadding { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueNotifier<global::Doroti.Generated.Framework.Widgets.MagnifierInfo> magnifierInfo { get; private set; } = default!;
    internal static Duration _kDragAnimationDuration = Duration.Create(milliseconds: 45L);

    public CupertinoTextMagnifier(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Animation.Curve animationCurve = default!, global::Doroti.Generated.Framework.Widgets.MagnifierController controller = default!, double dragResistance = 10.0, double hideBelowThreshold = 48.0, double horizontalScreenEdgePadding = 10.0, global::Doroti.Generated.Framework.Foundation.ValueNotifier<global::Doroti.Generated.Framework.Widgets.MagnifierInfo> magnifierInfo = default!) : base(key: key)
    {
        global::Doroti.Generated.Framework.Animation.Curve __animationCurve = animationCurve ?? global::Doroti.Generated.Framework.Animation.Curves.easeOut;
        this.animationCurve = __animationCurve;
        this.controller = controller;
        this.dragResistance = dragResistance;
        this.hideBelowThreshold = hideBelowThreshold;
        this.horizontalScreenEdgePadding = horizontalScreenEdgePadding;
        this.magnifierInfo = magnifierInfo;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoTextMagnifierState__magnifier());
}

internal class _CupertinoTextMagnifierState__magnifier : global::Doroti.Generated.Framework.Widgets.State<CupertinoTextMagnifier>, global::Doroti.Generated.Framework.Widgets.SingleTickerProviderStateMixin<CupertinoTextMagnifier>
{
    internal virtual Offset _currentAdjustedMagnifierPosition { get; set; } = Offset.zero;
    internal virtual double _verticalFocalPointAdjustment { get; set; } = 0;
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationController _ioAnimationController { get; private set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.Animation<double> _ioAnimation { get; private set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation _ioCurvedAnimation { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _ioAnimationController = ((Func<global::Doroti.Generated.Framework.Animation.AnimationController>)(() =>
{            var __cascade = new global::Doroti.Generated.Framework.Animation.AnimationController(value: 0, vsync: this, duration: CupertinoMagnifier._kInOutAnimationDuration);
            __cascade.addListener(((global::System.Action)(() => { setState(((global::System.Action)(() => {
}))); })));
            return __cascade;        }))();
        ((CupertinoTextMagnifier)this.widget).controller.animationController = this._ioAnimationController;
        ((CupertinoTextMagnifier)this.widget).magnifierInfo.addListener(() => this._determineMagnifierPositionAndFocalPoint());
        _ioCurvedAnimation = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: this._ioAnimationController, curve: ((CupertinoTextMagnifier)this.widget).animationCurve);
        _ioAnimation = new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 0.0, end: 1.0).animate(this._ioCurvedAnimation);
    }

    public override void dispose()
    {
        ((CupertinoTextMagnifier)this.widget).controller.animationController = null;
        this._ioAnimationController.dispose();
        this._ioCurvedAnimation.dispose();
        ((CupertinoTextMagnifier)this.widget).magnifierInfo.removeListener(() => this._determineMagnifierPositionAndFocalPoint());
        DartRuntimePrimitives.Assert(() =>
            {
                if (((this._ticker is null) || !this._ticker!.isActive))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), this._ticker!.describeForError("The offending ticker was") }));
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override void didUpdateWidget(CupertinoTextMagnifier oldWidget)
    {
        if ((!object.Equals(((CupertinoTextMagnifier)oldWidget).magnifierInfo, ((CupertinoTextMagnifier)this.widget).magnifierInfo)))
        {
            ((CupertinoTextMagnifier)oldWidget).magnifierInfo.removeListener(() => this._determineMagnifierPositionAndFocalPoint());
            ((CupertinoTextMagnifier)this.widget).magnifierInfo.addListener(() => this._determineMagnifierPositionAndFocalPoint());
        }
        base.didUpdateWidget(oldWidget);
    }

    public override void didChangeDependencies()
    {
        _determineMagnifierPositionAndFocalPoint();
        base.didChangeDependencies();
    }

    internal virtual void _determineMagnifierPositionAndFocalPoint()
    {
        global::Doroti.Generated.Framework.Widgets.MagnifierInfo textEditingContext__5306 = ((CupertinoTextMagnifier)this.widget).magnifierInfo.value;
        double verticalCenterOfCurrentLine__5427 = ((Offset)((dynamic)((global::Doroti.Generated.Framework.Widgets.MagnifierInfo)textEditingContext__5306).caretRect).center).dy;
        if (((verticalCenterOfCurrentLine__5427 - ((global::Doroti.Generated.Framework.Widgets.MagnifierInfo)textEditingContext__5306).globalGesturePosition.dy) < -((CupertinoTextMagnifier)this.widget).hideBelowThreshold))
        {
            if (((CupertinoTextMagnifier)this.widget).controller.shown)
            {
                DartRuntimePrimitives.Ignore(((CupertinoTextMagnifier)this.widget).controller.hide(removeFromOverlay: false));
            }
            return;
        }
        if (!((CupertinoTextMagnifier)this.widget).controller.shown)
        {
            this._ioAnimationController.forward();
        }
        double verticalPositionOfLens__6199 = Math.Max(verticalCenterOfCurrentLine__5427, (verticalCenterOfCurrentLine__5427 - (((verticalCenterOfCurrentLine__5427 - ((global::Doroti.Generated.Framework.Widgets.MagnifierInfo)textEditingContext__5306).globalGesturePosition.dy)) / ((CupertinoTextMagnifier)this.widget).dragResistance)));
        var rawMagnifierPosition__6504 = new global::Doroti.Flutter.Ui.Offset((((global::Doroti.Generated.Framework.Widgets.MagnifierInfo)textEditingContext__5306).globalGesturePosition.dx - (CupertinoMagnifier.kDefaultSize.width / 2L)), (verticalPositionOfLens__6199 - ((CupertinoMagnifier.kDefaultSize.height - CupertinoMagnifier.kMagnifierAboveFocalPoint))));
        global::Doroti.Flutter.Ui.Rect screenRect__6783 = ((global::Doroti.Flutter.Ui.Rect)(object?)(Offset.zero & MediaQuery.sizeOf(this.context)));
        global::Doroti.Flutter.Ui.Offset adjustedMagnifierPosition__6956 = ((global::Doroti.Flutter.Ui.Offset)(object?)MagnifierController.shiftWithinBounds(bounds: global::Doroti.Flutter.Ui.Rect.fromLTRB((screenRect__6783.left + ((CupertinoTextMagnifier)this.widget).horizontalScreenEdgePadding), (screenRect__6783.top - ((CupertinoMagnifier.kDefaultSize.height + CupertinoMagnifier.kMagnifierAboveFocalPoint))), (screenRect__6783.right - ((CupertinoTextMagnifier)this.widget).horizontalScreenEdgePadding), (screenRect__6783.bottom + ((CupertinoMagnifier.kDefaultSize.height + CupertinoMagnifier.kMagnifierAboveFocalPoint)))), rect: (rawMagnifierPosition__6504 & CupertinoMagnifier.kDefaultSize)).topLeft);
        setState(((global::System.Action)(() => {
_currentAdjustedMagnifierPosition = adjustedMagnifierPosition__6956;
_verticalFocalPointAdjustment = (verticalCenterOfCurrentLine__5427 - verticalPositionOfLens__6199);
})));
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        CupertinoThemeData themeData__8009 = CupertinoTheme.of(context);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.AnimatedPositioned(duration: CupertinoTextMagnifier._kDragAnimationDuration, curve: ((CupertinoTextMagnifier)this.widget).animationCurve, left: this._currentAdjustedMagnifierPosition.dx, top: this._currentAdjustedMagnifierPosition.dy, child: new CupertinoMagnifier(inOutAnimation: this._ioAnimation, additionalFocalPointOffset: new global::Doroti.Flutter.Ui.Offset(0, this._verticalFocalPointAdjustment), borderSide: new global::Doroti.Generated.Framework.Painting.BorderSide(color: themeData__8009.primaryColor, width: 2.0))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._ticker is null))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this.GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
            });
        this._ticker = new global::Doroti.Generated.Framework.Scheduler.Ticker((global::System.Action<Duration>)onTick, debugLabel: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
        _updateTickerModeNotifier();
        _updateTicker();
        return this._ticker!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTicker();
    }

    public virtual void _updateTicker()
    {
        TickerModeData values__15157 = this._tickerModeNotifier!.value;
        if ((this._ticker is not null))
        {
            this._ticker!.muted = !((TickerModeData)values__15157).enabled;
            this._ticker!.forceFrames = ((TickerModeData)values__15157).forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__15400 = ((global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__15400, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        newNotifier__15400.addListener(() => this._updateTicker());
        this._tickerModeNotifier = newNotifier__15400;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription__15805 = ((this._ticker?.isActive, this._ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) });
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Scheduler.Ticker>("ticker", this._ticker, description: tickerDescription__15805, showSeparator: false, defaultValue: default));
    }

}

public class CupertinoMagnifier : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual List<global::Doroti.Generated.Framework.Painting.BoxShadow> shadows { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BorderSide borderSide { get; private set; } = default!;
    public static double kMagnifierAboveFocalPoint = -26;
    public static Size kDefaultSize = new global::Doroti.Flutter.Ui.Size(80, 47.5);
    internal static Duration _kInOutAnimationDuration = Duration.Create(milliseconds: 150L);
    public virtual Size size { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadius borderRadius { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double>? inOutAnimation { get; private set; }
    public virtual Offset additionalFocalPointOffset { get; private set; } = default!;
    public virtual double magnificationScale { get; private set; } = default!;

    public CupertinoMagnifier(global::Doroti.Generated.Framework.Foundation.Key? key = null, Size? size = null, global::Doroti.Generated.Framework.Painting.BorderRadius borderRadius = default!, Offset additionalFocalPointOffset = default, List<global::Doroti.Generated.Framework.Painting.BoxShadow> shadows = default!, Clip clipBehavior = Clip.none, global::Doroti.Generated.Framework.Painting.BorderSide borderSide = default!, global::Doroti.Generated.Framework.Animation.Animation<double>? inOutAnimation = null, double magnificationScale = 1.0) : base(key: key)
    {
        Size __size = size ?? kDefaultSize;
        global::Doroti.Generated.Framework.Painting.BorderRadius __borderRadius = borderRadius ?? global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(Radius.elliptical(60, 50));
        List<global::Doroti.Generated.Framework.Painting.BoxShadow> __shadows = shadows ?? new List<global::Doroti.Generated.Framework.Painting.BoxShadow> { new global::Doroti.Generated.Framework.Painting.BoxShadow(color: Color.fromARGB(25, 0, 0, 0), blurRadius: 11, spreadRadius: 0.2, blurStyle: BlurStyle.outer) };
        global::Doroti.Generated.Framework.Painting.BorderSide __borderSide = borderSide ?? new global::Doroti.Generated.Framework.Painting.BorderSide(color: Color.fromARGB(255, 0, 124, 255), width: 2.0);
        this.size = __size;
        this.borderRadius = __borderRadius;
        this.additionalFocalPointOffset = additionalFocalPointOffset;
        this.shadows = __shadows;
        this.clipBehavior = clipBehavior;
        this.borderSide = __borderSide;
        this.inOutAnimation = inOutAnimation;
        this.magnificationScale = magnificationScale;
        System.Diagnostics.Debug.Assert((magnificationScale > 0L));
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        var focalPointOffset__13317 = new global::Doroti.Flutter.Ui.Offset(0, (((kDefaultSize.height / 2L)) - kMagnifierAboveFocalPoint));
        focalPointOffset__13317.scale(1, (this.inOutAnimation?.value ?? 1));
        focalPointOffset__13317 += this.additionalFocalPointOffset;
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)global::Doroti.Generated.Framework.Widgets.Transform.CreateTranslate(offset: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(new global::Doroti.Flutter.Ui.Offset(0, -kMagnifierAboveFocalPoint), Offset.zero, (this.inOutAnimation?.value ?? 1))), child: new global::Doroti.Generated.Framework.Widgets.RawMagnifier(size: DartRuntimePrimitives.RequireValue(this.size), focalPointOffset: focalPointOffset__13317, decoration: new global::Doroti.Generated.Framework.Widgets.MagnifierDecoration(opacity: (this.inOutAnimation?.value ?? 1), shape: new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: this.borderRadius, side: this.borderSide), shadows: this.shadows), clipBehavior: this.clipBehavior, magnificationScale: this.magnificationScale)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
