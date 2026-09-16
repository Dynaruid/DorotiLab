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
    internal static global::Doroti.Framework.Painting.EdgeInsetsDirectional _kPadding = EdgeInsetsDirectional.CreateOnly(start: 20.0, end: 14.0);
}

public static partial class List_tileLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsetsDirectional _kPaddingWithSubtitle = EdgeInsetsDirectional.CreateOnly(start: 20.0, end: 14.0);
}

public static partial class List_tileLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsets _kNotchedPadding = EdgeInsets.CreateSymmetric(horizontal: 14.0);
}

public static partial class List_tileLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsetsDirectional _kNotchedPaddingWithoutLeading = new global::Doroti.Framework.Painting.EdgeInsetsDirectional(28.0, 10.0, 14.0, 10.0);
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
    notched
}

public class CupertinoListTile : global::Doroti.Framework.Widgets.StatefulWidget
{
    internal virtual _CupertinoListTileType__list_tile _type { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget title { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? subtitle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? additionalInfo { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? leading { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? trailing { get; private set; }
    public virtual global::System.Func<object>? onTap { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? backgroundColorActivated { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual double leadingSize { get; private set; } = default!;
    public virtual double leadingToTitle { get; private set; } = default!;

    public CupertinoListTile(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget title = default!, global::Doroti.Framework.Widgets.Widget? subtitle = null, global::Doroti.Framework.Widgets.Widget? additionalInfo = null, global::Doroti.Framework.Widgets.Widget? leading = null, global::Doroti.Framework.Widgets.Widget? trailing = null, global::System.Func<object>? onTap = null, Color? backgroundColor = null, Color? backgroundColorActivated = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, double? leadingSize = null, double? leadingToTitle = null) : base(key: key)
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

    public static CupertinoListTile CreateNotched(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget title = default!, global::Doroti.Framework.Widgets.Widget? subtitle = null, global::Doroti.Framework.Widgets.Widget? additionalInfo = null, global::Doroti.Framework.Widgets.Widget? leading = null, global::Doroti.Framework.Widgets.Widget? trailing = null, global::System.Func<object>? onTap = null, Color? backgroundColor = null, Color? backgroundColorActivated = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, double? leadingSize = null, double? leadingToTitle = null)
    {
        var __instance = new CupertinoListTile(key: key, title: title, subtitle: subtitle, additionalInfo: additionalInfo, leading: leading, trailing: trailing, onTap: onTap, backgroundColor: backgroundColor, backgroundColorActivated: backgroundColorActivated, padding: padding, leadingSize: leadingSize, leadingToTitle: leadingToTitle);
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

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoListTileState__list_tile());
}

internal class _CupertinoListTileState__list_tile : global::Doroti.Framework.Widgets.State<CupertinoListTile>
{
    internal virtual bool _tapped { get; set; } = false;

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Painting.TextStyle textStyleLocal = CupertinoTheme.of(context).textTheme.textStyle;
        global::Doroti.Framework.Painting.TextStyle coloredStyle = textStyleLocal.copyWith(color: CupertinoColors.secondaryLabel.resolveFrom(context));
        bool baseType = widget._type switch { _CupertinoListTileType__list_tile.@base => true, _CupertinoListTileType__list_tile.notched => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        global::Doroti.Framework.Widgets.Widget titleLocal = new global::Doroti.Framework.Widgets.DefaultTextStyle(style: (baseType || (widget.subtitle is null)) ? textStyleLocal : textStyleLocal.copyWith(fontWeight: FontWeight.w600, fontSize: (widget.leading is null) ? List_tileLibrary._kNotchedTitleWithSubtitleFontSize : null), maxLines: 1L, overflow: TextOverflow.ellipsis, child: widget.title);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry paddingLocal = widget.padding ?? (widget._type switch { _CupertinoListTileType__list_tile.@base when widget.subtitle is not null => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(List_tileLibrary._kPaddingWithSubtitle), _CupertinoListTileType__list_tile.notched when widget.leading is not null => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(List_tileLibrary._kNotchedPadding), _CupertinoListTileType__list_tile.@base => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(List_tileLibrary._kPadding), _CupertinoListTileType__list_tile.notched => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(List_tileLibrary._kNotchedPaddingWithoutLeading), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Ui.Color backgroundColorLocal = widget.backgroundColor ?? CupertinoColors.transparent;
        if (_tapped)
        {
            backgroundColorLocal = widget.backgroundColorActivated ?? CupertinoColors.systemGrey4.resolveFrom(context);
        }
        double minHeightLocal = widget._type switch { _CupertinoListTileType__list_tile.@base when widget.subtitle is not null => List_tileLibrary._kMinHeightWithSubtitle, _CupertinoListTileType__list_tile.notched when widget.leading is not null => List_tileLibrary._kNotchedMinHeight, _CupertinoListTileType__list_tile.@base => List_tileLibrary._kMinHeight, _CupertinoListTileType__list_tile.notched => List_tileLibrary._kNotchedMinHeightWithoutLeading, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        global::Doroti.Framework.Widgets.Widget childLocal = new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: double.PositiveInfinity, minHeight: minHeightLocal), child: new global::Doroti.Framework.Widgets.ColoredBox(color: backgroundColorLocal, child: new global::Doroti.Framework.Widgets.Padding(padding: paddingLocal, child: new global::Doroti.Framework.Widgets.Row(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection14447 = new List<global::Doroti.Framework.Widgets.Widget>(); if (widget.leading is global::Doroti.Framework.Widgets.Widget leadingLocal) { __collection14447.AddRange(new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(SizedBox.CreateSquare(dimension: widget.leadingSize, child: new global::Doroti.Framework.Widgets.Center(child: leadingLocal))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(width: widget.leadingToTitle)) }); } else { __collection14447.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(height: widget.leadingSize))); } __collection14447.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.Column(mainAxisAlignment: MainAxisAlignment.spaceBetween, crossAxisAlignment: CrossAxisAlignment.start, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection15027 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection15027.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(titleLocal)); if (widget.subtitle is global::Doroti.Framework.Widgets.Widget subtitleLocal) { __collection15027.AddRange(new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(height: List_tileLibrary._kNotchedTitleToSubtitle)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.DefaultTextStyle(style: coloredStyle.copyWith(fontSize: baseType ? List_tileLibrary._kSubtitleFontSize : List_tileLibrary._kNotchedSubtitleFontSize), maxLines: 1L, overflow: TextOverflow.ellipsis, child: subtitleLocal)) }); } return __collection15027; }))())))); if (widget.additionalInfo is global::Doroti.Framework.Widgets.Widget additionalInfoLocal) { __collection14447.AddRange(((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection15750 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection15750.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.DefaultTextStyle(style: coloredStyle, maxLines: 1L, child: additionalInfoLocal))); if (widget.trailing is not null) { __collection15750.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(width: List_tileLibrary._kAdditionalInfoToTrailing))); } return __collection15750; }))()); } var __collectionElement15978 = widget.trailing; if (__collectionElement15978 is { } __nonNullCollectionElement15978) { __collection14447.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement15978)); } return __collection14447; }))()))));
        if (widget.onTap is null)
        {
            return childLocal;
        }
        return new global::Doroti.Framework.Widgets.GestureDetector(onTapDown: (_) =>
        {
            setState(() =>
            {
                _tapped = true;
            });
        }, onTapCancel: () =>
        {
            setState(() =>
            {
                _tapped = false;
            });
        }, onTap: async () =>
        {
            await DartAsyncRuntime.AwaitFutureOrValue<object?>(widget.onTap!());
            if (mounted)
            {
                setState(() =>
                {
                    _tapped = false;
                });
            }
        }, behavior: HitTestBehavior.opaque, child: childLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoListTileChevron : global::Doroti.Framework.Widgets.StatelessWidget
{
    public CupertinoListTileChevron(global::Doroti.Framework.Foundation.Key? key = null) : base(key: key)
    {
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.Icon(CupertinoIcons.right_chevron, size: CupertinoTheme.of(context).textTheme.textStyle.fontSize, color: CupertinoColors.systemGrey2.resolveFrom(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
