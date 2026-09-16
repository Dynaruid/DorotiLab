// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/button.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class RawMaterialButton : StatefulWidget
{
    public virtual Action? onPressed { get; private set; }
    public virtual Action? onLongPress { get; private set; }
    public virtual System.Action<bool>? onHighlightChanged { get; private set; }
    public virtual MouseCursor? mouseCursor { get; private set; }
    public virtual TextStyle? textStyle { get; private set; }
    public virtual Color? fillColor { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual Color? highlightColor { get; private set; }
    public virtual Color? splashColor { get; private set; }
    public virtual double elevation { get; private set; } = default!;
    public virtual double hoverElevation { get; private set; } = default!;
    public virtual double focusElevation { get; private set; } = default!;
    public virtual double highlightElevation { get; private set; } = default!;
    public virtual double disabledElevation { get; private set; } = default!;
    public virtual EdgeInsetsGeometry padding { get; private set; } = default!;
    public virtual VisualDensity visualDensity { get; private set; } = default!;
    public virtual BoxConstraints constraints { get; private set; } = default!;
    public virtual ShapeBorder shape { get; private set; } = default!;
    public virtual Duration animationDuration { get; private set; } = default!;
    public virtual Widget? child { get; private set; }
    public virtual MaterialTapTargetSize materialTapTargetSize { get; private set; } = default!;
    public virtual FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual bool enableFeedback { get; private set; } = default!;

    public RawMaterialButton(Key? key = null, Action? onPressed = default!, Action? onLongPress = null, System.Action<bool>? onHighlightChanged = null, MouseCursor? mouseCursor = null, TextStyle? textStyle = null, Color? fillColor = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, Color? splashColor = null, double elevation = 2.0, double focusElevation = 4.0, double hoverElevation = 4.0, double highlightElevation = 8.0, double disabledElevation = 0.0, EdgeInsetsGeometry padding = default!, VisualDensity visualDensity = default!, BoxConstraints constraints = default!, ShapeBorder shape = default!, Duration? animationDuration = null, Clip clipBehavior = Clip.none, FocusNode? focusNode = null, bool autofocus = false, MaterialTapTargetSize? materialTapTargetSize = null, Widget? child = null, bool enableFeedback = true) : base(key: key)
    {
        EdgeInsetsGeometry __padding = padding ?? EdgeInsets.zero;
        VisualDensity __visualDensity = visualDensity ?? VisualDensity.standard;
        BoxConstraints __constraints = constraints ?? new BoxConstraints(minWidth: 88.0, minHeight: 36.0);
        ShapeBorder __shape = shape ?? new RoundedRectangleBorder();
        Duration __animationDuration = animationDuration ?? ConstantsLibrary.kThemeChangeDuration;
        this.onPressed = onPressed;
        this.onLongPress = onLongPress;
        this.onHighlightChanged = onHighlightChanged;
        this.mouseCursor = mouseCursor;
        this.textStyle = textStyle;
        this.fillColor = fillColor;
        this.focusColor = focusColor;
        this.hoverColor = hoverColor;
        this.highlightColor = highlightColor;
        this.splashColor = splashColor;
        this.elevation = elevation;
        this.focusElevation = focusElevation;
        this.hoverElevation = hoverElevation;
        this.highlightElevation = highlightElevation;
        this.disabledElevation = disabledElevation;
        this.padding = __padding;
        this.visualDensity = __visualDensity;
        this.constraints = __constraints;
        this.shape = __shape;
        this.animationDuration = __animationDuration;
        this.clipBehavior = clipBehavior;
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.child = child;
        this.enableFeedback = enableFeedback;
        this.materialTapTargetSize = materialTapTargetSize ?? MaterialTapTargetSize.padded;
        System.Diagnostics.Debug.Assert(elevation >= 0.0);
        System.Diagnostics.Debug.Assert(focusElevation >= 0.0);
        System.Diagnostics.Debug.Assert(hoverElevation >= 0.0);
        System.Diagnostics.Debug.Assert(highlightElevation >= 0.0);
        System.Diagnostics.Debug.Assert(disabledElevation >= 0.0);
    }

    public virtual bool enabled => DartRuntimePrimitives.ConvertValue<bool>((onPressed is not null) || (onLongPress is not null));
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _RawMaterialButtonState__button());
}

internal class _RawMaterialButtonState__button : State<RawMaterialButton>, MaterialStateMixin<RawMaterialButton>
{
    public virtual HashSet<WidgetState> materialStates { get; set; } = new HashSet<WidgetState>();

    public override void initState()
    {
        base.initState();
        setMaterialState(WidgetState.disabled, !widget.enabled);
    }

    public override void didUpdateWidget(RawMaterialButton oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        setMaterialState(WidgetState.disabled, !widget.enabled);
        if (isDisabled && isPressed)
        {
            removeMaterialState(WidgetState.pressed);
        }
    }

    internal virtual double _effectiveElevation
    {
        get
        {
            if (isDisabled)
            {
                return widget.disabledElevation;
            }
            if (isPressed)
            {
                return widget.highlightElevation;
            }
            if (isHovered)
            {
                return widget.hoverElevation;
            }
            if (isFocused)
            {
                return widget.focusElevation;
            }
            return widget.elevation;
        }
    }
    public override Widget build(BuildContext context)
    {
        Color? effectiveTextColor = WidgetStateProperty.resolveAs(widget.textStyle?.color, materialStates);
        ShapeBorder? effectiveShape = WidgetStateProperty.resolveAs<ShapeBorder?>(widget.shape, materialStates);
        Offset densityAdjustment = widget.visualDensity.baseSizeAdjustment;
        BoxConstraints effectiveConstraintsLocal = widget.visualDensity.effectiveConstraints(widget.constraints);
        MouseCursor? effectiveMouseCursor = WidgetStateProperty.resolveAs<MouseCursor?>(widget.mouseCursor ?? WidgetStateMouseCursor.adaptiveClickable, materialStates);
        EdgeInsetsGeometry paddingLocal = widget.padding.add(EdgeInsets.CreateOnly(left: densityAdjustment.dx, top: densityAdjustment.dy, right: densityAdjustment.dx, bottom: densityAdjustment.dy)).clamp(EdgeInsets.zero, EdgeInsetsGeometry.infinity);
        Widget result = new ConstrainedBox(constraints: effectiveConstraintsLocal, child: new Material(elevation: _effectiveElevation, textStyle: widget.textStyle?.copyWith(color: effectiveTextColor), shape: effectiveShape, color: widget.fillColor, shadowColor: Theme.of(context).shadowColor, type: (widget.fillColor is null) ? MaterialType.transparency : MaterialType.button, animationDuration: widget.animationDuration, clipBehavior: widget.clipBehavior, child: new InkWell(focusNode: widget.focusNode, canRequestFocus: widget.enabled, onFocusChange: updateMaterialState(WidgetState.focused), autofocus: widget.autofocus, onHighlightChanged: updateMaterialState(WidgetState.pressed, onChanged: widget.onHighlightChanged), splashColor: widget.splashColor, highlightColor: widget.highlightColor, focusColor: widget.focusColor, hoverColor: widget.hoverColor, onHover: updateMaterialState(WidgetState.hovered), onTap: widget.onPressed, onLongPress: widget.onLongPress, enableFeedback: widget.enableFeedback, customBorder: effectiveShape, mouseCursor: effectiveMouseCursor, child: IconTheme.merge(data: new IconThemeData(color: effectiveTextColor), child: new Padding(padding: paddingLocal, child: new Center(widthFactor: 1.0, heightFactor: 1.0, child: widget.child))))));
        Size minSizeLocal = default!;
        switch (widget.materialTapTargetSize)
        {
            case var __constant15001 when Equals(__constant15001, MaterialTapTargetSize.padded):
                {
                    minSizeLocal = new Size(ConstantsLibrary.kMinInteractiveDimension + densityAdjustment.dx, ConstantsLibrary.kMinInteractiveDimension + densityAdjustment.dy);
                    DartRuntimePrimitives.Assert(() => minSizeLocal.width >= 0.0);
                    DartRuntimePrimitives.Assert(() => minSizeLocal.height >= 0.0);
                    break;
                }
            case var __constant15272 when Equals(__constant15272, MaterialTapTargetSize.shrinkWrap):
                {
                    minSizeLocal = Size.zero;
                    break;
                }
        }
        return new Widgets.Semantics(container: true, button: true, enabled: widget.enabled, child: new _InputPadding__button(minSize: minSizeLocal, child: result));
    }

    public virtual System.Action<bool> updateMaterialState(WidgetState key, System.Action<bool>? onChanged = null)
    {
        return (value) =>
        {
            if (materialStates.Contains(key) == value)
            {
                return;
            }
            setMaterialState(key, value);
            onChanged?.Invoke(value);
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void setMaterialState(WidgetState state, bool isSet)
    {
        if (isSet) { addMaterialState(state); } else { removeMaterialState(state); }
        return;
    }

    public virtual void addMaterialState(WidgetState state)
    {
        if (materialStates.Add(state))
        {
            setState(() =>
            {
            });
        }
    }

    public virtual void removeMaterialState(WidgetState state)
    {
        if (materialStates.Remove(state))
        {
            setState(() =>
            {
            });
        }
    }

    public virtual bool isDisabled => materialStates.Contains(WidgetState.disabled);
    public virtual bool isDragged => materialStates.Contains(WidgetState.dragged);
    public virtual bool isErrored => materialStates.Contains(WidgetState.error);
    public virtual bool isFocused => materialStates.Contains(WidgetState.focused);
    public virtual bool isHovered => materialStates.Contains(WidgetState.hovered);
    public virtual bool isPressed => materialStates.Contains(WidgetState.pressed);
    public virtual bool isScrolledUnder => materialStates.Contains(WidgetState.scrolledUnder);
    public virtual bool isSelected => materialStates.Contains(WidgetState.selected);
    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<HashSet<WidgetState>>("materialStates", materialStates, defaultValue: new HashSet<WidgetState>()));
    }

}

internal class _InputPadding__button : SingleChildRenderObjectWidget
{
    public virtual Size minSize { get; private set; } = default!;

    internal _InputPadding__button(Widget? child = null, Size minSize = default!) : base(child: child)
    {
        this.minSize = minSize;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderInputPadding__button(minSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderInputPadding__button)renderObject;
        __renderObject.minSize = minSize;
    }

}

public class _RenderInputPadding__button : RenderShiftedBox
{
    internal virtual Size _minSize { get; set; } = default!;

    internal _RenderInputPadding__button(Size _minSize, RenderBox? child = null) : base(child)
    {
        this._minSize = _minSize;
    }

    public virtual Size minSize
    {
        get => _minSize;
        set
        {
            var __value = value;
            if (Equals(_minSize, __value))
            {
                return;
            }
            _minSize = __value;
            markNeedsLayout();
        }
    }
    public override double computeMinIntrinsicWidth(double height)
    {
        if (child is not null)
        {
            return Math.Max(child!.getMinIntrinsicWidth(height), minSize.width);
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        if (child is not null)
        {
            return Math.Max(child!.getMinIntrinsicHeight(width), minSize.height);
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        if (child is not null)
        {
            return Math.Max(child!.getMaxIntrinsicWidth(height), minSize.width);
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        if (child is not null)
        {
            return Math.Max(child!.getMaxIntrinsicHeight(width), minSize.height);
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Size _computeSize(BoxConstraints constraints, Func<RenderBox, BoxConstraints, Size> layoutChild)
    {
        if (child is not null)
        {
            Size childSize = layoutChild(child!, constraints);
            double widthLocal = Math.Max(childSize.width, minSize.width);
            double heightLocal = Math.Max(childSize.height, minSize.height);
            return constraints.constrain(new Size(widthLocal, heightLocal));
        }
        return Size.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _computeSize(constraints: constraints, layoutChild: ChildLayoutHelper.dryLayoutChild);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return null;
        }
        double? result = childLocal.getDryBaseline(constraints, baseline);
        if (result is null)
        {
            return null;
        }
        Size childSize = childLocal.getDryLayout(constraints);
        return DartRuntimePrimitives.RequireValue(result) + Alignment.center.alongOffset(getDryLayout(constraints) - childSize).dy;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        size = _computeSize(constraints: constraints, layoutChild: ChildLayoutHelper.layoutChild);
        if (child is not null)
        {
            var childParentData = ((BoxParentData?)child!.parentData!)!;
            childParentData.offset = Alignment.center.alongOffset(size - child!.size);
        }
    }

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        if (base.hitTest(result, position: position))
        {
            return true;
        }
        Offset centerLocal = child!.size.center(Offset.zero);
        return result.addWithRawTransform(transform: MatrixUtils.forceToPoint(centerLocal), position: centerLocal, hitTest: (result, position) =>
        {
            DartRuntimePrimitives.Assert(() => Equals(position, centerLocal));
            return child!.hitTest(result, position: centerLocal);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
