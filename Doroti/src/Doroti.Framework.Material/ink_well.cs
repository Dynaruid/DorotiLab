// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/ink_well.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public abstract class InteractiveInkFeature : InkFeature
{
    internal virtual Color _color { get; set; } = default!;
    internal virtual ShapeBorder? _customBorder { get; set; } = default;

    protected InteractiveInkFeature(
        MaterialInkController controller,
        RenderBox referenceBox,
        Color color,
        ShapeBorder? customBorder = null,
        Action? onRemoved = null
    )
        : base(controller: controller, referenceBox: referenceBox, onRemoved: onRemoved)
    {
        _color = color;
        _customBorder = customBorder;
    }

    public virtual void confirm() { }

    public virtual void cancel() { }

    public virtual Color color
    {
        get => _color;
        set
        {
            var __value = value;
            if (Equals(__value, _color))
            {
                return;
            }
            _color = __value;
            controller.markNeedsPaint();
        }
    }
    public virtual ShapeBorder? customBorder
    {
        get => _customBorder;
        set
        {
            var __value = value;
            if (Equals(__value, _customBorder))
            {
                return;
            }
            _customBorder = __value;
            controller.markNeedsPaint();
        }
    }

    public virtual void paintInkCircle(
        Canvas canvas,
        Matrix4 transform,
        Paint paint,
        Offset center,
        double radius,
        TextDirection? textDirection = null,
        ShapeBorder? customBorder = null,
        BorderRadius borderRadius = default!,
        Func<Rect>? clipCallback = null
    )
    {
        Offset? originOffset = MatrixUtils.getAsTranslation(transform);
        canvas.save();
        if (originOffset is null)
        {
            canvas.transform(transform.storage);
        }
        else
        {
            canvas.translate(
                DartRuntimePrimitives.RequireValue(originOffset).dx,
                DartRuntimePrimitives.RequireValue(originOffset).dy
            );
        }
        if (clipCallback is not null)
        {
            Rect rect = clipCallback();
            if (customBorder is not null)
            {
                canvas.clipPath(customBorder.getOuterPath(rect, textDirection: textDirection));
            }
            else
            {
                if (!Equals(borderRadius, BorderRadius.zero))
                {
                    canvas.clipRRect(
                        RRect.fromRectAndCorners(
                            rect,
                            topLeft: borderRadius.topLeft,
                            topRight: borderRadius.topRight,
                            bottomLeft: borderRadius.bottomLeft,
                            bottomRight: borderRadius.bottomRight
                        )
                    );
                }
                else
                {
                    canvas.clipRect(rect);
                }
            }
        }
        canvas.drawCircle(center, radius, paint);
        canvas.restore();
    }
}

public interface InteractiveInkFeatureFactory
{
    public InteractiveInkFeature create(
        MaterialInkController controller,
        RenderBox referenceBox,
        Offset position,
        Color color,
        TextDirection textDirection,
        bool containedInkWell = false,
        Func<Rect>? rectCallback = null,
        BorderRadius? borderRadius = null,
        ShapeBorder? customBorder = null,
        double? radius = null,
        Action? onRemoved = null
    );
}

public interface _ParentInkResponseState__ink_well
{
    public void markChildInkResponsePressed(
        _ParentInkResponseState__ink_well childState,
        bool value
    );
}

internal class _ParentInkResponseProvider__ink_well : InheritedWidget
{
    public virtual _ParentInkResponseState__ink_well state { get; private set; } = default!;

    internal _ParentInkResponseProvider__ink_well(
        _ParentInkResponseState__ink_well state,
        Widget child
    )
        : base(child: child)
    {
        this.state = state;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        DartRuntimePrimitives.ConvertValue<bool>(
            !Equals(state, ((_ParentInkResponseProvider__ink_well)oldWidget).state)
        );

    public static _ParentInkResponseState__ink_well? maybeOf(BuildContext context)
    {
        return context
            .dependOnInheritedWidgetOfExactType<_ParentInkResponseProvider__ink_well>()
            ?.state;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal delegate Func<Rect>? _GetRectCallback__ink_well(RenderBox referenceBox);

internal delegate bool _CheckContext__ink_well(BuildContext context);

public class InkResponse : StatelessWidget
{
    public virtual Widget? child { get; private set; }
    public virtual Action? onTap { get; private set; }
    public virtual Action<Gestures.TapDownDetails>? onTapDown { get; private set; }
    public virtual Action<Gestures.TapUpDetails>? onTapUp { get; private set; }
    public virtual Action? onTapCancel { get; private set; }
    public virtual Action? onDoubleTap { get; private set; }
    public virtual Action? onLongPress { get; private set; }
    public virtual Action? onLongPressUp { get; private set; }
    public virtual Action? onSecondaryTap { get; private set; }
    public virtual Action<Gestures.TapDownDetails>? onSecondaryTapDown { get; private set; }
    public virtual Action<Gestures.TapUpDetails>? onSecondaryTapUp { get; private set; }
    public virtual Action? onSecondaryTapCancel { get; private set; }
    public virtual Action<bool>? onHighlightChanged { get; private set; }
    public virtual Action<bool>? onHover { get; private set; }
    public virtual MouseCursor? mouseCursor { get; private set; }
    public virtual bool containedInkWell { get; private set; } = default!;
    public virtual BoxShape highlightShape { get; private set; } = default!;
    public virtual double? radius { get; private set; }
    public virtual BorderRadius? borderRadius { get; private set; }
    public virtual ShapeBorder? customBorder { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual Color? highlightColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual Color? splashColor { get; private set; }
    public virtual InteractiveInkFeatureFactory? splashFactory { get; private set; }
    public virtual bool enableFeedback { get; private set; } = default!;
    public virtual bool excludeFromSemantics { get; private set; } = default!;
    public virtual Action<bool>? onFocusChange { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual FocusNode? focusNode { get; private set; }
    public virtual bool canRequestFocus { get; private set; } = default!;
    public virtual WidgetStatesController? statesController { get; private set; }
    public virtual Duration? hoverDuration { get; private set; }

    public InkResponse(
        Key? key = null,
        Widget? child = null,
        Action? onTap = null,
        Action<Gestures.TapDownDetails>? onTapDown = null,
        Action<Gestures.TapUpDetails>? onTapUp = null,
        Action? onTapCancel = null,
        Action? onDoubleTap = null,
        Action? onLongPress = null,
        Action? onLongPressUp = null,
        Action? onSecondaryTap = null,
        Action<Gestures.TapUpDetails>? onSecondaryTapUp = null,
        Action<Gestures.TapDownDetails>? onSecondaryTapDown = null,
        Action? onSecondaryTapCancel = null,
        Action<bool>? onHighlightChanged = null,
        Action<bool>? onHover = null,
        MouseCursor? mouseCursor = null,
        bool containedInkWell = false,
        BoxShape highlightShape = BoxShape.circle,
        double? radius = null,
        BorderRadius? borderRadius = null,
        ShapeBorder? customBorder = null,
        Color? focusColor = null,
        Color? hoverColor = null,
        Color? highlightColor = null,
        WidgetStateProperty<Color?>? overlayColor = null,
        Color? splashColor = null,
        InteractiveInkFeatureFactory? splashFactory = null,
        bool enableFeedback = true,
        bool excludeFromSemantics = false,
        FocusNode? focusNode = null,
        bool canRequestFocus = true,
        Action<bool>? onFocusChange = null,
        bool autofocus = false,
        WidgetStatesController? statesController = null,
        Duration? hoverDuration = null
    )
        : base(key: key)
    {
        this.child = child;
        this.onTap = onTap;
        this.onTapDown = onTapDown;
        this.onTapUp = onTapUp;
        this.onTapCancel = onTapCancel;
        this.onDoubleTap = onDoubleTap;
        this.onLongPress = onLongPress;
        this.onLongPressUp = onLongPressUp;
        this.onSecondaryTap = onSecondaryTap;
        this.onSecondaryTapUp = onSecondaryTapUp;
        this.onSecondaryTapDown = onSecondaryTapDown;
        this.onSecondaryTapCancel = onSecondaryTapCancel;
        this.onHighlightChanged = onHighlightChanged;
        this.onHover = onHover;
        this.mouseCursor = mouseCursor;
        this.containedInkWell = containedInkWell;
        this.highlightShape = highlightShape;
        this.radius = radius;
        this.borderRadius = borderRadius;
        this.customBorder = customBorder;
        this.focusColor = focusColor;
        this.hoverColor = hoverColor;
        this.highlightColor = highlightColor;
        this.overlayColor = overlayColor;
        this.splashColor = splashColor;
        this.splashFactory = splashFactory;
        this.enableFeedback = enableFeedback;
        this.excludeFromSemantics = excludeFromSemantics;
        this.focusNode = focusNode;
        this.canRequestFocus = canRequestFocus;
        this.onFocusChange = onFocusChange;
        this.autofocus = autofocus;
        this.statesController = statesController;
        this.hoverDuration = hoverDuration;
    }

    public virtual Func<Rect>? getRectCallback(RenderBox referenceBox) =>
        DartRuntimePrimitives.ConvertValue<Func<Rect>>(null);

    public override Widget build(BuildContext context)
    {
        _ParentInkResponseState__ink_well? parentStateLocal =
            _ParentInkResponseProvider__ink_well.maybeOf(context);
        return new _InkResponseStateWidget__ink_well(
            onTap: onTap,
            onTapDown: onTapDown,
            onTapUp: onTapUp,
            onTapCancel: onTapCancel,
            onDoubleTap: onDoubleTap,
            onLongPress: onLongPress,
            onLongPressUp: onLongPressUp,
            onSecondaryTap: onSecondaryTap,
            onSecondaryTapUp: onSecondaryTapUp,
            onSecondaryTapDown: onSecondaryTapDown,
            onSecondaryTapCancel: onSecondaryTapCancel,
            onHighlightChanged: onHighlightChanged,
            onHover: onHover,
            mouseCursor: mouseCursor,
            containedInkWell: containedInkWell,
            highlightShape: highlightShape,
            radius: radius,
            borderRadius: borderRadius,
            customBorder: customBorder,
            focusColor: focusColor,
            hoverColor: hoverColor,
            highlightColor: highlightColor,
            overlayColor: overlayColor,
            splashColor: splashColor,
            splashFactory: splashFactory,
            enableFeedback: enableFeedback,
            excludeFromSemantics: excludeFromSemantics,
            focusNode: focusNode,
            canRequestFocus: canRequestFocus,
            onFocusChange: onFocusChange,
            autofocus: autofocus,
            parentState: parentStateLocal,
            getRectCallback: getRectCallback,
            debugCheckContext: debugCheckContext,
            statesController: statesController,
            hoverDuration: hoverDuration,
            child: child
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool debugCheckContext(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() =>
            Widgets.DebugLibrary.debugCheckHasDirectionality(context)
        );
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class _InkResponseStateWidget__ink_well : StatefulWidget
{
    public virtual Widget? child { get; private set; }
    public virtual Action? onTap { get; private set; }
    public virtual Action<Gestures.TapDownDetails>? onTapDown { get; private set; }
    public virtual Action<Gestures.TapUpDetails>? onTapUp { get; private set; }
    public virtual Action? onTapCancel { get; private set; }
    public virtual Action? onDoubleTap { get; private set; }
    public virtual Action? onLongPress { get; private set; }
    public virtual Action? onLongPressUp { get; private set; }
    public virtual Action? onSecondaryTap { get; private set; }
    public virtual Action<Gestures.TapUpDetails>? onSecondaryTapUp { get; private set; }
    public virtual Action<Gestures.TapDownDetails>? onSecondaryTapDown { get; private set; }
    public virtual Action? onSecondaryTapCancel { get; private set; }
    public virtual Action<bool>? onHighlightChanged { get; private set; }
    public virtual Action<bool>? onHover { get; private set; }
    public virtual MouseCursor? mouseCursor { get; private set; }
    public virtual bool containedInkWell { get; private set; } = default!;
    public virtual BoxShape highlightShape { get; private set; } = default!;
    public virtual double? radius { get; private set; }
    public virtual BorderRadius? borderRadius { get; private set; }
    public virtual ShapeBorder? customBorder { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual Color? highlightColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual Color? splashColor { get; private set; }
    public virtual InteractiveInkFeatureFactory? splashFactory { get; private set; }
    public virtual bool enableFeedback { get; private set; } = default!;
    public virtual bool excludeFromSemantics { get; private set; } = default!;
    public virtual Action<bool>? onFocusChange { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual FocusNode? focusNode { get; private set; }
    public virtual bool canRequestFocus { get; private set; } = default!;
    public virtual _ParentInkResponseState__ink_well? parentState { get; private set; }
    public virtual Func<RenderBox, Func<Rect>?>? getRectCallback { get; private set; }
    public virtual Func<BuildContext, bool> debugCheckContext { get; private set; } = default!;
    public virtual WidgetStatesController? statesController { get; private set; }
    public virtual Duration? hoverDuration { get; private set; }

    internal _InkResponseStateWidget__ink_well(
        Widget? child = null,
        Action? onTap = null,
        Action<Gestures.TapDownDetails>? onTapDown = null,
        Action<Gestures.TapUpDetails>? onTapUp = null,
        Action? onTapCancel = null,
        Action? onDoubleTap = null,
        Action? onLongPress = null,
        Action? onLongPressUp = null,
        Action? onSecondaryTap = null,
        Action<Gestures.TapUpDetails>? onSecondaryTapUp = null,
        Action<Gestures.TapDownDetails>? onSecondaryTapDown = null,
        Action? onSecondaryTapCancel = null,
        Action<bool>? onHighlightChanged = null,
        Action<bool>? onHover = null,
        MouseCursor? mouseCursor = null,
        bool containedInkWell = false,
        BoxShape highlightShape = BoxShape.circle,
        double? radius = null,
        BorderRadius? borderRadius = null,
        ShapeBorder? customBorder = null,
        Color? focusColor = null,
        Color? hoverColor = null,
        Color? highlightColor = null,
        WidgetStateProperty<Color?>? overlayColor = null,
        Color? splashColor = null,
        InteractiveInkFeatureFactory? splashFactory = null,
        bool enableFeedback = true,
        bool excludeFromSemantics = false,
        FocusNode? focusNode = null,
        bool canRequestFocus = true,
        Action<bool>? onFocusChange = null,
        bool autofocus = false,
        _ParentInkResponseState__ink_well? parentState = null,
        Func<RenderBox, Func<Rect>?>? getRectCallback = null,
        Func<BuildContext, bool> debugCheckContext = default!,
        WidgetStatesController? statesController = null,
        Duration? hoverDuration = null
    )
    {
        this.child = child;
        this.onTap = onTap;
        this.onTapDown = onTapDown;
        this.onTapUp = onTapUp;
        this.onTapCancel = onTapCancel;
        this.onDoubleTap = onDoubleTap;
        this.onLongPress = onLongPress;
        this.onLongPressUp = onLongPressUp;
        this.onSecondaryTap = onSecondaryTap;
        this.onSecondaryTapUp = onSecondaryTapUp;
        this.onSecondaryTapDown = onSecondaryTapDown;
        this.onSecondaryTapCancel = onSecondaryTapCancel;
        this.onHighlightChanged = onHighlightChanged;
        this.onHover = onHover;
        this.mouseCursor = mouseCursor;
        this.containedInkWell = containedInkWell;
        this.highlightShape = highlightShape;
        this.radius = radius;
        this.borderRadius = borderRadius;
        this.customBorder = customBorder;
        this.focusColor = focusColor;
        this.hoverColor = hoverColor;
        this.highlightColor = highlightColor;
        this.overlayColor = overlayColor;
        this.splashColor = splashColor;
        this.splashFactory = splashFactory;
        this.enableFeedback = enableFeedback;
        this.excludeFromSemantics = excludeFromSemantics;
        this.focusNode = focusNode;
        this.canRequestFocus = canRequestFocus;
        this.onFocusChange = onFocusChange;
        this.autofocus = autofocus;
        this.parentState = parentState;
        this.getRectCallback = getRectCallback;
        this.debugCheckContext = debugCheckContext;
        this.statesController = statesController;
        this.hoverDuration = hoverDuration;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _InkResponseState__ink_well());

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        var gestures = (
            (Func<List<string>>)(
                () =>
                {
                    var __collection30382 = new List<string>();
                    if (onTap is not null)
                    {
                        __collection30382.Add("tap");
                    }
                    if (onDoubleTap is not null)
                    {
                        __collection30382.Add("double tap");
                    }
                    if (onLongPress is not null)
                    {
                        __collection30382.Add("long press");
                    }
                    if (onLongPressUp is not null)
                    {
                        __collection30382.Add("long press up");
                    }
                    if (onTapDown is not null)
                    {
                        __collection30382.Add("tap down");
                    }
                    if (onTapUp is not null)
                    {
                        __collection30382.Add("tap up");
                    }
                    if (onTapCancel is not null)
                    {
                        __collection30382.Add("tap cancel");
                    }
                    if (onSecondaryTap is not null)
                    {
                        __collection30382.Add("secondary tap");
                    }
                    if (onSecondaryTapUp is not null)
                    {
                        __collection30382.Add("secondary tap up");
                    }
                    if (onSecondaryTapDown is not null)
                    {
                        __collection30382.Add("secondary tap down");
                    }
                    if (onSecondaryTapCancel is not null)
                    {
                        __collection30382.Add("secondary tap cancel");
                    }
                    return __collection30382;
                }
            )
        )();
        properties.add(
            new IterableProperty<string>("gestures", gestures.Cast<string>(), ifEmpty: "<none>")
        );
        properties.add(new DiagnosticsProperty<MouseCursor>("mouseCursor", mouseCursor));
        properties.add(
            new DiagnosticsProperty<bool>(
                "containedInkWell",
                containedInkWell,
                level: DiagnosticLevel.fine
            )
        );
        properties.add(
            new DiagnosticsProperty<BoxShape>(
                "highlightShape",
                highlightShape,
                description: $"{(containedInkWell ? "clipped to " : "")}{highlightShape}",
                showName: false
            )
        );
    }
}

public enum _HighlightType__ink_well
{
    pressed,
    hover,
    focus,
}

public class _InkResponseState__ink_well
    : State<_InkResponseStateWidget__ink_well>,
        AutomaticKeepAliveClientMixin<_InkResponseStateWidget__ink_well>,
        _ParentInkResponseState__ink_well
{
    internal virtual HashSet<InteractiveInkFeature>? _splashes { get; set; } = default;
    internal virtual InteractiveInkFeature? _currentSplash { get; set; } = default;
    internal virtual bool _hovering { get; set; } = false;
    internal virtual DartMap<_HighlightType__ink_well, InkHighlight?> _highlights
    {
        get;
        private set;
    } = new DartMap<_HighlightType__ink_well, InkHighlight?>();
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
                        onInvoke: activateOnIntent
                    ),
                    [typeof(ButtonActivateIntent)] = new CallbackAction<ButtonActivateIntent>(
                        onInvoke: activateOnIntent
                    ),
                };
                __late__actionMap_initialized = true;
            }
            return __late__actionMap;
        }
    }
    public virtual WidgetStatesController? internalStatesController { get; set; } = default;
    internal virtual ObserverList<_ParentInkResponseState__ink_well> _activeChildren
    {
        get;
        private set;
    } = new ObserverList<_ParentInkResponseState__ink_well>();
    internal static Duration _activationDuration = Duration.Create(milliseconds: 100L);
    internal virtual Timer? _activationTimer { get; set; } = default;
    internal virtual bool _hasFocus { get; set; } = false;
    public virtual KeepAliveHandle? _keepAliveHandle { get; set; } = default;

    public virtual bool highlightsExist =>
        Enumerable.Any(_highlights.Values.where((highlight) => highlight is not null));

    public virtual void markChildInkResponsePressed(
        _ParentInkResponseState__ink_well childState,
        bool value
    )
    {
        bool lastAnyPressed = _anyChildInkResponsePressed;
        if (value)
        {
            _activeChildren.add(childState);
        }
        else
        {
            _activeChildren.remove(childState);
        }
        bool nowAnyPressed = _anyChildInkResponsePressed;
        if (nowAnyPressed != lastAnyPressed)
        {
            widget.parentState?.markChildInkResponsePressed(this, nowAnyPressed);
        }
    }

    internal virtual bool _anyChildInkResponsePressed => Enumerable.Any(_activeChildren);

    public virtual void activateOnIntent(Intent? intent)
    {
        _activationTimer?.cancel();
        _activationTimer = null;
        _startNewSplash(context: context);
        _currentSplash?.confirm();
        _currentSplash = null;
        if (widget.onTap is not null)
        {
            if (widget.enableFeedback)
            {
                DartRuntimePrimitives.Ignore(Feedback.forTap(context));
            }
            widget.onTap?.Invoke();
        }
        _activationTimer = new Timer(
            _activationDuration,
            () =>
            {
                updateHighlight(_HighlightType__ink_well.pressed, value: false);
            }
        );
    }

    public virtual void simulateTap(Intent? intent = null)
    {
        _startNewSplash(context: context);
        handleTap();
    }

    public virtual void simulateLongPress()
    {
        _startNewSplash(context: context);
        handleLongPress();
    }

    public virtual void handleStatesControllerChange()
    {
        setState(() => { });
    }

    public virtual WidgetStatesController statesController =>
        DartRuntimePrimitives.ConvertValue<WidgetStatesController>(
            widget.statesController ?? internalStatesController!
        );

    public virtual void initStatesController()
    {
        if (widget.statesController is null)
        {
            internalStatesController = new WidgetStatesController();
        }
        statesController.update(WidgetState.disabled, !enabled);
        statesController.addListener(handleStatesControllerChange);
    }

    public override void initState()
    {
        base.initState();
        if (wantKeepAlive)
        {
            _ensureKeepAlive();
        }
        initStatesController();
        FocusManager.instance.addHighlightModeListener(handleFocusHighlightModeChange);
    }

    public override void didUpdateWidget(_InkResponseStateWidget__ink_well oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.statesController, oldWidget.statesController))
        {
            oldWidget.statesController?.removeListener(handleStatesControllerChange);
            if (widget.statesController is not null)
            {
                internalStatesController?.dispose();
                internalStatesController = null;
            }
            initStatesController();
        }
        if (
            (widget.radius != oldWidget.radius)
            || (!Equals(widget.highlightShape, oldWidget.highlightShape))
            || (!Equals(widget.borderRadius, oldWidget.borderRadius))
        )
        {
            InkHighlight? hoverHighlight = _highlights.GetValueOrDefault(
                _HighlightType__ink_well.hover
            );
            if (hoverHighlight is not null)
            {
                hoverHighlight.dispose();
                updateHighlight(
                    _HighlightType__ink_well.hover,
                    value: _hovering,
                    callOnHover: false
                );
            }
            InkHighlight? focusHighlight = _highlights.GetValueOrDefault(
                _HighlightType__ink_well.focus
            );
            if (focusHighlight is not null)
            {
                focusHighlight.dispose();
            }
        }
        if (!Equals(widget.customBorder, oldWidget.customBorder))
        {
            _updateHighlightsAndSplashes();
        }
        if (enabled != isWidgetEnabled(oldWidget))
        {
            statesController.update(WidgetState.disabled, !enabled);
            if (!enabled)
            {
                statesController.update(WidgetState.pressed, false);
                InkHighlight? hoverHighlightLocal = _highlights.GetValueOrDefault(
                    _HighlightType__ink_well.hover
                );
                hoverHighlightLocal?.dispose();
            }
            updateHighlight(_HighlightType__ink_well.hover, value: _hovering, callOnHover: false);
        }
        updateFocusHighlights();
    }

    public override void dispose()
    {
        FocusManager.instance.removeHighlightModeListener(handleFocusHighlightModeChange);
        statesController.removeListener(handleStatesControllerChange);
        internalStatesController?.dispose();
        _activationTimer?.cancel();
        _activationTimer = null;
        base.dispose();
    }

    public virtual bool wantKeepAlive =>
        DartRuntimePrimitives.ConvertValue<bool>(
            highlightsExist || ((_splashes is not null) && Enumerable.Any(_splashes!))
        );

    public virtual Duration getFadeDurationForType(_HighlightType__ink_well type)
    {
        switch (type)
        {
            case _HighlightType__ink_well.pressed:
            {
                return Duration.Create(milliseconds: 200L);
            }
            case _HighlightType__ink_well.hover:
            case _HighlightType__ink_well.focus:
            {
                return widget.hoverDuration ?? Duration.Create(milliseconds: 50L);
            }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void updateHighlight(
        _HighlightType__ink_well type,
        bool value,
        bool callOnHover = true
    )
    {
        InkHighlight? highlight = _highlights.GetValueOrDefault(type);
        void handleInkRemoval()
        {
            DartRuntimePrimitives.Assert(() => _highlights.ContainsKey(type));
            _highlights[type] = null;
            updateKeepAlive();
        }
        switch (type)
        {
            case _HighlightType__ink_well.pressed:
            {
                statesController.update(WidgetState.pressed, value);
                break;
            }
            case _HighlightType__ink_well.hover:
            {
                if (callOnHover)
                {
                    statesController.update(WidgetState.hovered, value);
                }
                break;
            }
            case _HighlightType__ink_well.focus:
            {
                break;
            }
        }
        if (Equals(type, _HighlightType__ink_well.pressed))
        {
            widget.parentState?.markChildInkResponsePressed(this, value);
        }
        if (value == ((highlight is not null) && highlight.active))
        {
            return;
        }
        if (value)
        {
            if (highlight is null)
            {
                Color resolvedOverlayColor =
                    widget.overlayColor?.resolve(statesController.value)
                    ?? (
                        type switch
                        {
                            _HighlightType__ink_well.pressed => widget.highlightColor
                                ?? Theme.of(context).highlightColor,
                            _HighlightType__ink_well.focus => widget.focusColor
                                ?? Theme.of(context).focusColor,
                            _HighlightType__ink_well.hover => widget.hoverColor
                                ?? Theme.of(context).hoverColor,
                            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                                throw new InvalidOperationException(
                                    "Non-exhaustive Dart switch value."
                                ),
                        }
                    );
                var referenceBoxLocal = ((RenderBox?)context.findRenderObject()!)!;
                _highlights[type] = new InkHighlight(
                    controller: Material.of(context),
                    referenceBox: referenceBoxLocal,
                    color: enabled ? resolvedOverlayColor : resolvedOverlayColor.withAlpha(0L),
                    shape: widget.highlightShape,
                    radius: widget.radius,
                    borderRadius: widget.borderRadius,
                    customBorder: widget.customBorder,
                    rectCallback: widget.getRectCallback!(referenceBoxLocal),
                    onRemoved: () => handleInkRemoval(),
                    textDirection: Directionality.of(context),
                    fadeDuration: getFadeDurationForType(type)
                );
                updateKeepAlive();
            }
            else
            {
                highlight.activate();
            }
        }
        else
        {
            highlight!.deactivate();
        }
        DartRuntimePrimitives.Assert(() =>
            value
            == (
                (_highlights.GetValueOrDefault(type) is InkHighlight currentHighlight)
                && currentHighlight.active
            )
        );
        switch (type)
        {
            case _HighlightType__ink_well.pressed:
            {
                widget.onHighlightChanged?.Invoke(value);
                break;
            }
            case _HighlightType__ink_well.hover:
            {
                if (callOnHover)
                {
                    widget.onHover?.Invoke(value);
                }
                break;
            }
            case _HighlightType__ink_well.focus:
            {
                break;
            }
        }
    }

    internal virtual void _updateHighlightsAndSplashes()
    {
        foreach (InkHighlight? highlight in _highlights.Values)
        {
            highlight?.customBorder = widget.customBorder;
        }
        _currentSplash?.customBorder = widget.customBorder;
        if ((_splashes is not null) && Enumerable.Any(_splashes!))
        {
            foreach (InteractiveInkFeature inkFeature in _splashes!)
            {
                inkFeature.customBorder = widget.customBorder;
            }
        }
    }

    internal virtual InteractiveInkFeature _createSplash(Offset globalPosition)
    {
        MaterialInkController inkController = Material.of(context);
        var referenceBoxLocal = ((RenderBox?)context.findRenderObject()!)!;
        Offset positionLocal = referenceBoxLocal.globalToLocal(globalPosition);
        Color colorLocal =
            (widget.overlayColor?.resolve(statesController.value) ?? widget.splashColor)
            ?? Theme.of(context).splashColor;
        Func<Rect>? rectCallbackLocal = widget.containedInkWell
            ? widget.getRectCallback!(referenceBoxLocal)
            : null;
        BorderRadius? borderRadiusLocal = widget.borderRadius;
        ShapeBorder? customBorderLocal = widget.customBorder;
        InteractiveInkFeature? splash = default!;
        void onRemoved()
        {
            if (_splashes is not null)
            {
                DartRuntimePrimitives.Assert(() => _splashes!.Contains(splash));
                _splashes!.Remove(splash);
                if (Equals(_currentSplash, splash))
                {
                    _currentSplash = null;
                }
                updateKeepAlive();
            }
        }
        splash = (widget.splashFactory ?? Theme.of(context).splashFactory).create(
            controller: inkController,
            referenceBox: referenceBoxLocal,
            position: positionLocal,
            color: colorLocal,
            containedInkWell: widget.containedInkWell,
            rectCallback: rectCallbackLocal,
            radius: widget.radius,
            borderRadius: borderRadiusLocal,
            customBorder: customBorderLocal,
            onRemoved: () => onRemoved(),
            textDirection: Directionality.of(context)
        );
        return splash;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleFocusHighlightModeChange(FocusHighlightMode mode)
    {
        if (!mounted)
        {
            return;
        }
        setState(() =>
        {
            updateFocusHighlights();
        });
    }

    internal virtual bool _shouldShowFocus =>
        MediaQuery.maybeNavigationModeOf(context) switch
        {
            NavigationMode.traditional => enabled && _hasFocus,
            null => enabled && _hasFocus,
            NavigationMode.directional => _hasFocus,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };

    public virtual void updateFocusHighlights()
    {
        bool showFocus = FocusManager.instance.highlightMode switch
        {
            FocusHighlightMode.touch => false,
            FocusHighlightMode.traditional => _shouldShowFocus,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        updateHighlight(_HighlightType__ink_well.focus, value: showFocus);
    }

    public virtual void handleFocusUpdate(bool hasFocus)
    {
        _hasFocus = hasFocus;
        statesController.update(WidgetState.focused, hasFocus);
        updateFocusHighlights();
        widget.onFocusChange?.Invoke(hasFocus);
    }

    public virtual void handleAnyTapDown(Gestures.TapDownDetails details)
    {
        if (_anyChildInkResponsePressed)
        {
            return;
        }
        _startNewSplash(details: details);
    }

    public virtual void handleTapDown(Gestures.TapDownDetails details)
    {
        handleAnyTapDown(details);
        widget.onTapDown?.Invoke(details);
    }

    public virtual void handleTapUp(Gestures.TapUpDetails details)
    {
        widget.onTapUp?.Invoke(details);
    }

    public virtual void handleSecondaryTapDown(Gestures.TapDownDetails details)
    {
        handleAnyTapDown(details);
        widget.onSecondaryTapDown?.Invoke(details);
    }

    public virtual void handleSecondaryTapUp(Gestures.TapUpDetails details)
    {
        widget.onSecondaryTapUp?.Invoke(details);
    }

    internal virtual void _startNewSplash(
        Gestures.TapDownDetails? details = null,
        BuildContext? context = null
    )
    {
        DartRuntimePrimitives.Assert(() => (details is not null) || (context is not null));
        Offset globalPositionLocal = default!;
        if (context is not null)
        {
            var referenceBox = ((RenderBox?)context.findRenderObject()!)!;
            DartRuntimePrimitives.Assert(
                () => referenceBox.hasSize,
                () => (object?)"InkResponse must be done with layout before starting a splash."
            );
            globalPositionLocal = referenceBox.localToGlobal(referenceBox.paintBounds.center);
        }
        else
        {
            globalPositionLocal = details!.globalPosition;
        }
        statesController.update(WidgetState.pressed, true);
        InteractiveInkFeature splash = _createSplash(globalPositionLocal);
        _splashes ??= new HashSet<InteractiveInkFeature>();
        _splashes!.Add(splash);
        _currentSplash?.cancel();
        _currentSplash = splash;
        updateKeepAlive();
        updateHighlight(_HighlightType__ink_well.pressed, value: true);
    }

    public virtual void handleTap()
    {
        _currentSplash?.confirm();
        _currentSplash = null;
        updateHighlight(_HighlightType__ink_well.pressed, value: false);
        if (widget.onTap is not null)
        {
            if (widget.enableFeedback)
            {
                DartRuntimePrimitives.Ignore(Feedback.forTap(context));
            }
            widget.onTap?.Invoke();
        }
    }

    public virtual void handleTapCancel()
    {
        _currentSplash?.cancel();
        _currentSplash = null;
        widget.onTapCancel?.Invoke();
        updateHighlight(_HighlightType__ink_well.pressed, value: false);
    }

    public virtual void handleDoubleTap()
    {
        _currentSplash?.confirm();
        _currentSplash = null;
        updateHighlight(_HighlightType__ink_well.pressed, value: false);
        widget.onDoubleTap?.Invoke();
    }

    public virtual void handleLongPress()
    {
        _currentSplash?.confirm();
        _currentSplash = null;
        if (widget.onLongPress is not null)
        {
            if (widget.enableFeedback)
            {
                DartRuntimePrimitives.Ignore(Feedback.forLongPress(context));
            }
            widget.onLongPress!();
        }
    }

    public virtual void handleLongPressUp()
    {
        _currentSplash?.confirm();
        _currentSplash = null;
        widget.onLongPressUp?.Invoke();
    }

    public virtual void handleSecondaryTap()
    {
        _currentSplash?.confirm();
        _currentSplash = null;
        updateHighlight(_HighlightType__ink_well.pressed, value: false);
        widget.onSecondaryTap?.Invoke();
    }

    public virtual void handleSecondaryTapCancel()
    {
        _currentSplash?.cancel();
        _currentSplash = null;
        widget.onSecondaryTapCancel?.Invoke();
        updateHighlight(_HighlightType__ink_well.pressed, value: false);
    }

    public override void deactivate()
    {
        if (_splashes is not null)
        {
            HashSet<InteractiveInkFeature> splashes = _splashes!;
            _splashes = null;
            foreach (var splash in splashes)
            {
                splash.dispose();
            }
            _currentSplash = null;
        }
        DartRuntimePrimitives.Assert(() => _currentSplash is null);
        foreach (_HighlightType__ink_well highlight in _highlights.Keys)
        {
            _highlights.GetValueOrDefault(highlight)?.dispose();
            _highlights[DartRuntimePrimitives.RequireValue(highlight)] = null;
        }
        widget.parentState?.markChildInkResponsePressed(this, false);
        if (_keepAliveHandle is not null)
        {
            _releaseKeepAlive();
        }
        base.deactivate();
    }

    public virtual bool isWidgetEnabled(_InkResponseStateWidget__ink_well widget)
    {
        return _primaryButtonEnabled(widget) || _secondaryButtonEnabled(widget);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _primaryButtonEnabled(_InkResponseStateWidget__ink_well widget)
    {
        return (widget.onTap is not null)
            || (widget.onDoubleTap is not null)
            || (widget.onLongPress is not null)
            || (widget.onLongPressUp is not null)
            || (widget.onTapUp is not null)
            || (widget.onTapDown is not null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _secondaryButtonEnabled(_InkResponseStateWidget__ink_well widget)
    {
        return (widget.onSecondaryTap is not null)
            || (widget.onSecondaryTapUp is not null)
            || (widget.onSecondaryTapDown is not null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool enabled => isWidgetEnabled(widget);
    internal virtual bool _primaryEnabled => _primaryButtonEnabled(widget);
    internal virtual bool _secondaryEnabled => _secondaryButtonEnabled(widget);

    public virtual void handleMouseEnter(Gestures.PointerEnterEvent @event)
    {
        _hovering = true;
        if (enabled)
        {
            handleHoverChange();
        }
    }

    public virtual void handleMouseExit(Gestures.PointerExitEvent @event)
    {
        _hovering = false;
        handleHoverChange();
    }

    public virtual void handleHoverChange()
    {
        updateHighlight(_HighlightType__ink_well.hover, value: _hovering);
    }

    internal virtual bool _canRequestFocus =>
        MediaQuery.maybeNavigationModeOf(context) switch
        {
            NavigationMode.traditional => enabled && widget.canRequestFocus,
            null => enabled && widget.canRequestFocus,
            NavigationMode.directional => true,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => widget.debugCheckContext(context));
        if (wantKeepAlive && (_keepAliveHandle is null))
        {
            _ensureKeepAlive();
        }
        ThemeData theme = Theme.of(context);
        var highlightableStates = new HashSet<WidgetState>
        {
            WidgetState.focused,
            WidgetState.hovered,
            WidgetState.pressed,
        };
        HashSet<WidgetState> nonHighlightableStates = statesController.value.difference(
            highlightableStates
        );
        var pressedLocal = (
            (Func<HashSet<WidgetState>>)(
                () =>
                {
                    var __collection48677 = new HashSet<WidgetState>();
                    __collection48677.UnionWith(nonHighlightableStates);
                    __collection48677.Add(WidgetState.pressed);
                    return __collection48677;
                }
            )
        )();
        var focusedLocal = (
            (Func<HashSet<WidgetState>>)(
                () =>
                {
                    var __collection48760 = new HashSet<WidgetState>();
                    __collection48760.UnionWith(nonHighlightableStates);
                    __collection48760.Add(WidgetState.focused);
                    return __collection48760;
                }
            )
        )();
        var hoveredLocal = (
            (Func<HashSet<WidgetState>>)(
                () =>
                {
                    var __collection48843 = new HashSet<WidgetState>();
                    __collection48843.UnionWith(nonHighlightableStates);
                    __collection48843.Add(WidgetState.hovered);
                    return __collection48843;
                }
            )
        )();
        Color getHighlightColorForType(_HighlightType__ink_well type)
        {
            return type switch
            {
                _HighlightType__ink_well.pressed => (
                    widget.overlayColor?.resolve(pressedLocal) ?? widget.highlightColor
                ) ?? theme.highlightColor,
                _HighlightType__ink_well.focus => (
                    widget.overlayColor?.resolve(focusedLocal) ?? widget.focusColor
                ) ?? theme.focusColor,
                _HighlightType__ink_well.hover => (
                    widget.overlayColor?.resolve(hoveredLocal) ?? widget.hoverColor
                ) ?? theme.hoverColor,
                _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                    throw new InvalidOperationException("Non-exhaustive Dart switch value."),
            };
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        foreach (_HighlightType__ink_well typeLocal in _highlights.Keys)
        {
            _highlights[typeLocal]?.color = getHighlightColorForType(typeLocal);
        }
        _currentSplash?.color =
            (widget.overlayColor?.resolve(statesController.value) ?? widget.splashColor)
            ?? Theme.of(context).splashColor;
        MouseCursor effectiveMouseCursor = WidgetStateProperty.resolveAs(
            widget.mouseCursor ?? WidgetStateMouseCursor.adaptiveClickable,
            statesController.value
        );
        return new _ParentInkResponseProvider__ink_well(
            state: this,
            child: new Actions(
                actions: _actionMap,
                child: new Focus(
                    focusNode: widget.focusNode,
                    canRequestFocus: _canRequestFocus,
                    onFocusChange: handleFocusUpdate,
                    autofocus: widget.autofocus,
                    child: new MouseRegion(
                        cursor: effectiveMouseCursor,
                        onEnter: handleMouseEnter,
                        onExit: handleMouseExit,
                        child: DefaultSelectionStyle.merge(
                            mouseCursor: effectiveMouseCursor,
                            child: new Widgets.Semantics(
                                onTap: (widget.excludeFromSemantics || widget.onTap is null)
                                    ? null
                                    : () => simulateTap(null),
                                onLongPress: (
                                    widget.excludeFromSemantics || (widget.onLongPress is null)
                                )
                                    ? null
                                    : simulateLongPress,
                                child: new GestureDetector(
                                    onTapDown: _primaryEnabled ? handleTapDown : null,
                                    onTapUp: _primaryEnabled ? handleTapUp : null,
                                    onTap: _primaryEnabled ? handleTap : null,
                                    onTapCancel: _primaryEnabled ? handleTapCancel : null,
                                    onDoubleTap: (widget.onDoubleTap is not null)
                                        ? handleDoubleTap
                                        : null,
                                    onLongPress: (widget.onLongPress is not null)
                                        ? handleLongPress
                                        : null,
                                    onLongPressUp: (widget.onLongPressUp is not null)
                                        ? handleLongPressUp
                                        : null,
                                    onSecondaryTapDown: _secondaryEnabled
                                        ? handleSecondaryTapDown
                                        : null,
                                    onSecondaryTapUp: _secondaryEnabled
                                        ? handleSecondaryTapUp
                                        : null,
                                    onSecondaryTap: _secondaryEnabled ? handleSecondaryTap : null,
                                    onSecondaryTapCancel: _secondaryEnabled
                                        ? handleSecondaryTapCancel
                                        : null,
                                    behavior: HitTestBehavior.opaque,
                                    excludeFromSemantics: true,
                                    child: widget.child
                                )
                            )
                        )
                    )
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _ensureKeepAlive()
    {
        DartRuntimePrimitives.Assert(() => _keepAliveHandle is null);
        _keepAliveHandle = new KeepAliveHandle();
        new KeepAliveNotification(_keepAliveHandle!).dispatch(context);
    }

    public virtual void _releaseKeepAlive()
    {
        _keepAliveHandle!.dispose();
        _keepAliveHandle = null;
    }

    public virtual void updateKeepAlive()
    {
        if (wantKeepAlive)
        {
            if (_keepAliveHandle is null)
            {
                _ensureKeepAlive();
            }
        }
        else
        {
            if (_keepAliveHandle is not null)
            {
                _releaseKeepAlive();
            }
        }
    }
}

public class InkWell : InkResponse
{
    public InkWell(
        Key? key = null,
        Widget? child = null,
        Action? onTap = null,
        Action? onDoubleTap = null,
        Action? onLongPress = null,
        Action? onLongPressUp = null,
        Action<Gestures.TapDownDetails>? onTapDown = null,
        Action<Gestures.TapUpDetails>? onTapUp = null,
        Action? onTapCancel = null,
        Action? onSecondaryTap = null,
        Action<Gestures.TapUpDetails>? onSecondaryTapUp = null,
        Action<Gestures.TapDownDetails>? onSecondaryTapDown = null,
        Action? onSecondaryTapCancel = null,
        Action<bool>? onHighlightChanged = null,
        Action<bool>? onHover = null,
        MouseCursor? mouseCursor = null,
        Color? focusColor = null,
        Color? hoverColor = null,
        Color? highlightColor = null,
        WidgetStateProperty<Color?>? overlayColor = null,
        Color? splashColor = null,
        InteractiveInkFeatureFactory? splashFactory = null,
        double? radius = null,
        BorderRadius? borderRadius = null,
        ShapeBorder? customBorder = null,
        bool enableFeedback = true,
        bool excludeFromSemantics = false,
        FocusNode? focusNode = null,
        bool canRequestFocus = true,
        Action<bool>? onFocusChange = null,
        bool autofocus = false,
        WidgetStatesController? statesController = null,
        Duration? hoverDuration = null
    )
        : base(
            key: key,
            child: child,
            onTap: onTap,
            onDoubleTap: onDoubleTap,
            onLongPress: onLongPress,
            onLongPressUp: onLongPressUp,
            onTapDown: onTapDown,
            onTapUp: onTapUp,
            onTapCancel: onTapCancel,
            onSecondaryTap: onSecondaryTap,
            onSecondaryTapUp: onSecondaryTapUp,
            onSecondaryTapDown: onSecondaryTapDown,
            onSecondaryTapCancel: onSecondaryTapCancel,
            onHighlightChanged: onHighlightChanged,
            onHover: onHover,
            mouseCursor: mouseCursor,
            focusColor: focusColor,
            hoverColor: hoverColor,
            highlightColor: highlightColor,
            overlayColor: overlayColor,
            splashColor: splashColor,
            splashFactory: splashFactory,
            radius: radius,
            borderRadius: borderRadius,
            customBorder: customBorder,
            enableFeedback: enableFeedback,
            excludeFromSemantics: excludeFromSemantics,
            focusNode: focusNode,
            canRequestFocus: canRequestFocus,
            onFocusChange: onFocusChange,
            autofocus: autofocus,
            statesController: statesController,
            hoverDuration: hoverDuration,
            containedInkWell: true,
            highlightShape: BoxShape.rectangle
        ) { }
}
