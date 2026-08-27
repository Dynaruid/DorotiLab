// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/gesture_detector.dart
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

public abstract class GestureRecognizerFactoryBase
{
    internal abstract global::Doroti.Framework.Gestures.GestureRecognizer createRecognizer();
    internal abstract void initializeRecognizer(global::Doroti.Framework.Gestures.GestureRecognizer instance);
}

public abstract class GestureRecognizerFactory<T> : GestureRecognizerFactoryBase where T : global::Doroti.Framework.Gestures.GestureRecognizer
{
    protected GestureRecognizerFactory()
    {
    }

    public abstract T constructor();
    public abstract void initializer(T instance);
    internal override global::Doroti.Framework.Gestures.GestureRecognizer createRecognizer() => constructor();
    internal override void initializeRecognizer(global::Doroti.Framework.Gestures.GestureRecognizer instance) => initializer((T)instance);
    internal virtual bool _debugAssertTypeMatches(Type type)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(type, typeof(T))), () => (object?)$"GestureRecognizerFactory of type {typeof(T)} was used where type {type} was specified.");
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public delegate T GestureRecognizerFactoryConstructor<T>() where T : global::Doroti.Framework.Gestures.GestureRecognizer;

public delegate void GestureRecognizerFactoryInitializer<T>(T instance) where T : global::Doroti.Framework.Gestures.GestureRecognizer;

public class GestureRecognizerFactoryWithHandlers<T> : GestureRecognizerFactory<T> where T : global::Doroti.Framework.Gestures.GestureRecognizer
{
    internal virtual global::System.Func<T> _constructor { get; private set; } = default!;
    internal virtual global::System.Action<T> _initializer { get; private set; } = default!;

    public GestureRecognizerFactoryWithHandlers(global::System.Func<T> _constructor, global::System.Action<T> _initializer)
    {
        this._constructor = _constructor;
        this._initializer = _initializer;
    }

    public override T constructor() => this._constructor();
    public override void initializer(T instance) => this._initializer(instance);
}

public class GestureDetector : StatelessWidget
{
    public virtual Widget? child { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.TapDownDetails>? onTapDown { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.TapUpDetails>? onTapUp { get; private set; }
    public virtual global::System.Action? onTap { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.TapMoveDetails>? onTapMove { get; private set; }
    public virtual global::System.Action? onTapCancel { get; private set; }
    public virtual global::System.Action? onSecondaryTap { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.TapDownDetails>? onSecondaryTapDown { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.TapUpDetails>? onSecondaryTapUp { get; private set; }
    public virtual global::System.Action? onSecondaryTapCancel { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.TapDownDetails>? onTertiaryTapDown { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.TapUpDetails>? onTertiaryTapUp { get; private set; }
    public virtual global::System.Action? onTertiaryTapCancel { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.TapDownDetails>? onDoubleTapDown { get; private set; }
    public virtual global::System.Action? onDoubleTap { get; private set; }
    public virtual global::System.Action? onDoubleTapCancel { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.LongPressDownDetails>? onLongPressDown { get; private set; }
    public virtual global::System.Action? onLongPressCancel { get; private set; }
    public virtual global::System.Action? onLongPress { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.LongPressStartDetails>? onLongPressStart { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.LongPressMoveUpdateDetails>? onLongPressMoveUpdate { get; private set; }
    public virtual global::System.Action? onLongPressUp { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.LongPressEndDetails>? onLongPressEnd { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.LongPressDownDetails>? onSecondaryLongPressDown { get; private set; }
    public virtual global::System.Action? onSecondaryLongPressCancel { get; private set; }
    public virtual global::System.Action? onSecondaryLongPress { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.LongPressStartDetails>? onSecondaryLongPressStart { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.LongPressMoveUpdateDetails>? onSecondaryLongPressMoveUpdate { get; private set; }
    public virtual global::System.Action? onSecondaryLongPressUp { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.LongPressEndDetails>? onSecondaryLongPressEnd { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.LongPressDownDetails>? onTertiaryLongPressDown { get; private set; }
    public virtual global::System.Action? onTertiaryLongPressCancel { get; private set; }
    public virtual global::System.Action? onTertiaryLongPress { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.LongPressStartDetails>? onTertiaryLongPressStart { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.LongPressMoveUpdateDetails>? onTertiaryLongPressMoveUpdate { get; private set; }
    public virtual global::System.Action? onTertiaryLongPressUp { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.LongPressEndDetails>? onTertiaryLongPressEnd { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.DragDownDetails>? onVerticalDragDown { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>? onVerticalDragStart { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>? onVerticalDragUpdate { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>? onVerticalDragEnd { get; private set; }
    public virtual global::System.Action? onVerticalDragCancel { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.DragDownDetails>? onHorizontalDragDown { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>? onHorizontalDragStart { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>? onHorizontalDragUpdate { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>? onHorizontalDragEnd { get; private set; }
    public virtual global::System.Action? onHorizontalDragCancel { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.DragDownDetails>? onPanDown { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>? onPanStart { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>? onPanUpdate { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>? onPanEnd { get; private set; }
    public virtual global::System.Action? onPanCancel { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.ScaleStartDetails>? onScaleStart { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.ScaleUpdateDetails>? onScaleUpdate { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.ScaleEndDetails>? onScaleEnd { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.ForcePressDetails>? onForcePressStart { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.ForcePressDetails>? onForcePressPeak { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.ForcePressDetails>? onForcePressUpdate { get; private set; }
    public virtual global::System.Action<global::Doroti.Framework.Gestures.ForcePressDetails>? onForcePressEnd { get; private set; }
    public virtual global::Doroti.Framework.Rendering.HitTestBehavior? behavior { get; private set; }
    public virtual bool excludeFromSemantics { get; private set; } = default!;
    public virtual global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual HashSet<PointerDeviceKind>? supportedDevices { get; private set; }
    public virtual bool trackpadScrollCausesScale { get; private set; } = default!;
    public virtual Offset trackpadScrollToScaleFactor { get; private set; } = default!;

    public GestureDetector(global::Doroti.Framework.Foundation.Key? key = null, Widget? child = null, global::System.Action<global::Doroti.Framework.Gestures.TapDownDetails>? onTapDown = null, global::System.Action<global::Doroti.Framework.Gestures.TapUpDetails>? onTapUp = null, global::System.Action? onTap = null, global::System.Action<global::Doroti.Framework.Gestures.TapMoveDetails>? onTapMove = null, global::System.Action? onTapCancel = null, global::System.Action? onSecondaryTap = null, global::System.Action<global::Doroti.Framework.Gestures.TapDownDetails>? onSecondaryTapDown = null, global::System.Action<global::Doroti.Framework.Gestures.TapUpDetails>? onSecondaryTapUp = null, global::System.Action? onSecondaryTapCancel = null, global::System.Action<global::Doroti.Framework.Gestures.TapDownDetails>? onTertiaryTapDown = null, global::System.Action<global::Doroti.Framework.Gestures.TapUpDetails>? onTertiaryTapUp = null, global::System.Action? onTertiaryTapCancel = null, global::System.Action<global::Doroti.Framework.Gestures.TapDownDetails>? onDoubleTapDown = null, global::System.Action? onDoubleTap = null, global::System.Action? onDoubleTapCancel = null, global::System.Action<global::Doroti.Framework.Gestures.LongPressDownDetails>? onLongPressDown = null, global::System.Action? onLongPressCancel = null, global::System.Action? onLongPress = null, global::System.Action<global::Doroti.Framework.Gestures.LongPressStartDetails>? onLongPressStart = null, global::System.Action<global::Doroti.Framework.Gestures.LongPressMoveUpdateDetails>? onLongPressMoveUpdate = null, global::System.Action? onLongPressUp = null, global::System.Action<global::Doroti.Framework.Gestures.LongPressEndDetails>? onLongPressEnd = null, global::System.Action<global::Doroti.Framework.Gestures.LongPressDownDetails>? onSecondaryLongPressDown = null, global::System.Action? onSecondaryLongPressCancel = null, global::System.Action? onSecondaryLongPress = null, global::System.Action<global::Doroti.Framework.Gestures.LongPressStartDetails>? onSecondaryLongPressStart = null, global::System.Action<global::Doroti.Framework.Gestures.LongPressMoveUpdateDetails>? onSecondaryLongPressMoveUpdate = null, global::System.Action? onSecondaryLongPressUp = null, global::System.Action<global::Doroti.Framework.Gestures.LongPressEndDetails>? onSecondaryLongPressEnd = null, global::System.Action<global::Doroti.Framework.Gestures.LongPressDownDetails>? onTertiaryLongPressDown = null, global::System.Action? onTertiaryLongPressCancel = null, global::System.Action? onTertiaryLongPress = null, global::System.Action<global::Doroti.Framework.Gestures.LongPressStartDetails>? onTertiaryLongPressStart = null, global::System.Action<global::Doroti.Framework.Gestures.LongPressMoveUpdateDetails>? onTertiaryLongPressMoveUpdate = null, global::System.Action? onTertiaryLongPressUp = null, global::System.Action<global::Doroti.Framework.Gestures.LongPressEndDetails>? onTertiaryLongPressEnd = null, global::System.Action<global::Doroti.Framework.Gestures.DragDownDetails>? onVerticalDragDown = null, global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>? onVerticalDragStart = null, global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>? onVerticalDragUpdate = null, global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>? onVerticalDragEnd = null, global::System.Action? onVerticalDragCancel = null, global::System.Action<global::Doroti.Framework.Gestures.DragDownDetails>? onHorizontalDragDown = null, global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>? onHorizontalDragStart = null, global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>? onHorizontalDragUpdate = null, global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>? onHorizontalDragEnd = null, global::System.Action? onHorizontalDragCancel = null, global::System.Action<global::Doroti.Framework.Gestures.ForcePressDetails>? onForcePressStart = null, global::System.Action<global::Doroti.Framework.Gestures.ForcePressDetails>? onForcePressPeak = null, global::System.Action<global::Doroti.Framework.Gestures.ForcePressDetails>? onForcePressUpdate = null, global::System.Action<global::Doroti.Framework.Gestures.ForcePressDetails>? onForcePressEnd = null, global::System.Action<global::Doroti.Framework.Gestures.DragDownDetails>? onPanDown = null, global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>? onPanStart = null, global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>? onPanUpdate = null, global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>? onPanEnd = null, global::System.Action? onPanCancel = null, global::System.Action<global::Doroti.Framework.Gestures.ScaleStartDetails>? onScaleStart = null, global::System.Action<global::Doroti.Framework.Gestures.ScaleUpdateDetails>? onScaleUpdate = null, global::System.Action<global::Doroti.Framework.Gestures.ScaleEndDetails>? onScaleEnd = null, global::Doroti.Framework.Rendering.HitTestBehavior? behavior = null, bool excludeFromSemantics = false, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start, bool trackpadScrollCausesScale = false, Offset? trackpadScrollToScaleFactor = null, HashSet<PointerDeviceKind>? supportedDevices = null) : base(key: key)
    {
        Offset __trackpadScrollToScaleFactor = trackpadScrollToScaleFactor ?? ScaleLibrary.kDefaultTrackpadScrollToScaleFactor;
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
        System.Diagnostics.Debug.Assert(((global::System.Func<bool>)(() =>
        {
            bool haveVerticalDrag = (((onVerticalDragStart is not null) || (onVerticalDragUpdate is not null)) || (onVerticalDragEnd is not null));
            bool haveHorizontalDrag = (((onHorizontalDragStart is not null) || (onHorizontalDragUpdate is not null)) || (onHorizontalDragEnd is not null));
            bool havePan = (((onPanStart is not null) || (onPanUpdate is not null)) || (onPanEnd is not null));
            bool haveScale = (((onScaleStart is not null) || (onScaleUpdate is not null)) || (onScaleEnd is not null));
            if ((havePan || haveScale))
            {
                if ((havePan && haveScale))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Incorrect GestureDetector arguments."), new global::Doroti.Framework.Foundation.ErrorDescription("Having both a pan gesture recognizer and a scale gesture recognizer is redundant; scale is a superset of pan."), new global::Doroti.Framework.Foundation.ErrorHint("Just use the scale gesture recognizer.") }));
                }
                var recognizer = (havePan ? "pan" : "scale");
                if ((haveVerticalDrag && haveHorizontalDrag))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("Incorrect GestureDetector arguments.\n" + $"Simultaneously having a vertical drag gesture recognizer, a horizontal drag gesture recognizer, and a {recognizer} gesture recognizer " + $"will result in the {recognizer} gesture recognizer being ignored, since the other two will catch all drags."));
                }
            }
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))());
    }

    public override Widget build(BuildContext context)
    {
        var gesturesLocal = new DartMap<Type, dynamic>();
        global::Doroti.Framework.Gestures.DeviceGestureSettings? gestureSettingsLocal = ((global::Doroti.Framework.Gestures.DeviceGestureSettings?)(object?)MediaQuery.maybeGestureSettingsOf(context));
        ScrollBehavior configuration = ((ScrollBehavior)(object?)ScrollConfiguration.of(context));
        if ((((((((((((this.onTapDown is not null) || (this.onTapUp is not null)) || (this.onTap is not null)) || (this.onTapCancel is not null)) || (this.onSecondaryTap is not null)) || (this.onSecondaryTapDown is not null)) || (this.onSecondaryTapUp is not null)) || (this.onSecondaryTapCancel is not null)) || (this.onTertiaryTapDown is not null)) || (this.onTertiaryTapUp is not null)) || (this.onTertiaryTapCancel is not null)))
        {
            gesturesLocal[typeof(global::Doroti.Framework.Gestures.TapGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.TapGestureRecognizer>(((global::System.Func<global::Doroti.Framework.Gestures.TapGestureRecognizer>)(() => new global::Doroti.Framework.Gestures.TapGestureRecognizer(debugOwner: this, supportedDevices: this.supportedDevices))), ((global::System.Action<global::Doroti.Framework.Gestures.TapGestureRecognizer>)((instance) =>
            {
                DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.TapGestureRecognizer>)(() =>
                {
                    var __cascade = instance;
                    __cascade.onTapDown = this.onTapDown;
                    __cascade.onTapUp = this.onTapUp;
                    __cascade.onTap = this.onTap;
                    __cascade.onTapCancel = this.onTapCancel;
                    __cascade.onSecondaryTap = this.onSecondaryTap;
                    __cascade.onSecondaryTapDown = this.onSecondaryTapDown;
                    __cascade.onSecondaryTapUp = this.onSecondaryTapUp;
                    __cascade.onSecondaryTapCancel = this.onSecondaryTapCancel;
                    __cascade.onTertiaryTapDown = this.onTertiaryTapDown;
                    __cascade.onTertiaryTapUp = this.onTertiaryTapUp;
                    __cascade.onTertiaryTapCancel = this.onTertiaryTapCancel;
                    __cascade.gestureSettings = gestureSettingsLocal;
                    __cascade.supportedDevices = this.supportedDevices;
                    return __cascade;
                }))());
            })));
        }
        if ((((this.onDoubleTap is not null) || (this.onDoubleTapDown is not null)) || (this.onDoubleTapCancel is not null)))
        {
            gesturesLocal[typeof(global::Doroti.Framework.Gestures.DoubleTapGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.DoubleTapGestureRecognizer>(((global::System.Func<global::Doroti.Framework.Gestures.DoubleTapGestureRecognizer>)(() => new global::Doroti.Framework.Gestures.DoubleTapGestureRecognizer(debugOwner: this, supportedDevices: this.supportedDevices))), ((global::System.Action<global::Doroti.Framework.Gestures.DoubleTapGestureRecognizer>)((instance) =>
            {
                DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.DoubleTapGestureRecognizer>)(() =>
                {
                    var __cascade = instance;
                    __cascade.onDoubleTapDown = this.onDoubleTapDown;
                    __cascade.onDoubleTap = this.onDoubleTap;
                    __cascade.onDoubleTapCancel = this.onDoubleTapCancel;
                    __cascade.gestureSettings = gestureSettingsLocal;
                    __cascade.supportedDevices = this.supportedDevices;
                    return __cascade;
                }))());
            })));
        }
        if ((((((((((((((((((((((this.onLongPressDown is not null) || (this.onLongPressCancel is not null)) || (this.onLongPress is not null)) || (this.onLongPressStart is not null)) || (this.onLongPressMoveUpdate is not null)) || (this.onLongPressUp is not null)) || (this.onLongPressEnd is not null)) || (this.onSecondaryLongPressDown is not null)) || (this.onSecondaryLongPressCancel is not null)) || (this.onSecondaryLongPress is not null)) || (this.onSecondaryLongPressStart is not null)) || (this.onSecondaryLongPressMoveUpdate is not null)) || (this.onSecondaryLongPressUp is not null)) || (this.onSecondaryLongPressEnd is not null)) || (this.onTertiaryLongPressDown is not null)) || (this.onTertiaryLongPressCancel is not null)) || (this.onTertiaryLongPress is not null)) || (this.onTertiaryLongPressStart is not null)) || (this.onTertiaryLongPressMoveUpdate is not null)) || (this.onTertiaryLongPressUp is not null)) || (this.onTertiaryLongPressEnd is not null)))
        {
            gesturesLocal[typeof(global::Doroti.Framework.Gestures.LongPressGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.LongPressGestureRecognizer>(((global::System.Func<global::Doroti.Framework.Gestures.LongPressGestureRecognizer>)(() => new global::Doroti.Framework.Gestures.LongPressGestureRecognizer(debugOwner: this, supportedDevices: this.supportedDevices))), ((global::System.Action<global::Doroti.Framework.Gestures.LongPressGestureRecognizer>)((instance) =>
            {
                DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.LongPressGestureRecognizer>)(() =>
                {
                    var __cascade = instance;
                    __cascade.onLongPressDown = this.onLongPressDown;
                    __cascade.onLongPressCancel = this.onLongPressCancel;
                    __cascade.onLongPress = this.onLongPress;
                    __cascade.onLongPressStart = this.onLongPressStart;
                    __cascade.onLongPressMoveUpdate = this.onLongPressMoveUpdate;
                    __cascade.onLongPressUp = this.onLongPressUp;
                    __cascade.onLongPressEnd = this.onLongPressEnd;
                    __cascade.onSecondaryLongPressDown = this.onSecondaryLongPressDown;
                    __cascade.onSecondaryLongPressCancel = this.onSecondaryLongPressCancel;
                    __cascade.onSecondaryLongPress = this.onSecondaryLongPress;
                    __cascade.onSecondaryLongPressStart = this.onSecondaryLongPressStart;
                    __cascade.onSecondaryLongPressMoveUpdate = this.onSecondaryLongPressMoveUpdate;
                    __cascade.onSecondaryLongPressUp = this.onSecondaryLongPressUp;
                    __cascade.onSecondaryLongPressEnd = this.onSecondaryLongPressEnd;
                    __cascade.onTertiaryLongPressDown = this.onTertiaryLongPressDown;
                    __cascade.onTertiaryLongPressCancel = this.onTertiaryLongPressCancel;
                    __cascade.onTertiaryLongPress = this.onTertiaryLongPress;
                    __cascade.onTertiaryLongPressStart = this.onTertiaryLongPressStart;
                    __cascade.onTertiaryLongPressMoveUpdate = this.onTertiaryLongPressMoveUpdate;
                    __cascade.onTertiaryLongPressUp = this.onTertiaryLongPressUp;
                    __cascade.onTertiaryLongPressEnd = this.onTertiaryLongPressEnd;
                    __cascade.gestureSettings = gestureSettingsLocal;
                    __cascade.supportedDevices = this.supportedDevices;
                    return __cascade;
                }))());
            })));
        }
        if ((((((this.onVerticalDragDown is not null) || (this.onVerticalDragStart is not null)) || (this.onVerticalDragUpdate is not null)) || (this.onVerticalDragEnd is not null)) || (this.onVerticalDragCancel is not null)))
        {
            gesturesLocal[typeof(global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer>(((global::System.Func<global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer>)(() => new global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer(debugOwner: this, supportedDevices: this.supportedDevices))), ((global::System.Action<global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer>)((instance) =>
            {
                DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer>)(() =>
                {
                    var __cascade = instance;
                    __cascade.onDown = this.onVerticalDragDown;
                    __cascade.onStart = this.onVerticalDragStart;
                    __cascade.onUpdate = this.onVerticalDragUpdate;
                    __cascade.onEnd = this.onVerticalDragEnd;
                    __cascade.onCancel = this.onVerticalDragCancel;
                    __cascade.dragStartBehavior = this.dragStartBehavior;
                    __cascade.multitouchDragStrategy = configuration.getMultitouchDragStrategy(context);
                    __cascade.gestureSettings = gestureSettingsLocal;
                    __cascade.supportedDevices = this.supportedDevices;
                    return __cascade;
                }))());
            })));
        }
        if ((((((this.onHorizontalDragDown is not null) || (this.onHorizontalDragStart is not null)) || (this.onHorizontalDragUpdate is not null)) || (this.onHorizontalDragEnd is not null)) || (this.onHorizontalDragCancel is not null)))
        {
            gesturesLocal[typeof(global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer>(((global::System.Func<global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer>)(() => new global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer(debugOwner: this, supportedDevices: this.supportedDevices))), ((global::System.Action<global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer>)((instance) =>
            {
                DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer>)(() =>
                {
                    var __cascade = instance;
                    __cascade.onDown = this.onHorizontalDragDown;
                    __cascade.onStart = this.onHorizontalDragStart;
                    __cascade.onUpdate = this.onHorizontalDragUpdate;
                    __cascade.onEnd = this.onHorizontalDragEnd;
                    __cascade.onCancel = this.onHorizontalDragCancel;
                    __cascade.dragStartBehavior = this.dragStartBehavior;
                    __cascade.multitouchDragStrategy = configuration.getMultitouchDragStrategy(context);
                    __cascade.gestureSettings = gestureSettingsLocal;
                    __cascade.supportedDevices = this.supportedDevices;
                    return __cascade;
                }))());
            })));
        }
        if ((((((this.onPanDown is not null) || (this.onPanStart is not null)) || (this.onPanUpdate is not null)) || (this.onPanEnd is not null)) || (this.onPanCancel is not null)))
        {
            gesturesLocal[typeof(global::Doroti.Framework.Gestures.PanGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.PanGestureRecognizer>(((global::System.Func<global::Doroti.Framework.Gestures.PanGestureRecognizer>)(() => new global::Doroti.Framework.Gestures.PanGestureRecognizer(debugOwner: this, supportedDevices: this.supportedDevices))), ((global::System.Action<global::Doroti.Framework.Gestures.PanGestureRecognizer>)((instance) =>
            {
                DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.PanGestureRecognizer>)(() =>
                {
                    var __cascade = instance;
                    __cascade.onDown = this.onPanDown;
                    __cascade.onStart = this.onPanStart;
                    __cascade.onUpdate = this.onPanUpdate;
                    __cascade.onEnd = this.onPanEnd;
                    __cascade.onCancel = this.onPanCancel;
                    __cascade.dragStartBehavior = this.dragStartBehavior;
                    __cascade.multitouchDragStrategy = configuration.getMultitouchDragStrategy(context);
                    __cascade.gestureSettings = gestureSettingsLocal;
                    __cascade.supportedDevices = this.supportedDevices;
                    return __cascade;
                }))());
            })));
        }
        if ((((this.onScaleStart is not null) || (this.onScaleUpdate is not null)) || (this.onScaleEnd is not null)))
        {
            gesturesLocal[typeof(global::Doroti.Framework.Gestures.ScaleGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.ScaleGestureRecognizer>(((global::System.Func<global::Doroti.Framework.Gestures.ScaleGestureRecognizer>)(() => new global::Doroti.Framework.Gestures.ScaleGestureRecognizer(debugOwner: this, supportedDevices: this.supportedDevices))), ((global::System.Action<global::Doroti.Framework.Gestures.ScaleGestureRecognizer>)((instance) =>
            {
                DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.ScaleGestureRecognizer>)(() =>
                {
                    var __cascade = instance;
                    __cascade.onStart = this.onScaleStart;
                    __cascade.onUpdate = this.onScaleUpdate;
                    __cascade.onEnd = this.onScaleEnd;
                    __cascade.dragStartBehavior = this.dragStartBehavior;
                    __cascade.gestureSettings = gestureSettingsLocal;
                    __cascade.trackpadScrollCausesScale = this.trackpadScrollCausesScale;
                    __cascade.trackpadScrollToScaleFactor = this.trackpadScrollToScaleFactor;
                    __cascade.supportedDevices = this.supportedDevices;
                    return __cascade;
                }))());
            })));
        }
        if (((((this.onForcePressStart is not null) || (this.onForcePressPeak is not null)) || (this.onForcePressUpdate is not null)) || (this.onForcePressEnd is not null)))
        {
            gesturesLocal[typeof(global::Doroti.Framework.Gestures.ForcePressGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.ForcePressGestureRecognizer>(((global::System.Func<global::Doroti.Framework.Gestures.ForcePressGestureRecognizer>)(() => new global::Doroti.Framework.Gestures.ForcePressGestureRecognizer(debugOwner: this, supportedDevices: this.supportedDevices))), ((global::System.Action<global::Doroti.Framework.Gestures.ForcePressGestureRecognizer>)((instance) =>
            {
                DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.ForcePressGestureRecognizer>)(() =>
                {
                    var __cascade = instance;
                    __cascade.onStart = this.onForcePressStart;
                    __cascade.onPeak = this.onForcePressPeak;
                    __cascade.onUpdate = this.onForcePressUpdate;
                    __cascade.onEnd = this.onForcePressEnd;
                    __cascade.gestureSettings = gestureSettingsLocal;
                    __cascade.supportedDevices = this.supportedDevices;
                    return __cascade;
                }))());
            })));
        }
        return ((Widget)(object?)new RawGestureDetector(gestures: gesturesLocal, behavior: this.behavior, excludeFromSemantics: this.excludeFromSemantics, child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Gestures.DragStartBehavior>("startBehavior", this.dragStartBehavior));
    }

}

public class RawGestureDetector : StatefulWidget
{
    public virtual Widget? child { get; private set; }
    public virtual DartMap<Type, dynamic> gestures { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.HitTestBehavior? behavior { get; private set; }
    public virtual bool excludeFromSemantics { get; private set; } = default!;
    public virtual SemanticsGestureDelegate? semantics { get; private set; }

    public RawGestureDetector(global::Doroti.Framework.Foundation.Key? key = null, Widget? child = null, DartMap<Type, dynamic> gestures = default!, global::Doroti.Framework.Rendering.HitTestBehavior? behavior = null, bool excludeFromSemantics = false, SemanticsGestureDelegate? semantics = null) : base(key: key)
    {
        DartMap<Type, dynamic> __gestures = gestures ?? new DartMap<Type, GestureRecognizerFactory<global::Doroti.Framework.Gestures.GestureRecognizer>>().cast<Type, dynamic>();
        this.child = child;
        this.gestures = __gestures;
        this.behavior = behavior;
        this.excludeFromSemantics = excludeFromSemantics;
        this.semantics = semantics;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new RawGestureDetectorState());
}

public class RawGestureDetectorState : State<RawGestureDetector>
{
    internal virtual DartMap<Type, global::Doroti.Framework.Gestures.GestureRecognizer>? _recognizers { get; set; } = new DartMap<Type, global::Doroti.Framework.Gestures.GestureRecognizer>();
    internal virtual SemanticsGestureDelegate? _semantics { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _semantics = (((RawGestureDetector)(object)this.widget).semantics ?? new _DefaultSemanticsGestureDelegate__gesture_detector(this));
        _syncAll(((RawGestureDetector)(object)this.widget).gestures);
    }

    public override void didUpdateWidget(RawGestureDetector oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!(((((RawGestureDetector)oldWidget).semantics is null) && (((RawGestureDetector)(object)this.widget).semantics is null))))
        {
            _semantics = (((RawGestureDetector)(object)this.widget).semantics ?? new _DefaultSemanticsGestureDelegate__gesture_detector(this));
        }
        _syncAll(((RawGestureDetector)(object)this.widget).gestures);
    }

    public virtual void replaceGestureRecognizers(DartMap<Type, dynamic> gestures)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (!((global::Doroti.Framework.Rendering.PipelineOwner?)((dynamic)this.context.findRenderObject()!).owner)!.debugDoingLayout)
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Unexpected call to replaceGestureRecognizers() method of RawGestureDetectorState."), new global::Doroti.Framework.Foundation.ErrorDescription("The replaceGestureRecognizers() method can only be called during the layout phase."), new global::Doroti.Framework.Foundation.ErrorHint("To set the gesture recognizers at other times, trigger a new build using setState() " + "and provide the new gesture recognizers as constructor arguments to the corresponding " + "RawGestureDetector or GestureDetector object.") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _syncAll(gestures);
        if (!((RawGestureDetector)(object)this.widget).excludeFromSemantics)
        {
            var semanticsGestureHandler = ((global::Doroti.Framework.Rendering.RenderSemanticsGestureHandler?)(object?)this.context.findRenderObject()!)!;
            _updateSemanticsForRenderObject(semanticsGestureHandler);
        }
    }

    public virtual void replaceSemanticsActions(HashSet<SemanticsAction> actions)
    {
        if (((RawGestureDetector)(object)this.widget).excludeFromSemantics)
        {
            return;
        }
        var semanticsGestureHandler = ((global::Doroti.Framework.Rendering.RenderSemanticsGestureHandler?)(object?)this.context.findRenderObject())!;
        DartRuntimePrimitives.Assert(() =>
            {
                if ((semanticsGestureHandler is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("Unexpected call to replaceSemanticsActions() method of RawGestureDetectorState.\n" + "The replaceSemanticsActions() method can only be called after the RenderSemanticsGestureHandler has been created."));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        semanticsGestureHandler!.validActions = actions;
    }

    public override void dispose()
    {
        foreach (global::Doroti.Framework.Gestures.GestureRecognizer recognizer in this._recognizers!.Values)
        {
            recognizer.dispose();
        }
        _recognizers = null;
        base.dispose();
    }

    internal virtual void _syncAll(DartMap<Type, dynamic> gestures)
    {
        DartRuntimePrimitives.Assert(() => (this._recognizers is not null));
        DartMap<Type, global::Doroti.Framework.Gestures.GestureRecognizer> oldRecognizers = this._recognizers!;
        _recognizers = new DartMap<Type, global::Doroti.Framework.Gestures.GestureRecognizer>().cast<Type, global::Doroti.Framework.Gestures.GestureRecognizer>();
        foreach (Type @type in gestures.Keys)
        {
            DartRuntimePrimitives.Assert(() => (gestures.ContainsKey(@type)));
            DartRuntimePrimitives.Assert(() => ((bool)((dynamic)gestures.GetValueOrDefault(@type)!)._debugAssertTypeMatches(@type)));
            DartRuntimePrimitives.Assert(() => !this._recognizers!.ContainsKey(@type));
            var factory = (GestureRecognizerFactoryBase)gestures.GetValueOrDefault(@type)!;
            this._recognizers![@type] = ((oldRecognizers.GetValueOrDefault(@type) ?? factory.createRecognizer()));
            DartRuntimePrimitives.Assert(() => (object.Equals(DartRuntimePrimitives.RuntimeType(this._recognizers!.GetValueOrDefault(@type)), @type)), () => (object?)$"GestureRecognizerFactory of type {@type} created a GestureRecognizer of type {DartRuntimePrimitives.RuntimeType(this._recognizers!.GetValueOrDefault(@type))}. The GestureRecognizerFactory must be specialized with the type of the class that it returns from its constructor method.");
            factory.initializeRecognizer(this._recognizers!.GetValueOrDefault(@type)!);
        }
        foreach (Type typeLocal in oldRecognizers.Keys)
        {
            if (!this._recognizers!.ContainsKey(typeLocal))
            {
                oldRecognizers.GetValueOrDefault(typeLocal)!.dispose();
            }
        }
    }

    internal virtual void _handlePointerDown(global::Doroti.Framework.Gestures.PointerDownEvent @event)
    {
        DartRuntimePrimitives.Assert(() => (this._recognizers is not null));
        foreach (global::Doroti.Framework.Gestures.GestureRecognizer recognizer in this._recognizers!.Values)
        {
            recognizer.addPointer((global::Doroti.Framework.Gestures.PointerDownEvent)(object)@event);
        }
    }

    internal virtual void _handlePointerPanZoomStart(global::Doroti.Framework.Gestures.PointerPanZoomStartEvent @event)
    {
        DartRuntimePrimitives.Assert(() => (this._recognizers is not null));
        foreach (global::Doroti.Framework.Gestures.GestureRecognizer recognizer in this._recognizers!.Values)
        {
            recognizer.addPointerPanZoom(@event);
        }
    }

    internal virtual global::Doroti.Framework.Rendering.HitTestBehavior _defaultBehavior
    {
        get
        {
            return ((((RawGestureDetector)(object)this.widget).child is null) ? global::Doroti.Framework.Rendering.HitTestBehavior.translucent : global::Doroti.Framework.Rendering.HitTestBehavior.deferToChild);
            return default!;
        }
    }
    internal virtual void _updateSemanticsForRenderObject(global::Doroti.Framework.Rendering.RenderSemanticsGestureHandler renderObject)
    {
        DartRuntimePrimitives.Assert(() => !((RawGestureDetector)(object)this.widget).excludeFromSemantics);
        DartRuntimePrimitives.Assert(() => (this._semantics is not null));
        this._semantics!.assignSemantics(renderObject);
    }

    public override Widget build(BuildContext context)
    {
        Widget result = ((Widget)(object?)new Listener(onPointerDown: (global::System.Action<global::Doroti.Framework.Gestures.PointerDownEvent>)this._handlePointerDown, onPointerPanZoomStart: (global::System.Action<global::Doroti.Framework.Gestures.PointerPanZoomStartEvent>)this._handlePointerPanZoomStart, behavior: ((((RawGestureDetector)(object)this.widget).behavior ?? (global::Doroti.Framework.Rendering.HitTestBehavior)this._defaultBehavior)), child: ((RawGestureDetector)(object)this.widget).child));
        if (!((RawGestureDetector)(object)this.widget).excludeFromSemantics)
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(new _GestureSemantics__gesture_detector(behavior: ((((RawGestureDetector)(object)this.widget).behavior ?? (global::Doroti.Framework.Rendering.HitTestBehavior)this._defaultBehavior)), assignSemantics: (global::System.Action<global::Doroti.Framework.Rendering.RenderSemanticsGestureHandler>)this._updateSemanticsForRenderObject, child: result));
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        if ((this._recognizers is null))
        {
            properties.add(global::Doroti.Framework.Foundation.DiagnosticsNode.CreateMessage("DISPOSED"));
        }
        else
        {
            List<string> gestures = this._recognizers!.Values.map<global::Doroti.Framework.Gestures.GestureRecognizer, string>(((recognizer) => ((global::Doroti.Framework.Gestures.GestureRecognizer)recognizer).debugDescription)).ToList().ToList();
            properties.add(new global::Doroti.Framework.Foundation.IterableProperty<string>("gestures", gestures.Cast<string>(), ifEmpty: "<none>"));
            properties.add(new global::Doroti.Framework.Foundation.IterableProperty<global::Doroti.Framework.Gestures.GestureRecognizer>("recognizers", this._recognizers!.Values.Cast<global::Doroti.Framework.Gestures.GestureRecognizer>(), level: global::Doroti.Framework.Foundation.DiagnosticLevel.fine));
            properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("excludeFromSemantics", ((RawGestureDetector)(object)this.widget).excludeFromSemantics, defaultValue: false));
            if (!((RawGestureDetector)(object)this.widget).excludeFromSemantics)
            {
                properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SemanticsGestureDelegate>("semantics", ((RawGestureDetector)(object)this.widget).semantics, defaultValue: null));
            }
        }
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Rendering.HitTestBehavior>("behavior", ((RawGestureDetector)(object)this.widget).behavior, defaultValue: null));
    }

}

internal delegate void _AssignSemantics__gesture_detector(global::Doroti.Framework.Rendering.RenderSemanticsGestureHandler __unused0);

internal class _GestureSemantics__gesture_detector : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Framework.Rendering.HitTestBehavior behavior { get; private set; } = default!;
    public virtual global::System.Action<global::Doroti.Framework.Rendering.RenderSemanticsGestureHandler> assignSemantics { get; private set; } = default!;

    internal _GestureSemantics__gesture_detector(Widget? child = null, global::Doroti.Framework.Rendering.HitTestBehavior behavior = default!, global::System.Action<global::Doroti.Framework.Rendering.RenderSemanticsGestureHandler> assignSemantics = default!) : base(child: child)
    {
        this.behavior = behavior;
        this.assignSemantics = assignSemantics;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        var renderObject = ((Func<global::Doroti.Framework.Rendering.RenderSemanticsGestureHandler>)(() =>
{
    var __cascade = new global::Doroti.Framework.Rendering.RenderSemanticsGestureHandler();
    __cascade.behavior = this.behavior;
    return __cascade;
}))();
        this.assignSemantics(renderObject);
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)renderObject);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderSemanticsGestureHandler)(object)renderObject;
        __renderObject.behavior = this.behavior;
        this.assignSemantics(__renderObject);
    }

}

public abstract class SemanticsGestureDelegate
{
    protected SemanticsGestureDelegate()
    {
    }

    public abstract void assignSemantics(global::Doroti.Framework.Rendering.RenderSemanticsGestureHandler renderObject);
    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "SemanticsGestureDelegate"))}()";
}

internal class _DefaultSemanticsGestureDelegate__gesture_detector : SemanticsGestureDelegate
{
    public virtual RawGestureDetectorState detectorState { get; private set; } = default!;

    internal _DefaultSemanticsGestureDelegate__gesture_detector(RawGestureDetectorState detectorState)
    {
        this.detectorState = detectorState;
    }

    internal static global::Doroti.Ui.Rect _getLocalRectFromRenderObject(global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        if ((renderObject is not global::Doroti.Framework.Rendering.RenderBox))
        {
            return Rect.zero;
        }
        global::Doroti.Ui.Size sizeLocal = ((global::Doroti.Ui.Size)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Size>(((global::Doroti.Framework.Rendering.RenderBox)((global::Doroti.Framework.Rendering.RenderBox)renderObject)).size));
        return global::Doroti.Ui.Rect.fromLTWH(0, 0, sizeLocal.width, sizeLocal.height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Ui.Offset _transformOffsetToGlobal(global::Doroti.Framework.Rendering.RenderObject @object, Offset local)
    {
        Matrix4 transform = ((Matrix4)(object?)((Matrix4)((dynamic)@object).getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null))));
        return ((global::Doroti.Ui.Offset)(object?)MatrixUtils.transformPoint(transform, local));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void assignSemantics(global::Doroti.Framework.Rendering.RenderSemanticsGestureHandler renderObject)
    {
        DartRuntimePrimitives.Assert(() => !this.detectorState.widget.excludeFromSemantics);
        DartMap<Type, global::Doroti.Framework.Gestures.GestureRecognizer> recognizers = ((RawGestureDetectorState)this.detectorState)._recognizers!;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Rendering.RenderSemanticsGestureHandler>)(() =>
{
    var __cascade = renderObject;
    __cascade.onTap = _getTapHandler(renderObject, recognizers);
    __cascade.onLongPress = _getLongPressHandler(renderObject, recognizers);
    __cascade.onHorizontalDragUpdate = _getHorizontalDragUpdateHandler(renderObject, recognizers);
    __cascade.onVerticalDragUpdate = _getVerticalDragUpdateHandler(renderObject, recognizers);
    return __cascade;
}))());
    }

    internal virtual global::System.Action? _getTapHandler(global::Doroti.Framework.Rendering.RenderObject renderObject, DartMap<Type, global::Doroti.Framework.Gestures.GestureRecognizer> recognizers)
    {
        var tap = ((global::Doroti.Framework.Gestures.TapGestureRecognizer?)(object?)recognizers.GetValueOrDefault(typeof(global::Doroti.Framework.Gestures.TapGestureRecognizer)))!;
        if ((tap is null))
        {
            return ((global::System.Action)(object)null);
        }
        return ((global::System.Action)(() =>
        {
            global::Doroti.Ui.Offset localCenter = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(((Offset)((dynamic)_DefaultSemanticsGestureDelegate__gesture_detector._getLocalRectFromRenderObject(renderObject)).center)));
            global::Doroti.Ui.Offset globalCenter = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(_DefaultSemanticsGestureDelegate__gesture_detector._transformOffsetToGlobal(renderObject, localCenter)));
            ((global::Doroti.Framework.Gestures.TapGestureRecognizer)tap).onTapDown?.Invoke(new global::Doroti.Framework.Gestures.TapDownDetails(globalPosition: globalCenter, localPosition: localCenter, kind: PointerDeviceKind.unknown));
            ((global::Doroti.Framework.Gestures.TapGestureRecognizer)tap).onTapUp?.Invoke(new global::Doroti.Framework.Gestures.TapUpDetails(globalPosition: globalCenter, localPosition: localCenter, kind: PointerDeviceKind.unknown));
            ((global::Doroti.Framework.Gestures.TapGestureRecognizer)tap).onTap?.Invoke();
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::System.Action? _getLongPressHandler(global::Doroti.Framework.Rendering.RenderObject renderObject, DartMap<Type, global::Doroti.Framework.Gestures.GestureRecognizer> recognizers)
    {
        var longPress = ((global::Doroti.Framework.Gestures.LongPressGestureRecognizer?)(object?)recognizers.GetValueOrDefault(typeof(global::Doroti.Framework.Gestures.LongPressGestureRecognizer)))!;
        if ((longPress is null))
        {
            return ((global::System.Action)(object)null);
        }
        return ((global::System.Action)(() =>
        {
            global::Doroti.Ui.Offset localCenter = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(((Offset)((dynamic)_DefaultSemanticsGestureDelegate__gesture_detector._getLocalRectFromRenderObject(renderObject)).center)));
            global::Doroti.Ui.Offset globalCenter = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(_DefaultSemanticsGestureDelegate__gesture_detector._transformOffsetToGlobal(renderObject, localCenter)));
            ((global::Doroti.Framework.Gestures.LongPressGestureRecognizer)longPress).onLongPressDown?.Invoke(new global::Doroti.Framework.Gestures.LongPressDownDetails(localPosition: localCenter, globalPosition: globalCenter));
            ((global::Doroti.Framework.Gestures.LongPressGestureRecognizer)longPress).onLongPressStart?.Invoke(new global::Doroti.Framework.Gestures.LongPressStartDetails(localPosition: localCenter, globalPosition: globalCenter));
            ((global::Doroti.Framework.Gestures.LongPressGestureRecognizer)longPress).onLongPress?.Invoke();
            ((global::Doroti.Framework.Gestures.LongPressGestureRecognizer)longPress).onLongPressEnd?.Invoke(new global::Doroti.Framework.Gestures.LongPressEndDetails(localPosition: localCenter, globalPosition: globalCenter));
            ((global::Doroti.Framework.Gestures.LongPressGestureRecognizer)longPress).onLongPressUp?.Invoke();
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>? _getHorizontalDragUpdateHandler(global::Doroti.Framework.Rendering.RenderObject renderObject, DartMap<Type, global::Doroti.Framework.Gestures.GestureRecognizer> recognizers)
    {
        var horizontal = ((global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer?)(object?)recognizers.GetValueOrDefault(typeof(global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer)))!;
        var pan = ((global::Doroti.Framework.Gestures.PanGestureRecognizer?)(object?)recognizers.GetValueOrDefault(typeof(global::Doroti.Framework.Gestures.PanGestureRecognizer)))!;
        global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>? horizontalHandler = DartRuntimePrimitives.ConvertValue<global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>>(((global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>)((horizontal is null) ? null : ((details) =>
        {
            global::Doroti.Ui.Offset localCenter = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(((Offset)((dynamic)_DefaultSemanticsGestureDelegate__gesture_detector._getLocalRectFromRenderObject(renderObject)).center)));
            global::Doroti.Ui.Offset globalCenter = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(_DefaultSemanticsGestureDelegate__gesture_detector._transformOffsetToGlobal(renderObject, localCenter)));
            global::Doroti.Ui.Offset newLocalOffset = ((global::Doroti.Ui.Offset)(object?)(localCenter + ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).delta));
            global::Doroti.Ui.Offset newGlobalOffset = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(_DefaultSemanticsGestureDelegate__gesture_detector._transformOffsetToGlobal(renderObject, newLocalOffset)));
            horizontal.onDown?.Invoke(new global::Doroti.Framework.Gestures.DragDownDetails(localPosition: localCenter, globalPosition: globalCenter));
            horizontal.onStart?.Invoke(new global::Doroti.Framework.Gestures.DragStartDetails(localPosition: localCenter, globalPosition: globalCenter));
            horizontal.onUpdate?.Invoke(details);
            horizontal.onEnd?.Invoke(new global::Doroti.Framework.Gestures.DragEndDetails(primaryVelocity: 0.0, localPosition: newLocalOffset, globalPosition: newGlobalOffset));
        }))));
        global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>? panHandler = DartRuntimePrimitives.ConvertValue<global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>>(((global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>)((pan is null) ? null : ((details) =>
        {
            global::Doroti.Ui.Offset localCenterLocal = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(((Offset)((dynamic)_DefaultSemanticsGestureDelegate__gesture_detector._getLocalRectFromRenderObject(renderObject)).center)));
            global::Doroti.Ui.Offset globalCenterLocal = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(_DefaultSemanticsGestureDelegate__gesture_detector._transformOffsetToGlobal(renderObject, localCenterLocal)));
            global::Doroti.Ui.Offset newLocalOffsetLocal = ((global::Doroti.Ui.Offset)(object?)(localCenterLocal + ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).delta));
            global::Doroti.Ui.Offset newGlobalOffsetLocal = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(_DefaultSemanticsGestureDelegate__gesture_detector._transformOffsetToGlobal(renderObject, newLocalOffsetLocal)));
            pan.onDown?.Invoke(new global::Doroti.Framework.Gestures.DragDownDetails(localPosition: localCenterLocal, globalPosition: globalCenterLocal));
            pan.onStart?.Invoke(new global::Doroti.Framework.Gestures.DragStartDetails(localPosition: localCenterLocal, globalPosition: globalCenterLocal));
            pan.onUpdate?.Invoke(details);
            pan.onEnd?.Invoke(new global::Doroti.Framework.Gestures.DragEndDetails(localPosition: newLocalOffsetLocal, globalPosition: newGlobalOffsetLocal));
        }))));
        if (((horizontalHandler is null) && (panHandler is null)))
        {
            return ((global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>)(object)null);
        }
        return ((global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>)((details) =>
        {
            horizontalHandler?.Invoke(details);
            panHandler?.Invoke(details);
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>? _getVerticalDragUpdateHandler(global::Doroti.Framework.Rendering.RenderObject renderObject, DartMap<Type, global::Doroti.Framework.Gestures.GestureRecognizer> recognizers)
    {
        var vertical = ((global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer?)(object?)recognizers.GetValueOrDefault(typeof(global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer)))!;
        var pan = ((global::Doroti.Framework.Gestures.PanGestureRecognizer?)(object?)recognizers.GetValueOrDefault(typeof(global::Doroti.Framework.Gestures.PanGestureRecognizer)))!;
        global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>? verticalHandler = DartRuntimePrimitives.ConvertValue<global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>>(((global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>)((vertical is null) ? null : ((details) =>
        {
            global::Doroti.Ui.Offset localCenter = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(((Offset)((dynamic)_DefaultSemanticsGestureDelegate__gesture_detector._getLocalRectFromRenderObject(renderObject)).center)));
            global::Doroti.Ui.Offset globalCenter = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(_DefaultSemanticsGestureDelegate__gesture_detector._transformOffsetToGlobal(renderObject, localCenter)));
            global::Doroti.Ui.Offset newLocalOffset = ((global::Doroti.Ui.Offset)(object?)(localCenter + ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).delta));
            global::Doroti.Ui.Offset newGlobalOffset = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(_DefaultSemanticsGestureDelegate__gesture_detector._transformOffsetToGlobal(renderObject, newLocalOffset)));
            vertical.onDown?.Invoke(new global::Doroti.Framework.Gestures.DragDownDetails(localPosition: localCenter, globalPosition: globalCenter));
            vertical.onStart?.Invoke(new global::Doroti.Framework.Gestures.DragStartDetails(localPosition: localCenter, globalPosition: globalCenter));
            vertical.onUpdate?.Invoke(details);
            vertical.onEnd?.Invoke(new global::Doroti.Framework.Gestures.DragEndDetails(primaryVelocity: 0.0, localPosition: newLocalOffset, globalPosition: newGlobalOffset));
        }))));
        global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>? panHandler = DartRuntimePrimitives.ConvertValue<global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>>(((global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>)((pan is null) ? null : ((details) =>
        {
            global::Doroti.Ui.Offset localCenterLocal = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(((Offset)((dynamic)_DefaultSemanticsGestureDelegate__gesture_detector._getLocalRectFromRenderObject(renderObject)).center)));
            global::Doroti.Ui.Offset globalCenterLocal = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(_DefaultSemanticsGestureDelegate__gesture_detector._transformOffsetToGlobal(renderObject, localCenterLocal)));
            global::Doroti.Ui.Offset newLocalOffsetLocal = ((global::Doroti.Ui.Offset)(object?)(localCenterLocal + ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).delta));
            global::Doroti.Ui.Offset newGlobalOffsetLocal = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(_DefaultSemanticsGestureDelegate__gesture_detector._transformOffsetToGlobal(renderObject, newLocalOffsetLocal)));
            pan.onDown?.Invoke(new global::Doroti.Framework.Gestures.DragDownDetails(localPosition: localCenterLocal, globalPosition: globalCenterLocal));
            pan.onStart?.Invoke(new global::Doroti.Framework.Gestures.DragStartDetails(localPosition: localCenterLocal, globalPosition: globalCenterLocal));
            pan.onUpdate?.Invoke(details);
            pan.onEnd?.Invoke(new global::Doroti.Framework.Gestures.DragEndDetails(localPosition: newLocalOffsetLocal, globalPosition: newGlobalOffsetLocal));
        }))));
        if (((verticalHandler is null) && (panHandler is null)))
        {
            return ((global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>)(object)null);
        }
        return ((global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>)((details) =>
        {
            verticalHandler?.Invoke(details);
            panHandler?.Invoke(details);
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
