// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/gesture_detector.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public abstract class GestureRecognizerFactoryBase
{
    internal abstract GestureRecognizer createRecognizer();
    internal abstract void initializeRecognizer(GestureRecognizer instance);
    internal abstract bool _debugAssertTypeMatches(Type type);
}

public abstract class GestureRecognizerFactory<T> : GestureRecognizerFactoryBase
    where T : GestureRecognizer
{
    protected GestureRecognizerFactory() { }

    public abstract T constructor();
    public abstract void initializer(T instance);

    internal override GestureRecognizer createRecognizer() => constructor();

    internal override void initializeRecognizer(GestureRecognizer instance) =>
        initializer((T)instance);

    internal override bool _debugAssertTypeMatches(Type type)
    {
        DartRuntimePrimitives.Assert(
            () => Equals(type, typeof(T)),
            () =>
                (object?)
                    $"GestureRecognizerFactory of type {typeof(T)} was used where type {type} was specified."
        );
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public delegate T GestureRecognizerFactoryConstructor<T>()
    where T : GestureRecognizer;

public delegate void GestureRecognizerFactoryInitializer<T>(T instance)
    where T : GestureRecognizer;

public class GestureRecognizerFactoryWithHandlers<T> : GestureRecognizerFactory<T>
    where T : GestureRecognizer
{
    internal virtual Func<T> _constructor { get; private set; } = default!;
    internal virtual Action<T> _initializer { get; private set; } = default!;

    public GestureRecognizerFactoryWithHandlers(Func<T> _constructor, Action<T> _initializer)
    {
        this._constructor = _constructor;
        this._initializer = _initializer;
    }

    public override T constructor() => _constructor();

    public override void initializer(T instance) => _initializer(instance);
}

public class GestureDetector : StatelessWidget
{
    public virtual Widget? child { get; private set; }
    public virtual Action<TapDownDetails>? onTapDown { get; private set; }
    public virtual Action<TapUpDetails>? onTapUp { get; private set; }
    public virtual Action? onTap { get; private set; }
    public virtual Action<TapMoveDetails>? onTapMove { get; private set; }
    public virtual Action? onTapCancel { get; private set; }
    public virtual Action? onSecondaryTap { get; private set; }
    public virtual Action<TapDownDetails>? onSecondaryTapDown { get; private set; }
    public virtual Action<TapUpDetails>? onSecondaryTapUp { get; private set; }
    public virtual Action? onSecondaryTapCancel { get; private set; }
    public virtual Action<TapDownDetails>? onTertiaryTapDown { get; private set; }
    public virtual Action<TapUpDetails>? onTertiaryTapUp { get; private set; }
    public virtual Action? onTertiaryTapCancel { get; private set; }
    public virtual Action<TapDownDetails>? onDoubleTapDown { get; private set; }
    public virtual Action? onDoubleTap { get; private set; }
    public virtual Action? onDoubleTapCancel { get; private set; }
    public virtual Action<LongPressDownDetails>? onLongPressDown { get; private set; }
    public virtual Action? onLongPressCancel { get; private set; }
    public virtual Action? onLongPress { get; private set; }
    public virtual Action<LongPressStartDetails>? onLongPressStart { get; private set; }
    public virtual Action<LongPressMoveUpdateDetails>? onLongPressMoveUpdate { get; private set; }
    public virtual Action? onLongPressUp { get; private set; }
    public virtual Action<LongPressEndDetails>? onLongPressEnd { get; private set; }
    public virtual Action<LongPressDownDetails>? onSecondaryLongPressDown { get; private set; }
    public virtual Action? onSecondaryLongPressCancel { get; private set; }
    public virtual Action? onSecondaryLongPress { get; private set; }
    public virtual Action<LongPressStartDetails>? onSecondaryLongPressStart { get; private set; }
    public virtual Action<LongPressMoveUpdateDetails>? onSecondaryLongPressMoveUpdate
    {
        get;
        private set;
    }
    public virtual Action? onSecondaryLongPressUp { get; private set; }
    public virtual Action<LongPressEndDetails>? onSecondaryLongPressEnd { get; private set; }
    public virtual Action<LongPressDownDetails>? onTertiaryLongPressDown { get; private set; }
    public virtual Action? onTertiaryLongPressCancel { get; private set; }
    public virtual Action? onTertiaryLongPress { get; private set; }
    public virtual Action<LongPressStartDetails>? onTertiaryLongPressStart { get; private set; }
    public virtual Action<LongPressMoveUpdateDetails>? onTertiaryLongPressMoveUpdate
    {
        get;
        private set;
    }
    public virtual Action? onTertiaryLongPressUp { get; private set; }
    public virtual Action<LongPressEndDetails>? onTertiaryLongPressEnd { get; private set; }
    public virtual Action<DragDownDetails>? onVerticalDragDown { get; private set; }
    public virtual Action<DragStartDetails>? onVerticalDragStart { get; private set; }
    public virtual Action<DragUpdateDetails>? onVerticalDragUpdate { get; private set; }
    public virtual Action<DragEndDetails>? onVerticalDragEnd { get; private set; }
    public virtual Action? onVerticalDragCancel { get; private set; }
    public virtual Action<DragDownDetails>? onHorizontalDragDown { get; private set; }
    public virtual Action<DragStartDetails>? onHorizontalDragStart { get; private set; }
    public virtual Action<DragUpdateDetails>? onHorizontalDragUpdate { get; private set; }
    public virtual Action<DragEndDetails>? onHorizontalDragEnd { get; private set; }
    public virtual Action? onHorizontalDragCancel { get; private set; }
    public virtual Action<DragDownDetails>? onPanDown { get; private set; }
    public virtual Action<DragStartDetails>? onPanStart { get; private set; }
    public virtual Action<DragUpdateDetails>? onPanUpdate { get; private set; }
    public virtual Action<DragEndDetails>? onPanEnd { get; private set; }
    public virtual Action? onPanCancel { get; private set; }
    public virtual Action<ScaleStartDetails>? onScaleStart { get; private set; }
    public virtual Action<ScaleUpdateDetails>? onScaleUpdate { get; private set; }
    public virtual Action<ScaleEndDetails>? onScaleEnd { get; private set; }
    public virtual Action<ForcePressDetails>? onForcePressStart { get; private set; }
    public virtual Action<ForcePressDetails>? onForcePressPeak { get; private set; }
    public virtual Action<ForcePressDetails>? onForcePressUpdate { get; private set; }
    public virtual Action<ForcePressDetails>? onForcePressEnd { get; private set; }
    public virtual HitTestBehavior? behavior { get; private set; }
    public virtual bool excludeFromSemantics { get; private set; } = default!;
    public virtual DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual HashSet<PointerDeviceKind>? supportedDevices { get; private set; }
    public virtual bool trackpadScrollCausesScale { get; private set; } = default!;
    public virtual Offset trackpadScrollToScaleFactor { get; private set; } = default!;

    public GestureDetector(
        Key? key = null,
        Widget? child = null,
        Action<TapDownDetails>? onTapDown = null,
        Action<TapUpDetails>? onTapUp = null,
        Action? onTap = null,
        Action<TapMoveDetails>? onTapMove = null,
        Action? onTapCancel = null,
        Action? onSecondaryTap = null,
        Action<TapDownDetails>? onSecondaryTapDown = null,
        Action<TapUpDetails>? onSecondaryTapUp = null,
        Action? onSecondaryTapCancel = null,
        Action<TapDownDetails>? onTertiaryTapDown = null,
        Action<TapUpDetails>? onTertiaryTapUp = null,
        Action? onTertiaryTapCancel = null,
        Action<TapDownDetails>? onDoubleTapDown = null,
        Action? onDoubleTap = null,
        Action? onDoubleTapCancel = null,
        Action<LongPressDownDetails>? onLongPressDown = null,
        Action? onLongPressCancel = null,
        Action? onLongPress = null,
        Action<LongPressStartDetails>? onLongPressStart = null,
        Action<LongPressMoveUpdateDetails>? onLongPressMoveUpdate = null,
        Action? onLongPressUp = null,
        Action<LongPressEndDetails>? onLongPressEnd = null,
        Action<LongPressDownDetails>? onSecondaryLongPressDown = null,
        Action? onSecondaryLongPressCancel = null,
        Action? onSecondaryLongPress = null,
        Action<LongPressStartDetails>? onSecondaryLongPressStart = null,
        Action<LongPressMoveUpdateDetails>? onSecondaryLongPressMoveUpdate = null,
        Action? onSecondaryLongPressUp = null,
        Action<LongPressEndDetails>? onSecondaryLongPressEnd = null,
        Action<LongPressDownDetails>? onTertiaryLongPressDown = null,
        Action? onTertiaryLongPressCancel = null,
        Action? onTertiaryLongPress = null,
        Action<LongPressStartDetails>? onTertiaryLongPressStart = null,
        Action<LongPressMoveUpdateDetails>? onTertiaryLongPressMoveUpdate = null,
        Action? onTertiaryLongPressUp = null,
        Action<LongPressEndDetails>? onTertiaryLongPressEnd = null,
        Action<DragDownDetails>? onVerticalDragDown = null,
        Action<DragStartDetails>? onVerticalDragStart = null,
        Action<DragUpdateDetails>? onVerticalDragUpdate = null,
        Action<DragEndDetails>? onVerticalDragEnd = null,
        Action? onVerticalDragCancel = null,
        Action<DragDownDetails>? onHorizontalDragDown = null,
        Action<DragStartDetails>? onHorizontalDragStart = null,
        Action<DragUpdateDetails>? onHorizontalDragUpdate = null,
        Action<DragEndDetails>? onHorizontalDragEnd = null,
        Action? onHorizontalDragCancel = null,
        Action<ForcePressDetails>? onForcePressStart = null,
        Action<ForcePressDetails>? onForcePressPeak = null,
        Action<ForcePressDetails>? onForcePressUpdate = null,
        Action<ForcePressDetails>? onForcePressEnd = null,
        Action<DragDownDetails>? onPanDown = null,
        Action<DragStartDetails>? onPanStart = null,
        Action<DragUpdateDetails>? onPanUpdate = null,
        Action<DragEndDetails>? onPanEnd = null,
        Action? onPanCancel = null,
        Action<ScaleStartDetails>? onScaleStart = null,
        Action<ScaleUpdateDetails>? onScaleUpdate = null,
        Action<ScaleEndDetails>? onScaleEnd = null,
        HitTestBehavior? behavior = null,
        bool excludeFromSemantics = false,
        DragStartBehavior dragStartBehavior = DragStartBehavior.start,
        bool trackpadScrollCausesScale = false,
        Offset? trackpadScrollToScaleFactor = null,
        HashSet<PointerDeviceKind>? supportedDevices = null
    )
        : base(key: key)
    {
        Offset __trackpadScrollToScaleFactor =
            trackpadScrollToScaleFactor ?? ScaleLibrary.kDefaultTrackpadScrollToScaleFactor;
        this.child = child;
        this.onTapDown = onTapDown;
        this.onTapUp = onTapUp;
        this.onTap = onTap;
        this.onTapMove = onTapMove;
        this.onTapCancel = onTapCancel;
        this.onSecondaryTap = onSecondaryTap;
        this.onSecondaryTapDown = onSecondaryTapDown;
        this.onSecondaryTapUp = onSecondaryTapUp;
        this.onSecondaryTapCancel = onSecondaryTapCancel;
        this.onTertiaryTapDown = onTertiaryTapDown;
        this.onTertiaryTapUp = onTertiaryTapUp;
        this.onTertiaryTapCancel = onTertiaryTapCancel;
        this.onDoubleTapDown = onDoubleTapDown;
        this.onDoubleTap = onDoubleTap;
        this.onDoubleTapCancel = onDoubleTapCancel;
        this.onLongPressDown = onLongPressDown;
        this.onLongPressCancel = onLongPressCancel;
        this.onLongPress = onLongPress;
        this.onLongPressStart = onLongPressStart;
        this.onLongPressMoveUpdate = onLongPressMoveUpdate;
        this.onLongPressUp = onLongPressUp;
        this.onLongPressEnd = onLongPressEnd;
        this.onSecondaryLongPressDown = onSecondaryLongPressDown;
        this.onSecondaryLongPressCancel = onSecondaryLongPressCancel;
        this.onSecondaryLongPress = onSecondaryLongPress;
        this.onSecondaryLongPressStart = onSecondaryLongPressStart;
        this.onSecondaryLongPressMoveUpdate = onSecondaryLongPressMoveUpdate;
        this.onSecondaryLongPressUp = onSecondaryLongPressUp;
        this.onSecondaryLongPressEnd = onSecondaryLongPressEnd;
        this.onTertiaryLongPressDown = onTertiaryLongPressDown;
        this.onTertiaryLongPressCancel = onTertiaryLongPressCancel;
        this.onTertiaryLongPress = onTertiaryLongPress;
        this.onTertiaryLongPressStart = onTertiaryLongPressStart;
        this.onTertiaryLongPressMoveUpdate = onTertiaryLongPressMoveUpdate;
        this.onTertiaryLongPressUp = onTertiaryLongPressUp;
        this.onTertiaryLongPressEnd = onTertiaryLongPressEnd;
        this.onVerticalDragDown = onVerticalDragDown;
        this.onVerticalDragStart = onVerticalDragStart;
        this.onVerticalDragUpdate = onVerticalDragUpdate;
        this.onVerticalDragEnd = onVerticalDragEnd;
        this.onVerticalDragCancel = onVerticalDragCancel;
        this.onHorizontalDragDown = onHorizontalDragDown;
        this.onHorizontalDragStart = onHorizontalDragStart;
        this.onHorizontalDragUpdate = onHorizontalDragUpdate;
        this.onHorizontalDragEnd = onHorizontalDragEnd;
        this.onHorizontalDragCancel = onHorizontalDragCancel;
        this.onForcePressStart = onForcePressStart;
        this.onForcePressPeak = onForcePressPeak;
        this.onForcePressUpdate = onForcePressUpdate;
        this.onForcePressEnd = onForcePressEnd;
        this.onPanDown = onPanDown;
        this.onPanStart = onPanStart;
        this.onPanUpdate = onPanUpdate;
        this.onPanEnd = onPanEnd;
        this.onPanCancel = onPanCancel;
        this.onScaleStart = onScaleStart;
        this.onScaleUpdate = onScaleUpdate;
        this.onScaleEnd = onScaleEnd;
        this.behavior = behavior;
        this.excludeFromSemantics = excludeFromSemantics;
        this.dragStartBehavior = dragStartBehavior;
        this.trackpadScrollCausesScale = trackpadScrollCausesScale;
        this.trackpadScrollToScaleFactor = __trackpadScrollToScaleFactor;
        this.supportedDevices = supportedDevices;
        System.Diagnostics.Debug.Assert(
            (
                (Func<bool>)(
                    () =>
                    {
                        bool haveVerticalDrag =
                            (onVerticalDragStart is not null)
                            || (onVerticalDragUpdate is not null)
                            || (onVerticalDragEnd is not null);
                        bool haveHorizontalDrag =
                            (onHorizontalDragStart is not null)
                            || (onHorizontalDragUpdate is not null)
                            || (onHorizontalDragEnd is not null);
                        bool havePan =
                            (onPanStart is not null)
                            || (onPanUpdate is not null)
                            || (onPanEnd is not null);
                        bool haveScale =
                            (onScaleStart is not null)
                            || (onScaleUpdate is not null)
                            || (onScaleEnd is not null);
                        if (havePan || haveScale)
                        {
                            if (havePan && haveScale)
                            {
                                throw DartRuntimePrimitives.AsException(
                                    new FlutterError(
                                        new List<DiagnosticsNode>
                                        {
                                            new ErrorSummary(
                                                "Incorrect GestureDetector arguments."
                                            ),
                                            new ErrorDescription(
                                                "Having both a pan gesture recognizer and a scale gesture recognizer is redundant; scale is a superset of pan."
                                            ),
                                            new ErrorHint("Just use the scale gesture recognizer."),
                                        }
                                    )
                                );
                            }
                            var recognizer = havePan ? "pan" : "scale";
                            if (haveVerticalDrag && haveHorizontalDrag)
                            {
                                throw DartRuntimePrimitives.AsException(
                                    FlutterError.Create(
                                        "Incorrect GestureDetector arguments.\n"
                                            + $"Simultaneously having a vertical drag gesture recognizer, a horizontal drag gesture recognizer, and a {recognizer} gesture recognizer "
                                            + $"will result in the {recognizer} gesture recognizer being ignored, since the other two will catch all drags."
                                    )
                                );
                            }
                        }
                        return true;
                        throw new InvalidOperationException(
                            "Callback completed without returning a value."
                        );
                    }
                )
            )()
        );
    }

    public override Widget build(BuildContext context)
    {
        var gesturesLocal = new DartMap<Type, dynamic>();
        DeviceGestureSettings? gestureSettingsLocal = MediaQuery.maybeGestureSettingsOf(context);
        ScrollBehavior configuration = ScrollConfiguration.of(context);
        if (
            (onTapDown is not null)
            || (onTapUp is not null)
            || (onTap is not null)
            || (onTapCancel is not null)
            || (onSecondaryTap is not null)
            || (onSecondaryTapDown is not null)
            || (onSecondaryTapUp is not null)
            || (onSecondaryTapCancel is not null)
            || (onTertiaryTapDown is not null)
            || (onTertiaryTapUp is not null)
            || (onTertiaryTapCancel is not null)
        )
        {
            gesturesLocal[typeof(TapGestureRecognizer)] =
                new GestureRecognizerFactoryWithHandlers<TapGestureRecognizer>(
                    () =>
                        new TapGestureRecognizer(
                            debugOwner: this,
                            supportedDevices: supportedDevices
                        ),
                    (instance) =>
                    {
                        DartRuntimePrimitives.Ignore(
                            (
                                (Func<TapGestureRecognizer>)(
                                    () =>
                                    {
                                        var __cascade = instance;
                                        __cascade.onTapDown = onTapDown;
                                        __cascade.onTapUp = onTapUp;
                                        __cascade.onTap = onTap;
                                        __cascade.onTapCancel = onTapCancel;
                                        __cascade.onSecondaryTap = onSecondaryTap;
                                        __cascade.onSecondaryTapDown = onSecondaryTapDown;
                                        __cascade.onSecondaryTapUp = onSecondaryTapUp;
                                        __cascade.onSecondaryTapCancel = onSecondaryTapCancel;
                                        __cascade.onTertiaryTapDown = onTertiaryTapDown;
                                        __cascade.onTertiaryTapUp = onTertiaryTapUp;
                                        __cascade.onTertiaryTapCancel = onTertiaryTapCancel;
                                        __cascade.gestureSettings = gestureSettingsLocal;
                                        __cascade.supportedDevices = supportedDevices;
                                        return __cascade;
                                    }
                                )
                            )()
                        );
                    }
                );
        }
        if (
            (onDoubleTap is not null)
            || (onDoubleTapDown is not null)
            || (onDoubleTapCancel is not null)
        )
        {
            gesturesLocal[typeof(DoubleTapGestureRecognizer)] =
                new GestureRecognizerFactoryWithHandlers<DoubleTapGestureRecognizer>(
                    () =>
                        new DoubleTapGestureRecognizer(
                            debugOwner: this,
                            supportedDevices: supportedDevices
                        ),
                    (instance) =>
                    {
                        DartRuntimePrimitives.Ignore(
                            (
                                (Func<DoubleTapGestureRecognizer>)(
                                    () =>
                                    {
                                        var __cascade = instance;
                                        __cascade.onDoubleTapDown = onDoubleTapDown;
                                        __cascade.onDoubleTap = onDoubleTap;
                                        __cascade.onDoubleTapCancel = onDoubleTapCancel;
                                        __cascade.gestureSettings = gestureSettingsLocal;
                                        __cascade.supportedDevices = supportedDevices;
                                        return __cascade;
                                    }
                                )
                            )()
                        );
                    }
                );
        }
        if (
            (onLongPressDown is not null)
            || (onLongPressCancel is not null)
            || (onLongPress is not null)
            || (onLongPressStart is not null)
            || (onLongPressMoveUpdate is not null)
            || (onLongPressUp is not null)
            || (onLongPressEnd is not null)
            || (onSecondaryLongPressDown is not null)
            || (onSecondaryLongPressCancel is not null)
            || (onSecondaryLongPress is not null)
            || (onSecondaryLongPressStart is not null)
            || (onSecondaryLongPressMoveUpdate is not null)
            || (onSecondaryLongPressUp is not null)
            || (onSecondaryLongPressEnd is not null)
            || (onTertiaryLongPressDown is not null)
            || (onTertiaryLongPressCancel is not null)
            || (onTertiaryLongPress is not null)
            || (onTertiaryLongPressStart is not null)
            || (onTertiaryLongPressMoveUpdate is not null)
            || (onTertiaryLongPressUp is not null)
            || (onTertiaryLongPressEnd is not null)
        )
        {
            gesturesLocal[typeof(LongPressGestureRecognizer)] =
                new GestureRecognizerFactoryWithHandlers<LongPressGestureRecognizer>(
                    () =>
                        new LongPressGestureRecognizer(
                            debugOwner: this,
                            supportedDevices: supportedDevices
                        ),
                    (instance) =>
                    {
                        DartRuntimePrimitives.Ignore(
                            (
                                (Func<LongPressGestureRecognizer>)(
                                    () =>
                                    {
                                        var __cascade = instance;
                                        __cascade.onLongPressDown = onLongPressDown;
                                        __cascade.onLongPressCancel = onLongPressCancel;
                                        __cascade.onLongPress = onLongPress;
                                        __cascade.onLongPressStart = onLongPressStart;
                                        __cascade.onLongPressMoveUpdate = onLongPressMoveUpdate;
                                        __cascade.onLongPressUp = onLongPressUp;
                                        __cascade.onLongPressEnd = onLongPressEnd;
                                        __cascade.onSecondaryLongPressDown =
                                            onSecondaryLongPressDown;
                                        __cascade.onSecondaryLongPressCancel =
                                            onSecondaryLongPressCancel;
                                        __cascade.onSecondaryLongPress = onSecondaryLongPress;
                                        __cascade.onSecondaryLongPressStart =
                                            onSecondaryLongPressStart;
                                        __cascade.onSecondaryLongPressMoveUpdate =
                                            onSecondaryLongPressMoveUpdate;
                                        __cascade.onSecondaryLongPressUp = onSecondaryLongPressUp;
                                        __cascade.onSecondaryLongPressEnd = onSecondaryLongPressEnd;
                                        __cascade.onTertiaryLongPressDown = onTertiaryLongPressDown;
                                        __cascade.onTertiaryLongPressCancel =
                                            onTertiaryLongPressCancel;
                                        __cascade.onTertiaryLongPress = onTertiaryLongPress;
                                        __cascade.onTertiaryLongPressStart =
                                            onTertiaryLongPressStart;
                                        __cascade.onTertiaryLongPressMoveUpdate =
                                            onTertiaryLongPressMoveUpdate;
                                        __cascade.onTertiaryLongPressUp = onTertiaryLongPressUp;
                                        __cascade.onTertiaryLongPressEnd = onTertiaryLongPressEnd;
                                        __cascade.gestureSettings = gestureSettingsLocal;
                                        __cascade.supportedDevices = supportedDevices;
                                        return __cascade;
                                    }
                                )
                            )()
                        );
                    }
                );
        }
        if (
            (onVerticalDragDown is not null)
            || (onVerticalDragStart is not null)
            || (onVerticalDragUpdate is not null)
            || (onVerticalDragEnd is not null)
            || (onVerticalDragCancel is not null)
        )
        {
            gesturesLocal[typeof(VerticalDragGestureRecognizer)] =
                new GestureRecognizerFactoryWithHandlers<VerticalDragGestureRecognizer>(
                    () =>
                        new VerticalDragGestureRecognizer(
                            debugOwner: this,
                            supportedDevices: supportedDevices
                        ),
                    (instance) =>
                    {
                        DartRuntimePrimitives.Ignore(
                            (
                                (Func<VerticalDragGestureRecognizer>)(
                                    () =>
                                    {
                                        var __cascade = instance;
                                        __cascade.onDown = onVerticalDragDown;
                                        __cascade.onStart = onVerticalDragStart;
                                        __cascade.onUpdate = onVerticalDragUpdate;
                                        __cascade.onEnd = onVerticalDragEnd;
                                        __cascade.onCancel = onVerticalDragCancel;
                                        __cascade.dragStartBehavior = dragStartBehavior;
                                        __cascade.multitouchDragStrategy =
                                            configuration.getMultitouchDragStrategy(context);
                                        __cascade.gestureSettings = gestureSettingsLocal;
                                        __cascade.supportedDevices = supportedDevices;
                                        return __cascade;
                                    }
                                )
                            )()
                        );
                    }
                );
        }
        if (
            (onHorizontalDragDown is not null)
            || (onHorizontalDragStart is not null)
            || (onHorizontalDragUpdate is not null)
            || (onHorizontalDragEnd is not null)
            || (onHorizontalDragCancel is not null)
        )
        {
            gesturesLocal[typeof(HorizontalDragGestureRecognizer)] =
                new GestureRecognizerFactoryWithHandlers<HorizontalDragGestureRecognizer>(
                    () =>
                        new HorizontalDragGestureRecognizer(
                            debugOwner: this,
                            supportedDevices: supportedDevices
                        ),
                    (instance) =>
                    {
                        DartRuntimePrimitives.Ignore(
                            (
                                (Func<HorizontalDragGestureRecognizer>)(
                                    () =>
                                    {
                                        var __cascade = instance;
                                        __cascade.onDown = onHorizontalDragDown;
                                        __cascade.onStart = onHorizontalDragStart;
                                        __cascade.onUpdate = onHorizontalDragUpdate;
                                        __cascade.onEnd = onHorizontalDragEnd;
                                        __cascade.onCancel = onHorizontalDragCancel;
                                        __cascade.dragStartBehavior = dragStartBehavior;
                                        __cascade.multitouchDragStrategy =
                                            configuration.getMultitouchDragStrategy(context);
                                        __cascade.gestureSettings = gestureSettingsLocal;
                                        __cascade.supportedDevices = supportedDevices;
                                        return __cascade;
                                    }
                                )
                            )()
                        );
                    }
                );
        }
        if (
            (onPanDown is not null)
            || (onPanStart is not null)
            || (onPanUpdate is not null)
            || (onPanEnd is not null)
            || (onPanCancel is not null)
        )
        {
            gesturesLocal[typeof(PanGestureRecognizer)] =
                new GestureRecognizerFactoryWithHandlers<PanGestureRecognizer>(
                    () =>
                        new PanGestureRecognizer(
                            debugOwner: this,
                            supportedDevices: supportedDevices
                        ),
                    (instance) =>
                    {
                        DartRuntimePrimitives.Ignore(
                            (
                                (Func<PanGestureRecognizer>)(
                                    () =>
                                    {
                                        var __cascade = instance;
                                        __cascade.onDown = onPanDown;
                                        __cascade.onStart = onPanStart;
                                        __cascade.onUpdate = onPanUpdate;
                                        __cascade.onEnd = onPanEnd;
                                        __cascade.onCancel = onPanCancel;
                                        __cascade.dragStartBehavior = dragStartBehavior;
                                        __cascade.multitouchDragStrategy =
                                            configuration.getMultitouchDragStrategy(context);
                                        __cascade.gestureSettings = gestureSettingsLocal;
                                        __cascade.supportedDevices = supportedDevices;
                                        return __cascade;
                                    }
                                )
                            )()
                        );
                    }
                );
        }
        if ((onScaleStart is not null) || (onScaleUpdate is not null) || (onScaleEnd is not null))
        {
            gesturesLocal[typeof(ScaleGestureRecognizer)] =
                new GestureRecognizerFactoryWithHandlers<ScaleGestureRecognizer>(
                    () =>
                        new ScaleGestureRecognizer(
                            debugOwner: this,
                            supportedDevices: supportedDevices
                        ),
                    (instance) =>
                    {
                        DartRuntimePrimitives.Ignore(
                            (
                                (Func<ScaleGestureRecognizer>)(
                                    () =>
                                    {
                                        var __cascade = instance;
                                        __cascade.onStart = onScaleStart;
                                        __cascade.onUpdate = onScaleUpdate;
                                        __cascade.onEnd = onScaleEnd;
                                        __cascade.dragStartBehavior = dragStartBehavior;
                                        __cascade.gestureSettings = gestureSettingsLocal;
                                        __cascade.trackpadScrollCausesScale =
                                            trackpadScrollCausesScale;
                                        __cascade.trackpadScrollToScaleFactor =
                                            trackpadScrollToScaleFactor;
                                        __cascade.supportedDevices = supportedDevices;
                                        return __cascade;
                                    }
                                )
                            )()
                        );
                    }
                );
        }
        if (
            (onForcePressStart is not null)
            || (onForcePressPeak is not null)
            || (onForcePressUpdate is not null)
            || (onForcePressEnd is not null)
        )
        {
            gesturesLocal[typeof(ForcePressGestureRecognizer)] =
                new GestureRecognizerFactoryWithHandlers<ForcePressGestureRecognizer>(
                    () =>
                        new ForcePressGestureRecognizer(
                            debugOwner: this,
                            supportedDevices: supportedDevices
                        ),
                    (instance) =>
                    {
                        DartRuntimePrimitives.Ignore(
                            (
                                (Func<ForcePressGestureRecognizer>)(
                                    () =>
                                    {
                                        var __cascade = instance;
                                        __cascade.onStart = onForcePressStart;
                                        __cascade.onPeak = onForcePressPeak;
                                        __cascade.onUpdate = onForcePressUpdate;
                                        __cascade.onEnd = onForcePressEnd;
                                        __cascade.gestureSettings = gestureSettingsLocal;
                                        __cascade.supportedDevices = supportedDevices;
                                        return __cascade;
                                    }
                                )
                            )()
                        );
                    }
                );
        }
        return new RawGestureDetector(
            gestures: gesturesLocal,
            behavior: behavior,
            excludeFromSemantics: excludeFromSemantics,
            child: child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<DragStartBehavior>("startBehavior", dragStartBehavior));
    }
}

public class RawGestureDetector : StatefulWidget
{
    public virtual Widget? child { get; private set; }
    public virtual DartMap<Type, dynamic> gestures { get; private set; } = default!;
    public virtual HitTestBehavior? behavior { get; private set; }
    public virtual bool excludeFromSemantics { get; private set; } = default!;
    public virtual SemanticsGestureDelegate? semantics { get; private set; }

    public RawGestureDetector(
        Key? key = null,
        Widget? child = null,
        DartMap<Type, dynamic> gestures = default!,
        HitTestBehavior? behavior = null,
        bool excludeFromSemantics = false,
        SemanticsGestureDelegate? semantics = null
    )
        : base(key: key)
    {
        DartMap<Type, dynamic> __gestures =
            gestures
            ?? new DartMap<Type, GestureRecognizerFactory<GestureRecognizer>>().cast<
                Type,
                dynamic
            >();
        this.child = child;
        this.gestures = __gestures;
        this.behavior = behavior;
        this.excludeFromSemantics = excludeFromSemantics;
        this.semantics = semantics;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new RawGestureDetectorState());
}

public class RawGestureDetectorState : State<RawGestureDetector>
{
    internal virtual DartMap<Type, GestureRecognizer>? _recognizers { get; set; } =
        new DartMap<Type, GestureRecognizer>();
    internal virtual SemanticsGestureDelegate? _semantics { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _semantics =
            widget.semantics ?? new _DefaultSemanticsGestureDelegate__gesture_detector(this);
        _syncAll(widget.gestures);
    }

    public override void didUpdateWidget(RawGestureDetector oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!((oldWidget.semantics is null) && (widget.semantics is null)))
        {
            _semantics =
                widget.semantics ?? new _DefaultSemanticsGestureDelegate__gesture_detector(this);
        }
        _syncAll(widget.gestures);
    }

    public virtual void replaceGestureRecognizers(DartMap<Type, dynamic> gestures)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (!context.findRenderObject()!.owner!.debugDoingLayout)
            {
                throw DartRuntimePrimitives.AsException(
                    new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            new ErrorSummary(
                                "Unexpected call to replaceGestureRecognizers() method of RawGestureDetectorState."
                            ),
                            new ErrorDescription(
                                "The replaceGestureRecognizers() method can only be called during the layout phase."
                            ),
                            new ErrorHint(
                                "To set the gesture recognizers at other times, trigger a new build using setState() "
                                    + "and provide the new gesture recognizers as constructor arguments to the corresponding "
                                    + "RawGestureDetector or GestureDetector object."
                            ),
                        }
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        _syncAll(gestures);
        if (!widget.excludeFromSemantics)
        {
            var semanticsGestureHandler = (
                (RenderSemanticsGestureHandler?)context.findRenderObject()!
            )!;
            _updateSemanticsForRenderObject(semanticsGestureHandler);
        }
    }

    public virtual void replaceSemanticsActions(HashSet<SemanticsAction> actions)
    {
        if (widget.excludeFromSemantics)
        {
            return;
        }
        var semanticsGestureHandler = ((RenderSemanticsGestureHandler?)context.findRenderObject())!;
        DartRuntimePrimitives.Assert(() =>
        {
            if (semanticsGestureHandler is null)
            {
                throw DartRuntimePrimitives.AsException(
                    FlutterError.Create(
                        "Unexpected call to replaceSemanticsActions() method of RawGestureDetectorState.\n"
                            + "The replaceSemanticsActions() method can only be called after the RenderSemanticsGestureHandler has been created."
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        semanticsGestureHandler!.validActions = actions;
    }

    public override void dispose()
    {
        foreach (GestureRecognizer recognizer in _recognizers!.Values)
        {
            recognizer.dispose();
        }
        _recognizers = null;
        base.dispose();
    }

    internal virtual void _syncAll(DartMap<Type, dynamic> gestures)
    {
        DartRuntimePrimitives.Assert(() => _recognizers is not null);
        DartMap<Type, GestureRecognizer> oldRecognizers = _recognizers!;
        _recognizers = new DartMap<Type, GestureRecognizer>().cast<Type, GestureRecognizer>();
        foreach (Type @type in gestures.Keys)
        {
            DartRuntimePrimitives.Assert(() => gestures.ContainsKey(@type));
            var factory = (GestureRecognizerFactoryBase)(object)gestures.GetValueOrDefault(@type)!;
            DartRuntimePrimitives.Assert(() => factory._debugAssertTypeMatches(@type));
            DartRuntimePrimitives.Assert(() => !_recognizers!.ContainsKey(@type));
            _recognizers![@type] =
                oldRecognizers.GetValueOrDefault(@type) ?? factory.createRecognizer();
            DartRuntimePrimitives.Assert(
                () =>
                    Equals(
                        DartRuntimePrimitives.RuntimeType(_recognizers!.GetValueOrDefault(@type)),
                        @type
                    ),
                () =>
                    (object?)
                        $"GestureRecognizerFactory of type {@type} created a GestureRecognizer of type {DartRuntimePrimitives.RuntimeType(_recognizers!.GetValueOrDefault(@type))}. The GestureRecognizerFactory must be specialized with the type of the class that it returns from its constructor method."
            );
            factory.initializeRecognizer(_recognizers!.GetValueOrDefault(@type)!);
        }
        foreach (Type typeLocal in oldRecognizers.Keys)
        {
            if (!_recognizers!.ContainsKey(typeLocal))
            {
                oldRecognizers.GetValueOrDefault(typeLocal)!.dispose();
            }
        }
    }

    internal virtual void _handlePointerDown(Gestures.PointerDownEvent @event)
    {
        DartRuntimePrimitives.Assert(() => _recognizers is not null);
        foreach (GestureRecognizer recognizer in _recognizers!.Values)
        {
            recognizer.addPointer(@event);
        }
    }

    internal virtual void _handlePointerPanZoomStart(PointerPanZoomStartEvent @event)
    {
        DartRuntimePrimitives.Assert(() => _recognizers is not null);
        foreach (GestureRecognizer recognizer in _recognizers!.Values)
        {
            recognizer.addPointerPanZoom(@event);
        }
    }

    internal virtual HitTestBehavior _defaultBehavior
    {
        get
        {
            return (widget.child is null)
                ? HitTestBehavior.translucent
                : HitTestBehavior.deferToChild;
        }
    }

    internal virtual void _updateSemanticsForRenderObject(
        RenderSemanticsGestureHandler renderObject
    )
    {
        DartRuntimePrimitives.Assert(() => !widget.excludeFromSemantics);
        DartRuntimePrimitives.Assert(() => _semantics is not null);
        _semantics!.assignSemantics(renderObject);
    }

    public override Widget build(BuildContext context)
    {
        Widget result = new Listener(
            onPointerDown: _handlePointerDown,
            onPointerPanZoomStart: _handlePointerPanZoomStart,
            behavior: widget.behavior ?? _defaultBehavior,
            child: widget.child
        );
        if (!widget.excludeFromSemantics)
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(
                new _GestureSemantics__gesture_detector(
                    behavior: widget.behavior ?? _defaultBehavior,
                    assignSemantics: _updateSemanticsForRenderObject,
                    child: result
                )
            );
        }
        return result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        if (_recognizers is null)
        {
            properties.add(DiagnosticsNode.CreateMessage("DISPOSED"));
        }
        else
        {
            List<string> gestures = _recognizers!
                .Values.map((recognizer) => recognizer.debugDescription)
                .ToList()
                .ToList();
            properties.add(
                new IterableProperty<string>("gestures", gestures.Cast<string>(), ifEmpty: "<none>")
            );
            properties.add(
                new IterableProperty<GestureRecognizer>(
                    "recognizers",
                    _recognizers!.Values.Cast<GestureRecognizer>(),
                    level: DiagnosticLevel.fine
                )
            );
            properties.add(
                new DiagnosticsProperty<bool>(
                    "excludeFromSemantics",
                    widget.excludeFromSemantics,
                    defaultValue: false
                )
            );
            if (!widget.excludeFromSemantics)
            {
                properties.add(
                    new DiagnosticsProperty<SemanticsGestureDelegate>(
                        "semantics",
                        widget.semantics,
                        defaultValue: null
                    )
                );
            }
        }
        properties.add(
            new EnumProperty<HitTestBehavior>("behavior", widget.behavior, defaultValue: null)
        );
    }
}

internal delegate void _AssignSemantics__gesture_detector(RenderSemanticsGestureHandler __unused0);

internal class _GestureSemantics__gesture_detector : SingleChildRenderObjectWidget
{
    public virtual HitTestBehavior behavior { get; private set; } = default!;
    public virtual Action<RenderSemanticsGestureHandler> assignSemantics { get; private set; } =
        default!;

    internal _GestureSemantics__gesture_detector(
        Widget? child = null,
        HitTestBehavior behavior = default!,
        Action<RenderSemanticsGestureHandler> assignSemantics = default!
    )
        : base(child: child)
    {
        this.behavior = behavior;
        this.assignSemantics = assignSemantics;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        var renderObject = (
            (Func<RenderSemanticsGestureHandler>)(
                () =>
                {
                    var __cascade = new RenderSemanticsGestureHandler();
                    __cascade.behavior = behavior;
                    return __cascade;
                }
            )
        )();
        assignSemantics(renderObject);
        return renderObject;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderSemanticsGestureHandler)renderObject;
        __renderObject.behavior = behavior;
        assignSemantics(__renderObject);
    }
}

public abstract class SemanticsGestureDelegate
{
    protected SemanticsGestureDelegate() { }

    public abstract void assignSemantics(RenderSemanticsGestureHandler renderObject);

    public override string ToString() =>
        $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "SemanticsGestureDelegate")}()";
}

internal class _DefaultSemanticsGestureDelegate__gesture_detector : SemanticsGestureDelegate
{
    public virtual RawGestureDetectorState detectorState { get; private set; } = default!;

    internal _DefaultSemanticsGestureDelegate__gesture_detector(
        RawGestureDetectorState detectorState
    )
    {
        this.detectorState = detectorState;
    }

    internal static Rect _getLocalRectFromRenderObject(RenderObject renderObject)
    {
        if (renderObject is not RenderBox)
        {
            return Rect.zero;
        }
        Size sizeLocal = DartRuntimePrimitives.ConvertValue<Size>(((RenderBox)renderObject).size);
        return Rect.fromLTWH(0, 0, sizeLocal.width, sizeLocal.height);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static Offset _transformOffsetToGlobal(RenderObject @object, Offset local)
    {
        Matrix4 transform = @object.getTransformTo(null);
        return MatrixUtils.transformPoint(transform, local);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void assignSemantics(RenderSemanticsGestureHandler renderObject)
    {
        DartRuntimePrimitives.Assert(() => !detectorState.widget.excludeFromSemantics);
        DartMap<Type, GestureRecognizer> recognizers = detectorState._recognizers!;
        DartRuntimePrimitives.Ignore(
            (
                (Func<RenderSemanticsGestureHandler>)(
                    () =>
                    {
                        var __cascade = renderObject;
                        __cascade.onTap = _getTapHandler(renderObject, recognizers);
                        __cascade.onLongPress = _getLongPressHandler(renderObject, recognizers);
                        __cascade.onHorizontalDragUpdate = _getHorizontalDragUpdateHandler(
                            renderObject,
                            recognizers
                        );
                        __cascade.onVerticalDragUpdate = _getVerticalDragUpdateHandler(
                            renderObject,
                            recognizers
                        );
                        return __cascade;
                    }
                )
            )()
        );
    }

    internal virtual Action? _getTapHandler(
        RenderObject renderObject,
        DartMap<Type, GestureRecognizer> recognizers
    )
    {
        var tap = (
            (TapGestureRecognizer?)recognizers.GetValueOrDefault(typeof(TapGestureRecognizer))
        )!;
        if (tap is null)
        {
            return null;
        }
        return () =>
        {
            Offset localCenter = DartRuntimePrimitives.ConvertValue<Offset>(
                _getLocalRectFromRenderObject(renderObject).center
            );
            Offset globalCenter = DartRuntimePrimitives.ConvertValue<Offset>(
                _transformOffsetToGlobal(renderObject, localCenter)
            );
            tap.onTapDown?.Invoke(
                new TapDownDetails(
                    globalPosition: globalCenter,
                    localPosition: localCenter,
                    kind: PointerDeviceKind.unknown
                )
            );
            tap.onTapUp?.Invoke(
                new TapUpDetails(
                    globalPosition: globalCenter,
                    localPosition: localCenter,
                    kind: PointerDeviceKind.unknown
                )
            );
            tap.onTap?.Invoke();
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Action? _getLongPressHandler(
        RenderObject renderObject,
        DartMap<Type, GestureRecognizer> recognizers
    )
    {
        var longPress = (
            (LongPressGestureRecognizer?)
                recognizers.GetValueOrDefault(typeof(LongPressGestureRecognizer))
        )!;
        if (longPress is null)
        {
            return null;
        }
        return () =>
        {
            Offset localCenter = DartRuntimePrimitives.ConvertValue<Offset>(
                _getLocalRectFromRenderObject(renderObject).center
            );
            Offset globalCenter = DartRuntimePrimitives.ConvertValue<Offset>(
                _transformOffsetToGlobal(renderObject, localCenter)
            );
            longPress.onLongPressDown?.Invoke(
                new LongPressDownDetails(localPosition: localCenter, globalPosition: globalCenter)
            );
            longPress.onLongPressStart?.Invoke(
                new LongPressStartDetails(localPosition: localCenter, globalPosition: globalCenter)
            );
            longPress.onLongPress?.Invoke();
            longPress.onLongPressEnd?.Invoke(
                new LongPressEndDetails(localPosition: localCenter, globalPosition: globalCenter)
            );
            longPress.onLongPressUp?.Invoke();
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Action<DragUpdateDetails>? _getHorizontalDragUpdateHandler(
        RenderObject renderObject,
        DartMap<Type, GestureRecognizer> recognizers
    )
    {
        var horizontal = (
            (HorizontalDragGestureRecognizer?)
                recognizers.GetValueOrDefault(typeof(HorizontalDragGestureRecognizer))
        )!;
        var pan = (
            (PanGestureRecognizer?)recognizers.GetValueOrDefault(typeof(PanGestureRecognizer))
        )!;
        Action<DragUpdateDetails>? horizontalHandler = DartRuntimePrimitives.ConvertValue<
            Action<DragUpdateDetails>
        >(
            (Action<DragUpdateDetails>?)(
                (horizontal is null)
                    ? null
                    : (
                        (details) =>
                        {
                            Offset localCenter = DartRuntimePrimitives.ConvertValue<Offset>(
                                _getLocalRectFromRenderObject(renderObject).center
                            );
                            Offset globalCenter = DartRuntimePrimitives.ConvertValue<Offset>(
                                _transformOffsetToGlobal(renderObject, localCenter)
                            );
                            Offset newLocalOffset = localCenter + details.delta;
                            Offset newGlobalOffset = DartRuntimePrimitives.ConvertValue<Offset>(
                                _transformOffsetToGlobal(renderObject, newLocalOffset)
                            );
                            horizontal.onDown?.Invoke(
                                new DragDownDetails(
                                    localPosition: localCenter,
                                    globalPosition: globalCenter
                                )
                            );
                            horizontal.onStart?.Invoke(
                                new DragStartDetails(
                                    localPosition: localCenter,
                                    globalPosition: globalCenter
                                )
                            );
                            horizontal.onUpdate?.Invoke(details);
                            horizontal.onEnd?.Invoke(
                                new DragEndDetails(
                                    primaryVelocity: 0.0,
                                    localPosition: newLocalOffset,
                                    globalPosition: newGlobalOffset
                                )
                            );
                        }
                    )
            )
        );
        Action<DragUpdateDetails>? panHandler = DartRuntimePrimitives.ConvertValue<
            Action<DragUpdateDetails>
        >(
            (Action<DragUpdateDetails>?)(
                (pan is null)
                    ? null
                    : (
                        (details) =>
                        {
                            Offset localCenterLocal = DartRuntimePrimitives.ConvertValue<Offset>(
                                _getLocalRectFromRenderObject(renderObject).center
                            );
                            Offset globalCenterLocal = DartRuntimePrimitives.ConvertValue<Offset>(
                                _transformOffsetToGlobal(renderObject, localCenterLocal)
                            );
                            Offset newLocalOffsetLocal = localCenterLocal + details.delta;
                            Offset newGlobalOffsetLocal =
                                DartRuntimePrimitives.ConvertValue<Offset>(
                                    _transformOffsetToGlobal(renderObject, newLocalOffsetLocal)
                                );
                            pan.onDown?.Invoke(
                                new DragDownDetails(
                                    localPosition: localCenterLocal,
                                    globalPosition: globalCenterLocal
                                )
                            );
                            pan.onStart?.Invoke(
                                new DragStartDetails(
                                    localPosition: localCenterLocal,
                                    globalPosition: globalCenterLocal
                                )
                            );
                            pan.onUpdate?.Invoke(details);
                            pan.onEnd?.Invoke(
                                new DragEndDetails(
                                    localPosition: newLocalOffsetLocal,
                                    globalPosition: newGlobalOffsetLocal
                                )
                            );
                        }
                    )
            )
        );
        if ((horizontalHandler is null) && (panHandler is null))
        {
            return null;
        }
        return (details) =>
        {
            horizontalHandler?.Invoke(details);
            panHandler?.Invoke(details);
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Action<DragUpdateDetails>? _getVerticalDragUpdateHandler(
        RenderObject renderObject,
        DartMap<Type, GestureRecognizer> recognizers
    )
    {
        var vertical = (
            (VerticalDragGestureRecognizer?)
                recognizers.GetValueOrDefault(typeof(VerticalDragGestureRecognizer))
        )!;
        var pan = (
            (PanGestureRecognizer?)recognizers.GetValueOrDefault(typeof(PanGestureRecognizer))
        )!;
        Action<DragUpdateDetails>? verticalHandler = DartRuntimePrimitives.ConvertValue<
            Action<DragUpdateDetails>
        >(
            (Action<DragUpdateDetails>?)(
                (vertical is null)
                    ? null
                    : (
                        (details) =>
                        {
                            Offset localCenter = DartRuntimePrimitives.ConvertValue<Offset>(
                                _getLocalRectFromRenderObject(renderObject).center
                            );
                            Offset globalCenter = DartRuntimePrimitives.ConvertValue<Offset>(
                                _transformOffsetToGlobal(renderObject, localCenter)
                            );
                            Offset newLocalOffset = localCenter + details.delta;
                            Offset newGlobalOffset = DartRuntimePrimitives.ConvertValue<Offset>(
                                _transformOffsetToGlobal(renderObject, newLocalOffset)
                            );
                            vertical.onDown?.Invoke(
                                new DragDownDetails(
                                    localPosition: localCenter,
                                    globalPosition: globalCenter
                                )
                            );
                            vertical.onStart?.Invoke(
                                new DragStartDetails(
                                    localPosition: localCenter,
                                    globalPosition: globalCenter
                                )
                            );
                            vertical.onUpdate?.Invoke(details);
                            vertical.onEnd?.Invoke(
                                new DragEndDetails(
                                    primaryVelocity: 0.0,
                                    localPosition: newLocalOffset,
                                    globalPosition: newGlobalOffset
                                )
                            );
                        }
                    )
            )
        );
        Action<DragUpdateDetails>? panHandler = DartRuntimePrimitives.ConvertValue<
            Action<DragUpdateDetails>
        >(
            (Action<DragUpdateDetails>?)(
                (pan is null)
                    ? null
                    : (
                        (details) =>
                        {
                            Offset localCenterLocal = DartRuntimePrimitives.ConvertValue<Offset>(
                                _getLocalRectFromRenderObject(renderObject).center
                            );
                            Offset globalCenterLocal = DartRuntimePrimitives.ConvertValue<Offset>(
                                _transformOffsetToGlobal(renderObject, localCenterLocal)
                            );
                            Offset newLocalOffsetLocal = localCenterLocal + details.delta;
                            Offset newGlobalOffsetLocal =
                                DartRuntimePrimitives.ConvertValue<Offset>(
                                    _transformOffsetToGlobal(renderObject, newLocalOffsetLocal)
                                );
                            pan.onDown?.Invoke(
                                new DragDownDetails(
                                    localPosition: localCenterLocal,
                                    globalPosition: globalCenterLocal
                                )
                            );
                            pan.onStart?.Invoke(
                                new DragStartDetails(
                                    localPosition: localCenterLocal,
                                    globalPosition: globalCenterLocal
                                )
                            );
                            pan.onUpdate?.Invoke(details);
                            pan.onEnd?.Invoke(
                                new DragEndDetails(
                                    localPosition: newLocalOffsetLocal,
                                    globalPosition: newGlobalOffsetLocal
                                )
                            );
                        }
                    )
            )
        );
        if ((verticalHandler is null) && (panHandler is null))
        {
            return null;
        }
        return (details) =>
        {
            verticalHandler?.Invoke(details);
            panHandler?.Invoke(details);
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
