// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/list_tile.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

internal delegate void _Sizes__list_tile();

internal delegate void _PositionChild__list_tile(global::Doroti.Generated.Framework.Rendering.RenderBox child, Offset offset);

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
        return (value switch { ListTileTitleAlignment.threeLine => (((_RenderListTile__list_tile)listTile).isThreeLine ? ListTileTitleAlignment.top._yOffsetFor(childHeight, tileHeight, listTile, isLeading) : ListTileTitleAlignment.center._yOffsetFor(childHeight, tileHeight, listTile, isLeading)), ListTileTitleAlignment.titleHeight when ((tileHeight > 72.0)) => 16.0, ListTileTitleAlignment.titleHeight => (isLeading ? Math.Min((((tileHeight - childHeight)) / 2.0), 16.0) : (((tileHeight - childHeight)) / 2.0)), ListTileTitleAlignment.top => ((_RenderListTile__list_tile)listTile).minVerticalPadding, ListTileTitleAlignment.center => (((tileHeight - childHeight)) / 2.0), ListTileTitleAlignment.bottom => ((tileHeight - childHeight) - ((_RenderListTile__list_tile)listTile).minVerticalPadding), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class ListTile : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? leading { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? title { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? subtitle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? trailing { get; private set; }
    public virtual bool? isThreeLine { get; private set; }
    public virtual bool? dense { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual Color? selectedColor { get; private set; }
    public virtual Color? iconColor { get; private set; }
    public virtual Color? textColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? subtitleTextStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? leadingAndTrailingTextStyle { get; private set; }
    public virtual ListTileStyle? style { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? contentPadding { get; private set; }
    public virtual bool enabled { get; private set; } = default!;
    public virtual global::System.Action? onTap { get; private set; }
    public virtual global::System.Action? onLongPress { get; private set; }
    public virtual global::System.Action<bool>? onFocusChange { get; private set; }
    public virtual global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual bool selected { get; private set; } = default!;
    public virtual Color? focusColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual Color? splashColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode { get; private set; }
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
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController { get; private set; }

    public ListTile(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget? leading = null, global::Doroti.Generated.Framework.Widgets.Widget? title = null, global::Doroti.Generated.Framework.Widgets.Widget? subtitle = null, global::Doroti.Generated.Framework.Widgets.Widget? trailing = null, bool? isThreeLine = null, bool? dense = null, VisualDensity? visualDensity = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, ListTileStyle? style = null, Color? selectedColor = null, Color? iconColor = null, Color? textColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? subtitleTextStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? leadingAndTrailingTextStyle = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, bool enabled = true, global::System.Action? onTap = null, global::System.Action? onLongPress = null, global::System.Action<bool>? onFocusChange = null, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null, bool selected = false, Color? focusColor = null, Color? hoverColor = null, Color? splashColor = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, Color? tileColor = null, Color? selectedTileColor = null, bool? enableFeedback = null, double? horizontalTitleGap = null, double? minVerticalPadding = null, double? minLeadingWidth = null, double? minTileHeight = null, ListTileTitleAlignment? titleAlignment = null, bool internalAddSemanticForOnTap = true, global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController = null) : base(key: key)
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
        System.Diagnostics.Debug.Assert(((isThreeLine != true) || (subtitle is not null)));
    }

    public static IEnumerable<global::Doroti.Generated.Framework.Widgets.Widget> divideTiles(global::Doroti.Generated.Framework.Widgets.BuildContext? context = null, IEnumerable<global::Doroti.Generated.Framework.Widgets.Widget> tiles = default!, Color? color = null)
    {
        DartRuntimePrimitives.Assert(() => ((color is not null) || (context is not null)));
        tiles = tiles.ToList();
        if ((!System.Linq.Enumerable.Any(tiles) || (tiles.Count() == 1L)))
        {
            return tiles;
        }
        global::Doroti.Generated.Framework.Widgets.Widget wrapTile(global::Doroti.Generated.Framework.Widgets.Widget tile)
        {
            return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.DecoratedBox(position: global::Doroti.Generated.Framework.Rendering.DecorationPosition.foreground, decoration: new global::Doroti.Generated.Framework.Painting.BoxDecoration(border: new global::Doroti.Generated.Framework.Painting.Border(bottom: Divider.createBorderSide(context, color: color))), child: tile));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        return ((IEnumerable<global::Doroti.Generated.Framework.Widgets.Widget>)(object?)((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection30919 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection30919.AddRange(tiles.take((tiles.Count() - 1L)).map<global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Widgets.Widget>(wrapTile)); __collection30919.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(tiles.Last())); return __collection30919; }))());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isDenseLayout(ThemeData theme, ListTileThemeData tileTheme)
    {
        return (((this.dense ?? tileTheme.dense) ?? theme.listTileTheme.dense) ?? false);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        ThemeData theme__31258 = Theme.of(context);
        IconButtonThemeData iconButtonTheme__31315 = IconButtonTheme.of(context);
        ListTileThemeData tileTheme__31390 = ListTileTheme.of(context);
        ListTileStyle listTileStyle__31453 = (((this.style ?? tileTheme__31390.style) ?? theme__31258.listTileTheme.style) ?? ListTileStyle.list);
        ListTileThemeData defaults__31582 = (theme__31258.useMaterial3 ? new _LisTileDefaultsM3__list_tile(context) : new _LisTileDefaultsM2__list_tile(context, listTileStyle__31453));
        global::Doroti.Ui.Color backgroundColor__31721 = ((this.tileColor ?? tileTheme__31390.tileColor) ?? theme__31258.listTileTheme.tileColor) ?? defaults__31582.tileColor ?? new global::Doroti.Ui.Color(0L);
        global::Doroti.Ui.Color selectedBackgroundColor__31853 = ((this.selectedTileColor ?? tileTheme__31390.selectedTileColor) ?? theme__31258.listTileTheme.selectedTileColor) ?? defaults__31582.tileColor ?? new global::Doroti.Ui.Color(0L);
        var effectiveTileColor__32035 = (this.selected ? selectedBackgroundColor__31853 : backgroundColor__31721);
        bool hasOpaqueBackground__32125 = ((backgroundColor__31721.alpha > 0L) || (selectedBackgroundColor__31853.alpha > 0L));
        if ((((this.onTap is not null) || (this.onLongPress is not null)) || hasOpaqueBackground__32125))
        {
            DartRuntimePrimitives.Assert(() => _debugCheckBackgroundIsHidden(context));
        }
        var states__32352 = ((Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>>)(() => { var __collection32361 = new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>(); if (!this.enabled) { __collection32361.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled); } if (this.selected) { __collection32361.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.selected); } return __collection32361; }))();
        Color? resolveColor(Color? explicitColor, Color? selectedColor, Color? enabledColor, Color? disabledColor = null)
        {
            return ((Color?)(object?)new _IndividualOverrides__list_tile(explicitColor: explicitColor, selectedColor: selectedColor, enabledColor: enabledColor, disabledColor: disabledColor).resolve(states__32352));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::Doroti.Ui.Color? effectiveIconColor__32843 = ((global::Doroti.Ui.Color?)(object?)((resolveColor(this.iconColor, this.selectedColor, this.iconColor) ?? resolveColor(tileTheme__31390.iconColor, tileTheme__31390.selectedColor, tileTheme__31390.iconColor)) ?? resolveColor(theme__31258.listTileTheme.iconColor, theme__31258.listTileTheme.selectedColor, theme__31258.listTileTheme.iconColor)));
        global::Doroti.Ui.Color? defaultEffectiveIconColor__33194 = ((global::Doroti.Ui.Color?)(object?)resolveColor(defaults__31582.iconColor, defaults__31582.selectedColor, defaults__31582.iconColor, theme__31258.disabledColor));
        global::Doroti.Ui.Color? effectiveIconButtonColor__33370 = ((global::Doroti.Ui.Color?)(object?)(((effectiveIconColor__32843 ?? (Color)iconButtonTheme__31315.style?.foregroundColor?.resolve(states__32352))) ?? defaultEffectiveIconColor__33194));
        effectiveIconColor__32843 ??= defaultEffectiveIconColor__33194;
        global::Doroti.Ui.Color? effectiveColor__33602 = ((global::Doroti.Ui.Color?)(object?)(((resolveColor(this.textColor, this.selectedColor, this.textColor) ?? resolveColor(tileTheme__31390.textColor, tileTheme__31390.selectedColor, tileTheme__31390.textColor)) ?? resolveColor(theme__31258.listTileTheme.textColor, theme__31258.listTileTheme.selectedColor, theme__31258.listTileTheme.textColor)) ?? resolveColor(defaults__31582.textColor, defaults__31582.selectedColor, defaults__31582.textColor, theme__31258.disabledColor)));
        var iconThemeData__34101 = new global::Doroti.Generated.Framework.Widgets.IconThemeData(color: effectiveIconColor__32843);
        var iconButtonThemeData__34169 = new IconButtonThemeData(style: ((IconButtonTheme.of(context).style?.copyWith(foregroundColor: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color?>(effectiveIconButtonColor__33370)) ?? (ButtonStyle)IconButton.styleFrom(foregroundColor: effectiveIconButtonColor__33370))));
        global::Doroti.Generated.Framework.Painting.TextStyle? leadingAndTrailingStyle__34480 = default!;
        if (((this.leading is not null) || (this.trailing is not null)))
        {
            leadingAndTrailingStyle__34480 = ((this.leadingAndTrailingTextStyle ?? tileTheme__31390.leadingAndTrailingTextStyle) ?? defaults__31582.leadingAndTrailingTextStyle!);
            var leadingAndTrailingTextColor__34737 = effectiveColor__33602;
            leadingAndTrailingStyle__34480 = leadingAndTrailingStyle__34480.copyWith(color: leadingAndTrailingTextColor__34737);
        }
        global::Doroti.Generated.Framework.Widgets.Widget? leadingIcon__34921 = default!;
        if ((this.leading is not null))
        {
            leadingIcon__34921 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.AnimatedDefaultTextStyle(style: leadingAndTrailingStyle__34480!, duration: ConstantsLibrary.kThemeChangeDuration, child: this.leading!));
        }
        global::Doroti.Generated.Framework.Painting.TextStyle titleStyle__35143 = ((this.titleTextStyle ?? tileTheme__31390.titleTextStyle) ?? defaults__31582.titleTextStyle!);
        var titleColor__35238 = effectiveColor__33602;
        titleStyle__35143 = titleStyle__35143.copyWith(color: titleColor__35238, fontSize: (_isDenseLayout(theme__31258, tileTheme__31390) ? 13.0 : null));
        global::Doroti.Generated.Framework.Widgets.Widget titleText__35418 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.AnimatedDefaultTextStyle(style: titleStyle__35143, duration: ConstantsLibrary.kThemeChangeDuration, child: (this.title ?? new global::Doroti.Generated.Framework.Widgets.SizedBox())));
        global::Doroti.Generated.Framework.Widgets.Widget? subtitleText__35579 = default!;
        global::Doroti.Generated.Framework.Painting.TextStyle? subtitleStyle__35608 = default!;
        if ((this.subtitle is not null))
        {
            subtitleStyle__35608 = ((this.subtitleTextStyle ?? tileTheme__31390.subtitleTextStyle) ?? defaults__31582.subtitleTextStyle!);
            var subtitleColor__35776 = effectiveColor__33602;
            subtitleStyle__35608 = subtitleStyle__35608.copyWith(color: subtitleColor__35776, fontSize: (_isDenseLayout(theme__31258, tileTheme__31390) ? 12.0 : null));
            subtitleText__35579 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.AnimatedDefaultTextStyle(style: subtitleStyle__35608, duration: ConstantsLibrary.kThemeChangeDuration, child: this.subtitle!));
        }
        global::Doroti.Generated.Framework.Widgets.Widget? trailingIcon__36130 = default!;
        if ((this.trailing is not null))
        {
            trailingIcon__36130 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.AnimatedDefaultTextStyle(style: leadingAndTrailingStyle__34480!, duration: ConstantsLibrary.kThemeChangeDuration, child: this.trailing!));
        }
        global::Doroti.Ui.TextDirection textDirection__36366 = Directionality.of(context);
        global::Doroti.Generated.Framework.Painting.EdgeInsets resolvedContentPadding__36431 = ((((this.contentPadding?.resolve(textDirection__36366) ?? (global::Doroti.Generated.Framework.Painting.EdgeInsets)tileTheme__31390.contentPadding?.resolve(textDirection__36366))) ?? (global::Doroti.Generated.Framework.Painting.EdgeInsets)defaults__31582.contentPadding!.resolve(textDirection__36366)));
        var mouseStates__36718 = ((Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>>)(() => { var __collection36732 = new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>(); if ((!this.enabled || (((this.onTap is null) && (this.onLongPress is null))))) { __collection36732.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled); } return __collection36732; }))();
        global::Doroti.Generated.Framework.Services.MouseCursor effectiveMouseCursor__36860 = ((((WidgetStateProperty.resolveAs<global::Doroti.Generated.Framework.Services.MouseCursor?>(this.mouseCursor, mouseStates__36718) ?? (global::Doroti.Generated.Framework.Services.MouseCursor)tileTheme__31390.mouseCursor?.resolve(mouseStates__36718))) ?? (global::Doroti.Generated.Framework.Services.MouseCursor)global::Doroti.Generated.Framework.Widgets.WidgetStateMouseCursor.clickable.resolve(mouseStates__36718)));
        ListTileTitleAlignment effectiveTitleAlignment__37116 = ((this.titleAlignment ?? tileTheme__31390.titleAlignment) ?? ((theme__31258.useMaterial3 ? ListTileTitleAlignment.threeLine : ListTileTitleAlignment.titleHeight)));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new InkWell(customBorder: (this.shape ?? tileTheme__31390.shape), onTap: ((global::System.Action)(this.enabled ? this.onTap : null)), onLongPress: ((global::System.Action)(this.enabled ? this.onLongPress : null)), onFocusChange: this.onFocusChange, mouseCursor: effectiveMouseCursor__36860, canRequestFocus: this.enabled, focusNode: this.focusNode, focusColor: this.focusColor, hoverColor: this.hoverColor, splashColor: this.splashColor, autofocus: this.autofocus, enableFeedback: ((this.enableFeedback ?? tileTheme__31390.enableFeedback) ?? true), statesController: this.statesController, child: new global::Doroti.Generated.Framework.Widgets.Semantics(button: (this.internalAddSemanticForOnTap && (((this.onTap is not null) || (this.onLongPress is not null)))), selected: this.selected, enabled: this.enabled, child: new Ink(decoration: new global::Doroti.Generated.Framework.Painting.ShapeDecoration(shape: ((this.shape ?? tileTheme__31390.shape) ?? new global::Doroti.Generated.Framework.Painting.Border()), color: effectiveTileColor__32035), child: new global::Doroti.Generated.Framework.Widgets.SafeArea(top: false, bottom: false, minimum: resolvedContentPadding__36431, child: IconTheme.merge(data: iconThemeData__34101, child: new IconButtonTheme(data: iconButtonThemeData__34169, child: new _ListTile__list_tile(leading: leadingIcon__34921, title: titleText__35418, subtitle: subtitleText__35579, trailing: trailingIcon__36130, isDense: _isDenseLayout(theme__31258, tileTheme__31390), visualDensity: ((this.visualDensity ?? tileTheme__31390.visualDensity) ?? theme__31258.visualDensity), isThreeLine: (((this.isThreeLine ?? tileTheme__31390.isThreeLine) ?? theme__31258.listTileTheme.isThreeLine) ?? false), textDirection: textDirection__36366, titleBaselineType: (((global::Doroti.Generated.Framework.Painting.TextStyle)titleStyle__35143).textBaseline ?? DartRuntimePrimitives.RequireValue(defaults__31582.titleTextStyle!.textBaseline)), subtitleBaselineType: (subtitleStyle__35608?.textBaseline ?? DartRuntimePrimitives.RequireValue(defaults__31582.subtitleTextStyle!.textBaseline)), horizontalTitleGap: ((this.horizontalTitleGap ?? tileTheme__31390.horizontalTitleGap) ?? 16), minVerticalPadding: ((this.minVerticalPadding ?? tileTheme__31390.minVerticalPadding) ?? DartRuntimePrimitives.RequireValue(defaults__31582.minVerticalPadding)), minLeadingWidth: ((this.minLeadingWidth ?? tileTheme__31390.minLeadingWidth) ?? DartRuntimePrimitives.RequireValue(defaults__31582.minLeadingWidth)), minTileHeight: (this.minTileHeight ?? tileTheme__31390.minTileHeight), titleAlignment: effectiveTitleAlignment__37116))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("isThreeLine", value: this.isThreeLine, ifTrue: "THREE_LINE", ifFalse: "TWO_LINE", showName: true));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("dense", value: this.dense, ifTrue: "true", ifFalse: "false", showName: true));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<VisualDensity>("visualDensity", this.visualDensity, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.ShapeBorder>("shape", this.shape, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<ListTileStyle>("style", this.style, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("selectedColor", this.selectedColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("iconColor", this.iconColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("textColor", this.textColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("titleTextStyle", this.titleTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("subtitleTextStyle", this.subtitleTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("leadingAndTrailingTextStyle", this.leadingAndTrailingTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>("contentPadding", this.contentPadding, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("enabled", value: this.enabled, ifTrue: "true", ifFalse: "false", showName: true, defaultValue: true));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<Delegate>("onTap", this.onTap, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<Delegate>("onLongPress", this.onLongPress, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Services.MouseCursor>("mouseCursor", this.mouseCursor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("selected", value: this.selected, ifTrue: "true", ifFalse: "false", showName: true, defaultValue: false));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("focusColor", this.focusColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("hoverColor", this.hoverColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.FocusNode>("focusNode", this.focusNode, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("autofocus", value: this.autofocus, ifTrue: "true", ifFalse: "false", showName: true, defaultValue: false));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("tileColor", this.tileColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("selectedTileColor", this.selectedTileColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("enableFeedback", value: this.enableFeedback, ifTrue: "true", ifFalse: "false", showName: true));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("horizontalTitleGap", this.horizontalTitleGap, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("minVerticalPadding", this.minVerticalPadding, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("minLeadingWidth", this.minLeadingWidth, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<ListTileTitleAlignment>("titleAlignment", this.titleAlignment, defaultValue: null));
    }

    internal virtual bool _debugCheckBackgroundIsHidden(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                global::Doroti.Generated.Framework.Widgets.Widget? intermediateWidget__43598 = ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)_findIntermediateWidget(context));
                if ((intermediateWidget__43598 is not null))
                {
                    FlutterError.reportError(new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("ListTile background color or ink splashes may be invisible."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"The ListTile is wrapped in a {DartRuntimePrimitives.RuntimeType(intermediateWidget__43598)} that has a background color. " + "Because ListTile paints its background and ink splashes on the nearest Material ancestor, " + $"this {DartRuntimePrimitives.RuntimeType(intermediateWidget__43598)} will hide those effects."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("To fix this, wrap the ListTile in its own Material widget, " + $"or remove the background color from the intermediate {DartRuntimePrimitives.RuntimeType(intermediateWidget__43598)}.") }), informationCollector: ((InformationCollector)(() => new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<ListTile>("ListTile", this, expandableValue: true), new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.Widget>($"{DartRuntimePrimitives.RuntimeType(intermediateWidget__43598)}", intermediateWidget__43598, expandableValue: true) }))));
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget? _findIntermediateWidget(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Generated.Framework.Widgets.Widget? intermediateWidget__45010 = default!;
        (((global::Doroti.Generated.Framework.Widgets.Element?)(object?)context)!).visitAncestorElements(((global::System.Func<global::Doroti.Generated.Framework.Widgets.Element, bool>)((ancestor) => {
if ((((global::Doroti.Generated.Framework.Widgets.Element)ancestor).widget is Material))
{
    return false;
}
global::Doroti.Generated.Framework.Widgets.Widget widget__45188 = ((global::Doroti.Generated.Framework.Widgets.Element)ancestor).widget;
global::Doroti.Ui.Color? color__45233 = ((global::Doroti.Ui.Color?)(object?)(widget__45188 switch { global::Doroti.Generated.Framework.Widgets.ColoredBox { color: global::Doroti.Ui.Color color__45291 } __object45267 => color__45291, global::Doroti.Generated.Framework.Widgets.DecoratedBox { decoration: global::Doroti.Generated.Framework.Painting.BoxDecoration { color: global::Doroti.Ui.Color color__45369 } __object45341 } __object45316 => color__45369, global::Doroti.Generated.Framework.Widgets.DecoratedBox { decoration: global::Doroti.Generated.Framework.Painting.ShapeDecoration { color: global::Doroti.Ui.Color color__45450 } __object45420 } __object45395 => color__45450, _ => DartRuntimePrimitives.ConvertValue<Color>(null) }));
if (((color__45233 is not null) && (color__45233.a > 0L)))
{
    intermediateWidget__45010 = widget__45188;
    return false;
}
return true;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        return intermediateWidget__45010;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _IndividualOverrides__list_tile : global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>
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

    public virtual Color? resolve(HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> states)
    {
        if ((this.explicitColor is global::Doroti.Generated.Framework.Widgets.WidgetStateColor))
        {
            global::Doroti.Generated.Framework.Widgets.WidgetStateColor explicitColor__as46046 = (global::Doroti.Generated.Framework.Widgets.WidgetStateColor)explicitColor;
            return ((Color?)(object?)WidgetStateProperty.resolveAs<global::Doroti.Ui.Color?>(this.explicitColor, states));
        }
        if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
        {
            return this.disabledColor;
        }
        if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
        {
            return this.selectedColor;
        }
        return this.enabledColor;
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

internal class _ListTile__list_tile : global::Doroti.Generated.Framework.Widgets.SlottedMultiChildRenderObjectWidget<_ListTileSlot__list_tile, global::Doroti.Generated.Framework.Rendering.RenderBox>
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? leading { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget title { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? subtitle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? trailing { get; private set; }
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

    internal _ListTile__list_tile(global::Doroti.Generated.Framework.Widgets.Widget? leading = null, global::Doroti.Generated.Framework.Widgets.Widget title = default!, global::Doroti.Generated.Framework.Widgets.Widget? subtitle = null, global::Doroti.Generated.Framework.Widgets.Widget? trailing = null, bool isThreeLine = default!, bool isDense = default!, VisualDensity visualDensity = default!, TextDirection textDirection = default!, TextBaseline titleBaselineType = default!, double horizontalTitleGap = default!, double minVerticalPadding = default!, double minLeadingWidth = default!, double? minTileHeight = null, TextBaseline? subtitleBaselineType = null, ListTileTitleAlignment titleAlignment = default!)
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

    public override IEnumerable<_ListTileSlot__list_tile> slots => DartRuntimePrimitives.ConvertValue<IEnumerable<_ListTileSlot__list_tile>>(System.Enum.GetValues<_ListTileSlot__list_tile>().ToList());
    public override global::Doroti.Generated.Framework.Widgets.Widget? childForSlot(_ListTileSlot__list_tile slot)
    {
        return (slot switch { _ListTileSlot__list_tile.leading => this.leading, _ListTileSlot__list_tile.title => this.title, _ListTileSlot__list_tile.subtitle => this.subtitle, _ListTileSlot__list_tile.trailing => this.trailing, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderListTile__list_tile(isThreeLine: this.isThreeLine, isDense: this.isDense, visualDensity: this.visualDensity, textDirection: this.textDirection, titleBaselineType: this.titleBaselineType, subtitleBaselineType: this.subtitleBaselineType, horizontalTitleGap: this.horizontalTitleGap, minVerticalPadding: this.minVerticalPadding, minLeadingWidth: this.minLeadingWidth, minTileHeight: this.minTileHeight, titleAlignment: this.titleAlignment));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void updateRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.SlottedContainerRenderObjectMixin<_ListTileSlot__list_tile, global::Doroti.Generated.Framework.Rendering.RenderBox> renderObject)
    {
        var __renderObject = (_RenderListTile__list_tile)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderListTile__list_tile>)(() =>
{            var __cascade = __renderObject;
            __cascade.isThreeLine = this.isThreeLine;
            __cascade.isDense = this.isDense;
            __cascade.visualDensity = this.visualDensity;
            __cascade.textDirection = this.textDirection;
            __cascade.titleBaselineType = this.titleBaselineType;
            __cascade.subtitleBaselineType = this.subtitleBaselineType;
            __cascade.horizontalTitleGap = this.horizontalTitleGap;
            __cascade.minLeadingWidth = this.minLeadingWidth;
            __cascade.minTileHeight = this.minTileHeight;
            __cascade.minVerticalPadding = this.minVerticalPadding;
            __cascade.titleAlignment = this.titleAlignment;
            return __cascade;        }))());
    }

}

public class _RenderListTile__list_tile : global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Widgets.SlottedContainerRenderObjectMixin<_ListTileSlot__list_tile, global::Doroti.Generated.Framework.Rendering.RenderBox>
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
    public virtual DartMap<_ListTileSlot__list_tile, global::Doroti.Generated.Framework.Rendering.RenderBox> _slotToChild { get; set; } = new DartMap<_ListTileSlot__list_tile, global::Doroti.Generated.Framework.Rendering.RenderBox>();

    internal _RenderListTile__list_tile(bool isDense, VisualDensity visualDensity, bool isThreeLine, TextDirection textDirection, TextBaseline titleBaselineType, TextBaseline? subtitleBaselineType = null, double horizontalTitleGap = default!, double minVerticalPadding = default!, double minLeadingWidth = default!, double? minTileHeight = null, ListTileTitleAlignment titleAlignment = default!)
    {
        this._isDense = isDense;
        this._visualDensity = visualDensity;
        this._isThreeLine = isThreeLine;
        this._textDirection = textDirection;
        this._titleBaselineType = titleBaselineType;
        this._subtitleBaselineType = subtitleBaselineType;
        this._horizontalTitleGap = horizontalTitleGap;
        this._minVerticalPadding = minVerticalPadding;
        this._minLeadingWidth = minLeadingWidth;
        this._minTileHeight = minTileHeight;
        this._titleAlignment = titleAlignment;
    }

    public virtual global::Doroti.Generated.Framework.Rendering.RenderBox? leading => childForSlot(DartRuntimePrimitives.RequireValue(_ListTileSlot__list_tile.leading));
    public virtual global::Doroti.Generated.Framework.Rendering.RenderBox title => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderBox>(childForSlot(_ListTileSlot__list_tile.title)!);
    public virtual global::Doroti.Generated.Framework.Rendering.RenderBox? subtitle => childForSlot(DartRuntimePrimitives.RequireValue(_ListTileSlot__list_tile.subtitle));
    public virtual global::Doroti.Generated.Framework.Rendering.RenderBox? trailing => childForSlot(DartRuntimePrimitives.RequireValue(_ListTileSlot__list_tile.trailing));
    public virtual IEnumerable<global::Doroti.Generated.Framework.Rendering.RenderBox> children
    {
        get
        {
            global::Doroti.Generated.Framework.Rendering.RenderBox? title__50350 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)childForSlot(_ListTileSlot__list_tile.title));
            return ((IEnumerable<global::Doroti.Generated.Framework.Rendering.RenderBox>)(object?)((Func<List<global::Doroti.Generated.Framework.Rendering.RenderBox>>)(() => { var __collection50404 = new List<global::Doroti.Generated.Framework.Rendering.RenderBox>(); var __collectionElement50416 = this.leading; if (__collectionElement50416 is { } __nonNullCollectionElement50416) { __collection50404.Add(__nonNullCollectionElement50416); } var __collectionElement50426 = title__50350; if (__collectionElement50426 is { } __nonNullCollectionElement50426) { __collection50404.Add(__nonNullCollectionElement50426); } var __collectionElement50434 = this.subtitle; if (__collectionElement50434 is { } __nonNullCollectionElement50434) { __collection50404.Add(__nonNullCollectionElement50434); } var __collectionElement50445 = this.trailing; if (__collectionElement50445 is { } __nonNullCollectionElement50445) { __collection50404.Add(__nonNullCollectionElement50445); } return __collection50404; }))());
            return default!;
        }
    }
    public virtual bool isDense
    {
        get => this._isDense;
        set
        {
            var __value = value;
            if ((this._isDense == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _isDense = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual VisualDensity visualDensity
    {
        get => this._visualDensity;
        set
        {
            var __value = value;
            if ((object.Equals(this._visualDensity, __value)))
            {
                return;
            }
            _visualDensity = __value;
            markNeedsLayout();
        }
    }
    public virtual bool isThreeLine
    {
        get => this._isThreeLine;
        set
        {
            var __value = value;
            if ((this._isThreeLine == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _isThreeLine = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.TextDirection textDirection
    {
        get => this._textDirection;
        set
        {
            var __value = value;
            if ((object.Equals(this._textDirection, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _textDirection = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.TextBaseline titleBaselineType
    {
        get => this._titleBaselineType;
        set
        {
            var __value = value;
            if ((object.Equals(this._titleBaselineType, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _titleBaselineType = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.TextBaseline? subtitleBaselineType
    {
        get => this._subtitleBaselineType;
        set
        {
            var __value = value;
            if ((object.Equals(this._subtitleBaselineType, __value)))
            {
                return;
            }
            _subtitleBaselineType = __value;
            markNeedsLayout();
        }
    }
    public virtual double horizontalTitleGap
    {
        get => this._horizontalTitleGap;
        set
        {
            var __value = value;
            if ((this._horizontalTitleGap == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _horizontalTitleGap = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    internal virtual double _effectiveHorizontalTitleGap => DartRuntimePrimitives.ConvertValue<double>((this._horizontalTitleGap + (this.visualDensity.horizontal * 2.0)));
    public virtual double minVerticalPadding
    {
        get => this._minVerticalPadding;
        set
        {
            var __value = value;
            if ((this._minVerticalPadding == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _minVerticalPadding = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual double minLeadingWidth
    {
        get => this._minLeadingWidth;
        set
        {
            var __value = value;
            if ((this._minLeadingWidth == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _minLeadingWidth = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual double? minTileHeight
    {
        get => this._minTileHeight;
        set
        {
            var __value = value;
            if ((this._minTileHeight == __value))
            {
                return;
            }
            _minTileHeight = __value;
            markNeedsLayout();
        }
    }
    public virtual ListTileTitleAlignment titleAlignment
    {
        get => this._titleAlignment;
        set
        {
            var __value = value;
            if ((object.Equals(this._titleAlignment, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _titleAlignment = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public override bool sizedByParent => false;
    internal static double _minWidth(global::Doroti.Generated.Framework.Rendering.RenderBox? box, double height)
    {
        return ((box is null) ? 0.0 : box.getMinIntrinsicWidth(height));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _maxWidth(global::Doroti.Generated.Framework.Rendering.RenderBox? box, double height)
    {
        return ((box is null) ? 0.0 : box.getMaxIntrinsicWidth(height));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        double leadingWidth__53564 = ((this.leading is not null) ? (Math.Max(this.leading!.getMinIntrinsicWidth(height), this._minLeadingWidth) + this._effectiveHorizontalTitleGap) : 0.0);
        return ((leadingWidth__53564 + Math.Max(_RenderListTile__list_tile._minWidth(this.title, height), _RenderListTile__list_tile._minWidth(this.subtitle, height))) + _RenderListTile__list_tile._maxWidth(this.trailing, height));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        double leadingWidth__53953 = ((this.leading is not null) ? (Math.Max(this.leading!.getMaxIntrinsicWidth(height), this._minLeadingWidth) + this._effectiveHorizontalTitleGap) : 0.0);
        return ((leadingWidth__53953 + Math.Max(_RenderListTile__list_tile._maxWidth(this.title, height), _RenderListTile__list_tile._maxWidth(this.subtitle, height))) + _RenderListTile__list_tile._maxWidth(this.trailing, height));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _defaultTileHeight
    {
        get
        {
            global::Doroti.Ui.Offset baseDensity__54384 = ((global::Doroti.Ui.Offset)(object?)this.visualDensity.baseSizeAdjustment);
            return (baseDensity__54384.dy + ((this.isThreeLine, (this.subtitle is not null)) switch { (true, _) => (this.isDense ? 76.0 : 88.0), (false, true) => (this.isDense ? 64.0 : 72.0), (false, false) => (this.isDense ? 48.0 : 56.0) }));
            return default!;
        }
    }
    internal virtual double _targetTileHeight => DartRuntimePrimitives.ConvertValue<double>(((this._minTileHeight ?? (double)this._defaultTileHeight)));
    public override double computeMinIntrinsicHeight(double width)
    {
        double titleMinHeight__54861 = this.title.getMinIntrinsicHeight(width);
        double? subtitleMinHeight__54932 = this.subtitle?.getMinIntrinsicHeight(width);
        var topAndBottomPaddingMultiplier__55003 = 2L;
        double contentHeight__55055 = ((titleMinHeight__54861 + ((subtitleMinHeight__54932 ?? 0.0))) + (topAndBottomPaddingMultiplier__55003 * this._minVerticalPadding));
        return Math.Max(this._targetTileHeight, contentHeight__55055);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return getMinIntrinsicHeight(width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        var parentData__55453 = ((global::Doroti.Generated.Framework.Rendering.BoxParentData?)(object?)this.title.parentData!)!;
        global::Doroti.Generated.Framework.Rendering.BaselineOffset offset__55527 = (new global::Doroti.Generated.Framework.Rendering.BaselineOffset(this.title.getDistanceToActualBaseline(baseline)).op_Add(((global::Doroti.Generated.Framework.Rendering.BoxParentData)parentData__55453).offset.dy));
        return offset__55527.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints maxIconHeightConstraint => new global::Doroti.Generated.Framework.Rendering.BoxConstraints(maxHeight: (((this.isDense ? 48.0 : 56.0)) + this.visualDensity.baseSizeAdjustment.dy));
    internal static void _positionBox(global::Doroti.Generated.Framework.Rendering.RenderBox box, Offset offset)
    {
        var parentData__56194 = ((global::Doroti.Generated.Framework.Rendering.BoxParentData?)(object?)box.parentData!)!;
        parentData__56194.offset = offset;
    }

    internal virtual (global::Doroti.Generated.Framework.Rendering.BoxConstraints textConstraints, Size tileSize, double titleY) _computeSizes(global::System.Func<global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Rendering.BoxConstraints, TextBaseline, double?> getBaseline, global::System.Func<global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Rendering.BoxConstraints, Size> getSize, global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderBox, Offset>? positionChild = null)
    {
        global::Doroti.Generated.Framework.Rendering.BoxConstraints looseConstraints__56761 = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)(object?)constraints.loosen());
        double tileWidth__56819 = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)looseConstraints__56761).maxWidth;
        global::Doroti.Generated.Framework.Rendering.BoxConstraints iconConstraints__56883 = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)(object?)looseConstraints__56761.enforce(this.maxIconHeightConstraint));
        global::Doroti.Generated.Framework.Rendering.RenderBox? leading__56973 = this.leading;
        global::Doroti.Generated.Framework.Rendering.RenderBox? trailing__57018 = this.trailing;
        global::Doroti.Ui.Size? leadingSize__57061 = ((global::Doroti.Ui.Size?)(object?)((leading__56973 is null) ? null : getSize(leading__56973, iconConstraints__56883)));
        global::Doroti.Ui.Size? trailingSize__57151 = ((global::Doroti.Ui.Size?)(object?)((trailing__57018 is null) ? null : getSize(trailing__57018, iconConstraints__56883)));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((tileWidth__56819 == 0.0))
                {
                    return true;
                }
                string? overflowedWidget__57319 = default!;
                if ((tileWidth__56819 == leadingSize__57061?.width))
                {
                    overflowedWidget__57319 = "Leading";
                }
                else
                {
                    if ((tileWidth__56819 == trailingSize__57151?.width))
                    {
                        overflowedWidget__57319 = "Trailing";
                    }
                }
                if ((overflowedWidget__57319 is null))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{overflowedWidget__57319} widget consumes the entire tile width (including ListTile.contentPadding)."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"Either resize the tile width so that the {overflowedWidget__57319.toLowerCase()} widget plus any content padding " + "do not exceed the tile width, or use a sized widget, or consider replacing " + "ListTile with a custom widget."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("See also: https://api.flutter.dev/flutter/material/ListTile-class.html#material.ListTile.4") }));
            });
        double titleStart__58243 = ((leadingSize__57061 is null) ? 0.0 : (Math.Max(this._minLeadingWidth, DartRuntimePrimitives.RequireValue(leadingSize__57061).width) + this._effectiveHorizontalTitleGap));
        double adjustedTrailingWidth__58396 = ((trailingSize__57151 is null) ? 0.0 : Math.Max((DartRuntimePrimitives.RequireValue(trailingSize__57151).width + this._effectiveHorizontalTitleGap), 32.0));
        global::Doroti.Generated.Framework.Rendering.BoxConstraints textConstraints__58558 = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)(object?)looseConstraints__56761.tighten(width: ((tileWidth__56819 - titleStart__58243) - adjustedTrailingWidth__58396)));
        global::Doroti.Generated.Framework.Rendering.RenderBox? subtitle__58692 = this.subtitle;
        double titleHeight__58735 = getSize(this.title, textConstraints__58558).height;
        bool isLTR__58805 = (this.textDirection switch { TextDirection.ltr => true, TextDirection.rtl => false, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        double titleY__58930 = default!;
        double tileHeight__58955 = default!;
        if ((subtitle__58692 is null))
        {
            tileHeight__58955 = Math.Max(this._targetTileHeight, (titleHeight__58735 + (2.0 * this._minVerticalPadding)));
            titleY__58930 = (((tileHeight__58955 - titleHeight__58735)) / 2.0);
        }
        else
        {
            double subtitleHeight__59165 = getSize(subtitle__58692, textConstraints__58558).height;
            double titleBaseline__59244 = (getBaseline(this.title, textConstraints__58558, this.titleBaselineType) ?? titleHeight__58735);
            double subtitleBaseline__59360 = (getBaseline(subtitle__58692, textConstraints__58558, DartRuntimePrimitives.RequireValue(this.subtitleBaselineType)) ?? subtitleHeight__59165);
            double targetTitleY__59490 = (((this.isThreeLine ? ((this.isDense ? 22.0 : 28.0)) : ((this.isDense ? 28.0 : 32.0)))) - titleBaseline__59244);
            double targetSubtitleY__59617 = ((((this.isThreeLine ? ((this.isDense ? 42.0 : 48.0)) : ((this.isDense ? 48.0 : 52.0)))) + (this.visualDensity.vertical * 2.0)) - subtitleBaseline__59360);
            double halfOverlap__59929 = (Math.Max(((targetTitleY__59490 + titleHeight__58735) - targetSubtitleY__59617), 0L) / 2L);
            double idealTitleY__60025 = (targetTitleY__59490 - halfOverlap__59929);
            double idealSubtitleY__60086 = (targetSubtitleY__59617 + halfOverlap__59929);
            bool compact__60285 = ((idealTitleY__60025 < this.minVerticalPadding) || (((idealSubtitleY__60086 + subtitleHeight__59165) + this.minVerticalPadding) > this._targetTileHeight));
            positionChild?.Invoke(subtitle__58692, new global::Doroti.Ui.Offset((isLTR__58805 ? titleStart__58243 : adjustedTrailingWidth__58396), (compact__60285 ? (this.minVerticalPadding + titleHeight__58735) : idealSubtitleY__60086)));
            tileHeight__58955 = (compact__60285 ? (((2L * this._minVerticalPadding) + titleHeight__58735) + subtitleHeight__59165) : this._targetTileHeight);
            titleY__58930 = (compact__60285 ? this.minVerticalPadding : idealTitleY__60025);
        }
        if ((positionChild is not null))
        {
            positionChild(this.title, new global::Doroti.Ui.Offset((isLTR__58805 ? titleStart__58243 : adjustedTrailingWidth__58396), titleY__58930));
            if (((leading__56973 is not null) && (leadingSize__57061 is not null)))
            {
                Size leadingSize__57061__value61002 = DartRuntimePrimitives.RequireValue(leadingSize__57061);
                positionChild(leading__56973, new global::Doroti.Ui.Offset((isLTR__58805 ? 0.0 : (tileWidth__56819 - DartRuntimePrimitives.RequireValue(leadingSize__57061__value61002).width)), this.titleAlignment._yOffsetFor(DartRuntimePrimitives.RequireValue(leadingSize__57061__value61002).height, tileHeight__58955, this, true)));
            }
            if (((trailing__57018 is not null) && (trailingSize__57151 is not null)))
            {
                Size trailingSize__57151__value61289 = DartRuntimePrimitives.RequireValue(trailingSize__57151);
                positionChild(trailing__57018, new global::Doroti.Ui.Offset((isLTR__58805 ? (tileWidth__56819 - DartRuntimePrimitives.RequireValue(trailingSize__57151__value61289).width) : 0.0), this.titleAlignment._yOffsetFor(DartRuntimePrimitives.RequireValue(trailingSize__57151__value61289).height, tileHeight__58955, this, false)));
            }
        }
        return (textConstraints: textConstraints__58558, tileSize: new global::Doroti.Ui.Size(tileWidth__56819, tileHeight__58955), titleY: titleY__58930);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        (global::Doroti.Generated.Framework.Rendering.BoxConstraints textConstraints, Size tileSize, double titleY) sizes__61810 = _computeSizes((global::System.Func<global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Rendering.BoxConstraints, TextBaseline, double?>)global::Doroti.Generated.Framework.Rendering.ChildLayoutHelper.getDryBaseline, (global::System.Func<global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Rendering.BoxConstraints, Size>)global::Doroti.Generated.Framework.Rendering.ChildLayoutHelper.dryLayoutChild, constraints);
        global::Doroti.Generated.Framework.Rendering.BaselineOffset titleBaseline__61964 = (new global::Doroti.Generated.Framework.Rendering.BaselineOffset(this.title.getDryBaseline(sizes__61810.textConstraints, baseline)).op_Add(sizes__61810.titleY));
        return titleBaseline__61964.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        return constraints.constrain(_computeSizes((global::System.Func<global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Rendering.BoxConstraints, TextBaseline, double?>)global::Doroti.Generated.Framework.Rendering.ChildLayoutHelper.getDryBaseline, (global::System.Func<global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Rendering.BoxConstraints, Size>)global::Doroti.Generated.Framework.Rendering.ChildLayoutHelper.dryLayoutChild, constraints).tileSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Ui.Size tileSize__62420 = ((global::Doroti.Ui.Size)(object?)_computeSizes((global::System.Func<global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Rendering.BoxConstraints, TextBaseline, double?>)global::Doroti.Generated.Framework.Rendering.ChildLayoutHelper.getBaseline, (global::System.Func<global::Doroti.Generated.Framework.Rendering.RenderBox, global::Doroti.Generated.Framework.Rendering.BoxConstraints, Size>)global::Doroti.Generated.Framework.Rendering.ChildLayoutHelper.layoutChild, this.constraints, positionChild: (global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderBox, Offset>)_positionBox).tileSize);
        size = this.constraints.constrain(tileSize__62420);
        DartRuntimePrimitives.Assert(() => (this.size.width == this.constraints.constrainWidth(tileSize__62420.width)));
        DartRuntimePrimitives.Assert(() => (this.size.height == this.constraints.constrainHeight(tileSize__62420.height)));
    }

    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        void doPaint(global::Doroti.Generated.Framework.Rendering.RenderBox? child)
        {
            if ((child is not null))
            {
                var parentData__62928 = ((global::Doroti.Generated.Framework.Rendering.BoxParentData?)(object?)child.parentData!)!;
                context.paintChild(child, (((global::Doroti.Generated.Framework.Rendering.BoxParentData)parentData__62928).offset + offset));
            }
        }
        doPaint(this.leading);
        doPaint(this.title);
        doPaint(this.subtitle);
        doPaint(this.trailing);
    }

    public override bool hitTestSelf(Offset position) => true;
    public override bool hitTestChildren(global::Doroti.Generated.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        foreach (global::Doroti.Generated.Framework.Rendering.RenderBox child__63321 in this.children)
        {
            var parentData__63354 = ((global::Doroti.Generated.Framework.Rendering.BoxParentData?)(object?)child__63321.parentData!)!;
            bool isHit__63420 = result.addWithPaintOffset(offset: ((global::Doroti.Generated.Framework.Rendering.BoxParentData)parentData__63354).offset, position: position, hitTest: ((global::System.Func<global::Doroti.Generated.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, transformed) => {
DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - ((global::Doroti.Generated.Framework.Rendering.BoxParentData)parentData__63354).offset))));
return child__63321.hitTest(result, position: transformed);
throw new InvalidOperationException("Dart closure completed without a value.");
})));
            if (isHit__63420)
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Rendering.RenderBox? childForSlot(_ListTileSlot__list_tile slot) => this._slotToChild.GetValueOrDefault(slot);
    public virtual string debugNameForSlot(_ListTileSlot__list_tile slot)
    {
        if (true)
        {
            return slot.ToString();
        }
        return slot.ToString();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void attach(global::Doroti.Generated.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        foreach (global::Doroti.Generated.Framework.Rendering.RenderBox child__6961 in this.children)
        {
            ((dynamic)child__6961).attach(owner);
        }
    }

    public override void detach()
    {
        base.detach();
        foreach (global::Doroti.Generated.Framework.Rendering.RenderBox child__7095 in this.children)
        {
            ((dynamic)child__7095).detach();
        }
    }

    public override void redepthChildren()
    {
        this.children.forEach((__arg0) => ((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)this.redepthChild)(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(__arg0)));
    }

    public override void visitChildren(global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject> visitor)
    {
        this.children.forEach((__arg0) => ((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)visitor)(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(__arg0)));
    }

    public override List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var value__7401 = new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>();
        var childToSlot__7440 = new DartMap<global::Doroti.Generated.Framework.Rendering.RenderBox, _ListTileSlot__list_tile>(this._slotToChild.Values, this._slotToChild.Keys);
        foreach (global::Doroti.Generated.Framework.Rendering.RenderBox child__7578 in this.children)
        {
            _addDiagnostics(child__7578, value__7401, debugNameForSlot(((_ListTileSlot__list_tile)DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<_ListTileSlot__list_tile>(childToSlot__7440, child__7578)))));
        }
        return value__7401;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _addDiagnostics(global::Doroti.Generated.Framework.Rendering.RenderBox child, List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> value, string name)
    {
        value.Add(((Diagnosticable)child).toDiagnosticsNode(name: name));
    }

    public virtual void _setChild(global::Doroti.Generated.Framework.Rendering.RenderBox? child, _ListTileSlot__list_tile slot)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? oldChild__8003 = this._slotToChild.GetValueOrDefault(slot);
        if ((oldChild__8003 is not null))
        {
            dropChild(oldChild__8003);
            this._slotToChild.remove(slot);
        }
        if ((child is not null))
        {
            this._slotToChild[slot] = child;
            adoptChild(child);
        }
    }

    public virtual void _moveChild(global::Doroti.Generated.Framework.Rendering.RenderBox child, _ListTileSlot__list_tile slot, _ListTileSlot__list_tile oldSlot)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(slot, oldSlot)));
        global::Doroti.Generated.Framework.Rendering.RenderBox? oldChild__8343 = this._slotToChild.GetValueOrDefault(oldSlot);
        if ((object.Equals(oldChild__8343, child)))
        {
            _setChild(null, oldSlot);
        }
        _setChild(child, slot);
    }

}

internal class _LisTileDefaultsM2__list_tile : ListTileThemeData
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__theme_initialized;
    private ThemeData __late__theme = default!;
    internal virtual ThemeData _theme
    {
        get
        {
            if (!__late__theme_initialized)
            {
                __late__theme = Theme.of(this.context);
                __late__theme_initialized = true;
            }
            return __late__theme;
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
                __late__textTheme = this._theme.textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }

    internal _LisTileDefaultsM2__list_tile(global::Doroti.Generated.Framework.Widgets.BuildContext context, ListTileStyle style) : base(contentPadding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 16.0), minLeadingWidth: 40, minVerticalPadding: 4, shape: new global::Doroti.Generated.Framework.Painting.Border(), style: style)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? tileColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public override global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle => (DartRuntimePrimitives.RequireValue(style) switch { ListTileStyle.drawer => this._textTheme.bodyLarge, ListTileStyle.list => this._textTheme.titleMedium, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    public override global::Doroti.Generated.Framework.Painting.TextStyle? subtitleTextStyle => this._textTheme.bodyMedium!.copyWith(color: this._textTheme.bodySmall!.color);
    public override global::Doroti.Generated.Framework.Painting.TextStyle? leadingAndTrailingTextStyle => this._textTheme.bodyMedium;
    public virtual global::Doroti.Ui.Color? selectedColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._theme.colorScheme.primary);
    public virtual global::Doroti.Ui.Color? iconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._theme.brightness switch { Brightness.light => Colors.black45, Brightness.dark => DartRuntimePrimitives.ConvertValue<Color>(null), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
}

internal class _LisTileDefaultsM3__list_tile : ListTileThemeData
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__theme_initialized;
    private ThemeData __late__theme = default!;
    internal virtual ThemeData _theme
    {
        get
        {
            if (!__late__theme_initialized)
            {
                __late__theme = Theme.of(this.context);
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
                __late__colors = this._theme.colorScheme;
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
                __late__textTheme = this._theme.textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }

    internal _LisTileDefaultsM3__list_tile(global::Doroti.Generated.Framework.Widgets.BuildContext context) : base(contentPadding: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 16.0, end: 24.0), minLeadingWidth: 24, minVerticalPadding: 8, shape: new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder())
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? tileColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public override global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle => this._textTheme.bodyLarge!.copyWith(color: this._colors.onSurface);
    public override global::Doroti.Generated.Framework.Painting.TextStyle? subtitleTextStyle => this._textTheme.bodyMedium!.copyWith(color: this._colors.onSurfaceVariant);
    public override global::Doroti.Generated.Framework.Painting.TextStyle? leadingAndTrailingTextStyle => this._textTheme.labelSmall!.copyWith(color: this._colors.onSurfaceVariant);
    public virtual global::Doroti.Ui.Color? selectedColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary);
    public virtual global::Doroti.Ui.Color? iconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurfaceVariant);
}
