// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/form_row.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class Form_rowLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsetsGeometry _kDefaultPadding = new global::Doroti.Framework.Painting.EdgeInsetsDirectional(20.0, 6.0, 6.0, 6.0);
}

public class CupertinoFormRow : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget? prefix { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? helper { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? error { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;

    public CupertinoFormRow(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget child = default!, global::Doroti.Framework.Widgets.Widget? prefix = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Widgets.Widget? helper = null, global::Doroti.Framework.Widgets.Widget? error = null) : base(key: key)
    {
        this.child = child;
        this.prefix = prefix;
        this.padding = padding;
        this.helper = helper;
        this.error = error;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        CupertinoThemeData theme = CupertinoTheme.of(context);
        global::Doroti.Framework.Painting.TextStyle textStyleLocal = theme.textTheme.textStyle.copyWith(color: CupertinoDynamicColor.maybeResolve(theme.textTheme.textStyle.color, context));
        return new global::Doroti.Framework.Widgets.Padding(padding: padding ?? Form_rowLibrary._kDefaultPadding, child: new global::Doroti.Framework.Widgets.Column(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection5156 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection5156.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Row(mainAxisAlignment: MainAxisAlignment.spaceBetween, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection5266 = new List<global::Doroti.Framework.Widgets.Widget>(); if (prefix is not null) { __collection5266.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.DefaultTextStyle(style: textStyleLocal, child: prefix!))); } __collection5266.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: new global::Doroti.Framework.Widgets.Align(alignment: AlignmentDirectional.centerEnd, child: child)))); return __collection5266; }))()))); if (helper is not null) { __collection5156.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Align(alignment: AlignmentDirectional.centerStart, child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: textStyleLocal, child: helper!)))); } if (error is not null) { __collection5156.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Align(alignment: AlignmentDirectional.centerStart, child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: new global::Doroti.Framework.Painting.TextStyle(color: CupertinoColors.destructiveRed, fontWeight: FontWeight.w500), child: error!)))); } return __collection5156; }))()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
