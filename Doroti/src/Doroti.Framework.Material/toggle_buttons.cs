// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/toggle_buttons.dart
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

public class ToggleButtons : global::Doroti.Framework.Widgets.StatelessWidget
{
    internal const double _defaultBorderWidth = 1.0;
    public virtual List<global::Doroti.Framework.Widgets.Widget> children { get; private set; } = default!;
    public virtual List<bool> isSelected { get; private set; } = default!;
    public virtual global::System.Action<long>? onPressed { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual MaterialTapTargetSize? tapTargetSize { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? textStyle { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? constraints { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual Color? selectedColor { get; private set; }
    public virtual Color? disabledColor { get; private set; }
    public virtual Color? fillColor { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual Color? highlightColor { get; private set; }
    public virtual Color? splashColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual List<global::Doroti.Framework.Widgets.FocusNode>? focusNodes { get; private set; }
    public virtual bool renderBorder { get; private set; } = default!;
    public virtual Color? borderColor { get; private set; }
    public virtual Color? selectedBorderColor { get; private set; }
    public virtual Color? disabledBorderColor { get; private set; }
    public virtual double? borderWidth { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderRadius? borderRadius { get; private set; }
    public virtual global::Doroti.Framework.Painting.Axis direction { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.VerticalDirection verticalDirection { get; private set; } = default!;

    public ToggleButtons(global::Doroti.Framework.Foundation.Key? key = null, List<global::Doroti.Framework.Widgets.Widget> children = default!, List<bool> isSelected = default!, global::System.Action<long>? onPressed = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, MaterialTapTargetSize? tapTargetSize = null, global::Doroti.Framework.Painting.TextStyle? textStyle = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, Color? color = null, Color? selectedColor = null, Color? disabledColor = null, Color? fillColor = null, Color? focusColor = null, Color? highlightColor = null, Color? hoverColor = null, Color? splashColor = null, List<global::Doroti.Framework.Widgets.FocusNode>? focusNodes = null, bool renderBorder = true, Color? borderColor = null, Color? selectedBorderColor = null, Color? disabledBorderColor = null, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, double? borderWidth = null, global::Doroti.Framework.Painting.Axis direction = global::Doroti.Framework.Painting.Axis.horizontal, global::Doroti.Framework.Painting.VerticalDirection verticalDirection = global::Doroti.Framework.Painting.VerticalDirection.down) : base(key: key)
    {
        this.children = children;
        this.isSelected = isSelected;
        this.onPressed = onPressed;
        this.mouseCursor = mouseCursor;
        this.tapTargetSize = tapTargetSize;
        this.textStyle = textStyle;
        this.constraints = constraints;
        this.color = color;
        this.selectedColor = selectedColor;
        this.disabledColor = disabledColor;
        this.fillColor = fillColor;
        this.focusColor = focusColor;
        this.highlightColor = highlightColor;
        this.hoverColor = hoverColor;
        this.splashColor = splashColor;
        this.focusNodes = focusNodes;
        this.renderBorder = renderBorder;
        this.borderColor = borderColor;
        this.selectedBorderColor = selectedBorderColor;
        this.disabledBorderColor = disabledBorderColor;
        this.borderRadius = borderRadius;
        this.borderWidth = borderWidth;
        this.direction = direction;
        this.verticalDirection = verticalDirection;
        System.Diagnostics.Debug.Assert((checked((long)(children.Count)) == checked((long)(isSelected.Count))));
    }

    internal virtual bool _isFirstButton(long index, long length, TextDirection textDirection)
    {
        switch (this.direction)
        {
            case global::Doroti.Framework.Painting.Axis.horizontal:
                {
                    return (textDirection switch { TextDirection.rtl => (index == (length - 1L)), TextDirection.ltr => (index == 0L), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                }
            case global::Doroti.Framework.Painting.Axis.vertical:
                {
                    return (this.verticalDirection switch { global::Doroti.Framework.Painting.VerticalDirection.up => (index == (length - 1L)), global::Doroti.Framework.Painting.VerticalDirection.down => (index == 0L), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isLastButton(long index, long length, TextDirection textDirection)
    {
        switch (this.direction)
        {
            case global::Doroti.Framework.Painting.Axis.horizontal:
                {
                    return (textDirection switch { TextDirection.rtl => (index == 0L), TextDirection.ltr => (index == (length - 1L)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                }
            case global::Doroti.Framework.Painting.Axis.vertical:
                {
                    return (this.verticalDirection switch { global::Doroti.Framework.Painting.VerticalDirection.up => (index == 0L), global::Doroti.Framework.Painting.VerticalDirection.down => (index == (length - 1L)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.BorderRadius _getEdgeBorderRadius(long index, long length, TextDirection textDirection, ToggleButtonsThemeData toggleButtonsTheme)
    {
        global::Doroti.Framework.Painting.BorderRadius resultingBorderRadius = ((this.borderRadius ?? toggleButtonsTheme.borderRadius) ?? global::Doroti.Framework.Painting.BorderRadius.zero);
        if ((length == 1L))
        {
            return resultingBorderRadius;
        }
        else
        {
            if ((object.Equals(this.direction, global::Doroti.Framework.Painting.Axis.horizontal)))
            {
                if (_isFirstButton(index, length, textDirection))
                {
                    return new global::Doroti.Framework.Painting.BorderRadius(topLeft: ((global::Doroti.Framework.Painting.BorderRadius)resultingBorderRadius).topLeft, bottomLeft: ((global::Doroti.Framework.Painting.BorderRadius)resultingBorderRadius).bottomLeft);
                }
                else
                {
                    if (_isLastButton(index, length, textDirection))
                    {
                        return new global::Doroti.Framework.Painting.BorderRadius(topRight: ((global::Doroti.Framework.Painting.BorderRadius)resultingBorderRadius).topRight, bottomRight: ((global::Doroti.Framework.Painting.BorderRadius)resultingBorderRadius).bottomRight);
                    }
                }
            }
            else
            {
                if (_isFirstButton(index, length, textDirection))
                {
                    return new global::Doroti.Framework.Painting.BorderRadius(topLeft: ((global::Doroti.Framework.Painting.BorderRadius)resultingBorderRadius).topLeft, topRight: ((global::Doroti.Framework.Painting.BorderRadius)resultingBorderRadius).topRight);
                }
                else
                {
                    if (_isLastButton(index, length, textDirection))
                    {
                        return new global::Doroti.Framework.Painting.BorderRadius(bottomLeft: ((global::Doroti.Framework.Painting.BorderRadius)resultingBorderRadius).bottomLeft, bottomRight: ((global::Doroti.Framework.Painting.BorderRadius)resultingBorderRadius).bottomRight);
                    }
                }
            }
        }
        return global::Doroti.Framework.Painting.BorderRadius.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.BorderRadius _getClipBorderRadius(long index, long length, TextDirection textDirection, ToggleButtonsThemeData toggleButtonsTheme)
    {
        global::Doroti.Framework.Painting.BorderRadius resultingBorderRadius = ((this.borderRadius ?? toggleButtonsTheme.borderRadius) ?? global::Doroti.Framework.Painting.BorderRadius.zero);
        double resultingBorderWidth = ((this.borderWidth ?? toggleButtonsTheme.borderWidth) ?? _defaultBorderWidth);
        if ((length == 1L))
        {
            return new global::Doroti.Framework.Painting.BorderRadius(topLeft: (((global::Doroti.Framework.Painting.BorderRadius)resultingBorderRadius).topLeft - global::Doroti.Ui.Radius.circular((resultingBorderWidth / 2.0))), bottomLeft: (((global::Doroti.Framework.Painting.BorderRadius)resultingBorderRadius).bottomLeft - global::Doroti.Ui.Radius.circular((resultingBorderWidth / 2.0))), topRight: (((global::Doroti.Framework.Painting.BorderRadius)resultingBorderRadius).topRight - global::Doroti.Ui.Radius.circular((resultingBorderWidth / 2.0))), bottomRight: (((global::Doroti.Framework.Painting.BorderRadius)resultingBorderRadius).bottomRight - global::Doroti.Ui.Radius.circular((resultingBorderWidth / 2.0))));
        }
        else
        {
            if ((object.Equals(this.direction, global::Doroti.Framework.Painting.Axis.horizontal)))
            {
                if (_isFirstButton(index, length, textDirection))
                {
                    return new global::Doroti.Framework.Painting.BorderRadius(topLeft: (((global::Doroti.Framework.Painting.BorderRadius)resultingBorderRadius).topLeft - global::Doroti.Ui.Radius.circular((resultingBorderWidth / 2.0))), bottomLeft: (((global::Doroti.Framework.Painting.BorderRadius)resultingBorderRadius).bottomLeft - global::Doroti.Ui.Radius.circular((resultingBorderWidth / 2.0))));
                }
                else
                {
                    if (_isLastButton(index, length, textDirection))
                    {
                        return new global::Doroti.Framework.Painting.BorderRadius(topRight: (((global::Doroti.Framework.Painting.BorderRadius)resultingBorderRadius).topRight - global::Doroti.Ui.Radius.circular((resultingBorderWidth / 2.0))), bottomRight: (((global::Doroti.Framework.Painting.BorderRadius)resultingBorderRadius).bottomRight - global::Doroti.Ui.Radius.circular((resultingBorderWidth / 2.0))));
                    }
                }
            }
            else
            {
                if (_isFirstButton(index, length, textDirection))
                {
                    return new global::Doroti.Framework.Painting.BorderRadius(topLeft: (((global::Doroti.Framework.Painting.BorderRadius)resultingBorderRadius).topLeft - global::Doroti.Ui.Radius.circular((resultingBorderWidth / 2.0))), topRight: (((global::Doroti.Framework.Painting.BorderRadius)resultingBorderRadius).topRight - global::Doroti.Ui.Radius.circular((resultingBorderWidth / 2.0))));
                }
                else
                {
                    if (_isLastButton(index, length, textDirection))
                    {
                        return new global::Doroti.Framework.Painting.BorderRadius(bottomLeft: (((global::Doroti.Framework.Painting.BorderRadius)resultingBorderRadius).bottomLeft - global::Doroti.Ui.Radius.circular((resultingBorderWidth / 2.0))), bottomRight: (((global::Doroti.Framework.Painting.BorderRadius)resultingBorderRadius).bottomRight - global::Doroti.Ui.Radius.circular((resultingBorderWidth / 2.0))));
                    }
                }
            }
        }
        return global::Doroti.Framework.Painting.BorderRadius.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.BorderSide _getLeadingBorderSide(long index, ThemeData theme, ToggleButtonsThemeData toggleButtonsTheme)
    {
        if (!this.renderBorder)
        {
            return global::Doroti.Framework.Painting.BorderSide.none;
        }
        double resultingBorderWidth = ((this.borderWidth ?? toggleButtonsTheme.borderWidth) ?? _defaultBorderWidth);
        if (((this.onPressed is not null) && ((this.isSelected[(int)(index)] || (((index != 0L) && this.isSelected[(int)((index - 1L))]))))))
        {
            return new global::Doroti.Framework.Painting.BorderSide(color: ((this.selectedBorderColor ?? toggleButtonsTheme.selectedBorderColor) ?? theme.colorScheme.onSurface.withOpacity(0.12)), width: resultingBorderWidth);
        }
        else
        {
            if (((this.onPressed is not null) && !this.isSelected[(int)(index)]))
            {
                return new global::Doroti.Framework.Painting.BorderSide(color: ((this.borderColor ?? toggleButtonsTheme.borderColor) ?? theme.colorScheme.onSurface.withOpacity(0.12)), width: resultingBorderWidth);
            }
            else
            {
                return new global::Doroti.Framework.Painting.BorderSide(color: ((this.disabledBorderColor ?? toggleButtonsTheme.disabledBorderColor) ?? theme.colorScheme.onSurface.withOpacity(0.12)), width: resultingBorderWidth);
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.BorderSide _getBorderSide(long index, ThemeData theme, ToggleButtonsThemeData toggleButtonsTheme)
    {
        if (!this.renderBorder)
        {
            return global::Doroti.Framework.Painting.BorderSide.none;
        }
        double resultingBorderWidth = ((this.borderWidth ?? toggleButtonsTheme.borderWidth) ?? _defaultBorderWidth);
        if (((this.onPressed is not null) && this.isSelected[(int)(index)]))
        {
            return new global::Doroti.Framework.Painting.BorderSide(color: ((this.selectedBorderColor ?? toggleButtonsTheme.selectedBorderColor) ?? theme.colorScheme.onSurface.withOpacity(0.12)), width: resultingBorderWidth);
        }
        else
        {
            if (((this.onPressed is not null) && !this.isSelected[(int)(index)]))
            {
                return new global::Doroti.Framework.Painting.BorderSide(color: ((this.borderColor ?? toggleButtonsTheme.borderColor) ?? theme.colorScheme.onSurface.withOpacity(0.12)), width: resultingBorderWidth);
            }
            else
            {
                return new global::Doroti.Framework.Painting.BorderSide(color: ((this.disabledBorderColor ?? toggleButtonsTheme.disabledBorderColor) ?? theme.colorScheme.onSurface.withOpacity(0.12)), width: resultingBorderWidth);
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.BorderSide _getTrailingBorderSide(long index, ThemeData theme, ToggleButtonsThemeData toggleButtonsTheme)
    {
        if (!this.renderBorder)
        {
            return global::Doroti.Framework.Painting.BorderSide.none;
        }
        if ((index != (checked((long)(this.children.Count)) - 1L)))
        {
            return global::Doroti.Framework.Painting.BorderSide.none;
        }
        double resultingBorderWidth = ((this.borderWidth ?? toggleButtonsTheme.borderWidth) ?? _defaultBorderWidth);
        if (((this.onPressed is not null) && (this.isSelected[(int)(index)])))
        {
            return new global::Doroti.Framework.Painting.BorderSide(color: ((this.selectedBorderColor ?? toggleButtonsTheme.selectedBorderColor) ?? theme.colorScheme.onSurface.withOpacity(0.12)), width: resultingBorderWidth);
        }
        else
        {
            if (((this.onPressed is not null) && !this.isSelected[(int)(index)]))
            {
                return new global::Doroti.Framework.Painting.BorderSide(color: ((this.borderColor ?? toggleButtonsTheme.borderColor) ?? theme.colorScheme.onSurface.withOpacity(0.12)), width: resultingBorderWidth);
            }
            else
            {
                return new global::Doroti.Framework.Painting.BorderSide(color: ((this.disabledBorderColor ?? toggleButtonsTheme.disabledBorderColor) ?? theme.colorScheme.onSurface.withOpacity(0.12)), width: resultingBorderWidth);
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this.focusNodes is not null))
                {
                    return (checked((long)(this.focusNodes!.Count)) == checked((long)(this.children.Count)));
                }
                return true;
            }, () => (object?)"focusNodes.length must match children.length.\n" + $"There are {checked((long)(this.focusNodes!.Count))} focus nodes, while " + $"there are {checked((long)(this.children.Count))} children.");
        ThemeData theme = Theme.of(context);
        ToggleButtonsThemeData toggleButtonsTheme = ToggleButtonsTheme.of(context);
        global::Doroti.Ui.TextDirection textDirection = Directionality.of(context);
        var buttons = new List<global::Doroti.Framework.Widgets.Widget>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)checked((long)(this.children.Count)))), ((index) =>
        {
            global::Doroti.Framework.Painting.BorderRadius edgeBorderRadius = ((global::Doroti.Framework.Painting.BorderRadius)(object?)_getEdgeBorderRadius(index, checked((long)(this.children.Count)), textDirection, toggleButtonsTheme));
            global::Doroti.Framework.Painting.BorderRadius clipBorderRadius = ((global::Doroti.Framework.Painting.BorderRadius)(object?)_getClipBorderRadius(index, checked((long)(this.children.Count)), textDirection, toggleButtonsTheme));
            global::Doroti.Framework.Painting.BorderSide leadingBorderSideLocal = ((global::Doroti.Framework.Painting.BorderSide)(object?)_getLeadingBorderSide(index, theme, toggleButtonsTheme));
            global::Doroti.Framework.Painting.BorderSide borderSideLocal = ((global::Doroti.Framework.Painting.BorderSide)(object?)_getBorderSide(index, theme, toggleButtonsTheme));
            global::Doroti.Framework.Painting.BorderSide trailingBorderSideLocal = ((global::Doroti.Framework.Painting.BorderSide)(object?)_getTrailingBorderSide(index, theme, toggleButtonsTheme));
            var states = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection26216 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if ((this.isSelected[(int)(index)] && (this.onPressed is not null))) { __collection26216.Add(global::Doroti.Framework.Widgets.WidgetState.selected); } if ((this.onPressed is null)) { __collection26216.Add(global::Doroti.Framework.Widgets.WidgetState.disabled); } return __collection26216; }))();
            global::Doroti.Ui.Color effectiveFillColor = ((global::Doroti.Ui.Color)(object?)((new _ResolveFillColor__toggle_buttons((this.fillColor ?? toggleButtonsTheme.fillColor)).resolve(states) ?? (Color)new _DefaultFillColor__toggle_buttons(theme.colorScheme).resolve(states))));
            global::Doroti.Ui.Color currentColor = default!;
            if (((this.onPressed is not null) && this.isSelected[(int)(index)]))
            {
                currentColor = ((this.selectedColor ?? toggleButtonsTheme.selectedColor) ?? theme.colorScheme.primary);
            }
            else
            {
                if (((this.onPressed is not null) && !this.isSelected[(int)(index)]))
                {
                    currentColor = ((this.color ?? toggleButtonsTheme.color) ?? theme.colorScheme.onSurface.withOpacity(0.87));
                }
                else
                {
                    currentColor = ((this.disabledColor ?? toggleButtonsTheme.disabledColor) ?? theme.colorScheme.onSurface.withOpacity(0.38));
                }
            }
            global::Doroti.Framework.Painting.TextStyle currentTextStyle = ((this.textStyle ?? toggleButtonsTheme.textStyle) ?? theme.textTheme.bodyMedium!);
            global::Doroti.Framework.Rendering.BoxConstraints? currentConstraints = (this.constraints ?? toggleButtonsTheme.constraints);
            global::Doroti.Ui.Size minimumSizeLocal = ((global::Doroti.Ui.Size)(object?)(currentConstraints?.smallest ?? new global::Doroti.Ui.Size(global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension)));
            global::Doroti.Ui.Size? maximumSizeLocal = ((global::Doroti.Ui.Size?)(object?)currentConstraints?.biggest);
            global::Doroti.Ui.Size minPaddingSize = default!;
            switch ((this.tapTargetSize ?? theme.materialTapTargetSize))
            {
                case var __constant27628 when (object.Equals(__constant27628, MaterialTapTargetSize.padded)):
                    {
                        minPaddingSize = (this.direction switch { global::Doroti.Framework.Painting.Axis.horizontal => new global::Doroti.Ui.Size(0.0, global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension), global::Doroti.Framework.Painting.Axis.vertical => new global::Doroti.Ui.Size(global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension, 0.0), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                        DartRuntimePrimitives.Assert(() => (minPaddingSize.width >= 0.0));
                        DartRuntimePrimitives.Assert(() => (minPaddingSize.height >= 0.0));
                        break;
                    }
                case var __constant27973 when (object.Equals(__constant27973, MaterialTapTargetSize.shrinkWrap)):
                    {
                        minPaddingSize = Size.zero;
                        break;
                    }
            }
            global::Doroti.Framework.Widgets.Widget button = ((global::Doroti.Framework.Widgets.Widget)(object?)new _SelectToggleButton__toggle_buttons(leadingBorderSide: leadingBorderSideLocal, borderSide: borderSideLocal, trailingBorderSide: trailingBorderSideLocal, borderRadius: edgeBorderRadius, isFirstButton: (index == 0L), isLastButton: (index == (checked((long)(this.children.Count)) - 1L)), direction: this.direction, verticalDirection: this.verticalDirection, child: new global::Doroti.Framework.Widgets.ClipRRect(borderRadius: clipBorderRadius, child: new TextButton(focusNode: ((this.focusNodes is not null) ? this.focusNodes![(int)(index)] : null), style: new ButtonStyle(backgroundColor: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color?>(effectiveFillColor), foregroundColor: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color?>(currentColor), iconSize: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(24.0), iconColor: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color?>(currentColor), overlayColor: new _ToggleButtonDefaultOverlay__toggle_buttons(selected: ((this.onPressed is not null) && this.isSelected[(int)(index)]), unselected: ((this.onPressed is not null) && !this.isSelected[(int)(index)]), colorScheme: theme.colorScheme, disabledColor: (this.disabledColor ?? toggleButtonsTheme.disabledColor), focusColor: (this.focusColor ?? toggleButtonsTheme.focusColor), highlightColor: (this.highlightColor ?? toggleButtonsTheme.highlightColor), hoverColor: (this.hoverColor ?? toggleButtonsTheme.hoverColor), splashColor: (this.splashColor ?? toggleButtonsTheme.splashColor)), elevation: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(0), textStyle: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.TextStyle?>(currentTextStyle.copyWith(color: currentColor)), padding: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Framework.Painting.EdgeInsets.zero), minimumSize: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Size?>(minimumSizeLocal), maximumSize: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Size?>(maximumSizeLocal), shape: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.OutlinedBorder>(new global::Doroti.Framework.Painting.RoundedRectangleBorder()), mouseCursor: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Services.MouseCursor?>(this.mouseCursor), visualDensity: VisualDensity.standard, tapTargetSize: MaterialTapTargetSize.shrinkWrap, animationDuration: ConstantsLibrary.kThemeChangeDuration, enableFeedback: true, alignment: global::Doroti.Framework.Painting.Alignment.center, splashFactory: InkRipple.splashFactory), onPressed: ((global::System.Action)((this.onPressed is not null) ? (() =>
            {
                this.onPressed!(index);
            }) : null)), child: this.children[(int)(index)]))));
            if ((currentConstraints is not null))
            {
                button = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Center(child: button));
            }
            return new global::Doroti.Framework.Widgets.MergeSemantics(child: new global::Doroti.Framework.Widgets.Semantics(container: true, @checked: this.isSelected[(int)(index)], enabled: (this.onPressed is not null), child: new _InputPadding__toggle_buttons(minSize: minPaddingSize, direction: this.direction, child: button)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        if ((object.Equals(this.direction, global::Doroti.Framework.Painting.Axis.vertical)))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.IntrinsicWidth(child: new global::Doroti.Framework.Widgets.Column(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.stretch, verticalDirection: this.verticalDirection, children: buttons)));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.IntrinsicHeight(child: new global::Doroti.Framework.Widgets.Row(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.stretch, children: buttons)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("disabled", value: (this.onPressed is null), ifTrue: "Buttons are disabled", ifFalse: "Buttons are enabled"));
        this.textStyle?.debugFillProperties(properties, prefix: "textStyle.");
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("color", this.color, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("selectedColor", this.selectedColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("disabledColor", this.disabledColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("fillColor", this.fillColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("focusColor", this.focusColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("highlightColor", this.highlightColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("hoverColor", this.hoverColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("splashColor", this.splashColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("borderColor", this.borderColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("selectedBorderColor", this.selectedBorderColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("disabledBorderColor", this.disabledBorderColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.BorderRadius>("borderRadius", this.borderRadius, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("borderWidth", this.borderWidth, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.Axis>("direction", this.direction, defaultValue: global::Doroti.Framework.Painting.Axis.horizontal));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.VerticalDirection>("verticalDirection", this.verticalDirection, defaultValue: global::Doroti.Framework.Painting.VerticalDirection.down));
    }

}

internal class _ResolveFillColor__toggle_buttons : global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>, global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual Color? primary { get; private set; }

    internal _ResolveFillColor__toggle_buttons(Color? primary)
    {
        this.primary = primary;
    }

    public virtual Color? resolve(HashSet<global::Doroti.Framework.Widgets.WidgetState> states)
    {
        if ((this.primary is global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>))
        {
            global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> primary__as33643 = (global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>)primary;
            return ((Color?)(object?)WidgetStateProperty.resolveAs<global::Doroti.Ui.Color?>(this.primary, states));
        }
        return (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected) ? this.primary : null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return ((fullString ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
    }

}

internal class _DefaultFillColor__toggle_buttons : global::Doroti.Framework.Widgets.WidgetStateProperty<Color>, global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual ColorScheme colorScheme { get; private set; } = default!;

    internal _DefaultFillColor__toggle_buttons(ColorScheme colorScheme)
    {
        this.colorScheme = colorScheme;
    }

    public virtual Color resolve(HashSet<global::Doroti.Framework.Widgets.WidgetState> states)
    {
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
        {
            return this.colorScheme.primary.withOpacity(0.12);
        }
        return this.colorScheme.surface.withOpacity(0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return ((fullString ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
    }

}

internal class _ToggleButtonDefaultOverlay__toggle_buttons : global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>
{
    public virtual bool selected { get; private set; } = default!;
    public virtual bool unselected { get; private set; } = default!;
    public virtual ColorScheme? colorScheme { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual Color? highlightColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual Color? splashColor { get; private set; }
    public virtual Color? disabledColor { get; private set; }

    internal _ToggleButtonDefaultOverlay__toggle_buttons(bool selected, bool unselected, ColorScheme? colorScheme = null, Color? focusColor = null, Color? highlightColor = null, Color? hoverColor = null, Color? splashColor = null, Color? disabledColor = null)
    {
        this.selected = selected;
        this.unselected = unselected;
        this.colorScheme = colorScheme;
        this.focusColor = focusColor;
        this.highlightColor = highlightColor;
        this.hoverColor = hoverColor;
        this.splashColor = splashColor;
        this.disabledColor = disabledColor;
    }

    public virtual Color? resolve(HashSet<global::Doroti.Framework.Widgets.WidgetState> states)
    {
        if (this.selected)
        {
            if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
            {
                return (this.splashColor ?? this.colorScheme?.primary.withOpacity(0.16));
            }
            if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
            {
                return (this.hoverColor ?? this.colorScheme?.primary.withOpacity(0.04));
            }
            if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
            {
                return (this.focusColor ?? this.colorScheme?.primary.withOpacity(0.12));
            }
        }
        else
        {
            if (this.unselected)
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
                {
                    return ((this.splashColor ?? this.highlightColor) ?? this.colorScheme?.onSurface.withOpacity(0.16));
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
                {
                    return (this.hoverColor ?? this.colorScheme?.onSurface.withOpacity(0.04));
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
                {
                    return (this.focusColor ?? this.colorScheme?.onSurface.withOpacity(0.12));
                }
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"    {{\n      selected:\n        hovered: {this.hoverColor}, otherwise: {this.colorScheme?.primary.withOpacity(0.04)},\n        focused: {this.focusColor}, otherwise: {this.colorScheme?.primary.withOpacity(0.12)},\n        pressed: {this.splashColor}, otherwise: {this.colorScheme?.primary.withOpacity(0.16)},\n      unselected:\n        hovered: {this.hoverColor}, otherwise: {this.colorScheme?.onSurface.withOpacity(0.04)},\n        focused: {this.focusColor}, otherwise: {this.colorScheme?.onSurface.withOpacity(0.12)},\n        pressed: {this.splashColor}, otherwise: {this.colorScheme?.onSurface.withOpacity(0.16)},\n      otherwise: null,\n    }}\n    ";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SelectToggleButton__toggle_buttons : global::Doroti.Framework.Widgets.SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Framework.Painting.BorderSide leadingBorderSide { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.BorderSide borderSide { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.BorderSide trailingBorderSide { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.BorderRadius borderRadius { get; private set; } = default!;
    public virtual bool isFirstButton { get; private set; } = default!;
    public virtual bool isLastButton { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.Axis direction { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.VerticalDirection verticalDirection { get; private set; } = default!;

    internal _SelectToggleButton__toggle_buttons(global::Doroti.Framework.Widgets.Widget child, global::Doroti.Framework.Painting.BorderSide leadingBorderSide, global::Doroti.Framework.Painting.BorderSide borderSide, global::Doroti.Framework.Painting.BorderSide trailingBorderSide, global::Doroti.Framework.Painting.BorderRadius borderRadius, bool isFirstButton, bool isLastButton, global::Doroti.Framework.Painting.Axis direction, global::Doroti.Framework.Painting.VerticalDirection verticalDirection) : base(child: child)
    {
        this.leadingBorderSide = leadingBorderSide;
        this.borderSide = borderSide;
        this.trailingBorderSide = trailingBorderSide;
        this.borderRadius = borderRadius;
        this.isFirstButton = isFirstButton;
        this.isLastButton = isLastButton;
        this.direction = direction;
        this.verticalDirection = verticalDirection;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(new _SelectToggleButtonRenderObject__toggle_buttons(this.leadingBorderSide, this.borderSide, this.trailingBorderSide, this.borderRadius, this.isFirstButton, this.isLastButton, this.direction, this.verticalDirection, Directionality.of(context)));
    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_SelectToggleButtonRenderObject__toggle_buttons)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_SelectToggleButtonRenderObject__toggle_buttons>)(() =>
{
    var __cascade = __renderObject;
    __cascade.leadingBorderSide = this.leadingBorderSide;
    __cascade.borderSide = this.borderSide;
    __cascade.trailingBorderSide = this.trailingBorderSide;
    __cascade.borderRadius = this.borderRadius;
    __cascade.isFirstButton = this.isFirstButton;
    __cascade.isLastButton = this.isLastButton;
    __cascade.direction = this.direction;
    __cascade.verticalDirection = this.verticalDirection;
    __cascade.textDirection = Directionality.of(context);
    return __cascade;
}))());
    }

}

public class _SelectToggleButtonRenderObject__toggle_buttons : global::Doroti.Framework.Rendering.RenderShiftedBox
{
    internal virtual global::Doroti.Framework.Painting.Axis _direction { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.VerticalDirection _verticalDirection { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.BorderSide _leadingBorderSide { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.BorderSide _borderSide { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.BorderSide _trailingBorderSide { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.BorderRadius _borderRadius { get; set; } = default!;
    internal virtual bool _isFirstButton { get; set; } = default!;
    internal virtual bool _isLastButton { get; set; } = default!;
    internal virtual TextDirection _textDirection { get; set; } = default!;

    internal _SelectToggleButtonRenderObject__toggle_buttons(global::Doroti.Framework.Painting.BorderSide _leadingBorderSide, global::Doroti.Framework.Painting.BorderSide _borderSide, global::Doroti.Framework.Painting.BorderSide _trailingBorderSide, global::Doroti.Framework.Painting.BorderRadius _borderRadius, bool _isFirstButton, bool _isLastButton, global::Doroti.Framework.Painting.Axis _direction, global::Doroti.Framework.Painting.VerticalDirection _verticalDirection, TextDirection _textDirection, global::Doroti.Framework.Rendering.RenderBox? child = null) : base(child)
    {
        this._leadingBorderSide = _leadingBorderSide;
        this._borderSide = _borderSide;
        this._trailingBorderSide = _trailingBorderSide;
        this._borderRadius = _borderRadius;
        this._isFirstButton = _isFirstButton;
        this._isLastButton = _isLastButton;
        this._direction = _direction;
        this._verticalDirection = _verticalDirection;
        this._textDirection = _textDirection;
    }

    public virtual global::Doroti.Framework.Painting.Axis direction
    {
        get => this._direction;
        set
        {
            var __value = value;
            if ((object.Equals(this._direction, __value)))
            {
                return;
            }
            _direction = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Painting.VerticalDirection verticalDirection
    {
        get => this._verticalDirection;
        set
        {
            var __value = value;
            if ((object.Equals(this._verticalDirection, __value)))
            {
                return;
            }
            _verticalDirection = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Painting.BorderSide leadingBorderSide
    {
        get => this._leadingBorderSide;
        set
        {
            var __value = value;
            if ((object.Equals(this._leadingBorderSide, __value)))
            {
                return;
            }
            _leadingBorderSide = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Painting.BorderSide borderSide
    {
        get => this._borderSide;
        set
        {
            var __value = value;
            if ((object.Equals(this._borderSide, __value)))
            {
                return;
            }
            _borderSide = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Painting.BorderSide trailingBorderSide
    {
        get => this._trailingBorderSide;
        set
        {
            var __value = value;
            if ((object.Equals(this._trailingBorderSide, __value)))
            {
                return;
            }
            _trailingBorderSide = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Painting.BorderRadius borderRadius
    {
        get => this._borderRadius;
        set
        {
            var __value = value;
            if ((object.Equals(this._borderRadius, __value)))
            {
                return;
            }
            _borderRadius = __value;
            markNeedsLayout();
        }
    }
    public virtual bool isFirstButton
    {
        get => this._isFirstButton;
        set
        {
            var __value = value;
            if ((this._isFirstButton == __value))
            {
                return;
            }
            _isFirstButton = __value;
            markNeedsLayout();
        }
    }
    public virtual bool isLastButton
    {
        get => this._isLastButton;
        set
        {
            var __value = value;
            if ((this._isLastButton == __value))
            {
                return;
            }
            _isLastButton = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.TextDirection textDirection
    {
        get => this._textDirection;
        set
        {
            var __value = value;
            if ((object.Equals(this._textDirection, __value)))
            {
                return;
            }
            _textDirection = __value;
            markNeedsLayout();
        }
    }
    internal static double _maxHeight(global::Doroti.Framework.Rendering.RenderBox? box, double width)
    {
        return (box?.getMaxIntrinsicHeight(width) ?? 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _minHeight(global::Doroti.Framework.Rendering.RenderBox? box, double width)
    {
        return (box?.getMinIntrinsicHeight(width) ?? 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _minWidth(global::Doroti.Framework.Rendering.RenderBox? box, double height)
    {
        return (box?.getMinIntrinsicWidth(height) ?? 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _maxWidth(global::Doroti.Framework.Rendering.RenderBox? box, double height)
    {
        return (box?.getMaxIntrinsicWidth(height) ?? 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        var childOffset = new global::Doroti.Framework.Rendering.BaselineOffset(this.child?.getDistanceToActualBaseline(baseline));
        return (this.direction switch { global::Doroti.Framework.Painting.Axis.horizontal => (childOffset.op_Add(((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width)), global::Doroti.Framework.Painting.Axis.vertical => (childOffset.op_Add((this.verticalDirection switch { global::Doroti.Framework.Painting.VerticalDirection.down => ((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width, global::Doroti.Framework.Painting.VerticalDirection.up => ((global::Doroti.Framework.Painting.BorderSide)this.trailingBorderSide).width, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }).offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return ((object.Equals(this.direction, global::Doroti.Framework.Painting.Axis.horizontal)) ? ((((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width * 2.0) + _SelectToggleButtonRenderObject__toggle_buttons._maxHeight(this.child, width)) : ((((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width + _SelectToggleButtonRenderObject__toggle_buttons._maxHeight(this.child, width)) + ((global::Doroti.Framework.Painting.BorderSide)this.trailingBorderSide).width));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return ((object.Equals(this.direction, global::Doroti.Framework.Painting.Axis.horizontal)) ? ((((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width * 2.0) + _SelectToggleButtonRenderObject__toggle_buttons._minHeight(this.child, width)) : ((((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width + _SelectToggleButtonRenderObject__toggle_buttons._maxHeight(this.child, width)) + ((global::Doroti.Framework.Painting.BorderSide)this.trailingBorderSide).width));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return ((object.Equals(this.direction, global::Doroti.Framework.Painting.Axis.horizontal)) ? ((((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width + _SelectToggleButtonRenderObject__toggle_buttons._maxWidth(this.child, height)) + ((global::Doroti.Framework.Painting.BorderSide)this.trailingBorderSide).width) : ((((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width * 2.0) + _SelectToggleButtonRenderObject__toggle_buttons._maxWidth(this.child, height)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        return ((object.Equals(this.direction, global::Doroti.Framework.Painting.Axis.horizontal)) ? ((((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width + _SelectToggleButtonRenderObject__toggle_buttons._minWidth(this.child, height)) + ((global::Doroti.Framework.Painting.BorderSide)this.trailingBorderSide).width) : ((((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width * 2.0) + _SelectToggleButtonRenderObject__toggle_buttons._minWidth(this.child, height)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return _computeSize(constraints: constraints, layoutChild: (global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size>)global::Doroti.Framework.Rendering.ChildLayoutHelper.dryLayoutChild);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.EdgeInsetsDirectional _childPadding
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (this.child is not null));
            return (this.direction switch { global::Doroti.Framework.Painting.Axis.horizontal => global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: ((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width, end: ((global::Doroti.Framework.Painting.BorderSide)this.trailingBorderSide).width, top: ((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width, bottom: ((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width), global::Doroti.Framework.Painting.Axis.vertical => global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: ((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width, end: ((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width, top: ((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width, bottom: ((global::Doroti.Framework.Painting.BorderSide)this.trailingBorderSide).width), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return default!;
        }
    }
    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        double? childBaseline = this.child?.getDryBaseline(constraints.deflate(this._childPadding), baseline);
        if ((childBaseline is null))
        {
            return null;
        }
        return (DartRuntimePrimitives.RequireValue(childBaseline) + (this.direction switch { global::Doroti.Framework.Painting.Axis.horizontal => ((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width, global::Doroti.Framework.Painting.Axis.vertical => (this.verticalDirection switch { global::Doroti.Framework.Painting.VerticalDirection.down => ((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width, global::Doroti.Framework.Painting.VerticalDirection.up => ((global::Doroti.Framework.Painting.BorderSide)this.trailingBorderSide).width, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        size = _computeSize(constraints: this.constraints, layoutChild: (global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size>)global::Doroti.Framework.Rendering.ChildLayoutHelper.layoutChild);
        if ((this.child is null))
        {
            return;
        }
        var childParentData = ((global::Doroti.Framework.Rendering.BoxParentData?)(object?)this.child!.parentData!)!;
        if ((object.Equals(this.direction, global::Doroti.Framework.Painting.Axis.horizontal)))
        {
            childParentData.offset = (this.textDirection switch { TextDirection.ltr => new global::Doroti.Ui.Offset(((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width, ((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width), TextDirection.rtl => new global::Doroti.Ui.Offset(((global::Doroti.Framework.Painting.BorderSide)this.trailingBorderSide).width, ((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        }
        else
        {
            childParentData.offset = (this.verticalDirection switch { global::Doroti.Framework.Painting.VerticalDirection.down => new global::Doroti.Ui.Offset(((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width, ((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width), global::Doroti.Framework.Painting.VerticalDirection.up => new global::Doroti.Ui.Offset(((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width, ((global::Doroti.Framework.Painting.BorderSide)this.trailingBorderSide).width), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        }
    }

    internal virtual global::Doroti.Ui.Size _computeSize(global::Doroti.Framework.Rendering.BoxConstraints constraints, global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size> layoutChild)
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((childLocal is null))
        {
            var horizontalSize = new global::Doroti.Ui.Size((((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width + ((global::Doroti.Framework.Painting.BorderSide)this.trailingBorderSide).width), (((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width * 2.0));
            return (this.direction switch { global::Doroti.Framework.Painting.Axis.horizontal => constraints.constrain(horizontalSize), global::Doroti.Framework.Painting.Axis.vertical => constraints.constrain(horizontalSize.flipped), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        }
        global::Doroti.Framework.Painting.EdgeInsetsDirectional childPadding = this._childPadding;
        global::Doroti.Framework.Rendering.BoxConstraints innerConstraints = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)constraints.deflate(childPadding));
        return ((global::Doroti.Ui.Size)(object?)constraints.constrain(childPadding.inflateSize(layoutChild(childLocal, innerConstraints))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        base.paint(context, offset);
        global::Doroti.Ui.Offset bottomRightLocal = ((global::Doroti.Ui.Offset)(object?)this.size.bottomRight(offset));
        var outer = global::Doroti.Ui.Rect.fromLTRB(offset.dx, offset.dy, bottomRightLocal.dx, bottomRightLocal.dy);
        global::Doroti.Ui.Rect center = ((global::Doroti.Ui.Rect)(object?)outer.deflate((((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width / 2.0)));
        double sweepAngle = (Dart_mathLibrary.pi / 2.0);
        global::Doroti.Ui.RRect rrect = ((global::Doroti.Ui.RRect)(object?)global::Doroti.Ui.RRect.fromRectAndCorners(center, topLeft: ((((((global::Doroti.Framework.Painting.BorderRadius)this.borderRadius).topLeft.x * ((global::Doroti.Framework.Painting.BorderRadius)this.borderRadius).topLeft.y) != 0.0)) ? ((global::Doroti.Framework.Painting.BorderRadius)this.borderRadius).topLeft : Radius.zero), topRight: ((((((global::Doroti.Framework.Painting.BorderRadius)this.borderRadius).topRight.x * ((global::Doroti.Framework.Painting.BorderRadius)this.borderRadius).topRight.y) != 0.0)) ? ((global::Doroti.Framework.Painting.BorderRadius)this.borderRadius).topRight : Radius.zero), bottomLeft: ((((((global::Doroti.Framework.Painting.BorderRadius)this.borderRadius).bottomLeft.x * ((global::Doroti.Framework.Painting.BorderRadius)this.borderRadius).bottomLeft.y) != 0.0)) ? ((global::Doroti.Framework.Painting.BorderRadius)this.borderRadius).bottomLeft : Radius.zero), bottomRight: ((((((global::Doroti.Framework.Painting.BorderRadius)this.borderRadius).bottomRight.x * ((global::Doroti.Framework.Painting.BorderRadius)this.borderRadius).bottomRight.y) != 0.0)) ? ((global::Doroti.Framework.Painting.BorderRadius)this.borderRadius).bottomRight : Radius.zero)).scaleRadii());
        var tlCorner = global::Doroti.Ui.Rect.fromLTWH(rrect.left, rrect.top, (rrect.tlRadiusX * 2.0), (rrect.tlRadiusY * 2.0));
        var blCorner = global::Doroti.Ui.Rect.fromLTWH(rrect.left, (rrect.bottom - ((rrect.blRadiusY * 2.0))), (rrect.blRadiusX * 2.0), (rrect.blRadiusY * 2.0));
        var trCorner = global::Doroti.Ui.Rect.fromLTWH((rrect.right - ((rrect.trRadiusX * 2L))), rrect.top, (rrect.trRadiusX * 2L), (rrect.trRadiusY * 2L));
        var brCorner = global::Doroti.Ui.Rect.fromLTWH((rrect.right - ((rrect.brRadiusX * 2L))), (rrect.bottom - ((rrect.brRadiusY * 2L))), (rrect.brRadiusX * 2L), (rrect.brRadiusY * 2L));
        global::Doroti.Ui.Paint leadingPaint = ((global::Doroti.Ui.Paint)(object?)this.leadingBorderSide.toPaint());
        if ((this.isFirstButton && this.isLastButton))
        {
            var leadingPath = new global::Doroti.Ui.Path();
            double startX = (((rrect.brRadiusX == 0.0)) ? outer.right : (rrect.right - rrect.brRadiusX));
            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = leadingPath;
    __cascade.moveTo(startX, rrect.bottom);
    __cascade.lineTo((rrect.left + rrect.blRadiusX), rrect.bottom);
    __cascade.addArc(blCorner, (Dart_mathLibrary.pi / 2.0), sweepAngle);
    __cascade.lineTo(rrect.left, (rrect.top + rrect.tlRadiusY));
    __cascade.addArc(tlCorner, Dart_mathLibrary.pi, sweepAngle);
    __cascade.lineTo((rrect.right - rrect.trRadiusX), rrect.top);
    __cascade.addArc(trCorner, ((Dart_mathLibrary.pi * 3.0) / 2.0), sweepAngle);
    __cascade.lineTo(rrect.right, (rrect.bottom - rrect.brRadiusY));
    __cascade.addArc(brCorner, 0, sweepAngle);
    return __cascade;
}))());
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawPath(leadingPath, leadingPaint);
            return;
        }
        if ((object.Equals(this.direction, global::Doroti.Framework.Painting.Axis.horizontal)))
        {
            switch (this.textDirection)
            {
                case TextDirection.ltr:
                    {
                        if (this.isLastButton)
                        {
                            var leftPath = new global::Doroti.Ui.Path();
                            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = leftPath;
    __cascade.moveTo(rrect.left, (rrect.bottom + (((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2L)));
    __cascade.lineTo(rrect.left, (rrect.top - (((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2L)));
    return __cascade;
}))());
                            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawPath(leftPath, leadingPaint);
                            global::Doroti.Ui.Paint endingPaint = ((global::Doroti.Ui.Paint)(object?)this.trailingBorderSide.toPaint());
                            var endingPath = new global::Doroti.Ui.Path();
                            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = endingPath;
    __cascade.moveTo((rrect.left + (((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width / 2.0)), rrect.top);
    __cascade.lineTo((rrect.right - rrect.trRadiusX), rrect.top);
    __cascade.addArc(trCorner, ((Dart_mathLibrary.pi * 3.0) / 2.0), sweepAngle);
    __cascade.lineTo(rrect.right, (rrect.bottom - rrect.brRadiusY));
    __cascade.addArc(brCorner, 0, sweepAngle);
    __cascade.lineTo((rrect.left + (((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width / 2.0)), rrect.bottom);
    return __cascade;
}))());
                            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawPath(endingPath, endingPaint);
                        }
                        else
                        {
                            if (this.isFirstButton)
                            {
                                var leadingPathLocal = new global::Doroti.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = leadingPathLocal;
    __cascade.moveTo(outer.right, rrect.bottom);
    __cascade.lineTo((rrect.left + rrect.blRadiusX), rrect.bottom);
    __cascade.addArc(blCorner, (Dart_mathLibrary.pi / 2.0), sweepAngle);
    __cascade.lineTo(rrect.left, (rrect.top + rrect.tlRadiusY));
    __cascade.addArc(tlCorner, Dart_mathLibrary.pi, sweepAngle);
    __cascade.lineTo(outer.right, rrect.top);
    return __cascade;
}))());
                                ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawPath(leadingPathLocal, leadingPaint);
                            }
                            else
                            {
                                var leadingPathAlternate = new global::Doroti.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = leadingPathAlternate;
    __cascade.moveTo(rrect.left, (rrect.bottom + (((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2L)));
    __cascade.lineTo(rrect.left, (rrect.top - (((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2L)));
    return __cascade;
}))());
                                ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawPath(leadingPathAlternate, leadingPaint);
                                global::Doroti.Ui.Paint horizontalPaint = ((global::Doroti.Ui.Paint)(object?)this.borderSide.toPaint());
                                var horizontalPaths = new global::Doroti.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = horizontalPaths;
    __cascade.moveTo((rrect.left + (((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width / 2.0)), rrect.top);
    __cascade.lineTo((outer.right - rrect.trRadiusX), rrect.top);
    __cascade.moveTo(((rrect.left + (((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width / 2.0)) + rrect.tlRadiusX), rrect.bottom);
    __cascade.lineTo((outer.right - rrect.trRadiusX), rrect.bottom);
    return __cascade;
}))());
                                ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawPath(horizontalPaths, horizontalPaint);
                            }
                        }
                        break;
                    }
                case TextDirection.rtl:
                    {
                        if (this.isLastButton)
                        {
                            var leadingPathNested = new global::Doroti.Ui.Path();
                            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = leadingPathNested;
    __cascade.moveTo(rrect.right, (rrect.bottom + (((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2L)));
    __cascade.lineTo(rrect.right, (rrect.top - (((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2L)));
    return __cascade;
}))());
                            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawPath(leadingPathNested, leadingPaint);
                            global::Doroti.Ui.Paint endingPaintLocal = ((global::Doroti.Ui.Paint)(object?)this.trailingBorderSide.toPaint());
                            var endingPathLocal = new global::Doroti.Ui.Path();
                            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = endingPathLocal;
    __cascade.moveTo((rrect.right - (((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width / 2.0)), rrect.top);
    __cascade.lineTo((rrect.left + rrect.tlRadiusX), rrect.top);
    __cascade.addArc(tlCorner, ((Dart_mathLibrary.pi * 3.0) / 2.0), -sweepAngle);
    __cascade.lineTo(rrect.left, (rrect.bottom - rrect.blRadiusY));
    __cascade.addArc(blCorner, Dart_mathLibrary.pi, -sweepAngle);
    __cascade.lineTo((rrect.right - (((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width / 2.0)), rrect.bottom);
    return __cascade;
}))());
                            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawPath(endingPathLocal, endingPaintLocal);
                        }
                        else
                        {
                            if (this.isFirstButton)
                            {
                                var leadingPathCurrent = new global::Doroti.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = leadingPathCurrent;
    __cascade.moveTo(outer.left, rrect.bottom);
    __cascade.lineTo((rrect.right - rrect.brRadiusX), rrect.bottom);
    __cascade.addArc(brCorner, (Dart_mathLibrary.pi / 2.0), -sweepAngle);
    __cascade.lineTo(rrect.right, (rrect.top + rrect.trRadiusY));
    __cascade.addArc(trCorner, 0, -sweepAngle);
    __cascade.lineTo(outer.left, rrect.top);
    return __cascade;
}))());
                                ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawPath(leadingPathCurrent, leadingPaint);
                            }
                            else
                            {
                                var leadingPathNext = new global::Doroti.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = leadingPathNext;
    __cascade.moveTo(rrect.right, (rrect.bottom + (((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2L)));
    __cascade.lineTo(rrect.right, (rrect.top - (((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2L)));
    return __cascade;
}))());
                                ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawPath(leadingPathNext, leadingPaint);
                                global::Doroti.Ui.Paint horizontalPaintLocal = ((global::Doroti.Ui.Paint)(object?)this.borderSide.toPaint());
                                var horizontalPathsLocal = new global::Doroti.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = horizontalPathsLocal;
    __cascade.moveTo((rrect.right - (((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width / 2.0)), rrect.top);
    __cascade.lineTo((outer.left - rrect.tlRadiusX), rrect.top);
    __cascade.moveTo(((rrect.right - (((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width / 2.0)) + rrect.trRadiusX), rrect.bottom);
    __cascade.lineTo((outer.left - rrect.tlRadiusX), rrect.bottom);
    return __cascade;
}))());
                                ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawPath(horizontalPathsLocal, horizontalPaintLocal);
                            }
                        }
                        break;
                    }
            }
        }
        else
        {
            switch (this.verticalDirection)
            {
                case global::Doroti.Framework.Painting.VerticalDirection.down:
                    {
                        if (this.isLastButton)
                        {
                            var topPath = new global::Doroti.Ui.Path();
                            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = topPath;
    __cascade.moveTo(outer.left, (outer.top + (((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2L)));
    __cascade.lineTo(outer.right, (outer.top + (((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2L)));
    return __cascade;
}))());
                            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawPath(topPath, leadingPaint);
                            global::Doroti.Ui.Paint endingPaintAlternate = ((global::Doroti.Ui.Paint)(object?)this.trailingBorderSide.toPaint());
                            var endingPathAlternate = new global::Doroti.Ui.Path();
                            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = endingPathAlternate;
    __cascade.moveTo(rrect.left, (rrect.top + (((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2.0)));
    __cascade.lineTo(rrect.left, (rrect.bottom - rrect.blRadiusY));
    __cascade.addArc(blCorner, (Dart_mathLibrary.pi * 3.0), -sweepAngle);
    __cascade.lineTo((rrect.right - rrect.blRadiusX), rrect.bottom);
    __cascade.addArc(brCorner, (Dart_mathLibrary.pi / 2.0), -sweepAngle);
    __cascade.lineTo(rrect.right, (rrect.top + (((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2.0)));
    return __cascade;
}))());
                            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawPath(endingPathAlternate, endingPaintAlternate);
                        }
                        else
                        {
                            if (this.isFirstButton)
                            {
                                var leadingPathCandidate = new global::Doroti.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = leadingPathCandidate;
    __cascade.moveTo(rrect.left, outer.bottom);
    __cascade.lineTo(rrect.left, (rrect.top + rrect.tlRadiusX));
    __cascade.addArc(tlCorner, Dart_mathLibrary.pi, sweepAngle);
    __cascade.lineTo((rrect.right - rrect.trRadiusX), rrect.top);
    __cascade.addArc(trCorner, ((Dart_mathLibrary.pi * 3.0) / 2.0), sweepAngle);
    __cascade.lineTo(rrect.right, outer.bottom);
    return __cascade;
}))());
                                ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawPath(leadingPathCandidate, leadingPaint);
                            }
                            else
                            {
                                var topPathLocal = new global::Doroti.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = topPathLocal;
    __cascade.moveTo(outer.left, (outer.top + (((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2L)));
    __cascade.lineTo(outer.right, (outer.top + (((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2L)));
    return __cascade;
}))());
                                ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawPath(topPathLocal, leadingPaint);
                                global::Doroti.Ui.Paint paintLocal = ((global::Doroti.Ui.Paint)(object?)this.borderSide.toPaint());
                                var paths = new global::Doroti.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = paths;
    __cascade.moveTo(rrect.left, (outer.top + ((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width));
    __cascade.lineTo(rrect.left, outer.bottom);
    __cascade.moveTo(rrect.right, (outer.top + ((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width));
    __cascade.lineTo(rrect.right, outer.bottom);
    return __cascade;
}))());
                                ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawPath(paths, paintLocal);
                            }
                        }
                        break;
                    }
                case global::Doroti.Framework.Painting.VerticalDirection.up:
                    {
                        if (this.isLastButton)
                        {
                            var bottomPath = new global::Doroti.Ui.Path();
                            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = bottomPath;
    __cascade.moveTo(outer.left, (outer.bottom - (((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2.0)));
    __cascade.lineTo(outer.right, (outer.bottom - (((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2.0)));
    return __cascade;
}))());
                            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawPath(bottomPath, leadingPaint);
                            global::Doroti.Ui.Paint endingPaintNested = ((global::Doroti.Ui.Paint)(object?)this.trailingBorderSide.toPaint());
                            var endingPathNested = new global::Doroti.Ui.Path();
                            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = endingPathNested;
    __cascade.moveTo(rrect.left, (rrect.bottom - (((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2.0)));
    __cascade.lineTo(rrect.left, (rrect.top + rrect.tlRadiusY));
    __cascade.addArc(tlCorner, Dart_mathLibrary.pi, sweepAngle);
    __cascade.lineTo((rrect.right - rrect.trRadiusX), rrect.top);
    __cascade.addArc(trCorner, ((Dart_mathLibrary.pi * 3.0) / 2.0), sweepAngle);
    __cascade.lineTo(rrect.right, (rrect.bottom - (((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2.0)));
    return __cascade;
}))());
                            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawPath(endingPathNested, endingPaintNested);
                        }
                        else
                        {
                            if (this.isFirstButton)
                            {
                                var leadingPathA = new global::Doroti.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = leadingPathA;
    __cascade.moveTo(rrect.left, outer.top);
    __cascade.lineTo(rrect.left, (rrect.bottom - rrect.blRadiusY));
    __cascade.addArc(blCorner, Dart_mathLibrary.pi, -sweepAngle);
    __cascade.lineTo((rrect.right - rrect.brRadiusX), rrect.bottom);
    __cascade.addArc(brCorner, (Dart_mathLibrary.pi / 2.0), -sweepAngle);
    __cascade.lineTo(rrect.right, outer.top);
    return __cascade;
}))());
                                ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawPath(leadingPathA, leadingPaint);
                            }
                            else
                            {
                                var bottomPathLocal = new global::Doroti.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = bottomPathLocal;
    __cascade.moveTo(outer.left, (outer.bottom - (((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2.0)));
    __cascade.lineTo(outer.right, (outer.bottom - (((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2.0)));
    return __cascade;
}))());
                                ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawPath(bottomPathLocal, leadingPaint);
                                global::Doroti.Ui.Paint paintAlternate = ((global::Doroti.Ui.Paint)(object?)this.borderSide.toPaint());
                                var pathsLocal = new global::Doroti.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = pathsLocal;
    __cascade.moveTo(rrect.left, outer.top);
    __cascade.lineTo(rrect.left, (outer.bottom - ((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width));
    __cascade.moveTo(rrect.right, outer.top);
    __cascade.lineTo(rrect.right, (outer.bottom - ((global::Doroti.Framework.Painting.BorderSide)this.leadingBorderSide).width));
    return __cascade;
}))());
                                ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawPath(pathsLocal, paintAlternate);
                            }
                        }
                        break;
                    }
            }
        }
    }

}

internal class _InputPadding__toggle_buttons : global::Doroti.Framework.Widgets.SingleChildRenderObjectWidget
{
    public virtual Size minSize { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.Axis direction { get; private set; } = default!;

    internal _InputPadding__toggle_buttons(global::Doroti.Framework.Widgets.Widget? child = null, Size minSize = default!, global::Doroti.Framework.Painting.Axis direction = default!) : base(child: child)
    {
        this.minSize = minSize;
        this.direction = direction;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderInputPadding__toggle_buttons(this.minSize, this.direction));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderInputPadding__toggle_buttons)(object)renderObject;
        __renderObject.minSize = this.minSize;
        __renderObject.direction = this.direction;
    }

}

public class _RenderInputPadding__toggle_buttons : global::Doroti.Framework.Rendering.RenderShiftedBox
{
    internal virtual Size _minSize { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.Axis _direction { get; set; } = default!;

    internal _RenderInputPadding__toggle_buttons(Size _minSize, global::Doroti.Framework.Painting.Axis _direction, global::Doroti.Framework.Rendering.RenderBox? child = null) : base(child)
    {
        this._minSize = _minSize;
        this._direction = _direction;
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
    public virtual global::Doroti.Framework.Painting.Axis direction
    {
        get => this._direction;
        set
        {
            var __value = value;
            if ((object.Equals(this._direction, __value)))
            {
                return;
            }
            _direction = __value;
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
        global::Doroti.Ui.Size drySize = ((global::Doroti.Ui.Size)(object?)getDryLayout(constraints));
        global::Doroti.Ui.Size childSize = ((global::Doroti.Ui.Size)(object?)childLocal.getDryLayout(constraints));
        global::Doroti.Ui.Offset childOffset = ((global::Doroti.Ui.Offset)(object?)global::Doroti.Framework.Painting.Alignment.center.alongOffset((drySize - childSize)));
        return (DartRuntimePrimitives.RequireValue(result) + childOffset.dy);
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
        if (!this.size.contains(position))
        {
            return false;
        }
        global::Doroti.Ui.Offset center = ((global::Doroti.Ui.Offset)(object?)(this.direction switch { global::Doroti.Framework.Painting.Axis.horizontal => new global::Doroti.Ui.Offset(position.dx, (this.child!.size.height / 2L)), global::Doroti.Framework.Painting.Axis.vertical => new global::Doroti.Ui.Offset((this.child!.size.width / 2L), position.dy), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        return result.addWithRawTransform(transform: MatrixUtils.forceToPoint(center), position: center, hitTest: ((global::System.Func<global::Doroti.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, position) =>
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(position, center)));
            return this.child!.hitTest(result, position: center);
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
