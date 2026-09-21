// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/dialog.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class DialogLibrary
{
    internal static TextStyle _kCupertinoDialogTitleStyle = new TextStyle(
        fontFamily: "CupertinoSystemText",
        inherit: false,
        fontSize: 17.0,
        fontWeight: FontWeight.w600,
        height: 1.3,
        letterSpacing: -0.5,
        textBaseline: TextBaseline.alphabetic
    );
}

public static partial class DialogLibrary
{
    internal static TextStyle _kCupertinoDialogContentStyle = new TextStyle(
        fontFamily: "CupertinoSystemText",
        inherit: false,
        fontSize: 13.0,
        fontWeight: FontWeight.w400,
        height: 1.35,
        letterSpacing: -0.2,
        textBaseline: TextBaseline.alphabetic
    );
}

public static partial class DialogLibrary
{
    internal static TextStyle _kCupertinoDialogActionStyle = new TextStyle(
        fontFamily: "CupertinoSystemText",
        inherit: false,
        fontSize: 16.8,
        fontWeight: FontWeight.w400,
        textBaseline: TextBaseline.alphabetic
    );
}

public static partial class DialogLibrary
{
    internal static TextStyle _kActionSheetActionStyle = new TextStyle(
        fontFamily: "CupertinoSystemDisplay",
        inherit: false,
        fontSize: 17.0,
        fontWeight: FontWeight.w400,
        textBaseline: TextBaseline.alphabetic
    );
}

public static partial class DialogLibrary
{
    internal static TextStyle _kActionSheetContentStyle = new TextStyle(
        fontFamily: "CupertinoSystemText",
        inherit: false,
        fontSize: 13.0,
        fontWeight: FontWeight.w400,
        textBaseline: TextBaseline.alphabetic
    );
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
    internal static Color _kDialogColor = new CupertinoDynamicColor(
        color: new Color(3438473970L),
        darkColor: new Color(3425512749L)
    );
}

public static partial class DialogLibrary
{
    internal static Color _kDialogPressedColor = new CupertinoDynamicColor(
        color: new Color(4292993505L),
        darkColor: new Color(4282400832L)
    );
}

public static partial class DialogLibrary
{
    internal static Color _kActionSheetPressedColor = new CupertinoDynamicColor(
        color: new Color(3403735264L),
        darkColor: new Color(3243331921L)
    );
}

public static partial class DialogLibrary
{
    internal static Color _kActionSheetCancelColor = new CupertinoDynamicColor(
        color: new Color(4294967295L),
        darkColor: new Color(4281084972L)
    );
}

public static partial class DialogLibrary
{
    internal static Color _kActionSheetCancelPressedColor = new CupertinoDynamicColor(
        color: new Color(4293717228L),
        darkColor: new Color(4282992969L)
    );
}

public static partial class DialogLibrary
{
    internal static Color _kActionSheetBackgroundColor = new CupertinoDynamicColor(
        color: new Color(3372023036L),
        darkColor: new Color(3190368553L)
    );
}

public static partial class DialogLibrary
{
    internal static Color _kActionSheetContentTextColor = new CupertinoDynamicColor(
        color: new Color(2233277725L),
        darkColor: new Color(2532438513L)
    );
}

public static partial class DialogLibrary
{
    internal static Color _kActionSheetButtonDividerColor = new CupertinoDynamicColor(
        color: new Color(3569994185L),
        darkColor: new Color(3581771133L)
    );
}

public static partial class DialogLibrary
{
    internal static double _kMaxRegularTextScaleFactor = 1.4;
}

public static partial class DialogLibrary
{
    internal static bool _isInAccessibilityMode(BuildContext context)
    {
        var defaultFontSize = 14.0;
        double? scaledFontSize = MediaQuery.maybeTextScalerOf(context)?.scale(defaultFontSize);
        return (scaledFontSize is not null)
            && (
                DartRuntimePrimitives.RequireValue(scaledFontSize)
                > (defaultFontSize * _kMaxRegularTextScaleFactor)
            );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class CupertinoAlertDialog : StatefulWidget
{
    public virtual Widget? title { get; private set; }
    public virtual Widget? content { get; private set; }
    public virtual List<Widget> actions { get; private set; } = default!;
    public virtual ScrollController? scrollController { get; private set; }
    public virtual ScrollController? actionScrollController { get; private set; }
    public virtual Duration insetAnimationDuration { get; private set; } = default!;
    public virtual Curve insetAnimationCurve { get; private set; } = default!;

    public CupertinoAlertDialog(
        Key? key = null,
        Widget? title = null,
        Widget? content = null,
        List<Widget> actions = default!,
        ScrollController? scrollController = null,
        ScrollController? actionScrollController = null,
        Duration? insetAnimationDuration = null,
        Curve insetAnimationCurve = default!
    )
        : base(key: key)
    {
        List<Widget> __actions = actions ?? new List<Widget>();
        Duration __insetAnimationDuration =
            insetAnimationDuration ?? Duration.Create(milliseconds: 100);
        Curve __insetAnimationCurve = insetAnimationCurve ?? Curves.decelerate;
        this.title = title;
        this.content = content;
        this.actions = __actions;
        this.scrollController = scrollController;
        this.actionScrollController = actionScrollController;
        this.insetAnimationDuration = __insetAnimationDuration;
        this.insetAnimationCurve = __insetAnimationCurve;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoAlertDialogState__dialog());
}

internal class _CupertinoAlertDialogState__dialog : State<CupertinoAlertDialog>
{
    internal virtual long? _pressedIndex { get; set; } = default;
    internal virtual ScrollController? _backupScrollController { get; set; } = default;
    internal virtual ScrollController? _backupActionScrollController { get; set; } = default;

    internal virtual ScrollController _effectiveScrollController =>
        DartRuntimePrimitives.ConvertValue<ScrollController>(
            widget.scrollController ?? (_backupScrollController ??= new ScrollController())
        );
    internal virtual ScrollController _effectiveActionScrollController =>
        DartRuntimePrimitives.ConvertValue<ScrollController>(
            widget.actionScrollController
                ?? (_backupActionScrollController ??= new ScrollController())
        );

    internal virtual Widget? _buildContent(BuildContext context)
    {
        bool hasContent = (widget.title is not null) || (widget.content is not null);
        if (!hasContent)
        {
            return null;
        }
        var defaultFontSize = 14.0;
        double effectiveTextScaleFactor =
            MediaQuery.textScalerOf(context).scale(defaultFontSize) / defaultFontSize;
        Widget childLocal = new _CupertinoAlertContentSection__dialog(
            title: widget.title,
            message: widget.content,
            scrollController: _effectiveScrollController,
            titlePadding: EdgeInsets.CreateOnly(
                left: DialogLibrary._kDialogEdgePadding,
                right: DialogLibrary._kDialogEdgePadding,
                bottom: (widget.content is null) ? DialogLibrary._kDialogEdgePadding : 1.0,
                top: DialogLibrary._kDialogEdgePadding * effectiveTextScaleFactor
            ),
            messagePadding: EdgeInsets.CreateOnly(
                left: DialogLibrary._kDialogEdgePadding,
                right: DialogLibrary._kDialogEdgePadding,
                bottom: DialogLibrary._kDialogEdgePadding * effectiveTextScaleFactor,
                top: (widget.title is null) ? DialogLibrary._kDialogEdgePadding : 1.0
            ),
            titleTextStyle: DialogLibrary._kCupertinoDialogTitleStyle.copyWith(
                color: CupertinoDynamicColor.resolve(CupertinoColors.label, context)
            ),
            messageTextStyle: DialogLibrary._kCupertinoDialogContentStyle.copyWith(
                color: CupertinoDynamicColor.resolve(CupertinoColors.label, context)
            )
        );
        return (Widget?)
            new ColoredBox(
                color: CupertinoDynamicColor.resolve(DialogLibrary._kDialogColor, context),
                child: childLocal
            );
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

    internal virtual Widget? _buildActions()
    {
        if (!Enumerable.Any(widget.actions))
        {
            return null;
        }
        else
        {
            return (Widget?)
                new _CupertinoAlertActionSection__dialog(
                    scrollController: _effectiveActionScrollController,
                    actions: widget.actions,
                    pressedIndex: _pressedIndex,
                    onPressedUpdate: _onPressedUpdate
                );
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildBody(BuildContext context)
    {
        Color backgroundColor = CupertinoDynamicColor.resolve(DialogLibrary._kDialogColor, context);
        Color dividerColorLocal = CupertinoDynamicColor.resolve(CupertinoColors.separator, context);
        return MediaQuery.CreateRemovePadding(
            removeLeft: true,
            removeTop: true,
            removeRight: true,
            removeBottom: true,
            context: context,
            child: new LayoutBuilder(
                builder: (context, constraints) =>
                {
                    Widget? contentSection = _buildContent(context);
                    Widget? actionsSection = _buildActions();
                    if (actionsSection is null)
                    {
                        return contentSection
                            ?? new LimitedBox(
                                maxWidth: 0,
                                child: new SizedBox(width: double.PositiveInfinity, height: 0)
                            );
                    }
                    Widget scrolledActionsSection = new _OverscrollBackground__dialog(
                        color: backgroundColor,
                        child: actionsSection
                    );
                    if (contentSection is null)
                    {
                        return scrolledActionsSection;
                    }
                    double actionsMinHeight = DialogLibrary._isInAccessibilityMode(context)
                        ? ((constraints.maxHeight / 2L) + DialogLibrary._kDividerThickness)
                        : (
                            DialogLibrary._kDialogActionsSectionMinHeight
                            + DialogLibrary._kDividerThickness
                        );
                    return new _PriorityColumn__dialog(
                        top: contentSection,
                        bottom: new Column(
                            children: new List<Widget>
                            {
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    new SizedBox(
                                        width: double.PositiveInfinity,
                                        child: new _Divider__dialog(
                                            dividerColor: dividerColorLocal,
                                            hiddenColor: backgroundColor,
                                            hidden: false
                                        )
                                    )
                                ),
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    new Flexible(child: scrolledActionsSection)
                                ),
                            }
                        ),
                        bottomMinHeight: actionsMinHeight
                    );
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        CupertinoLocalizations localizations = CupertinoLocalizations.of(context);
        bool isInAccessibilityMode = DialogLibrary._isInAccessibilityMode(context);
        return new CupertinoUserInterfaceLevel(
            data: CupertinoUserInterfaceLevelData.elevated,
            child: MediaQuery.withClampedTextScaling(
                minScaleFactor: 1.0,
                child: new ScrollConfiguration(
                    behavior: ScrollConfiguration.of(context).copyWith(scrollbars: false),
                    child: new LayoutBuilder(
                        builder: (context, constraints) =>
                        {
                            return new AnimatedPadding(
                                padding: MediaQuery
                                    .viewInsetsOf(context)
                                    .op_Add(
                                        EdgeInsets.CreateSymmetric(horizontal: 40.0, vertical: 24.0)
                                    ),
                                duration: widget.insetAnimationDuration,
                                curve: widget.insetAnimationCurve,
                                child: MediaQuery.CreateRemoveViewInsets(
                                    removeLeft: true,
                                    removeTop: true,
                                    removeRight: true,
                                    removeBottom: true,
                                    context: context,
                                    child: new Center(
                                        child: new Padding(
                                            padding: EdgeInsets.CreateSymmetric(
                                                vertical: DialogLibrary._kDialogEdgePadding
                                            ),
                                            child: new SizedBox(
                                                width: isInAccessibilityMode
                                                    ? DialogLibrary._kAccessibilityCupertinoDialogWidth
                                                    : DialogLibrary._kCupertinoDialogWidth,
                                                child: new _ActionSheetGestureDetector__dialog(
                                                    child: new CupertinoPopupSurface(
                                                        isSurfacePainted: false,
                                                        child: new Widgets.Semantics(
                                                            role: SemanticsRole.alertDialog,
                                                            namesRoute: true,
                                                            scopesRoute: true,
                                                            explicitChildNodes: true,
                                                            label: localizations.alertDialogLabel,
                                                            child: _buildBody(context)
                                                        )
                                                    )
                                                )
                                            )
                                        )
                                    )
                                )
                            );
                            throw new InvalidOperationException(
                                "Dart closure completed without a value."
                            );
                        }
                    )
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        _backupScrollController?.dispose();
        _backupActionScrollController?.dispose();
        base.dispose();
    }
}

public class CupertinoPopupSurface : StatelessWidget
{
    public virtual double blurSigma { get; private set; } = default!;
    public virtual bool isSurfacePainted { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;
    public const double defaultBlurSigma = 30.0;
    internal static BorderRadius _clipper = BorderRadius.CreateAll(Radius.circular(13));
    internal static List<double> _lightSaturationMatrix = new List<double>
    {
        1.74,
        -0.4,
        -0.17,
        0.0,
        0.0,
        -0.26,
        1.6,
        -0.17,
        0.0,
        0.0,
        -0.26,
        -0.4,
        1.83,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        1.0,
        0.0,
    };
    internal static List<double> _darkSaturationMatrix = new List<double>
    {
        1.39,
        -0.56,
        -0.11,
        0.0,
        0.3,
        -0.32,
        1.14,
        -0.11,
        0.0,
        0.3,
        -0.32,
        -0.56,
        1.59,
        0.0,
        0.3,
        0.0,
        0.0,
        0.0,
        1.0,
        0.0,
    };
    public static bool debugIsVibrancePainted = true;

    public CupertinoPopupSurface(
        Key? key = null,
        double? blurSigma = null,
        bool isSurfacePainted = true,
        Widget child = default!
    )
        : base(key: key)
    {
        double __blurSigma = blurSigma ?? defaultBlurSigma;
        this.blurSigma = __blurSigma;
        this.isSurfacePainted = isSurfacePainted;
        this.child = child;
        System.Diagnostics.Debug.Assert(__blurSigma >= 0L);
    }

    internal virtual ImageFilterConfig? _buildFilter(Brightness? brightness)
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
            return ImageFilterConfig.CreateBlur(
                sigmaX: DartRuntimePrimitives.RequireValue(blurSigma),
                sigmaY: DartRuntimePrimitives.RequireValue(blurSigma)
            );
        }
        var colorFilter = ImageFilterConfig.Create(
            brightness switch
            {
                Brightness.dark => ColorFilter.matrix(_darkSaturationMatrix),
                Brightness.light => ColorFilter.matrix(_lightSaturationMatrix),
                null => ColorFilter.matrix(_lightSaturationMatrix),
                _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
            }
        );
        if (blurSigma == 0L)
        {
            return colorFilter;
        }
        return ImageFilterConfig.CreateCompose(
            inner: colorFilter,
            outer: ImageFilterConfig.CreateBlur(
                sigmaX: DartRuntimePrimitives.RequireValue(blurSigma),
                sigmaY: DartRuntimePrimitives.RequireValue(blurSigma)
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        ImageFilterConfig? filter = _buildFilter(CupertinoTheme.maybeBrightnessOf(context));
        Widget contents = child;
        if (isSurfacePainted)
        {
            contents = DartRuntimePrimitives.ConvertValue<Widget>(
                new ColoredBox(
                    color: CupertinoDynamicColor.resolve(DialogLibrary._kDialogColor, context),
                    child: contents
                )
            );
        }
        if (filter is not null)
        {
            return new ClipRSuperellipse(
                borderRadius: _clipper,
                child: new BackdropFilter(filterConfig: filter, child: contents)
            );
        }
        return new ClipRSuperellipse(borderRadius: _clipper, child: contents);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal delegate Gestures.HitTestResult _HitTester__dialog(Offset location);

internal class _SlidingTapGestureRecognizer__dialog : Gestures.VerticalDragGestureRecognizer
{
    public virtual Action<Offset>? onResponsiveUpdate { get; set; } = default;
    public virtual Action<Offset>? onResponsiveEnd { get; set; } = default;
    internal virtual long? _primaryPointer { get; set; } = default;

    internal _SlidingTapGestureRecognizer__dialog(object? debugOwner = null)
        : base(debugOwner: debugOwner)
    {
        dragStartBehavior = Gestures.DragStartBehavior.down;
    }

    public override void addAllowedPointer(Gestures.PointerDownEvent @event)
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

    public override void handleEvent(Gestures.PointerEvent @event)
    {
        if (@event.pointer == _primaryPointer)
        {
            if (@event is Gestures.PointerMoveEvent)
            {
                Gestures.PointerMoveEvent @event__as28218 = (Gestures.PointerMoveEvent)@event;
                onResponsiveUpdate?.Invoke(@event__as28218.position);
            }
            if (@event is Gestures.PointerUpEvent)
            {
                Gestures.PointerUpEvent @event__as29359 = (Gestures.PointerUpEvent)@event;
                stopTrackingPointer(DartRuntimePrimitives.RequireValue(_primaryPointer));
                onResponsiveEnd?.Invoke(@event__as29359.position);
                _primaryPointer = null;
                return;
            }
            if (@event is Gestures.PointerCancelEvent)
            {
                Gestures.PointerCancelEvent @event__as29658 = (Gestures.PointerCancelEvent)@event;
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

internal class _TargetSelectionGestureRecognizer__dialog : Gestures.GestureRecognizer
{
    public virtual Func<Offset, Gestures.HitTestResult> hitTest { get; private set; } = default!;
    internal virtual List<_SlideTarget__dialog> _currentTargets { get; private set; } =
        new List<_SlideTarget__dialog>();
    internal virtual _SlidingTapGestureRecognizer__dialog _slidingTap { get; private set; } =
        default!;

    internal _TargetSelectionGestureRecognizer__dialog(
        object? debugOwner = null,
        Func<Offset, Gestures.HitTestResult> hitTest = default!
    )
        : base(debugOwner: debugOwner)
    {
        this.hitTest = hitTest;
        _slidingTap = new _SlidingTapGestureRecognizer__dialog(debugOwner: debugOwner);
        DartRuntimePrimitives.Ignore(
            (
                (Func<_SlidingTapGestureRecognizer__dialog>)(
                    () =>
                    {
                        var __cascade = _slidingTap;
                        __cascade.onDown = _onDown;
                        __cascade.onResponsiveUpdate = _onUpdate;
                        __cascade.onResponsiveEnd = _onEnd;
                        __cascade.onCancel = _onCancel;
                        return __cascade;
                    }
                )
            )()
        );
    }

    public override void acceptGesture(long pointer)
    {
        _slidingTap.acceptGesture(pointer);
    }

    public override void rejectGesture(long pointer)
    {
        _slidingTap.rejectGesture(pointer);
    }

    public override void addPointer(Gestures.PointerDownEvent @event)
    {
        _slidingTap.addPointer(@event);
    }

    public override void addPointerPanZoom(Gestures.PointerPanZoomStartEvent @event)
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
        Gestures.HitTestResult result = hitTest(pointerPosition);
        var foundTargets = new List<_SlideTarget__dialog>();
        foreach (Gestures.HitTestEntry<Gestures.HitTestTarget> entry in result.path)
        {
            if (entry.target is RenderMetaData targetLocal)
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
            DartRuntimePrimitives.Ignore(
                (
                    (Func<List<_SlideTarget__dialog>>)(
                        () =>
                        {
                            var __cascade = _currentTargets;
                            __cascade.Clear();
                            __cascade.AddRange(foundTargets.Cast<_SlideTarget__dialog>());
                            return __cascade;
                        }
                    )
                )()
            );
            var enabled = true;
            foreach (_SlideTarget__dialog targetNested in _currentTargets)
            {
                enabled = targetNested.didEnter(
                    fromPointerDown: fromPointerDown,
                    innerEnabled: enabled
                );
            }
        }
    }

    internal virtual void _onDown(Gestures.DragDownDetails details)
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

internal class _ActionSheetGestureDetector__dialog : StatelessWidget
{
    public virtual Widget? child { get; private set; }

    internal _ActionSheetGestureDetector__dialog(Widget? child = null)
    {
        this.child = child;
    }

    internal virtual Gestures.HitTestResult _hitTest(BuildContext context, Offset globalPosition)
    {
        long viewIdLocal = checked((long)View.of(context).viewId);
        var result = new Gestures.HitTestResult();
        WidgetsBinding.instance.hitTestInView(result, globalPosition, viewIdLocal);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        var gesturesLocal = new DartMap<Type, dynamic>();
        gesturesLocal[typeof(_TargetSelectionGestureRecognizer__dialog)] =
            new GestureRecognizerFactoryWithHandlers<_TargetSelectionGestureRecognizer__dialog>(
                () =>
                    new _TargetSelectionGestureRecognizer__dialog(
                        debugOwner: this,
                        hitTest: (globalPosition) => _hitTest(context, globalPosition)
                    ),
                (instance) => { }
            );
        return new RawGestureDetector(
            excludeFromSemantics: true,
            gestures: gesturesLocal,
            child: child
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class CupertinoActionSheet : StatefulWidget
{
    public virtual Widget? title { get; private set; }
    public virtual Widget? message { get; private set; }
    public virtual List<Widget>? actions { get; private set; }
    public virtual ScrollController? messageScrollController { get; private set; }
    public virtual ScrollController? actionScrollController { get; private set; }
    public virtual Widget? cancelButton { get; private set; }

    public CupertinoActionSheet(
        Key? key = null,
        Widget? title = null,
        Widget? message = null,
        List<Widget>? actions = null,
        ScrollController? messageScrollController = null,
        ScrollController? actionScrollController = null,
        Widget? cancelButton = null
    )
        : base(key: key)
    {
        this.title = title;
        this.message = message;
        this.actions = actions;
        this.messageScrollController = messageScrollController;
        this.actionScrollController = actionScrollController;
        this.cancelButton = cancelButton;
        System.Diagnostics.Debug.Assert(
            (actions is not null)
                || (title is not null)
                || (message is not null)
                || (cancelButton is not null)
        );
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoActionSheetState__dialog());
}

internal class _CupertinoActionSheetState__dialog : State<CupertinoActionSheet>
{
    internal virtual long? _pressedIndex { get; set; } = default;
    internal static long _kCancelButtonIndex = -1L;
    internal virtual ScrollController? _backupMessageScrollController { get; set; } = default;
    internal virtual ScrollController? _backupActionScrollController { get; set; } = default;

    internal virtual ScrollController _effectiveMessageScrollController =>
        DartRuntimePrimitives.ConvertValue<ScrollController>(
            widget.messageScrollController
                ?? (_backupMessageScrollController ??= new ScrollController())
        );
    internal virtual ScrollController _effectiveActionScrollController =>
        DartRuntimePrimitives.ConvertValue<ScrollController>(
            widget.actionScrollController
                ?? (_backupActionScrollController ??= new ScrollController())
        );

    public override void dispose()
    {
        _backupMessageScrollController?.dispose();
        _backupActionScrollController?.dispose();
        base.dispose();
    }

    public virtual bool hasContent =>
        DartRuntimePrimitives.ConvertValue<bool>(
            (widget.title is not null) || (widget.message is not null)
        );

    internal virtual Widget? _buildContent(BuildContext context)
    {
        if (!hasContent)
        {
            return null;
        }
        TextStyle textStyle = DialogLibrary._kActionSheetContentStyle.copyWith(
            color: CupertinoDynamicColor.resolve(
                DialogLibrary._kActionSheetContentTextColor,
                context
            )
        );
        return (Widget?)
            new ColoredBox(
                color: CupertinoDynamicColor.resolve(
                    DialogLibrary._kActionSheetBackgroundColor,
                    context
                ),
                child: new _CupertinoAlertContentSection__dialog(
                    title: widget.title,
                    message: widget.message,
                    scrollController: _effectiveMessageScrollController,
                    titlePadding: EdgeInsets.CreateOnly(
                        left: DialogLibrary._kActionSheetContentHorizontalPadding,
                        right: DialogLibrary._kActionSheetContentHorizontalPadding,
                        bottom: (widget.message is null)
                            ? DialogLibrary._kActionSheetContentVerticalPadding
                            : 0.0,
                        top: DialogLibrary._kActionSheetContentVerticalPadding
                    ),
                    messagePadding: EdgeInsets.CreateOnly(
                        left: DialogLibrary._kActionSheetContentHorizontalPadding,
                        right: DialogLibrary._kActionSheetContentHorizontalPadding,
                        bottom: DialogLibrary._kActionSheetContentVerticalPadding,
                        top: (widget.title is null)
                            ? DialogLibrary._kActionSheetContentVerticalPadding
                            : 0.0
                    ),
                    titleTextStyle: (widget.message is null)
                        ? textStyle
                        : textStyle.copyWith(fontWeight: FontWeight.w600),
                    messageTextStyle: (widget.title is null)
                        ? textStyle.copyWith(fontWeight: FontWeight.w600)
                        : textStyle,
                    additionalPaddingBetweenTitleAndMessage: EdgeInsets.CreateOnly(top: 4.0)
                )
            );
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

    internal virtual Widget _buildCancelButton()
    {
        DartRuntimePrimitives.Assert(() => widget.cancelButton is not null);
        double cancelPadding =
            (
                (widget.actions is not null)
                || (widget.message is not null)
                || (widget.title is not null)
            )
                ? DialogLibrary._kActionSheetCancelButtonPadding
                : 0.0;
        return new Padding(
            padding: EdgeInsets.CreateOnly(top: cancelPadding),
            child: CupertinoFocusHalo.CreateWithRRect(
                borderRadius: ConstantsLibrary.kCupertinoButtonSizeBorderRadius.GetValueOrDefault(
                    CupertinoButtonSize.large
                )!,
                child: new _ActionSheetButtonBackground__dialog(
                    isCancel: true,
                    pressed: _pressedIndex == _kCancelButtonIndex,
                    onPressStateChange: (state) =>
                    {
                        _onPressedUpdate(_kCancelButtonIndex, state);
                    },
                    child: widget.cancelButton!
                )
            )
        );
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
                return DartRuntimePrimitives.RequireValue(
                    Dart_uiLibrary.lerpDouble(y1, y2, (x - x1) / (x2 - x1))
                );
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _topPadding(BuildContext context)
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
        double currentPaddingRatio = _lerp(
            currentViewPadding,
            viewPaddingData1,
            paddingRatioData1,
            viewPaddingData2,
            paddingRatioData2
        );
        double padding = (currentPaddingRatio * currentViewPadding).roundToDouble();
        return Math.Max(padding, DialogLibrary._kDialogEdgePadding);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        var childrenLocal = (
            (Func<List<Widget>>)(
                () =>
                {
                    var __collection47257 = new List<Widget>();
                    __collection47257.Add(
                        DartRuntimePrimitives.ConvertValue<Widget>(
                            new Flexible(
                                child: new ClipRSuperellipse(
                                    borderRadius: BorderRadius.CreateAll(Radius.circular(12.0)),
                                    child: new BackdropFilter(
                                        filter: new ImageFilter(
                                            sigmaX: CupertinoPopupSurface.defaultBlurSigma,
                                            sigmaY: CupertinoPopupSurface.defaultBlurSigma
                                        ),
                                        child: new _ActionSheetMainSheet__dialog(
                                            pressedIndex: _pressedIndex,
                                            onPressedUpdate: _onPressedUpdate,
                                            scrollController: _effectiveActionScrollController,
                                            contentSection: _buildContent(context),
                                            actions: widget.actions ?? new List<Widget>(),
                                            dividerColor: CupertinoDynamicColor.resolve(
                                                DialogLibrary._kActionSheetButtonDividerColor,
                                                context
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    );
                    if (widget.cancelButton is not null)
                    {
                        __collection47257.Add(
                            DartRuntimePrimitives.ConvertValue<Widget>(_buildCancelButton())
                        );
                    }
                    return __collection47257;
                }
            )
        )();
        double actionSheetWidth = MediaQuery.orientationOf(context) switch
        {
            Orientation.portrait => MediaQuery.widthOf(context),
            Orientation.landscape => MediaQuery.heightOf(context),
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        return new SafeArea(
            minimum: EdgeInsets.CreateOnly(bottom: DialogLibrary._kActionSheetEdgePadding),
            child: new ScrollConfiguration(
                behavior: ScrollConfiguration.of(context).copyWith(scrollbars: false),
                child: new Widgets.Semantics(
                    namesRoute: true,
                    scopesRoute: true,
                    explicitChildNodes: true,
                    role: SemanticsRole.dialog,
                    label: "Alert",
                    child: new CupertinoUserInterfaceLevel(
                        data: CupertinoUserInterfaceLevelData.elevated,
                        child: new Padding(
                            padding: EdgeInsets.CreateOnly(
                                left: DialogLibrary._kActionSheetEdgePadding,
                                right: DialogLibrary._kActionSheetEdgePadding,
                                top: _topPadding(context)
                            ),
                            child: new SizedBox(
                                width: actionSheetWidth
                                    - (DialogLibrary._kActionSheetEdgePadding * 2L),
                                child: new _ActionSheetGestureDetector__dialog(
                                    child: new Widgets.Semantics(
                                        explicitChildNodes: true,
                                        child: new Column(
                                            mainAxisAlignment: MainAxisAlignment.end,
                                            mainAxisSize: MainAxisSize.min,
                                            crossAxisAlignment: CrossAxisAlignment.stretch,
                                            children: childrenLocal
                                        )
                                    )
                                )
                            )
                        )
                    )
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class CupertinoActionSheetAction : StatefulWidget
{
    public virtual Action onPressed { get; private set; } = default!;
    public virtual bool isDefaultAction { get; private set; } = default!;
    public virtual bool isDestructiveAction { get; private set; } = default!;
    public virtual MouseCursor? mouseCursor { get; private set; }
    public virtual FocusNode? focusNode { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public CupertinoActionSheetAction(
        Key? key = null,
        Action onPressed = default!,
        bool isDefaultAction = false,
        bool isDestructiveAction = false,
        MouseCursor? mouseCursor = null,
        FocusNode? focusNode = null,
        Color? focusColor = null,
        Widget child = default!
    )
        : base(key: key)
    {
        this.onPressed = onPressed;
        this.isDefaultAction = isDefaultAction;
        this.isDestructiveAction = isDestructiveAction;
        this.mouseCursor = mouseCursor;
        this.focusNode = focusNode;
        this.focusColor = focusColor;
        this.child = child;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoActionSheetActionState__dialog());
}

internal class _CupertinoActionSheetActionState__dialog
    : State<CupertinoActionSheetAction>,
        _SlideTarget__dialog
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
                __late__actionMap = new DartMap<Type, dynamic>
                {
                    [typeof(ActivateIntent)] = new CallbackAction<ActivateIntent>(
                        onInvoke: _handleTap
                    ),
                };
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

    public virtual void didLeave() { }

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

    internal virtual void _handleTap(Intent? __unused0 = null)
    {
        widget.onPressed();
        context.findRenderObject()!.sendSemanticsEvent(new Semantics.TapSemanticEvent());
    }

    public virtual Color effectiveFocusBackgroundColor =>
        DartRuntimePrimitives.ConvertValue<Color>(
            HSLColor
                .CreateFromColor(
                    (widget.focusColor ?? CupertinoColors.activeBlue).withOpacity(
                        Equals(CupertinoTheme.brightnessOf(context), Brightness.light)
                            ? ConstantsLibrary.kCupertinoButtonTintedOpacityLight
                            : ConstantsLibrary.kCupertinoButtonTintedOpacityDark
                    )
                )
                .toColor()
        );

    public override Widget build(BuildContext context)
    {
        return new MouseRegion(
            cursor: widget.mouseCursor
                ?? (
                    Foundation.ConstantsLibrary.kIsWeb
                        ? SystemMouseCursors.click
                        : MouseCursor.defer
                ),
            child: new MetaData(
                metaData: this,
                behavior: HitTestBehavior.opaque,
                child: new ConstrainedBox(
                    constraints: new BoxConstraints(
                        minHeight: DialogLibrary._kActionSheetButtonMinHeight
                    ),
                    child: new FocusableActionDetector(
                        actions: _actionMap,
                        focusNode: widget.focusNode,
                        onShowFocusHighlight: _onShowFocusHighlight,
                        child: new Widgets.Semantics(
                            button: true,
                            onTap: () => widget.onPressed(),
                            child: _showHighlight
                                ? new global::Doroti.Framework.Widgets.DecoratedBox(
                                    decoration: new global::Doroti.Framework.Painting.BoxDecoration(
                                        color: effectiveFocusBackgroundColor
                                    ),
                                    child: new _ActionSheetActionContent__dialog(
                                        isDestructiveAction: widget.isDestructiveAction,
                                        isDefaultAction: widget.isDefaultAction,
                                        child: widget.child
                                    )
                                )
                                : new _ActionSheetActionContent__dialog(
                                    isDestructiveAction: widget.isDestructiveAction,
                                    isDefaultAction: widget.isDefaultAction,
                                    child: widget.child
                                )
                        )
                    )
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _ActionSheetActionContent__dialog : StatelessWidget
{
    public virtual bool isDestructiveAction { get; private set; } = default!;
    public virtual bool isDefaultAction { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    internal _ActionSheetActionContent__dialog(
        bool isDestructiveAction,
        bool isDefaultAction,
        Widget child
    )
    {
        this.isDestructiveAction = isDestructiveAction;
        this.isDefaultAction = isDefaultAction;
        this.child = child;
    }

    internal static double _buttonFontSize(double contextBodySize)
    {
        return contextBodySize switch
        {
            <= 17L => 21.0,
            <= 19L => DartRuntimePrimitives.RequireValue(
                Dart_uiLibrary.lerpDouble(21.0, 23.0, (contextBodySize - 17.0) / (19.0 - 17.0))
            ),
            <= 21L => DartRuntimePrimitives.RequireValue(
                Dart_uiLibrary.lerpDouble(23.0, 24.0, (contextBodySize - 19.0) / (21.0 - 19.0))
            ),
            <= 24L => 24.0,
            _ => contextBodySize,
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        var higLargeBodySize = 17.0;
        double contextBodySize = MediaQuery.textScalerOf(context).scale(higLargeBodySize);
        double contextScaleFactor = contextBodySize / higLargeBodySize;
        double fontSizeLocal = _buttonFontSize(contextBodySize);
        TextStyle styleLocal = DialogLibrary._kActionSheetActionStyle.copyWith(
            fontSize: fontSizeLocal / contextScaleFactor,
            color: isDestructiveAction
                ? CupertinoDynamicColor.resolve(CupertinoColors.systemRed, context)
                : CupertinoTheme.of(context).primaryColor
        );
        if (isDefaultAction)
        {
            styleLocal = styleLocal.copyWith(fontWeight: FontWeight.w600);
        }
        double verticalPadding =
            DialogLibrary._kActionSheetButtonVerticalPaddingBase
            + (fontSizeLocal * DialogLibrary._kActionSheetButtonVerticalPaddingFactor);
        return new Padding(
            padding: new EdgeInsets(
                DialogLibrary._kActionSheetButtonHorizontalPadding,
                verticalPadding,
                DialogLibrary._kActionSheetButtonHorizontalPadding,
                verticalPadding
            ),
            child: new DefaultTextStyle(
                style: styleLocal,
                textAlign: TextAlign.center,
                child: new Center(child: child)
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _ActionSheetButtonBackground__dialog : StatefulWidget
{
    public virtual bool isCancel { get; private set; } = default!;
    public virtual bool pressed { get; private set; } = default!;
    public virtual Action<bool>? onPressStateChange { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    internal _ActionSheetButtonBackground__dialog(
        bool isCancel = false,
        bool pressed = default!,
        Action<bool>? onPressStateChange = null,
        Widget child = default!
    )
    {
        this.isCancel = isCancel;
        this.pressed = pressed;
        this.onPressStateChange = onPressStateChange;
        this.child = child;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _ActionSheetButtonBackgroundState__dialog());
}

internal class _ActionSheetButtonBackgroundState__dialog
    : State<_ActionSheetButtonBackground__dialog>,
        _SlideTarget__dialog
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

    public override Widget build(BuildContext context)
    {
        Widget childLocal = default!;
        if (!widget.isCancel)
        {
            childLocal = DartRuntimePrimitives.ConvertValue<Widget>(
                new ColoredBox(
                    color: CupertinoDynamicColor.resolve(
                        widget.pressed
                            ? DialogLibrary._kActionSheetPressedColor
                            : DialogLibrary._kActionSheetBackgroundColor,
                        context
                    ),
                    child: widget.child
                )
            );
        }
        else
        {
            var borderRadiusLocal = BorderRadius.CreateAll(
                Radius.circular(DialogLibrary._kCornerRadius)
            );
            childLocal = DartRuntimePrimitives.ConvertValue<Widget>(
                new ClipRSuperellipse(
                    borderRadius: borderRadiusLocal,
                    child: new DecoratedBox(
                        decoration: new BoxDecoration(
                            color: CupertinoDynamicColor.resolve(
                                widget.pressed
                                    ? DialogLibrary._kActionSheetCancelPressedColor
                                    : DialogLibrary._kActionSheetCancelColor,
                                context
                            )
                        ),
                        child: widget.child
                    )
                )
            );
        }
        return new MetaData(metaData: this, child: childLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _Divider__dialog : StatelessWidget
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

    public override Widget build(BuildContext context)
    {
        return new LimitedBox(
            maxHeight: DialogLibrary._kDividerThickness,
            maxWidth: DialogLibrary._kDividerThickness,
            child: new ConstrainedBox(
                constraints: new BoxConstraints(
                    minHeight: DialogLibrary._kDividerThickness,
                    minWidth: DialogLibrary._kDividerThickness
                ),
                child: new DecoratedBox(
                    decoration: new BoxDecoration(
                        color: hidden
                            ? CupertinoDynamicColor.resolve(hiddenColor, context)
                            : dividerColor
                    )
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _OverscrollBackground__dialog : StatefulWidget
{
    public virtual Color color { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    internal _OverscrollBackground__dialog(Color color, Widget child)
    {
        this.color = color;
        this.child = child;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _OverscrollBackgroundState__dialog());
}

internal class _OverscrollBackgroundState__dialog : State<_OverscrollBackground__dialog>
{
    internal virtual double _topOverscroll { get; set; } = 0;
    internal virtual double _bottomOverscroll { get; set; } = 0;

    internal virtual bool _onScrollUpdate(ScrollUpdateNotification notification)
    {
        ScrollMetrics metricsLocal = notification.metrics;
        setState(() =>
        {
            _topOverscroll = Math.Min(
                Math.Max(metricsLocal.minScrollExtent - metricsLocal.pixels, 0),
                metricsLocal.viewportDimension
            );
            _bottomOverscroll = Math.Min(
                Math.Max(metricsLocal.pixels - metricsLocal.maxScrollExtent, 0),
                metricsLocal.viewportDimension
            );
        });
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        Widget overscroll = new Column(
            mainAxisSize: MainAxisSize.min,
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: new List<Widget>
            {
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new DecoratedBox(
                        decoration: new BoxDecoration(color: widget.color),
                        child: new SizedBox(height: _topOverscroll)
                    )
                ),
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new DecoratedBox(
                        decoration: new BoxDecoration(color: widget.color),
                        child: new SizedBox(height: _bottomOverscroll)
                    )
                ),
            }
        );
        return new Stack(
            children: new List<Widget>
            {
                DartRuntimePrimitives.ConvertValue<Widget>(
                    Positioned.CreateFill(child: overscroll)
                ),
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new NotificationListener<ScrollUpdateNotification>(
                        onNotification: _onScrollUpdate,
                        child: widget.child
                    )
                ),
            }
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal delegate void _PressedUpdateHandler__dialog(long actionIndex, bool state);

internal class _ActionSheetActionSection__dialog : StatelessWidget
{
    public virtual List<Widget>? actions { get; private set; }
    public virtual Action<long, bool> onPressedUpdate { get; private set; } = default!;
    public virtual long? pressedIndex { get; private set; }
    public virtual Color dividerColor { get; private set; } = default!;
    public virtual Color backgroundColor { get; private set; } = default!;
    public virtual ScrollController scrollController { get; private set; } = default!;

    internal _ActionSheetActionSection__dialog(
        List<Widget>? actions,
        long? pressedIndex,
        Color dividerColor,
        Color backgroundColor,
        Action<long, bool> onPressedUpdate,
        ScrollController scrollController
    )
    {
        this.actions = actions;
        this.pressedIndex = pressedIndex;
        this.dividerColor = dividerColor;
        this.backgroundColor = backgroundColor;
        this.onPressedUpdate = onPressedUpdate;
        this.scrollController = scrollController;
    }

    public override Widget build(BuildContext context)
    {
        if ((actions is null) || !Enumerable.Any(actions!))
        {
            return new LimitedBox(
                maxWidth: 0,
                child: new SizedBox(width: double.PositiveInfinity, height: 0)
            );
        }
        var column = new List<Widget>();
        for (var actionIndex = 0L; actionIndex < checked(actions!.Count); actionIndex += 1L)
        {
            if (actionIndex != 0L)
            {
                column.Add(
                    new _Divider__dialog(
                        dividerColor: dividerColor,
                        hiddenColor: DialogLibrary._kActionSheetBackgroundColor,
                        hidden: (pressedIndex == (actionIndex - 1L))
                            || (pressedIndex == actionIndex)
                    )
                );
            }
            column.Add(
                new _ActionSheetButtonBackground__dialog(
                    pressed: pressedIndex == actionIndex,
                    onPressStateChange: (state) =>
                    {
                        onPressedUpdate(actionIndex, state);
                    },
                    child: actions![(int)actionIndex]
                )
            );
        }
        return new CupertinoScrollbar(
            controller: scrollController,
            child: new SingleChildScrollView(
                controller: scrollController,
                child: new Column(crossAxisAlignment: CrossAxisAlignment.stretch, children: column)
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _ActionSheetMainSheet__dialog : StatelessWidget
{
    public virtual long? pressedIndex { get; private set; }
    public virtual Action<long, bool> onPressedUpdate { get; private set; } = default!;
    public virtual ScrollController scrollController { get; private set; } = default!;
    public virtual List<Widget> actions { get; private set; } = default!;
    public virtual Widget? contentSection { get; private set; }
    public virtual Color dividerColor { get; private set; } = default!;
    internal static Widget _empty = new LimitedBox(
        maxWidth: 0,
        child: new SizedBox(width: double.PositiveInfinity, height: 0)
    );

    internal _ActionSheetMainSheet__dialog(
        long? pressedIndex,
        Action<long, bool> onPressedUpdate,
        ScrollController scrollController,
        List<Widget> actions,
        Widget? contentSection,
        Color dividerColor
    )
    {
        this.pressedIndex = pressedIndex;
        this.onPressedUpdate = onPressedUpdate;
        this.scrollController = scrollController;
        this.actions = actions;
        this.contentSection = contentSection;
        this.dividerColor = dividerColor;
    }

    internal virtual Widget _scrolledActionsSection(BuildContext context)
    {
        Color backgroundColorLocal = CupertinoDynamicColor.resolve(
            DialogLibrary._kActionSheetBackgroundColor,
            context
        );
        return new _OverscrollBackground__dialog(
            color: backgroundColorLocal,
            child: CupertinoFocusHalo.CreateWithRRect(
                borderRadius: ConstantsLibrary
                    .kCupertinoButtonSizeBorderRadius.GetValueOrDefault(CupertinoButtonSize.large)!
                    .copyWith(topLeft: Radius.zero, topRight: Radius.zero),
                child: new _ActionSheetActionSection__dialog(
                    actions: actions,
                    scrollController: scrollController,
                    dividerColor: dividerColor,
                    backgroundColor: backgroundColorLocal,
                    pressedIndex: pressedIndex,
                    onPressedUpdate: onPressedUpdate
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _dividerAndActionsSection(BuildContext context)
    {
        Color backgroundColor = CupertinoDynamicColor.resolve(
            DialogLibrary._kActionSheetBackgroundColor,
            context
        );
        return new Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: new List<Widget>
            {
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new _Divider__dialog(
                        dividerColor: dividerColor,
                        hiddenColor: backgroundColor,
                        hidden: false
                    )
                ),
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new Flexible(child: _scrolledActionsSection(context))
                ),
            }
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        if (!Enumerable.Any(actions))
        {
            return contentSection ?? _empty;
        }
        if (contentSection is null)
        {
            return _scrolledActionsSection(context);
        }
        return new _PriorityColumn__dialog(
            top: contentSection!,
            bottom: _dividerAndActionsSection(context),
            bottomMinHeight: DialogLibrary._kActionSheetActionsSectionMinHeight
                + DialogLibrary._kDividerThickness
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _CupertinoAlertContentSection__dialog : StatelessWidget
{
    public virtual Widget? title { get; private set; }
    public virtual Widget? message { get; private set; }
    public virtual ScrollController scrollController { get; private set; } = default!;
    public virtual EdgeInsets? titlePadding { get; private set; }
    public virtual EdgeInsets? messagePadding { get; private set; }
    public virtual EdgeInsets? additionalPaddingBetweenTitleAndMessage { get; private set; }
    public virtual TextStyle? titleTextStyle { get; private set; }
    public virtual TextStyle? messageTextStyle { get; private set; }

    internal _CupertinoAlertContentSection__dialog(
        Widget? title = null,
        Widget? message = null,
        ScrollController scrollController = default!,
        EdgeInsets? titlePadding = null,
        EdgeInsets? messagePadding = null,
        TextStyle? titleTextStyle = null,
        TextStyle? messageTextStyle = null,
        EdgeInsets? additionalPaddingBetweenTitleAndMessage = null
    )
    {
        this.title = title;
        this.message = message;
        this.scrollController = scrollController;
        this.titlePadding = titlePadding;
        this.messagePadding = messagePadding;
        this.titleTextStyle = titleTextStyle;
        this.messageTextStyle = messageTextStyle;
        this.additionalPaddingBetweenTitleAndMessage = additionalPaddingBetweenTitleAndMessage;
        System.Diagnostics.Debug.Assert(
            (title is null) || ((titlePadding is not null) && (titleTextStyle is not null))
        );
        System.Diagnostics.Debug.Assert(
            (message is null) || ((messagePadding is not null) && (messageTextStyle is not null))
        );
    }

    public override Widget build(BuildContext context)
    {
        if ((title is null) && (message is null))
        {
            return new SingleChildScrollView(
                controller: scrollController,
                child: SizedBox.CreateShrink()
            );
        }
        var titleContentGroup = (
            (Func<List<Widget>>)(
                () =>
                {
                    var __collection70765 = new List<Widget>();
                    if (title is not null)
                    {
                        __collection70765.Add(
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                new Padding(
                                    padding: titlePadding!,
                                    child: new DefaultTextStyle(
                                        style: titleTextStyle!,
                                        textAlign: TextAlign.center,
                                        child: title!
                                    )
                                )
                            )
                        );
                    }
                    if (message is not null)
                    {
                        __collection70765.Add(
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                new Padding(
                                    padding: messagePadding!,
                                    child: new DefaultTextStyle(
                                        style: messageTextStyle!,
                                        textAlign: TextAlign.center,
                                        child: message!
                                    )
                                )
                            )
                        );
                    }
                    return __collection70765;
                }
            )
        )();
        if (
            (additionalPaddingBetweenTitleAndMessage is not null)
            && (checked(titleContentGroup.Count) > 1L)
        )
        {
            titleContentGroup.Insert(
                checked((int)1L),
                new Padding(padding: additionalPaddingBetweenTitleAndMessage!)
            );
        }
        return new CupertinoScrollbar(
            controller: scrollController,
            child: new SingleChildScrollView(
                controller: scrollController,
                child: new Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: titleContentGroup
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _CupertinoAlertActionSection__dialog : StatelessWidget
{
    public virtual List<Widget> actions { get; private set; } = default!;
    public virtual Action<long, bool> onPressedUpdate { get; private set; } = default!;
    public virtual long? pressedIndex { get; private set; }
    public virtual ScrollController scrollController { get; private set; } = default!;

    internal _CupertinoAlertActionSection__dialog(
        List<Widget> actions,
        Action<long, bool> onPressedUpdate,
        long? pressedIndex,
        ScrollController scrollController
    )
    {
        this.actions = actions;
        this.onPressedUpdate = onPressedUpdate;
        this.pressedIndex = pressedIndex;
        this.scrollController = scrollController;
        System.Diagnostics.Debug.Assert(checked(actions.Count) != 0L);
    }

    public override Widget build(BuildContext context)
    {
        Color dialogColor = CupertinoDynamicColor.resolve(DialogLibrary._kDialogColor, context);
        Color dialogPressedColor = CupertinoDynamicColor.resolve(
            DialogLibrary._kDialogPressedColor,
            context
        );
        Color dividerColorLocal = CupertinoDynamicColor.resolve(CupertinoColors.separator, context);
        var column = new List<Widget>();
        for (var actionIndex = 0L; actionIndex < checked(actions.Count); actionIndex += 1L)
        {
            if (actionIndex != 0L)
            {
                column.Add(
                    new _Divider__dialog(
                        dividerColor: dividerColorLocal,
                        hiddenColor: dialogColor,
                        hidden: (pressedIndex == (actionIndex - 1L))
                            || (pressedIndex == actionIndex)
                    )
                );
            }
            column.Add(
                new _AlertDialogButtonBackground__dialog(
                    idleColor: dialogColor,
                    pressedColor: dialogPressedColor,
                    pressed: pressedIndex == actionIndex,
                    onPressStateChange: (state) =>
                    {
                        onPressedUpdate(actionIndex, state);
                    },
                    child: actions[(int)actionIndex]
                )
            );
        }
        return new CupertinoScrollbar(
            controller: scrollController,
            child: new SingleChildScrollView(
                controller: scrollController,
                child: new _AlertDialogActionsLayout__dialog(
                    dividerThickness: DialogLibrary._kDividerThickness,
                    children: column
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _AlertDialogButtonBackground__dialog : StatefulWidget
{
    public virtual bool pressed { get; private set; } = default!;
    public virtual Action<bool>? onPressStateChange { get; private set; }
    public virtual Color idleColor { get; private set; } = default!;
    public virtual Color pressedColor { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    internal _AlertDialogButtonBackground__dialog(
        Color idleColor,
        Color pressedColor,
        bool pressed,
        Action<bool>? onPressStateChange,
        Widget child
    )
    {
        this.idleColor = idleColor;
        this.pressedColor = pressedColor;
        this.pressed = pressed;
        this.onPressStateChange = onPressStateChange;
        this.child = child;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _AlertDialogButtonBackgroundState__dialog());
}

internal class _AlertDialogButtonBackgroundState__dialog
    : State<_AlertDialogButtonBackground__dialog>,
        _SlideTarget__dialog
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

    public override Widget build(BuildContext context)
    {
        Color backgroundColor = widget.pressed ? widget.pressedColor : widget.idleColor;
        return new MetaData(
            metaData: this,
            child: new MergeSemantics(
                child: new Container(
                    decoration: new BoxDecoration(
                        color: CupertinoDynamicColor.resolve(backgroundColor, context)
                    ),
                    child: widget.child
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class CupertinoDialogAction : StatefulWidget
{
    public virtual Action? onPressed { get; private set; }
    public virtual bool isDefaultAction { get; private set; } = default!;
    public virtual bool isDestructiveAction { get; private set; } = default!;
    public virtual TextStyle? textStyle { get; private set; }
    public virtual MouseCursor? mouseCursor { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public CupertinoDialogAction(
        Key? key = null,
        Action? onPressed = null,
        bool isDefaultAction = false,
        bool isDestructiveAction = false,
        TextStyle? textStyle = null,
        MouseCursor? mouseCursor = null,
        Widget child = default!
    )
        : base(key: key)
    {
        this.onPressed = onPressed;
        this.isDefaultAction = isDefaultAction;
        this.isDestructiveAction = isDestructiveAction;
        this.textStyle = textStyle;
        this.mouseCursor = mouseCursor;
        this.child = child;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoDialogActionState__dialog());
}

internal class _CupertinoDialogActionState__dialog
    : State<CupertinoDialogAction>,
        _SlideTarget__dialog
{
    public virtual bool enabled =>
        DartRuntimePrimitives.ConvertValue<bool>(widget.onPressed is not null);

    public virtual bool didEnter(bool fromPointerDown, bool innerEnabled)
    {
        return enabled;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void didLeave() { }

    public virtual void didConfirm()
    {
        widget.onPressed?.Invoke();
    }

    internal virtual Widget _buildContentWithRegularSizingPolicy(
        BuildContext context,
        TextStyle textStyle,
        Widget content,
        double padding
    )
    {
        bool isInAccessibilityMode = DialogLibrary._isInAccessibilityMode(context);
        double dialogWidth = isInAccessibilityMode
            ? DialogLibrary._kAccessibilityCupertinoDialogWidth
            : DialogLibrary._kCupertinoDialogWidth;
        double fontSizeRatio =
            MediaQuery
                .textScalerOf(context)
                .scale(DartRuntimePrimitives.RequireValue(textStyle.fontSize))
            / DialogLibrary._kDialogMinButtonFontSize;
        return new FittedBox(
            fit: BoxFit.scaleDown,
            child: new ConstrainedBox(
                constraints: new BoxConstraints(
                    maxWidth: fontSizeRatio * (dialogWidth - (2L * padding))
                ),
                child: new Widgets.Semantics(
                    button: true,
                    onTap: widget.onPressed,
                    child: new DefaultTextStyle(
                        style: textStyle,
                        textAlign: TextAlign.center,
                        overflow: TextOverflow.ellipsis,
                        maxLines: 1L,
                        child: content
                    )
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildContentWithAccessibilitySizingPolicy(
        TextStyle textStyle,
        Widget content
    )
    {
        return new DefaultTextStyle(style: textStyle, textAlign: TextAlign.center, child: content);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        TextStyle style = DialogLibrary
            ._kCupertinoDialogActionStyle.copyWith(
                color: CupertinoDynamicColor.resolve(
                    widget.isDestructiveAction
                        ? CupertinoColors.systemRed
                        : CupertinoTheme.of(context).primaryColor,
                    context
                )
            )
            .merge(widget.textStyle);
        if (widget.isDefaultAction)
        {
            style = style.copyWith(fontWeight: FontWeight.w600);
        }
        if (!enabled)
        {
            style = style.copyWith(color: style.color!.withOpacity(0.5));
        }
        double fontSizeLocal = style.fontSize ?? Text_painterLibrary.kDefaultFontSize;
        double fontSizeToScale =
            (fontSizeLocal == 0.0) ? Text_painterLibrary.kDefaultFontSize : fontSizeLocal;
        double effectiveTextScale =
            MediaQuery.textScalerOf(context).scale(fontSizeToScale) / fontSizeToScale;
        double paddingLocal = 8.0 * effectiveTextScale;
        Widget sizedContent = DialogLibrary._isInAccessibilityMode(context)
            ? _buildContentWithAccessibilitySizingPolicy(textStyle: style, content: widget.child)
            : _buildContentWithRegularSizingPolicy(
                context: context,
                textStyle: style,
                content: widget.child,
                padding: paddingLocal
            );
        return new MouseRegion(
            cursor: widget.mouseCursor
                ?? (
                    (enabled && Foundation.ConstantsLibrary.kIsWeb)
                        ? SystemMouseCursors.click
                        : MouseCursor.defer
                ),
            child: new MetaData(
                metaData: this,
                behavior: HitTestBehavior.opaque,
                child: new ConstrainedBox(
                    constraints: new BoxConstraints(
                        minHeight: DialogLibrary._kDialogMinButtonHeight
                    ),
                    child: new Padding(
                        padding: EdgeInsets.CreateAll(paddingLocal),
                        child: new Center(child: sizedContent)
                    )
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _AlertDialogActionsLayout__dialog : MultiChildRenderObjectWidget
{
    internal virtual double _dividerThickness { get; private set; } = default!;

    internal _AlertDialogActionsLayout__dialog(double dividerThickness, List<Widget> children)
        : base(children: children)
    {
        _dividerThickness = dividerThickness;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderAlertDialogActionsLayout__dialog(
            dividerThickness: _dividerThickness,
            textDirection: Directionality.of(context)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderAlertDialogActionsLayout__dialog)renderObject;
        DartRuntimePrimitives.Ignore(
            (
                (Func<_RenderAlertDialogActionsLayout__dialog>)(
                    () =>
                    {
                        var __cascade = __renderObject;
                        __cascade.dividerThickness = _dividerThickness;
                        __cascade.textDirection = Directionality.of(context);
                        return __cascade;
                    }
                )
            )()
        );
    }
}

public class _RenderAlertDialogActionsLayout__dialog : RenderFlex
{
    internal virtual double _dividerThickness { get; set; } = default!;

    internal _RenderAlertDialogActionsLayout__dialog(
        List<RenderBox>? children = null,
        double dividerThickness = default!,
        TextDirection? textDirection = null
    )
        : base(
            textDirection: textDirection,
            direction: Axis.vertical,
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.stretch
        )
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

    public virtual double horizontalSlotWidthFor(double overallWidth) =>
        DartRuntimePrimitives.ConvertValue<double>((overallWidth - dividerThickness) / 2L);

    public override double computeMinIntrinsicHeight(double width)
    {
        if (!_useHorizontalLayout(width))
        {
            return base.computeMinIntrinsicHeight(width);
        }
        double slotWidth = horizontalSlotWidthFor(overallWidth: width);
        double height = 0;
        _forEachSlot(
            (slot) =>
            {
                height = Math.Max(height, slot.getMinIntrinsicHeight(slotWidth));
            }
        );
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
        _forEachSlot(
            (slot) =>
            {
                height = Math.Max(height, slot.getMaxIntrinsicHeight(slotWidth));
            }
        );
        return height;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
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
        return new Size(overallWidth, height);
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
        size = new Size(overallWidthLocal, height);
        var ltrLocal = Equals(textDirection, TextDirection.ltr);
        RenderBox slot = firstChild!;
        double x = ltrLocal ? 0 : (overallWidthLocal - slotWidth);
        while (true)
        {
            slot.layout(
                BoxConstraints.CreateTight(new Size(slotWidth, height)),
                parentUsesSize: true
            );
            ((FlexParentData?)slot.parentData!)!.offset = new Offset(x, 0);
            if (ltrLocal)
            {
                x += slot.size.width;
            }
            else
            {
                x -= slot.size.width;
            }
            RenderBox? divider = childAfter(slot);
            if (divider is null)
            {
                break;
            }
            divider.layout(BoxConstraints.CreateTight(new Size(dividerThickness, height)));
            ((FlexParentData?)divider.parentData!)!.offset = new Offset(x, 0);
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

    internal virtual bool _debugHasValidConstraints(BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            ErrorSummary? errorSummary = default!;
            if (constraints.maxWidth == double.PositiveInfinity)
            {
                errorSummary = new ErrorSummary("The incoming width constraints are unbounded.");
            }
            if (errorSummary is not null)
            {
                throw DartRuntimePrimitives.AsException(
                    new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            errorSummary,
                            new ErrorDescription($"The incoming constraints are: {constraints}"),
                        }
                    )
                );
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
        RenderBox child = firstChild!;
        while (true)
        {
            if (child.getMaxIntrinsicWidth(double.PositiveInfinity) > slotWidth)
            {
                return false;
            }
            RenderBox? divider = childAfter(child);
            if (divider is null)
            {
                break;
            }
            child = childAfter(divider)!;
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _forEachSlot(Action<RenderBox> action)
    {
        DartRuntimePrimitives.Assert(() => (checked(childCount) & 1L) != 0L);
        RenderBox slot = firstChild!;
        while (true)
        {
            action(slot);
            RenderBox? divider = childAfter(slot);
            if (divider is null)
            {
                break;
            }
            slot = childAfter(divider)!;
        }
    }
}

internal delegate void _TwoChildrenHeights__dialog();

internal class _PriorityColumn__dialog : MultiChildRenderObjectWidget
{
    public virtual double bottomMinHeight { get; private set; } = default!;

    internal _PriorityColumn__dialog(Widget top, Widget bottom, double bottomMinHeight)
        : base(
            children: new List<Widget>
            {
                DartRuntimePrimitives.ConvertValue<Widget>(top),
                DartRuntimePrimitives.ConvertValue<Widget>(bottom),
            }
        )
    {
        this.bottomMinHeight = bottomMinHeight;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderPriorityColumn__dialog(bottomMinHeight: bottomMinHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderPriorityColumn__dialog)renderObject;
        __renderObject.bottomMinHeight = bottomMinHeight;
    }
}

public class _RenderPriorityColumn__dialog : RenderFlex
{
    internal virtual double _bottomMinHeight { get; set; } = default!;

    internal _RenderPriorityColumn__dialog(
        List<RenderBox>? children = null,
        double bottomMinHeight = default!
    )
        : base(
            direction: Axis.vertical,
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.stretch
        )
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

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        double width = constraints.maxWidth;
        double maxHeightLocal = constraints.maxHeight;
        var (topChildHeight, bottomChildHeight) = _childrenHeights(width, maxHeightLocal);
        return new Size(width, topChildHeight + bottomChildHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        double width = constraints.maxWidth;
        double maxHeightLocal = constraints.maxHeight;
        var (topChildHeight, bottomChildHeight) = _childrenHeights(width, maxHeightLocal);
        size = new Size(width, topChildHeight + bottomChildHeight);
        firstChild!.layout(
            BoxConstraints.CreateTight(new Size(width, topChildHeight)),
            parentUsesSize: true
        );
        ((FlexParentData?)firstChild!.parentData!)!.offset = Offset.zero;
        lastChild!.layout(
            BoxConstraints.CreateTight(new Size(width, bottomChildHeight)),
            parentUsesSize: true
        );
        ((FlexParentData?)lastChild!.parentData!)!.offset = new Offset(0, topChildHeight);
    }

    internal virtual (double bottomChildHeight, double topChildHeight) _childrenHeights(
        double width,
        double maxHeight
    )
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
            return (
                bottomChildHeight: effectiveBottomMinHeight,
                topChildHeight: maxHeight - effectiveBottomMinHeight
            );
        }
        return (bottomChildHeight: maxHeight, topChildHeight: 0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
