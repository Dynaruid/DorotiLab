// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/list_section.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Cupertino;

public static partial class List_sectionLibrary
{
    internal static double _kMarginTop = 22.0;
}

public static partial class List_sectionLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsetsDirectional _kDefaultHeaderMargin = new global::Doroti.Framework.Painting.EdgeInsetsDirectional(20.0, 0.0, 20.0, 6.0);
}

public static partial class List_sectionLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsetsDirectional _kInsetGroupedDefaultHeaderMargin = new global::Doroti.Framework.Painting.EdgeInsetsDirectional(20.0, 16.0, 20.0, 6.0);
}

public static partial class List_sectionLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsetsDirectional _kDefaultFooterMargin = new global::Doroti.Framework.Painting.EdgeInsetsDirectional(20.0, 0.0, 20.0, 0.0);
}

public static partial class List_sectionLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsetsDirectional _kInsetGroupedDefaultFooterMargin = new global::Doroti.Framework.Painting.EdgeInsetsDirectional(20.0, 0.0, 20.0, 10.0);
}

public static partial class List_sectionLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsets _kDefaultRowsMargin = global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(bottom: 8.0);
}

public static partial class List_sectionLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsetsDirectional _kDefaultInsetGroupedRowsMargin = new global::Doroti.Framework.Painting.EdgeInsetsDirectional(20.0, 20.0, 20.0, 10.0);
}

public static partial class List_sectionLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsetsDirectional _kDefaultInsetGroupedRowsMarginWithHeader = new global::Doroti.Framework.Painting.EdgeInsetsDirectional(20.0, 0.0, 20.0, 10.0);
}

public static partial class List_sectionLibrary
{
    internal static global::Doroti.Framework.Painting.BorderRadius _kDefaultInsetGroupedBorderRadius = global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(10.0));
}

public static partial class List_sectionLibrary
{
    internal static double _kBaseDividerMargin = 20.0;
}

public static partial class List_sectionLibrary
{
    internal static double _kBaseAdditionalDividerMargin = 44.0;
}

public static partial class List_sectionLibrary
{
    internal static double _kInsetDividerMargin = 14.0;
}

public static partial class List_sectionLibrary
{
    internal static double _kInsetAdditionalDividerMargin = 42.0;
}

public static partial class List_sectionLibrary
{
    internal static double _kInsetAdditionalDividerMarginWithoutLeading = 14.0;
}

public static partial class List_sectionLibrary
{
    internal static Color _kHeaderFooterColor = ((Color)(object?)new CupertinoDynamicColor(color: global::Doroti.Ui.Color.fromRGBO(108L, 108L, 108L, 1.0), darkColor: global::Doroti.Ui.Color.fromRGBO(142L, 142L, 146L, 1.0), highContrastColor: global::Doroti.Ui.Color.fromRGBO(74L, 74L, 77L, 1.0), darkHighContrastColor: global::Doroti.Ui.Color.fromRGBO(176L, 176L, 183L, 1.0), elevatedColor: global::Doroti.Ui.Color.fromRGBO(108L, 108L, 108L, 1.0), darkElevatedColor: global::Doroti.Ui.Color.fromRGBO(142L, 142L, 146L, 1.0), highContrastElevatedColor: global::Doroti.Ui.Color.fromRGBO(108L, 108L, 108L, 1.0), darkHighContrastElevatedColor: global::Doroti.Ui.Color.fromRGBO(142L, 142L, 146L, 1.0)));
}

public enum CupertinoListSectionType
{
    @base,
    insetGrouped
}

public class CupertinoListSection : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual CupertinoListSectionType type { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? header { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? footer { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry margin { get; private set; } = default!;
    public virtual List<global::Doroti.Framework.Widgets.Widget>? children { get; private set; }
    public virtual global::Doroti.Framework.Painting.BoxDecoration? decoration { get; private set; }
    public virtual Color backgroundColor { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual double dividerMargin { get; private set; } = default!;
    public virtual double additionalDividerMargin { get; private set; } = default!;
    public virtual double? topMargin { get; private set; }
    public virtual Color? separatorColor { get; private set; }

    public CupertinoListSection(global::Doroti.Framework.Foundation.Key? key = null, List<global::Doroti.Framework.Widgets.Widget>? children = null, global::Doroti.Framework.Widgets.Widget? header = null, global::Doroti.Framework.Widgets.Widget? footer = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry margin = default!, Color backgroundColor = default!, global::Doroti.Framework.Painting.BoxDecoration? decoration = null, Clip clipBehavior = Clip.none, double? dividerMargin = null, double? additionalDividerMargin = null, double? topMargin = null, bool hasLeading = true, Color? separatorColor = null) : base(key: key)
    {
        global::Doroti.Framework.Painting.EdgeInsetsGeometry __margin = margin ?? List_sectionLibrary._kDefaultRowsMargin;
        Color __backgroundColor = backgroundColor ?? CupertinoColors.systemGroupedBackground;
        double __dividerMargin = dividerMargin ?? List_sectionLibrary._kBaseDividerMargin;
        double? __topMargin = topMargin ?? List_sectionLibrary._kMarginTop;
        this.children = children;
        this.header = header;
        this.footer = footer;
        this.margin = __margin;
        this.backgroundColor = __backgroundColor;
        this.decoration = decoration;
        this.clipBehavior = clipBehavior;
        this.dividerMargin = __dividerMargin;
        this.topMargin = __topMargin;
        this.separatorColor = separatorColor;
        this.type = CupertinoListSectionType.@base;
        this.additionalDividerMargin = (additionalDividerMargin ?? ((hasLeading ? List_sectionLibrary._kBaseAdditionalDividerMargin : 0.0)));
        System.Diagnostics.Debug.Assert(((((children is not null) && (checked((long)(children.Count)) > 0L))) || (header is not null)));
    }

    public static CupertinoListSection CreateInsetGrouped(global::Doroti.Framework.Foundation.Key? key = null, List<global::Doroti.Framework.Widgets.Widget>? children = null, global::Doroti.Framework.Widgets.Widget? header = null, global::Doroti.Framework.Widgets.Widget? footer = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin = null, Color backgroundColor = default!, global::Doroti.Framework.Painting.BoxDecoration? decoration = null, Clip clipBehavior = Clip.hardEdge, double? dividerMargin = null, double? additionalDividerMargin = null, double? topMargin = null, bool hasLeading = true, Color? separatorColor = null)
    {
        var __instance = new CupertinoListSection(key: key, children: children, header: header, footer: footer, margin: margin, backgroundColor: backgroundColor, decoration: decoration, clipBehavior: clipBehavior, dividerMargin: dividerMargin, additionalDividerMargin: additionalDividerMargin, topMargin: topMargin, hasLeading: hasLeading, separatorColor: separatorColor);
        Color __backgroundColor = backgroundColor ?? CupertinoColors.systemGroupedBackground;
        double __dividerMargin = dividerMargin ?? List_sectionLibrary._kInsetDividerMargin;
        __instance.children = children;
        __instance.header = header;
        __instance.footer = footer;
        __instance.backgroundColor = __backgroundColor;
        __instance.decoration = decoration;
        __instance.clipBehavior = clipBehavior;
        __instance.dividerMargin = __dividerMargin;
        __instance.topMargin = topMargin;
        __instance.separatorColor = separatorColor;
        __instance.type = CupertinoListSectionType.insetGrouped;
        __instance.additionalDividerMargin = (additionalDividerMargin ?? ((hasLeading ? List_sectionLibrary._kInsetAdditionalDividerMargin : List_sectionLibrary._kInsetAdditionalDividerMarginWithoutLeading)));
        __instance.margin = (margin ?? (((header is null) ? List_sectionLibrary._kDefaultInsetGroupedRowsMargin : List_sectionLibrary._kDefaultInsetGroupedRowsMarginWithHeader)));
        return __instance;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color dividerColor = ((global::Doroti.Ui.Color)(object?)(this.separatorColor ?? CupertinoColors.separator.resolveFrom(context)));
        double dividerHeight = (1.0 / MediaQuery.devicePixelRatioOf(context));
        global::Doroti.Framework.Widgets.Widget longDivider = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Container(color: dividerColor, height: dividerHeight));
        global::Doroti.Framework.Widgets.Widget shortDivider = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Container(margin: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: (this.dividerMargin + this.additionalDividerMargin)), color: dividerColor, height: dividerHeight));
        global::Doroti.Framework.Painting.TextStyle styleLocal = CupertinoTheme.of(context).textTheme.textStyle;
        global::Doroti.Framework.Widgets.Widget? headerWidget = default!;
        global::Doroti.Framework.Widgets.Widget? footerWidget = default!;
        switch (this.type)
        {
            case CupertinoListSectionType.@base:
                {
                    styleLocal = styleLocal.merge(new global::Doroti.Framework.Painting.TextStyle(fontSize: 13.0, color: CupertinoDynamicColor.resolve(List_sectionLibrary._kHeaderFooterColor, context)));
                    if ((this.header is not null))
                    {
                        headerWidget = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.DefaultTextStyle(style: styleLocal, child: this.header!));
                    }
                    if ((this.footer is not null))
                    {
                        footerWidget = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.DefaultTextStyle(style: styleLocal, child: this.footer!));
                    }
                    break;
                }
            case CupertinoListSectionType.insetGrouped:
                {
                    if ((this.header is not null))
                    {
                        headerWidget = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.DefaultTextStyle(style: styleLocal.merge(new global::Doroti.Framework.Painting.TextStyle(fontSize: 20.0, fontWeight: FontWeight.bold)), child: this.header!));
                    }
                    if ((this.footer is not null))
                    {
                        footerWidget = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.DefaultTextStyle(style: styleLocal, child: this.footer!));
                    }
                    break;
                }
        }
        global::Doroti.Framework.Widgets.Widget? decoratedChildrenGroup = default!;
        if (((this.children is not null) && System.Linq.Enumerable.Any(this.children!)))
        {
            var childrenWithDividers = new List<global::Doroti.Framework.Widgets.Widget>();
            if ((object.Equals(this.type, CupertinoListSectionType.@base)))
            {
                childrenWithDividers.Add(longDivider);
            }
            this.children!.GetRange(0L, (checked((long)(this.children!.Count)) - 1L)).forEach(((global::System.Action<global::Doroti.Framework.Widgets.Widget>)((widget) =>
            {
                childrenWithDividers.Add(widget);
                childrenWithDividers.Add(shortDivider);
            })));
            childrenWithDividers.Add(this.children!.Last());
            if ((object.Equals(this.type, CupertinoListSectionType.@base)))
            {
                childrenWithDividers.Add(longDivider);
            }
            global::Doroti.Framework.Painting.BorderRadius childrenGroupBorderRadius = (this.type switch { CupertinoListSectionType.insetGrouped => List_sectionLibrary._kDefaultInsetGroupedBorderRadius, CupertinoListSectionType.@base => global::Doroti.Framework.Painting.BorderRadius.zero, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            decoratedChildrenGroup = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.DecoratedBox(decoration: DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.Decoration>((object?)this.decoration ?? (object?)new global::Doroti.Framework.Painting.ShapeDecoration(color: CupertinoDynamicColor.resolve((this.decoration?.color ?? CupertinoColors.secondarySystemGroupedBackground), context), shape: new global::Doroti.Framework.Painting.RoundedSuperellipseBorder(borderRadius: childrenGroupBorderRadius))), child: new global::Doroti.Framework.Widgets.Column(children: childrenWithDividers)));
            decoratedChildrenGroup = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: this.margin, child: ((object.Equals(this.clipBehavior, Clip.none)) ? decoratedChildrenGroup : new global::Doroti.Framework.Widgets.ClipRSuperellipse(borderRadius: childrenGroupBorderRadius, clipBehavior: this.clipBehavior, child: decoratedChildrenGroup))));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.DecoratedBox(decoration: new global::Doroti.Framework.Painting.BoxDecoration(color: CupertinoDynamicColor.resolve(this.backgroundColor, context)), child: new global::Doroti.Framework.Widgets.Column(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection19880 = new List<global::Doroti.Framework.Widgets.Widget>(); if ((object.Equals(this.type, CupertinoListSectionType.@base))) { __collection19880.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(height: DartRuntimePrimitives.RequireValue(this.topMargin)))); } if ((headerWidget is not null)) { __collection19880.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.AlignmentDirectional.centerStart, child: new global::Doroti.Framework.Widgets.Padding(padding: ((object.Equals(this.type, CupertinoListSectionType.@base)) ? List_sectionLibrary._kDefaultHeaderMargin : List_sectionLibrary._kInsetGroupedDefaultHeaderMargin), child: headerWidget)))); } var __collectionElement20359 = decoratedChildrenGroup; if (__collectionElement20359 is { } __nonNullCollectionElement20359) { __collection19880.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement20359)); } if ((footerWidget is not null)) { __collection19880.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.AlignmentDirectional.centerStart, child: new global::Doroti.Framework.Widgets.Padding(padding: ((object.Equals(this.type, CupertinoListSectionType.@base)) ? List_sectionLibrary._kDefaultFooterMargin : List_sectionLibrary._kInsetGroupedDefaultFooterMargin), child: footerWidget)))); } return __collection19880; }))())));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
