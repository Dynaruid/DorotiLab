// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/toggle_buttons.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public class ToggleButtons : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    internal const double _defaultBorderWidth = 1.0;
    public virtual List<global::Doroti.Generated.Framework.Widgets.Widget> children { get; private set; } = default!;
    public virtual List<bool> isSelected { get; private set; } = default!;
    public virtual global::System.Action<long>? onPressed { get; private set; }
    public virtual global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual MaterialTapTargetSize? tapTargetSize { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? textStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual Color? selectedColor { get; private set; }
    public virtual Color? disabledColor { get; private set; }
    public virtual Color? fillColor { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual Color? highlightColor { get; private set; }
    public virtual Color? splashColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual List<global::Doroti.Generated.Framework.Widgets.FocusNode>? focusNodes { get; private set; }
    public virtual bool renderBorder { get; private set; } = default!;
    public virtual Color? borderColor { get; private set; }
    public virtual Color? selectedBorderColor { get; private set; }
    public virtual Color? disabledBorderColor { get; private set; }
    public virtual double? borderWidth { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.Axis direction { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.VerticalDirection verticalDirection { get; private set; } = default!;

    public ToggleButtons(global::Doroti.Generated.Framework.Foundation.Key? key = null, List<global::Doroti.Generated.Framework.Widgets.Widget> children = default!, List<bool> isSelected = default!, global::System.Action<long>? onPressed = null, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null, MaterialTapTargetSize? tapTargetSize = null, global::Doroti.Generated.Framework.Painting.TextStyle? textStyle = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, Color? color = null, Color? selectedColor = null, Color? disabledColor = null, Color? fillColor = null, Color? focusColor = null, Color? highlightColor = null, Color? hoverColor = null, Color? splashColor = null, List<global::Doroti.Generated.Framework.Widgets.FocusNode>? focusNodes = null, bool renderBorder = true, Color? borderColor = null, Color? selectedBorderColor = null, Color? disabledBorderColor = null, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null, double? borderWidth = null, global::Doroti.Generated.Framework.Painting.Axis direction = global::Doroti.Generated.Framework.Painting.Axis.horizontal, global::Doroti.Generated.Framework.Painting.VerticalDirection verticalDirection = global::Doroti.Generated.Framework.Painting.VerticalDirection.down) : base(key: key)
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
            case global::Doroti.Generated.Framework.Painting.Axis.horizontal:
                {
                    return (textDirection switch { TextDirection.rtl => (index == (length - 1L)), TextDirection.ltr => (index == 0L), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                }
            case global::Doroti.Generated.Framework.Painting.Axis.vertical:
                {
                    return (this.verticalDirection switch { global::Doroti.Generated.Framework.Painting.VerticalDirection.up => (index == (length - 1L)), global::Doroti.Generated.Framework.Painting.VerticalDirection.down => (index == 0L), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
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
            case global::Doroti.Generated.Framework.Painting.Axis.horizontal:
                {
                    return (textDirection switch { TextDirection.rtl => (index == 0L), TextDirection.ltr => (index == (length - 1L)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                }
            case global::Doroti.Generated.Framework.Painting.Axis.vertical:
                {
                    return (this.verticalDirection switch { global::Doroti.Generated.Framework.Painting.VerticalDirection.up => (index == 0L), global::Doroti.Generated.Framework.Painting.VerticalDirection.down => (index == (length - 1L)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Painting.BorderRadius _getEdgeBorderRadius(long index, long length, TextDirection textDirection, ToggleButtonsThemeData toggleButtonsTheme)
    {
        global::Doroti.Generated.Framework.Painting.BorderRadius resultingBorderRadius__18021 = ((this.borderRadius ?? toggleButtonsTheme.borderRadius) ?? global::Doroti.Generated.Framework.Painting.BorderRadius.zero);
        if ((length == 1L))
        {
            return resultingBorderRadius__18021;
        }
        else
        {
            if ((object.Equals(this.direction, global::Doroti.Generated.Framework.Painting.Axis.horizontal)))
            {
                if (_isFirstButton(index, length, textDirection))
                {
                    return new global::Doroti.Generated.Framework.Painting.BorderRadius(topLeft: ((global::Doroti.Generated.Framework.Painting.BorderRadius)resultingBorderRadius__18021).topLeft, bottomLeft: ((global::Doroti.Generated.Framework.Painting.BorderRadius)resultingBorderRadius__18021).bottomLeft);
                }
                else
                {
                    if (_isLastButton(index, length, textDirection))
                    {
                        return new global::Doroti.Generated.Framework.Painting.BorderRadius(topRight: ((global::Doroti.Generated.Framework.Painting.BorderRadius)resultingBorderRadius__18021).topRight, bottomRight: ((global::Doroti.Generated.Framework.Painting.BorderRadius)resultingBorderRadius__18021).bottomRight);
                    }
                }
            }
            else
            {
                if (_isFirstButton(index, length, textDirection))
                {
                    return new global::Doroti.Generated.Framework.Painting.BorderRadius(topLeft: ((global::Doroti.Generated.Framework.Painting.BorderRadius)resultingBorderRadius__18021).topLeft, topRight: ((global::Doroti.Generated.Framework.Painting.BorderRadius)resultingBorderRadius__18021).topRight);
                }
                else
                {
                    if (_isLastButton(index, length, textDirection))
                    {
                        return new global::Doroti.Generated.Framework.Painting.BorderRadius(bottomLeft: ((global::Doroti.Generated.Framework.Painting.BorderRadius)resultingBorderRadius__18021).bottomLeft, bottomRight: ((global::Doroti.Generated.Framework.Painting.BorderRadius)resultingBorderRadius__18021).bottomRight);
                    }
                }
            }
        }
        return global::Doroti.Generated.Framework.Painting.BorderRadius.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Painting.BorderRadius _getClipBorderRadius(long index, long length, TextDirection textDirection, ToggleButtonsThemeData toggleButtonsTheme)
    {
        global::Doroti.Generated.Framework.Painting.BorderRadius resultingBorderRadius__19334 = ((this.borderRadius ?? toggleButtonsTheme.borderRadius) ?? global::Doroti.Generated.Framework.Painting.BorderRadius.zero);
        double resultingBorderWidth__19453 = ((this.borderWidth ?? toggleButtonsTheme.borderWidth) ?? _defaultBorderWidth);
        if ((length == 1L))
        {
            return new global::Doroti.Generated.Framework.Painting.BorderRadius(topLeft: (((global::Doroti.Generated.Framework.Painting.BorderRadius)resultingBorderRadius__19334).topLeft - global::Doroti.Flutter.Ui.Radius.circular((resultingBorderWidth__19453 / 2.0))), bottomLeft: (((global::Doroti.Generated.Framework.Painting.BorderRadius)resultingBorderRadius__19334).bottomLeft - global::Doroti.Flutter.Ui.Radius.circular((resultingBorderWidth__19453 / 2.0))), topRight: (((global::Doroti.Generated.Framework.Painting.BorderRadius)resultingBorderRadius__19334).topRight - global::Doroti.Flutter.Ui.Radius.circular((resultingBorderWidth__19453 / 2.0))), bottomRight: (((global::Doroti.Generated.Framework.Painting.BorderRadius)resultingBorderRadius__19334).bottomRight - global::Doroti.Flutter.Ui.Radius.circular((resultingBorderWidth__19453 / 2.0))));
        }
        else
        {
            if ((object.Equals(this.direction, global::Doroti.Generated.Framework.Painting.Axis.horizontal)))
            {
                if (_isFirstButton(index, length, textDirection))
                {
                    return new global::Doroti.Generated.Framework.Painting.BorderRadius(topLeft: (((global::Doroti.Generated.Framework.Painting.BorderRadius)resultingBorderRadius__19334).topLeft - global::Doroti.Flutter.Ui.Radius.circular((resultingBorderWidth__19453 / 2.0))), bottomLeft: (((global::Doroti.Generated.Framework.Painting.BorderRadius)resultingBorderRadius__19334).bottomLeft - global::Doroti.Flutter.Ui.Radius.circular((resultingBorderWidth__19453 / 2.0))));
                }
                else
                {
                    if (_isLastButton(index, length, textDirection))
                    {
                        return new global::Doroti.Generated.Framework.Painting.BorderRadius(topRight: (((global::Doroti.Generated.Framework.Painting.BorderRadius)resultingBorderRadius__19334).topRight - global::Doroti.Flutter.Ui.Radius.circular((resultingBorderWidth__19453 / 2.0))), bottomRight: (((global::Doroti.Generated.Framework.Painting.BorderRadius)resultingBorderRadius__19334).bottomRight - global::Doroti.Flutter.Ui.Radius.circular((resultingBorderWidth__19453 / 2.0))));
                    }
                }
            }
            else
            {
                if (_isFirstButton(index, length, textDirection))
                {
                    return new global::Doroti.Generated.Framework.Painting.BorderRadius(topLeft: (((global::Doroti.Generated.Framework.Painting.BorderRadius)resultingBorderRadius__19334).topLeft - global::Doroti.Flutter.Ui.Radius.circular((resultingBorderWidth__19453 / 2.0))), topRight: (((global::Doroti.Generated.Framework.Painting.BorderRadius)resultingBorderRadius__19334).topRight - global::Doroti.Flutter.Ui.Radius.circular((resultingBorderWidth__19453 / 2.0))));
                }
                else
                {
                    if (_isLastButton(index, length, textDirection))
                    {
                        return new global::Doroti.Generated.Framework.Painting.BorderRadius(bottomLeft: (((global::Doroti.Generated.Framework.Painting.BorderRadius)resultingBorderRadius__19334).bottomLeft - global::Doroti.Flutter.Ui.Radius.circular((resultingBorderWidth__19453 / 2.0))), bottomRight: (((global::Doroti.Generated.Framework.Painting.BorderRadius)resultingBorderRadius__19334).bottomRight - global::Doroti.Flutter.Ui.Radius.circular((resultingBorderWidth__19453 / 2.0))));
                    }
                }
            }
        }
        return global::Doroti.Generated.Framework.Painting.BorderRadius.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Painting.BorderSide _getLeadingBorderSide(long index, ThemeData theme, ToggleButtonsThemeData toggleButtonsTheme)
    {
        if (!this.renderBorder)
        {
            return global::Doroti.Generated.Framework.Painting.BorderSide.none;
        }
        double resultingBorderWidth__21624 = ((this.borderWidth ?? toggleButtonsTheme.borderWidth) ?? _defaultBorderWidth);
        if (((this.onPressed is not null) && ((this.isSelected[(int)(index)] || (((index != 0L) && this.isSelected[(int)((index - 1L))]))))))
        {
            return new global::Doroti.Generated.Framework.Painting.BorderSide(color: ((this.selectedBorderColor ?? toggleButtonsTheme.selectedBorderColor) ?? theme.colorScheme.onSurface.withOpacity(0.12)), width: resultingBorderWidth__21624);
        }
        else
        {
            if (((this.onPressed is not null) && !this.isSelected[(int)(index)]))
            {
                return new global::Doroti.Generated.Framework.Painting.BorderSide(color: ((this.borderColor ?? toggleButtonsTheme.borderColor) ?? theme.colorScheme.onSurface.withOpacity(0.12)), width: resultingBorderWidth__21624);
            }
            else
            {
                return new global::Doroti.Generated.Framework.Painting.BorderSide(color: ((this.disabledBorderColor ?? toggleButtonsTheme.disabledBorderColor) ?? theme.colorScheme.onSurface.withOpacity(0.12)), width: resultingBorderWidth__21624);
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Painting.BorderSide _getBorderSide(long index, ThemeData theme, ToggleButtonsThemeData toggleButtonsTheme)
    {
        if (!this.renderBorder)
        {
            return global::Doroti.Generated.Framework.Painting.BorderSide.none;
        }
        double resultingBorderWidth__22766 = ((this.borderWidth ?? toggleButtonsTheme.borderWidth) ?? _defaultBorderWidth);
        if (((this.onPressed is not null) && this.isSelected[(int)(index)]))
        {
            return new global::Doroti.Generated.Framework.Painting.BorderSide(color: ((this.selectedBorderColor ?? toggleButtonsTheme.selectedBorderColor) ?? theme.colorScheme.onSurface.withOpacity(0.12)), width: resultingBorderWidth__22766);
        }
        else
        {
            if (((this.onPressed is not null) && !this.isSelected[(int)(index)]))
            {
                return new global::Doroti.Generated.Framework.Painting.BorderSide(color: ((this.borderColor ?? toggleButtonsTheme.borderColor) ?? theme.colorScheme.onSurface.withOpacity(0.12)), width: resultingBorderWidth__22766);
            }
            else
            {
                return new global::Doroti.Generated.Framework.Painting.BorderSide(color: ((this.disabledBorderColor ?? toggleButtonsTheme.disabledBorderColor) ?? theme.colorScheme.onSurface.withOpacity(0.12)), width: resultingBorderWidth__22766);
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Painting.BorderSide _getTrailingBorderSide(long index, ThemeData theme, ToggleButtonsThemeData toggleButtonsTheme)
    {
        if (!this.renderBorder)
        {
            return global::Doroti.Generated.Framework.Painting.BorderSide.none;
        }
        if ((index != (checked((long)(this.children.Count)) - 1L)))
        {
            return global::Doroti.Generated.Framework.Painting.BorderSide.none;
        }
        double resultingBorderWidth__23967 = ((this.borderWidth ?? toggleButtonsTheme.borderWidth) ?? _defaultBorderWidth);
        if (((this.onPressed is not null) && (this.isSelected[(int)(index)])))
        {
            return new global::Doroti.Generated.Framework.Painting.BorderSide(color: ((this.selectedBorderColor ?? toggleButtonsTheme.selectedBorderColor) ?? theme.colorScheme.onSurface.withOpacity(0.12)), width: resultingBorderWidth__23967);
        }
        else
        {
            if (((this.onPressed is not null) && !this.isSelected[(int)(index)]))
            {
                return new global::Doroti.Generated.Framework.Painting.BorderSide(color: ((this.borderColor ?? toggleButtonsTheme.borderColor) ?? theme.colorScheme.onSurface.withOpacity(0.12)), width: resultingBorderWidth__23967);
            }
            else
            {
                return new global::Doroti.Generated.Framework.Painting.BorderSide(color: ((this.disabledBorderColor ?? toggleButtonsTheme.disabledBorderColor) ?? theme.colorScheme.onSurface.withOpacity(0.12)), width: resultingBorderWidth__23967);
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this.focusNodes is not null))
                {
                    return (checked((long)(this.focusNodes!.Count)) == checked((long)(this.children.Count)));
                }
                return true;
            }, () => (object?)"focusNodes.length must match children.length.\n" + $"There are {checked((long)(this.focusNodes!.Count))} focus nodes, while " + $"there are {checked((long)(this.children.Count))} children.");
        ThemeData theme__25285 = Theme.of(context);
        ToggleButtonsThemeData toggleButtonsTheme__25345 = ToggleButtonsTheme.of(context);
        global::Doroti.Flutter.Ui.TextDirection textDirection__25422 = Directionality.of(context);
        var buttons__25477 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)checked((long)(this.children.Count)))), ((index) => {
global::Doroti.Generated.Framework.Painting.BorderRadius edgeBorderRadius__25565 = ((global::Doroti.Generated.Framework.Painting.BorderRadius)(object?)_getEdgeBorderRadius(index, checked((long)(this.children.Count)), textDirection__25422, toggleButtonsTheme__25345));
global::Doroti.Generated.Framework.Painting.BorderRadius clipBorderRadius__25731 = ((global::Doroti.Generated.Framework.Painting.BorderRadius)(object?)_getClipBorderRadius(index, checked((long)(this.children.Count)), textDirection__25422, toggleButtonsTheme__25345));
global::Doroti.Generated.Framework.Painting.BorderSide leadingBorderSide__25896 = ((global::Doroti.Generated.Framework.Painting.BorderSide)(object?)_getLeadingBorderSide(index, theme__25285, toggleButtonsTheme__25345));
global::Doroti.Generated.Framework.Painting.BorderSide borderSide__25996 = ((global::Doroti.Generated.Framework.Painting.BorderSide)(object?)_getBorderSide(index, theme__25285, toggleButtonsTheme__25345));
global::Doroti.Generated.Framework.Painting.BorderSide trailingBorderSide__26082 = ((global::Doroti.Generated.Framework.Painting.BorderSide)(object?)_getTrailingBorderSide(index, theme__25285, toggleButtonsTheme__25345));
var states__26207 = ((Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>>)(() => { var __collection26216 = new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>(); if ((this.isSelected[(int)(index)] && (this.onPressed is not null))) { __collection26216.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.selected); } if ((this.onPressed is null)) { __collection26216.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled); } return __collection26216; }))();
global::Doroti.Flutter.Ui.Color effectiveFillColor__26385 = ((global::Doroti.Flutter.Ui.Color)(object?)((new _ResolveFillColor__toggle_buttons((this.fillColor ?? toggleButtonsTheme__25345.fillColor)).resolve(states__26207) ?? (Color)new _DefaultFillColor__toggle_buttons(theme__25285.colorScheme).resolve(states__26207))));
global::Doroti.Flutter.Ui.Color currentColor__26578 = default!;
if (((this.onPressed is not null) && this.isSelected[(int)(index)]))
{
    currentColor__26578 = ((this.selectedColor ?? toggleButtonsTheme__25345.selectedColor) ?? theme__25285.colorScheme.primary);
}
else
{
    if (((this.onPressed is not null) && !this.isSelected[(int)(index)]))
    {
        currentColor__26578 = ((this.color ?? toggleButtonsTheme__25345.color) ?? theme__25285.colorScheme.onSurface.withOpacity(0.87));
    }
    else
    {
        currentColor__26578 = ((this.disabledColor ?? toggleButtonsTheme__25345.disabledColor) ?? theme__25285.colorScheme.onSurface.withOpacity(0.38));
    }
}
global::Doroti.Generated.Framework.Painting.TextStyle currentTextStyle__27142 = ((this.textStyle ?? toggleButtonsTheme__25345.textStyle) ?? theme__25285.textTheme.bodyMedium!);
global::Doroti.Generated.Framework.Rendering.BoxConstraints? currentConstraints__27273 = (this.constraints ?? toggleButtonsTheme__25345.constraints);
global::Doroti.Flutter.Ui.Size minimumSize__27358 = ((global::Doroti.Flutter.Ui.Size)(object?)(currentConstraints__27273?.smallest ?? new global::Doroti.Flutter.Ui.Size(global::Doroti.Generated.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension)));
global::Doroti.Flutter.Ui.Size? maximumSize__27477 = ((global::Doroti.Flutter.Ui.Size?)(object?)currentConstraints__27273?.biggest);
global::Doroti.Flutter.Ui.Size minPaddingSize__27537 = default!;
switch ((this.tapTargetSize ?? theme__25285.materialTapTargetSize))
{
    case var __constant27628 when (object.Equals(__constant27628, MaterialTapTargetSize.padded)):
        {
            minPaddingSize__27537 = (this.direction switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => new global::Doroti.Flutter.Ui.Size(0.0, global::Doroti.Generated.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension), global::Doroti.Generated.Framework.Painting.Axis.vertical => new global::Doroti.Flutter.Ui.Size(global::Doroti.Generated.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension, 0.0), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            DartRuntimePrimitives.Assert(() => (minPaddingSize__27537.width >= 0.0));
            DartRuntimePrimitives.Assert(() => (minPaddingSize__27537.height >= 0.0));
            break;
        }
    case var __constant27973 when (object.Equals(__constant27973, MaterialTapTargetSize.shrinkWrap)):
        {
            minPaddingSize__27537 = Size.zero;
            break;
        }
}
global::Doroti.Generated.Framework.Widgets.Widget button__28067 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _SelectToggleButton__toggle_buttons(leadingBorderSide: leadingBorderSide__25896, borderSide: borderSide__25996, trailingBorderSide: trailingBorderSide__26082, borderRadius: edgeBorderRadius__25565, isFirstButton: (index == 0L), isLastButton: (index == (checked((long)(this.children.Count)) - 1L)), direction: this.direction, verticalDirection: this.verticalDirection, child: new global::Doroti.Generated.Framework.Widgets.ClipRRect(borderRadius: clipBorderRadius__25731, child: new TextButton(focusNode: ((this.focusNodes is not null) ? this.focusNodes![(int)(index)] : null), style: new ButtonStyle(backgroundColor: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Flutter.Ui.Color?>(effectiveFillColor__26385), foregroundColor: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Flutter.Ui.Color?>(currentColor__26578), iconSize: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<double?>(24.0), iconColor: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Flutter.Ui.Color?>(currentColor__26578), overlayColor: new _ToggleButtonDefaultOverlay__toggle_buttons(selected: ((this.onPressed is not null) && this.isSelected[(int)(index)]), unselected: ((this.onPressed is not null) && !this.isSelected[(int)(index)]), colorScheme: theme__25285.colorScheme, disabledColor: (this.disabledColor ?? toggleButtonsTheme__25345.disabledColor), focusColor: (this.focusColor ?? toggleButtonsTheme__25345.focusColor), highlightColor: (this.highlightColor ?? toggleButtonsTheme__25345.highlightColor), hoverColor: (this.hoverColor ?? toggleButtonsTheme__25345.hoverColor), splashColor: (this.splashColor ?? toggleButtonsTheme__25345.splashColor)), elevation: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<double?>(0), textStyle: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.TextStyle?>(currentTextStyle__27142.copyWith(color: currentColor__26578)), padding: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Generated.Framework.Painting.EdgeInsets.zero), minimumSize: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Flutter.Ui.Size?>(minimumSize__27358), maximumSize: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Flutter.Ui.Size?>(maximumSize__27477), shape: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder()), mouseCursor: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Services.MouseCursor?>(this.mouseCursor), visualDensity: VisualDensity.standard, tapTargetSize: MaterialTapTargetSize.shrinkWrap, animationDuration: ConstantsLibrary.kThemeChangeDuration, enableFeedback: true, alignment: global::Doroti.Generated.Framework.Painting.Alignment.center, splashFactory: InkRipple.splashFactory), onPressed: ((global::System.Action)((this.onPressed is not null) ? (() => {
this.onPressed!(index);
}) : null)), child: this.children[(int)(index)]))));
if ((currentConstraints__27273 is not null))
{
    button__28067 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Center(child: button__28067));
}
return new global::Doroti.Generated.Framework.Widgets.MergeSemantics(child: new global::Doroti.Generated.Framework.Widgets.Semantics(container: true, @checked: this.isSelected[(int)(index)], enabled: (this.onPressed is not null), child: new _InputPadding__toggle_buttons(minSize: minPaddingSize__27537, direction: this.direction, child: button__28067)));
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        if ((object.Equals(this.direction, global::Doroti.Generated.Framework.Painting.Axis.vertical)))
        {
            return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.IntrinsicWidth(child: new global::Doroti.Generated.Framework.Widgets.Column(mainAxisSize: global::Doroti.Generated.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.stretch, verticalDirection: this.verticalDirection, children: buttons__25477)));
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.IntrinsicHeight(child: new global::Doroti.Generated.Framework.Widgets.Row(mainAxisSize: global::Doroti.Generated.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.stretch, children: buttons__25477)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("disabled", value: (this.onPressed is null), ifTrue: "Buttons are disabled", ifFalse: "Buttons are enabled"));
        this.textStyle?.debugFillProperties(properties, prefix: "textStyle.");
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("color", this.color, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("selectedColor", this.selectedColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("disabledColor", this.disabledColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("fillColor", this.fillColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("focusColor", this.focusColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("highlightColor", this.highlightColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("hoverColor", this.hoverColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("splashColor", this.splashColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("borderColor", this.borderColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("selectedBorderColor", this.selectedBorderColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("disabledBorderColor", this.disabledBorderColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.BorderRadius>("borderRadius", this.borderRadius, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("borderWidth", this.borderWidth, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.Axis>("direction", this.direction, defaultValue: global::Doroti.Generated.Framework.Painting.Axis.horizontal));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.VerticalDirection>("verticalDirection", this.verticalDirection, defaultValue: global::Doroti.Generated.Framework.Painting.VerticalDirection.down));
    }

}

internal class _ResolveFillColor__toggle_buttons : global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>, global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual Color? primary { get; private set; }

    internal _ResolveFillColor__toggle_buttons(Color? primary)
    {
        this.primary = primary;
    }

    public virtual Color? resolve(HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> states)
    {
        if ((this.primary is global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color>))
        {
            global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color> primary__as33643 = (global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color>)primary;
            return ((Color?)(object?)WidgetStateProperty.resolveAs<global::Doroti.Flutter.Ui.Color?>(this.primary, states));
        }
        return (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected) ? this.primary : null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string toStringShort() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return ((fullString__105654 ?? (string)toStringShort()));
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

internal class _DefaultFillColor__toggle_buttons : global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color>, global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual ColorScheme colorScheme { get; private set; } = default!;

    internal _DefaultFillColor__toggle_buttons(ColorScheme colorScheme)
    {
        this.colorScheme = colorScheme;
    }

    public virtual Color resolve(HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> states)
    {
        if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
        {
            return this.colorScheme.primary.withOpacity(0.12);
        }
        return this.colorScheme.surface.withOpacity(0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string toStringShort() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return ((fullString__105654 ?? (string)toStringShort()));
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

internal class _ToggleButtonDefaultOverlay__toggle_buttons : global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>
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

    public virtual Color? resolve(HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> states)
    {
        if (this.selected)
        {
            if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
            {
                return (this.splashColor ?? this.colorScheme?.primary.withOpacity(0.16));
            }
            if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
            {
                return (this.hoverColor ?? this.colorScheme?.primary.withOpacity(0.04));
            }
            if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
            {
                return (this.focusColor ?? this.colorScheme?.primary.withOpacity(0.12));
            }
        }
        else
        {
            if (this.unselected)
            {
                if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
                {
                    return ((this.splashColor ?? this.highlightColor) ?? this.colorScheme?.onSurface.withOpacity(0.16));
                }
                if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
                {
                    return (this.hoverColor ?? this.colorScheme?.onSurface.withOpacity(0.04));
                }
                if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
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

internal class _SelectToggleButton__toggle_buttons : global::Doroti.Generated.Framework.Widgets.SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Painting.BorderSide leadingBorderSide { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BorderSide borderSide { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BorderSide trailingBorderSide { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadius borderRadius { get; private set; } = default!;
    public virtual bool isFirstButton { get; private set; } = default!;
    public virtual bool isLastButton { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.Axis direction { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.VerticalDirection verticalDirection { get; private set; } = default!;

    internal _SelectToggleButton__toggle_buttons(global::Doroti.Generated.Framework.Widgets.Widget child, global::Doroti.Generated.Framework.Painting.BorderSide leadingBorderSide, global::Doroti.Generated.Framework.Painting.BorderSide borderSide, global::Doroti.Generated.Framework.Painting.BorderSide trailingBorderSide, global::Doroti.Generated.Framework.Painting.BorderRadius borderRadius, bool isFirstButton, bool isLastButton, global::Doroti.Generated.Framework.Painting.Axis direction, global::Doroti.Generated.Framework.Painting.VerticalDirection verticalDirection) : base(child: child)
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

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(new _SelectToggleButtonRenderObject__toggle_buttons(this.leadingBorderSide, this.borderSide, this.trailingBorderSide, this.borderRadius, this.isFirstButton, this.isLastButton, this.direction, this.verticalDirection, Directionality.of(context)));
    public override void updateRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_SelectToggleButtonRenderObject__toggle_buttons)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_SelectToggleButtonRenderObject__toggle_buttons>)(() =>
{            var __cascade = __renderObject;
            __cascade.leadingBorderSide = this.leadingBorderSide;
            __cascade.borderSide = this.borderSide;
            __cascade.trailingBorderSide = this.trailingBorderSide;
            __cascade.borderRadius = this.borderRadius;
            __cascade.isFirstButton = this.isFirstButton;
            __cascade.isLastButton = this.isLastButton;
            __cascade.direction = this.direction;
            __cascade.verticalDirection = this.verticalDirection;
            __cascade.textDirection = Directionality.of(context);
            return __cascade;        }))());
    }

}

public class _SelectToggleButtonRenderObject__toggle_buttons : global::Doroti.Generated.Framework.Rendering.RenderShiftedBox
{
    internal virtual global::Doroti.Generated.Framework.Painting.Axis _direction { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Painting.VerticalDirection _verticalDirection { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Painting.BorderSide _leadingBorderSide { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Painting.BorderSide _borderSide { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Painting.BorderSide _trailingBorderSide { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Painting.BorderRadius _borderRadius { get; set; } = default!;
    internal virtual bool _isFirstButton { get; set; } = default!;
    internal virtual bool _isLastButton { get; set; } = default!;
    internal virtual TextDirection _textDirection { get; set; } = default!;

    internal _SelectToggleButtonRenderObject__toggle_buttons(global::Doroti.Generated.Framework.Painting.BorderSide _leadingBorderSide, global::Doroti.Generated.Framework.Painting.BorderSide _borderSide, global::Doroti.Generated.Framework.Painting.BorderSide _trailingBorderSide, global::Doroti.Generated.Framework.Painting.BorderRadius _borderRadius, bool _isFirstButton, bool _isLastButton, global::Doroti.Generated.Framework.Painting.Axis _direction, global::Doroti.Generated.Framework.Painting.VerticalDirection _verticalDirection, TextDirection _textDirection, global::Doroti.Generated.Framework.Rendering.RenderBox? child = null) : base(child)
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

    public virtual global::Doroti.Generated.Framework.Painting.Axis direction
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
    public virtual global::Doroti.Generated.Framework.Painting.VerticalDirection verticalDirection
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
    public virtual global::Doroti.Generated.Framework.Painting.BorderSide leadingBorderSide
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
    public virtual global::Doroti.Generated.Framework.Painting.BorderSide borderSide
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
    public virtual global::Doroti.Generated.Framework.Painting.BorderSide trailingBorderSide
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
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadius borderRadius
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
    public virtual global::Doroti.Flutter.Ui.TextDirection textDirection
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
    internal static double _maxHeight(global::Doroti.Generated.Framework.Rendering.RenderBox? box, double width)
    {
        return (box?.getMaxIntrinsicHeight(width) ?? 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _minHeight(global::Doroti.Generated.Framework.Rendering.RenderBox? box, double width)
    {
        return (box?.getMinIntrinsicHeight(width) ?? 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _minWidth(global::Doroti.Generated.Framework.Rendering.RenderBox? box, double height)
    {
        return (box?.getMinIntrinsicWidth(height) ?? 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _maxWidth(global::Doroti.Generated.Framework.Rendering.RenderBox? box, double height)
    {
        return (box?.getMaxIntrinsicWidth(height) ?? 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        var childOffset__42261 = new global::Doroti.Generated.Framework.Rendering.BaselineOffset(this.child?.getDistanceToActualBaseline(baseline));
        return (this.direction switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => (childOffset__42261.op_Add(((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width)), global::Doroti.Generated.Framework.Painting.Axis.vertical => (childOffset__42261.op_Add((this.verticalDirection switch { global::Doroti.Generated.Framework.Painting.VerticalDirection.down => ((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width, global::Doroti.Generated.Framework.Painting.VerticalDirection.up => ((global::Doroti.Generated.Framework.Painting.BorderSide)this.trailingBorderSide).width, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }).offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return ((object.Equals(this.direction, global::Doroti.Generated.Framework.Painting.Axis.horizontal)) ? ((((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width * 2.0) + _SelectToggleButtonRenderObject__toggle_buttons._maxHeight(this.child, width)) : ((((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width + _SelectToggleButtonRenderObject__toggle_buttons._maxHeight(this.child, width)) + ((global::Doroti.Generated.Framework.Painting.BorderSide)this.trailingBorderSide).width));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return ((object.Equals(this.direction, global::Doroti.Generated.Framework.Painting.Axis.horizontal)) ? ((((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width * 2.0) + _SelectToggleButtonRenderObject__toggle_buttons._minHeight(this.child, width)) : ((((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width + _SelectToggleButtonRenderObject__toggle_buttons._maxHeight(this.child, width)) + ((global::Doroti.Generated.Framework.Painting.BorderSide)this.trailingBorderSide).width));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return ((object.Equals(this.direction, global::Doroti.Generated.Framework.Painting.Axis.horizontal)) ? ((((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width + _SelectToggleButtonRenderObject__toggle_buttons._maxWidth(this.child, height)) + ((global::Doroti.Generated.Framework.Painting.BorderSide)this.trailingBorderSide).width) : ((((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width * 2.0) + _SelectToggleButtonRenderObject__toggle_buttons._maxWidth(this.child, height)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        return ((object.Equals(this.direction, global::Doroti.Generated.Framework.Painting.Axis.horizontal)) ? ((((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width + _SelectToggleButtonRenderObject__toggle_buttons._minWidth(this.child, height)) + ((global::Doroti.Generated.Framework.Painting.BorderSide)this.trailingBorderSide).width) : ((((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width * 2.0) + _SelectToggleButtonRenderObject__toggle_buttons._minWidth(this.child, height)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        return _computeSize(constraints: constraints, layoutChild: (global::System.Func<global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Rendering.BoxConstraints, Size>)global::Doroti.Generated.Framework.Rendering.ChildLayoutHelper.dryLayoutChild);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional _childPadding
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (this.child is not null));
            return (this.direction switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: ((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width, end: ((global::Doroti.Generated.Framework.Painting.BorderSide)this.trailingBorderSide).width, top: ((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width, bottom: ((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width), global::Doroti.Generated.Framework.Painting.Axis.vertical => global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: ((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width, end: ((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width, top: ((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width, bottom: ((global::Doroti.Generated.Framework.Painting.BorderSide)this.trailingBorderSide).width), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return default!;
        }
    }
    public override double? computeDryBaseline(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        double? childBaseline__44718 = this.child?.getDryBaseline(constraints.deflate(this._childPadding), baseline);
        if ((childBaseline__44718 is null))
        {
            return null;
        }
        return (DartRuntimePrimitives.RequireValue(childBaseline__44718) + (this.direction switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => ((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width, global::Doroti.Generated.Framework.Painting.Axis.vertical => (this.verticalDirection switch { global::Doroti.Generated.Framework.Painting.VerticalDirection.down => ((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width, global::Doroti.Generated.Framework.Painting.VerticalDirection.up => ((global::Doroti.Generated.Framework.Painting.BorderSide)this.trailingBorderSide).width, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        size = _computeSize(constraints: this.constraints, layoutChild: (global::System.Func<global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Rendering.BoxConstraints, Size>)global::Doroti.Generated.Framework.Rendering.ChildLayoutHelper.layoutChild);
        if ((this.child is null))
        {
            return;
        }
        var childParentData__45380 = ((global::Doroti.Generated.Framework.Rendering.BoxParentData?)(object?)this.child!.parentData!)!;
        if ((object.Equals(this.direction, global::Doroti.Generated.Framework.Painting.Axis.horizontal)))
        {
            childParentData__45380.offset = (this.textDirection switch { TextDirection.ltr => new global::Doroti.Flutter.Ui.Offset(((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width, ((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width), TextDirection.rtl => new global::Doroti.Flutter.Ui.Offset(((global::Doroti.Generated.Framework.Painting.BorderSide)this.trailingBorderSide).width, ((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        }
        else
        {
            childParentData__45380.offset = (this.verticalDirection switch { global::Doroti.Generated.Framework.Painting.VerticalDirection.down => new global::Doroti.Flutter.Ui.Offset(((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width, ((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width), global::Doroti.Generated.Framework.Painting.VerticalDirection.up => new global::Doroti.Flutter.Ui.Offset(((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width, ((global::Doroti.Generated.Framework.Painting.BorderSide)this.trailingBorderSide).width), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        }
    }

    internal virtual global::Doroti.Flutter.Ui.Size _computeSize(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, global::System.Func<global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Rendering.BoxConstraints, Size> layoutChild)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__46081 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((child__46081 is null))
        {
            var horizontalSize__46138 = new global::Doroti.Flutter.Ui.Size((((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width + ((global::Doroti.Generated.Framework.Painting.BorderSide)this.trailingBorderSide).width), (((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width * 2.0));
            return (this.direction switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => constraints.constrain(horizontalSize__46138), global::Doroti.Generated.Framework.Painting.Axis.vertical => constraints.constrain(horizontalSize__46138.flipped), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        }
        global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional childPadding__46482 = this._childPadding;
        global::Doroti.Generated.Framework.Rendering.BoxConstraints innerConstraints__46537 = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)(object?)constraints.deflate(childPadding__46482));
        return ((global::Doroti.Flutter.Ui.Size)(object?)constraints.constrain(childPadding__46482.inflateSize(layoutChild(child__46081, innerConstraints__46537))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        base.paint(context, offset);
        global::Doroti.Flutter.Ui.Offset bottomRight__46812 = ((global::Doroti.Flutter.Ui.Offset)(object?)this.size.bottomRight(offset));
        var outer__46862 = global::Doroti.Flutter.Ui.Rect.fromLTRB(offset.dx, offset.dy, bottomRight__46812.dx, bottomRight__46812.dy);
        global::Doroti.Flutter.Ui.Rect center__46954 = ((global::Doroti.Flutter.Ui.Rect)(object?)outer__46862.deflate((((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width / 2.0)));
        double sweepAngle__47019 = (Dart_mathLibrary.pi / 2.0);
        global::Doroti.Flutter.Ui.RRect rrect__47063 = ((global::Doroti.Flutter.Ui.RRect)(object?)global::Doroti.Flutter.Ui.RRect.fromRectAndCorners(center__46954, topLeft: ((((((global::Doroti.Generated.Framework.Painting.BorderRadius)this.borderRadius).topLeft.x * ((global::Doroti.Generated.Framework.Painting.BorderRadius)this.borderRadius).topLeft.y) != 0.0)) ? ((global::Doroti.Generated.Framework.Painting.BorderRadius)this.borderRadius).topLeft : Radius.zero), topRight: ((((((global::Doroti.Generated.Framework.Painting.BorderRadius)this.borderRadius).topRight.x * ((global::Doroti.Generated.Framework.Painting.BorderRadius)this.borderRadius).topRight.y) != 0.0)) ? ((global::Doroti.Generated.Framework.Painting.BorderRadius)this.borderRadius).topRight : Radius.zero), bottomLeft: ((((((global::Doroti.Generated.Framework.Painting.BorderRadius)this.borderRadius).bottomLeft.x * ((global::Doroti.Generated.Framework.Painting.BorderRadius)this.borderRadius).bottomLeft.y) != 0.0)) ? ((global::Doroti.Generated.Framework.Painting.BorderRadius)this.borderRadius).bottomLeft : Radius.zero), bottomRight: ((((((global::Doroti.Generated.Framework.Painting.BorderRadius)this.borderRadius).bottomRight.x * ((global::Doroti.Generated.Framework.Painting.BorderRadius)this.borderRadius).bottomRight.y) != 0.0)) ? ((global::Doroti.Generated.Framework.Painting.BorderRadius)this.borderRadius).bottomRight : Radius.zero)).scaleRadii());
        var tlCorner__47694 = global::Doroti.Flutter.Ui.Rect.fromLTWH(rrect__47063.left, rrect__47063.top, (rrect__47063.tlRadiusX * 2.0), (rrect__47063.tlRadiusY * 2.0));
        var blCorner__47830 = global::Doroti.Flutter.Ui.Rect.fromLTWH(rrect__47063.left, (rrect__47063.bottom - ((rrect__47063.blRadiusY * 2.0))), (rrect__47063.blRadiusX * 2.0), (rrect__47063.blRadiusY * 2.0));
        var trCorner__47995 = global::Doroti.Flutter.Ui.Rect.fromLTWH((rrect__47063.right - ((rrect__47063.trRadiusX * 2L))), rrect__47063.top, (rrect__47063.trRadiusX * 2L), (rrect__47063.trRadiusY * 2L));
        var brCorner__48152 = global::Doroti.Flutter.Ui.Rect.fromLTWH((rrect__47063.right - ((rrect__47063.brRadiusX * 2L))), (rrect__47063.bottom - ((rrect__47063.brRadiusY * 2L))), (rrect__47063.brRadiusX * 2L), (rrect__47063.brRadiusY * 2L));
        global::Doroti.Flutter.Ui.Paint leadingPaint__48343 = ((global::Doroti.Flutter.Ui.Paint)(object?)this.leadingBorderSide.toPaint());
        if ((this.isFirstButton && this.isLastButton))
        {
            var leadingPath__48464 = new global::Doroti.Flutter.Ui.Path();
            double startX__48505 = (((rrect__47063.brRadiusX == 0.0)) ? outer__46862.right : (rrect__47063.right - rrect__47063.brRadiusX));
            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{            var __cascade = leadingPath__48464;
            __cascade.moveTo(startX__48505, rrect__47063.bottom);
            __cascade.lineTo((rrect__47063.left + rrect__47063.blRadiusX), rrect__47063.bottom);
            __cascade.addArc(blCorner__47830, (Dart_mathLibrary.pi / 2.0), sweepAngle__47019);
            __cascade.lineTo(rrect__47063.left, (rrect__47063.top + rrect__47063.tlRadiusY));
            __cascade.addArc(tlCorner__47694, Dart_mathLibrary.pi, sweepAngle__47019);
            __cascade.lineTo((rrect__47063.right - rrect__47063.trRadiusX), rrect__47063.top);
            __cascade.addArc(trCorner__47995, ((Dart_mathLibrary.pi * 3.0) / 2.0), sweepAngle__47019);
            __cascade.lineTo(rrect__47063.right, (rrect__47063.bottom - rrect__47063.brRadiusY));
            __cascade.addArc(brCorner__48152, 0, sweepAngle__47019);
            return __cascade;        }))());
            ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawPath(leadingPath__48464, leadingPaint__48343);
            return;
        }
        if ((object.Equals(this.direction, global::Doroti.Generated.Framework.Painting.Axis.horizontal)))
        {
            switch (this.textDirection)
            {
                case TextDirection.ltr:
                    {
                        if (this.isLastButton)
                        {
                            var leftPath__49318 = new global::Doroti.Flutter.Ui.Path();
                            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{            var __cascade = leftPath__49318;
            __cascade.moveTo(rrect__47063.left, (rrect__47063.bottom + (((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2L)));
            __cascade.lineTo(rrect__47063.left, (rrect__47063.top - (((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2L)));
            return __cascade;        }))());
                            ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawPath(leftPath__49318, leadingPaint__48343);
                            global::Doroti.Flutter.Ui.Paint endingPaint__49600 = ((global::Doroti.Flutter.Ui.Paint)(object?)this.trailingBorderSide.toPaint());
                            var endingPath__49662 = new global::Doroti.Flutter.Ui.Path();
                            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{            var __cascade = endingPath__49662;
            __cascade.moveTo((rrect__47063.left + (((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width / 2.0)), rrect__47063.top);
            __cascade.lineTo((rrect__47063.right - rrect__47063.trRadiusX), rrect__47063.top);
            __cascade.addArc(trCorner__47995, ((Dart_mathLibrary.pi * 3.0) / 2.0), sweepAngle__47019);
            __cascade.lineTo(rrect__47063.right, (rrect__47063.bottom - rrect__47063.brRadiusY));
            __cascade.addArc(brCorner__48152, 0, sweepAngle__47019);
            __cascade.lineTo((rrect__47063.left + (((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width / 2.0)), rrect__47063.bottom);
            return __cascade;        }))());
                            ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawPath(endingPath__49662, endingPaint__49600);
                        }
                        else
                        {
                            if (this.isFirstButton)
                            {
                                var leadingPath__50217 = new global::Doroti.Flutter.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{            var __cascade = leadingPath__50217;
            __cascade.moveTo(outer__46862.right, rrect__47063.bottom);
            __cascade.lineTo((rrect__47063.left + rrect__47063.blRadiusX), rrect__47063.bottom);
            __cascade.addArc(blCorner__47830, (Dart_mathLibrary.pi / 2.0), sweepAngle__47019);
            __cascade.lineTo(rrect__47063.left, (rrect__47063.top + rrect__47063.tlRadiusY));
            __cascade.addArc(tlCorner__47694, Dart_mathLibrary.pi, sweepAngle__47019);
            __cascade.lineTo(outer__46862.right, rrect__47063.top);
            return __cascade;        }))());
                                ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawPath(leadingPath__50217, leadingPaint__48343);
                            }
                            else
                            {
                                var leadingPath__50707 = new global::Doroti.Flutter.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{            var __cascade = leadingPath__50707;
            __cascade.moveTo(rrect__47063.left, (rrect__47063.bottom + (((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2L)));
            __cascade.lineTo(rrect__47063.left, (rrect__47063.top - (((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2L)));
            return __cascade;        }))());
                                ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawPath(leadingPath__50707, leadingPaint__48343);
                                global::Doroti.Flutter.Ui.Paint horizontalPaint__50998 = ((global::Doroti.Flutter.Ui.Paint)(object?)this.borderSide.toPaint());
                                var horizontalPaths__51056 = new global::Doroti.Flutter.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{            var __cascade = horizontalPaths__51056;
            __cascade.moveTo((rrect__47063.left + (((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width / 2.0)), rrect__47063.top);
            __cascade.lineTo((outer__46862.right - rrect__47063.trRadiusX), rrect__47063.top);
            __cascade.moveTo(((rrect__47063.left + (((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width / 2.0)) + rrect__47063.tlRadiusX), rrect__47063.bottom);
            __cascade.lineTo((outer__46862.right - rrect__47063.trRadiusX), rrect__47063.bottom);
            return __cascade;        }))());
                                ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawPath(horizontalPaths__51056, horizontalPaint__50998);
                            }
                        }
                        break;
                    }
                case TextDirection.rtl:
                    {
                        if (this.isLastButton)
                        {
                            var leadingPath__51570 = new global::Doroti.Flutter.Ui.Path();
                            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{            var __cascade = leadingPath__51570;
            __cascade.moveTo(rrect__47063.right, (rrect__47063.bottom + (((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2L)));
            __cascade.lineTo(rrect__47063.right, (rrect__47063.top - (((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2L)));
            return __cascade;        }))());
                            ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawPath(leadingPath__51570, leadingPaint__48343);
                            global::Doroti.Flutter.Ui.Paint endingPaint__51863 = ((global::Doroti.Flutter.Ui.Paint)(object?)this.trailingBorderSide.toPaint());
                            var endingPath__51925 = new global::Doroti.Flutter.Ui.Path();
                            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{            var __cascade = endingPath__51925;
            __cascade.moveTo((rrect__47063.right - (((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width / 2.0)), rrect__47063.top);
            __cascade.lineTo((rrect__47063.left + rrect__47063.tlRadiusX), rrect__47063.top);
            __cascade.addArc(tlCorner__47694, ((Dart_mathLibrary.pi * 3.0) / 2.0), -sweepAngle__47019);
            __cascade.lineTo(rrect__47063.left, (rrect__47063.bottom - rrect__47063.blRadiusY));
            __cascade.addArc(blCorner__47830, Dart_mathLibrary.pi, -sweepAngle__47019);
            __cascade.lineTo((rrect__47063.right - (((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width / 2.0)), rrect__47063.bottom);
            return __cascade;        }))());
                            ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawPath(endingPath__51925, endingPaint__51863);
                        }
                        else
                        {
                            if (this.isFirstButton)
                            {
                                var leadingPath__52488 = new global::Doroti.Flutter.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{            var __cascade = leadingPath__52488;
            __cascade.moveTo(outer__46862.left, rrect__47063.bottom);
            __cascade.lineTo((rrect__47063.right - rrect__47063.brRadiusX), rrect__47063.bottom);
            __cascade.addArc(brCorner__48152, (Dart_mathLibrary.pi / 2.0), -sweepAngle__47019);
            __cascade.lineTo(rrect__47063.right, (rrect__47063.top + rrect__47063.trRadiusY));
            __cascade.addArc(trCorner__47995, 0, -sweepAngle__47019);
            __cascade.lineTo(outer__46862.left, rrect__47063.top);
            return __cascade;        }))());
                                ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawPath(leadingPath__52488, leadingPaint__48343);
                            }
                            else
                            {
                                var leadingPath__52974 = new global::Doroti.Flutter.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{            var __cascade = leadingPath__52974;
            __cascade.moveTo(rrect__47063.right, (rrect__47063.bottom + (((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2L)));
            __cascade.lineTo(rrect__47063.right, (rrect__47063.top - (((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2L)));
            return __cascade;        }))());
                                ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawPath(leadingPath__52974, leadingPaint__48343);
                                global::Doroti.Flutter.Ui.Paint horizontalPaint__53267 = ((global::Doroti.Flutter.Ui.Paint)(object?)this.borderSide.toPaint());
                                var horizontalPaths__53325 = new global::Doroti.Flutter.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{            var __cascade = horizontalPaths__53325;
            __cascade.moveTo((rrect__47063.right - (((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width / 2.0)), rrect__47063.top);
            __cascade.lineTo((outer__46862.left - rrect__47063.tlRadiusX), rrect__47063.top);
            __cascade.moveTo(((rrect__47063.right - (((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width / 2.0)) + rrect__47063.trRadiusX), rrect__47063.bottom);
            __cascade.lineTo((outer__46862.left - rrect__47063.tlRadiusX), rrect__47063.bottom);
            return __cascade;        }))());
                                ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawPath(horizontalPaths__53325, horizontalPaint__53267);
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
                case global::Doroti.Generated.Framework.Painting.VerticalDirection.down:
                    {
                        if (this.isLastButton)
                        {
                            var topPath__53900 = new global::Doroti.Flutter.Ui.Path();
                            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{            var __cascade = topPath__53900;
            __cascade.moveTo(outer__46862.left, (outer__46862.top + (((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2L)));
            __cascade.lineTo(outer__46862.right, (outer__46862.top + (((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2L)));
            return __cascade;        }))());
                            ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawPath(topPath__53900, leadingPaint__48343);
                            global::Doroti.Flutter.Ui.Paint endingPaint__54177 = ((global::Doroti.Flutter.Ui.Paint)(object?)this.trailingBorderSide.toPaint());
                            var endingPath__54239 = new global::Doroti.Flutter.Ui.Path();
                            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{            var __cascade = endingPath__54239;
            __cascade.moveTo(rrect__47063.left, (rrect__47063.top + (((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2.0)));
            __cascade.lineTo(rrect__47063.left, (rrect__47063.bottom - rrect__47063.blRadiusY));
            __cascade.addArc(blCorner__47830, (Dart_mathLibrary.pi * 3.0), -sweepAngle__47019);
            __cascade.lineTo((rrect__47063.right - rrect__47063.blRadiusX), rrect__47063.bottom);
            __cascade.addArc(brCorner__48152, (Dart_mathLibrary.pi / 2.0), -sweepAngle__47019);
            __cascade.lineTo(rrect__47063.right, (rrect__47063.top + (((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2.0)));
            return __cascade;        }))());
                            ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawPath(endingPath__54239, endingPaint__54177);
                        }
                        else
                        {
                            if (this.isFirstButton)
                            {
                                var leadingPath__54816 = new global::Doroti.Flutter.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{            var __cascade = leadingPath__54816;
            __cascade.moveTo(rrect__47063.left, outer__46862.bottom);
            __cascade.lineTo(rrect__47063.left, (rrect__47063.top + rrect__47063.tlRadiusX));
            __cascade.addArc(tlCorner__47694, Dart_mathLibrary.pi, sweepAngle__47019);
            __cascade.lineTo((rrect__47063.right - rrect__47063.trRadiusX), rrect__47063.top);
            __cascade.addArc(trCorner__47995, ((Dart_mathLibrary.pi * 3.0) / 2.0), sweepAngle__47019);
            __cascade.lineTo(rrect__47063.right, outer__46862.bottom);
            return __cascade;        }))());
                                ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawPath(leadingPath__54816, leadingPaint__48343);
                            }
                            else
                            {
                                var topPath__55312 = new global::Doroti.Flutter.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{            var __cascade = topPath__55312;
            __cascade.moveTo(outer__46862.left, (outer__46862.top + (((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2L)));
            __cascade.lineTo(outer__46862.right, (outer__46862.top + (((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2L)));
            return __cascade;        }))());
                                ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawPath(topPath__55312, leadingPaint__48343);
                                global::Doroti.Flutter.Ui.Paint paint__55589 = ((global::Doroti.Flutter.Ui.Paint)(object?)this.borderSide.toPaint());
                                var paths__55637 = new global::Doroti.Flutter.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{            var __cascade = paths__55637;
            __cascade.moveTo(rrect__47063.left, (outer__46862.top + ((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width));
            __cascade.lineTo(rrect__47063.left, outer__46862.bottom);
            __cascade.moveTo(rrect__47063.right, (outer__46862.top + ((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width));
            __cascade.lineTo(rrect__47063.right, outer__46862.bottom);
            return __cascade;        }))());
                                ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawPath(paths__55637, paint__55589);
                            }
                        }
                        break;
                    }
                case global::Doroti.Generated.Framework.Painting.VerticalDirection.up:
                    {
                        if (this.isLastButton)
                        {
                            var bottomPath__56089 = new global::Doroti.Flutter.Ui.Path();
                            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{            var __cascade = bottomPath__56089;
            __cascade.moveTo(outer__46862.left, (outer__46862.bottom - (((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2.0)));
            __cascade.lineTo(outer__46862.right, (outer__46862.bottom - (((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2.0)));
            return __cascade;        }))());
                            ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawPath(bottomPath__56089, leadingPaint__48343);
                            global::Doroti.Flutter.Ui.Paint endingPaint__56385 = ((global::Doroti.Flutter.Ui.Paint)(object?)this.trailingBorderSide.toPaint());
                            var endingPath__56447 = new global::Doroti.Flutter.Ui.Path();
                            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{            var __cascade = endingPath__56447;
            __cascade.moveTo(rrect__47063.left, (rrect__47063.bottom - (((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2.0)));
            __cascade.lineTo(rrect__47063.left, (rrect__47063.top + rrect__47063.tlRadiusY));
            __cascade.addArc(tlCorner__47694, Dart_mathLibrary.pi, sweepAngle__47019);
            __cascade.lineTo((rrect__47063.right - rrect__47063.trRadiusX), rrect__47063.top);
            __cascade.addArc(trCorner__47995, ((Dart_mathLibrary.pi * 3.0) / 2.0), sweepAngle__47019);
            __cascade.lineTo(rrect__47063.right, (rrect__47063.bottom - (((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2.0)));
            return __cascade;        }))());
                            ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawPath(endingPath__56447, endingPaint__56385);
                        }
                        else
                        {
                            if (this.isFirstButton)
                            {
                                var leadingPath__57022 = new global::Doroti.Flutter.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{            var __cascade = leadingPath__57022;
            __cascade.moveTo(rrect__47063.left, outer__46862.top);
            __cascade.lineTo(rrect__47063.left, (rrect__47063.bottom - rrect__47063.blRadiusY));
            __cascade.addArc(blCorner__47830, Dart_mathLibrary.pi, -sweepAngle__47019);
            __cascade.lineTo((rrect__47063.right - rrect__47063.brRadiusX), rrect__47063.bottom);
            __cascade.addArc(brCorner__48152, (Dart_mathLibrary.pi / 2.0), -sweepAngle__47019);
            __cascade.lineTo(rrect__47063.right, outer__46862.top);
            return __cascade;        }))());
                                ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawPath(leadingPath__57022, leadingPaint__48343);
                            }
                            else
                            {
                                var bottomPath__57514 = new global::Doroti.Flutter.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{            var __cascade = bottomPath__57514;
            __cascade.moveTo(outer__46862.left, (outer__46862.bottom - (((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2.0)));
            __cascade.lineTo(outer__46862.right, (outer__46862.bottom - (((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width / 2.0)));
            return __cascade;        }))());
                                ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawPath(bottomPath__57514, leadingPaint__48343);
                                global::Doroti.Flutter.Ui.Paint paint__57810 = ((global::Doroti.Flutter.Ui.Paint)(object?)this.borderSide.toPaint());
                                var paths__57858 = new global::Doroti.Flutter.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{            var __cascade = paths__57858;
            __cascade.moveTo(rrect__47063.left, outer__46862.top);
            __cascade.lineTo(rrect__47063.left, (outer__46862.bottom - ((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width));
            __cascade.moveTo(rrect__47063.right, outer__46862.top);
            __cascade.lineTo(rrect__47063.right, (outer__46862.bottom - ((global::Doroti.Generated.Framework.Painting.BorderSide)this.leadingBorderSide).width));
            return __cascade;        }))());
                                ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawPath(paths__57858, paint__57810);
                            }
                        }
                        break;
                    }
            }
        }
    }

}

internal class _InputPadding__toggle_buttons : global::Doroti.Generated.Framework.Widgets.SingleChildRenderObjectWidget
{
    public virtual Size minSize { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.Axis direction { get; private set; } = default!;

    internal _InputPadding__toggle_buttons(global::Doroti.Generated.Framework.Widgets.Widget? child = null, Size minSize = default!, global::Doroti.Generated.Framework.Painting.Axis direction = default!) : base(child: child)
    {
        this.minSize = minSize;
        this.direction = direction;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderInputPadding__toggle_buttons(this.minSize, this.direction));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderInputPadding__toggle_buttons)(object)renderObject;
        __renderObject.minSize = this.minSize;
        __renderObject.direction = this.direction;
    }

}

public class _RenderInputPadding__toggle_buttons : global::Doroti.Generated.Framework.Rendering.RenderShiftedBox
{
    internal virtual Size _minSize { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Painting.Axis _direction { get; set; } = default!;

    internal _RenderInputPadding__toggle_buttons(Size _minSize, global::Doroti.Generated.Framework.Painting.Axis _direction, global::Doroti.Generated.Framework.Rendering.RenderBox? child = null) : base(child)
    {
        this._minSize = _minSize;
        this._direction = _direction;
    }

    public virtual global::Doroti.Flutter.Ui.Size minSize
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
    public virtual global::Doroti.Generated.Framework.Painting.Axis direction
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

    internal virtual global::Doroti.Flutter.Ui.Size _computeSize(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, global::System.Func<global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Rendering.BoxConstraints, Size> layoutChild)
    {
        if ((this.child is not null))
        {
            global::Doroti.Flutter.Ui.Size childSize__60615 = ((global::Doroti.Flutter.Ui.Size)(object?)layoutChild(this.child!, constraints));
            double width__60680 = Math.Max(childSize__60615.width, this.minSize.width);
            double height__60749 = Math.Max(childSize__60615.height, this.minSize.height);
            return ((global::Doroti.Flutter.Ui.Size)(object?)constraints.constrain(new global::Doroti.Flutter.Ui.Size(width__60680, height__60749)));
        }
        return Size.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        return _computeSize(constraints: constraints, layoutChild: (global::System.Func<global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Rendering.BoxConstraints, Size>)global::Doroti.Generated.Framework.Rendering.ChildLayoutHelper.dryLayoutChild);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__61186 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((child__61186 is null))
        {
            return null;
        }
        double? result__61274 = child__61186.getDryBaseline(constraints, baseline);
        if ((result__61274 is null))
        {
            return null;
        }
        global::Doroti.Flutter.Ui.Size drySize__61475 = ((global::Doroti.Flutter.Ui.Size)(object?)getDryLayout(constraints));
        global::Doroti.Flutter.Ui.Size childSize__61527 = ((global::Doroti.Flutter.Ui.Size)(object?)child__61186.getDryLayout(constraints));
        global::Doroti.Flutter.Ui.Offset childOffset__61589 = ((global::Doroti.Flutter.Ui.Offset)(object?)global::Doroti.Generated.Framework.Painting.Alignment.center.alongOffset((drySize__61475 - childSize__61527)));
        return (DartRuntimePrimitives.RequireValue(result__61274) + childOffset__61589.dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        size = _computeSize(constraints: this.constraints, layoutChild: (global::System.Func<global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Rendering.BoxConstraints, Size>)global::Doroti.Generated.Framework.Rendering.ChildLayoutHelper.layoutChild);
        if ((this.child is not null))
        {
            var childParentData__61874 = ((global::Doroti.Generated.Framework.Rendering.BoxParentData?)(object?)this.child!.parentData!)!;
            childParentData__61874.offset = global::Doroti.Generated.Framework.Painting.Alignment.center.alongOffset((this.size - this.child!.size));
        }
    }

    public override bool hitTest(global::Doroti.Generated.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        if (!this.size.contains(position))
        {
            return false;
        }
        global::Doroti.Flutter.Ui.Offset center__62432 = ((global::Doroti.Flutter.Ui.Offset)(object?)(this.direction switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => new global::Doroti.Flutter.Ui.Offset(position.dx, (this.child!.size.height / 2L)), global::Doroti.Generated.Framework.Painting.Axis.vertical => new global::Doroti.Flutter.Ui.Offset((this.child!.size.width / 2L), position.dy), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        return result.addWithRawTransform(transform: MatrixUtils.forceToPoint(center__62432), position: center__62432, hitTest: ((global::System.Func<global::Doroti.Generated.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, position) => {
DartRuntimePrimitives.Assert(() => (object.Equals(position, center__62432)));
return this.child!.hitTest(result, position: center__62432);
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
