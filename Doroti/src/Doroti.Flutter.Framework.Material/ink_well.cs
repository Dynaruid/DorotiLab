// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/ink_well.dart
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public abstract class InteractiveInkFeature : InkFeature
{
    internal virtual Color _color { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? _customBorder { get; set; } = default;

    protected InteractiveInkFeature(MaterialInkController controller, global::Doroti.Generated.Framework.Rendering.RenderBox referenceBox, Color color, global::Doroti.Generated.Framework.Painting.ShapeBorder? customBorder = null, global::System.Action? onRemoved = null) : base(controller: controller, referenceBox: referenceBox, onRemoved: onRemoved)
    {
        this._color = color;
        this._customBorder = customBorder;
    }

    public virtual void confirm()
    {
    }

    public virtual void cancel()
    {
    }

    public virtual global::Doroti.Flutter.Ui.Color color
    {
        get => this._color;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(__value, this._color)))
            {
                return;
            }
            _color = __value;
            this.controller.markNeedsPaint();
        }
    }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? customBorder
    {
        get => this._customBorder;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._customBorder)))
            {
                return;
            }
            _customBorder = __value;
            this.controller.markNeedsPaint();
        }
    }
    public virtual void paintInkCircle(Canvas canvas, Matrix4 transform, Paint paint, Offset center, double radius, TextDirection? textDirection = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? customBorder = null, global::Doroti.Generated.Framework.Painting.BorderRadius borderRadius = default!, global::System.Func<Rect>? clipCallback = null)
    {
        global::Doroti.Flutter.Ui.Offset? originOffset__4629 = ((global::Doroti.Flutter.Ui.Offset?)(object?)MatrixUtils.getAsTranslation(transform));
        canvas.save();
        if ((originOffset__4629 is null))
        {
            canvas.transform(transform.storage);
        }
        else
        {
            canvas.translate(DartRuntimePrimitives.RequireValue(originOffset__4629).dx, DartRuntimePrimitives.RequireValue(originOffset__4629).dy);
        }
        if ((clipCallback is not null))
        {
            global::Doroti.Flutter.Ui.Rect rect__4905 = ((global::Doroti.Flutter.Ui.Rect)(object?)clipCallback());
            if ((customBorder is not null))
            {
                canvas.clipPath(customBorder.getOuterPath(rect__4905, textDirection: textDirection));
            }
            else
            {
                if ((!object.Equals(borderRadius, global::Doroti.Generated.Framework.Painting.BorderRadius.zero)))
                {
                    canvas.clipRRect(global::Doroti.Flutter.Ui.RRect.fromRectAndCorners(rect__4905, topLeft: ((global::Doroti.Generated.Framework.Painting.BorderRadius)borderRadius).topLeft, topRight: ((global::Doroti.Generated.Framework.Painting.BorderRadius)borderRadius).topRight, bottomLeft: ((global::Doroti.Generated.Framework.Painting.BorderRadius)borderRadius).bottomLeft, bottomRight: ((global::Doroti.Generated.Framework.Painting.BorderRadius)borderRadius).bottomRight));
                }
                else
                {
                    canvas.clipRect(rect__4905);
                }
            }
        }
        canvas.drawCircle(center, radius, paint);
        canvas.restore();
    }

}

public interface InteractiveInkFeatureFactory
{
    public InteractiveInkFeature create(MaterialInkController controller, global::Doroti.Generated.Framework.Rendering.RenderBox referenceBox, Offset position, Color color, TextDirection textDirection, bool containedInkWell = false, global::System.Func<Rect>? rectCallback = null, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? customBorder = null, double? radius = null, global::System.Action? onRemoved = null);
}

public interface _ParentInkResponseState__ink_well
{
    public void markChildInkResponsePressed(_ParentInkResponseState__ink_well childState, bool value);
}

internal class _ParentInkResponseProvider__ink_well : global::Doroti.Generated.Framework.Widgets.InheritedWidget
{
    public virtual _ParentInkResponseState__ink_well state { get; private set; } = default!;

    internal _ParentInkResponseProvider__ink_well(_ParentInkResponseState__ink_well state, global::Doroti.Generated.Framework.Widgets.Widget child) : base(child: child)
    {
        this.state = state;
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.state, ((_ParentInkResponseProvider__ink_well)oldWidget).state)));
    public static _ParentInkResponseState__ink_well? maybeOf(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<_ParentInkResponseProvider__ink_well>()?.state;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal delegate global::System.Func<Rect>? _GetRectCallback__ink_well(global::Doroti.Generated.Framework.Rendering.RenderBox referenceBox);

internal delegate bool _CheckContext__ink_well(global::Doroti.Generated.Framework.Widgets.BuildContext context);

public class InkResponse : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }
    public virtual global::System.Action? onTap { get; private set; }
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.TapDownDetails>? onTapDown { get; private set; }
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.TapUpDetails>? onTapUp { get; private set; }
    public virtual global::System.Action? onTapCancel { get; private set; }
    public virtual global::System.Action? onDoubleTap { get; private set; }
    public virtual global::System.Action? onLongPress { get; private set; }
    public virtual global::System.Action? onLongPressUp { get; private set; }
    public virtual global::System.Action? onSecondaryTap { get; private set; }
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.TapDownDetails>? onSecondaryTapDown { get; private set; }
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.TapUpDetails>? onSecondaryTapUp { get; private set; }
    public virtual global::System.Action? onSecondaryTapCancel { get; private set; }
    public virtual global::System.Action<bool>? onHighlightChanged { get; private set; }
    public virtual global::System.Action<bool>? onHover { get; private set; }
    public virtual global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual bool containedInkWell { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BoxShape highlightShape { get; private set; } = default!;
    public virtual double? radius { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? customBorder { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual Color? highlightColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual Color? splashColor { get; private set; }
    public virtual InteractiveInkFeatureFactory? splashFactory { get; private set; }
    public virtual bool enableFeedback { get; private set; } = default!;
    public virtual bool excludeFromSemantics { get; private set; } = default!;
    public virtual global::System.Action<bool>? onFocusChange { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool canRequestFocus { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController { get; private set; }
    public virtual Duration? hoverDuration { get; private set; }

    public InkResponse(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget? child = null, global::System.Action? onTap = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.TapDownDetails>? onTapDown = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.TapUpDetails>? onTapUp = null, global::System.Action? onTapCancel = null, global::System.Action? onDoubleTap = null, global::System.Action? onLongPress = null, global::System.Action? onLongPressUp = null, global::System.Action? onSecondaryTap = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.TapUpDetails>? onSecondaryTapUp = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.TapDownDetails>? onSecondaryTapDown = null, global::System.Action? onSecondaryTapCancel = null, global::System.Action<bool>? onHighlightChanged = null, global::System.Action<bool>? onHover = null, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null, bool containedInkWell = false, global::Doroti.Generated.Framework.Painting.BoxShape highlightShape = global::Doroti.Generated.Framework.Painting.BoxShape.circle, double? radius = null, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? customBorder = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, Color? splashColor = null, InteractiveInkFeatureFactory? splashFactory = null, bool enableFeedback = true, bool excludeFromSemantics = false, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool canRequestFocus = true, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController = null, Duration? hoverDuration = null) : base(key: key)
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

    public virtual global::System.Func<Rect>? getRectCallback(global::Doroti.Generated.Framework.Rendering.RenderBox referenceBox) => DartRuntimePrimitives.ConvertValue<global::System.Func<Rect>>(null);
    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        _ParentInkResponseState__ink_well? parentState__25830 = ((_ParentInkResponseState__ink_well?)(object?)_ParentInkResponseProvider__ink_well.maybeOf(context));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _InkResponseStateWidget__ink_well(onTap: this.onTap, onTapDown: (global::System.Action<global::Doroti.Generated.Framework.Gestures.TapDownDetails>?)this.onTapDown, onTapUp: (global::System.Action<global::Doroti.Generated.Framework.Gestures.TapUpDetails>?)this.onTapUp, onTapCancel: this.onTapCancel, onDoubleTap: this.onDoubleTap, onLongPress: this.onLongPress, onLongPressUp: this.onLongPressUp, onSecondaryTap: this.onSecondaryTap, onSecondaryTapUp: (global::System.Action<global::Doroti.Generated.Framework.Gestures.TapUpDetails>?)this.onSecondaryTapUp, onSecondaryTapDown: (global::System.Action<global::Doroti.Generated.Framework.Gestures.TapDownDetails>?)this.onSecondaryTapDown, onSecondaryTapCancel: this.onSecondaryTapCancel, onHighlightChanged: (global::System.Action<bool>?)this.onHighlightChanged, onHover: (global::System.Action<bool>?)this.onHover, mouseCursor: this.mouseCursor, containedInkWell: this.containedInkWell, highlightShape: this.highlightShape, radius: this.radius, borderRadius: this.borderRadius, customBorder: this.customBorder, focusColor: this.focusColor, hoverColor: this.hoverColor, highlightColor: this.highlightColor, overlayColor: this.overlayColor, splashColor: this.splashColor, splashFactory: this.splashFactory, enableFeedback: this.enableFeedback, excludeFromSemantics: this.excludeFromSemantics, focusNode: this.focusNode, canRequestFocus: this.canRequestFocus, onFocusChange: (global::System.Action<bool>?)this.onFocusChange, autofocus: this.autofocus, parentState: parentState__25830, getRectCallback: (global::System.Func<global::Doroti.Generated.Framework.Rendering.RenderBox, global::System.Func<Rect>?>)this.getRectCallback, debugCheckContext: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, bool>)this.debugCheckContext, statesController: this.statesController, hoverDuration: this.hoverDuration, child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool debugCheckContext(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _InkResponseStateWidget__ink_well : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? child { get; private set; }
    public virtual global::System.Action? onTap { get; private set; }
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.TapDownDetails>? onTapDown { get; private set; }
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.TapUpDetails>? onTapUp { get; private set; }
    public virtual global::System.Action? onTapCancel { get; private set; }
    public virtual global::System.Action? onDoubleTap { get; private set; }
    public virtual global::System.Action? onLongPress { get; private set; }
    public virtual global::System.Action? onLongPressUp { get; private set; }
    public virtual global::System.Action? onSecondaryTap { get; private set; }
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.TapUpDetails>? onSecondaryTapUp { get; private set; }
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.TapDownDetails>? onSecondaryTapDown { get; private set; }
    public virtual global::System.Action? onSecondaryTapCancel { get; private set; }
    public virtual global::System.Action<bool>? onHighlightChanged { get; private set; }
    public virtual global::System.Action<bool>? onHover { get; private set; }
    public virtual global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual bool containedInkWell { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BoxShape highlightShape { get; private set; } = default!;
    public virtual double? radius { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? customBorder { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual Color? highlightColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual Color? splashColor { get; private set; }
    public virtual InteractiveInkFeatureFactory? splashFactory { get; private set; }
    public virtual bool enableFeedback { get; private set; } = default!;
    public virtual bool excludeFromSemantics { get; private set; } = default!;
    public virtual global::System.Action<bool>? onFocusChange { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool canRequestFocus { get; private set; } = default!;
    public virtual _ParentInkResponseState__ink_well? parentState { get; private set; }
    public virtual global::System.Func<global::Doroti.Generated.Framework.Rendering.RenderBox, global::System.Func<Rect>?>? getRectCallback { get; private set; }
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, bool> debugCheckContext { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController { get; private set; }
    public virtual Duration? hoverDuration { get; private set; }

    internal _InkResponseStateWidget__ink_well(global::Doroti.Generated.Framework.Widgets.Widget? child = null, global::System.Action? onTap = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.TapDownDetails>? onTapDown = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.TapUpDetails>? onTapUp = null, global::System.Action? onTapCancel = null, global::System.Action? onDoubleTap = null, global::System.Action? onLongPress = null, global::System.Action? onLongPressUp = null, global::System.Action? onSecondaryTap = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.TapUpDetails>? onSecondaryTapUp = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.TapDownDetails>? onSecondaryTapDown = null, global::System.Action? onSecondaryTapCancel = null, global::System.Action<bool>? onHighlightChanged = null, global::System.Action<bool>? onHover = null, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null, bool containedInkWell = false, global::Doroti.Generated.Framework.Painting.BoxShape highlightShape = global::Doroti.Generated.Framework.Painting.BoxShape.circle, double? radius = null, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? customBorder = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, Color? splashColor = null, InteractiveInkFeatureFactory? splashFactory = null, bool enableFeedback = true, bool excludeFromSemantics = false, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool canRequestFocus = true, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, _ParentInkResponseState__ink_well? parentState = null, global::System.Func<global::Doroti.Generated.Framework.Rendering.RenderBox, global::System.Func<Rect>?>? getRectCallback = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, bool> debugCheckContext = default!, global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController = null, Duration? hoverDuration = null)
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

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _InkResponseState__ink_well());
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        var gestures__30371 = ((Func<List<string>>)(() => { var __collection30382 = new List<string>(); if ((this.onTap is not null)) { __collection30382.Add("tap"); } if ((this.onDoubleTap is not null)) { __collection30382.Add("double tap"); } if ((this.onLongPress is not null)) { __collection30382.Add("long press"); } if ((this.onLongPressUp is not null)) { __collection30382.Add("long press up"); } if ((this.onTapDown is not null)) { __collection30382.Add("tap down"); } if ((this.onTapUp is not null)) { __collection30382.Add("tap up"); } if ((this.onTapCancel is not null)) { __collection30382.Add("tap cancel"); } if ((this.onSecondaryTap is not null)) { __collection30382.Add("secondary tap"); } if ((this.onSecondaryTapUp is not null)) { __collection30382.Add("secondary tap up"); } if ((this.onSecondaryTapDown is not null)) { __collection30382.Add("secondary tap down"); } if ((this.onSecondaryTapCancel is not null)) { __collection30382.Add("secondary tap cancel"); } return __collection30382; }))();
        properties.add(new global::Doroti.Generated.Framework.Foundation.IterableProperty<string>("gestures", gestures__30371.Cast<string>(), ifEmpty: "<none>"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Services.MouseCursor>("mouseCursor", this.mouseCursor));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("containedInkWell", this.containedInkWell, level: global::Doroti.Generated.Framework.Foundation.DiagnosticLevel.fine));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.BoxShape>("highlightShape", this.highlightShape, description: $"{(this.containedInkWell ? "clipped to " : "")}{this.highlightShape}", showName: false));
    }

}

public enum _HighlightType__ink_well
{
    pressed,
    hover,
    focus
}

public class _InkResponseState__ink_well : global::Doroti.Generated.Framework.Widgets.State<_InkResponseStateWidget__ink_well>, global::Doroti.Generated.Framework.Widgets.AutomaticKeepAliveClientMixin<_InkResponseStateWidget__ink_well>, _ParentInkResponseState__ink_well
{
    internal virtual HashSet<InteractiveInkFeature>? _splashes { get; set; } = default;
    internal virtual InteractiveInkFeature? _currentSplash { get; set; } = default;
    internal virtual bool _hovering { get; set; } = false;
    internal virtual DartMap<_HighlightType__ink_well, InkHighlight?> _highlights { get; private set; } = new DartMap<_HighlightType__ink_well, InkHighlight?>();
    private bool __late__actionMap_initialized;
    private DartMap<Type, dynamic> __late__actionMap = default!;
    internal virtual DartMap<Type, dynamic> _actionMap
    {
        get
        {
            if (!__late__actionMap_initialized)
            {
                __late__actionMap = new DartMap<Type, dynamic> { [typeof(global::Doroti.Generated.Framework.Widgets.ActivateIntent)] = new global::Doroti.Generated.Framework.Widgets.CallbackAction<global::Doroti.Generated.Framework.Widgets.ActivateIntent>(onInvoke: (global::System.Action<global::Doroti.Generated.Framework.Widgets.Intent?>)this.activateOnIntent), [typeof(global::Doroti.Generated.Framework.Widgets.ButtonActivateIntent)] = new global::Doroti.Generated.Framework.Widgets.CallbackAction<global::Doroti.Generated.Framework.Widgets.ButtonActivateIntent>(onInvoke: (global::System.Action<global::Doroti.Generated.Framework.Widgets.Intent?>)this.activateOnIntent) };
                __late__actionMap_initialized = true;
            }
            return __late__actionMap;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStatesController? internalStatesController { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Foundation.ObserverList<_ParentInkResponseState__ink_well> _activeChildren { get; private set; } = new global::Doroti.Generated.Framework.Foundation.ObserverList<_ParentInkResponseState__ink_well>();
    internal static Duration _activationDuration = Duration.Create(milliseconds: 100L);
    internal virtual Timer? _activationTimer { get; set; } = default;
    internal virtual bool _hasFocus { get; set; } = false;
    public virtual KeepAliveHandle? _keepAliveHandle { get; set; } = default;

    public virtual bool highlightsExist => System.Linq.Enumerable.Any(this._highlights.Values.where(((highlight) => (highlight is not null))));
    public virtual void markChildInkResponsePressed(_ParentInkResponseState__ink_well childState, bool value)
    {
        bool lastAnyPressed__32737 = this._anyChildInkResponsePressed;
        if (value)
        {
            this._activeChildren.add(childState);
        }
        else
        {
            this._activeChildren.remove(childState);
        }
        bool nowAnyPressed__32915 = this._anyChildInkResponsePressed;
        if ((nowAnyPressed__32915 != lastAnyPressed__32737))
        {
            ((_InkResponseStateWidget__ink_well)this.widget).parentState?.markChildInkResponsePressed(this, nowAnyPressed__32915);
        }
    }

    internal virtual bool _anyChildInkResponsePressed => System.Linq.Enumerable.Any(this._activeChildren);
    public virtual void activateOnIntent(global::Doroti.Generated.Framework.Widgets.Intent? intent)
    {
        this._activationTimer?.cancel();
        _activationTimer = null;
        _startNewSplash(context: this.context);
        this._currentSplash?.confirm();
        _currentSplash = null;
        if ((((_InkResponseStateWidget__ink_well)this.widget).onTap is not null))
        {
            if (((_InkResponseStateWidget__ink_well)this.widget).enableFeedback)
            {
                DartRuntimePrimitives.Ignore(Feedback.forTap(this.context));
            }
            ((_InkResponseStateWidget__ink_well)this.widget).onTap?.Invoke();
        }
        _activationTimer = new Timer(_activationDuration, (() => {
updateHighlight(_HighlightType__ink_well.pressed, value: false);
}));
    }

    public virtual void simulateTap(global::Doroti.Generated.Framework.Widgets.Intent? intent = null)
    {
        _startNewSplash(context: this.context);
        handleTap();
    }

    public virtual void simulateLongPress()
    {
        _startNewSplash(context: this.context);
        handleLongPress();
    }

    public virtual void handleStatesControllerChange()
    {
        setState(((global::System.Action)(() => {
})));
    }

    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStatesController statesController => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStatesController>((((_InkResponseStateWidget__ink_well)this.widget).statesController ?? this.internalStatesController!));
    public virtual void initStatesController()
    {
        if ((((_InkResponseStateWidget__ink_well)this.widget).statesController is null))
        {
            internalStatesController = new global::Doroti.Generated.Framework.Widgets.WidgetStatesController();
        }
        this.statesController.update(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled, !this.enabled);
        this.statesController.addListener(() => this.handleStatesControllerChange());
    }

    public override void initState()
    {
        base.initState();
        if (this.wantKeepAlive)
        {
            _ensureKeepAlive();
        }
        initStatesController();
        global::Doroti.Generated.Framework.Widgets.FocusManager.instance.addHighlightModeListener((global::System.Action<global::Doroti.Generated.Framework.Widgets.FocusHighlightMode>)this.handleFocusHighlightModeChange);
    }

    public override void didUpdateWidget(_InkResponseStateWidget__ink_well oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((_InkResponseStateWidget__ink_well)this.widget).statesController, ((_InkResponseStateWidget__ink_well)oldWidget).statesController)))
        {
            ((_InkResponseStateWidget__ink_well)oldWidget).statesController?.removeListener(() => this.handleStatesControllerChange());
            if ((((_InkResponseStateWidget__ink_well)this.widget).statesController is not null))
            {
                this.internalStatesController?.dispose();
                internalStatesController = null;
            }
            initStatesController();
        }
        if ((((((_InkResponseStateWidget__ink_well)this.widget).radius != ((_InkResponseStateWidget__ink_well)oldWidget).radius) || (!object.Equals(((_InkResponseStateWidget__ink_well)this.widget).highlightShape, ((_InkResponseStateWidget__ink_well)oldWidget).highlightShape))) || (!object.Equals(((_InkResponseStateWidget__ink_well)this.widget).borderRadius, ((_InkResponseStateWidget__ink_well)oldWidget).borderRadius))))
        {
            InkHighlight? hoverHighlight__35286 = this._highlights.GetValueOrDefault(_HighlightType__ink_well.hover);
            if ((hoverHighlight__35286 is not null))
            {
                hoverHighlight__35286.dispose();
                updateHighlight(_HighlightType__ink_well.hover, value: this._hovering, callOnHover: false);
            }
            InkHighlight? focusHighlight__35527 = this._highlights.GetValueOrDefault(_HighlightType__ink_well.focus);
            if ((focusHighlight__35527 is not null))
            {
                focusHighlight__35527.dispose();
            }
        }
        if ((!object.Equals(((_InkResponseStateWidget__ink_well)this.widget).customBorder, ((_InkResponseStateWidget__ink_well)oldWidget).customBorder)))
        {
            _updateHighlightsAndSplashes();
        }
        if ((this.enabled != isWidgetEnabled(oldWidget)))
        {
            this.statesController.update(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled, !this.enabled);
            if (!this.enabled)
            {
                this.statesController.update(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed, false);
                InkHighlight? hoverHighlight__36473 = this._highlights.GetValueOrDefault(_HighlightType__ink_well.hover);
                hoverHighlight__36473?.dispose();
            }
            updateHighlight(_HighlightType__ink_well.hover, value: this._hovering, callOnHover: false);
        }
        updateFocusHighlights();
    }

    public override void dispose()
    {
        global::Doroti.Generated.Framework.Widgets.FocusManager.instance.removeHighlightModeListener((global::System.Action<global::Doroti.Generated.Framework.Widgets.FocusHighlightMode>)this.handleFocusHighlightModeChange);
        this.statesController.removeListener(() => this.handleStatesControllerChange());
        this.internalStatesController?.dispose();
        this._activationTimer?.cancel();
        _activationTimer = null;
        base.dispose();
    }

    public virtual bool wantKeepAlive => DartRuntimePrimitives.ConvertValue<bool>((this.highlightsExist || (((this._splashes is not null) && System.Linq.Enumerable.Any(this._splashes!)))));
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
                    return (((_InkResponseStateWidget__ink_well)this.widget).hoverDuration ?? Duration.Create(milliseconds: 50L));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void updateHighlight(_HighlightType__ink_well type, bool value, bool callOnHover = true)
    {
        InkHighlight? highlight__37688 = this._highlights.GetValueOrDefault(type);
        void handleInkRemoval()
        {
            DartRuntimePrimitives.Assert(() => (this._highlights.ContainsKey(type)));
            this._highlights[type] = null;
            updateKeepAlive();
        }
        switch (type)
        {
            case _HighlightType__ink_well.pressed:
                {
                    this.statesController.update(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed, value);
                    break;
                }
            case _HighlightType__ink_well.hover:
                {
                    if (callOnHover)
                    {
                        this.statesController.update(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered, value);
                    }
                    break;
                }
            case _HighlightType__ink_well.focus:
                {
                    break;
                }
        }
        if ((object.Equals(type, _HighlightType__ink_well.pressed)))
        {
            ((_InkResponseStateWidget__ink_well)this.widget).parentState?.markChildInkResponsePressed(this, value);
        }
        if ((value == (((highlight__37688 is not null) && ((InkHighlight)highlight__37688).active))))
        {
            return;
        }
        if (value)
        {
            if ((highlight__37688 is null))
            {
                global::Doroti.Flutter.Ui.Color resolvedOverlayColor__38458 = ((global::Doroti.Flutter.Ui.Color)(object?)(((_InkResponseStateWidget__ink_well)this.widget).overlayColor?.resolve(this.statesController.value) ?? (type switch { _HighlightType__ink_well.pressed => (((_InkResponseStateWidget__ink_well)this.widget).highlightColor ?? Theme.of(this.context).highlightColor), _HighlightType__ink_well.focus => (((_InkResponseStateWidget__ink_well)this.widget).focusColor ?? Theme.of(this.context).focusColor), _HighlightType__ink_well.hover => (((_InkResponseStateWidget__ink_well)this.widget).hoverColor ?? Theme.of(this.context).hoverColor), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })));
                var referenceBox__38938 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)this.context.findRenderObject()!)!;
                this._highlights[type] = new InkHighlight(controller: Material.of(this.context), referenceBox: referenceBox__38938, color: (this.enabled ? resolvedOverlayColor__38458 : resolvedOverlayColor__38458.withAlpha(0L)), shape: ((_InkResponseStateWidget__ink_well)this.widget).highlightShape, radius: ((_InkResponseStateWidget__ink_well)this.widget).radius, borderRadius: ((_InkResponseStateWidget__ink_well)this.widget).borderRadius, customBorder: ((_InkResponseStateWidget__ink_well)this.widget).customBorder, rectCallback: ((_InkResponseStateWidget__ink_well)this.widget).getRectCallback!(referenceBox__38938), onRemoved: () => handleInkRemoval(), textDirection: Directionality.of(this.context), fadeDuration: getFadeDurationForType(type));
                updateKeepAlive();
            }
            else
            {
                highlight__37688.activate();
            }
        }
        else
        {
            highlight__37688!.deactivate();
        }
        DartRuntimePrimitives.Assert(() => (value == ((this._highlights.GetValueOrDefault(type) is InkHighlight currentHighlight) && currentHighlight.active)));
        switch (type)
        {
            case _HighlightType__ink_well.pressed:
                {
                    ((_InkResponseStateWidget__ink_well)this.widget).onHighlightChanged?.Invoke(value);
                    break;
                }
            case _HighlightType__ink_well.hover:
                {
                    if (callOnHover)
                    {
                        ((_InkResponseStateWidget__ink_well)this.widget).onHover?.Invoke(value);
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
        foreach (InkHighlight? highlight__40137 in this._highlights.Values)
        {
            highlight__40137?.customBorder = ((_InkResponseStateWidget__ink_well)this.widget).customBorder;
        }
        this._currentSplash?.customBorder = ((_InkResponseStateWidget__ink_well)this.widget).customBorder;
        if (((this._splashes is not null) && System.Linq.Enumerable.Any(this._splashes!)))
        {
            foreach (InteractiveInkFeature inkFeature__40381 in this._splashes!)
            {
                inkFeature__40381.customBorder = ((_InkResponseStateWidget__ink_well)this.widget).customBorder;
            }
        }
    }

    internal virtual InteractiveInkFeature _createSplash(Offset globalPosition)
    {
        MaterialInkController inkController__40578 = ((MaterialInkController)(object?)Material.of(this.context));
        var referenceBox__40626 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)this.context.findRenderObject()!)!;
        global::Doroti.Flutter.Ui.Offset position__40700 = ((global::Doroti.Flutter.Ui.Offset)(object?)((Offset)((dynamic)referenceBox__40626).globalToLocal(globalPosition)));
        global::Doroti.Flutter.Ui.Color color__40771 = ((global::Doroti.Flutter.Ui.Color)(object?)((((_InkResponseStateWidget__ink_well)this.widget).overlayColor?.resolve(this.statesController.value) ?? ((_InkResponseStateWidget__ink_well)this.widget).splashColor) ?? Theme.of(this.context).splashColor));
        global::System.Func<Rect>? rectCallback__40936 = ((global::System.Func<Rect>)(((_InkResponseStateWidget__ink_well)this.widget).containedInkWell ? ((_InkResponseStateWidget__ink_well)this.widget).getRectCallback!(referenceBox__40626) : null));
        global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius__41063 = ((_InkResponseStateWidget__ink_well)this.widget).borderRadius;
        global::Doroti.Generated.Framework.Painting.ShapeBorder? customBorder__41122 = ((_InkResponseStateWidget__ink_well)this.widget).customBorder;
        InteractiveInkFeature? splash__41186 = default!;
        void onRemoved()
        {
            if ((this._splashes is not null))
            {
                DartRuntimePrimitives.Assert(() => this._splashes!.Contains(splash__41186));
                this._splashes!.Remove(splash__41186);
                if ((object.Equals(this._currentSplash, splash__41186)))
                {
                    _currentSplash = null;
                }
                updateKeepAlive();
            }
        }
        splash__41186 = ((((_InkResponseStateWidget__ink_well)this.widget).splashFactory ?? Theme.of(this.context).splashFactory)).create(controller: inkController__40578, referenceBox: referenceBox__40626, position: position__40700, color: color__40771, containedInkWell: ((_InkResponseStateWidget__ink_well)this.widget).containedInkWell, rectCallback: (global::System.Func<Rect>?)rectCallback__40936, radius: ((_InkResponseStateWidget__ink_well)this.widget).radius, borderRadius: borderRadius__41063, customBorder: customBorder__41122, onRemoved: () => onRemoved(), textDirection: Directionality.of(this.context));
        return splash__41186;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleFocusHighlightModeChange(global::Doroti.Generated.Framework.Widgets.FocusHighlightMode mode)
    {
        if (!this.mounted)
        {
            return;
        }
        setState(((global::System.Action)(() => {
updateFocusHighlights();
})));
    }

    internal virtual bool _shouldShowFocus => (MediaQuery.maybeNavigationModeOf(this.context) switch { global::Doroti.Generated.Framework.Widgets.NavigationMode.traditional => (this.enabled && this._hasFocus), null => (this.enabled && this._hasFocus), global::Doroti.Generated.Framework.Widgets.NavigationMode.directional => this._hasFocus, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    public virtual void updateFocusHighlights()
    {
        bool showFocus__42387 = (global::Doroti.Generated.Framework.Widgets.FocusManager.instance.highlightMode switch { global::Doroti.Generated.Framework.Widgets.FocusHighlightMode.touch => false, global::Doroti.Generated.Framework.Widgets.FocusHighlightMode.traditional => this._shouldShowFocus, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        updateHighlight(_HighlightType__ink_well.focus, value: showFocus__42387);
    }

    public virtual void handleFocusUpdate(bool hasFocus)
    {
        _hasFocus = hasFocus;
        this.statesController.update(global::Doroti.Generated.Framework.Widgets.WidgetState.focused, hasFocus);
        updateFocusHighlights();
        ((_InkResponseStateWidget__ink_well)this.widget).onFocusChange?.Invoke(hasFocus);
    }

    public virtual void handleAnyTapDown(global::Doroti.Generated.Framework.Gestures.TapDownDetails details)
    {
        if (this._anyChildInkResponsePressed)
        {
            return;
        }
        _startNewSplash(details: details);
    }

    public virtual void handleTapDown(global::Doroti.Generated.Framework.Gestures.TapDownDetails details)
    {
        handleAnyTapDown(details);
        ((_InkResponseStateWidget__ink_well)this.widget).onTapDown?.Invoke(details);
    }

    public virtual void handleTapUp(global::Doroti.Generated.Framework.Gestures.TapUpDetails details)
    {
        ((_InkResponseStateWidget__ink_well)this.widget).onTapUp?.Invoke(details);
    }

    public virtual void handleSecondaryTapDown(global::Doroti.Generated.Framework.Gestures.TapDownDetails details)
    {
        handleAnyTapDown(details);
        ((_InkResponseStateWidget__ink_well)this.widget).onSecondaryTapDown?.Invoke(details);
    }

    public virtual void handleSecondaryTapUp(global::Doroti.Generated.Framework.Gestures.TapUpDetails details)
    {
        ((_InkResponseStateWidget__ink_well)this.widget).onSecondaryTapUp?.Invoke(details);
    }

    internal virtual void _startNewSplash(global::Doroti.Generated.Framework.Gestures.TapDownDetails? details = null, global::Doroti.Generated.Framework.Widgets.BuildContext? context = null)
    {
        DartRuntimePrimitives.Assert(() => ((details is not null) || (context is not null)));
        global::Doroti.Flutter.Ui.Offset globalPosition__43831 = default!;
        if ((context is not null))
        {
            var referenceBox__43886 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)context.findRenderObject()!)!;
            DartRuntimePrimitives.Assert(() => ((global::Doroti.Generated.Framework.Rendering.RenderBox)referenceBox__43886).hasSize, () => (object?)"InkResponse must be done with layout before starting a splash.");
            globalPosition__43831 = ((Offset)((dynamic)referenceBox__43886).localToGlobal(((Offset)((dynamic)((global::Doroti.Generated.Framework.Rendering.RenderBox)referenceBox__43886).paintBounds).center)));
        }
        else
        {
            globalPosition__43831 = details!.globalPosition;
        }
        this.statesController.update(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed, true);
        InteractiveInkFeature splash__44343 = ((InteractiveInkFeature)(object?)_createSplash(globalPosition__43831));
        _splashes ??= new HashSet<InteractiveInkFeature>();
        this._splashes!.Add(splash__44343);
        this._currentSplash?.cancel();
        _currentSplash = splash__44343;
        updateKeepAlive();
        updateHighlight(_HighlightType__ink_well.pressed, value: true);
    }

    public virtual void handleTap()
    {
        this._currentSplash?.confirm();
        _currentSplash = null;
        updateHighlight(_HighlightType__ink_well.pressed, value: false);
        if ((((_InkResponseStateWidget__ink_well)this.widget).onTap is not null))
        {
            if (((_InkResponseStateWidget__ink_well)this.widget).enableFeedback)
            {
                DartRuntimePrimitives.Ignore(Feedback.forTap(this.context));
            }
            ((_InkResponseStateWidget__ink_well)this.widget).onTap?.Invoke();
        }
    }

    public virtual void handleTapCancel()
    {
        this._currentSplash?.cancel();
        _currentSplash = null;
        ((_InkResponseStateWidget__ink_well)this.widget).onTapCancel?.Invoke();
        updateHighlight(_HighlightType__ink_well.pressed, value: false);
    }

    public virtual void handleDoubleTap()
    {
        this._currentSplash?.confirm();
        _currentSplash = null;
        updateHighlight(_HighlightType__ink_well.pressed, value: false);
        ((_InkResponseStateWidget__ink_well)this.widget).onDoubleTap?.Invoke();
    }

    public virtual void handleLongPress()
    {
        this._currentSplash?.confirm();
        _currentSplash = null;
        if ((((_InkResponseStateWidget__ink_well)this.widget).onLongPress is not null))
        {
            if (((_InkResponseStateWidget__ink_well)this.widget).enableFeedback)
            {
                DartRuntimePrimitives.Ignore(Feedback.forLongPress(this.context));
            }
            ((_InkResponseStateWidget__ink_well)this.widget).onLongPress!();
        }
    }

    public virtual void handleLongPressUp()
    {
        this._currentSplash?.confirm();
        _currentSplash = null;
        ((_InkResponseStateWidget__ink_well)this.widget).onLongPressUp?.Invoke();
    }

    public virtual void handleSecondaryTap()
    {
        this._currentSplash?.confirm();
        _currentSplash = null;
        updateHighlight(_HighlightType__ink_well.pressed, value: false);
        ((_InkResponseStateWidget__ink_well)this.widget).onSecondaryTap?.Invoke();
    }

    public virtual void handleSecondaryTapCancel()
    {
        this._currentSplash?.cancel();
        _currentSplash = null;
        ((_InkResponseStateWidget__ink_well)this.widget).onSecondaryTapCancel?.Invoke();
        updateHighlight(_HighlightType__ink_well.pressed, value: false);
    }

    public override void deactivate()
    {
        if ((this._splashes is not null))
        {
            HashSet<InteractiveInkFeature> splashes__46114 = this._splashes!;
            _splashes = null;
            foreach (var splash__46178 in splashes__46114)
            {
                splash__46178.dispose();
            }
            _currentSplash = null;
        }
        DartRuntimePrimitives.Assert(() => (this._currentSplash is null));
        foreach (_HighlightType__ink_well highlight__46335 in this._highlights.Keys)
        {
            this._highlights.GetValueOrDefault(highlight__46335)?.dispose();
            this._highlights[DartRuntimePrimitives.RequireValue(highlight__46335)] = null;
        }
        ((_InkResponseStateWidget__ink_well)this.widget).parentState?.markChildInkResponsePressed(this, false);
        if ((this._keepAliveHandle is not null))
        {
            _releaseKeepAlive();
        }
        base.deactivate();
    }

    public virtual bool isWidgetEnabled(_InkResponseStateWidget__ink_well widget)
    {
        return (_primaryButtonEnabled(widget) || _secondaryButtonEnabled(widget));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _primaryButtonEnabled(_InkResponseStateWidget__ink_well widget)
    {
        return ((((((((_InkResponseStateWidget__ink_well)widget).onTap is not null) || (((_InkResponseStateWidget__ink_well)widget).onDoubleTap is not null)) || (((_InkResponseStateWidget__ink_well)widget).onLongPress is not null)) || (((_InkResponseStateWidget__ink_well)widget).onLongPressUp is not null)) || (((_InkResponseStateWidget__ink_well)widget).onTapUp is not null)) || (((_InkResponseStateWidget__ink_well)widget).onTapDown is not null));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _secondaryButtonEnabled(_InkResponseStateWidget__ink_well widget)
    {
        return (((((_InkResponseStateWidget__ink_well)widget).onSecondaryTap is not null) || (((_InkResponseStateWidget__ink_well)widget).onSecondaryTapUp is not null)) || (((_InkResponseStateWidget__ink_well)widget).onSecondaryTapDown is not null));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool enabled => isWidgetEnabled(this.widget);
    internal virtual bool _primaryEnabled => _primaryButtonEnabled(this.widget);
    internal virtual bool _secondaryEnabled => _secondaryButtonEnabled(this.widget);
    public virtual void handleMouseEnter(global::Doroti.Generated.Framework.Gestures.PointerEnterEvent @event)
    {
        _hovering = true;
        if (this.enabled)
        {
            handleHoverChange();
        }
    }

    public virtual void handleMouseExit(global::Doroti.Generated.Framework.Gestures.PointerExitEvent @event)
    {
        _hovering = false;
        handleHoverChange();
    }

    public virtual void handleHoverChange()
    {
        updateHighlight(_HighlightType__ink_well.hover, value: this._hovering);
    }

    internal virtual bool _canRequestFocus => (MediaQuery.maybeNavigationModeOf(this.context) switch { global::Doroti.Generated.Framework.Widgets.NavigationMode.traditional => (this.enabled && ((_InkResponseStateWidget__ink_well)this.widget).canRequestFocus), null => (this.enabled && ((_InkResponseStateWidget__ink_well)this.widget).canRequestFocus), global::Doroti.Generated.Framework.Widgets.NavigationMode.directional => true, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => this.widget.debugCheckContext(context));
        if ((this.wantKeepAlive && (this._keepAliveHandle is null)))
        {
            _ensureKeepAlive();
        }
        ThemeData theme__48190 = Theme.of(context);
        var highlightableStates__48227 = new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> { global::Doroti.Generated.Framework.Widgets.WidgetState.focused, global::Doroti.Generated.Framework.Widgets.WidgetState.hovered, global::Doroti.Generated.Framework.Widgets.WidgetState.pressed };
        HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> nonHighlightableStates__48379 = this.statesController.value.difference<global::Doroti.Generated.Framework.Widgets.WidgetState>(highlightableStates__48227);
        var pressed__48667 = ((Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>>)(() => { var __collection48677 = new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>(); __collection48677.UnionWith(nonHighlightableStates__48379); __collection48677.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed); return __collection48677; }))();
        var focused__48750 = ((Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>>)(() => { var __collection48760 = new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>(); __collection48760.UnionWith(nonHighlightableStates__48379); __collection48760.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.focused); return __collection48760; }))();
        var hovered__48833 = ((Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>>)(() => { var __collection48843 = new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>(); __collection48843.UnionWith(nonHighlightableStates__48379); __collection48843.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered); return __collection48843; }))();
        Color getHighlightColorForType(_HighlightType__ink_well type)
        {
            return (type switch { _HighlightType__ink_well.pressed => ((((_InkResponseStateWidget__ink_well)this.widget).overlayColor?.resolve(pressed__48667) ?? ((_InkResponseStateWidget__ink_well)this.widget).highlightColor) ?? theme__48190.highlightColor), _HighlightType__ink_well.focus => ((((_InkResponseStateWidget__ink_well)this.widget).overlayColor?.resolve(focused__48750) ?? ((_InkResponseStateWidget__ink_well)this.widget).focusColor) ?? theme__48190.focusColor), _HighlightType__ink_well.hover => ((((_InkResponseStateWidget__ink_well)this.widget).overlayColor?.resolve(hovered__48833) ?? ((_InkResponseStateWidget__ink_well)this.widget).hoverColor) ?? theme__48190.hoverColor), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        foreach (_HighlightType__ink_well type__49640 in this._highlights.Keys)
        {
            this._highlights[type__49640]?.color = getHighlightColorForType(type__49640);
        }
        this._currentSplash?.color = ((((_InkResponseStateWidget__ink_well)this.widget).overlayColor?.resolve(this.statesController.value) ?? ((_InkResponseStateWidget__ink_well)this.widget).splashColor) ?? Theme.of(context).splashColor);
        global::Doroti.Generated.Framework.Services.MouseCursor effectiveMouseCursor__49924 = ((global::Doroti.Generated.Framework.Services.MouseCursor)(object?)WidgetStateProperty.resolveAs<global::Doroti.Generated.Framework.Services.MouseCursor>((((_InkResponseStateWidget__ink_well)this.widget).mouseCursor ?? global::Doroti.Generated.Framework.Widgets.WidgetStateMouseCursor.adaptiveClickable), this.statesController.value));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _ParentInkResponseProvider__ink_well(state: this, child: new global::Doroti.Generated.Framework.Widgets.Actions(actions: this._actionMap, child: new global::Doroti.Generated.Framework.Widgets.Focus(focusNode: ((_InkResponseStateWidget__ink_well)this.widget).focusNode, canRequestFocus: this._canRequestFocus, onFocusChange: (global::System.Action<bool>)this.handleFocusUpdate, autofocus: ((_InkResponseStateWidget__ink_well)this.widget).autofocus, child: new global::Doroti.Generated.Framework.Widgets.MouseRegion(cursor: effectiveMouseCursor__49924, onEnter: (global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerEnterEvent>)this.handleMouseEnter, onExit: (global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerExitEvent>)this.handleMouseExit, child: DefaultSelectionStyle.merge(mouseCursor: effectiveMouseCursor__49924, child: new global::Doroti.Generated.Framework.Widgets.Semantics(onTap: () => ((global::System.Action<global::Doroti.Generated.Framework.Widgets.Intent?>)((((_InkResponseStateWidget__ink_well)this.widget).excludeFromSemantics || (((_InkResponseStateWidget__ink_well)this.widget).onTap is null)) ? null : this.simulateTap))(default), onLongPress: ((global::System.Action)((((_InkResponseStateWidget__ink_well)this.widget).excludeFromSemantics || (((_InkResponseStateWidget__ink_well)this.widget).onLongPress is null)) ? null : this.simulateLongPress)), child: new global::Doroti.Generated.Framework.Widgets.GestureDetector(onTapDown: ((global::System.Action<global::Doroti.Generated.Framework.Gestures.TapDownDetails>)(this._primaryEnabled ? this.handleTapDown : null)), onTapUp: ((global::System.Action<global::Doroti.Generated.Framework.Gestures.TapUpDetails>)(this._primaryEnabled ? this.handleTapUp : null)), onTap: ((global::System.Action)(this._primaryEnabled ? this.handleTap : null)), onTapCancel: ((global::System.Action)(this._primaryEnabled ? this.handleTapCancel : null)), onDoubleTap: ((global::System.Action)((((_InkResponseStateWidget__ink_well)this.widget).onDoubleTap is not null) ? this.handleDoubleTap : null)), onLongPress: ((global::System.Action)((((_InkResponseStateWidget__ink_well)this.widget).onLongPress is not null) ? this.handleLongPress : null)), onLongPressUp: ((global::System.Action)((((_InkResponseStateWidget__ink_well)this.widget).onLongPressUp is not null) ? this.handleLongPressUp : null)), onSecondaryTapDown: ((global::System.Action<global::Doroti.Generated.Framework.Gestures.TapDownDetails>)(this._secondaryEnabled ? this.handleSecondaryTapDown : null)), onSecondaryTapUp: ((global::System.Action<global::Doroti.Generated.Framework.Gestures.TapUpDetails>)(this._secondaryEnabled ? this.handleSecondaryTapUp : null)), onSecondaryTap: ((global::System.Action)(this._secondaryEnabled ? this.handleSecondaryTap : null)), onSecondaryTapCancel: ((global::System.Action)(this._secondaryEnabled ? this.handleSecondaryTapCancel : null)), behavior: global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque, excludeFromSemantics: true, child: ((_InkResponseStateWidget__ink_well)this.widget).child))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _ensureKeepAlive()
    {
        DartRuntimePrimitives.Assert(() => (this._keepAliveHandle is null));
        this._keepAliveHandle = new KeepAliveHandle();
        new KeepAliveNotification(this._keepAliveHandle!).dispatch(this.context);
    }

    public virtual void _releaseKeepAlive()
    {
        this._keepAliveHandle!.dispose();
        this._keepAliveHandle = null;
    }

    public virtual void updateKeepAlive()
    {
        if (this.wantKeepAlive)
        {
            if ((this._keepAliveHandle is null))
            {
                _ensureKeepAlive();
            }
        }
        else
        {
            if ((this._keepAliveHandle is not null))
            {
                _releaseKeepAlive();
            }
        }
    }

}

public class InkWell : InkResponse
{
    public InkWell(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget? child = null, global::System.Action? onTap = null, global::System.Action? onDoubleTap = null, global::System.Action? onLongPress = null, global::System.Action? onLongPressUp = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.TapDownDetails>? onTapDown = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.TapUpDetails>? onTapUp = null, global::System.Action? onTapCancel = null, global::System.Action? onSecondaryTap = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.TapUpDetails>? onSecondaryTapUp = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.TapDownDetails>? onSecondaryTapDown = null, global::System.Action? onSecondaryTapCancel = null, global::System.Action<bool>? onHighlightChanged = null, global::System.Action<bool>? onHover = null, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, Color? splashColor = null, InteractiveInkFeatureFactory? splashFactory = null, double? radius = null, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? customBorder = null, bool enableFeedback = true, bool excludeFromSemantics = false, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool canRequestFocus = true, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController = null, Duration? hoverDuration = null) : base(key: key, child: child, onTap: onTap, onDoubleTap: onDoubleTap, onLongPress: onLongPress, onLongPressUp: onLongPressUp, onTapDown: onTapDown, onTapUp: onTapUp, onTapCancel: onTapCancel, onSecondaryTap: onSecondaryTap, onSecondaryTapUp: onSecondaryTapUp, onSecondaryTapDown: onSecondaryTapDown, onSecondaryTapCancel: onSecondaryTapCancel, onHighlightChanged: onHighlightChanged, onHover: onHover, mouseCursor: mouseCursor, focusColor: focusColor, hoverColor: hoverColor, highlightColor: highlightColor, overlayColor: overlayColor, splashColor: splashColor, splashFactory: splashFactory, radius: radius, borderRadius: borderRadius, customBorder: customBorder, enableFeedback: enableFeedback, excludeFromSemantics: excludeFromSemantics, focusNode: focusNode, canRequestFocus: canRequestFocus, onFocusChange: onFocusChange, autofocus: autofocus, statesController: statesController, hoverDuration: hoverDuration, containedInkWell: true, highlightShape: global::Doroti.Generated.Framework.Painting.BoxShape.rectangle)
    {
    }

}
