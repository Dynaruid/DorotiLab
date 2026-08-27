// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/button.dart
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

public class RawMaterialButton : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual global::System.Action? onLongPress { get; private set; }
    public virtual global::System.Action<bool>? onHighlightChanged { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? textStyle { get; private set; }
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
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry padding { get; private set; } = default!;
    public virtual VisualDensity visualDensity { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.BoxConstraints constraints { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.ShapeBorder shape { get; private set; } = default!;
    public virtual Duration animationDuration { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? child { get; private set; }
    public virtual MaterialTapTargetSize materialTapTargetSize { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual bool enableFeedback { get; private set; } = default!;

    public RawMaterialButton(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action? onPressed = default!, global::System.Action? onLongPress = null, global::System.Action<bool>? onHighlightChanged = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Framework.Painting.TextStyle? textStyle = null, Color? fillColor = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, Color? splashColor = null, double elevation = 2.0, double focusElevation = 4.0, double hoverElevation = 4.0, double highlightElevation = 8.0, double disabledElevation = 0.0, global::Doroti.Framework.Painting.EdgeInsetsGeometry padding = default!, VisualDensity visualDensity = default!, global::Doroti.Framework.Rendering.BoxConstraints constraints = default!, global::Doroti.Framework.Painting.ShapeBorder shape = default!, Duration? animationDuration = null, Clip clipBehavior = Clip.none, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, MaterialTapTargetSize? materialTapTargetSize = null, global::Doroti.Framework.Widgets.Widget? child = null, bool enableFeedback = true) : base(key: key)
    {
        global::Doroti.Framework.Painting.EdgeInsetsGeometry __padding = padding ?? global::Doroti.Framework.Painting.EdgeInsets.zero;
        VisualDensity __visualDensity = visualDensity ?? VisualDensity.standard;
        global::Doroti.Framework.Rendering.BoxConstraints __constraints = constraints ?? new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: 88.0, minHeight: 36.0);
        global::Doroti.Framework.Painting.ShapeBorder __shape = shape ?? new global::Doroti.Framework.Painting.RoundedRectangleBorder();
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
        this.materialTapTargetSize = (materialTapTargetSize ?? MaterialTapTargetSize.padded);
        System.Diagnostics.Debug.Assert((elevation >= 0.0));
        System.Diagnostics.Debug.Assert((focusElevation >= 0.0));
        System.Diagnostics.Debug.Assert((hoverElevation >= 0.0));
        System.Diagnostics.Debug.Assert((highlightElevation >= 0.0));
        System.Diagnostics.Debug.Assert((disabledElevation >= 0.0));
    }

    public virtual bool enabled => DartRuntimePrimitives.ConvertValue<bool>(((this.onPressed is not null) || (this.onLongPress is not null)));
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _RawMaterialButtonState__button());
}

internal class _RawMaterialButtonState__button : global::Doroti.Framework.Widgets.State<RawMaterialButton>, MaterialStateMixin<RawMaterialButton>
{
    public virtual HashSet<global::Doroti.Framework.Widgets.WidgetState> materialStates { get; set; } = new HashSet<global::Doroti.Framework.Widgets.WidgetState>();

    public override void initState()
    {
        base.initState();
        setMaterialState(global::Doroti.Framework.Widgets.WidgetState.disabled, !((RawMaterialButton)this.widget).enabled);
    }

    public override void didUpdateWidget(RawMaterialButton oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        setMaterialState(global::Doroti.Framework.Widgets.WidgetState.disabled, !((RawMaterialButton)this.widget).enabled);
        if ((this.isDisabled && this.isPressed))
        {
            removeMaterialState(global::Doroti.Framework.Widgets.WidgetState.pressed);
        }
    }

    internal virtual double _effectiveElevation
    {
        get
        {
            if (this.isDisabled)
            {
                return ((RawMaterialButton)this.widget).disabledElevation;
            }
            if (this.isPressed)
            {
                return ((RawMaterialButton)this.widget).highlightElevation;
            }
            if (this.isHovered)
            {
                return ((RawMaterialButton)this.widget).hoverElevation;
            }
            if (this.isFocused)
            {
                return ((RawMaterialButton)this.widget).focusElevation;
            }
            return ((RawMaterialButton)this.widget).elevation;
            return default!;
        }
    }
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color? effectiveTextColor = ((global::Doroti.Ui.Color?)(object?)WidgetStateProperty.resolveAs<global::Doroti.Ui.Color?>(((RawMaterialButton)this.widget).textStyle?.color, this.materialStates));
        global::Doroti.Framework.Painting.ShapeBorder? effectiveShape = ((global::Doroti.Framework.Painting.ShapeBorder?)(object?)WidgetStateProperty.resolveAs<global::Doroti.Framework.Painting.ShapeBorder?>(((RawMaterialButton)this.widget).shape, this.materialStates));
        global::Doroti.Ui.Offset densityAdjustment = ((global::Doroti.Ui.Offset)(object?)((RawMaterialButton)this.widget).visualDensity.baseSizeAdjustment);
        global::Doroti.Framework.Rendering.BoxConstraints effectiveConstraintsLocal = ((RawMaterialButton)this.widget).visualDensity.effectiveConstraints(((RawMaterialButton)this.widget).constraints);
        global::Doroti.Framework.Services.MouseCursor? effectiveMouseCursor = ((global::Doroti.Framework.Services.MouseCursor?)(object?)WidgetStateProperty.resolveAs<global::Doroti.Framework.Services.MouseCursor?>((((RawMaterialButton)this.widget).mouseCursor ?? global::Doroti.Framework.Widgets.WidgetStateMouseCursor.adaptiveClickable), this.materialStates));
        global::Doroti.Framework.Painting.EdgeInsetsGeometry paddingLocal = ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)(object?)((RawMaterialButton)this.widget).padding.add(global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(left: densityAdjustment.dx, top: densityAdjustment.dy, right: densityAdjustment.dx, bottom: densityAdjustment.dy)).clamp(global::Doroti.Framework.Painting.EdgeInsets.zero, global::Doroti.Framework.Painting.EdgeInsetsGeometry.infinity));
        global::Doroti.Framework.Widgets.Widget result = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: effectiveConstraintsLocal, child: new Material(elevation: this._effectiveElevation, textStyle: ((RawMaterialButton)this.widget).textStyle?.copyWith(color: effectiveTextColor), shape: effectiveShape, color: ((RawMaterialButton)this.widget).fillColor, shadowColor: (Theme.of(context).useMaterial3 ? Theme.of(context).shadowColor : null), type: ((((RawMaterialButton)this.widget).fillColor is null) ? MaterialType.transparency : MaterialType.button), animationDuration: ((RawMaterialButton)this.widget).animationDuration, clipBehavior: ((RawMaterialButton)this.widget).clipBehavior, child: new InkWell(focusNode: ((RawMaterialButton)this.widget).focusNode, canRequestFocus: ((RawMaterialButton)this.widget).enabled, onFocusChange: updateMaterialState(global::Doroti.Framework.Widgets.WidgetState.focused), autofocus: ((RawMaterialButton)this.widget).autofocus, onHighlightChanged: updateMaterialState(global::Doroti.Framework.Widgets.WidgetState.pressed, onChanged: (global::System.Action<bool>?)((RawMaterialButton)this.widget).onHighlightChanged), splashColor: ((RawMaterialButton)this.widget).splashColor, highlightColor: ((RawMaterialButton)this.widget).highlightColor, focusColor: ((RawMaterialButton)this.widget).focusColor, hoverColor: ((RawMaterialButton)this.widget).hoverColor, onHover: updateMaterialState(global::Doroti.Framework.Widgets.WidgetState.hovered), onTap: ((RawMaterialButton)this.widget).onPressed, onLongPress: ((RawMaterialButton)this.widget).onLongPress, enableFeedback: ((RawMaterialButton)this.widget).enableFeedback, customBorder: effectiveShape, mouseCursor: effectiveMouseCursor, child: IconTheme.merge(data: new global::Doroti.Framework.Widgets.IconThemeData(color: effectiveTextColor), child: new global::Doroti.Framework.Widgets.Padding(padding: paddingLocal, child: new global::Doroti.Framework.Widgets.Center(widthFactor: 1.0, heightFactor: 1.0, child: ((RawMaterialButton)this.widget).child)))))));
        global::Doroti.Ui.Size minSizeLocal = default!;
        switch (((RawMaterialButton)this.widget).materialTapTargetSize)
        {
            case var __constant15001 when (object.Equals(__constant15001, MaterialTapTargetSize.padded)):
                {
                    minSizeLocal = new global::Doroti.Ui.Size((ConstantsLibrary.kMinInteractiveDimension + densityAdjustment.dx), (ConstantsLibrary.kMinInteractiveDimension + densityAdjustment.dy));
                    DartRuntimePrimitives.Assert(() => (minSizeLocal.width >= 0.0));
                    DartRuntimePrimitives.Assert(() => (minSizeLocal.height >= 0.0));
                    break;
                }
            case var __constant15272 when (object.Equals(__constant15272, MaterialTapTargetSize.shrinkWrap)):
                {
                    minSizeLocal = Size.zero;
                    break;
                }
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(container: true, button: true, enabled: ((RawMaterialButton)this.widget).enabled, child: new _InputPadding__button(minSize: minSizeLocal, child: result)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::System.Action<bool> updateMaterialState(global::Doroti.Framework.Widgets.WidgetState key, global::System.Action<bool>? onChanged = null)
    {
        return ((global::System.Action<bool>)((value) =>
        {
            if ((this.materialStates.Contains(key) == value))
            {
                return;
            }
            setMaterialState(key, value);
            onChanged?.Invoke(value);
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void setMaterialState(global::Doroti.Framework.Widgets.WidgetState state, bool isSet)
    {
        if (isSet) { addMaterialState(state); } else { removeMaterialState(state); }
        return;
    }

    public virtual void addMaterialState(global::Doroti.Framework.Widgets.WidgetState state)
    {
        if (this.materialStates.Add(state))
        {
            setState(((global::System.Action)(() =>
            {
            })));
        }
    }

    public virtual void removeMaterialState(global::Doroti.Framework.Widgets.WidgetState state)
    {
        if (this.materialStates.Remove(state))
        {
            setState(((global::System.Action)(() =>
            {
            })));
        }
    }

    public virtual bool isDisabled => this.materialStates.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled);
    public virtual bool isDragged => this.materialStates.Contains(global::Doroti.Framework.Widgets.WidgetState.dragged);
    public virtual bool isErrored => this.materialStates.Contains(global::Doroti.Framework.Widgets.WidgetState.error);
    public virtual bool isFocused => this.materialStates.Contains(global::Doroti.Framework.Widgets.WidgetState.focused);
    public virtual bool isHovered => this.materialStates.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered);
    public virtual bool isPressed => this.materialStates.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed);
    public virtual bool isScrolledUnder => this.materialStates.Contains(global::Doroti.Framework.Widgets.WidgetState.scrolledUnder);
    public virtual bool isSelected => this.materialStates.Contains(global::Doroti.Framework.Widgets.WidgetState.selected);
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Widgets.WidgetState>>("materialStates", this.materialStates, defaultValue: new HashSet<global::Doroti.Framework.Widgets.WidgetState>()));
    }

}

internal class _InputPadding__button : global::Doroti.Framework.Widgets.SingleChildRenderObjectWidget
{
    public virtual Size minSize { get; private set; } = default!;

    internal _InputPadding__button(global::Doroti.Framework.Widgets.Widget? child = null, Size minSize = default!) : base(child: child)
    {
        this.minSize = minSize;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderInputPadding__button(this.minSize));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderInputPadding__button)(object)renderObject;
        __renderObject.minSize = this.minSize;
    }

}

public class _RenderInputPadding__button : global::Doroti.Framework.Rendering.RenderShiftedBox
{
    internal virtual Size _minSize { get; set; } = default!;

    internal _RenderInputPadding__button(Size _minSize, global::Doroti.Framework.Rendering.RenderBox? child = null) : base(child)
    {
        this._minSize = _minSize;
    }

    public virtual global::Doroti.Ui.Size minSize
    {
        get => this._minSize;
        set
        {
            var __value = value;
            if ((object.Equals(this._minSize, __value)))
            {
                return;
            }
            _minSize = __value;
            markNeedsLayout();
        }
    }
    public override double computeMinIntrinsicWidth(double height)
    {
        if ((this.child is not null))
        {
            return Math.Max(this.child!.getMinIntrinsicWidth(height), this.minSize.width);
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        if ((this.child is not null))
        {
            return Math.Max(this.child!.getMinIntrinsicHeight(width), this.minSize.height);
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        if ((this.child is not null))
        {
            return Math.Max(this.child!.getMaxIntrinsicWidth(height), this.minSize.width);
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        if ((this.child is not null))
        {
            return Math.Max(this.child!.getMaxIntrinsicHeight(width), this.minSize.height);
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _computeSize(global::Doroti.Framework.Rendering.BoxConstraints constraints, global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size> layoutChild)
    {
        if ((this.child is not null))
        {
            global::Doroti.Ui.Size childSize = ((global::Doroti.Ui.Size)(object?)layoutChild(this.child!, constraints));
            double widthLocal = Math.Max(childSize.width, this.minSize.width);
            double heightLocal = Math.Max(childSize.height, this.minSize.height);
            return ((global::Doroti.Ui.Size)(object?)constraints.constrain(new global::Doroti.Ui.Size(widthLocal, heightLocal)));
        }
        return Size.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return _computeSize(constraints: constraints, layoutChild: (global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size>)global::Doroti.Framework.Rendering.ChildLayoutHelper.dryLayoutChild);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((childLocal is null))
        {
            return null;
        }
        double? result = childLocal.getDryBaseline(constraints, baseline);
        if ((result is null))
        {
            return null;
        }
        global::Doroti.Ui.Size childSize = ((global::Doroti.Ui.Size)(object?)childLocal.getDryLayout(constraints));
        return (DartRuntimePrimitives.RequireValue(result) + global::Doroti.Framework.Painting.Alignment.center.alongOffset((getDryLayout(constraints) - childSize)).dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        size = _computeSize(constraints: this.constraints, layoutChild: (global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size>)global::Doroti.Framework.Rendering.ChildLayoutHelper.layoutChild);
        if ((this.child is not null))
        {
            var childParentData = ((global::Doroti.Framework.Rendering.BoxParentData?)(object?)this.child!.parentData!)!;
            childParentData.offset = global::Doroti.Framework.Painting.Alignment.center.alongOffset((this.size - this.child!.size));
        }
    }

    public override bool hitTest(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        if (base.hitTest(result, position: position))
        {
            return true;
        }
        global::Doroti.Ui.Offset centerLocal = ((global::Doroti.Ui.Offset)(object?)this.child!.size.center(Offset.zero));
        return result.addWithRawTransform(transform: MatrixUtils.forceToPoint(centerLocal), position: centerLocal, hitTest: ((global::System.Func<global::Doroti.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, position) =>
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(position, centerLocal)));
            return this.child!.hitTest(result, position: centerLocal);
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
