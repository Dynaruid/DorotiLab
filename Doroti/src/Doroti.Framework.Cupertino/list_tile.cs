// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/list_tile.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Cupertino;

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
    internal static double _kMinHeight = (List_tileLibrary._kLeadingSize + (2L * 8.0));
}

public static partial class List_tileLibrary
{
    internal static double _kMinHeightWithSubtitle = (List_tileLibrary._kLeadingSize + (2L * 10.0));
}

public static partial class List_tileLibrary
{
    internal static double _kNotchedMinHeight = (List_tileLibrary._kNotchedLeadingSize + (2L * 12.0));
}

public static partial class List_tileLibrary
{
    internal static double _kNotchedMinHeightWithoutLeading = (List_tileLibrary._kNotchedLeadingSize + (2L * 10.0));
}

public static partial class List_tileLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional _kPadding = global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 20.0, end: 14.0);
}

public static partial class List_tileLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional _kPaddingWithSubtitle = global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 20.0, end: 14.0);
}

public static partial class List_tileLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.EdgeInsets _kNotchedPadding = global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 14.0);
}

public static partial class List_tileLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional _kNotchedPaddingWithoutLeading = new global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional(28.0, 10.0, 14.0, 10.0);
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

public class CupertinoListTile : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    internal virtual _CupertinoListTileType__list_tile _type { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget title { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? subtitle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? additionalInfo { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? leading { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? trailing { get; private set; }
    public virtual global::System.Func<object>? onTap { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? backgroundColorActivated { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual double leadingSize { get; private set; } = default!;
    public virtual double leadingToTitle { get; private set; } = default!;

    public CupertinoListTile(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget title = default!, global::Doroti.Generated.Framework.Widgets.Widget? subtitle = null, global::Doroti.Generated.Framework.Widgets.Widget? additionalInfo = null, global::Doroti.Generated.Framework.Widgets.Widget? leading = null, global::Doroti.Generated.Framework.Widgets.Widget? trailing = null, global::System.Func<object>? onTap = null, Color? backgroundColor = null, Color? backgroundColorActivated = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, double? leadingSize = null, double? leadingToTitle = null) : base(key: key)
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
        this._type = _CupertinoListTileType__list_tile.@base;
    }

    public static CupertinoListTile CreateNotched(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget title = default!, global::Doroti.Generated.Framework.Widgets.Widget? subtitle = null, global::Doroti.Generated.Framework.Widgets.Widget? additionalInfo = null, global::Doroti.Generated.Framework.Widgets.Widget? leading = null, global::Doroti.Generated.Framework.Widgets.Widget? trailing = null, global::System.Func<object>? onTap = null, Color? backgroundColor = null, Color? backgroundColorActivated = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, double? leadingSize = null, double? leadingToTitle = null)
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

internal class _CupertinoListTileState__list_tile : global::Doroti.Generated.Framework.Widgets.State<CupertinoListTile>
{
    internal virtual bool _tapped { get; set; } = false;

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Generated.Framework.Painting.TextStyle textStyle__12145 = CupertinoTheme.of(context).textTheme.textStyle;
        global::Doroti.Generated.Framework.Painting.TextStyle coloredStyle__12225 = ((global::Doroti.Generated.Framework.Painting.TextStyle)(object?)textStyle__12145.copyWith(color: CupertinoColors.secondaryLabel.resolveFrom(context)));
        bool baseType__12349 = (((CupertinoListTile)this.widget)._type switch { _CupertinoListTileType__list_tile.@base => true, _CupertinoListTileType__list_tile.notched => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Generated.Framework.Widgets.Widget title__12498 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: ((baseType__12349 || (((CupertinoListTile)this.widget).subtitle is null)) ? textStyle__12145 : textStyle__12145.copyWith(fontWeight: FontWeight.w600, fontSize: ((((CupertinoListTile)this.widget).leading is null) ? List_tileLibrary._kNotchedTitleWithSubtitleFontSize : null))), maxLines: 1L, overflow: global::Doroti.Generated.Framework.Painting.TextOverflow.ellipsis, child: ((CupertinoListTile)this.widget).title));
        global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding__12899 = (((CupertinoListTile)this.widget).padding ?? (((CupertinoListTile)this.widget)._type switch { _CupertinoListTileType__list_tile.@base when ((((CupertinoListTile)this.widget).subtitle is not null)) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(List_tileLibrary._kPaddingWithSubtitle), _CupertinoListTileType__list_tile.notched when ((((CupertinoListTile)this.widget).leading is not null)) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(List_tileLibrary._kNotchedPadding), _CupertinoListTileType__list_tile.@base => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(List_tileLibrary._kPadding), _CupertinoListTileType__list_tile.notched => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(List_tileLibrary._kNotchedPaddingWithoutLeading), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        global::Doroti.Ui.Color backgroundColor__13587 = ((global::Doroti.Ui.Color)(object?)(((CupertinoListTile)this.widget).backgroundColor ?? CupertinoColors.transparent));
        if (this._tapped)
        {
            backgroundColor__13587 = (((CupertinoListTile)this.widget).backgroundColorActivated ?? CupertinoColors.systemGrey4.resolveFrom(context));
        }
        double minHeight__13822 = (((CupertinoListTile)this.widget)._type switch { _CupertinoListTileType__list_tile.@base when ((((CupertinoListTile)this.widget).subtitle is not null)) => List_tileLibrary._kMinHeightWithSubtitle, _CupertinoListTileType__list_tile.notched when ((((CupertinoListTile)this.widget).leading is not null)) => List_tileLibrary._kNotchedMinHeight, _CupertinoListTileType__list_tile.@base => List_tileLibrary._kMinHeight, _CupertinoListTileType__list_tile.notched => List_tileLibrary._kNotchedMinHeightWithoutLeading, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Generated.Framework.Widgets.Widget child__14186 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Generated.Framework.Rendering.BoxConstraints(minWidth: double.PositiveInfinity, minHeight: minHeight__13822), child: new global::Doroti.Generated.Framework.Widgets.ColoredBox(color: backgroundColor__13587, child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: padding__12899, child: new global::Doroti.Generated.Framework.Widgets.Row(children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection14447 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); if (((CupertinoListTile)this.widget).leading is global::Doroti.Generated.Framework.Widgets.Widget leading__14508) { __collection14447.AddRange(new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(global::Doroti.Generated.Framework.Widgets.SizedBox.CreateSquare(dimension: ((CupertinoListTile)this.widget).leadingSize, child: new global::Doroti.Generated.Framework.Widgets.Center(child: leading__14508))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(width: ((CupertinoListTile)this.widget).leadingToTitle)) }); } else { __collection14447.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(height: ((CupertinoListTile)this.widget).leadingSize))); } __collection14447.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Expanded(child: new global::Doroti.Generated.Framework.Widgets.Column(mainAxisAlignment: global::Doroti.Generated.Framework.Rendering.MainAxisAlignment.spaceBetween, crossAxisAlignment: global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.start, children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection15027 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection15027.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(title__12498)); if (((CupertinoListTile)this.widget).subtitle is global::Doroti.Generated.Framework.Widgets.Widget subtitle__15122) { __collection15027.AddRange(new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(height: List_tileLibrary._kNotchedTitleToSubtitle)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: coloredStyle__12225.copyWith(fontSize: (baseType__12349 ? List_tileLibrary._kSubtitleFontSize : List_tileLibrary._kNotchedSubtitleFontSize)), maxLines: 1L, overflow: global::Doroti.Generated.Framework.Painting.TextOverflow.ellipsis, child: subtitle__15122)) }); } return __collection15027; }))())))); if (((CupertinoListTile)this.widget).additionalInfo is global::Doroti.Generated.Framework.Widgets.Widget additionalInfo__15731) { __collection14447.AddRange(((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection15750 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection15750.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: coloredStyle__12225, maxLines: 1L, child: additionalInfo__15731))); if ((((CupertinoListTile)this.widget).trailing is not null)) { __collection15750.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(width: List_tileLibrary._kAdditionalInfoToTrailing))); } return __collection15750; }))()); } var __collectionElement15978 = ((CupertinoListTile)this.widget).trailing; if (__collectionElement15978 is { } __nonNullCollectionElement15978) { __collection14447.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement15978)); } return __collection14447; }))())))));
        if ((((CupertinoListTile)this.widget).onTap is null))
        {
            return child__14186;
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.GestureDetector(onTapDown: ((global::System.Action<global::Doroti.Generated.Framework.Gestures.TapDownDetails>)((_) => { setState(((global::System.Action)(() => {
_tapped = true;
}))); })), onTapCancel: ((global::System.Action)(() => { setState(((global::System.Action)(() => {
_tapped = false;
}))); })), onTap: ((global::System.Action)(async () => {
await DartAsyncRuntime.AwaitFutureOrValue<object?>(((CupertinoListTile)this.widget).onTap!());
if (this.mounted)
{
    setState(((global::System.Action)(() => {
_tapped = false;
})));
}
})), behavior: global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque, child: child__14186));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoListTileChevron : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public CupertinoListTileChevron(global::Doroti.Generated.Framework.Foundation.Key? key = null) : base(key: key)
    {
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Icon(CupertinoIcons.right_chevron, size: CupertinoTheme.of(context).textTheme.textStyle.fontSize, color: CupertinoColors.systemGrey2.resolveFrom(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
