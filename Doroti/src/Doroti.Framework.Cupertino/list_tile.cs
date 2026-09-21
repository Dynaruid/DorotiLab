// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/list_tile.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class List_tileLibrary
{
    internal static double _kLeadingSize = 28.0;
}

public static partial class List_tileLibrary
{
    internal static double _kNotchedLeadingSize = 30.0;
}

public static partial class List_tileLibrary
{
    internal static double _kMinHeight = _kLeadingSize + (2L * 8.0);
}

public static partial class List_tileLibrary
{
    internal static double _kMinHeightWithSubtitle = _kLeadingSize + (2L * 10.0);
}

public static partial class List_tileLibrary
{
    internal static double _kNotchedMinHeight = _kNotchedLeadingSize + (2L * 12.0);
}

public static partial class List_tileLibrary
{
    internal static double _kNotchedMinHeightWithoutLeading = _kNotchedLeadingSize + (2L * 10.0);
}

public static partial class List_tileLibrary
{
    internal static EdgeInsetsDirectional _kPadding = EdgeInsetsDirectional.CreateOnly(
        start: 20.0,
        end: 14.0
    );
}

public static partial class List_tileLibrary
{
    internal static EdgeInsetsDirectional _kPaddingWithSubtitle = EdgeInsetsDirectional.CreateOnly(
        start: 20.0,
        end: 14.0
    );
}

public static partial class List_tileLibrary
{
    internal static EdgeInsets _kNotchedPadding = EdgeInsets.CreateSymmetric(horizontal: 14.0);
}

public static partial class List_tileLibrary
{
    internal static EdgeInsetsDirectional _kNotchedPaddingWithoutLeading =
        new EdgeInsetsDirectional(28.0, 10.0, 14.0, 10.0);
}

public static partial class List_tileLibrary
{
    internal static double _kLeadingToTitle = 16.0;
}

public static partial class List_tileLibrary
{
    internal static double _kNotchedLeadingToTitle = 12.0;
}

public static partial class List_tileLibrary
{
    internal static double _kNotchedTitleToSubtitle = 3.0;
}

public static partial class List_tileLibrary
{
    internal static double _kAdditionalInfoToTrailing = 6.0;
}

public static partial class List_tileLibrary
{
    internal static double _kNotchedTitleWithSubtitleFontSize = 16.0;
}

public static partial class List_tileLibrary
{
    internal static double _kSubtitleFontSize = 12.0;
}

public static partial class List_tileLibrary
{
    internal static double _kNotchedSubtitleFontSize = 14.0;
}

internal enum _CupertinoListTileType__list_tile
{
    @base,
    notched,
}

public class CupertinoListTile : StatefulWidget
{
    internal virtual _CupertinoListTileType__list_tile _type { get; private set; } = default!;
    public virtual Widget title { get; private set; } = default!;
    public virtual Widget? subtitle { get; private set; }
    public virtual Widget? additionalInfo { get; private set; }
    public virtual Widget? leading { get; private set; }
    public virtual Widget? trailing { get; private set; }
    public virtual Func<object>? onTap { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? backgroundColorActivated { get; private set; }
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual double leadingSize { get; private set; } = default!;
    public virtual double leadingToTitle { get; private set; } = default!;

    public CupertinoListTile(
        Key? key = null,
        Widget title = default!,
        Widget? subtitle = null,
        Widget? additionalInfo = null,
        Widget? leading = null,
        Widget? trailing = null,
        Func<object>? onTap = null,
        Color? backgroundColor = null,
        Color? backgroundColorActivated = null,
        EdgeInsetsGeometry? padding = null,
        double? leadingSize = null,
        double? leadingToTitle = null
    )
        : base(key: key)
    {
        double __leadingSize = leadingSize ?? List_tileLibrary._kLeadingSize;
        double __leadingToTitle = leadingToTitle ?? List_tileLibrary._kLeadingToTitle;
        this.title = title;
        this.subtitle = subtitle;
        this.additionalInfo = additionalInfo;
        this.leading = leading;
        this.trailing = trailing;
        this.onTap = onTap;
        this.backgroundColor = backgroundColor;
        this.backgroundColorActivated = backgroundColorActivated;
        this.padding = padding;
        this.leadingSize = __leadingSize;
        this.leadingToTitle = __leadingToTitle;
        _type = _CupertinoListTileType__list_tile.@base;
    }

    public static CupertinoListTile CreateNotched(
        Key? key = null,
        Widget title = default!,
        Widget? subtitle = null,
        Widget? additionalInfo = null,
        Widget? leading = null,
        Widget? trailing = null,
        Func<object>? onTap = null,
        Color? backgroundColor = null,
        Color? backgroundColorActivated = null,
        EdgeInsetsGeometry? padding = null,
        double? leadingSize = null,
        double? leadingToTitle = null
    )
    {
        var __instance = new CupertinoListTile(
            key: key,
            title: title,
            subtitle: subtitle,
            additionalInfo: additionalInfo,
            leading: leading,
            trailing: trailing,
            onTap: onTap,
            backgroundColor: backgroundColor,
            backgroundColorActivated: backgroundColorActivated,
            padding: padding,
            leadingSize: leadingSize,
            leadingToTitle: leadingToTitle
        );
        double __leadingSize = leadingSize ?? List_tileLibrary._kNotchedLeadingSize;
        double __leadingToTitle = leadingToTitle ?? List_tileLibrary._kNotchedLeadingToTitle;
        __instance.title = title;
        __instance.subtitle = subtitle;
        __instance.additionalInfo = additionalInfo;
        __instance.leading = leading;
        __instance.trailing = trailing;
        __instance.onTap = onTap;
        __instance.backgroundColor = backgroundColor;
        __instance.backgroundColorActivated = backgroundColorActivated;
        __instance.padding = padding;
        __instance.leadingSize = __leadingSize;
        __instance.leadingToTitle = __leadingToTitle;
        __instance._type = _CupertinoListTileType__list_tile.notched;
        return __instance;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoListTileState__list_tile());
}

internal class _CupertinoListTileState__list_tile : State<CupertinoListTile>
{
    internal virtual bool _tapped { get; set; } = false;

    public override Widget build(BuildContext context)
    {
        TextStyle textStyleLocal = CupertinoTheme.of(context).textTheme.textStyle;
        TextStyle coloredStyle = textStyleLocal.copyWith(
            color: CupertinoColors.secondaryLabel.resolveFrom(context)
        );
        bool baseType = widget._type switch
        {
            _CupertinoListTileType__list_tile.@base => true,
            _CupertinoListTileType__list_tile.notched => false,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        Widget titleLocal = new DefaultTextStyle(
            style: (baseType || (widget.subtitle is null))
                ? textStyleLocal
                : textStyleLocal.copyWith(
                    fontWeight: FontWeight.w600,
                    fontSize: (widget.leading is null)
                        ? List_tileLibrary._kNotchedTitleWithSubtitleFontSize
                        : null
                ),
            maxLines: 1L,
            overflow: TextOverflow.ellipsis,
            child: widget.title
        );
        EdgeInsetsGeometry paddingLocal =
            widget.padding
            ?? (
                widget._type switch
                {
                    _CupertinoListTileType__list_tile.@base when widget.subtitle is not null =>
                        DartRuntimePrimitives.ConvertValue<EdgeInsetsGeometry>(
                            List_tileLibrary._kPaddingWithSubtitle
                        ),
                    _CupertinoListTileType__list_tile.notched when widget.leading is not null =>
                        DartRuntimePrimitives.ConvertValue<EdgeInsetsGeometry>(
                            List_tileLibrary._kNotchedPadding
                        ),
                    _CupertinoListTileType__list_tile.@base =>
                        DartRuntimePrimitives.ConvertValue<EdgeInsetsGeometry>(
                            List_tileLibrary._kPadding
                        ),
                    _CupertinoListTileType__list_tile.notched =>
                        DartRuntimePrimitives.ConvertValue<EdgeInsetsGeometry>(
                            List_tileLibrary._kNotchedPaddingWithoutLeading
                        ),
                    _ => throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
                }
            );
        Color backgroundColorLocal = widget.backgroundColor ?? CupertinoColors.transparent;
        if (_tapped)
        {
            backgroundColorLocal =
                widget.backgroundColorActivated ?? CupertinoColors.systemGrey4.resolveFrom(context);
        }
        double minHeightLocal = widget._type switch
        {
            _CupertinoListTileType__list_tile.@base when widget.subtitle is not null =>
                List_tileLibrary._kMinHeightWithSubtitle,
            _CupertinoListTileType__list_tile.notched when widget.leading is not null =>
                List_tileLibrary._kNotchedMinHeight,
            _CupertinoListTileType__list_tile.@base => List_tileLibrary._kMinHeight,
            _CupertinoListTileType__list_tile.notched =>
                List_tileLibrary._kNotchedMinHeightWithoutLeading,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        Widget childLocal = new ConstrainedBox(
            constraints: new BoxConstraints(
                minWidth: double.PositiveInfinity,
                minHeight: minHeightLocal
            ),
            child: new ColoredBox(
                color: backgroundColorLocal,
                child: new Padding(
                    padding: paddingLocal,
                    child: new Row(
                        children: (
                            (Func<List<Widget>>)(
                                () =>
                                {
                                    var __collection14447 = new List<Widget>();
                                    if (widget.leading is Widget leadingLocal)
                                    {
                                        __collection14447.AddRange(
                                            new List<Widget>
                                            {
                                                DartRuntimePrimitives.ConvertValue<Widget>(
                                                    SizedBox.CreateSquare(
                                                        dimension: widget.leadingSize,
                                                        child: new Center(child: leadingLocal)
                                                    )
                                                ),
                                                DartRuntimePrimitives.ConvertValue<Widget>(
                                                    new SizedBox(width: widget.leadingToTitle)
                                                ),
                                            }
                                        );
                                    }
                                    else
                                    {
                                        __collection14447.Add(
                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                new SizedBox(height: widget.leadingSize)
                                            )
                                        );
                                    }
                                    __collection14447.Add(
                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                            new Expanded(
                                                child: new Column(
                                                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                                    crossAxisAlignment: CrossAxisAlignment.start,
                                                    children: (
                                                        (Func<List<Widget>>)(
                                                            () =>
                                                            {
                                                                var __collection15027 =
                                                                    new List<Widget>();
                                                                __collection15027.Add(
                                                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                                                        titleLocal
                                                                    )
                                                                );
                                                                if (
                                                                    widget.subtitle
                                                                    is Widget subtitleLocal
                                                                )
                                                                {
                                                                    __collection15027.AddRange(
                                                                        new List<Widget>
                                                                        {
                                                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                                                new SizedBox(
                                                                                    height: List_tileLibrary._kNotchedTitleToSubtitle
                                                                                )
                                                                            ),
                                                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                                                new DefaultTextStyle(
                                                                                    style: coloredStyle.copyWith(
                                                                                        fontSize: baseType
                                                                                            ? List_tileLibrary._kSubtitleFontSize
                                                                                            : List_tileLibrary._kNotchedSubtitleFontSize
                                                                                    ),
                                                                                    maxLines: 1L,
                                                                                    overflow: TextOverflow.ellipsis,
                                                                                    child: subtitleLocal
                                                                                )
                                                                            ),
                                                                        }
                                                                    );
                                                                }
                                                                return __collection15027;
                                                            }
                                                        )
                                                    )()
                                                )
                                            )
                                        )
                                    );
                                    if (widget.additionalInfo is Widget additionalInfoLocal)
                                    {
                                        __collection14447.AddRange(
                                            (
                                                (Func<List<Widget>>)(
                                                    () =>
                                                    {
                                                        var __collection15750 = new List<Widget>();
                                                        __collection15750.Add(
                                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                                new DefaultTextStyle(
                                                                    style: coloredStyle,
                                                                    maxLines: 1L,
                                                                    child: additionalInfoLocal
                                                                )
                                                            )
                                                        );
                                                        if (widget.trailing is not null)
                                                        {
                                                            __collection15750.Add(
                                                                DartRuntimePrimitives.ConvertValue<Widget>(
                                                                    new SizedBox(
                                                                        width: List_tileLibrary._kAdditionalInfoToTrailing
                                                                    )
                                                                )
                                                            );
                                                        }
                                                        return __collection15750;
                                                    }
                                                )
                                            )()
                                        );
                                    }
                                    var __collectionElement15978 = widget.trailing;
                                    if (
                                        __collectionElement15978 is
                                        { } __nonNullCollectionElement15978
                                    )
                                    {
                                        __collection14447.Add(
                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                __nonNullCollectionElement15978
                                            )
                                        );
                                    }
                                    return __collection14447;
                                }
                            )
                        )()
                    )
                )
            )
        );
        if (widget.onTap is null)
        {
            return childLocal;
        }
        return new GestureDetector(
            onTapDown: (_) =>
            {
                setState(() =>
                {
                    _tapped = true;
                });
            },
            onTapCancel: () =>
            {
                setState(() =>
                {
                    _tapped = false;
                });
            },
            onTap: async () =>
            {
                await DartAsyncRuntime.AwaitFutureOrValue<object?>(widget.onTap!());
                if (mounted)
                {
                    setState(() =>
                    {
                        _tapped = false;
                    });
                }
            },
            behavior: HitTestBehavior.opaque,
            child: childLocal
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class CupertinoListTileChevron : StatelessWidget
{
    public CupertinoListTileChevron(Key? key = null)
        : base(key: key) { }

    public override Widget build(BuildContext context)
    {
        return new Icon(
            CupertinoIcons.right_chevron,
            size: CupertinoTheme.of(context).textTheme.textStyle.fontSize,
            color: CupertinoColors.systemGrey2.resolveFrom(context)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
