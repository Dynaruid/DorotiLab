// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/form_row.dart
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

public static partial class Form_rowLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry _kDefaultPadding = ((global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry)(object?)new global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional(20.0, 6.0, 6.0, 6.0));
}

public class CupertinoFormRow : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? prefix { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? helper { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? error { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;

    public CupertinoFormRow(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget child = default!, global::Doroti.Generated.Framework.Widgets.Widget? prefix = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Generated.Framework.Widgets.Widget? helper = null, global::Doroti.Generated.Framework.Widgets.Widget? error = null) : base(key: key)
    {
        this.child = child;
        this.prefix = prefix;
        this.padding = padding;
        this.helper = helper;
        this.error = error;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        CupertinoThemeData theme__4850 = CupertinoTheme.of(context);
        global::Doroti.Generated.Framework.Painting.TextStyle textStyle__4906 = ((global::Doroti.Generated.Framework.Painting.TextStyle)(object?)theme__4850.textTheme.textStyle.copyWith(color: CupertinoDynamicColor.maybeResolve(theme__4850.textTheme.textStyle.color, context)));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Padding(padding: (this.padding ?? Form_rowLibrary._kDefaultPadding), child: new global::Doroti.Generated.Framework.Widgets.Column(children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection5156 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection5156.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Row(mainAxisAlignment: global::Doroti.Generated.Framework.Rendering.MainAxisAlignment.spaceBetween, children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection5266 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); if ((this.prefix is not null)) { __collection5266.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: textStyle__4906, child: this.prefix!))); } __collection5266.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Flexible(child: new global::Doroti.Generated.Framework.Widgets.Align(alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerEnd, child: this.child)))); return __collection5266; }))()))); if ((this.helper is not null)) { __collection5156.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Align(alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerStart, child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: textStyle__4906, child: this.helper!)))); } if ((this.error is not null)) { __collection5156.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Align(alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerStart, child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: new global::Doroti.Generated.Framework.Painting.TextStyle(color: CupertinoColors.destructiveRed, fontWeight: FontWeight.w500), child: this.error!)))); } return __collection5156; }))())));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
