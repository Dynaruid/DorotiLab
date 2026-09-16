// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/list_tile.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

internal delegate void _Sizes__list_tile();

internal delegate void _PositionChild__list_tile(global::Doroti.Framework.Rendering.RenderBox child, Offset offset);

public enum ListTileStyle
{
    list,
    drawer
}

public enum ListTileControlAffinity
{
    leading,
    trailing,
    platform
}

public enum ListTileTitleAlignment
{
    threeLine,
    titleHeight,
    top,
    center,
    bottom
}

public static class ListTileTitleAlignmentMembers
{
    internal static double _yOffsetFor(this ListTileTitleAlignment value, double childHeight, double tileHeight, _RenderListTile__list_tile listTile, bool isLeading)
    {
        return value switch { ListTileTitleAlignment.threeLine => listTile.isThreeLine ? ListTileTitleAlignment.top._yOffsetFor(childHeight, tileHeight, listTile, isLeading) : ListTileTitleAlignment.center._yOffsetFor(childHeight, tileHeight, listTile, isLeading), ListTileTitleAlignment.titleHeight when tileHeight > 72.0 => 16.0, ListTileTitleAlignment.titleHeight => isLeading ? Math.Min((tileHeight - childHeight) / 2.0, 16.0) : ((tileHeight - childHeight) / 2.0), ListTileTitleAlignment.top => listTile.minVerticalPadding, ListTileTitleAlignment.center => (tileHeight - childHeight) / 2.0, ListTileTitleAlignment.bottom => tileHeight - childHeight - listTile.minVerticalPadding, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class ListTile : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget? leading { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? title { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? subtitle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? trailing { get; private set; }
    public virtual bool? isThreeLine { get; private set; }
    public virtual bool? dense { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual Color? selectedColor { get; private set; }
    public virtual Color? iconColor { get; private set; }
    public virtual Color? textColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? titleTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? subtitleTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? leadingAndTrailingTextStyle { get; private set; }
    public virtual ListTileStyle? style { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding { get; private set; }
    public virtual bool enabled { get; private set; } = default!;
    public virtual global::System.Action? onTap { get; private set; }
    public virtual global::System.Action? onLongPress { get; private set; }
    public virtual global::System.Action<bool>? onFocusChange { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual bool selected { get; private set; } = default!;
    public virtual Color? focusColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual Color? splashColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual Color? tileColor { get; private set; }
    public virtual Color? selectedTileColor { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual double? horizontalTitleGap { get; private set; }
    public virtual double? minVerticalPadding { get; private set; }
    public virtual double? minLeadingWidth { get; private set; }
    public virtual double? minTileHeight { get; private set; }
    public virtual ListTileTitleAlignment? titleAlignment { get; private set; }
    public virtual bool internalAddSemanticForOnTap { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.WidgetStatesController? statesController { get; private set; }

    public ListTile(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget? leading = null, global::Doroti.Framework.Widgets.Widget? title = null, global::Doroti.Framework.Widgets.Widget? subtitle = null, global::Doroti.Framework.Widgets.Widget? trailing = null, bool? isThreeLine = null, bool? dense = null, VisualDensity? visualDensity = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, ListTileStyle? style = null, Color? selectedColor = null, Color? iconColor = null, Color? textColor = null, global::Doroti.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Framework.Painting.TextStyle? subtitleTextStyle = null, global::Doroti.Framework.Painting.TextStyle? leadingAndTrailingTextStyle = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, bool enabled = true, global::System.Action? onTap = null, global::System.Action? onLongPress = null, global::System.Action<bool>? onFocusChange = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, bool selected = false, Color? focusColor = null, Color? hoverColor = null, Color? splashColor = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, Color? tileColor = null, Color? selectedTileColor = null, bool? enableFeedback = null, double? horizontalTitleGap = null, double? minVerticalPadding = null, double? minLeadingWidth = null, double? minTileHeight = null, ListTileTitleAlignment? titleAlignment = null, bool internalAddSemanticForOnTap = true, global::Doroti.Framework.Widgets.WidgetStatesController? statesController = null) : base(key: key)
    {
        this.leading = leading;
        this.title = title;
        this.subtitle = subtitle;
        this.trailing = trailing;
        this.isThreeLine = isThreeLine;
        this.dense = dense;
        this.visualDensity = visualDensity;
        this.shape = shape;
        this.style = style;
        this.selectedColor = selectedColor;
        this.iconColor = iconColor;
        this.textColor = textColor;
        this.titleTextStyle = titleTextStyle;
        this.subtitleTextStyle = subtitleTextStyle;
        this.leadingAndTrailingTextStyle = leadingAndTrailingTextStyle;
        this.contentPadding = contentPadding;
        this.enabled = enabled;
        this.onTap = onTap;
        this.onLongPress = onLongPress;
        this.onFocusChange = onFocusChange;
        this.mouseCursor = mouseCursor;
        this.selected = selected;
        this.focusColor = focusColor;
        this.hoverColor = hoverColor;
        this.splashColor = splashColor;
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.tileColor = tileColor;
        this.selectedTileColor = selectedTileColor;
        this.enableFeedback = enableFeedback;
        this.horizontalTitleGap = horizontalTitleGap;
        this.minVerticalPadding = minVerticalPadding;
        this.minLeadingWidth = minLeadingWidth;
        this.minTileHeight = minTileHeight;
        this.titleAlignment = titleAlignment;
        this.internalAddSemanticForOnTap = internalAddSemanticForOnTap;
        this.statesController = statesController;
        System.Diagnostics.Debug.Assert((isThreeLine != true) || (subtitle is not null));
    }

    public static IEnumerable<global::Doroti.Framework.Widgets.Widget> divideTiles(global::Doroti.Framework.Widgets.BuildContext? context = null, IEnumerable<global::Doroti.Framework.Widgets.Widget> tiles = default!, Color? color = null)
    {
        DartRuntimePrimitives.Assert(() => (color is not null) || (context is not null));
        tiles = tiles.ToList();
        if (!Enumerable.Any(tiles) || (tiles.Count() == 1L))
        {
            return tiles;
        }
        global::Doroti.Framework.Widgets.Widget wrapTile(global::Doroti.Framework.Widgets.Widget tile)
        {
            return new global::Doroti.Framework.Widgets.DecoratedBox(position: DecorationPosition.foreground, decoration: new global::Doroti.Framework.Painting.BoxDecoration(border: new global::Doroti.Framework.Painting.Border(bottom: Divider.createBorderSide(context, color: color))), child: tile);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        return ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection30919 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection30919.AddRange(tiles.take(tiles.Count() - 1L).map<global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Widget>(wrapTile)); __collection30919.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(tiles.Last())); return __collection30919; }))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isDenseLayout(ThemeData theme, ListTileThemeData tileTheme)
    {
        return ((dense ?? tileTheme.dense) ?? theme.listTileTheme.dense) ?? false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        ThemeData theme = Theme.of(context);
        IconButtonThemeData iconButtonTheme = IconButtonTheme.of(context);
        ListTileThemeData tileTheme = ListTileTheme.of(context);
        ListTileStyle listTileStyle = ((style ?? tileTheme.style) ?? theme.listTileTheme.style) ?? ListTileStyle.list;
        ListTileThemeData defaults = new _LisTileDefaultsM3__list_tile(context);
        global::Doroti.Ui.Color backgroundColor = ((tileColor ?? tileTheme.tileColor) ?? theme.listTileTheme.tileColor) ?? defaults.tileColor ?? new global::Doroti.Ui.Color(0L);
        global::Doroti.Ui.Color selectedBackgroundColor = ((selectedTileColor ?? tileTheme.selectedTileColor) ?? theme.listTileTheme.selectedTileColor) ?? defaults.tileColor ?? new global::Doroti.Ui.Color(0L);
        var effectiveTileColor = selected ? selectedBackgroundColor : backgroundColor;
        bool hasOpaqueBackground = (backgroundColor.alpha > 0L) || (selectedBackgroundColor.alpha > 0L);
        if ((onTap is not null) || (onLongPress is not null) || hasOpaqueBackground)
        {
            DartRuntimePrimitives.Assert(() => _debugCheckBackgroundIsHidden(context));
        }
        var states = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection32361 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (!enabled) { __collection32361.Add(WidgetState.disabled); } if (selected) { __collection32361.Add(WidgetState.selected); } return __collection32361; }))();
        Color? resolveColor(Color? explicitColor, Color? selectedColor, Color? enabledColor, Color? disabledColor = null)
        {
            return new _IndividualOverrides__list_tile(explicitColor: explicitColor, selectedColor: selectedColor, enabledColor: enabledColor, disabledColor: disabledColor).resolve(states);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::Doroti.Ui.Color? effectiveIconColor = (resolveColor(iconColor, selectedColor, iconColor) ?? resolveColor(tileTheme.iconColor, tileTheme.selectedColor, tileTheme.iconColor)) ?? resolveColor(theme.listTileTheme.iconColor, theme.listTileTheme.selectedColor, theme.listTileTheme.iconColor);
        global::Doroti.Ui.Color? defaultEffectiveIconColor = resolveColor(defaults.iconColor, defaults.selectedColor, defaults.iconColor, theme.disabledColor);
        global::Doroti.Ui.Color? effectiveIconButtonColor = (effectiveIconColor ?? (iconButtonTheme.style?.foregroundColor?.resolve(states))) ?? defaultEffectiveIconColor;
        effectiveIconColor ??= defaultEffectiveIconColor;
        global::Doroti.Ui.Color? effectiveColor = ((resolveColor(textColor, selectedColor, textColor) ?? resolveColor(tileTheme.textColor, tileTheme.selectedColor, tileTheme.textColor)) ?? resolveColor(theme.listTileTheme.textColor, theme.listTileTheme.selectedColor, theme.listTileTheme.textColor)) ?? resolveColor(defaults.textColor, defaults.selectedColor, defaults.textColor, theme.disabledColor);
        var iconThemeData = new global::Doroti.Framework.Widgets.IconThemeData(color: effectiveIconColor);
        var iconButtonThemeData = new IconButtonThemeData(style: IconButtonTheme.of(context).style?.copyWith(foregroundColor: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color?>(effectiveIconButtonColor)) ?? IconButton.styleFrom(foregroundColor: effectiveIconButtonColor));
        global::Doroti.Framework.Painting.TextStyle? leadingAndTrailingStyle = default!;
        if ((leading is not null) || (trailing is not null))
        {
            leadingAndTrailingStyle = (leadingAndTrailingTextStyle ?? tileTheme.leadingAndTrailingTextStyle) ?? defaults.leadingAndTrailingTextStyle!;
            var leadingAndTrailingTextColor = effectiveColor;
            leadingAndTrailingStyle = leadingAndTrailingStyle.copyWith(color: leadingAndTrailingTextColor);
        }
        global::Doroti.Framework.Widgets.Widget? leadingIcon = default!;
        if (leading is not null)
        {
            leadingIcon = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.AnimatedDefaultTextStyle(style: leadingAndTrailingStyle!, duration: ConstantsLibrary.kThemeChangeDuration, child: leading!));
        }
        global::Doroti.Framework.Painting.TextStyle titleStyle = (titleTextStyle ?? tileTheme.titleTextStyle) ?? defaults.titleTextStyle!;
        var titleColor = effectiveColor;
        titleStyle = titleStyle.copyWith(color: titleColor, fontSize: _isDenseLayout(theme, tileTheme) ? 13.0 : null);
        global::Doroti.Framework.Widgets.Widget titleText = new global::Doroti.Framework.Widgets.AnimatedDefaultTextStyle(style: titleStyle, duration: ConstantsLibrary.kThemeChangeDuration, child: title ?? new global::Doroti.Framework.Widgets.SizedBox());
        global::Doroti.Framework.Widgets.Widget? subtitleText = default!;
        global::Doroti.Framework.Painting.TextStyle? subtitleStyle = default!;
        if (subtitle is not null)
        {
            subtitleStyle = (subtitleTextStyle ?? tileTheme.subtitleTextStyle) ?? defaults.subtitleTextStyle!;
            var subtitleColor = effectiveColor;
            subtitleStyle = subtitleStyle.copyWith(color: subtitleColor, fontSize: _isDenseLayout(theme, tileTheme) ? 12.0 : null);
            subtitleText = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.AnimatedDefaultTextStyle(style: subtitleStyle, duration: ConstantsLibrary.kThemeChangeDuration, child: subtitle!));
        }
        global::Doroti.Framework.Widgets.Widget? trailingIcon = default!;
        if (trailing is not null)
        {
            trailingIcon = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.AnimatedDefaultTextStyle(style: leadingAndTrailingStyle!, duration: ConstantsLibrary.kThemeChangeDuration, child: trailing!));
        }
        global::Doroti.Ui.TextDirection textDirectionLocal = Directionality.of(context);
        global::Doroti.Framework.Painting.EdgeInsets resolvedContentPadding = (contentPadding?.resolve(textDirectionLocal) ?? (tileTheme.contentPadding?.resolve(textDirectionLocal))) ?? defaults.contentPadding!.resolve(textDirectionLocal);
        var mouseStates = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection36732 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (!enabled || (onTap is null) && (onLongPress is null)) { __collection36732.Add(WidgetState.disabled); } return __collection36732; }))();
        global::Doroti.Framework.Services.MouseCursor effectiveMouseCursor = (WidgetStateProperty.resolveAs<global::Doroti.Framework.Services.MouseCursor?>(mouseCursor, mouseStates) ?? (tileTheme.mouseCursor?.resolve(mouseStates))) ?? WidgetStateMouseCursor.clickable.resolve(mouseStates);
        ListTileTitleAlignment effectiveTitleAlignment = (titleAlignment ?? tileTheme.titleAlignment) ?? ListTileTitleAlignment.threeLine;
        return new InkWell(customBorder: shape ?? tileTheme.shape, onTap: enabled ? onTap : null, onLongPress: enabled ? onLongPress : null, onFocusChange: onFocusChange, mouseCursor: effectiveMouseCursor, canRequestFocus: enabled, focusNode: focusNode, focusColor: focusColor, hoverColor: hoverColor, splashColor: splashColor, autofocus: autofocus, enableFeedback: (enableFeedback ?? tileTheme.enableFeedback) ?? true, statesController: statesController, child: new global::Doroti.Framework.Widgets.Semantics(button: internalAddSemanticForOnTap && ((onTap is not null) || (onLongPress is not null)), selected: selected, enabled: enabled, child: new Ink(decoration: new global::Doroti.Framework.Painting.ShapeDecoration(shape: (shape ?? tileTheme.shape) ?? new global::Doroti.Framework.Painting.Border(), color: effectiveTileColor), child: new global::Doroti.Framework.Widgets.SafeArea(top: false, bottom: false, minimum: resolvedContentPadding, child: IconTheme.merge(data: iconThemeData, child: new IconButtonTheme(data: iconButtonThemeData, child: new _ListTile__list_tile(leading: leadingIcon, title: titleText, subtitle: subtitleText, trailing: trailingIcon, isDense: _isDenseLayout(theme, tileTheme), visualDensity: (visualDensity ?? tileTheme.visualDensity) ?? theme.visualDensity, isThreeLine: ((isThreeLine ?? tileTheme.isThreeLine) ?? theme.listTileTheme.isThreeLine) ?? false, textDirection: textDirectionLocal, titleBaselineType: titleStyle.textBaseline ?? DartRuntimePrimitives.RequireValue(defaults.titleTextStyle!.textBaseline), subtitleBaselineType: subtitleStyle?.textBaseline ?? DartRuntimePrimitives.RequireValue(defaults.subtitleTextStyle!.textBaseline), horizontalTitleGap: (horizontalTitleGap ?? tileTheme.horizontalTitleGap) ?? 16, minVerticalPadding: (minVerticalPadding ?? tileTheme.minVerticalPadding) ?? DartRuntimePrimitives.RequireValue(defaults.minVerticalPadding), minLeadingWidth: (minLeadingWidth ?? tileTheme.minLeadingWidth) ?? DartRuntimePrimitives.RequireValue(defaults.minLeadingWidth), minTileHeight: minTileHeight ?? tileTheme.minTileHeight, titleAlignment: effectiveTitleAlignment)))))));
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("isThreeLine", value: isThreeLine, ifTrue: "THREE_LINE", ifFalse: "TWO_LINE", showName: true));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("dense", value: dense, ifTrue: "true", ifFalse: "false", showName: true));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<VisualDensity>("visualDensity", visualDensity, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ShapeBorder>("shape", shape, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ListTileStyle>("style", style, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("selectedColor", selectedColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("iconColor", iconColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("textColor", textColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("titleTextStyle", titleTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("subtitleTextStyle", subtitleTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("leadingAndTrailingTextStyle", leadingAndTrailingTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("contentPadding", contentPadding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("enabled", value: enabled, ifTrue: "true", ifFalse: "false", showName: true, defaultValue: true));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Delegate>("onTap", onTap, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<Delegate>("onLongPress", onLongPress, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Services.MouseCursor>("mouseCursor", mouseCursor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("selected", value: selected, ifTrue: "true", ifFalse: "false", showName: true, defaultValue: false));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("focusColor", focusColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("hoverColor", hoverColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.FocusNode>("focusNode", focusNode, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("autofocus", value: autofocus, ifTrue: "true", ifFalse: "false", showName: true, defaultValue: false));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("tileColor", tileColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("selectedTileColor", selectedTileColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("enableFeedback", value: enableFeedback, ifTrue: "true", ifFalse: "false", showName: true));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("horizontalTitleGap", horizontalTitleGap, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("minVerticalPadding", minVerticalPadding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("minLeadingWidth", minLeadingWidth, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ListTileTitleAlignment>("titleAlignment", titleAlignment, defaultValue: null));
    }

    internal virtual bool _debugCheckBackgroundIsHidden(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                global::Doroti.Framework.Widgets.Widget? intermediateWidget = _findIntermediateWidget(context);
                if (intermediateWidget is not null)
                {
                    FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("ListTile background color or ink splashes may be invisible."), new global::Doroti.Framework.Foundation.ErrorDescription($"The ListTile is wrapped in a {DartRuntimePrimitives.RuntimeType(intermediateWidget)} that has a background color. " + "Because ListTile paints its background and ink splashes on the nearest Material ancestor, " + $"this {DartRuntimePrimitives.RuntimeType(intermediateWidget)} will hide those effects."), new global::Doroti.Framework.Foundation.ErrorHint("To fix this, wrap the ListTile in its own Material widget, " + $"or remove the background color from the intermediate {DartRuntimePrimitives.RuntimeType(intermediateWidget)}.") }), informationCollector: () => new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.DiagnosticsProperty<ListTile>("ListTile", this, expandableValue: true), new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.Widget>($"{DartRuntimePrimitives.RuntimeType(intermediateWidget)}", intermediateWidget, expandableValue: true) }));
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget? _findIntermediateWidget(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Widgets.Widget? intermediateWidget = default!;
        ((global::Doroti.Framework.Widgets.Element?)context)!.visitAncestorElements((ancestor) =>
        {
            if (ancestor.widget is Material)
            {
                return false;
            }
            global::Doroti.Framework.Widgets.Widget widgetLocal = ancestor.widget;
            global::Doroti.Ui.Color? colorLocal = (global::Doroti.Ui.Color?)(widgetLocal switch { global::Doroti.Framework.Widgets.ColoredBox { color: global::Doroti.Ui.Color colorAlternate } __object45267 => colorAlternate, global::Doroti.Framework.Widgets.DecoratedBox { decoration: global::Doroti.Framework.Painting.BoxDecoration { color: global::Doroti.Ui.Color colorNested } __object45341 } __object45316 => colorNested, global::Doroti.Framework.Widgets.DecoratedBox { decoration: global::Doroti.Framework.Painting.ShapeDecoration { color: global::Doroti.Ui.Color colorCurrent } __object45420 } __object45395 => colorCurrent, _ => DartRuntimePrimitives.ConvertValue<Color>(null) });
            if ((colorLocal is not null) && (colorLocal.a > 0L))
            {
                intermediateWidget = widgetLocal;
                return false;
            }
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        return intermediateWidget;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _IndividualOverrides__list_tile : global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>
{
    public virtual Color? explicitColor { get; private set; }
    public virtual Color? enabledColor { get; private set; }
    public virtual Color? selectedColor { get; private set; }
    public virtual Color? disabledColor { get; private set; }

    internal _IndividualOverrides__list_tile(Color? explicitColor = null, Color? enabledColor = null, Color? selectedColor = null, Color? disabledColor = null)
    {
        this.explicitColor = explicitColor;
        this.enabledColor = enabledColor;
        this.selectedColor = selectedColor;
        this.disabledColor = disabledColor;
    }

    public virtual Color? resolve(HashSet<global::Doroti.Framework.Widgets.WidgetState> states)
    {
        if (explicitColor is global::Doroti.Framework.Widgets.WidgetStateColor)
        {
            global::Doroti.Framework.Widgets.WidgetStateColor explicitColor__as46046 = (global::Doroti.Framework.Widgets.WidgetStateColor)explicitColor;
            return WidgetStateProperty.resolveAs<global::Doroti.Ui.Color?>(explicitColor, states);
        }
        if (states.Contains(WidgetState.disabled))
        {
            return disabledColor;
        }
        if (states.Contains(WidgetState.selected))
        {
            return selectedColor;
        }
        return enabledColor;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public enum _ListTileSlot__list_tile
{
    leading,
    title,
    subtitle,
    trailing
}

internal class _ListTile__list_tile : global::Doroti.Framework.Widgets.SlottedMultiChildRenderObjectWidget<_ListTileSlot__list_tile, global::Doroti.Framework.Rendering.RenderBox>
{
    public virtual global::Doroti.Framework.Widgets.Widget? leading { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget title { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? subtitle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? trailing { get; private set; }
    public virtual bool isThreeLine { get; private set; } = default!;
    public virtual bool isDense { get; private set; } = default!;
    public virtual VisualDensity visualDensity { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;
    public virtual TextBaseline titleBaselineType { get; private set; } = default!;
    public virtual TextBaseline? subtitleBaselineType { get; private set; }
    public virtual double horizontalTitleGap { get; private set; } = default!;
    public virtual double minVerticalPadding { get; private set; } = default!;
    public virtual double minLeadingWidth { get; private set; } = default!;
    public virtual double? minTileHeight { get; private set; }
    public virtual ListTileTitleAlignment titleAlignment { get; private set; } = default!;

    internal _ListTile__list_tile(global::Doroti.Framework.Widgets.Widget? leading = null, global::Doroti.Framework.Widgets.Widget title = default!, global::Doroti.Framework.Widgets.Widget? subtitle = null, global::Doroti.Framework.Widgets.Widget? trailing = null, bool isThreeLine = default!, bool isDense = default!, VisualDensity visualDensity = default!, TextDirection textDirection = default!, TextBaseline titleBaselineType = default!, double horizontalTitleGap = default!, double minVerticalPadding = default!, double minLeadingWidth = default!, double? minTileHeight = null, TextBaseline? subtitleBaselineType = null, ListTileTitleAlignment titleAlignment = default!)
    {
        this.leading = leading;
        this.title = title;
        this.subtitle = subtitle;
        this.trailing = trailing;
        this.isThreeLine = isThreeLine;
        this.isDense = isDense;
        this.visualDensity = visualDensity;
        this.textDirection = textDirection;
        this.titleBaselineType = titleBaselineType;
        this.horizontalTitleGap = horizontalTitleGap;
        this.minVerticalPadding = minVerticalPadding;
        this.minLeadingWidth = minLeadingWidth;
        this.minTileHeight = minTileHeight;
        this.subtitleBaselineType = subtitleBaselineType;
        this.titleAlignment = titleAlignment;
    }

    public override IEnumerable<_ListTileSlot__list_tile> slots => DartRuntimePrimitives.ConvertValue<IEnumerable<_ListTileSlot__list_tile>>(Enum.GetValues<_ListTileSlot__list_tile>().ToList());
    public override global::Doroti.Framework.Widgets.Widget? childForSlot(_ListTileSlot__list_tile slot)
    {
        return slot switch { _ListTileSlot__list_tile.leading => leading, _ListTileSlot__list_tile.title => title, _ListTileSlot__list_tile.subtitle => subtitle, _ListTileSlot__list_tile.trailing => trailing, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new _RenderListTile__list_tile(isThreeLine: isThreeLine, isDense: isDense, visualDensity: visualDensity, textDirection: textDirection, titleBaselineType: titleBaselineType, subtitleBaselineType: subtitleBaselineType, horizontalTitleGap: horizontalTitleGap, minVerticalPadding: minVerticalPadding, minLeadingWidth: minLeadingWidth, minTileHeight: minTileHeight, titleAlignment: titleAlignment);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderListTile__list_tile)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderListTile__list_tile>)(() =>
{
    var __cascade = __renderObject;
    __cascade.isThreeLine = isThreeLine;
    __cascade.isDense = isDense;
    __cascade.visualDensity = visualDensity;
    __cascade.textDirection = textDirection;
    __cascade.titleBaselineType = titleBaselineType;
    __cascade.subtitleBaselineType = subtitleBaselineType;
    __cascade.horizontalTitleGap = horizontalTitleGap;
    __cascade.minLeadingWidth = minLeadingWidth;
    __cascade.minTileHeight = minTileHeight;
    __cascade.minVerticalPadding = minVerticalPadding;
    __cascade.titleAlignment = titleAlignment;
    return __cascade;
}))());
    }

}

public class _RenderListTile__list_tile : global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Widgets.SlottedContainerRenderObjectMixin<_ListTileSlot__list_tile, global::Doroti.Framework.Rendering.RenderBox>
{
    internal virtual bool _isDense { get; set; } = default!;
    internal virtual VisualDensity _visualDensity { get; set; } = default!;
    internal virtual bool _isThreeLine { get; set; } = default!;
    internal virtual TextDirection _textDirection { get; set; } = default!;
    internal virtual TextBaseline _titleBaselineType { get; set; } = default!;
    internal virtual TextBaseline? _subtitleBaselineType { get; set; } = default;
    internal virtual double _horizontalTitleGap { get; set; } = default!;
    internal virtual double _minVerticalPadding { get; set; } = default!;
    internal virtual double _minLeadingWidth { get; set; } = default!;
    internal virtual double? _minTileHeight { get; set; } = default;
    internal virtual ListTileTitleAlignment _titleAlignment { get; set; } = default!;
    public virtual DartMap<_ListTileSlot__list_tile, global::Doroti.Framework.Rendering.RenderBox> _slotToChild { get; set; } = new DartMap<_ListTileSlot__list_tile, global::Doroti.Framework.Rendering.RenderBox>();

    internal _RenderListTile__list_tile(bool isDense, VisualDensity visualDensity, bool isThreeLine, TextDirection textDirection, TextBaseline titleBaselineType, TextBaseline? subtitleBaselineType = null, double horizontalTitleGap = default!, double minVerticalPadding = default!, double minLeadingWidth = default!, double? minTileHeight = null, ListTileTitleAlignment titleAlignment = default!)
    {
        _isDense = isDense;
        _visualDensity = visualDensity;
        _isThreeLine = isThreeLine;
        _textDirection = textDirection;
        _titleBaselineType = titleBaselineType;
        _subtitleBaselineType = subtitleBaselineType;
        _horizontalTitleGap = horizontalTitleGap;
        _minVerticalPadding = minVerticalPadding;
        _minLeadingWidth = minLeadingWidth;
        _minTileHeight = minTileHeight;
        _titleAlignment = titleAlignment;
    }

    public virtual global::Doroti.Framework.Rendering.RenderBox? leading => childForSlot(DartRuntimePrimitives.RequireValue(_ListTileSlot__list_tile.leading));
    public virtual global::Doroti.Framework.Rendering.RenderBox title => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderBox>(childForSlot(_ListTileSlot__list_tile.title)!);
    public virtual global::Doroti.Framework.Rendering.RenderBox? subtitle => childForSlot(DartRuntimePrimitives.RequireValue(_ListTileSlot__list_tile.subtitle));
    public virtual global::Doroti.Framework.Rendering.RenderBox? trailing => childForSlot(DartRuntimePrimitives.RequireValue(_ListTileSlot__list_tile.trailing));
    public virtual IEnumerable<global::Doroti.Framework.Rendering.RenderBox> children
    {
        get
        {
            global::Doroti.Framework.Rendering.RenderBox? titleLocal = childForSlot(_ListTileSlot__list_tile.title);
            return ((Func<List<global::Doroti.Framework.Rendering.RenderBox>>)(() => { var __collection50404 = new List<global::Doroti.Framework.Rendering.RenderBox>(); var __collectionElement50416 = leading; if (__collectionElement50416 is { } __nonNullCollectionElement50416) { __collection50404.Add(__nonNullCollectionElement50416); } var __collectionElement50426 = titleLocal; if (__collectionElement50426 is { } __nonNullCollectionElement50426) { __collection50404.Add(__nonNullCollectionElement50426); } var __collectionElement50434 = subtitle; if (__collectionElement50434 is { } __nonNullCollectionElement50434) { __collection50404.Add(__nonNullCollectionElement50434); } var __collectionElement50445 = trailing; if (__collectionElement50445 is { } __nonNullCollectionElement50445) { __collection50404.Add(__nonNullCollectionElement50445); } return __collection50404; }))();
        }
    }
    public virtual bool isDense
    {
        get => _isDense;
        set
        {
            var __value = value;
            if (_isDense == DartRuntimePrimitives.RequireValue(__value))
            {
                return;
            }
            _isDense = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual VisualDensity visualDensity
    {
        get => _visualDensity;
        set
        {
            var __value = value;
            if (Equals(_visualDensity, __value))
            {
                return;
            }
            _visualDensity = __value;
            markNeedsLayout();
        }
    }
    public virtual bool isThreeLine
    {
        get => _isThreeLine;
        set
        {
            var __value = value;
            if (_isThreeLine == DartRuntimePrimitives.RequireValue(__value))
            {
                return;
            }
            _isThreeLine = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.TextDirection textDirection
    {
        get => _textDirection;
        set
        {
            var __value = value;
            if (Equals(_textDirection, DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _textDirection = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.TextBaseline titleBaselineType
    {
        get => _titleBaselineType;
        set
        {
            var __value = value;
            if (Equals(_titleBaselineType, DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _titleBaselineType = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.TextBaseline? subtitleBaselineType
    {
        get => _subtitleBaselineType;
        set
        {
            var __value = value;
            if (Equals(_subtitleBaselineType, __value))
            {
                return;
            }
            _subtitleBaselineType = __value;
            markNeedsLayout();
        }
    }
    public virtual double horizontalTitleGap
    {
        get => _horizontalTitleGap;
        set
        {
            var __value = value;
            if (_horizontalTitleGap == DartRuntimePrimitives.RequireValue(__value))
            {
                return;
            }
            _horizontalTitleGap = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    internal virtual double _effectiveHorizontalTitleGap => DartRuntimePrimitives.ConvertValue<double>(_horizontalTitleGap + (visualDensity.horizontal * 2.0));
    public virtual double minVerticalPadding
    {
        get => _minVerticalPadding;
        set
        {
            var __value = value;
            if (_minVerticalPadding == DartRuntimePrimitives.RequireValue(__value))
            {
                return;
            }
            _minVerticalPadding = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual double minLeadingWidth
    {
        get => _minLeadingWidth;
        set
        {
            var __value = value;
            if (_minLeadingWidth == DartRuntimePrimitives.RequireValue(__value))
            {
                return;
            }
            _minLeadingWidth = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual double? minTileHeight
    {
        get => _minTileHeight;
        set
        {
            var __value = value;
            if (_minTileHeight == __value)
            {
                return;
            }
            _minTileHeight = __value;
            markNeedsLayout();
        }
    }
    public virtual ListTileTitleAlignment titleAlignment
    {
        get => _titleAlignment;
        set
        {
            var __value = value;
            if (Equals(_titleAlignment, DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _titleAlignment = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public override bool sizedByParent => false;
    internal static double _minWidth(global::Doroti.Framework.Rendering.RenderBox? box, double height)
    {
        return (box is null) ? 0.0 : box.getMinIntrinsicWidth(height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _maxWidth(global::Doroti.Framework.Rendering.RenderBox? box, double height)
    {
        return (box is null) ? 0.0 : box.getMaxIntrinsicWidth(height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        double leadingWidth = (leading is not null) ? (Math.Max(leading!.getMinIntrinsicWidth(height), _minLeadingWidth) + _effectiveHorizontalTitleGap) : 0.0;
        return leadingWidth + Math.Max(_minWidth(title, height), _minWidth(subtitle, height)) + _maxWidth(trailing, height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        double leadingWidth = (leading is not null) ? (Math.Max(leading!.getMaxIntrinsicWidth(height), _minLeadingWidth) + _effectiveHorizontalTitleGap) : 0.0;
        return leadingWidth + Math.Max(_maxWidth(title, height), _maxWidth(subtitle, height)) + _maxWidth(trailing, height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _defaultTileHeight
    {
        get
        {
            global::Doroti.Ui.Offset baseDensity = visualDensity.baseSizeAdjustment;
            return baseDensity.dy + ((isThreeLine, subtitle is not null) switch { (true, _) => isDense ? 76.0 : 88.0, (false, true) => isDense ? 64.0 : 72.0, (false, false) => isDense ? 48.0 : 56.0 });
        }
    }
    internal virtual double _targetTileHeight => DartRuntimePrimitives.ConvertValue<double>(_minTileHeight ?? (double)_defaultTileHeight);
    public override double computeMinIntrinsicHeight(double width)
    {
        double titleMinHeight = title.getMinIntrinsicHeight(width);
        double? subtitleMinHeight = subtitle?.getMinIntrinsicHeight(width);
        var topAndBottomPaddingMultiplier = 2L;
        double contentHeight = titleMinHeight + (subtitleMinHeight ?? 0.0) + (topAndBottomPaddingMultiplier * _minVerticalPadding);
        return Math.Max(_targetTileHeight, contentHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return getMinIntrinsicHeight(width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        var parentDataLocal = ((global::Doroti.Framework.Rendering.BoxParentData?)title.parentData!)!;
        global::Doroti.Framework.Rendering.BaselineOffset offsetLocal = new global::Doroti.Framework.Rendering.BaselineOffset(title.getDistanceToActualBaseline(baseline)).op_Add(parentDataLocal.offset.dy);
        return offsetLocal.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Rendering.BoxConstraints maxIconHeightConstraint => new global::Doroti.Framework.Rendering.BoxConstraints(maxHeight: (isDense ? 48.0 : 56.0) + visualDensity.baseSizeAdjustment.dy);
    internal static void _positionBox(global::Doroti.Framework.Rendering.RenderBox box, Offset offset)
    {
        var parentDataLocal = ((global::Doroti.Framework.Rendering.BoxParentData?)box.parentData!)!;
        parentDataLocal.offset = offset;
    }

    internal virtual (global::Doroti.Framework.Rendering.BoxConstraints textConstraints, Size tileSize, double titleY) _computeSizes(global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, TextBaseline, double?> getBaseline, global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size> getSize, global::Doroti.Framework.Rendering.BoxConstraints constraints, global::System.Action<global::Doroti.Framework.Rendering.RenderBox, Offset>? positionChild = null)
    {
        global::Doroti.Framework.Rendering.BoxConstraints looseConstraints = constraints.loosen();
        double tileWidth = looseConstraints.maxWidth;
        global::Doroti.Framework.Rendering.BoxConstraints iconConstraints = looseConstraints.enforce(maxIconHeightConstraint);
        global::Doroti.Framework.Rendering.RenderBox? leadingLocal = leading;
        global::Doroti.Framework.Rendering.RenderBox? trailingLocal = trailing;
        global::Doroti.Ui.Size? leadingSize = (leadingLocal is null) ? null : getSize(leadingLocal, iconConstraints);
        global::Doroti.Ui.Size? trailingSize = (trailingLocal is null) ? null : getSize(trailingLocal, iconConstraints);
        DartRuntimePrimitives.Assert(() =>
            {
                if (tileWidth == 0.0)
                {
                    return true;
                }
                string? overflowedWidget = default!;
                if (tileWidth == leadingSize?.width)
                {
                    overflowedWidget = "Leading";
                }
                else
                {
                    if (tileWidth == trailingSize?.width)
                    {
                        overflowedWidget = "Trailing";
                    }
                }
                if (overflowedWidget is null)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{overflowedWidget} widget consumes the entire tile width (including ListTile.contentPadding)."), new global::Doroti.Framework.Foundation.ErrorDescription($"Either resize the tile width so that the {overflowedWidget.toLowerCase()} widget plus any content padding " + "do not exceed the tile width, or use a sized widget, or consider replacing " + "ListTile with a custom widget."), new global::Doroti.Framework.Foundation.ErrorHint("See also: https://api.flutter.dev/flutter/material/ListTile-class.html#material.ListTile.4") }));
            });
        double titleStart = (leadingSize is null) ? 0.0 : (Math.Max(_minLeadingWidth, DartRuntimePrimitives.RequireValue(leadingSize).width) + _effectiveHorizontalTitleGap);
        double adjustedTrailingWidth = (trailingSize is null) ? 0.0 : Math.Max(DartRuntimePrimitives.RequireValue(trailingSize).width + _effectiveHorizontalTitleGap, 32.0);
        global::Doroti.Framework.Rendering.BoxConstraints textConstraintsLocal = looseConstraints.tighten(width: tileWidth - titleStart - adjustedTrailingWidth);
        global::Doroti.Framework.Rendering.RenderBox? subtitleLocal = subtitle;
        double titleHeight = getSize(title, textConstraintsLocal).height;
        bool isLTR = textDirection switch { TextDirection.ltr => true, TextDirection.rtl => false, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        double titleYLocal = default!;
        double tileHeight = default!;
        if (subtitleLocal is null)
        {
            tileHeight = Math.Max(_targetTileHeight, titleHeight + (2.0 * _minVerticalPadding));
            titleYLocal = (tileHeight - titleHeight) / 2.0;
        }
        else
        {
            double subtitleHeight = getSize(subtitleLocal, textConstraintsLocal).height;
            double titleBaseline = getBaseline(title, textConstraintsLocal, titleBaselineType) ?? titleHeight;
            double subtitleBaseline = getBaseline(subtitleLocal, textConstraintsLocal, DartRuntimePrimitives.RequireValue(subtitleBaselineType)) ?? subtitleHeight;
            double targetTitleY = (isThreeLine ? (isDense ? 22.0 : 28.0) : (isDense ? 28.0 : 32.0)) - titleBaseline;
            double targetSubtitleY = (isThreeLine ? (isDense ? 42.0 : 48.0) : (isDense ? 48.0 : 52.0)) + (visualDensity.vertical * 2.0) - subtitleBaseline;
            double halfOverlap = Math.Max(targetTitleY + titleHeight - targetSubtitleY, 0L) / 2L;
            double idealTitleY = targetTitleY - halfOverlap;
            double idealSubtitleY = targetSubtitleY + halfOverlap;
            bool compact = (idealTitleY < minVerticalPadding) || ((idealSubtitleY + subtitleHeight + minVerticalPadding) > _targetTileHeight);
            positionChild?.Invoke(subtitleLocal, new global::Doroti.Ui.Offset(isLTR ? titleStart : adjustedTrailingWidth, compact ? (minVerticalPadding + titleHeight) : idealSubtitleY));
            tileHeight = compact ? ((2L * _minVerticalPadding) + titleHeight + subtitleHeight) : _targetTileHeight;
            titleYLocal = compact ? minVerticalPadding : idealTitleY;
        }
        if (positionChild is not null)
        {
            positionChild(title, new global::Doroti.Ui.Offset(isLTR ? titleStart : adjustedTrailingWidth, titleYLocal));
            if ((leadingLocal is not null) && (leadingSize is not null))
            {
                Size leadingSize__57061__value61002 = DartRuntimePrimitives.RequireValue(leadingSize);
                positionChild(leadingLocal, new global::Doroti.Ui.Offset(isLTR ? 0.0 : (tileWidth - DartRuntimePrimitives.RequireValue(leadingSize__57061__value61002).width), titleAlignment._yOffsetFor(DartRuntimePrimitives.RequireValue(leadingSize__57061__value61002).height, tileHeight, this, true)));
            }
            if ((trailingLocal is not null) && (trailingSize is not null))
            {
                Size trailingSize__57151__value61289 = DartRuntimePrimitives.RequireValue(trailingSize);
                positionChild(trailingLocal, new global::Doroti.Ui.Offset(isLTR ? (tileWidth - DartRuntimePrimitives.RequireValue(trailingSize__57151__value61289).width) : 0.0, titleAlignment._yOffsetFor(DartRuntimePrimitives.RequireValue(trailingSize__57151__value61289).height, tileHeight, this, false)));
            }
        }
        return (textConstraints: textConstraintsLocal, tileSize: new global::Doroti.Ui.Size(tileWidth, tileHeight), titleY: titleYLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        (global::Doroti.Framework.Rendering.BoxConstraints textConstraints, Size tileSize, double titleY) sizes = _computeSizes(ChildLayoutHelper.getDryBaseline, ChildLayoutHelper.dryLayoutChild, constraints);
        global::Doroti.Framework.Rendering.BaselineOffset titleBaseline = new global::Doroti.Framework.Rendering.BaselineOffset(title.getDryBaseline(sizes.textConstraints, baseline)).op_Add(sizes.titleY);
        return titleBaseline.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return constraints.constrain(_computeSizes(ChildLayoutHelper.getDryBaseline, ChildLayoutHelper.dryLayoutChild, constraints).tileSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Ui.Size tileSizeLocal = _computeSizes(ChildLayoutHelper.getBaseline, ChildLayoutHelper.layoutChild, constraints, positionChild: _positionBox).tileSize;
        size = constraints.constrain(tileSizeLocal);
        DartRuntimePrimitives.Assert(() => size.width == constraints.constrainWidth(tileSizeLocal.width));
        DartRuntimePrimitives.Assert(() => size.height == constraints.constrainHeight(tileSizeLocal.height));
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        void doPaint(global::Doroti.Framework.Rendering.RenderBox? child)
        {
            if (child is not null)
            {
                var parentDataLocal = ((global::Doroti.Framework.Rendering.BoxParentData?)child.parentData!)!;
                context.paintChild(child, parentDataLocal.offset + offset);
            }
        }
        doPaint(leading);
        doPaint(title);
        doPaint(subtitle);
        doPaint(trailing);
    }

    public override bool hitTestSelf(Offset position) => true;
    public override bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        foreach (global::Doroti.Framework.Rendering.RenderBox child in children)
        {
            var parentDataLocal = ((global::Doroti.Framework.Rendering.BoxParentData?)child.parentData!)!;
            bool isHit = result.addWithPaintOffset(offset: parentDataLocal.offset, position: position, hitTest: (result, transformed) =>
            {
                DartRuntimePrimitives.Assert(() => Equals(transformed, position - parentDataLocal.offset));
                return child.hitTest(result, position: transformed);
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
            if (isHit)
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Rendering.RenderBox? childForSlot(_ListTileSlot__list_tile slot) => _slotToChild.GetValueOrDefault(slot);
    public virtual string debugNameForSlot(_ListTileSlot__list_tile slot)
    {
        {
            return slot.ToString();
        }
    }

    public override void attach(global::Doroti.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        foreach (global::Doroti.Framework.Rendering.RenderBox child in children)
        {
            child.attach(owner);
        }
    }

    public override void detach()
    {
        base.detach();
        foreach (global::Doroti.Framework.Rendering.RenderBox child in children)
        {
            child.detach();
        }
    }

    public override void redepthChildren()
    {
        children.forEach((__arg0) => ((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)redepthChild)(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(__arg0)));
    }

    public override void visitChildren(global::System.Action<global::Doroti.Framework.Rendering.RenderObject> visitor)
    {
        children.forEach((__arg0) => visitor(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(__arg0)));
    }

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var value = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        var childToSlot = new DartMap<global::Doroti.Framework.Rendering.RenderBox, _ListTileSlot__list_tile>(_slotToChild.Values, _slotToChild.Keys);
        foreach (global::Doroti.Framework.Rendering.RenderBox child in children)
        {
            _addDiagnostics(child, value, debugNameForSlot(DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<_ListTileSlot__list_tile>(childToSlot, child))));
        }
        return value;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _addDiagnostics(global::Doroti.Framework.Rendering.RenderBox child, List<global::Doroti.Framework.Foundation.DiagnosticsNode> value, string name)
    {
        value.Add(((Diagnosticable)child).toDiagnosticsNode(name: name));
    }

    public virtual void _setChild(global::Doroti.Framework.Rendering.RenderBox? child, _ListTileSlot__list_tile slot)
    {
        global::Doroti.Framework.Rendering.RenderBox? oldChild = _slotToChild.GetValueOrDefault(slot);
        if (oldChild is not null)
        {
            dropChild(oldChild);
            _slotToChild.remove(slot);
        }
        if (child is not null)
        {
            _slotToChild[slot] = child;
            adoptChild(child);
        }
    }

    public virtual void _moveChild(global::Doroti.Framework.Rendering.RenderBox child, _ListTileSlot__list_tile slot, _ListTileSlot__list_tile oldSlot)
    {
        DartRuntimePrimitives.Assert(() => !Equals(slot, oldSlot));
        global::Doroti.Framework.Rendering.RenderBox? oldChild = _slotToChild.GetValueOrDefault(oldSlot);
        if (Equals(oldChild, child))
        {
            _setChild(null, oldSlot);
        }
        _setChild(child, slot);
    }

}

internal class _LisTileDefaultsM3__list_tile : ListTileThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__theme_initialized;
    private ThemeData __late__theme = default!;
    internal virtual ThemeData _theme
    {
        get
        {
            if (!__late__theme_initialized)
            {
                __late__theme = Theme.of(context);
                __late__theme_initialized = true;
            }
            return __late__theme;
        }
    }
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = _theme.colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }
    private bool __late__textTheme_initialized;
    private TextTheme __late__textTheme = default!;
    internal virtual TextTheme _textTheme
    {
        get
        {
            if (!__late__textTheme_initialized)
            {
                __late__textTheme = _theme.textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }

    internal _LisTileDefaultsM3__list_tile(global::Doroti.Framework.Widgets.BuildContext context) : base(contentPadding: EdgeInsetsDirectional.CreateOnly(start: 16.0, end: 24.0), minLeadingWidth: 24, minVerticalPadding: 8, shape: new global::Doroti.Framework.Painting.RoundedRectangleBorder())
    {
        this.context = context;
    }

    public override global::Doroti.Ui.Color? tileColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public override global::Doroti.Framework.Painting.TextStyle? titleTextStyle => _textTheme.bodyLarge!.copyWith(color: _colors.onSurface);
    public override global::Doroti.Framework.Painting.TextStyle? subtitleTextStyle => _textTheme.bodyMedium!.copyWith(color: _colors.onSurfaceVariant);
    public override global::Doroti.Framework.Painting.TextStyle? leadingAndTrailingTextStyle => _textTheme.labelSmall!.copyWith(color: _colors.onSurfaceVariant);
    public override global::Doroti.Ui.Color? selectedColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.primary);
    public override global::Doroti.Ui.Color? iconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.onSurfaceVariant);
}
