// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/checkbox_list_tile.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

internal enum _CheckboxType__checkbox_list_tile
{
    material,
    adaptive,
}

public class CheckboxListTile : StatelessWidget
{
    public virtual bool? value { get; private set; }
    public virtual Action<bool?>? onChanged { get; private set; }
    public virtual MouseCursor? mouseCursor { get; private set; }
    public virtual Color? activeColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? fillColor { get; private set; }
    public virtual Color? checkColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual double? splashRadius { get; private set; }
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual FocusNode? focusNode { get; private set; }
    public virtual WidgetStatesController? statesController { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual ShapeBorder? shape { get; private set; }
    public virtual BorderSide? side { get; private set; }
    public virtual bool isError { get; private set; } = default!;
    public virtual Color? tileColor { get; private set; }
    public virtual Widget? title { get; private set; }
    public virtual Widget? subtitle { get; private set; }
    public virtual Widget? secondary { get; private set; }
    public virtual bool? isThreeLine { get; private set; }
    public virtual bool? dense { get; private set; }
    public virtual bool selected { get; private set; } = default!;
    public virtual ListTileControlAffinity? controlAffinity { get; private set; }
    public virtual EdgeInsetsGeometry? contentPadding { get; private set; }
    public virtual bool tristate { get; private set; } = default!;
    public virtual OutlinedBorder? checkboxShape { get; private set; }
    public virtual Color? selectedTileColor { get; private set; }
    public virtual Action<bool>? onFocusChange { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual double? horizontalTitleGap { get; private set; }
    public virtual double? minVerticalPadding { get; private set; }
    public virtual double? minLeadingWidth { get; private set; }
    public virtual double? minTileHeight { get; private set; }
    public virtual bool? enabled { get; private set; }
    public virtual ListTileTitleAlignment? titleAlignment { get; private set; }
    public virtual bool internalAddSemanticForOnTap { get; private set; } = default!;
    public virtual double checkboxScaleFactor { get; private set; } = default!;
    public virtual string? checkboxSemanticLabel { get; private set; }
    internal virtual _CheckboxType__checkbox_list_tile _checkboxType { get; private set; } =
        default!;

    public CheckboxListTile(
        Key? key = null,
        bool? value = default!,
        Action<bool?>? onChanged = default!,
        MouseCursor? mouseCursor = null,
        Color? activeColor = null,
        WidgetStateProperty<Color?>? fillColor = null,
        Color? checkColor = null,
        Color? hoverColor = null,
        WidgetStateProperty<Color?>? overlayColor = null,
        double? splashRadius = null,
        MaterialTapTargetSize? materialTapTargetSize = null,
        VisualDensity? visualDensity = null,
        FocusNode? focusNode = null,
        WidgetStatesController? statesController = null,
        bool autofocus = false,
        ShapeBorder? shape = null,
        BorderSide? side = null,
        bool isError = false,
        bool? enabled = null,
        Color? tileColor = null,
        Widget? title = null,
        Widget? subtitle = null,
        bool? isThreeLine = null,
        bool? dense = null,
        Widget? secondary = null,
        bool selected = false,
        ListTileControlAffinity? controlAffinity = null,
        EdgeInsetsGeometry? contentPadding = null,
        bool tristate = false,
        OutlinedBorder? checkboxShape = null,
        Color? selectedTileColor = null,
        Action<bool>? onFocusChange = null,
        bool? enableFeedback = null,
        double? horizontalTitleGap = null,
        double? minVerticalPadding = null,
        double? minLeadingWidth = null,
        double? minTileHeight = null,
        string? checkboxSemanticLabel = null,
        double checkboxScaleFactor = 1.0,
        ListTileTitleAlignment? titleAlignment = null,
        bool internalAddSemanticForOnTap = false
    )
        : base(key: key)
    {
        this.value = value;
        this.onChanged = onChanged;
        this.mouseCursor = mouseCursor;
        this.activeColor = activeColor;
        this.fillColor = fillColor;
        this.checkColor = checkColor;
        this.hoverColor = hoverColor;
        this.overlayColor = overlayColor;
        this.splashRadius = splashRadius;
        this.materialTapTargetSize = materialTapTargetSize;
        this.visualDensity = visualDensity;
        this.focusNode = focusNode;
        this.statesController = statesController;
        this.autofocus = autofocus;
        this.shape = shape;
        this.side = side;
        this.isError = isError;
        this.enabled = enabled;
        this.tileColor = tileColor;
        this.title = title;
        this.subtitle = subtitle;
        this.isThreeLine = isThreeLine;
        this.dense = dense;
        this.secondary = secondary;
        this.selected = selected;
        this.controlAffinity = controlAffinity;
        this.contentPadding = contentPadding;
        this.tristate = tristate;
        this.checkboxShape = checkboxShape;
        this.selectedTileColor = selectedTileColor;
        this.onFocusChange = onFocusChange;
        this.enableFeedback = enableFeedback;
        this.horizontalTitleGap = horizontalTitleGap;
        this.minVerticalPadding = minVerticalPadding;
        this.minLeadingWidth = minLeadingWidth;
        this.minTileHeight = minTileHeight;
        this.checkboxSemanticLabel = checkboxSemanticLabel;
        this.checkboxScaleFactor = checkboxScaleFactor;
        this.titleAlignment = titleAlignment;
        this.internalAddSemanticForOnTap = internalAddSemanticForOnTap;
        _checkboxType = _CheckboxType__checkbox_list_tile.material;
        System.Diagnostics.Debug.Assert(tristate || (value is not null));
        System.Diagnostics.Debug.Assert((isThreeLine != true) || (subtitle is not null));
    }

    public static CheckboxListTile CreateAdaptive(
        Key? key = null,
        bool? value = default!,
        Action<bool?>? onChanged = default!,
        MouseCursor? mouseCursor = null,
        Color? activeColor = null,
        WidgetStateProperty<Color?>? fillColor = null,
        Color? checkColor = null,
        Color? hoverColor = null,
        WidgetStateProperty<Color?>? overlayColor = null,
        double? splashRadius = null,
        MaterialTapTargetSize? materialTapTargetSize = null,
        VisualDensity? visualDensity = null,
        FocusNode? focusNode = null,
        WidgetStatesController? statesController = null,
        bool autofocus = false,
        ShapeBorder? shape = null,
        BorderSide? side = null,
        bool isError = false,
        bool? enabled = null,
        Color? tileColor = null,
        Widget? title = null,
        Widget? subtitle = null,
        bool? isThreeLine = null,
        bool? dense = null,
        Widget? secondary = null,
        bool selected = false,
        ListTileControlAffinity? controlAffinity = null,
        EdgeInsetsGeometry? contentPadding = null,
        bool tristate = false,
        OutlinedBorder? checkboxShape = null,
        Color? selectedTileColor = null,
        Action<bool>? onFocusChange = null,
        bool? enableFeedback = null,
        double? horizontalTitleGap = null,
        double? minVerticalPadding = null,
        double? minLeadingWidth = null,
        double? minTileHeight = null,
        string? checkboxSemanticLabel = null,
        double checkboxScaleFactor = 1.0,
        ListTileTitleAlignment? titleAlignment = null,
        bool internalAddSemanticForOnTap = false
    )
    {
        var __instance = new CheckboxListTile(
            key: key,
            value: value,
            onChanged: onChanged,
            mouseCursor: mouseCursor,
            activeColor: activeColor,
            fillColor: fillColor,
            checkColor: checkColor,
            hoverColor: hoverColor,
            overlayColor: overlayColor,
            splashRadius: splashRadius,
            materialTapTargetSize: materialTapTargetSize,
            visualDensity: visualDensity,
            focusNode: focusNode,
            statesController: statesController,
            autofocus: autofocus,
            shape: shape,
            side: side,
            isError: isError,
            enabled: enabled,
            tileColor: tileColor,
            title: title,
            subtitle: subtitle,
            isThreeLine: isThreeLine,
            dense: dense,
            secondary: secondary,
            selected: selected,
            controlAffinity: controlAffinity,
            contentPadding: contentPadding,
            tristate: tristate,
            checkboxShape: checkboxShape,
            selectedTileColor: selectedTileColor,
            onFocusChange: onFocusChange,
            enableFeedback: enableFeedback,
            horizontalTitleGap: horizontalTitleGap,
            minVerticalPadding: minVerticalPadding,
            minLeadingWidth: minLeadingWidth,
            minTileHeight: minTileHeight,
            checkboxSemanticLabel: checkboxSemanticLabel,
            checkboxScaleFactor: checkboxScaleFactor,
            titleAlignment: titleAlignment,
            internalAddSemanticForOnTap: internalAddSemanticForOnTap
        );
        __instance.value = value;
        __instance.onChanged = onChanged;
        __instance.mouseCursor = mouseCursor;
        __instance.activeColor = activeColor;
        __instance.fillColor = fillColor;
        __instance.checkColor = checkColor;
        __instance.hoverColor = hoverColor;
        __instance.overlayColor = overlayColor;
        __instance.splashRadius = splashRadius;
        __instance.materialTapTargetSize = materialTapTargetSize;
        __instance.visualDensity = visualDensity;
        __instance.focusNode = focusNode;
        __instance.statesController = statesController;
        __instance.autofocus = autofocus;
        __instance.shape = shape;
        __instance.side = side;
        __instance.isError = isError;
        __instance.enabled = enabled;
        __instance.tileColor = tileColor;
        __instance.title = title;
        __instance.subtitle = subtitle;
        __instance.isThreeLine = isThreeLine;
        __instance.dense = dense;
        __instance.secondary = secondary;
        __instance.selected = selected;
        __instance.controlAffinity = controlAffinity;
        __instance.contentPadding = contentPadding;
        __instance.tristate = tristate;
        __instance.checkboxShape = checkboxShape;
        __instance.selectedTileColor = selectedTileColor;
        __instance.onFocusChange = onFocusChange;
        __instance.enableFeedback = enableFeedback;
        __instance.horizontalTitleGap = horizontalTitleGap;
        __instance.minVerticalPadding = minVerticalPadding;
        __instance.minLeadingWidth = minLeadingWidth;
        __instance.minTileHeight = minTileHeight;
        __instance.checkboxSemanticLabel = checkboxSemanticLabel;
        __instance.checkboxScaleFactor = checkboxScaleFactor;
        __instance.titleAlignment = titleAlignment;
        __instance.internalAddSemanticForOnTap = internalAddSemanticForOnTap;
        __instance._checkboxType = _CheckboxType__checkbox_list_tile.adaptive;
        return __instance;
    }

    internal virtual void _handleValueChange()
    {
        DartRuntimePrimitives.Assert(() => onChanged is not null);
        switch (value)
        {
            case false:
            {
                onChanged!(true);
                break;
            }
            case true:
            {
                onChanged!(tristate ? null : false);
                break;
            }
            case null:
            {
                onChanged!(false);
                break;
            }
        }
    }

    public override Widget build(BuildContext context)
    {
        Widget control = default!;
        switch (_checkboxType)
        {
            case _CheckboxType__checkbox_list_tile.material:
            {
                control = DartRuntimePrimitives.ConvertValue<Widget>(
                    new ExcludeFocus(
                        child: new Checkbox(
                            value: value,
                            onChanged: (enabled ?? true) ? onChanged : null,
                            mouseCursor: mouseCursor,
                            activeColor: activeColor,
                            fillColor: fillColor,
                            checkColor: checkColor,
                            hoverColor: hoverColor,
                            overlayColor: overlayColor,
                            splashRadius: splashRadius,
                            materialTapTargetSize: materialTapTargetSize
                                ?? MaterialTapTargetSize.shrinkWrap,
                            autofocus: autofocus,
                            tristate: tristate,
                            shape: checkboxShape,
                            side: side,
                            isError: isError,
                            semanticLabel: checkboxSemanticLabel
                        )
                    )
                );
                break;
            }
            case _CheckboxType__checkbox_list_tile.adaptive:
            {
                control = DartRuntimePrimitives.ConvertValue<Widget>(
                    new ExcludeFocus(
                        child: Checkbox.CreateAdaptive(
                            value: value,
                            onChanged: (enabled ?? true) ? onChanged : null,
                            mouseCursor: mouseCursor,
                            activeColor: activeColor,
                            fillColor: fillColor,
                            checkColor: checkColor,
                            hoverColor: hoverColor,
                            overlayColor: overlayColor,
                            splashRadius: splashRadius,
                            materialTapTargetSize: materialTapTargetSize
                                ?? MaterialTapTargetSize.shrinkWrap,
                            autofocus: autofocus,
                            tristate: tristate,
                            shape: checkboxShape,
                            side: side,
                            isError: isError,
                            semanticLabel: checkboxSemanticLabel
                        )
                    )
                );
                break;
            }
        }
        if (checkboxScaleFactor != 1.0)
        {
            control = DartRuntimePrimitives.ConvertValue<Widget>(
                Transform.CreateScale(scale: checkboxScaleFactor, child: control)
            );
        }
        ListTileThemeData listTileTheme = ListTileTheme.of(context);
        ListTileControlAffinity effectiveControlAffinity =
            (controlAffinity ?? listTileTheme.controlAffinity) ?? ListTileControlAffinity.platform;
        var (leadingLocal, trailingLocal) = effectiveControlAffinity switch
        {
            ListTileControlAffinity.leading => DartRuntimePrimitives.ConvertValue<(
                Widget?,
                Widget?
            )>((control, secondary)),
            ListTileControlAffinity.trailing => DartRuntimePrimitives.ConvertValue<(
                Widget?,
                Widget?
            )>((secondary, control)),
            ListTileControlAffinity.platform => DartRuntimePrimitives.ConvertValue<(
                Widget?,
                Widget?
            )>((secondary, control)),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        ThemeData theme = Theme.of(context);
        CheckboxThemeData checkboxTheme = CheckboxTheme.of(context);
        var states = (
            (Func<HashSet<WidgetState>>)(
                () =>
                {
                    var __collection21344 = new HashSet<WidgetState>();
                    if (selected)
                    {
                        __collection21344.Add(WidgetState.selected);
                    }
                    return __collection21344;
                }
            )
        )();
        Color effectiveActiveColor =
            (activeColor ?? (checkboxTheme.fillColor?.resolve(states)))
            ?? theme.colorScheme.secondary;
        return new MergeSemantics(
            child: new ListTile(
                selectedColor: effectiveActiveColor,
                leading: leadingLocal,
                title: title,
                subtitle: subtitle,
                trailing: trailingLocal,
                isThreeLine: isThreeLine,
                dense: dense,
                enabled: enabled ?? (onChanged is not null),
                onTap: (onChanged is not null) ? _handleValueChange : null,
                selected: selected,
                autofocus: autofocus,
                contentPadding: contentPadding,
                shape: shape,
                selectedTileColor: selectedTileColor,
                tileColor: tileColor,
                visualDensity: visualDensity,
                focusNode: focusNode,
                statesController: statesController,
                onFocusChange: onFocusChange,
                enableFeedback: enableFeedback,
                horizontalTitleGap: horizontalTitleGap,
                minVerticalPadding: minVerticalPadding,
                minLeadingWidth: minLeadingWidth,
                minTileHeight: minTileHeight,
                titleAlignment: titleAlignment,
                internalAddSemanticForOnTap: internalAddSemanticForOnTap
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
