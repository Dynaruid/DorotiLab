// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/dialog.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class DialogLibrary
{
    internal static global::Doroti.Framework.Painting.TextStyle _kCupertinoDialogTitleStyle = new global::Doroti.Framework.Painting.TextStyle(fontFamily: "CupertinoSystemText", inherit: false, fontSize: 17.0, fontWeight: FontWeight.w600, height: 1.3, letterSpacing: -0.5, textBaseline: TextBaseline.alphabetic);
}

public static partial class DialogLibrary
{
    internal static global::Doroti.Framework.Painting.TextStyle _kCupertinoDialogContentStyle = new global::Doroti.Framework.Painting.TextStyle(fontFamily: "CupertinoSystemText", inherit: false, fontSize: 13.0, fontWeight: FontWeight.w400, height: 1.35, letterSpacing: -0.2, textBaseline: TextBaseline.alphabetic);
}

public static partial class DialogLibrary
{
    internal static global::Doroti.Framework.Painting.TextStyle _kCupertinoDialogActionStyle = new global::Doroti.Framework.Painting.TextStyle(fontFamily: "CupertinoSystemText", inherit: false, fontSize: 16.8, fontWeight: FontWeight.w400, textBaseline: TextBaseline.alphabetic);
}

public static partial class DialogLibrary
{
    internal static global::Doroti.Framework.Painting.TextStyle _kActionSheetActionStyle = new global::Doroti.Framework.Painting.TextStyle(fontFamily: "CupertinoSystemDisplay", inherit: false, fontSize: 17.0, fontWeight: FontWeight.w400, textBaseline: TextBaseline.alphabetic);
}

public static partial class DialogLibrary
{
    internal static global::Doroti.Framework.Painting.TextStyle _kActionSheetContentStyle = new global::Doroti.Framework.Painting.TextStyle(fontFamily: "CupertinoSystemText", inherit: false, fontSize: 13.0, fontWeight: FontWeight.w400, textBaseline: TextBaseline.alphabetic);
}

public static partial class DialogLibrary
{
    internal static double _kCornerRadius = 14.0;
}

public static partial class DialogLibrary
{
    internal static double _kDividerThickness = 0.3;
}

public static partial class DialogLibrary
{
    internal static double _kCupertinoDialogWidth = 270.0;
}

public static partial class DialogLibrary
{
    internal static double _kAccessibilityCupertinoDialogWidth = 310.0;
}

public static partial class DialogLibrary
{
    internal static double _kDialogEdgePadding = 20.0;
}

public static partial class DialogLibrary
{
    internal static double _kDialogMinButtonHeight = 45.0;
}

public static partial class DialogLibrary
{
    internal static double _kDialogMinButtonFontSize = 10.0;
}

public static partial class DialogLibrary
{
    internal static double _kDialogActionsSectionMinHeight = 67.8;
}

public static partial class DialogLibrary
{
    internal static double _kActionSheetEdgePadding = 8.0;
}

public static partial class DialogLibrary
{
    internal static double _kActionSheetCancelButtonPadding = 8.0;
}

public static partial class DialogLibrary
{
    internal static double _kActionSheetContentHorizontalPadding = 16.0;
}

public static partial class DialogLibrary
{
    internal static double _kActionSheetContentVerticalPadding = 13.5;
}

public static partial class DialogLibrary
{
    internal static double _kActionSheetActionsSectionMinHeight = 84.0;
}

public static partial class DialogLibrary
{
    internal static double _kActionSheetButtonHorizontalPadding = 10.0;
}

public static partial class DialogLibrary
{
    internal static double _kActionSheetButtonMinHeight = 57.17;
}

public static partial class DialogLibrary
{
    internal static double _kActionSheetButtonVerticalPaddingFactor = 0.4;
}

public static partial class DialogLibrary
{
    internal static double _kActionSheetButtonVerticalPaddingBase = 1.8;
}

public static partial class DialogLibrary
{
    internal static Color _kDialogColor = new CupertinoDynamicColor(color: new global::Doroti.Ui.Color(3438473970L), darkColor: new global::Doroti.Ui.Color(3425512749L));
}

public static partial class DialogLibrary
{
    internal static Color _kDialogPressedColor = new CupertinoDynamicColor(color: new global::Doroti.Ui.Color(4292993505L), darkColor: new global::Doroti.Ui.Color(4282400832L));
}

public static partial class DialogLibrary
{
    internal static Color _kActionSheetPressedColor = new CupertinoDynamicColor(color: new global::Doroti.Ui.Color(3403735264L), darkColor: new global::Doroti.Ui.Color(3243331921L));
}

public static partial class DialogLibrary
{
    internal static Color _kActionSheetCancelColor = new CupertinoDynamicColor(color: new global::Doroti.Ui.Color(4294967295L), darkColor: new global::Doroti.Ui.Color(4281084972L));
}

public static partial class DialogLibrary
{
    internal static Color _kActionSheetCancelPressedColor = new CupertinoDynamicColor(color: new global::Doroti.Ui.Color(4293717228L), darkColor: new global::Doroti.Ui.Color(4282992969L));
}

public static partial class DialogLibrary
{
    internal static Color _kActionSheetBackgroundColor = new CupertinoDynamicColor(color: new global::Doroti.Ui.Color(3372023036L), darkColor: new global::Doroti.Ui.Color(3190368553L));
}

public static partial class DialogLibrary
{
    internal static Color _kActionSheetContentTextColor = new CupertinoDynamicColor(color: new global::Doroti.Ui.Color(2233277725L), darkColor: new global::Doroti.Ui.Color(2532438513L));
}

public static partial class DialogLibrary
{
    internal static Color _kActionSheetButtonDividerColor = new CupertinoDynamicColor(color: new global::Doroti.Ui.Color(3569994185L), darkColor: new global::Doroti.Ui.Color(3581771133L));
}

public static partial class DialogLibrary
{
    internal static double _kMaxRegularTextScaleFactor = 1.4;
}

public static partial class DialogLibrary
{
    internal static bool _isInAccessibilityMode(global::Doroti.Framework.Widgets.BuildContext context)
    {
        var defaultFontSize = 14.0;
        double? scaledFontSize = MediaQuery.maybeTextScalerOf(context)?.scale(defaultFontSize);
        return (scaledFontSize is not null) && (DartRuntimePrimitives.RequireValue(scaledFontSize) > (defaultFontSize * _kMaxRegularTextScaleFactor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class CupertinoAlertDialog : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget? title { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? content { get; private set; }
    public virtual List<global::Doroti.Framework.Widgets.Widget> actions { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.ScrollController? scrollController { get; private set; }
    public virtual global::Doroti.Framework.Widgets.ScrollController? actionScrollController { get; private set; }
    public virtual Duration insetAnimationDuration { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Curve insetAnimationCurve { get; private set; } = default!;

    public CupertinoAlertDialog(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget? title = null, global::Doroti.Framework.Widgets.Widget? content = null, List<global::Doroti.Framework.Widgets.Widget> actions = default!, global::Doroti.Framework.Widgets.ScrollController? scrollController = null, global::Doroti.Framework.Widgets.ScrollController? actionScrollController = null, Duration? insetAnimationDuration = null, global::Doroti.Framework.Animation.Curve insetAnimationCurve = default!) : base(key: key)
    {
        List<global::Doroti.Framework.Widgets.Widget> __actions = actions ?? new List<global::Doroti.Framework.Widgets.Widget>();
        Duration __insetAnimationDuration = insetAnimationDuration ?? Duration.Create(milliseconds: 100);
        global::Doroti.Framework.Animation.Curve __insetAnimationCurve = insetAnimationCurve ?? Curves.decelerate;
        this.title = title;
        this.content = content;
        this.actions = __actions;
        this.scrollController = scrollController;
        this.actionScrollController = actionScrollController;
        this.insetAnimationDuration = __insetAnimationDuration;
        this.insetAnimationCurve = __insetAnimationCurve;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoAlertDialogState__dialog());
}

internal class _CupertinoAlertDialogState__dialog : global::Doroti.Framework.Widgets.State<CupertinoAlertDialog>
{
    internal virtual long? _pressedIndex { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.ScrollController? _backupScrollController { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.ScrollController? _backupActionScrollController { get; set; } = default;

    internal virtual global::Doroti.Framework.Widgets.ScrollController _effectiveScrollController => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.ScrollController>(widget.scrollController ?? (_backupScrollController ??= new global::Doroti.Framework.Widgets.ScrollController()));
    internal virtual global::Doroti.Framework.Widgets.ScrollController _effectiveActionScrollController => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.ScrollController>(widget.actionScrollController ?? (_backupActionScrollController ??= new global::Doroti.Framework.Widgets.ScrollController()));
    internal virtual global::Doroti.Framework.Widgets.Widget? _buildContent(global::Doroti.Framework.Widgets.BuildContext context)
    {
        bool hasContent = (widget.title is not null) || (widget.content is not null);
        if (!hasContent)
        {
            return null;
        }
        var defaultFontSize = 14.0;
        double effectiveTextScaleFactor = MediaQuery.textScalerOf(context).scale(defaultFontSize) / defaultFontSize;
        global::Doroti.Framework.Widgets.Widget childLocal = new _CupertinoAlertContentSection__dialog(title: widget.title, message: widget.content, scrollController: _effectiveScrollController, titlePadding: EdgeInsets.CreateOnly(left: DialogLibrary._kDialogEdgePadding, right: DialogLibrary._kDialogEdgePadding, bottom: (widget.content is null) ? DialogLibrary._kDialogEdgePadding : 1.0, top: DialogLibrary._kDialogEdgePadding * effectiveTextScaleFactor), messagePadding: EdgeInsets.CreateOnly(left: DialogLibrary._kDialogEdgePadding, right: DialogLibrary._kDialogEdgePadding, bottom: DialogLibrary._kDialogEdgePadding * effectiveTextScaleFactor, top: (widget.title is null) ? DialogLibrary._kDialogEdgePadding : 1.0), titleTextStyle: DialogLibrary._kCupertinoDialogTitleStyle.copyWith(color: CupertinoDynamicColor.resolve(CupertinoColors.label, context)), messageTextStyle: DialogLibrary._kCupertinoDialogContentStyle.copyWith(color: CupertinoDynamicColor.resolve(CupertinoColors.label, context)));
        return (global::Doroti.Framework.Widgets.Widget?)new global::Doroti.Framework.Widgets.ColoredBox(color: CupertinoDynamicColor.resolve(DialogLibrary._kDialogColor, context), child: childLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _onPressedUpdate(long actionIndex, bool isPressed)
    {
        if (isPressed)
        {
            setState(() =>
            {
                _pressedIndex = actionIndex;
            });
        }
        else
        {
            if (_pressedIndex == actionIndex)
            {
                setState(() =>
                {
                    _pressedIndex = null;
                });
            }
        }
    }

    internal virtual global::Doroti.Framework.Widgets.Widget? _buildActions()
    {
        if (!Enumerable.Any(widget.actions))
        {
            return null;
        }
        else
        {
            return (global::Doroti.Framework.Widgets.Widget?)new _CupertinoAlertActionSection__dialog(scrollController: _effectiveActionScrollController, actions: widget.actions, pressedIndex: _pressedIndex, onPressedUpdate: _onPressedUpdate);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildBody(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color backgroundColor = CupertinoDynamicColor.resolve(DialogLibrary._kDialogColor, context);
        global::Doroti.Ui.Color dividerColorLocal = CupertinoDynamicColor.resolve(CupertinoColors.separator, context);
        return MediaQuery.CreateRemovePadding(removeLeft: true, removeTop: true, removeRight: true, removeBottom: true, context: context, child: new global::Doroti.Framework.Widgets.LayoutBuilder(builder: (context, constraints) =>
        {
            global::Doroti.Framework.Widgets.Widget? contentSection = _buildContent(context);
            global::Doroti.Framework.Widgets.Widget? actionsSection = _buildActions();
            if (actionsSection is null)
            {
                return contentSection ?? new global::Doroti.Framework.Widgets.LimitedBox(maxWidth: 0, child: new global::Doroti.Framework.Widgets.SizedBox(width: double.PositiveInfinity, height: 0));
            }
            global::Doroti.Framework.Widgets.Widget scrolledActionsSection = new _OverscrollBackground__dialog(color: backgroundColor, child: actionsSection);
            if (contentSection is null)
            {
                return scrolledActionsSection;
            }
            double actionsMinHeight = DialogLibrary._isInAccessibilityMode(context) ? ((constraints.maxHeight / 2L) + DialogLibrary._kDividerThickness) : (DialogLibrary._kDialogActionsSectionMinHeight + DialogLibrary._kDividerThickness);
            return new _PriorityColumn__dialog(top: contentSection, bottom: new global::Doroti.Framework.Widgets.Column(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(width: double.PositiveInfinity, child: new _Divider__dialog(dividerColor: dividerColorLocal, hiddenColor: backgroundColor, hidden: false))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: scrolledActionsSection)) }), bottomMinHeight: actionsMinHeight);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        CupertinoLocalizations localizations = CupertinoLocalizations.of(context);
        bool isInAccessibilityMode = DialogLibrary._isInAccessibilityMode(context);
        return new CupertinoUserInterfaceLevel(data: CupertinoUserInterfaceLevelData.elevated, child: MediaQuery.withClampedTextScaling(minScaleFactor: 1.0, child: new global::Doroti.Framework.Widgets.ScrollConfiguration(behavior: ScrollConfiguration.of(context).copyWith(scrollbars: false), child: new global::Doroti.Framework.Widgets.LayoutBuilder(builder: (context, constraints) =>
        {
            return new global::Doroti.Framework.Widgets.AnimatedPadding(padding: MediaQuery.viewInsetsOf(context).op_Add(EdgeInsets.CreateSymmetric(horizontal: 40.0, vertical: 24.0)), duration: widget.insetAnimationDuration, curve: widget.insetAnimationCurve, child: MediaQuery.CreateRemoveViewInsets(removeLeft: true, removeTop: true, removeRight: true, removeBottom: true, context: context, child: new global::Doroti.Framework.Widgets.Center(child: new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsets.CreateSymmetric(vertical: DialogLibrary._kDialogEdgePadding), child: new global::Doroti.Framework.Widgets.SizedBox(width: isInAccessibilityMode ? DialogLibrary._kAccessibilityCupertinoDialogWidth : DialogLibrary._kCupertinoDialogWidth, child: new _ActionSheetGestureDetector__dialog(child: new CupertinoPopupSurface(isSurfacePainted: false, child: new global::Doroti.Framework.Widgets.Semantics(role: SemanticsRole.alertDialog, namesRoute: true, scopesRoute: true, explicitChildNodes: true, label: localizations.alertDialogLabel, child: _buildBody(context)))))))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        _backupScrollController?.dispose();
        _backupActionScrollController?.dispose();
        base.dispose();
    }

}

public class CupertinoPopupSurface : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual double blurSigma { get; private set; } = default!;
    public virtual bool isSurfacePainted { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    public const double defaultBlurSigma = 30.0;
    internal static global::Doroti.Framework.Painting.BorderRadius _clipper = BorderRadius.CreateAll(Radius.circular(13));
    internal static List<double> _lightSaturationMatrix = new List<double> { 1.74, -0.4, -0.17, 0.0, 0.0, -0.26, 1.6, -0.17, 0.0, 0.0, -0.26, -0.4, 1.83, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0 };
    internal static List<double> _darkSaturationMatrix = new List<double> { 1.39, -0.56, -0.11, 0.0, 0.3, -0.32, 1.14, -0.11, 0.0, 0.3, -0.32, -0.56, 1.59, 0.0, 0.3, 0.0, 0.0, 0.0, 1.0, 0.0 };
    public static bool debugIsVibrancePainted = true;

    public CupertinoPopupSurface(global::Doroti.Framework.Foundation.Key? key = null, double? blurSigma = null, bool isSurfacePainted = true, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key)
    {
        double __blurSigma = blurSigma ?? defaultBlurSigma;
        this.blurSigma = __blurSigma;
        this.isSurfacePainted = isSurfacePainted;
        this.child = child;
        System.Diagnostics.Debug.Assert(__blurSigma >= 0L);
    }

    internal virtual global::Doroti.Framework.Rendering.ImageFilterConfig? _buildFilter(Brightness? brightness)
    {
        var isVibrancePainted = true;
        DartRuntimePrimitives.Assert(() =>
            {
                isVibrancePainted = debugIsVibrancePainted;
                return true;
            });
        if (!isVibrancePainted)
        {
            if (blurSigma == 0L)
            {
                return null;
            }
            return ImageFilterConfig.CreateBlur(sigmaX: DartRuntimePrimitives.RequireValue(blurSigma), sigmaY: DartRuntimePrimitives.RequireValue(blurSigma));
        }
        var colorFilter = ImageFilterConfig.Create(brightness switch { Brightness.dark => ColorFilter.matrix(_darkSaturationMatrix), Brightness.light => ColorFilter.matrix(_lightSaturationMatrix), null => ColorFilter.matrix(_lightSaturationMatrix), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        if (blurSigma == 0L)
        {
            return colorFilter;
        }
        return ImageFilterConfig.CreateCompose(inner: colorFilter, outer: ImageFilterConfig.CreateBlur(sigmaX: DartRuntimePrimitives.RequireValue(blurSigma), sigmaY: DartRuntimePrimitives.RequireValue(blurSigma)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Rendering.ImageFilterConfig? filter = _buildFilter(CupertinoTheme.maybeBrightnessOf(context));
        global::Doroti.Framework.Widgets.Widget contents = child;
        if (isSurfacePainted)
        {
            contents = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ColoredBox(color: CupertinoDynamicColor.resolve(DialogLibrary._kDialogColor, context), child: contents));
        }
        if (filter is not null)
        {
            return new global::Doroti.Framework.Widgets.ClipRSuperellipse(borderRadius: _clipper, child: new global::Doroti.Framework.Widgets.BackdropFilter(filterConfig: filter, child: contents));
        }
        return new global::Doroti.Framework.Widgets.ClipRSuperellipse(borderRadius: _clipper, child: contents);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal delegate global::Doroti.Framework.Gestures.HitTestResult _HitTester__dialog(Offset location);

internal class _SlidingTapGestureRecognizer__dialog : global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer
{
    public virtual global::System.Action<Offset>? onResponsiveUpdate { get; set; } = default;
    public virtual global::System.Action<Offset>? onResponsiveEnd { get; set; } = default;
    internal virtual long? _primaryPointer { get; set; } = default;

    internal _SlidingTapGestureRecognizer__dialog(object? debugOwner = null) : base(debugOwner: debugOwner)
    {
        dragStartBehavior = Gestures.DragStartBehavior.down;
    }

    public override void addAllowedPointer(global::Doroti.Framework.Gestures.PointerDownEvent @event)
    {
        _primaryPointer ??= @event.pointer;
        base.addAllowedPointer(@event);
    }

    public override void rejectGesture(long pointer)
    {
        if (pointer == _primaryPointer)
        {
            _primaryPointer = null;
        }
        base.rejectGesture(pointer);
    }

    public override void handleEvent(global::Doroti.Framework.Gestures.PointerEvent @event)
    {
        if (@event.pointer == _primaryPointer)
        {
            if (@event is global::Doroti.Framework.Gestures.PointerMoveEvent)
            {
                global::Doroti.Framework.Gestures.PointerMoveEvent @event__as28218 = (global::Doroti.Framework.Gestures.PointerMoveEvent)@event;
                onResponsiveUpdate?.Invoke(@event__as28218.position);
            }
            if (@event is global::Doroti.Framework.Gestures.PointerUpEvent)
            {
                global::Doroti.Framework.Gestures.PointerUpEvent @event__as29359 = (global::Doroti.Framework.Gestures.PointerUpEvent)@event;
                stopTrackingPointer(DartRuntimePrimitives.RequireValue(_primaryPointer));
                onResponsiveEnd?.Invoke(@event__as29359.position);
                _primaryPointer = null;
                return;
            }
            if (@event is global::Doroti.Framework.Gestures.PointerCancelEvent)
            {
                global::Doroti.Framework.Gestures.PointerCancelEvent @event__as29658 = (global::Doroti.Framework.Gestures.PointerCancelEvent)@event;
                _primaryPointer = null;
            }
        }
        base.handleEvent(@event);
    }

    public override string debugDescription => "tap slide";
}

internal interface _SlideTarget__dialog
{
    public bool didEnter(bool fromPointerDown, bool innerEnabled);
    public void didLeave();
    public void didConfirm();
}

internal class _TargetSelectionGestureRecognizer__dialog : global::Doroti.Framework.Gestures.GestureRecognizer
{
    public virtual global::System.Func<Offset, global::Doroti.Framework.Gestures.HitTestResult> hitTest { get; private set; } = default!;
    internal virtual List<_SlideTarget__dialog> _currentTargets { get; private set; } = new List<_SlideTarget__dialog>();
    internal virtual _SlidingTapGestureRecognizer__dialog _slidingTap { get; private set; } = default!;

    internal _TargetSelectionGestureRecognizer__dialog(object? debugOwner = null, global::System.Func<Offset, global::Doroti.Framework.Gestures.HitTestResult> hitTest = default!) : base(debugOwner: debugOwner)
    {
        this.hitTest = hitTest;
        _slidingTap = new _SlidingTapGestureRecognizer__dialog(debugOwner: debugOwner);
        DartRuntimePrimitives.Ignore(((Func<_SlidingTapGestureRecognizer__dialog>)(() =>
{
    var __cascade = _slidingTap;
    __cascade.onDown = _onDown;
    __cascade.onResponsiveUpdate = _onUpdate;
    __cascade.onResponsiveEnd = _onEnd;
    __cascade.onCancel = _onCancel;
    return __cascade;
}))());
    }

    public override void acceptGesture(long pointer)
    {
        _slidingTap.acceptGesture(pointer);
    }

    public override void rejectGesture(long pointer)
    {
        _slidingTap.rejectGesture(pointer);
    }

    public override void addPointer(global::Doroti.Framework.Gestures.PointerDownEvent @event)
    {
        _slidingTap.addPointer(@event);
    }

    public override void addPointerPanZoom(global::Doroti.Framework.Gestures.PointerPanZoomStartEvent @event)
    {
        _slidingTap.addPointerPanZoom(@event);
    }

    public override void dispose()
    {
        _slidingTap.dispose();
        base.dispose();
    }

    internal virtual void _updateDrag(Offset pointerPosition, bool fromPointerDown)
    {
        global::Doroti.Framework.Gestures.HitTestResult result = hitTest(pointerPosition);
        var foundTargets = new List<_SlideTarget__dialog>();
        foreach (global::Doroti.Framework.Gestures.HitTestEntry<global::Doroti.Framework.Gestures.HitTestTarget> entry in result.path)
        {
            if (entry.target is global::Doroti.Framework.Rendering.RenderMetaData targetLocal)
            {
                if (targetLocal.metaData is _SlideTarget__dialog)
                {
                    foundTargets.Add(((_SlideTarget__dialog?)targetLocal.metaData)!);
                }
            }
        }
        if (!Equals(_currentTargets.FirstOrDefault(), foundTargets.FirstOrDefault()))
        {
            foreach (_SlideTarget__dialog targetAlternate in _currentTargets)
            {
                targetAlternate.didLeave();
            }
            DartRuntimePrimitives.Ignore(((Func<List<_SlideTarget__dialog>>)(() =>
{
    var __cascade = _currentTargets;
    __cascade.Clear();
    __cascade.AddRange(foundTargets.Cast<_SlideTarget__dialog>());
    return __cascade;
}))());
            var enabled = true;
            foreach (_SlideTarget__dialog targetNested in _currentTargets)
            {
                enabled = targetNested.didEnter(fromPointerDown: fromPointerDown, innerEnabled: enabled);
            }
        }
    }

    internal virtual void _onDown(global::Doroti.Framework.Gestures.DragDownDetails details)
    {
        _updateDrag(details.globalPosition, fromPointerDown: true);
    }

    internal virtual void _onUpdate(Offset globalPosition)
    {
        _updateDrag(globalPosition, fromPointerDown: false);
    }

    internal virtual void _onEnd(Offset globalPosition)
    {
        _updateDrag(globalPosition, fromPointerDown: false);
        foreach (_SlideTarget__dialog target in _currentTargets)
        {
            target.didConfirm();
        }
        _currentTargets.Clear();
    }

    internal virtual void _onCancel()
    {
        foreach (_SlideTarget__dialog target in _currentTargets)
        {
            target.didLeave();
        }
        _currentTargets.Clear();
    }

    public override string debugDescription => "target selection";
}

internal class _ActionSheetGestureDetector__dialog : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }

    internal _ActionSheetGestureDetector__dialog(global::Doroti.Framework.Widgets.Widget? child = null)
    {
        this.child = child;
    }

    internal virtual global::Doroti.Framework.Gestures.HitTestResult _hitTest(global::Doroti.Framework.Widgets.BuildContext context, Offset globalPosition)
    {
        long viewIdLocal = checked((long)View.of(context).viewId);
        var result = new global::Doroti.Framework.Gestures.HitTestResult();
        WidgetsBinding.instance.hitTestInView(result, globalPosition, viewIdLocal);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        var gesturesLocal = new DartMap<Type, dynamic>();
        gesturesLocal[typeof(_TargetSelectionGestureRecognizer__dialog)] = new global::Doroti.Framework.Widgets.GestureRecognizerFactoryWithHandlers<_TargetSelectionGestureRecognizer__dialog>(() => new _TargetSelectionGestureRecognizer__dialog(debugOwner: this, hitTest: (globalPosition) => _hitTest(context, globalPosition)), (instance) =>
        {
        });
        return new global::Doroti.Framework.Widgets.RawGestureDetector(excludeFromSemantics: true, gestures: gesturesLocal, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoActionSheet : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget? title { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? message { get; private set; }
    public virtual List<global::Doroti.Framework.Widgets.Widget>? actions { get; private set; }
    public virtual global::Doroti.Framework.Widgets.ScrollController? messageScrollController { get; private set; }
    public virtual global::Doroti.Framework.Widgets.ScrollController? actionScrollController { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? cancelButton { get; private set; }

    public CupertinoActionSheet(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget? title = null, global::Doroti.Framework.Widgets.Widget? message = null, List<global::Doroti.Framework.Widgets.Widget>? actions = null, global::Doroti.Framework.Widgets.ScrollController? messageScrollController = null, global::Doroti.Framework.Widgets.ScrollController? actionScrollController = null, global::Doroti.Framework.Widgets.Widget? cancelButton = null) : base(key: key)
    {
        this.title = title;
        this.message = message;
        this.actions = actions;
        this.messageScrollController = messageScrollController;
        this.actionScrollController = actionScrollController;
        this.cancelButton = cancelButton;
        System.Diagnostics.Debug.Assert((actions is not null) || (title is not null) || (message is not null) || (cancelButton is not null));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoActionSheetState__dialog());
}

internal class _CupertinoActionSheetState__dialog : global::Doroti.Framework.Widgets.State<CupertinoActionSheet>
{
    internal virtual long? _pressedIndex { get; set; } = default;
    internal static long _kCancelButtonIndex = -1L;
    internal virtual global::Doroti.Framework.Widgets.ScrollController? _backupMessageScrollController { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.ScrollController? _backupActionScrollController { get; set; } = default;

    internal virtual global::Doroti.Framework.Widgets.ScrollController _effectiveMessageScrollController => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.ScrollController>(widget.messageScrollController ?? (_backupMessageScrollController ??= new global::Doroti.Framework.Widgets.ScrollController()));
    internal virtual global::Doroti.Framework.Widgets.ScrollController _effectiveActionScrollController => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.ScrollController>(widget.actionScrollController ?? (_backupActionScrollController ??= new global::Doroti.Framework.Widgets.ScrollController()));
    public override void dispose()
    {
        _backupMessageScrollController?.dispose();
        _backupActionScrollController?.dispose();
        base.dispose();
    }

    public virtual bool hasContent => DartRuntimePrimitives.ConvertValue<bool>((widget.title is not null) || (widget.message is not null));
    internal virtual global::Doroti.Framework.Widgets.Widget? _buildContent(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if (!hasContent)
        {
            return null;
        }
        global::Doroti.Framework.Painting.TextStyle textStyle = DialogLibrary._kActionSheetContentStyle.copyWith(color: CupertinoDynamicColor.resolve(DialogLibrary._kActionSheetContentTextColor, context));
        return (global::Doroti.Framework.Widgets.Widget?)new global::Doroti.Framework.Widgets.ColoredBox(color: CupertinoDynamicColor.resolve(DialogLibrary._kActionSheetBackgroundColor, context), child: new _CupertinoAlertContentSection__dialog(title: widget.title, message: widget.message, scrollController: _effectiveMessageScrollController, titlePadding: EdgeInsets.CreateOnly(left: DialogLibrary._kActionSheetContentHorizontalPadding, right: DialogLibrary._kActionSheetContentHorizontalPadding, bottom: (widget.message is null) ? DialogLibrary._kActionSheetContentVerticalPadding : 0.0, top: DialogLibrary._kActionSheetContentVerticalPadding), messagePadding: EdgeInsets.CreateOnly(left: DialogLibrary._kActionSheetContentHorizontalPadding, right: DialogLibrary._kActionSheetContentHorizontalPadding, bottom: DialogLibrary._kActionSheetContentVerticalPadding, top: (widget.title is null) ? DialogLibrary._kActionSheetContentVerticalPadding : 0.0), titleTextStyle: (widget.message is null) ? textStyle : textStyle.copyWith(fontWeight: FontWeight.w600), messageTextStyle: (widget.title is null) ? textStyle.copyWith(fontWeight: FontWeight.w600) : textStyle, additionalPaddingBetweenTitleAndMessage: EdgeInsets.CreateOnly(top: 4.0)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _onPressedUpdate(long actionIndex, bool state)
    {
        if (!state)
        {
            if (_pressedIndex == actionIndex)
            {
                setState(() =>
                {
                    _pressedIndex = null;
                });
            }
        }
        else
        {
            setState(() =>
            {
                _pressedIndex = actionIndex;
            });
        }
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildCancelButton()
    {
        DartRuntimePrimitives.Assert(() => widget.cancelButton is not null);
        double cancelPadding = ((widget.actions is not null) || (widget.message is not null) || (widget.title is not null)) ? DialogLibrary._kActionSheetCancelButtonPadding : 0.0;
        return new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsets.CreateOnly(top: cancelPadding), child: CupertinoFocusHalo.CreateWithRRect(borderRadius: ConstantsLibrary.kCupertinoButtonSizeBorderRadius.GetValueOrDefault(CupertinoButtonSize.large)!, child: new _ActionSheetButtonBackground__dialog(isCancel: true, pressed: _pressedIndex == _kCancelButtonIndex, onPressStateChange: (state) =>
        {
            _onPressedUpdate(_kCancelButtonIndex, state);
        }, child: widget.cancelButton!)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _lerp(double x, double x1, double y1, double x2, double y2)
    {
        if (x <= x1)
        {
            return y1;
        }
        else
        {
            if (x >= x2)
            {
                return y2;
            }
            else
            {
                return DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(y1, y2, (x - x1) / (x2 - x1)));
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _topPadding(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if (Equals(MediaQuery.orientationOf(context), Orientation.landscape))
        {
            return DialogLibrary._kActionSheetEdgePadding;
        }
        var viewPaddingData1 = 47.0;
        var paddingRatioData1 = 1.0;
        var viewPaddingData2 = 59.0;
        double paddingRatioData2 = 54.0 / 59.0;
        double currentViewPadding = MediaQuery.viewPaddingOf(context).top;
        double currentPaddingRatio = _lerp(currentViewPadding, viewPaddingData1, paddingRatioData1, viewPaddingData2, paddingRatioData2);
        double padding = (currentPaddingRatio * currentViewPadding).roundToDouble();
        return Math.Max(padding, DialogLibrary._kDialogEdgePadding);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        var childrenLocal = ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection47257 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection47257.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: new global::Doroti.Framework.Widgets.ClipRSuperellipse(borderRadius: BorderRadius.CreateAll(Radius.circular(12.0)), child: new global::Doroti.Framework.Widgets.BackdropFilter(filter: new global::Doroti.Ui.ImageFilter(sigmaX: CupertinoPopupSurface.defaultBlurSigma, sigmaY: CupertinoPopupSurface.defaultBlurSigma), child: new _ActionSheetMainSheet__dialog(pressedIndex: _pressedIndex, onPressedUpdate: _onPressedUpdate, scrollController: _effectiveActionScrollController, contentSection: _buildContent(context), actions: widget.actions ?? new List<global::Doroti.Framework.Widgets.Widget>(), dividerColor: CupertinoDynamicColor.resolve(DialogLibrary._kActionSheetButtonDividerColor, context))))))); if (widget.cancelButton is not null) { __collection47257.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildCancelButton())); } return __collection47257; }))();
        double actionSheetWidth = MediaQuery.orientationOf(context) switch { Orientation.portrait => MediaQuery.widthOf(context), Orientation.landscape => MediaQuery.heightOf(context), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        return new global::Doroti.Framework.Widgets.SafeArea(minimum: EdgeInsets.CreateOnly(bottom: DialogLibrary._kActionSheetEdgePadding), child: new global::Doroti.Framework.Widgets.ScrollConfiguration(behavior: ScrollConfiguration.of(context).copyWith(scrollbars: false), child: new global::Doroti.Framework.Widgets.Semantics(namesRoute: true, scopesRoute: true, explicitChildNodes: true, role: SemanticsRole.dialog, label: "Alert", child: new CupertinoUserInterfaceLevel(data: CupertinoUserInterfaceLevelData.elevated, child: new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsets.CreateOnly(left: DialogLibrary._kActionSheetEdgePadding, right: DialogLibrary._kActionSheetEdgePadding, top: _topPadding(context)), child: new global::Doroti.Framework.Widgets.SizedBox(width: actionSheetWidth - (DialogLibrary._kActionSheetEdgePadding * 2L), child: new _ActionSheetGestureDetector__dialog(child: new global::Doroti.Framework.Widgets.Semantics(explicitChildNodes: true, child: new global::Doroti.Framework.Widgets.Column(mainAxisAlignment: MainAxisAlignment.end, mainAxisSize: MainAxisSize.min, crossAxisAlignment: CrossAxisAlignment.stretch, children: childrenLocal)))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoActionSheetAction : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::System.Action onPressed { get; private set; } = default!;
    public virtual bool isDefaultAction { get; private set; } = default!;
    public virtual bool isDestructiveAction { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;

    public CupertinoActionSheetAction(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action onPressed = default!, bool isDefaultAction = false, bool isDestructiveAction = false, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, Color? focusColor = null, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key)
    {
        this.onPressed = onPressed;
        this.isDefaultAction = isDefaultAction;
        this.isDestructiveAction = isDestructiveAction;
        this.mouseCursor = mouseCursor;
        this.focusNode = focusNode;
        this.focusColor = focusColor;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoActionSheetActionState__dialog());
}

internal class _CupertinoActionSheetActionState__dialog : global::Doroti.Framework.Widgets.State<CupertinoActionSheetAction>, _SlideTarget__dialog
{
    internal virtual bool _showHighlight { get; set; } = false;
    private bool __late__actionMap_initialized;
    private DartMap<Type, dynamic> __late__actionMap = default!;
    internal virtual DartMap<Type, dynamic> _actionMap
    {
        get
        {
            if (!__late__actionMap_initialized)
            {
                __late__actionMap = new DartMap<Type, dynamic> { [typeof(global::Doroti.Framework.Widgets.ActivateIntent)] = new global::Doroti.Framework.Widgets.CallbackAction<global::Doroti.Framework.Widgets.ActivateIntent>(onInvoke: _handleTap) };
                __late__actionMap_initialized = true;
            }
            return __late__actionMap;
        }
    }

    public virtual bool didEnter(bool fromPointerDown, bool innerEnabled)
    {
        return innerEnabled;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void didLeave()
    {
    }

    public virtual void didConfirm()
    {
        widget.onPressed();
    }

    internal virtual void _onShowFocusHighlight(bool showHighlight)
    {
        setState(() =>
        {
            _showHighlight = showHighlight;
        });
    }

    internal virtual void _handleTap(global::Doroti.Framework.Widgets.Intent? __unused0 = null)
    {
        widget.onPressed();
        context.findRenderObject()!.sendSemanticsEvent(new global::Doroti.Framework.Semantics.TapSemanticEvent());
    }

    public virtual global::Doroti.Ui.Color effectiveFocusBackgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(HSLColor.CreateFromColor((widget.focusColor ?? CupertinoColors.activeBlue).withOpacity(Equals(CupertinoTheme.brightnessOf(context), Brightness.light) ? ConstantsLibrary.kCupertinoButtonTintedOpacityLight : ConstantsLibrary.kCupertinoButtonTintedOpacityDark)).toColor());
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.MouseRegion(cursor: widget.mouseCursor ?? (Foundation.ConstantsLibrary.kIsWeb ? SystemMouseCursors.click : MouseCursor.defer), child: new global::Doroti.Framework.Widgets.MetaData(metaData: this, behavior: HitTestBehavior.opaque, child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minHeight: DialogLibrary._kActionSheetButtonMinHeight), child: new global::Doroti.Framework.Widgets.FocusableActionDetector(actions: _actionMap, focusNode: widget.focusNode, onShowFocusHighlight: _onShowFocusHighlight, child: new global::Doroti.Framework.Widgets.Semantics(button: true, onTap: () => widget.onPressed(), child: _showHighlight ? new global::Doroti.Framework.Widgets.DecoratedBox(decoration: new global::Doroti.Framework.Painting.BoxDecoration(color: effectiveFocusBackgroundColor), child: new _ActionSheetActionContent__dialog(isDestructiveAction: widget.isDestructiveAction, isDefaultAction: widget.isDefaultAction, child: widget.child)) : new _ActionSheetActionContent__dialog(isDestructiveAction: widget.isDestructiveAction, isDefaultAction: widget.isDefaultAction, child: widget.child))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ActionSheetActionContent__dialog : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual bool isDestructiveAction { get; private set; } = default!;
    public virtual bool isDefaultAction { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _ActionSheetActionContent__dialog(bool isDestructiveAction, bool isDefaultAction, global::Doroti.Framework.Widgets.Widget child)
    {
        this.isDestructiveAction = isDestructiveAction;
        this.isDefaultAction = isDefaultAction;
        this.child = child;
    }

    internal static double _buttonFontSize(double contextBodySize)
    {
        return contextBodySize switch { <= 17L => 21.0, <= 19L => DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(21.0, 23.0, (contextBodySize - 17.0) / (19.0 - 17.0))), <= 21L => DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(23.0, 24.0, (contextBodySize - 19.0) / (21.0 - 19.0))), <= 24L => 24.0, _ => contextBodySize };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        var higLargeBodySize = 17.0;
        double contextBodySize = MediaQuery.textScalerOf(context).scale(higLargeBodySize);
        double contextScaleFactor = contextBodySize / higLargeBodySize;
        double fontSizeLocal = _buttonFontSize(contextBodySize);
        global::Doroti.Framework.Painting.TextStyle styleLocal = DialogLibrary._kActionSheetActionStyle.copyWith(fontSize: fontSizeLocal / contextScaleFactor, color: isDestructiveAction ? CupertinoDynamicColor.resolve(CupertinoColors.systemRed, context) : CupertinoTheme.of(context).primaryColor);
        if (isDefaultAction)
        {
            styleLocal = styleLocal.copyWith(fontWeight: FontWeight.w600);
        }
        double verticalPadding = DialogLibrary._kActionSheetButtonVerticalPaddingBase + (fontSizeLocal * DialogLibrary._kActionSheetButtonVerticalPaddingFactor);
        return new global::Doroti.Framework.Widgets.Padding(padding: new global::Doroti.Framework.Painting.EdgeInsets(DialogLibrary._kActionSheetButtonHorizontalPadding, verticalPadding, DialogLibrary._kActionSheetButtonHorizontalPadding, verticalPadding), child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: styleLocal, textAlign: TextAlign.center, child: new global::Doroti.Framework.Widgets.Center(child: child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ActionSheetButtonBackground__dialog : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual bool isCancel { get; private set; } = default!;
    public virtual bool pressed { get; private set; } = default!;
    public virtual global::System.Action<bool>? onPressStateChange { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _ActionSheetButtonBackground__dialog(bool isCancel = false, bool pressed = default!, global::System.Action<bool>? onPressStateChange = null, global::Doroti.Framework.Widgets.Widget child = default!)
    {
        this.isCancel = isCancel;
        this.pressed = pressed;
        this.onPressStateChange = onPressStateChange;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ActionSheetButtonBackgroundState__dialog());
}

internal class _ActionSheetButtonBackgroundState__dialog : global::Doroti.Framework.Widgets.State<_ActionSheetButtonBackground__dialog>, _SlideTarget__dialog
{
    internal virtual void _emitVibration()
    {
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.iOS:
            case TargetPlatform.android:
                {
                    DartRuntimePrimitives.Ignore(HapticFeedback.selectionClick());
                    break;
                }
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.macOS:
            case TargetPlatform.windows:
                {
                    break;
                }
        }
    }

    public virtual bool didEnter(bool fromPointerDown, bool innerEnabled)
    {
        DartRuntimePrimitives.Assert(() => innerEnabled);
        widget.onPressStateChange?.Invoke(true);
        if (!fromPointerDown)
        {
            _emitVibration();
        }
        return innerEnabled;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void didLeave()
    {
        widget.onPressStateChange?.Invoke(false);
    }

    public virtual void didConfirm()
    {
        widget.onPressStateChange?.Invoke(false);
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Widgets.Widget childLocal = default!;
        if (!widget.isCancel)
        {
            childLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ColoredBox(color: CupertinoDynamicColor.resolve(widget.pressed ? DialogLibrary._kActionSheetPressedColor : DialogLibrary._kActionSheetBackgroundColor, context), child: widget.child));
        }
        else
        {
            var borderRadiusLocal = BorderRadius.CreateAll(Radius.circular(DialogLibrary._kCornerRadius));
            childLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ClipRSuperellipse(borderRadius: borderRadiusLocal, child: new global::Doroti.Framework.Widgets.DecoratedBox(decoration: new global::Doroti.Framework.Painting.BoxDecoration(color: CupertinoDynamicColor.resolve(widget.pressed ? DialogLibrary._kActionSheetCancelPressedColor : DialogLibrary._kActionSheetCancelColor, context)), child: widget.child)));
        }
        return new global::Doroti.Framework.Widgets.MetaData(metaData: this, child: childLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _Divider__dialog : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual Color dividerColor { get; private set; } = default!;
    public virtual Color hiddenColor { get; private set; } = default!;
    public virtual bool hidden { get; private set; } = default!;

    internal _Divider__dialog(Color dividerColor, Color hiddenColor, bool hidden)
    {
        this.dividerColor = dividerColor;
        this.hiddenColor = hiddenColor;
        this.hidden = hidden;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.LimitedBox(maxHeight: DialogLibrary._kDividerThickness, maxWidth: DialogLibrary._kDividerThickness, child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minHeight: DialogLibrary._kDividerThickness, minWidth: DialogLibrary._kDividerThickness), child: new global::Doroti.Framework.Widgets.DecoratedBox(decoration: new global::Doroti.Framework.Painting.BoxDecoration(color: hidden ? CupertinoDynamicColor.resolve(hiddenColor, context) : dividerColor))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _OverscrollBackground__dialog : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual Color color { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _OverscrollBackground__dialog(Color color, global::Doroti.Framework.Widgets.Widget child)
    {
        this.color = color;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _OverscrollBackgroundState__dialog());
}

internal class _OverscrollBackgroundState__dialog : global::Doroti.Framework.Widgets.State<_OverscrollBackground__dialog>
{
    internal virtual double _topOverscroll { get; set; } = 0;
    internal virtual double _bottomOverscroll { get; set; } = 0;

    internal virtual bool _onScrollUpdate(global::Doroti.Framework.Widgets.ScrollUpdateNotification notification)
    {
        global::Doroti.Framework.Widgets.ScrollMetrics metricsLocal = notification.metrics;
        setState(() =>
        {
            _topOverscroll = Math.Min(Math.Max(metricsLocal.minScrollExtent - metricsLocal.pixels, 0), metricsLocal.viewportDimension);
            _bottomOverscroll = Math.Min(Math.Max(metricsLocal.pixels - metricsLocal.maxScrollExtent, 0), metricsLocal.viewportDimension);
        });
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Widgets.Widget overscroll = new global::Doroti.Framework.Widgets.Column(mainAxisSize: MainAxisSize.min, mainAxisAlignment: MainAxisAlignment.spaceBetween, crossAxisAlignment: CrossAxisAlignment.stretch, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.DecoratedBox(decoration: new global::Doroti.Framework.Painting.BoxDecoration(color: widget.color), child: new global::Doroti.Framework.Widgets.SizedBox(height: _topOverscroll))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.DecoratedBox(decoration: new global::Doroti.Framework.Painting.BoxDecoration(color: widget.color), child: new global::Doroti.Framework.Widgets.SizedBox(height: _bottomOverscroll))) });
        return new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(Positioned.CreateFill(child: overscroll)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.NotificationListener<global::Doroti.Framework.Widgets.ScrollUpdateNotification>(onNotification: _onScrollUpdate, child: widget.child)) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal delegate void _PressedUpdateHandler__dialog(long actionIndex, bool state);

internal class _ActionSheetActionSection__dialog : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual List<global::Doroti.Framework.Widgets.Widget>? actions { get; private set; }
    public virtual global::System.Action<long, bool> onPressedUpdate { get; private set; } = default!;
    public virtual long? pressedIndex { get; private set; }
    public virtual Color dividerColor { get; private set; } = default!;
    public virtual Color backgroundColor { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.ScrollController scrollController { get; private set; } = default!;

    internal _ActionSheetActionSection__dialog(List<global::Doroti.Framework.Widgets.Widget>? actions, long? pressedIndex, Color dividerColor, Color backgroundColor, global::System.Action<long, bool> onPressedUpdate, global::Doroti.Framework.Widgets.ScrollController scrollController)
    {
        this.actions = actions;
        this.pressedIndex = pressedIndex;
        this.dividerColor = dividerColor;
        this.backgroundColor = backgroundColor;
        this.onPressedUpdate = onPressedUpdate;
        this.scrollController = scrollController;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if ((actions is null) || !Enumerable.Any(actions!))
        {
            return new global::Doroti.Framework.Widgets.LimitedBox(maxWidth: 0, child: new global::Doroti.Framework.Widgets.SizedBox(width: double.PositiveInfinity, height: 0));
        }
        var column = new List<global::Doroti.Framework.Widgets.Widget>();
        for (var actionIndex = 0L; actionIndex < checked(actions!.Count); actionIndex += 1L)
        {
            if (actionIndex != 0L)
            {
                column.Add(new _Divider__dialog(dividerColor: dividerColor, hiddenColor: DialogLibrary._kActionSheetBackgroundColor, hidden: (pressedIndex == (actionIndex - 1L)) || (pressedIndex == actionIndex)));
            }
            column.Add(new _ActionSheetButtonBackground__dialog(pressed: pressedIndex == actionIndex, onPressStateChange: (state) =>
            {
                onPressedUpdate(actionIndex, state);
            }, child: actions![(int)actionIndex]));
        }
        return new CupertinoScrollbar(controller: scrollController, child: new global::Doroti.Framework.Widgets.SingleChildScrollView(controller: scrollController, child: new global::Doroti.Framework.Widgets.Column(crossAxisAlignment: CrossAxisAlignment.stretch, children: column)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ActionSheetMainSheet__dialog : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual long? pressedIndex { get; private set; }
    public virtual global::System.Action<long, bool> onPressedUpdate { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.ScrollController scrollController { get; private set; } = default!;
    public virtual List<global::Doroti.Framework.Widgets.Widget> actions { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? contentSection { get; private set; }
    public virtual Color dividerColor { get; private set; } = default!;
    internal static global::Doroti.Framework.Widgets.Widget _empty = new global::Doroti.Framework.Widgets.LimitedBox(maxWidth: 0, child: new global::Doroti.Framework.Widgets.SizedBox(width: double.PositiveInfinity, height: 0));

    internal _ActionSheetMainSheet__dialog(long? pressedIndex, global::System.Action<long, bool> onPressedUpdate, global::Doroti.Framework.Widgets.ScrollController scrollController, List<global::Doroti.Framework.Widgets.Widget> actions, global::Doroti.Framework.Widgets.Widget? contentSection, Color dividerColor)
    {
        this.pressedIndex = pressedIndex;
        this.onPressedUpdate = onPressedUpdate;
        this.scrollController = scrollController;
        this.actions = actions;
        this.contentSection = contentSection;
        this.dividerColor = dividerColor;
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _scrolledActionsSection(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color backgroundColorLocal = CupertinoDynamicColor.resolve(DialogLibrary._kActionSheetBackgroundColor, context);
        return new _OverscrollBackground__dialog(color: backgroundColorLocal, child: CupertinoFocusHalo.CreateWithRRect(borderRadius: ConstantsLibrary.kCupertinoButtonSizeBorderRadius.GetValueOrDefault(CupertinoButtonSize.large)!.copyWith(topLeft: Radius.zero, topRight: Radius.zero), child: new _ActionSheetActionSection__dialog(actions: actions, scrollController: scrollController, dividerColor: dividerColor, backgroundColor: backgroundColorLocal, pressedIndex: pressedIndex, onPressedUpdate: onPressedUpdate)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _dividerAndActionsSection(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color backgroundColor = CupertinoDynamicColor.resolve(DialogLibrary._kActionSheetBackgroundColor, context);
        return new global::Doroti.Framework.Widgets.Column(mainAxisSize: MainAxisSize.min, crossAxisAlignment: CrossAxisAlignment.stretch, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _Divider__dialog(dividerColor: dividerColor, hiddenColor: backgroundColor, hidden: false)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: _scrolledActionsSection(context))) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if (!Enumerable.Any(actions))
        {
            return contentSection ?? _empty;
        }
        if (contentSection is null)
        {
            return _scrolledActionsSection(context);
        }
        return new _PriorityColumn__dialog(top: contentSection!, bottom: _dividerAndActionsSection(context), bottomMinHeight: DialogLibrary._kActionSheetActionsSectionMinHeight + DialogLibrary._kDividerThickness);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CupertinoAlertContentSection__dialog : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget? title { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? message { get; private set; }
    public virtual global::Doroti.Framework.Widgets.ScrollController scrollController { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsets? titlePadding { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsets? messagePadding { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsets? additionalPaddingBetweenTitleAndMessage { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? titleTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? messageTextStyle { get; private set; }

    internal _CupertinoAlertContentSection__dialog(global::Doroti.Framework.Widgets.Widget? title = null, global::Doroti.Framework.Widgets.Widget? message = null, global::Doroti.Framework.Widgets.ScrollController scrollController = default!, global::Doroti.Framework.Painting.EdgeInsets? titlePadding = null, global::Doroti.Framework.Painting.EdgeInsets? messagePadding = null, global::Doroti.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Framework.Painting.TextStyle? messageTextStyle = null, global::Doroti.Framework.Painting.EdgeInsets? additionalPaddingBetweenTitleAndMessage = null)
    {
        this.title = title;
        this.message = message;
        this.scrollController = scrollController;
        this.titlePadding = titlePadding;
        this.messagePadding = messagePadding;
        this.titleTextStyle = titleTextStyle;
        this.messageTextStyle = messageTextStyle;
        this.additionalPaddingBetweenTitleAndMessage = additionalPaddingBetweenTitleAndMessage;
        System.Diagnostics.Debug.Assert((title is null) || ((titlePadding is not null) && (titleTextStyle is not null)));
        System.Diagnostics.Debug.Assert((message is null) || ((messagePadding is not null) && (messageTextStyle is not null)));
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if ((title is null) && (message is null))
        {
            return new global::Doroti.Framework.Widgets.SingleChildScrollView(controller: scrollController, child: SizedBox.CreateShrink());
        }
        var titleContentGroup = ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection70765 = new List<global::Doroti.Framework.Widgets.Widget>(); if (title is not null) { __collection70765.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: titlePadding!, child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: titleTextStyle!, textAlign: TextAlign.center, child: title!)))); } if (message is not null) { __collection70765.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: messagePadding!, child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: messageTextStyle!, textAlign: TextAlign.center, child: message!)))); } return __collection70765; }))();
        if ((additionalPaddingBetweenTitleAndMessage is not null) && (checked(titleContentGroup.Count) > 1L))
        {
            titleContentGroup.Insert(checked((int)1L), new global::Doroti.Framework.Widgets.Padding(padding: additionalPaddingBetweenTitleAndMessage!));
        }
        return new CupertinoScrollbar(controller: scrollController, child: new global::Doroti.Framework.Widgets.SingleChildScrollView(controller: scrollController, child: new global::Doroti.Framework.Widgets.Column(crossAxisAlignment: CrossAxisAlignment.stretch, children: titleContentGroup)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CupertinoAlertActionSection__dialog : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual List<global::Doroti.Framework.Widgets.Widget> actions { get; private set; } = default!;
    public virtual global::System.Action<long, bool> onPressedUpdate { get; private set; } = default!;
    public virtual long? pressedIndex { get; private set; }
    public virtual global::Doroti.Framework.Widgets.ScrollController scrollController { get; private set; } = default!;

    internal _CupertinoAlertActionSection__dialog(List<global::Doroti.Framework.Widgets.Widget> actions, global::System.Action<long, bool> onPressedUpdate, long? pressedIndex, global::Doroti.Framework.Widgets.ScrollController scrollController)
    {
        this.actions = actions;
        this.onPressedUpdate = onPressedUpdate;
        this.pressedIndex = pressedIndex;
        this.scrollController = scrollController;
        System.Diagnostics.Debug.Assert(checked(actions.Count) != 0L);
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color dialogColor = CupertinoDynamicColor.resolve(DialogLibrary._kDialogColor, context);
        global::Doroti.Ui.Color dialogPressedColor = CupertinoDynamicColor.resolve(DialogLibrary._kDialogPressedColor, context);
        global::Doroti.Ui.Color dividerColorLocal = CupertinoDynamicColor.resolve(CupertinoColors.separator, context);
        var column = new List<global::Doroti.Framework.Widgets.Widget>();
        for (var actionIndex = 0L; actionIndex < checked(actions.Count); actionIndex += 1L)
        {
            if (actionIndex != 0L)
            {
                column.Add(new _Divider__dialog(dividerColor: dividerColorLocal, hiddenColor: dialogColor, hidden: (pressedIndex == (actionIndex - 1L)) || (pressedIndex == actionIndex)));
            }
            column.Add(new _AlertDialogButtonBackground__dialog(idleColor: dialogColor, pressedColor: dialogPressedColor, pressed: pressedIndex == actionIndex, onPressStateChange: (state) =>
            {
                onPressedUpdate(actionIndex, state);
            }, child: actions[(int)actionIndex]));
        }
        return new CupertinoScrollbar(controller: scrollController, child: new global::Doroti.Framework.Widgets.SingleChildScrollView(controller: scrollController, child: new _AlertDialogActionsLayout__dialog(dividerThickness: DialogLibrary._kDividerThickness, children: column)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AlertDialogButtonBackground__dialog : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual bool pressed { get; private set; } = default!;
    public virtual global::System.Action<bool>? onPressStateChange { get; private set; }
    public virtual Color idleColor { get; private set; } = default!;
    public virtual Color pressedColor { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _AlertDialogButtonBackground__dialog(Color idleColor, Color pressedColor, bool pressed, global::System.Action<bool>? onPressStateChange, global::Doroti.Framework.Widgets.Widget child)
    {
        this.idleColor = idleColor;
        this.pressedColor = pressedColor;
        this.pressed = pressed;
        this.onPressStateChange = onPressStateChange;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _AlertDialogButtonBackgroundState__dialog());
}

internal class _AlertDialogButtonBackgroundState__dialog : global::Doroti.Framework.Widgets.State<_AlertDialogButtonBackground__dialog>, _SlideTarget__dialog
{
    internal virtual void _emitVibration()
    {
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.iOS:
            case TargetPlatform.android:
                {
                    DartRuntimePrimitives.Ignore(HapticFeedback.selectionClick());
                    break;
                }
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.macOS:
            case TargetPlatform.windows:
                {
                    break;
                }
        }
    }

    public virtual bool didEnter(bool fromPointerDown, bool innerEnabled)
    {
        widget.onPressStateChange?.Invoke(innerEnabled);
        if (innerEnabled && !fromPointerDown)
        {
            _emitVibration();
        }
        return innerEnabled;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void didLeave()
    {
        widget.onPressStateChange?.Invoke(false);
    }

    public virtual void didConfirm()
    {
        widget.onPressStateChange?.Invoke(false);
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color backgroundColor = widget.pressed ? widget.pressedColor : widget.idleColor;
        return new global::Doroti.Framework.Widgets.MetaData(metaData: this, child: new global::Doroti.Framework.Widgets.MergeSemantics(child: new global::Doroti.Framework.Widgets.Container(decoration: new global::Doroti.Framework.Painting.BoxDecoration(color: CupertinoDynamicColor.resolve(backgroundColor, context)), child: widget.child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoDialogAction : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual bool isDefaultAction { get; private set; } = default!;
    public virtual bool isDestructiveAction { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle? textStyle { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;

    public CupertinoDialogAction(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action? onPressed = null, bool isDefaultAction = false, bool isDestructiveAction = false, global::Doroti.Framework.Painting.TextStyle? textStyle = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key)
    {
        this.onPressed = onPressed;
        this.isDefaultAction = isDefaultAction;
        this.isDestructiveAction = isDestructiveAction;
        this.textStyle = textStyle;
        this.mouseCursor = mouseCursor;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoDialogActionState__dialog());
}

internal class _CupertinoDialogActionState__dialog : global::Doroti.Framework.Widgets.State<CupertinoDialogAction>, _SlideTarget__dialog
{
    public virtual bool enabled => DartRuntimePrimitives.ConvertValue<bool>(widget.onPressed is not null);
    public virtual bool didEnter(bool fromPointerDown, bool innerEnabled)
    {
        return enabled;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void didLeave()
    {
    }

    public virtual void didConfirm()
    {
        widget.onPressed?.Invoke();
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildContentWithRegularSizingPolicy(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Painting.TextStyle textStyle, global::Doroti.Framework.Widgets.Widget content, double padding)
    {
        bool isInAccessibilityMode = DialogLibrary._isInAccessibilityMode(context);
        double dialogWidth = isInAccessibilityMode ? DialogLibrary._kAccessibilityCupertinoDialogWidth : DialogLibrary._kCupertinoDialogWidth;
        double fontSizeRatio = MediaQuery.textScalerOf(context).scale(DartRuntimePrimitives.RequireValue(textStyle.fontSize)) / DialogLibrary._kDialogMinButtonFontSize;
        return new global::Doroti.Framework.Widgets.FittedBox(fit: BoxFit.scaleDown, child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(maxWidth: fontSizeRatio * (dialogWidth - 2L * padding)), child: new global::Doroti.Framework.Widgets.Semantics(button: true, onTap: widget.onPressed, child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: textStyle, textAlign: TextAlign.center, overflow: TextOverflow.ellipsis, maxLines: 1L, child: content))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildContentWithAccessibilitySizingPolicy(global::Doroti.Framework.Painting.TextStyle textStyle, global::Doroti.Framework.Widgets.Widget content)
    {
        return new global::Doroti.Framework.Widgets.DefaultTextStyle(style: textStyle, textAlign: TextAlign.center, child: content);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Painting.TextStyle style = DialogLibrary._kCupertinoDialogActionStyle.copyWith(color: CupertinoDynamicColor.resolve(widget.isDestructiveAction ? CupertinoColors.systemRed : CupertinoTheme.of(context).primaryColor, context)).merge(widget.textStyle);
        if (widget.isDefaultAction)
        {
            style = style.copyWith(fontWeight: FontWeight.w600);
        }
        if (!enabled)
        {
            style = style.copyWith(color: style.color!.withOpacity(0.5));
        }
        double fontSizeLocal = style.fontSize ?? Text_painterLibrary.kDefaultFontSize;
        double fontSizeToScale = (fontSizeLocal == 0.0) ? Text_painterLibrary.kDefaultFontSize : fontSizeLocal;
        double effectiveTextScale = MediaQuery.textScalerOf(context).scale(fontSizeToScale) / fontSizeToScale;
        double paddingLocal = 8.0 * effectiveTextScale;
        global::Doroti.Framework.Widgets.Widget sizedContent = DialogLibrary._isInAccessibilityMode(context) ? _buildContentWithAccessibilitySizingPolicy(textStyle: style, content: widget.child) : _buildContentWithRegularSizingPolicy(context: context, textStyle: style, content: widget.child, padding: paddingLocal);
        return new global::Doroti.Framework.Widgets.MouseRegion(cursor: widget.mouseCursor ?? ((enabled && Foundation.ConstantsLibrary.kIsWeb) ? SystemMouseCursors.click : MouseCursor.defer), child: new global::Doroti.Framework.Widgets.MetaData(metaData: this, behavior: HitTestBehavior.opaque, child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minHeight: DialogLibrary._kDialogMinButtonHeight), child: new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsets.CreateAll(paddingLocal), child: new global::Doroti.Framework.Widgets.Center(child: sizedContent)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AlertDialogActionsLayout__dialog : global::Doroti.Framework.Widgets.MultiChildRenderObjectWidget
{
    internal virtual double _dividerThickness { get; private set; } = default!;

    internal _AlertDialogActionsLayout__dialog(double dividerThickness, List<global::Doroti.Framework.Widgets.Widget> children) : base(children: children)
    {
        _dividerThickness = dividerThickness;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new _RenderAlertDialogActionsLayout__dialog(dividerThickness: _dividerThickness, textDirection: Directionality.of(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderAlertDialogActionsLayout__dialog)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderAlertDialogActionsLayout__dialog>)(() =>
{
    var __cascade = __renderObject;
    __cascade.dividerThickness = _dividerThickness;
    __cascade.textDirection = Directionality.of(context);
    return __cascade;
}))());
    }

}

public class _RenderAlertDialogActionsLayout__dialog : global::Doroti.Framework.Rendering.RenderFlex
{
    internal virtual double _dividerThickness { get; set; } = default!;

    internal _RenderAlertDialogActionsLayout__dialog(List<global::Doroti.Framework.Rendering.RenderBox>? children = null, double dividerThickness = default!, TextDirection? textDirection = null) : base(textDirection: textDirection, direction: Axis.vertical, mainAxisSize: MainAxisSize.min, crossAxisAlignment: CrossAxisAlignment.stretch)
    {
        _dividerThickness = dividerThickness;
        addAll(children);
    }

    public virtual double dividerThickness
    {
        get => _dividerThickness;
        set
        {
            var newValue = value;
            if (newValue != _dividerThickness)
            {
                _dividerThickness = newValue;
                markNeedsLayout();
            }
        }
    }
    public virtual double horizontalSlotWidthFor(double overallWidth) => DartRuntimePrimitives.ConvertValue<double>((overallWidth - dividerThickness) / 2L);
    public override double computeMinIntrinsicHeight(double width)
    {
        if (!_useHorizontalLayout(width))
        {
            return base.computeMinIntrinsicHeight(width);
        }
        double slotWidth = horizontalSlotWidthFor(overallWidth: width);
        double height = 0;
        _forEachSlot((slot) =>
        {
            height = Math.Max(height, slot.getMinIntrinsicHeight(slotWidth));
        });
        return height;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        if (!_useHorizontalLayout(width))
        {
            return base.computeMaxIntrinsicHeight(width);
        }
        double slotWidth = horizontalSlotWidthFor(overallWidth: width);
        double height = 0;
        _forEachSlot((slot) =>
        {
            height = Math.Max(height, slot.getMaxIntrinsicHeight(slotWidth));
        });
        return height;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        if (!_debugHasValidConstraints(constraints))
        {
            return Size.zero;
        }
        double overallWidth = constraints.maxWidth;
        if (!_useHorizontalLayout(overallWidth))
        {
            return base.computeDryLayout(constraints);
        }
        double height = getMinIntrinsicHeight(overallWidth);
        return new global::Doroti.Ui.Size(overallWidth, height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        if (firstChild is null)
        {
            size = constraints.smallest;
            return;
        }
        if (!_debugHasValidConstraints(constraints))
        {
            size = constraints.smallest;
            return;
        }
        double overallWidthLocal = constraints.maxWidth;
        if (!_useHorizontalLayout(overallWidthLocal))
        {
            base.performLayout();
            return;
        }
        double slotWidth = horizontalSlotWidthFor(overallWidth: overallWidthLocal);
        double height = getMinIntrinsicHeight(overallWidthLocal);
        size = new global::Doroti.Ui.Size(overallWidthLocal, height);
        var ltrLocal = Equals(textDirection, TextDirection.ltr);
        global::Doroti.Framework.Rendering.RenderBox slot = firstChild!;
        double x = ltrLocal ? 0 : (overallWidthLocal - slotWidth);
        while (true)
        {
            slot.layout(BoxConstraints.CreateTight(new global::Doroti.Ui.Size(slotWidth, height)), parentUsesSize: true);
            ((global::Doroti.Framework.Rendering.FlexParentData?)slot.parentData!)!.offset = new global::Doroti.Ui.Offset(x, 0);
            if (ltrLocal)
            {
                x += slot.size.width;
            }
            else
            {
                x -= slot.size.width;
            }
            global::Doroti.Framework.Rendering.RenderBox? divider = childAfter(slot);
            if (divider is null)
            {
                break;
            }
            divider.layout(BoxConstraints.CreateTight(new global::Doroti.Ui.Size(dividerThickness, height)));
            ((global::Doroti.Framework.Rendering.FlexParentData?)divider.parentData!)!.offset = new global::Doroti.Ui.Offset(x, 0);
            if (ltrLocal)
            {
                x += dividerThickness;
            }
            else
            {
                x -= dividerThickness;
            }
            slot = childAfter(divider)!;
        }
    }

    internal virtual bool _debugHasValidConstraints(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                global::Doroti.Framework.Foundation.ErrorSummary? errorSummary = default!;
                if (constraints.maxWidth == double.PositiveInfinity)
                {
                    errorSummary = new global::Doroti.Framework.Foundation.ErrorSummary("The incoming width constraints are unbounded.");
                }
                if (errorSummary is not null)
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { errorSummary, new global::Doroti.Framework.Foundation.ErrorDescription($"The incoming constraints are: {constraints}") }));
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _useHorizontalLayout(double overallWidth)
    {
        if (childCount != 3L)
        {
            return false;
        }
        double slotWidth = horizontalSlotWidthFor(overallWidth: overallWidth);
        global::Doroti.Framework.Rendering.RenderBox child = firstChild!;
        while (true)
        {
            if (child.getMaxIntrinsicWidth(double.PositiveInfinity) > slotWidth)
            {
                return false;
            }
            global::Doroti.Framework.Rendering.RenderBox? divider = childAfter(child);
            if (divider is null)
            {
                break;
            }
            child = childAfter(divider)!;
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _forEachSlot(global::System.Action<global::Doroti.Framework.Rendering.RenderBox> action)
    {
        DartRuntimePrimitives.Assert(() => (checked(childCount) & 1L) != 0L);
        global::Doroti.Framework.Rendering.RenderBox slot = firstChild!;
        while (true)
        {
            action(slot);
            global::Doroti.Framework.Rendering.RenderBox? divider = childAfter(slot);
            if (divider is null)
            {
                break;
            }
            slot = childAfter(divider)!;
        }
    }

}

internal delegate void _TwoChildrenHeights__dialog();

internal class _PriorityColumn__dialog : global::Doroti.Framework.Widgets.MultiChildRenderObjectWidget
{
    public virtual double bottomMinHeight { get; private set; } = default!;

    internal _PriorityColumn__dialog(global::Doroti.Framework.Widgets.Widget top, global::Doroti.Framework.Widgets.Widget bottom, double bottomMinHeight) : base(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(top), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(bottom) })
    {
        this.bottomMinHeight = bottomMinHeight;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new _RenderPriorityColumn__dialog(bottomMinHeight: bottomMinHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderPriorityColumn__dialog)renderObject;
        __renderObject.bottomMinHeight = bottomMinHeight;
    }

}

public class _RenderPriorityColumn__dialog : global::Doroti.Framework.Rendering.RenderFlex
{
    internal virtual double _bottomMinHeight { get; set; } = default!;

    internal _RenderPriorityColumn__dialog(List<global::Doroti.Framework.Rendering.RenderBox>? children = null, double bottomMinHeight = default!) : base(direction: Axis.vertical, mainAxisSize: MainAxisSize.min, crossAxisAlignment: CrossAxisAlignment.stretch)
    {
        _bottomMinHeight = bottomMinHeight;
        addAll(children);
    }

    public virtual double bottomMinHeight
    {
        get => _bottomMinHeight;
        set
        {
            var newValue = value;
            if (newValue != _bottomMinHeight)
            {
                _bottomMinHeight = newValue;
                markNeedsLayout();
            }
        }
    }
    public override double computeMinIntrinsicHeight(double width)
    {
        DartRuntimePrimitives.Assert(() => childCount == 2L);
        return firstChild!.getMinIntrinsicHeight(width) + lastChild!.getMinIntrinsicHeight(width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        DartRuntimePrimitives.Assert(() => childCount == 2L);
        return firstChild!.getMaxIntrinsicHeight(width) + lastChild!.getMaxIntrinsicHeight(width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        double width = constraints.maxWidth;
        double maxHeightLocal = constraints.maxHeight;
        var (topChildHeight, bottomChildHeight) = _childrenHeights(width, maxHeightLocal);
        return new global::Doroti.Ui.Size(width, topChildHeight + bottomChildHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        double width = constraints.maxWidth;
        double maxHeightLocal = constraints.maxHeight;
        var (topChildHeight, bottomChildHeight) = _childrenHeights(width, maxHeightLocal);
        size = new global::Doroti.Ui.Size(width, topChildHeight + bottomChildHeight);
        firstChild!.layout(BoxConstraints.CreateTight(new global::Doroti.Ui.Size(width, topChildHeight)), parentUsesSize: true);
        ((global::Doroti.Framework.Rendering.FlexParentData?)firstChild!.parentData!)!.offset = Offset.zero;
        lastChild!.layout(BoxConstraints.CreateTight(new global::Doroti.Ui.Size(width, bottomChildHeight)), parentUsesSize: true);
        ((global::Doroti.Framework.Rendering.FlexParentData?)lastChild!.parentData!)!.offset = new global::Doroti.Ui.Offset(0, topChildHeight);
    }

    internal virtual (double bottomChildHeight, double topChildHeight) _childrenHeights(double width, double maxHeight)
    {
        DartRuntimePrimitives.Assert(() => childCount == 2L);
        double topIntrinsic = firstChild!.getMinIntrinsicHeight(width);
        double bottomIntrinsic = lastChild!.getMinIntrinsicHeight(width);
        if ((topIntrinsic + bottomIntrinsic) <= maxHeight)
        {
            return (bottomChildHeight: bottomIntrinsic, topChildHeight: topIntrinsic);
        }
        double effectiveBottomMinHeight = Math.Min(_bottomMinHeight, bottomIntrinsic);
        if ((maxHeight - topIntrinsic) >= effectiveBottomMinHeight)
        {
            return (bottomChildHeight: maxHeight - topIntrinsic, topChildHeight: topIntrinsic);
        }
        if (maxHeight >= effectiveBottomMinHeight)
        {
            return (bottomChildHeight: effectiveBottomMinHeight, topChildHeight: maxHeight - effectiveBottomMinHeight);
        }
        return (bottomChildHeight: maxHeight, topChildHeight: 0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
