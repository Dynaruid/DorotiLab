// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/toggle_buttons.dart

using Doroti.Runtime;
using Doroti.Ui;

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

    public ToggleButtons(global::Doroti.Framework.Foundation.Key? key = null, List<global::Doroti.Framework.Widgets.Widget> children = default!, List<bool> isSelected = default!, global::System.Action<long>? onPressed = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, MaterialTapTargetSize? tapTargetSize = null, global::Doroti.Framework.Painting.TextStyle? textStyle = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, Color? color = null, Color? selectedColor = null, Color? disabledColor = null, Color? fillColor = null, Color? focusColor = null, Color? highlightColor = null, Color? hoverColor = null, Color? splashColor = null, List<global::Doroti.Framework.Widgets.FocusNode>? focusNodes = null, bool renderBorder = true, Color? borderColor = null, Color? selectedBorderColor = null, Color? disabledBorderColor = null, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, double? borderWidth = null, global::Doroti.Framework.Painting.Axis direction = Axis.horizontal, global::Doroti.Framework.Painting.VerticalDirection verticalDirection = VerticalDirection.down) : base(key: key)
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
        System.Diagnostics.Debug.Assert(checked(children.Count) == checked((long)isSelected.Count));
    }

    internal virtual bool _isFirstButton(long index, long length, TextDirection textDirection)
    {
        switch (direction)
        {
            case Axis.horizontal:
                {
                    return textDirection switch { TextDirection.rtl => index == (length - 1L), TextDirection.ltr => index == 0L, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
                }
            case Axis.vertical:
                {
                    return verticalDirection switch { VerticalDirection.up => index == (length - 1L), VerticalDirection.down => index == 0L, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isLastButton(long index, long length, TextDirection textDirection)
    {
        switch (direction)
        {
            case Axis.horizontal:
                {
                    return textDirection switch { TextDirection.rtl => index == 0L, TextDirection.ltr => index == (length - 1L), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
                }
            case Axis.vertical:
                {
                    return verticalDirection switch { VerticalDirection.up => index == 0L, VerticalDirection.down => index == (length - 1L), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.BorderRadius _getEdgeBorderRadius(long index, long length, TextDirection textDirection, ToggleButtonsThemeData toggleButtonsTheme)
    {
        global::Doroti.Framework.Painting.BorderRadius resultingBorderRadius = (borderRadius ?? toggleButtonsTheme.borderRadius) ?? BorderRadius.zero;
        if (length == 1L)
        {
            return resultingBorderRadius;
        }
        else
        {
            if (Equals(direction, Axis.horizontal))
            {
                if (_isFirstButton(index, length, textDirection))
                {
                    return new global::Doroti.Framework.Painting.BorderRadius(topLeft: resultingBorderRadius.topLeft, bottomLeft: resultingBorderRadius.bottomLeft);
                }
                else
                {
                    if (_isLastButton(index, length, textDirection))
                    {
                        return new global::Doroti.Framework.Painting.BorderRadius(topRight: resultingBorderRadius.topRight, bottomRight: resultingBorderRadius.bottomRight);
                    }
                }
            }
            else
            {
                if (_isFirstButton(index, length, textDirection))
                {
                    return new global::Doroti.Framework.Painting.BorderRadius(topLeft: resultingBorderRadius.topLeft, topRight: resultingBorderRadius.topRight);
                }
                else
                {
                    if (_isLastButton(index, length, textDirection))
                    {
                        return new global::Doroti.Framework.Painting.BorderRadius(bottomLeft: resultingBorderRadius.bottomLeft, bottomRight: resultingBorderRadius.bottomRight);
                    }
                }
            }
        }
        return BorderRadius.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.BorderRadius _getClipBorderRadius(long index, long length, TextDirection textDirection, ToggleButtonsThemeData toggleButtonsTheme)
    {
        global::Doroti.Framework.Painting.BorderRadius resultingBorderRadius = (borderRadius ?? toggleButtonsTheme.borderRadius) ?? BorderRadius.zero;
        double resultingBorderWidth = (borderWidth ?? toggleButtonsTheme.borderWidth) ?? _defaultBorderWidth;
        if (length == 1L)
        {
            return new global::Doroti.Framework.Painting.BorderRadius(topLeft: resultingBorderRadius.topLeft - Radius.circular(resultingBorderWidth / 2.0), bottomLeft: resultingBorderRadius.bottomLeft - Radius.circular(resultingBorderWidth / 2.0), topRight: resultingBorderRadius.topRight - Radius.circular(resultingBorderWidth / 2.0), bottomRight: resultingBorderRadius.bottomRight - Radius.circular(resultingBorderWidth / 2.0));
        }
        else
        {
            if (Equals(direction, Axis.horizontal))
            {
                if (_isFirstButton(index, length, textDirection))
                {
                    return new global::Doroti.Framework.Painting.BorderRadius(topLeft: resultingBorderRadius.topLeft - Radius.circular(resultingBorderWidth / 2.0), bottomLeft: resultingBorderRadius.bottomLeft - Radius.circular(resultingBorderWidth / 2.0));
                }
                else
                {
                    if (_isLastButton(index, length, textDirection))
                    {
                        return new global::Doroti.Framework.Painting.BorderRadius(topRight: resultingBorderRadius.topRight - Radius.circular(resultingBorderWidth / 2.0), bottomRight: resultingBorderRadius.bottomRight - Radius.circular(resultingBorderWidth / 2.0));
                    }
                }
            }
            else
            {
                if (_isFirstButton(index, length, textDirection))
                {
                    return new global::Doroti.Framework.Painting.BorderRadius(topLeft: resultingBorderRadius.topLeft - Radius.circular(resultingBorderWidth / 2.0), topRight: resultingBorderRadius.topRight - Radius.circular(resultingBorderWidth / 2.0));
                }
                else
                {
                    if (_isLastButton(index, length, textDirection))
                    {
                        return new global::Doroti.Framework.Painting.BorderRadius(bottomLeft: resultingBorderRadius.bottomLeft - Radius.circular(resultingBorderWidth / 2.0), bottomRight: resultingBorderRadius.bottomRight - Radius.circular(resultingBorderWidth / 2.0));
                    }
                }
            }
        }
        return BorderRadius.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.BorderSide _getLeadingBorderSide(long index, ThemeData theme, ToggleButtonsThemeData toggleButtonsTheme)
    {
        if (!renderBorder)
        {
            return BorderSide.none;
        }
        double resultingBorderWidth = (borderWidth ?? toggleButtonsTheme.borderWidth) ?? _defaultBorderWidth;
        if ((onPressed is not null) && (isSelected[(int)index] || (index != 0L) && isSelected[(int)(index - 1L)]))
        {
            return new global::Doroti.Framework.Painting.BorderSide(color: (selectedBorderColor ?? toggleButtonsTheme.selectedBorderColor) ?? theme.colorScheme.onSurface.withOpacity(0.12), width: resultingBorderWidth);
        }
        else
        {
            if ((onPressed is not null) && !isSelected[(int)index])
            {
                return new global::Doroti.Framework.Painting.BorderSide(color: (borderColor ?? toggleButtonsTheme.borderColor) ?? theme.colorScheme.onSurface.withOpacity(0.12), width: resultingBorderWidth);
            }
            else
            {
                return new global::Doroti.Framework.Painting.BorderSide(color: (disabledBorderColor ?? toggleButtonsTheme.disabledBorderColor) ?? theme.colorScheme.onSurface.withOpacity(0.12), width: resultingBorderWidth);
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.BorderSide _getBorderSide(long index, ThemeData theme, ToggleButtonsThemeData toggleButtonsTheme)
    {
        if (!renderBorder)
        {
            return BorderSide.none;
        }
        double resultingBorderWidth = (borderWidth ?? toggleButtonsTheme.borderWidth) ?? _defaultBorderWidth;
        if ((onPressed is not null) && isSelected[(int)index])
        {
            return new global::Doroti.Framework.Painting.BorderSide(color: (selectedBorderColor ?? toggleButtonsTheme.selectedBorderColor) ?? theme.colorScheme.onSurface.withOpacity(0.12), width: resultingBorderWidth);
        }
        else
        {
            if ((onPressed is not null) && !isSelected[(int)index])
            {
                return new global::Doroti.Framework.Painting.BorderSide(color: (borderColor ?? toggleButtonsTheme.borderColor) ?? theme.colorScheme.onSurface.withOpacity(0.12), width: resultingBorderWidth);
            }
            else
            {
                return new global::Doroti.Framework.Painting.BorderSide(color: (disabledBorderColor ?? toggleButtonsTheme.disabledBorderColor) ?? theme.colorScheme.onSurface.withOpacity(0.12), width: resultingBorderWidth);
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.BorderSide _getTrailingBorderSide(long index, ThemeData theme, ToggleButtonsThemeData toggleButtonsTheme)
    {
        if (!renderBorder)
        {
            return BorderSide.none;
        }
        if (index != (checked(children.Count) - 1L))
        {
            return BorderSide.none;
        }
        double resultingBorderWidth = (borderWidth ?? toggleButtonsTheme.borderWidth) ?? _defaultBorderWidth;
        if ((onPressed is not null) && isSelected[(int)index])
        {
            return new global::Doroti.Framework.Painting.BorderSide(color: (selectedBorderColor ?? toggleButtonsTheme.selectedBorderColor) ?? theme.colorScheme.onSurface.withOpacity(0.12), width: resultingBorderWidth);
        }
        else
        {
            if ((onPressed is not null) && !isSelected[(int)index])
            {
                return new global::Doroti.Framework.Painting.BorderSide(color: (borderColor ?? toggleButtonsTheme.borderColor) ?? theme.colorScheme.onSurface.withOpacity(0.12), width: resultingBorderWidth);
            }
            else
            {
                return new global::Doroti.Framework.Painting.BorderSide(color: (disabledBorderColor ?? toggleButtonsTheme.disabledBorderColor) ?? theme.colorScheme.onSurface.withOpacity(0.12), width: resultingBorderWidth);
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (focusNodes is not null)
                {
                    return checked(focusNodes!.Count) == checked((long)children.Count);
                }
                return true;
            }, () => (object?)"focusNodes.length must match children.length.\n" + $"There are {checked((long)focusNodes!.Count)} focus nodes, while " + $"there are {checked((long)children.Count)} children.");
        ThemeData theme = Theme.of(context);
        ToggleButtonsThemeData toggleButtonsTheme = ToggleButtonsTheme.of(context);
        global::Doroti.Ui.TextDirection textDirection = Directionality.of(context);
        var buttons = new List<global::Doroti.Framework.Widgets.Widget>(Enumerable.Select(Enumerable.Range(0, checked((int)checked((long)children.Count))), (index) =>
        {
            global::Doroti.Framework.Painting.BorderRadius edgeBorderRadius = _getEdgeBorderRadius(index, checked(children.Count), textDirection, toggleButtonsTheme);
            global::Doroti.Framework.Painting.BorderRadius clipBorderRadius = _getClipBorderRadius(index, checked(children.Count), textDirection, toggleButtonsTheme);
            global::Doroti.Framework.Painting.BorderSide leadingBorderSideLocal = _getLeadingBorderSide(index, theme, toggleButtonsTheme);
            global::Doroti.Framework.Painting.BorderSide borderSideLocal = _getBorderSide(index, theme, toggleButtonsTheme);
            global::Doroti.Framework.Painting.BorderSide trailingBorderSideLocal = _getTrailingBorderSide(index, theme, toggleButtonsTheme);
            var states = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection26216 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (isSelected[index] && (onPressed is not null)) { __collection26216.Add(WidgetState.selected); } if (onPressed is null) { __collection26216.Add(WidgetState.disabled); } return __collection26216; }))();
            global::Doroti.Ui.Color effectiveFillColor = new _ResolveFillColor__toggle_buttons(fillColor ?? toggleButtonsTheme.fillColor).resolve(states) ?? new _DefaultFillColor__toggle_buttons(theme.colorScheme).resolve(states);
            global::Doroti.Ui.Color currentColor = default!;
            if ((onPressed is not null) && isSelected[index])
            {
                currentColor = (selectedColor ?? toggleButtonsTheme.selectedColor) ?? theme.colorScheme.primary;
            }
            else
            {
                if ((onPressed is not null) && !isSelected[index])
                {
                    currentColor = (color ?? toggleButtonsTheme.color) ?? theme.colorScheme.onSurface.withOpacity(0.87);
                }
                else
                {
                    currentColor = (disabledColor ?? toggleButtonsTheme.disabledColor) ?? theme.colorScheme.onSurface.withOpacity(0.38);
                }
            }
            global::Doroti.Framework.Painting.TextStyle currentTextStyle = (textStyle ?? toggleButtonsTheme.textStyle) ?? theme.textTheme.bodyMedium!;
            global::Doroti.Framework.Rendering.BoxConstraints? currentConstraints = constraints ?? toggleButtonsTheme.constraints;
            global::Doroti.Ui.Size minimumSizeLocal = currentConstraints?.smallest ?? new global::Doroti.Ui.Size(Widgets.ConstantsLibrary.kMinInteractiveDimension);
            global::Doroti.Ui.Size? maximumSizeLocal = currentConstraints?.biggest;
            global::Doroti.Ui.Size minPaddingSize = default!;
            switch (tapTargetSize ?? theme.materialTapTargetSize)
            {
                case var __constant27628 when Equals(__constant27628, MaterialTapTargetSize.padded):
                    {
                        minPaddingSize = direction switch { Axis.horizontal => new global::Doroti.Ui.Size(0.0, Widgets.ConstantsLibrary.kMinInteractiveDimension), Axis.vertical => new global::Doroti.Ui.Size(Widgets.ConstantsLibrary.kMinInteractiveDimension, 0.0), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
                        DartRuntimePrimitives.Assert(() => minPaddingSize.width >= 0.0);
                        DartRuntimePrimitives.Assert(() => minPaddingSize.height >= 0.0);
                        break;
                    }
                case var __constant27973 when Equals(__constant27973, MaterialTapTargetSize.shrinkWrap):
                    {
                        minPaddingSize = Size.zero;
                        break;
                    }
            }
            global::Doroti.Framework.Widgets.Widget button = new _SelectToggleButton__toggle_buttons(leadingBorderSide: leadingBorderSideLocal, borderSide: borderSideLocal, trailingBorderSide: trailingBorderSideLocal, borderRadius: edgeBorderRadius, isFirstButton: index == 0L, isLastButton: index == (checked(children.Count) - 1L), direction: direction, verticalDirection: verticalDirection, child: new global::Doroti.Framework.Widgets.ClipRRect(borderRadius: clipBorderRadius, child: new TextButton(focusNode: (focusNodes is not null) ? focusNodes![index] : null, style: new ButtonStyle(backgroundColor: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color?>(effectiveFillColor), foregroundColor: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color?>(currentColor), iconSize: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(24.0), iconColor: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color?>(currentColor), overlayColor: new _ToggleButtonDefaultOverlay__toggle_buttons(selected: (onPressed is not null) && isSelected[index], unselected: (onPressed is not null) && !isSelected[index], colorScheme: theme.colorScheme, disabledColor: disabledColor ?? toggleButtonsTheme.disabledColor, focusColor: focusColor ?? toggleButtonsTheme.focusColor, highlightColor: highlightColor ?? toggleButtonsTheme.highlightColor, hoverColor: hoverColor ?? toggleButtonsTheme.hoverColor, splashColor: splashColor ?? toggleButtonsTheme.splashColor), elevation: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(0), textStyle: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.TextStyle?>(currentTextStyle.copyWith(color: currentColor)), padding: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(EdgeInsets.zero), minimumSize: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Size?>(minimumSizeLocal), maximumSize: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Size?>(maximumSizeLocal), shape: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.OutlinedBorder>(new global::Doroti.Framework.Painting.RoundedRectangleBorder()), mouseCursor: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Services.MouseCursor?>(mouseCursor), visualDensity: VisualDensity.standard, tapTargetSize: MaterialTapTargetSize.shrinkWrap, animationDuration: ConstantsLibrary.kThemeChangeDuration, enableFeedback: true, alignment: Alignment.center, splashFactory: InkRipple.splashFactory), onPressed: (onPressed is not null) ? (() =>
            {
                onPressed!(index);
            }) : null, child: children[index])));
            if (currentConstraints is not null)
            {
                button = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Center(child: button));
            }
            return new global::Doroti.Framework.Widgets.MergeSemantics(child: new global::Doroti.Framework.Widgets.Semantics(container: true, @checked: isSelected[index], enabled: onPressed is not null, child: new _InputPadding__toggle_buttons(minSize: minPaddingSize, direction: direction, child: button)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        if (Equals(direction, Axis.vertical))
        {
            return new global::Doroti.Framework.Widgets.IntrinsicWidth(child: new global::Doroti.Framework.Widgets.Column(mainAxisSize: MainAxisSize.min, crossAxisAlignment: CrossAxisAlignment.stretch, verticalDirection: verticalDirection, children: buttons));
        }
        return new global::Doroti.Framework.Widgets.IntrinsicHeight(child: new global::Doroti.Framework.Widgets.Row(mainAxisSize: MainAxisSize.min, crossAxisAlignment: CrossAxisAlignment.stretch, children: buttons));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("disabled", value: onPressed is null, ifTrue: "Buttons are disabled", ifFalse: "Buttons are enabled"));
        textStyle?.debugFillProperties(properties, prefix: "textStyle.");
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("color", color, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("selectedColor", selectedColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("disabledColor", disabledColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("fillColor", fillColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("focusColor", focusColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("highlightColor", highlightColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("hoverColor", hoverColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("splashColor", splashColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("borderColor", borderColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("selectedBorderColor", selectedBorderColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("disabledBorderColor", disabledBorderColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.BorderRadius>("borderRadius", borderRadius, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("borderWidth", borderWidth, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.Axis>("direction", direction, defaultValue: Axis.horizontal));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.VerticalDirection>("verticalDirection", verticalDirection, defaultValue: VerticalDirection.down));
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
        if (primary is global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>)
        {
            global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> primary__as33643 = (global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>)primary;
            return WidgetStateProperty.resolveAs<global::Doroti.Ui.Color?>(primary, states);
        }
        return states.Contains(WidgetState.selected) ? primary : null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);
    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
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
        if (states.Contains(WidgetState.selected))
        {
            return colorScheme.primary.withOpacity(0.12);
        }
        return colorScheme.surface.withOpacity(0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);
    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
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
        if (selected)
        {
            if (states.Contains(WidgetState.pressed))
            {
                return splashColor ?? colorScheme?.primary.withOpacity(0.16);
            }
            if (states.Contains(WidgetState.hovered))
            {
                return hoverColor ?? colorScheme?.primary.withOpacity(0.04);
            }
            if (states.Contains(WidgetState.focused))
            {
                return focusColor ?? colorScheme?.primary.withOpacity(0.12);
            }
        }
        else
        {
            if (unselected)
            {
                if (states.Contains(WidgetState.pressed))
                {
                    return (splashColor ?? highlightColor) ?? colorScheme?.onSurface.withOpacity(0.16);
                }
                if (states.Contains(WidgetState.hovered))
                {
                    return hoverColor ?? colorScheme?.onSurface.withOpacity(0.04);
                }
                if (states.Contains(WidgetState.focused))
                {
                    return focusColor ?? colorScheme?.onSurface.withOpacity(0.12);
                }
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"    {{\n      selected:\n        hovered: {hoverColor}, otherwise: {colorScheme?.primary.withOpacity(0.04)},\n        focused: {focusColor}, otherwise: {colorScheme?.primary.withOpacity(0.12)},\n        pressed: {splashColor}, otherwise: {colorScheme?.primary.withOpacity(0.16)},\n      unselected:\n        hovered: {hoverColor}, otherwise: {colorScheme?.onSurface.withOpacity(0.04)},\n        focused: {focusColor}, otherwise: {colorScheme?.onSurface.withOpacity(0.12)},\n        pressed: {splashColor}, otherwise: {colorScheme?.onSurface.withOpacity(0.16)},\n      otherwise: null,\n    }}\n    ";
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

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(new _SelectToggleButtonRenderObject__toggle_buttons(leadingBorderSide, borderSide, trailingBorderSide, borderRadius, isFirstButton, isLastButton, direction, verticalDirection, Directionality.of(context)));
    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_SelectToggleButtonRenderObject__toggle_buttons)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_SelectToggleButtonRenderObject__toggle_buttons>)(() =>
{
    var __cascade = __renderObject;
    __cascade.leadingBorderSide = leadingBorderSide;
    __cascade.borderSide = borderSide;
    __cascade.trailingBorderSide = trailingBorderSide;
    __cascade.borderRadius = borderRadius;
    __cascade.isFirstButton = isFirstButton;
    __cascade.isLastButton = isLastButton;
    __cascade.direction = direction;
    __cascade.verticalDirection = verticalDirection;
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
        get => _direction;
        set
        {
            var __value = value;
            if (Equals(_direction, __value))
            {
                return;
            }
            _direction = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Painting.VerticalDirection verticalDirection
    {
        get => _verticalDirection;
        set
        {
            var __value = value;
            if (Equals(_verticalDirection, __value))
            {
                return;
            }
            _verticalDirection = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Painting.BorderSide leadingBorderSide
    {
        get => _leadingBorderSide;
        set
        {
            var __value = value;
            if (Equals(_leadingBorderSide, __value))
            {
                return;
            }
            _leadingBorderSide = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Painting.BorderSide borderSide
    {
        get => _borderSide;
        set
        {
            var __value = value;
            if (Equals(_borderSide, __value))
            {
                return;
            }
            _borderSide = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Painting.BorderSide trailingBorderSide
    {
        get => _trailingBorderSide;
        set
        {
            var __value = value;
            if (Equals(_trailingBorderSide, __value))
            {
                return;
            }
            _trailingBorderSide = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Painting.BorderRadius borderRadius
    {
        get => _borderRadius;
        set
        {
            var __value = value;
            if (Equals(_borderRadius, __value))
            {
                return;
            }
            _borderRadius = __value;
            markNeedsLayout();
        }
    }
    public virtual bool isFirstButton
    {
        get => _isFirstButton;
        set
        {
            var __value = value;
            if (_isFirstButton == __value)
            {
                return;
            }
            _isFirstButton = __value;
            markNeedsLayout();
        }
    }
    public virtual bool isLastButton
    {
        get => _isLastButton;
        set
        {
            var __value = value;
            if (_isLastButton == __value)
            {
                return;
            }
            _isLastButton = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.TextDirection textDirection
    {
        get => _textDirection;
        set
        {
            var __value = value;
            if (Equals(_textDirection, __value))
            {
                return;
            }
            _textDirection = __value;
            markNeedsLayout();
        }
    }
    internal static double _maxHeight(global::Doroti.Framework.Rendering.RenderBox? box, double width)
    {
        return box?.getMaxIntrinsicHeight(width) ?? 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _minHeight(global::Doroti.Framework.Rendering.RenderBox? box, double width)
    {
        return box?.getMinIntrinsicHeight(width) ?? 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _minWidth(global::Doroti.Framework.Rendering.RenderBox? box, double height)
    {
        return box?.getMinIntrinsicWidth(height) ?? 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _maxWidth(global::Doroti.Framework.Rendering.RenderBox? box, double height)
    {
        return box?.getMaxIntrinsicWidth(height) ?? 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        var childOffset = new global::Doroti.Framework.Rendering.BaselineOffset(child?.getDistanceToActualBaseline(baseline));
        return (direction switch { Axis.horizontal => childOffset.op_Add(borderSide.width), Axis.vertical => childOffset.op_Add(verticalDirection switch { VerticalDirection.down => leadingBorderSide.width, VerticalDirection.up => trailingBorderSide.width, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }).offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return Equals(direction, Axis.horizontal) ? ((borderSide.width * 2.0) + _maxHeight(child, width)) : (leadingBorderSide.width + _maxHeight(child, width) + trailingBorderSide.width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return Equals(direction, Axis.horizontal) ? ((borderSide.width * 2.0) + _minHeight(child, width)) : (leadingBorderSide.width + _maxHeight(child, width) + trailingBorderSide.width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return Equals(direction, Axis.horizontal) ? (leadingBorderSide.width + _maxWidth(child, height) + trailingBorderSide.width) : ((borderSide.width * 2.0) + _maxWidth(child, height));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        return Equals(direction, Axis.horizontal) ? (leadingBorderSide.width + _minWidth(child, height) + trailingBorderSide.width) : ((borderSide.width * 2.0) + _minWidth(child, height));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return _computeSize(constraints: constraints, layoutChild: ChildLayoutHelper.dryLayoutChild);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.EdgeInsetsDirectional _childPadding
    {
        get
        {
            DartRuntimePrimitives.Assert(() => child is not null);
            return direction switch { Axis.horizontal => EdgeInsetsDirectional.CreateOnly(start: leadingBorderSide.width, end: trailingBorderSide.width, top: borderSide.width, bottom: borderSide.width), Axis.vertical => EdgeInsetsDirectional.CreateOnly(start: borderSide.width, end: borderSide.width, top: leadingBorderSide.width, bottom: trailingBorderSide.width), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        }
    }
    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        double? childBaseline = child?.getDryBaseline(constraints.deflate(_childPadding), baseline);
        if (childBaseline is null)
        {
            return null;
        }
        return DartRuntimePrimitives.RequireValue(childBaseline) + (direction switch { Axis.horizontal => borderSide.width, Axis.vertical => verticalDirection switch { VerticalDirection.down => leadingBorderSide.width, VerticalDirection.up => trailingBorderSide.width, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        size = _computeSize(constraints: constraints, layoutChild: ChildLayoutHelper.layoutChild);
        if (child is null)
        {
            return;
        }
        var childParentData = ((global::Doroti.Framework.Rendering.BoxParentData?)child!.parentData!)!;
        if (Equals(direction, Axis.horizontal))
        {
            childParentData.offset = textDirection switch { TextDirection.ltr => new global::Doroti.Ui.Offset(leadingBorderSide.width, borderSide.width), TextDirection.rtl => new global::Doroti.Ui.Offset(trailingBorderSide.width, borderSide.width), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        }
        else
        {
            childParentData.offset = verticalDirection switch { VerticalDirection.down => new global::Doroti.Ui.Offset(borderSide.width, leadingBorderSide.width), VerticalDirection.up => new global::Doroti.Ui.Offset(borderSide.width, trailingBorderSide.width), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        }
    }

    internal virtual global::Doroti.Ui.Size _computeSize(global::Doroti.Framework.Rendering.BoxConstraints constraints, global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size> layoutChild)
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = child;
        if (childLocal is null)
        {
            var horizontalSize = new global::Doroti.Ui.Size(leadingBorderSide.width + trailingBorderSide.width, borderSide.width * 2.0);
            return direction switch { Axis.horizontal => constraints.constrain(horizontalSize), Axis.vertical => constraints.constrain(horizontalSize.flipped), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        }
        global::Doroti.Framework.Painting.EdgeInsetsDirectional childPadding = _childPadding;
        global::Doroti.Framework.Rendering.BoxConstraints innerConstraints = constraints.deflate(childPadding);
        return constraints.constrain(childPadding.inflateSize(layoutChild(childLocal, innerConstraints)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        base.paint(context, offset);
        global::Doroti.Ui.Offset bottomRightLocal = size.bottomRight(offset);
        var outer = Rect.fromLTRB(offset.dx, offset.dy, bottomRightLocal.dx, bottomRightLocal.dy);
        global::Doroti.Ui.Rect center = outer.deflate(borderSide.width / 2.0);
        double sweepAngle = Dart_mathLibrary.pi / 2.0;
        global::Doroti.Ui.RRect rrect = RRect.fromRectAndCorners(center, topLeft: ((borderRadius.topLeft.x * borderRadius.topLeft.y) != 0.0) ? borderRadius.topLeft : Radius.zero, topRight: ((borderRadius.topRight.x * borderRadius.topRight.y) != 0.0) ? borderRadius.topRight : Radius.zero, bottomLeft: ((borderRadius.bottomLeft.x * borderRadius.bottomLeft.y) != 0.0) ? borderRadius.bottomLeft : Radius.zero, bottomRight: ((borderRadius.bottomRight.x * borderRadius.bottomRight.y) != 0.0) ? borderRadius.bottomRight : Radius.zero).scaleRadii();
        var tlCorner = Rect.fromLTWH(rrect.left, rrect.top, rrect.tlRadiusX * 2.0, rrect.tlRadiusY * 2.0);
        var blCorner = Rect.fromLTWH(rrect.left, rrect.bottom - rrect.blRadiusY * 2.0, rrect.blRadiusX * 2.0, rrect.blRadiusY * 2.0);
        var trCorner = Rect.fromLTWH(rrect.right - rrect.trRadiusX * 2L, rrect.top, rrect.trRadiusX * 2L, rrect.trRadiusY * 2L);
        var brCorner = Rect.fromLTWH(rrect.right - rrect.brRadiusX * 2L, rrect.bottom - rrect.brRadiusY * 2L, rrect.brRadiusX * 2L, rrect.brRadiusY * 2L);
        global::Doroti.Ui.Paint leadingPaint = leadingBorderSide.toPaint();
        if (isFirstButton && isLastButton)
        {
            var leadingPath = new global::Doroti.Ui.Path();
            double startX = (rrect.brRadiusX == 0.0) ? outer.right : (rrect.right - rrect.brRadiusX);
            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = leadingPath;
    __cascade.moveTo(startX, rrect.bottom);
    __cascade.lineTo(rrect.left + rrect.blRadiusX, rrect.bottom);
    __cascade.addArc(blCorner, Dart_mathLibrary.pi / 2.0, sweepAngle);
    __cascade.lineTo(rrect.left, rrect.top + rrect.tlRadiusY);
    __cascade.addArc(tlCorner, Dart_mathLibrary.pi, sweepAngle);
    __cascade.lineTo(rrect.right - rrect.trRadiusX, rrect.top);
    __cascade.addArc(trCorner, Dart_mathLibrary.pi * 3.0 / 2.0, sweepAngle);
    __cascade.lineTo(rrect.right, rrect.bottom - rrect.brRadiusY);
    __cascade.addArc(brCorner, 0, sweepAngle);
    return __cascade;
}))());
            context.canvas.drawPath(leadingPath, leadingPaint);
            return;
        }
        if (Equals(direction, Axis.horizontal))
        {
            switch (textDirection)
            {
                case TextDirection.ltr:
                    {
                        if (isLastButton)
                        {
                            var leftPath = new global::Doroti.Ui.Path();
                            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = leftPath;
    __cascade.moveTo(rrect.left, rrect.bottom + (leadingBorderSide.width / 2L));
    __cascade.lineTo(rrect.left, rrect.top - (leadingBorderSide.width / 2L));
    return __cascade;
}))());
                            context.canvas.drawPath(leftPath, leadingPaint);
                            global::Doroti.Ui.Paint endingPaint = trailingBorderSide.toPaint();
                            var endingPath = new global::Doroti.Ui.Path();
                            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = endingPath;
    __cascade.moveTo(rrect.left + (borderSide.width / 2.0), rrect.top);
    __cascade.lineTo(rrect.right - rrect.trRadiusX, rrect.top);
    __cascade.addArc(trCorner, Dart_mathLibrary.pi * 3.0 / 2.0, sweepAngle);
    __cascade.lineTo(rrect.right, rrect.bottom - rrect.brRadiusY);
    __cascade.addArc(brCorner, 0, sweepAngle);
    __cascade.lineTo(rrect.left + (borderSide.width / 2.0), rrect.bottom);
    return __cascade;
}))());
                            context.canvas.drawPath(endingPath, endingPaint);
                        }
                        else
                        {
                            if (isFirstButton)
                            {
                                var leadingPathLocal = new global::Doroti.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = leadingPathLocal;
    __cascade.moveTo(outer.right, rrect.bottom);
    __cascade.lineTo(rrect.left + rrect.blRadiusX, rrect.bottom);
    __cascade.addArc(blCorner, Dart_mathLibrary.pi / 2.0, sweepAngle);
    __cascade.lineTo(rrect.left, rrect.top + rrect.tlRadiusY);
    __cascade.addArc(tlCorner, Dart_mathLibrary.pi, sweepAngle);
    __cascade.lineTo(outer.right, rrect.top);
    return __cascade;
}))());
                                context.canvas.drawPath(leadingPathLocal, leadingPaint);
                            }
                            else
                            {
                                var leadingPathAlternate = new global::Doroti.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = leadingPathAlternate;
    __cascade.moveTo(rrect.left, rrect.bottom + (leadingBorderSide.width / 2L));
    __cascade.lineTo(rrect.left, rrect.top - (leadingBorderSide.width / 2L));
    return __cascade;
}))());
                                context.canvas.drawPath(leadingPathAlternate, leadingPaint);
                                global::Doroti.Ui.Paint horizontalPaint = borderSide.toPaint();
                                var horizontalPaths = new global::Doroti.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = horizontalPaths;
    __cascade.moveTo(rrect.left + (borderSide.width / 2.0), rrect.top);
    __cascade.lineTo(outer.right - rrect.trRadiusX, rrect.top);
    __cascade.moveTo(rrect.left + (borderSide.width / 2.0) + rrect.tlRadiusX, rrect.bottom);
    __cascade.lineTo(outer.right - rrect.trRadiusX, rrect.bottom);
    return __cascade;
}))());
                                context.canvas.drawPath(horizontalPaths, horizontalPaint);
                            }
                        }
                        break;
                    }
                case TextDirection.rtl:
                    {
                        if (isLastButton)
                        {
                            var leadingPathNested = new global::Doroti.Ui.Path();
                            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = leadingPathNested;
    __cascade.moveTo(rrect.right, rrect.bottom + (leadingBorderSide.width / 2L));
    __cascade.lineTo(rrect.right, rrect.top - (leadingBorderSide.width / 2L));
    return __cascade;
}))());
                            context.canvas.drawPath(leadingPathNested, leadingPaint);
                            global::Doroti.Ui.Paint endingPaintLocal = trailingBorderSide.toPaint();
                            var endingPathLocal = new global::Doroti.Ui.Path();
                            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = endingPathLocal;
    __cascade.moveTo(rrect.right - (borderSide.width / 2.0), rrect.top);
    __cascade.lineTo(rrect.left + rrect.tlRadiusX, rrect.top);
    __cascade.addArc(tlCorner, Dart_mathLibrary.pi * 3.0 / 2.0, -sweepAngle);
    __cascade.lineTo(rrect.left, rrect.bottom - rrect.blRadiusY);
    __cascade.addArc(blCorner, Dart_mathLibrary.pi, -sweepAngle);
    __cascade.lineTo(rrect.right - (borderSide.width / 2.0), rrect.bottom);
    return __cascade;
}))());
                            context.canvas.drawPath(endingPathLocal, endingPaintLocal);
                        }
                        else
                        {
                            if (isFirstButton)
                            {
                                var leadingPathCurrent = new global::Doroti.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = leadingPathCurrent;
    __cascade.moveTo(outer.left, rrect.bottom);
    __cascade.lineTo(rrect.right - rrect.brRadiusX, rrect.bottom);
    __cascade.addArc(brCorner, Dart_mathLibrary.pi / 2.0, -sweepAngle);
    __cascade.lineTo(rrect.right, rrect.top + rrect.trRadiusY);
    __cascade.addArc(trCorner, 0, -sweepAngle);
    __cascade.lineTo(outer.left, rrect.top);
    return __cascade;
}))());
                                context.canvas.drawPath(leadingPathCurrent, leadingPaint);
                            }
                            else
                            {
                                var leadingPathNext = new global::Doroti.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = leadingPathNext;
    __cascade.moveTo(rrect.right, rrect.bottom + (leadingBorderSide.width / 2L));
    __cascade.lineTo(rrect.right, rrect.top - (leadingBorderSide.width / 2L));
    return __cascade;
}))());
                                context.canvas.drawPath(leadingPathNext, leadingPaint);
                                global::Doroti.Ui.Paint horizontalPaintLocal = borderSide.toPaint();
                                var horizontalPathsLocal = new global::Doroti.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = horizontalPathsLocal;
    __cascade.moveTo(rrect.right - (borderSide.width / 2.0), rrect.top);
    __cascade.lineTo(outer.left - rrect.tlRadiusX, rrect.top);
    __cascade.moveTo(rrect.right - (borderSide.width / 2.0) + rrect.trRadiusX, rrect.bottom);
    __cascade.lineTo(outer.left - rrect.tlRadiusX, rrect.bottom);
    return __cascade;
}))());
                                context.canvas.drawPath(horizontalPathsLocal, horizontalPaintLocal);
                            }
                        }
                        break;
                    }
            }
        }
        else
        {
            switch (verticalDirection)
            {
                case VerticalDirection.down:
                    {
                        if (isLastButton)
                        {
                            var topPath = new global::Doroti.Ui.Path();
                            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = topPath;
    __cascade.moveTo(outer.left, outer.top + (leadingBorderSide.width / 2L));
    __cascade.lineTo(outer.right, outer.top + (leadingBorderSide.width / 2L));
    return __cascade;
}))());
                            context.canvas.drawPath(topPath, leadingPaint);
                            global::Doroti.Ui.Paint endingPaintAlternate = trailingBorderSide.toPaint();
                            var endingPathAlternate = new global::Doroti.Ui.Path();
                            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = endingPathAlternate;
    __cascade.moveTo(rrect.left, rrect.top + (leadingBorderSide.width / 2.0));
    __cascade.lineTo(rrect.left, rrect.bottom - rrect.blRadiusY);
    __cascade.addArc(blCorner, Dart_mathLibrary.pi * 3.0, -sweepAngle);
    __cascade.lineTo(rrect.right - rrect.blRadiusX, rrect.bottom);
    __cascade.addArc(brCorner, Dart_mathLibrary.pi / 2.0, -sweepAngle);
    __cascade.lineTo(rrect.right, rrect.top + (leadingBorderSide.width / 2.0));
    return __cascade;
}))());
                            context.canvas.drawPath(endingPathAlternate, endingPaintAlternate);
                        }
                        else
                        {
                            if (isFirstButton)
                            {
                                var leadingPathCandidate = new global::Doroti.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = leadingPathCandidate;
    __cascade.moveTo(rrect.left, outer.bottom);
    __cascade.lineTo(rrect.left, rrect.top + rrect.tlRadiusX);
    __cascade.addArc(tlCorner, Dart_mathLibrary.pi, sweepAngle);
    __cascade.lineTo(rrect.right - rrect.trRadiusX, rrect.top);
    __cascade.addArc(trCorner, Dart_mathLibrary.pi * 3.0 / 2.0, sweepAngle);
    __cascade.lineTo(rrect.right, outer.bottom);
    return __cascade;
}))());
                                context.canvas.drawPath(leadingPathCandidate, leadingPaint);
                            }
                            else
                            {
                                var topPathLocal = new global::Doroti.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = topPathLocal;
    __cascade.moveTo(outer.left, outer.top + (leadingBorderSide.width / 2L));
    __cascade.lineTo(outer.right, outer.top + (leadingBorderSide.width / 2L));
    return __cascade;
}))());
                                context.canvas.drawPath(topPathLocal, leadingPaint);
                                global::Doroti.Ui.Paint paintLocal = borderSide.toPaint();
                                var paths = new global::Doroti.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = paths;
    __cascade.moveTo(rrect.left, outer.top + leadingBorderSide.width);
    __cascade.lineTo(rrect.left, outer.bottom);
    __cascade.moveTo(rrect.right, outer.top + leadingBorderSide.width);
    __cascade.lineTo(rrect.right, outer.bottom);
    return __cascade;
}))());
                                context.canvas.drawPath(paths, paintLocal);
                            }
                        }
                        break;
                    }
                case VerticalDirection.up:
                    {
                        if (isLastButton)
                        {
                            var bottomPath = new global::Doroti.Ui.Path();
                            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = bottomPath;
    __cascade.moveTo(outer.left, outer.bottom - (leadingBorderSide.width / 2.0));
    __cascade.lineTo(outer.right, outer.bottom - (leadingBorderSide.width / 2.0));
    return __cascade;
}))());
                            context.canvas.drawPath(bottomPath, leadingPaint);
                            global::Doroti.Ui.Paint endingPaintNested = trailingBorderSide.toPaint();
                            var endingPathNested = new global::Doroti.Ui.Path();
                            DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = endingPathNested;
    __cascade.moveTo(rrect.left, rrect.bottom - (leadingBorderSide.width / 2.0));
    __cascade.lineTo(rrect.left, rrect.top + rrect.tlRadiusY);
    __cascade.addArc(tlCorner, Dart_mathLibrary.pi, sweepAngle);
    __cascade.lineTo(rrect.right - rrect.trRadiusX, rrect.top);
    __cascade.addArc(trCorner, Dart_mathLibrary.pi * 3.0 / 2.0, sweepAngle);
    __cascade.lineTo(rrect.right, rrect.bottom - (leadingBorderSide.width / 2.0));
    return __cascade;
}))());
                            context.canvas.drawPath(endingPathNested, endingPaintNested);
                        }
                        else
                        {
                            if (isFirstButton)
                            {
                                var leadingPathA = new global::Doroti.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = leadingPathA;
    __cascade.moveTo(rrect.left, outer.top);
    __cascade.lineTo(rrect.left, rrect.bottom - rrect.blRadiusY);
    __cascade.addArc(blCorner, Dart_mathLibrary.pi, -sweepAngle);
    __cascade.lineTo(rrect.right - rrect.brRadiusX, rrect.bottom);
    __cascade.addArc(brCorner, Dart_mathLibrary.pi / 2.0, -sweepAngle);
    __cascade.lineTo(rrect.right, outer.top);
    return __cascade;
}))());
                                context.canvas.drawPath(leadingPathA, leadingPaint);
                            }
                            else
                            {
                                var bottomPathLocal = new global::Doroti.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = bottomPathLocal;
    __cascade.moveTo(outer.left, outer.bottom - (leadingBorderSide.width / 2.0));
    __cascade.lineTo(outer.right, outer.bottom - (leadingBorderSide.width / 2.0));
    return __cascade;
}))());
                                context.canvas.drawPath(bottomPathLocal, leadingPaint);
                                global::Doroti.Ui.Paint paintAlternate = borderSide.toPaint();
                                var pathsLocal = new global::Doroti.Ui.Path();
                                DartRuntimePrimitives.Ignore(((Func<Path>)(() =>
{
    var __cascade = pathsLocal;
    __cascade.moveTo(rrect.left, outer.top);
    __cascade.lineTo(rrect.left, outer.bottom - leadingBorderSide.width);
    __cascade.moveTo(rrect.right, outer.top);
    __cascade.lineTo(rrect.right, outer.bottom - leadingBorderSide.width);
    return __cascade;
}))());
                                context.canvas.drawPath(pathsLocal, paintAlternate);
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
        return new _RenderInputPadding__toggle_buttons(minSize, direction);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderInputPadding__toggle_buttons)renderObject;
        __renderObject.minSize = minSize;
        __renderObject.direction = direction;
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
    public virtual global::Doroti.Framework.Painting.Axis direction
    {
        get => _direction;
        set
        {
            var __value = value;
            if (Equals(_direction, __value))
            {
                return;
            }
            _direction = __value;
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

    internal virtual global::Doroti.Ui.Size _computeSize(global::Doroti.Framework.Rendering.BoxConstraints constraints, global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size> layoutChild)
    {
        if (child is not null)
        {
            global::Doroti.Ui.Size childSize = layoutChild(child!, constraints);
            double widthLocal = Math.Max(childSize.width, minSize.width);
            double heightLocal = Math.Max(childSize.height, minSize.height);
            return constraints.constrain(new global::Doroti.Ui.Size(widthLocal, heightLocal));
        }
        return Size.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return _computeSize(constraints: constraints, layoutChild: ChildLayoutHelper.dryLayoutChild);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return null;
        }
        double? result = childLocal.getDryBaseline(constraints, baseline);
        if (result is null)
        {
            return null;
        }
        global::Doroti.Ui.Size drySize = getDryLayout(constraints);
        global::Doroti.Ui.Size childSize = childLocal.getDryLayout(constraints);
        global::Doroti.Ui.Offset childOffset = Alignment.center.alongOffset(drySize - childSize);
        return DartRuntimePrimitives.RequireValue(result) + childOffset.dy;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        size = _computeSize(constraints: constraints, layoutChild: ChildLayoutHelper.layoutChild);
        if (child is not null)
        {
            var childParentData = ((global::Doroti.Framework.Rendering.BoxParentData?)child!.parentData!)!;
            childParentData.offset = Alignment.center.alongOffset(size - child!.size);
        }
    }

    public override bool hitTest(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        if (!size.contains(position))
        {
            return false;
        }
        global::Doroti.Ui.Offset center = direction switch { Axis.horizontal => new global::Doroti.Ui.Offset(position.dx, child!.size.height / 2L), Axis.vertical => new global::Doroti.Ui.Offset(child!.size.width / 2L, position.dy), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        return result.addWithRawTransform(transform: MatrixUtils.forceToPoint(center), position: center, hitTest: (result, position) =>
        {
            DartRuntimePrimitives.Assert(() => Equals(position, center));
            return child!.hitTest(result, position: center);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
