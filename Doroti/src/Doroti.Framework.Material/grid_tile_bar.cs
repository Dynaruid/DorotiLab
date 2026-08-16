// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/grid_tile_bar.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

public class GridTileBar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? leading { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? title { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? subtitle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? trailing { get; private set; }

    public GridTileBar(global::Doroti.Framework.Foundation.Key? key = null, Color? backgroundColor = null, global::Doroti.Framework.Widgets.Widget? leading = null, global::Doroti.Framework.Widgets.Widget? title = null, global::Doroti.Framework.Widgets.Widget? subtitle = null, global::Doroti.Framework.Widgets.Widget? trailing = null) : base(key: key)
    {
        this.backgroundColor = backgroundColor;
        this.leading = leading;
        this.title = title;
        this.subtitle = subtitle;
        this.trailing = trailing;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Painting.BoxDecoration? decoration__1671 = default!;
        if ((this.backgroundColor is not null))
        {
            decoration__1671 = new global::Doroti.Framework.Painting.BoxDecoration(color: this.backgroundColor);
        }
        var padding__1793 = global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: ((this.leading is not null) ? 8.0 : 16.0), end: ((this.trailing is not null) ? 8.0 : 16.0));
        var darkTheme__1934 = ThemeData.Create();
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Container(padding: padding__1793, decoration: decoration__1671, height: ((((this.title is not null) && (this.subtitle is not null))) ? 68.0 : 48.0), child: new Theme(data: darkTheme__1934, child: IconTheme.merge(data: new global::Doroti.Framework.Widgets.IconThemeData(color: Colors.white), child: new global::Doroti.Framework.Widgets.Row(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection2284 = new List<global::Doroti.Framework.Widgets.Widget>(); if ((this.leading is not null)) { __collection2284.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(end: 8.0), child: this.leading))); } if (((this.title is not null) && (this.subtitle is not null))) { __collection2284.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.Column(mainAxisAlignment: global::Doroti.Framework.Rendering.MainAxisAlignment.center, crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.start, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.DefaultTextStyle(style: darkTheme__1934.textTheme.titleMedium!, softWrap: false, overflow: global::Doroti.Framework.Painting.TextOverflow.ellipsis, child: this.title!)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.DefaultTextStyle(style: darkTheme__1934.textTheme.bodySmall!, softWrap: false, overflow: global::Doroti.Framework.Painting.TextOverflow.ellipsis, child: this.subtitle!)) })))); } else { if (((this.title is not null) || (this.subtitle is not null))) { __collection2284.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: darkTheme__1934.textTheme.titleMedium!, softWrap: false, overflow: global::Doroti.Framework.Painting.TextOverflow.ellipsis, child: (this.title ?? this.subtitle!))))); } } if ((this.trailing is not null)) { __collection2284.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 8.0), child: this.trailing))); } return __collection2284; }))())))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
