// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/list_section.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class List_sectionLibrary
{
    internal static double _kMarginTop = 22.0;
}

public static partial class List_sectionLibrary
{
    internal static EdgeInsetsDirectional _kDefaultHeaderMargin = new EdgeInsetsDirectional(
        20.0,
        0.0,
        20.0,
        6.0
    );
}

public static partial class List_sectionLibrary
{
    internal static EdgeInsetsDirectional _kInsetGroupedDefaultHeaderMargin =
        new EdgeInsetsDirectional(20.0, 16.0, 20.0, 6.0);
}

public static partial class List_sectionLibrary
{
    internal static EdgeInsetsDirectional _kDefaultFooterMargin = new EdgeInsetsDirectional(
        20.0,
        0.0,
        20.0,
        0.0
    );
}

public static partial class List_sectionLibrary
{
    internal static EdgeInsetsDirectional _kInsetGroupedDefaultFooterMargin =
        new EdgeInsetsDirectional(20.0, 0.0, 20.0, 10.0);
}

public static partial class List_sectionLibrary
{
    internal static EdgeInsets _kDefaultRowsMargin = EdgeInsets.CreateOnly(bottom: 8.0);
}

public static partial class List_sectionLibrary
{
    internal static EdgeInsetsDirectional _kDefaultInsetGroupedRowsMargin =
        new EdgeInsetsDirectional(20.0, 20.0, 20.0, 10.0);
}

public static partial class List_sectionLibrary
{
    internal static EdgeInsetsDirectional _kDefaultInsetGroupedRowsMarginWithHeader =
        new EdgeInsetsDirectional(20.0, 0.0, 20.0, 10.0);
}

public static partial class List_sectionLibrary
{
    internal static BorderRadius _kDefaultInsetGroupedBorderRadius = BorderRadius.CreateAll(
        Radius.circular(10.0)
    );
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
    internal static Color _kHeaderFooterColor = new CupertinoDynamicColor(
        color: Color.fromRGBO(108L, 108L, 108L, 1.0),
        darkColor: Color.fromRGBO(142L, 142L, 146L, 1.0),
        highContrastColor: Color.fromRGBO(74L, 74L, 77L, 1.0),
        darkHighContrastColor: Color.fromRGBO(176L, 176L, 183L, 1.0),
        elevatedColor: Color.fromRGBO(108L, 108L, 108L, 1.0),
        darkElevatedColor: Color.fromRGBO(142L, 142L, 146L, 1.0),
        highContrastElevatedColor: Color.fromRGBO(108L, 108L, 108L, 1.0),
        darkHighContrastElevatedColor: Color.fromRGBO(142L, 142L, 146L, 1.0)
    );
}

public enum CupertinoListSectionType
{
    @base,
    insetGrouped,
}

public class CupertinoListSection : StatelessWidget
{
    public virtual CupertinoListSectionType type { get; private set; } = default!;
    public virtual Widget? header { get; private set; }
    public virtual Widget? footer { get; private set; }
    public virtual EdgeInsetsGeometry margin { get; private set; } = default!;
    public virtual List<Widget>? children { get; private set; }
    public virtual BoxDecoration? decoration { get; private set; }
    public virtual Color backgroundColor { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual double dividerMargin { get; private set; } = default!;
    public virtual double additionalDividerMargin { get; private set; } = default!;
    public virtual double? topMargin { get; private set; }
    public virtual Color? separatorColor { get; private set; }

    public CupertinoListSection(
        Key? key = null,
        List<Widget>? children = null,
        Widget? header = null,
        Widget? footer = null,
        EdgeInsetsGeometry? margin = null,
        Color backgroundColor = default!,
        BoxDecoration? decoration = null,
        Clip clipBehavior = Clip.none,
        double? dividerMargin = null,
        double? additionalDividerMargin = null,
        double? topMargin = null,
        bool hasLeading = true,
        Color? separatorColor = null
    )
        : base(key: key)
    {
        EdgeInsetsGeometry __margin = margin ?? List_sectionLibrary._kDefaultRowsMargin;
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
        type = CupertinoListSectionType.@base;
        this.additionalDividerMargin =
            additionalDividerMargin
            ?? (hasLeading ? List_sectionLibrary._kBaseAdditionalDividerMargin : 0.0);
        System.Diagnostics.Debug.Assert(
            ((children is not null) && (checked(children.Count) > 0L)) || (header is not null)
        );
    }

    public static CupertinoListSection CreateInsetGrouped(
        Key? key = null,
        List<Widget>? children = null,
        Widget? header = null,
        Widget? footer = null,
        EdgeInsetsGeometry? margin = null,
        Color backgroundColor = default!,
        BoxDecoration? decoration = null,
        Clip clipBehavior = Clip.hardEdge,
        double? dividerMargin = null,
        double? additionalDividerMargin = null,
        double? topMargin = null,
        bool hasLeading = true,
        Color? separatorColor = null
    )
    {
        var __instance = new CupertinoListSection(
            key: key,
            children: children,
            header: header,
            footer: footer,
            margin: margin,
            backgroundColor: backgroundColor,
            decoration: decoration,
            clipBehavior: clipBehavior,
            dividerMargin: dividerMargin,
            additionalDividerMargin: additionalDividerMargin,
            topMargin: topMargin,
            hasLeading: hasLeading,
            separatorColor: separatorColor
        );
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
        __instance.additionalDividerMargin =
            additionalDividerMargin
            ?? (
                hasLeading
                    ? List_sectionLibrary._kInsetAdditionalDividerMargin
                    : List_sectionLibrary._kInsetAdditionalDividerMarginWithoutLeading
            );
        __instance.margin =
            margin
            ?? (
                (header is null)
                    ? List_sectionLibrary._kDefaultInsetGroupedRowsMargin
                    : List_sectionLibrary._kDefaultInsetGroupedRowsMarginWithHeader
            );
        return __instance;
    }

    public override Widget build(BuildContext context)
    {
        Color dividerColor = separatorColor ?? CupertinoColors.separator.resolveFrom(context);
        double dividerHeight = 1.0 / MediaQuery.devicePixelRatioOf(context);
        Widget longDivider = new Container(color: dividerColor, height: dividerHeight);
        Widget shortDivider = new Container(
            margin: EdgeInsetsDirectional.CreateOnly(
                start: dividerMargin + additionalDividerMargin
            ),
            color: dividerColor,
            height: dividerHeight
        );
        TextStyle styleLocal = CupertinoTheme.of(context).textTheme.textStyle;
        Widget? headerWidget = default!;
        Widget? footerWidget = default!;
        switch (type)
        {
            case CupertinoListSectionType.@base:
            {
                styleLocal = styleLocal.merge(
                    new TextStyle(
                        fontSize: 13.0,
                        color: CupertinoDynamicColor.resolve(
                            List_sectionLibrary._kHeaderFooterColor,
                            context
                        )
                    )
                );
                if (header is not null)
                {
                    headerWidget = DartRuntimePrimitives.ConvertValue<Widget>(
                        new DefaultTextStyle(style: styleLocal, child: header!)
                    );
                }
                if (footer is not null)
                {
                    footerWidget = DartRuntimePrimitives.ConvertValue<Widget>(
                        new DefaultTextStyle(style: styleLocal, child: footer!)
                    );
                }
                break;
            }
            case CupertinoListSectionType.insetGrouped:
            {
                if (header is not null)
                {
                    headerWidget = DartRuntimePrimitives.ConvertValue<Widget>(
                        new DefaultTextStyle(
                            style: styleLocal.merge(
                                new TextStyle(fontSize: 20.0, fontWeight: FontWeight.bold)
                            ),
                            child: header!
                        )
                    );
                }
                if (footer is not null)
                {
                    footerWidget = DartRuntimePrimitives.ConvertValue<Widget>(
                        new DefaultTextStyle(style: styleLocal, child: footer!)
                    );
                }
                break;
            }
        }
        Widget? decoratedChildrenGroup = default!;
        if ((children is not null) && Enumerable.Any(children!))
        {
            var childrenWithDividers = new List<Widget>();
            if (Equals(type, CupertinoListSectionType.@base))
            {
                childrenWithDividers.Add(longDivider);
            }
            children!
                .GetRange(0L, checked(children!.Count) - 1L)
                .forEach(
                    (widget) =>
                    {
                        childrenWithDividers.Add(widget);
                        childrenWithDividers.Add(shortDivider);
                    }
                );
            childrenWithDividers.Add(children!.Last());
            if (Equals(type, CupertinoListSectionType.@base))
            {
                childrenWithDividers.Add(longDivider);
            }
            BorderRadius childrenGroupBorderRadius = type switch
            {
                CupertinoListSectionType.insetGrouped =>
                    List_sectionLibrary._kDefaultInsetGroupedBorderRadius,
                CupertinoListSectionType.@base => BorderRadius.zero,
                _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
            };
            decoratedChildrenGroup = DartRuntimePrimitives.ConvertValue<Widget>(
                new DecoratedBox(
                    decoration: DartRuntimePrimitives.ConvertValue<Decoration>(
                        (object?)decoration
                            ?? (object?)
                                new ShapeDecoration(
                                    color: CupertinoDynamicColor.resolve(
                                        decoration?.color
                                            ?? CupertinoColors.secondarySystemGroupedBackground,
                                        context
                                    ),
                                    shape: new RoundedSuperellipseBorder(
                                        borderRadius: childrenGroupBorderRadius
                                    )
                                )
                    ),
                    child: new Column(children: childrenWithDividers)
                )
            );
            decoratedChildrenGroup = DartRuntimePrimitives.ConvertValue<Widget>(
                new Padding(
                    padding: margin,
                    child: Equals(clipBehavior, Clip.none)
                        ? decoratedChildrenGroup
                        : new ClipRSuperellipse(
                            borderRadius: childrenGroupBorderRadius,
                            clipBehavior: clipBehavior,
                            child: decoratedChildrenGroup
                        )
                )
            );
        }
        return new DecoratedBox(
            decoration: new BoxDecoration(
                color: CupertinoDynamicColor.resolve(backgroundColor, context)
            ),
            child: new Column(
                children: (
                    (Func<List<Widget>>)(
                        () =>
                        {
                            var __collection19880 = new List<Widget>();
                            if (Equals(type, CupertinoListSectionType.@base))
                            {
                                __collection19880.Add(
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        new SizedBox(
                                            height: (
                                                topMargin
                                                ?? throw new global::System.NullReferenceException(
                                                    "Dart null assertion failed."
                                                )
                                            )
                                        )
                                    )
                                );
                            }
                            if (headerWidget is not null)
                            {
                                __collection19880.Add(
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        new Align(
                                            alignment: AlignmentDirectional.centerStart,
                                            child: new Padding(
                                                padding: Equals(
                                                    type,
                                                    CupertinoListSectionType.@base
                                                )
                                                    ? List_sectionLibrary._kDefaultHeaderMargin
                                                    : List_sectionLibrary._kInsetGroupedDefaultHeaderMargin,
                                                child: headerWidget
                                            )
                                        )
                                    )
                                );
                            }
                            var __collectionElement20359 = decoratedChildrenGroup;
                            if (__collectionElement20359 is { } __nonNullCollectionElement20359)
                            {
                                __collection19880.Add(
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        __nonNullCollectionElement20359
                                    )
                                );
                            }
                            if (footerWidget is not null)
                            {
                                __collection19880.Add(
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        new Align(
                                            alignment: AlignmentDirectional.centerStart,
                                            child: new Padding(
                                                padding: Equals(
                                                    type,
                                                    CupertinoListSectionType.@base
                                                )
                                                    ? List_sectionLibrary._kDefaultFooterMargin
                                                    : List_sectionLibrary._kInsetGroupedDefaultFooterMargin,
                                                child: footerWidget
                                            )
                                        )
                                    )
                                );
                            }
                            return __collection19880;
                        }
                    )
                )()
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
